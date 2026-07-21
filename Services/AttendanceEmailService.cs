using Financial.Data;
using Financial.Model.Dtr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Financial.Services
{
    public sealed class EtrsMailerOptions
    {
        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public string SenderName { get; set; } = "BSWM ETRS";
        public List<EtrsMailerAccount> Accounts { get; set; } = new();
    }

    public sealed class EtrsMailerAccount
    {
        public string Email { get; set; } = string.Empty;
        public string AppPassword { get; set; } = string.Empty;
    }

    public interface IAttendanceEmailService
    {
        Task SendAttendanceLogsAsync(AttendanceEmailRequest request, CancellationToken cancellationToken = default);
        Task SendRejectionEmailAsync(AttendanceEmailRequest request, string? remarks, CancellationToken cancellationToken = default);
        Task SendVerificationCodeAsync(string recipient, string code, CancellationToken cancellationToken = default);
    }

    public sealed class AttendanceEmailService : IAttendanceEmailService
    {
        private readonly IDbContextFactory<EtrsDbContext> _dbFactory;
        private readonly EtrsMailerOptions _options;
        private readonly ILogger<AttendanceEmailService> _logger;
        private static int _nextAccount = -1;

        public AttendanceEmailService(
            IDbContextFactory<EtrsDbContext> dbFactory,
            IOptions<EtrsMailerOptions> options,
            ILogger<AttendanceEmailService> logger)
        {
            _dbFactory = dbFactory;
            _options = options.Value;
            _logger = logger;
        }

        public async Task SendAttendanceLogsAsync(AttendanceEmailRequest request, CancellationToken cancellationToken = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);
            var endExclusive = request.PeriodEnd.Date.AddDays(1);
            var wfh = await db.AttendanceLogsWfh.AsNoTracking()
                .Where(log => log.EmployeeId == request.EmployeeId
                    && log.AttendanceDate >= request.PeriodStart.Date
                    && log.AttendanceDate < endExclusive)
                .OrderBy(log => log.AttendanceDate)
                .ToListAsync(cancellationToken);

            var rows = wfh.Select(log => new AttendanceEmailRow(
                    log.AttendanceDate, log.TimeIn, log.TimeOut))
                .OrderBy(row => row.Date)
                .ToList();

            var subject = $"ETRS WFH attendance logs - {request.PeriodStart:MMM d} to {request.PeriodEnd:MMM d, yyyy}";
            var body = BuildHtmlBody(request, rows);
            var csv = BuildCsv(request, rows);

            using var message = CreateMessage(request.RecipientEmail, subject, body);
            message.Attachments.Add(new Attachment(
                new MemoryStream(Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv)).ToArray()),
                $"ETRS-{SafeFilePart(request.IdNumber)}-{request.PeriodStart:yyyyMMdd}-{request.PeriodEnd:yyyyMMdd}.csv",
                "text/csv"));
            await SendAsync(message, cancellationToken);
        }

        public async Task SendVerificationCodeAsync(string recipient, string code, CancellationToken cancellationToken = default)
        {
            using var message = CreateMessage(
                recipient,
                "Your ETRS verification code",
                $"<p>Your ETRS verification code is <strong>{WebUtility.HtmlEncode(code)}</strong>.</p><p>This code expires in 10 minutes.</p>");
            await SendAsync(message, cancellationToken);
        }

        public async Task SendRejectionEmailAsync(
            AttendanceEmailRequest request,
            string? remarks,
            CancellationToken cancellationToken = default)
        {
            var rejectionMessage = string.IsNullOrWhiteSpace(remarks)
                ? "Your request was not approved. Please verify the employee details, email address, and requested period before submitting another request."
                : remarks.Trim();
            var subject = $"ETRS WFH attendance request rejected - {request.PeriodStart:MMM d} to {request.PeriodEnd:MMM d, yyyy}";
            var body = $"""
                <div style="font-family:Arial,sans-serif;color:#1e2939;line-height:1.55">
                  <h2 style="margin-bottom:4px">WFH Attendance Request</h2>
                  <p>Your request for the WFH attendance logs of <strong>{WebUtility.HtmlEncode(request.EmployeeName)}</strong> ({WebUtility.HtmlEncode(request.IdNumber)}) was not approved.</p>
                  <p>Period: <strong>{request.PeriodStart:MMMM d, yyyy}</strong> to <strong>{request.PeriodEnd:MMMM d, yyyy}</strong></p>
                  <div style="margin:18px 0;padding:14px 16px;border-left:4px solid #ebca4a;background:#f8fafc">
                    <strong>Remarks</strong><br />
                    {WebUtility.HtmlEncode(rejectionMessage).Replace("\r\n", "<br />").Replace("\n", "<br />")}
                  </div>
                  <p style="color:#64748b"><strong>This is a system-generated message. Please do not reply to this email.</strong></p>
                </div>
                """;

            using var message = CreateMessage(request.RecipientEmail, subject, body);
            await SendAsync(message, cancellationToken);
        }

        private MailMessage CreateMessage(string recipient, string subject, string htmlBody)
        {
            var account = GetConfiguredAccounts().First();
            return new MailMessage
            {
                From = new MailAddress(account.Email, _options.SenderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true,
                To = { new MailAddress(recipient) }
            };
        }

        private async Task SendAsync(MailMessage message, CancellationToken cancellationToken)
        {
            var accounts = GetConfiguredAccounts();
            var startIndex = (int)((uint)Interlocked.Increment(ref _nextAccount) % (uint)accounts.Count);
            Exception? lastException = null;

            for (var attempt = 0; attempt < accounts.Count; attempt++)
            {
                var account = accounts[(startIndex + attempt) % accounts.Count];
                message.From = new MailAddress(account.Email, _options.SenderName);

                using var client = new SmtpClient(_options.Host, _options.Port)
                {
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(account.Email, account.AppPassword.Replace(" ", string.Empty)),
                    Timeout = 30000
                };

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await client.SendMailAsync(message, cancellationToken);
                    _logger.LogInformation("ETRS email sent through {SenderEmail}.", account.Email);
                    return;
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    lastException = ex;
                    _logger.LogWarning(
                        ex,
                        "ETRS email delivery failed through {SenderEmail} ({Attempt}/{AccountCount}).",
                        account.Email,
                        attempt + 1,
                        accounts.Count);

                    if (!CanTryAnotherAccount(ex))
                    {
                        break;
                    }
                }
            }

            throw new InvalidOperationException(
                BuildDeliveryFailureMessage(lastException, accounts.Count),
                lastException);
        }

        private static bool CanTryAnotherAccount(Exception exception)
        {
            var detail = GetExceptionDetail(exception);
            return detail.Contains("authentication", StringComparison.OrdinalIgnoreCase)
                || detail.Contains("credentials", StringComparison.OrdinalIgnoreCase)
                || detail.Contains("username and password", StringComparison.OrdinalIgnoreCase)
                || detail.Contains("5.7.0", StringComparison.OrdinalIgnoreCase)
                || detail.Contains("5.7.8", StringComparison.OrdinalIgnoreCase)
                || detail.Contains("4.7.0", StringComparison.OrdinalIgnoreCase)
                || detail.Contains("daily user sending limit", StringComparison.OrdinalIgnoreCase)
                || detail.Contains("quota", StringComparison.OrdinalIgnoreCase);
        }

        private string BuildDeliveryFailureMessage(Exception? exception, int accountCount)
        {
            var detail = GetExceptionDetail(exception);
            if (detail.Contains("authentication", StringComparison.OrdinalIgnoreCase)
                || detail.Contains("credentials", StringComparison.OrdinalIgnoreCase)
                || detail.Contains("username and password", StringComparison.OrdinalIgnoreCase)
                || detail.Contains("5.7.0", StringComparison.OrdinalIgnoreCase)
                || detail.Contains("5.7.8", StringComparison.OrdinalIgnoreCase))
            {
                return $"Gmail rejected the app password for all {accountCount} configured ETRS mailer accounts.";
            }

            if (detail.Contains("timed out", StringComparison.OrdinalIgnoreCase)
                || detail.Contains("connection", StringComparison.OrdinalIgnoreCase)
                || detail.Contains("network", StringComparison.OrdinalIgnoreCase)
                || detail.Contains("socket", StringComparison.OrdinalIgnoreCase))
            {
                return $"ETRS could not connect to {_options.Host}:{_options.Port}. Check the server firewall and SMTP network access.";
            }

            return string.IsNullOrWhiteSpace(detail)
                ? "The SMTP server did not provide an error reason."
                : $"SMTP delivery failed: {Truncate(detail, 600)}";
        }

        private static string GetExceptionDetail(Exception? exception)
        {
            var messages = new List<string>();
            for (var current = exception; current is not null; current = current.InnerException)
            {
                if (!string.IsNullOrWhiteSpace(current.Message))
                {
                    messages.Add(current.Message.Trim());
                }
            }
            return string.Join(" ", messages.Distinct(StringComparer.OrdinalIgnoreCase));
        }

        private static string Truncate(string value, int maxLength)
            => value.Length <= maxLength ? value : value[..maxLength];

        private IReadOnlyList<EtrsMailerAccount> GetConfiguredAccounts()
        {
            var accounts = _options.Accounts
                .Where(account => !string.IsNullOrWhiteSpace(account.Email) && !string.IsNullOrWhiteSpace(account.AppPassword))
                .ToList();
            if (accounts.Count == 0)
            {
                throw new InvalidOperationException("No ETRS mailer account is configured. Configure Email:Smtp:Accounts through user secrets or environment variables.");
            }

            return accounts;
        }

        private static string BuildHtmlBody(AttendanceEmailRequest request, IReadOnlyList<AttendanceEmailRow> rows)
        {
            var tableRows = rows.Count == 0
                ? "<tr><td colspan=\"3\" style=\"padding:12px\">No WFH attendance entries were found for this period.</td></tr>"
                : string.Join(string.Empty, rows.Select(row => $"<tr><td>{row.Date:MMM d, yyyy}</td><td>{Clock(row.TimeIn)}</td><td>{Clock(row.TimeOut)}</td></tr>"));

            return $"""
                <div style="font-family:Arial,sans-serif;color:#1e2939">
                  <h2 style="margin-bottom:4px">Electronic Time Record — Work From Home</h2>
                  <p style="margin-top:0">WFH attendance details requested for <strong>{WebUtility.HtmlEncode(request.EmployeeName)}</strong> ({WebUtility.HtmlEncode(request.IdNumber)}).</p>
                  <p>Period: <strong>{request.PeriodStart:MMMM d, yyyy}</strong> to <strong>{request.PeriodEnd:MMMM d, yyyy}</strong></p>
                  <table style="border-collapse:collapse;width:100%;max-width:620px" border="1" cellpadding="8">
                    <thead style="background:#f1cd42"><tr><th>Date</th><th>Time In</th><th>Time Out</th></tr></thead>
                    <tbody>{tableRows}</tbody>
                  </table>
                  <p style="color:#64748b">A CSV copy is attached for your reference.</p>
                  <p style="color:#64748b"><strong>This is a system-generated message. Please do not reply to this email.</strong></p>
                </div>
                """;
        }

        private static string BuildCsv(AttendanceEmailRequest request, IReadOnlyList<AttendanceEmailRow> rows)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Employee ID,Employee,Date,Time In,Time Out");
            foreach (var row in rows)
            {
                csv.Append(Csv(request.IdNumber)).Append(',')
                    .Append(Csv(request.EmployeeName)).Append(',')
                    .Append(row.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)).Append(',')
                    .Append(Clock(row.TimeIn)).Append(',')
                    .AppendLine(Clock(row.TimeOut));
            }
            return csv.ToString();
        }

        private static string Clock(DateTime? value) => value?.ToString("h:mm tt", CultureInfo.InvariantCulture) ?? string.Empty;
        private static string Csv(string value) => $"\"{value.Replace("\"", "\"\"")}\"";
        private static string SafeFilePart(string value) => string.Concat(value.Where(character => char.IsLetterOrDigit(character) || character is '-' or '_'));
        private sealed record AttendanceEmailRow(DateTime Date, DateTime? TimeIn, DateTime? TimeOut);
    }
}

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO.Compression;

namespace Financial.Services;

public sealed record AttendancePdfSection(
    string Title,
    IReadOnlyList<string> Headers,
    IReadOnlyList<IReadOnlyList<string>> Rows);

public static class AttendanceLogsPdfExporter
{
    public static byte[] Generate(
        DateTime periodStart,
        DateTime periodEnd,
        string preparedBy,
        string scope,
        IReadOnlyList<AttendancePdfSection> sections)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var generatedAt = DateTime.Now;
        var totalRecords = sections.Sum(section => section.Rows.Count);

        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(24);
                page.DefaultTextStyle(style => style.FontFamily(Fonts.Arial).FontSize(7));

                page.Header().Column(header =>
                {
                    header.Item().Text("Electronic Time Record System")
                        .FontSize(9).SemiBold().FontColor(Colors.Green.Darken2);
                    header.Item().Text("Attendance Logs Report")
                        .FontSize(18).Bold().FontColor(Colors.Grey.Darken3);
                    header.Item().PaddingTop(4).LineHorizontal(1).LineColor(Colors.Green.Darken1);
                });

                page.Content().PaddingVertical(12).Column(content =>
                {
                    content.Spacing(12);
                    content.Item().Background(Colors.Grey.Lighten4).Padding(10).Table(details =>
                    {
                        details.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        Detail(details, "Reporting period", $"{periodStart:MMMM d, yyyy} - {periodEnd:MMMM d, yyyy}");
                        Detail(details, "Generated", generatedAt.ToString("MMMM d, yyyy h:mm tt"));
                        Detail(details, "Prepared by", string.IsNullOrWhiteSpace(preparedBy) ? "System user" : preparedBy);
                        Detail(details, "Coverage", scope);
                        Detail(details, "Total records", totalRecords.ToString("N0"));
                        Detail(details, "Report contents", string.Join(" and ", sections.Select(section => section.Title)));
                    });

                    foreach (var section in sections)
                    {
                        content.Item().Column(sectionContent =>
                        {
                            sectionContent.Spacing(5);
                            sectionContent.Item().Row(row =>
                            {
                                row.RelativeItem().Text(section.Title).FontSize(11).Bold().FontColor(Colors.Green.Darken2);
                                row.AutoItem().AlignBottom().Text($"{section.Rows.Count:N0} record{(section.Rows.Count == 1 ? string.Empty : "s")}")
                                    .FontColor(Colors.Grey.Darken1);
                            });
                            sectionContent.Item().Element(container => ComposeTable(container, section));
                        });
                    }
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("ETRS Attendance Logs  |  ").FontColor(Colors.Grey.Darken1);
                    text.Span("Page ");
                    text.CurrentPageNumber();
                    text.Span(" of ");
                    text.TotalPages();
                });
            });
        }).GeneratePdf();
    }

    public static string CreateZipBase64(IEnumerable<(string FileName, byte[] Content)> files)
    {
        using var stream = new MemoryStream();
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var file in files)
            {
                var entry = archive.CreateEntry(file.FileName, CompressionLevel.Fastest);
                using var entryStream = entry.Open();
                entryStream.Write(file.Content);
            }
        }

        return Convert.ToBase64String(stream.ToArray());
    }

    private static void Detail(TableDescriptor table, string label, string value)
    {
        table.Cell().PaddingVertical(3).Column(column =>
        {
            column.Item().Text(label.ToUpperInvariant()).FontSize(6).SemiBold().FontColor(Colors.Grey.Darken1);
            column.Item().Text(value).FontSize(8).SemiBold();
        });
    }

    private static void ComposeTable(IContainer container, AttendancePdfSection section)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                foreach (var _ in section.Headers)
                {
                    columns.RelativeColumn();
                }
            });

            table.Header(header =>
            {
                foreach (var heading in section.Headers)
                {
                    header.Cell().Background(Colors.Green.Darken2).Border(0.5f).BorderColor(Colors.White)
                        .Padding(4).Text(heading).FontSize(6.5f).SemiBold().FontColor(Colors.White);
                }
            });

            for (var rowIndex = 0; rowIndex < section.Rows.Count; rowIndex++)
            {
                var background = rowIndex % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;
                foreach (var value in section.Rows[rowIndex])
                {
                    table.Cell().Background(background).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                        .Padding(3).Text(value ?? string.Empty).FontSize(6.5f);
                }
            }

            if (section.Rows.Count == 0)
            {
                table.Cell().ColumnSpan((uint)section.Headers.Count).Border(0.5f).BorderColor(Colors.Grey.Lighten2)
                    .Padding(8).AlignCenter().Text("No attendance records for this period.").Italic().FontColor(Colors.Grey.Darken1);
            }
        });
    }
}

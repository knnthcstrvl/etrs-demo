using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Financial.Data;
using System.Security.Claims;

namespace Financial.Services
{
    public class CurrentUserService
    {
        private readonly AuthenticationStateProvider _authProvider;
        private readonly IDbContextFactory<ProcurementDbContext> _dbFactory;

        public bool IsInitialized { get; private set; }
        public string Role { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string Username { get; private set; } = string.Empty;
        public string DisplayName { get; private set; } = string.Empty;
        public HashSet<string> AccessSet { get; private set; } = new(StringComparer.OrdinalIgnoreCase);

        public event Action? OnChanged;

        public CurrentUserService(AuthenticationStateProvider authProvider, IDbContextFactory<ProcurementDbContext> dbFactory)
        {
            _authProvider = authProvider;
            _dbFactory = dbFactory;
            // subscribe to auth changes (use async handler so RefreshAsync is awaited)
            _authProvider.AuthenticationStateChanged += async _ => await RefreshAsync();
        }

        public async Task InitializeAsync()
        {
            if (IsInitialized) return;
            await RefreshAsync();
        }

        public async Task RefreshAsync()
        {
            try
            {
                var state = await _authProvider.GetAuthenticationStateAsync();
                var user = state.User;
                AccessSet.Clear();

                if (user?.Identity is null || !user.Identity.IsAuthenticated)
                {
                    Role = string.Empty;
                    Email = string.Empty;
                    Username = string.Empty;
                    DisplayName = string.Empty;
                    IsInitialized = true;
                    OnChanged?.Invoke();
                    return;
                }

                Role = user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
                Email = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
                Username = user.Identity?.Name ?? string.Empty;

                await using var db = _dbFactory.CreateDbContext();
                var account = await db.UserAccounts
                    .Where(u => u.Email == Email || (!string.IsNullOrEmpty(Username) && u.Username == Username))
                    .FirstOrDefaultAsync();

                DisplayName = FormatDisplayName(account?.LastName, account?.FirstName, account?.MiddleName);
                if (string.IsNullOrWhiteSpace(DisplayName))
                {
                    DisplayName = !string.IsNullOrWhiteSpace(Username) ? Username : Email;
                }

                if (string.Equals(Role, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    // admin: full access
                    AccessSet = new HashSet<string>(new[] { "Add", "Edit", "Delete", "Import", "Export", "View" }, StringComparer.OrdinalIgnoreCase);
                }
                else
                {
                    if (account != null && !string.IsNullOrWhiteSpace(account.AccessLevel))
                    {
                        foreach (var tok in account.AccessLevel.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                        {
                            AccessSet.Add(tok);
                        }
                    }
                }

                IsInitialized = true;
                OnChanged?.Invoke();
            }
            catch
            {
                IsInitialized = true;
                OnChanged?.Invoke();
            }
        }

        public bool HasAccess(string access)
        {
            if (string.IsNullOrWhiteSpace(access)) return false;
            if (string.Equals(Role, "Admin", StringComparison.OrdinalIgnoreCase)) return true;
            return AccessSet.Contains(access);
        }

        private static string FormatDisplayName(string? lastName, string? firstName, string? middleName)
        {
            var parts = new[] { firstName, middleName, lastName }
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value!.Trim());

            return string.Join(" ", parts);
        }
    }
}

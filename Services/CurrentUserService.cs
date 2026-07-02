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
        public string Username { get; private set; } = string.Empty;
        public string DisplayName { get; private set; } = string.Empty;
        public HashSet<string> AccessSet { get; private set; } = new(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> DivisionScope { get; private set; } = new(StringComparer.OrdinalIgnoreCase);

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
                DivisionScope.Clear();

                if (user?.Identity is null || !user.Identity.IsAuthenticated)
                {
                    Role = string.Empty;
                    Username = string.Empty;
                    DisplayName = string.Empty;
                    IsInitialized = true;
                    OnChanged?.Invoke();
                    return;
                }

                Role = user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
                Username = user.Identity?.Name ?? string.Empty;
                var userIdText = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var hasUserId = int.TryParse(userIdText, out var userId);

                await using var db = _dbFactory.CreateDbContext();
                var account = await db.UserAccounts
                    .Where(u => (hasUserId && u.UserAccountId == userId) || (!string.IsNullOrEmpty(Username) && u.Username == Username))
                    .FirstOrDefaultAsync();

                DisplayName = account?.Name ?? string.Empty;
                if (string.IsNullOrWhiteSpace(DisplayName))
                {
                    DisplayName = Username;
                }

                if (account is not null)
                {
                    DivisionScope = ParseDivisionScope(account.DivisionCenterUnit);
                }

                if (string.Equals(Role, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    AccessSet = new HashSet<string>(AccessPolicy.AdminAssignableAccess, StringComparer.OrdinalIgnoreCase);
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
            return AccessPolicy.HasAccess(AccessSet, access);
        }

        public bool HasAnyAccess()
        {
            if (string.Equals(Role, "Admin", StringComparison.OrdinalIgnoreCase)) return true;
            return AccessPolicy.HasAnyAccess(AccessSet);
        }

        public bool CanAccessDivision(string? division)
        {
            if (string.Equals(Role, "Admin", StringComparison.OrdinalIgnoreCase)) return true;
            if (DivisionScope.Count == 0) return false;
            return !string.IsNullOrWhiteSpace(division)
                && DivisionScope.Contains(division.Trim());
        }

        public bool HasDivisionScope()
            => string.Equals(Role, "Admin", StringComparison.OrdinalIgnoreCase)
                || DivisionScope.Count > 0;

        private static HashSet<string> ParseDivisionScope(string? value)
            => new((value ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(item => !string.IsNullOrWhiteSpace(item)),
                StringComparer.OrdinalIgnoreCase);
    }
}

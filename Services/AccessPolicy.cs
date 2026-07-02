namespace Financial.Services
{
    public static class AccessPolicy
    {
        public const string ViewLogs = "View Logs";
        public const string Add = "Add";
        public const string InputLogs = "Input Logs (Manually)";
        public const string AddEmployee = "Add Employee";
        public const string AddUser = "Add User";
        public const string Edit = "Edit";
        public const string EditLogs = "Edit Logs";
        public const string EditEmployee = "Edit Employee";
        public const string EditUser = "Edit User";
        public const string Delete = "Delete";
        public const string DeleteEmployee = "Delete Employee";
        public const string DeleteUser = "Delete User";
        public const string ImportEmployees = "Import Employees";
        public const string Export = "Export";
        public const string ExportLogs = "Export Logs";
        public const string ExportEmployees = "Export Employees";
        public const string ApproveVerifications = "Approve Verifications";
        public const string ManageAttendanceLogs = "Manage Attendance Logs";
        public const string ManageEmployees = "Manage Employees";
        public const string ManageExceptions = "Manage Exceptions";
        public const string ManageUsers = "Manage Users";

        public static readonly IReadOnlyList<string> SpecificAccess = new[]
        {
            ViewLogs,
            Add,
            InputLogs,
            AddEmployee,
            AddUser,
            Edit,
            EditLogs,
            EditEmployee,
            EditUser,
            Delete,
            DeleteUser,
            ImportEmployees,
            Export,
            ExportLogs,
            ExportEmployees,
            ApproveVerifications
        };

        public static readonly IReadOnlyList<string> GeneralAccess = new[]
        {
            ManageAttendanceLogs,
            ManageEmployees,
            ManageExceptions,
            ManageUsers
        };

        public static readonly IReadOnlyList<string> AdminAssignableAccess = SpecificAccess
            .Concat(new[] { DeleteEmployee })
            .Concat(GeneralAccess)
            .ToArray();

        public static bool HasAccess(IEnumerable<string> accessLevels, string requested)
        {
            if (string.IsNullOrWhiteSpace(requested))
            {
                return false;
            }

            var accessSet = accessLevels as ISet<string>
                ?? new HashSet<string>(accessLevels, StringComparer.OrdinalIgnoreCase);

            if (accessSet.Contains(requested))
            {
                return true;
            }

            return requested switch
            {
                "View" => accessSet.Contains(ViewLogs),
                "Import" => accessSet.Contains(ImportEmployees),

                ViewLogs => accessSet.Contains("View")
                    || accessSet.Contains(ManageAttendanceLogs),

                Add => accessSet.Contains(InputLogs)
                    || accessSet.Contains(AddEmployee)
                    || accessSet.Contains(AddUser),
                InputLogs => accessSet.Contains("Add")
                    || accessSet.Contains(Add)
                    || accessSet.Contains(ManageAttendanceLogs),
                AddEmployee => accessSet.Contains("Add")
                    || accessSet.Contains(Add)
                    || accessSet.Contains(ManageEmployees),
                AddUser => accessSet.Contains("Add")
                    || accessSet.Contains(Add)
                    || accessSet.Contains(ManageUsers),

                Edit => accessSet.Contains(EditLogs)
                    || accessSet.Contains(EditEmployee)
                    || accessSet.Contains(EditUser),
                EditLogs => accessSet.Contains(Edit)
                    || accessSet.Contains(ManageAttendanceLogs),
                EditEmployee => accessSet.Contains(Edit)
                    || accessSet.Contains(ManageEmployees),
                EditUser => accessSet.Contains(Edit)
                    || accessSet.Contains(ManageUsers),

                Delete => accessSet.Contains(DeleteUser),
                DeleteUser => accessSet.Contains(Delete)
                    || accessSet.Contains(ManageUsers),

                ImportEmployees => accessSet.Contains("Import")
                    || accessSet.Contains(ManageEmployees),

                Export => accessSet.Contains(ExportLogs)
                    || accessSet.Contains(ExportEmployees),
                ExportLogs => accessSet.Contains(Export)
                    || accessSet.Contains(ManageAttendanceLogs),
                ExportEmployees => accessSet.Contains(Export)
                    || accessSet.Contains(ManageEmployees),

                ManageAttendanceLogs => accessSet.Contains(ViewLogs)
                    || accessSet.Contains(InputLogs)
                    || accessSet.Contains(EditLogs)
                    || accessSet.Contains(ExportLogs)
                    || accessSet.Contains("View")
                    || accessSet.Contains("Edit")
                    || accessSet.Contains("Export")
                    || accessSet.Contains("Import"),
                ManageEmployees => accessSet.Contains(AddEmployee)
                    || accessSet.Contains(EditEmployee)
                    || accessSet.Contains(ImportEmployees)
                    || accessSet.Contains(ExportEmployees)
                    || accessSet.Contains("View")
                    || accessSet.Contains("Edit")
                    || accessSet.Contains("Import")
                    || accessSet.Contains("Export"),
                ManageExceptions => accessSet.Contains(ManageExceptions),
                ManageUsers => accessSet.Contains(AddUser)
                    || accessSet.Contains(EditUser)
                    || accessSet.Contains(DeleteUser)
                    || accessSet.Contains("Add")
                    || accessSet.Contains("Edit")
                    || accessSet.Contains("Delete"),
                _ => false
            };
        }

        public static bool HasAnyAccess(IEnumerable<string> accessLevels)
            => accessLevels.Any(level => SpecificAccess.Contains(level, StringComparer.OrdinalIgnoreCase)
                || GeneralAccess.Contains(level, StringComparer.OrdinalIgnoreCase)
                || IsLegacyAccess(level));

        public static bool IsLegacyAccess(string access)
            => string.Equals(access, "View", StringComparison.OrdinalIgnoreCase)
                || string.Equals(access, "Import", StringComparison.OrdinalIgnoreCase);
    }
}

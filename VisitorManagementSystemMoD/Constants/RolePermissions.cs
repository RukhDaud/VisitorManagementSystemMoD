namespace VisitorManagementSystemMoD.Constants
{
    /// <summary>
    /// Defines all roles in the Visitor Management System
    /// </summary>
    public static class Roles
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        public const string SecurityOfficer = "Security Officer";
        public const string SectionOfficer = "Section Officer";
        public const string Employee = "Employee";
        public const string Reception = "Reception";
    }

    /// <summary>
    /// Defines all permissions/actions available in the system
    /// </summary>
    public static class Permissions
    {
        // Visitor Request Permissions
        public const string ViewAllVisitorRequests = "ViewAllVisitorRequests";
        public const string ViewPersonalVisitorRequests = "ViewPersonalVisitorRequests";
        public const string CreateVisitorRequest = "CreateVisitorRequest";
        public const string EditPersonalVisitorRequest = "EditPersonalVisitorRequest";
        public const string DeletePersonalVisitorRequest = "DeletePersonalVisitorRequest";
        public const string EditAnyVisitorRequest = "EditAnyVisitorRequest";
        public const string DeleteAnyVisitorRequest = "DeleteAnyVisitorRequest";

        // Approval Permissions
        public const string ApproveVisitorRequest = "ApproveVisitorRequest";
        public const string RejectVisitorRequest = "RejectVisitorRequest";

        // Check-In/Check-Out Permissions
        public const string CheckInVisitor = "CheckInVisitor";
        public const string CheckOutVisitor = "CheckOutVisitor";
        public const string ViewCheckInStatus = "ViewCheckInStatus";

        // Report Permissions
        public const string GenerateAllReports = "GenerateAllReports";
        public const string GenerateVisitorReports = "GenerateVisitorReports";
        public const string GenerateLimitedReports = "GenerateLimitedReports";
        public const string ViewCheckedInVisitors = "ViewCheckedInVisitors";
        public const string ViewCheckedOutVisitors = "ViewCheckedOutVisitors";
        public const string ViewPendingVisitors = "ViewPendingVisitors";

        // User Management Permissions
        public const string CreateUser = "CreateUser";
        public const string EditUser = "EditUser";
        public const string DeleteUser = "DeleteUser";
        public const string ViewAllUsers = "ViewAllUsers";

        // Role Management Permissions
        public const string CreateRole = "CreateRole";
        public const string EditRole = "EditRole";
        public const string DeleteRole = "DeleteRole";
        public const string ViewAllRoles = "ViewAllRoles";
        public const string AssignRoles = "AssignRoles";

        // Department Management Permissions
        public const string CreateDepartment = "CreateDepartment";
        public const string EditDepartment = "EditDepartment";
        public const string DeleteDepartment = "DeleteDepartment";
        public const string ViewAllDepartments = "ViewAllDepartments";

        // Department Employee Management Permissions
        public const string ManageDepartmentEmployees = "ManageDepartmentEmployees";
        public const string ViewDepartmentEmployees = "ViewDepartmentEmployees";

        // System Permissions
        public const string AccessAdminPanel = "AccessAdminPanel";
        public const string ManageSystemSettings = "ManageSystemSettings";
        public const string ViewSystemLogs = "ViewSystemLogs";
    }

    /// <summary>
    /// Maps roles to their respective permissions
    /// </summary>
    public static class RolePermissionsMapping
    {
        private static readonly Dictionary<string, List<string>> _rolePermissions = new()
        {
            {
                Roles.SuperAdmin, new List<string>
                {
                    // All Visitor Permissions
                    Permissions.ViewAllVisitorRequests,
                    Permissions.ViewPersonalVisitorRequests,
                    Permissions.CreateVisitorRequest,
                    Permissions.EditPersonalVisitorRequest,
                    Permissions.DeletePersonalVisitorRequest,
                    Permissions.EditAnyVisitorRequest,
                    Permissions.DeleteAnyVisitorRequest,

                    // All Approval Permissions
                    Permissions.ApproveVisitorRequest,
                    Permissions.RejectVisitorRequest,

                    // All Check-In/Out Permissions
                    Permissions.CheckInVisitor,
                    Permissions.CheckOutVisitor,
                    Permissions.ViewCheckInStatus,

                    // All Report Permissions
                    Permissions.GenerateAllReports,
                    Permissions.GenerateVisitorReports,
                    Permissions.GenerateLimitedReports,
                    Permissions.ViewCheckedInVisitors,
                    Permissions.ViewCheckedOutVisitors,
                    Permissions.ViewPendingVisitors,

                    // All User Management Permissions
                    Permissions.CreateUser,
                    Permissions.EditUser,
                    Permissions.DeleteUser,
                    Permissions.ViewAllUsers,

                    // All Role Management Permissions
                    Permissions.CreateRole,
                    Permissions.EditRole,
                    Permissions.DeleteRole,
                    Permissions.ViewAllRoles,
                    Permissions.AssignRoles,

                    // All Department Management Permissions
                    Permissions.CreateDepartment,
                    Permissions.EditDepartment,
                    Permissions.DeleteDepartment,
                    Permissions.ViewAllDepartments,

                    // Department Employee Management
                    Permissions.ManageDepartmentEmployees,
                    Permissions.ViewDepartmentEmployees,

                    // All System Permissions
                    Permissions.AccessAdminPanel,
                    Permissions.ManageSystemSettings,
                    Permissions.ViewSystemLogs
                }
            },
            {
                Roles.Admin, new List<string>
                {
                    // Visitor Permissions
                    Permissions.ViewAllVisitorRequests,
                    Permissions.ViewPersonalVisitorRequests,
                    Permissions.CreateVisitorRequest,
                    Permissions.EditPersonalVisitorRequest,
                    Permissions.DeletePersonalVisitorRequest,
                    Permissions.EditAnyVisitorRequest,

                    // Approval Permissions
                    Permissions.ApproveVisitorRequest,
                    Permissions.RejectVisitorRequest,

                    // Check-In/Out Permissions
                    Permissions.CheckInVisitor,
                    Permissions.CheckOutVisitor,
                    Permissions.ViewCheckInStatus,

                    // Report Permissions
                    Permissions.GenerateAllReports,
                    Permissions.GenerateVisitorReports,
                    Permissions.ViewCheckedInVisitors,
                    Permissions.ViewCheckedOutVisitors,
                    Permissions.ViewPendingVisitors,

                    // User Management Permissions (Limited)
                    Permissions.ViewAllUsers,

                    // Department Management Permissions
                    Permissions.ViewAllDepartments,

                    // System Permissions
                    Permissions.AccessAdminPanel
                }
            },
            {
                Roles.SecurityOfficer, new List<string>
                {
                    // Visitor Permissions
                    Permissions.ViewAllVisitorRequests,
                    Permissions.ViewPersonalVisitorRequests,

                    // Approval Permissions
                    Permissions.ApproveVisitorRequest,
                    Permissions.RejectVisitorRequest,

                    // Check-In/Out Status Viewing
                    Permissions.ViewCheckInStatus,

                    // Report Permissions
                    Permissions.GenerateVisitorReports,
                    Permissions.ViewCheckedInVisitors,
                    Permissions.ViewCheckedOutVisitors,
                    Permissions.ViewPendingVisitors
                }
            },
            {
                Roles.SectionOfficer, new List<string>
                {
                    // Visitor Permissions — create on behalf of department employees
                    Permissions.ViewPersonalVisitorRequests,
                    Permissions.CreateVisitorRequest,
                    Permissions.EditPersonalVisitorRequest,
                    Permissions.DeletePersonalVisitorRequest,

                    // Department Employee Management
                    Permissions.ViewDepartmentEmployees,

                    // View Check-In Status for own visitors
                    Permissions.ViewCheckInStatus
                }
            },
            {
                Roles.Employee, new List<string>
                {
                    // Personal Visitor Permissions Only
                    Permissions.ViewPersonalVisitorRequests,
                    Permissions.CreateVisitorRequest,
                    Permissions.EditPersonalVisitorRequest,
                    Permissions.DeletePersonalVisitorRequest,

                    // View Check-In Status for own visitors
                    Permissions.ViewCheckInStatus
                }
            },
            {
                Roles.Reception, new List<string>
                {
                    // View Approved/Pending Requests
                    Permissions.ViewAllVisitorRequests,
                    Permissions.ViewPendingVisitors,

                    // Check-In/Out Permissions
                    Permissions.CheckInVisitor,
                    Permissions.CheckOutVisitor,
                    Permissions.ViewCheckInStatus,

                    // Limited Report Generation
                    Permissions.GenerateLimitedReports,
                    Permissions.ViewCheckedInVisitors,
                    Permissions.ViewCheckedOutVisitors
                }
            }
        };

        /// <summary>
        /// Gets all permissions for a specific role
        /// </summary>
        public static List<string> GetPermissionsForRole(string roleName)
        {
            return _rolePermissions.TryGetValue(roleName, out var permissions) 
                ? permissions 
                : new List<string>();
        }

        /// <summary>
        /// Checks if a role has a specific permission
        /// </summary>
        public static bool HasPermission(string roleName, string permission)
        {
            return _rolePermissions.TryGetValue(roleName, out var permissions) 
                && permissions.Contains(permission);
        }

        /// <summary>
        /// Gets all roles
        /// </summary>
        public static IEnumerable<string> GetAllRoles()
        {
            return _rolePermissions.Keys;
        }
    }

    /// <summary>
    /// Defines role descriptions for display purposes
    /// </summary>
    public static class RoleDescriptions
    {
        private static readonly Dictionary<string, string> _descriptions = new()
        {
            {
                Roles.SuperAdmin,
                "Full system access with ability to manage all users, roles, permissions, and system settings. Can oversee all visitor requests, generate all reports, and perform all system operations."
            },
            {
                Roles.Admin,
                "System oversight and analytics with access to all visitor requests and comprehensive reporting capabilities. Can approve/reject requests and manage visitor operations but cannot modify system roles or permissions."
            },
            {
                Roles.SecurityOfficer,
                "Chief Security Officer responsible for approval management. Can view all visitor requests, approve or reject them, and generate visitor-related reports. No access to visitor creation or modification."
            },
            {
                Roles.SectionOfficer,
                "Section Officer responsible for creating visitor requests on behalf of department employees. Manages high-priority and regular employees within their assigned department."
            },
            {
                Roles.Employee,
                "Standard user who can create and manage their own visitor requests. Can view the status of their personal requests and update visitor information for their own visitors only."
            },
            {
                Roles.Reception,
                "Gate/Reception personnel responsible for visitor check-in and check-out. Can view approved and pending requests, process visitor entry/exit, and generate limited check-in/out reports."
            }
        };

        public static string GetDescription(string roleName)
        {
            return _descriptions.TryGetValue(roleName, out var description) 
                ? description 
                : "No description available";
        }
    }
}

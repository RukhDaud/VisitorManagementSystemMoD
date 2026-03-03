using VisitorManagementSystemMoD.Constants;

namespace VisitorManagementSystemMoD.Services
{
    /// <summary>
    /// Service for checking user permissions and roles
    /// </summary>
    public interface IAuthorizationService
    {
        bool HasPermission(string roleName, string permission);
        bool HasAnyPermission(string roleName, params string[] permissions);
        bool HasAllPermissions(string roleName, params string[] permissions);
        List<string> GetUserPermissions(string roleName);
        bool IsInRole(string userRole, params string[] allowedRoles);
        string GetRoleDescription(string roleName);
    }

    public class AuthorizationService : IAuthorizationService
    {
        /// <summary>
        /// Checks if a role has a specific permission
        /// </summary>
        public bool HasPermission(string roleName, string permission)
        {
            return RolePermissionsMapping.HasPermission(roleName, permission);
        }

        /// <summary>
        /// Checks if a role has any of the specified permissions
        /// </summary>
        public bool HasAnyPermission(string roleName, params string[] permissions)
        {
            return permissions.Any(p => RolePermissionsMapping.HasPermission(roleName, p));
        }

        /// <summary>
        /// Checks if a role has all of the specified permissions
        /// </summary>
        public bool HasAllPermissions(string roleName, params string[] permissions)
        {
            return permissions.All(p => RolePermissionsMapping.HasPermission(roleName, p));
        }

        /// <summary>
        /// Gets all permissions for a specific role
        /// </summary>
        public List<string> GetUserPermissions(string roleName)
        {
            return RolePermissionsMapping.GetPermissionsForRole(roleName);
        }

        /// <summary>
        /// Checks if a user role is in the list of allowed roles
        /// </summary>
        public bool IsInRole(string userRole, params string[] allowedRoles)
        {
            return allowedRoles.Contains(userRole);
        }

        /// <summary>
        /// Gets the description for a specific role
        /// </summary>
        public string GetRoleDescription(string roleName)
        {
            return RoleDescriptions.GetDescription(roleName);
        }
    }

    /// <summary>
    /// Extension methods for HttpContext to easily check permissions
    /// </summary>
    public static class HttpContextAuthorizationExtensions
    {
        /// <summary>
        /// Checks if current user has a specific permission
        /// </summary>
        public static bool HasPermission(this HttpContext context, string permission)
        {
            var userRole = context.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(userRole))
                return false;

            return RolePermissionsMapping.HasPermission(userRole, permission);
        }

        /// <summary>
        /// Checks if current user has any of the specified permissions
        /// </summary>
        public static bool HasAnyPermission(this HttpContext context, params string[] permissions)
        {
            var userRole = context.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(userRole))
                return false;

            return permissions.Any(p => RolePermissionsMapping.HasPermission(userRole, p));
        }

        /// <summary>
        /// Checks if current user is in any of the specified roles
        /// </summary>
        public static bool IsInRole(this HttpContext context, params string[] roles)
        {
            var userRole = context.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(userRole))
                return false;

            return roles.Contains(userRole);
        }

        /// <summary>
        /// Gets current user's role
        /// </summary>
        public static string? GetUserRole(this HttpContext context)
        {
            return context.Session.GetString("UserRole");
        }

        /// <summary>
        /// Gets all permissions for current user
        /// </summary>
        public static List<string> GetUserPermissions(this HttpContext context)
        {
            var userRole = context.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(userRole))
                return new List<string>();

            return RolePermissionsMapping.GetPermissionsForRole(userRole);
        }
    }
}

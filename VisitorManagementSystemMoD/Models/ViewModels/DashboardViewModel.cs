namespace VisitorManagementSystemMoD.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalVisitors { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }
        public int CheckedInVisitors { get; set; }
        public int CheckedOutVisitors { get; set; }
        public List<Visitor> RecentVisitors { get; set; } = new List<Visitor>();
        public List<Visitor> UpcomingVisitors { get; set; } = new List<Visitor>();
        public List<Visitor> TodayVisitors { get; set; } = new List<Visitor>();
        public List<Visitor> AllVisitorsList { get; set; } = new List<Visitor>();
        public List<Visitor> PendingVisitorsList { get; set; } = new List<Visitor>();
        public List<Visitor> ApprovedVisitorsList { get; set; } = new List<Visitor>();
        public List<Visitor> RejectedVisitorsList { get; set; } = new List<Visitor>();
        public List<Visitor> CheckedInVisitorsList { get; set; } = new List<Visitor>();
        public List<Visitor> CheckedOutVisitorsList { get; set; } = new List<Visitor>();
        public List<DepartmentStatViewModel> DepartmentStats { get; set; } = new List<DepartmentStatViewModel>();

        // SuperAdmin Dashboard Properties
        public List<RoleWithUsersViewModel> RolesWithUsers { get; set; } = new List<RoleWithUsersViewModel>();
        public List<UserListViewModel> AllUsers { get; set; } = new List<UserListViewModel>();
        public int TotalUsers { get; set; }
        public int TotalRoles { get; set; }
        public int TotalDepartments { get; set; }
    }

    public class DepartmentStatViewModel
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int VisitorCount { get; set; }
    }

    public class RoleWithUsersViewModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public int UserCount { get; set; }
        public List<UserListViewModel> Users { get; set; } = new List<UserListViewModel>();
    }

    public class UserListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisitorManagementSystemMoD.Models;
using VisitorManagementSystemMoD.Models.ViewModels;

namespace VisitorManagementSystemMoD.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool CheckAuthentication()
        {
            return HttpContext.Session.GetInt32("UserId") != null;
        }

        public IActionResult Index()
        {
            if (!CheckAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            var viewModel = new DashboardViewModel();

            switch (userRole)
            {
                case "Employee":
                    var employeeVisitors = _context.Visitors
                        .Where(v => v.EmployeeId == userId)
                        .Include(v => v.Employee)
                        .Include(v => v.ApprovedBy)
                        .Include(v => v.Department)
                        .OrderByDescending(v => v.RequestCreatedAt)
                        .ToList();

                    viewModel.TotalVisitors = employeeVisitors.Count;
                    viewModel.PendingRequests = employeeVisitors.Count(v => v.Status == "Pending");
                    viewModel.ApprovedRequests = employeeVisitors.Count(v => v.Status == "Approved");
                    viewModel.RejectedRequests = employeeVisitors.Count(v => v.Status == "Rejected");
                    viewModel.CheckedInVisitors = employeeVisitors.Count(v => v.CheckInTime.HasValue && !v.CheckOutTime.HasValue);
                    viewModel.CheckedOutVisitors = employeeVisitors.Count(v => v.CheckOutTime.HasValue);
                    viewModel.AllVisitorsList = employeeVisitors;
                    viewModel.PendingVisitorsList = employeeVisitors.Where(v => v.Status == "Pending").ToList();
                    viewModel.ApprovedVisitorsList = employeeVisitors.Where(v => v.Status == "Approved").ToList();
                    viewModel.RejectedVisitorsList = employeeVisitors.Where(v => v.Status == "Rejected").ToList();
                    viewModel.CheckedInVisitorsList = employeeVisitors.Where(v => v.CheckInTime.HasValue && !v.CheckOutTime.HasValue).ToList();
                    viewModel.CheckedOutVisitorsList = employeeVisitors.Where(v => v.CheckOutTime.HasValue).ToList();
                    // Recent Activity - Only today's visitors that have checked in or out
                    viewModel.RecentVisitors = employeeVisitors
                        .Where(v => v.CheckInTime.HasValue && v.CheckInTime.Value.Date == DateTime.Today)
                        .OrderByDescending(v => v.CheckInTime)
                        .ToList();
                    viewModel.UpcomingVisitors = employeeVisitors
                        .Where(v => (v.Status == "Approved" || v.Status == "Pending") && v.ExpectedTime > DateTime.Now && !v.CheckInTime.HasValue)
                        .OrderBy(v => v.ExpectedTime)
                        .Take(10)
                        .ToList();
                    break;

                case "Security Officer":
                    var allVisitors = _context.Visitors
                        .Include(v => v.Employee)
                        .Include(v => v.ApprovedBy)
                        .Include(v => v.Department)
                        .OrderByDescending(v => v.RequestCreatedAt)
                        .ToList();

                    viewModel.TotalVisitors = allVisitors.Count;
                    viewModel.PendingRequests = allVisitors.Count(v => v.Status == "Pending");
                    viewModel.ApprovedRequests = allVisitors.Count(v => v.Status == "Approved");
                    viewModel.RejectedRequests = allVisitors.Count(v => v.Status == "Rejected");
                    viewModel.CheckedInVisitors = allVisitors.Count(v => v.CheckInTime.HasValue && !v.CheckOutTime.HasValue);
                    viewModel.CheckedOutVisitors = allVisitors.Count(v => v.CheckOutTime.HasValue);
                    viewModel.AllVisitorsList = allVisitors;
                    viewModel.PendingVisitorsList = allVisitors.Where(v => v.Status == "Pending").ToList();
                    viewModel.ApprovedVisitorsList = allVisitors.Where(v => v.Status == "Approved").ToList();
                    viewModel.RejectedVisitorsList = allVisitors.Where(v => v.Status == "Rejected").ToList();
                    viewModel.CheckedInVisitorsList = allVisitors.Where(v => v.CheckInTime.HasValue && !v.CheckOutTime.HasValue).ToList();
                    viewModel.CheckedOutVisitorsList = allVisitors.Where(v => v.CheckOutTime.HasValue).ToList();
                    // Recent Activity - Only today's visitors that have checked in
                    viewModel.RecentVisitors = allVisitors
                        .Where(v => v.Status == "Approved" && v.CheckInTime.HasValue && v.CheckInTime.Value.Date == DateTime.Today)
                        .OrderByDescending(v => v.CheckInTime)
                        .ToList();
                    viewModel.TodayVisitors = allVisitors.Where(v => v.ExpectedTime.Date == DateTime.Today && v.Status == "Approved").ToList();
                    break;

                case "Reception":
                    var gateVisitors = _context.Visitors
                        .Include(v => v.Employee)
                        .Include(v => v.ApprovedBy)
                        .Include(v => v.Department)
                        .Where(v => v.Status == "Approved")
                        .OrderByDescending(v => v.ExpectedTime)
                        .ToList();

                    viewModel.TotalVisitors = gateVisitors.Count;
                    viewModel.PendingRequests = gateVisitors.Count(v => !v.CheckInTime.HasValue);
                    viewModel.CheckedInVisitors = gateVisitors.Count(v => v.CheckInTime.HasValue && !v.CheckOutTime.HasValue);
                    viewModel.CheckedOutVisitors = gateVisitors.Count(v => v.CheckOutTime.HasValue);
                    viewModel.AllVisitorsList = gateVisitors;
                    viewModel.PendingVisitorsList = gateVisitors.Where(v => !v.CheckInTime.HasValue).ToList();
                    viewModel.CheckedInVisitorsList = gateVisitors.Where(v => v.CheckInTime.HasValue && !v.CheckOutTime.HasValue).ToList();
                    viewModel.CheckedOutVisitorsList = gateVisitors.Where(v => v.CheckOutTime.HasValue).ToList();
                    viewModel.TodayVisitors = gateVisitors.Where(v => v.ExpectedTime.Date == DateTime.Today).ToList();
                    viewModel.RecentVisitors = gateVisitors.Where(v => v.CheckInTime.HasValue && v.CheckInTime.Value.Date == DateTime.Today).OrderByDescending(v => v.CheckInTime).ToList();
                    break;

                case "Admin":
                case "SuperAdmin":
                    var adminVisitors = _context.Visitors
                        .Include(v => v.Employee)
                        .Include(v => v.ApprovedBy)
                        .Include(v => v.Department)
                        .OrderByDescending(v => v.RequestCreatedAt)
                        .ToList();

                    viewModel.TotalVisitors = adminVisitors.Count;
                    viewModel.PendingRequests = adminVisitors.Count(v => v.Status == "Pending");
                    viewModel.ApprovedRequests = adminVisitors.Count(v => v.Status == "Approved");
                    viewModel.RejectedRequests = adminVisitors.Count(v => v.Status == "Rejected");
                    viewModel.CheckedInVisitors = adminVisitors.Count(v => v.CheckInTime.HasValue && !v.CheckOutTime.HasValue);
                    viewModel.CheckedOutVisitors = adminVisitors.Count(v => v.CheckOutTime.HasValue);
                    viewModel.AllVisitorsList = adminVisitors;
                    viewModel.PendingVisitorsList = adminVisitors.Where(v => v.Status == "Pending").ToList();
                    viewModel.ApprovedVisitorsList = adminVisitors.Where(v => v.Status == "Approved").ToList();
                    viewModel.RejectedVisitorsList = adminVisitors.Where(v => v.Status == "Rejected").ToList();
                    viewModel.CheckedInVisitorsList = adminVisitors.Where(v => v.CheckInTime.HasValue && !v.CheckOutTime.HasValue).ToList();
                    viewModel.CheckedOutVisitorsList = adminVisitors.Where(v => v.CheckOutTime.HasValue).ToList();
                    viewModel.TodayVisitors = adminVisitors.Where(v => v.ExpectedTime.Date == DateTime.Today).ToList();
                    viewModel.RecentVisitors = adminVisitors.Where(v => v.CheckInTime.HasValue && v.CheckInTime.Value.Date == DateTime.Today).OrderByDescending(v => v.CheckInTime).ToList();

                    // Department statistics for charts
                    viewModel.DepartmentStats = _context.Visitors
                        .Include(v => v.Department)
                        .Where(v => v.DepartmentId.HasValue)
                        .GroupBy(v => v.Department!.Name)
                        .Select(g => new DepartmentStatViewModel
                        {
                            DepartmentName = g.Key,
                            VisitorCount = g.Count()
                        })
                        .OrderByDescending(d => d.VisitorCount)
                        .Take(10)
                        .ToList();

                    break;
            }

            ViewBag.UserRole = userRole;
            return View(viewModel);
        }

        // Get visitor statistics for charts (AJAX)
        [HttpGet]
        public IActionResult GetVisitorStats()
        {
            if (!CheckAuthentication())
            {
                return Json(new { success = false, message = "Not authenticated" });
            }

            var allVisitors = _context.Visitors.ToList();

            var insideCount = allVisitors.Count(v => v.CheckInTime.HasValue && !v.CheckOutTime.HasValue);
            var outsideCount = allVisitors.Count(v => v.CheckOutTime.HasValue || !v.CheckInTime.HasValue);

            return Json(new
            {
                success = true,
                inside = insideCount,
                outside = outsideCount
            });
        }

        // Get department statistics for charts (AJAX)
        [HttpGet]
        public IActionResult GetDepartmentStats()
        {
            if (!CheckAuthentication())
            {
                return Json(new { success = false, message = "Not authenticated" });
            }

            var stats = _context.Visitors
                .Include(v => v.Department)
                .Where(v => v.DepartmentId.HasValue)
                .GroupBy(v => v.Department!.Name)
                .Select(g => new
                {
                    department = g.Key,
                    count = g.Count()
                })
                .OrderByDescending(d => d.count)
                .Take(10)
                .ToList();

            return Json(new { success = true, data = stats });
        }

        // Refresh CheckedIn/CheckedOut counts (AJAX)
        [HttpGet]
        public IActionResult RefreshCounts()
        {
            if (!CheckAuthentication())
            {
                return Json(new { success = false, message = "Not authenticated" });
            }

            var allVisitors = _context.Visitors.ToList();

            var checkedIn = allVisitors.Count(v => v.CheckInTime.HasValue && !v.CheckOutTime.HasValue);
            var checkedOut = allVisitors.Count(v => v.CheckOutTime.HasValue);
            var lateCheckouts = allVisitors.Count(v => v.CheckOutTime.HasValue && v.CheckOutTime.Value.Hour >= 19);

            return Json(new
            {
                success = true,
                checkedIn = checkedIn,
                checkedOut = checkedOut,
                lateCheckouts = lateCheckouts
            });
        }

        // Administration Dashboard (SuperAdmin only)
        public IActionResult Administration()
        {
            if (!CheckAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            var userRole = HttpContext.Session.GetString("UserRole");

            // Only SuperAdmin can access administration dashboard
            if (userRole != "SuperAdmin")
            {
                return RedirectToAction("Index");
            }

            var viewModel = new DashboardViewModel();

            viewModel.TotalUsers = _context.Users.Count();
            viewModel.TotalRoles = _context.Roles.Count();
            viewModel.TotalDepartments = _context.Departments.Count();
            viewModel.TotalVisitors = _context.Visitors.Count();

            // Get all roles with their users (top 3 per role for cards)
            viewModel.RolesWithUsers = _context.Roles
                .OrderBy(r => r.Name)
                .Select(r => new RoleWithUsersViewModel
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    UserCount = r.Users.Count,
                    Users = r.Users
                        .OrderByDescending(u => u.IsActive)
                        .ThenBy(u => u.Name)
                        .Take(3)
                        .Select(u => new UserListViewModel
                        {
                            Id = u.Id,
                            Name = u.Name,
                            Username = u.Username,
                            RoleName = r.Name,
                            DepartmentName = u.Department != null ? u.Department.Name : null,
                            IsActive = u.IsActive,
                            CreatedAt = u.CreatedAt
                        })
                        .ToList()
                })
                .ToList();

            // Get all users for the table
            viewModel.AllUsers = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Department)
                .OrderByDescending(u => u.IsActive)
                .ThenBy(u => u.Name)
                .Select(u => new UserListViewModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    Username = u.Username,
                    RoleName = u.Role.Name,
                    DepartmentName = u.Department != null ? u.Department.Name : null,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                })
                .ToList();

            ViewBag.UserRole = userRole;
            return View("SuperAdminIndex", viewModel);
        }

        // Audit Trail Page
        public IActionResult Audit()
        {
            if (!CheckAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole != "Admin" && userRole != "SuperAdmin" && userRole != "Security Officer")
            {
                return RedirectToAction("Index");
            }

            ViewBag.UserRole = userRole;
            ViewBag.Departments = _context.Departments.OrderBy(d => d.Name).ToList();
            return View();
        }

        // AJAX endpoint for filtered audit data
        [HttpGet]
        public IActionResult GetAuditData(string? search, string? status, int? departmentId, string? dateFrom, string? dateTo)
        {
            if (!CheckAuthentication())
            {
                return Json(new { success = false, message = "Not authenticated" });
            }

            var query = _context.Visitors
                .Include(v => v.Employee)
                .Include(v => v.ApprovedBy)
                .Include(v => v.Department)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                query = query.Where(v => v.Name.ToLower().Contains(s) ||
                                          v.CNIC.Contains(s) ||
                                          v.EmployeeName.ToLower().Contains(s) ||
                                          v.Purpose.ToLower().Contains(s));
            }

            if (!string.IsNullOrWhiteSpace(status) && status != "all")
            {
                if (status == "CheckedIn")
                    query = query.Where(v => v.CheckInTime.HasValue && !v.CheckOutTime.HasValue);
                else if (status == "CheckedOut")
                    query = query.Where(v => v.CheckOutTime.HasValue);
                else
                    query = query.Where(v => v.Status == status);
            }

            if (departmentId.HasValue && departmentId > 0)
            {
                query = query.Where(v => v.DepartmentId == departmentId);
            }

            if (!string.IsNullOrWhiteSpace(dateFrom) && DateTime.TryParse(dateFrom, out var from))
            {
                query = query.Where(v => v.RequestCreatedAt >= from);
            }

            if (!string.IsNullOrWhiteSpace(dateTo) && DateTime.TryParse(dateTo, out var to))
            {
                to = to.Date.AddDays(1);
                query = query.Where(v => v.RequestCreatedAt < to);
            }

            var data = query
                .OrderByDescending(v => v.RequestCreatedAt)
                .ToList()
                .Select(v => new
                {
                    id = v.Id,
                    name = v.Name,
                    cnic = v.CNIC,
                    phone = v.Phone,
                    purpose = v.Purpose,
                    expectedTime = v.ExpectedTime.ToString("MMM dd, yyyy hh:mm tt"),
                    hasVehicle = v.HasVehicle,
                    vehicleNumber = v.VehicleNumber ?? "N/A",
                    status = v.CheckOutTime.HasValue ? "Checked Out" :
                             v.CheckInTime.HasValue ? "Checked In" :
                             v.Status,
                    employeeName = v.EmployeeName,
                    departmentName = v.Department != null ? v.Department.Name : "N/A",
                    approvedByName = v.ApprovedByName ?? "N/A",
                    approvedAt = v.ApprovedAt.HasValue ? v.ApprovedAt.Value.ToString("MMM dd, yyyy hh:mm tt") : "N/A",
                    rejectionReason = v.RejectionReason ?? "N/A",
                    checkInTime = v.CheckInTime.HasValue ? v.CheckInTime.Value.ToString("MMM dd, yyyy hh:mm tt") : "N/A",
                    checkOutTime = v.CheckOutTime.HasValue ? v.CheckOutTime.Value.ToString("MMM dd, yyyy hh:mm tt") : "N/A",
                    requestCreatedAt = v.RequestCreatedAt.ToString("MMM dd, yyyy hh:mm tt")
                })
                .ToList();

            return Json(new { success = true, data, totalCount = data.Count });
        }

        // Generate report based on period
        [HttpGet]
        public IActionResult GenerateReport(string type, string period)
        {
            if (!CheckAuthentication())
            {
                return Json(new { success = false, message = "Not authenticated" });
            }

            DateTime startDate = DateTime.Today;
            DateTime endDate = DateTime.Now;

            switch (period?.ToLower())
            {
                case "weekly":
                    startDate = DateTime.Today.AddDays(-7);
                    break;
                case "monthly":
                    startDate = DateTime.Today.AddMonths(-1);
                    break;
                case "yearly":
                    startDate = DateTime.Today.AddYears(-1);
                    break;
                default:
                    startDate = DateTime.Today;
                    break;
            }

            var visitors = _context.Visitors
                .Include(v => v.Employee)
                .Include(v => v.Department)
                .Where(v => (type == "checkedin" && v.CheckInTime >= startDate && v.CheckInTime <= endDate) ||
                           (type == "checkedout" && v.CheckOutTime >= startDate && v.CheckOutTime <= endDate))
                .ToList();

            var reportData = visitors.Select(v => new
            {
                name = v.Name,
                cnic = v.CNIC,
                purpose = v.Purpose,
                employeeName = v.EmployeeName,
                departmentName = v.Department?.Name ?? "N/A",
                checkInTime = v.CheckInTime?.ToString("MMM dd, yyyy hh:mm tt") ?? "N/A",
                checkOutTime = v.CheckOutTime?.ToString("MMM dd, yyyy hh:mm tt") ?? "N/A",
                isLateCheckout = v.CheckOutTime.HasValue && v.CheckOutTime.Value.Hour >= 19
            }).ToList();

            var lateCheckouts = reportData.Count(r => r.isLateCheckout);

            return Json(new
            {
                success = true,
                period = period,
                type = type,
                startDate = startDate.ToString("MMM dd, yyyy"),
                endDate = endDate.ToString("MMM dd, yyyy"),
                totalCount = reportData.Count,
                lateCheckouts = lateCheckouts,
                data = reportData
            });
        }
    }
}

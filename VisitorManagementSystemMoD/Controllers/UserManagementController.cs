using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VisitorManagementSystemMoD.Models;
using VisitorManagementSystemMoD.Models.ViewModels;

namespace VisitorManagementSystemMoD.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserManagementController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool CheckSuperAdminAuthentication()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return false;

            var user = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.Id == userId);
            return user?.Role?.Name == "SuperAdmin";
        }

        // GET: UserManagement
        public IActionResult Index()
        {
            if (!CheckSuperAdminAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            var users = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Department)
                .OrderBy(u => u.Name)
                .ToList();

            return View(users);
        }

        // GET: UserManagement/Create
        public IActionResult Create()
        {
            if (!CheckSuperAdminAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Roles = new SelectList(_context.Roles.OrderBy(r => r.Name), "Id", "Name");
            ViewBag.Departments = new SelectList(_context.Departments.OrderBy(d => d.Name), "Id", "Name");

            return View();
        }

        // POST: UserManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserViewModel model)
        {
            if (!CheckSuperAdminAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new SelectList(_context.Roles.OrderBy(r => r.Name), "Id", "Name", model.RoleId);
                ViewBag.Departments = new SelectList(_context.Departments.OrderBy(d => d.Name), "Id", "Name", model.DepartmentId);
                return View(model);
            }

            // Check if username already exists
            if (_context.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Username already exists");
                ViewBag.Roles = new SelectList(_context.Roles.OrderBy(r => r.Name), "Id", "Name", model.RoleId);
                ViewBag.Departments = new SelectList(_context.Departments.OrderBy(d => d.Name), "Id", "Name", model.DepartmentId);
                return View(model);
            }

            var user = new User
            {
                Name = model.Name,
                Username = model.Username,
                Password = model.Password, // In production, hash this!
                RoleId = model.RoleId,
                DepartmentId = model.DepartmentId,
                IsActive = model.IsActive,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            TempData["Success"] = $"User '{user.Name}' created successfully!";
            return RedirectToAction("Index");
        }

        // POST: UserManagement/CreateAjax
        [HttpPost]
        public IActionResult CreateAjax([FromBody] CreateUserAjaxDto dto)
        {
            if (!CheckSuperAdminAuthentication())
                return Json(new { success = false, message = "Unauthorized" });

            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return Json(new { success = false, message = "Name, Username and Password are required." });

            if (dto.Password.Length < 6)
                return Json(new { success = false, message = "Password must be at least 6 characters." });

            if (_context.Users.Any(u => u.Username == dto.Username))
                return Json(new { success = false, message = "Username already exists." });

            if (dto.RoleId <= 0)
                return Json(new { success = false, message = "Please select a role." });

            var user = new User
            {
                Name = dto.Name,
                Username = dto.Username,
                Password = dto.Password,
                RoleId = dto.RoleId,
                DepartmentId = dto.DepartmentId,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Json(new { success = true, message = $"User '{user.Name}' created successfully!" });
        }

        public class CreateUserAjaxDto
        {
            public string Name { get; set; } = string.Empty;
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public int RoleId { get; set; }
            public int? DepartmentId { get; set; }
            public bool IsActive { get; set; } = true;
        }

        // GET: UserManagement/GetRolesAndDepartments
        [HttpGet]
        public IActionResult GetRolesAndDepartments()
        {
            if (!CheckSuperAdminAuthentication())
                return Json(new { success = false });

            var roles = _context.Roles.OrderBy(r => r.Name).Select(r => new { r.Id, r.Name }).ToList();
            var departments = _context.Departments.OrderBy(d => d.Name).Select(d => new { d.Id, d.Name }).ToList();

            return Json(new { success = true, roles, departments });
        }

        // GET: UserManagement/Edit/5
        public IActionResult Edit(int id)
        {
            if (!CheckSuperAdminAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                TempData["Error"] = "User not found";
                return RedirectToAction("Index");
            }

            // Prevent editing SuperAdmin user
            if (user.Role?.Name == "SuperAdmin" && user.Id == 1)
            {
                TempData["Error"] = "Cannot edit default SuperAdmin user";
                return RedirectToAction("Index");
            }

            var model = new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Username = user.Username,
                Password = user.Password,
                RoleId = user.RoleId ?? 0,
                DepartmentId = user.DepartmentId,
                IsActive = user.IsActive
            };

            ViewBag.Roles = new SelectList(_context.Roles.OrderBy(r => r.Name), "Id", "Name", model.RoleId);
            ViewBag.Departments = new SelectList(_context.Departments.OrderBy(d => d.Name), "Id", "Name", model.DepartmentId);

            return View(model);
        }

        // POST: UserManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, UserViewModel model)
        {
            if (!CheckSuperAdminAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new SelectList(_context.Roles.OrderBy(r => r.Name), "Id", "Name", model.RoleId);
                ViewBag.Departments = new SelectList(_context.Departments.OrderBy(d => d.Name), "Id", "Name", model.DepartmentId);
                return View(model);
            }

            var user = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                TempData["Error"] = "User not found";
                return RedirectToAction("Index");
            }

            // Prevent editing SuperAdmin user
            if (user.Role?.Name == "SuperAdmin" && user.Id == 1)
            {
                TempData["Error"] = "Cannot edit default SuperAdmin user";
                return RedirectToAction("Index");
            }

            // Check if username conflicts with another user
            if (_context.Users.Any(u => u.Username == model.Username && u.Id != id))
            {
                ModelState.AddModelError("Username", "Username already exists");
                ViewBag.Roles = new SelectList(_context.Roles.OrderBy(r => r.Name), "Id", "Name", model.RoleId);
                ViewBag.Departments = new SelectList(_context.Departments.OrderBy(d => d.Name), "Id", "Name", model.DepartmentId);
                return View(model);
            }

            user.Name = model.Name;
            user.Username = model.Username;
            user.Password = model.Password; // In production, only update if changed and hash it!
            user.RoleId = model.RoleId;
            user.DepartmentId = model.DepartmentId;
            user.IsActive = model.IsActive;
            user.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            TempData["Success"] = $"User '{user.Name}' updated successfully!";
            return RedirectToAction("Index");
        }

        // POST: UserManagement/Delete/5
        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!CheckSuperAdminAuthentication())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            // Prevent deleting SuperAdmin user
            if (user.Role?.Name == "SuperAdmin" && user.Id == 1)
            {
                return Json(new { success = false, message = "Cannot delete default SuperAdmin user" });
            }

            // Clean up all FK references before deleting
            // 1. Delete visitors created by this user
            var createdVisitors = _context.Visitors.Where(v => v.EmployeeId == id).ToList();
            _context.Visitors.RemoveRange(createdVisitors);

            // 2. Null out approver reference on visitors approved by this user
            var approvedVisitors = _context.Visitors.Where(v => v.ApprovedById == id).ToList();
            foreach (var v in approvedVisitors)
            {
                v.ApprovedById = null;
            }

            // 3. Delete alerts created by this user
            var alerts = _context.Alerts.Where(a => a.CreatedById == id).ToList();
            _context.Alerts.RemoveRange(alerts);

            // 4. Delete blocked visitors created by this user
            var blockedVisitors = _context.BlockedVisitors.Where(b => b.BlockedById == id).ToList();
            _context.BlockedVisitors.RemoveRange(blockedVisitors);

            // 5. Delete department employees owned by this user
            var deptEmployees = _context.DepartmentEmployees.Where(de => de.UserId == id).ToList();
            _context.DepartmentEmployees.RemoveRange(deptEmployees);

            _context.Users.Remove(user);
            _context.SaveChanges();

            var deleted = createdVisitors.Count + alerts.Count + blockedVisitors.Count + deptEmployees.Count;
            var msg = $"User '{user.Name}' deleted successfully";
            if (deleted > 0)
            {
                msg += $" along with {createdVisitors.Count} visitor(s), {alerts.Count} alert(s), {blockedVisitors.Count} blocked record(s), {deptEmployees.Count} employee(s)";
            }

            return Json(new { success = true, message = msg });
        }

        // POST: UserManagement/ToggleActive/5
        [HttpPost]
        public IActionResult ToggleActive(int id)
        {
            if (!CheckSuperAdminAuthentication())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var user = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            // Prevent deactivating SuperAdmin user
            if (user.Role?.Name == "SuperAdmin" && user.Id == 1)
            {
                return Json(new { success = false, message = "Cannot deactivate default SuperAdmin user" });
            }

            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            return Json(new { success = true, isActive = user.IsActive, message = $"User '{user.Name}' {(user.IsActive ? "activated" : "deactivated")} successfully" });
        }
    }
}

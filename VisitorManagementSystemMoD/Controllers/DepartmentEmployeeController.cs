using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VisitorManagementSystemMoD.Models;

namespace VisitorManagementSystemMoD.Controllers
{
    public class DepartmentEmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentEmployeeController(ApplicationDbContext context)
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

        // GET: DepartmentEmployee
        public IActionResult Index(int? userId)
        {
            if (!CheckSuperAdminAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            var query = _context.DepartmentEmployees
                .Include(de => de.User)
                .ThenInclude(u => u!.Role)
                .Include(de => de.User)
                .ThenInclude(u => u!.Department)
                .AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(de => de.UserId == userId.Value);
                var soUser = _context.Users.Include(u => u.Department).FirstOrDefault(u => u.Id == userId.Value);
                ViewBag.FilteredUser = soUser;
            }

            var employees = query.OrderBy(de => de.User!.Name).ThenBy(de => de.Name).ToList();

            // Get all SO users for the filter dropdown
            var soUsers = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Department)
                .Where(u => u.Role != null && u.Role.Name == "Section Officer")
                .OrderBy(u => u.Name)
                .ToList();
            ViewBag.SOUsers = soUsers;
            ViewBag.FilterUserId = userId;

            return View(employees);
        }

        // GET: DepartmentEmployee/Create
        public IActionResult Create(int? userId)
        {
            if (!CheckSuperAdminAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            PopulateDropdowns(userId);
            return View();
        }

        // POST: DepartmentEmployee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string name, int userId, bool isActive)
        {
            if (!CheckSuperAdminAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError("Name", "Employee name is required");
                PopulateDropdowns(userId);
                return View();
            }

            if (userId <= 0)
            {
                ModelState.AddModelError("UserId", "Please select a Section Officer account");
                PopulateDropdowns(userId);
                return View();
            }

            var employee = new DepartmentEmployee
            {
                Name = name.Trim(),
                IsHighPriority = false,
                UserId = userId,
                IsActive = isActive,
                CreatedAt = DateTime.Now
            };

            _context.DepartmentEmployees.Add(employee);
            _context.SaveChanges();

            TempData["Success"] = $"Employee '{employee.Name}' added successfully!";
            return RedirectToAction("Index", new { userId });
        }

        // POST: DepartmentEmployee/CreateAjax
        [HttpPost]
        public IActionResult CreateAjax([FromBody] CreateDeptEmployeeDto dto)
        {
            if (!CheckSuperAdminAuthentication())
                return Json(new { success = false, message = "Unauthorized" });

            if (string.IsNullOrWhiteSpace(dto.Name))
                return Json(new { success = false, message = "Employee name is required." });

            if (dto.UserId <= 0)
                return Json(new { success = false, message = "Please select a Section Officer account." });

            var employee = new DepartmentEmployee
            {
                Name = dto.Name.Trim(),
                IsHighPriority = false,
                UserId = dto.UserId,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.Now
            };

            _context.DepartmentEmployees.Add(employee);
            _context.SaveChanges();

            return Json(new { success = true, message = $"Employee '{employee.Name}' added successfully!", id = employee.Id });
        }

        // GET: DepartmentEmployee/Edit/5
        public IActionResult Edit(int id)
        {
            if (!CheckSuperAdminAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            var employee = _context.DepartmentEmployees
                .Include(de => de.User)
                .FirstOrDefault(de => de.Id == id);

            if (employee == null)
            {
                TempData["Error"] = "Employee not found";
                return RedirectToAction("Index");
            }

            PopulateDropdowns(employee.UserId);
            return View(employee);
        }

        // POST: DepartmentEmployee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, string name, int userId, bool isActive)
        {
            if (!CheckSuperAdminAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            var employee = _context.DepartmentEmployees.FirstOrDefault(de => de.Id == id);
            if (employee == null)
            {
                TempData["Error"] = "Employee not found";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError("Name", "Employee name is required");
                PopulateDropdowns(userId);
                return View(employee);
            }

            employee.Name = name.Trim();
            employee.UserId = userId;
            employee.IsActive = isActive;
            employee.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            TempData["Success"] = $"Employee '{employee.Name}' updated successfully!";
            return RedirectToAction("Index", new { userId });
        }

        // POST: DepartmentEmployee/Delete/5
        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!CheckSuperAdminAuthentication())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var employee = _context.DepartmentEmployees.FirstOrDefault(de => de.Id == id);
            if (employee == null)
            {
                return Json(new { success = false, message = "Employee not found" });
            }

            // Check if employee has visitor records
            var hasVisitors = _context.Visitors.Any(v => v.DepartmentEmployeeId == id);
            if (hasVisitors)
            {
                return Json(new { success = false, message = $"Cannot delete '{employee.Name}'. Employee has visitor records. Consider deactivating instead." });
            }

            _context.DepartmentEmployees.Remove(employee);
            _context.SaveChanges();

            return Json(new { success = true, message = $"Employee '{employee.Name}' deleted successfully" });
        }

        // POST: DepartmentEmployee/ToggleActive/5
        [HttpPost]
        public IActionResult ToggleActive(int id)
        {
            if (!CheckSuperAdminAuthentication())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var employee = _context.DepartmentEmployees.FirstOrDefault(de => de.Id == id);
            if (employee == null)
            {
                return Json(new { success = false, message = "Employee not found" });
            }

            employee.IsActive = !employee.IsActive;
            employee.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            return Json(new { success = true, isActive = employee.IsActive, message = $"Employee '{employee.Name}' {(employee.IsActive ? "activated" : "deactivated")} successfully" });
        }

        // API: Get employees for a specific SO user (used by Visitor Create form)
        [HttpGet]
        public IActionResult GetEmployeesForUser(int userId)
        {
            var employees = _context.DepartmentEmployees
                .Where(de => de.UserId == userId && de.IsActive);

            var result = employees.OrderBy(de => de.Name)
                .Select(de => new { de.Id, de.Name })
                .ToList();

            return Json(new { success = true, employees = result });
        }

        // ====== Section Officer Self-Management ======

        private bool CheckSOAuthentication()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return false;
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Section Officer";
        }

        private int? GetSODepartmentId()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return null;
            return _context.Users.Where(u => u.Id == userId).Select(u => u.DepartmentId).FirstOrDefault();
        }

        // GET: DepartmentEmployee/MyEmployees
        public IActionResult MyEmployees()
        {
            if (!CheckSOAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            var deptId = GetSODepartmentId();
            if (deptId == null)
            {
                TempData["Error"] = "Your account is not assigned to a department. Contact the administrator.";
                return RedirectToAction("Index", "Dashboard");
            }

            var employees = _context.DepartmentEmployees
                .Where(de => de.User!.DepartmentId == deptId)
                .OrderBy(de => de.Name)
                .ToList();

            ViewBag.DepartmentName = _context.Departments.Where(d => d.Id == deptId).Select(d => d.Name).FirstOrDefault();
            return View(employees);
        }

        // POST: DepartmentEmployee/AddMyEmployee (AJAX)
        [HttpPost]
        public IActionResult AddMyEmployee([FromBody] MyEmployeeDto dto)
        {
            if (!CheckSOAuthentication())
                return Json(new { success = false, message = "Unauthorized" });

            if (string.IsNullOrWhiteSpace(dto.Name))
                return Json(new { success = false, message = "Employee name is required." });

            var userId = HttpContext.Session.GetInt32("UserId")!.Value;
            var deptId = GetSODepartmentId();
            if (deptId == null)
                return Json(new { success = false, message = "Your account is not assigned to a department." });

            var employee = new DepartmentEmployee
            {
                Name = dto.Name.Trim(),
                IsHighPriority = false,
                UserId = userId,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.DepartmentEmployees.Add(employee);
            _context.SaveChanges();

            return Json(new { success = true, id = employee.Id, name = employee.Name, message = $"Employee '{employee.Name}' added successfully!" });
        }

        // POST: DepartmentEmployee/EditMyEmployee (AJAX)
        [HttpPost]
        public IActionResult EditMyEmployee([FromBody] EditMyEmployeeDto dto)
        {
            if (!CheckSOAuthentication())
                return Json(new { success = false, message = "Unauthorized" });

            var deptId = GetSODepartmentId();
            var employee = _context.DepartmentEmployees.Include(de => de.User).FirstOrDefault(de => de.Id == dto.Id && de.User!.DepartmentId == deptId);
            if (employee == null)
                return Json(new { success = false, message = "Employee not found." });

            if (string.IsNullOrWhiteSpace(dto.Name))
                return Json(new { success = false, message = "Employee name is required." });

            employee.Name = dto.Name.Trim();
            employee.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            return Json(new { success = true, message = $"Employee '{employee.Name}' updated successfully!" });
        }

        // POST: DepartmentEmployee/ToggleMyEmployeeActive/5 (AJAX)
        [HttpPost]
        public IActionResult ToggleMyEmployeeActive(int id)
        {
            if (!CheckSOAuthentication())
                return Json(new { success = false, message = "Unauthorized" });

            var deptId = GetSODepartmentId();
            var employee = _context.DepartmentEmployees.Include(de => de.User).FirstOrDefault(de => de.Id == id && de.User!.DepartmentId == deptId);
            if (employee == null)
                return Json(new { success = false, message = "Employee not found." });

            employee.IsActive = !employee.IsActive;
            employee.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            return Json(new { success = true, isActive = employee.IsActive, message = $"'{employee.Name}' {(employee.IsActive ? "activated" : "deactivated")}." });
        }

        // POST: DepartmentEmployee/DeleteMyEmployee/5 (AJAX)
        [HttpPost]
        public IActionResult DeleteMyEmployee(int id)
        {
            if (!CheckSOAuthentication())
                return Json(new { success = false, message = "Unauthorized" });

            var deptId = GetSODepartmentId();
            var employee = _context.DepartmentEmployees.Include(de => de.User).FirstOrDefault(de => de.Id == id && de.User!.DepartmentId == deptId);
            if (employee == null)
                return Json(new { success = false, message = "Employee not found." });

            var hasVisitors = _context.Visitors.Any(v => v.DepartmentEmployeeId == id);
            if (hasVisitors)
                return Json(new { success = false, message = $"Cannot delete '{employee.Name}'. Employee has visitor records. Deactivate instead." });

            _context.DepartmentEmployees.Remove(employee);
            _context.SaveChanges();

            return Json(new { success = true, message = $"Employee '{employee.Name}' deleted successfully." });
        }

        // ====== Shared helpers ======

        private void PopulateDropdowns(int? selectedUserId = null)
        {
            var soUsers = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Department)
                .Where(u => u.Role != null && u.Role.Name == "Section Officer")
                .OrderBy(u => u.Name)
                .Select(u => new { u.Id, Display = u.Name + (u.Department != null ? " (" + u.Department.Name + ")" : "") })
                .ToList();

            ViewBag.SOUsers = new SelectList(soUsers, "Id", "Display", selectedUserId);
        }

        public class CreateDeptEmployeeDto
        {
            public string Name { get; set; } = string.Empty;
            public int UserId { get; set; }
            public bool IsActive { get; set; } = true;
        }

        public class MyEmployeeDto
        {
            public string Name { get; set; } = string.Empty;
        }

        public class EditMyEmployeeDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisitorManagementSystemMoD.Models;
using VisitorManagementSystemMoD.Models.ViewModels;

namespace VisitorManagementSystemMoD.Controllers
{
    public class DepartmentManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentManagementController(ApplicationDbContext context)
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

        // GET: DepartmentManagement
        public IActionResult Index()
        {
            if (!CheckSuperAdminAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            var departments = _context.Departments
                .Include(d => d.Users)
                .Include(d => d.Visitors)
                .OrderBy(d => d.Name)
                .ToList();

            return View(departments);
        }

        // GET: DepartmentManagement/Create
        public IActionResult Create()
        {
            if (!CheckSuperAdminAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // POST: DepartmentManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DepartmentViewModel model)
        {
            if (!CheckSuperAdminAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if department name already exists
            if (_context.Departments.Any(d => d.Name == model.Name))
            {
                ModelState.AddModelError("Name", "Department name already exists");
                return View(model);
            }

            var department = new Department
            {
                Name = model.Name,
                Code = model.Code,
                Description = model.Description,
                CreatedAt = DateTime.Now
            };

            _context.Departments.Add(department);
            _context.SaveChanges();

            TempData["Success"] = $"Department '{department.Name}' created successfully!";
            return RedirectToAction("Index");
        }

        // GET: DepartmentManagement/Edit/5
        public IActionResult Edit(int id)
        {
            if (!CheckSuperAdminAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            var department = _context.Departments.Find(id);
            if (department == null)
            {
                TempData["Error"] = "Department not found";
                return RedirectToAction("Index");
            }

            var model = new DepartmentViewModel
            {
                Id = department.Id,
                Name = department.Name,
                Code = department.Code,
                Description = department.Description
            };

            return View(model);
        }

        // POST: DepartmentManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, DepartmentViewModel model)
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
                return View(model);
            }

            var department = _context.Departments.Find(id);
            if (department == null)
            {
                TempData["Error"] = "Department not found";
                return RedirectToAction("Index");
            }

            // Check if new name conflicts with existing department
            if (_context.Departments.Any(d => d.Name == model.Name && d.Id != id))
            {
                ModelState.AddModelError("Name", "Department name already exists");
                return View(model);
            }

            department.Name = model.Name;
            department.Code = model.Code;
            department.Description = model.Description;
            department.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            TempData["Success"] = $"Department '{department.Name}' updated successfully!";
            return RedirectToAction("Index");
        }

        // POST: DepartmentManagement/Delete/5
        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!CheckSuperAdminAuthentication())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var department = _context.Departments
                .Include(d => d.Users)
                .Include(d => d.Visitors)
                .FirstOrDefault(d => d.Id == id);

            if (department == null)
            {
                return Json(new { success = false, message = "Department not found" });
            }

            // Unassign department from users and visitors before deleting
            var userCount = department.Users.Count;
            var visitorCount = department.Visitors.Count;

            foreach (var user in department.Users)
            {
                user.DepartmentId = null;
            }

            foreach (var visitor in department.Visitors)
            {
                visitor.DepartmentId = null;
            }

            _context.Departments.Remove(department);
            _context.SaveChanges();

            var msg = $"Department '{department.Name}' deleted successfully";
            if (userCount > 0 || visitorCount > 0)
            {
                msg += $". {userCount} user(s) and {visitorCount} visitor(s) have been unassigned.";
            }

            return Json(new { success = true, message = msg });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisitorManagementSystemMoD.Models;
using VisitorManagementSystemMoD.Models.ViewModels;
using VisitorManagementSystemMoD.Constants;

namespace VisitorManagementSystemMoD.Controllers
{
    public class RoleManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoleManagementController(ApplicationDbContext context)
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

        // GET: RoleManagement
        public IActionResult Index()
        {
            if (!CheckSuperAdminAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            var roles = _context.Roles
                .Include(r => r.Users)
                .OrderBy(r => r.Name)
                .ToList();

            return View(roles);
        }

        // GET: RoleManagement/Create
        public IActionResult Create()
        {
            if (!CheckSuperAdminAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // POST: RoleManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RoleViewModel model)
        {
            if (!CheckSuperAdminAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if role name already exists
            if (_context.Roles.Any(r => r.Name == model.Name))
            {
                ModelState.AddModelError("Name", "Role name already exists");
                return View(model);
            }

            var role = new Role
            {
                Name = model.Name,
                CreatedAt = DateTime.Now
            };

            _context.Roles.Add(role);
            _context.SaveChanges();

            TempData["Success"] = $"Role '{role.Name}' created successfully!";
            return RedirectToAction("Index");
        }

        // POST: RoleManagement/CreateAjax
        [HttpPost]
        public IActionResult CreateAjax([FromBody] CreateRoleAjaxDto dto)
        {
            if (!CheckSuperAdminAuthentication())
                return Json(new { success = false, message = "Unauthorized" });

            if (string.IsNullOrWhiteSpace(dto.Name))
                return Json(new { success = false, message = "Role name is required." });

            if (dto.Name.Length > 50)
                return Json(new { success = false, message = "Role name cannot exceed 50 characters." });

            if (_context.Roles.Any(r => r.Name == dto.Name))
                return Json(new { success = false, message = "Role name already exists." });

            var role = new Role
            {
                Name = dto.Name,
                CreatedAt = DateTime.Now
            };

            _context.Roles.Add(role);
            _context.SaveChanges();

            return Json(new { success = true, message = $"Role '{role.Name}' created successfully!" });
        }

        public class CreateRoleAjaxDto
        {
            public string Name { get; set; } = string.Empty;
        }

        // GET: RoleManagement/Edit/5
        public IActionResult Edit(int id)
        {
            if (!CheckSuperAdminAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            var role = _context.Roles.Find(id);
            if (role == null)
            {
                TempData["Error"] = "Role not found";
                return RedirectToAction("Index");
            }

            // Prevent editing SuperAdmin role
            if (role.Name == "SuperAdmin")
            {
                TempData["Error"] = "Cannot edit SuperAdmin role";
                return RedirectToAction("Index");
            }

            var model = new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name
            };

            // Get user count for this role
            ViewBag.UserCount = _context.Users.Count(u => u.RoleId == role.Id);

            return View(model);
        }

        // POST: RoleManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, RoleViewModel model)
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

            var role = _context.Roles.Find(id);
            if (role == null)
            {
                TempData["Error"] = "Role not found";
                return RedirectToAction("Index");
            }

            // Prevent editing SuperAdmin role
            if (role.Name == "SuperAdmin")
            {
                TempData["Error"] = "Cannot edit SuperAdmin role";
                return RedirectToAction("Index");
            }

            // Check if new name conflicts with existing role
            if (_context.Roles.Any(r => r.Name == model.Name && r.Id != id))
            {
                ModelState.AddModelError("Name", "Role name already exists");
                ViewBag.UserCount = _context.Users.Count(u => u.RoleId == role.Id);
                return View(model);
            }

            role.Name = model.Name;
            role.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            TempData["Success"] = $"Role '{role.Name}' updated successfully!";
            return RedirectToAction("Index");
        }

        // POST: RoleManagement/Delete/5
        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!CheckSuperAdminAuthentication())
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var role = _context.Roles.Include(r => r.Users).FirstOrDefault(r => r.Id == id);
            if (role == null)
            {
                return Json(new { success = false, message = "Role not found" });
            }

            // Prevent deleting SuperAdmin role
            if (role.Name == "SuperAdmin")
            {
                return Json(new { success = false, message = "Cannot delete SuperAdmin role" });
            }

            // Check if role has users
            if (role.Users.Any())
            {
                return Json(new { success = false, message = $"Cannot delete role. {role.Users.Count} user(s) are assigned to this role." });
            }

            _context.Roles.Remove(role);
            _context.SaveChanges();

            return Json(new { success = true, message = $"Role '{role.Name}' deleted successfully" });
        }
    }
}

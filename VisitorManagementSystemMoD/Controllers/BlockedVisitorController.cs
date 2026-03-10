using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisitorManagementSystemMoD.Models;

namespace VisitorManagementSystemMoD.Controllers
{
    public class BlockedVisitorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlockedVisitorController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool CheckAuthentication()
        {
            return HttpContext.Session.GetInt32("UserId") != null;
        }

        private bool HasAccess()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "SuperAdmin" || role == "Admin" || role == "Security Officer" || role == "Reception";
        }

        private bool CanEdit()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "SuperAdmin" || role == "Admin" || role == "Security Officer";
        }

        public IActionResult Index()
        {
            if (!CheckAuthentication()) return RedirectToAction("Login", "Account");
            if (!HasAccess()) return RedirectToAction("Index", "Dashboard");

            var blockedVisitors = _context.BlockedVisitors
                .Include(b => b.BlockedBy)
                .OrderByDescending(b => b.CreatedAt)
                .ToList();

            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            return View(blockedVisitors);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!CheckAuthentication()) return RedirectToAction("Login", "Account");
            if (!CanEdit()) return RedirectToAction("Index");

            return View();
        }

        [HttpPost]
        public IActionResult Create(BlockedVisitor model)
        {
            if (!CheckAuthentication()) return RedirectToAction("Login", "Account");
            if (!CanEdit()) return RedirectToAction("Index");

            if (string.IsNullOrWhiteSpace(model.Name))
                ModelState.AddModelError("Name", "Name is required");
            if (string.IsNullOrWhiteSpace(model.CNIC))
                ModelState.AddModelError("CNIC", "CNIC is required");

            if (!ModelState.IsValid)
                return View(model);

            var existing = _context.BlockedVisitors.FirstOrDefault(b => b.CNIC == model.CNIC && b.IsActive);
            if (existing != null)
            {
                ModelState.AddModelError("CNIC", "This CNIC is already in the blocked list.");
                return View(model);
            }

            var userId = HttpContext.Session.GetInt32("UserId")!.Value;
            var userName = HttpContext.Session.GetString("UserName");

            model.BlockedById = userId;
            model.BlockedByName = userName ?? "";
            model.CreatedAt = DateTime.Now;
            model.IsActive = true;

            _context.BlockedVisitors.Add(model);
            _context.SaveChanges();

            TempData["Success"] = $"Visitor \"{model.Name}\" has been added to the blocked list.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!CheckAuthentication()) return RedirectToAction("Login", "Account");
            if (!CanEdit()) return RedirectToAction("Index");

            var blocked = _context.BlockedVisitors.Find(id);
            if (blocked == null)
            {
                TempData["Error"] = "Blocked visitor not found.";
                return RedirectToAction("Index");
            }

            return View(blocked);
        }

        [HttpPost]
        public IActionResult Edit(int id, BlockedVisitor model)
        {
            if (!CheckAuthentication()) return RedirectToAction("Login", "Account");
            if (!CanEdit()) return RedirectToAction("Index");

            if (string.IsNullOrWhiteSpace(model.Name))
                ModelState.AddModelError("Name", "Name is required");
            if (string.IsNullOrWhiteSpace(model.CNIC))
                ModelState.AddModelError("CNIC", "CNIC is required");

            if (!ModelState.IsValid)
                return View(model);

            var blocked = _context.BlockedVisitors.Find(id);
            if (blocked == null)
            {
                TempData["Error"] = "Blocked visitor not found.";
                return RedirectToAction("Index");
            }

            var duplicate = _context.BlockedVisitors.FirstOrDefault(b => b.CNIC == model.CNIC && b.IsActive && b.Id != id);
            if (duplicate != null)
            {
                ModelState.AddModelError("CNIC", "This CNIC is already in the blocked list.");
                return View(model);
            }

            blocked.Name = model.Name;
            blocked.CNIC = model.CNIC;
            blocked.Reason = model.Reason;
            blocked.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            TempData["Success"] = "Blocked visitor updated successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ToggleActive([FromBody] ToggleBlockedRequest request)
        {
            if (!CheckAuthentication()) return Json(new { success = false, message = "Not authenticated" });
            if (!CanEdit()) return Json(new { success = false, message = "Access denied" });

            var blocked = _context.BlockedVisitors.Find(request.Id);
            if (blocked == null) return Json(new { success = false, message = "Not found" });

            blocked.IsActive = !blocked.IsActive;
            blocked.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult Delete([FromBody] ToggleBlockedRequest request)
        {
            if (!CheckAuthentication()) return Json(new { success = false, message = "Not authenticated" });
            if (!CanEdit()) return Json(new { success = false, message = "Access denied" });

            var blocked = _context.BlockedVisitors.Find(request.Id);
            if (blocked == null) return Json(new { success = false, message = "Not found" });

            _context.BlockedVisitors.Remove(blocked);
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult CheckCNIC(string cnic)
        {
            if (!CheckAuthentication()) return Json(new { success = false });

            var blocked = _context.BlockedVisitors.FirstOrDefault(b => b.CNIC == cnic && b.IsActive);
            if (blocked != null)
            {
                return Json(new { success = true, isBlocked = true, name = blocked.Name, reason = blocked.Reason ?? "No reason provided." });
            }

            return Json(new { success = true, isBlocked = false });
        }
    }

    public class ToggleBlockedRequest
    {
        public int Id { get; set; }
    }
}

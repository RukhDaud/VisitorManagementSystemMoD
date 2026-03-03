using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisitorManagementSystemMoD.Models;

namespace VisitorManagementSystemMoD.Controllers
{
    public class AlertController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AlertController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool CheckAuthentication()
        {
            return HttpContext.Session.GetInt32("UserId") != null;
        }

        private bool IsAuthorized()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "SuperAdmin" || role == "Security Officer";
        }

        // GET: Alert/GetActiveAlerts (AJAX - for bell icon on all dashboards)
        [HttpGet]
        public IActionResult GetActiveAlerts()
        {
            if (!CheckAuthentication())
                return Json(new { success = false });

            var now = DateTime.Now;
            var rawAlerts = _context.Alerts
                .Where(a => a.IsActive && (!a.ExpiresAt.HasValue || a.ExpiresAt > now))
                .OrderByDescending(a => a.CreatedAt)
                .Take(10)
                .ToList();

            var alerts = rawAlerts
                .OrderByDescending(a => a.Priority == "Critical" ? 0 : a.Priority == "Warning" ? 1 : 2)
                .ThenByDescending(a => a.CreatedAt)
                .Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.Message,
                    a.Priority,
                    a.Category,
                    a.CreatedByName,
                    createdAt = a.CreatedAt.ToString("MMM dd, h:mm tt"),
                    timeAgo = GetTimeAgo(a.CreatedAt)
                })
                .ToList();

            var totalActive = _context.Alerts.Count(a => a.IsActive && (!a.ExpiresAt.HasValue || a.ExpiresAt > now));

            return Json(new { success = true, alerts, totalActive });
        }

        // GET: Alert/GetTodayAlerts (AJAX - for login popup)
        [HttpGet]
        public IActionResult GetTodayAlerts()
        {
            if (!CheckAuthentication())
                return Json(new { success = false });

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var now = DateTime.Now;
            var rawAlerts = _context.Alerts
                .Where(a => a.IsActive && a.CreatedAt >= today && a.CreatedAt < tomorrow && (!a.ExpiresAt.HasValue || a.ExpiresAt > now))
                .OrderByDescending(a => a.CreatedAt)
                .ToList();

            var alerts = rawAlerts
                .OrderByDescending(a => a.Priority == "Critical" ? 0 : a.Priority == "Warning" ? 1 : 2)
                .ThenByDescending(a => a.CreatedAt)
                .Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.Message,
                    a.Priority,
                    a.Category,
                    a.CreatedByName,
                    createdAt = a.CreatedAt.ToString("MMM dd, h:mm tt"),
                    timeAgo = GetTimeAgo(a.CreatedAt)
                })
                .ToList();

            return Json(new { success = true, alerts, count = alerts.Count });
        }

        // GET: Alert/GetAlertCount (AJAX - for badge count)
        [HttpGet]
        public IActionResult GetAlertCount()
        {
            if (!CheckAuthentication())
                return Json(new { count = 0 });

            var now = DateTime.Now;
            var count = _context.Alerts.Count(a => a.IsActive && (!a.ExpiresAt.HasValue || a.ExpiresAt > now));
            return Json(new { count });
        }

        // POST: Alert/Create (AJAX)
        [HttpPost]
        public IActionResult Create([FromBody] AlertCreateModel model)
        {
            if (!CheckAuthentication() || !IsAuthorized())
                return Json(new { success = false, message = "Unauthorized" });

            if (string.IsNullOrWhiteSpace(model.Title) || string.IsNullOrWhiteSpace(model.Message))
                return Json(new { success = false, message = "Title and Message are required" });

            var userId = HttpContext.Session.GetInt32("UserId")!.Value;
            var userName = HttpContext.Session.GetString("UserName") ?? "Unknown";

            var alert = new Alert
            {
                Title = model.Title,
                Message = model.Message,
                Priority = model.Priority ?? "Info",
                Category = model.Category ?? "General",
                IsActive = true,
                CreatedAt = DateTime.Now,
                ExpiresAt = model.ExpiresAt,
                CreatedById = userId,
                CreatedByName = userName
            };

            _context.Alerts.Add(alert);
            _context.SaveChanges();

            return Json(new
            {
                success = true,
                alert = new
                {
                    alert.Id,
                    alert.Title,
                    alert.Message,
                    alert.Priority,
                    alert.Category,
                    alert.CreatedByName,
                    createdAt = alert.CreatedAt.ToString("MMM dd, h:mm tt"),
                    timeAgo = "Just now"
                }
            });
        }

        // POST: Alert/Deactivate (AJAX)
        [HttpPost]
        public IActionResult Deactivate([FromBody] AlertIdModel model)
        {
            if (!CheckAuthentication() || !IsAuthorized())
                return Json(new { success = false, message = "Unauthorized" });

            var alert = _context.Alerts.Find(model.Id);
            if (alert == null)
                return Json(new { success = false, message = "Alert not found" });

            alert.IsActive = false;
            _context.SaveChanges();

            return Json(new { success = true });
        }

        // POST: Alert/Delete (AJAX)
        [HttpPost]
        public IActionResult Delete([FromBody] AlertIdModel model)
        {
            if (!CheckAuthentication() || !IsAuthorized())
                return Json(new { success = false, message = "Unauthorized" });

            var alert = _context.Alerts.Find(model.Id);
            if (alert == null)
                return Json(new { success = false, message = "Alert not found" });

            _context.Alerts.Remove(alert);
            _context.SaveChanges();

            return Json(new { success = true });
        }

        // GET: Alert/GetAllAlerts (AJAX - for management page)
        [HttpGet]
        public IActionResult GetAllAlerts()
        {
            if (!CheckAuthentication() || !IsAuthorized())
                return Json(new { success = false });

            var now = DateTime.Now;
            var rawAlerts = _context.Alerts
                .OrderByDescending(a => a.CreatedAt)
                .ToList();

            var alerts = rawAlerts
                .Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.Message,
                    a.Priority,
                    a.Category,
                    a.IsActive,
                    a.CreatedByName,
                    createdAt = a.CreatedAt.ToString("MMM dd, yyyy h:mm tt"),
                    expiresAt = a.ExpiresAt.HasValue ? a.ExpiresAt.Value.ToString("MMM dd, yyyy h:mm tt") : "Never",
                    isExpired = a.ExpiresAt.HasValue && a.ExpiresAt < now
                })
                .ToList();

            return Json(new { success = true, alerts });
        }

        private static string GetTimeAgo(DateTime dateTime)
        {
            var span = DateTime.Now - dateTime;
            if (span.TotalMinutes < 1) return "Just now";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes}m ago";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours}h ago";
            if (span.TotalDays < 7) return $"{(int)span.TotalDays}d ago";
            return dateTime.ToString("MMM dd");
        }
    }

    public class AlertCreateModel
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Priority { get; set; }
        public string? Category { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    public class AlertIdModel
    {
        public int Id { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisitorManagementSystemMoD.Models;
using VisitorManagementSystemMoD.Models.ViewModels;

namespace VisitorManagementSystemMoD.Controllers
{
    public class VisitorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VisitorController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool CheckAuthentication()
        {
            return HttpContext.Session.GetInt32("UserId") != null;
        }

        // Employee: Create Visitor Request (Bulk Entry)
        [HttpGet]
        public IActionResult Create()
        {
            if (!CheckAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Employee")
            {
                return RedirectToAction("Index", "Dashboard");
            }

            var model = new BulkVisitorViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(BulkVisitorViewModel model)
        {
            if (!CheckAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            // Remove empty visitor entries
            model.Visitors = model.Visitors.Where(v => !string.IsNullOrWhiteSpace(v.Name)).ToList();

            if (!model.Visitors.Any())
            {
                ModelState.AddModelError("", "Please add at least one visitor");
                return View(model);
            }

            // Validate each visitor
            bool hasErrors = false;
            for (int i = 0; i < model.Visitors.Count; i++)
            {
                var visitor = model.Visitors[i];

                if (string.IsNullOrWhiteSpace(visitor.Name))
                {
                    ModelState.AddModelError($"Visitors[{i}].Name", "Name is required");
                    hasErrors = true;
                }
                if (string.IsNullOrWhiteSpace(visitor.CNIC))
                {
                    ModelState.AddModelError($"Visitors[{i}].CNIC", "CNIC is required");
                    hasErrors = true;
                }
                if (string.IsNullOrWhiteSpace(visitor.Phone))
                {
                    ModelState.AddModelError($"Visitors[{i}].Phone", "Phone is required");
                    hasErrors = true;
                }
                if (string.IsNullOrWhiteSpace(visitor.Purpose))
                {
                    ModelState.AddModelError($"Visitors[{i}].Purpose", "Purpose is required");
                    hasErrors = true;
                }
                if (visitor.ExpectedTime == default)
                {
                    ModelState.AddModelError($"Visitors[{i}].ExpectedTime", "Expected time is required");
                    hasErrors = true;
                }
            }

            if (hasErrors)
            {
                return View(model);
            }

            var userId = HttpContext.Session.GetInt32("UserId")!.Value;
            var userName = HttpContext.Session.GetString("UserName");

            // Create all visitors
            foreach (var visitorModel in model.Visitors)
            {
                var visitor = new Visitor
                {
                    Name = visitorModel.Name,
                    CNIC = visitorModel.CNIC,
                    Phone = visitorModel.Phone,
                    Purpose = visitorModel.Purpose,
                    ExpectedTime = visitorModel.ExpectedTime,
                    HasVehicle = visitorModel.HasVehicle,
                    VehicleNumber = visitorModel.VehicleNumber,
                    VehicleType = visitorModel.VehicleType,
                    EmployeeId = userId,
                    EmployeeName = userName ?? "",
                    Status = "Pending",
                    RequestCreatedAt = DateTime.Now
                };

                _context.Visitors.Add(visitor);
            }

            _context.SaveChanges();

            TempData["Success"] = $"{model.Visitors.Count} visitor request(s) submitted for SCO approval!";
            return RedirectToAction("MyVisitors");
        }

        // Employee: View My Visitors
        public IActionResult MyVisitors(string status = "All", int entries = 10)
        {
            if (!CheckAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = HttpContext.Session.GetInt32("UserId")!.Value;
            var query = _context.Visitors
                .Include(v => v.Employee)
                .Include(v => v.ApprovedBy)
                .Where(v => v.EmployeeId == userId);

            if (status != "All")
            {
                query = query.Where(v => v.Status == status);
            }

            var allVisitors = query.OrderByDescending(v => v.RequestCreatedAt);
            var visitors = entries == -1 ? allVisitors.ToList() : allVisitors.Take(entries).ToList();

            ViewBag.Status = status;
            ViewBag.Entries = entries;
            ViewBag.TotalCount = query.Count();
            return View(visitors);
        }

        // Employee: Edit Visitor Request (Only before check-in)
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!CheckAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = HttpContext.Session.GetInt32("UserId")!.Value;
            var visitor = _context.Visitors.FirstOrDefault(v => v.Id == id && v.EmployeeId == userId);

            if (visitor == null)
            {
                TempData["Error"] = "Visitor not found or you don't have permission to edit.";
                return RedirectToAction("MyVisitors");
            }

            // Check if visitor has already checked in
            if (visitor.CheckInTime.HasValue)
            {
                TempData["Error"] = "Cannot edit visitor. The visitor is already on premises.";
                return RedirectToAction("MyVisitors");
            }

            // Map to ViewModel
            var model = new CreateVisitorViewModel
            {
                Name = visitor.Name,
                CNIC = visitor.CNIC,
                Phone = visitor.Phone,
                Purpose = visitor.Purpose,
                ExpectedTime = visitor.ExpectedTime,
                HasVehicle = visitor.HasVehicle,
                VehicleNumber = visitor.VehicleNumber,
                VehicleType = visitor.VehicleType
            };

            ViewBag.VisitorId = id;
            ViewBag.CurrentStatus = visitor.Status;
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(int id, CreateVisitorViewModel model)
        {
            if (!CheckAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.VisitorId = id;
                return View(model);
            }

            var userId = HttpContext.Session.GetInt32("UserId")!.Value;
            var visitor = _context.Visitors.FirstOrDefault(v => v.Id == id && v.EmployeeId == userId);

            if (visitor == null)
            {
                TempData["Error"] = "Visitor not found or you don't have permission to edit.";
                return RedirectToAction("MyVisitors");
            }

            // Check if visitor has already checked in
            if (visitor.CheckInTime.HasValue)
            {
                TempData["Error"] = "Cannot edit visitor. The visitor is already on premises.";
                return RedirectToAction("MyVisitors");
            }

            // Update visitor details
            visitor.Name = model.Name;
            visitor.CNIC = model.CNIC;
            visitor.Phone = model.Phone;
            visitor.Purpose = model.Purpose;
            visitor.ExpectedTime = model.ExpectedTime;
            visitor.HasVehicle = model.HasVehicle;
            visitor.VehicleNumber = model.VehicleNumber;
            visitor.VehicleType = model.VehicleType;
            visitor.UpdatedAt = DateTime.Now;

            // If the visitor was approved, reset to pending since details changed
            if (visitor.Status == "Approved")
            {
                visitor.Status = "Pending";
                visitor.ApprovedById = null;
                visitor.ApprovedByName = null;
                visitor.ApprovedAt = null;
                TempData["Info"] = "Visitor updated successfully. Request sent back to pending for SCO re-approval.";
            }
            else
            {
                TempData["Success"] = "Visitor updated successfully!";
            }

            _context.SaveChanges();
            return RedirectToAction("MyVisitors");
        }

        // Employee: Delete Visitor Request (Only before check-in)
        [HttpPost]
        public IActionResult Delete([FromBody] DeleteRequest request)
        {
            if (!CheckAuthentication())
            {
                return Json(new { success = false, message = "Not authenticated" });
            }

            if (request == null || request.Id <= 0)
            {
                return Json(new { success = false, message = "Invalid request" });
            }

            var userId = HttpContext.Session.GetInt32("UserId")!.Value;
            var visitor = _context.Visitors.FirstOrDefault(v => v.Id == request.Id && v.EmployeeId == userId);

            if (visitor == null)
            {
                return Json(new { success = false, message = "Visitor not found or you don't have permission to delete." });
            }

            // Check if visitor has already checked in
            if (visitor.CheckInTime.HasValue)
            {
                return Json(new { success = false, message = "Cannot delete visitor. The visitor is already on premises." });
            }

            _context.Visitors.Remove(visitor);
            _context.SaveChanges();

            return Json(new { success = true, message = "Visitor deleted successfully" });
        }

        // Security Officer: Pending Approvals
        public IActionResult PendingApprovals(int entries = 10)
        {
            if (!CheckAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Security Officer" && userRole != "Admin")
            {
                return RedirectToAction("Index", "Dashboard");
            }

            var query = _context.Visitors
                .Include(v => v.Employee)
                .Where(v => v.Status == "Pending")
                .OrderByDescending(v => v.RequestCreatedAt);

            var visitors = entries == -1 ? query.ToList() : query.Take(entries).ToList();

            ViewBag.Entries = entries;
            ViewBag.TotalCount = _context.Visitors.Count(v => v.Status == "Pending");
            return View(visitors);
        }

        // Security Officer: Approve Visitor
        [HttpPost]
        public IActionResult Approve([FromBody] ApproveRequest request)
        {
            if (!CheckAuthentication())
            {
                return Json(new { success = false, message = "Not authenticated" });
            }

            if (request == null || request.Id <= 0)
            {
                return Json(new { success = false, message = "Invalid request" });
            }

            var visitor = _context.Visitors.Find(request.Id);
            if (visitor == null)
            {
                return Json(new { success = false, message = "Visitor not found" });
            }

            var userId = HttpContext.Session.GetInt32("UserId")!.Value;
            var userName = HttpContext.Session.GetString("UserName");

            visitor.Status = "Approved";
            visitor.ApprovedById = userId;
            visitor.ApprovedByName = userName;
            visitor.ApprovedAt = DateTime.Now;
            visitor.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            return Json(new { success = true, message = "Visitor approved successfully" });
        }

        // Security Officer: Reject Visitor
        [HttpPost]
        public IActionResult Reject([FromBody] RejectRequest request)
        {
            if (!CheckAuthentication())
            {
                return Json(new { success = false, message = "Not authenticated" });
            }

            if (request == null || request.Id <= 0)
            {
                return Json(new { success = false, message = "Invalid request" });
            }

            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                return Json(new { success = false, message = "Rejection reason is required" });
            }

            var visitor = _context.Visitors.Find(request.Id);
            if (visitor == null)
            {
                return Json(new { success = false, message = "Visitor not found" });
            }

            visitor.Status = "Rejected";
            visitor.RejectionReason = request.Reason;
            visitor.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            return Json(new { success = true, message = "Visitor rejected" });
        }

        // All Visitors (Security Officer / Admin / SuperAdmin)
        public IActionResult AllVisitors(string status = "All", string? checkStatus = null, int entries = 10)
        {
            if (!CheckAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Security Officer" && userRole != "Admin" && userRole != "SuperAdmin")
            {
                return RedirectToAction("Index", "Dashboard");
            }

            var query = _context.Visitors
                .Include(v => v.Employee)
                .Include(v => v.ApprovedBy)
                .AsQueryable();

            // Filter by approval status
            if (status != "All")
            {
                query = query.Where(v => v.Status == status);
            }

            // Filter by check-in/out status
            if (!string.IsNullOrEmpty(checkStatus))
            {
                if (checkStatus == "CheckedIn")
                {
                    query = query.Where(v => v.CheckInTime.HasValue && !v.CheckOutTime.HasValue);
                }
                else if (checkStatus == "CheckedOut")
                {
                    query = query.Where(v => v.CheckOutTime.HasValue);
                }
                else if (checkStatus == "Pending")
                {
                    query = query.Where(v => !v.CheckInTime.HasValue && v.Status == "Approved");
                }
            }

            var allVisitors = query.OrderByDescending(v => v.RequestCreatedAt);
            var visitors = entries == -1 ? allVisitors.ToList() : allVisitors.Take(entries).ToList();

            ViewBag.Status = status;
            ViewBag.CheckStatus = checkStatus;
            ViewBag.Entries = entries;
            ViewBag.TotalCount = query.Count();
            return View(visitors);
        }

        // Visitors Inside (Currently Checked In)
        public IActionResult VisitorsInside()
        {
            if (!CheckAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Security Officer" && userRole != "Admin" && userRole != "SuperAdmin")
            {
                return RedirectToAction("Index", "Dashboard");
            }

            var visitors = _context.Visitors
                .Include(v => v.Employee)
                .Include(v => v.ApprovedBy)
                .Where(v => v.CheckInTime.HasValue && !v.CheckOutTime.HasValue)
                .OrderByDescending(v => v.CheckInTime)
                .ToList();

            return View(visitors);
        }

        // Visitors Checked Out
        public IActionResult VisitorsCheckedOut()
        {
            if (!CheckAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Security Officer" && userRole != "Admin" && userRole != "SuperAdmin")
            {
                return RedirectToAction("Index", "Dashboard");
            }

            var visitors = _context.Visitors
                .Include(v => v.Employee)
                .Include(v => v.ApprovedBy)
                .Where(v => v.CheckOutTime.HasValue)
                .OrderByDescending(v => v.CheckOutTime)
                .ToList();

            return View(visitors);
        }

        // Gate: View Assigned Visitors
        public IActionResult GateVisitors(string? checkStatus = null, int entries = 10)
        {
            if (!CheckAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Reception")
            {
                return RedirectToAction("Index", "Dashboard");
            }

            var query = _context.Visitors
                .Include(v => v.Employee)
                .Include(v => v.ApprovedBy)
                .Where(v => v.Status == "Approved");

            // Filter by check-in/out status
            if (!string.IsNullOrEmpty(checkStatus))
            {
                if (checkStatus == "CheckedIn")
                {
                    query = query.Where(v => v.CheckInTime.HasValue && !v.CheckOutTime.HasValue);
                }
                else if (checkStatus == "CheckedOut")
                {
                    query = query.Where(v => v.CheckOutTime.HasValue);
                }
                else if (checkStatus == "Pending")
                {
                    query = query.Where(v => !v.CheckInTime.HasValue);
                }
            }

            var allVisitors = query.OrderByDescending(v => v.ExpectedTime);
            var visitors = entries == -1 ? allVisitors.ToList() : allVisitors.Take(entries).ToList();

            ViewBag.GateName = "Reception";
            ViewBag.CheckStatus = checkStatus;
            return View(visitors);
        }

        // Gate: Check In
        [HttpPost]
        public IActionResult CheckIn([FromBody] CheckInRequest request)
        {
            if (!CheckAuthentication())
            {
                return Json(new { success = false, message = "Not authenticated" });
            }

            if (request == null || request.Id <= 0)
            {
                return Json(new { success = false, message = "Invalid request" });
            }

            var visitor = _context.Visitors.Find(request.Id);
            if (visitor == null)
            {
                return Json(new { success = false, message = "Visitor not found" });
            }

            if (visitor.CheckInTime.HasValue)
            {
                return Json(new { success = false, message = "Visitor already checked in" });
            }

            visitor.CheckInTime = DateTime.Now;
            visitor.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            return Json(new { success = true, message = "Visitor checked in successfully" });
        }

        // Gate: Check Out
        [HttpPost]
        public IActionResult CheckOut([FromBody] CheckOutRequest request)
        {
            if (!CheckAuthentication())
            {
                return Json(new { success = false, message = "Not authenticated" });
            }

            if (request == null || request.Id <= 0)
            {
                return Json(new { success = false, message = "Invalid request" });
            }

            var visitor = _context.Visitors.Find(request.Id);
            if (visitor == null)
            {
                return Json(new { success = false, message = "Visitor not found" });
            }

            if (!visitor.CheckInTime.HasValue)
            {
                return Json(new { success = false, message = "Visitor must be checked in first" });
            }

            if (visitor.CheckOutTime.HasValue)
            {
                return Json(new { success = false, message = "Visitor already checked out" });
            }

            visitor.CheckOutTime = DateTime.Now;
            visitor.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            return Json(new { success = true, message = "Visitor checked out successfully" });
        }

        // Get Visitor Details (for modal)
        public IActionResult Details(int id)
        {
            try
            {
                if (!CheckAuthentication())
                {
                    return Json(new { success = false, message = "Not authenticated. Please login again." });
                }

                if (id <= 0)
                {
                    return Json(new { success = false, message = "Invalid visitor ID" });
                }

                var visitor = _context.Visitors
                    .Include(v => v.Employee)
                    .Include(v => v.ApprovedBy)
                    .FirstOrDefault(v => v.Id == id);

                if (visitor == null)
                {
                    return Json(new { success = false, message = "Visitor not found" });
                }

                // Use ISO 8601 format for better JavaScript compatibility
                return Json(new
                {
                    success = true,
                    visitor = new
                    {
                        visitor.Id,
                        visitor.Name,
                        CNIC = visitor.CNIC ?? "",
                        Phone = visitor.Phone ?? "",
                        Purpose = visitor.Purpose ?? "",
                        ExpectedTime = visitor.ExpectedTime.ToString("o"), // ISO 8601 format
                        visitor.HasVehicle,
                        VehicleNumber = visitor.VehicleNumber ?? "",
                        VehicleType = visitor.VehicleType ?? "",
                        Status = visitor.Status ?? "Pending",
                        EmployeeName = visitor.EmployeeName ?? "",
                        ApprovedByName = visitor.ApprovedByName ?? "",
                        ApprovedAt = visitor.ApprovedAt?.ToString("o"), // ISO 8601 format
                        RejectionReason = visitor.RejectionReason ?? "",
                        CheckInTime = visitor.CheckInTime?.ToString("o"), // ISO 8601 format
                        CheckOutTime = visitor.CheckOutTime?.ToString("o"), // ISO 8601 format
                        RequestCreatedAt = visitor.RequestCreatedAt.ToString("o") // ISO 8601 format
                    }
                });
            }
            catch (Exception ex)
            {
                // Log the error (you can add proper logging here)
                return Json(new { success = false, message = $"Server error: {ex.Message}" });
            }
        }

        // Reports: Visitors Inside After Office Hours
        public IActionResult VisitorsInsideReport(DateTime? startDate, DateTime? endDate)
        {
            if (!CheckAuthentication())
            {
                return RedirectToAction("Login", "Account");
            }

            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Security Officer" && userRole != "Admin")
            {
                return RedirectToAction("Index", "Dashboard");
            }

            var query = _context.Visitors
                .Include(v => v.Employee)
                .Include(v => v.ApprovedBy)
                .Where(v => v.CheckInTime.HasValue)
                .AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(v => v.CheckInTime >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(v => v.CheckInTime <= endDate.Value);
            }

            // Visitors inside after 4 PM (16:00)
            var visitors = query.ToList()
                .Where(v =>
                {
                    if (!v.CheckOutTime.HasValue)
                    {
                        // Still inside
                        return v.CheckInTime!.Value.TimeOfDay < new TimeSpan(16, 0, 0);
                    }
                    else
                    {
                        // Checked out after 4 PM
                        return v.CheckOutTime.Value.TimeOfDay > new TimeSpan(16, 0, 0);
                    }
                })
                .OrderByDescending(v => v.CheckInTime)
                .ToList();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(visitors);
        }

        // Get Visitor Details (AJAX)
        [HttpGet]
        public IActionResult GetVisitorDetails(int id)
        {
            if (!CheckAuthentication())
            {
                return Json(new { success = false, message = "Not authenticated" });
            }

            var visitor = _context.Visitors
                .Include(v => v.Employee)
                .Include(v => v.ApprovedBy)
                .Include(v => v.Department)
                .FirstOrDefault(v => v.Id == id);

            if (visitor == null)
            {
                return Json(new { success = false, message = "Visitor not found" });
            }

            var visitorData = new
            {
                name = visitor.Name,
                cnic = visitor.CNIC,
                phone = visitor.Phone,
                purpose = visitor.Purpose,
                employeeName = visitor.EmployeeName,
                departmentName = visitor.Department?.Name ?? "N/A",
                expectedTime = visitor.ExpectedTime.ToString("MMM dd, yyyy h:mm tt"),
                status = visitor.Status,
                approvedByName = visitor.ApprovedByName ?? "N/A",
                approvedAt = visitor.ApprovedAt?.ToString("MMM dd, yyyy h:mm tt") ?? "N/A",
                rejectionReason = visitor.RejectionReason,
                checkInTime = visitor.CheckInTime?.ToString("MMM dd, yyyy h:mm tt"),
                checkOutTime = visitor.CheckOutTime?.ToString("MMM dd, yyyy h:mm tt"),
                hasVehicle = visitor.HasVehicle,
                vehicleNumber = visitor.VehicleNumber,
                vehicleType = visitor.VehicleType
            };

            return Json(new { success = true, data = visitorData });
        }
    }

    // Request models for JSON body binding
    public class ApproveRequest
    {
        public int Id { get; set; }
    }

    public class RejectRequest
    {
        public int Id { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class DeleteRequest
    {
        public int Id { get; set; }
    }

    public class CheckInRequest
    {
        public int Id { get; set; }
    }

    public class CheckOutRequest
    {
        public int Id { get; set; }
    }
}

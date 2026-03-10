using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisitorManagementSystemMoD.Models
{
    public class Visitor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "CNIC")]
        public string? CNIC { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(500)]
        public string? Purpose { get; set; }

        [Required]
        public DateTime ExpectedTime { get; set; }

        public bool HasVehicle { get; set; } = false;

        [StringLength(50)]
        public string? VehicleNumber { get; set; }

        [StringLength(50)]
        public string? VehicleType { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        // Foreign key for Employee who created the request
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public User? Employee { get; set; }

        [StringLength(100)]
        public string EmployeeName { get; set; } = string.Empty;

        // Department being visited
        public int? DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public Department? Department { get; set; }

        // Department Employee on whose behalf the request is created (SO flow)
        public int? DepartmentEmployeeId { get; set; }

        [ForeignKey(nameof(DepartmentEmployeeId))]
        public DepartmentEmployee? DepartmentEmployee { get; set; }

        // Security Officer approval info
        public int? ApprovedById { get; set; }

        [ForeignKey("ApprovedById")]
        public User? ApprovedBy { get; set; }

        [StringLength(100)]
        public string? ApprovedByName { get; set; }

        public DateTime? ApprovedAt { get; set; }

        [StringLength(500)]
        public string? RejectionReason { get; set; }

        // Check-in/Check-out info
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }

        // Urgency flag (set when employee's role is high priority)
        public bool IsUrgent { get; set; } = false;

        // Timestamps
        public DateTime RequestCreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }
}

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

        [Required]
        [StringLength(20)]
        [Display(Name = "CNIC")]
        public string CNIC { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Purpose { get; set; } = string.Empty;

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

        // Timestamps
        public DateTime RequestCreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }
}

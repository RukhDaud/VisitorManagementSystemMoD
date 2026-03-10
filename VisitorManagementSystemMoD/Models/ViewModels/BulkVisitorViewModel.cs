using System.ComponentModel.DataAnnotations;

namespace VisitorManagementSystemMoD.Models.ViewModels
{
    public class BulkVisitorViewModel
    {
        public List<VisitorEntryViewModel> Visitors { get; set; } = new List<VisitorEntryViewModel>
        {
            new VisitorEntryViewModel() // Start with one empty entry
        };
    }

    public class VisitorEntryViewModel
    {
        [Required(ErrorMessage = "Visitor name is required")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(15, MinimumLength = 15, ErrorMessage = "CNIC must be in format XXXXX-XXXXXXX-X (15 characters)")]
        [RegularExpression(@"^\d{5}-\d{7}-\d{1}$", ErrorMessage = "CNIC must be in format XXXXX-XXXXXXX-X (e.g., 42101-1234567-1)")]
        public string? CNIC { get; set; }

        [StringLength(20, MinimumLength = 11, ErrorMessage = "Phone number must be at least 11 characters")]
        [RegularExpression(@"^(\+92[\s-]?)?0?3\d{2}[\s-]?\d{7}$", ErrorMessage = "Phone must be a valid Pakistani number (e.g., +92 300-1234567 or 03001234567)")]
        public string? Phone { get; set; }

        [StringLength(500)]
        public string? Purpose { get; set; }

        public DateTime ExpectedTime { get; set; } = DateTime.Now.AddHours(1);

        public bool HasVehicle { get; set; } = false;

        [StringLength(50)]
        public string? VehicleNumber { get; set; }

        [StringLength(50)]
        public string? VehicleType { get; set; }

        public int? DepartmentId { get; set; }

        // Department Employee on whose behalf the request is created (SO flow)
        public int? DepartmentEmployeeId { get; set; }
    }
}

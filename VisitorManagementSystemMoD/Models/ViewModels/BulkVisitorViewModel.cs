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

        [Required(ErrorMessage = "CNIC is required")]
        [StringLength(20)]
        public string CNIC { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Purpose is required")]
        [StringLength(500)]
        public string Purpose { get; set; } = string.Empty;

        [Required(ErrorMessage = "Expected time is required")]
        public DateTime ExpectedTime { get; set; } = DateTime.Now.AddHours(1);

        public bool HasVehicle { get; set; } = false;

        [StringLength(50)]
        public string? VehicleNumber { get; set; }

        [StringLength(50)]
        public string? VehicleType { get; set; }
    }
}

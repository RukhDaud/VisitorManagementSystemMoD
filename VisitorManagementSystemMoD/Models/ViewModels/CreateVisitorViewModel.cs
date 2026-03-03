using System.ComponentModel.DataAnnotations;

namespace VisitorManagementSystemMoD.Models.ViewModels
{
    public class CreateVisitorViewModel
    {
        [Required(ErrorMessage = "Visitor name is required")]
        [StringLength(100)]
        [Display(Name = "Visitor Full Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "CNIC is required")]
        [StringLength(20)]
        [Display(Name = "CNIC")]
        public string CNIC { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Purpose is required")]
        [StringLength(500)]
        [Display(Name = "Purpose of Visit")]
        public string Purpose { get; set; } = string.Empty;

        [Required(ErrorMessage = "Expected time is required")]
        [Display(Name = "Expected Visit Time")]
        public DateTime ExpectedTime { get; set; }

        [Display(Name = "Has Vehicle")]
        public bool HasVehicle { get; set; } = false;

        [StringLength(50)]
        [Display(Name = "Vehicle Number")]
        public string? VehicleNumber { get; set; }

        [StringLength(50)]
        [Display(Name = "Vehicle Type")]
        public string? VehicleType { get; set; }
    }
}

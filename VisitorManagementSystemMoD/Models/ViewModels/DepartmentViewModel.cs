using System.ComponentModel.DataAnnotations;

namespace VisitorManagementSystemMoD.Models.ViewModels
{
    public class DepartmentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Department name is required")]
        [StringLength(100, ErrorMessage = "Department name cannot exceed 100 characters")]
        [Display(Name = "Department Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Code cannot exceed 50 characters")]
        [Display(Name = "Department Code")]
        public string? Code { get; set; }

        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}

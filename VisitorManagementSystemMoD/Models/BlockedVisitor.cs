using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisitorManagementSystemMoD.Models
{
    public class BlockedVisitor
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

        [StringLength(500)]
        public string? Reason { get; set; }

        public int BlockedById { get; set; }

        [ForeignKey("BlockedById")]
        public User? BlockedBy { get; set; }

        [StringLength(100)]
        public string BlockedByName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

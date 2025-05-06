using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesApp.Models
{
    public class Classes
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? ClassName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int StudentCount { get; set; }

        public string? Description { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
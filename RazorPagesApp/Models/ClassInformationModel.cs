using System.ComponentModel.DataAnnotations;

namespace RazorPagesApp.Models
{
    public class ClassInformationModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Sınıf Adı gereklidir.")]
        public string? ClassName { get; set; }

        [Required(ErrorMessage = "Öğrenci Sayısı gereklidir.")]
        [Range(1, 100, ErrorMessage = "Öğrenci sayısı 1 ile 100 arasında olmalıdır.")]
        public int StudentCount { get; set; }

        public string? Description { get; set; }
    }
}
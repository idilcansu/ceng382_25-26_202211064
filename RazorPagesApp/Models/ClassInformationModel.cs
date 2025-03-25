// Models/ClassInformationModel.cs
using System.ComponentModel.DataAnnotations;

namespace YourProjectName.Models
{
    public class ClassInformationModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Class Name is required.")]
        public string ClassName { get; set; }

        [Required(ErrorMessage = "Student Count is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Student Count must be a non-negative number.")]
        public int StudentCount { get; set; }

        public string Description { get; set; }
    }
}
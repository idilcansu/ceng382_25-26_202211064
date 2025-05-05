namespace MyRazorApp.Models
{
    public class ClassInformationTable
    {
        public int Id { get; set; } // Used for edit/delete, not shown in the UI
        public string? ClassName { get; set; }
        public int StudentCount { get; set; }
        public string? Description { get; set; }

        public bool IsActive { get; set; }

         
    }
}
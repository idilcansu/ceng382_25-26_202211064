using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace RazorPagesApp.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public ClassInformationModel ClassInfo { get; set; } = new ClassInformationModel();

        public static List<ClassInformationModel> ClassList { get; set; } = new();
        private static int _idCounter = 1;

        // Add class data
        public void OnPost()
        {
            // Ensure the form data is valid
            if (ModelState.IsValid)
            {
                ClassInfo.Id = _idCounter++; // Assign unique ID
                ClassList.Add(ClassInfo);
                ClassInfo = new ClassInformationModel(); // Reset form
            }
        }

        // Delete class data
        public IActionResult OnPostDelete(int id)
        {
            var item = ClassList.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                ClassList.Remove(item);
            }
            return RedirectToPage(); // Refresh the page
        }

        // Edit class data
        public IActionResult OnPostEdit(int id)
        {
            var item = ClassList.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                ClassInfo = item; // Prefill form with selected class data
            }
            return Page(); // Stay on the same page for editing
        }
    }
}

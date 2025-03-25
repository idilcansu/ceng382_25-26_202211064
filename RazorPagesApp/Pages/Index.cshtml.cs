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
        private static int _idCounter = 1; // Benzersiz ID'ler için sayaç

        public void OnPost()
        {
            if (!string.IsNullOrEmpty(ClassInfo.ClassName))
            {
                // Yeni bir ID oluşturuyoruz
                ClassInfo.Id = _idCounter++;

                // Sınıf bilgisini listeye ekliyoruz
                ClassList.Add(new ClassInformationModel
                {
                    Id = ClassInfo.Id,
                    ClassName = ClassInfo.ClassName,
                    StudentCount = ClassInfo.StudentCount,
                    Description = ClassInfo.Description
                });
            }
        }

        public IActionResult OnPostDelete(int id)
        {
            var item = ClassList.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                ClassList.Remove(item);
            }
            return RedirectToPage();
        }

        public IActionResult OnPostAdd(int id)
        {
            var item = ClassList.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                // Aynı sınıfı tekrar listeye ekliyoruz ve yeni bir ID oluşturuyoruz
                ClassList.Add(new ClassInformationModel
                {
                    Id = _idCounter++, // Yeni bir ID
                    ClassName = item.ClassName,
                    StudentCount = item.StudentCount,
                    Description = item.Description
                });
            }
            return RedirectToPage();
        }
    }
}
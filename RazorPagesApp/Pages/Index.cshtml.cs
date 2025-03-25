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

        public void OnGet(int? id)
        {
            if (id.HasValue)
            {
                // Düzenleme için sınıfı bul ve ClassInfo'ya yükle
                var item = ClassList.FirstOrDefault(x => x.Id == id);
                if (item != null)
                {
                    ClassInfo = item;
                }
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (ClassInfo.Id == 0)
            {
                // Yeni kayıt
                ClassInfo.Id = _idCounter++;
                ClassList.Add(ClassInfo);
            }
            else
            {
                // Düzenleme
                var item = ClassList.FirstOrDefault(x => x.Id == ClassInfo.Id);
                if (item != null)
                {
                    item.ClassName = ClassInfo.ClassName;
                    item.StudentCount = ClassInfo.StudentCount;
                    item.Description = ClassInfo.Description;
                }
            }

            return RedirectToPage();
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
                ClassList.Add(new ClassInformationModel
                {
                    Id = _idCounter++,
                    ClassName = item.ClassName,
                    StudentCount = item.StudentCount,
                    Description = item.Description
                });
            }
            return RedirectToPage();
        }
    }
}
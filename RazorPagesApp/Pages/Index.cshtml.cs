using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using MyRazorApp.Helpers;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;


// --- Page Model Definition ---
namespace MyRazorApp.Pages
{
    using MyRazorApp.Models;

    public class IndexModel : PageModel
    {
        private readonly IWebHostEnvironment _env;

    public IndexModel(IWebHostEnvironment env)
    {
        _env = env;
    }
        private static List<ClassInformationModel> ClassList = new();
        [BindProperty]
        public ClassInformationModel ClassInfo { get; set; } = new();

        [BindProperty]
        public int? EditId { get; set; }

        // --- Filtering and Pagination Properties ---
        [BindProperty(SupportsGet = true)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Filter { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages => (TotalItems + PageSize - 1) / PageSize;

        public List<ClassInformationTable> DisplayList { get; set; } = new();

        public IActionResult OnPostExportJson(string selectedColumns = "")
    {
        try
        {
            var data = GetFilteredData();
            var columns = string.IsNullOrEmpty(selectedColumns) 
                ? new List<string>() 
                : selectedColumns.Split(',').ToList();

            string json = Utils.Instance.ExportToJson(data, columns);
            
            // Create exports directory if it doesn't exist
            var exportDir = Path.Combine(_env.ContentRootPath, "Exports");
            Directory.CreateDirectory(exportDir);

            // Create filename with timestamp
            var fileName = $"class-export-{DateTime.Now:yyyyMMdd-HHmmss}.json";
            var filePath = Path.Combine(exportDir, fileName);

            // Write to file
            System.IO.File.WriteAllText(filePath, json);

            TempData["SuccessMessage"] = $"File exported successfully to Exports folder.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error exporting file: {ex.Message}";
        }

        return RedirectToPage(new { Filter, PageNumber });
    }
        public void OnGet()
        {
            if (!ClassList.Any())
            {
                GenerateSyntheticData();
            }
            UpdateDisplayList();

            if (!EditId.HasValue)
            {
                if (ClassInfo == null || ClassInfo.Id == 0) {
                     ClassInfo = new ClassInformationModel();
                     ModelState.Clear();
                }
            }
            else
            {
                 if (ClassInfo == null || ClassInfo.Id != EditId.Value)
                 {
                     var classToEdit = ClassList.FirstOrDefault(c => c.Id == EditId.Value);
                     if (classToEdit != null)
                     {
                         ClassInfo = classToEdit;
                     }
                     else
                     {
                         TempData["ErrorMessage"] = "The item you were trying to edit could not be found.";
                         EditId = null;
                         ClassInfo = new ClassInformationModel();
                     }
                 }
            }
        }

        // --- POST Handlers ---
        public IActionResult OnPostAdd()
        {

            if (!ModelState.IsValid)
            {
                UpdateDisplayList();
                return Page();
            }

            bool isUpdate = EditId.HasValue;

            if (isUpdate)
            {
                var existing = ClassList.FirstOrDefault(c => c.Id == EditId.Value);
                if (existing != null)
                {
                    existing.ClassName = ClassInfo.ClassName;
                    existing.StudentCount = ClassInfo.StudentCount;
                    existing.Description = ClassInfo.Description;
                    TempData["SuccessMessage"] = "Class updated successfully.";
                }
                else
                {
                     ModelState.AddModelError(string.Empty, "The item you were trying to edit could not be found. It might have been deleted.");
                     UpdateDisplayList();
                     return Page();
                }
                EditId = null;
            }
            else // Add new item
            {
                int newId = ClassList.Any() ? ClassList.Max(c => c.Id) + 1 : 1;
                var newClass = new ClassInformationModel
                {
                    Id = newId,
                    ClassName = ClassInfo.ClassName,
                    StudentCount = ClassInfo.StudentCount,
                    Description = ClassInfo.Description
                };
                ClassList.Add(newClass);
                TempData["SuccessMessage"] = "Class added successfully.";
            }

            ClassInfo = new ClassInformationModel();
            ModelState.Clear();

            string currentFilter = this.Filter ?? string.Empty;
            int currentPage = this.PageNumber;

            return RedirectToPage(new { Filter = currentFilter, PageNumber = currentPage });
        }

        public IActionResult OnPostEdit(int id)
        {
            this.Filter ??= string.Empty;

            var classToEdit = ClassList.FirstOrDefault(c => c.Id == id);
            if (classToEdit != null)
            {
                ClassInfo = classToEdit; 
                EditId = id; 
            }
            else
            {
                TempData["ErrorMessage"] = "The item you tried to edit was not found.";
                string currentFilter = this.Filter ?? string.Empty;
                int currentPage = this.PageNumber;
  
                return RedirectToPage(new { Filter = currentFilter, PageNumber = currentPage });
            }

            UpdateDisplayList();
            return Page();
        }

        public IActionResult OnPostDelete(int id)
        {
            this.Filter ??= string.Empty;

            var classToDelete = ClassList.FirstOrDefault(c => c.Id == id);
            if (classToDelete != null)
            {
                ClassList.Remove(classToDelete);
                TempData["SuccessMessage"] = "Class deleted successfully.";
            }
            else
            {
                 TempData["ErrorMessage"] = "The item you tried to delete was not found.";
            }

            UpdateDisplayList();

            int pageNum = this.PageNumber;
            if (pageNum > TotalPages && TotalPages > 0)
            {
                pageNum = TotalPages;
            }
            else if (TotalPages == 0)
            {
                pageNum = 1;
            }


            string currentFilter = this.Filter ?? string.Empty;
            return RedirectToPage(new { Filter = currentFilter, PageNumber = pageNum });
        }

        // --- Helper Methods ---
        private void GenerateSyntheticData()
        {
            ClassList = new List<ClassInformationModel>();
            for (int i = 1; i <= 105; i++)
            {
                ClassList.Add(new ClassInformationModel
                {
                    Id = i,
                    ClassName = $"Class {i:000}",
                    StudentCount = (i % 15) + 5,
                    Description = $"Description for Class {i:000}"
                });
            }
        }

        private void UpdateDisplayList()
        {
            string currentFilter = this.Filter ?? string.Empty;

            IQueryable<ClassInformationModel> query = ClassList.AsQueryable();

            if (!string.IsNullOrWhiteSpace(currentFilter))
            {
                string lowerFilter = currentFilter.ToLowerInvariant();
                query = query.Where(c =>
                    (c.ClassName != null && c.ClassName.ToLowerInvariant().Contains(lowerFilter)) ||
                    (c.Description != null && c.Description.ToLowerInvariant().Contains(lowerFilter))
                );
            }

            TotalItems = query.Count();
            query = query.OrderBy(c => c.Id);

            // Apply pagination
            List<ClassInformationModel> pagedList = query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();


            DisplayList = pagedList.Select(c => new ClassInformationTable
            {
                Id = c.Id,
                ClassName = c.ClassName,
                StudentCount = c.StudentCount,
                Description = c.Description
            }).ToList();
        }

        private List<ClassInformationModel> GetFilteredData()
        {
            IQueryable<ClassInformationModel> query = ClassList.AsQueryable();

            if (!string.IsNullOrWhiteSpace(Filter))
            {
                string lowerFilter = Filter.ToLowerInvariant();
                query = query.Where(c =>
                    (c.ClassName != null && c.ClassName.ToLowerInvariant().Contains(lowerFilter)) ||
                    (c.Description != null && c.Description.ToLowerInvariant().Contains(lowerFilter))
                );
            }

            return query.ToList();
        }
    }
}
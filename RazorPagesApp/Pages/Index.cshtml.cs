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
using Microsoft.EntityFrameworkCore;
 
using MyRazorApp.Models;
using RazorPagesApp.Data;
 
namespace MyRazorApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        private readonly SchoolDbContext _context;
 
        public IndexModel(SchoolDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
 
        private static List<ClassInformationModel> ClassList = new();
 
        [BindProperty]
        public ClassInformationModel ClassInfo { get; set; } = new();
 
        [BindProperty]
        public int? EditId { get; set; }
 
        [BindProperty(SupportsGet = true)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Filter { get; set; } = string.Empty;
 
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
 
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages => (TotalItems + PageSize - 1) / PageSize;
 
        public List<ClassInformationTable> DisplayList { get; set; } = new();
 
        public async Task<IActionResult> OnGetAsync()
        {
            if (!IsAuthenticated())
                return RedirectToPage("Login");
 
            if (!ClassList.Any())
            {
                await GenerateSyntheticDataAsync(); // Artık veritabanından çekecek
            }
 
            UpdateDisplayList();
 
            if (!EditId.HasValue)
            {
                if (ClassInfo == null || ClassInfo.Id == 0)
                {
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
 
            return Page();
        }
 
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
                var existing = ClassList.FirstOrDefault(c => c.Id == EditId.GetValueOrDefault());
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
            else
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
 
            return RedirectToPage(new { Filter, PageNumber });
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
                return RedirectToPage(new { Filter, PageNumber });
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
 
            return RedirectToPage(new { Filter, PageNumber = pageNum });
        }
 
        // --- Helper Methods ---
 
        private async Task GenerateSyntheticDataAsync()
        {
            var classData = await _context.Classes.ToListAsync();
 
            ClassList = classData.Select(c => new ClassInformationModel
            {
                Id = c.Id,
                ClassName = c.Name,
                StudentCount = c.PersonCount,
                Description = c.Description,
                IsActive = c.IsActive
            }).ToList();
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
 
            var pagedList = query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();
 
            DisplayList = pagedList.Select(c => new ClassInformationTable
            {
                Id = c.Id,
                ClassName = c.ClassName,
                StudentCount = c.StudentCount,
                Description = c.Description,
                IsActive = c.IsActive
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
 
        private bool IsAuthenticated()
        {
            var sessionUsername = HttpContext.Session.GetString("username");
            var cookieUsername = Request.Cookies["username"];
            var sessionToken = HttpContext.Session.GetString("token");
            var cookieToken = Request.Cookies["token"];
            var sessionId = HttpContext.Session.GetString("session_id");
            var cookieSessionId = Request.Cookies["session_id"];
 
            return sessionUsername != null &&
                   cookieUsername == sessionUsername &&
                   cookieToken == sessionToken &&
                   cookieSessionId == sessionId;
        }
    }
}

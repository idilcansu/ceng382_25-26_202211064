using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using RazorPagesApp.Models;
using RazorPagesApp.Data;
using RazorPagesApp.Helpers;

namespace RazorPagesApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        private readonly SchoolDbContext _context;

        public IndexModel(IWebHostEnvironment env, SchoolDbContext context)
        {
            _env = env;
            _context = context;
        }

        [BindProperty]
        public ClassInformationModel ClassInfo { get; set; } = new();

        [BindProperty]
        public int? EditId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Filter { get; set; } = string.Empty;

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

            await UpdateDisplayListAsync();

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
                var classToEdit = await _context.Classes.FindAsync(EditId.Value);
                if (classToEdit != null)
                {
                    ClassInfo = new ClassInformationModel
                    {
                        Id = classToEdit.Id,
                        ClassName = classToEdit.ClassName,
                        StudentCount = classToEdit.StudentCount,
                        Description = classToEdit.Description
                    };
                }
                else
                {
                    TempData["ErrorMessage"] = "The item you were trying to edit could not be found.";
                    EditId = null;
                    ClassInfo = new ClassInformationModel();
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!ModelState.IsValid)
            {
                await UpdateDisplayListAsync();
                return Page();
            }

            bool isUpdate = EditId.HasValue;

            if (isUpdate)
            {
                var existing = await _context.Classes.FindAsync(EditId.Value);
                if (existing != null)
                {
                    existing.ClassName = ClassInfo.ClassName;
                    existing.StudentCount = ClassInfo.StudentCount;
                    existing.Description = ClassInfo.Description;
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Class updated successfully.";
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "The item you were trying to edit could not be found.");
                    await UpdateDisplayListAsync();
                    return Page();
                }

                EditId = null;
            }
            else
            {
                var newClass = new Classes
                {
                    ClassName = ClassInfo.ClassName,
                    StudentCount = ClassInfo.StudentCount,
                    Description = ClassInfo.Description,
                    IsActive = true
                };

                _context.Classes.Add(newClass);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Class added successfully.";
            }

            ClassInfo = new ClassInformationModel();
            ModelState.Clear();

            return RedirectToPage(new { Filter, PageNumber });
        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            var classToEdit = await _context.Classes.FindAsync(id);
            if (classToEdit != null)
            {
                ClassInfo = new ClassInformationModel
                {
                    Id = classToEdit.Id,
                    ClassName = classToEdit.ClassName,
                    StudentCount = classToEdit.StudentCount,
                    Description = classToEdit.Description
                };
                EditId = id;
            }
            else
            {
                TempData["ErrorMessage"] = "The item you tried to edit was not found.";
                return RedirectToPage(new { Filter, PageNumber });
            }

            await UpdateDisplayListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var classToDelete = await _context.Classes.FindAsync(id);
            if (classToDelete != null)
            {
                classToDelete.IsActive = false;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Class deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "The item you tried to delete was not found.";
            }

            await UpdateDisplayListAsync();

            int pageNum = PageNumber > TotalPages ? TotalPages : PageNumber;
            pageNum = pageNum < 1 ? 1 : pageNum;

            return RedirectToPage(new { Filter, PageNumber = pageNum });
        }

        public async Task<IActionResult> OnPostExportJson(string selectedColumns = "")
        {
            try
            {
                var data = await GetFilteredDataAsync();

                var columns = string.IsNullOrWhiteSpace(selectedColumns)
                    ? new List<string>()
                    : selectedColumns.Split(',').ToList();

                string json = Utils.Instance.ExportToJson(data, columns);

                var exportDir = Path.Combine(_env.ContentRootPath, "Exports");
                Directory.CreateDirectory(exportDir);

                var fileName = $"class-export-{DateTime.Now:yyyyMMdd-HHmmss}.json";
                var filePath = Path.Combine(exportDir, fileName);

                System.IO.File.WriteAllText(filePath, json);

                TempData["SuccessMessage"] = "File exported successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Export failed: {ex.Message}";
            }

            return RedirectToPage(new { Filter, PageNumber });
        }

        private async Task UpdateDisplayListAsync()
        {
            IQueryable<Classes> query = _context.Classes.Where(c => c.IsActive);

            if (!string.IsNullOrWhiteSpace(Filter))
            {
                var lowerFilter = Filter.ToLower();
                query = query.Where(c =>
                    (!string.IsNullOrEmpty(c.ClassName) && c.ClassName.ToLower().Contains(lowerFilter)) ||
                    (!string.IsNullOrEmpty(c.Description) && c.Description.ToLower().Contains(lowerFilter))
                );
            }

            TotalItems = await query.CountAsync();

            var paged = await query
                .OrderBy(c => c.Id)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            DisplayList = paged.Select(c => new ClassInformationTable
            {
                Id = c.Id,
                ClassName = c.ClassName,
                StudentCount = c.StudentCount,
                Description = c.Description,
                IsActive = c.IsActive
            }).ToList();
        }

        private async Task<List<ClassInformationModel>> GetFilteredDataAsync()
        {
            IQueryable<Classes> query = _context.Classes.Where(c => c.IsActive);

            if (!string.IsNullOrWhiteSpace(Filter))
            {
                var lowerFilter = Filter.ToLower();
                query = query.Where(c =>
                    (!string.IsNullOrEmpty(c.ClassName) && c.ClassName.ToLower().Contains(lowerFilter)) ||
                    (!string.IsNullOrEmpty(c.Description) && c.Description.ToLower().Contains(lowerFilter))
                );
            }

            var list = await query.ToListAsync();

            return list.Select(c => new ClassInformationModel
            {
                Id = c.Id,
                ClassName = c.ClassName,
                StudentCount = c.StudentCount,
                Description = c.Description,
                IsActive = c.IsActive
            }).ToList();
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

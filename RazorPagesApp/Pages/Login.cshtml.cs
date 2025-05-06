using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using RazorPagesApp.Models;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace RazorPagesApp.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IWebHostEnvironment _env;

        public LoginModel(IWebHostEnvironment env)
        {
            _env = env;
        }

        [BindProperty]
        public string? Username { get; set; }

        [BindProperty]
        public string? Password { get; set; }

        public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            if (IsAuthenticated())
                return RedirectToPage("Index");
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var users = await LoadUsersAsync();
            var user = users.FirstOrDefault(u => 
                u.Username == Username && 
                u.Password == Password && 
                u.IsActive
            );

            if (user == null)
            {
                ErrorMessage = "Invalid username or password";
                return Page();
            }

            // Generate token and set session/cookies
            var token = Guid.NewGuid().ToString();
            HttpContext.Session.SetString("username", user.Username);
            HttpContext.Session.SetString("token", token);
            HttpContext.Session.SetString("session_id", HttpContext.Session.Id);

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(30),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("username", user.Username, cookieOptions);
            Response.Cookies.Append("token", token, cookieOptions);
            Response.Cookies.Append("session_id", HttpContext.Session.Id, cookieOptions);

            return RedirectToPage("Index");
        }

        private bool IsAuthenticated()
        {
            return HttpContext.Session.GetString("username") != null &&
                   Request.Cookies["username"] == HttpContext.Session.GetString("username") &&
                   Request.Cookies["token"] == HttpContext.Session.GetString("token") &&
                   Request.Cookies["session_id"] == HttpContext.Session.GetString("session_id");
        }

        private async Task<List<User>> LoadUsersAsync()
        {
            var path = Path.Combine(_env.WebRootPath, "data", "users.json");
            if (!System.IO.File.Exists(path))
                return new List<User>();

            using var stream = new FileStream(path, FileMode.Open);
    var result = await JsonSerializer.DeserializeAsync<List<User>>(stream);
    return result ?? new List<User>();        }
    }
}
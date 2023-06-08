using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using Transitify.Data;

namespace Transitify.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly MongoDbContext _dbContext;

        [BindProperty]
        [Required(ErrorMessage = "Nazwa użytkownika jest wymagana.")]
        public string Username { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Hasło jest wymagane.")]
        public string Password { get; set; }

        public LoginModel(MongoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/Tickets/MyTickets");
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var user = _dbContext.Users.Find(u => u.Username == Username).FirstOrDefault();
            if (user == null)
            {
                ModelState.AddModelError("Username", "Nieprawidłowa nazwa użytkownika.");
                return Page();
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError(string.Empty, "Hasło jest wymagane.");
                return Page();
            }

            if (!BCrypt.Net.BCrypt.Verify(Password, user.PasswordHash))
            {
                ModelState.AddModelError("Password", "Nieprawidłowe hasło.");
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToPage("/Tickets/MyTickets");
        }
    }
}

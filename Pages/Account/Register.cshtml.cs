using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using Transitify.Data;
using Transitify.Models;

namespace Transitify.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly MongoDbContext _dbContext;

        [BindProperty]
        [Required(ErrorMessage = "Username is required.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username may contain only numbers, uppercase and lowercase letters.")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Username must be between 5 and 30 characters.")]
        public string Username { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])[a-zA-Z0-9]+$", ErrorMessage = "Password must contain lowercase letter, uppercase letter and number.")]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 30 characters.")]
        public string Password { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "You must repeat your password.")]
        [Compare("Password", ErrorMessage = "The passwords do not match.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])[a-zA-Z0-9]+$", ErrorMessage = "Password must contain lowercase letter, uppercase letter and number.")]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 30 characters.")]
        public string RepeatPassword { get; set; }

        public RegisterModel(MongoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/Privacy");
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = _dbContext.Users.Find(u => u.Username == Username).FirstOrDefault();
            if (user != null)
            {
                ModelState.AddModelError("Username", "Username is already taken.");
                return Page();
            }

            var userId = GetNextUserId();

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(Password);

            var newUser = new User
            {
                UserId = userId,
                Username = Username,
                PasswordHash = passwordHash
            };

            _dbContext.Users.InsertOne(newUser);

            return RedirectToPage("/Index");
        }
        private int GetNextUserId()
        {
            var usersExist = _dbContext.Users.AsQueryable().Any();
            if (!usersExist)
            {
                return 1;
            }

            var highestUserId = _dbContext.Users.AsQueryable().Max(u => u.UserId);
            return highestUserId + 1;
        }
    }
}

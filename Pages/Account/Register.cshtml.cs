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
        [Required(ErrorMessage = "Nazwa użytkownika jest wymagana.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Nazwa użytkownika może zawierać jedynie małe litery, duże litery i cyfry.")]
        [StringLength(30, MinimumLength = 4, ErrorMessage = "Nazwa użytkownika musi mieć od 4 do 30 znaków.")]
        public string Username { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])[a-zA-Z0-9]+$", ErrorMessage = "Hasło musi zawierać przynajmniej jedną: małą literę, dużą literę i cyfrę.")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Hasło musi mieć od 6 do 30 znaków.")]
        public string Password { get; set; }

        [BindProperty]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Powtórzenie hasła jest wymagane.")]
        [Compare("Password", ErrorMessage = "Wprowadzone hasła nie są identyczne.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])[a-zA-Z0-9]+$", ErrorMessage = "Hasło musi zawierać przynajmniej jedną: małą literę, dużą literę i cyfrę.")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Hasło musi mieć od 6 do 30 znaków.")]
        public string RepeatPassword { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Imię jest wymagane.")]
        [RegularExpression(@"^[A-Z][a-z]+$", ErrorMessage = "Imię musi zaczynać się dużą literą. Pozostałe znaki mają być małymi literami.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Imię musi mieć od 3 do 30 znaków.")]
        public string Name { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Nazwisko jest wymagane.")]
        [RegularExpression(@"^[A-Z][a-z]+$", ErrorMessage = "Nazwisko musi zaczynać się dużą literą. Pozostałe znaki mają być małymi literami.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Nazwisko musi mieć od 3 do 30 znaków.")]
        public string Surname { get; set; }

        [TempData]
        public bool IsRegistrationSuccessful { get; set; }

        public RegisterModel(MongoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/Index");
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
                ModelState.AddModelError("Username", "Podana nazwa użytkownika już istnieje.");
                return Page();
            }

            var userId = GetNextUserId();

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(Password);

            var newUser = new User
            {
                UserId = userId,
                Username = Username,
                PasswordHash = passwordHash,
                Name = Name,
                Surname = Surname
            };

            _dbContext.Users.InsertOne(newUser);

            IsRegistrationSuccessful = true;

            return Page();
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

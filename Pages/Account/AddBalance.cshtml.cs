using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using Transitify.Data;
using Transitify.Models;

namespace Transitify.Pages.Account
{
    public class AddBalanceModel : PageModel
    {
        private readonly MongoDbContext _dbContext;

        [BindProperty]
        public decimal Amount { get; set; }

        public AddBalanceModel(MongoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult OnGet()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Account/Login");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Account/Login");
            }

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            User user = _dbContext.Users.Find(u => u.UserId == userId).FirstOrDefault();

            if (user != null)
            {
                user.Balance += Amount;
                _dbContext.Users.ReplaceOne(u => u.UserId == userId, user);
            }

            return Page();
        }

    }
}

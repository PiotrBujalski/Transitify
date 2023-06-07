using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using System.Linq;
using System.Security.Claims;
using Transitify.Data;
using Transitify.Models;

namespace Transitify.Pages.Tickets
{
    public class CreateModel : PageModel
    {
        private readonly MongoDbContext _dbContext;

        [BindProperty]
        public Ticket Ticket { get; set; }

        public CreateModel(MongoDbContext dbContext)
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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            Ticket.UserId = userId;

            Ticket.TicketId = GetNextTicketId();

            if (Ticket.TicketGroup == "Miesieczny")
            {
                Ticket.TicketTimeMinutes = 30 * 24 * 60;
            }
            else if (Ticket.TicketGroup == "Tygodniowy")
            {
                Ticket.TicketTimeMinutes = 7 * 24 * 60;
            }

            _dbContext.Tickets.InsertOne(Ticket);

            return RedirectToPage("/Index");
        }

        private int GetNextTicketId()
        {
            var ticketExist = _dbContext.Tickets.AsQueryable().Any();
            if (!ticketExist)
            {
                return 1;
            }

            var highestTicketId = _dbContext.Tickets.AsQueryable().Max(u => u.TicketId);
            return highestTicketId + 1;
        }
    }
}

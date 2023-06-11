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

        [TempData]
        public bool IsCreateSuccessful { get; set; }

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

            Ticket.IsActive = false;

            Ticket.IsPaid = false;

            if (Ticket.TicketGroup == "Miesięczny" && Ticket.TicketType == "Normalny")
            {
                Ticket.TicketTimeMinutes = 30 * 24 * 60;
                Ticket.TicketPrice = 168;
            }
            else if (Ticket.TicketGroup == "Miesięczny" && Ticket.TicketType == "Ulgowy")
            {
                Ticket.TicketTimeMinutes = 30 * 24 * 60;
                Ticket.TicketPrice = 84;
            }
            else if (Ticket.TicketGroup == "Tygodniowy" && Ticket.TicketType == "Normalny")
            {
                Ticket.TicketTimeMinutes = 7 * 24 * 60;
                Ticket.TicketPrice = 36;
            }
            else if (Ticket.TicketGroup == "Tygodniowy" && Ticket.TicketType == "Ulgowy")
            {
                Ticket.TicketTimeMinutes = 7 * 24 * 60;
                Ticket.TicketPrice = 18;
            }
            else if (Ticket.TicketGroup == "Jednorazowy" && Ticket.TicketType == "Normalny" && Ticket.TicketTimeMinutes == 20)
            {
                Ticket.TicketPrice = 4.4m;
            }
            else if (Ticket.TicketGroup == "Jednorazowy" && Ticket.TicketType == "Ulgowy" && Ticket.TicketTimeMinutes == 20)
            {
                Ticket.TicketPrice = 2.2m;
            }
            else if (Ticket.TicketGroup == "Jednorazowy" && Ticket.TicketType == "Normalny" && Ticket.TicketTimeMinutes == 40)
            {
                Ticket.TicketPrice = 5.6m;
            }
            else if (Ticket.TicketGroup == "Jednorazowy" && Ticket.TicketType == "Ulgowy" && Ticket.TicketTimeMinutes == 40)
            {
                Ticket.TicketPrice = 2.8m;
            }

            _dbContext.Tickets.InsertOne(Ticket);

            IsCreateSuccessful = true;

            return Page();
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

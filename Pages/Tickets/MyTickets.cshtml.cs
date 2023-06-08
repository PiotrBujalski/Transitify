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

namespace Transitify.Pages.Tickets
{
    public class MyTicketsModel : PageModel
    {
        private readonly MongoDbContext _dbContext;
        public MyTicketsModel(MongoDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<Ticket> Tickets { get; set; }

        public IActionResult OnGet()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Account/Login");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var parsedUserId = int.Parse(userId);
            Tickets = _dbContext.Tickets.Find(t => t.UserId == parsedUserId).ToList();

            return Page();
        }
        public IActionResult OnPost(int ticketId, int ticketTimeMinutes)
        {
            var ticket = _dbContext.Tickets.Find(t => t.TicketId == ticketId).FirstOrDefault();
            if (ticket != null)
            {
                ticket.TicketActivationDate = DateTime.Now;
                ticket.TicketExpirationDate = DateTime.Now.AddMinutes(ticketTimeMinutes);
                ticket.IsActive = true;
                _dbContext.Tickets.ReplaceOne(t => t.TicketId == ticketId, ticket);
            }

            return Redirect("/Tickets/MyTickets");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        public decimal UserBalance { get; set; }

        [TempData]
        public int ErrorTicketId { get; set; }

        public IActionResult OnGet()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Account/Login");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var parsedUserId = int.Parse(userId);
            Tickets = _dbContext.Tickets.Find(t => t.UserId == parsedUserId).ToList();

            var user = _dbContext.Users.Find(u => u.UserId == parsedUserId).FirstOrDefault();
            if (user != null)
            {
                UserBalance = user.Balance;
            }

            return Page();
        }
        public IActionResult OnPost(int ticketId, int ticketTimeMinutes, decimal ticketPrice, string handler)
        {
            var ticket = _dbContext.Tickets.Find(t => t.TicketId == ticketId).FirstOrDefault();
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User user = _dbContext.Users.Find(u => u.UserId == userId).FirstOrDefault();
            if (user != null)
            {
                UserBalance = user.Balance;
            }
            if (ticket != null)
            {
                
                if (handler == "Activate")
                {
                    ticket.TicketActivationDate = DateTime.Now.AddMinutes(120);
                    ticket.TicketExpirationDate = DateTime.Now.AddMinutes(120+ticketTimeMinutes);
                    ticket.IsActive = true;
                    _dbContext.Tickets.ReplaceOne(t => t.TicketId == ticketId, ticket);
                }                
                else if (handler == "Pay")
                {
                    if (user != null)
                    {
                        if (user.Balance >= ticketPrice)
                        {
                            user.Balance -= ticketPrice;
                            ticket.IsPaid = true;
                            _dbContext.Users.ReplaceOne(u => u.UserId == userId, user);
                        }
                        else
                        {
                            ErrorTicketId = ticketId;
                        }
                    }
                    _dbContext.Tickets.ReplaceOne(t => t.TicketId == ticketId, ticket);
                }
                else if (handler == "Delete")
                {
                    return OnPostDelete(ticketId);
                }
                //else if (handler == "Details")
                //{
                //    return OnPostDetails(ticketId);
                //}
                //_dbContext.Tickets.ReplaceOne(t => t.TicketId == ticketId, ticket);
            }

            return Redirect("/Tickets/MyTickets");
        }

        public IActionResult OnPostDelete(int ticketId)
        {
            var ticket = _dbContext.Tickets.Find(t => t.TicketId == ticketId).FirstOrDefault();
            if (ticket != null)
            {
                _dbContext.Tickets.DeleteOne(t => t.TicketId == ticketId);
            }

            return Redirect("/Tickets/MyTickets");
        }
    }
}

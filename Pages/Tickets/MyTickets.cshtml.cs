using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using QRCoder;
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
                    ticket.TicketActivationDate = DateTime.Now;
                    ticket.TicketExpirationDate = DateTime.Now.AddMinutes(ticketTimeMinutes);
                    ticket.IsActive = true;

                    var qrGenerator = new QRCodeGenerator();
                    var qrCodeData = qrGenerator.CreateQrCode($"Posiadacz biletu: {user.Name} {user.Surname}\nTyp biletu: {ticket.TicketType}\nOkres biletu: {ticket.TicketGroup}\nData wygaśnięcia: {ticket.TicketExpirationDate}\n", QRCodeGenerator.ECCLevel.Q);
                    var qrCode = new QRCode(qrCodeData);
                    var qrCodeImage = qrCode.GetGraphic(10);

                    var stream = new MemoryStream();
                    qrCodeImage.Save(stream, ImageFormat.Png);
                    ticket.QRCodeImageBytes = stream.ToArray();

                    ticket.TicketActivationDate = DateTime.Now.AddMinutes(120);
                    ticket.TicketExpirationDate = DateTime.Now.AddMinutes(120 + ticketTimeMinutes);

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

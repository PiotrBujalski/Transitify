using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Transitify.Models
{
    public class Ticket
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("UserId")]
        public int UserId { get; set; }

        [BsonElement("TicketId")]
        public int TicketId { get; set; }

        [BsonElement("TicketType")]
        [Required(ErrorMessage = "Please select a ticket type.")]
        public string TicketType { get; set; }

        [BsonElement("TicketGroup")]
        [Required(ErrorMessage = "Please select a ticket group.")]
        public string TicketGroup { get; set; }

        [BsonElement("TicketTimeMinutes")]
        [Required(ErrorMessage = "Please enter the ticket time in minutes.")]
        public int TicketTimeMinutes { get; set; }

        [BsonElement("TicketActivationTime")]
        public DateTime? TicketActivationTime { get; set; }

        [BsonElement("IsActive")]
        public bool? IsActive { get; set; }
    }
}

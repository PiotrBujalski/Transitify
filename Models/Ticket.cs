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
        [Required]
        public string TicketType { get; set; }

        [BsonElement("TicketGroup")]
        [Required]
        public string TicketGroup { get; set; }

        [BsonElement("TicketTimeMinutes")]
        [Required]
        public int TicketTimeMinutes { get; set; }

        [BsonElement("TicketPrice")]
        [Required]
        public decimal TicketPrice { get; set; }

        [BsonElement("TicketActivationDate")]
        public DateTime? TicketActivationDate { get; set; }

        [BsonElement("TicketExpirationDate")]
        public DateTime? TicketExpirationDate { get; set; }

        [BsonElement("IsActive")]
        public bool IsActive { get; set; }

        [BsonElement("IsPaid")]
        public bool IsPaid { get; set; }

        [BsonElement("QRCodeImageBytes")]
        public byte[] QRCodeImageBytes { get; set; }
    }
}

﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transitify.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("UserId")]
        public int UserId { get; set; }

        [BsonElement("Username")]
        public string Username { get; set; }

        [BsonElement("PasswordHash")]
        public string PasswordHash { get; set; }

        [BsonElement("Age")]
        public int Age { get; set; }

        //[BsonElement("TicketIds")]
        //public List<string> TicketIds { get; set; }

        //[BsonElement("UsedTicketIds")]
        //public List<string> UsedTicketIds { get; set; }
    }
}

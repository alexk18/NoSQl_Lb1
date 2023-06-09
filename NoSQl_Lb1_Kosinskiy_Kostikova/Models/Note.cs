﻿using MongoDB.Bson.Serialization.Attributes;

namespace NoSQl_Lb1_Kosinskiy_Kostikova.Models
{
    public class Note
    {
        [BsonId]
        public Guid Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("text")]
        public string Text { get; set; }

        [BsonElement("userId")]
        public Guid UserId { get; set; }

        [BsonElement("lastUpd")]
        public DateTime? LastUpdate { get; set; }
    }
}

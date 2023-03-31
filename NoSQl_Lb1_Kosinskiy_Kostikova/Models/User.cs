using MongoDB.Bson.Serialization.Attributes;

namespace NoSQl_Lb1_Kosinskiy_Kostikova.Models
{
    public class User
    {
            [BsonId]
            public Guid Id { get; set; }
            [BsonElement("userName")]
            public string UserName { get; set; }
            [BsonElement("password")]
            public string Password { get; set; }
    }
}


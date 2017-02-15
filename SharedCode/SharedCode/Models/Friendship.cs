using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCode.Models
{
    public class Friendship
    {
        [BsonElement("_id")]
        public ObjectId id;

        [BsonElement("requester")]
        public ObjectId Requester;

        [BsonElement("receiver")]
        public ObjectId Receiver;

        [BsonElement("date")]
        public DateTime Date;

        [BsonElement("accepted")]
        public bool Accepted;
    }
}

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SharedCode.Models
{
    [DataContract]
    public class Friendship
    {
        [DataMember]
        [BsonElement("_id")]
        public ObjectId id;

        [DataMember]
        [BsonElement("requester")]
        public ObjectId Requester;

        [DataMember]
        [BsonElement("receiver")]
        public ObjectId Receiver;

        [DataMember]
        [BsonElement("date")]
        public DateTime Date;

        [DataMember]
        [BsonElement("accepted")]
        public bool Accepted;
    }
}

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SharedCode.Models
{
    [DataContract]
    [JsonObject(MemberSerialization.OptIn)]
    public class Friendship
    {
        [DataMember]
        [BsonElement("_id")]
        public ObjectId id;

        [DataMember]
        [BsonElement("sender")]
        [JsonProperty("from")]
        public ObjectId Requester;

        [DataMember]
        [BsonElement("receiver")]
        [JsonProperty("to")]
        public ObjectId Receiver;

        [DataMember]
        [BsonElement("date_request")]
        [JsonProperty("date")]
        public DateTime Date;

        [DataMember]
        [BsonElement("accepted")]
        public bool Accepted;

        [BsonElement("date_accepted")]
        public DateTime DateAccepted;
    }
}

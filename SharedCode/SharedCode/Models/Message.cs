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
    /// <summary>
    /// elements of a message
    /// </summary>
    [DataContract]
    [JsonObject(MemberSerialization.OptIn)]
    public class Message
    {
        /// <summary>
        /// text of a message
        /// </summary>
        [DataMember]
        [BsonElement("text")]
        [JsonProperty("message")]
        public string Text { get; set; }

        /// <summary>
        /// friend id of the sender
        /// </summary>
        [DataMember]
        [BsonElement("receiver")]
        [JsonProperty("to")]
        public ObjectId Receiver { get; set; }

        /// <summary>
        /// point of time when message was sent
        /// </summary>
        [DataMember]
        [BsonElement("date")]
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [DataMember]
        [BsonElement("sender")]
        [JsonProperty("from")]
        public ObjectId Sender { get; set; }

        [DataMember]
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonElement("read")]
        public bool Read { get; set; }
    }
}

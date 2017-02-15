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
    /// <summary>
    /// elements of a message
    /// </summary>
    [DataContract]
    public class Message
    {
        /// <summary>
        /// text of a message
        /// </summary>
        [DataMember]
        [BsonElement("text")]
        public string Text { get; set; }

        /// <summary>
        /// friend id of the sender
        /// </summary>
        [DataMember]
        [BsonElement("receiver")]
        public ObjectId Receiver { get; set; }

        /// <summary>
        /// point of time when message was sent
        /// </summary>
        [DataMember]
        [BsonElement("date")]
        public DateTime Date { get; set; }

        [DataMember]
        [BsonElement("sender")]
        public ObjectId Sender { get; set; }

        [BsonElement("_id")]
        public ObjectId Id { get; set; }
    }
}

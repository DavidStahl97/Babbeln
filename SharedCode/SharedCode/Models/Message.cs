using MongoDB.Bson;
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
        public string Text { get; set; }

        /// <summary>
        /// friend id of the sender
        /// </summary>
        [DataMember]
        public ObjectId Receiver { get; set; }

        /// <summary>
        /// point of time when message was sent
        /// </summary>
        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public ObjectId Sender { get; set; }

        public static implicit operator Message(VoIPApp.Common.ServerServiceReference.Message v)
        {
            return new Message { Text = v.Text, Receiver= v.Receiver, Date = v.Date, Sender = v.Sender };
        }
    }
}

using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace SharedCode.Models
{
    /// <summary>
    /// represents a friend of the user
    /// </summary>
    [DataContract]
    public class Friend
    {
        /// <summary>
        /// name of the friend
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// profile name of the friend
        /// </summary>
        [DataMember]
        public string ProfileName { get; set; }

        /// <summary>
        /// profile picture of the friend
        /// </summary>
        [DataMember]
        public string Icon { get; set; }

        /// <summary>
        /// id of the friend
        /// </summary>
        [DataMember]
        public ObjectId _id { get; set; }

        /// <summary>
        /// represents the current status of the friend for example Online or Offline
        /// </summary>
        [DataMember]
        public string Status {
            get { return SharedCode.Models.Status.Online.ToString(); }
            set { Status = value; }
        }

        /// <summary>
        /// ip of the friend
        /// </summary>
        [DataMember]
        public string IP { get; set; }
    }

    /// <summary>
    /// status that a user can have
    /// </summary>
    public enum Status
    {
        Online,
        Offline
    }
}

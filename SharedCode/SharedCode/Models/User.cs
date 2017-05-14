using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Prism.Mvvm;
using System.ComponentModel;
using System.Windows.Media;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedCode.Models
{
    [DataContract]
    [BsonIgnoreExtraElements]
    public class User : INotifyPropertyChanged
    {
        public User()
        {
            UnreadMessages = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private string name;

        [BsonElement("username")]
        [DataMember]
        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        /// <summary>
        /// profile name of the friend
        /// </summary>
        private string profileName; 

        [DataMember]
        public string ProfileName
        {
            get { return this.name; }
            set
            {
                if(this.profileName != value)
                {
                    this.profileName = value;
                    OnPropertyChanged(nameof(ProfileName));
                }
            }
        }

        /// <summary>
        /// profile picture of the friend
        /// </summary>
        private string icon;

        [DataMember]
        public string Icon
        {
            get { return this.icon; }
            set
            {
                if(icon != value)
                {
                    this.icon = value;
                    OnPropertyChanged(nameof(Icon));
                }
            }
        }

        /// <summary>
        /// id of the friend
        /// </summary>
        [BsonElement("_id")]
        [DataMember]
        public ObjectId _id { get; set; }

        /// <summary>
        /// represents the current status of the friend for example Online or Offline
        /// </summary>
        private Status status; 

        [DataMember]
        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)]
        public Status FriendStatus {
            get { return status; }
            set
            {
                if(status != value)
                {
                    this.status = value;
                    OnPropertyChanged(nameof(FriendStatus));
                }
            }
        }

        /// <summary>
        /// ip of the friend
        /// </summary>
        [BsonElement("ip")]
        [DataMember]
        public string IP { get; set; }

        [BsonElement("email")]
        public string EMail { get; set; }

        [DataMember]
        public Friendship Friendship { get; set; }

        private int unreadMessages;
        public int UnreadMessages
        {
            get { return unreadMessages; }
            set
            {
                if(unreadMessages != value)
                {
                    this.unreadMessages = value;
                    OnPropertyChanged(nameof(UnreadMessages));
                }
            }
        }
    }

    /// <summary>
    /// status that a user can have
    /// </summary>
    public enum Status
    {
        Offline,
        Online,
        Abwesend,
        Beschäftigt,
        Web
    }
}

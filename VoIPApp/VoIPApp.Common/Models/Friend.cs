using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoIPApp.Common.Models
{
    /// <summary>
    /// represents a friend of the user
    /// </summary>
    public class Friend
    {
        /// <summary>
        /// name of the friend
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// profile name of the friend
        /// </summary>
        public string ProfileName { get; set; }
        /// <summary>
        /// profile picture of the friend
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// id of the friend
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// represents the current status of the friend for example Online or Offline
        /// </summary>
        public Status CurrentStatus { get; set; }
        /// <summary>
        /// ip of the friend
        /// </summary>
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

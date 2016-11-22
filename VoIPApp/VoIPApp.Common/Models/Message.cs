using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoIPApp.Common.Models
{
    /// <summary>
    /// elements of a message
    /// </summary>
    public class Message
    {
        /// <summary>
        /// text of a message
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// hour when the message was sent
        /// </summary>
        public int Hour { get; set; }
        /// <summary>
        /// minute when the message was sent
        /// </summary>
        public int Minute { get; set; }
        /// <summary>
        /// friend id of the sender
        /// </summary>
        public int FriendID { get; set; }
    }
}

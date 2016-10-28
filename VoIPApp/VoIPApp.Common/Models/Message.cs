using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoIPApp.Common.Models
{
    public class Message
    {
        public string Text { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int FriendID { get; set; }
    }
}

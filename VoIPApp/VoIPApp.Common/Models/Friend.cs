using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoIPApp.Common.Models
{
    public class Friend
    {
        public string Name { get; set; }
        public string ProfileName { get; set; }
        public string Icon { get; set; }
        public int ID { get; set; }
        public Status CurrentStatus { get; set; }
    }

    public enum Status
    {
        Online,
        Offline
    }
}

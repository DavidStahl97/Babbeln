using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoIPApp.Common.Models
{
    public class AccountDetails
    {
        public ObjectId UserID { get; set; }

        public string IP { get; set; }

        public string Username { get; set; }
    }
}

using MongoDB.Bson;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoIPApp.Common.Models
{
    public class AccountDetails : BindableBase
    {
        public ObjectId UserID { get; set; }

        public string IP { get; set; }

        private string username;
        public string Username
        {
            get { return username; }
            set { SetProperty(ref username, value); }
        }
    }
}

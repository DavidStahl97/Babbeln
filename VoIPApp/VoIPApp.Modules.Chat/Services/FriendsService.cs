using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoIPApp.Common.Models;

namespace VoIPApp.Modules.Chat.Services
{
    public class FriendsService : IFriendsService
    {
        public Dictionary<int, Friend> Friends { get; set; }

        public FriendsService()
        {
            Friends = new Dictionary<int, Friend>();

            Friends.Add(0, new Friend { Name = "David Stahl", ProfileName = "@totalhirn", Icon = "..\\Assets\\profile1.png", ID = 0, CurrentStatus = Status.Online, IP = "192.168.1.53"});
            Friends.Add(1, new Friend { Name = "David Stahl2", ProfileName = "@totalhirn", Icon = "..\\Assets\\profile1.png", ID = 1, CurrentStatus = Status.Online, IP = "127.0.0.1" });
            Friends.Add(2, new Friend { Name = "Rebecca", ProfileName = "@becci", Icon = "..\\Assets\\profile2.png", ID = 2, CurrentStatus = Status.Online, IP = "192.168.1.33"});
            Friends.Add(3, new Friend { Name = "Pedro Mano", ProfileName = "@crazycape", Icon = "..\\Assets\\profile3.png", ID = 3, CurrentStatus = Status.Online });
            Friends.Add(4, new Friend { Name = "ahl", ProfileName = "@crazycape", Icon = "..\\Assets\\profile3.png", ID = 4, CurrentStatus = Status.Offline });
        }


    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoIPApp.Common.Models;

namespace VoIPApp.Modules.Chat.Services
{
    public interface IFriendsService
    {
        Dictionary<int, Friend> Friends { get; set; }
    }
}

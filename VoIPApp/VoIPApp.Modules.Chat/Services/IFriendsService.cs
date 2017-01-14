using MongoDB.Bson;
using SharedCode.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoIPApp.Modules.Chat.Services
{
    public interface IFriendsService
    {
        ObservableCollection<Friend> Friends { get; set; }
        Task UpdateFriendsList();
        void UpdateProfilePictures();
        Task AddFriendByName(string friendName);
        Friend GetFriendById(ObjectId id);
    }
}

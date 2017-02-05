using Microsoft.Practices.Unity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SharedCode.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCode.Services;
using VoIPApp.Common.Services;
using System.IO;

namespace VoIPApp.Modules.Chat.Services
{
    public class FriendsService : IFriendsService
    {
        private readonly ServerServiceProxy serverService;
        private readonly ObjectId userId;
        private readonly DataBaseService dataBaseService;

        public FriendsService(IUnityContainer container, DataBaseService dataBaseService, ServerServiceProxy serverService)
        {
            this.dataBaseService = dataBaseService;
            Friends = new ObservableCollection<Friend>();

            this.serverService = serverService;
            this.userId = serverService.UserId;
        }

        public ObservableCollection<Friend> Friends { get; set; }

        public async Task PopulateFriendList()
        {
            List<Friend> friends = await dataBaseService.GetFriendList(userId, Builders<BsonDocument>.Filter.Empty);
            foreach(Friend f in friends)
            {
                Friends.Add(f);
            }
        }

        public void UpdateProfilePictures()
        {
            foreach(Friend friend in Friends)
            {
                string profilePicturePath = string.Format("pack://application:,,,/Images/{0}.jpg", friend._id.ToString());
                if (File.Exists(profilePicturePath))
                {
                    friend.Icon = profilePicturePath;
                }
                else
                {
                    friend.Icon = "pack://application:,,,/Assets/profile_high.jpg";
                }
            }
        }

        public async Task AddFriendByName(string friendName)
        {
            Friend f = await serverService.ServerService.AddFriendByNameAsync(friendName);
            if(f != null)
            {
                Friends.Add(f);
            }
        }

        public Friend GetFriendById(ObjectId id)
        {
            foreach(Friend f in Friends)
            {
                if(f._id.Equals(id))
                {
                    return f;
                }
            }

            return null;
        }
    }
}

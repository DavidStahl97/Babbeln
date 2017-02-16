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
using MongoDB.Driver.Linq;
using Prism.Events;
using VoIPApp.Common;

namespace VoIPApp.Modules.Chat.Services
{
    public class FriendsService : IFriendsService
    {
        private readonly ServerServiceProxy serverService;
        private readonly ObjectId userId;
        private readonly DataBaseService dataBaseService;
        private readonly EventAggregator eventAggregator;

        public FriendsService(IUnityContainer container, DataBaseService dataBaseService, ServerServiceProxy serverService, EventAggregator eventAggregator)
        {
            this.dataBaseService = dataBaseService;
            this.eventAggregator = eventAggregator;
            Friends = new ObservableCollection<User>();

            this.serverService = serverService;
            this.userId = serverService.UserId;

            eventAggregator.GetEvent<FriendshipRequestedEvent>().Subscribe(this.OnFriendshipRequested, ThreadOption.BackgroundThread);
            eventAggregator.GetEvent<FriendshipRequestAnswerdEvent>().Subscribe(this.OnFriendshipAnswered);
        }

        public ObservableCollection<User> Friends { get; set; }

        public async Task PopulateFriendList()
        {
            List<User> friends = await dataBaseService.GetFriendList(userId);
            Friends.AddRange(friends);

            IMongoQueryable<Friendship> query = from Friendship in dataBaseService.FriendshipCollection.AsQueryable()
                                     where Friendship.Receiver.Equals(userId) || Friendship.Requester.Equals(userId)
                                     select Friendship;

            await query.ForEachAsync(friendship =>
            {
                ObjectId friendId = (friendship.Receiver.Equals(userId)) ? friendship.Requester : friendship.Receiver;
                GetFriendById(friendId).Friendship = friendship;
            });
        }

        public void UpdateProfilePictures()
        {
            foreach(User friend in Friends)
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

        private void OnFriendshipRequested(ObjectId friendId)
        {
            User friend = (from user in dataBaseService.UserCollection.AsQueryable()
                          where user._id.Equals(friendId)
                          select user).First();

            if (friend != null)
            {
                friend.Friendship = (from friendship in dataBaseService.FriendshipCollection.AsQueryable()
                                     where friendship.Receiver.Equals(userId) && friendship.Requester.Equals(friendId)
                                     select friendship).First();

                Friends.Add(friend);
            }
        }

        private void OnFriendshipAnswered(FriendshipRequestAnsweredEventArgs args)
        {
            User friend = GetFriendById(args.FriendId);
            friend.Friendship.Accepted = args.Accepted;
        }

        public async Task<bool> SendFriendRequest(string friendName)
        {
            User f = await serverService.ServerService.SendFriendRequestAsync(friendName);
            if(f != null)
            {
                Friends.Add(f);
                return true;
            }

            return false;
        }

        public User GetFriendById(ObjectId id)
        {
            foreach(User f in Friends)
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

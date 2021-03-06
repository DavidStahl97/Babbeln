﻿using Microsoft.Practices.Unity;
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
using System.Windows.Threading;
using System.Windows;

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
            this.userId = serverService.UserInfo.UserID;

            eventAggregator.GetEvent<FriendshipRequestedEvent>().Subscribe(this.OnFriendshipRequested, ThreadOption.BackgroundThread);
            eventAggregator.GetEvent<FriendshipRequestAnswerdEvent>().Subscribe(this.OnFriendshipAnswered);
            eventAggregator.GetEvent<FriendUsernameChanged>().Subscribe(this.OnFriendUsernameChanged);
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
                friend.Icon = "pack://application:,,,/Assets/profile_high.jpg";
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
                friend.Icon = "pack://application:,,,/Assets/profile_high.jpg";

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Friends.Add(friend);
                });
            }
        }

        private void OnFriendshipAnswered(FriendshipRequestAnsweredEventArgs args)
        {
            User friend = GetFriendById(args.FriendId);
            if(args.Accepted)
            {
                friend.Friendship.Accepted = args.Accepted;
            }
            else
            {
                Friends.Remove(friend);
            }
        }

        public async Task<bool> SendFriendRequest(string friendName)
        {
            if(Friends.Where(friend => friend.Name.Equals(friendName)).Count() == 0)
            {
                User f = await serverService.ServerService.SendFriendRequestAsync(friendName);
                if(f != null)
                {
                    f.Icon = "pack://application:,,,/Assets/profile_high.jpg";
                    Friends.Add(f);
                    return true;
                }
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

        public async Task DeleteFriend(ObjectId id)
        {
            //serverService.ServerService.
        }

        public async Task AnswerFriendshipRequest(ObjectId friendId, bool accept)
        {
            await serverService.ServerService.ReplyToFriendRequestAsync(friendId, accept);
            if(!accept)
            {
                Friends.Remove(GetFriendById(friendId));
            }
        }


        private void OnFriendUsernameChanged(FriendUsernameChangedEventArgs obj)
        {
            User friend = GetFriendById(obj.FriendId);
            friend.Name = obj.NewUsername;
        }
    }
}

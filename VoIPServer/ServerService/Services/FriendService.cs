﻿using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SharedCode.Models;
using SharedCode.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoIPServer.ServerServiceLibrary.Services
{
    public class FriendService
    {
        private readonly DataBaseService dataBaseService;
        private readonly LoginService loginService;

        public FriendService(DataBaseService dataBaseService, LoginService loginService)
        {
            this.dataBaseService = dataBaseService;
            this.loginService = loginService;
        }

        public async Task<User> AddFriendByName(string friendName)
        {
            IMongoQueryable<User> query = from user in dataBaseService.UserCollection.AsQueryable()
                          where user.Name.Equals(friendName)
                          select user;

            if (await query.AnyAsync())
            {
                User friend = await query.FirstAsync();
                Friendship friendship = new Friendship
                {
                    Receiver = friend._id,
                    Requester = loginService.UserId,
                    Date = DateTime.Now,
                    Accepted = false
                };
        
                await dataBaseService.FriendshipCollection.InsertOneAsync(friendship);

                friend.Friendship = friendship;

                IServerCallBack friendCallback = loginService.GetCallbackChannelByID(friend._id);
                if (friendCallback != null)
                {
                    friendCallback.OnFriendshipRequested(loginService.UserId);
                }

                return friend;
            }

            return null;
        }

        public async Task ReplyToFriendRequest(ObjectId friendId, bool accept)
        {
            FilterDefinitionBuilder<Friendship> builder = Builders<Friendship>.Filter;
            FilterDefinition<Friendship> filter = builder.Eq(f => f.Receiver, loginService.UserId) & builder.Eq(f => f.Requester, friendId);

            if(accept)
            {
                UpdateDefinition<Friendship> update = Builders<Friendship>.Update.Set(f => f.Accepted, accept);
                await dataBaseService.FriendshipCollection.UpdateOneAsync(filter, update);
            }
            else
            {
                await dataBaseService.FriendshipCollection.DeleteOneAsync(filter);
            }

            IServerCallBack friendCallback = loginService.GetCallbackChannelByID(friendId);
            if(friendCallback != null)
            {
                friendCallback.OnFriendshipRequestAnswered(loginService.UserId, accept);
            }
        }
    }
}

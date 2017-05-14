using MongoDB.Bson;
using MongoDB.Driver;
using ServerServiceLibrary.Model;
using SharedCode.Models;
using SharedCode.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoIPServer.ServerServiceLibrary.DataContract;

namespace VoIPServer.ServerServiceLibrary.Services
{
    public class AccountService
    {
        private readonly DataBaseService databaseService;
        private readonly LoginService loginService;

        public AccountService(DataBaseService databaseService, LoginService loginService)
        {
            this.databaseService = databaseService;
            this.loginService = loginService;
        }

        public async Task ChangePassword(string password)
        {

        }

        public async Task ChangeUsername(string username)
        {
            List<ObjectId> friends = await databaseService.GetFriendIdList(loginService.UserId);
            friends.ForEach(friendId =>
            {
                ClientCallback clientCallback = loginService.GetCallbackChannelByID(friendId);
                if(clientCallback != null)
                {
                    clientCallback.OnFriendsUsernameChanged(loginService.UserId, username);
                }
            });

            FilterDefinition<User> filter = Builders<User>.Filter.Eq("_id", loginService.UserId);
            UpdateDefinition<User> update = Builders<User>.Update.Set("username", username);
            await databaseService.UserCollection.UpdateOneAsync(filter, update);
        }
    }
}

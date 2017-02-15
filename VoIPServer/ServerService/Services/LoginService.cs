using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SharedCode.Models;
using SharedCode.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using VoIPServer.ServerServiceLibrary;

namespace VoIPServer.ServerServiceLibrary.Services
{
    public class LoginService
    {
        private static readonly Dictionary<ObjectId, IServerCallBack> subscribers = new Dictionary<ObjectId, IServerCallBack>();

        private readonly DataBaseService dataBaseService;

        private ObjectId userId;

        public LoginService(DataBaseService dataBaseService)
        {
            this.userId = ObjectId.Empty;
            this.dataBaseService = dataBaseService;
        }

        public async Task<ObjectId> Subscribe(string userName, string password, string ip, IServerCallBack callbackChannel)
        {
            userId = await dataBaseService.GetUserId(userName);
            userId = await dataBaseService.GetUserId(userName, password);

            if (!userId.Equals(ObjectId.Empty))
            {
                if (subscribers.ContainsKey(userId))
                {
                    Console.WriteLine(userName + " is already connected");
                    return userId;
                }
                else
                {
                    FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", userId);
                    UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("ip", ip);
                    await dataBaseService.UserBsonCollection.UpdateOneAsync(filter, update);

                    subscribers.Add(userId, callbackChannel);

                    await NotifyFriendsAboutStatusChange(Status.Online);

                    Console.WriteLine(userName + " successfully connected with id: " + userId.ToString());

                    return userId;
                }
            }

            return ObjectId.Empty;
        }

        public async Task Unsubscribe()
        {
            if (!userId.Equals(ObjectId.Empty) && subscribers.ContainsKey(userId))
            {
                subscribers.Remove(userId);
                Console.WriteLine(userId + " removed");

                await NotifyFriendsAboutStatusChange(Status.Offline);
            }
            else
            {
                Console.WriteLine(userId + " could not be unsubscribed.");
            }
        }

        public async Task<string> Register(string userName, string password, string email, string ip)
        {
            /*FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;

            FilterDefinition<BsonDocument> filter = builder.Eq("username", userName);
            if (await dataBaseService.UserCollection.CountAsync(filter) > 0)
            {
                return "Benutzername schon vergeben";
            }

            filter = builder.Eq("email", email);
            if (await dataBaseService.UserCollection.CountAsync(filter) > 0)
            {
                return "E-Mail schon vergeben";
            }*/

            int result = await (from user in dataBaseService.UserCollection.AsQueryable()
                          select user).CountAsync(user => user.Name.Equals(userName));

            if(result > 0)
            {
                return "Benutzername schon vergeben";
            }

            result = await (from user in dataBaseService.UserBsonCollection.AsQueryable()
                            select user).CountAsync(user => user["email"].ToString().Equals(email));

            if(result > 0)
            {
                return "E-Mail schonn vergeben";
            }

            BsonDocument doc = new BsonDocument
            {
                {"username", userName},
                {"password", password },
                {"email", email },
                {"ip", ip }
            };

            await dataBaseService.UserBsonCollection.InsertOneAsync(doc);

            return string.Empty;
        }

        public IServerCallBack GetCallbackChannelByID(ObjectId id)
        {
            IServerCallBack callbackChannel = null;
            subscribers.TryGetValue(id, out callbackChannel);
            return callbackChannel;
        }

        private async Task NotifyFriendsAboutStatusChange(Status status)
        {
            List<ObjectId> friendIds = await dataBaseService.GetFriendIdList(userId);
            foreach (ObjectId friendId in friendIds)
            {
                IServerCallBack callback = GetCallbackChannelByID(friendId);
                if (callback != null)
                {
                    callback.OnFriendStatusChanged(friendId, status);
                }
            }
        }

        public bool LoggedIn
        {
            get
            {
                return !userId.Equals(ObjectId.Empty);
            }
        }

        public ObjectId UserId
        {
            get { return this.userId; }
        }
    }
}

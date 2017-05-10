using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json.Linq;
using SharedCode.Models;
using SharedCode.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using VoIPServer.ServerServiceLibrary;
using VoIPServer.ServerServiceLibrary.DataContract;
using VoIPServer.ServerServiceLibrary.Model;

namespace VoIPServer.ServerServiceLibrary.Services
{
    public class LoginService
    {
        //use threadsafe Dictionary because its used in multiple threads. note static
        private static readonly ConcurrentDictionary<ObjectId, IClientCallback> subscribers = new ConcurrentDictionary<ObjectId, IClientCallback>();

        private readonly DataBaseService dataBaseService;

        private ObjectId userId;

        public LoginService(DataBaseService dataBaseService)
        {
            this.userId = ObjectId.Empty;
            this.dataBaseService = dataBaseService;
        }

        public async Task Subscribe(JToken data, IClientCallback callbackChannel)
        {
            string username = data[nameof(username)].ToString();
            string password = data[nameof(password)].ToString();

            await Subscribe(username, password, string.Empty, callbackChannel);
        }

        public async Task<ObjectId> Subscribe(string userName, string password, string ip, IClientCallback callbackChannel)
        {
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

                    subscribers.TryAdd(userId, callbackChannel);

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
                IClientCallback cb;
                subscribers.TryRemove(userId, out cb);
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
            bool result = await (from user in dataBaseService.UserCollection.AsQueryable()
                                where user.Name.Equals(userName)
                                select user).AnyAsync();

            if(result)
            {
                return "Benutzername schon vergeben";
            }

            result = await (from user in dataBaseService.UserCollection.AsQueryable()
                            where user.EMail.Equals(email)
                            select user).AnyAsync();

            if(result)
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

        public IClientCallback GetCallbackChannelByID(ObjectId id)
        {
            IClientCallback callbackChannel = null;
            subscribers.TryGetValue(id, out callbackChannel);
            return callbackChannel;
        }

        private async Task NotifyFriendsAboutStatusChange(Status status)
        {
            List<ObjectId> friendIds = await dataBaseService.GetFriendIdList(userId);
            foreach (ObjectId friendId in friendIds)
            {
                IClientCallback callback = GetCallbackChannelByID(friendId);
                if (callback != null)
                {
                    callback.OnFriendStatusChanged(userId, status);
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

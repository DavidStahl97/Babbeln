using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SharedCode.Models;
using SharedCode.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoIPServer.ServerServiceLibrary.DataContract;

namespace VoIPServer.ServerServiceLibrary.Services
{
    public class LoginService
    {
        //use threadsafe Dictionary because its used in multiple threads. note static
        private static readonly ConcurrentDictionary<ObjectId, IServerCallback> subscribers = new ConcurrentDictionary<ObjectId, IServerCallback>();

        private static IWebsocketCallback websocketCallback = null;

        private readonly DataBaseService dataBaseService;

        private ObjectId userId;

        public LoginService(DataBaseService dataBaseService)
        {
            this.userId = ObjectId.Empty;
            this.dataBaseService = dataBaseService;
        }

        public async Task<ObjectId> Subscribe(string userName, string password, string ip, IServerCallback callbackChannel)
        {
            userId = await GetUserId(userName, password);

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
                IServerCallback cb;
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

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            BsonDocument doc = new BsonDocument
            {
                {"username", userName},
                {"password", hashedPassword },
                {"email", email },
                {"ip", ip }
            };

            await dataBaseService.UserBsonCollection.InsertOneAsync(doc);

            return string.Empty;
        }

        public IServerCallback GetCallbackChannelByID(ObjectId id)
        {
            IServerCallback callbackChannel = null;
            subscribers.TryGetValue(id, out callbackChannel);
            return callbackChannel;
        }

        private async Task NotifyFriendsAboutStatusChange(Status status)
        {
            List<ObjectId> friendIds = await dataBaseService.GetFriendIdList(userId);
            foreach (ObjectId friendId in friendIds)
            {
                IServerCallback callback = GetCallbackChannelByID(friendId);
                if (callback != null)
                {
                    callback.OnFriendStatusChanged(userId, status);
                }
            }
        }

        public async Task<ObjectId> GetUserId(string userName, string password)
        {
            FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;
            FilterDefinition<BsonDocument> filter = builder.Eq("username", userName);

            using (IAsyncCursor<BsonDocument> cursor = await dataBaseService.UserBsonCollection.FindAsync(filter))
            {
                while (await cursor.MoveNextAsync())
                {
                    IEnumerable<BsonDocument> batch = cursor.Current;

                    foreach (BsonDocument document in batch)
                    {
                        string hashedPassword = document["password"].AsString;
                        if(BCrypt.Net.BCrypt.Verify(password, hashedPassword))
                        {
                            return document["_id"].AsObjectId;
                        }
                    }
                }
            }
            return ObjectId.Empty;
        }

        public static IWebsocketCallback WebsocketCallback { get; set; }

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

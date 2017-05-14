﻿using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json.Linq;
using ServerServiceLibrary.Model;
using SharedCode.Models;
using SharedCode.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using VoIPServer.ServerServiceLibrary.DataContract;

namespace VoIPServer.ServerServiceLibrary.Services
{
    public class LoginService
    {
        //use threadsafe Dictionary because its used in multiple threads. note static
        private static readonly ConcurrentDictionary<ObjectId, ClientCallback> subscribers = new ConcurrentDictionary<ObjectId, ClientCallback>();

        private readonly DataBaseService dataBaseService;

        private ObjectId userId;

        public LoginService(DataBaseService dataBaseService)
        {
            this.userId = ObjectId.Empty;
            this.dataBaseService = dataBaseService;
        }

        public async Task Subscribe(JToken data, IWebsocketCallback webcallback)
        {
            ObjectId id = ObjectId.Parse(data["id"].ToString());
            ClientCallback callback = new ClientCallback(webcallback, id);
            subscribers.TryAdd(id, callback);
            await ChangeStatus(Status.Web, id);
        }

        public async Task Unsubscribe(JToken data)
        {
            ObjectId id = ObjectId.Parse(data["id"].ToString());
            ClientCallback cb;
            subscribers.TryRemove(id, out cb);
            await ChangeStatus(Status.Offline, id);
        }

        public async Task<Tuple<ObjectId, string>> Subscribe(string userName, string password, string ip, IServerCallback desktopCallback)
        {
            userId = await GetUserId(userName, password);

            if (!userId.Equals(ObjectId.Empty))
            {
                if (subscribers.ContainsKey(userId))
                {
                    Console.WriteLine(userName + " is already connected");
                    return new Tuple<ObjectId, string>(ObjectId.Empty, "Sie sind schon angemeldet");
                }
                else
                {
                    FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", userId);
                    UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("ip", ip);
                    await dataBaseService.UserBsonCollection.UpdateOneAsync(filter, update);

                    ClientCallback callback = new ClientCallback(desktopCallback);

                    ICommunicationObject obj = (ICommunicationObject)desktopCallback;
                    obj.Closed += async (s, e) =>
                    {
                        await Unsubscribe();
                    };

                    subscribers.TryAdd(userId, callback);

                    await ChangeStatus(Status.Online, userId);

                    Console.WriteLine(userName + " successfully connected with id: " + userId.ToString());

                    return new Tuple<ObjectId, string>(userId, string.Empty);
                }
            }
            else
            {
                return new Tuple<ObjectId, string>(ObjectId.Empty, "Passwort oder Benutzername falsch");
            }
        }

        public async Task Unsubscribe()
        {
            if (!userId.Equals(ObjectId.Empty) && subscribers.ContainsKey(userId))
            {
                ClientCallback cb;
                subscribers.TryRemove(userId, out cb);
                Console.WriteLine(userId + " removed");

                await ChangeStatus(Status.Offline, userId);
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

        public ClientCallback GetCallbackChannelByID(ObjectId id)
        {
            ClientCallback callbackChannel = null;
            subscribers.TryGetValue(id, out callbackChannel);
            return callbackChannel;
        }

        public async Task ChangeStatus(Status status, ObjectId id)
        {
            List<ObjectId> friendIds = await dataBaseService.GetFriendIdList(id);
            foreach (ObjectId friendId in friendIds)
            {
                ClientCallback callback = GetCallbackChannelByID(friendId);
                if (callback != null)
                {
                    callback.OnFriendStatusChanged(id, status);
                }
            }

            FilterDefinition<User> filter = Builders<User>.Filter.Eq("_id", id);
            UpdateDefinition<User> update = Builders<User>.Update.Set("status", status.ToString());
            await dataBaseService.UserCollection.UpdateOneAsync(filter, update);
        }

        public async Task ChangeStatus(Status status)
        {
            await ChangeStatus(status, userId);
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

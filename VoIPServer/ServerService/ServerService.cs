using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SharedCode.Models;
using SharedCode.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace VoIPServer.ServerServiceLibrary
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.PerSession, UseSynchronizationContext = true)]
    public class ServerService : IServerService
    {
        private static readonly Dictionary<IServerCallBack, ObjectId> subscribers = new Dictionary<IServerCallBack, ObjectId>();
        private static readonly DataBaseService dataBaseService = new DataBaseService();
        private static int connections = 0;

        private IServerCallBack currentCallback;
        private int connectionId;

        static ServerService()
        {
            dataBaseService.Connect();
        }

        public ServerService()
        {
            connectionId = connections;
            connections++;
            Console.WriteLine(connections);
        }

        public async void SendMessage(Message msg)
        {
            BsonDocument document = new BsonDocument
            {
                {"sender", msg.Sender},
                {"receiver", msg.Receiver},
                {"date", msg.Date },
                {"text", msg.Text }
            };

            IMongoCollection<BsonDocument> messageCollection = dataBaseService.Database.GetCollection<BsonDocument>("messages");
            await messageCollection.InsertOneAsync(document);

            IServerCallBack receiverCallback = GetCallbackChannelById(msg.Receiver);
            if(receiverCallback != null)
            {
                receiverCallback.OnMessageReceived(msg);
            }
        }

        public async Task<ObjectId> Subscribe(string userName, string password, string ip)
        {
            ObjectId id = await GetUserId(userName, password);
            if (id != ObjectId.Empty && id != null)
            {
                Console.WriteLine(id.ToString());
                IMongoCollection<BsonDocument> userCollection = dataBaseService.Database.GetCollection<BsonDocument>("users");
                FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", id);
                UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("ip", ip);
                await userCollection.UpdateOneAsync(filter, update);

                IServerCallBack callback = OperationContext.Current.GetCallbackChannel<IServerCallBack>();
                if (!subscribers.ContainsKey(callback))
                {
                    subscribers.Add(callback, id);
                    currentCallback = callback;
                }

                ICommunicationObject obj = (ICommunicationObject)callback;
                obj.Closed += (s, e) =>
                {
                    subscribers.Remove(callback);
                };

                return id;
            }

            return ObjectId.Empty;
        }

        public void Unsubscribe()
        {            
            if (subscribers.ContainsKey(currentCallback))
            {
                subscribers.Remove(currentCallback);
                Console.WriteLine(connectionId + "removed");
            }
        }

        public bool Call(ObjectId receiver)
        {
            IServerCallBack receiverCallback = GetCallbackChannelById(receiver);
            if(receiverCallback != null)
            {
                receiverCallback.OnCall(receiver);
                return true;
            }

            return false;
        }

        private async Task<ObjectId> GetUserId(string userName, string password)
        {
            FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;
            FilterDefinition<BsonDocument> filter = builder.Eq("username", userName) & builder.Eq("password", password);

            IMongoCollection<BsonDocument> userCollection = dataBaseService.Database.GetCollection<BsonDocument>("users");
            using (IAsyncCursor<BsonDocument> cursor = await userCollection.FindAsync(filter))
            {
                while (await cursor.MoveNextAsync())
                {
                    IEnumerable<BsonDocument> batch = cursor.Current;
                    foreach (BsonDocument document in batch)
                    {
                        return document["_id"].AsObjectId;
                    }
                }
            }

            return ObjectId.Empty;
        }

        public async Task<string> Register(string userName, string password, string email, string ip)
        {
            IMongoCollection<BsonDocument> userCollection = dataBaseService.Database.GetCollection<BsonDocument>("users");
            FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;

            FilterDefinition<BsonDocument> filter = builder.Eq("username", userName);
            if(await userCollection.CountAsync(filter) > 0)
            {
                return "Benutzername schon vergeben";
            }

            filter = builder.Eq("email", email);
            if(await userCollection.CountAsync(filter) > 0)
            {
                return "E-Mail schon vergeben";
            }

            BsonDocument doc = new BsonDocument
            {
                {"username", userName},
                {"password", password },
                {"email", email },
                {"ip", ip }
            };

            await userCollection.InsertOneAsync(doc);

            return string.Empty;
        }

        public async Task<Friend> AddFriendByName(string friendName)
        {

            IMongoCollection<BsonDocument> userCollection = dataBaseService.Database.GetCollection<BsonDocument>("users");
            FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;
            FilterDefinition<BsonDocument> filter = builder.Eq("username", friendName);

            Friend f = null;

            using (IAsyncCursor<BsonDocument> cursor = await userCollection.FindAsync(filter))
            {
                cursor.MoveNext();
                if(cursor.Current != null)
                {
                    IEnumerable<BsonDocument> batch = cursor.Current;
                    BsonDocument doc = batch.First();
                    f = new Friend { Name = doc["username"].AsString, IP = doc["ip"].AsString, _id = doc["_id"].AsObjectId };
                }
            }

            if(f != null)
            {
                ObjectId userId;
                if(subscribers.TryGetValue(currentCallback, out userId))
                {
                    BsonDocument document = new BsonDocument
                    {
                        { "requester", userId },
                        { "receiver", f._id },
                        { "date", DateTime.Now },
                        { "accepted", true }
                    };

                    IMongoCollection<BsonDocument> friendCollection = dataBaseService.Database.GetCollection<BsonDocument>("friends");
                    await friendCollection.InsertOneAsync(document);
                }
                else
                {
                    return null;
                }
            }

            return f;
        }

        private IServerCallBack GetCallbackChannelById(ObjectId id)
        {
            foreach (KeyValuePair<IServerCallBack, ObjectId> pair in subscribers.AsEnumerable())
            {
                if (pair.Value.Equals(id))
                {
                    return pair.Key;
                }
            }

            return null;
        }

        private ObjectId GetCurrentObjectId()
        {
            ObjectId userId;
            if (subscribers.TryGetValue(currentCallback, out userId))
            {
                return userId;
            }

            return ObjectId.Empty;
        }

        public void CancelCall(ObjectId friendId)
        {
            ObjectId userId = GetCurrentObjectId();
            if(!userId.Equals(ObjectId.Empty))
            {
                IServerCallBack friendCallback = GetCallbackChannelById(friendId);
                if (friendId != null)
                {
                    friendCallback.OnCallCancelled(userId);
                }
            }
        }
            

        public void AcceptCall(ObjectId friendId)
        {
            ObjectId userId = GetCurrentObjectId();
            if (!userId.Equals(ObjectId.Empty))
            {
                IServerCallBack friendCallback = GetCallbackChannelById(friendId);
                if (friendId != null)
                {
                    friendCallback.OnCallAccepted(userId);
                }
            }
        }
    }
}

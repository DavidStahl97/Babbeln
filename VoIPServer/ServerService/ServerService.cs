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
using VoIPServer.ServerServiceLibrary.Services;

namespace VoIPServer.ServerServiceLibrary
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.PerSession, UseSynchronizationContext = true)]
    public class ServerService : IServerService
    {
        private static readonly DataBaseService dataBaseService = new DataBaseService();
        private readonly LoginService loginService;
        private static int instanceCount = 0;

        private readonly IServerCallBack currentCallback;
        private int connectionId;

        static ServerService()
        {
            dataBaseService.Connect();
        }

        public ServerService()
        {
            connectionId = instanceCount;
            instanceCount++;
            Console.WriteLine("New Server Instance was Created with id: " + instanceCount);

            currentCallback = OperationContext.Current.GetCallbackChannel<IServerCallBack>();
            loginService = new LoginService(dataBaseService, currentCallback);
        }

        public async Task<ObjectId> Subscribe(string userName, string password, string ip)
        {
            ICommunicationObject obj = (ICommunicationObject)currentCallback;
            obj.Closed += (s, e) =>
            {
                loginService.Unsubscribe();
            };

            return await loginService.Subscribe(userName, password, ip, currentCallback);
        } 

        public void Unsubscribe()
        {
            loginService.Unsubscribe();
        }

        public async Task<string> Register(string userName, string password, string email, string ip)
        {
            return await loginService.Register(userName, password, email, ip);
        }

        public async Task SendMessage(Message msg)
        {
            if(loginService.LoggedIn)
            {
                BsonDocument document = new BsonDocument
                {
                    {"sender", msg.Sender},
                    {"receiver", msg.Receiver},
                    {"date", msg.Date },
                    {"text", msg.Text }
                };

                await dataBaseService.MessageCollection.InsertOneAsync(document);

                IServerCallBack receiverCallback = loginService.GetCallbackChannelByID(msg.Receiver);
                if(receiverCallback != null)
                {
                    receiverCallback.OnMessageReceived(msg);
                }
            }
        }

        public void Call(ObjectId receiver)
        {
            if(loginService.LoggedIn)
            {
                IServerCallBack receiverCallback = loginService.GetCallbackChannelByID(receiver);
                if(receiver != null)
                {
                    receiverCallback.OnCall(loginService.UserId);
                }
            }
        }

        public async Task<Friend> AddFriendByName(string friendName)
        {
            Friend f = null;

            if(loginService.LoggedIn)
            {
                FilterDefinitionBuilder<BsonDocument> builder = Builders<BsonDocument>.Filter;
                FilterDefinition<BsonDocument> filter = builder.Eq("username", friendName);

                using (IAsyncCursor<BsonDocument> cursor = await dataBaseService.UserCollection.FindAsync(filter))
                {
                    cursor.MoveNext();
                    if (cursor.Current != null)
                    {
                        IEnumerable<BsonDocument> batch = cursor.Current;
                        BsonDocument doc = batch.First();
                        f = new Friend { Name = doc["username"].AsString, IP = doc["ip"].AsString, _id = doc["_id"].AsObjectId };
                    }
                }

                if (f != null)
                {
                    BsonDocument document = new BsonDocument
                    {
                        { "requester", loginService.UserId },
                        { "receiver", f._id },
                        { "date", DateTime.Now },
                        { "accepted", true }
                    };

                        await dataBaseService.FriendCollection.InsertOneAsync(document);
                }
            }

            return f;
        }

        public void CancelCall(ObjectId friendId)
        {
            if(loginService.LoggedIn)
            {
                IServerCallBack friendCallback = loginService.GetCallbackChannelByID(friendId);
                if(friendCallback != null)
                {
                    friendCallback.OnCallCancelled(loginService.UserId);
                }

            }
        }

        public void AcceptCall(ObjectId friendId)
        {
            if (loginService.LoggedIn)
            {
                IServerCallBack friendCallback = loginService.GetCallbackChannelByID(friendId);
                if (friendCallback != null)
                {
                    friendCallback.OnCallAccepted(loginService.UserId);
                }
            }
        }

        /*public async Task<string> GetProfilePictureHash(ObjectId friendId)
        {
            IMongoCollection<BsonDocument> userCollection = dataBaseService.Database.GetCollection<BsonDocument>("users");
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", friendId);
            using (IAsyncCursor<BsonDocument> cursor = await userCollection.FindAsync(filter))
            {
                cursor.MoveNext();
                if (cursor.Current != null)
                {
                    IEnumerable<BsonDocument> batch = cursor.Current;
                    BsonDocument doc = batch.First();
                    return doc["picturehash"].AsString;
                }
            }

            return string.Empty;
        }*/
    }
}

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

namespace VoIPServer.ServerServiceLibrary
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.PerSession, UseSynchronizationContext = true)]
    public class ServerService : IServerService
    {
        private static readonly Dictionary<ObjectId, IMessageCallBack> subscribers = new Dictionary<ObjectId, IMessageCallBack>();
        private static readonly DataBaseService dataBaseService = new DataBaseService();

        static ServerService()
        {
            dataBaseService.Connect();
        }

        public async void SendMessage(Message msg)
        {
            BsonDocument document = new BsonDocument
            {
                {"sender", "127.0.0.1"},
                {"receiver", msg.FriendID},
                {"date", msg.Date },
                {"text", msg.Text }
            };

            IMongoCollection<BsonDocument> messageCollection = dataBaseService.Database.GetCollection<BsonDocument>("messsages");
            await messageCollection.InsertOneAsync(document);

            IMessageCallBack cb;
            if(subscribers.TryGetValue(msg.FriendID, out cb))
            {
                cb.OnMessageReceived(msg);
            }
        }

        public void Subscribe(ObjectId id)
        {
            IMessageCallBack callback = OperationContext.Current.GetCallbackChannel<IMessageCallBack>();
            if (!subscribers.ContainsKey(id))
            {
                subscribers.Add(id, callback);
            }

            ICommunicationObject obj = (ICommunicationObject)callback;
            obj.Closed += (s, e) =>
            {
                subscribers.Remove(id);
            };
        }

        public void Unsubscribe(ObjectId id)
        {
            if (subscribers.ContainsKey(id))
            {
                subscribers.Remove(id);
            }
        }
    }
}

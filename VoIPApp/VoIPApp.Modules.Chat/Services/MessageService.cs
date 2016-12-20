using Microsoft.Practices.Unity;
using MongoDB.Bson;
using MongoDB.Driver;
using Prism.Modularity;
using SharedCode.Models;
using SharedCode.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using VoIPApp.Common.Services;

namespace VoIPApp.Modules.Chat.Services
{
    public class MessageService : IMessageService
    {
        private readonly ServerServiceProxy serverServiceProxy;
        private readonly IMongoCollection<BsonDocument> messageCollection;
        private readonly Dictionary<ObjectId, ObservableCollection<Message>> messages;

        public MessageService(IUnityContainer container, IModuleManager moduleManager, ServerServiceProxy serverServiceProxy)
        {
            DataBaseService dataBaseService = container.Resolve<DataBaseService>();
            this.messageCollection = dataBaseService.Database.GetCollection<BsonDocument>("messages");

            this.messages = new Dictionary<ObjectId, ObservableCollection<Message>>();
            this.serverServiceProxy = serverServiceProxy;
        }

        public async Task SendMessage(Message msg)
        {
            VoIPApp.Common.ServerServiceReference.Message wcfMessage = new VoIPApp.Common.ServerServiceReference.Message { Date = msg.Date, Receiver = msg.Receiver, Sender = msg.Sender, Text = msg.Text };
            await serverServiceProxy.ServerService.SendMessageAsync(wcfMessage);
        }

        public ObservableCollection<Message> GetMessages(ObjectId _id)
        {
            ObservableCollection<Message> msg = null;
            messages.TryGetValue(_id, out msg);
            if(msg == null)
            {
                return new ObservableCollection<Message>();
            }
            return msg;
        }

        public async Task PopulateMessageDictionary()
        {
            ObjectId userId = serverServiceProxy.UserId;

            FilterDefinitionBuilder<BsonDocument>
                builder = Builders<BsonDocument>.Filter;

            FilterDefinition<BsonDocument> filter = builder.Eq("sender", userId) | builder.Eq("receiver", userId);

            using (IAsyncCursor<BsonDocument> cursor = await messageCollection.FindAsync(filter))
            {
                while (await cursor.MoveNextAsync())
                {
                    IEnumerable<BsonDocument> batch = cursor.Current;
                    foreach (BsonDocument document in batch)
                    {
                        ObjectId _id = document["sender"].AsObjectId;
                        if (_id.CompareTo(userId) == 0)
                        {
                            _id = document["receiver"].AsObjectId;
                        }

                        Message msg = new Message { Sender = userId, Receiver = _id, Date = document["date"].ToUniversalTime(), Text = document["text"].AsString };

                        ObservableCollection<Message> msgList;
                        if (messages.TryGetValue(_id, out msgList))
                        {
                            msgList.Add(msg);
                        }
                        else
                        {
                            msgList = new ObservableCollection<Message>();
                            msgList.Add(msg);
                            messages.Add(_id, msgList);
                        }
                    }
                }
            }
        }
        
    }
}

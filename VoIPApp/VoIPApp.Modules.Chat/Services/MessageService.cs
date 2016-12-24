using Microsoft.Practices.Unity;
using MongoDB.Bson;
using MongoDB.Driver;
using Prism.Events;
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
using VoIPApp.Common;
using VoIPApp.Common.Services;

namespace VoIPApp.Modules.Chat.Services
{
    public class MessageService : IMessageService
    {
        private readonly ServerServiceProxy serverServiceProxy;
        private readonly IMongoCollection<BsonDocument> messageCollection;
        private readonly Dictionary<ObjectId, ObservableCollection<Message>> messages;

        public MessageService(IUnityContainer container, IModuleManager moduleManager, ServerServiceProxy serverServiceProxy, EventAggregator eventAggregator)
        {
            DataBaseService dataBaseService = container.Resolve<DataBaseService>();
            this.messageCollection = dataBaseService.Database.GetCollection<BsonDocument>("messages");

            this.messages = new Dictionary<ObjectId, ObservableCollection<Message>>();
            this.serverServiceProxy = serverServiceProxy;

            eventAggregator.GetEvent<MessageEvent>().Subscribe(OnMessageReceived);
        }

        private void OnMessageReceived(Message obj)
        {
            ObservableCollection<Message> msgList;
            if(messages.TryGetValue(obj.Sender, out msgList))
            {
                msgList.Add(obj);
            }
        }

        public async Task SendMessage(Message msg)
        {
            await serverServiceProxy.ServerService.SendMessageAsync(msg);
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
                        if (userId.Equals(_id))
                        {
                            _id = document["receiver"].AsObjectId;
                        }

                        Message msg = new Message { Sender = document["sender"].AsObjectId, Receiver = document["receiver"].AsObjectId, Date = document["date"].ToUniversalTime(), Text = document["text"].AsString };

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

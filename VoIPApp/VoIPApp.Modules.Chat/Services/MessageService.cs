using Microsoft.Practices.Unity;
using MongoDB.Bson;
using MongoDB.Driver;
using SharedCode.Models;
using SharedCode.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using VoIPApp.Modules.Chat.ServerServiceReference;

namespace VoIPApp.Modules.Chat.Services
{
    public class MessageService : IMessageService, IDisposable, IServerServiceCallback
    {
        private readonly IServerService serverServiceClient;
        private readonly IMongoCollection<BsonDocument> messageCollection;
        private readonly Dictionary<ObjectId, ObservableCollection<Message>> messages;

        //remove
        private readonly IMongoDatabase dataBase;
        private readonly ObjectId userId;

        public MessageService(IUnityContainer container)
        {
            DataBaseService dataBaseService = container.Resolve<DataBaseService>();
            this.messageCollection = dataBaseService.Database.GetCollection<BsonDocument>("messages");
            this.dataBase = dataBaseService.Database;

            this.messages = new Dictionary<ObjectId, ObservableCollection<Message>>();

            WSDualHttpBinding binding = new WSDualHttpBinding();
            EndpointAddress endpoint = new EndpointAddress("http://localhost/VoIPServer/ServerService");
            DuplexChannelFactory<IServerService> channelFactory = new DuplexChannelFactory<IServerService>(this, binding, endpoint);

            try
            {
                serverServiceClient = channelFactory.CreateChannel();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                if(serverServiceClient != null)
                {
                    ((ICommunicationObject)serverServiceClient).Abort();
                }
            }


            //remove
            IMongoCollection<BsonDocument> friendCollection = dataBase.GetCollection<BsonDocument>("users");
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("Name", "david");
            List<BsonDocument> result = friendCollection.Find(filter).ToList();
            this.userId = result[0]["_id"].AsObjectId;
            //

            serverServiceClient.Subscribe(userId);
        }

        public async Task SendMessage(Message msg)
        {
            msg.Sender = userId;
            await serverServiceClient.SendMessageAsync(msg);
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

        public void Dispose()
        {
            serverServiceClient.Unsubscribe(userId);
            ((ICommunicationObject)serverServiceClient).Close();
        }

        public void OnMessageReceived(Message msg)
        {
            
        }

        public async Task PopulateMessageDictionary()
        {
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

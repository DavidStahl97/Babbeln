using Microsoft.Practices.Unity;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
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
        private readonly Dictionary<ObjectId, ObservableCollection<Message>> messages;
        private readonly DataBaseService dataBaseService;

        public MessageService(IUnityContainer container, IModuleManager moduleManager, ServerServiceProxy serverServiceProxy, EventAggregator eventAggregator, DataBaseService dataBaseService)
        {
            this.dataBaseService = dataBaseService;

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

            IMongoQueryable<Message> query = from message in dataBaseService.MessageCollection.AsQueryable()
                    where message.Sender.Equals(userId) || message.Receiver.Equals(userId)
                    select message;

            await query.ForEachAsync((m) =>
            {
                 ObjectId friendId = (m.Sender.Equals(userId)) ? m.Receiver : m.Sender;
                 ObservableCollection<Message> msgList;
                 if (messages.TryGetValue(friendId, out msgList))
                 {
                     msgList.Add(m);
                 }
                 else
                 {
                     msgList = new ObservableCollection<Message>();
                     msgList.Add(m);
                     messages.Add(friendId, msgList);
                 }
             });
        }
        
    }
}

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
        private readonly FriendsService friendsService;

        private ObjectId lastMessagesRead;

        public MessageService(ServerServiceProxy serverServiceProxy, EventAggregator eventAggregator, DataBaseService dataBaseService, FriendsService friendsService)
        {
            this.dataBaseService = dataBaseService;
            this.friendsService = friendsService;

            this.messages = new Dictionary<ObjectId, ObservableCollection<Message>>();
            this.serverServiceProxy = serverServiceProxy;

            eventAggregator.GetEvent<MessageEvent>().Subscribe(OnMessageReceived);

            lastMessagesRead = ObjectId.Empty;
        }

        private void OnMessageReceived(Message obj)
        {
            ObservableCollection<Message> msgList;
            if(messages.TryGetValue(obj.Sender, out msgList))
            {
                msgList.Add(obj);
                if(obj.Sender.Equals(lastMessagesRead))
                {
                    ChangeMessageToRead(obj);
                }
                else
                {
                    friendsService.GetFriendById(obj.Sender).UnreadMessages++;
                }
            }
        }

        public async Task SendMessage(Message msg)
        {
            await serverServiceProxy.ServerService.SendMessageAsync(msg);
        }

        public ObservableCollection<Message> ReadMessages(ObjectId _id)
        {
            ObservableCollection<Message> msg = null;
            messages.TryGetValue(_id, out msg);
            if(msg == null)
            {
                return new ObservableCollection<Message>();
            }

            foreach(Message message in msg.Where(message => !message.Read))
            {
                ChangeMessageToRead(message);
            }

            friendsService.GetFriendById(_id).UnreadMessages = 0;
            lastMessagesRead = _id;

            return msg;
        }

        public async Task PopulateMessageDictionary()
        {
            ObjectId userId = serverServiceProxy.UserInfo.UserID;

            List<ObjectId> friendIds = await dataBaseService.GetFriendIdList(userId);
            friendIds.ForEach((friendId) =>
            {
                messages.Add(friendId, new ObservableCollection<Message>());
            });

            IMongoQueryable<Message> query = from message in dataBaseService.MessageCollection.AsQueryable()
                    where message.Sender.Equals(userId) || message.Receiver.Equals(userId)
                    select message;

            await query.ForEachAsync((m) =>
            {
                 ObjectId friendId = (m.Sender.Equals(userId)) ? m.Receiver : m.Sender;
                 ObservableCollection<Message> msgList;
                 if (messages.TryGetValue(friendId, out msgList))
                 {
                    if(!m.Read)
                    {
                        friendsService.GetFriendById(friendId).UnreadMessages++;
                    }

                    msgList.Add(m);
                 }
             });
        }

        public void ChangeMessageToRead(Message message)
        {
            Task.Run(() =>
            {
                message.Read = true;
                FilterDefinition<Message> filter = Builders<Message>.Filter.Eq("_id", message.Id);
                UpdateDefinition<Message> update = Builders<Message>.Update.Set("read", true);
                dataBaseService.MessageCollection.UpdateOneAsync(filter, update);
            });
        }
        
    }
}

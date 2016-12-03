using MongoDB.Bson;
using SharedCode.Models;
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
    public class MessageService : IMessageService
    {

        private readonly IServerService serverServiceClient;

        public MessageService()
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            EndpointAddress endpoint = new EndpointAddress("http://localhost:1111/VoIPServer/ServerService");
            ChannelFactory<IServerService> channelFactory = new ChannelFactory<IServerService>(binding, endpoint);

            try
            {
                serverServiceClient = channelFactory.CreateChannel();
            }
            catch
            {
                if(serverServiceClient != null)
                {
                    ((ICommunicationObject)serverServiceClient).Abort();
                }
            }

            serverServiceClient.Subscribe(new ObjectId());
        }

        public async Task SendMessage(Message msg)
        {
            await serverServiceClient.SendMessageAsync(msg);
        }

        public ObservableCollection<Message> GetMessages(ObjectId _id)
        {
            return new ObservableCollection<Message>();
        }

        public void AddMessage(ObjectId _id, Message message)
        {
            
        }
    }
}

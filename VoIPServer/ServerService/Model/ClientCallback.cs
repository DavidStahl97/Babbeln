using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using SharedCode.Models;
using VoIPServer.ServerServiceLibrary.DataContract;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace VoIPServer.ServerServiceLibrary.Model
{
    public class ClientCallback : IClientCallback
    {
        public IServerCallback ServerCallback { get; private set; }
        public IWebsocketCallback WebsocketCallback { get; private set; }

        private ClientCallback(IServerCallback serverCallback)
        {
            this.ServerCallback = serverCallback;
            this.WebsocketCallback = null;
        }

        private ClientCallback(IWebsocketCallback websocketCallback)
        {
            this.WebsocketCallback = websocketCallback;
            this.ServerCallback = ServerCallback;
        }

        public static IClientCallback CreateClientCallback()
        {
            IClientCallback currentCallback;
            string contractName = OperationContext.Current.EndpointDispatcher.ContractName;
            if (contractName.Equals(nameof(IServerService)))
            {
                IServerCallback desktopClientCallback = OperationContext.Current.GetCallbackChannel<IServerCallback>();
                currentCallback = new ClientCallback(desktopClientCallback);
                Console.WriteLine("created desktop client callback");
            }
            else
            {
                IWebsocketCallback webClientCallback = OperationContext.Current.GetCallbackChannel<IWebsocketCallback>();
                currentCallback = new ClientCallback(webClientCallback);
                Console.WriteLine("created web client callback");
            }

            return currentCallback;
        }

        public void OnCall(ObjectId friendId)
        {
            if(ServerCallback != null)
            {
                ServerCallback.OnCall(friendId);
            }
            else
            {
                Console.WriteLine(String.Format("cannot call webbased client with id {0}", friendId));
            }
        }

        public void OnCallAccepted(ObjectId friendId)
        {
            if(ServerCallback != null)
            {
                ServerCallback.OnCallAccepted(friendId);
            }
            else
            {
                Console.WriteLine(String.Format("cannot accept call of webbased client with id {0}", friendId));
            }
        }

        public void OnCallCancelled(ObjectId friendId)
        {
            if(ServerCallback != null)
            {
                ServerCallback.OnCallCancelled(friendId);
            }
            else
            {
                Console.WriteLine(String.Format("caccot cancel call of webbase client with id {0}", friendId));
            }
        }

        public void OnFriendshipRequestAnswered(ObjectId friendId, bool accept)
        {
            if(ServerCallback != null)
            {
                ServerCallback.OnFriendshipRequestAnswered(friendId, accept);
            }
            else
            {
                //TO-DO implement friendsip anwsered for webbased clients
            }
        }

        public void OnFriendshipRequested(ObjectId friendId, ObjectId userId)
        {
            if(ServerCallback != null)
            {
                ServerCallback.OnFriendshipRequested(friendId);
            }
            else
            {
                string jsonAccept = String.Format(@"'type':'accept',
                                                    'data': {
                                                        'from':'{0}'
                                                        'to':'{1}'
                                                        'date'{2}'",
                                                        userId.ToString(), friendId.ToString(), string.Empty);
                WebsocketCallback.OnMessageReceived(System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.Default, jsonAccept));
            }
        }

        public void OnFriendStatusChanged(ObjectId friendId, Status status)
        {
            if(ServerCallback != null)
            {
                ServerCallback.OnFriendStatusChanged(friendId, status);
            }
            else
            {
                //TO-DO implement status changed for webbased clients
            }
        }

        public void OnMessageReceived(SharedCode.Models.Message msg)
        {
            if(ServerCallback != null)
            {
                ServerCallback.OnMessageReceived(msg);
            }
            else
            {
                string jsonMessage = Newtonsoft.Json.JsonConvert.SerializeObject(msg);
                WebsocketCallback.OnMessageReceived(System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.Default, jsonMessage));
            }
        }
    }
}

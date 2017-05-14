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

namespace ServerServiceLibrary.Model
{
    public class ClientCallback
    {
        private readonly IServerCallback desktopCallback;
        private readonly IWebsocketCallback websocketCallback;
        private ObjectId userId;

        public ClientCallback(IServerCallback serverCallback)
        {
            this.desktopCallback = serverCallback;
            this.websocketCallback = null;
            this.userId = ObjectId.Empty;
        }

        public ClientCallback(IWebsocketCallback websocketCallback, ObjectId userId)
        {
            this.userId = userId;
            this.desktopCallback = null;
            this.websocketCallback = websocketCallback;
        }

        public void OnCall(ObjectId friendId)
        {
            if(websocketCallback == null)
            {
                desktopCallback.OnCall(friendId);
            }
            else
            {
                Console.WriteLine("cannot call web client");
            }
        }

        public void OnCallAccepted(ObjectId friendId)
        {
            if(websocketCallback == null)
            {
                desktopCallback.OnCallAccepted(friendId);
            }
            else
            {
                Console.WriteLine("cannot accept call of web client");
            }
        }

        public void OnCallCancelled(ObjectId friendId)
        {
            if(websocketCallback == null)
            {
                desktopCallback.OnCallCancelled(friendId);
            }
            else
            {
                Console.WriteLine("cannot cancel call of web client");
            }
        }

        public void OnFriendshipRequestAnswered(ObjectId friendId, bool accept)
        {
            if(websocketCallback == null)
            {
                desktopCallback.OnFriendshipRequestAnswered(friendId, accept);
            }
            else
            {
                //TO-DO implement friendsip anwsered for webbased clients
            }
        }

        public void OnFriendshipRequested(ObjectId friendId)
        {
            if(websocketCallback == null)
            {
                desktopCallback.OnFriendshipRequested(friendId);
            }
            else
            {
                string jsonAccept = String.Format(@"'type':'accept',
                                                'data': {
                                                    'from':'{0}'
                                                    'to':'{1}'
                                                    'date'{2}'",
                                        userId.ToString(), friendId.ToString(), string.Empty);
                websocketCallback.OnMessageReceived(System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.Default, jsonAccept));
            }
        }

        public void OnFriendStatusChanged(ObjectId friendId, Status status)
        {
            if(websocketCallback == null)
            {
                desktopCallback.OnFriendStatusChanged(friendId, status);
            }
            else
            {
                //TO-DO implement status change for websocket
            }
        }

        public void OnFriendsUsernameChanged(ObjectId friendId, string username)
        {
            if(websocketCallback == null)
            {
                desktopCallback.OnFriendsUsernameChanged(friendId, username);
            }
            else
            {
                //TO-DO implement username change for websocket
            }
        }

        public void OnMessageReceived(SharedCode.Models.Message msg)
        {
            if(websocketCallback == null)
            {
                desktopCallback.OnMessageReceived(msg);
            }
            else
            {
                string jsonMessage = Newtonsoft.Json.JsonConvert.SerializeObject(msg);
                websocketCallback.OnMessageReceived(System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.Default, jsonMessage));
            }
        }

        public IServerCallback DesktopCallback
        {
            get { return desktopCallback; }
        }
    }
}

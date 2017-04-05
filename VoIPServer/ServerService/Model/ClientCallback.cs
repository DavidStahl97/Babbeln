using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using SharedCode.Models;
using VoIPServer.ServerServiceLibrary.DataContract;

namespace VoIPServer.ServerServiceLibrary.Model
{
    public class ClientCallback : IClientCallback
    {
        private readonly IServerCallback serverCallback;
        private readonly IWebsocketCallback websocketCallback;

        public ClientCallback(IServerCallback clientCallback)
        {
            this.serverCallback = clientCallback;
            this.websocketCallback = null;
        }

        public ClientCallback(IWebsocketCallback clientCallback)
        {
            this.serverCallback = null;
            this.websocketCallback = clientCallback;
        }

        public void OnCall(ObjectId friendId)
        {
            if(serverCallback != null)
            {
                serverCallback.OnCall(friendId);
            }
            else
            {
                Console.WriteLine(String.Format("cannot call webbased client with id {0}", friendId));
            }
        }

        public void OnCallAccepted(ObjectId friendId)
        {
            if(serverCallback != null)
            {
                serverCallback.OnCallAccepted(friendId);
            }
            else
            {
                Console.WriteLine(String.Format("cannot accept call of webbased client with id {0}", friendId));
            }
        }

        public void OnCallCancelled(ObjectId friendId)
        {
            if(serverCallback != null)
            {
                serverCallback.OnCallCancelled(friendId);
            }
            else
            {
                Console.WriteLine(String.Format("caccot cancel call of webbase client with id {0}", friendId));
            }
        }

        public void OnFriendshipRequestAnswered(ObjectId friendId, bool accept)
        {
            if(serverCallback != null)
            {
                serverCallback.OnFriendshipRequestAnswered(friendId, accept);
            }
            else
            {
                //TO-DO implement friendsip anwsered for webbased clients
            }
        }

        public void OnFriendshipRequested(ObjectId friendId)
        {
            if(serverCallback != null)
            {
                serverCallback.OnFriendshipRequested(friendId);
            }
            else
            {
                //TO-DO implement friendship request for webbased clients
            }
        }

        public void OnFriendStatusChanged(ObjectId friendId, Status status)
        {
            if(serverCallback != null)
            {
                serverCallback.OnFriendStatusChanged(friendId, status);
            }
            else
            {
                //TO-DO implement status changed for webbased clients
            }
        }

        public void OnMessageReceived(Message msg)
        {
            if(serverCallback != null)
            {
                serverCallback.OnMessageReceived(msg);
            }
            else
            {
                //TO-DO implement message receive for webbased clients
            }
        }
    }
}

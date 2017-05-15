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
using System.Net.WebSockets;

namespace ServerServiceLibrary.Model
{
    public class ClientCallback
    {
        private readonly IServerCallback desktopCallback;
        private readonly IWebsocketCallback websocketCallback;

        public ClientCallback(IServerCallback serverCallback)
        {
            this.desktopCallback = serverCallback;
            this.websocketCallback = null;
        }

        public ClientCallback(IWebsocketCallback websocketCallback)
        {
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

        public void OnFriendshipRequestAnswered(ObjectId userId, bool accept)
        {
            if(websocketCallback == null)
            {
                desktopCallback.OnFriendshipRequestAnswered(userId, accept);
            }
            else
            {
                string json = "{'type':'message','data': {'from':'" + userId.ToString() + "' 'accept':'" + accept + "'}";
                websocketCallback.OnMessageReceived(CreateMessage(json));
            }
        }

        public void OnFriendshipRequested(ObjectId userId)
        {
            if(websocketCallback == null)
            {
                desktopCallback.OnFriendshipRequested(userId);
            }
            else
            {
                string json = "{'type':'accept','data': {'from':'" + userId.ToString() + "'}";
                websocketCallback.OnMessageReceived(CreateMessage(json));
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
                string json = "{'type':'message','data': {'to':'" + msg.Sender + "' 'hour':" + msg.Hour + "' 'minute':" + msg.Minute + "' 'message':'" + msg.Text + "'}";
                websocketCallback.OnMessageReceived(CreateMessage(json));
            }
        }

        private System.ServiceModel.Channels.Message CreateMessage(string msgText)
        {
            System.ServiceModel.Channels.Message msg = ByteStreamMessage.CreateMessage(
                new ArraySegment<byte>(Encoding.UTF8.GetBytes(msgText)));

            msg.Properties["WebSocketMessageProperty"] =
                new WebSocketMessageProperty
                {
                    MessageType = WebSocketMessageType.Text
                };

            return msg;
        }
    }
}

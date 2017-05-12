using MongoDB.Bson;
using SharedCode.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using VoIPServer.ServerServiceLibrary.DataContract;

namespace ServerServiceLibrary.Model
{
    public class Websocket : IWebosocket
    {
        private readonly IWebsocketCallback websocketCallback;

        public Websocket(IWebsocketCallback websocketCallback)
        {
            this.websocketCallback = websocketCallback;
        }

        public void OnFriendshipRequestAnswered(ObjectId friendId, bool accept)
        {
            //TO-DO implement friendsip anwsered for webbased clients
        }

        public void OnFriendshipRequested(ObjectId friendId, ObjectId userId)
        {
            string jsonAccept = String.Format(@"'type':'accept',
                                                'data': {
                                                    'from':'{0}'
                                                    'to':'{1}'
                                                    'date'{2}'",
                                                    userId.ToString(), friendId.ToString(), string.Empty);
            websocketCallback.OnMessageReceived(System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.Default, jsonAccept));
        }

        public void OnFriendStatusChanged(ObjectId friendId, Status status)
        {
            //TO-DO implement status changed for webbased clients
        }

        public void OnMessageReceived(SharedCode.Models.Message msg)
        {
            string jsonMessage = Newtonsoft.Json.JsonConvert.SerializeObject(msg);
            websocketCallback.OnMessageReceived(System.ServiceModel.Channels.Message.CreateMessage(MessageVersion.Default, jsonMessage));
        }
    }
}

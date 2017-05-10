using MongoDB.Bson;
using SharedCode.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoIPServer.ServerServiceLibrary.DataContract;

namespace VoIPServer.ServerServiceLibrary.Model
{
    public interface IClientCallback
    {
        void OnCall(ObjectId friendId);

        void OnCallAccepted(ObjectId friendId);

        void OnCallCancelled(ObjectId friendId);

        void OnFriendshipRequestAnswered(ObjectId friendId, bool accept);

        void OnFriendshipRequested(ObjectId friendId, ObjectId userId);

        void OnFriendStatusChanged(ObjectId friendId, Status status);

        void OnMessageReceived(Message msg);

        IServerCallback ServerCallback { get; }

        IWebsocketCallback WebsocketCallback { get; }
    }
}

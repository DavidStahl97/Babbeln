using MongoDB.Bson;
using SharedCode.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoIPServer.ServerServiceLibrary.Model
{
    public interface IClientCallback
    {
        void OnMessageReceived(Message msg);

        void OnCall(ObjectId friendId);

        void OnCallAccepted(ObjectId friendId);

        void OnCallCancelled(ObjectId friendId);

        void OnFriendStatusChanged(ObjectId friendId, Status status);

        void OnFriendshipRequested(ObjectId friendId);

        void OnFriendshipRequestAnswered(ObjectId friendId, bool accept);
    }
}

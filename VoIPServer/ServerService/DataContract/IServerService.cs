using MongoDB.Bson;
using SharedCode.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace VoIPServer.ServerServiceLibrary.DataContract
{
    [ServiceContract(CallbackContract = typeof(IServerCallback), SessionMode = SessionMode.Required)]
    public interface IServerService
    {
        [OperationContract(IsOneWay = true)]
        Task SendMessage(Message msg);

        [OperationContract(IsOneWay = true)]
        void Call(ObjectId receiver);

        [OperationContract(IsOneWay = true)]
        void CancelCall(ObjectId friendId);

        [OperationContract(IsOneWay = true)]
        void AcceptCall(ObjectId friendId);

        [OperationContract(IsOneWay = false)]
        Task<Tuple<ObjectId, string>> Subscribe(string userName, string password, string ip);

        [OperationContract(IsOneWay = true)]
        Task Unsubscribe();

        [OperationContract(IsOneWay = false)]
        Task<string> Register(string userName, string password, string email, string ip);

        [OperationContract(IsOneWay = false)]
        Task<User> SendFriendRequest(string friendName);

        [OperationContract(IsOneWay = true)]
        Task ReplyToFriendRequest(ObjectId friendId, bool accept);

        [OperationContract(IsOneWay = true)]
        Task ChangeStatus(Status status);

        [OperationContract(IsOneWay = true)]
        Task ChangeUsername(string username);
    }

    public interface IServerCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnMessageReceived(Message msg);

        [OperationContract(IsOneWay = true)]
        void OnCall(ObjectId friendId);

        [OperationContract(IsOneWay = true)]
        void OnCallAccepted(ObjectId friendId);

        [OperationContract(IsOneWay = true)]
        void OnCallCancelled(ObjectId friendId);

        [OperationContract(IsOneWay = true)]
        void OnFriendStatusChanged(ObjectId friendId, Status status);

        [OperationContract(IsOneWay = true)]
        void OnFriendshipRequested(ObjectId friendId);

        [OperationContract(IsOneWay = true)]
        void OnFriendshipRequestAnswered(ObjectId friendId, bool accept);

        [OperationContract(IsOneWay = true)]
        void OnFriendsUsernameChanged(ObjectId friendId, string username);
    }
}

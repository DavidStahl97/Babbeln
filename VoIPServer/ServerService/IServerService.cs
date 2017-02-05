using MongoDB.Bson;
using SharedCode.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace VoIPServer.ServerServiceLibrary
{
    [ServiceContract(CallbackContract = typeof(IServerCallBack), SessionMode = SessionMode.Required)]
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
        Task<Friend> AddFriendByName(string friendName);

        [OperationContract(IsOneWay = false)]
        Task<ObjectId> Subscribe(string userName, string password, string ip);

        [OperationContract(IsOneWay = true)]
        void Unsubscribe();

        [OperationContract(IsOneWay = false)]
        Task<string> Register(string userName, string password, string email, string ip);
    }

    public interface IServerCallBack
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
    }
}

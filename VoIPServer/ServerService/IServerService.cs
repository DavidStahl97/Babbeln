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
        void SendMessage(Message msg);

        [OperationContract(IsOneWay = false)]
        bool Call(ObjectId receiver);

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

        [OperationContract(IsOneWay = false)]
        void OnCall(ObjectId id);
    }
}

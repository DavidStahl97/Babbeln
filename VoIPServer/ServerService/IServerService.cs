using MongoDB.Bson;
using SharedCode.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace VoIPServer.ServerServiceLibrary
{
    [ServiceContract(CallbackContract = typeof(IServerCallBack))]
    public interface IServerService
    {
        [OperationContract(IsOneWay =true)]
        void SendMessage(Message msg);

        [OperationContract(IsOneWay = true)]
        void Subscribe(ObjectId id);

        [OperationContract(IsOneWay = true)]
        void Unsubscribe(ObjectId id);
    }

    public interface IServerCallBack
    {
        [OperationContract(IsOneWay = true)]
        void OnMessageReceived(Message msg);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace VoIPServer.ServerServiceLibrary.DataContract
{
    [ServiceContract(CallbackContract = typeof(IWebsocketCallback), SessionMode = SessionMode.Required)]
    public interface IWebsocketService
    {
        [OperationContract(IsOneWay = true, Action = "*")]
        Task SendMessageToServer(Message msg);
    }

    [ServiceContract]
    public interface IWebsocketCallback
    {
        [OperationContract(IsOneWay = true, Action = "*")]
        void OnMessageReceived(Message msg);
    }
}

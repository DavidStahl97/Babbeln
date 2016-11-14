using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace VoiceSessionInitializerLibrary
{
    [ServiceContract]
    public interface ISessionInitializerService
    {
        [OperationContract]
        void Call(int friendID);
    }
}

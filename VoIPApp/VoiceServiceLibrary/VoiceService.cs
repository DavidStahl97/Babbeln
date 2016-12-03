using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace VoiceServiceLibrary
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = true)]
    public class VoiceService : IVoiceService
    {
        public int Call(int i)
        {
            return ++i;
        }
    }
}

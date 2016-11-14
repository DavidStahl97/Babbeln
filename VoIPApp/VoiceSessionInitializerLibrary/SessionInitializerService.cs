using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace VoiceSessionInitializerLibrary
{
    public delegate int CalledEventHandler(int friendId);

    public class SessionInitializerService : ISessionInitializerService
    {
        public event CalledEventHandler Called;

        public void Call(int friendId)
        {
            if(Called != null)
            {
                Called(friendId);
            }
        }
    }
}

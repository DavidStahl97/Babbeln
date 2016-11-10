using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoIPApp.Modules.Chat.Services
{
    public interface IVoIPService
    {
        void StartCall(string ip);
        void StopCall();
    }
}

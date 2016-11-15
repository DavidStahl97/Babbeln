using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using VoiceServiceLibrary;

namespace VoIPApp.Modules.Chat
{
    public class VoiceServiceManager
    {
        private ServiceHost serviceHost;

        public VoiceServiceManager(VoiceService voiceService)
        {
            Uri baseAddress = new Uri("http://localhost/VoIPApp/VoiceService");
            serviceHost = new ServiceHost(voiceService, baseAddress);

            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            serviceHost.Description.Behaviors.Add(smb);
        }

        public void StartVoiceService()
        {
            serviceHost.BeginOpen(new AsyncCallback(OpenCallback), null);
        }

        private void OpenCallback(IAsyncResult ar)
        {
            serviceHost.EndOpen(ar);
            Console.WriteLine("Voice service up and running at:");
            foreach (var ea in serviceHost.Description.Endpoints)
            {
                Console.WriteLine(ea.Address);
            }
        }

        public void StopVoiceService()
        {
            serviceHost.BeginClose(new AsyncCallback(CloseCallback), null);
        }

        private void CloseCallback(IAsyncResult ar)
        {
            serviceHost.EndClose(ar);

            Console.WriteLine("Voice service stopped");
        }
    }
}

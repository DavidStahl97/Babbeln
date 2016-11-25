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
    /// <summary>
    /// manages the wcf voice service, starts and destroys it
    /// </summary>
    public class VoiceServiceManager
    {
        /// <summary>
        /// wcf service connection
        /// </summary>
        private ServiceHost serviceHost;

        /// <summary>
        /// creates a new instance of the <see cref="VoiceServiceManager"/> class
        /// </summary>
        /// <param name="voiceService">injected by the <see cref="Microsoft.Practices.Unity.IUnityContainer"/>, stored in <see cref="serviceHost"/></param>
        public VoiceServiceManager(VoiceService voiceService)
        {
            Uri baseAddress = new Uri("http://localhost/VoIPApp/VoiceService");
            serviceHost = new ServiceHost(voiceService, baseAddress);

            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            serviceHost.Description.Behaviors.Add(smb);
        }

        /// <summary>
        /// opens the wcf service asynchronously
        /// </summary>
        public void StartVoiceService()
        {
            serviceHost.BeginOpen(new AsyncCallback(OpenCallback), null);
        }

        /// <summary>
        /// called when opening finished
        /// </summary>
        /// <param name="ar"></param>
        private void OpenCallback(IAsyncResult ar)
        {
            serviceHost.EndOpen(ar);
            Console.WriteLine("Voice service up and running at:");
            foreach (var ea in serviceHost.Description.Endpoints)
            {
                Console.WriteLine(ea.Address);
            }
        }

        /// <summary>
        /// stops the wcf service asynchronously
        /// </summary>
        public void StopVoiceService()
        {
            serviceHost.BeginClose(new AsyncCallback(CloseCallback), null);
        }

        /// <summary>
        /// called when closing finished
        /// </summary>
        /// <param name="ar"></param>
        private void CloseCallback(IAsyncResult ar)
        {
            serviceHost.EndClose(ar);

            Console.WriteLine("Voice service stopped");
        }
    }
}

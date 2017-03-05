using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using VoIPServer.ServerServiceLibrary;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;

namespace VoIPServer
{
    class Program
    {
        private ServiceHost websocketHost;

        static void Main(string[] args)
        {
            ServiceHost serviceHost = new ServiceHost(typeof(ServerService));

            try
            {
                serviceHost.Open();
                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <ENTER> to terminate the service.");
                foreach(var end in serviceHost.Description.Endpoints)
                {
                    Console.WriteLine(end.Address);
                }
               
                Console.WriteLine();
                Console.ReadLine();

                serviceHost.Close();
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex.Message);
                serviceHost.Abort();
                Console.ReadLine();
            }
        }

        private void createWebsocketEndpoint()
        {
            /*Uri baseAddress = new Uri("http://localhost:8080/hello");

            // Create the ServiceHost.
            using (websocketHost = new ServiceHost(typeof(WebSocketsServer), baseAddress))
            {
                // Enable metadata publishing.
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                websocketHost.Description.Behaviors.Add(smb);

                CustomBinding binding = new CustomBinding();
                binding.Elements.Add(new ByteStreamMessageEncodingBindingElement());
                HttpTransportBindingElement transport = new HttpTransportBindingElement();
                //transport.WebSocketSettings = new WebSocketTransportSettings();
                transport.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
                transport.WebSocketSettings.CreateNotificationOnConnection = true;
                binding.Elements.Add(transport);

                websocketHost.AddServiceEndpoint(typeof(IWebSocketsServer), binding, "");

                websocketHost.Open();

                // Close the ServiceHost.
                websocketHost.Close();*/
            
        }
    }
    
}

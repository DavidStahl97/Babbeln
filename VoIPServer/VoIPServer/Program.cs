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
    }
    
}

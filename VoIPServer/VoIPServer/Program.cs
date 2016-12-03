using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using VoIPServer.ServerServiceLibrary;
using System.ServiceModel.Description;
using SharedCode.Services;

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

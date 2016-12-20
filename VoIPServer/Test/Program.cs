using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Test.ServiceReference1;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                MyCallBack obj = new MyCallBack();
                obj.callService();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadLine();
        }
    }

    class MyCallBack : IServerServiceCallback, IDisposable
    {
        ServerServiceClient proxy;

        public void callService()
        {
            InstanceContext context = new InstanceContext(this);
            proxy = new ServerServiceClient(context);
            proxy.Subscribe("david", "frederickochs1", "dfsf");
        }
        public void Dispose()
        {
            proxy.Close();
        }

        public void OnCall(ObjectId id)
        {
            throw new NotImplementedException();
        }

        public void OnMessageReceived(Message msg)
        {
            throw new NotImplementedException();
        }
    }
}

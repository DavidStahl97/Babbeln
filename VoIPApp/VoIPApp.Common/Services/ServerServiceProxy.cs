using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using SharedCode.Models;
using VoIPApp.Common.ServerServiceReference;
using SharedCode.Services;
using MongoDB.Driver;
using Microsoft.Practices.Unity;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;

namespace VoIPApp.Common.Services
{
    public class ServerServiceProxy : IServerServiceCallback, IDisposable
    {
        private IServerService serverServiceClient;
        private ObjectId userId;
        private string ipAdress;

        public delegate void ProxyEventDelegate();
        public event ProxyEventDelegate MessageReceived;
        public event ProxyEventDelegate Call;

        public ServerServiceProxy()
        {
            this.userId = ObjectId.Empty;
            this.ipAdress = string.Empty;
        }

        public ObjectId UserId
        {
            get { return this.userId; }
        }

        public IServerService ServerService
        {
            get { return this.serverServiceClient; }
        }

        public bool Connect()
        {
            ipAdress = GetLocalIPAdress();
            if (string.IsNullOrEmpty(ipAdress))
            {
                return false;
            }

            try
            {
                serverServiceClient = new ServerServiceClient(new InstanceContext(this));
            }
            catch(Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> LogIn(string userName, string password)
        {
            userId = await serverServiceClient.SubscribeAsync(userName, password, ipAdress);
            if(userId.Equals(ObjectId.Empty))
            {
                return false;
            }

            Console.WriteLine("userId: " + userId.ToString());
            return true;
        }

        public async Task<string> Register(string userName, string password, string email)
        {
            return await serverServiceClient.RegisterAsync(userName, password, email, ipAdress);
        }

        public void OnCall(ObjectId id)
        {
            if(Call != null)
            {
                Call();
            }
        }

        public void OnMessageReceived(ServerServiceReference.Message msg)
        {
            if (MessageReceived != null)
            {
                MessageReceived();
            }
        }

        public void Dispose()
        {
            serverServiceClient.UnsubscribeAsync();
            ((ICommunicationObject)serverServiceClient).Close();
        }

        private string GetLocalIPAdress()
        {
            string localIP = string.Empty;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("10.0.2.4", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }

            return localIP;
        }
    }
}

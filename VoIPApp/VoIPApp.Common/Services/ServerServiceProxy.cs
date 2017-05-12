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
using Prism.Events;
using VoIPApp.Common.Models;

namespace VoIPApp.Common.Services
{
    //TODO: rearange this project
    //TODO: only put interfaces of services her
    public class ServerServiceProxy : IServerServiceCallback
    {
        private IServerService serverServiceClient;
        private readonly EventAggregator eventAggregator;
        private AccountDetails accountDetails;

        public ServerServiceProxy(EventAggregator eventAggregator)
        {
            this.accountDetails = new AccountDetails();
            this.eventAggregator = eventAggregator;
        }

        public IServerService ServerService
        {
            get { return this.serverServiceClient; }
        }

        public bool Connect()
        {
            accountDetails.IP = GetLocalIPAdress();
            if (string.IsNullOrEmpty(accountDetails.IP))
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
            accountDetails.UserID = await serverServiceClient.SubscribeAsync(userName, password, accountDetails.IP);
            if(accountDetails.UserID.Equals(ObjectId.Empty))
            {
                return false;
            }

            Console.WriteLine("userId: " + accountDetails.UserID.ToString());
            UserInfo.Username = userName;
            return true;
        }

        public async Task<string> Register(string userName, string password, string email)
        {
            return await serverServiceClient.RegisterAsync(userName, password, email, accountDetails.IP);
        }

        public void OnCall(ObjectId friendId)
        {
            eventAggregator.GetEvent<CallEvent>().Publish(friendId);
        }

        public void OnMessageReceived(Message msg)
        {
            eventAggregator.GetEvent<MessageEvent>().Publish(msg);
        }

        public void OnCallAccepted(ObjectId friendId)
        {
            eventAggregator.GetEvent<AcceptedCallEvent>().Publish(friendId);
        }

        public void OnCallCancelled(ObjectId friendId)
        {
            eventAggregator.GetEvent<CanceledCallEvent>().Publish(friendId);
        }

        public void OnFriendStatusChanged(ObjectId friendId, Status status)
        {
            eventAggregator.GetEvent<FriendStatusChangedEvent>().Publish(new FriendStatusChangedEventArgs { FriendId = friendId, Status = status });
        }

        public void OnFriendshipRequested(ObjectId friendId)
        {
            eventAggregator.GetEvent<FriendshipRequestedEvent>().Publish(friendId);
        }

        public void OnFriendshipRequestAnswered(ObjectId friendId, bool accept)
        {
            eventAggregator.GetEvent<FriendshipRequestAnswerdEvent>().Publish(new FriendshipRequestAnsweredEventArgs { FriendId = friendId, Accepted = accept });
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

        public AccountDetails UserInfo
        {
            get { return accountDetails; }
        }
    }
}

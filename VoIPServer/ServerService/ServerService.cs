using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using ServerServiceLibrary;
using SharedCode.Models;
using SharedCode.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using VoIPServer.ServerServiceLibrary.Services;
using System.ServiceModel.Channels;

namespace VoIPServer.ServerServiceLibrary
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, UseSynchronizationContext = true)]
    [ErrorHandlerExtension]
    public class ServerService : IServerService, IWebsocketService
    {
        private static readonly DataBaseService dataBaseService = new DataBaseService();
        private readonly LoginService loginService;
        private readonly FriendService friendService;
        private readonly ChatService chatService;
        private static int instanceCount = 0;

        private readonly IServerCallBack currentCallback;
        private int connectionId;

        static ServerService()
        {
            dataBaseService.Connect();
        }

        public ServerService()
        {
            connectionId = instanceCount;
            instanceCount++;
            Console.WriteLine("New Server Instance was Created with id: " + instanceCount);

            currentCallback = OperationContext.Current.GetCallbackChannel<IServerCallBack>();
            loginService = new LoginService(dataBaseService);
            chatService = new ChatService(loginService, dataBaseService);
            friendService = new FriendService(dataBaseService, loginService);
        }

        public async Task<ObjectId> Subscribe(string userName, string password, string ip)
        {
            ICommunicationObject obj = (ICommunicationObject)currentCallback;
            obj.Closed += async (s, e) =>
            {
                await loginService.Unsubscribe();
            };

            return await loginService.Subscribe(userName, password, ip, currentCallback);
        } 

        public async Task Unsubscribe()
        {
            await loginService.Unsubscribe();
        }

        public async Task<string> Register(string userName, string password, string email, string ip)
        {
            return await loginService.Register(userName, password, email, ip);
        }

        public async Task SendMessage(SharedCode.Models.Message msg)
        {
            if(loginService.LoggedIn)
            {
                await chatService.SendMessage(msg);
            }
        }

        public void Call(ObjectId receiver)
        {
            if(loginService.LoggedIn)
            {
                chatService.Call(receiver);
            }
        }

        public void CancelCall(ObjectId friendId)
        {
            if(loginService.LoggedIn)
            {
                chatService.CancelCall(friendId);
            }
        }

        public void AcceptCall(ObjectId friendId)
        {
            if (loginService.LoggedIn)
            {
                chatService.AcceptCall(friendId);
            }
        }

        public async Task<User> SendFriendRequest(string friendName)
        {
           if(loginService.LoggedIn)
            {
                return await friendService.AddFriendByName(friendName);
            }
           else
            {
                return null;
            }
        }

        public async Task ReplyToFriendRequest(ObjectId friendId, bool accept)
        {
            if(loginService.LoggedIn)
            {
                await friendService.ReplyToFriendRequest(friendId, accept);
            }
        }

        //TODO: implement sendmessagetoserver correctly
        public async Task SendMessageToServer(System.ServiceModel.Channels.Message msg)
        {
            if(msg.IsEmpty)
            {
                return;
            }

            byte[] body = msg.GetBody<byte[]>();
            string msgTextFromClient = Encoding.UTF8.GetString(body);
        }
    }
}

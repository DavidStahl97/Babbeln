using MongoDB.Bson;
using ServerServiceLibrary;
using SharedCode.Models;
using SharedCode.Services;
using System;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using VoIPServer.ServerServiceLibrary.Services;
using Newtonsoft.Json.Linq;
using VoIPServer.ServerServiceLibrary.DataContract;
using VoIPServer.ServerServiceLibrary.Model;

namespace VoIPServer.ServerServiceLibrary
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, UseSynchronizationContext = true)]
    [ErrorHandlerExtension]
    public class ServerService : IServerService, IWebsocketService
    {
        private const string Message = "message";
        private const string FriendshipRequest = "request";
        private const string FriendshipAccept = "accept";
        private const string Login = "login";

        private static readonly DataBaseService dataBaseService = new DataBaseService();
        private readonly LoginService loginService;
        private readonly FriendService friendService;
        private readonly ChatService chatService;
        private static int instanceCount = 0;

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

            loginService = new LoginService(dataBaseService);
            chatService = new ChatService(loginService, dataBaseService);
            friendService = new FriendService(dataBaseService, loginService);
        }

        public async Task<ObjectId> Subscribe(string userName, string password, string ip)
        {
            IClientCallback currentCallback = CreateCallback();
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
        
        public async Task SendMessageToServer(System.ServiceModel.Channels.Message msg)
        {
            if(msg.IsEmpty)
            {
                Console.WriteLine("received empty message");
                return;
            }

            byte[] body = msg.GetBody<byte[]>();
            string clientMessage = Encoding.UTF8.GetString(body);

            JObject jsonMessage = JObject.Parse(clientMessage);
            string type = jsonMessage["type"].ToString();

            JToken data = jsonMessage["data"];

            if(type.Equals(Login))
            {
                IClientCallback webCallback = CreateCallback();
                await loginService.Subscribe(data, webCallback);
            }
            else if(loginService.LoggedIn)
            {
                switch (type)
                {
                    case Message:
                        await chatService.SendMessage(data);
                        break;

                    case FriendshipRequest:
                        friendService.AddFriend(data);
                        break;

                    case FriendshipAccept:
                        //TO-DO implement Friendsipaccept
                        break;

                    default:
                        Console.WriteLine(String.Format("{0} type is not supported", type));
                        break;
                }
            }
        }

        private IClientCallback CreateCallback()
        {
            IClientCallback currentCallback = ClientCallback.CreateClientCallback();

            ICommunicationObject obj;
            IServerCallback sc = currentCallback.ServerCallback;
            if (sc != null)
            {
                obj = (ICommunicationObject)sc;
            }
            else
            {
                obj = (ICommunicationObject)currentCallback.WebsocketCallback;
            }

            obj.Closed += async (s, e) =>
            {
                await loginService.Unsubscribe();
            };

            return currentCallback;
        }
    }
}

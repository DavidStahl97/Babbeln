using MongoDB.Bson;
using SharedCode.Models;
using SharedCode.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoIPServer.ServerServiceLibrary;

namespace VoIPServer.ServerServiceLibrary.Services
{
    public class ChatService
    {
        private readonly LoginService loginService;
        private readonly DataBaseService dataBaseService;

        public ChatService(LoginService loginService, DataBaseService dataBaseService)
        {
            this.loginService = loginService;
            this.dataBaseService = dataBaseService;
        }

        public async Task SendMessage(Message msg)
        {
            if (loginService.LoggedIn)
            {
                await dataBaseService.MessageCollection.InsertOneAsync(msg);

                IServerCallBack receiverCallback = loginService.GetCallbackChannelByID(msg.Receiver);
                if (receiverCallback != null)
                {
                    receiverCallback.OnMessageReceived(msg);
                }
            }
        }

        public void Call(ObjectId receiver)
        {
            if (loginService.LoggedIn)
            {
                IServerCallBack receiverCallback = loginService.GetCallbackChannelByID(receiver);
                if (receiver != null)
                {
                    receiverCallback.OnCall(loginService.UserId);
                }
            }
        }

        public void CancelCall(ObjectId friendId)
        {
            if (loginService.LoggedIn)
            {
                IServerCallBack friendCallback = loginService.GetCallbackChannelByID(friendId);
                if (friendCallback != null)
                {
                    friendCallback.OnCallCancelled(loginService.UserId);
                }

            }
        }

        public void AcceptCall(ObjectId friendId)
        {
            if (loginService.LoggedIn)
            {
                IServerCallBack friendCallback = loginService.GetCallbackChannelByID(friendId);
                if (friendCallback != null)
                {
                    friendCallback.OnCallAccepted(loginService.UserId);
                }
            }
        }
    }
}

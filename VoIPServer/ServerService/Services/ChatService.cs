﻿using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using ServerServiceLibrary.Model;
using SharedCode.Models;
using SharedCode.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoIPServer.ServerServiceLibrary;
using VoIPServer.ServerServiceLibrary.DataContract;

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

        public async Task SendMessage(JToken data)
        {
            ObjectId receiver = ObjectId.Parse(data["to"].ToString());
            string message = data["message"].ToString();

            Message msg = new Message { Sender = loginService.UserId, Receiver = receiver, Hour = String.Format("{0:00}",DateTime.Now.Hour), Minute = String.Format("{0:00}", DateTime.Now.Minute), Read = false, Text =  message};
            await dataBaseService.MessageCollection.InsertOneAsync(msg);

            ClientCallback receiverCallback = loginService.GetCallbackChannelByID(msg.Receiver);
            if (receiverCallback != null)
            {
                receiverCallback.OnMessageReceived(msg);
            }
        }

        public async Task SendMessage(Message msg)
        {
            if (loginService.LoggedIn)
            {
                msg.Read = false;
                await dataBaseService.MessageCollection.InsertOneAsync(msg);

                ClientCallback receiverCallback = loginService.GetCallbackChannelByID(msg.Receiver);
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
                ClientCallback receiverCallback = loginService.GetCallbackChannelByID(receiver);
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
                ClientCallback friendCallback = loginService.GetCallbackChannelByID(friendId);
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
                ClientCallback friendCallback = loginService.GetCallbackChannelByID(friendId);
                if (friendCallback != null)
                {
                    friendCallback.OnCallAccepted(loginService.UserId);
                }
            }
        }
    }
}

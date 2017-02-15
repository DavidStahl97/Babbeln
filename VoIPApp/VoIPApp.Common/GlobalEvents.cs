﻿using MongoDB.Bson;
using Prism.Events;
using SharedCode.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoIPApp.Common
{
    public class FriendStatusChangedEventArgs
    {
        public ObjectId FriendId { get; set; }
        public Status Status { get; set; }
    }

    public class MessageEvent : PubSubEvent<Message>
    {
    }

    public class CallEvent : PubSubEvent<ObjectId>
    {
    }

    public class AcceptedCallEvent : PubSubEvent<ObjectId>
    {
    }

    public class CanceledCallEvent : PubSubEvent<ObjectId>
    {
    }

    public class FriendStatusChangedEvent : PubSubEvent<FriendStatusChangedEventArgs>
    {
    }
}

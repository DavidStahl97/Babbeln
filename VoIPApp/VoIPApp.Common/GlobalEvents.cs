using Prism.Events;
using SharedCode.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoIPApp.Common
{
    public class MessageEvent : PubSubEvent<Message>
    {
    }

    public class CallEvent : PubSubEvent<Friend>
    {
    }
}

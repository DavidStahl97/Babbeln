using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoIPApp.Common.Models;

namespace VoIPApp.Modules.Chat.Services
{
    public interface IChatService
    {
        Dictionary<int, Friend> Friends { get; set; }
        ObservableCollection<Message> GetMessages(int id);
        void AddMessage(int id, Message message);
    }
}

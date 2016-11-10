using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoIPApp.Common.Models;

namespace VoIPApp.Modules.Chat.Services
{
    public class MessageService : IMessageService
    {
        //remove later
        public Dictionary<int, ObservableCollection<Message>> Messages { get; set; }

        public MessageService()
        {
            Messages = new Dictionary<int, ObservableCollection<Message>>();

            ObservableCollection<Message> messages = new ObservableCollection<Message>();
            messages.Add(new Message { Text = "Hallo Dies ist nur ein ausgedachter Chat.", Minute = 20, Hour = 20, FriendID = -1 });
            messages.Add(new Message { Text = "Hallo Dies ist nur ein ausgedachter Chat.", Minute = 28, Hour = 5, FriendID = -1 });
            Messages.Add(0, messages);

            ObservableCollection<Message> messages2 = new ObservableCollection<Message>();
            messages2.Add(new Message { Text = "Hallo Dies ist nur ein ausgedachter Chat.", Minute = 20, Hour = 20, FriendID = -1 });
            messages2.Add(new Message { Text = "Der spätere Text wird nämlich noch kommen.", Minute = 2, Hour = 20, FriendID = 20 });
            messages2.Add(new Message { Text = "WEnn der Phillip die Datenbank für die Narichten erstellt hat.", Minute = 10, Hour = 5, FriendID = 3 });
            messages2.Add(new Message { Text = "Hallo Dies ist nur ein ausgedachter Chat.", Minute = 28, Hour = 5, FriendID = -1 });
            Messages.Add(1, messages2);

            ObservableCollection<Message> messages3 = new ObservableCollection<Message>();
            messages3.Add(new Message { Text = "Der spätere Text wird nämlich noch kommen.", Minute = 2, Hour = 20, FriendID = 20 });
            messages3.Add(new Message { Text = "WEnn der Phillip die Datenbank für die Narichten erstellt hat.", Minute = 10, Hour = 5, FriendID = 3 });
            messages3.Add(new Message { Text = "Hallo Dies ist nur ein ausgedachter Chat.", Minute = 28, Hour = 5, FriendID = -1 });
            Messages.Add(2, messages3);

            ObservableCollection<Message> messages4 = new ObservableCollection<Message>();
            messages4.Add(new Message { Text = "Hallo Gundi", Minute = 20, Hour = 20, FriendID = -1 });
            messages4.Add(new Message { Text = "Der spätere Text wird nämlich noch kommen. Hallo Phillip", Minute = 2, Hour = 20, FriendID = 20 });
            messages4.Add(new Message { Text = "XD", Minute = 10, Hour = 5, FriendID = 3 });
            messages4.Add(new Message { Text = "Hallo Dies ist nur ein ausgedachter Chat.", Minute = 28, Hour = 5, FriendID = -1 });
            Messages.Add(3, messages4);

            ObservableCollection<Message> messages5 = new ObservableCollection<Message>();
            messages5.Add(new Message { Text = "Hallo Dies ist nur ein ausgedachter Chat.", Minute = 20, Hour = 20, FriendID = -1 });
            messages5.Add(new Message { Text = "Hallo Dies ist nur ein ausgedachter Chat.", Minute = 28, Hour = 5, FriendID = -1 });
            Messages.Add(4, messages5);
        }

        public ObservableCollection<Message> GetMessages(int id)
        {
            return Messages[id];
        }

        public void AddMessage(int id, Message message)
        {
            Messages[id].Add(message);
        }
    }
}

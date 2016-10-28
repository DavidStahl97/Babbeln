using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using VoIPApp.Common.Models;
using VoIPApp.Modules.Chat.Services;

namespace VoIPApp.Modules.Chat.ViewModels
{
    public class ChatViewModel : BindableBase
    {
        private readonly IChatService chatService;
        private readonly ListCollectionView friends;
        private readonly ObservableCollection<Message> messages;
        private readonly DelegateCommand<object> sendCommand;
        private readonly DelegateCommand<object> callCommand;
        private readonly DelegateCommand<object> searchTextBoxChanged;
        private readonly DelegateCommand<object> messageTextBoxChanged;
        private readonly InteractionRequest<VoiceChatViewModel> showVoiceChatRequest;
        private readonly IUnityContainer container;
        private int currentFriendID;
        private bool calling;

        public ChatViewModel(IChatService chatService, IUnityContainer container)
        {
            if(chatService == null)
            {
                throw new ArgumentNullException("chatService");
            }

            if(container == null)
            {
                throw new ArgumentNullException("container");
            }

            this.container = container;
            this.chatService = chatService;

            friends = new ListCollectionView(chatService.Friends.Values.ToList());
            messages = new ObservableCollection<Message>();

            this.sendCommand = new DelegateCommand<object>(this.OnSend, this.CanSend);
            this.callCommand = new DelegateCommand<object>(this.OnCall, this.CanCall);
            this.searchTextBoxChanged = new DelegateCommand<object>(this.OnSearchTextBoxChanged);
            this.messageTextBoxChanged = new DelegateCommand<object>(this.OnMessageTextBoxChanged);
            this.showVoiceChatRequest = new InteractionRequest<VoiceChatViewModel>();

            Friends.CurrentChanged += SelectedFriendChanged;
        }

        public ICollectionView Friends
        {
            get { return this.friends; }
        }

        public ObservableCollection<Message> Messages
        {
            get { return this.messages; }
        }

        public ICommand SendCommand
        {
            get { return this.sendCommand; }
        }

        public ICommand CallCommand
        {
            get { return this.callCommand; }
        }

        public ICommand SearchTextBoxChanged
        {
            get { return this.searchTextBoxChanged; }
        }

        public ICommand MessageTextBoxChanged
        {
            get { return this.messageTextBoxChanged; }
        }

        public IInteractionRequest ShowVoiceChatRequest
        {
            get { return this.showVoiceChatRequest; }
        }

        private void OnMessageTextBoxChanged(object obj)
        {
            (SendCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
        }

        private void OnSearchTextBoxChanged(object obj)
        {
            TextChangedEventArgs arg = obj as TextChangedEventArgs;
            TextBox tb = arg.Source as TextBox;
            string filter = tb.Text;

            this.Friends.Filter = item =>
            {
                Friend friend = item as Friend;
                if (friend == null) return false;

                CultureInfo culture = new CultureInfo("en-US");
                return culture.CompareInfo.IndexOf(friend.Name, filter, CompareOptions.IgnoreCase) >= 0;
            };

            friends.CustomSort = new CustomSorter(filter);

            Friends.MoveCurrentToPosition(0);
        }

        private void SelectedFriendChanged(object sender, EventArgs e)
        {
            Messages.Clear();

            try
            {
                currentFriendID = (Friends.CurrentItem as Friend).ID;
                Messages.AddRange(chatService.GetMessages(currentFriendID));
            }
            catch(NullReferenceException ex) { }

            callCommand.RaiseCanExecuteChanged();
        }

        private void OnSend(object arg)
        {
            string userMessage = arg as string;
            Message message = new Message { Text = userMessage, FriendID = -1 };
            chatService.AddMessage(currentFriendID, message);
            Messages.Add(message);
        }

        private bool CanSend(object arg)
        {
            string userMessage = arg as string;

            if (!string.IsNullOrEmpty(userMessage))
            {
                return true;
            }

            return false;
        }

        private bool CanCall(object arg)
        {
            try
            {
                Friend currentFriend = Friends.CurrentItem as Friend;
                if (currentFriend.CurrentStatus == Status.Online && !calling)
                {
                    return true;
                }
            }
            catch (NullReferenceException ex) { }

            return false;
        }

        private void OnCall(object obj)
        {
            calling = true;
            callCommand.RaiseCanExecuteChanged();

            VoiceChatViewModel voiceChatViewModel = this.container.Resolve<VoiceChatViewModel>();

            voiceChatViewModel.Title = "Call " + (friends.CurrentItem as Friend).Name;
            this.showVoiceChatRequest.Raise(
                voiceChatViewModel,
                finishCall =>
                {
                    voiceChatViewModel.StopStreaming();
                    calling = false;
                    callCommand.RaiseCanExecuteChanged();
                });

            voiceChatViewModel.StartStreaming(chatService.GetFriendIP(currentFriendID));
        }
    }
}

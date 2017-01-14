using Microsoft.Practices.Unity;
using MongoDB.Bson;
using MongoDB.Driver;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Modularity;
using Prism.Mvvm;
using SharedCode.Models;
using SharedCode.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using VoIPApp.Common;
using VoIPApp.Common.Services;
using VoIPApp.Modules.Chat.Services;

namespace VoIPApp.Modules.Chat.ViewModels
{
    public class ChatViewModel : BindableBase
    {
        private readonly IFriendsService friendsService;
        private readonly IMessageService messageService;
        private readonly ListCollectionView friends;
        private readonly ObservableCollection<Message> messages;
        private readonly DelegateCommand<object> sendCommand;
        private readonly DelegateCommand<object> callCommand;
        private readonly DelegateCommand<object> searchTextBoxChanged;
        private readonly DelegateCommand<object> messageTextBoxChanged;
        private readonly DelegateCommand<object> addFriendCommand;
        private readonly DelegateCommand<object> windowLoadedCommand;
        private readonly InteractionRequest<VoiceChatViewModel> showVoiceChatRequest;
        private readonly IUnityContainer container;
        private ObjectId currentFriendID;
        private ObjectId userId;
        private bool calling;

        public ChatViewModel(FriendsService friendsService, MessageService messageService, IUnityContainer container, EventAggregator eventAggregator)
        {
            if(friendsService == null)
            {
                throw new ArgumentNullException("chatService");
            }

            if(container == null)
            {
                throw new ArgumentNullException("container");
            }

            if(messageService == null)
            {
                throw new ArgumentNullException("messageService");
            }

            this.container = container;
            this.friendsService = friendsService;
            this.messageService = messageService;

            friends = new ListCollectionView(friendsService.Friends);
            messages = new ObservableCollection<Message>();

            this.sendCommand = DelegateCommand<object>.FromAsyncHandler(OnSend, CanSend);
            this.callCommand = new DelegateCommand<object>(this.OnCall, this.CanCall);
            this.windowLoadedCommand = DelegateCommand<object>.FromAsyncHandler(this.OnWindowLoaded);
            this.searchTextBoxChanged = new DelegateCommand<object>(this.OnSearchTextBoxChanged);
            this.messageTextBoxChanged = new DelegateCommand<object>(this.OnMessageTextBoxChanged);
            this.addFriendCommand = new DelegateCommand<object>(this.OnAddFriend);
            this.showVoiceChatRequest = new InteractionRequest<VoiceChatViewModel>();

            Friends.CurrentChanged += SelectedFriendChanged;

            this.userId = container.Resolve<ServerServiceProxy>().UserId;

            eventAggregator.GetEvent<MessageEvent>().Subscribe(OnMessageReceived, ThreadOption.UIThread, true);
            eventAggregator.GetEvent<CallEvent>().Subscribe(OnIncomingCall, ThreadOption.UIThread, true);
        }

        public ObjectId UserID
        {
            get { return this.userId; }
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

        public ICommand AddFriendCommand
        {
            get { return this.addFriendCommand; }
        }

        public ICommand WindowLoadedCommand
        {
            get { return this.windowLoadedCommand; }
        }

        public IInteractionRequest ShowVoiceChatRequest
        {
            get { return this.showVoiceChatRequest; }
        }

        private void OnMessageTextBoxChanged(object obj)
        {
            sendCommand.RaiseCanExecuteChanged();
        }

        private async Task OnWindowLoaded(object arg)
        {
            await friendsService.UpdateFriendsList();
            await messageService.PopulateMessageDictionary();
            friendsService.UpdateProfilePictures();
        }

        private void OnAddFriend(object obj)
        {
            string friendName = obj as string;
            friendsService.AddFriendByName(friendName);
        }


        private void OnMessageReceived(Message obj)
        {
            if(currentFriendID.Equals(obj.Sender))
            {
                Messages.Add(obj);
            }
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

            Friend currentFriend = (Friends.CurrentItem as Friend);
            if(currentFriend != null)
            {
                currentFriendID = currentFriend._id;
                Messages.AddRange(messageService.GetMessages(currentFriendID));
            }

            callCommand.RaiseCanExecuteChanged();
        }

        private async Task OnSend(object arg)
        {
            string userMessage = arg as string;
            Message message = new Message { Text = userMessage, Receiver = currentFriendID, Date = DateTime.Now, Sender = userId };
            await messageService.SendMessage(message);
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
            Friend currentFriend = Friends.CurrentItem as Friend;
            if(currentFriend != null)
            {
                if (currentFriend.Status == Status.Online.ToString() && !calling)
                {
                    return true;
                }
            }
            return false;
        }

        private void OnCall(object obj)
        {
            VoiceChatViewModel voiceChatViewModel = this.container.Resolve<VoiceChatViewModel>();
            voiceChatViewModel.Title = (friends.CurrentItem as Friend).Name + " anrufen";
            voiceChatViewModel.IncomingCall = false;
            voiceChatViewModel.CallPartner = (friends.CurrentItem as Friend);
            voiceChatViewModel.CanAccept = false;
            voiceChatViewModel.CallAccepted = false;

            OpenCallDialog(voiceChatViewModel);
        }


        private void OnIncomingCall(ObjectId obj)
        {
            Friend caller = friendsService.GetFriendById(obj);
            if(caller != null)
            {
                VoiceChatViewModel viewModel = this.container.Resolve<VoiceChatViewModel>();
                viewModel.Title = "Anruf von " + caller.Name;
                viewModel.IncomingCall = true;
                viewModel.CallPartner = caller;
                viewModel.CanAccept = true;
                viewModel.CallAccepted = true;

                OpenCallDialog(viewModel); 
            }
        }

        private void OpenCallDialog(VoiceChatViewModel viewModel)
        {
            calling = true;
            callCommand.RaiseCanExecuteChanged();

            this.showVoiceChatRequest.Raise(
                viewModel,
                finishCall =>
                {
                    calling = false;
                    callCommand.RaiseCanExecuteChanged();
                });
        }
    }
}

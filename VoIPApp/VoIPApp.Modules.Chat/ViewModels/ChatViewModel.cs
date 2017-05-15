using Microsoft.Practices.Unity;
using MongoDB.Bson;
using MongoDB.Driver;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
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
        private readonly DelegateCommand<object> acceptFriendshipCommand;
        private readonly DelegateCommand<object> declineFriendshipCommand;
        private readonly DelegateCommand<object> navigateToProfileCommand;
        private readonly DelegateCommand<object> deleteFriendCommand;
        private readonly InteractionRequest<VoiceChatViewModel> showVoiceChatRequest;
        private readonly IUnityContainer container;
        private readonly IRegionManager regionManager;
        private ObjectId userId;
        private User currentFriend;
        private bool calling;
        private bool populatedChatView;
        private bool showFriendshipInfo;
        private bool showFriendshipRequest;
        private string username;


        public ChatViewModel(FriendsService friendsService, MessageService messageService, IUnityContainer container, EventAggregator eventAggregator, IRegionManager regionManager, ServerServiceProxy serverServie)
        {
            if (friendsService == null)
            {
                throw new ArgumentNullException("chatService");
            }

            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            if (messageService == null)
            {
                throw new ArgumentNullException("messageService");
            }

            username = serverServie.UserInfo.Username;

            this.container = container;
            this.regionManager = regionManager;
            this.friendsService = friendsService;
            this.messageService = messageService;

            friends = new ListCollectionView(friendsService.Friends);
            messages = new ObservableCollection<Message>();

            this.sendCommand = DelegateCommand<object>.FromAsyncHandler(OnSend, CanSend);
            this.callCommand = new DelegateCommand<object>(this.OnCall, this.CanCall);
            this.windowLoadedCommand = DelegateCommand<object>.FromAsyncHandler(this.OnWindowLoaded);
            this.searchTextBoxChanged = new DelegateCommand<object>(this.OnSearchTextBoxChanged);
            this.messageTextBoxChanged = new DelegateCommand<object>(this.OnMessageTextBoxChanged);
            this.addFriendCommand = DelegateCommand<object>.FromAsyncHandler(this.OnAddFriend);
            this.acceptFriendshipCommand = DelegateCommand<object>.FromAsyncHandler(this.OnAcceptFriendship);
            this.declineFriendshipCommand = DelegateCommand<object>.FromAsyncHandler(this.OnDeclineFriendship);
            this.showVoiceChatRequest = new InteractionRequest<VoiceChatViewModel>();
            this.navigateToProfileCommand = new DelegateCommand<object>(OnNavigateToProfile, CanNavigateToProfile);
            this.deleteFriendCommand = DelegateCommand<object>.FromAsyncHandler(OnDeleteFriend, CanDeleteFriend);

            FriendsCollectionView.CurrentChanged += SelectedFriendChanged;

            this.userId = container.Resolve<ServerServiceProxy>().UserInfo.UserID;

            eventAggregator.GetEvent<MessageEvent>().Subscribe(OnMessageReceived, ThreadOption.UIThread, true);
            eventAggregator.GetEvent<CallEvent>().Subscribe(OnIncomingCall, ThreadOption.UIThread, true);
            eventAggregator.GetEvent<FriendStatusChangedEvent>().Subscribe(OnFriendStatusChanged, ThreadOption.UIThread, true);
            eventAggregator.GetEvent<FriendshipRequestAnswerdEvent>().Subscribe(OnFriendshipRequestAnswered, ThreadOption.UIThread, true);

            showFriendshipInfo = false;
            showFriendshipRequest = false;
        }

        public ObjectId UserID
        {
            get { return this.userId; }
        }

        public bool ShowFriendshipInfo
        {
            get { return this.showFriendshipInfo; }
            set { SetProperty(ref this.showFriendshipInfo, value); }
        }

        public bool ShowFriendshipRequest
        {
            get { return this.showFriendshipRequest; }
            set { SetProperty(ref this.showFriendshipRequest, value); }
        }

        public ObservableCollection<Message> Messages
        {
            get { return this.messages; }
        }

        public ICommand DeleteFriendCommand
        {
            get { return this.deleteFriendCommand; }
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

        public ICommand AcceptFriendshipCommand
        {
            get { return this.acceptFriendshipCommand; }
        }

        public ICommand DeclineFriendshipCommand
        {
            get { return this.declineFriendshipCommand; }
        }

        public ICommand NavigateToProfileCommand
        {
            get { return this.navigateToProfileCommand; }
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
            if (!populatedChatView)
            {
                await friendsService.PopulateFriendList();
                await messageService.PopulateMessageDictionary();
                friendsService.UpdateProfilePictures();

                populatedChatView = true;
            }
        }

        private async Task OnAddFriend(object obj)
        {
            string friendName = obj as string;
            if(!friendName.Equals(username))
            {
                bool successfull = await friendsService.SendFriendRequest(friendName);
                FriendsCollectionView.MoveCurrentToLast();
            }
        }


        private void OnMessageReceived(Message obj)
        {
            if (currentFriend != null && currentFriend._id.Equals(obj.Sender))
            {
                Messages.Add(obj);
            }
        }

        public ICollectionView FriendsCollectionView
        {
            get { return this.friends; }
        }

        private void OnSearchTextBoxChanged(object obj)
        {
            TextChangedEventArgs arg = obj as TextChangedEventArgs;
            TextBox tb = arg.Source as TextBox;
            string filter = tb.Text;

            this.FriendsCollectionView.Filter = item =>
            {
                User friend = item as User;
                if (friend == null) return false;

                CultureInfo culture = new CultureInfo("en-US");
                return culture.CompareInfo.IndexOf(friend.Name, filter, CompareOptions.IgnoreCase) >= 0;
            };

            friends.CustomSort = new CustomSorter(filter);

            FriendsCollectionView.MoveCurrentToPosition(0);
        }

        private void SelectedFriendChanged(object sender, EventArgs e)
        {
            Messages.Clear();

            currentFriend = (FriendsCollectionView.CurrentItem as User);
            if (currentFriend != null)
            {
                Messages.AddRange(messageService.ReadMessages(currentFriend._id));

                if (!currentFriend.Friendship.Accepted)
                {
                    if (currentFriend.Friendship.Receiver.Equals(userId))
                    {
                        ShowFriendshipRequest = true;
                        ShowFriendshipInfo = false;
                    }
                    else
                    {
                        ShowFriendshipInfo = true;
                        ShowFriendshipRequest = false;
                    }
                }
                else
                {
                    ShowFriendshipRequest = false;
                    ShowFriendshipInfo = false;
                }
            }
            else
            {
                ShowFriendshipRequest = false;
                ShowFriendshipInfo = false;
            }

            RaiseCanExecuteFriendNavigationButtons();
        }

        private async Task OnSend(object arg)
        {
            string userMessage = arg as string;
            Message message = new Message { Text = userMessage, Receiver = currentFriend._id, Hour = String.Format("{0:00}", DateTime.Now.Hour), Minute = String.Format("{0:00}", DateTime.Now.Minute), Sender = userId };
            await messageService.SendMessage(message);
            Messages.Add(message);
        }

        private bool CanSend(object arg)
        {
            string userMessage = arg as string;

            if (!string.IsNullOrEmpty(userMessage))
            {
                if (currentFriend != null)
                {
                    if (currentFriend.Friendship.Accepted)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CanCall(object arg)
        {
            return IsUserSelected() && currentFriend.FriendStatus == Status.Online && !calling && currentFriend.Friendship.Accepted;
        }

        private void OnCall(object obj)
        {
            VoiceChatViewModel voiceChatViewModel = this.container.Resolve<VoiceChatViewModel>();
            voiceChatViewModel.Title = (friends.CurrentItem as User).Name + " anrufen";
            voiceChatViewModel.IncomingCall = false;
            voiceChatViewModel.CallPartner = (friends.CurrentItem as User);
            voiceChatViewModel.CanAccept = false;
            voiceChatViewModel.CallAccepted = false;

            OpenCallDialog(voiceChatViewModel);
        }


        private void OnIncomingCall(ObjectId obj)
        {
            User caller = friendsService.GetFriendById(obj);
            if (caller != null)
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

        private void OnFriendStatusChanged(FriendStatusChangedEventArgs obj)
        {
            User f = friendsService.GetFriendById(obj.FriendId);
            if(f != null)
            {
                f.FriendStatus = obj.Status;

                if (currentFriend != null && f._id.Equals(currentFriend._id))
                {
                    callCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private async Task OnDeclineFriendship(object obj)
        {
            if (currentFriend != null)
            {
                await friendsService.AnswerFriendshipRequest(currentFriend._id, false);
                ShowFriendshipRequest = false;
            }
        }

        private async Task OnAcceptFriendship(object obj)
        {
            if (currentFriend != null)
            {
                await friendsService.AnswerFriendshipRequest(currentFriend._id, true);
                ShowFriendshipRequest = false;
                currentFriend.Friendship.Accepted = true;
                sendCommand.RaiseCanExecuteChanged();
            }
        }

        private void OnFriendshipRequestAnswered(FriendshipRequestAnsweredEventArgs obj)
        {
            if (obj.Accepted)
            {
                if (currentFriend != null)
                {
                    if (currentFriend._id.Equals(obj.FriendId))
                    {
                        ShowFriendshipInfo = false;
                        sendCommand.RaiseCanExecuteChanged();
                    }
                }
            }
        }

        private bool CanNavigateToProfile(object arg)
        {
            return IsUserSelected();
        }

        private void OnNavigateToProfile(object obj)
        {
            NavigationParameters navigationParameters = new NavigationParameters();
            navigationParameters.Add("username", currentFriend.Name);
            regionManager.RequestNavigate(RegionNames.MainContentRegion, NavigationURIs.ProfileViewUri, navigationParameters);
        }

        private bool CanDeleteFriend(object arg)
        {
            return IsUserSelected() && currentFriend.Friendship.Accepted;
        }

        private async Task OnDeleteFriend(object arg)
        {
            //server
        }

        private void RaiseCanExecuteFriendNavigationButtons()
        {
            callCommand.RaiseCanExecuteChanged();
            sendCommand.RaiseCanExecuteChanged();
            navigateToProfileCommand.RaiseCanExecuteChanged();
        }

        private bool IsUserSelected()
        {
            User currentFriend = FriendsCollectionView.CurrentItem as User;
            return currentFriend != null;
        }
    }
}

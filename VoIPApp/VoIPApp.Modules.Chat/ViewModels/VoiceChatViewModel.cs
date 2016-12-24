using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CPPWrapper;
using Prism.Interactivity.InteractionRequest;
using System.ComponentModel;
using VoIPApp.Modules.Chat.Services;
using SharedCode.Models;
using System.Threading.Tasks;
using Prism.Events;
using VoIPApp.Common;
using MongoDB.Bson;

namespace VoIPApp.Modules.Chat.ViewModels
{
    /// <summary>
    /// data context for <see cref="VoIPApp.Modules.Chat.Views.VoiceChatView"/>
    /// </summary>
    public class VoiceChatViewModel : BindableBase, IConfirmation, IInteractionRequestAware
    {
        private readonly IVoIPService voIPService;
        private readonly DelegateCommand<object> cancelCallCommand;
        private readonly DelegateCommand<object> acceptCallCommand;
        private readonly DelegateCommand<object> windowLoadedCommand;

        public VoiceChatViewModel(IVoIPService voIPService, EventAggregator eventAggregator)
        {
            this.voIPService = voIPService;
            this.cancelCallCommand = DelegateCommand<object>.FromAsyncHandler(this.OnCancelCall);
            this.acceptCallCommand = DelegateCommand<object>.FromAsyncHandler(this.OnAcceptCall);
            this.windowLoadedCommand = new DelegateCommand<object>(this.OnWindowLoaded);

            eventAggregator.GetEvent<AcceptedCallEvent>().Subscribe(OnCallAccepted, ThreadOption.BackgroundThread, true);
            eventAggregator.GetEvent<CanceledCallEvent>().Subscribe(OnCallCanceled, ThreadOption.BackgroundThread, true);
        }

        private void OnCallCanceled(ObjectId obj)
        {
            OnCancelCall(null);
        }

        private void OnCallAccepted(ObjectId obj)
        {
            OnAcceptCall(null);
        }

        private async Task OnAcceptCall(object arg)
        {
            await voIPService.AcceptCall(CallPartner);
        }

        private void OnWindowLoaded(object obj)
        {
            voIPService.StartCallSession(CallPartner);
        }

        private async Task OnCancelCall(object obj)
        {
            await voIPService.CancelCall(CallPartner);
            this.FinishInteraction();
        }

        public ICommand CancelCallCommand
        {
            get { return this.cancelCallCommand; }
        }

        public bool Confirmed { get; set; }

        public string Title { get; set; }

        public object Content { get; set; }

        public INotification Notification { get; set; }

        public Action FinishInteraction { get; set; }

        public bool IncomingCall { get; set; }

        public Friend CallPartner { get; set; }
    }
}

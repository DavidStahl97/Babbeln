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
using VoIPApp.Common.Services;

namespace VoIPApp.Modules.Chat.ViewModels
{
    /// <summary>
    /// data context for <see cref="VoIPApp.Modules.Chat.Views.VoiceChatView"/>
    /// </summary>
    public class VoiceChatViewModel : BindableBase, IConfirmation, IInteractionRequestAware
    {
        private readonly AudioStreamingService audioStreamingService;
        private readonly ServerServiceProxy serverServiceProxy;
        private readonly DelegateCommand<object> cancelCallCommand;
        private readonly DelegateCommand<object> acceptCallCommand;
        private readonly DelegateCommand<object> windowLoadedCommand;

        private bool startedAudioStreaming;

        public VoiceChatViewModel(AudioStreamingService audioStreamingService, EventAggregator eventAggregator, ServerServiceProxy serverServiceProxy)
        {
            this.audioStreamingService = audioStreamingService;
            this.serverServiceProxy = serverServiceProxy;
            this.cancelCallCommand = DelegateCommand<object>.FromAsyncHandler(this.OnCancelCall);
            this.acceptCallCommand = DelegateCommand<object>.FromAsyncHandler(this.OnAcceptCall);
            this.windowLoadedCommand = DelegateCommand<object>.FromAsyncHandler(this.OnWindowLoaded);

            eventAggregator.GetEvent<AcceptedCallEvent>().Subscribe(OnCallAccepted, ThreadOption.UIThread, true);
            eventAggregator.GetEvent<CanceledCallEvent>().Subscribe(OnCallCanceled, ThreadOption.UIThread, true);
        }

        private void OnCallCanceled(ObjectId obj)
        {
            if(startedAudioStreaming)
            {
                audioStreamingService.StopAsync();
            }
            this.FinishInteraction();
        }

        private void OnCallAccepted(ObjectId obj)
        {
            CallAccepted = true;
            if(!startedAudioStreaming)
            {
                startedAudioStreaming = true;
                audioStreamingService.StartAsync(CallPartner.IP, 10000);
            }
        }

        private async Task OnAcceptCall(object arg)
        {
            CanAccept = false;
            if(!startedAudioStreaming)
            {
                startedAudioStreaming = true;
                audioStreamingService.StartAsync(CallPartner.IP, 10000);
                await serverServiceProxy.ServerService.AcceptCallAsync(CallPartner._id);
            }
        }

        private async Task OnWindowLoaded(object obj)
        {
            if (!canAccept)
            {
                await serverServiceProxy.ServerService.CallAsync(CallPartner._id);
            }
        }

        private async Task OnCancelCall(object obj)
        {
            if (startedAudioStreaming)
            {
                audioStreamingService.StopAsync();
                startedAudioStreaming = false;
            }
            await serverServiceProxy.ServerService.CancelCallAsync(CallPartner._id);

            this.FinishInteraction();
        }

        public ICommand CancelCallCommand
        {
            get { return this.cancelCallCommand; }
        }

        public ICommand WindowLoadedCommand
        {
            get { return this.windowLoadedCommand; }
        }

        public ICommand AcceptCallCommand
        {
            get { return this.acceptCallCommand; }
        }

        public double PlayerDecibelValue
        {
            set
            {
                audioStreamingService.SetOutputVolumeGain(value);
            }
        }

        private bool canAccept;
        public bool CanAccept
        {
            get { return this.canAccept; }
            set { SetProperty(ref this.canAccept, value); }
        }

        private bool callAccepted;
        public bool CallAccepted
        {
            get { return this.callAccepted; }
            set { SetProperty(ref this.callAccepted, value); }
        }

        public bool Confirmed { get; set; }

        public string Title { get; set; }

        public object Content { get; set; }

        public INotification Notification { get; set; }

        public Action FinishInteraction { get; set; }

        public bool IncomingCall { get; set; }

        public User CallPartner { get; set; }
    }
}

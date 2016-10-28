using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CPPWrapper;
using Prism.Interactivity.InteractionRequest;

namespace VoIPApp.Modules.Chat.ViewModels
{
    public class VoiceChatViewModel : BindableBase, IConfirmation, IInteractionRequestAware
    {
        private readonly AudioStreamingService audioStreamer;
        private readonly DelegateCommand<object> cancelCallCommand;

        public VoiceChatViewModel(AudioStreamingService audioStreamer)
        {
            this.audioStreamer = audioStreamer;
            this.cancelCallCommand = new DelegateCommand<object>(this.OnCancelCall);
        }

        private void OnCancelCall(object obj)
        {
            this.FinishInteraction();
        }

        public void StartStreaming(string targetIP)
        {
            audioStreamer.Start(targetIP, 10000);
        }

        public void StopStreaming()
        {
            audioStreamer.Stop();
        }

        public ICommand CancelCallCommand
        {
            get { return this.cancelCallCommand; }
        }

        public string ExampleText { get { return "Example text for popup window"; } }

        public bool Confirmed { get; set; }

        public string Title { get; set; }

        public object Content { get; set; }

        public INotification Notification { get; set; }

        public Action FinishInteraction { get; set; }
    }
}

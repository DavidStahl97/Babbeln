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

namespace VoIPApp.Modules.Chat.ViewModels
{
    /// <summary>
    /// data context for <see cref="VoIPApp.Modules.Chat.Views.VoiceChatView"/>
    /// </summary>
    public class VoiceChatViewModel : BindableBase, IConfirmation, IInteractionRequestAware
    {
        private readonly IVoIPService voIPService;
        private readonly DelegateCommand<object> cancelCallCommand;

        public VoiceChatViewModel(IVoIPService voIPService)
        {
            this.voIPService = voIPService;
            this.cancelCallCommand = new DelegateCommand<object>(this.OnCancelCall);
        }

        private void OnCancelCall(object obj)
        {
            this.FinishInteraction();
        }

        public async Task StartCall(Friend f)
        {
            if (await voIPService.StartCall(f))
            {
                this.FinishInteraction();
            }
        }

        public void StopCall()
        {
            voIPService.StopCall();
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

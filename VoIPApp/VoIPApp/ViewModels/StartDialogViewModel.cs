using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using VoIPApp.Services;

namespace VoIPApp.ViewModels
{
    public class StartDialogViewModel : BindableBase
    {
        private bool? dialogResult;
        private readonly StartService loginService;
        private readonly DelegateCommand<object> windowLoadedCommand;

        public StartDialogViewModel(StartService loginService, EventAggregator eventAggregator)
        {
            this.loginService = loginService;
            this.windowLoadedCommand = DelegateCommand<object>.FromAsyncHandler(OnWindowLoaded);

            eventAggregator.GetEvent<CloseStartDialogEvent>().Subscribe(OnCloseDialog);
        }

        public ICommand WindowLoadedCommand
        {
            get { return this.windowLoadedCommand; }
        }

        public bool? DialogResult
        {
            get { return this.dialogResult; }
            set { SetProperty(ref this.dialogResult, value); }
        }

        private async Task OnWindowLoaded(object arg)
        {
            await loginService.Connect();
        }

        private void OnCloseDialog(bool obj)
        {
            DialogResult = obj;
        }
    }
}

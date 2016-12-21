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

        public StartDialogViewModel(StartService loginService, EventAggregator eventAggregator)
        {
            this.loginService = loginService;

            eventAggregator.GetEvent<CloseStartDialogEvent>().Subscribe(OnCloseDialog);
        }

        public bool? DialogResult
        {
            get { return this.dialogResult; }
            set { SetProperty(ref this.dialogResult, value); }
        }

        private void OnCloseDialog(bool obj)
        {
            DialogResult = obj;
        }
    }
}

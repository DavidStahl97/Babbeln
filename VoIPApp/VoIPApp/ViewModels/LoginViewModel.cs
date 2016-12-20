using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using VoIPApp.Services;

namespace VoIPApp.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        private string userName;
        private string message;
        private readonly StartService loginService;
        private readonly DelegateCommand<object> loginCommand;
        private readonly DelegateCommand<object> passwordChangedCommand;
        private readonly DelegateCommand<object> cancelCommand;
        private readonly EventAggregator eventAggregator;

        public LoginViewModel(StartService loginService, EventAggregator eventAggregator)
        {
            this.loginCommand = DelegateCommand<object>.FromAsyncHandler(OnLogin, CanLogin);
            this.passwordChangedCommand = new DelegateCommand<object>(OnPasswordChanged);
            this.cancelCommand = new DelegateCommand<object>(OnCancel);
            this.userName = string.Empty;
            this.message = string.Empty;
            this.loginService = loginService;
            this.eventAggregator = eventAggregator;
        }

        public string ViewName
        {
            get { return "Anmelden"; }
        }

        public string UserName
        {
            get { return this.userName; }
            set
            {
                this.userName = value;
                loginCommand.RaiseCanExecuteChanged();
            }
        }

        public ICommand LoginCommand
        {
            get { return this.loginCommand; }
        }

        public ICommand PasswordChangedCommand
        {
            get { return this.passwordChangedCommand; }
        }

        public ICommand CancelCommand
        {
            get { return this.cancelCommand; }
        }

        public string Message
        {
            get { return this.message; }
            set { SetProperty(ref this.message, value); }
        }

        private async Task OnLogin(object arg)
        {
            IHavePassword passwordContainer = arg as IHavePassword;
            if(passwordContainer != null)
            {
                Message = "Anmelden...";
                string errorMessage = await loginService.LogIn(userName, SecureStringConverter.ConvertToUnsecureString(passwordContainer.Password));
                if (errorMessage != null)
                {
                    Message = errorMessage;
                    Console.WriteLine(errorMessage);
                }
                else
                {
                    eventAggregator.GetEvent<CloseStartDialogEvent>().Publish(true);
                }
            }
        }

        private bool CanLogin(object arg)
        {
            IHavePassword passwordContainer = arg as IHavePassword;
            if (passwordContainer != null)
            {
                string unsecureString = SecureStringConverter.ConvertToUnsecureString(passwordContainer.Password);
                if (!string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(unsecureString))
                {
                    Message = string.Empty;
                    return true;
                }
                else
                {
                    Message = "Füllen sie alle Felder aus";
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void OnCancel(object obj)
        {
            eventAggregator.GetEvent<CloseStartDialogEvent>().Publish(false);
        }

        private void OnPasswordChanged(object obj)
        {
            loginCommand.RaiseCanExecuteChanged(); 
        }
    }
}

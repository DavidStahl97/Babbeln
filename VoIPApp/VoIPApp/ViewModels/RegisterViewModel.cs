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
    public class RegisterViewModel : BindableBase
    {
        private string userName;
        private string message;
        private string email;

        private readonly StartService loginService;
        private readonly DelegateCommand<object> registerCommand;
        private readonly DelegateCommand<object> passwordChangedCommand;
        private readonly DelegateCommand<object> cancelCommand;
        private readonly EventAggregator eventAggregator;

        public RegisterViewModel(StartService loginService, EventAggregator eventAggregator)
        {
            this.userName = string.Empty;
            this.message = string.Empty;
            this.email = string.Empty;

            this.loginService = loginService;
            this.eventAggregator = eventAggregator;

            this.registerCommand = DelegateCommand<object>.FromAsyncHandler(OnRegister, CanRegister);
            this.passwordChangedCommand = new DelegateCommand<object>(OnPasswordChanged);
            this.cancelCommand = new DelegateCommand<object>(OnCancel);
        }

        public string ViewName
        {
            get { return "Registrieren"; }
        }

        public string UserName
        {
            get { return this.userName; }
            set
            {
                this.userName = value;
                registerCommand.RaiseCanExecuteChanged();
            }
        }

        public string EMail
        {
            get { return this.email; }
            set
            {
                this.email = value;
                registerCommand.RaiseCanExecuteChanged();
            }
        }

        public string Message
        {
            get { return this.message; }
            set { SetProperty(ref this.message, value); }
        }

        public ICommand RegisterCommand
        {
            get { return this.registerCommand; }
        }

        public ICommand PasswordChangedCommand
        {
            get { return this.passwordChangedCommand; }
        }

        public ICommand CancelCommand
        {
            get { return this.cancelCommand; }
        }

        private void OnPasswordChanged(object obj)
        {
            registerCommand.RaiseCanExecuteChanged();
        }

        private bool CanRegister(object arg)
        {
            IHavePassword passwordContainer = arg as IHavePassword;
            if(passwordContainer != null)
            {
                string unsecurePassword = SecureStringConverter.ConvertToUnsecureString(passwordContainer.Password);
                string unsecureConfirmationPassword = SecureStringConverter.ConvertToUnsecureString(passwordContainer.ConfirmationPassword);

                if(!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(unsecurePassword) && !string.IsNullOrWhiteSpace(unsecureConfirmationPassword))
                {
                    if (!unsecurePassword.Equals(unsecureConfirmationPassword))
                    {
                        Message = "Passwörter stimmen nicht überein";
                        return false;
                    }

                    Message = string.Empty;
                    return true;
                }
                else
                {
                    Message = "Füllen Sie alle Felder aus";
                }
            }

            return false;
        }

        private async Task OnRegister(object obj)
        {
            IHavePassword passwordContainer = obj as IHavePassword;
            if (passwordContainer != null)
            {
                Message = "Registrieren...";

                string unsecurePassword = SecureStringConverter.ConvertToUnsecureString(passwordContainer.Password);
                string result = await loginService.Register(userName, unsecurePassword, email);
                Message = result;

                if(result.Equals(string.Empty))
                {
                    Message = "Registrierung erfolgreich. Anmelden...";
                    result = await loginService.LogIn(userName, unsecurePassword);
                    if(result != null)
                    {
                        Message = result;
                    }
                    else
                    {
                        eventAggregator.GetEvent<CloseStartDialogEvent>().Publish(true);
                    }
                }
            }
        }


        private void OnCancel(object obj)
        {
            eventAggregator.GetEvent<CloseStartDialogEvent>().Publish(false);
        }
    }
}

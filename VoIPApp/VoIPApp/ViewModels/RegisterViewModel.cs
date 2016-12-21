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
    public class RegisterViewModel : BindableBase
    {
        private string userName;
        private string message;
        private string email;

        private readonly DelegateCommand<object> registerCommand;
        private readonly DelegateCommand<object> passwordChangedCommand;
        private readonly DelegateCommand<object> cancelCommand;
        private readonly EventAggregator eventAggregator;
        private readonly InteractionRequest<LoginDialogViewModel> showLoginDialogRequest;
        private readonly IUnityContainer container;

        public RegisterViewModel(EventAggregator eventAggregator, IUnityContainer container)
        {
            this.userName = string.Empty;
            this.message = string.Empty;
            this.email = string.Empty;

            this.eventAggregator = eventAggregator;
            this.container = container;

            this.registerCommand = new DelegateCommand<object>(OnRegister, CanRegister);
            this.passwordChangedCommand = new DelegateCommand<object>(OnPasswordChanged);
            this.cancelCommand = new DelegateCommand<object>(OnCancel);
            this.showLoginDialogRequest = new InteractionRequest<LoginDialogViewModel>();
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

        public IInteractionRequest ShowLoginDialogRequest
        {
            get { return this.showLoginDialogRequest; }
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

        private void OnRegister(object obj)
        {
            IHavePassword passwordContainer = obj as IHavePassword;
            if (passwordContainer != null)
            {
                LoginDialogViewModel dialogViewModel = container.Resolve<LoginDialogViewModel>();
                dialogViewModel.Title = "hallo";
                dialogViewModel.Password = passwordContainer.Password;
                dialogViewModel.UserName = userName;
                dialogViewModel.EMail = email;

                this.showLoginDialogRequest.Raise(
                    dialogViewModel,
                    finishCall =>
                    {
                        if (dialogViewModel.Result == null)
                        {
                            Message = string.Empty;
                        }
                        else if (dialogViewModel.Result.Equals(string.Empty))
                        {
                            eventAggregator.GetEvent<CloseStartDialogEvent>().Publish(true);
                        }
                        else
                        {
                            Message = dialogViewModel.Result;
                        }
                    });
            }
        }


        private void OnCancel(object obj)
        {
            eventAggregator.GetEvent<CloseStartDialogEvent>().Publish(false);
        }
    }
}

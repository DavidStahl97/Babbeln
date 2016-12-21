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
        private readonly DelegateCommand<object> loginCommand;
        private readonly DelegateCommand<object> passwordChangedCommand;
        private readonly DelegateCommand<object> cancelCommand;
        private readonly InteractionRequest<LoginDialogViewModel> showLoginDialogRequest;
        private readonly EventAggregator eventAggregator;
        private readonly IUnityContainer container;

        public LoginViewModel(EventAggregator eventAggregator, IUnityContainer container)
        {
            this.loginCommand = new DelegateCommand<object>(OnLogin, CanLogin);
            this.passwordChangedCommand = new DelegateCommand<object>(OnPasswordChanged);
            this.cancelCommand = new DelegateCommand<object>(OnCancel);
            this.userName = string.Empty;
            this.message = string.Empty;
            this.showLoginDialogRequest = new InteractionRequest<LoginDialogViewModel>();
            this.eventAggregator = eventAggregator;
            this.container = container;
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

        public IInteractionRequest ShowLoginDialogRequest
        {
            get { return this.showLoginDialogRequest; }
        }

        public string Message
        {
            get { return this.message; }
            set { SetProperty(ref this.message, value); }
        }

        private void OnLogin(object arg)
        {
            IHavePassword passwordContainer = arg as IHavePassword;
            if(passwordContainer != null)
            {
                LoginDialogViewModel dialogViewModel = container.Resolve<LoginDialogViewModel>();
                dialogViewModel.Title = "hallo";
                dialogViewModel.Password = passwordContainer.Password;
                dialogViewModel.UserName = userName;
                dialogViewModel.EMail = null;

                this.showLoginDialogRequest.Raise(
                    dialogViewModel,
                    finishCall =>
                    {
                        if(dialogViewModel.Result == null)
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

using Microsoft.Practices.Unity;
using Prism.Commands;
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
        private DelegateCommand<object> okCommand;
        private string userName;
        private string password;
        private IUnityContainer container;

        public LoginViewModel(IUnityContainer container)
        {
            this.okCommand = DelegateCommand<object>.FromAsyncHandler(OnOk, CanOk);
            this.userName = string.Empty;
            this.password = string.Empty;
            this.container = container;
        }

        public string UserName
        {
            get { return this.userName; }
            set
            {
                this.userName = value;
                okCommand.RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get { return this.password; }
            set
            {
                this.password = value;
                okCommand.RaiseCanExecuteChanged();
            }
        }

        public ICommand OkCommand
        {
            get { return this.okCommand; }
        }

        public string Message { get; set; }

        private bool CanOk(object arg)
        {
            if(!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task OnOk(object arg)
        {
            LoginService loginService = container.Resolve<LoginService>();
            string errorMessage = await loginService.LogIn(userName, password);
            Message = "Connecting...";
            if(errorMessage != null)
            {
                Message = errorMessage;
                Console.WriteLine(errorMessage);
            }
        }
    }
}

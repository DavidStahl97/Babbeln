using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Input;
using VoIPApp.Services;

namespace VoIPApp.ViewModels
{
    public class LoginDialogViewModel : BindableBase, IConfirmation, IInteractionRequestAware
    {
        private readonly StartService startService;
        private readonly DelegateCommand<object> windowLoadedCommand;
        private readonly DelegateCommand<object> cancelCommand;
        private string userMessage;

        public LoginDialogViewModel(StartService startService)
        {
            this.startService = startService;
            this.userMessage = string.Empty;
            this.windowLoadedCommand = DelegateCommand<object>.FromAsyncHandler(OnWindowLoaded);
            this.cancelCommand = new DelegateCommand<object>(OnCancel);
        }

        private void OnCancel(object obj)
        {
            Result = null;
            this.FinishInteraction();
        }

        private async Task OnWindowLoaded(object arg)
        {
            try
            {
                UserMessage = (EMail == null) ? "Anmelden" : "Registrieren";

                if(!StartService.Connected)
                {
                    await startService.Connect();
                    if(!StartService.Connected)
                    {
                        Result = "Verbindung mit dem Server fehlgeschlagen";
                        StartService.Connected = true;
                        this.FinishInteraction();
                        return;
                    }
                }
                if (EMail == null)
                {
                    await Login(UserName, SecureStringConverter.ConvertToUnsecureString(Password));
                }
                else
                {
                    await Register(UserName, SecureStringConverter.ConvertToUnsecureString(Password), EMail);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Result = "Verbindung mit dem Server fehlgeschlagen";
                this.FinishInteraction();
            }
        }

        private async Task Login(string userName, string password)
        {
            Result = await startService.LogIn(userName, password);
            this.FinishInteraction();
        }

        private async Task Register(string userName, string password, string email)
        {
            Result = await startService.Register(userName, password, email);
            if(Result.Equals(string.Empty))
            {
                this.FinishInteraction();
            }
            else
            {
                UserMessage = "Anmelden";
                await Login(userName, password);
            }
        }

        public string UserName { get; set; }

        public SecureString Password { get; set; }

        public string EMail { get; set; }

        public string UserMessage
        {
            get { return this.userMessage; }
            set { SetProperty(ref this.userMessage, value); }
        }
            

        public string Result { get; set; }

        public bool Confirmed { get; set; }

        public object Content { get; set; }

        public Action FinishInteraction { get; set; }

        public INotification Notification { get; set; }

        public string Title { get; set; }

        public ICommand WindowLoadedCommand
        {
            get { return this.windowLoadedCommand; }
        }

        public ICommand CancelCommand
        {
            get { return this.cancelCommand; }
        }
    }
}

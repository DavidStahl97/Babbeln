using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using CPPWrapper;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Controls;
using SharedCode.Models;
using VoIPApp.Common.Services;
using System.Threading.Tasks;

namespace VoIPApp.Modules.Options.ViewModels
{
    /// <summary>
    /// data context for <see cref="VoIPApp.Modules.Options.Views.OptionsView"/>
    /// </summary>
    public class OptionsViewModel : BindableBase
    {
        /// <summary>
        /// audio streaming service used to get the audio devices etc.
        /// </summary>
        private readonly AudioStreamingService audioStreamingService;
        /// <summary>
        /// command that will be fired when the input device selection changed
        /// </summary>
        private readonly DelegateCommand<object> inputDeviceSelectionChanged;
        /// <summary>
        /// command that wiil be fired when the output device selection changed
        /// </summary>
        private readonly DelegateCommand<object> outputDeviceSelectionChanged;

        private readonly DelegateCommand<object> statusSelectionChanged;

        private readonly DelegateCommand<object> accountInfoSendCommand;

        private readonly ServerServiceProxy serverService;

        private bool changedUsername;

        /// <summary>
        /// creates a new instance of the <see cref="OptionsViewModel"/> class
        /// </summary>
        /// <param name="audioStreamingService">will be injected by the <see cref="IUnityContainer"/>, stored in <see cref="audioStreamingService"/></param>
        public OptionsViewModel(AudioStreamingService audioStreamingService, ServerServiceProxy serverService)
        {
            this.audioStreamingService = audioStreamingService;
            this.serverService = serverService;

            this.inputDeviceSelectionChanged = new DelegateCommand<object>(this.OnInputDeviceSelectionChanged);
            this.outputDeviceSelectionChanged = new DelegateCommand<object>(this.OnOutputDeviceSelectionChanged);
            this.statusSelectionChanged = DelegateCommand<object>.FromAsyncHandler(this.OnStatusSelectionChanged);
            this.accountInfoSendCommand = DelegateCommand<object>.FromAsyncHandler(this.OnAccountInfoSendCommand, CanSendAccountInfo);

            InputDevices = new ObservableCollection<string>(audioStreamingService.GetInputDevice());
            OutputDevices = new ObservableCollection<string>(audioStreamingService.GetOutputDevice());

            StatusStrings = new List<string>(Enum.GetNames(typeof(Status)));
        }

        public List<string> StatusStrings { get; private set; }

        private string newUsername;
        public string NewUserName
        {
            get { return this.newUsername; }
            set
            {
                this.newUsername = value;
                changedUsername = true;
                accountInfoSendCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// collection of the input devices
        /// </summary>
        public ObservableCollection<string> InputDevices { get; }

        /// <summary>
        /// collection of the output devices
        /// </summary>
        public ObservableCollection<string> OutputDevices { get; }

        public double PlayerDecibelValue
        {
            set
            {
                audioStreamingService.SetOutputVolumeGain(value);
            }
        }

        /// <summary>
        /// property for <see cref="inputDeviceSelectionChanged"/>
        /// </summary>
        public ICommand InputDeviceSelectionChanged
        {
            get { return inputDeviceSelectionChanged; }
        }

        /// <summary>
        /// property for <see cref="outputDeviceSelectionChanged"/>
        /// </summary>
        public ICommand OutputDeviceSelectionChanged
        {
            get { return outputDeviceSelectionChanged; }
        }

        public ICommand StatusSelectionChanged
        {
            get { return statusSelectionChanged; }
        }

        public ICommand AccountInfoSendCommand
        {
            get { return accountInfoSendCommand; }
        }

        /// <summary>
        /// sets the new output device in the <see cref="audioStreamingService"/> when changed
        /// </summary>
        /// <param name="obj"></param>
        private void OnOutputDeviceSelectionChanged(object obj)
        {
            string selectedDeviceName = GetSelectedStringFromEventArgs(obj as SelectionChangedEventArgs);
            if (!string.IsNullOrEmpty(selectedDeviceName))
            {
                audioStreamingService.SetOutputDevice(selectedDeviceName);
            }
        }

        /// <summary>
        /// sets the new input sevice int the <see cref="audioStreamingService"/> when changed
        /// </summary>
        /// <param name="obj"></param>
        private void OnInputDeviceSelectionChanged(object obj)
        {
            string selectedDeviceName = GetSelectedStringFromEventArgs(obj as SelectionChangedEventArgs);
            if(!string.IsNullOrEmpty(selectedDeviceName))
            {
                audioStreamingService.SetInputDevice(selectedDeviceName);
            }
        }

        private async Task OnStatusSelectionChanged(object arg)
        {
            string selectedStatus = GetSelectedStringFromEventArgs(arg as SelectionChangedEventArgs);
            Status status = (Status)Enum.Parse(typeof(Status), selectedStatus);
            await serverService.ServerService.ChangeStatusAsync(status);
        }

        private async Task OnAccountInfoSendCommand(object arg)
        {
            serverService.UserInfo.Username = NewUserName;
            await serverService.ServerService.ChangeUsernameAsync(NewUserName);
            changedUsername = false;
            accountInfoSendCommand.RaiseCanExecuteChanged();
        }

        private bool CanSendAccountInfo(object arg)
        {
            return !string.IsNullOrWhiteSpace(newUsername) && changedUsername && !newUsername.Equals(serverService.UserInfo.Username);
        }

        /// <summary>
        /// helper method that gets the <see cref="string"/> of the selected item
        /// </summary>
        /// <param name="arg">event args from combobox selection changed</param>
        /// <returns>string of the selected combobox item</returns>
        private string GetSelectedStringFromEventArgs(SelectionChangedEventArgs arg)
        {
            ComboBox comboBox = arg.Source as ComboBox;
            return comboBox.SelectedItem as string;
        }
    }
}

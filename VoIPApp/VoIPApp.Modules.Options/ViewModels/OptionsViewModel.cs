using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using CPPWrapper;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Controls;

namespace VoIPApp.Modules.Options.ViewModels
{
    public class OptionsViewModel : BindableBase
    {
        private readonly AudioStreamingService audioStreamingService;
        private readonly DelegateCommand<object> inputDeviceSelectionChanged;
        private readonly DelegateCommand<object> outputDeviceSelectionChanged;

        public OptionsViewModel(AudioStreamingService audioStreamingService)
        {
            this.audioStreamingService = audioStreamingService;

            this.inputDeviceSelectionChanged = new DelegateCommand<object>(this.OnInputDeviceSelectionChanged);
            this.outputDeviceSelectionChanged = new DelegateCommand<object>(this.OnOutputDeviceSelectionChanged);

            InputDevices = new ObservableCollection<string>(audioStreamingService.GetInputDevice());
            OutputDevices = new ObservableCollection<string>(audioStreamingService.GetOutputDevice());
        }

        public ObservableCollection<string> InputDevices { get; }

        public ObservableCollection<string> OutputDevices { get; }

        public ICommand InputDeviceSelectionChanged
        {
            get { return inputDeviceSelectionChanged; }
        }

        public ICommand OutputDeviceSelectionChanged
        {
            get { return outputDeviceSelectionChanged; }
        }

        private void OnOutputDeviceSelectionChanged(object obj)
        {
            string selectedDeviceName = GetSelectedStringFromEventArgs(obj as SelectionChangedEventArgs);
            if (!string.IsNullOrEmpty(selectedDeviceName))
            {
                audioStreamingService.SetOutputDevice(selectedDeviceName);
            }
        }

        private void OnInputDeviceSelectionChanged(object obj)
        {
            string selectedDeviceName = GetSelectedStringFromEventArgs(obj as SelectionChangedEventArgs);
            if(!string.IsNullOrEmpty(selectedDeviceName))
            {
                audioStreamingService.SetInputDevice(selectedDeviceName);
            }
        }

        private string GetSelectedStringFromEventArgs(SelectionChangedEventArgs arg)
        {
            ComboBox comboBox = arg.Source as ComboBox;
            return comboBox.SelectedItem as string;
        }
    }
}

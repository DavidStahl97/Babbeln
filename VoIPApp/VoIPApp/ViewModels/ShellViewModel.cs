using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using SharedCode.Services;

namespace VoIPApp.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        private readonly DelegateCommand<object> loadModuleCompleted;
        private readonly DataBaseService dataBaseService;

        public ShellViewModel(DataBaseService dataBaseService, IUnityContainer container)
        {
            this.loadModuleCompleted = new DelegateCommand<object>(OnLoadModuleCompleted);
            this.dataBaseService = dataBaseService;
        }

        public ICommand LoadModuleCompleted
        {
            get { return this.loadModuleCompleted; }
        }

        private void OnLoadModuleCompleted(object obj)
        {
            dataBaseService.Connect();
        }
    }
}

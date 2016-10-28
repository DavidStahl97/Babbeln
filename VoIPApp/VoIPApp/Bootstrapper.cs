using Microsoft.Practices.Unity;
using Prism.Unity;
using VoIPApp.Views;
using System.Windows;
using Prism.Modularity;
using System;

namespace VoIPApp
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new AggregateModuleCatalog();
        }

        protected override void ConfigureModuleCatalog()
        {
            DirectoryModuleCatalog directoryCatalog = new DirectoryModuleCatalog() { ModulePath = @".\DirectoryModules" };
            ((AggregateModuleCatalog)ModuleCatalog).AddCatalog(directoryCatalog);
        }
    }
}

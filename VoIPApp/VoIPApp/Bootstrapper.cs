using Microsoft.Practices.Unity;
using Prism.Unity;
using VoIPApp.Views;
using System.Windows;
using Prism.Modularity;
using System;
using CPPWrapper;

namespace VoIPApp
{
    class Bootstrapper : UnityBootstrapper
    {

        private readonly AudioStreamingService audioStreamingService = new AudioStreamingService();

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

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterInstance(audioStreamingService);
        }
    }
}

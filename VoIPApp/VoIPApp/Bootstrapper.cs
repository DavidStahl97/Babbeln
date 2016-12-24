using Microsoft.Practices.Unity;
using Prism.Unity;
using VoIPApp.Views;
using System.Windows;
using Prism.Modularity;
using System;
using CPPWrapper;
using SharedCode.Services;
using VoIPApp.Common.Services;
using VoIPApp.Services;
using Prism.Interactivity.InteractionRequest;
using VoIPApp.ViewModels;
using Prism.Regions;
using Prism.Events;

namespace VoIPApp
{
    /// <summary>
    /// manages the startup of the application using the Unity container
    /// </summary>
    class Bootstrapper : UnityBootstrapper
    {
        /// <summary>
        /// instance of the c++/cli <see cref="audioStreamingService"/> for entire application
        /// </summary>
        private readonly AudioStreamingService audioStreamingService = new AudioStreamingService();
        /// <summary>
        /// instance of <see cref="DataBaseService"/> for entire application
        /// </summary>
        private readonly DataBaseService dataBaseService = new DataBaseService();

        private ServerServiceProxy serverServiceProxy;

        /// <summary>
        /// creates shell
        /// </summary>
        /// <returns>returns the resolved shell object</returns>
        protected override DependencyObject CreateShell()
        {
            StartDialogView startView = new StartDialogView();
            RegionManager regionManager = Container.Resolve<RegionManager>();
            regionManager.RegisterViewWithRegion("TabRegion", () => Container.Resolve<LoginView>());
            regionManager.RegisterViewWithRegion("TabRegion", () => Container.Resolve<RegisterView>());
            if (startView.ShowDialog() == true)
            {
                return Container.Resolve<Shell>();
            } 
            else
            {
                Environment.Exit(0);
                return null;
            }
        }

        /// <summary>
        /// shows the shell as the main window
        /// </summary>
        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        /// <summary>
        /// creates the module catalog that will load the modules for the application (eg. the chat module)
        /// </summary>
        /// <returns><see cref="AggregateModuleCatalog"/></returns>
        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new AggregateModuleCatalog();
        }

        /// <summary>
        /// adds the <see cref="DirectoryModuleCatalog"/> to the <see cref="AggregateModuleCatalog"/>. All modules in the DirectoryModules folder will be loaded.
        /// </summary>
        protected override void ConfigureModuleCatalog()
        {
            DirectoryModuleCatalog directoryCatalog = new DirectoryModuleCatalog() { ModulePath = @".\DirectoryModules" };
            ((AggregateModuleCatalog)ModuleCatalog).AddCatalog(directoryCatalog);
        }

        /// <summary>
        /// registers singleton instances of global services/>
        /// </summary>
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            serverServiceProxy = new ServerServiceProxy(Container.Resolve<EventAggregator>());

            Container.RegisterInstance(audioStreamingService);
            Container.RegisterInstance(dataBaseService);
            Container.RegisterInstance(serverServiceProxy);
            Container.RegisterType<StartService>();
            Container.RegisterType<LoginDialogViewModel>();
        }
    }
}

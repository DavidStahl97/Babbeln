using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VoIPApp.Common.Services;

namespace VoIPApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Bootstrapper bootstrapper;

        /// <summary>
        /// creates and runs the bootstrapper
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            bootstrapper = new Bootstrapper();
            try
            {
                bootstrapper.Run();
            }
           catch (Exception ex)
            {
                Console.WriteLine("Failed running the bootstrapper: " + ex.Message);
            }

            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            ServerServiceProxy serverServiceProxy = bootstrapper.Container.Resolve<ServerServiceProxy>();
            if(serverServiceProxy != null)
            {
                serverServiceProxy.ServerService.Unsubscribe();
            }
        }
    }
}

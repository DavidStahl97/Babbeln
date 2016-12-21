﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace VoIPApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// creates and runs the bootstrapper
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            var bootstrapper = new Bootstrapper();
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
    }
}

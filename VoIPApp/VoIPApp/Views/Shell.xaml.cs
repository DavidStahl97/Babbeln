using Prism.Modularity;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Input;
using VoIPApp.Common;

namespace VoIPApp.Views
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Shell"/> class.
        /// <para>
        /// registers a callback that will be fired after the modules finished loading.
        /// the callback will request a navigation to the ChatView
        /// </para>
        /// </summary>
        /// <param name="moduleManager">the applications modulemanager</param>
        /// <param name="regionManager">the applications regionmanager</param>
        public Shell(IModuleManager moduleManager, IRegionManager regionManager)
        {
            InitializeComponent();

            moduleManager.LoadModuleCompleted +=
                (s, e) =>
                {
                    if (e.ModuleInfo.ModuleName.Equals(ModuleNames.ChatModuleName))
                    {
                        regionManager.RequestNavigate(RegionNames.MainContentRegion, NavigationURIs.chatViewUri,
                            (NavigationResult nr) => 
                            {
                                var error = nr.Error;
                                var result = nr.Result;
                            }
                        );
                    }
                };
        }
    }
}

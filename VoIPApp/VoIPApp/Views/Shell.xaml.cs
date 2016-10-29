using Prism.Modularity;
using Prism.Regions;
using System;
using System.Windows;
using VoIPApp.Common;

namespace VoIPApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Shell : Window
    {
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

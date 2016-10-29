using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using System;
using VoIPApp.Common;
using VoIPApp.Modules.Options.Views;

namespace VoIPApp.Modules.Options
{
    public class OptionsModule : IModule
    {
        private readonly IRegionManager regionManager;
        private readonly IUnityContainer container;

        public OptionsModule(IRegionManager regionManager, IUnityContainer container)
        {
            this.regionManager = regionManager;
            this.container = container;

            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, typeof(OptionsNavigationItemView));
        }

        public void Initialize()
        {
            this.container.RegisterType<object, OptionsView>(NavigationURIs.optionViewUri.OriginalString);
        }
    }
}
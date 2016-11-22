using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using System;
using VoIPApp.Common;
using VoIPApp.Modules.Options.Views;
using CPPWrapper;

namespace VoIPApp.Modules.Options
{
    /// <summary>
    /// wíll be loaded by the module manager
    /// </summary>
    public class OptionsModule : IModule
    {
        /// <summary>
        /// <see cref="IRegionManager"/> of the application
        /// </summary>
        private readonly IRegionManager regionManager;
        /// <summary>
        /// <see cref="IUnityContainer"/> of the application
        /// </summary>
        private readonly IUnityContainer container;

        /// <summary>
        /// creates a new instance of the <see cref="OptionsModule"/> class
        /// </summary>
        /// <param name="regionManager">injected by the unity container, stored in <see cref="regionManager"/></param>
        /// <param name="container">injected by the unity container, stored in <see cref="container"/></param>
        public OptionsModule(IRegionManager regionManager, IUnityContainer container)
        {
            this.regionManager = regionManager;
            this.container = container;

            this.regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, typeof(OptionsNavigationItemView));
        }

        /// <summary>
        /// register types for the <see cref="IUnityContainer"/>
        /// </summary>
        public void Initialize()
        {
            this.container.RegisterType<object, OptionsView>(NavigationURIs.optionViewUri.OriginalString);
        }
    }
}
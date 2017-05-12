using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoIPApp.Common;
using VoIPApp.Modules.Profile.Views;

namespace VoIPApp.Module.Profile
{
    public class ProfileModule : IModule
    {
        private readonly IUnityContainer container;
        
        public ProfileModule(IRegionManager regionManager, IUnityContainer container)
        {
            this.container = container;

            regionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, typeof(ProfileNavigationItemView));
        }

        public void Initialize()
        {
            this.container.RegisterType<object, ProfileView>(NavigationURIs.ProfileViewUri.OriginalString);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoIPApp.Common
{
    /// <summary>
    /// static class that defines the navigation <see cref="Uri"/>s that are needed to reuest a navigation from the <see cref="Prism.Region.IRegionManager"/>
    /// </summary>
    public static class NavigationURIs
    {
        /// <summary>
        /// <see cref="Uri"/> for the chat view
        /// </summary>
        public static Uri ChatViewUri = new Uri("ChatView", UriKind.Relative);

        /// <summary>
        /// <see cref="Uri"/> for the option view
        /// </summary>
        public static Uri OptionViewUri = new Uri("OptionView", UriKind.Relative);

        public static Uri ProfileViewUri = new Uri("ProfileView", UriKind.Relative);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoIPApp.Common
{
    /// <summary>
    /// static class that defines the <see cref="string"/>s for the regions, that will be used by the <see cref="Prism.Region.IRegionManager"/>
    /// </summary>
    public static class RegionNames
    {
        /// <summary>
        /// <see cref="string"/> for the main content region
        /// </summary>
        public const string MainContentRegion = "MainContentRegion";
        /// <summary>
        /// <see cref="string"/> for the main navigation region
        /// </summary>
        public const string MainNavigationRegion = "MainNavigationRegion";
    }
}

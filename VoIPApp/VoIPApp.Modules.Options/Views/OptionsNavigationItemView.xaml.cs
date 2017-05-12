using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VoIPApp.Common;

namespace VoIPApp.Modules.Options.Views
{
    /// <summary>
    /// Interaktionslogik für OptionsNavigationItemView.xaml
    /// </summary>
    [ViewSortHint("10")]
    public partial class OptionsNavigationItemView : UserControl
    {
        /// <summary>
        /// <see cref="IRegionManager"/> of the application
        /// </summary>
        private readonly IRegionManager regionManager;
        
        /// <summary>
        /// creates a new instance of the <see cref="OptionsNavigationItemView"/> class
        /// </summary>
        /// <param name="regionManager">will be inhected by the <see cref="IUnityContainer"/>, stored in <see cref="regionManager"/></param>
        public OptionsNavigationItemView(IRegionManager regionManager)
        {
            InitializeComponent();

            this.regionManager = regionManager;
        }

        /// <summary>
        /// reuest navigate to the view of the module when clicked on the modules <see cref="VoIPApp.Common.Controls.HamburgerMenuItem"/>
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">mouse button event args</param>
        private void HamburgerMenuItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            object currentView = regionManager.Regions[RegionNames.MainContentRegion].ActiveViews.FirstOrDefault();
            OptionsNavigationItemView optionsView = currentView as OptionsNavigationItemView;
            if(optionsView == null)
            {
                regionManager.RequestNavigate(RegionNames.MainContentRegion, NavigationURIs.OptionViewUri);
            }
        }
    }
}

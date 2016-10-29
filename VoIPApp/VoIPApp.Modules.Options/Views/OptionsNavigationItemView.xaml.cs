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
        private readonly IRegionManager regionManager;

        public OptionsNavigationItemView(IRegionManager regionManager)
        {
            InitializeComponent();

            this.regionManager = regionManager;
        }

        private void HamburgerMenuItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            object currentView = regionManager.Regions[RegionNames.MainContentRegion].ActiveViews.FirstOrDefault();
            OptionsNavigationItemView optionsView = currentView as OptionsNavigationItemView;
            if(optionsView == null)
            {
                regionManager.RequestNavigate(RegionNames.MainContentRegion, NavigationURIs.optionViewUri);
            }
        }
    }
}

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

namespace VoIPApp.Modules.Chat.Views
{
    /// <summary>
    /// Interaktionslogik für ChatNavigationItemView.xaml
    /// </summary>
    [ViewSortHint("3")]
    public partial class ChatNavigationItemView : UserControl
    {
        private readonly IRegionManager regionManager;

        public ChatNavigationItemView(IRegionManager regionManager)
        {
            InitializeComponent();

            this.regionManager = regionManager;
        }

        private void HamburgerMenuItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            object currentView = regionManager.Regions[RegionNames.MainContentRegion].ActiveViews.FirstOrDefault();
            ChatView chatView = currentView as ChatView;
            if(chatView == null)
            {
                this.regionManager.RequestNavigate(RegionNames.MainContentRegion, NavigationURIs.chatViewUri);
            }
        }
    }
}

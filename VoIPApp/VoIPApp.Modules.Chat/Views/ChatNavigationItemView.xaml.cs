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
        /// <summary>
        /// <see cref="IRegionManager"/> of the application
        /// </summary>
        private readonly IRegionManager regionManager;

        /// <summary>
        /// creates a new instance of the <see cref="ChatNavigationItemView"/> class
        /// </summary>
        /// <param name="regionManager">injected by the <see cref="Microsoft.Practices.Unity.IUnityContainer"/>, stored in <see cref="regionManager"/></param>
        public ChatNavigationItemView(IRegionManager regionManager)
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
            ChatView chatView = currentView as ChatView;
            if(chatView == null)
            {
                this.regionManager.RequestNavigate(RegionNames.MainContentRegion, NavigationURIs.ChatViewUri);
            }
        }
    }
}

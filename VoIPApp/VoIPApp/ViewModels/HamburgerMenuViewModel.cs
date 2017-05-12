using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using VoIPApp.Common.Services;

namespace VoIPApp.ViewModels
{
    /// <summary>
    /// Data Context for <see cref="VoIPApp.Views.HamburgerMenu"/>
    /// </summary>
    public class HamburgerMenuViewModel : BindableBase
    {
        /// <summary>
        /// bitmap profile image
        /// </summary>
        private BitmapImage profileIcon;
        /// <summary>
        /// profile name of the user
        /// </summary>
        private string profileName;

        /// <summary>
        /// creates a new instance of the <see cref="HamburgerMenuViewModel"/> class. Initializes the <see cref="profileIcon"/> and <see cref="profileName"/>
        /// </summary>
        public HamburgerMenuViewModel(ServerServiceProxy serverService)
        {
            profileIcon = new BitmapImage(new Uri("pack://application:,,,/Assets/profile_high.jpg"));
            profileName = serverService.UserInfo.Username;
        }

        /// <summary>
        /// bindable property for <see cref="profileIcon"/>
        /// </summary>
        public BitmapImage ProfileIcon
        {
            get { return this.profileIcon; }
            set { SetProperty(ref this.profileIcon, value); }
        }

        /// <summary>
        /// bindable property for <see cref="profileName"/>
        /// </summary>
        public string ProfileName
        {
            get { return this.profileName; }
            set { SetProperty(ref this.profileName, value); }
        }

    }
}

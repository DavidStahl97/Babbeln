using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VoIPApp.ViewModels
{
    public class HamburgerMenuViewModel : BindableBase
    {
        private BitmapImage profileIcon;
        private string profileName;

        public HamburgerMenuViewModel()
        {
            profileIcon = new BitmapImage(new Uri("pack://application:,,,/Assets/profile_high.jpg"));
            profileName = "Günther";
        }

        public BitmapImage ProfileIcon
        {
            get { return this.profileIcon; }
            set { SetProperty(ref this.profileIcon, value); }
        }

        public string ProfileName
        {
            get { return this.profileName; }
            set { SetProperty(ref this.profileName, value); }
        }

    }
}

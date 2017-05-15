using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoIPApp.Modules.Profile.ViewModels
{
    public class ProfileViewModel : BindableBase, INavigationAware
    {
        private string baseURL = ConfigurationManager.AppSettings["WebsiteProfile"].ToString();

        public ProfileViewModel()
        {
            Address = baseURL;
        }

        private string adress;
        public string Address
        {
            get { return adress; }
            set
            {
                SetProperty(ref adress, value);
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            string username = navigationContext.Parameters["username"].ToString();
            Address = string.Format("{0}/?username=\"?username={1}\"", baseURL, username); 
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}

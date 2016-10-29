using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoIPApp.Common
{
    public static class NavigationURIs
    {
        public static Uri chatViewUri = new Uri("ChatView", UriKind.Relative);
        public static Uri optionViewUri = new Uri("OptionView", UriKind.Relative);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoIPApp.Common.Models;

namespace VoIPApp.Modules.Chat
{
    public class CustomSorter : IComparer
    {
        private readonly string filter;

        public CustomSorter(string filter)
        {
            this.filter = filter;
        }

        public int Compare(object x, object y)
        {
            Friend a = x as Friend;
            Friend b = y as Friend;

            int ai = a.Name.IndexOf(filter);
            int bi = b.Name.IndexOf(filter);

            if(ai > bi)
            {
                return 1;
            }
            else if (ai == bi)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }
}

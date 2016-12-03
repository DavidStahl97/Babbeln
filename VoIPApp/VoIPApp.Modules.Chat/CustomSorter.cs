using SharedCode.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoIPApp.Modules.Chat
{
    /// <summary>
    /// custom sorter for friend list
    /// </summary>
    public class CustomSorter : IComparer
    {
        /// <summary>
        /// the input <see cref="string"/> that the user wants to search after
        /// </summary>
        private readonly string filter;

        /// <summary>
        /// creates a new instance of the <see cref="CustomSorter"/> class
        /// </summary>
        /// <param name="filter">stored in <see cref="filter"/></param>
        public CustomSorter(string filter)
        {
            this.filter = filter;
        }

        /// <summary>
        /// compares to <see cref="Friend"/>s based on <see cref="filter"/>
        /// </summary>
        /// <param name="x">first <see cref="Friend"/></param>
        /// <param name="y">second <see cref="Friend"/></param>
        /// <returns></returns>
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

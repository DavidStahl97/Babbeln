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

namespace VoIPApp.Common.Controls
{
    /// <summary>
    /// Interaktionslogik für HamburgerMenuItem.xaml
    /// </summary>
    public partial class HamburgerMenuItem : UserControl
    {
        /// <summary>
        /// text that will be displayed when the HamburgerMenu is opened
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="Text"/>
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string),
              typeof(HamburgerMenuItem), new PropertyMetadata(""));


        /// <summary>
        /// font icon that will be displayed in HamburgerMenu
        /// </summary>
        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> of <see cref="Icon"/>
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string),
              typeof(HamburgerMenuItem), new PropertyMetadata(""));

        /// <summary>
        /// creates a new instance of the <see cref="HamburgerMenuItem"/> class
        /// </summary>
        public HamburgerMenuItem()
        {
            InitializeComponent();
        }
    }
}

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
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string),
              typeof(HamburgerMenuItem), new PropertyMetadata(""));

        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string),
              typeof(HamburgerMenuItem), new PropertyMetadata(""));

        public HamburgerMenuItem()
        {
            InitializeComponent();
        }
    }
}

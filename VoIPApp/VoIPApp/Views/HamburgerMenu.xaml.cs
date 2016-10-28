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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VoIPApp.Views
{
    /// <summary>
    /// Interaktionslogik für HamburgerMenu.xaml
    /// </summary>
    public partial class HamburgerMenu : UserControl
    {
        private bool isOpen;
        private bool animFinished = true;

        public int PictureSizeBig
        {
            get { return 128; }
            private set { }
        }

        public int PictureSizeSmall
        {
            get { return 64; }
            private set { }
        }

        public new List<HamburgerMenuItem> Content
        {
            get { return (List<HamburgerMenuItem>)GetValue(ContentProperty); }
            set
            {
                SetValue(ContentProperty, value);
            }
        }

        public new static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(List<HamburgerMenuItem>), typeof(HamburgerMenu),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public HamburgerMenu()
        {
            Content = new List<HamburgerMenuItem>();
            InitializeComponent();
            MenuItemList.ItemsSource = Content;
        }

        private void ProfileIconContainer_MouseEnter(object sender, MouseEventArgs e)
        {
            if(!isOpen && animFinished)
            {
                isOpen = true;
                animFinished = false;

                GridLengthAnimation gridAnim = new GridLengthAnimation
                {
                    From = new GridLength(PictureSizeSmall, GridUnitType.Pixel),
                    To = new GridLength(PictureSizeBig, GridUnitType.Pixel),
                    Duration = new TimeSpan(0, 0, 0, 0, 200)
                };

                gridAnim.Completed += animationCompleted;
                MainGrid.ColumnDefinitions[0].BeginAnimation(ColumnDefinition.WidthProperty, gridAnim);

                ProfileName.Visibility = Visibility.Visible;
            }
        }

        private void animationCompleted(object sender, EventArgs e)
        {
            animFinished = true;
        }

        private void MainGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            if(isOpen && animFinished)
            {
                isOpen = false;
                animFinished = false;

                GridLengthAnimation gridAnim = new GridLengthAnimation
                {
                    From = new GridLength(PictureSizeBig, GridUnitType.Pixel),
                    To = new GridLength(PictureSizeSmall, GridUnitType.Pixel),
                    Duration = new TimeSpan(0, 0, 0, 0, 200)
                };

                gridAnim.Completed += animationCompleted;
                MainGrid.ColumnDefinitions[0].BeginAnimation(ColumnDefinition.WidthProperty, gridAnim);

                ProfileName.Visibility = Visibility.Collapsed;
            }
        }
    }
}

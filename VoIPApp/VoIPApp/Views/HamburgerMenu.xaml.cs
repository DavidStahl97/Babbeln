using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using System.Windows.Media.Animation;
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
        /// <summary>
        /// true when the <see cref="HamburgerMenu"/> is opened
        /// </summary>
        private bool isOpen;
        /// <summary>
        /// true when the animation is finished
        /// </summary>
        private bool animFinished = true;

        private TimeSpan AnimationDuration = new TimeSpan(0, 0, 0, 0, 200);

        /// <summary>
        /// size of the profile picture when the <see cref="HamburgerMenu"/> is opened
        /// </summary>
        public int PictureSizeBig
        {
            get { return 128; }
            private set { }
        }

        /// <summary>
        /// size of the profile picture when the <see cref="HamburgerMenu"/> is closed
        /// </summary>
        public int PictureSizeSmall
        {
            get { return 64; }
            private set { }
        }

        /// <summary>
        /// creates a new instance of the <see cref="HamburgerMenu"/> class
        /// </summary>
        public HamburgerMenu()
        {
            InitializeComponent();
        }

        /// <summary>
        /// starts the animation if the mouse hovers over the profile picture and the <see cref="HamburgerMenu"/> is not already opened or is not animating
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">mouse information</param>
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
                    Duration = AnimationDuration
                };

                DoubleAnimation doubleAnim = new DoubleAnimation
                {
                    From = PictureSizeSmall,
                    To = PictureSizeBig,
                    Duration = AnimationDuration 
                };

                gridAnim.Completed += animationCompleted;
                MainGrid.ColumnDefinitions[0].BeginAnimation(ColumnDefinition.WidthProperty, gridAnim);
                ProfileImage.BeginAnimation(Ellipse.HeightProperty, doubleAnim);
                ProfileImage.BeginAnimation(Ellipse.WidthProperty, doubleAnim);

                ProfileName.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// sets <see cref="animFinished"/> to true when the animation is completed
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">event arguments</param>
        private void animationCompleted(object sender, EventArgs e)
        {
            animFinished = true;
        }

        /// <summary>
        /// starts the animation that will set it to the original size when the mouse leaves the profile picture
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">mouse information</param>
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
                    Duration = AnimationDuration
                };

                DoubleAnimation doubleAnim = new DoubleAnimation
                {
                    From = PictureSizeBig,
                    To = PictureSizeSmall,
                    Duration = AnimationDuration
                };

                gridAnim.Completed += animationCompleted;
                MainGrid.ColumnDefinitions[0].BeginAnimation(ColumnDefinition.WidthProperty, gridAnim);
                ProfileImage.BeginAnimation(Ellipse.HeightProperty, doubleAnim);
                ProfileImage.BeginAnimation(Ellipse.WidthProperty, doubleAnim);

                ProfileName.Visibility = Visibility.Collapsed;
            }
        }
    }
}

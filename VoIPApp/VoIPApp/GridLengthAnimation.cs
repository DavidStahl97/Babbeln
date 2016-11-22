using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace VoIPApp
{
    /// <summary>
    /// implements an animation for grids. Grid sizes cannot be used by the default animation because they can be used with percentage sizes.
    /// </summary>
    public class GridLengthAnimation : AnimationTimeline
    {
        /// <summary>
        /// registers the <see cref="DependencyProperty"/>s
        /// </summary>
        static GridLengthAnimation()
        {
            FromProperty = DependencyProperty.Register("From", typeof(GridLength), typeof(GridLengthAnimation));
            ToProperty = DependencyProperty.Register("To", typeof(GridLength), typeof(GridLengthAnimation));
        }

        /// <summary>
        /// property that sets the <see cref="GridLength"/> as data type for sizes
        /// </summary>
        public override Type TargetPropertyType
        {
            get
            {
                return typeof(GridLength);
            }
        }

        /// <summary>
        /// returns an instance of <see cref="GridLengthAnimation"/>
        /// </summary>
        /// <returns><see cref="GridLengthAnimation"/></returns>
        protected override Freezable CreateInstanceCore()
        {
            return new GridLengthAnimation();
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="From"/>
        /// </summary>
        public static readonly DependencyProperty FromProperty;
        /// <summary>
        /// start size of the animation
        /// </summary>
        public GridLength From
        {
            get { return (GridLength)GetValue(GridLengthAnimation.FromProperty); }
            set { SetValue(GridLengthAnimation.FromProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="To"/>
        /// </summary>
        public static readonly DependencyProperty ToProperty;
        /// <summary>
        /// end size of the animation
        /// </summary>
        public GridLength To
        {
            get { return (GridLength)GetValue(GridLengthAnimation.ToProperty); }
            set { SetValue(GridLengthAnimation.ToProperty, value); }
        }

        /// <summary>
        /// returns the current size value for a given time between <see cref="From"/> and <see cref="To"/>
        /// </summary>
        /// <param name="defaultOriginValue">start size value</param>
        /// <param name="defaultDestinationValue">end size value</param>
        /// <param name="animationClock">describes the current time</param>
        /// <returns></returns>
        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            double fromValue = From.Value;
            double toValue = To.Value;

            if(fromValue > toValue)
            {
                return new GridLength((1 - animationClock.CurrentProgress.Value) * (fromValue - toValue) + toValue, GridUnitType.Pixel);
            }
            else
            {
                return new GridLength(animationClock.CurrentProgress.Value * (toValue - fromValue) + fromValue, GridUnitType.Pixel);
            }
        }
    }
}

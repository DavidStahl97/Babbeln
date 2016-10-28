using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace VoIPApp
{
    public class GridLengthAnimation : AnimationTimeline
    {
        static GridLengthAnimation()
        {
            FromProperty = DependencyProperty.Register("From", typeof(GridLength), typeof(GridLengthAnimation));
            ToProperty = DependencyProperty.Register("To", typeof(GridLength), typeof(GridLengthAnimation));
        }

        public override Type TargetPropertyType
        {
            get
            {
                return typeof(GridLength);
            }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new GridLengthAnimation();
        }

        public static readonly DependencyProperty FromProperty;
        public GridLength From
        {
            get { return (GridLength)GetValue(GridLengthAnimation.FromProperty); }
            set { SetValue(GridLengthAnimation.FromProperty, value); }
        }

        public static readonly DependencyProperty ToProperty;
        public GridLength To
        {
            get { return (GridLength)GetValue(GridLengthAnimation.ToProperty); }
            set { SetValue(GridLengthAnimation.ToProperty, value); }
        }

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

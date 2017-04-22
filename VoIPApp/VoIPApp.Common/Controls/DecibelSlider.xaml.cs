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
    /// Interaction logic for DecibelSlider.xaml
    /// </summary>
    public partial class DecibelSlider : UserControl
    {
        public double DecibelValue
        {
            get
            {
                return (double)GetValue(DecibelValueProperty);
            }

            set
            {
                SetValue(DecibelValueProperty, value);
            }
        }

        public static readonly DependencyProperty DecibelValueProperty =
            DependencyProperty.Register(nameof(DecibelValue), typeof(double),
              typeof(DecibelSlider), new PropertyMetadata(0.0));
              
        public DecibelSlider()
        {
            InitializeComponent();
        }
    }
}

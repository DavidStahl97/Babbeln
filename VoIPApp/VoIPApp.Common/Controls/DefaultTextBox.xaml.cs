using System;
using System.Windows;
using System.Windows.Controls;

namespace VoIPApp.Common.Controls
{
    /// <summary>
    /// Interaction logic for DefaultTextBox
    /// </summary>
    public partial class DefaultTextBox : UserControl
    {
        public string DefaultText
        {
            get { return (string)GetValue(DefaultTextProperty); }
            set { SetValue(DefaultTextProperty, value); }
        }

        public static readonly DependencyProperty DefaultTextProperty=
            DependencyProperty.Register("DefaultText", typeof(string),
              typeof(DefaultTextBox), new PropertyMetadata(""));

        public new string BorderThickness
        {
            get { return (string)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        public static readonly new DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(string),
              typeof(DefaultTextBox), new PropertyMetadata(""));

        public string InputText
        {
            get { return (string)GetValue(InputTextProperty); }
            set { SetValue(InputTextProperty, value); }
        }

        public static readonly DependencyProperty InputTextProperty =
            DependencyProperty.Register("InputText", typeof(string),
              typeof(DefaultTextBox), new FrameworkPropertyMetadata(
            null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public event RoutedEventHandler TextChanged;

        public DefaultTextBox()
        {
            InitializeComponent();    
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextChanged != null) TextChanged(sender, e);
        }
    }
}

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
        /// <summary>
        /// text that will be displayed if the text box is empty
        /// </summary>
        public string DefaultText
        {
            get { return (string)GetValue(DefaultTextProperty); }
            set { SetValue(DefaultTextProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> of <see cref="DefaultText"/>
        /// </summary>
        public static readonly DependencyProperty DefaultTextProperty=
            DependencyProperty.Register("DefaultText", typeof(string),
              typeof(DefaultTextBox), new PropertyMetadata(""));

        /// <summary>
        /// border thickness of the text box
        /// </summary>
        public new string BorderThickness
        {
            get { return (string)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> of <see cref="BorderThickness"/>
        /// </summary>
        public static readonly new DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(string),
              typeof(DefaultTextBox), new PropertyMetadata(""));

        /// <summary>
        /// input text from the user
        /// </summary>
        public string InputText
        {
            get { return (string)GetValue(InputTextProperty); }
            set { SetValue(InputTextProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="InputText"/>
        /// </summary>
        public static readonly DependencyProperty InputTextProperty =
            DependencyProperty.Register("InputText", typeof(string),
              typeof(DefaultTextBox), new FrameworkPropertyMetadata(
            null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// event that will be fired when the <see cref="InputText"/> changes
        /// </summary>
        public event RoutedEventHandler TextChanged;

        /// <summary>
        /// creates a new instance of the <see cref="DefaultTextBox"/> class
        /// </summary>
        public DefaultTextBox()
        {
            InitializeComponent();    
        }

        /// <summary>
        /// fires the <see cref="TextChanged"/> event when the text box input has changed
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">text changed information</param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextChanged != null) TextChanged(sender, e);
        }
    }
}

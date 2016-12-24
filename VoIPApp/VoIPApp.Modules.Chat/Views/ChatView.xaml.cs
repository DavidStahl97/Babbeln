using Prism.Regions;
using System;
using System.Windows.Controls;

namespace VoIPApp.Modules.Chat.Views
{
    /// <summary>
    /// Interaction logic for ChatView
    /// </summary>
    public partial class ChatView : UserControl
    {
        public ChatView()
        {
            try
            {
                InitializeComponent();  
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void SendButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var btn = sender as Button;
            btn.Command.Execute(btn.CommandParameter);
            MessageTextBox.InputText = string.Empty;
        }
    }
}

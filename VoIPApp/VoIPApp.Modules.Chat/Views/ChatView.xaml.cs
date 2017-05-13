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

        private void MenuButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            /*ContextMenu cm = this.FindResource("FriendContextMenu") as ContextMenu;
            //cm.Style = FindResource("FlatContextMenu") as System.Windows.Style;
            cm.PlacementTarget = sender as Button;
            cm.IsOpen = true;*/
        }
    }
}

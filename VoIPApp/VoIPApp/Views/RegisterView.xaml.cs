using Prism.Regions;
using System;
using System.Security;
using System.Windows.Controls;
using VoIPApp.ViewModels;

namespace VoIPApp.Views
{
    /// <summary>
    /// Interaction logic for RegisterView
    /// </summary>
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();
        }

        /*public SecureString ConfirmationPassword
        {
            get { return this.PasswordBox.SecurePassword; }
        }

        public SecureString Password
        {
            get { return this.ConfirmationPasswordBox.SecurePassword; }
        }*/
    }
}

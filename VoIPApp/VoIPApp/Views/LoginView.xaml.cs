﻿using Prism.Regions;
using System;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using VoIPApp.ViewModels;

namespace VoIPApp.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class LoginView : UserControl, IHavePassword
    {
        public LoginView()
        {
            InitializeComponent();
        }

        public SecureString ConfirmationPassword
        {
            get { throw new NotImplementedException(); }
        }

        public SecureString Password
        {
            get { return PasswordBox.SecurePassword; }
        }
    }
}

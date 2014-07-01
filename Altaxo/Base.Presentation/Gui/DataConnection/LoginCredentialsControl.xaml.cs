using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.DataConnection
{
	/// <summary>
	/// Interaction logic for LoginCredentialsControl.xaml
	/// </summary>
	public partial class LoginCredentialsControl : UserControl, ILoginCredentialsView
	{
		private bool _isPasswordVisible;

		public LoginCredentialsControl()
		{
			InitializeComponent();
		}

		private void EhMakePasswordVisible(object sender, RoutedEventArgs e)
		{
			var pass = Password;
			_isPasswordVisible = true;
			Password = pass;
			SetPasswordControlsVisibility();
		}

		private void EhMakePasswordHidden(object sender, RoutedEventArgs e)
		{
			var pass = Password;
			_isPasswordVisible = false;
			Password = pass;
			SetPasswordControlsVisibility();
		}

		private void SetPasswordControlsVisibility()
		{
			_guiPasswordHidden.Visibility = _isPasswordVisible ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
			_guiPasswordVisible.Visibility = _isPasswordVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
		}

		public string Username
		{
			get
			{
				return _guiUsername.Text;
			}
			set
			{
				_guiUsername.Text = value;
			}
		}

		public string Password
		{
			get
			{
				return _isPasswordVisible ? _guiPasswordVisible.Text : _guiPasswordHidden.Password;
			}
			set
			{
				if (_isPasswordVisible)
					_guiPasswordVisible.Text = value;
				else
					_guiPasswordHidden.Password = value;
			}
		}
	}
}
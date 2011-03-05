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

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Interaction logic for HelpAboutControl.xaml
	/// </summary>
	public partial class HelpAboutControl : Window
	{
		public HelpAboutControl()
		{
			InitializeComponent();
		}


		public string VersionString
		{
			get
			{
				string result = "Version: ";

				var ass = System.Reflection.Assembly.GetEntryAssembly();

				result += ass.ToString();

				return result;
			}
		}

		private void EhOpenExplorer(object sender, RoutedEventArgs e)
		{
			var hyperlink = (Hyperlink)sender;
			System.Diagnostics.Process.Start(hyperlink.NavigateUri.ToString());
		}
	}
}

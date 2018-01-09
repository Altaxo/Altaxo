using Altaxo.Main.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Interaction logic for SaveErrorChooseDialog.xaml
	/// </summary>
	public partial class SaveErrorInformDialog : Window
	{
		private string displayMessage;
		private Exception exceptionGot;

		public SaveErrorInformDialog()
		{
			InitializeComponent();

			this.descriptionLabel.Content = StringParser.Parse("${res:ICSharpCode.Core.Services.ErrorDialogs.DescriptionLabel}");
			this.exceptionButton.Content = Current.ResourceService.GetString("ICSharpCode.Core.Services.ErrorDialogs.ShowExceptionButton");
			this.okButton.Content = StringParser.Parse("${res:Global.OKButtonText}");
		}

		public SaveErrorInformDialog(string fileName, string message, string dialogName, Exception exceptionGot)
		{
			//  Must be called for initialization
			this.InitializeComponent();

			this.descriptionLabel.Content = StringParser.Parse("${res:ICSharpCode.Core.Services.ErrorDialogs.DescriptionLabel}");
			this.exceptionButton.Content = Current.ResourceService.GetString("ICSharpCode.Core.Services.ErrorDialogs.ShowExceptionButton");
			this.okButton.Content = StringParser.Parse("${res:Global.OKButtonText}");
			this.Title = StringParser.Parse(dialogName);

			displayMessage = StringParser.Parse(
				message,
				new StringTagPair("FileName", fileName),
				new StringTagPair("Path", System.IO.Path.GetDirectoryName(fileName)),
				new StringTagPair("FileNameWithoutPath", System.IO.Path.GetFileName(fileName)),
				new StringTagPair("Exception", exceptionGot.GetType().FullName)
			);
			descriptionTextBox.Text = this.displayMessage;

			this.exceptionGot = exceptionGot;
		}

		private void EhShowException(object sender, RoutedEventArgs e)
		{
			MessageService.ShowMessage(exceptionGot.ToString(), "Exception got");
		}

		private void EhOKButtonClicked(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}
	}
}
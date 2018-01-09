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
	public partial class SaveErrorChooseDialog : Window
	{
		private string displayMessage;
		private Exception exceptionGot;

		public SaveErrorChooseDialogResult DetailedDialogResult { get; private set; }

		public enum SaveErrorChooseDialogResult
		{
			Retry,
			Ignore,
			ChooseLocation
		}

		public SaveErrorChooseDialog()
		{
			InitializeComponent();

			this.descriptionLabel.Content = StringParser.Parse("${res:ICSharpCode.Core.Services.ErrorDialogs.DescriptionLabel}");
			this.retryButton.Content = StringParser.Parse("${res:Global.RetryButtonText}");
			this.ignoreButton.Content = StringParser.Parse("${res:Global.IgnoreButtonText}");
			this.exceptionButton.Content = Current.ResourceService.GetString("ICSharpCode.Core.Services.ErrorDialogs.ShowExceptionButton");
			this.chooseLocationButton.Content = Current.ResourceService.GetString("Global.ChooseLocationButtonText");
		}

		public SaveErrorChooseDialog(string fileName, string message, string dialogName, Exception exceptionGot, bool chooseLocationEnabled)
		{
			InitializeComponent();

			this.descriptionLabel.Content = StringParser.Parse("${res:ICSharpCode.Core.Services.ErrorDialogs.DescriptionLabel}");
			this.retryButton.Content = StringParser.Parse("${res:Global.RetryButtonText}");
			this.ignoreButton.Content = StringParser.Parse("${res:Global.IgnoreButtonText}");
			this.exceptionButton.Content = Current.ResourceService.GetString("ICSharpCode.Core.Services.ErrorDialogs.ShowExceptionButton");
			this.chooseLocationButton.Content = Current.ResourceService.GetString("Global.ChooseLocationButtonText");

			this.Title = StringParser.Parse(dialogName);
			//  Must be called for initialization
			chooseLocationButton.IsEnabled = chooseLocationEnabled;

			displayMessage = StringParser.Parse(
				message,
				new StringTagPair("FileName", fileName),
				new StringTagPair("Path", System.IO.Path.GetDirectoryName(fileName)),
				new StringTagPair("FileNameWithoutPath", System.IO.Path.GetFileName(fileName)),
				new StringTagPair("Exception", exceptionGot.GetType().FullName)
			);

			descriptionTextBox.Text = StringParser.Parse(this.displayMessage);

			this.exceptionGot = exceptionGot;
		}

		private void EhShowException(object sender, RoutedEventArgs e)
		{
			MessageService.ShowMessage(exceptionGot.ToString(), StringParser.Parse("${res:ICSharpCode.Core.Services.ErrorDialogs.ExceptionGotDescription}"));
		}

		private void EhRetryButton_Clicked(object sender, RoutedEventArgs e)
		{
			DetailedDialogResult = SaveErrorChooseDialogResult.Retry;
			DialogResult = true;
		}

		private void EhIgnoreButton_Clicked(object sender, RoutedEventArgs e)
		{
			DetailedDialogResult = SaveErrorChooseDialogResult.Ignore;
			DialogResult = true;
		}

		private void EhChooseLocationButton_Clicked(object sender, RoutedEventArgs e)
		{
			DetailedDialogResult = SaveErrorChooseDialogResult.ChooseLocation;
			DialogResult = true;
		}
	}
}
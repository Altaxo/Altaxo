#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Interaction logic for SaveErrorChooseDialog.xaml
  /// </summary>
  public partial class SaveErrorChooseDialog : Window
  {
    private string displayMessage;
    private Exception exceptionGot;

    /// <summary>
    /// Gets the detailed dialog result.
    /// </summary>
    public SaveErrorChooseDialogResult DetailedDialogResult { get; private set; }

    /// <summary>
    /// Represents the detailed outcome of the dialog.
    /// </summary>
    public enum SaveErrorChooseDialogResult
    {
      /// <summary>
      /// Retry saving.
      /// </summary>
      Retry,
      /// <summary>
      /// Ignore the error.
      /// </summary>
      Ignore,
      /// <summary>
      /// Choose a new location.
      /// </summary>
      ChooseLocation
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveErrorChooseDialog"/> class.
    /// </summary>
    public SaveErrorChooseDialog()
    {
      InitializeComponent();

      descriptionLabel.Content = StringParser.Parse("${res:Altaxo.Gui.Common.ErrorDialogs.DescriptionLabel}");
      retryButton.Content = StringParser.Parse("${res:Global.RetryButtonText}");
      ignoreButton.Content = StringParser.Parse("${res:Global.IgnoreButtonText}");
      exceptionButton.Content = Current.ResourceService.GetString("Altaxo.Gui.Common.ErrorDialogs.ShowExceptionButton");
      chooseLocationButton.Content = Current.ResourceService.GetString("Global.ChooseLocationButtonText");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveErrorChooseDialog"/> class.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <param name="message">The message template.</param>
    /// <param name="dialogName">The dialog title.</param>
    /// <param name="exceptionGot">The exception that occurred.</param>
    /// <param name="chooseLocationEnabled">If set to <c>true</c>, the choose-location option is enabled.</param>
    public SaveErrorChooseDialog(string fileName, string message, string dialogName, Exception exceptionGot, bool chooseLocationEnabled)
    {
      InitializeComponent();

      descriptionLabel.Content = StringParser.Parse("${res:Altaxo.Gui.Common.ErrorDialogs.DescriptionLabel}");
      retryButton.Content = StringParser.Parse("${res:Global.RetryButtonText}");
      ignoreButton.Content = StringParser.Parse("${res:Global.IgnoreButtonText}");
      exceptionButton.Content = Current.ResourceService.GetString("Altaxo.Gui.Common.ErrorDialogs.ShowExceptionButton");
      chooseLocationButton.Content = Current.ResourceService.GetString("Global.ChooseLocationButtonText");

      Title = StringParser.Parse(dialogName);
      //  Must be called for initialization
      chooseLocationButton.IsEnabled = chooseLocationEnabled;

      displayMessage = StringParser.Parse(
        message,
        new StringTagPair("FileName", fileName),
        new StringTagPair("Path", System.IO.Path.GetDirectoryName(fileName)),
        new StringTagPair("FileNameWithoutPath", System.IO.Path.GetFileName(fileName)),
        new StringTagPair("Exception", exceptionGot.GetType().FullName)
      );

      descriptionTextBox.Text = StringParser.Parse(displayMessage);

      this.exceptionGot = exceptionGot;
    }

    private void EhShowException(object sender, RoutedEventArgs e)
    {
      MessageService.ShowMessage(exceptionGot.ToString(), StringParser.Parse("${res:Altaxo.Gui.Common.ErrorDialogs.ExceptionGotDescription}"));
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

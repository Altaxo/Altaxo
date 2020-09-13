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
    private string? displayMessage;
    private Exception? exceptionGot;

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

      descriptionLabel.Content = StringParser.Parse("${res:ICSharpCode.Core.Services.ErrorDialogs.DescriptionLabel}");
      retryButton.Content = StringParser.Parse("${res:Global.RetryButtonText}");
      ignoreButton.Content = StringParser.Parse("${res:Global.IgnoreButtonText}");
      exceptionButton.Content = Current.ResourceService.GetString("ICSharpCode.Core.Services.ErrorDialogs.ShowExceptionButton");
      chooseLocationButton.Content = Current.ResourceService.GetString("Global.ChooseLocationButtonText");
    }

    public SaveErrorChooseDialog(string fileName, string message, string dialogName, Exception exceptionGot, bool chooseLocationEnabled)
    {
      InitializeComponent();

      descriptionLabel.Content = StringParser.Parse("${res:ICSharpCode.Core.Services.ErrorDialogs.DescriptionLabel}");
      retryButton.Content = StringParser.Parse("${res:Global.RetryButtonText}");
      ignoreButton.Content = StringParser.Parse("${res:Global.IgnoreButtonText}");
      exceptionButton.Content = Current.ResourceService.GetString("ICSharpCode.Core.Services.ErrorDialogs.ShowExceptionButton");
      chooseLocationButton.Content = Current.ResourceService.GetString("Global.ChooseLocationButtonText");

      Title = StringParser.Parse(dialogName);
      //  Must be called for initialization
      chooseLocationButton.IsEnabled = chooseLocationEnabled;

      displayMessage = StringParser.Parse(
          message,
          new StringTagPair("FileName", fileName),
          new StringTagPair("Path", FileName.GetDirectoryName(fileName)),
          new StringTagPair("FileNameWithoutPath", System.IO.Path.GetFileName(fileName)),
          new StringTagPair("Exception", exceptionGot.GetType().FullName ?? exceptionGot.GetType().Name)
      );

      descriptionTextBox.Text = StringParser.Parse(displayMessage);

      this.exceptionGot = exceptionGot;
    }

    private void EhShowException(object sender, RoutedEventArgs e)
    {
      MessageService.ShowMessage(exceptionGot?.ToString() ?? string.Empty, StringParser.Parse("${res:ICSharpCode.Core.Services.ErrorDialogs.ExceptionGotDescription}"));
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

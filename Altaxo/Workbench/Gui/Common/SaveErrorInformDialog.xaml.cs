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
  public partial class SaveErrorInformDialog : Window
  {
    private string? displayMessage;
    private Exception? exceptionGot;

    public SaveErrorInformDialog()
    {
      InitializeComponent();

      descriptionLabel.Content = StringParser.Parse("${res:Altaxo.Gui.Common.ErrorDialogs.DescriptionLabel}");
      exceptionButton.Content = Current.ResourceService.GetString("Altaxo.Gui.Common.ErrorDialogs.ShowExceptionButton");
      okButton.Content = StringParser.Parse("${res:Global.OKButtonText}");
    }

    public SaveErrorInformDialog(string fileName, string message, string dialogName, Exception exceptionGot)
    {
      //  Must be called for initialization
      InitializeComponent();

      descriptionLabel.Content = StringParser.Parse("${res:Altaxo.Gui.Common.ErrorDialogs.DescriptionLabel}");
      exceptionButton.Content = Current.ResourceService.GetString("Altaxo.Gui.Common.ErrorDialogs.ShowExceptionButton");
      okButton.Content = StringParser.Parse("${res:Global.OKButtonText}");
      Title = StringParser.Parse(dialogName);

      displayMessage = StringParser.Parse(
          message,
          new StringTagPair("FileName", fileName),
          new StringTagPair("Path", FileName.GetDirectoryName(fileName)),
          new StringTagPair("FileNameWithoutPath", System.IO.Path.GetFileName(fileName)),
          new StringTagPair("Exception", exceptionGot.GetType().FullName ?? exceptionGot.GetType().Name)
      );
      descriptionTextBox.Text = displayMessage;

      this.exceptionGot = exceptionGot;
    }

    private void EhShowException(object sender, RoutedEventArgs e)
    {
      MessageService.ShowMessage(exceptionGot?.ToString() ?? string.Empty, "Exception got");
    }

    private void EhOKButtonClicked(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
      Close();
    }
  }
}

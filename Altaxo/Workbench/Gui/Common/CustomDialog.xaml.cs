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

using Altaxo.Main.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Interaction logic for SaveErrorChooseDialog.xaml
  /// </summary>
  public partial class CustomDialog : Window
  {
    public int Result { get; private set; } = -1;

    private string displayMessage;
    private Exception exceptionGot;

    private int cancelButton;
    private int acceptButton;

    private Button AcceptButton;

    private Button CancelButton;

    public CustomDialog()
    {
      InitializeComponent();
    }

    public CustomDialog(string caption, string message, int acceptButton, int cancelButton, string[] buttonLabels)
    {
      InitializeComponent();

      this.Icon = null;
      this.acceptButton = acceptButton;
      this.cancelButton = cancelButton;

      message = StringParser.Parse(message);
      this.Title = StringParser.Parse(caption);

      buttonGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

      int idx = 0;
      foreach (var lbl in buttonLabels)
      {
        buttonGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
        buttonGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

        var newButton = new Button()
        {
          Tag = idx,
          Content = StringParser.Parse(lbl)
        };

        newButton.Click += EhButtonClick;

        if (acceptButton == idx)
        {
          AcceptButton = newButton;
        }
        if (cancelButton == idx)
        {
          CancelButton = newButton;
        }

        ++idx;
      }

      label.Content = message;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      if (cancelButton == -1 && e.Key == Key.Escape)
      {
        this.Close();
      }
    }

    private void EhButtonClick(object sender, RoutedEventArgs e)
    {
      Result = (int)((Button)sender).Tag;
      this.DialogResult = true;
      this.Close();
    }
  }
}

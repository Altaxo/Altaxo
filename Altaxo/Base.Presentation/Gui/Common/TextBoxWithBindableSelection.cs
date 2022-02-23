#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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

using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// TextBox with a bindable SelectionInfo property (two way be default).
  /// </summary>
  /// <seealso cref="System.Windows.Controls.TextBox" />
  public class TextBoxWithBindableSelection : TextBox
  {
    public TextBoxWithBindableSelection()
    {
    }

    protected override void OnSelectionChanged(RoutedEventArgs e)
    {
      base.OnSelectionChanged(e);
      SelectionInfo = (this.SelectionStart, this.SelectionLength);
    }

    /// <summary>
    /// Gets or sets the selection information.
    /// </summary>
    /// <value>
    /// The selection information, consisting of Start and Length of the selection.
    /// </value>
    public (int Start, int Length) SelectionInfo
    {
      get
      {
        return ((int, int))GetValue(SelectionInfoProperty);
      }
      set
      {
        SetValue(SelectionInfoProperty, value);
      }
    }

    public static readonly DependencyProperty SelectionInfoProperty =
      DependencyProperty.Register(nameof(SelectionInfo), typeof((int, int)),
      typeof(TextBoxWithBindableSelection),
      new FrameworkPropertyMetadata(EhSelectionInfoChanged) { BindsTwoWayByDefault = true });

    private static void EhSelectionInfoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if(d is TextBoxWithBindableSelection thiss)
      {
        var newSel = ((int start, int length))e.NewValue;
        if(newSel.start != thiss.SelectionStart || newSel.length != thiss.SelectionLength)
        {
          thiss.Focus();
          thiss.Select(newSel.start, newSel.length);
        }
      }
    }
  }
}

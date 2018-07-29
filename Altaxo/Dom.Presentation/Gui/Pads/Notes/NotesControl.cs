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
using System.Windows.Controls;
using System.Windows.Input;

namespace Altaxo.Gui.Pads.Notes
{
  public class NotesControl : TextBox, INotesView
  {
    private System.Windows.Data.BindingExpressionBase _textBinding;

    public event Action ShouldSaveText;

    public NotesControl()
    {
      this.TextWrapping = System.Windows.TextWrapping.NoWrap;
      this.AcceptsReturn = true;
      this.AcceptsTab = true;
      this.VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Visible;
      this.HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Visible;
      this.FontFamily = new System.Windows.Media.FontFamily("Global Monospace");

      this.LostFocus += new System.Windows.RoutedEventHandler(EhLostFocus);
      this.LostKeyboardFocus += new System.Windows.Input.KeyboardFocusChangedEventHandler(EhLostKeyboardFocus);
      this.TextChanged += new System.Windows.Controls.TextChangedEventHandler(EhTextChanged);
      this.IsEnabled = false;
    }

    public void ClearBinding()
    {
      // Clears the old binding
      _textBinding = null; // to avoid updates when the text changed in the next line, and then the TextChanged event of the TextBox is triggered
      System.Windows.Data.BindingOperations.ClearBinding(this, System.Windows.Controls.TextBox.TextProperty);
    }

    public void SetTextFromNotesAndSetBinding(Altaxo.Main.ITextBackedConsole con)
    {
      this.Text = con.Text;

      var binding = new System.Windows.Data.Binding
      {
        Source = con,
        Path = new System.Windows.PropertyPath(nameof(this.Text)),
        Mode = System.Windows.Data.BindingMode.TwoWay
      };
      _textBinding = this.SetBinding(System.Windows.Controls.TextBox.TextProperty, binding);
    }

    private void EhTextChanged(object sender, TextChangedEventArgs e)
    {
      _textBinding?.UpdateSource();
    }

    private void EhLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
      ShouldSaveText?.Invoke();
    }

    private void EhLostFocus(object sender, RoutedEventArgs e)
    {
      ShouldSaveText?.Invoke();
    }
  }
}

#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Altaxo.Gui.Common
{
  public class DateTimeOffsetTextBox : TextBox
  {
    public event DependencyPropertyChangedEventHandler? SelectedValueChanged;

    private DateTimeOffsetConverter _converter;

    /// <summary>
    /// Static initialization.
    /// </summary>
    static DateTimeOffsetTextBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimeOffsetTextBox), new FrameworkPropertyMetadata(typeof(DateTimeOffsetTextBox)));
    }

    /// <summary>
    /// Default constructor.
    /// </summary>
    public DateTimeOffsetTextBox()
    {
      var binding = new Binding
      {
        Source = this,
        Path = new PropertyPath("SelectedValue"),
        Mode = BindingMode.TwoWay,
        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
      };
      _converter = new DateTimeOffsetConverter();
      binding.Converter = _converter;
      binding.ValidationRules.Add(_converter);
      SetBinding(TextBox.TextProperty, binding);
    }

    #region Change selection behaviour

    // The next three overrides change the selection behaviour of the text box as described in
    // 'How to SelectAll in TextBox when TextBox gets focus by mouse click?'
    // (http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/564b5731-af8a-49bf-b297-6d179615819f/)

    protected override void OnGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
    {
      SelectAll();
      base.OnGotKeyboardFocus(e);
    }

    protected override void OnMouseDoubleClick(System.Windows.Input.MouseButtonEventArgs e)
    {
      SelectAll();
      base.OnMouseDoubleClick(e);
    }

    protected override void OnPreviewMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
    {
      if (!IsKeyboardFocusWithin)
      {
        e.Handled = true;
        Focus();
      }
      else
      {
        base.OnPreviewMouseLeftButtonDown(e);
      }
    }

    #endregion Change selection behaviour

    #region Dependency property

    /// <summary>
    /// Gets/sets the quantity. The quantity consist of a numeric value together with a unit.
    /// </summary>
    public DateTimeOffset SelectedValue
    {
      get { return (DateTimeOffset)GetValue(SelectedValueProperty); }
      set { SetValue(SelectedValueProperty, value); }
    }

    public static readonly DependencyProperty SelectedValueProperty =
        DependencyProperty.Register("SelectedValue", typeof(DateTimeOffset), typeof(DateTimeOffsetTextBox),
        new FrameworkPropertyMetadata(EhSelectedValueChanged));

    private static void EhSelectedValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((DateTimeOffsetTextBox)obj).OnSelectedValueChanged(obj, args);
    }

    /// <summary>
    /// Triggers the <see cref="SelectedValueChanged"/> event.
    /// </summary>
    /// <param name="obj">Dependency object (here: the control).</param>
    /// <param name="args">Property changed event arguments.</param>
    protected void OnSelectedValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        SelectedValueChanged?.Invoke(obj, args);
    }

    #endregion Dependency property
  }
}

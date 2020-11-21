#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Interaction logic for CheckableGroupBox.xaml
  /// </summary>
  public partial class CheckableGroupBox : GroupBox
  {
    public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(
  "IsChecked",
  typeof(bool?),
  typeof(CheckableGroupBox), new PropertyMetadata(false)
  );

    public static readonly DependencyProperty EnableContentWithCheckProperty = DependencyProperty.Register(
  "EnableContentWithCheck",
  typeof(bool),
  typeof(CheckableGroupBox)
  );

    public event RoutedEventHandler? Checked;

    public event RoutedEventHandler? Unchecked;

    public CheckableGroupBox()
    {
      InitializeComponent();
    }

    public bool? IsChecked
    {
      get
      {
        return (bool?)GetValue(CheckableGroupBox.IsCheckedProperty);
      }
      set
      {
        SetValue(CheckableGroupBox.IsCheckedProperty, value);
      }
    }

    /// <summary>
    /// If set to true, the content of the group box is enabled when the CheckBox is checked, and disabled, when it is unchecked.
    /// </summary>
    public bool EnableContentWithCheck
    {
      get
      {
        return (bool)GetValue(CheckableGroupBox.EnableContentWithCheckProperty);
      }
      set
      {
        SetValue(CheckableGroupBox.EnableContentWithCheckProperty, value);
      }
    }

    private void EhCheckBox_Checked(object sender, RoutedEventArgs e)
    {
      if (EnableContentWithCheck && Content is UIElement)
        (Content as UIElement).IsEnabled = true;

      if (Checked is not null)
        Checked(this, e);
    }

    private void EhCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
      if (EnableContentWithCheck && Content is UIElement)
        (Content as UIElement).IsEnabled = false;

      if (Unchecked is not null)
        Unchecked(this, e);
    }

    protected override void OnContentChanged(object oldContent, object newContent)
    {
      base.OnContentChanged(oldContent, newContent);

      if (EnableContentWithCheck)
      {
        var uicontent = newContent as UIElement;
        if (uicontent is not null)
          uicontent.IsEnabled = IsChecked == true;
      }
    }
  }
}

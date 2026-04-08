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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Altaxo.Collections;

namespace Altaxo.Gui.Main
{
  /// <summary>
  /// Interaction logic for AddBasicPropertyValueControl.xaml
  /// </summary>
  public partial class AddBasicPropertyValueControl : UserControl, IAddBasicPropertyValueView
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="AddBasicPropertyValueControl"/> class.
    /// </summary>
    public AddBasicPropertyValueControl()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Gets or sets the property name.
    /// </summary>
    public string PropertyName
    {
      get
      {
        return _guiPropertyName.Text;
      }

      set
      {
        _guiPropertyName.Text = value;
      }
    }

    /// <summary>
    /// Gets or sets the available property types.
    /// </summary>
    public SelectableListNodeList PropertyTypes
    {
      set
      {
        GuiHelper.Initialize(_guiPropertyType, value);
      }
    }

    private void ShowNoValueControl()
    {
      _guiPropertyValueAsString.Visibility = Visibility.Collapsed;
      _guiPropertyValueAsDouble.Visibility = Visibility.Collapsed;
      _guiPropertyValueAsInt.Visibility = Visibility.Collapsed;
      _guiPropertyValueAsDateTime.Visibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the date-time editor is shown.
    /// </summary>
    public bool ShowValueAsDateTime
    {
      set
      {
        if (true == value)
        {
          ShowNoValueControl();
          _guiPropertyValueAsDateTime.Visibility = Visibility.Visible;
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the floating-point editor is shown.
    /// </summary>
    public bool ShowValueAsDouble
    {
      set
      {
        if (true == value)
        {
          ShowNoValueControl();
          _guiPropertyValueAsDouble.Visibility = Visibility.Visible;
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the integer editor is shown.
    /// </summary>
    public bool ShowValueAsInt
    {
      set
      {
        if (true == value)
        {
          ShowNoValueControl();
          _guiPropertyValueAsInt.Visibility = Visibility.Visible;
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the string editor is shown.
    /// </summary>
    public bool ShowValueAsString
    {
      set
      {
        if (true == value)
        {
          ShowNoValueControl();
          _guiPropertyValueAsString.Visibility = Visibility.Visible;
        }
      }
    }

    /// <summary>
    /// Gets or sets the date-time value.
    /// </summary>
    public DateTime ValueAsDateTime
    {
      get
      {
        return _guiPropertyValueAsDateTime.SelectedValue;
      }

      set
      {
        _guiPropertyValueAsDateTime.SelectedValue = value;
      }
    }

    /// <summary>
    /// Gets or sets the floating-point value.
    /// </summary>
    public double ValueAsDouble
    {
      get
      {
        return _guiPropertyValueAsDouble.SelectedValue;
      }

      set
      {
        _guiPropertyValueAsDouble.SelectedValue = value;
      }
    }

    /// <summary>
    /// Gets or sets the integer value.
    /// </summary>
    public int ValueAsInt
    {
      get
      {
        return _guiPropertyValueAsInt.Value;
      }

      set
      {
        _guiPropertyValueAsInt.Value = value;
      }
    }

    /// <summary>
    /// Gets or sets the string value.
    /// </summary>
    public string ValueAsString
    {
      get
      {
        return _guiPropertyValueAsString.Text;
      }

      set
      {
        _guiPropertyValueAsString.Text = value;
      }
    }

    /// <summary>
    /// Occurs when the selected property type changes.
    /// </summary>
    public event Action? PropertyTypeChanged;

    private void EhPropertyTypeChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiPropertyType);
      var ev = PropertyTypeChanged;
      if (ev is not null)
        ev();
    }
  }
}

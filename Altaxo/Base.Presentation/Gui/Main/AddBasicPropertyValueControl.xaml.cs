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
    public AddBasicPropertyValueControl()
    {
      InitializeComponent();
    }

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

    public event Action PropertyTypeChanged;

    private void EhPropertyTypeChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiPropertyType);
      var ev = PropertyTypeChanged;
      if (ev is not null)
        ev();
    }
  }
}

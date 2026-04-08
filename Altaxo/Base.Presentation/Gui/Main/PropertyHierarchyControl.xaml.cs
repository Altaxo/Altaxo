#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using System.Windows.Input;

namespace Altaxo.Gui.Main
{
  /// <summary>
  /// Interaction logic for PropertyBagControl.xaml
  /// </summary>
  public partial class PropertyHierarchyControl : UserControl, IPropertyHierarchyView
  {
    /// <summary>
    /// Occurs when the selected item should be edited.
    /// </summary>
    public event Action? ItemEditing;

    /// <summary>
    /// Occurs when the selected item should be removed.
    /// </summary>
    public event Action? ItemRemoving;

    /// <summary>
    /// Occurs when a new property should be created.
    /// </summary>
    public event Action? PropertyCreation;

    /// <summary>
    /// Occurs when a new basic property should be added.
    /// </summary>
    public event Action? AddNewBasicProperty;

    /// <summary>
    /// Occurs when the show-all-properties setting changes.
    /// </summary>
    public event Action<bool>? ShowAllPropertiesChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyHierarchyControl"/> class.
    /// </summary>
    public PropertyHierarchyControl()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Gets or sets the list of property values.
    /// </summary>
    public Collections.SelectableListNodeList PropertyValueList
    {
      set { GuiHelper.Initialize(_guiPropertyList, value); }
    }

    /// <summary>
    /// Gets or sets the list of available property keys.
    /// </summary>
    public Collections.SelectableListNodeList AvailablePropertyKeyList
    {
      set { GuiHelper.Initialize(_guiAvailablePropertyKeyList, value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether all properties are shown.
    /// </summary>
    public bool ShowAllProperties
    {
      set
      {
        if (false == value)
          _guiShowEditablePropertiesOnly.IsChecked = true;
        else
          _guiShowAllProperties.IsChecked = true;
      }
    }

    private void EhListViewMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiPropertyList);
      var ev = ItemEditing;
      if (ev is not null)
        ev();
    }

    private void EhAvailablePropertyKeyListMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiAvailablePropertyKeyList);
      var ev = PropertyCreation;
      if (ev is not null)
        ev();
    }

    private void EhAddNewProperty(object sender, RoutedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiAvailablePropertyKeyList);
      var ev = PropertyCreation;
      if (ev is not null)
        ev();
    }

    private void EhAddNewBasicProperty(object sender, RoutedEventArgs e)
    {
      var ev = AddNewBasicProperty;
      if (ev is not null)
        ev();
    }

    private void EhEditPropertyValue(object sender, RoutedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiPropertyList);
      var ev = ItemEditing;
      if (ev is not null)
        ev();
    }

    private void EhRemoveProperty(object sender, RoutedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiPropertyList);
      var ev = ItemRemoving;
      if (ev is not null)
        ev();
    }

    private void EhShowEditablePropertiesOnly(object sender, RoutedEventArgs e)
    {
      var ev = ShowAllPropertiesChanged;
      if (ev is not null)
        ev(false);
    }

    private void EhShowAllProperties(object sender, RoutedEventArgs e)
    {
      var ev = ShowAllPropertiesChanged;
      if (ev is not null)
        ev(true);
    }
  }
}

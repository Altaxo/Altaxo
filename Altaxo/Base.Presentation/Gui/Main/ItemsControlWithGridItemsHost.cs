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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Altaxo.Gui.Main
{
  /// <summary>
  /// ItemsControl (to with items can be bound to using <see cref="ItemsControl.ItemsSource"/>, but which features a <see cref="Grid"/> control as the ItemsHost.
  /// The number of columns in the Grid must be fixed using <see cref="NumberOfColumns"/> (default is 4).
  /// Items that are derived from <see cref="FrameworkElement"/> will be used directly. All other items are using the item template of the <see cref="ItemsControl"/>.
  /// </summary>
  /// <seealso cref="System.Windows.Controls.ItemsControl" />
  /// <example>
  /// Use this class in a way like this:
  /// <code>
  ///     <local:ItemsControlWithGridItemsHost ItemsSource="{Binding ControllerList}" NumberOfColumns="4">
  ///      <ItemsControl.ItemsPanel>
  ///          <ItemsPanelTemplate>
  ///              <Grid>
  ///                  <Grid.ColumnDefinitions>
  ///                      <ColumnDefinition Width = "Auto" SharedSizeGroup="LabelColumn1"/>
  ///                      <ColumnDefinition Width = "Auto" SharedSizeGroup="EditColumn1" />
  ///                      <ColumnDefinition Width = "Auto" SharedSizeGroup="LabelColumn2"/>
  ///                      <ColumnDefinition Width = "Auto" SharedSizeGroup="EditColumn2"/>
  ///                  </Grid.ColumnDefinitions>
  ///                  <Grid.RowDefinitions>
  ///                      <RowDefinition Height = "Auto" />
  ///                  </ Grid.RowDefinitions >
  ///              </ Grid >
  ///          </ ItemsPanelTemplate >
  ///      </ ItemsControl.ItemsPanel >
  ///      < ItemsControl.ItemTemplate >
  ///          < DataTemplate >
  ///              < Label Content="{Binding}" />
  ///          </DataTemplate>
  ///      </ItemsControl.ItemTemplate>
  ///      <ItemsControl.ItemContainerStyle>
  ///          <Style>
  ///          </Style>
  ///      </ItemsControl.ItemContainerStyle>
  ///  </local:ItemsControlWithGridItemsHost>
  /// </code>
  /// </example>
  public class ItemsControlWithGridItemsHost : ItemsControl
  {
    public ItemsControlWithGridItemsHost()
    {
      AlternationCount = 65535; // is neccessary in order to use AlternationIndex as an index of the container elements
    }

    /// <summary>
    /// Gets or sets the number of columns of the grid (default: 4).
    /// </summary>
    /// <value>
    /// The number of columns of the grid (default: 4).
    /// </value>
    public int NumberOfColumns { get; set; } = 4;

    /// <summary>
    /// Prepares the specified element to display the specified item.
    /// Here, we use the <see cref="ItemsControl.AlternationIndexProperty"/> to get the index of the element. Using that index, we can calculate
    /// the row and column property of the grid, and set it at the element. Additionally, new rows are inserted in the grid if needed.
    /// </summary>
    /// <param name="element">Element used to display the specified item.</param>
    /// <param name="item">Specified item.</param>
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
      base.PrepareContainerForItemOverride(element, item);
      var idx = (int)element.GetValue(ItemsControl.AlternationIndexProperty);

      var row = idx / NumberOfColumns;
      int column = idx % NumberOfColumns;

      element.SetValue(Grid.RowProperty, row);
      element.SetValue(Grid.ColumnProperty, column);
      if (element is FrameworkElement l)
      {
        l.Margin = new Thickness(4);
      }

      if (row >= 1 && ItemsHost is Grid g)
      {
        if (row >= g.RowDefinitions.Count)
        {
          g.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
        }
      }
    }

    /// <summary>
    /// Gets the items host of this items control. Here, it should be a grid.
    /// </summary>
    /// <value>
    /// The items host.
    /// </value>
    protected Panel ItemsHost
    {
      get
      {
        return (Panel)typeof(MultiSelector).InvokeMember("ItemsHost",
            BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Instance,
            null, this, null);
      }
    }

    /// <summary>
    /// Here, if the item is a <see cref="DependencyObject"/>, we assume to use this object directly.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>
    ///   <see langword="true" /> if the item is (or is eligible to be) its own container; otherwise, <see langword="false" />.
    /// </returns>
    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return item is FrameworkElement;
    }
  }
}

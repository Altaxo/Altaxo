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

#nullable disable warnings
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Altaxo.Units;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Interaction logic for QuantityWithUnitList.xaml
  /// </summary>
  public partial class QuantityWithUnitList : UserControl
  {
    public QuantityWithUnitList()
    {
      InitializeComponent();
    }


    /// <summary>
    /// Gets or sets the unit environment that is used for all Gui elements.
    /// </summary>
    /// <value>
    /// The unit environment.
    /// </value>
    public QuantityWithUnitGuiEnvironment Environment
    {
      get { return (QuantityWithUnitGuiEnvironment)GetValue(EnvironmentProperty); }
      set { SetValue(EnvironmentProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Environment.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty EnvironmentProperty =
        DependencyProperty.Register(nameof(Environment),
          typeof(QuantityWithUnitGuiEnvironment),
          typeof(QuantityWithUnitList),
          new PropertyMetadata(EhEnvironmentChanged));

    private static void EhEnvironmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (e.OldValue is null && e.NewValue is not null)
      {
        ((QuantityWithUnitList)d).FillItems();
      }
    }

    /// <summary>
    /// Sets the default quantity, i.e. that quantity that is used if the user inserts a new row.
    /// </summary>
    /// <value>
    /// The default quantity.
    /// </value>
    public DimensionfulQuantity DefaultQuantity
    {
      get { return (DimensionfulQuantity)GetValue(DefaultQuantityProperty); }
      set { SetValue(DefaultQuantityProperty, value); }
    }

    // Using a DependencyProperty as the backing store for DefaultQuantity.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty DefaultQuantityProperty =
        DependencyProperty.Register(nameof(DefaultQuantity),
          typeof(DimensionfulQuantity),
          typeof(QuantityWithUnitList),
          new PropertyMetadata(DimensionfulQuantity.Empty, EhDefaultQuantityChanged));

    private static void EhDefaultQuantityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (e.OldValue is DimensionfulQuantity old && old.IsEmpty && e.NewValue is DimensionfulQuantity newq && !newq.IsEmpty)
      {
        ((QuantityWithUnitList)d).FillItems();
      }
    }

    /// <summary>
    /// Gets or sets the items source, i.e. a collection of <see cref="Altaxo.Units.DimensionfulQuantity"/>
    /// </summary>
    /// <value>
    /// The items source.
    /// </value>
    public ObservableCollection<DimensionfulQuantity> ItemsSource
    {
      get { return (ObservableCollection<DimensionfulQuantity>)GetValue(ItemsSourceProperty); }
      set { SetValue(ItemsSourceProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource),
          typeof(ObservableCollection<DimensionfulQuantity>),
          typeof(QuantityWithUnitList),
          new PropertyMetadata(EhItemsSourceChanged));

    private static void EhItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((QuantityWithUnitList)d).FillItems();
    }

    private void FillItems()
    {
      _guiItemStack.Children.Clear();

      if (Environment is null || ItemsSource is null || DefaultQuantity.IsEmpty)
        return;

      for (int i = 0; i < ItemsSource.Count; ++i)
      {
        var sp = new DockPanel { HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch };
        var ta = new Button() { Content = "+", Margin = new Thickness(0, -8, 4, 8), Tag = i };
        var tb = new QuantityWithUnitTextBox { UnitEnvironment = Environment, SelectedQuantity = ItemsSource[i], Tag = i, HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch };
        var tc = new Button { Content = "Del", Tag = i, Margin = new Thickness(4, 0, 0, 0) };
        tb.SelectedQuantityChanged += EhSelectedQuantityChanged;
        ta.Click += EhAddRow;
        tc.Click += EhDelRow;

        ta.SetValue(DockPanel.DockProperty, Dock.Left);
        sp.Children.Add(ta);

        tc.SetValue(DockPanel.DockProperty, Dock.Right);
        sp.Children.Add(tc);

        sp.Children.Add(tb);

        _guiItemStack.Children.Add(sp);
      }

      var sp1 = new StackPanel { Orientation = Orientation.Horizontal };
      var ta1 = new Button() { Content = "+", Margin = new Thickness(0, -8, 0, 8), Tag = ItemsSource.Count };
      ta1.Click += EhAddRow;
      sp1.Children.Add(ta1);

      _guiItemStack.Children.Add(sp1);
    }

    private void EhDelRow(object sender, RoutedEventArgs e)
    {
      if (ItemsSource is not null)
      {
        var qb = (Button)sender;
        int idx = (int)qb.Tag;
        ItemsSource.RemoveAt(idx);
        FillItems();
      }
    }

    private void EhAddRow(object sender, RoutedEventArgs e)
    {
      if (ItemsSource is not null)
      {
        var qb = (Button)sender;
        int idx = (int)qb.Tag;
        ItemsSource.Insert(idx, DefaultQuantity);
        FillItems();
      }
    }

    private void EhSelectedQuantityChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (ItemsSource is not null)
      {
        var qb = (QuantityWithUnitTextBox)sender;
        int idx = (int)qb.Tag;
        ItemsSource[idx] = qb.SelectedQuantity;
      }
    }
  }
}

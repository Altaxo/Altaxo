// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

#nullable disable warnings
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Allows to automatically sort a grid view.
  /// </summary>
  public class SortableGridViewColumn : GridViewColumn
  {
    private static readonly ComponentResourceKey headerTemplateKey = new ComponentResourceKey(typeof(SortableGridViewColumn), "ColumnHeaderTemplate");

    /// <summary>
    /// Initializes a new instance of the <see cref="SortableGridViewColumn"/> class.
    /// </summary>
    public SortableGridViewColumn()
    {
      this.SetValueToExtension(HeaderTemplateProperty, new DynamicResourceExtension(headerTemplateKey));
    }

    private string sortBy;

    /// <summary>
    /// Gets or sets the property name that should be used for sorting.
    /// </summary>
    public string SortBy
    {
      get { return sortBy; }
      set
      {
        if (sortBy != value)
        {
          sortBy = value;
          OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("SortBy"));
        }
      }
    }

    #region SortDirection property

    /// <summary>
    /// Identifies the <see cref="SortDirection"/> attached dependency property.
    /// </summary>
    public static readonly DependencyProperty SortDirectionProperty =
      DependencyProperty.RegisterAttached("SortDirection", typeof(ColumnSortDirection), typeof(SortableGridViewColumn),
                                          new FrameworkPropertyMetadata(ColumnSortDirection.None, OnSortDirectionChanged));

    /// <summary>
    /// Gets or sets the sort direction.
    /// </summary>
    public ColumnSortDirection SortDirection
    {
      get { return (ColumnSortDirection)GetValue(SortDirectionProperty); }
      set { SetValue(SortDirectionProperty, value); }
    }

    /// <summary>
    /// Gets the sort direction attached to the specified list view.
    /// </summary>
    /// <param name="listView">The list view.</param>
    /// <returns>The current sort direction.</returns>
    public static ColumnSortDirection GetSortDirection(ListView listView)
    {
      return (ColumnSortDirection)listView.GetValue(SortDirectionProperty);
    }

    /// <summary>
    /// Sets the sort direction attached to the specified list view.
    /// </summary>
    /// <param name="listView">The list view.</param>
    /// <param name="value">The sort direction to apply.</param>
    public static void SetSortDirection(ListView listView, ColumnSortDirection value)
    {
      listView.SetValue(SortDirectionProperty, value);
    }

    private static void OnSortDirectionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      var grid = sender as ListView;
      if (grid is not null)
      {
        SortableGridViewColumn col = GetCurrentSortColumn(grid);
        if (col is not null)
          col.SortDirection = (ColumnSortDirection)args.NewValue;
        Sort(grid);
      }
    }

    #endregion SortDirection property

    #region CurrentSortColumn property

    /// <summary>
    /// Identifies the current sort column attached dependency property.
    /// </summary>
    public static readonly DependencyProperty CurrentSortColumnProperty =
      DependencyProperty.RegisterAttached("CurrentSortColumn", typeof(SortableGridViewColumn), typeof(SortableGridViewColumn),
                                          new FrameworkPropertyMetadata(OnCurrentSortColumnChanged));

    /// <summary>
    /// Gets the current sort column for the specified list view.
    /// </summary>
    /// <param name="listView">The list view.</param>
    /// <returns>The current sort column.</returns>
    public static SortableGridViewColumn GetCurrentSortColumn(ListView listView)
    {
      return (SortableGridViewColumn)listView.GetValue(CurrentSortColumnProperty);
    }

    /// <summary>
    /// Sets the current sort column for the specified list view.
    /// </summary>
    /// <param name="listView">The list view.</param>
    /// <param name="value">The current sort column.</param>
    public static void SetCurrentSortColumn(ListView listView, SortableGridViewColumn value)
    {
      listView.SetValue(CurrentSortColumnProperty, value);
    }

    private static void OnCurrentSortColumnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      var grid = sender as ListView;
      if (grid is not null)
      {
        var oldColumn = (SortableGridViewColumn)args.OldValue;
        if (oldColumn is not null)
          oldColumn.SortDirection = ColumnSortDirection.None;
        var newColumn = (SortableGridViewColumn)args.NewValue;
        if (newColumn is not null)
        {
          newColumn.SortDirection = GetSortDirection(grid);
        }
        Sort(grid);
      }
    }

    #endregion CurrentSortColumn property

    #region SortMode property

    /// <summary>
    /// Identifies the sort mode attached dependency property.
    /// </summary>
    public static readonly DependencyProperty SortModeProperty =
      DependencyProperty.RegisterAttached("SortMode", typeof(ListViewSortMode), typeof(SortableGridViewColumn),
                                          new FrameworkPropertyMetadata(ListViewSortMode.None, OnSortModeChanged));

    /// <summary>
    /// Gets the sort mode attached to the specified list view.
    /// </summary>
    /// <param name="listView">The list view.</param>
    /// <returns>The current sort mode.</returns>
    public static ListViewSortMode GetSortMode(ListView listView)
    {
      return (ListViewSortMode)listView.GetValue(SortModeProperty);
    }

    /// <summary>
    /// Sets the sort mode attached to the specified list view.
    /// </summary>
    /// <param name="listView">The list view.</param>
    /// <param name="value">The sort mode to apply.</param>
    public static void SetSortMode(ListView listView, ListViewSortMode value)
    {
      listView.SetValue(SortModeProperty, value);
    }

    private static void OnSortModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      var grid = sender as ListView;
      if (grid is not null)
      {
        if ((ListViewSortMode)args.NewValue != ListViewSortMode.None)
          grid.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(GridViewColumnHeaderClickHandler));
        else
          grid.RemoveHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(GridViewColumnHeaderClickHandler));
      }
    }

    private static void GridViewColumnHeaderClickHandler(object sender, RoutedEventArgs e)
    {
      var grid = sender as ListView;
      var headerClicked = e.OriginalSource as GridViewColumnHeader;
      if (grid is not null && headerClicked is not null && headerClicked.Role != GridViewColumnHeaderRole.Padding)
      {
        if (headerClicked.Column == GetCurrentSortColumn(grid))
        {
          if (GetSortDirection(grid) == ColumnSortDirection.Ascending)
            SetSortDirection(grid, ColumnSortDirection.Descending);
          else
            SetSortDirection(grid, ColumnSortDirection.Ascending);
        }
        else
        {
          SetSortDirection(grid, ColumnSortDirection.Ascending);
          SetCurrentSortColumn(grid, headerClicked.Column as SortableGridViewColumn);
        }
      }
    }

    #endregion SortMode property

    private static void Sort(ListView grid)
    {
      ColumnSortDirection currentDirection = GetSortDirection(grid);
      SortableGridViewColumn column = GetCurrentSortColumn(grid);
      if (column is not null && GetSortMode(grid) == ListViewSortMode.Automatic && currentDirection != ColumnSortDirection.None)
      {
        ICollectionView dataView = CollectionViewSource.GetDefaultView(grid.ItemsSource);

        string sortBy = column.SortBy;
        if (sortBy is null)
        {
          var binding = column.DisplayMemberBinding as Binding;
          if (binding is not null && binding.Path is not null)
          {
            sortBy = binding.Path.Path;
          }
        }

        dataView.SortDescriptions.Clear();
        if (sortBy is not null)
        {
          ListSortDirection direction;
          if (currentDirection == ColumnSortDirection.Descending)
            direction = ListSortDirection.Descending;
          else
            direction = ListSortDirection.Ascending;
          dataView.SortDescriptions.Add(new SortDescription(sortBy, direction));
        }
        dataView.Refresh();
      }
    }
  }

  /// <summary>
  /// Specifies the direction of a column sort operation.
  /// </summary>
  public enum ColumnSortDirection
  {
    /// <summary>
    /// No sorting is applied.
    /// </summary>
    None,
    /// <summary>
    /// Sort in ascending order.
    /// </summary>
    Ascending,
    /// <summary>
    /// Sort in descending order.
    /// </summary>
    Descending
  }

  /// <summary>
  /// Specifies how sorting is handled for a list view.
  /// </summary>
  public enum ListViewSortMode
  {
    /// <summary>
    /// Disable automatic sorting when sortable columns are clicked.
    /// </summary>
    None,

    /// <summary>
    /// Fully automatic sorting.
    /// </summary>
    Automatic,

    /// <summary>
    /// Automatically update SortDirection and CurrentSortColumn properties,
    /// but do not actually sort the data.
    /// </summary>
    HalfAutomatic
  }

  /// <summary>
  /// Converts a column sort direction match to a visibility value.
  /// </summary>
  internal sealed class ColumnSortDirectionToVisibilityConverter : IValueConverter
  {
    /// <summary>
    /// Converts a sort direction to a visibility value.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="parameter">The comparison parameter.</param>
    /// <param name="culture">The culture to use.</param>
    /// <returns><see cref="Visibility.Visible"/> when the values match; otherwise, <see cref="Visibility.Collapsed"/>.</returns>
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return Equals(value, parameter) ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>
    /// Not supported.
    /// </summary>
    /// <param name="value">The value to convert back.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="parameter">The comparison parameter.</param>
    /// <param name="culture">The culture to use.</param>
    /// <returns>Never returns a value.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
}

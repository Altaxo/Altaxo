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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Altaxo.Collections;

namespace Altaxo.Gui.Common
{

  /// <summary>
  /// New data context aware single choice radio stack panel.
  /// </summary>
  /// <seealso cref="System.Windows.Controls.ItemsControl" />
  /// <summary>
  /// Data-context-aware items control that displays a single selection as radio buttons in a stack panel.
  /// </summary>
  public class SingleChoiceRadioStackPanelDC : ItemsControl
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SingleChoiceRadioStackPanelDC"/> class.
    /// </summary>
    public SingleChoiceRadioStackPanelDC()
    {
      // set the ItemsPanelTemplate to a StackPanel (default: vertical orientation)
      this.ItemsPanel = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(StackPanel)));
    }


    /// <summary>
    /// Identifies the <see cref="Orientation"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(System.Windows.Controls.Orientation),
        typeof(SingleChoiceRadioStackPanelDC),
        new PropertyMetadata(Orientation.Vertical, EhOrientationChanged));



    /// <summary>
    /// Gets or sets the arrangement direction of the generated radio buttons.
    /// </summary>
    public System.Windows.Controls.Orientation Orientation
    {
      get
      {
        return (System.Windows.Controls.Orientation)GetValue(OrientationProperty);
      }
      set
      {
        SetValue(OrientationProperty, value);
      }
    }

    private static void EhOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is SingleChoiceRadioStackPanelDC thiss && e.NewValue is System.Windows.Controls.Orientation newOrientation)
      {
        thiss.ItemsPanel = newOrientation == Orientation.Horizontal ? GetItemsPanelTemplateHorizontal() : GetItemsPanelTemplateVertical();
      }
    }

    /// <summary>
    /// Identifies the <see cref="SelectedItem"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
        nameof(SelectedItem),
        typeof(object),
        typeof(SingleChoiceRadioStackPanelDC),
        new FrameworkPropertyMetadata(null, EhSelectedItemChanged) { BindsTwoWayByDefault = true });

    /// <summary>
    /// Gets or sets the selected item.
    /// </summary>
    public object SelectedItem
    {
      get
      {
        return GetValue(SelectedItemProperty);
      }
      set
      {
        SetValue(SelectedItemProperty, value);
      }
    }

    private static void EhSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (d is SingleChoiceRadioStackPanelDC thiss &&
          e.NewValue is { } newItem &&
          thiss.ItemContainerGenerator.ContainerFromItem(newItem) is ToggleButton rb)
      {
        rb.IsChecked = true;
      }
    }

    /// <inheritdoc/>
    protected override DependencyObject GetContainerForItemOverride()
    {
      var rb = new RadioButton();
      rb.SetBinding(ContentControl.ContentProperty, new Binding { Path = new PropertyPath("Text") });
      rb.SetBinding(ToolTipProperty, new Binding { Path = new PropertyPath("Text0") });
      rb.SetBinding(RadioButton.IsCheckedProperty, new Binding { Path = new PropertyPath("IsSelected"), Mode = BindingMode.TwoWay });
      rb.Checked += EhButtonChecked;
      rb.Margin = Orientation == Orientation.Horizontal ? new Thickness(3, 0, 3, 0) : new Thickness(0, 3, 0, 3);

      return rb;
    }

    private void EhButtonChecked(object sender, RoutedEventArgs e)
    {
      if (ItemsSource is { } itemsSource)
      {
        foreach (var item in itemsSource)
        {
          var itContainer = this.ItemContainerGenerator.ContainerFromItem(item);
          if (!object.ReferenceEquals(sender, itContainer))
          {
            ((RadioButton)itContainer).IsChecked = false;
          }
          else
          {
            SelectedItem = item;
          }
        }
      }
    }

    private static ItemsPanelTemplate GetItemsPanelTemplateHorizontal()
    {
      string xaml = @"<ItemsPanelTemplate   xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                            <StackPanel Orientation='Horizontal' />
                            </ItemsPanelTemplate>";
      return System.Windows.Markup.XamlReader.Parse(xaml) as ItemsPanelTemplate;
    }
    private static ItemsPanelTemplate GetItemsPanelTemplateVertical()
    {
      return new ItemsPanelTemplate(new FrameworkElementFactory(typeof(StackPanel)));
    }




    /// <inheritdoc/>
    protected override void ClearContainerForItemOverride(DependencyObject element, object item)
    {
      if (element is ToggleButton rb)
      {
        rb.Checked -= EhButtonChecked;
      }

      base.ClearContainerForItemOverride(element, item);
    }

    /// <inheritdoc/>
    protected override bool IsItemItsOwnContainerOverride(object item)
    {
      return item is RadioButton;
    }

  }



  
  /// <summary>
  /// Stack panel that displays a single selectable list as radio buttons.
  /// </summary>
  public class SingleChoiceRadioStackPanel : StackPanel
  {
    /// <summary>
    /// Occurs when the selected item changes.
    /// </summary>
    public event EventHandler? SelectionChanged;

    #region Dependency properties

    /// <summary>
    /// Identifies the <see cref="ItemsSource"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty =
    DependencyProperty.Register(
      nameof(ItemsSource),
      typeof(SelectableListNodeList),
      typeof(SingleChoiceRadioStackPanel),
      new FrameworkPropertyMetadata(EhItemsSourceChanged));

    /// <summary>
    /// Gets or sets the selectable items.
    /// </summary>
    public SelectableListNodeList? ItemsSource
    {
      get { return (SelectableListNodeList)GetValue(ItemsSourceProperty); }
      set { SetValue(ItemsSourceProperty, value); }
    }

    private static void EhItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((SingleChoiceRadioStackPanel)obj).OnItemsSourceChanged(obj, args);
    }

    /// <summary>
    /// Updates the control after <see cref="ItemsSource"/> changes.
    /// </summary>
    /// <param name="obj">The dependency object whose property changed.</param>
    /// <param name="args">The property change arguments.</param>
    protected void OnItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      if (args.NewValue is SelectableListNodeList newItemsSource)
      {
        Initialize(newItemsSource);
        SelectedItem = newItemsSource.FirstSelectedNode;
      }
      else
      {
        SelectedItem = null;
      }
    }

    /// <summary>
    /// Identifies the <see cref="SelectedItem"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SelectedItemProperty =
      DependencyProperty.Register(
        nameof(SelectedItem),
        typeof(SelectableListNode),
        typeof(SingleChoiceRadioStackPanel),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, EhSelectedItemChanged));

    /// <summary>
    /// Gets or sets the selected item.
    /// </summary>
    public SelectableListNode? SelectedItem
    {
      get { return (SelectableListNode)GetValue(SelectedItemProperty); }
      set { SetValue(SelectedItemProperty, value); }
    }

    private static void EhSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((SingleChoiceRadioStackPanel)obj).OnSelectedItemChanged(obj, args);
    }

    /// <summary>
    /// Raises <see cref="SelectionChanged"/> after <see cref="SelectedItem"/> changes.
    /// </summary>
    /// <param name="obj">The dependency object whose property changed.</param>
    /// <param name="args">The property change arguments.</param>
    protected void OnSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    private SelectableListNodeList _choices;

    /// <summary>
    /// Initializes the control with the specified choices.
    /// </summary>
    /// <param name="choices">The selectable items to display.</param>
    public void Initialize(SelectableListNodeList choices)
    {
      _choices = choices;
      if (!object.ReferenceEquals(_choices, ItemsSource))
      {
        ItemsSource = _choices;
      }

      Children.Clear();
      if (_choices is not null)
      {
        foreach (var choice in _choices)
        {
          var rb = new RadioButton
          {
            Content = choice.Text,
            ToolTip = choice.Text0,
            Tag = choice,
            IsChecked = choice.IsSelected,
            Margin = Orientation == Orientation.Horizontal ? new Thickness(3, 0, 3, 0) : new Thickness(0, 3, 0, 3),
          };
          rb.Checked += EhRadioButtonChecked;

          Children.Add(rb);
        }
      }
    }

    private void EhRadioButtonChecked(object sender, RoutedEventArgs e)
    {
      var rb = (RadioButton)sender;
      if (rb.Tag is SelectableListNode node)
      {
        _choices.ClearSelectionsAll();
        node.IsSelected = true == rb.IsChecked;

        if (node.IsSelected)
        {
          SelectedItem = node;
        }
      }
    }
  }
  
}

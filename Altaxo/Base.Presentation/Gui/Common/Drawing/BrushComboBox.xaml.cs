﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2012 Dr. Dirk Lellinger
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
//    along with ctrl program; if not, write to the Free Software
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
using System.Windows.Documents;
using System.Windows.Input;
using Altaxo.Drawing;
using Altaxo.Drawing.ColorManagement;
using Altaxo.Graph;

namespace Altaxo.Gui.Common.Drawing
{
  /// <summary>
  /// Interaction logic for ColorComboBoxEx.xaml
  /// </summary>
  public partial class BrushComboBox : ColorComboBoxBase
  {
    public static readonly DependencyProperty SelectedBrushProperty;

    public static readonly DependencyProperty CustomPenCommandProperty;

    public event DependencyPropertyChangedEventHandler? SelectedBrushChanged;

    private List<BrushX> _lastLocalUsedItems = new List<BrushX>();

    #region Constructors

    static BrushComboBox()
    {
      SelectedBrushProperty = DependencyProperty.Register(
        nameof(SelectedBrush),
        typeof(BrushX),
        typeof(BrushComboBox),
        new FrameworkPropertyMetadata(new BrushX(NamedColors.Black), EhSelectedBrushChanged, EhSelectedBrushCoerce) { BindsTwoWayByDefault=true});

      CustomPenCommandProperty = DependencyProperty.RegisterAttached(
    nameof(CustomPenCommand),
    typeof(ICommand),
    typeof(BrushComboBox),
    new FrameworkPropertyMetadata(OnCustomPenCommandChanged)
    );
  }

    public BrushComboBox()
    {
      UpdateTreeViewTreeNodes();

      InitializeComponent();

      UpdateComboBoxSourceSelection(InternalSelectedBrush);
      UpdateTreeViewSelection();
    }

    #endregion Constructors

    #region Implementation of abstract base class members

    protected override TreeView GuiTreeView { get { return _treeView; } }

    protected override ComboBox GuiComboBox { get { return _guiComboBox; } }

    protected override NamedColor InternalSelectedColor
    {
      get
      {
        return InternalSelectedBrush.Color;
      }
      set
      {
        var selBrush = InternalSelectedBrush;
        if (selBrush is not null)
        {
          selBrush = selBrush.WithColor(value);
          InternalSelectedBrush = selBrush;
        }
      }
    }

    #endregion Implementation of abstract base class members

    #region Dependency property

    /// <summary>
    /// Gets/sets the selected brush. Since <see cref="BrushX"/> is not immutable, the Brush is cloned when setting the property, as well as when getting the property.
    /// </summary>
    /// <remarks>
    /// <para>Reasons to clone the brush at setting/getting:</para>
    /// <para>
    /// Scenario 1: the SelectedBrush property is set without cloning, then an external function changes the brush color: the BrushComboBox will not show the new color, since it don't know anything about the changed color.
    /// </para>
    /// <para>
    /// The user selects a brush in this BrushComboBox, the value is used by an external function, which changes the color. Here also, the new color is not shown in the BrushComboBox.
    /// </para>
    /// </remarks>
    public BrushX SelectedBrush
    {
      get
      {
        return (BrushX)GetValue(SelectedBrushProperty); // use only a copy - don't give the original selected brush away from this combobox, it might be changed externally
      }
      set
      {
        SetValue(SelectedBrushProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the selected brush. Here, the getting/setting is done without cloning the brush before. Thus, it must be absolutely ensured, that the brush's properties are not changed.
    /// When some properties must be changed, it is absolutely neccessary to clone the brush <b>before</b>, then make the changes at the cloned brush, and then using the cloned brush for setting this property.
    /// </summary>
    /// <value>
    /// The selected brush.
    /// </value>
    protected BrushX InternalSelectedBrush
    {
      get
      {
        return ((BrushX)GetValue(SelectedBrushProperty));
      }
      set
      {
        SetValue(SelectedBrushProperty, value);
      }
    }

    private static object EhSelectedBrushCoerce(DependencyObject obj, object coerceValue)
    {
      var thiss = (BrushComboBox)obj;
      return thiss.InternalSelectedBrushCoerce(obj, (BrushX)coerceValue);
    }

    protected virtual BrushX InternalSelectedBrushCoerce(DependencyObject obj, BrushX brush)
    {
      if (brush is null)
        brush = new BrushX(NamedColors.Transparent);

      var coercedColor = brush.Color.CoerceParentColorSetToNullIfNotMember();
      if (!brush.Color.Equals(coercedColor))
      {
        brush = brush.WithColor(coercedColor);
      }

      if (ShowPlotColorsOnly && (brush.Color.ParentColorSet is null || !ColorSetManager.Instance.IsPlotColorSet(brush.Color.ParentColorSet)))
      {
        brush = brush.WithColor(ColorSetManager.Instance.BuiltinDarkPlotColors[0]);
      }
      return brush;
    }

    private static void EhSelectedBrushChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((BrushComboBox)obj).OnSelectedBrushChanged(obj, args);
    }

    protected virtual void OnSelectedBrushChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      var oldBrush = (BrushX)args.OldValue;
      var newBrush = (BrushX)args.NewValue;

      var oldColor = oldBrush.Color;
      var newColor = newBrush.Color;

      if (newBrush.BrushType != BrushType.SolidBrush || newBrush.Color.ParentColorSet is null)
      {
        StoreAsLastUsedItem(_lastLocalUsedItems, newBrush);
      }

      if (!newBrush.Equals(_guiComboBox.SelectedValue))
        UpdateComboBoxSourceSelection(newBrush);

      if (!object.ReferenceEquals(oldColor.ParentColorSet, newColor.ParentColorSet) && !object.ReferenceEquals(newColor.ParentColorSet, _treeView.SelectedValue))
        UpdateTreeViewSelection();

      if (SelectedBrushChanged is not null)
        SelectedBrushChanged(obj, args);
    }

    public ICommand CustomPenCommand
    {
      get
      {
        return ((ICommand)GetValue(CustomPenCommandProperty));
      }
      set
      {
        SetValue(CustomPenCommandProperty, value);
      }
    }

    protected static void OnCustomPenCommandChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      var thiss = (BrushComboBox)obj;
      thiss._guiMenuShowCustomPen.Visibility = args.NewValue is null ? Visibility.Collapsed : Visibility.Visible;
    }

    private void EhShowCustomPenDialog(object sender, RoutedEventArgs e)
    {
      if (CustomPenCommand is not null && CustomPenCommand.CanExecute(SelectedBrush))
      {
        CustomPenCommand.Execute(SelectedBrush);
      }
    }

    #endregion Dependency property

    #region ComboBox

    #region ComboBox data handling

    private void UpdateComboBoxSourceSelection(BrushX brush)
    {
      if (brush.Equals(_guiComboBox.SelectedValue))
        return;

      _filterString = string.Empty;
      FillComboBoxWithFilteredItems(_filterString, false);
      _guiComboBox.SelectedValue = brush;
    }

    private List<object> _comboBoxSeparator1 = new List<object> { new Separator() { Name = "ThisIsASeparatorForTheComboBox", Tag = "Last used brushes" } };
    private List<object> _comboBoxSeparator2 = new List<object> { new Separator() { Name = "ThisIsASeparatorForTheComboBox", Tag = "Color set" } };

    protected override bool FillComboBoxWithFilteredItems(string filterString, bool onlyIfItemsRemaining)
    {
      List<object> lastUsed;

      lastUsed = GetFilteredList(_lastLocalUsedItems, filterString, ShowPlotColorsOnly);

      var colorSet = GetColorSetForComboBox();
      var known = GetFilteredList(colorSet, filterString);

      if ((lastUsed.Count + known.Count) > 0 || !onlyIfItemsRemaining)
      {
        IEnumerable<object> source = null;

        if (lastUsed.Count > 0)
        {
          source = _comboBoxSeparator1.Concat(lastUsed);
        }
        if (known.Count > 0)
        {
          (_comboBoxSeparator2[0] as Separator).Tag = colorSet.Name;
          if (source is null)
            source = _comboBoxSeparator2.Concat(known);
          else
            source = source.Concat(_comboBoxSeparator2).Concat(known);
        }
        _guiComboBox.ItemsSource = source;
        return true;
      }

      return false;
    }

    protected static List<object> GetFilteredList(IReadOnlyList<NamedColor> originalList, string filterString)
    {
      var result = new List<object>();
      filterString = filterString.ToLowerInvariant();
      foreach (var item in originalList)
      {
        if (item.Name.ToLowerInvariant().StartsWith(filterString))
          result.Add(new BrushX(item));
      }
      return result;
    }

    protected static List<object> GetFilteredList(IList<BrushX> originalList, string filterString, bool showPlotColorsOnly)
    {
      var result = new List<object>();
      filterString = filterString.ToLowerInvariant();

      foreach (var item in originalList)
      {
        if (showPlotColorsOnly && (item.Color.ParentColorSet is null || !ColorSetManager.Instance.IsPlotColorSet(item.Color.ParentColorSet)))
          continue;

        if (item.Color.Name.ToLowerInvariant().StartsWith(filterString))
          result.Add(item);
      }
      return result;
    }

    #endregion ComboBox data handling

    #region ComboBox event handling

    private void EhPopupClosed(object sender, EventArgs e)
    {
    }

    private void EhComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
    }

    protected void EhComboBox_DropDownClosed(object sender, EventArgs e)
    {
      if (_filterString.Length > 0)
      {
        var selItem = _guiComboBox.SelectedValue;
        _filterString = string.Empty;
        FillComboBoxWithFilteredItems(_filterString, false);
        _guiComboBox.SelectedValue = selItem;
      }

      if (_guiComboBox.SelectedValue is null)
      {
        _guiComboBox.SelectedValue = InternalSelectedBrush;
      }
      else
      {
        if (_guiComboBox.SelectedValue is BrushX)
          InternalSelectedBrush = (BrushX)_guiComboBox.SelectedValue;
        else
          InternalSelectedBrush = new BrushX((NamedColor)_guiComboBox.SelectedValue);
      }
    }

    #endregion ComboBox event handling

    #endregion ComboBox

    #region Context menus

    private void EhShowCustomBrushDialog(object sender, RoutedEventArgs e)
    {
      var localBrush = InternalSelectedBrush; // under no circumstances change the selected brush, since it may come from an unknown source
      var ctrl = new BrushControllerAdvanced
      {
        ShowPlotColorsOnly = ShowPlotColorsOnly
      };
      ctrl.InitializeDocument(localBrush);
      if (Current.Gui.ShowDialog(ctrl, "Edit brush properties", false))
        InternalSelectedBrush = (BrushX)ctrl.ModelObject;
    }

    protected void EhShowCustomColorDialog(object sender, RoutedEventArgs e)
    {
      if (base.InternalShowCustomColorDialog(sender, out var newColor))
      {
        InternalSelectedBrush = InternalSelectedBrush.WithColor(newColor);
      }
    }

    protected void EhChooseOpacityFromContextMenu(object sender, RoutedEventArgs e)
    {
      if (base.InternalChooseOpacityFromContextMenu(sender, out var newColor))
      {
        InternalSelectedBrush = InternalSelectedBrush.WithColor(newColor);
      }
    }

    #endregion Context menus

   
  }
}

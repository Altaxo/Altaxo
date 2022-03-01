#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using Altaxo.Drawing.ColorManagement;

namespace Altaxo.Gui.Drawing.D3D
{
  using Altaxo.Drawing;
  using Altaxo.Drawing.D3D;
  using Altaxo.Drawing.D3D.Material;
  using Altaxo.Gui.Common.Drawing;

  /// <summary>
  /// Interaction logic for ColorComboBoxEx.xaml
  /// </summary>
  public partial class MaterialComboBox : ColorComboBoxBase, Altaxo.Gui.Graph.Graph3D.Material.IMaterialViewSimple
  {
    public static readonly DependencyProperty SelectedMaterialProperty;

    public static readonly DependencyProperty IsNoMaterialAllowedProperty;

    public event DependencyPropertyChangedEventHandler? SelectedMaterialChanged;

    private List<IMaterial> _lastLocalUsedItems = new List<IMaterial>();

    /// <summary>
    /// Is used as a template for the items of this combobox. The items will use the same specular properties as in this template, but with a different color.
    /// </summary>
    private MaterialWithUniformColor _solidMaterialTemplate = new MaterialWithUniformColor(NamedColors.Black);

    #region Constructors

    static MaterialComboBox()
    {
      SelectedMaterialProperty = DependencyProperty.Register(
        "SelectedMaterial",
        typeof(IMaterial),
        typeof(MaterialComboBox),
        new FrameworkPropertyMetadata(Materials.GetSolidMaterial(NamedColors.Black), EhSelectedMaterialChanged, EhSelectedMaterialCoerce) { BindsTwoWayByDefault=true});

      IsNoMaterialAllowedProperty = DependencyProperty.Register(
        nameof(IsNoMaterialAllowed),
        typeof(bool),
        typeof(MaterialComboBox),
        new FrameworkPropertyMetadata((false), EhIsNoMaterialAllowedChanged));
    }

    public MaterialComboBox()
    {
      UpdateTreeViewTreeNodes();

      InitializeComponent();

      UpdateComboBoxSourceSelection(InternalSelectedMaterial);

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
        return InternalSelectedMaterial.Color;
      }
      set
      {
        IMaterial selMaterial = _solidMaterialTemplate ?? new MaterialWithUniformColor(NamedColors.Black);
        selMaterial = selMaterial.WithColor(value);
        InternalSelectedMaterial = selMaterial;
      }
    }

    #endregion Implementation of abstract base class members

    #region IsNoMaterialAllowed

    public bool IsNoMaterialAllowed
    {
      get
      {
        return (bool)GetValue(IsNoMaterialAllowedProperty);
      }
      set
      {
        SetValue(IsNoMaterialAllowedProperty, value);
      }
    }

    private static void EhIsNoMaterialAllowedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((MaterialComboBox)obj).OnIsNoMaterialAllowedChanged(obj, args);
    }

    protected virtual void OnIsNoMaterialAllowedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      UpdateComboBoxSourceSelection(SelectedMaterial);
    }

    #endregion IsNoMaterialAllowed

    #region Dependency property

    /// <summary>
    /// Gets/sets the selected material.
    /// </summary>
    public IMaterial SelectedMaterial
    {
      get
      {
        var result = (IMaterial)GetValue(SelectedMaterialProperty); // Material is immutable, no need for cloning
        if (IsNoMaterialAllowed && !result.IsVisible)
          result = null;

        return result;
      }
      set
      {
        SetValue(SelectedMaterialProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the selected brush. Here, the getting/setting is done without cloning the brush before. Thus, it must be absolutely ensured, that the brush's properties are not changed.
    /// When some properties must be changed, it is absolutely neccessary to clone the brush <b>before</b>, then make the changes at the cloned brush, and then using the cloned brush for setting this property.
    /// </summary>
    /// <value>
    /// The selected brush.
    /// </value>
    protected IMaterial InternalSelectedMaterial
    {
      get
      {
        return (IMaterial)GetValue(SelectedMaterialProperty);
      }
      set
      {
        SetValue(SelectedMaterialProperty, value);
      }
    }

    private static object EhSelectedMaterialCoerce(DependencyObject obj, object coerceValue)
    {
      var thiss = (MaterialComboBox)obj;
      return thiss.InternalSelectedMaterialCoerce(obj, (IMaterial)coerceValue);
    }

    protected virtual IMaterial InternalSelectedMaterialCoerce(DependencyObject obj, IMaterial material)
    {
      if (material is null)
        material = MaterialInvisible.Instance;

      var coercedColor = material.Color.CoerceParentColorSetToNullIfNotMember();
      if (!material.Color.Equals(coercedColor))
      {
        material = Materials.GetMaterialWithNewColor(material, coercedColor);
      }

      if (ShowPlotColorsOnly && (material.Color.ParentColorSet is null || false == ColorSetManager.Instance.IsPlotColorSet(material.Color.ParentColorSet)))
      {
        material = Materials.GetMaterialWithNewColor(material, ColorSetManager.Instance.BuiltinDarkPlotColors[0]);
      }
      return material;
    }

    private static void EhSelectedMaterialChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((MaterialComboBox)obj).OnSelectedMaterialChanged(obj, args);
    }

    protected virtual void OnSelectedMaterialChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      var oldMaterial = (IMaterial)args.OldValue;
      var newMaterial = (IMaterial)args.NewValue;

      var oldColor = oldMaterial.Color;
      var newColor = newMaterial.Color;

      if (newMaterial.Color.ParentColorSet is null)
      {
        StoreAsLastUsedItem(_lastLocalUsedItems, newMaterial);
      }
      else if (!newMaterial.HasSameSpecularPropertiesAs(_solidMaterialTemplate) && newMaterial.IsVisible)
      {
        // StoreAsLastUsedItem(_lastLocalUsedItems, newMaterial);
        _solidMaterialTemplate = (MaterialWithUniformColor)_solidMaterialTemplate.WithSpecularPropertiesAs(newMaterial);
      }

      if (!newMaterial.Equals(_guiComboBox.SelectedValue))
        UpdateComboBoxSourceSelection(newMaterial);

      if (!object.ReferenceEquals(oldColor.ParentColorSet, newColor.ParentColorSet) && !object.ReferenceEquals(newColor.ParentColorSet, _treeView.SelectedValue))
        UpdateTreeViewSelection();

      if (SelectedMaterialChanged is not null)
        SelectedMaterialChanged(obj, args);
    }

    #endregion Dependency property

    #region ComboBox

    #region ComboBox data handling

    private void UpdateComboBoxSourceSelection(IMaterial brush)
    {
      if (brush.Equals(_guiComboBox.SelectedValue))
        return;

      _filterString = string.Empty;
      FillComboBoxWithFilteredItems(_filterString, false);
      _guiComboBox.SelectedValue = brush;
    }

    private List<object> _comboBoxSeparator1 = new List<object> { new Separator() { Name = "ThisIsASeparatorForTheComboBox", Tag = "Last used materials" } };
    private List<object> _comboBoxSeparator2 = new List<object> { new Separator() { Name = "ThisIsASeparatorForTheComboBox", Tag = "Color set" } };

    protected override bool FillComboBoxWithFilteredItems(string filterString, bool onlyIfItemsRemaining)
    {
      List<object> lastUsed;

      lastUsed = GetFilteredList(_lastLocalUsedItems, filterString, ShowPlotColorsOnly);

      var colorSet = GetColorSetForComboBox();
      var known = GetFilteredList(colorSet, _solidMaterialTemplate, filterString);

      if (IsNoMaterialAllowed)
        known.Add(MaterialInvisible.Instance);

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

    protected static List<object> GetFilteredList(IReadOnlyList<NamedColor> originalList, MaterialWithUniformColor materialTemplate, string filterString)
    {
      var result = new List<object>();

      filterString = filterString.ToLowerInvariant();
      foreach (var namedColor in originalList)
      {
        if (namedColor.Name.ToLowerInvariant().StartsWith(filterString))
          result.Add(materialTemplate.WithColor(namedColor));
      }
      return result;
    }

    protected static List<object> GetFilteredList(IReadOnlyList<IMaterial> originalList, string filterString, bool showPlotColorsOnly)
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
        _guiComboBox.SelectedValue = InternalSelectedMaterial;
      }
      else
      {
        if (_guiComboBox.SelectedValue is IMaterial)
          InternalSelectedMaterial = (IMaterial)_guiComboBox.SelectedValue;
        else
          InternalSelectedMaterial = Materials.GetSolidMaterial((NamedColor)_guiComboBox.SelectedValue);
      }
    }

    #endregion ComboBox event handling

    #endregion ComboBox

    #region Context menus

    private void EhShowCustomMaterialDialog(object sender, RoutedEventArgs e)
    {
      var localMaterial = SelectedMaterial;

      var ctrl = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { localMaterial }, typeof(IMVCANController), UseDocument.Copy);
      //ctrl.RestrictBrushColorToPlotColorsOnly = ShowPlotColorsOnly;
      //ctrl.InitializeDocument(localMaterial);
      if (Current.Gui.ShowDialog(ctrl, "Edit brush properties", false))
      {
        SelectedMaterial = (IMaterial)ctrl.ModelObject;
      }
    }

    protected void EhShowCustomColorDialog(object sender, RoutedEventArgs e)
    {
      if (base.InternalShowCustomColorDialog(sender, out var newColor))
      {
        var newMat = Materials.GetMaterialWithNewColor(InternalSelectedMaterial, newColor);
        InternalSelectedMaterial = newMat;
      }
    }

    protected void EhChooseOpacityFromContextMenu(object sender, RoutedEventArgs e)
    {
      if (base.InternalChooseOpacityFromContextMenu(sender, out var newColor))
      {
        var newMat = Materials.GetMaterialWithNewColor(InternalSelectedMaterial, newColor);
        InternalSelectedMaterial = newMat;
      }
    }

    private void EhChooseSpecularPropertiesFromContextMenu(object sender, RoutedEventArgs e)
    {
      var sfsender = (FrameworkElement)sender;
      string tag = (string)sfsender.Tag;
      var tagparts = tag.Split(';');
      if (tagparts.Length != 3)
        return;

      double smoothness;
      double metalness;
      double indexOfRefraction;

      try
      {
        smoothness = double.Parse(tagparts[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
        metalness = double.Parse(tagparts[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
        indexOfRefraction = double.Parse(tagparts[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
        var newMaterial = SelectedMaterial.WithSpecularProperties(smoothness, metalness, indexOfRefraction);
        SelectedMaterial = newMaterial;
      }
      catch (Exception)
      {
      }
    }

    #endregion Context menus
  }
}

#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

using Altaxo.Drawing.ColorManagement;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Altaxo.Gui.Graph3D.Material
{
	using Altaxo.Graph.Graph3D;
	using Altaxo.Gui.Common.Drawing;
	using Drawing;
	using Drawing.D3D;

	/// <summary>
	/// Interaction logic for ColorComboBoxEx.xaml
	/// </summary>
	public partial class MaterialComboBox : ColorComboBoxBase, Altaxo.Gui.Graph3D.Material.IMaterialViewSimple
	{
		public static readonly DependencyProperty SelectedMaterialProperty;

		public event DependencyPropertyChangedEventHandler SelectedMaterialChanged;

		private List<IMaterial> _lastLocalUsedItems = new List<IMaterial>();

		#region Constructors

		static MaterialComboBox()
		{
			SelectedMaterialProperty = DependencyProperty.Register("SelectedMaterial", typeof(IMaterial), typeof(MaterialComboBox), new FrameworkPropertyMetadata(Materials.GetSolidMaterial(NamedColors.Black), EhSelectedMaterialChanged, EhSelectedMaterialCoerce));
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
				var selMaterial = InternalSelectedMaterial;
				if (null != selMaterial)
				{
					selMaterial = Materials.GetMaterialWithNewColor(selMaterial, value);
					InternalSelectedMaterial = selMaterial;
				}
			}
		}

		#endregion Implementation of abstract base class members

		#region Dependency property

		/// <summary>
		/// Gets/sets the selected brush. Since <see cref="IMaterial"/> is not immutable, the material is cloned when setting the property, as well as when getting the property.
		/// </summary>
		/// <remarks>
		/// <para>Reasons to clone the brush at setting/getting:</para>
		/// <para>
		/// Scenario 1: the SelectedMaterial property is set without cloning, then an external function changes the brush color: the MaterialComboBox will not show the new color, since it don't know anything about the changed color.
		/// </para>
		/// <para>
		/// The user selects a brush in this MaterialComboBox, the value is used by an external function, which changes the color. Here also, the new color is not shown in the MaterialComboBox.
		/// </para>
		/// </remarks>
		public IMaterial SelectedMaterial
		{
			get
			{
				return (IMaterial)GetValue(SelectedMaterialProperty); // Material is immutable, no need for cloning
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

		protected virtual IMaterial InternalSelectedMaterialCoerce(DependencyObject obj, IMaterial brush)
		{
			if (null == brush)
				brush = Materials.GetSolidMaterial(NamedColors.Transparent);

			var coercedColor = brush.Color.CoerceParentColorSetToNullIfNotMember();
			if (!brush.Color.Equals(coercedColor))
			{
				brush = Materials.GetMaterialWithNewColor(brush, coercedColor);
			}

			if (this.ShowPlotColorsOnly && (brush.Color.ParentColorSet == null || false == brush.Color.ParentColorSet.IsPlotColorSet))
			{
				brush = Materials.GetMaterialWithNewColor(brush, ColorSetManager.Instance.BuiltinDarkPlotColors[0]);
			}
			return brush;
		}

		private static void EhSelectedMaterialChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((MaterialComboBox)obj).OnSelectedMaterialChanged(obj, args);
		}

		protected virtual void OnSelectedMaterialChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var oldMat = (IMaterial)args.OldValue;
			var newMat = (IMaterial)args.NewValue;

			var oldColor = oldMat.Color;
			var newColor = newMat.Color;

			if (newMat.Color.ParentColorSet == null)
			{
				StoreAsLastUsedItem(_lastLocalUsedItems, newMat);
			}

			if (!newMat.Equals(_guiComboBox.SelectedValue))
				this.UpdateComboBoxSourceSelection(newMat);

			if (!object.ReferenceEquals(oldColor.ParentColorSet, newColor.ParentColorSet) && !object.ReferenceEquals(newColor.ParentColorSet, _treeView.SelectedValue))
				this.UpdateTreeViewSelection();

			if (null != SelectedMaterialChanged)
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
					if (source == null)
						source = _comboBoxSeparator2.Concat(known);
					else
						source = source.Concat(_comboBoxSeparator2).Concat(known);
				}
				_guiComboBox.ItemsSource = source;
				return true;
			}

			return false;
		}

		protected static List<object> GetFilteredList(IList<NamedColor> originalList, string filterString)
		{
			var result = new List<object>();
			filterString = filterString.ToLowerInvariant();
			foreach (var item in originalList)
			{
				if (item.Name.ToLowerInvariant().StartsWith(filterString))
					result.Add(Materials.GetSolidMaterial(item));
			}
			return result;
		}

		protected static List<object> GetFilteredList(IList<IMaterial> originalList, string filterString, bool showPlotColorsOnly)
		{
			var result = new List<object>();
			filterString = filterString.ToLowerInvariant();
			foreach (var item in originalList)
			{
				if (showPlotColorsOnly && (item.Color.ParentColorSet == null || !item.Color.ParentColorSet.IsPlotColorSet))
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

			if (_guiComboBox.SelectedValue == null)
			{
				_guiComboBox.SelectedValue = InternalSelectedMaterial;
			}
			else
			{
				if (_guiComboBox.SelectedValue is IMaterial)
					this.InternalSelectedMaterial = (IMaterial)_guiComboBox.SelectedValue;
				else
					this.InternalSelectedMaterial = Materials.GetSolidMaterial((NamedColor)_guiComboBox.SelectedValue);
			}
		}

		#endregion ComboBox event handling

		#endregion ComboBox

		#region Context menus

		private void EhShowCustomMaterialDialog(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
			/*
			var localMaterial = this.InternalSelectedMaterial.Clone(); // under no circumstances change the selected brush, since it may come from an unknown source
			var ctrl = new BrushControllerAdvanced();
			ctrl.RestrictBrushColorToPlotColorsOnly = ShowPlotColorsOnly;
			ctrl.InitializeDocument(localMaterial);
			if (Current.Gui.ShowDialog(ctrl, "Edit brush properties", false))
				this.InternalSelectedMaterial = (IMaterial3D)ctrl.ModelObject;
				*/
		}

		protected void EhShowCustomColorDialog(object sender, RoutedEventArgs e)
		{
			NamedColor newColor;
			if (base.InternalShowCustomColorDialog(sender, out newColor))
			{
				var newMat = Materials.GetMaterialWithNewColor((IMaterial)InternalSelectedMaterial, newColor);
				InternalSelectedMaterial = newMat;
			}
		}

		protected void EhChooseOpacityFromContextMenu(object sender, RoutedEventArgs e)
		{
			NamedColor newColor;
			if (base.InternalChooseOpacityFromContextMenu(sender, out newColor))
			{
				var newMat = Materials.GetMaterialWithNewColor((IMaterial)InternalSelectedMaterial, newColor);
				InternalSelectedMaterial = newMat;
			}
		}

		#endregion Context menus
	}
}
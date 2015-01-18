#region Copyright

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

using Altaxo.Collections;
using Altaxo.Graph;
using Altaxo.Graph.ColorManagement;
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

namespace Altaxo.Gui.Common.Drawing
{
	/// <summary>
	/// Interaction logic for ColorComboBoxEx.xaml
	/// </summary>
	public partial class ColorComboBox : ColorComboBoxBase, INamedColorView
	{
		private List<NamedColor> _lastLocalUsedItems = new List<NamedColor>();

		public event DependencyPropertyChangedEventHandler SelectedColorChanged;

		private event Action ViewEvent_SelectedColorChanged;

		event Action Altaxo.Gui.Common.Drawing.INamedColorView.SelectedColorChanged
		{
			add { ViewEvent_SelectedColorChanged += value; }
			remove { ViewEvent_SelectedColorChanged -= value; }
		}

		#region Constructors

		static ColorComboBox()
		{
			SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(NamedColor), typeof(ColorComboBox), new FrameworkPropertyMetadata(NamedColors.Black, EhSelectedColorChanged, EhSelectedColorCoerce));
		}

		public ColorComboBox()
		{
			UpdateTreeViewTreeNodes();

			InitializeComponent();

			UpdateComboBoxSourceSelection(SelectedColor);
			UpdateTreeViewSelection();
		}

		#endregion Constructors

		#region Implementation of abstract base class members

		protected override TreeView GuiTreeView { get { return _treeView; } }

		protected override ComboBox GuiComboBox { get { return _guiComboBox; } }

		protected override NamedColor InternalSelectedColor { get { return SelectedColor; } set { SelectedColor = value; } }

		#endregion Implementation of abstract base class members

		#region Dependency property

		public NamedColor SelectedColor
		{
			get { return (NamedColor)GetValue(SelectedColorProperty); }
			set { SetValue(SelectedColorProperty, value); }
		}

		public Color SelectedWpfColor
		{
			get { return GuiHelper.ToWpf(((NamedColor)GetValue(SelectedColorProperty)).Color); }
			set
			{
				AxoColor c = GuiHelper.ToAxo(value);
				SetValue(SelectedColorProperty, new NamedColor(c, NamedColor.GetColorName(c)));
			}
		}

		public System.Drawing.Color SelectedGdiColor
		{
			get { return GuiHelper.ToGdi(((NamedColor)GetValue(SelectedColorProperty)).Color); }
			set
			{
				AxoColor c = GuiHelper.ToAxo(value);
				SetValue(SelectedColorProperty, new NamedColor(c, NamedColor.GetColorName(c)));
			}
		}

		public static readonly DependencyProperty SelectedColorProperty;

		private static void EhSelectedColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((ColorComboBox)obj).OnSelectedColorChanged(obj, args);
		}

		private static object EhSelectedColorCoerce(DependencyObject obj, object coerceValue)
		{
			var thiss = (ColorComboBox)obj;
			return thiss.InternalSelectedColorCoerce((NamedColor)coerceValue);
		}

		protected virtual void OnSelectedColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var oldColor = (NamedColor)args.OldValue;
			var newColor = (NamedColor)args.NewValue;

			// make sure, that the item is part of the data items of the ComboBox
			if (newColor.ParentColorSet == null)
			{
				StoreAsLastUsedItem(_lastLocalUsedItems, newColor);
			}

			if (!newColor.Equals(_guiComboBox.SelectedValue))
				this.UpdateComboBoxSourceSelection(newColor);

			if (!object.ReferenceEquals(oldColor.ParentColorSet, newColor.ParentColorSet) && !object.ReferenceEquals(newColor.ParentColorSet, _treeView.SelectedValue))
				this.UpdateTreeViewSelection();

			if (null != SelectedColorChanged)
				SelectedColorChanged(obj, args);
			if (null != ViewEvent_SelectedColorChanged)
				ViewEvent_SelectedColorChanged();
		}

		#endregion Dependency property

		#region ComboBox

		#region ComboBox data handling

		private void UpdateComboBoxSourceSelection(NamedColor color)
		{
			if (color.Equals(_guiComboBox.SelectedValue))
				return;

			_filterString = string.Empty;
			FillComboBoxWithFilteredItems(_filterString, false);
			_guiComboBox.SelectedValue = color;
		}

		protected override IColorSet GetColorSetForComboBox()
		{
			NamedColor selColor = this.SelectedColor;
			if (selColor.ParentColorSet != null)
				return selColor.ParentColorSet;
			else
				return NamedColors.Instance;
		}

		private List<object> _comboBoxSeparator1 = new List<object> { new Separator() { Name = "ThisIsASeparatorForTheComboBox", Tag = "Last used colors" } };
		private List<object> _comboBoxSeparator2 = new List<object> { new Separator() { Name = "ThisIsASeparatorForTheComboBox", Tag = "Color set" } };

		protected override bool FillComboBoxWithFilteredItems(string filterString, bool onlyIfItemsRemaining)
		{
			List<object> lastUsed;

			List<object> separator = new List<object> { new Separator() { Name = "ThisIsASeparatorForTheComboBox", Tag = "Color set" } };

			lastUsed = GetFilteredList(_lastLocalUsedItems, filterString, ShowPlotColorsOnly);

			var colorSet = GetColorSetForComboBox();
			var known = GetFilteredList(colorSet, filterString, false); // False as 3rd argument is acceptable here because we know that if ShowPlotColorsOnly is set, then the colorSet is for sure a plot color set

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

		protected static List<object> GetFilteredList(IList<NamedColor> originalList, string filterString, bool showPlotColorsOnly)
		{
			var result = new List<object>();
			filterString = filterString.ToLowerInvariant();
			foreach (var item in originalList)
			{
				if (showPlotColorsOnly && (item.ParentColorSet == null || !item.ParentColorSet.IsPlotColorSet))
					continue;
				if (item.Name.ToLowerInvariant().StartsWith(filterString))
					result.Add(item);
			}
			return result;
		}

		#endregion ComboBox data handling

		#region ComboBox event handling

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
				_guiComboBox.SelectedValue = SelectedColor;
			else
				this.SelectedColor = (NamedColor)_guiComboBox.SelectedValue;
		}

		#endregion ComboBox event handling

		#endregion ComboBox

		#region Context menus

		protected void EhShowCustomColorDialog(object sender, RoutedEventArgs e)
		{
			NamedColor newColor;
			if (base.InternalShowCustomColorDialog(sender, out newColor))
			{
				InternalSelectedColor = newColor;
			}
		}

		protected void EhChooseOpacityFromContextMenu(object sender, RoutedEventArgs e)
		{
			NamedColor newColor;
			if (base.InternalChooseOpacityFromContextMenu(sender, out newColor))
			{
				InternalSelectedColor = newColor;
			}
		}

		#endregion Context menus
	}
}
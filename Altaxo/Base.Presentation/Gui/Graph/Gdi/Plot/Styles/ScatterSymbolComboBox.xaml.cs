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

using Altaxo.Graph.Graph2D.Plot.Groups;
using Altaxo.Graph.Graph2D.Plot.Styles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{
	public abstract class ScatterSymbolComboBoxBase : Altaxo.Gui.Drawing.StyleListComboBoxBase<ScatterSymbolListManager, ScatterSymbolList, IScatterSymbol>
	{
		public ScatterSymbolComboBoxBase(ScatterSymbolListManager manager) : base(manager)
		{
		}
	}

	/// <summary>
	/// Interaction logic for ColorComboBoxEx.xaml
	/// </summary>
	public partial class ScatterSymbolComboBox : ScatterSymbolComboBoxBase
	{
		private ScatterSymbolToItemNameConverter _itemToItemNameConverter = new ScatterSymbolToItemNameConverter();

		#region Constructors

		public ScatterSymbolComboBox()
			: base(ScatterSymbolListManager.Instance)
		{
			UpdateTreeViewTreeNodes();

			InitializeComponent();

			UpdateComboBoxSourceSelection(SelectedItem);
			UpdateTreeViewSelection();
		}

		#endregion Constructors

		#region Implementation of abstract base class members

		protected override TreeView GuiTreeView { get { return _guiTreeView; } }

		protected override ComboBox GuiComboBox { get { return _guiComboBox; } }

		public override string GetDisplayName(IScatterSymbol item)
		{
			return (string)_itemToItemNameConverter.Convert(item, typeof(string), null, System.Globalization.CultureInfo.InvariantCulture);
		}

		#endregion Implementation of abstract base class members
	}
}
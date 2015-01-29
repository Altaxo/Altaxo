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

using Altaxo.Graph.Plot.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for XYPlotGroupControl.xaml
	/// </summary>
	public partial class PlotGroupCollectionControlSimple : UserControl, IPlotGroupCollectionViewSimple
	{
		public PlotGroupCollectionControlSimple()
		{
			InitializeComponent();
		}

		#region IXYPlotGroupView

		public void InitializePlotGroupConditions(bool bColor, bool bLineType, bool bSymbol, bool bConcurrently, Altaxo.Graph.Plot.Groups.PlotGroupStrictness bStrict)
		{
			this._rbtConcurrently.IsChecked = bConcurrently;
			this._rbtSequential.IsChecked = !bConcurrently;

			this.m_chkPlotGroupColor.IsChecked = bColor;
			this.m_chkPlotGroupLineType.IsChecked = bLineType;
			this.m_chkPlotGroupSymbol.IsChecked = bSymbol;

			this._cbStrict.ItemsSource = new object[] { "Normal", "Exact", "Strict" };
			this._cbStrict.SelectedIndex = (int)bStrict;
		}

		public Altaxo.Graph.Plot.Groups.PlotGroupStrictness PlotGroupStrict
		{
			get
			{
				return (PlotGroupStrictness)(System.Enum.GetValues(typeof(PlotGroupStrictness))).GetValue(this._cbStrict.SelectedIndex);
			}
		}

		public bool PlotGroupColor
		{
			get { return true == m_chkPlotGroupColor.IsChecked; }
		}

		public bool PlotGroupLineType
		{
			get { return true == m_chkPlotGroupLineType.IsChecked; }
		}

		public bool PlotGroupSymbol
		{
			get { return true == m_chkPlotGroupSymbol.IsChecked; }
		}

		public bool PlotGroupConcurrently
		{
			get { return true == _rbtConcurrently.IsChecked; }
		}

		public bool PlotGroupUpdate
		{
			get { return true; }
		}

		#endregion IXYPlotGroupView
	}
}
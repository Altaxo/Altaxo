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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph.Scales.Ticks
{
	/// <summary>
	/// Interaction logic for AngularTickSpacingControl.xaml
	/// </summary>
	public partial class SpanTickSpacingControl : UserControl, ISpanTickSpacingView
	{
		public SpanTickSpacingControl()
		{
			InitializeComponent();
		}

		public double RelativePositionOfTick
		{
			get
			{
				return _guiRelTickPos.SelectedQuantityAsValueInSIUnits;
			}
			set
			{
				_guiRelTickPos.SelectedQuantityAsValueInSIUnits = value;
			}
		}

		public bool ShowEndOrgRatio
		{
			get
			{
				return _rbRatio.IsChecked == true;
			}
			set
			{
				_rbRatio.IsChecked = value;
				_rbDifference.IsChecked = !value;
			}
		}

		public bool TransfoOperationIsMultiply
		{
			get { return _cbTransfoOperation.SelectedIndex == 1; }
			set { _cbTransfoOperation.SelectedIndex = (value ? 1 : 0); }
		}

		public double DivideBy
		{
			get
			{
				return _edDivideBy.SelectedValue;
			}
			set
			{
				_edDivideBy.SelectedValue = value;
			}
		}
	}
}
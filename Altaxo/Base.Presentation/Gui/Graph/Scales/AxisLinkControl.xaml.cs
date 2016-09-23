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
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph.Scales
{
	/// <summary>
	/// Interaction logic for AxisLinkControl.xaml
	/// </summary>
	public partial class AxisLinkControl : UserControl, IAxisLinkView
	{
		public AxisLinkControl()
		{
			InitializeComponent();
		}

		private void EnableCustom(bool bEnab)
		{
			this._guiLinkAxisOrgA.IsEnabled = bEnab;
			this._guiLinkAxisOrgB.IsEnabled = bEnab;
			this._guiLinkAxisEndA.IsEnabled = bEnab;
			this._guiLinkAxisEndB.IsEnabled = bEnab;
		}

		private void EhLinkStraight_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (this._guiLinkAxisStraight.IsChecked == true)
				EnableCustom(false);
		}

		private void EhLinkCustom_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (this._guiLinkAxisCustom.IsChecked == true)
				EnableCustom(true);
		}

		#region IAxisLinkView

		public bool IsStraightLink
		{
			get
			{
				return _guiLinkAxisStraight.IsChecked == true;
			}
			set
			{
				if (value)
				{
					this._guiLinkAxisStraight.IsChecked = true;
					EnableCustom(false);
				}
				else
				{
					this._guiLinkAxisCustom.IsChecked = true;
					EnableCustom(true);
				}
			}
		}

		public double OrgA
		{
			get { return _guiLinkAxisOrgA.SelectedValue; }
			set { _guiLinkAxisOrgA.SelectedValue = value; }
		}

		public double OrgB
		{
			get { return _guiLinkAxisOrgB.SelectedValue; }
			set { _guiLinkAxisOrgB.SelectedValue = value; }
		}

		public double EndA
		{
			get { return _guiLinkAxisEndA.SelectedValue; }
			set { _guiLinkAxisEndA.SelectedValue = value; }
		}

		public double EndB
		{
			get { return _guiLinkAxisEndB.SelectedValue; }
			set { _guiLinkAxisEndB.SelectedValue = value; }
		}

		#endregion IAxisLinkView
	}
}
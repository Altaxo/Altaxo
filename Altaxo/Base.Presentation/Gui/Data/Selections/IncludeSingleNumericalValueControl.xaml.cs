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
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using Altaxo.Gui.Common;
using Altaxo.Gui.Graph.Plot.Data;
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

namespace Altaxo.Gui.Data.Selections
{
	/// <summary>
	/// Interaction logic for RangeOfPhysicalValuesControl.xaml
	/// </summary>
	public partial class IncludeSingleNumericalValueControl : UserControl, IIncludeSingleNumericalValueView
	{
		public IncludeSingleNumericalValueControl()
		{
			InitializeComponent();
		}

		public void Init_Column(string boxText, string toolTip, int status)
		{
			this._guiColumn.Text = boxText;
			this._guiColumn.ToolTip = toolTip;
			this._guiColumn.Background = DefaultSeverityColumnColors.GetSeverityColor(status);
		}

		public void Init_ColumnTransformation(string boxText, string toolTip)
		{
			if (null == boxText)
			{
				this._guiColumnTransformation.Visibility = Visibility.Collapsed;
			}
			else
			{
				this._guiColumnTransformation.Text = boxText;
				this._guiColumnTransformation.ToolTip = toolTip;
				this._guiColumnTransformation.Visibility = Visibility.Visible;
			}
		}

		public void Init_Index(int idx)
		{
			_guiDataLabel.Content = string.Format("Col#{0}:", idx);
		}

		public double Value
		{
			get { return _guiValue.Value; }
			set
			{
				_guiValue.Value = value;
			}
		}
	}
}
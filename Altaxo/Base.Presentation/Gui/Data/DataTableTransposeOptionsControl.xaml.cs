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

namespace Altaxo.Gui.Data
{
	/// <summary>
	/// Interaction logic for TransposeWorksheetControl.xaml
	/// </summary>
	public partial class DataTableTransposeOptionsControl : UserControl, IDataTableTransposeOptionsView
	{
		public DataTableTransposeOptionsControl()
		{
			InitializeComponent();
		}

		public string ColumnNamingPreString
		{
			get
			{
				return _guiPreStringForColumnName.Text;
			}

			set
			{
				_guiPreStringForColumnName.Text = value;
			}
		}

		/// <summary>
		/// Get/sets the number of data columns that are moved to the property columns before transposing the data columns.
		/// </summary>
		public int DataColumnsMoveToPropertyColumns
		{
			get { return _ctrlNumMovedDataCols.Value; }
			set
			{
				_ctrlNumMovedDataCols.Value = value;
			}
		}

		/// <summary>
		/// Get/sets the number of property columns that are moved after transposing the data columns to the data columns collection.
		/// </summary>
		public int PropertyColumnsMoveToDataColumns
		{
			get { return _ctrlNumMovedPropCols.Value; }
			set
			{
				_ctrlNumMovedPropCols.Value = value;
			}
		}

		public bool StoreDataColumnNamesInFirstDataColumn
		{
			get
			{
				return _guiStoreSourceDataColumnNames.IsChecked == true;
			}

			set
			{
				_guiStoreSourceDataColumnNames.IsChecked = value;
			}
		}

		public bool UseFirstDataColumnForColumnNaming
		{
			get
			{
				return _guiUseFirstColForColumnNames.IsChecked == true;
			}

			set
			{
				_guiUseFirstColForColumnNames.IsChecked = value;
			}
		}
	}
}
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

using Altaxo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph.Plot.Data
{
	public class PlotColumnInformation
	{
		/// <summary>
		/// The data table that is supposed to be the parent data table of the column.
		/// </summary>
		protected DataTable _supposedDataTable;

		/// <summary>
		/// The group number that is supposed to be the group number of the column.
		/// </summary>
		protected int _supposedGroupNumber;

		/// <summary>
		/// If the underlying column is or was a data column, then here we store the data column's name.
		/// </summary>
		protected string _nameOfUnderlyingDataColumn;

		protected IReadableColumn _underlyingColumn;
		protected IVariantToVariantTransformation _transformation;

		// Data for the control
		protected string _plotColumnBoxText;

		protected string _plotColumnToolTip;

		protected PlotColumnControlState _plotColumnBoxState;

		protected string _transformationBoxText;
		protected string _transformationToolTip;

		/// <summary>
		/// Gives the plot column box state if the column is missing.
		/// </summary>
		protected PlotColumnControlState _plotColumnBoxStateIfColumnIsMissing = PlotColumnControlState.Normal;

		protected bool _isDirty;

		#region Constructors

		public PlotColumnInformation(IReadableColumn column, string nameOfUnderlyingDataColumn)
		{
			_nameOfUnderlyingDataColumn = nameOfUnderlyingDataColumn;
			Column = column;
		}

		#endregion Constructors

		#region Columns and transformations

		/// <summary>
		/// The data table that is supposed to be the parent data table of the column.
		/// </summary>
		public DataTable SupposedDataTable
		{
			get
			{
				return _supposedDataTable;
			}
		}

		/// <summary>The column itself.</summary>
		public IReadableColumn Column
		{
			get
			{
				if (null != _transformation && null != _underlyingColumn)
					return new TransformedReadableColumn(_underlyingColumn, _transformation);
				else
					return _underlyingColumn;
			}
			set
			{
				if (value is ITransformedReadableColumn)
				{
					_underlyingColumn = (value as ITransformedReadableColumn).UnderlyingReadableColumn;
					_transformation = (value as ITransformedReadableColumn).Transformation;
				}
				else
				{
					_underlyingColumn = value;
					_transformation = null;
				}

				_isDirty = true;
			}
		}

		public IReadableColumn UnderlyingColumn
		{
			get
			{
				return _underlyingColumn;
			}
			set
			{
				if (value is ITransformedReadableColumn)
					throw new ArgumentException("Nesting transformed columns is not allowed", nameof(value));

				_underlyingColumn = value;
				_nameOfUnderlyingDataColumn = null;

				_isDirty = true;
			}
		}

		/// <summary>
		/// The column transformation.
		/// </summary>
		public IVariantToVariantTransformation Transformation
		{
			get
			{
				return _transformation;
			}
			set
			{
				_transformation = value;

				_isDirty = true;
			}
		}

		/// <summary>
		/// Gets the name of the (underlying) data column, or null if this instance holds another column, or the name is unkown.
		/// </summary>
		/// <value>
		/// The name of the (underlying) data column.
		/// </value>
		public string NameOfDataColumn { get { return _nameOfUnderlyingDataColumn; } }

		#endregion Columns and transformations

		#region properties to use with Gui controls

		/// <summary>The text that will be shown in the plot column text box.</summary>
		public string PlotColumnBoxText { get { return _plotColumnBoxText; } }

		/// <summary>The tooltip that will be shown when the user hovers over the plot column text box.</summary>
		public string PlotColumnToolTip
		{
			get { return _plotColumnToolTip; }
		}

		/// <summary>State of the column textbox. Depending on the state, the background of the textbox will assume different colors.</summary>
		public PlotColumnControlState PlotColumnBoxState { get { return _plotColumnBoxState; } }

		/// <summary>
		/// Set the plot column box state that is used if the column is missing.
		/// </summary>
		public PlotColumnControlState PlotColumnBoxStateIfColumnIsMissing { set { _plotColumnBoxStateIfColumnIsMissing = value; } }

		/// <summary>This text will be shown in the transformation text box.</summary>
		public string TransformationTextToShow { get { return _transformationBoxText; } }

		/// <summary>The tooltip that will be shown when the user hovers over the transformation text box.</summary>
		public string TransformationToolTip { get { return _transformationToolTip; } }

		#endregion properties to use with Gui controls

		protected virtual void OnChanged()
		{
		}

		private static bool InternalSet<T>(ref T member, T value)
		{
			var result = !EqualityComparer<T>.Default.Equals(member, value);
			member = value;
			return result;
		}

		/// <summary>
		/// Updates the information, assuming that the underlying data table is the same as before.
		/// </summary>
		/// <param name="dataTableOfPlotItem">The data table of plot item.</param>
		/// <param name="groupNumberOfPlotItem">The group number of plot item.</param>
		public void Update(DataTable dataTableOfPlotItem, int groupNumberOfPlotItem)
		{
			Update(dataTableOfPlotItem, groupNumberOfPlotItem, false);
		}

		/// <summary>
		/// Updates the information, indicating in <paramref name="hasTableChanged"/> whether the underlying data table has changed.
		/// </summary>
		/// <param name="dataTableOfPlotItem">The data table of plot item.</param>
		/// <param name="groupNumberOfPlotItem">The group number of plot item.</param>
		/// <param name="hasTableChanged">If set to <c>true</c>, the data table has recently changed.</param>
		public void Update(DataTable dataTableOfPlotItem, int groupNumberOfPlotItem, bool hasTableChanged)
		{
			bool hasChanged = false;

			if (!object.ReferenceEquals(_supposedDataTable, dataTableOfPlotItem))
			{
				_supposedDataTable = dataTableOfPlotItem;
				hasChanged = true;
			}

			if (!(_supposedGroupNumber == groupNumberOfPlotItem))
			{
				_supposedGroupNumber = groupNumberOfPlotItem;
				hasChanged = true;
			}

			if (null == _underlyingColumn)
			{
				if (string.IsNullOrEmpty(_nameOfUnderlyingDataColumn))
				{
					hasChanged |= InternalSet(ref _plotColumnBoxText, string.Empty);
					hasChanged |= InternalSet(ref _plotColumnBoxState, _plotColumnBoxStateIfColumnIsMissing);
					switch (_plotColumnBoxState)
					{
						case PlotColumnControlState.Normal:
							hasChanged |= InternalSet(ref _plotColumnToolTip, string.Empty);
							break;

						case PlotColumnControlState.Warning:
							hasChanged |= InternalSet(ref _plotColumnToolTip, "Warning: it is highly recommended to set a column!");
							break;

						case PlotColumnControlState.Error:
							hasChanged |= InternalSet(ref _plotColumnToolTip, "Error: it is mandatory to set a column!");
							break;
					}
				}
				else
				{
					hasChanged |= InternalSet(ref _plotColumnBoxText, _nameOfUnderlyingDataColumn);
					hasChanged |= InternalSet(ref _plotColumnToolTip, string.Format("Column {0} can not be found in this table with this group number", _nameOfUnderlyingDataColumn));
					hasChanged |= InternalSet(ref _plotColumnBoxState, PlotColumnControlState.Error);
				}
			}
			else if (_underlyingColumn is DataColumn)
			{
				var dcolumn = (DataColumn)_underlyingColumn;
				var parentTable = DataTable.GetParentDataTableOf(dcolumn);
				var parentCollection = DataColumnCollection.GetParentDataColumnCollectionOf(dcolumn);
				if (null == parentTable)
				{
					hasChanged |= InternalSet(ref _plotColumnToolTip, string.Format("This column is an orphaned data column without a parent data table", _nameOfUnderlyingDataColumn));
					hasChanged |= InternalSet(ref _plotColumnBoxState, PlotColumnControlState.Error);
					if (parentCollection == null)
					{
						hasChanged |= InternalSet(ref _plotColumnBoxText, string.Format("Orphaned {0}", dcolumn.GetType().Name));
					}
					else
					{
						string columnName = parentCollection.GetColumnName(dcolumn);
						hasChanged |= InternalSet(ref _nameOfUnderlyingDataColumn, columnName);
						hasChanged |= InternalSet(ref _plotColumnBoxText, columnName);
					}
				}
				else // UnderlyingColumn has a parent table
				{
					if (!object.ReferenceEquals(parentTable, dataTableOfPlotItem))
					{
						hasChanged |= InternalSet(ref _plotColumnBoxText, parentTable.DataColumns.GetColumnName(dcolumn));
						hasChanged |= InternalSet(ref _plotColumnToolTip, string.Format("The column {0} is a data column with another parent data table: {1}", _nameOfUnderlyingDataColumn, parentTable.Name));
						hasChanged |= InternalSet(ref _plotColumnBoxState, PlotColumnControlState.Warning);
					}
					if(!(parentTable.DataColumns.GetColumnGroup(dcolumn) ==_supposedGroupNumber))
					{
						string columnName = parentTable.DataColumns.GetColumnName(dcolumn);
						hasChanged |= InternalSet(ref _nameOfUnderlyingDataColumn, columnName);
						hasChanged |= InternalSet(ref _plotColumnBoxText, columnName);
						hasChanged |= InternalSet(ref _plotColumnToolTip, string.Format("The column {0} is a data column with another group number: {1}", _nameOfUnderlyingDataColumn, parentTable.DataColumns.GetColumnGroup(dcolumn)));
						hasChanged |= InternalSet(ref _plotColumnBoxState, PlotColumnControlState.Warning);
					}
					else
					{
						string columnName = parentTable.DataColumns.GetColumnName(dcolumn);
						hasChanged |= InternalSet(ref _nameOfUnderlyingDataColumn, columnName);
						hasChanged |= InternalSet(ref _plotColumnBoxText, columnName);
						hasChanged |= InternalSet(ref _plotColumnToolTip, string.Format("UnderlyingColumn {0} of data table {1}", _nameOfUnderlyingDataColumn, parentTable.Name));
						hasChanged |= InternalSet(ref _plotColumnBoxState, PlotColumnControlState.Normal);
					}
				}
			}
			else // UnderlyingColumn is not a DataColumn, but is something else
			{
				hasChanged |= InternalSet(ref _nameOfUnderlyingDataColumn, null);
				hasChanged |= InternalSet(ref _plotColumnBoxText, _underlyingColumn.FullName);
				hasChanged |= InternalSet(ref _plotColumnToolTip, string.Format("Independent data of type {0}: {1}", _underlyingColumn.GetType(), _underlyingColumn.ToString()));
				hasChanged |= InternalSet(ref _plotColumnBoxState, PlotColumnControlState.Normal);
			}

			// now the transformation
			if (null != _transformation)
			{
				hasChanged |= InternalSet(ref _transformationBoxText, _transformation.RepresentationAsOperator ?? _transformation.RepresentationAsFunction);
				hasChanged |= InternalSet(ref _transformationToolTip, string.Format("Transforms the column data by the function f(x)={0}", _transformation.RepresentationAsFunction));
			}
			else // transformation is null
			{
				hasChanged |= InternalSet(ref _transformationBoxText, string.Empty);
				hasChanged |= InternalSet(ref _transformationToolTip, "No transformation applied.");
			}

			_isDirty |= hasChanged;
			_isDirty |= hasTableChanged;

			if (_isDirty)
			{
				_isDirty = false;
				OnChanged();
			}
		}
	}
}
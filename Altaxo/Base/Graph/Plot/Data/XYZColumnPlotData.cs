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
using Altaxo.Data.Selections;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D;
using Altaxo.Graph.Graph3D.Plot.Data;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Main;
using Altaxo.Serialization;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Altaxo.Graph.Plot.Data
{
	/// <summary>
	/// Summary description for XYColumnPlotData.
	/// </summary>
	public class XYZColumnPlotData
		:
		Main.SuspendableDocumentNodeWithSetOfEventArgs,
		IColumnPlotData,
		System.ICloneable
	{
		/// <summary>Holds a reference to the underlying data table. If the Empty property of the proxy is null, the underlying table must be determined from the column proxies.</summary>
		protected DataTableProxy _dataTable;

		/// <summary>The group number of the data columns. All data columns should have this group number. Data columns having other group numbers will be marked.</summary>
		protected int _groupNumber;

		/// <summary>
		/// The selection of data rows to be plotted.
		/// </summary>
		protected IRowSelection _dataRowSelection;

		protected Altaxo.Data.IReadableColumnProxy _xColumn; // the X-Column
		protected Altaxo.Data.IReadableColumnProxy _yColumn; // the Y-Column
		protected Altaxo.Data.IReadableColumnProxy _zColumn; // the Z-Column

		// cached or temporary data
		protected IPhysicalBoundaries _xBoundaries;

		protected IPhysicalBoundaries _yBoundaries;

		protected IPhysicalBoundaries _zBoundaries;

		/// <summary>List of plot points that is allocated once per thread (as thread local storage variable).</summary>
		[ThreadStatic]
		[NonSerialized]
		protected static List<PointD3D> _tlsBufferedPlotData;

		/// <summary>
		/// One more that the index to the last valid pair of plot data.
		/// </summary>
		protected int _pointCount;

		protected bool _isCachedDataValidX = false;
		protected bool _isCachedDataValidY = false;
		protected bool _isCachedDataValidZ = false;

		#region Serialization

		/// <summary>
		/// Deserialization constructor. Initializes a new instance of the <see cref="XYZColumnPlotData"/> class without any member initialization.
		/// </summary>
		/// <param name="info">The information.</param>
		protected XYZColumnPlotData(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
		}

		/// <summary>
		/// 2016-05-31 initial version
		/// </summary>
		/// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYZColumnPlotData), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (XYZColumnPlotData)obj;

				info.AddValue("DataTable", s._dataTable);
				info.AddValue("GroupNumber", s._groupNumber);

				info.AddValue("RowSelection", s._dataRowSelection);

				info.AddValue("XColumn", s._xColumn);
				info.AddValue("YColumn", s._yColumn);
				info.AddValue("ZColumn", s._zColumn);

				info.AddValue("XBoundaries", s._xBoundaries);
				info.AddValue("YBoundaries", s._yBoundaries);
				info.AddValue("ZBoundaries", s._zBoundaries);
			}

			public virtual XYZColumnPlotData SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (XYZColumnPlotData)o ?? new XYZColumnPlotData(info);

				s._dataTable = (DataTableProxy)info.GetValue("DataTable", s);
				if (null != s._dataTable) s._dataTable.ParentObject = s;

				s._groupNumber = info.GetInt32("GroupNumber");

				s._dataRowSelection = (IRowSelection)info.GetValue("RowSelection", s);
				if (null != s._dataRowSelection) s._dataRowSelection.ParentObject = s;

				s._xColumn = (IReadableColumnProxy)info.GetValue("XColumn", s);
				if (null != s._xColumn) s._xColumn.ParentObject = s;

				s._yColumn = (IReadableColumnProxy)info.GetValue("YColumn", s);
				if (null != s._yColumn) s._yColumn.ParentObject = s;

				s._zColumn = (IReadableColumnProxy)info.GetValue("ZColumn", s);
				if (null != s._zColumn) s._zColumn.ParentObject = s;

				s._xBoundaries = (IPhysicalBoundaries)info.GetValue("XBoundaries", s);
				if (null != s._xBoundaries) s._xBoundaries.ParentObject = s;

				s._yBoundaries = (IPhysicalBoundaries)info.GetValue("YBoundaries", s);
				if (null != s._yBoundaries) s._yBoundaries.ParentObject = s;

				s._zBoundaries = (IPhysicalBoundaries)info.GetValue("ZBoundaries", s);
				if (null != s._zBoundaries) s._zBoundaries.ParentObject = s;

				return s;
			}

			public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = SDeserialize(o, info, parent);
				return s;
			}
		}

		#endregion Serialization

		public XYZColumnPlotData(Altaxo.Data.DataTable dataTable, int groupNumber, Altaxo.Data.IReadableColumn xColumn, Altaxo.Data.IReadableColumn yColumn, Altaxo.Data.IReadableColumn zColumn)
		{
			DataTable = dataTable;
			ChildSetMember(ref _dataRowSelection, new AllRows());
			_groupNumber = groupNumber;
			XColumn = xColumn;
			YColumn = yColumn;
			ZColumn = zColumn;
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="from">The object to copy from.</param>
		/// <remarks>Only clones the references to the data columns, not the columns itself.</remarks>
		public XYZColumnPlotData(XYZColumnPlotData from)
		{
			ChildCopyToMember(ref _dataTable, from._dataTable);
			this._groupNumber = from._groupNumber;
			ChildCloneToMember(ref _dataRowSelection, from._dataRowSelection);

			this._dataRowSelection = from._dataRowSelection;

			ChildCopyToMember(ref _xColumn, from._xColumn);
			ChildCopyToMember(ref _yColumn, from._yColumn);
			ChildCopyToMember(ref _zColumn, from._zColumn);

			// cached or temporary data

			if (null != from._xBoundaries)
				ChildCopyToMember(ref _xBoundaries, from._xBoundaries);

			if (null != from._yBoundaries)
				ChildCopyToMember(ref _yBoundaries, from._yBoundaries);

			if (null != from._zBoundaries)
				ChildCopyToMember(ref _zBoundaries, from._zBoundaries);

			this._pointCount = from._pointCount;
			this._isCachedDataValidX = from._isCachedDataValidX;
			this._isCachedDataValidY = from._isCachedDataValidY;
		}

		protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _dataTable)
				yield return new DocumentNodeAndName(_dataTable, "DataTable");

			if (null != _dataRowSelection)
				yield return new DocumentNodeAndName(_dataRowSelection, nameof(DataRowSelection));

			if (null != _xColumn)
				yield return new Main.DocumentNodeAndName(_xColumn, "XColumn");

			if (null != _yColumn)
				yield return new Main.DocumentNodeAndName(_yColumn, "YColumn");

			if (null != _zColumn)
				yield return new Main.DocumentNodeAndName(_yColumn, "VColumn");

			if (null != _xBoundaries)
				yield return new Main.DocumentNodeAndName(_xBoundaries, "XBoundaries");

			if (null != _yBoundaries)
				yield return new Main.DocumentNodeAndName(_yBoundaries, "YBoundaries");

			if (null != _zBoundaries)
				yield return new Main.DocumentNodeAndName(_yBoundaries, "VBoundaries");
		}

		/// <summary>
		/// Creates a cloned copy of this object.
		/// </summary>
		/// <returns>The cloned copy of this object.</returns>
		/// <remarks>The data columns refered by this object are <b>not</b> cloned, only the reference is cloned here.</remarks>
		public object Clone()
		{
			return new XYZColumnPlotData(this);
		}

		public DataTable DataTable
		{
			get
			{
				return _dataTable?.Document;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));

				if (object.ReferenceEquals(DataTable, value))
					return;

				if (ChildSetMember(ref _dataTable, new DataTableProxy(value)))
				{
					EhSelfChanged(PlotItemDataChangedEventArgs.Empty);
				}
			}
		}

		public int GroupNumber
		{
			get
			{
				return _groupNumber;
			}
			set
			{
				if (!(_groupNumber == value))
				{
					_groupNumber = value;
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// The selection of data rows to be plotted.
		/// </summary>
		public IRowSelection DataRowSelection
		{
			get
			{
				return _dataRowSelection;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));

				if (!_dataRowSelection.Equals(value))
				{
					ChildSetMember(ref _dataRowSelection, value);
					_isCachedDataValidX = _isCachedDataValidY = _isCachedDataValidZ = false;
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets the name of the x column, depending on the provided level.
		/// </summary>
		/// <param name="level">The level (0..2).</param>
		/// <returns>The name of the x-column, depending on the provided level: 0: only name of the data column. 1: table name and column name. 2: table name, collection, and column name.</returns>
		public string GetXName(int level)
		{
			IReadableColumn col = this._xColumn.Document;
			if (col is Altaxo.Data.DataColumn)
			{
				Altaxo.Data.DataTable table = Altaxo.Data.DataTable.GetParentDataTableOf((DataColumn)col);
				string tablename = table == null ? string.Empty : table.Name + "\\";
				string collectionname = table == null ? string.Empty : (table.PropertyColumns.ContainsColumn((DataColumn)col) ? "PropCols\\" : "DataCols\\");
				if (level <= 0)
					return ((DataColumn)col).Name;
				else if (level == 1)
					return tablename + ((DataColumn)col).Name;
				else
					return tablename + collectionname + ((DataColumn)col).Name;
			}
			else if (col != null)
			{
				return col.FullName;
			}
			else
			{
				return _xColumn.GetName(level) + " (broken)";
			}
		}

		/// <summary>
		/// Gets the name of the y column, depending on the provided level.
		/// </summary>
		/// <param name="level">The level (0..2).</param>
		/// <returns>The name of the y-column, depending on the provided level: 0: only name of the data column. 1: table name and column name. 2: table name, collection, and column name.</returns>
		public string GetYName(int level)
		{
			IReadableColumn col = this._yColumn.Document;
			if (col is Altaxo.Data.DataColumn)
			{
				Altaxo.Data.DataTable table = Altaxo.Data.DataTable.GetParentDataTableOf((DataColumn)col);
				string tablename = table == null ? string.Empty : table.Name + "\\";
				string collectionname = table == null ? string.Empty : (table.PropertyColumns.ContainsColumn((DataColumn)col) ? "PropCols\\" : "DataCols\\");
				if (level <= 0)
					return ((DataColumn)col).Name;
				else if (level == 1)
					return tablename + ((DataColumn)col).Name;
				else
					return tablename + collectionname + ((DataColumn)col).Name;
			}
			else if (col != null)
			{
				return col.FullName;
			}
			else
			{
				return _yColumn.GetName(level) + " (broken)";
			}
		}

		/// <summary>
		/// Gets the name of the z column, depending on the provided level.
		/// </summary>
		/// <param name="level">The level (0..2).</param>
		/// <returns>The name of the z-column, depending on the provided level: 0: only name of the data column. 1: table name and column name. 2: table name, collection, and column name.</returns>
		public string GetZName(int level)
		{
			IReadableColumn col = this._zColumn.Document;
			if (col is Altaxo.Data.DataColumn)
			{
				Altaxo.Data.DataTable table = Altaxo.Data.DataTable.GetParentDataTableOf((DataColumn)col);
				string tablename = table == null ? string.Empty : table.Name + "\\";
				string collectionname = table == null ? string.Empty : (table.PropertyColumns.ContainsColumn((DataColumn)col) ? "PropCols\\" : "DataCols\\");
				if (level <= 0)
					return ((DataColumn)col).Name;
				else if (level == 1)
					return tablename + ((DataColumn)col).Name;
				else
					return tablename + collectionname + ((DataColumn)col).Name;
			}
			else if (col != null)
			{
				return col.FullName;
			}
			else
			{
				return _zColumn.GetName(level) + " (broken)";
			}
		}

		public void MergeXBoundsInto(IPhysicalBoundaries pb)
		{
			if (null == _xBoundaries || pb.GetType() != _xBoundaries.GetType())
				this.SetXBoundsFromTemplate(pb);

			if (!this._isCachedDataValidX)
			{
				using (var suspendToken = SuspendGetToken())
				{
					this.CalculateCachedData();
				}
			}
			pb.Add(_xBoundaries);
		}

		public void MergeYBoundsInto(IPhysicalBoundaries pb)
		{
			if (null == _yBoundaries || pb.GetType() != _yBoundaries.GetType())
				this.SetYBoundsFromTemplate(pb);

			if (!this._isCachedDataValidY)
			{
				using (var suspendToken = SuspendGetToken())
				{
					this.CalculateCachedData();
				}
			}
			pb.Add(_yBoundaries);
		}

		public void MergeZBoundsInto(IPhysicalBoundaries pb)
		{
			if (null == _zBoundaries || pb.GetType() != _zBoundaries.GetType())
				this.SetZBoundsFromTemplate(pb);

			if (!this._isCachedDataValidY)
			{
				using (var suspendToken = SuspendGetToken())
				{
					this.CalculateCachedData();
				}
			}
			pb.Add(_zBoundaries);
		}

		/// <summary>
		/// This sets the x boundary object to a object of the same type as val. The inner data of the boundary, if present,
		/// are copied into the new x boundary object.
		/// </summary>
		/// <param name="val">The template boundary object.</param>
		protected void SetXBoundsFromTemplate(IPhysicalBoundaries val)
		{
			if (null == _xBoundaries || val.GetType() != _xBoundaries.GetType())
			{
				if (ChildCopyToMember(ref _xBoundaries, val))
				{
					_isCachedDataValidX = false;

					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// This sets the y boundary object to a object of the same type as val. The inner data of the boundary, if present,
		/// are copied into the new y boundary object.
		/// </summary>
		/// <param name="val">The template boundary object.</param>
		protected void SetYBoundsFromTemplate(IPhysicalBoundaries val)
		{
			if (null == _yBoundaries || val.GetType() != _yBoundaries.GetType())
			{
				if (ChildCopyToMember(ref _yBoundaries, val))
				{
					_isCachedDataValidY = false;

					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// This sets the v boundary object to a object of the same type as val. The inner data of the boundary, if present,
		/// are copied into the new y boundary object.
		/// </summary>
		/// <param name="val">The template boundary object.</param>
		protected void SetZBoundsFromTemplate(IPhysicalBoundaries val)
		{
			if (null == _zBoundaries || val.GetType() != _zBoundaries.GetType())
			{
				if (ChildCopyToMember(ref _zBoundaries, val))
				{
					_isCachedDataValidZ = false;

					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
		/// to change a plot so that the plot items refer to another table.
		/// </summary>
		/// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
		public void VisitDocumentReferences(DocNodeProxyReporter Report)
		{
			Report(_dataTable, this, "DataTable");
			Report(_xColumn, this, "XColumn");
			Report(_yColumn, this, "YColumn");
			Report(_zColumn, this, "ZColumn");

			_dataRowSelection.VisitDocumentReferences(Report);
		}

		/// <summary>
		/// Gets the columns used additionally by this style, e.g. the label column for a label plot style, or the error columns for an error bar plot style.
		/// </summary>
		/// <returns>An enumeration of tuples. Each tuple consist of the column name, as it should be used to identify the column in the data dialog. The second item of this
		/// tuple is a function that returns the column proxy for this column, in order to get the underlying column or to set the underlying column.</returns>
		public IEnumerable<Tuple<string, // Name of the column group, e.g. "X-Y-Data"
		IEnumerable<Tuple<
	string, // Column label
	IReadableColumn, // the column as it was at the time of this call
	string, // the name of the column (last part of the column proxies document path)
	Action<IReadableColumn, DataTable> // action to set the column during Apply of the controller
	>>>> GetAdditionallyUsedColumns()
		{
			yield return new Tuple<string, IEnumerable<Tuple<string, IReadableColumn, string, Action<IReadableColumn, DataTable>>>>("#0: X-Y-Z-Data", GetColumns());
		}

		/// <summary>
		/// Gets the columns used additionally by this style, e.g. the label column for a label plot style, or the error columns for an error bar plot style.
		/// </summary>
		/// <returns>An enumeration of tuples. Each tuple consist of the column name, as it should be used to identify the column in the data dialog. The second item of this
		/// tuple is a function that returns the column proxy for this column, in order to get the underlying column or to set the underlying column.</returns>
		private IEnumerable<Tuple<
			string, // Column label
			IReadableColumn, // the column as it was at the time of this call
			string, // the name of the column (last part of the column proxies document path)
			Action<IReadableColumn, DataTable> // action to set the column during Apply of the controller
			>> GetColumns()
		{
			yield return new Tuple<string, IReadableColumn, string, Action<IReadableColumn, DataTable>>("X", XColumn, _xColumn?.DocumentPath?.LastPartOrDefault, (col, table) => { XColumn = col; DataTable = table; });
			yield return new Tuple<string, IReadableColumn, string, Action<IReadableColumn, DataTable>>("Y", YColumn, _yColumn?.DocumentPath?.LastPartOrDefault, (col, table) => { YColumn = col; DataTable = table; });
			yield return new Tuple<string, IReadableColumn, string, Action<IReadableColumn, DataTable>>("Z", ZColumn, _zColumn?.DocumentPath?.LastPartOrDefault, (col, table) => { ZColumn = col; DataTable = table; });
		}

		/// <summary>
		/// One more than the index to the last valid plot data point. This is <b>not</b>
		/// the number of plottable points!
		/// </summary>
		/// <remarks>This is not neccessarily (PlotRangeStart+PlotRangeLength), but always less or equal than this. This is because
		/// the underlying arrays can be smaller than the proposed plot range.</remarks>
		public int PlotRangeEnd
		{
			get
			{
				if (!this._isCachedDataValidX || !_isCachedDataValidY || !_isCachedDataValidZ)
					this.CalculateCachedData();
				return this._pointCount;
			}
		}

		public Altaxo.Data.IReadableColumn XColumn
		{
			get
			{
				return _xColumn == null ? null : _xColumn.Document;
			}
			set
			{
				if (object.ReferenceEquals(XColumn, value))
					return;

				if (ChildSetMember(ref _xColumn, ReadableColumnProxyBase.FromColumn(value)))
				{
					_isCachedDataValidX = _isCachedDataValidY = _isCachedDataValidZ = false; // this influences both x and y boundaries
					EhSelfChanged(PlotItemDataChangedEventArgs.Empty);
				}
			}
		}

		public string XColumnName
		{
			get
			{
				return _xColumn?.DocumentPath?.LastPartOrDefault;
			}
		}

		public Altaxo.Data.IReadableColumn YColumn
		{
			get
			{
				return _yColumn == null ? null : _yColumn.Document;
			}
			set
			{
				if (object.ReferenceEquals(YColumn, value))
					return;

				if (ChildSetMember(ref _yColumn, ReadableColumnProxyBase.FromColumn(value)))
				{
					_isCachedDataValidX = _isCachedDataValidY = _isCachedDataValidZ = false; // this influences both x and y boundaries
					EhSelfChanged(PlotItemDataChangedEventArgs.Empty);
				}
			}
		}

		public string YColumnName
		{
			get
			{
				return _yColumn?.DocumentPath?.LastPartOrDefault;
			}
		}

		public Altaxo.Data.IReadableColumn ZColumn
		{
			get
			{
				return _zColumn?.Document;
			}
			set
			{
				if (object.ReferenceEquals(ZColumn, value))
					return;

				if (ChildSetMember(ref _zColumn, ReadableColumnProxyBase.FromColumn(value)))
				{
					_isCachedDataValidX = _isCachedDataValidY = _isCachedDataValidZ = false; // this influences both x and y boundaries
					EhSelfChanged(PlotItemDataChangedEventArgs.Empty);
				}
			}
		}

		public string ZColumnName
		{
			get
			{
				return _zColumn?.DocumentPath?.LastPartOrDefault;
			}
		}

		public override string ToString()
		{
			return String.Format("{0}(X), {1}(Y), {2}(V)", _xColumn.ToString(), _yColumn.ToString(), _zColumn.ToString());
		}

		/// <summary>
		/// Gets the maximum row index that can be deduced from the data columns. The calculation does <b>not</b> include the DataRowSelection.
		/// </summary>
		/// <returns>The maximum row index that can be deduced from the data columns.</returns>
		public int GetMaximumRowIndexFromDataColumns()
		{
			IReadableColumn xColumn = this.XColumn;
			IReadableColumn yColumn = this.YColumn;
			IReadableColumn zColumn = this.ZColumn;

			int maxRowIndex;

			if (xColumn == null || yColumn == null || zColumn == null)
			{
				maxRowIndex = 0;
			}
			else
			{
				maxRowIndex = int.MaxValue;

				if (xColumn.Count.HasValue)
					maxRowIndex = System.Math.Min(maxRowIndex, xColumn.Count.Value);
				if (yColumn.Count.HasValue)
					maxRowIndex = System.Math.Min(maxRowIndex, yColumn.Count.Value);
				if (zColumn.Count.HasValue)
					maxRowIndex = System.Math.Min(maxRowIndex, zColumn.Count.Value);

				// if both columns are indefinite long, we set the length to zero
				if (maxRowIndex == int.MaxValue || maxRowIndex < 0)
					maxRowIndex = 0;
			}

			return maxRowIndex;
		}

		public void CalculateCachedData(IPhysicalBoundaries xBounds, IPhysicalBoundaries yBounds, IPhysicalBoundaries vBounds)
		{
			if (this.IsDisposeInProgress)
				return;

			if (_xBoundaries == null || (xBounds != null && _xBoundaries.GetType() != xBounds.GetType()))
			{
				_isCachedDataValidX = false;
				this.SetXBoundsFromTemplate(xBounds);
			}

			if (_yBoundaries == null || (yBounds != null && _yBoundaries.GetType() != yBounds.GetType()))
			{
				_isCachedDataValidY = false;
				this.SetYBoundsFromTemplate(yBounds);
			}

			if (_zBoundaries == null || (vBounds != null && _zBoundaries.GetType() != vBounds.GetType()))
			{
				_isCachedDataValidZ = false;
				this.SetZBoundsFromTemplate(yBounds);
			}

			if (!_isCachedDataValidX || !_isCachedDataValidY || !_isCachedDataValidZ)
				CalculateCachedData();
		}

		public void CalculateCachedData()
		{
			if (this.IsDisposeInProgress)
				return;

			// we can calulate the bounds only if they are set before
			if (null == _xBoundaries && null == _yBoundaries && null == _zBoundaries)
				return;

			ISuspendToken suspendTokenX = null;
			ISuspendToken suspendTokenY = null;
			ISuspendToken suspendTokenZ = null;

			suspendTokenX = this._xBoundaries?.SuspendGetToken();
			suspendTokenY = this._yBoundaries?.SuspendGetToken();
			suspendTokenZ = this._zBoundaries?.SuspendGetToken();

			try
			{
				this._xBoundaries?.Reset();
				this._yBoundaries?.Reset();
				this._zBoundaries?.Reset();

				_pointCount = GetMaximumRowIndexFromDataColumns();

				IReadableColumn xColumn = this.XColumn;
				IReadableColumn yColumn = this.YColumn;
				IReadableColumn zColumn = this.ZColumn;

				foreach (int i in _dataRowSelection.GetSelectedRowIndicesFromTo(0, _pointCount, _dataTable?.Document?.DataColumns, _pointCount))
				{
					if (!xColumn.IsElementEmpty(i) && !yColumn.IsElementEmpty(i) && !zColumn.IsElementEmpty(i))
					{
						_xBoundaries?.Add(xColumn, i);
						_yBoundaries?.Add(yColumn, i);
						_zBoundaries?.Add(zColumn, i);
					}
				}

				// now the cached data are valid
				_isCachedDataValidX = null != _xBoundaries;
				_isCachedDataValidY = null != _yBoundaries;
				_isCachedDataValidZ = null != _zBoundaries;

				// now when the cached data are valid, we can reenable the events
			}
			finally
			{
				suspendTokenX?.Resume();
				suspendTokenY?.Resume();
				suspendTokenZ?.Resume();
			}
		}

		private class MyPlotData
		{
			private IReadableColumn _xColumn;
			private IReadableColumn _yColumm;
			private IReadableColumn _zColumm;

			public MyPlotData(IReadableColumn xc, IReadableColumn yc, IReadableColumn zc)
			{
				_xColumn = xc;
				_yColumm = yc;
				_zColumm = zc;
			}

			public AltaxoVariant GetXPhysical(int originalRowIndex)
			{
				return _xColumn[originalRowIndex];
			}

			public AltaxoVariant GetYPhysical(int originalRowIndex)
			{
				return _yColumm[originalRowIndex];
			}

			public AltaxoVariant GetZPhysical(int originalRowIndex)
			{
				return _zColumm[originalRowIndex];
			}
		}

		/// <summary>
		/// This will create a point list out of the data, which can be used to plot the data. In order to create this list,
		/// the function must have knowledge how to calculate the points out of the data. This will be done
		/// by a function provided by the calling function.
		/// </summary>
		/// <param name="layer">The plot layer.</param>
		/// <returns>An array of plot points in layer coordinates.</returns>
		public Processed3DPlotData GetRangesAndPoints(
			IPlotArea layer)
		{
			const double MaxRelativeValue = 1E2;

			Altaxo.Data.IReadableColumn xColumn = this.XColumn;
			Altaxo.Data.IReadableColumn yColumn = this.YColumn;
			Altaxo.Data.IReadableColumn zColumn = this.ZColumn;

			if (null == xColumn || null == yColumn || null == zColumn)
				return null; // this plotitem is only for x and y double columns

			var result = new Processed3DPlotData();
			MyPlotData myPlotData = new MyPlotData(xColumn, yColumn, zColumn);
			result.XPhysicalAccessor = new IndexedPhysicalValueAccessor(myPlotData.GetXPhysical);
			result.YPhysicalAccessor = new IndexedPhysicalValueAccessor(myPlotData.GetYPhysical);
			result.ZPhysicalAccessor = new IndexedPhysicalValueAccessor(myPlotData.GetZPhysical);
			PlotRangeList rangeList = null;

			// allocate an array PointF to hold the line points
			// _tlsBufferedPlotData is a static buffer that is allocated per thread
			// and thus is only used temporary here in this routine
			if (null == _tlsBufferedPlotData)
				_tlsBufferedPlotData = new List<PointD3D>();
			else
				_tlsBufferedPlotData.Clear();

			// Fill the array with values
			// only the points where x and y are not NaNs are plotted!

			bool bInPlotSpace = true;
			int rangeStart = 0;
			int rangeOffset = 0;
			rangeList = new PlotRangeList();
			result.RangeList = rangeList;

			Scale xAxis = layer.XAxis;
			Scale yAxis = layer.YAxis;
			Scale zAxis = layer.ZAxis;
			G3DCoordinateSystem coordsys = layer.CoordinateSystem;

			int maxRowIndex = GetMaximumRowIndexFromDataColumns();
			int plotArrayIdx = 0;
			foreach (int dataRowIdx in _dataRowSelection.GetSelectedRowIndicesFromTo(0, maxRowIndex, _dataTable?.Document?.DataColumns, maxRowIndex))
			{
				if (xColumn.IsElementEmpty(dataRowIdx) || yColumn.IsElementEmpty(dataRowIdx) || zColumn.IsElementEmpty(dataRowIdx))
				{
					if (!bInPlotSpace)
					{
						bInPlotSpace = true;
						rangeList.Add(new PlotRange(rangeStart, plotArrayIdx, rangeOffset));
					}
					continue;
				}

				double x_rel, y_rel, z_rel;
				PointD3D coord;

				x_rel = xAxis.PhysicalVariantToNormal(xColumn[dataRowIdx]);
				y_rel = yAxis.PhysicalVariantToNormal(yColumn[dataRowIdx]);
				z_rel = zAxis.PhysicalVariantToNormal(zColumn[dataRowIdx]);

				// chop relative values to an range of about -+ 10^6
				if (x_rel > MaxRelativeValue)
					x_rel = MaxRelativeValue;
				if (x_rel < -MaxRelativeValue)
					x_rel = -MaxRelativeValue;
				if (y_rel > MaxRelativeValue)
					y_rel = MaxRelativeValue;
				if (y_rel < -MaxRelativeValue)
					y_rel = -MaxRelativeValue;
				if (z_rel > MaxRelativeValue)
					z_rel = MaxRelativeValue;
				if (z_rel < -MaxRelativeValue)
					z_rel = -MaxRelativeValue;

				// after the conversion to relative coordinates it is possible
				// that with the choosen axis the point is undefined
				// (for instance negative values on a logarithmic axis)
				// in this case the returned value is NaN
				if (coordsys.LogicalToLayerCoordinates(new Logical3D(x_rel, y_rel, z_rel), out coord))
				{
					if (bInPlotSpace)
					{
						bInPlotSpace = false;
						rangeStart = plotArrayIdx;
						rangeOffset = dataRowIdx - plotArrayIdx;
					}
					_tlsBufferedPlotData.Add(coord);
					plotArrayIdx++;
				}
				else
				{
					if (!bInPlotSpace)
					{
						bInPlotSpace = true;
						rangeList.Add(new PlotRange(rangeStart, plotArrayIdx, rangeOffset));
					}
				}
			} // end for
			if (!bInPlotSpace)
			{
				bInPlotSpace = true;
				rangeList.Add(new PlotRange(rangeStart, plotArrayIdx, rangeOffset)); // add the last range
			}

			result.PlotPointsInAbsoluteLayerCoordinates = _tlsBufferedPlotData.ToArray();

			return result;
		}

		#region Change event handling

		protected override bool HandleHighPriorityChildChangeCases(object sender, ref EventArgs e)
		{
			if (object.ReferenceEquals(sender, _xColumn) || object.ReferenceEquals(sender, _yColumn) || object.ReferenceEquals(sender, _zColumn))
				_isCachedDataValidX = _isCachedDataValidY = _isCachedDataValidZ = false;

			// If it is BoundaryChangedEventArgs, we have to set a flag for which boundary is affected
			var eAsBCEA = e as BoundariesChangedEventArgs;
			if (null != eAsBCEA)
			{
				if (object.ReferenceEquals(sender, _xBoundaries))
				{
					eAsBCEA.SetXBoundaryChangedFlag();
				}
				else if (object.ReferenceEquals(sender, _yBoundaries))
				{
					eAsBCEA.SetYBoundaryChangedFlag();
				}
				else if (object.ReferenceEquals(sender, _zBoundaries))
				{
					eAsBCEA.SetZBoundaryChangedFlag();
				}
			}

			return base.HandleHighPriorityChildChangeCases(sender, ref e);
		}

		/// <summary>
		/// Looks whether one of data data columns have changed their data. If this is the case, we must recalculate the boundaries,
		/// and trigger the boundary changed event if one of the boundaries have changed.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data. On return, you can provided transformed event args by this parameter.</param>
		/// <returns>
		/// The return value of the base handling function
		/// </returns>
		protected override void OnChanged(EventArgs e)
		{
			if (!_isCachedDataValidX || !_isCachedDataValidY || !_isCachedDataValidZ)
				CalculateCachedData(); // Calculates cached data -> If boundaries changed, this will trigger a boundary changed event

			base.OnChanged(e);
		}

		#endregion Change event handling
	}
}
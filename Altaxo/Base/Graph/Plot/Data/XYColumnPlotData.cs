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
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Main;
using Altaxo.Serialization;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Altaxo.Graph.Plot.Data
{
	using Altaxo.Data.Selections;
	using Gdi.Plot.Data;

	/// <summary>
	/// Summary description for XYColumnPlotData.
	/// </summary>
	public class XYColumnPlotData
		:
		Main.SuspendableDocumentNodeWithSetOfEventArgs,
		IColumnPlotData,
		System.ICloneable
	{
		/// <summary>Holds a reference to the underlying data table. If the Empty property of the proxy is null, the underlying table must be determined from the column proxies.</summary>
		protected DataTableProxy _dataTable;

		/// <summary>The group number of the data columns. All data columns should have this group number. Data columns having other group numbers will be marked.</summary>
		protected int _groupNumber;

		protected Altaxo.Data.IReadableColumnProxy _xColumn; // the X-Column
		protected Altaxo.Data.IReadableColumnProxy _yColumn; // the Y-Column

		/// <summary>This is here only for backward deserialization compatibility. Do not use it.</summary>
		private Altaxo.Data.IReadableColumn _deprecatedLabelColumn; // the label column

		/// <summary>
		/// The selection of data rows to be plotted.
		/// </summary>
		protected IRowSelection _dataRowSelection;

		// cached or temporary data
		protected IPhysicalBoundaries _xBoundaries;

		protected IPhysicalBoundaries _yBoundaries;

		/// <summary>List of plot points that is allocated once per thread (as thread local storage variable).</summary>
		[ThreadStatic]
		[NonSerialized]
		protected static List<PointF> _tlsBufferedPlotData;

		/// <summary>
		/// One more that the index to the last valid pair of plot data.
		/// </summary>
		protected int _pointCount;

		protected bool _isCachedDataValidX = false;
		protected bool _isCachedDataValidY = false;

		#region Serialization

		#region Xml 0 und 1

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYColumnPlotData", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYColumnPlotData", 1)] // by mistake the data of version 0 and 1 are identical
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new ApplicationException("Calling a deprecated serialization handler for XYColumnPlotData");
				/*
				XYColumnPlotData s = (XYColumnPlotData)obj;

				if(s.m_xColumn is Main.IDocumentNode && !s.Equals(((Main.IDocumentNode)s.m_xColumn).ParentObject))
				{
					info.AddValue("XColumn",Main.DocumentPath.GetAbsolutePath((Main.IDocumentNode)s.m_xColumn));
				}
				else
				{
					info.AddValue("XColumn",s.m_xColumn);
				}

				if(s.m_yColumn is Main.IDocumentNode && !s.Equals(((Main.IDocumentNode)s.m_yColumn).ParentObject))
				{
					info.AddValue("YColumn",Main.DocumentPath.GetAbsolutePath((Main.IDocumentNode)s.m_yColumn));
				}
				else
				{
					info.AddValue("YColumn",s.m_yColumn);
				}

				info.AddValue("XBoundaries",s.m_xBoundaries);
				info.AddValue("YBoundaries",s.m_yBoundaries);
				*/
			}

			protected Main.AbsoluteDocumentPath _xColumn = null;
			protected Main.AbsoluteDocumentPath _yColumn = null;

			protected XYColumnPlotData _plotAssociation = null;

			public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				bool bNeedsCallback = false;
				XYColumnPlotData s = null != o ? (XYColumnPlotData)o : new XYColumnPlotData(info);

				object xColumn = info.GetValue("XColumn", s);
				object yColumn = info.GetValue("YColumn", s);

				if (xColumn is Altaxo.Data.IReadableColumn)
					s.XColumn = (Altaxo.Data.IReadableColumn)xColumn;
				else if (xColumn is Main.AbsoluteDocumentPath)
					bNeedsCallback = true;

				if (yColumn is Altaxo.Data.IReadableColumn)
					s.YColumn = (Altaxo.Data.IReadableColumn)yColumn;
				else if (yColumn is Main.AbsoluteDocumentPath)
					bNeedsCallback = true;

				s._xBoundaries = (IPhysicalBoundaries)info.GetValue("XBoundaries", s);
				if (null != s._xBoundaries)
					s._xBoundaries.ParentObject = s;

				s._yBoundaries = (IPhysicalBoundaries)info.GetValue("YBoundaries", s);
				if (null != s._yBoundaries)
					s._yBoundaries.ParentObject = s;

				if (bNeedsCallback)
				{
					XmlSerializationSurrogate0 surr = new XmlSerializationSurrogate0();
					surr._xColumn = xColumn as Main.AbsoluteDocumentPath;
					surr._yColumn = yColumn as Main.AbsoluteDocumentPath;
					surr._plotAssociation = s;

					info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.EhDeserializationFinished);
				}
				return s;
			}

			private void EhDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, Main.IDocumentNode documentRoot, bool isFinallyCall)
			{
				bool bAllResolved = true;

				if (this._xColumn != null)
				{
					object xColumn = Main.AbsoluteDocumentPath.GetObject(this._xColumn, this._plotAssociation, documentRoot);
					bAllResolved &= (null != xColumn);
					if (xColumn is Altaxo.Data.IReadableColumn)
						_plotAssociation.XColumn = (Altaxo.Data.IReadableColumn)xColumn;
				}

				if (this._yColumn != null)
				{
					object yColumn = Main.AbsoluteDocumentPath.GetObject(this._yColumn, this._plotAssociation, documentRoot);
					bAllResolved &= (null != yColumn);
					if (yColumn is Altaxo.Data.IReadableColumn)
						_plotAssociation.YColumn = (Altaxo.Data.IReadableColumn)yColumn;
				}

				if (bAllResolved)
					info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.EhDeserializationFinished);
			}
		}

		#endregion Xml 0 und 1

		#region Xml2

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYColumnPlotData", 2)]
		private class XmlSerializationSurrogate2 : XmlSerializationSurrogate0
		{
			public override void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new ApplicationException("Calling a deprecated serialization handler for XYColumnPlotData");
				/*
				XYColumnPlotData s = (XYColumnPlotData)obj;
				base.Serialize(obj,info);

				// -----------------------Added in version 2 ------------------------

				// the rest of the plot data is stored in kind of a array
				// so it should be easy to add more data here, and only data that are valid
				// are been serialized
				int nElements = s.LabelColumn==null ? 0 : 1;
				info.CreateArray("OptionalData",nElements);
				if(null!=s.LabelColumn)
				{
					if(s.m_LabelColumn is Main.IDocumentNode && !s.Equals(((Main.IDocumentNode)s.m_LabelColumn).ParentObject))
						info.AddValue("LabelColumn",Main.DocumentPath.GetAbsolutePath((Main.IDocumentNode)s.m_LabelColumn));
					else
						info.AddValue("LabelColumn",s.m_LabelColumn);
				}
				info.CommitArray(); // end of array OptionalData
			*/
			}

			public override object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				XYColumnPlotData s = null != o ? (XYColumnPlotData)o : new XYColumnPlotData(info);
				base.Deserialize(s, info, parent);

				bool bNeedsCallback = false;

				object labelColumn = null;

				int nOptionalData = info.OpenArray();
				{
					if (nOptionalData == 1)
					{
						string keystring = info.GetNodeName();
						labelColumn = info.GetValue("LabelColumn", s);

						if (labelColumn is Altaxo.Data.IReadableColumn)
							s._deprecatedLabelColumn = (Altaxo.Data.IReadableColumn)labelColumn;
						else if (labelColumn is Main.AbsoluteDocumentPath)
							bNeedsCallback = true;
					}
				}
				info.CloseArray(nOptionalData);

				if (bNeedsCallback)
				{
					XmlSerializationSurrogate2 surr = new XmlSerializationSurrogate2();
					surr._labelColumn = labelColumn as Main.AbsoluteDocumentPath;
					surr._plotAssociation = s;

					info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.EhDeserializationFinished2);
				}

				return s;
			}

			private Main.AbsoluteDocumentPath _labelColumn = null;

			private void EhDeserializationFinished2(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, Main.IDocumentNode documentRoot, bool isFinallyCall)
			{
				bool bAllResolved = true;

				if (this._labelColumn != null)
				{
					object labelColumn = Main.AbsoluteDocumentPath.GetObject(this._labelColumn, this._plotAssociation, documentRoot);
					bAllResolved &= (null != labelColumn);
					if (labelColumn is Altaxo.Data.IReadableColumn)
						_plotAssociation._deprecatedLabelColumn = (Altaxo.Data.IReadableColumn)labelColumn;
				}

				if (bAllResolved)
					info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.EhDeserializationFinished2);
			}
		}

		#endregion Xml2

		#region Xml 3

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYColumnPlotData", 3)]
		private class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Serialization of old version is not allowed");
				/*

				XYColumnPlotData s = (XYColumnPlotData)obj;

				info.AddValue("XColumn", s._xColumn);
				info.AddValue("YColumn", s._yColumn);

				info.AddValue("XBoundaries", s._xBoundaries);
				info.AddValue("YBoundaries", s._yBoundaries);

				*/
			}

			public virtual XYColumnPlotData SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				XYColumnPlotData s = null != o ? (XYColumnPlotData)o : new XYColumnPlotData(info);

				s._xColumn = (IReadableColumnProxy)info.GetValue("XColumn", s);
				s._yColumn = (IReadableColumnProxy)info.GetValue("YColumn", s);

				s._xBoundaries = (IPhysicalBoundaries)info.GetValue("XBoundaries", s);
				s._yBoundaries = (IPhysicalBoundaries)info.GetValue("YBoundaries", s);

				return s;
			}

			public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				XYColumnPlotData s = SDeserialize(o, info, parent);
				CreateEventChain(s);
				return s;
			}

			public virtual void CreateEventChain(XYColumnPlotData s)
			{
				if (null != s._xColumn)
					s._xColumn.ParentObject = s;
				if (null != s._yColumn)
					s._yColumn.ParentObject = s;

				if (null != s._xBoundaries)
					s._xBoundaries.ParentObject = s;

				if (null != s._yBoundaries)
					s._yBoundaries.ParentObject = s;
			}
		}

		#endregion Xml 3

		#region Xml 4 und 5

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYColumnPlotData", 4)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Plot.Data.XYColumnPlotData", 5)]
		private class XmlSerializationSurrogate4 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Serialization of old version is not allowed");

				/*
				XYColumnPlotData s = (XYColumnPlotData)obj;

				info.AddValue("XColumn", s._xColumn);
				info.AddValue("YColumn", s._yColumn);

				info.AddValue("XBoundaries", s._xBoundaries);
				info.AddValue("YBoundaries", s._yBoundaries);

				info.AddValue("RangeStart", s._plotRangeStart);
				info.AddValue("RangeLength", s._plotRangeLength);
				*/
			}

			public virtual XYColumnPlotData SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				XYColumnPlotData s = null != o ? (XYColumnPlotData)o : new XYColumnPlotData(info);

				s._xColumn = (IReadableColumnProxy)info.GetValue("XColumn", s);
				if (null != s._xColumn) s._xColumn.ParentObject = s;

				s._yColumn = (IReadableColumnProxy)info.GetValue("YColumn", s);
				if (null != s._yColumn) s._yColumn.ParentObject = s;

				s._xBoundaries = (IPhysicalBoundaries)info.GetValue("XBoundaries", s);
				if (null != s._xBoundaries) s._xBoundaries.ParentObject = s;

				s._yBoundaries = (IPhysicalBoundaries)info.GetValue("YBoundaries", s);
				if (null != s._yBoundaries) s._yBoundaries.ParentObject = s;

				int rangeStart = info.GetInt32("RangeStart");
				int rangeLength = info.GetInt32("RangeLength");

				if (rangeStart < 0 || rangeLength != int.MaxValue)
					s.ChildSetMember(ref s._dataRowSelection, RangeOfRowIndices.FromStartAndCount(rangeStart, rangeLength));

				return s;
			}

			public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				XYColumnPlotData s = SDeserialize(o, info, parent);
				CreateEventChain(s);
				return s;
			}

			public virtual void CreateEventChain(XYColumnPlotData s)
			{
				if (null != s._xColumn)
					s._xColumn.ParentObject = s;
				if (null != s._yColumn)
					s._yColumn.ParentObject = s;

				if (null != s._xBoundaries)
					s._xBoundaries.ParentObject = s;

				if (null != s._yBoundaries)
					s._yBoundaries.ParentObject = s;
			}
		}

		#endregion Xml 4 und 5

		#region Xml 6

		/// <summary>
		/// 2016-09-25 Added DataTable and GroupNumber. Changed from RangeStart and RangeLength to RowSelection
		/// </summary>
		/// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYColumnPlotData), 6)]
		private class XmlSerializationSurrogate6 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				XYColumnPlotData s = (XYColumnPlotData)obj;

				info.AddValue("DataTable", s._dataTable);
				info.AddValue("GroupNumber", s._groupNumber);
				info.AddValue("RowSelection", s._dataRowSelection);

				info.AddValue("XColumn", s._xColumn);
				info.AddValue("YColumn", s._yColumn);

				info.AddValue("XBoundaries", s._xBoundaries);
				info.AddValue("YBoundaries", s._yBoundaries);
			}

			public virtual XYColumnPlotData SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (XYColumnPlotData)o ?? new XYColumnPlotData(info);

				s._dataTable = (DataTableProxy)info.GetValue("DataTable", s);
				if (null != s._dataTable) s._dataTable.ParentObject = s;

				s._groupNumber = info.GetInt32("GroupNumber");

				s.ChildSetMember(ref s._dataRowSelection, (IRowSelection)info.GetValue("RowSelection", s));

				s._xColumn = (IReadableColumnProxy)info.GetValue("XColumn", s);
				if (null != s._xColumn) s._xColumn.ParentObject = s;

				s._yColumn = (IReadableColumnProxy)info.GetValue("YColumn", s);
				if (null != s._yColumn) s._yColumn.ParentObject = s;

				s._xBoundaries = (IPhysicalBoundaries)info.GetValue("XBoundaries", s);
				if (null != s._xBoundaries) s._xBoundaries.ParentObject = s;

				s._yBoundaries = (IPhysicalBoundaries)info.GetValue("YBoundaries", s);
				if (null != s._yBoundaries) s._yBoundaries.ParentObject = s;

				return s;
			}

			public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				XYColumnPlotData s = SDeserialize(o, info, parent);
				CreateEventChain(s);
				return s;
			}

			public virtual void CreateEventChain(XYColumnPlotData s)
			{
				if (null != s._xColumn)
					s._xColumn.ParentObject = s;
				if (null != s._yColumn)
					s._yColumn.ParentObject = s;

				if (null != s._xBoundaries)
					s._xBoundaries.ParentObject = s;

				if (null != s._yBoundaries)
					s._yBoundaries.ParentObject = s;
			}
		}

		#endregion Xml 6

		/// <summary>
		/// Deserialization constructor. Initializes a new instance of the <see cref="XYZColumnPlotData"/> class without any member initialization.
		/// </summary>
		/// <param name="info">The information.</param>
		protected XYColumnPlotData(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
			ChildSetMember(ref _dataRowSelection, new AllRows());
		}

		#endregion Serialization

		public XYColumnPlotData(Altaxo.Data.DataTable dataTable, int groupNumber, Altaxo.Data.IReadableColumn xColumn, Altaxo.Data.IReadableColumn yColumn)
		{
			DataTable = dataTable;
			ChildSetMember(ref _dataRowSelection, new AllRows());
			_groupNumber = groupNumber;
			XColumn = xColumn;
			YColumn = yColumn;
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="from">The object to copy from.</param>
		/// <remarks>Only clones the references to the data columns, not the columns itself.</remarks>
		public XYColumnPlotData(XYColumnPlotData from)
		{
			ChildCopyToMember(ref _dataTable, from._dataTable);
			this._groupNumber = from._groupNumber;

			ChildCloneToMember(ref _dataRowSelection, from._dataRowSelection);

			ChildCopyToMember(ref _xColumn, from._xColumn);
			ChildCopyToMember(ref _yColumn, from._yColumn);

			// cached or temporary data

			if (null != from._xBoundaries)
				ChildCopyToMember(ref _xBoundaries, from._xBoundaries);

			if (null != from._yBoundaries)
				ChildCopyToMember(ref _yBoundaries, from._yBoundaries);

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

			if (null != _xBoundaries)
				yield return new Main.DocumentNodeAndName(_xBoundaries, "XBoundaries");

			if (null != _yBoundaries)
				yield return new Main.DocumentNodeAndName(_yBoundaries, "YBoundaries");
		}

		/// <summary>
		/// Creates a cloned copy of this object.
		/// </summary>
		/// <returns>The cloned copy of this object.</returns>
		/// <remarks>The data columns refered by this object are <b>not</b> cloned, only the reference is cloned here.</remarks>
		public object Clone()
		{
			return new XYColumnPlotData(this);
		}

		public DataTable DataTable
		{
			get
			{
				var resultTable = _dataTable?.Document;

				if (null != resultTable)
					return resultTable;

				bool nonUniformTables, nonUniformGroup;
				int? resultGroup;
				IReadableColumnExtensions.GetCommonDataTableAndGroupNumberFromColumns(GetAllColumns(), out nonUniformTables, out resultTable, out nonUniformGroup, out resultGroup);

				if (null != resultTable)
					this.DataTable = resultTable;
				if (null != resultGroup)
					this.GroupNumber = resultGroup.Value;

				return resultTable;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));

				if (object.ReferenceEquals(_dataTable?.Document, value))
					return;

				if (ChildSetMember(ref _dataTable, new DataTableProxy(value)))
				{
					EhSelfChanged(PlotItemDataChangedEventArgs.Empty);
				}
			}
		}

		private IEnumerable<IReadableColumn> GetAllColumns()
		{
			yield return XColumn;
			yield return YColumn;
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
					_isCachedDataValidX = _isCachedDataValidY = false;
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
			IReadableColumn col = this.XColumn;
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
			else if (_xColumn != null)
			{
				return _xColumn.GetName(level) + " (broken)";
			}
			else
			{
				return " (broken)";
			}
		}

		/// <summary>
		/// Gets the name of the y column, depending on the provided level.
		/// </summary>
		/// <param name="level">The level (0..2).</param>
		/// <returns>The name of the y-column, depending on the provided level: 0: only name of the data column. 1: table name and column name. 2: table name, collection, and column name.</returns>
		public string GetYName(int level)
		{
			IReadableColumn col = this.YColumn;
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
			else if (_yColumn != null)
			{
				return _yColumn.GetName(level) + " (broken)";
			}
			else
			{
				return " (broken)";
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
		/// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
		/// to change a plot so that the plot items refer to another table.
		/// </summary>
		/// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
		public void VisitDocumentReferences(DocNodeProxyReporter Report)
		{
			Report(_dataTable, this, "DataTable");
			Report(_xColumn, this, "XColumn");
			Report(_yColumn, this, "YColumn");

			_dataRowSelection.VisitDocumentReferences(Report);
		}

		/// <summary>
		/// Gets the columns used additionally by this style, e.g. the label column for a label plot style, or the error columns for an error bar plot style.
		/// </summary>
		/// <returns>An enumeration of tuples. Each tuple consist of the column name, as it should be used to identify the column in the data dialog. The second item of this
		/// tuple is a function that returns the column proxy for this column, in order to get the underlying column or to set the underlying column.</returns>
		public IEnumerable<(string NameOfColumnGroup, // Name of the column group, e.g. "X-Y-Data"
									IEnumerable<(
										string ColumnLabel, // Column label
										IReadableColumn Column, // the column as it was at the time of this call
										string ColumnName, // the name of the column (last part of the column proxies document path)
										Action<IReadableColumn, DataTable, int> SetColumnAction // action to set the column during Apply of the controller (Arguments are column, table and group number)
										)> columnInfos
								)> GetAdditionallyUsedColumns()
		{
			yield return ("#0: X-Y-Data", GetColumns());
		}

		/// <summary>
		/// Gets the columns used additionally by this style, e.g. the label column for a label plot style, or the error columns for an error bar plot style.
		/// </summary>
		/// <returns>An enumeration of tuples. Each tuple consist of the column name, as it should be used to identify the column in the data dialog. The second item of this
		/// tuple is a function that returns the column proxy for this column, in order to get the underlying column or to set the underlying column.</returns>
		private IEnumerable<(
	string ColumnLabel, // Column label
	IReadableColumn Column, // the column as it was at the time of this call
	string ColumnName, // the name of the column (last part of the column proxies document path)
	Action<IReadableColumn, DataTable, int> // action to set the column during Apply of the controller (Arguments are column, table and group number)
	)> GetColumns()
		{
			yield return ("X", XColumn, _xColumn?.DocumentPath?.LastPartOrDefault, (col, table, group) => { XColumn = col; if (null != table) { DataTable = table; GroupNumber = group; } });
			yield return ("Y", YColumn, _yColumn?.DocumentPath?.LastPartOrDefault, (col, table, group) => { YColumn = col; if (null != table) { DataTable = table; GroupNumber = group; } });
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
					_isCachedDataValidX = _isCachedDataValidY = false; // this influences both x and y boundaries
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
					_isCachedDataValidX = _isCachedDataValidY = false; // this influences both x and y boundaries
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

		/// <summary>
		/// For compatibility with older deserialization versions. Do not use it!
		/// </summary>
		public Altaxo.Data.IReadableColumn LabelColumn
		{
			get
			{
				return _deprecatedLabelColumn;
			}
		}

		public override string ToString()
		{
			return String.Format("{0}(X), {1}(Y)", _xColumn.ToString(), _yColumn.ToString());
		}

		public void CalculateCachedData(IPhysicalBoundaries xBounds, IPhysicalBoundaries yBounds)
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

			if (!_isCachedDataValidX || !_isCachedDataValidY)
				CalculateCachedData();
		}

		/// <summary>
		/// Gets the maximum row index that can be deduced from the data columns. The calculation does <b>not</b> include the DataRowSelection.
		/// </summary>
		/// <returns>The maximum row index that can be deduced from the data columns.</returns>
		public int GetMaximumRowIndexFromDataColumns()
		{
			IReadableColumn xColumn = this.XColumn;
			IReadableColumn yColumn = this.YColumn;

			int maxRowIndex;

			if (xColumn == null || yColumn == null)
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

				// if both columns are indefinite long, we set the length to zero
				if (maxRowIndex == int.MaxValue || maxRowIndex < 0)
					maxRowIndex = 0;
			}

			return maxRowIndex;
		}

		public void CalculateCachedData()
		{
			if (this.IsDisposeInProgress)
				return;

			// we can calulate the bounds only if they are set before
			if (null == _xBoundaries && null == _yBoundaries)
				return;

			ISuspendToken suspendTokenX = null;
			ISuspendToken suspendTokenY = null;

			suspendTokenX = this._xBoundaries?.SuspendGetToken();
			suspendTokenY = this._yBoundaries?.SuspendGetToken();

			try
			{
				this._xBoundaries?.Reset();
				this._yBoundaries?.Reset();

				_pointCount = GetMaximumRowIndexFromDataColumns();

				IReadableColumn xColumn = this.XColumn;
				IReadableColumn yColumn = this.YColumn;

				foreach (var segment in _dataRowSelection.GetSelectedRowIndexSegmentsFromTo(0, _pointCount, _dataTable?.Document?.DataColumns, _pointCount))
				{
					for (int rowIdx = segment.start; rowIdx < segment.endExclusive; ++rowIdx)
					{
						if (!xColumn.IsElementEmpty(rowIdx) && !yColumn.IsElementEmpty(rowIdx))
						{
							_xBoundaries?.Add(xColumn, rowIdx);
							_yBoundaries?.Add(yColumn, rowIdx);
						}
					}
				}

				// now the cached data are valid
				_isCachedDataValidX = null != _xBoundaries;
				_isCachedDataValidY = null != _yBoundaries;

				// now when the cached data are valid, we can reenable the events
			}
			finally
			{
				suspendTokenX?.Resume();
				suspendTokenY?.Resume();
			}
		}

		private class MyPlotData
		{
			private IReadableColumn _xColumn;
			private IReadableColumn _yColumm;

			public MyPlotData(IReadableColumn xc, IReadableColumn yc)
			{
				_xColumn = xc;
				_yColumm = yc;
			}

			public AltaxoVariant GetXPhysical(int originalRowIndex)
			{
				return _xColumn[originalRowIndex];
			}

			public AltaxoVariant GetYPhysical(int originalRowIndex)
			{
				return _yColumm[originalRowIndex];
			}
		}

		/// <summary>
		/// This will create a point list out of the data, which can be used to plot the data. In order to create this list,
		/// the function must have knowledge how to calculate the points out of the data. This will be done
		/// by a function provided by the calling function.
		/// </summary>
		/// <param name="layer">The plot layer.</param>
		/// <returns>An array of plot points in layer coordinates.</returns>
		public Processed2DPlotData GetRangesAndPoints(
			Gdi.IPlotArea layer)
		{
			const double MaxRelativeValue = 1E2;

			Altaxo.Data.IReadableColumn xColumn = this.XColumn;
			Altaxo.Data.IReadableColumn yColumn = this.YColumn;

			if (null == xColumn || null == yColumn)
				return null; // this plotitem is only for x and y double columns

			Processed2DPlotData result = new Processed2DPlotData();
			MyPlotData myPlotData = new MyPlotData(xColumn, yColumn);
			result.XPhysicalAccessor = new IndexedPhysicalValueAccessor(myPlotData.GetXPhysical);
			result.YPhysicalAccessor = new IndexedPhysicalValueAccessor(myPlotData.GetYPhysical);
			PlotRangeList rangeList = null;

			// allocate an array PointF to hold the line points
			// _tlsBufferedPlotData is a static buffer that is allocated per thread
			// and thus is only used temporary here in this routine
			if (null == _tlsBufferedPlotData)
				_tlsBufferedPlotData = new List<PointF>();
			else
				_tlsBufferedPlotData.Clear();

			// Fill the array with values
			// only the points where x and y are not NaNs are plotted!

			bool weAreInsideSegment = false;
			int rangeStart = 0;
			int rangeOffset = 0;
			rangeList = new PlotRangeList();
			result.RangeList = rangeList;

			Scale xAxis = layer.XAxis;
			Scale yAxis = layer.YAxis;
			Gdi.G2DCoordinateSystem coordsys = layer.CoordinateSystem;

			int maxRowIndex = GetMaximumRowIndexFromDataColumns();

			int plotArrayIdx = 0;

			foreach ((int start, int endExclusive) in _dataRowSelection.GetSelectedRowIndexSegmentsFromTo(0, maxRowIndex, _dataTable?.Document?.DataColumns, maxRowIndex))
			{
				for (int dataRowIdx = start; dataRowIdx < endExclusive; ++dataRowIdx)
				{
					if (xColumn.IsElementEmpty(dataRowIdx) || yColumn.IsElementEmpty(dataRowIdx))
					{
						if (weAreInsideSegment)
						{
							weAreInsideSegment = false;
							rangeList.Add(new PlotRange(rangeStart, plotArrayIdx, rangeOffset));
						}
						continue;
					}

					double x_rel, y_rel;
					double xcoord, ycoord;

					x_rel = xAxis.PhysicalVariantToNormal(xColumn[dataRowIdx]);
					y_rel = yAxis.PhysicalVariantToNormal(yColumn[dataRowIdx]);

					// chop relative values to an range of about -+ 10^6
					if (x_rel > MaxRelativeValue)
						x_rel = MaxRelativeValue;
					if (x_rel < -MaxRelativeValue)
						x_rel = -MaxRelativeValue;
					if (y_rel > MaxRelativeValue)
						y_rel = MaxRelativeValue;
					if (y_rel < -MaxRelativeValue)
						y_rel = -MaxRelativeValue;

					// after the conversion to relative coordinates it is possible
					// that with the choosen axis the point is undefined
					// (for instance negative values on a logarithmic axis)
					// in this case the returned value is NaN
					if (coordsys.LogicalToLayerCoordinates(new Logical3D(x_rel, y_rel), out xcoord, out ycoord))
					{
						if (!weAreInsideSegment)
						{
							weAreInsideSegment = true;
							rangeStart = plotArrayIdx;
							rangeOffset = dataRowIdx - plotArrayIdx;
						}
						_tlsBufferedPlotData.Add(new PointF((float)xcoord, (float)ycoord));
						plotArrayIdx++;
					}
					else
					{
						if (weAreInsideSegment)
						{
							weAreInsideSegment = false;
							rangeList.Add(new PlotRange(rangeStart, plotArrayIdx, rangeOffset));
						}
					}
				} // end for
				if (weAreInsideSegment)
				{
					weAreInsideSegment = false;
					rangeList.Add(new PlotRange(rangeStart, plotArrayIdx, rangeOffset));
				}
			} // end foreach

			result.PlotPointsInAbsoluteLayerCoordinates = _tlsBufferedPlotData.ToArray();

			return result;
		}

		#region Change event handling

		/*
		/// <summary>
		/// Used by the data proxies to report changes
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void EhColumnDataChangedEventHandler(object sender, EventArgs e)
		{
			// !!!todo!!! : special case if only data added to a column should
			// be handeld separately to save computing time
			this._isCachedDataValid = false;

			EhSelfChanged(PlotItemDataChangedEventArgs.Empty);
		}
		*/

		protected override bool HandleHighPriorityChildChangeCases(object sender, ref EventArgs e)
		{
			if (object.ReferenceEquals(sender, _xColumn) || object.ReferenceEquals(sender, _yColumn))
				_isCachedDataValidX = _isCachedDataValidY = false;

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
			}

			return base.HandleHighPriorityChildChangeCases(sender, ref e);
		}

		/*
		/// <summary>
		/// Looks whether one of data data columns have changed their data. If this is the case, we must recalculate the boundaries,
		/// and trigger the boundary changed event if one of the boundaries have changed.
		/// </summary>
		/// <param name="sender">The sender of the event args, usually a child of this object.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data. On return, you can provided transformed event args by this parameter.</param>
		/// <returns>
		/// The return value of the base handling function
		/// </returns>
		protected override bool HandleLowPriorityChildChangeCases(object sender, ref EventArgs e)
		{
			if (object.ReferenceEquals(sender, _xColumn) || object.ReferenceEquals(sender, _yColumn))
				_isCachedDataValid = false;

			if (!_isCachedDataValid)
				CalculateCachedData(); // Calculates cached data -> If boundaries changed, this will trigger a boundary changed event

			return base.HandleLowPriorityChildChangeCases(sender, ref e);
		}
		*/

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
			if (!_isCachedDataValidX || !_isCachedDataValidY)
				CalculateCachedData(); // Calculates cached data -> If boundaries changed, this will trigger a boundary changed event

			base.OnChanged(e);
		}

		#endregion Change event handling
	}
}
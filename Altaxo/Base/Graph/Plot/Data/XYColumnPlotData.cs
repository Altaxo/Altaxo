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
	using Gdi.Plot.Data;

	/// <summary>
	/// Summary description for XYColumnPlotData.
	/// </summary>
	public class XYColumnPlotData
		:
		Main.SuspendableDocumentNodeWithSetOfEventArgs,
		System.ICloneable
	{
		protected Altaxo.Data.IReadableColumnProxy _xColumn; // the X-Column
		protected Altaxo.Data.IReadableColumnProxy _yColumn; // the Y-Column

		/// <summary>This is here only for backward deserialization compatibility. Do not use it.</summary>
		private Altaxo.Data.IReadableColumn _deprecatedLabelColumn; // the label column

		protected int _plotRangeStart = 0;
		protected int _plotRangeLength = int.MaxValue;

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

		protected bool _isCachedDataValid = false;

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
				XYColumnPlotData s = null != o ? (XYColumnPlotData)o : new XYColumnPlotData();

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
				XYColumnPlotData s = null != o ? (XYColumnPlotData)o : new XYColumnPlotData();
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
				XYColumnPlotData s = (XYColumnPlotData)obj;

				info.AddValue("XColumn", s._xColumn);
				info.AddValue("YColumn", s._yColumn);

				info.AddValue("XBoundaries", s._xBoundaries);
				info.AddValue("YBoundaries", s._yBoundaries);
			}

			public virtual XYColumnPlotData SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				XYColumnPlotData s = null != o ? (XYColumnPlotData)o : new XYColumnPlotData();

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
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYColumnPlotData), 5)]
		private class XmlSerializationSurrogate4 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				XYColumnPlotData s = (XYColumnPlotData)obj;

				info.AddValue("XColumn", s._xColumn);
				info.AddValue("YColumn", s._yColumn);

				info.AddValue("XBoundaries", s._xBoundaries);
				info.AddValue("YBoundaries", s._yBoundaries);

				info.AddValue("RangeStart", s._plotRangeStart);
				info.AddValue("RangeLength", s._plotRangeLength);
			}

			public virtual XYColumnPlotData SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				XYColumnPlotData s = null != o ? (XYColumnPlotData)o : new XYColumnPlotData();

				s._xColumn = (IReadableColumnProxy)info.GetValue("XColumn", s);
				if (null != s._xColumn) s._xColumn.ParentObject = s;

				s._yColumn = (IReadableColumnProxy)info.GetValue("YColumn", s);
				if (null != s._yColumn) s._yColumn.ParentObject = s;

				s._xBoundaries = (IPhysicalBoundaries)info.GetValue("XBoundaries", s);
				if (null != s._xBoundaries) s._xBoundaries.ParentObject = s;

				s._yBoundaries = (IPhysicalBoundaries)info.GetValue("YBoundaries", s);
				if (null != s._yBoundaries) s._yBoundaries.ParentObject = s;

				s._plotRangeStart = info.GetInt32("RangeStart");
				s._plotRangeLength = info.GetInt32("RangeLength");

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

		#endregion Serialization

		public XYColumnPlotData(Altaxo.Data.IReadableColumn xColumn, Altaxo.Data.IReadableColumn yColumn)
		{
			XColumn = xColumn;
			YColumn = yColumn;

			//this.SetXBoundsFromTemplate( new FiniteNumericalBoundaries() );
			//this.SetYBoundsFromTemplate( new FiniteNumericalBoundaries() );
		}

		protected XYColumnPlotData()
		{
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="from">The object to copy from.</param>
		/// <remarks>Only clones the references to the data columns, not the columns itself.</remarks>
		public XYColumnPlotData(XYColumnPlotData from)
		{
			ChildCopyToMember(ref _xColumn, from._xColumn);
			ChildCopyToMember(ref _yColumn, from._yColumn);

			this._plotRangeStart = from._plotRangeStart;
			this._plotRangeLength = from._plotRangeLength;

			// cached or temporary data

			if (null != from._xBoundaries)
				ChildCopyToMember(ref _xBoundaries, from._xBoundaries);

			if (null != from._yBoundaries)
				ChildCopyToMember(ref _yBoundaries, _yBoundaries);

			this._pointCount = from._pointCount;
			this._isCachedDataValid = from._isCachedDataValid;
		}

		protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
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

		public void MergeXBoundsInto(IPhysicalBoundaries pb)
		{
			if (null == _xBoundaries || pb.GetType() != _xBoundaries.GetType())
				this.SetXBoundsFromTemplate(pb);

			if (!this._isCachedDataValid)
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

			if (!this._isCachedDataValid)
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
				ChildCopyToMember(ref _xBoundaries, val);

				_isCachedDataValid = false;

				EhSelfChanged(EventArgs.Empty);
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
					_isCachedDataValid = false;

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
			Report(_xColumn, this, "XColumn");
			Report(_yColumn, this, "YColumn");
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
				if (!this._isCachedDataValid)
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
					_isCachedDataValid = false;
					EhSelfChanged(PlotItemDataChangedEventArgs.Empty);
				}
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
					_isCachedDataValid = false;
					EhSelfChanged(PlotItemDataChangedEventArgs.Empty);
				}
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
				_isCachedDataValid = false;
				this.SetXBoundsFromTemplate(xBounds);
			}

			if (_yBoundaries == null || (yBounds != null && _yBoundaries.GetType() != yBounds.GetType()))
			{
				_isCachedDataValid = false;
				this.SetYBoundsFromTemplate(yBounds);
			}

			if (!_isCachedDataValid)
				CalculateCachedData();
		}

		public void CalculateCachedData()
		{
			if (this.IsDisposeInProgress)
				return;

			// we can calulate the bounds only if they are set before
			if (null == _xBoundaries || null == _yBoundaries)
				return;

			using (var suspendTokenX = this._xBoundaries.SuspendGetToken())
			{
				using (var suspendTokenY = this._yBoundaries.SuspendGetToken())
				{
					this._xBoundaries.Reset();
					this._yBoundaries.Reset();

					System.Diagnostics.Debug.Assert(_plotRangeStart >= 0);
					System.Diagnostics.Debug.Assert(_plotRangeLength >= 0);

					_pointCount = _plotRangeLength == int.MaxValue ? int.MaxValue : _plotRangeStart + _plotRangeLength;

					IReadableColumn xColumn = this.XColumn;
					IReadableColumn yColumn = this.YColumn;

					if (xColumn == null || yColumn == null)
					{
						_pointCount = 0;
					}
					else
					{
						if (xColumn is IDefinedCount)
							_pointCount = System.Math.Min(_pointCount, ((IDefinedCount)xColumn).Count);
						if (yColumn is IDefinedCount)
							_pointCount = System.Math.Min(_pointCount, ((IDefinedCount)yColumn).Count);

						// if both columns are indefinite long, we set the length to zero
						if (_pointCount == int.MaxValue || _pointCount < 0)
							_pointCount = 0;

						for (int i = _plotRangeStart; i < _pointCount; i++)
						{
							if (!xColumn.IsElementEmpty(i) && !yColumn.IsElementEmpty(i))
							{
								bool x_added = this._xBoundaries.Add(xColumn, i);
								bool y_added = this._yBoundaries.Add(yColumn, i);
							}
						}
					}

					// now the cached data are valid
					_isCachedDataValid = true;

					// now when the cached data are valid, we can reenable the events

					suspendTokenY.Resume();
				}
				suspendTokenX.Resume();
			}
		}

		/// <summary>
		/// Number of the first point to plot.
		/// </summary>
		public int PlotRangeStart
		{
			get { return this._plotRangeStart; }
			set
			{
				var oldValue = _plotRangeStart;
				_plotRangeStart = value < 0 ? 0 : value;

				if (_plotRangeStart != oldValue)
				{
					_isCachedDataValid = false;
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Length of the plot range. The last point of the plot range is PlotRangeStart+PlotRangeLength-1.
		/// This is not the number of plottable points!
		/// </summary>
		public int PlotRangeLength
		{
			get
			{
				return this._plotRangeLength;
			}
			set
			{
				var oldValue = _plotRangeLength;
				_plotRangeLength = value < 0 ? 0 : value;

				if (_plotRangeLength != oldValue)
				{
					_isCachedDataValid = false;
					EhSelfChanged(EventArgs.Empty);
				}
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

			int i, j;

			bool bInPlotSpace = true;
			int rangeStart = 0;
			int rangeOffset = 0;
			rangeList = new PlotRangeList();
			result.RangeList = rangeList;

			Scale xAxis = layer.XAxis;
			Scale yAxis = layer.YAxis;
			Gdi.G2DCoordinateSystem coordsys = layer.CoordinateSystem;

			int len = this.PlotRangeEnd;
			for (i = this.PlotRangeStart, j = 0; i < len; i++)
			{
				if (xColumn.IsElementEmpty(i) || yColumn.IsElementEmpty(i))
				{
					if (!bInPlotSpace)
					{
						bInPlotSpace = true;
						rangeList.Add(new PlotRange(rangeStart, j, rangeOffset));
					}
					continue;
				}

				double x_rel, y_rel;
				double xcoord, ycoord;

				x_rel = xAxis.PhysicalVariantToNormal(xColumn[i]);
				y_rel = yAxis.PhysicalVariantToNormal(yColumn[i]);

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
					if (bInPlotSpace)
					{
						bInPlotSpace = false;
						rangeStart = j;
						rangeOffset = i - j;
					}
					_tlsBufferedPlotData.Add(new PointF((float)xcoord, (float)ycoord));
					j++;
				}
				else
				{
					if (!bInPlotSpace)
					{
						bInPlotSpace = true;
						rangeList.Add(new PlotRange(rangeStart, j, rangeOffset));
					}
				}
			} // end for
			if (!bInPlotSpace)
			{
				bInPlotSpace = true;
				rangeList.Add(new PlotRange(rangeStart, j, rangeOffset)); // add the last range
			}

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
				_isCachedDataValid = false;

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
		/// <param name="sender">The sender of the event args, usually a child of this object.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data. On return, you can provided transformed event args by this parameter.</param>
		/// <returns>
		/// The return value of the base handling function
		/// </returns>
		protected override void OnChanged(EventArgs e)
		{
			if (!_isCachedDataValid)
				CalculateCachedData(); // Calculates cached data -> If boundaries changed, this will trigger a boundary changed event

			base.OnChanged(e);
		}

		#endregion Change event handling
	}
}
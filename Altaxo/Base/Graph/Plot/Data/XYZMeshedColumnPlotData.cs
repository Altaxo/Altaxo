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

using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Main;
using System;
using System.Collections.Generic;

namespace Altaxo.Graph.Plot.Data
{
	/// <summary>
	/// Summary description for XYColumnPlotData.
	/// </summary>
	[Serializable]
	public class XYZMeshedColumnPlotData
		:
		Main.SuspendableDocumentNodeWithSetOfEventArgs,
		System.ICloneable
	{
		protected DataTableMatrixProxy _matrixProxy;

		// cached or temporary data
		[NonSerialized]
		protected IPhysicalBoundaries _xBoundaries;

		[NonSerialized]
		protected IPhysicalBoundaries _yBoundaries;

		[NonSerialized]
		protected IPhysicalBoundaries _vBoundaries;

		[NonSerialized]
		protected bool _isCachedDataValid = false;

		/// <summary>
		/// Gets or sets the plot range start. Currently, this value is always 0.
		/// </summary>
		public int PlotRangeStart { get { return 0; } set { } }

		/// <summary>
		/// Gets or sets the plot range length. Currently, this value is always <c>int.MaxValue</c>.
		/// </summary>
		public int PlotRangeLength { get { return int.MaxValue; } set { } }

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYZEquidistantMeshColumnPlotData", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYZMeshedColumnPlotData), 1)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new ApplicationException("Calling a deprecated serialization handler for XYZMeshedColumnPlotData");
				/*
				XYZMeshedColumnPlotData s = (XYZMeshedColumnPlotData)obj;

				if(s.m_XColumn is Main.IDocumentNode && !s.Equals(((Main.IDocumentNode)s.m_XColumn).ParentObject))
				{
					info.AddValue("XColumn",Main.DocumentPath.GetAbsolutePath((Main.IDocumentNode)s.m_XColumn));
				}
				else
				{
					info.AddValue("XColumn",s.m_XColumn);
				}

				if(s.m_YColumn is Main.IDocumentNode && !s.Equals(((Main.IDocumentNode)s.m_YColumn).ParentObject))
				{
					info.AddValue("YColumn",Main.DocumentPath.GetAbsolutePath((Main.IDocumentNode)s.m_YColumn));
				}
				else
				{
					info.AddValue("YColumn",s.m_YColumn);
				}

				info.CreateArray("DataColumns",s.m_DataColumns.Length);
				for(int i=0;i<s.m_DataColumns.Length;i++)
				{
					Altaxo.Data.IReadableColumn col = s.m_DataColumns[i];
					if(col is Main.IDocumentNode && !s.Equals(((Main.IDocumentNode)col).ParentObject))
					{
						info.AddValue("e",Main.DocumentPath.GetAbsolutePath((Main.IDocumentNode)col));
					}
					else
					{
						info.AddValue("e",col);
					}
				}
				info.CommitArray();

				info.AddValue("XBoundaries",s.m_xBoundaries);
				info.AddValue("YBoundaries",s.m_yBoundaries);
				info.AddValue("VBoundaries",s.m_vBoundaries);
				*/
			}

			private Main.AbsoluteDocumentPath _xColumnPath = null;
			private Main.AbsoluteDocumentPath _yColumnPath = null;
			private Main.AbsoluteDocumentPath[] _vColumnPaths = null;

			private IReadableColumnProxy _xColumnProxy = null;
			private IReadableColumnProxy _yColumnProxy = null;
			private IReadableColumnProxy[] _vColumnProxies = null;

			private XYZMeshedColumnPlotData _plotAssociation = null;

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				bool bSurrogateUsed = false;

				XYZMeshedColumnPlotData s = null != o ? (XYZMeshedColumnPlotData)o : new XYZMeshedColumnPlotData();

				XmlSerializationSurrogate0 surr = new XmlSerializationSurrogate0();

#pragma warning disable 618
				s._matrixProxy = DataTableMatrixProxy.CreateEmptyInstance(); // this instance is replaced later in the deserialization callback function and is intended to avoid null reference errors
#pragma warning restore 618

				object deserobj;
				deserobj = info.GetValue("XColumn", s);
				if (deserobj is Main.AbsoluteDocumentPath)
				{
					surr._xColumnPath = (Main.AbsoluteDocumentPath)deserobj;
					bSurrogateUsed = true;
				}
				else
				{
					surr._xColumnProxy = ReadableColumnProxyBase.FromColumn((Altaxo.Data.INumericColumn)deserobj);
				}

				deserobj = info.GetValue("YColumn", s);
				if (deserobj is Main.AbsoluteDocumentPath)
				{
					surr._yColumnPath = (Main.AbsoluteDocumentPath)deserobj;
					bSurrogateUsed = true;
				}
				else
				{
					surr._yColumnProxy = ReadableColumnProxyBase.FromColumn((Altaxo.Data.INumericColumn)deserobj);
				}

				int count = info.OpenArray();
				surr._vColumnPaths = new Main.AbsoluteDocumentPath[count];
				surr._vColumnProxies = new IReadableColumnProxy[count];
				for (int i = 0; i < count; i++)
				{
					deserobj = info.GetValue("e", s);
					if (deserobj is Main.AbsoluteDocumentPath)
					{
						surr._vColumnPaths[i] = (Main.AbsoluteDocumentPath)deserobj;
						bSurrogateUsed = true;
					}
					else
					{
						surr._vColumnProxies[i] = ReadableColumnProxyBase.FromColumn((Altaxo.Data.IReadableColumn)deserobj);
					}
				}
				info.CloseArray(count);

				s._xBoundaries = (IPhysicalBoundaries)info.GetValue("XBoundaries", s);
				s._yBoundaries = (IPhysicalBoundaries)info.GetValue("YBoundaries", s);
				s._vBoundaries = (IPhysicalBoundaries)info.GetValue("VBoundaries", s);

				s._xBoundaries.ParentObject = s;
				s._yBoundaries.ParentObject = s;
				s._vBoundaries.ParentObject = s;

				if (bSurrogateUsed)
				{
					surr._plotAssociation = s;
					info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.EhDeserializationFinished);
				}

				return s;
			}

			public void EhDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, Main.IDocumentNode documentRoot, bool isFinallyCall)
			{
				bool bAllResolved = true;

				if (this._xColumnPath != null)
				{
					object xColumn = Main.AbsoluteDocumentPath.GetObject(this._xColumnPath, this._plotAssociation, documentRoot);
					bAllResolved &= (null != xColumn);
					if (xColumn is Altaxo.Data.INumericColumn)
					{
						this._xColumnPath = null;
						this._xColumnProxy = ReadableColumnProxyBase.FromColumn((Altaxo.Data.INumericColumn)xColumn);
					}
				}

				if (this._yColumnPath != null)
				{
					object yColumn = Main.AbsoluteDocumentPath.GetObject(this._yColumnPath, this._plotAssociation, documentRoot);
					bAllResolved &= (null != yColumn);
					if (yColumn is Altaxo.Data.INumericColumn)
					{
						this._yColumnPath = null;
						this._yColumnProxy = ReadableColumnProxyBase.FromColumn((Altaxo.Data.INumericColumn)yColumn);
					}
				}

				for (int i = 0; i < this._vColumnPaths.Length; i++)
				{
					if (this._vColumnPaths[i] != null)
					{
						object vColumn = Main.AbsoluteDocumentPath.GetObject(this._vColumnPaths[i], this._plotAssociation, documentRoot);
						bAllResolved &= (null != vColumn);
						if (vColumn is Altaxo.Data.IReadableColumn)
						{
							this._vColumnPaths[i] = null;
							this._vColumnProxies[i] = ReadableColumnProxyBase.FromColumn((Altaxo.Data.IReadableColumn)vColumn);
						}
					}
				}

				if (bAllResolved || isFinallyCall)
				{
					info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.EhDeserializationFinished);
#pragma warning disable 618
					_plotAssociation._matrixProxy = new DataTableMatrixProxy(_xColumnProxy, _yColumnProxy, _vColumnProxies) { ParentObject = _plotAssociation };
#pragma warning restore 618
				}
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYZMeshedColumnPlotData), 2)]
		private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Serialization of old versions not supported.");
				/*

				XYZMeshedColumnPlotData s = (XYZMeshedColumnPlotData)obj;

				info.AddValue("XColumn", s._xColumn);
				info.AddValue("YColumn", s._yColumn);

				info.CreateArray("DataColumns", s._dataColumns.Length);
				for (int i = 0; i < s._dataColumns.Length; i++)
				{
					info.AddValue("e", s._dataColumns[i]);
				}
				info.CommitArray();

				info.AddValue("XBoundaries", s._xBoundaries);
				info.AddValue("YBoundaries", s._yBoundaries);
				info.AddValue("VBoundaries", s._vBoundaries);
				*/
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				XYZMeshedColumnPlotData s = null != o ? (XYZMeshedColumnPlotData)o : new XYZMeshedColumnPlotData();

				var _xColumn = (IReadableColumnProxy)info.GetValue("XColumn", s);
				var _yColumn = (IReadableColumnProxy)info.GetValue("YColumn", s);

				int count = info.OpenArray();
				var _dataColumns = new IReadableColumnProxy[count];
				for (int i = 0; i < count; i++)
				{
					_dataColumns[i] = (IReadableColumnProxy)info.GetValue("e", s);
				}
				info.CloseArray(count);

#pragma warning disable 618
				s._matrixProxy = new DataTableMatrixProxy(_xColumn, _yColumn, _dataColumns);
#pragma warning restore 618

				s._xBoundaries = (IPhysicalBoundaries)info.GetValue("XBoundaries", s);
				s._yBoundaries = (IPhysicalBoundaries)info.GetValue("YBoundaries", s);
				s._vBoundaries = (IPhysicalBoundaries)info.GetValue("VBoundaries", s);

				s._matrixProxy.ParentObject = s;
				s._xBoundaries.ParentObject = s;
				s._yBoundaries.ParentObject = s;
				s._vBoundaries.ParentObject = s;

				s._isCachedDataValid = false;

				return s;
			}
		}

		/// <summary>2014-07-08 using _matrixProxy instead of single proxies for columns</summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYZMeshedColumnPlotData), 3)]
		private class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				XYZMeshedColumnPlotData s = (XYZMeshedColumnPlotData)obj;

				info.AddValue("MatrixProxy", s._matrixProxy);

				info.AddValue("XBoundaries", s._xBoundaries);
				info.AddValue("YBoundaries", s._yBoundaries);
				info.AddValue("VBoundaries", s._vBoundaries);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				XYZMeshedColumnPlotData s = null != o ? (XYZMeshedColumnPlotData)o : new XYZMeshedColumnPlotData();

				s._matrixProxy = (DataTableMatrixProxy)info.GetValue("MatrixProxy", s);

				s._xBoundaries = (IPhysicalBoundaries)info.GetValue("XBoundaries", s);
				s._yBoundaries = (IPhysicalBoundaries)info.GetValue("YBoundaries", s);
				s._vBoundaries = (IPhysicalBoundaries)info.GetValue("VBoundaries", s);

				s._matrixProxy.ParentObject = s;
				s._xBoundaries.ParentObject = s;
				s._yBoundaries.ParentObject = s;
				s._vBoundaries.ParentObject = s;

				s._isCachedDataValid = false;

				return s;
			}
		}

		#endregion Serialization

		/// <summary>
		/// Deserialization constructor.
		/// </summary>
		protected XYZMeshedColumnPlotData()
		{
		}

		public XYZMeshedColumnPlotData(DataTable table, IAscendingIntegerCollection selectedDataRows, IAscendingIntegerCollection selectedDataColumns, IAscendingIntegerCollection selectedPropertyColumns)
		{
			_matrixProxy = new DataTableMatrixProxy(table, selectedDataRows, selectedDataColumns, selectedPropertyColumns) { ParentObject = this };
			this.SetXBoundsFromTemplate(new FiniteNumericalBoundaries());
			this.SetYBoundsFromTemplate(new FiniteNumericalBoundaries());
			this.SetVBoundsFromTemplate(new FiniteNumericalBoundaries());
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="from">The object to copy from.</param>
		/// <remarks>Only clones the references to the data columns, not the columns itself.</remarks>
		public XYZMeshedColumnPlotData(XYZMeshedColumnPlotData from)
		{
			CopyHelper.Copy(ref _matrixProxy, from._matrixProxy);
			_matrixProxy.ParentObject = this;

			this.SetXBoundsFromTemplate(new FiniteNumericalBoundaries());
			this.SetYBoundsFromTemplate(new FiniteNumericalBoundaries());
			this.SetVBoundsFromTemplate(new FiniteNumericalBoundaries());
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _matrixProxy)
				yield return new Main.DocumentNodeAndName(_matrixProxy, "Matrix");

			if (null != _xBoundaries)
				yield return new Main.DocumentNodeAndName(_xBoundaries, "XBoundaries");

			if (null != _yBoundaries)
				yield return new Main.DocumentNodeAndName(_yBoundaries, "YBoundaries");

			if (null != _vBoundaries)
				yield return new Main.DocumentNodeAndName(_vBoundaries, "VBoundaries");
		}

		/// <summary>
		/// Creates a cloned copy of this object.
		/// </summary>
		/// <returns>The cloned copy of this object.</returns>
		/// <remarks>The data columns refered by this object are <b>not</b> cloned, only the reference is cloned here.</remarks>
		public object Clone()
		{
			return new XYZMeshedColumnPlotData(this);
		}

		public DataTableMatrixProxy DataTableMatrix
		{
			get
			{
				return this._matrixProxy;
			}
		}

		public void MergeXBoundsInto(IPhysicalBoundaries pb)
		{
			if (!this._isCachedDataValid)
				this.CalculateCachedData();
			pb.Add(_xBoundaries);
		}

		public void MergeYBoundsInto(IPhysicalBoundaries pb)
		{
			if (!this._isCachedDataValid)
				this.CalculateCachedData();
			pb.Add(_yBoundaries);
		}

		public void MergeVBoundsInto(IPhysicalBoundaries pb)
		{
			if (!this._isCachedDataValid)
				this.CalculateCachedData();
			pb.Add(_vBoundaries);
		}

		public void SetXBoundsFromTemplate(IPhysicalBoundaries val)
		{
			if (null == _xBoundaries || val.GetType() != _xBoundaries.GetType())
			{
				if (ChildCopyToMember(ref _xBoundaries, val))
				{
					this._isCachedDataValid = false;
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public void SetYBoundsFromTemplate(IPhysicalBoundaries val)
		{
			if (null == _yBoundaries || val.GetType() != _yBoundaries.GetType())
			{
				if (ChildCopyToMember(ref _yBoundaries, val))
				{
					this._isCachedDataValid = false;

					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public void SetVBoundsFromTemplate(IPhysicalBoundaries val)
		{
			if (null == _vBoundaries || val.GetType() != _vBoundaries.GetType())
			{
				if (ChildCopyToMember(ref _vBoundaries, val))
				{
					this._isCachedDataValid = false;

					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public int RowCount
		{
			get
			{
				return _matrixProxy.RowCount;
			}
		}

		public int ColumnCount
		{
			get
			{
				return _matrixProxy.ColumnCount;
			}
		}

		public Altaxo.Data.IReadableColumn GetDataColumn(int i)
		{
			return _matrixProxy.GetDataColumnProxy(i).Document;
		}

		public Altaxo.Data.IReadableColumn XColumn
		{
			get
			{
				return _matrixProxy.RowHeaderColumn;
			}
		}

		public Altaxo.Data.IReadableColumn YColumn
		{
			get
			{
				return _matrixProxy.ColumnHeaderColumn;
			}
		}

		public override string ToString()
		{
			var colCount = _matrixProxy.ColumnCount;

			if (colCount > 0)
				return String.Format("PictureData {0}-{1}", _matrixProxy.GetDataColumnProxy(0).GetName(2), _matrixProxy.GetDataColumnProxy(colCount - 1).GetName(2));
			else
				return "Empty (no data)";
		}

		public void CalculateCachedData(IPhysicalBoundaries xBounds, IPhysicalBoundaries yBounds)
		{
			if (_xBoundaries == null || (xBounds != null && _xBoundaries.GetType() != xBounds.GetType()))
				this.SetXBoundsFromTemplate(xBounds);

			if (_yBoundaries == null || (yBounds != null && _yBoundaries.GetType() != yBounds.GetType()))
				this.SetYBoundsFromTemplate(yBounds);

			CalculateCachedData();
		}

		public void CalculateCachedData()
		{
			if (0 == RowCount || 0 == ColumnCount)
				return;

			using (var suspendTokenX = this._xBoundaries.SuspendGetToken())
			{
				using (var suspendTokenY = this._yBoundaries.SuspendGetToken())
				{
					using (var suspendTokenV = this._vBoundaries.SuspendGetToken())
					{
						this._xBoundaries.Reset();
						this._yBoundaries.Reset();
						this._vBoundaries.Reset();

						_matrixProxy.ForEachMatrixElementDo((col, idx) => this._vBoundaries.Add(col, idx));
						_matrixProxy.ForEachRowHeaderElementDo((col, idx) => this._xBoundaries.Add(col, idx));
						_matrixProxy.ForEachColumnHeaderElementDo((col, idx) => this._yBoundaries.Add(col, idx));

						// now the cached data are valid
						_isCachedDataValid = true;

						// now when the cached data are valid, we can reenable the events
						suspendTokenV.Resume();
					}
					suspendTokenY.Resume();
				}
				suspendTokenX.Resume();
			}
		}

		#region Changed event handling

		protected override bool HandleHighPriorityChildChangeCases(object sender, ref EventArgs e)
		{
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
				else if (object.ReferenceEquals(sender, _vBoundaries))
				{
					eAsBCEA.SetVBoundaryChangedFlag();
				}
			}

			if (object.ReferenceEquals(sender, _matrixProxy))
			{
				_isCachedDataValid = false;
			}

			return base.HandleHighPriorityChildChangeCases(sender, ref e);
		}

		#endregion Changed event handling

		/// <summary>
		/// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
		/// to change a plot so that the plot items refer to another table.
		/// </summary>
		/// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
		public void VisitDocumentReferences(DocNodeProxyReporter Report)
		{
			_matrixProxy.VisitDocumentReferences(Report);
		}
	}
} // end name space
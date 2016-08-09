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

using Altaxo.Serialization;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Altaxo.Graph.Graph3D.Plot.Styles
{
	using Altaxo.Data;
	using Altaxo.Main;
	using Drawing;
	using Drawing.D3D;
	using Drawing.D3D.Material;
	using Geometry;
	using Graph.Plot.Data;
	using Graph.Plot.Groups;
	using GraphicsContext;
	using Plot.Data;
	using Plot.Groups;

	public class DropPlotStyle
		:
		Main.SuspendableDocumentNodeWithEventArgs,
		IG3DPlotStyle
	{
		/// <summary>Pen for the drop line.</summary>
		protected PenX3D _pen;

		/// <summary>Pen width 1 for the drop line, either absolute or relative to the size of the scatter symbol.</summary>
		protected RADouble _penWidth1;

		/// <summary>Pen width 2 for the drop line, either absolute or relative to the size of the scatter symbol.</summary>
		protected RADouble _penWidth2;

		/// <summary>Target(s) for the drop line.</summary>
		protected CSPlaneIDList _dropLine;

		/// <summary>Is the material color independent, i.e. not influenced by group styles.</summary>
		protected bool _independentColor;

		/// <summary>A value of 2 skips every other data point, a value of 3 skips 2 out of 3 data points, and so on.</summary>
		protected int? _skipFreq;

		/// <summary>The skip frequency that is currently applied.</summary>
		protected int _cachedSkipFreq;

		/// <summary>The symbol size that is received from another substyle.</summary>
		protected double _cachedSymbolSize = 1;

		// cached values:
		/// <summary>If this function is set, then _symbolSize is ignored and the symbol size is evaluated by this function.</summary>
		[field: NonSerialized]
		protected Func<int, double> _cachedSymbolSizeForIndexFunction;

		/// <summary>If this function is set, the symbol color is determined by calling this function on the index into the data.</summary>
		[field: NonSerialized]
		protected Func<int, Color> _cachedColorForIndexFunction;

		#region Serialization

		/// <summary>
		/// 2016-08-09 initial version.
		/// </summary>
		/// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DropPlotStyle), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				DropPlotStyle s = (DropPlotStyle)obj;

				info.AddValue("DropLine", s._dropLine);
				info.AddValue("Pen", s._pen);
				info.AddValue("PenWidth1", s._penWidth1);
				info.AddValue("PenWidth2", s._penWidth2);
				info.AddValue("IndependentColor", s._independentColor);
				info.AddValue("SkipFreq", s._skipFreq);
			}

			protected virtual DropPlotStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				DropPlotStyle s = null != o ? (DropPlotStyle)o : new DropPlotStyle((Altaxo.Main.Properties.IReadOnlyPropertyBag)null);

				s._dropLine = (CSPlaneIDList)info.GetValue("DropLine", s);
				s._pen = (PenX3D)info.GetValue("Pen", s);
				s._penWidth1 = (RADouble)info.GetValue("PenWidth1", s);
				s._penWidth2 = (RADouble)info.GetValue("PenWidth2", s);
				s._independentColor = info.GetBoolean("IndependentColor");
				s._skipFreq = info.GetNullableInt32("SkipFreq");
				return s;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				DropPlotStyle s = SDeserialize(o, info, parent);

				// restore the cached values
				s.SetCachedValues();

				return s;
			}
		}

		#endregion Serialization

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as DropPlotStyle;
			if (null != from)
			{
				CopyFrom(from, Main.EventFiring.Enabled);
				return true;
			}
			return false;
		}

		public void CopyFrom(DropPlotStyle from, Main.EventFiring eventFiring)
		{
			if (object.ReferenceEquals(this, from))
				return;

			using (var suspendToken = SuspendGetToken())
			{
				if (null == this._dropLine)
					this._dropLine = new CSPlaneIDList();
				else
					this._dropLine.Clear();
				this._dropLine.AddClonedRange(from._dropLine);

				this._pen = from._pen; // immutable
				this._penWidth1 = from._penWidth1;
				this._penWidth2 = from._penWidth2;
				this._independentColor = from._independentColor;
				this._skipFreq = from._skipFreq;

				EhSelfChanged(EventArgs.Empty);

				suspendToken.Resume(eventFiring);
			}
		}

		public DropPlotStyle(DropPlotStyle from)
		{
			CopyFrom(from, Main.EventFiring.Suppressed);
		}

		public DropPlotStyle(CSPlaneID planeID, PenX3D pen)
		{
			if (null == pen)
				throw new ArgumentNullException(nameof(pen));

			this._dropLine = new CSPlaneIDList();
			_dropLine.Add(planeID);

			// Cached values
			SetCachedValues();
		}

		public DropPlotStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
		{
			this._dropLine = new CSPlaneIDList();
			_dropLine.Add(new CSPlaneID(2, 0));

			var color = GraphDocument.GetDefaultPlotColor(context);
			double penWidth = GraphDocument.GetDefaultPenWidth(context);
			_pen = new PenX3D(color, penWidth);

			_penWidth1 = RADouble.NewAbs(penWidth);
			_penWidth2 = RADouble.NewRel(penWidth);
		}

		public object Clone()
		{
			return new DropPlotStyle(this);
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			yield break;
		}

		public bool IsVisible
		{
			get
			{
				return _pen.Material.IsVisible;
			}
		}

		public PenX3D Pen
		{
			get { return this._pen; }
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));

				if (!object.ReferenceEquals(this._pen, value))
				{
					_pen = value;
					SetCachedValues();
					EhSelfChanged(EventArgs.Empty); // Fire Changed event
				}
			}
		}

		public NamedColor Color
		{
			get { return this._pen.Material.Color; }
			set
			{
				Pen = _pen.WithColor(value);
			}
		}

		public bool IndependentColor
		{
			get
			{
				return _independentColor;
			}
			set
			{
				bool oldValue = _independentColor;
				_independentColor = value;
				if (value != oldValue)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public RADouble PenWidth1
		{
			get { return _penWidth1; }
			set
			{
				if (value != _penWidth1)
				{
					_penWidth1 = value;
					EhSelfChanged(EventArgs.Empty); // Fire Changed event
				}
			}
		}

		public RADouble PenWidth2
		{
			get { return _penWidth2; }
			set
			{
				if (value != _penWidth2)
				{
					_penWidth1 = value;
					EhSelfChanged(EventArgs.Empty); // Fire Changed event
				}
			}
		}

		public int? SkipFrequency
		{
			get { return _skipFreq; }
			set
			{
				if (value != _skipFreq)
				{
					_skipFreq = value;
					EhSelfChanged(EventArgs.Empty); // Fire Changed event
				}
			}
		}

		protected void SetCachedValues()
		{
		}

		#region I3DPlotItem Members

		public bool IsColorProvider
		{
			get
			{
				return !this._independentColor;
			}
		}

		public bool IsColorReceiver
		{
			get { return !this._independentColor; }
		}

		public bool IsSymbolSizeProvider
		{
			get
			{
				return false;
			}
		}

		public bool IsSymbolSizeReceiver
		{
			get
			{
				return true;
			}
		}

		#endregion I3DPlotItem Members

		public void Paint(IGraphicsContext3D g, IPlotArea layer, Processed3DPlotData pdata, Processed3DPlotData prevItemData, Processed3DPlotData nextItemData)
		{
			PlotRangeList rangeList = pdata.RangeList;
			var ptArray = pdata.PlotPointsInAbsoluteLayerCoordinates;

			// adjust the skip frequency if it was not set appropriate
			if (_cachedSkipFreq <= 0)
				_skipFreq = 1;

			foreach (CSPlaneID id in _dropLine)
				layer.UpdateCSPlaneID(id);

			// paint the scatter style

			PointD3D pos = PointD3D.Empty;
			VectorD3D diff;

			if (null == _cachedSymbolSizeForIndexFunction && null == _cachedColorForIndexFunction) // using a constant symbol size and constant color
			{
				// update pen widths
				double w1 = _penWidth1.IsRelative ? _penWidth1.Value * _cachedSymbolSize : _penWidth1.Value;
				double w2 = _penWidth2.IsRelative ? _penWidth2.Value * _cachedSymbolSize : _penWidth2.Value;
				_pen = _pen.WithThickness1(w1).WithThickness2(w2);

				int rangeidx = 0;
				PlotRange range = pdata.RangeList[rangeidx];
				for (int j = 0; j < ptArray.Length; j += _cachedSkipFreq)
				{
					// syncronize range
					while (j >= range.UpperBound)
					{
						rangeidx++;
						range = pdata.RangeList[rangeidx];
					}

					Logical3D r3d = layer.GetLogical3D(pdata, j + range.OffsetToOriginal);
					foreach (CSPlaneID id in _dropLine)
					{
						IPolylineD3D isoLine;
						layer.CoordinateSystem.GetIsolineFromPointToPlane(r3d, id, out isoLine);
						g.DrawLine(_pen, isoLine);
					}
				}
			}
			else // using a variable symbol size or variable symbol color
			{
				for (int r = 0; r < rangeList.Count; r++)
				{
					int lower = rangeList[r].LowerBound;
					int upper = rangeList[r].UpperBound;
					int offset = rangeList[r].OffsetToOriginal;
					for (int j = lower; j < upper; j += _cachedSkipFreq)
					{
						var pen = _pen;
						if (null == _cachedColorForIndexFunction)
						{
							double customSymbolSize = _cachedSymbolSizeForIndexFunction(j + offset);
							double w1 = _penWidth1.IsRelative ? _penWidth1.Value * _cachedSymbolSize : _penWidth1.Value;
							double w2 = _penWidth2.IsRelative ? _penWidth2.Value * _cachedSymbolSize : _penWidth2.Value;
							pen = _pen.WithThickness1(w1).WithThickness2(w2);
						}
						else
						{
							double customSymbolSize = null == _cachedSymbolSizeForIndexFunction ? _cachedSymbolSize : _cachedSymbolSizeForIndexFunction(j + offset);
							double w1 = _penWidth1.IsRelative ? _penWidth1.Value * _cachedSymbolSize : _penWidth1.Value;
							double w2 = _penWidth2.IsRelative ? _penWidth2.Value * _cachedSymbolSize : _penWidth2.Value;

							var customSymbolColor = _cachedColorForIndexFunction(j + offset);
							pen = _pen.WithThickness1(w1).WithThickness2(w2).WithColor(NamedColor.FromArgb(customSymbolColor.A, customSymbolColor.R, customSymbolColor.G, customSymbolColor.B));
						}

						Logical3D r3d = layer.GetLogical3D(pdata, j + rangeList[r].OffsetToOriginal);
						foreach (CSPlaneID id in _dropLine)
						{
							IPolylineD3D isoLine;
							layer.CoordinateSystem.GetIsolineFromPointToPlane(r3d, id, out isoLine);
							g.DrawLine(_pen, isoLine);
						}
					}
				}
			}
		}

		public RectangleD3D PaintSymbol(IGraphicsContext3D g, RectangleD3D bounds)
		{
			return RectangleD3D.Empty;
		}

		#region IPlotStyle Members

		public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
		{
			if (this.IsColorProvider)
				ColorGroupStyle.AddExternalGroupStyle(externalGroups);

			ScatterSymbolGroupStyle.AddExternalGroupStyle(externalGroups);
		}

		public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			ColorGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
			SkipFrequencyGroupStyle.AddLocalGroupStyle(externalGroups, localGroups); // (local group style only)
		}

		public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed3DPlotData pdata)
		{
			if (this.IsColorProvider)
				ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate () { return this.Color; });

			// SkipFrequency should be the same for all sub plot styles, so there is no "private" property
			SkipFrequencyGroupStyle.PrepareStyle(externalGroups, localGroups, delegate () { return SkipFrequency.HasValue ? SkipFrequency.Value : 1; });
		}

		public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			if (this.IsColorReceiver)
			{
				// try to get a constant color ...
				ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c) { this.Color = c; });
				// but if there is a color evaluation function, then use that function with higher priority
				if (!VariableColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (Func<int, Color> evalFunc) { _cachedColorForIndexFunction = evalFunc; }))
					_cachedColorForIndexFunction = null;
			}

			// try to get the symbol size ...
			SymbolSizeGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (double size) { this._cachedSymbolSize = size; });
			// but if there is an symbol size evaluation function, then use this with higher priority.
			if (!VariableSymbolSizeGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (Func<int, double> evalFunc) { _cachedSymbolSizeForIndexFunction = evalFunc; }))
				_cachedSymbolSizeForIndexFunction = null;

			// SkipFrequency should be the same for all sub plot styles, so there is no "private" property
			_cachedSkipFreq = _skipFreq.HasValue ? _skipFreq.Value : 1;
			SkipFrequencyGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (int c) { this._cachedSkipFreq = c; });
		}

		#endregion IPlotStyle Members

		#region IDocumentNode Members

		/// <summary>
		/// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
		/// to change a plot so that the plot items refer to another table.
		/// </summary>
		/// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
		public void VisitDocumentReferences(DocNodeProxyReporter Report)
		{
		}

		public IEnumerable<Tuple<string, IReadableColumn, string, Action<IReadableColumn>>> GetAdditionallyUsedColumns()
		{
			return null; // no additionally used columns
		}

		#endregion IDocumentNode Members
	}
}
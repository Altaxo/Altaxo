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

	public class ScatterPlotStyle
		:
		Main.SuspendableDocumentNodeWithEventArgs,
		IG3DPlotStyle
	{
		/// <summary>Material for the symbols.</summary>
		protected IMaterial _material;

		/// <summary>
		/// The scatter symbol.
		/// </summary>
		protected IScatterSymbolShape _symbolShape;

		/// <summary>Size of the symbols in points.</summary>
		protected double _symbolSize;

		/// <summary>Is the material color independent, i.e. not influenced by group styles.</summary>
		protected bool _independentColor;

		/// <summary>Is the size of the symbols independent, i.e. not influenced by group styles.</summary>
		protected bool _independentSymbolSize;

		/// <summary>A value of 2 skips every other data point, a value of 3 skips 2 out of 3 data points, and so on.</summary>
		protected int _skipFreq;

		// cached values:
		/// <summary>If this function is set, then _symbolSize is ignored and the symbol size is evaluated by this function.</summary>
		[field: NonSerialized]
		protected Func<int, double> _cachedSymbolSizeForIndexFunction;

		/// <summary>If this function is set, the symbol color is determined by calling this function on the index into the data.</summary>
		[field: NonSerialized]
		protected Func<int, Color> _cachedColorForIndexFunction;

		#region Serialization

		/// <summary>
		/// 2016-06-01 initial version.
		/// </summary>
		/// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ScatterPlotStyle), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				ScatterPlotStyle s = (ScatterPlotStyle)obj;
				info.AddValue("Shape", s._symbolShape);
				info.AddValue("Material", s._material);
				info.AddValue("SymbolSize", s._symbolSize);

				info.AddValue("IndependentColor", s._independentColor);
				info.AddValue("IndependentSymbolSize", s._independentSymbolSize);
				info.AddValue("SkipFreq", s._skipFreq);
			}

			protected virtual ScatterPlotStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				ScatterPlotStyle s = null != o ? (ScatterPlotStyle)o : new ScatterPlotStyle((Altaxo.Main.Properties.IReadOnlyPropertyBag)null);

				s._symbolShape = (IScatterSymbolShape)info.GetValue("Shape", s);
				s._material = (IMaterial)info.GetValue("Material", s);
				s._symbolSize = info.GetSingle("SymbolSize");
				s._independentColor = info.GetBoolean("IndependentColor");
				s._independentSymbolSize = info.GetBoolean("IndependentSymbolSize");
				s._skipFreq = info.GetInt32("SkipFreq");
				return s;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				ScatterPlotStyle s = SDeserialize(o, info, parent);

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
			var from = obj as ScatterPlotStyle;
			if (null != from)
			{
				CopyFrom(from, Main.EventFiring.Enabled);
				return true;
			}
			return false;
		}

		public void CopyFrom(ScatterPlotStyle from, Main.EventFiring eventFiring)
		{
			if (object.ReferenceEquals(this, from))
				return;

			using (var suspendToken = SuspendGetToken())
			{
				this._material = from._material; // immutable
				this._symbolShape = from._symbolShape; // immutable
				this._independentColor = from._independentColor;
				this._independentSymbolSize = from._independentSymbolSize;

				this._symbolSize = from._symbolSize;
				this._skipFreq = from._skipFreq;

				EhSelfChanged(EventArgs.Empty);

				suspendToken.Resume(eventFiring);
			}
		}

		public ScatterPlotStyle(ScatterPlotStyle from)
		{
			CopyFrom(from, Main.EventFiring.Suppressed);
		}

		public ScatterPlotStyle(IScatterSymbolShape symbol, double size, double penWidth, NamedColor penColor)
		{
			if (null == symbol)
				throw new ArgumentNullException(nameof(symbol));

			_symbolShape = symbol;
			_material = new MaterialWithUniformColor(penColor);
			_symbolSize = size;

			_skipFreq = 1;

			// Cached values
			SetCachedValues();
		}

		public ScatterPlotStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
		{
			double penWidth = GraphDocument.GetDefaultPenWidth(context);
			double symbolSize = GraphDocument.GetDefaultSymbolSize(context);
			var color = GraphDocument.GetDefaultPlotColor(context);

			this._symbolShape = new ScatterSymbolShapes.Cube();
			this._material = new MaterialWithUniformColor(color);
			this._independentColor = false;

			this._symbolSize = symbolSize;

			this._skipFreq = 1;
		}

		public object Clone()
		{
			return new ScatterPlotStyle(this);
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			yield break;
		}

		public IScatterSymbolShape Shape
		{
			get { return this._symbolShape; }
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));

				if (!object.ReferenceEquals(this._symbolShape, value))
				{
					this._symbolShape = value;

					SetCachedValues();

					EhSelfChanged(EventArgs.Empty); // Fire Changed event
				}
			}
		}

		public bool IsVisible
		{
			get
			{
				return _material.IsVisible;
			}
		}

		public IMaterial Pen
		{
			get { return this._material; }
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));

				if (!object.ReferenceEquals(this._material, value))
				{
					_material = value;
					SetCachedValues();
					EhSelfChanged(EventArgs.Empty); // Fire Changed event
				}
			}
		}

		public NamedColor Color
		{
			get { return this._material.Color; }
			set
			{
				Pen = _material.WithColor(value);
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

		public double SymbolSize
		{
			get { return _symbolSize; }
			set
			{
				if (value != _symbolSize)
				{
					_symbolSize = value;
					EhSelfChanged(EventArgs.Empty); // Fire Changed event
				}
			}
		}

		public bool IndependentSymbolSize
		{
			get
			{
				return _independentSymbolSize;
			}
			set
			{
				bool oldValue = _independentSymbolSize;
				_independentSymbolSize = value;
				if (value != oldValue)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public int SkipFrequency
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
				return !this._independentSymbolSize && !object.ReferenceEquals(_symbolShape, ScatterSymbolShapes.NoSymbol.Instance);
			}
		}

		public bool IsSymbolSizeReceiver
		{
			get
			{
				return !this._independentSymbolSize && !object.ReferenceEquals(_symbolShape, ScatterSymbolShapes.NoSymbol.Instance);
			}
		}

		#endregion I3DPlotItem Members

		public void Paint(IGraphicsContext3D g, IPlotArea layer, Processed3DPlotData pdata, Processed3DPlotData prevItemData, Processed3DPlotData nextItemData)
		{
			PlotRangeList rangeList = pdata.RangeList;
			var ptArray = pdata.PlotPointsInAbsoluteLayerCoordinates;

			// adjust the skip frequency if it was not set appropriate
			if (_skipFreq <= 0)
				_skipFreq = 1;

			// paint the scatter style
			if (!object.ReferenceEquals(_symbolShape, ScatterSymbolShapes.NoSymbol.Instance))
			{
				PointD3D pos = PointD3D.Empty;
				VectorD3D diff;

				if (null == _cachedSymbolSizeForIndexFunction && null == _cachedColorForIndexFunction) // using a constant symbol size
				{
					for (int j = 0; j < ptArray.Length; j += _skipFreq)
					{
						_symbolShape.Paint(g, _material, ptArray[j], _symbolSize);
					} // end for
				}
				else // using a variable symbol size or variable symbol color
				{
					for (int r = 0; r < rangeList.Count; r++)
					{
						int lower = rangeList[r].LowerBound;
						int upper = rangeList[r].UpperBound;
						int offset = rangeList[r].OffsetToOriginal;
						for (int j = lower; j < upper; j += _skipFreq)
						{
							if (null == _cachedColorForIndexFunction)
							{
								double customSymbolSize = _cachedSymbolSizeForIndexFunction(j + offset);
								_symbolShape.Paint(g, _material, ptArray[j], customSymbolSize);
							}
							else
							{
								double customSymbolSize = null == _cachedSymbolSizeForIndexFunction ? _symbolSize : _cachedSymbolSizeForIndexFunction(j + offset);
								var customSymbolColor = _cachedColorForIndexFunction(j + offset);
								_symbolShape.Paint(g, _material.WithColor(NamedColor.FromArgb(customSymbolColor.A, customSymbolColor.R, customSymbolColor.G, customSymbolColor.B)), ptArray[j], customSymbolSize);
							}
						}
					}
				}
			}
		}

		public RectangleD3D PaintSymbol(IGraphicsContext3D g, RectangleD3D bounds)
		{
			if (!object.ReferenceEquals(_symbolShape, ScatterSymbolShapes.NoSymbol.Instance))
			{
				_symbolShape.Paint(g, _material, bounds.Center, _symbolSize);
				bounds = bounds.WithPadding(0, Math.Max(0, this.SymbolSize - bounds.SizeY), Math.Max(0, this.SymbolSize - bounds.SizeZ));
			}

			return bounds;
		}

		#region IPlotStyle Members

		public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
		{
			if (this.IsColorProvider)
				ColorGroupStyle.AddExternalGroupStyle(externalGroups);
			if (this.IsSymbolSizeProvider)
				SymbolSizeGroupStyle.AddExternalGroupStyle(externalGroups);

			SymbolShapeStyleGroupStyle.AddExternalGroupStyle(externalGroups);
		}

		public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
		{
			ColorGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
			SymbolSizeGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
			SymbolShapeStyleGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
			SkipFrequencyGroupStyle.AddLocalGroupStyle(externalGroups, localGroups); // (local group style only)
		}

		public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed3DPlotData pdata)
		{
			if (this.IsColorProvider)
				ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate () { return this.Color; });

			SymbolShapeStyleGroupStyle.PrepareStyle(externalGroups, localGroups, delegate { return this._symbolShape; });

			if (this.IsSymbolSizeProvider)
				SymbolSizeGroupStyle.PrepareStyle(externalGroups, localGroups, delegate () { return SymbolSize; });

			// SkipFrequency should be the same for all sub plot styles, so there is no "private" property
			SkipFrequencyGroupStyle.PrepareStyle(externalGroups, localGroups, delegate () { return SkipFrequency; });
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

			SymbolShapeStyleGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (IScatterSymbolShape c) { this.Shape = c; });

			// per Default, set the symbol size evaluation function to null
			_cachedSymbolSizeForIndexFunction = null;
			if (!_independentSymbolSize)
			{
				// try to get a constant symbol size ...
				SymbolSizeGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (double size) { this.SymbolSize = size; });
				// but if there is an symbol size evaluation function, then use this with higher priority.
				if (!VariableSymbolSizeGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (Func<int, double> evalFunc) { _cachedSymbolSizeForIndexFunction = evalFunc; }))
					_cachedSymbolSizeForIndexFunction = null;
			}

			// SkipFrequency should be the same for all sub plot styles, so there is no "private" property
			SkipFrequencyGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (int c) { this.SkipFrequency = c; });
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
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

using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Graph.Graph2D.Plot.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols
{
	public abstract class ClosedSymbolBase : SymbolBase, IScatterSymbol
	{
		private PlotColorInfluence _plotColorInfluence = PlotColorInfluence.FillColorFull;
		protected NamedColor _fillColor = NamedColors.Black;

		protected double _relativeStructureWidth = 0.09375;
		protected IScatterSymbolFrame _frame;
		protected IScatterSymbolInset _inset;

		#region Serialization

		/// <summary>
		/// 2016-10-27 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ClosedSymbolBase), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (ClosedSymbolBase)obj;
				info.AddEnum("PlotColorInfluence", s._plotColorInfluence);
				info.AddValue("StructureScale", s._relativeStructureWidth);
				info.AddValue("Fill", s._fillColor);
				info.AddValue("Frame", s._frame);
				info.AddValue("Inset", s._inset);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (ClosedSymbolBase)o;
				s._plotColorInfluence = (PlotColorInfluence)info.GetEnum("PlotColorInfluence", typeof(PlotColorInfluence));
				s._relativeStructureWidth = info.GetDouble("StructureScale");
				s._fillColor = (NamedColor)info.GetValue("Fill", null);
				s._frame = (IScatterSymbolFrame)info.GetValue("Frame", null);
				s._inset = (IScatterSymbolInset)info.GetValue("Inset", null);

				return s;
			}
		}

		#endregion Serialization

		/// <summary>
		/// Gets a copy of the outer symbol shape as polygon(s).
		/// </summary>
		/// <returns>Polygon(s) of the outer symbol shape.</returns>
		public abstract List<List<ClipperLib.IntPoint>> GetCopyOfOuterPolygon();

		protected ClosedSymbolBase()
		{
		}

		protected ClosedSymbolBase(NamedColor fillColor, bool isFillColorInfluencedByPlotColor)
		{
			_fillColor = fillColor;
			_plotColorInfluence = isFillColorInfluencedByPlotColor ? PlotColorInfluence.FillColorFull : PlotColorInfluence.None;
		}

		public object Clone()
		{
			return this.MemberwiseClone();
		}

		public double DesignSize { get { return 2; } }

		public NamedColor FillColor { get { return _fillColor; } }

		public ClosedSymbolBase WithFillColor(NamedColor value)
		{
			if (_fillColor == value)
			{
				return this;
			}
			else
			{
				var result = (ClosedSymbolBase)this.MemberwiseClone();
				result._fillColor = value;
				return result;
			}
		}

		IScatterSymbol IScatterSymbol.WithFillColor(NamedColor fillColor)
		{
			return WithFillColor(fillColor);
		}

		public PlotColorInfluence PlotColorInfluence { get { return _plotColorInfluence; } }

		public ClosedSymbolBase WithPlotColorInfluence(PlotColorInfluence value)
		{
			if (_plotColorInfluence == value)
			{
				return this;
			}
			else
			{
				var result = (ClosedSymbolBase)this.MemberwiseClone();
				result._plotColorInfluence = value;
				return result;
			}
		}

		IScatterSymbol IScatterSymbol.WithPlotColorInfluence(PlotColorInfluence plotColorInfluence)
		{
			return WithPlotColorInfluence(plotColorInfluence);
		}

		public double RelativeStructureWidth { get { return _relativeStructureWidth; } }

		public ClosedSymbolBase WithRelativeStructureWidth(double value)
		{
			if (!(value >= 0) || !(value < 0.5))
				throw new ArgumentOutOfRangeException(nameof(value), "Provided value must be >=0 and <0.5");

			if (_relativeStructureWidth == value)
			{
				return this;
			}
			else
			{
				{
					var result = (ClosedSymbolBase)this.MemberwiseClone();
					result._relativeStructureWidth = value;
					return result;
				}
			}
		}

		IScatterSymbol IScatterSymbol.WithRelativeStructureWidth(double relativeStructureWidth)
		{
			return WithRelativeStructureWidth(relativeStructureWidth);
		}

		public IScatterSymbolFrame Frame
		{
			get { return _frame; }
		}

		public ClosedSymbolBase WithFrame(IScatterSymbolFrame frame)
		{
			return WithFrame(frame, null);
		}

		IScatterSymbol IScatterSymbol.WithFrame(IScatterSymbolFrame frame)
		{
			return WithFrame(frame, null);
		}

		public ClosedSymbolBase WithFrame(IScatterSymbolFrame frame, bool? isInfluencedByPlotColor)
		{
			if (object.ReferenceEquals(_frame, frame) && (!isInfluencedByPlotColor.HasValue || _plotColorInfluence.HasFlag(PlotColorInfluence.FrameColorFull) == isInfluencedByPlotColor.Value))
			{
				return this;
			}
			else
			{
				var result = (ClosedSymbolBase)this.MemberwiseClone();
				result._frame = frame;
				if (isInfluencedByPlotColor.HasValue)
					result._plotColorInfluence = result._plotColorInfluence.WithFlag(PlotColorInfluence.FrameColorFull, isInfluencedByPlotColor.Value);
				return result;
			}
		}

		public IScatterSymbolInset Inset { get { return _inset; } }

		public ClosedSymbolBase WithInset(IScatterSymbolInset inset)
		{
			return WithInset(inset, null);
		}

		IScatterSymbol IScatterSymbol.WithInset(IScatterSymbolInset inset)
		{
			return WithInset(inset, null);
		}

		public ClosedSymbolBase WithInset(IScatterSymbolInset inset, bool? isInfluencedByPlotColor)
		{
			if (object.ReferenceEquals(_inset, inset) && (!isInfluencedByPlotColor.HasValue || _plotColorInfluence.HasFlag(PlotColorInfluence.InsetColorFull) == isInfluencedByPlotColor.Value))
			{
				return this;
			}
			else
			{
				var result = (ClosedSymbolBase)this.MemberwiseClone();
				result._inset = inset;

				if (isInfluencedByPlotColor.HasValue)
					result._plotColorInfluence = result._plotColorInfluence.WithFlag(PlotColorInfluence.InsetColorFull, isInfluencedByPlotColor.Value);
				return result;
			}
		}

		public void CalculatePolygons(
			double? overrideRelativeStructureWidth,
			out List<List<ClipperLib.IntPoint>> framePolygon,
			out List<List<ClipperLib.IntPoint>> insetPolygon,
			out List<List<ClipperLib.IntPoint>> fillPolygon)

		{
			insetPolygon = null;
			framePolygon = null;
			fillPolygon = null;

			// get outer polygon
			var outerPolygon = GetCopyOfOuterPolygon();

			List<List<ClipperLib.IntPoint>> innerFramePolygon = null;
			double relativeStructureWidth = overrideRelativeStructureWidth ?? _relativeStructureWidth;
			if (null != _frame && relativeStructureWidth > 0)
			{
				// get frame polygon
				innerFramePolygon = _frame.GetCopyOfClipperPolygon(relativeStructureWidth, outerPolygon);
			}

			if (null != _inset)
			{
				// get inset polygon
				insetPolygon = _inset.GetCopyOfClipperPolygon(relativeStructureWidth);
			}

			// if null != insetPolygon
			// clip with innerPolygon ?? outerPolygon;
			// store clipped inset polygon / draw it with inset color
			if (null != insetPolygon)
			{
				var clipper = new ClipperLib.Clipper();
				var solution = new List<List<ClipperLib.IntPoint>>();
				clipper.AddPaths(insetPolygon, ClipperLib.PolyType.ptSubject, true);
				clipper.AddPaths(innerFramePolygon ?? outerPolygon, ClipperLib.PolyType.ptClip, true);
				clipper.Execute(ClipperLib.ClipType.ctIntersection, solution);
				insetPolygon = solution;
			}

			// if null != framePolygon
			// clip with outer polygon ????
			// draw combined path of outer polygon and frame polygon as a hole with frame color
			if (null != innerFramePolygon)
			{
				var clipper = new ClipperLib.Clipper();
				clipper.AddPaths(outerPolygon, ClipperLib.PolyType.ptSubject, true);
				clipper.AddPaths(innerFramePolygon, ClipperLib.PolyType.ptClip, true);
				framePolygon = new List<List<ClipperLib.IntPoint>>();
				clipper.Execute(ClipperLib.ClipType.ctDifference, framePolygon);
			}

			// calculate
			// if null != insetPolygon
			//	(framePolygon ?? outerPolygon ) - insetPolygon
			// or else use (framePolygon ?? outerPolygon ) directly
			// draw result with fillColor

			if (null != insetPolygon)
			{
				var clipper = new ClipperLib.Clipper();
				clipper.AddPaths(innerFramePolygon ?? outerPolygon, ClipperLib.PolyType.ptSubject, true);
				clipper.AddPaths(insetPolygon, ClipperLib.PolyType.ptClip, true);
				fillPolygon = new List<List<ClipperLib.IntPoint>>();
				clipper.Execute(ClipperLib.ClipType.ctDifference, fillPolygon);
			}
			else
			{
				fillPolygon = innerFramePolygon ?? outerPolygon;
			}
		}

		public override bool Equals(object obj)
		{
			if (!(this.GetType() == obj?.GetType()))
				return false;

			var from = (ClosedSymbolBase)obj;

			return
				this._plotColorInfluence == from._plotColorInfluence &&
				this._relativeStructureWidth == from._relativeStructureWidth &&
				this._fillColor == from._fillColor &&
				Equals(this._frame, from._frame) &&
				Equals(this._inset, from.Inset);
		}

		public override int GetHashCode()
		{
			return
				this.GetType().GetHashCode() +
				(int)this._plotColorInfluence +
				this._relativeStructureWidth.GetHashCode() +
				this._fillColor.GetHashCode() +
				(this._frame?.GetHashCode() ?? 0) +
				(this._inset?.GetHashCode() ?? 0);
		}
	}
}
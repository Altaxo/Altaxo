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
	public abstract class ScatterSymbolBase : IScatterSymbol
	{
		/// <summary>
		/// Used to create clipper polynoms for the outer symbol shape, frame and inset. The outer symbol shape
		/// must fit in a circle of radius 1 (diameter: 2). Translated to clipper values this means that the outer symbol shape
		/// must fit in a circle of this <see cref="ClipperScalingDouble"/>.</summary>
		public const double ClipperScalingDouble = 1073741824.49;

		/// <summary>
		/// Used to create clipper polynoms for the outer symbol shape, frame and inset. The outer symbol shape
		/// must fit in a circle of radius 1 (diameter: 2). Translated to clipper values this means that the outer symbol shape
		/// must fit in a circle of this <see cref="ClipperScalingInt"/>.</summary>
		public const int ClipperScalingInt = 1073741824;

		/// <summary>By multiplying the clipper polynom points with this factor, you will get a symbol size of 1.</summary>
		public const double InverseClipperScalingToSymbolSize1 = 0.5 / 1073741824.0;

		private PlotColorInfluence _plotColorInfluence = PlotColorInfluence.FillColor;
		protected NamedColor _fillColor;

		protected double _relativeStructureWidth = 0.09375;
		protected IScatterSymbolFrame _frame;
		protected IScatterSymbolInset _inset;

		#region Serialization

		protected static void SerializeSetV0(IScatterSymbol obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
		{
			var parent = ScatterSymbolListManager.Instance.GetParentList(obj);
			if (null != parent)
			{
				if (null == info.GetProperty(ScatterSymbolList.GetSerializationRegistrationKey(parent)))
					info.AddValue("Set", parent);
				else
					info.AddValue("SetName", parent.Name);
			}
		}

		protected static TItem DeserializeSetV0<TItem>(TItem instanceTemplate, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent) where TItem : IScatterSymbol
		{
			if (info.CurrentElementName == "Set")
			{
				var originalSet = (ScatterSymbolList)info.GetValue("Set", parent);
				ScatterSymbolList registeredSet;
				ScatterSymbolListManager.Instance.TryRegisterList(info, originalSet, Main.ItemDefinitionLevel.Project, out registeredSet);
				return (TItem)ScatterSymbolListManager.Instance.GetDeserializedInstanceFromInstanceAndSetName(info, instanceTemplate, originalSet.Name); // Note: here we use the name of the original set, not of the registered set. Because the original name is translated during registering into the registered name
			}
			else if (info.CurrentElementName == "SetName")
			{
				string setName = info.GetString("SetName");
				return (TItem)ScatterSymbolListManager.Instance.GetDeserializedInstanceFromInstanceAndSetName(info, instanceTemplate, setName);
			}
			else // nothing of both, thus symbol belongs to nothing
			{
				return instanceTemplate;
			}
		}

		/// <summary>
		/// 2016-10-27 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ScatterSymbolBase), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (ScatterSymbolBase)obj;
				info.AddEnum("PlotColorInfluence", s._plotColorInfluence);
				info.AddValue("StructureScale", s._relativeStructureWidth);
				info.AddValue("Fill", s._fillColor);
				info.AddValue("Frame", s._frame);
				info.AddValue("Inset", s._inset);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (ScatterSymbolBase)o;
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

		protected ScatterSymbolBase()
		{
		}

		protected ScatterSymbolBase(NamedColor fillColor, bool isFillColorInfluencedByPlotColor)
		{
			_fillColor = fillColor;
			_plotColorInfluence = isFillColorInfluencedByPlotColor ? PlotColorInfluence.FillColor : PlotColorInfluence.None;
		}

		public object Clone()
		{
			return this.MemberwiseClone();
		}

		public NamedColor FillColor { get { return _fillColor; } }

		public ScatterSymbolBase WithFillColor(NamedColor value)
		{
			if (_fillColor == value)
			{
				return this;
			}
			else
			{
				var result = (ScatterSymbolBase)this.MemberwiseClone();
				result._fillColor = value;
				return result;
			}
		}

		public PlotColorInfluence PlotColorInfluence { get { return _plotColorInfluence; } }

		public ScatterSymbolBase WithPlotColorInfluence(PlotColorInfluence value)
		{
			if (_plotColorInfluence == value)
			{
				return this;
			}
			else
			{
				var result = (ScatterSymbolBase)this.MemberwiseClone();
				result._plotColorInfluence = value;
				return result;
			}
		}

		public double RelativeStructureWidth { get { return _relativeStructureWidth; } }

		public ScatterSymbolBase WithRelativeStructureWidth(double value)
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
					var result = (ScatterSymbolBase)this.MemberwiseClone();
					result._relativeStructureWidth = value;
					return result;
				}
			}
		}

		public IScatterSymbolFrame Frame
		{
			get { return _frame; }
		}

		public ScatterSymbolBase WithFrame(IScatterSymbolFrame frame)
		{
			return WithFrame(frame, true);
		}

		public ScatterSymbolBase WithFrame(IScatterSymbolFrame frame, bool isInfluencedByPlotColor)
		{
			if (object.ReferenceEquals(_frame, frame) && _plotColorInfluence.HasFlag(PlotColorInfluence.FrameColor) == isInfluencedByPlotColor)
			{
				return this;
			}
			else
			{
				var result = (ScatterSymbolBase)this.MemberwiseClone();
				result._frame = frame;
				result._plotColorInfluence = result._plotColorInfluence.WithFlag(PlotColorInfluence.FrameColor, isInfluencedByPlotColor);
				return result;
			}
		}

		public IScatterSymbolInset Inset { get { return _inset; } }

		public ScatterSymbolBase WithInset(IScatterSymbolInset inset)
		{
			return WithInset(inset, true);
		}

		public ScatterSymbolBase WithInset(IScatterSymbolInset inset, bool isInfluencedByPlotColor)
		{
			if (object.ReferenceEquals(_inset, inset) && _plotColorInfluence.HasFlag(PlotColorInfluence.InsetColor) == isInfluencedByPlotColor)
			{
				return this;
			}
			else
			{
				var result = (ScatterSymbolBase)this.MemberwiseClone();
				result._inset = inset;
				result._plotColorInfluence = result._plotColorInfluence.WithFlag(PlotColorInfluence.InsetColor, isInfluencedByPlotColor);
				return result;
			}
		}

		public void CalculatePolygons(
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
			if (null != _frame && _relativeStructureWidth > 0)
			{
				// get frame polygon
				innerFramePolygon = _frame.GetCopyOfClipperPolygon(_relativeStructureWidth, outerPolygon);
			}

			if (null != _inset)
			{
				// get inset polygon
				insetPolygon = _inset.GetCopyOfClipperPolygon(_relativeStructureWidth);
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
	}
}
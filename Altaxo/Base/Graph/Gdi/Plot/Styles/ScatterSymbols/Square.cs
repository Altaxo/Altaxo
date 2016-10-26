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

using Altaxo.Drawing;
using Altaxo.Geometry;
using ClipperLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Gdi.Plot.Styles.ScatterSymbols
{
	public interface IScatterSymbolInset
	{
		NamedColor Color { get; }

		List<List<ClipperLib.IntPoint>> GetCopyOfClipperPolygon(double relativeWidth);
	}

	public class VerticalBarInset : IScatterSymbolInset
	{
		private const double ClipperScalingDouble = 1073741824.49;
		private const int ClipperScalingInt = 1073741824;

		public NamedColor Color
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public List<List<ClipperLib.IntPoint>> GetCopyOfClipperPolygon(double relativeWidth)
		{
			return new List<List<ClipperLib.IntPoint>>(1)
			{
				new List<ClipperLib.IntPoint>(4)
				{
				new ClipperLib.IntPoint(-relativeWidth*ClipperScalingDouble, -ClipperScalingInt),
				new ClipperLib.IntPoint(relativeWidth * ClipperScalingDouble, -ClipperScalingInt),
				new ClipperLib.IntPoint(relativeWidth * ClipperScalingDouble, ClipperScalingInt),
				new ClipperLib.IntPoint(-relativeWidth * ClipperScalingDouble, ClipperScalingInt)
			}
			};
		}
	}

	public class SquarePointInset : IScatterSymbolInset
	{
		private const double ClipperScalingDouble = 1073741824.49;
		private const int ClipperScalingInt = 1073741824;

		public NamedColor Color
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public List<List<ClipperLib.IntPoint>> GetCopyOfClipperPolygon(double relativeWidth)
		{
			return new List<List<ClipperLib.IntPoint>>(1)
			{
				new List<ClipperLib.IntPoint>(4)
				{
				new ClipperLib.IntPoint(-relativeWidth*ClipperScalingDouble, -relativeWidth*ClipperScalingDouble),
				new ClipperLib.IntPoint(relativeWidth * ClipperScalingDouble, -relativeWidth*ClipperScalingDouble),
				new ClipperLib.IntPoint(relativeWidth * ClipperScalingDouble, relativeWidth*ClipperScalingDouble),
				new ClipperLib.IntPoint(-relativeWidth * ClipperScalingDouble, relativeWidth*ClipperScalingDouble)
			}
			};
		}
	}

	public interface IScatterSymbolFrame
	{
		NamedColor Color { get; }

		List<List<ClipperLib.IntPoint>> GetCopyOfClipperPolygon(double relativeWidth, List<List<ClipperLib.IntPoint>> outerPolygon);
	}

	public class ConstantThicknessFrame : IScatterSymbolFrame
	{
		public NamedColor Color
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public List<List<IntPoint>> GetCopyOfClipperPolygon(double relativeWidth, List<List<IntPoint>> outerPolygon)
		{
			var delta = (-2 * relativeWidth) * ScatterSymbolBase.ClipperScalingInt;
			var clipper = new ClipperOffset();
			clipper.AddPaths(outerPolygon, JoinType.jtMiter, EndType.etClosedPolygon);
			var result = new List<List<IntPoint>>();
			clipper.Execute(ref result, delta);
			return result;
		}
	}

	public abstract class ScatterSymbolBase
	{
		public const double ClipperScalingDouble = 1073741824.49;
		public const int ClipperScalingInt = 1073741824;
		public const double InverseClipperScaling = 1 / 1073741824.0;

		public abstract List<List<ClipperLib.IntPoint>> GetCopyOfOuterPolygon();
	}

	public class NoSymbol : IScatterSymbol
	{
		public NamedColor FillColor
		{
			get
			{
				return NamedColors.Transparent;
			}
		}

		public IScatterSymbolFrame Frame
		{
			get
			{
				return null;
			}
		}

		public IScatterSymbolInset Inset
		{
			get
			{
				return null;
			}
		}

		public void CalculatePolygons(out List<List<IntPoint>> framePolygon, out List<List<IntPoint>> insetPolygon, out List<List<IntPoint>> fillPolygon)
		{
			framePolygon = null;
			insetPolygon = null;
			fillPolygon = null;
		}

		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}

	public abstract class ClosedScatterSymbol : ScatterSymbolBase, IScatterSymbol
	{
		protected IScatterSymbolFrame _frame;
		protected IScatterSymbolInset _inset;
		protected double _relativeStructureWidth = 0.125;

		public object Clone()
		{
			return this.MemberwiseClone();
		}

		public NamedColor FillColor { get; }

		public double RelativeStructureWidth { get { return _relativeStructureWidth; } }

		public ClosedScatterSymbol WithRelativeStructureWidth(double value)
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
					var result = (ClosedScatterSymbol)this.MemberwiseClone();
					result._relativeStructureWidth = value;
					return result;
				}
			}
		}

		public IScatterSymbolFrame Frame { get; }

		public ClosedScatterSymbol WithFrame(IScatterSymbolFrame frame)
		{
			if (object.ReferenceEquals(_frame, frame))
			{
				return this;
			}
			else
			{
				var result = (ClosedScatterSymbol)this.MemberwiseClone();
				result._frame = frame;
				return result;
			}
		}

		public IScatterSymbolInset Inset { get; }

		public ClosedScatterSymbol WithInset(IScatterSymbolInset inset)
		{
			if (object.ReferenceEquals(_inset, inset))
			{
				return this;
			}
			else
			{
				var result = (ClosedScatterSymbol)this.MemberwiseClone();
				result._inset = inset;
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

	public class Square : ClosedScatterSymbol
	{
		private const double Sqrt1By2 = 0.70710678118654752440084436210485;

		public IEnumerable<PointD2D> GetPolygon()
		{
			yield return new PointD2D(-Sqrt1By2, -Sqrt1By2);
			yield return new PointD2D(Sqrt1By2, -Sqrt1By2);
			yield return new PointD2D(Sqrt1By2, Sqrt1By2);
			yield return new PointD2D(-Sqrt1By2, Sqrt1By2);
		}

		public override List<List<ClipperLib.IntPoint>> GetCopyOfOuterPolygon()
		{
			int w = (int)(ClipperScalingInt * Sqrt1By2);

			return new List<List<ClipperLib.IntPoint>>(1)
			{
			new List<ClipperLib.IntPoint>(4)
			{
			new ClipperLib.IntPoint(-w, -w),
			new ClipperLib.IntPoint(w, -w),
			new ClipperLib.IntPoint(w, w),
			new ClipperLib.IntPoint(-w, w)
			}};
		}
	}

	public class SquareGdiImpl
	{
		public static PointF[] ToPointFArray(List<ClipperLib.IntPoint> list, double symbolSize)
		{
			var scale = ScatterSymbolBase.InverseClipperScaling * symbolSize / 2;
			return list.Select(
				intPoint =>
				new PointF((float)(scale * intPoint.X), (float)(scale * intPoint.Y))).ToArray();
		}

		public void Paint(Graphics g, Square obj, double size, AxoColor? color1, AxoColor? color2, AxoColor? color3)
		{
			List<List<ClipperLib.IntPoint>> insetPolygon = null;
			List<List<ClipperLib.IntPoint>> framePolygon = null;
			List<List<ClipperLib.IntPoint>> fillPolygon = null;

			obj.CalculatePolygons(out framePolygon, out insetPolygon, out fillPolygon);

			var path = new GraphicsPath();

			if (null != insetPolygon)
			{
				var brush = new SolidBrush(color3 ?? obj.Inset.Color);

				path.Reset();
				foreach (var list in insetPolygon)
					path.AddPolygon(ToPointFArray(list, size));
				g.FillPath(brush, path);
			}

			if (null != fillPolygon)
			{
				var brush = new SolidBrush(color1 ?? obj.FillColor);

				path.Reset();
				foreach (var list in fillPolygon)
					path.AddPolygon(ToPointFArray(list, size));

				g.FillPath(brush, path);
			}

			if (null != framePolygon)
			{
				var brush = new SolidBrush(color2 ?? obj.Frame.Color);

				path.Reset();
				foreach (var list in framePolygon)
					path.AddPolygon(ToPointFArray(list, size));

				g.FillPath(brush, path);
			}
		}
	}
}
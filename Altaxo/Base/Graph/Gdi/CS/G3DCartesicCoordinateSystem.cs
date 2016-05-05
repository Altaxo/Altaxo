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

using Altaxo.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Altaxo.Graph.Gdi.CS
{
	internal class G3DCartesicCoordinateSystem : G2DCoordinateSystem
	{
		private struct TMatrix
		{
			public double M11;
			public double M12;
			public double M13;
			public double M21;
			public double M22;
			public double M23;
			//public double M31;
			//public double M32;
			//public double M33;
		}

		private TMatrix _projectionMatrix;

		/// <summary>
		/// Copies the member variables from another coordinate system.
		/// </summary>
		/// <param name="fromb">The coordinate system to copy from.</param>
		public override void CopyFrom(G2DCoordinateSystem fromb)
		{
			if (object.ReferenceEquals(this, fromb))
				return;

			base.CopyFrom(fromb);
			if (fromb is G3DCartesicCoordinateSystem)
			{
				G3DCartesicCoordinateSystem from = (G3DCartesicCoordinateSystem)fromb;
				this._projectionMatrix = from._projectionMatrix;
			}
		}

		/// <summary>
		/// Updates the internal storage of the rectangular area size to a new value.
		/// </summary>
		/// <param name="size">The new size.</param>
		public override void UpdateAreaSize(PointD2D size)
		{
			base.UpdateAreaSize(size);
			UpdateProjectionMatrix();
		}

		private void UpdateProjectionMatrix()
		{
			// for test use isometric

			double width = _layerWidth / (1 + Math.Sqrt(0.5));
			double height = _layerHeight / (1 + Math.Sqrt(0.5));

			_projectionMatrix.M11 = width;
			_projectionMatrix.M12 = 0;
			_projectionMatrix.M13 = Math.Sqrt(0.5) * height;

			_projectionMatrix.M21 = 0;
			_projectionMatrix.M22 = height;
			_projectionMatrix.M23 = Math.Sqrt(0.5) * height;
		}

		public override bool IsOrthogonal
		{
			get { return true; }
		}

		public override bool IsAffine
		{
			get { return true; }
		}

		public override bool Is3D
		{
			get { return true; }
		}

		public override bool LogicalToLayerCoordinates(Logical3D r, out double xlocation, out double ylocation)
		{
			xlocation = _projectionMatrix.M11 * r.RX + _projectionMatrix.M12 * r.RY + _projectionMatrix.M13 * r.RZ;
			ylocation = _projectionMatrix.M21 * r.RX + _projectionMatrix.M22 * r.RY + _projectionMatrix.M23 * r.RZ;
			ylocation = _projectionMatrix.M22 - ylocation;

			return true;
		}

		public override bool LogicalToLayerCoordinatesAndDirection(Logical3D r0, Logical3D r1, double t, out double ax, out double ay, out double adx, out double ady)
		{
			LogicalToLayerCoordinates(Logical3D.Interpolate(r0, r1, t), out ax, out ay);

			double x0, y0, x1, y1;
			LogicalToLayerCoordinates(r0, out x0, out y0);
			LogicalToLayerCoordinates(r1, out x1, out y1);
			adx = x1 - x0;
			ady = y1 - y0;

			return true;
		}

		public override bool LayerToLogicalCoordinates(double xlocation, double ylocation, out Logical3D r)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void GetIsoline(System.Drawing.Drawing2D.GraphicsPath path, Logical3D r0, Logical3D r1)
		{
			double x0, y0, x1, y1;
			LogicalToLayerCoordinates(r0, out x0, out y0);
			LogicalToLayerCoordinates(r1, out x1, out y1);
			path.AddLine((float)x0, (float)y0, (float)x1, (float)y1);
		}

		protected override void UpdateAxisInfo()
		{
			int horzAx = 0;
			int vertAx = 1;
			int deptAx = 2;

			if (null == _axisStyleInformation)
				_axisStyleInformation = new List<CSAxisInformation>();
			else
				_axisStyleInformation.Clear();

			CSAxisInformation info;

			// BottomFront
			info = new CSAxisInformation(
				Identifier: new CSLineID(horzAx, 0, 0),
			NameOfAxisStyle: "BottomFront",
			NameOfFirstDownSide: "Below",
			NameOfFirstUpSide: "Above",
			NameOfSecondDownSide: "Before",
			NameOfSecondUpSide: "Behind",
			PreferredLabelSide: CSAxisSide.FirstDown,
			PreferredTickSide: CSAxisSide.FirstDown,
			IsShownByDefault: true,
			HasTicksByDefault: true,
			HasLabelsByDefault: true,
			HasTitleByDefault: true);

			_axisStyleInformation.Add(info);

			// TopFront
			info = new CSAxisInformation(
			Identifier: new CSLineID(horzAx, 1, 0),
			NameOfAxisStyle: "TopFront",
			NameOfFirstDownSide: "Below",
			NameOfFirstUpSide: "Above",
			NameOfSecondDownSide: "Before",
			NameOfSecondUpSide: "Behind",
			PreferredLabelSide: CSAxisSide.FirstUp,
			PreferredTickSide: CSAxisSide.FirstUp,
			IsShownByDefault: true,
			HasTicksByDefault: false,
			HasLabelsByDefault: false,
			HasTitleByDefault: false);

			_axisStyleInformation.Add(info);

			// LeftFront
			info = new CSAxisInformation(
				Identifier: new CSLineID(vertAx, 0, 0),
			NameOfAxisStyle: "LeftFront",
			NameOfFirstDownSide: "Left",
			NameOfFirstUpSide: "Right",
			NameOfSecondDownSide: "Before",
			NameOfSecondUpSide: "Behind",
			PreferredLabelSide: CSAxisSide.FirstDown,
			PreferredTickSide: CSAxisSide.FirstDown,
			IsShownByDefault: true,
			HasTicksByDefault: true,
			HasLabelsByDefault: true,
			HasTitleByDefault: true);

			_axisStyleInformation.Add(info);

			// RightFront
			info = new CSAxisInformation(
				Identifier: new CSLineID(vertAx, 1, 0),
			NameOfAxisStyle: "RightFront",
			NameOfFirstDownSide: "Left",
			NameOfFirstUpSide: "Right",
			NameOfSecondDownSide: "Before",
			NameOfSecondUpSide: "Behind",
			PreferredLabelSide: CSAxisSide.FirstUp,
			PreferredTickSide: CSAxisSide.FirstUp,
			IsShownByDefault: true,
			HasTicksByDefault: true,
			HasLabelsByDefault: true,
			HasTitleByDefault: true
);

			_axisStyleInformation.Add(info);

			// BottomBack
			info = new CSAxisInformation(
				new CSLineID(horzAx, 0, 1),
			NameOfAxisStyle: "BottomBack",
			NameOfFirstDownSide: "Below",
			NameOfFirstUpSide: "Above",
			NameOfSecondDownSide: "Before",
			NameOfSecondUpSide: "Behind",
			PreferredLabelSide: CSAxisSide.FirstDown,
			PreferredTickSide: CSAxisSide.FirstDown,
			IsShownByDefault: true,
			HasTicksByDefault: true,
			HasLabelsByDefault: true,
			HasTitleByDefault: true);

			_axisStyleInformation.Add(info);

			// TopBack
			info = new CSAxisInformation(
				Identifier: new CSLineID(horzAx, 1, 1),
			NameOfAxisStyle: "TopBack",
			NameOfFirstDownSide: "Below",
			NameOfFirstUpSide: "Above",
			NameOfSecondDownSide: "Before",
			NameOfSecondUpSide: "Behind",
			PreferredLabelSide: CSAxisSide.FirstUp,
			PreferredTickSide: CSAxisSide.FirstUp,
			IsShownByDefault: true,
			HasTicksByDefault: true,
			HasLabelsByDefault: true,
			HasTitleByDefault: true
);

			_axisStyleInformation.Add(info);

			// LeftBack
			info = new CSAxisInformation(
				new CSLineID(vertAx, 0, 1),
			NameOfAxisStyle: "LeftBack",
			NameOfFirstDownSide: "Left",
			NameOfFirstUpSide: "Right",
			NameOfSecondDownSide: "Before",
			NameOfSecondUpSide: "Behind",
			PreferredLabelSide: CSAxisSide.FirstDown,
			PreferredTickSide: CSAxisSide.FirstDown,
			IsShownByDefault: true,
				HasTicksByDefault: true,
			HasLabelsByDefault: true,
		HasTitleByDefault: true);

			_axisStyleInformation.Add(info);

			// RightBack
			info = new CSAxisInformation(
			Identifier: new CSLineID(vertAx, 1, 1),
			NameOfAxisStyle: "RightBack",
			NameOfFirstDownSide: "Left",
			NameOfFirstUpSide: "Right",
			NameOfSecondDownSide: "Before",
			NameOfSecondUpSide: "Behind",
			PreferredLabelSide: CSAxisSide.FirstUp,
			PreferredTickSide: CSAxisSide.FirstUp,
			IsShownByDefault: true,
				 HasTicksByDefault: true,
			HasLabelsByDefault: true,
			HasTitleByDefault: false
			);

			_axisStyleInformation.Add(info);

			// BottomLeft
			info = new CSAxisInformation(
			Identifier: new CSLineID(deptAx, 0, 0),
			NameOfAxisStyle: "BottomLeft",
			NameOfFirstDownSide: "Left",
			NameOfFirstUpSide: "Right",
			NameOfSecondDownSide: "Below",
			NameOfSecondUpSide: "Above",
			PreferredLabelSide: CSAxisSide.FirstDown,
			PreferredTickSide: CSAxisSide.FirstDown,
			IsShownByDefault: true,
			HasTicksByDefault: true,
			HasLabelsByDefault: true,
			HasTitleByDefault: true);

			_axisStyleInformation.Add(info);

			// TopLeft
			info = new CSAxisInformation(
				Identifier: new CSLineID(deptAx, 0, 1),
			NameOfAxisStyle: "TopLeft",
			NameOfFirstDownSide: "Left",
			NameOfFirstUpSide: "Right",
			NameOfSecondDownSide: "Below",
			NameOfSecondUpSide: "Above",
			PreferredLabelSide: CSAxisSide.FirstUp,
			PreferredTickSide: CSAxisSide.FirstUp,
			IsShownByDefault: true,
			HasTicksByDefault: true,
			HasLabelsByDefault: true,
			HasTitleByDefault: false
);

			_axisStyleInformation.Add(info);

			// BottomRight
			info = new CSAxisInformation(
			Identifier: new CSLineID(deptAx, 1, 0),
			NameOfAxisStyle: "BottomRight",
			NameOfFirstDownSide: "Left",
			NameOfFirstUpSide: "Right",
			NameOfSecondDownSide: "Below",
			NameOfSecondUpSide: "Above",
			PreferredLabelSide: CSAxisSide.FirstDown,
			PreferredTickSide: CSAxisSide.FirstDown,
			IsShownByDefault: true,
			HasTicksByDefault: true,
			HasLabelsByDefault: true,

			HasTitleByDefault: true);

			_axisStyleInformation.Add(info);

			// TopRight
			info = new CSAxisInformation(
				Identifier: new CSLineID(deptAx, 1, 1),
			NameOfAxisStyle: "TopRight",
			NameOfFirstDownSide: "Left",
			NameOfFirstUpSide: "Right",
			NameOfSecondDownSide: "Below",
			NameOfSecondUpSide: "Above",
			PreferredLabelSide: CSAxisSide.FirstUp,
			PreferredTickSide: CSAxisSide.FirstUp,
			IsShownByDefault: true,
			HasTicksByDefault: true,
			HasLabelsByDefault: true,
			HasTitleByDefault: false
);

			_axisStyleInformation.Add(info);

			// XAxis: Y=0, Z=0
			info = new CSAxisInformation(
				Identifier: CSLineID.FromPhysicalValue(horzAx, 0),
			NameOfAxisStyle: "YZ:0",
			NameOfFirstUpSide: "Above",
			NameOfFirstDownSide: "Below",
			NameOfSecondDownSide: "Before",
			NameOfSecondUpSide: "Behind",
			PreferredLabelSide: CSAxisSide.FirstDown,
			PreferredTickSide: CSAxisSide.FirstDown,
			IsShownByDefault: false,
			HasTicksByDefault: false,
			HasLabelsByDefault: false,
			HasTitleByDefault: false
);

			_axisStyleInformation.Add(info);

			// YAxis: X=0, Z=0
			info = new CSAxisInformation(
			Identifier: CSLineID.FromPhysicalValue(vertAx, 0),
			NameOfAxisStyle: "XZ:0",
			NameOfFirstDownSide: "Left",
			NameOfFirstUpSide: "Right",
			NameOfSecondDownSide: "Before",
			NameOfSecondUpSide: "Behind",
			PreferredLabelSide: CSAxisSide.FirstDown,
			PreferredTickSide: CSAxisSide.FirstDown,
			IsShownByDefault: false,
			HasTicksByDefault: false,
			HasLabelsByDefault: false,
			HasTitleByDefault: false
);

			_axisStyleInformation.Add(info);

			// ZAxis: X=0,Y=0
			info = new CSAxisInformation(
				CSLineID.FromPhysicalValue(deptAx, 0),
			NameOfAxisStyle: "XY:0",
			NameOfFirstDownSide: "Left",
			NameOfFirstUpSide: "Right",
			NameOfSecondDownSide: "Before",
			NameOfSecondUpSide: "Behind",
			PreferredLabelSide: CSAxisSide.FirstDown,
			PreferredTickSide: CSAxisSide.FirstDown,
			IsShownByDefault: false,
			HasTicksByDefault: false,
			HasLabelsByDefault: false,
			HasTitleByDefault: false);

			_axisStyleInformation.Add(info);
		}

		private static readonly string[,] _axisNamesNormal = new string[,] {
		{ "Above", "Below", "Behind", "Before" },
		{ "Right", "Left",  "Behind", "Before" },
		{ "Right", "Left",  "Above", "Below" }
		};

		/// <summary>Gets the name of the axis side.</summary>
		/// <param name="id">The axis identifier.</param>
		/// <param name="side">The axis side.</param>
		/// <returns>The name of the axis side for the axis line given by the identifier.</returns>
		public override string GetAxisSideName(CSLineID id, CSAxisSide side)
		{
			return _axisNamesNormal[id.ParallelAxisNumber, (int)side];
		}

		public override object Clone()
		{
			G3DCartesicCoordinateSystem result = new G3DCartesicCoordinateSystem();
			result.CopyFrom(this);
			return result;
		}

		public override Region GetRegion()
		{
			return new Region(new RectangleF(0, 0, (float)_projectionMatrix.M11, (float)_projectionMatrix.M22));
		}
	}
}
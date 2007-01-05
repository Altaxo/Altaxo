#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace Altaxo.Graph.Gdi.CS
{
  class G3DCartesicCoordinateSystem : G2DCoordinateSystem
  {
    struct TMatrix
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

    TMatrix _projectionMatrix;


    /// <summary>
    /// Copies the member variables from another coordinate system.
    /// </summary>
    /// <param name="fromb">The coordinate system to copy from.</param>
    public override void CopyFrom(G2DCoordinateSystem fromb)
    {
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
    public override void UpdateAreaSize(System.Drawing.SizeF size)
    {
      base.UpdateAreaSize(size);
      UpdateProjectionMatrix();
    }

    void UpdateProjectionMatrix()
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
      LogicalToLayerCoordinates(Logical3D.Interpolate(r0,r1,t),out ax, out ay);

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
      int horzAx=0;
      int vertAx=1;
      int deptAx=2;

     
      if (null == _axisStyleInformation)
        _axisStyleInformation = new List<CSAxisInformation>();
      else
        _axisStyleInformation.Clear();

      CSAxisInformation info;

      // BottomFront
      info = new CSAxisInformation(new CSLineID(horzAx, 0, 0));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "BottomFront";
      info.NameOfFirstDownSide = "Below";
      info.NameOfFirstUpSide =   "Above";
      info.NameOfSecondDownSide = "Before";
      info.NameOfSecondUpSide = "Behind";
      info.PreferedLabelSide =  CSAxisSide.FirstDown;
      info.IsShownByDefault = true;
      info.HasTitleByDefault = true;

      // TopFront
      info = new CSAxisInformation(new CSLineID(horzAx, 1 , 0));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "TopFront";
      info.NameOfFirstDownSide = "Below";
      info.NameOfFirstUpSide =  "Above";
      info.NameOfSecondDownSide = "Before";
      info.NameOfSecondUpSide = "Behind";
      info.PreferedLabelSide = CSAxisSide.FirstUp;
      info.IsShownByDefault = true;



      // LeftFront
      info = new CSAxisInformation(new CSLineID(vertAx,0,0));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "LeftFront";
      info.NameOfFirstDownSide = "Left";
      info.NameOfFirstUpSide = "Right";
      info.NameOfSecondDownSide = "Before";
      info.NameOfSecondUpSide = "Behind";
      info.PreferedLabelSide = CSAxisSide.FirstDown;
      info.IsShownByDefault = true;
      info.HasTitleByDefault = true;


      // RightFront
      info = new CSAxisInformation(new CSLineID(vertAx, 1 , 0));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "RightFront";
      info.NameOfFirstDownSide = "Left";
      info.NameOfFirstUpSide =  "Right";
      info.NameOfSecondDownSide = "Before";
      info.NameOfSecondUpSide = "Behind";
      info.PreferedLabelSide =  CSAxisSide.FirstUp;
      info.IsShownByDefault = true;


      // BottomBack
      info = new CSAxisInformation(new CSLineID(horzAx, 0, 1));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "BottomBack";
      info.NameOfFirstDownSide = "Below";
      info.NameOfFirstUpSide = "Above";
      info.NameOfSecondDownSide = "Before";
      info.NameOfSecondUpSide = "Behind";
      info.PreferedLabelSide = CSAxisSide.FirstDown;
      info.IsShownByDefault = true;
      info.HasTitleByDefault = true;

      // TopBack
      info = new CSAxisInformation(new CSLineID(horzAx, 1, 1));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "TopBack";
      info.NameOfFirstDownSide = "Below";
      info.NameOfFirstUpSide = "Above";
      info.NameOfSecondDownSide = "Before";
      info.NameOfSecondUpSide = "Behind";
      info.PreferedLabelSide = CSAxisSide.FirstUp;
      info.IsShownByDefault = true;

      // LeftBack
      info = new CSAxisInformation(new CSLineID(vertAx, 0, 1));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "LeftBack";
      info.NameOfFirstDownSide = "Left";
      info.NameOfFirstUpSide = "Right";
      info.NameOfSecondDownSide = "Before";
      info.NameOfSecondUpSide = "Behind";
      info.PreferedLabelSide = CSAxisSide.FirstDown;
      info.IsShownByDefault = true;
      info.HasTitleByDefault = true;


      // RightBack
      info = new CSAxisInformation(new CSLineID(vertAx, 1, 1));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "RightBack";
      info.NameOfFirstDownSide = "Left";
      info.NameOfFirstUpSide = "Right";
      info.NameOfSecondDownSide = "Before";
      info.NameOfSecondUpSide = "Behind";
      info.PreferedLabelSide = CSAxisSide.FirstUp;
      info.IsShownByDefault = true;




      // BottomLeft
      info = new CSAxisInformation(new CSLineID(deptAx, 0, 0));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "BottomLeft";
      info.NameOfFirstDownSide = "Left";
      info.NameOfFirstUpSide = "Right";
      info.NameOfSecondDownSide = "Below";
      info.NameOfSecondUpSide = "Above";
      info.PreferedLabelSide = CSAxisSide.FirstDown;
      info.IsShownByDefault = true;
      info.HasTitleByDefault = true;

      // TopLeft
      info = new CSAxisInformation(new CSLineID(deptAx, 0, 1));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "TopLeft";
      info.NameOfFirstDownSide = "Left";
      info.NameOfFirstUpSide = "Right";
      info.NameOfSecondDownSide = "Below";
      info.NameOfSecondUpSide = "Above";
      info.PreferedLabelSide = CSAxisSide.FirstUp;
      info.IsShownByDefault = true;



      // BottomRight
      info = new CSAxisInformation(new CSLineID(deptAx, 1, 0));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "BottomRight";
      info.NameOfFirstDownSide = "Left";
      info.NameOfFirstUpSide = "Right";
      info.NameOfSecondDownSide = "Below";
      info.NameOfSecondUpSide = "Above";
      info.PreferedLabelSide = CSAxisSide.FirstDown;
      info.IsShownByDefault = true;
      info.HasTitleByDefault = true;


      // TopRight
      info = new CSAxisInformation(new CSLineID(deptAx, 1, 1));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "TopRight";
      info.NameOfFirstDownSide = "Left";
      info.NameOfFirstUpSide = "Right";
      info.NameOfSecondDownSide = "Below";
      info.NameOfSecondUpSide = "Above";
      info.PreferedLabelSide = CSAxisSide.FirstUp;
      info.IsShownByDefault = true;














      // XAxis: Y=0, Z=0
      info = new CSAxisInformation(CSLineID.FromPhysicalValue(horzAx, 0));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "YZ=0";
      info.NameOfFirstUpSide =  "Above";
      info.NameOfFirstDownSide =  "Below";
      info.NameOfSecondDownSide = "Before";
      info.NameOfSecondUpSide = "Behind";
      info.PreferedLabelSide = CSAxisSide.FirstDown;

      // YAxis: X=0, Z=0
      info = new CSAxisInformation(CSLineID.FromPhysicalValue(vertAx, 0));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "XZ=0";
      info.NameOfFirstDownSide =  "Left";
      info.NameOfFirstUpSide = "Right";
      info.NameOfSecondDownSide = "Before";
      info.NameOfSecondUpSide = "Behind";
      info.PreferedLabelSide =  CSAxisSide.FirstDown;

      // ZAxis: X=0,Y=0
      info = new CSAxisInformation(CSLineID.FromPhysicalValue(deptAx, 0));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "XY=0";
      info.NameOfFirstDownSide =  "Left";
      info.NameOfFirstUpSide =  "Right";
      info.NameOfSecondDownSide = "Before";
      info.NameOfSecondUpSide = "Behind";
      info.PreferedLabelSide =  CSAxisSide.FirstDown;


    }


    public override object Clone()
    {
      G3DCartesicCoordinateSystem result = new G3DCartesicCoordinateSystem();
      result.CopyFrom(this);
      return result;
    }

    public override Region GetRegion()
    {
      return new Region(new RectangleF(0,0,(float)_projectionMatrix.M11,(float)_projectionMatrix.M22));
    }
  }
}

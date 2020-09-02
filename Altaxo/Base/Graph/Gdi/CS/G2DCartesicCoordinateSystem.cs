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

#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Altaxo.Geometry;

namespace Altaxo.Graph.Gdi.CS
{
  [Serializable]
  public class G2DCartesicCoordinateSystem : G2DCoordinateSystem
  {
    /// <summary>
    /// Is the normal position of x and y axes interchanged, for instance x is vertical and y horizontal.
    /// </summary>
    private bool _isXYInterchanged;

    /// <summary>
    /// Is the direction of the x axis reverse, for instance runs from right to left.
    /// </summary>
    protected bool _isXreverse;

    /// <summary>
    /// Is the direction of the y axis reverse, for instance runs from top to bottom.
    /// </summary>
    protected bool _isYreverse;

    /// <summary>
    /// Copies the member variables from another coordinate system.
    /// </summary>
    /// <param name="fromb">The coordinate system to copy from.</param>
    public override void CopyFrom(G2DCoordinateSystem fromb)
    {
      if (object.ReferenceEquals(this, fromb))
        return;

      base.CopyFrom(fromb);
      if (fromb is G2DCartesicCoordinateSystem)
      {
        var from = (G2DCartesicCoordinateSystem)fromb;
        _isXYInterchanged = from._isXYInterchanged;
        _isXreverse = from._isXreverse;
        _isYreverse = from._isYreverse;
      }
    }

    #region Serialization

    #region Version 0

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(G2DCartesicCoordinateSystem), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (G2DCartesicCoordinateSystem)obj;

        info.AddValue("XYInterchanged", s.IsXYInterchanged);
        info.AddValue("XReverse", s._isXreverse);
        info.AddValue("YReverse", s._isYreverse);
      }

      protected virtual G2DCartesicCoordinateSystem SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (G2DCartesicCoordinateSystem?)o ?? new G2DCartesicCoordinateSystem();

        s.IsXYInterchanged = info.GetBoolean("XYInterchanged");
        s._isXreverse = info.GetBoolean("XReverse");
        s._isYreverse = info.GetBoolean("YReverse");

        return s;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        G2DCartesicCoordinateSystem s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Version 0

    #endregion Serialization

    /// <summary>
    /// Is the normal position of x and y axes interchanged, for instance x is vertical and y horizontal.
    /// </summary>
    public bool IsXYInterchanged
    {
      get { return _isXYInterchanged; }
      set
      {
        if (_isXYInterchanged != value)
        {
          _isXYInterchanged = value;
          ClearCachedObjects();
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Is the direction of the x axis reverse, for instance runs from right to left.
    /// </summary>
    public bool IsXReverse
    {
      get { return _isXreverse; }
      set
      {
        if (_isXreverse != value)
        {
          _isXreverse = value;
          ClearCachedObjects();
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Is the direction of the y axis reverse, for instance runs from top to bottom.
    /// </summary>
    public bool IsYReverse
    {
      get { return _isYreverse; }
      set
      {
        if (_isYreverse != value)
        {
          _isYreverse = value;
          ClearCachedObjects();
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Returns true if the plot area is orthogonal, i.e. if the x and the y axis are orthogonal to each other.
    /// </summary>
    public override bool IsOrthogonal { get { return true; } }

    /// <summary>
    /// Returns true if the plot coordinates can be calculated as a linear transformation of the physical values.
    /// Returns false if this is for instance a polar diagram.
    /// </summary>
    public override bool IsAffine { get { return true; } }

    /// <summary>
    /// Returns true when this is a 3D coordinate system. Returns false in all other cases.
    /// </summary>
    public override bool Is3D { get { return false; } }

    protected override void UpdateAxisInfo()
    {
      int horzAx;
      int vertAx;
      bool vertRev;
      bool horzRev;

      if (_isXYInterchanged)
      {
        horzAx = 1;
        vertAx = 0;
        vertRev = _isXreverse;
        horzRev = _isYreverse;
      }
      else
      {
        horzAx = 0;
        vertAx = 1;
        vertRev = _isYreverse;
        horzRev = _isXreverse;
      }

      if (_axisStyleInformation is null)
        _axisStyleInformation = new List<CSAxisInformation>();
      else
        _axisStyleInformation.Clear();

      CSAxisInformation info;
      CSLineID lineID;

      // Left
      lineID = new CSLineID(vertAx, horzRev ? 1 : 0);
      info = new CSAxisInformation(
        Identifier: lineID,
        NameOfAxisStyle: GetAxisName(lineID),
        NameOfFirstDownSide: GetAxisSideName(lineID, CSAxisSide.FirstDown),
        NameOfFirstUpSide: GetAxisSideName(lineID, CSAxisSide.FirstUp),
        PreferredLabelSide: horzRev ? CSAxisSide.FirstUp : CSAxisSide.FirstDown,
        IsShownByDefault: true,
        HasTitleByDefault: true);
      _axisStyleInformation.Add(info);

      // Right
      lineID = new CSLineID(vertAx, horzRev ? 0 : 1);
      info = new CSAxisInformation(
        Identifier: lineID,
        NameOfAxisStyle: GetAxisName(lineID),
        NameOfFirstDownSide: GetAxisSideName(lineID, CSAxisSide.FirstDown),
        NameOfFirstUpSide: GetAxisSideName(lineID, CSAxisSide.FirstUp),
        PreferredLabelSide: horzRev ? CSAxisSide.FirstDown : CSAxisSide.FirstUp,
        IsShownByDefault: false,
        HasTitleByDefault: false);
      _axisStyleInformation.Add(info);

      // Bottom
      lineID = new CSLineID(horzAx, vertRev ? 1 : 0);
      info = new CSAxisInformation(
        Identifier: lineID,
        NameOfAxisStyle: GetAxisName(lineID),
        NameOfFirstDownSide: GetAxisSideName(lineID, CSAxisSide.FirstDown),
        NameOfFirstUpSide: GetAxisSideName(lineID, CSAxisSide.FirstUp),
        PreferredLabelSide: vertRev ? CSAxisSide.FirstUp : CSAxisSide.FirstDown,
        IsShownByDefault: true,
        HasTitleByDefault: true);
      _axisStyleInformation.Add(info);

      // Top
      lineID = new CSLineID(horzAx, vertRev ? 0 : 1);
      info = new CSAxisInformation(
        Identifier: lineID,
        NameOfAxisStyle: GetAxisName(lineID),
        NameOfFirstDownSide: GetAxisSideName(lineID, CSAxisSide.FirstDown),
        NameOfFirstUpSide: GetAxisSideName(lineID, CSAxisSide.FirstUp),
        PreferredLabelSide: vertRev ? CSAxisSide.FirstDown : CSAxisSide.FirstUp,
        IsShownByDefault: false,
        HasTitleByDefault: false);
      _axisStyleInformation.Add(info);

      // Y=0
      lineID = CSLineID.FromPhysicalValue(horzAx, 0);
      info = new CSAxisInformation(
        Identifier: lineID,
        NameOfAxisStyle: GetAxisName(lineID),
        NameOfFirstDownSide: GetAxisSideName(lineID, CSAxisSide.FirstDown),
        NameOfFirstUpSide: GetAxisSideName(lineID, CSAxisSide.FirstUp),
        PreferredLabelSide: vertRev ? CSAxisSide.FirstUp : CSAxisSide.FirstDown,
        IsShownByDefault: false,
        HasTitleByDefault: false);
      _axisStyleInformation.Add(info);

      // X=0
      lineID = CSLineID.FromPhysicalValue(vertAx, 0);
      info = new CSAxisInformation(
        Identifier: lineID,
        NameOfAxisStyle: GetAxisName(lineID),
        NameOfFirstDownSide: GetAxisSideName(lineID, CSAxisSide.FirstDown),
        NameOfFirstUpSide: GetAxisSideName(lineID, CSAxisSide.FirstUp),
        PreferredLabelSide: horzRev ? CSAxisSide.FirstUp : CSAxisSide.FirstDown,
        IsShownByDefault: false,
        HasTitleByDefault: false);
      _axisStyleInformation.Add(info);
    }

    private static readonly string[,] _axisSideNamesNormal = new string[,] { { "Above", "Below" }, { "Right", "Left" } };
    private static readonly string[,] _axisSideNamesOuterLines = new string[,] { { "Out", "In" }, { "Out", "In" } };

    /// <summary>Gets the name of the axis side.</summary>
    /// <param name="id">The axis identifier.</param>
    /// <param name="side">The axis side.</param>
    /// <returns>The name of the axis side for the axis line given by the identifier.</returns>
    public override string GetAxisSideName(CSLineID id, CSAxisSide side)
    {
      bool isHorizontal = id.ParallelAxisNumber == 0 ^ _isXYInterchanged;
      bool isOuterLine = (id.LogicalValueOtherFirst == 0 || id.LogicalValueOtherFirst == 1);

      if (isOuterLine)
      {
        bool isPointingOutwards = (id.LogicalValueOtherFirst == 0 && side == CSAxisSide.FirstDown) || (id.LogicalValueOtherFirst == 1 && side == CSAxisSide.FirstUp);
        return _axisSideNamesOuterLines[isHorizontal ? 0 : 1, isPointingOutwards ? 0 : 1];
      }
      else
      {
        bool isRight = ((id.ParallelAxisNumber == 0 && _isYreverse) || (id.ParallelAxisNumber == 1 && _isXreverse)) ^ (side == CSAxisSide.FirstUp);
        return _axisSideNamesNormal[isHorizontal ? 0 : 1, isRight ? 0 : 1];
      }
    }

    public string GetAxisName(CSLineID id)
    {
      if (id.Is3DIdentifier)
        throw new ArgumentException(nameof(id) + " is a 3D identifier, but here a 2D identifier is expected");

      if (id.UsePhysicalValueOtherFirst)
      {
        bool isX = id.ParallelAxisNumber == 0 ^ _isXYInterchanged;
        return string.Format("{0}={1}", isX ? "X" : "Y", id.PhysicalValueOtherFirst);
      }
      else // logical values
      {
        if (id.LogicalValueOtherFirst == 0 || id.LogicalValueOtherFirst == 1)
        {
          return GetAxisName_Logical0Or1(id);
        }
        else
        {
          if (id.LogicalValueOtherFirst <= 0.5)
          {
            var id1 = id.WithLogicalValueOtherFirst(0);
            var basename = GetAxisName_Logical0Or1(id1);
            return string.Format("{0} ({1}% offset)", basename, id.LogicalValueOtherFirst * 100);
          }
          else // id.LogicalValueOtherFirst>0.5)
          {
            var id1 = id.WithLogicalValueOtherFirst(1);
            var basename = GetAxisName_Logical0Or1(id1);
            return string.Format("{0} ({1}% offset)", basename, (id.LogicalValueOtherFirst - 1) * 100);
          }
        }
      }
    }

    private static readonly string[,] _axisNamesOuterLines = new string[,] { { "Bottom", "Top" }, { "Left", "Right" } };

    private string GetAxisName_Logical0Or1(CSLineID id)
    {
      bool isHorizontal = (id.ParallelAxisNumber == 0) ^ _isXYInterchanged;

      bool isLogical0 = (id.LogicalValueOtherFirst == 0) ^ ((id.ParallelAxisNumber == 1 && _isXreverse) || (id.ParallelAxisNumber == 0 && _isYreverse));

      return _axisNamesOuterLines[isHorizontal ? 0 : 1, isLogical0 ? 0 : 1];
    }

    private Matrix2x2 VectorTransformation
    {
      get
      {
        if (_isXYInterchanged)
          return new Matrix2x2(
            0, _isXreverse ? -1 : 1,
            _isYreverse ? -1 : 1, 0);
        else
          return new Matrix2x2(
            _isXreverse ? -1 : 1, 0,
            0, _isYreverse ? -1 : 1);
      }
    }

    public override string GetNameOfPlane(CSPlaneID planeId)
    {
      string name = "";
      if (planeId.UsePhysicalValue)
      {
        switch (planeId.PerpendicularAxisNumber)
        {
          case 0:
            name = string.Format("X = {0}", planeId.PhysicalValue);
            break;

          case 1:
            name = string.Format("Y = {0}", planeId.PhysicalValue);
            break;

          case 2:
            name = string.Format("Z = {0}", planeId.PhysicalValue);
            break;

          default:
            throw new NotImplementedException();
        }
      }
      else // use logical value
      {
        var uv = GetUntransformedAxisPlaneVector(planeId);
        var tv = VectorTransformation.Transform(uv);

        var lv = planeId.LogicalValue;

        if (tv.X == -1 || tv.Y == -1)
          lv = 1 - lv;
        if (Math.Abs(tv.X) == 1) // perpendicular vector in x-direction
        {
          if (lv == 0)
            name = "Left";
          else if (lv == 1)
            name = "Right";
          else
            name = string.Format("{0}% between left and right", lv * 100);
        }
        else if (Math.Abs(tv.Y) == 1) // perpendicular vector in y-direction
        {
          if (lv == 0)
            name = "Bottom";
          else if (lv == 1)
            name = "Top";
          else
            name = string.Format("{0}% between bottom and top", lv * 100);
        }
        else
        {
          throw new NotImplementedException();
        }
      }
      return name;
    }

    /// <summary>
    /// Calculates from two logical values (values between 0 and 1) the coordinates of the point. Returns true if the conversion
    /// is possible, otherwise false.
    /// </summary>
    /// <param name="r">The logical coordinates to convert.</param>
    /// <param name="xlocation">On return, gives the x coordinate of the converted value (for instance location).</param>
    /// <param name="ylocation">On return, gives the y coordinate of the converted value (for instance location).</param>
    /// <returns>True if the conversion was successfull, false if the conversion was not possible.</returns>
    public override bool LogicalToLayerCoordinates(Logical3D r, out double xlocation, out double ylocation)
    {
      if (_isXreverse)
        r.RX = 1 - r.RX;
      if (_isYreverse)
        r.RY = 1 - r.RY;
      if (_isXYInterchanged)
      {
        double hr = r.RX;
        r.RX = r.RY;
        r.RY = hr;
      }

      xlocation = _layerWidth * r.RX;
      ylocation = _layerHeight * (1 - r.RY);
      return !double.IsNaN(xlocation) && !double.IsNaN(ylocation);
    }

    public override bool LogicalToLayerCoordinatesAndDirection(
     Logical3D r0, Logical3D r1,
     double t,
     out double ax, out double ay, out double adx, out double ady)
    {
      if (_isXreverse)
      {
        r0.RX = 1 - r0.RX;
        r1.RX = 1 - r1.RX;
      }
      if (_isYreverse)
      {
        r0.RY = 1 - r0.RY;
        r1.RY = 1 - r1.RY;
      }
      if (_isXYInterchanged)
      {
        double hr0 = r0.RX;
        r0.RX = r0.RY;
        r0.RY = hr0;

        double hr1 = r1.RX;
        r1.RX = r1.RY;
        r1.RY = hr1;
      }

      double rx = r0.RX + t * (r1.RX - r0.RX);
      double ry = r0.RY + t * (r1.RY - r0.RY);

      ax = _layerWidth * rx;
      ay = _layerHeight * (1 - ry);

      adx = _layerWidth * (r1.RX - r0.RX);
      ady = _layerHeight * (r0.RY - r1.RY);

      return !double.IsNaN(ax) && !double.IsNaN(ay);
    }

    /// <summary>
    /// Calculates from two layer coordinate values (in points usually) the relative coordinates of the point (between 0 and 1). Returns true if the conversion
    /// is possible, otherwise false.
    /// </summary>
    /// <param name="xlocation">On return, gives the x coordinate of the converted value (for instance location).</param>
    /// <param name="ylocation">On return, gives the y coordinate of the converted value (for instance location).</param>
    /// <param name="r">The logical coordinates as the result of conversion.</param>
    /// <returns>True if the conversion was successfull, false if the conversion was not possible.</returns>
    public override bool LayerToLogicalCoordinates(double xlocation, double ylocation, out Logical3D r)
    {
      r = new Logical3D(xlocation / _layerWidth, 1 - ylocation / _layerHeight);

      if (_isXYInterchanged)
      {
        double hr = r.RX;
        r.RX = r.RY;
        r.RY = hr;
      }

      if (_isXreverse)
        r.RX = 1 - r.RX;
      if (_isYreverse)
        r.RY = 1 - r.RY;

      return !double.IsNaN(r.RX) && !double.IsNaN(r.RY);
    }

    public override void GetIsoline(System.Drawing.Drawing2D.GraphicsPath g, Logical3D r0, Logical3D r1)
    {
      if (LogicalToLayerCoordinates(r0, out var ax0, out var ay0) && LogicalToLayerCoordinates(r1, out var ax1, out var ay1))
      {
        g.AddLine((float)ax0, (float)ay0, (float)ax1, (float)ay1);
      }
    }

    /// <summary>
    /// Get a region object, which describes the plotting area. Used to clip the plotting to
    /// the plotting area.
    /// </summary>
    /// <returns>A region object describing the plotting area.</returns>
    public override Region GetRegion()
    {
      return new Region(new RectangleF(0, 0, (float)_layerWidth, (float)_layerHeight));
    }

    public override object Clone()
    {
      var result = new G2DCartesicCoordinateSystem();
      result.CopyFrom(this);
      return result;
    }
  }
}

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
      base.CopyFrom(fromb);
      if (fromb is G2DCartesicCoordinateSystem)
      {
        G2DCartesicCoordinateSystem from = (G2DCartesicCoordinateSystem)fromb;
        this._isXYInterchanged = from._isXYInterchanged;
        this._isXreverse = from._isXreverse;
        this._isYreverse = from._isYreverse;
      }
    }

    #region Serialization
    #region Version 0
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(G2DCartesicCoordinateSystem), 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        G2DCartesicCoordinateSystem s = (G2DCartesicCoordinateSystem)obj;

        info.AddValue("XYInterchanged", s.IsXYInterchanged);
        info.AddValue("XReverse", s._isXreverse);
        info.AddValue("YReverse", s._isYreverse);
      }
      protected virtual G2DCartesicCoordinateSystem SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        G2DCartesicCoordinateSystem s = (o == null ? new G2DCartesicCoordinateSystem() : (G2DCartesicCoordinateSystem)o);

        s.IsXYInterchanged = info.GetBoolean("XYInterchanged");
        s._isXreverse = info.GetBoolean("XReverse");
        s._isYreverse = info.GetBoolean("YReverse");

        return s;
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        G2DCartesicCoordinateSystem s = SDeserialize(o, info, parent);
        return s;
      }
    }
    #endregion
    #endregion

    public G2DCartesicCoordinateSystem()
    {
    }

    /// <summary>
    /// Is the normal position of x and y axes interchanged, for instance x is vertical and y horizontal.
    /// </summary>
    public bool IsXYInterchanged
    {
      get { return _isXYInterchanged; }
      set { 
        _isXYInterchanged = value;
        ClearCachedObjects();
      }
    }

    /// <summary>
    /// Is the direction of the x axis reverse, for instance runs from right to left.
    /// </summary>
    public bool IsXReverse
    {
      get { return _isXreverse; }
      set { 
        _isXreverse = value;
        ClearCachedObjects();
      }

    }

    /// <summary>
    /// Is the direction of the y axis reverse, for instance runs from top to bottom.
    /// </summary>
    public bool IsYReverse
    {
      get { return _isYreverse; }
      set { 
        _isYreverse = value;
        ClearCachedObjects();
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

      if (null == _axisStyleInformation)
        _axisStyleInformation = new List<CSAxisInformation>();
      else
        _axisStyleInformation.Clear();

      CSAxisInformation info;

      // Left
      info = new CSAxisInformation(new CSLineID(vertAx, horzRev ? 1 : 0));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "Left";
      info.NameOfFirstDownSide = horzRev ? "Inner" : "Outer";
      info.NameOfFirstUpSide = horzRev ? "Outer" : "Inner";
      info.PreferedLabelSide = horzRev ? CSAxisSide.FirstUp : CSAxisSide.FirstDown;
      info.IsShownByDefault = true;
      info.HasTitleByDefault = true;


      // Right
      info = new CSAxisInformation(new CSLineID(vertAx, horzRev ? 0 : 1));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "Right";
      info.NameOfFirstDownSide = horzRev ? "Outer" : "Inner";
      info.NameOfFirstUpSide = horzRev ? "Inner" : "Outer";
      info.PreferedLabelSide = horzRev ? CSAxisSide.FirstDown : CSAxisSide.FirstUp;

      // Bottom
      info = new CSAxisInformation(new CSLineID(horzAx, vertRev ? 1 : 0));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "Bottom";
      info.NameOfFirstDownSide = vertRev ? "Inner" : "Outer";
      info.NameOfFirstUpSide = vertRev ?  "Outer" : "Inner";
      info.PreferedLabelSide = vertRev ? CSAxisSide.FirstUp : CSAxisSide.FirstDown;
      info.IsShownByDefault = true;
      info.HasTitleByDefault = true;

      // Top
      info = new CSAxisInformation(new CSLineID(horzAx, vertRev ? 0 : 1));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "Top";
      info.NameOfFirstDownSide = vertRev ? "Outer" : "Inner";
      info.NameOfFirstUpSide = vertRev ? "Inner" : "Outer";
      info.PreferedLabelSide = vertRev ? CSAxisSide.FirstDown : CSAxisSide.FirstUp;


      // Y=0
      info = new CSAxisInformation(CSLineID.FromPhysicalValue(horzAx, 0));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "Y=0";
      info.NameOfFirstUpSide = vertRev ? "Below" : "Above";
      info.NameOfFirstDownSide = vertRev ? "Above" : "Below";
      info.PreferedLabelSide = vertRev ? CSAxisSide.FirstUp : CSAxisSide.FirstDown;

      // X=0
      info = new CSAxisInformation(CSLineID.FromPhysicalValue(vertAx, 0));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "X=0";
      info.NameOfFirstDownSide = horzRev ? "Right" : "Left";
      info.NameOfFirstUpSide = horzRev ? "Left" : "Right";
      info.PreferedLabelSide = horzRev ? CSAxisSide.FirstUp : CSAxisSide.FirstDown;



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

      return !double.IsNaN(r.RX) && !double.IsNaN(r.RY);
    }

    public override void GetIsoline(System.Drawing.Drawing2D.GraphicsPath g, Logical3D r0, Logical3D r1)
    {
      double ax0, ax1, ay0, ay1;
      if (LogicalToLayerCoordinates(r0, out ax0, out ay0) && LogicalToLayerCoordinates(r1, out ax1, out ay1))
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
      G2DCartesicCoordinateSystem result = new G2DCartesicCoordinateSystem();
      result.CopyFrom(this);
      return result;
    }
  }
}

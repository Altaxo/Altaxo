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
    /// <param name="from">The coordinate system to copy from.</param>
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
    public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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
        _axisStyleInformation = new List<A2DAxisStyleInformation>();
      else
        _axisStyleInformation.Clear();

      A2DAxisStyleInformation info;

      // Left
      info = new A2DAxisStyleInformation(new CSLineID(vertAx, horzRev ? 1 : 0));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "Left";
      info.NameOfLeftSide = vertRev ? "Inner" : "Outer";
      info.NameOfRightSide = vertRev ? "Outer" : "Inner";
      info.PreferedLabelSide = vertRev ? A2DAxisSide.Right : A2DAxisSide.Left;
      info.IsShownByDefault = true;
      info.HasTitleByDefault = true;


      // Right
      info = new A2DAxisStyleInformation(new CSLineID(vertAx, horzRev ? 0 : 1));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "Right";
      info.NameOfLeftSide = vertRev ? "Outer" : "Inner";
      info.NameOfRightSide = vertRev ? "Inner" : "Outer";
      info.PreferedLabelSide = vertRev ? A2DAxisSide.Left : A2DAxisSide.Right;

      // Bottom
      info = new A2DAxisStyleInformation(new CSLineID(horzAx, vertRev ? 1 : 0));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "Bottom";
      info.NameOfLeftSide = horzRev ? "Outer" : "Inner";
      info.NameOfRightSide = horzRev ? "Inner" : "Outer";
      info.PreferedLabelSide = horzRev ? A2DAxisSide.Left : A2DAxisSide.Right;
      info.IsShownByDefault = true;
      info.HasTitleByDefault = true;

      // Top
      info = new A2DAxisStyleInformation(new CSLineID(horzAx, vertRev ? 0 : 1));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "Top";
      info.NameOfLeftSide = horzRev ? "Inner" : "Outer";
      info.NameOfRightSide = horzRev ? "Outer" : "Inner";
      info.PreferedLabelSide = horzRev ? A2DAxisSide.Right : A2DAxisSide.Left;


      // Y=0
      info = new A2DAxisStyleInformation(CSLineID.FromPhysicalValue(horzAx, 0));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "Y=0";
      info.NameOfLeftSide = horzRev ? "Below" : "Above";
      info.NameOfRightSide = horzRev ? "Above" : "Below";
      info.PreferedLabelSide = horzRev ? A2DAxisSide.Left : A2DAxisSide.Right;

      // X=0
      info = new A2DAxisStyleInformation(CSLineID.FromPhysicalValue(vertAx, 0));
      _axisStyleInformation.Add(info);
      info.NameOfAxisStyle = "X=0";
      info.NameOfLeftSide = vertRev ? "Right" : "Left";
      info.NameOfRightSide = vertRev ? "Left" : "Right";
      info.PreferedLabelSide = vertRev ? A2DAxisSide.Right : A2DAxisSide.Left;



    }

    /// <summary>
    /// Calculates from two logical values (values between 0 and 1) the coordinates of the point. Returns true if the conversion
    /// is possible, otherwise false.
    /// </summary>
    /// <param name="rx">The logical x value.</param>
    /// <param name="ry">The logical y value.</param>
    /// <param name="xlocation">On return, gives the x coordinate of the converted value (for instance location).</param>
    /// <param name="ylocation">On return, gives the y coordinate of the converted value (for instance location).</param>
    /// <returns>True if the conversion was successfull, false if the conversion was not possible.</returns>
    public override bool LogicalToLayerCoordinates(double rx, double ry, out double xlocation, out double ylocation)
    {
      if (_isXreverse)
        rx = 1 - rx;
      if (_isYreverse)
        ry = 1 - ry;
      if (_isXYInterchanged)
      {
        double hr = rx;
        rx = ry;
        ry = hr;
      }


      xlocation = _layerWidth * rx;
      ylocation = _layerHeight * (1 - ry);
      return !double.IsNaN(xlocation) && !double.IsNaN(ylocation);
    }


    public override bool LogicalToLayerCoordinatesAndDirection(
     double rx0, double ry0, double rx1, double ry1,
     double t,
     out double ax, out double ay, out double adx, out double ady)
    {
      if (_isXreverse)
      {
        rx0 = 1 - rx0;
        rx1 = 1 - rx1;
      }
      if (_isYreverse)
      {
        ry0 = 1 - ry0;
        ry1 = 1 - ry1;
      }
      if (_isXYInterchanged)
      {
        double hr0 = rx0;
        rx0 = ry0;
        ry0 = hr0;

        double hr1 = rx1;
        rx1 = ry1;
        ry1 = hr1;
      }

      double rx = rx0 + t * (rx1 - rx0);
      double ry = ry0 + t * (ry1 - ry0);


      ax = _layerWidth * rx;
      ay = _layerHeight * (1 - ry);

      adx = _layerWidth * (rx1 - rx0);
      ady = _layerHeight * (ry0 - ry1);

      return !double.IsNaN(ax) && !double.IsNaN(ay);
    }




    /// <summary>
    /// Calculates from two layer coordinate values (in points usually) the relative coordinates of the point (between 0 and 1). Returns true if the conversion
    /// is possible, otherwise false.
    /// </summary>
    /// <param name="xlocation">On return, gives the x coordinate of the converted value (for instance location).</param>
    /// <param name="ylocation">On return, gives the y coordinate of the converted value (for instance location).</param>
    /// <param name="rx">The logical x value.</param>
    /// <param name="ry">The logical y value.</param>
    /// <returns>True if the conversion was successfull, false if the conversion was not possible.</returns>
    public override bool LayerToLogicalCoordinates(double xlocation, double ylocation, out double rx, out double ry)
    {
      rx = xlocation / _layerWidth;
      ry = 1 - ylocation / _layerHeight;

      if (_isXreverse)
        rx = 1 - rx;
      if (_isYreverse)
        ry = 1 - ry;
      if (_isXYInterchanged)
      {
        double hr = rx;
        rx = ry;
        ry = hr;
      }

      return !double.IsNaN(rx) && !double.IsNaN(ry);
    }

    public override void GetIsoline(System.Drawing.Drawing2D.GraphicsPath g, double rx0, double ry0, double rx1, double ry1)
    {
      double ax0, ax1, ay0, ay1;
      if (LogicalToLayerCoordinates(rx0, ry0, out ax0, out ay0) && LogicalToLayerCoordinates(rx1, ry1, out ax1, out ay1))
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

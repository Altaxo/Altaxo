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
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;

namespace Altaxo.Graph.Gdi
{
  [Serializable]
  public class XYPlotLayerPositionAndSize 
    :
    System.ICloneable,
    Altaxo.Main.IChangedEventSource,
    Altaxo.Main.IDocumentNode

  {
    /// <summary>
    /// The layers x position value, either absolute or relative, as determined by <see cref="_layerXPositionType"/>.
    /// </summary>
    private double _layerXPosition = 0;
    /// <summary>
    /// The type of the x position value, see <see cref="XYPlotLayerPositionType"/>.
    /// </summary>
    private XYPlotLayerPositionType _layerXPositionType = XYPlotLayerPositionType.AbsoluteValue;

    /// <summary>
    /// The layers y position value, either absolute or relative, as determined by <see cref="_layerYPositionType"/>.
    /// </summary>
    private double _layerYPosition = 0;
    /// <summary>
    /// The type of the y position value, see <see cref="XYPlotLayerPositionType"/>.
    /// </summary>
    private XYPlotLayerPositionType _layerYPositionType = XYPlotLayerPositionType.AbsoluteValue;

    /// <summary>
    /// The width of the layer, either as absolute value in point (1/72 inch), or as 
    /// relative value as pointed out by <see cref="_layerWidthType"/>.
    /// </summary>
    private double _layerWidth = 0;
    /// <summary>
    /// The type of the value for the layer width, see <see cref="XYPlotLayerSizeType"/>.
    /// </summary>
    private XYPlotLayerSizeType _layerWidthType = XYPlotLayerSizeType.AbsoluteValue;

    /// <summary>
    /// The height of the layer, either as absolute value in point (1/72 inch), or as 
    /// relative value as pointed out by <see cref="_layerHeightType"/>.
    /// </summary>
    private double _layerHeight = 0;
    /// <summary>
    /// The type of the value for the layer height, see <see cref="XYPlotLayerSizeType"/>.
    /// </summary>
    private XYPlotLayerSizeType _layerHeightType = XYPlotLayerSizeType.AbsoluteValue;

    /// <summary>The rotation angle (in degrees) of the layer.</summary>
    private double _layerAngle = 0; // Rotation

    /// <summary>The scaling factor of the layer, normally 1.</summary>
    private double _layerScale = 1;  // Scale

    [field: NonSerialized]
    event EventHandler _changed;

    [NonSerialized]
    object _parent;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.XYPlotLayerPositionAndSize", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLayerPositionAndSize), 1)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYPlotLayerPositionAndSize s = (XYPlotLayerPositionAndSize)obj;

        info.AddValue("Width", s._layerWidth);
        info.AddEnum("WidthType", s._layerWidthType);
        info.AddValue("Height", s._layerHeight);
        info.AddEnum("HeightType", s._layerHeightType);
        info.AddValue("Angle", s._layerAngle);
        info.AddValue("Scale", s._layerScale);

        info.AddValue("XPos", s._layerXPosition);
        info.AddEnum("XPosType", s._layerXPositionType);
        info.AddValue("YPos", s._layerYPosition);
        info.AddEnum("YPosType", s._layerYPositionType);
      }

      protected virtual XYPlotLayerPositionAndSize SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYPlotLayerPositionAndSize s = null != o ? (XYPlotLayerPositionAndSize)o : new XYPlotLayerPositionAndSize();

        s._layerWidth = info.GetDouble("Width");
        s._layerWidthType = (XYPlotLayerSizeType)info.GetEnum("WidthType", typeof(XYPlotLayerSizeType));
        s._layerHeight = info.GetDouble("Height");
        s._layerHeightType = (XYPlotLayerSizeType)info.GetEnum("HeightType", typeof(XYPlotLayerSizeType));
        s._layerAngle = info.GetDouble("Angle");
        s._layerScale = info.GetDouble("Scale");

        s._layerXPosition = info.GetDouble("XPos");
        s._layerXPositionType = (XYPlotLayerPositionType)info.GetEnum("XPosType", typeof(XYPlotLayerPositionType));
        s._layerYPosition = info.GetDouble("YPos");
        s._layerYPositionType = (XYPlotLayerPositionType)info.GetEnum("YPosType", typeof(XYPlotLayerPositionType));

        return s;
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYPlotLayerPositionAndSize s = SDeserialize(o, info, parent);
        return s;
      }


      
    }
    #endregion

    public XYPlotLayerPositionAndSize()
    {
    }

    public XYPlotLayerPositionAndSize(XYPlotLayerPositionAndSize from)
    {
      CopyFrom(from);
    }


    public void CopyFrom(XYPlotLayerPositionAndSize from)
    {
      this._layerXPosition = from._layerXPosition;
      this._layerXPositionType = from._layerXPositionType;
      this._layerYPosition = from._layerYPosition;
      this._layerYPositionType = from._layerYPositionType;

      this._layerWidth = from._layerWidth;
      this._layerWidthType = from._layerWidthType;
      this._layerHeight = from._layerHeight;
      this._layerHeightType = from._layerHeightType;

      this._layerAngle = from._layerAngle;
      this._layerScale = from._layerScale;
    }

    object System.ICloneable.Clone()
    {
      return new XYPlotLayerPositionAndSize(this);
    }

    public XYPlotLayerPositionAndSize Clone()
    {
      return new XYPlotLayerPositionAndSize(this);
    }

    /// <summary>
    /// The layers x position value, either absolute or relative, as determined by <see cref="_layerXPositionType"/>.
    /// </summary>
    public double XPosition
    {
      get { return _layerXPosition; }
      set 
      {
        double oldvalue = _layerXPosition;
        _layerXPosition = value;

        if (value != oldvalue)
          OnChanged();

      }
    }

   

    /// <summary>
    /// The type of the x position value, see <see cref="XYPlotLayerPositionType"/>.
    /// </summary>
    public XYPlotLayerPositionType XPositionType
    {
      get { return _layerXPositionType; }
      set 
      {
        XYPlotLayerPositionType oldvalue = _layerXPositionType;
        _layerXPositionType = value;

        if (value != oldvalue)
          OnChanged();

      }
    }

   

    /// <summary>
    /// The layers y position value, either absolute or relative, as determined by <see cref="_layerYPositionType"/>.
    /// </summary>
    public double YPosition
    {
      get { return _layerYPosition; }
      set 
      {
        double oldvalue = _layerYPosition;
        _layerYPosition = value;

        if (value != oldvalue)
          OnChanged();

      }
    }

 
    /// <summary>
    /// The type of the y position value, see <see cref="XYPlotLayerPositionType"/>.
    /// </summary>
    public XYPlotLayerPositionType YPositionType
    {
      get { return _layerYPositionType; }
      set 
      {
        XYPlotLayerPositionType oldvalue = _layerYPositionType;
        _layerYPositionType = value;

        if (value != oldvalue)
          OnChanged();

      }
    }


  

    /// <summary>
    /// The width of the layer, either as absolute value in point (1/72 inch), or as 
    /// relative value as pointed out by <see cref="_layerWidthType"/>.
    /// </summary>
    public double Width
    {
      get { return _layerWidth; }
      set
      {
        double oldvalue = _layerWidth;
        _layerWidth = value;

        if (value != oldvalue)
          OnChanged();

      }
    }

  

    /// <summary>
    /// The type of the value for the layer width, see <see cref="XYPlotLayerSizeType"/>.
    /// </summary>
    public XYPlotLayerSizeType WidthType
    {
      get { return _layerWidthType; }
      set 
      {
        XYPlotLayerSizeType oldvalue = _layerWidthType;
        _layerWidthType = value;

        if (value != oldvalue)
          OnChanged();

      }
    }

   

    /// <summary>
    /// The height of the layer, either as absolute value in point (1/72 inch), or as 
    /// relative value as pointed out by <see cref="_layerHeightType"/>.
    /// </summary>
    public double Height
    {
      get { return _layerHeight; }
      set 
      {
        double oldvalue = _layerHeight;
        _layerHeight = value;

        if (value != oldvalue)
          OnChanged();

      }
    }

   
    /// <summary>
    /// The type of the value for the layer height, see <see cref="XYPlotLayerSizeType"/>.
    /// </summary>
    public XYPlotLayerSizeType HeightType
    {
      get { return _layerHeightType; }
      set 
      {
        XYPlotLayerSizeType oldvalue = _layerHeightType;
        _layerHeightType = value;

        if (value != oldvalue)
          OnChanged();

      }
    }

  
    /// <summary>The rotation angle (in degrees) of the layer.</summary>
    public double Angle
    {
      get { return _layerAngle; }
      set 
      {
        double oldvalue = _layerAngle;
        _layerAngle = value;

        if (value != oldvalue)
          OnChanged();

      }
    }

   

    /// <summary>The scaling factor of the layer, normally 1.</summary>
    public double Scale
    {
      get { return _layerScale; }
      set
      {
        double oldvalue = _layerScale;
        _layerScale = value;
        if (value != oldvalue)
          OnChanged();
      }
    }





    #region IChangedEventSource Members

    event EventHandler Altaxo.Main.IChangedEventSource.Changed
    {
      add { _changed += value; }
      remove { _changed -= value; }
    }


    protected virtual void OnChanged()
    {
      if (_parent is Main.IChildChangedEventSink)
        ((Main.IChildChangedEventSink)_parent).EhChildChanged(this, EventArgs.Empty);

      if (null != _changed)
        _changed(this, EventArgs.Empty);
    }

    #endregion

    #region IDocumentNode Members

    public object ParentObject
    {
      get { return _parent; }
      set { _parent = value; }
    }

    public string Name
    {
      get { return "Layer Position and Size"; }
    }

    #endregion


  
  }
}

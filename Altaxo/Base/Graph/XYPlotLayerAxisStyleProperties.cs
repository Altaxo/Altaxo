#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
using Altaxo.Graph.Axes;
using Altaxo.Graph.Axes.Boundaries;

namespace Altaxo.Graph
{
  /// <summary>
  /// This class summarizes all members that are belonging to one edge of the layer.
  /// </summary>
  public class XYPlotLayerAxisStyleProperties : Main.IChangedEventSource, Main.IChildChangedEventSink, ICloneable
  {
    /// <summary>Type of the axis. Determines the orientation of labels and ticks.</summary>
    EdgeType _edgeType;

    /// <summary>True if the axis line and ticks and labels should be drawn.</summary>
    bool _showAxis;

    /// <summary>Style of axis. Determines the line width and color of the axis and the ticks.</summary>
    protected XYAxisStyle _axisStyle;
    /// <summary>
    /// If true, the major labels will be shown.
    /// </summary>
    bool _showMajorLabels;
    /// <summary>
    /// Determines the style of the major labels.
    /// </summary>
    AbstractXYAxisLabelStyle _majorLabelStyle;
    /// <summary>
    /// If true, the minor labels will be shown.
    /// </summary>
    bool _showMinorLabels;
    /// <summary>
    /// Determines the style of the minor labels.
    /// </summary>
    AbstractXYAxisLabelStyle _minorLabelStyle;
    /// <summary>
    /// The title of the axis.
    /// </summary>
    TextGraphics _axisTitle;


    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLayerAxisStyleProperties), 0)]
    public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYPlotLayerAxisStyleProperties s = (XYPlotLayerAxisStyleProperties)obj;

        info.AddValue("ShowAxis", s._showAxis);
        info.AddValue("Edge", s._edgeType);
        info.AddValue("AxisStyle", s._axisStyle);
        info.AddValue("ShowMajorLabels", s._showMajorLabels);
        if(s._showMajorLabels)
          info.AddValue("MajorLabelStyle", s._majorLabelStyle);
        info.AddValue("ShowMinorLabels", s._showMinorLabels);
        if(s._showMinorLabels)
          info.AddValue("MinorLabelStyle", s._minorLabelStyle);
        info.AddValue("AxisTitle", s._axisTitle);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYPlotLayerAxisStyleProperties s = SDeserialize(o, info, parent);
        return s;
      }


      protected virtual XYPlotLayerAxisStyleProperties SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        XYPlotLayerAxisStyleProperties s = null != o ? (XYPlotLayerAxisStyleProperties)o : new XYPlotLayerAxisStyleProperties();

        // Styles
        s._showAxis = info.GetBoolean("ShowAxis");
        s._edgeType = (EdgeType)info.GetEnum("Edge", typeof(EdgeType));
        s.AxisStyle = (XYAxisStyle)info.GetValue("AxisStyle", s);
        s._showMajorLabels = info.GetBoolean("ShowMajorLabels");
        if(s._showMajorLabels)
          s.MajorLabelStyle = (AbstractXYAxisLabelStyle)info.GetValue("MajorLabelStyle", s);
        s._showMinorLabels = info.GetBoolean("ShowMinorLabels");
        if(s._showMinorLabels)
          s.MinorLabelStyle = (AbstractXYAxisLabelStyle)info.GetValue("MinorLabelStyle", s);
        s.Title = (Graph.TextGraphics)info.GetValue("AxisTitle", s);


        return s;
      }
    }
    #endregion

    protected XYPlotLayerAxisStyleProperties()
    {
    }

    void CopyFrom(XYPlotLayerAxisStyleProperties from)
    {
      if (null != _majorLabelStyle)
        _majorLabelStyle.Changed -= new EventHandler(EhChildChanged);
      if (null != _minorLabelStyle)
        _minorLabelStyle.Changed -= new EventHandler(EhChildChanged);
      if (null != _axisTitle)
        _axisTitle.Changed -= new EventHandler(EhChildChanged);


      this._edgeType = from._edgeType;
      this._showAxis = from._showAxis;
      this._axisStyle = from._axisStyle == null ? null : (XYAxisStyle)from._axisStyle.Clone();
      this._showMajorLabels = from._showMajorLabels;
      this._majorLabelStyle = from._majorLabelStyle == null ? null : (AbstractXYAxisLabelStyle)from._majorLabelStyle.Clone();
      this._showMinorLabels = from._showMinorLabels;
      this._minorLabelStyle = from._minorLabelStyle == null ? null : (AbstractXYAxisLabelStyle)from._minorLabelStyle.Clone();
      this._axisTitle = from._axisTitle == null ? null : (TextGraphics)from._axisTitle.Clone();


      if (null != _majorLabelStyle)
        _majorLabelStyle.Changed += new EventHandler(EhChildChanged);
      if (null != _minorLabelStyle)
        _minorLabelStyle.Changed += new EventHandler(EhChildChanged);
      if (null != _axisTitle)
        _axisTitle.Changed += new EventHandler(EhChildChanged);
    }

    public XYPlotLayerAxisStyleProperties(EdgeType type)
    {
      _edgeType = type;
      _showAxis = true;
      _axisStyle = new XYAxisStyle(_edgeType);
      _axisStyle.Changed += new EventHandler(EhChildChanged);

      _showMajorLabels = true;
      _majorLabelStyle = new XYAxisLabelStyle(_edgeType);
      _majorLabelStyle.Changed += new EventHandler(EhChildChanged);

      _showMinorLabels = false;
      _minorLabelStyle = new XYAxisLabelStyle(_edgeType);
      _minorLabelStyle.Changed += new EventHandler(EhChildChanged);
      _axisTitle = null;
    }



    /// <summary>
    /// Tries to remove a child object of this collection.
    /// </summary>
    /// <param name="go">The object to remove.</param>
    /// <returns> If the provided object is a child object and
    /// the child object could be removed, the return value is true.</returns>
    public bool Remove(GraphicsObject go)
    {
      // test our own objects for removal (only that that _are_ removable)
      if (object.ReferenceEquals(go, this._axisTitle))
      {
        _axisTitle = null;
        return true;
      }
      return false;
    }

    public void Paint(Graphics g, XYPlotLayer layer, Axis axis)
    {
      if (_showAxis)
        _axisStyle.Paint(g, layer, axis);
      if (ShowMajorLabels)
        this._majorLabelStyle.Paint(g, layer, axis, _axisStyle, false);
      if (ShowMinorLabels)
        this._minorLabelStyle.Paint(g, layer, axis, _axisStyle, true);
      if (_showAxis && null != _axisTitle)
        _axisTitle.Paint(g, layer);
    }

    #region Properties
    /// <summary>
    /// Determines whether or not the axis line and ticks should be drawn.
    /// </summary>
    public bool ShowAxis
    {
      get
      {
        return _showAxis;
      }
      set
      {
        bool oldvalue = _showAxis;
        _showAxis = value;

        if (value != oldvalue)
          OnChanged();
      }
    }

    /// <summary>
    /// Determines whether or not the major labels should be shown.
    /// </summary>
    public bool ShowMajorLabels
    {
      get
      {
        return _showAxis && _showMajorLabels && _majorLabelStyle != null;
      }
      set
      {
        bool oldvalue = _showMajorLabels;
        _showMajorLabels = value;

        if (value == true && _majorLabelStyle == null)
          MajorLabelStyle = new XYAxisLabelStyle(this._edgeType);

        if (value != oldvalue)
          OnChanged();
      }
    }

    /// <summary>
    /// Determines whether or not the minor labels should be shown.
    /// </summary>
    public bool ShowMinorLabels
    {
      get
      {
        return _showAxis && _showMinorLabels && _minorLabelStyle != null;
      }
      set
      {
        bool oldvalue = _showMinorLabels;
        _showMinorLabels = value;

        if (value == true && _minorLabelStyle == null)
          MinorLabelStyle = new XYAxisLabelStyle(this._edgeType);

        if (value != oldvalue)
          OnChanged();
      }
    }

    /// <summary>Style of axis. Determines the line width and color of the axis and the ticks.</summary>
    public XYAxisStyle AxisStyle
    {
      get
      {
        return _axisStyle;
      }
      set
      {
        XYAxisStyle oldvalue = _axisStyle;
        _axisStyle = value;

        if (!object.ReferenceEquals(value, oldvalue))
        {
          if (null != oldvalue)
            oldvalue.Changed -= new EventHandler(EhChildChanged);
          if (null != value)
            value.Changed += new EventHandler(EhChildChanged);

          OnChanged();
        }
      }
    }

    /// <summary>
    /// Determines the style of the major labels.
    /// </summary>
    public AbstractXYAxisLabelStyle MajorLabelStyle
    {
      get
      {
        return _majorLabelStyle;
      }
      set
      {
        AbstractXYAxisLabelStyle oldvalue = _majorLabelStyle;
        _majorLabelStyle = value;

        if (!object.ReferenceEquals(value, oldvalue))
        {
          if (null != oldvalue)
            oldvalue.Changed -= new EventHandler(EhChildChanged);
          if (null != value)
            value.Changed += new EventHandler(EhChildChanged);

          OnChanged();
        }
      }
    }


    /// <summary>
    /// Determines the style of the minor labels.
    /// </summary>
    public AbstractXYAxisLabelStyle MinorLabelStyle
    {
      get
      {
        return _minorLabelStyle;
      }
      set
      {
        AbstractXYAxisLabelStyle oldvalue = _minorLabelStyle;
        _minorLabelStyle = value;

        if (!object.ReferenceEquals(value, oldvalue))
        {
          if (null != oldvalue)
            oldvalue.Changed -= new EventHandler(EhChildChanged);
          if (null != value)
            value.Changed += new EventHandler(EhChildChanged);

          OnChanged();
        }
      }
    }


    public TextGraphics Title
    {
      get { return _axisTitle; }
      set
      {
        TextGraphics oldvalue = _axisTitle;
        _axisTitle = value;

        if (!object.ReferenceEquals(_axisTitle, oldvalue))
        {
          OnChanged();
        }
      }
    }

    #endregion

    #region IChangedEventSource Members

    public event EventHandler Changed;

    protected void OnChanged()
    {
      if (Changed != null)
        Changed(this, EventArgs.Empty);
    }

    #endregion

    #region IChildChangedEventSink Members

    public void EhChildChanged(object child, EventArgs e)
    {
      OnChanged();
    }

    #endregion

    #region ICloneable Members

    public object Clone()
    {
      XYPlotLayerAxisStyleProperties res = new XYPlotLayerAxisStyleProperties(this._edgeType);
      res.CopyFrom(this);
      return res;
    }

    #endregion
  }

}

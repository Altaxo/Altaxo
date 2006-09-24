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
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Graph.Gdi.Shapes;

namespace Altaxo.Graph.Gdi.Axis
{
  /// <summary>
  /// This class summarizes all members that are belonging to one edge of the layer.
  /// </summary>
  public class AxisStyle : Main.IChangedEventSource, Main.IChildChangedEventSink, ICloneable
  {
    /// <summary>
    /// Identifies the axis style.
    /// </summary>
    A2DAxisStyleIdentifier _styleID;

    /// <summary>Style of axis. Determines the line width and color of the axis and the ticks.</summary>
    protected AxisLineStyle _axisLineStyle;
    /// <summary>
    /// Determines the style of the major labels.
    /// </summary>
    AxisLabelStyleBase _majorLabelStyle;
    /// <summary>
    /// Determines the style of the minor labels.
    /// </summary>
    AxisLabelStyleBase _minorLabelStyle;
    /// <summary>
    /// The title of the axis.
    /// </summary>
    TextGraphics _axisTitle;

    A2DAxisStyleInformation _cachedAxisInfo;


    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLayerAxisStyleProperties", 0)]
    public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotSupportedException("Serialization of old versions not supported - probably a programming error");
        /*
        XYPlotLayerAxisStyleProperties s = (XYPlotLayerAxisStyleProperties)obj;

        info.AddValue("ShowAxis", s._showAxis);
        info.AddValue("Edge", s._edgeType);
        info.AddValue("AxisStyle", s._axisStyle);
        info.AddValue("ShowMajorLabels", s._showMajorLabels);
        if (s._showMajorLabels)
          info.AddValue("MajorLabelStyle", s._majorLabelStyle);
        info.AddValue("ShowMinorLabels", s._showMinorLabels);
        if (s._showMinorLabels)
          info.AddValue("MinorLabelStyle", s._minorLabelStyle);
        info.AddValue("AxisTitle", s._axisTitle);
        */
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        AxisStyle s = SDeserialize(o, info, parent);
        return s;
      }


      protected virtual AxisStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        AxisStyle s = null != o ? (AxisStyle)o : new AxisStyle();

        // Styles
        bool showAxis = info.GetBoolean("ShowAxis");
        EdgeType edge = (EdgeType)info.GetEnum("Edge", typeof(EdgeType));
        s.AxisLineStyle = (AxisLineStyle)info.GetValue("AxisStyle", s);
        bool showMajorLabels = info.GetBoolean("ShowMajorLabels");
        if (showMajorLabels)
          s.MajorLabelStyle = (AxisLabelStyleBase)info.GetValue("MajorLabelStyle", s);
        bool showMinorLabels = info.GetBoolean("ShowMinorLabels");
        if (showMinorLabels)
          s.MinorLabelStyle = (AxisLabelStyleBase)info.GetValue("MinorLabelStyle", s);
        s.Title = (TextGraphics)info.GetValue("AxisTitle", s);

        switch (edge)
        {
          case EdgeType.Bottom:
            s._styleID = new A2DAxisStyleIdentifier(0, 0);
            break;
          case EdgeType.Top:
            s._styleID = new A2DAxisStyleIdentifier(0, 1);
            break;
          case EdgeType.Left:
            s._styleID = new A2DAxisStyleIdentifier(1, 0);
            break;
          case EdgeType.Right:
            s._styleID = new A2DAxisStyleIdentifier(1, 1);
            break;
        }


        return s;
      }
    }

    // 2006-09-06 renaming to G2DAxisStyle
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisStyle), 1)]
    public class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        AxisStyle s = (AxisStyle)obj;

        info.AddValue("StyleID", s._styleID);
        info.AddValue("AxisStyle", s._axisLineStyle);
        info.AddValue("MajorLabelStyle", s._majorLabelStyle);
        info.AddValue("MinorLabelStyle", s._minorLabelStyle);
        info.AddValue("AxisTitle", s._axisTitle);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        AxisStyle s = SDeserialize(o, info, parent);
        return s;
      }


      protected virtual AxisStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        AxisStyle s = null != o ? (AxisStyle)o : new AxisStyle();

        // Styles
        s._styleID = (A2DAxisStyleIdentifier)info.GetValue("StyleID", s);
        s.AxisLineStyle = (AxisLineStyle)info.GetValue("AxisStyle", s);
        s.MajorLabelStyle = (AxisLabelStyleBase)info.GetValue("MajorLabelStyle", s);
        s.MinorLabelStyle = (AxisLabelStyleBase)info.GetValue("MinorLabelStyle", s);
        s.Title = (TextGraphics)info.GetValue("AxisTitle", s);


        return s;
      }
    }
    #endregion

    protected AxisStyle()
    {
    }

    void CopyFrom(AxisStyle from)
    {
      this._styleID = from._styleID;

      if (null != _axisLineStyle)
        _axisLineStyle.Changed -= new EventHandler(EhChildChanged);
      if (null != _majorLabelStyle)
        _majorLabelStyle.Changed -= new EventHandler(EhChildChanged);
      if (null != _minorLabelStyle)
        _minorLabelStyle.Changed -= new EventHandler(EhChildChanged);
      if (null != _axisTitle)
        _axisTitle.Changed -= new EventHandler(EhChildChanged);

      this._axisLineStyle = from._axisLineStyle == null ? null : (AxisLineStyle)from._axisLineStyle.Clone();
      this._majorLabelStyle = from._majorLabelStyle == null ? null : (AxisLabelStyleBase)from._majorLabelStyle.Clone();
      this._minorLabelStyle = from._minorLabelStyle == null ? null : (AxisLabelStyleBase)from._minorLabelStyle.Clone();
      this._axisTitle = from._axisTitle == null ? null : (TextGraphics)from._axisTitle.Clone();


      if (null != _axisLineStyle)
        _axisLineStyle.Changed += new EventHandler(EhChildChanged);
      if (null != _majorLabelStyle)
        _majorLabelStyle.Changed += new EventHandler(EhChildChanged);
      if (null != _minorLabelStyle)
        _minorLabelStyle.Changed += new EventHandler(EhChildChanged);
      if (null != _axisTitle)
        _axisTitle.Changed += new EventHandler(EhChildChanged);
    }

    public AxisStyle(A2DAxisStyleIdentifier id)
    {
      _styleID = id;
      _axisLineStyle = new AxisLineStyle();
      _axisLineStyle.Changed += new EventHandler(EhChildChanged);

      _majorLabelStyle = new AxisLabelStyle();
      _majorLabelStyle.Changed += new EventHandler(EhChildChanged);
    }

    /// <summary>
    /// Identifies the axis style.
    /// </summary>
    public A2DAxisStyleIdentifier StyleID
    {
      get
      {
        return _styleID;
      }
    }

    public A2DAxisStyleInformation CachedAxisInformation
    {
      get
      {
        return _cachedAxisInfo;
      }
      set
      {
        _cachedAxisInfo = value;
      }
    }

    public bool IsEmpty
    {
      get
      {
        bool r = ShowAxisLine | ShowTitle | ShowMajorLabels | ShowMinorLabels;
        return !r;
      }
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

    public void Paint(Graphics g, XYPlotLayer layer, int axisnumber)
    {
      A2DAxisStyleInformation styleinfo = layer.CoordinateSystem.GetAxisStyleInformation(_styleID);
      _cachedAxisInfo = styleinfo;

      if (ShowAxisLine)
        _axisLineStyle.Paint(g, layer, styleinfo);
      if (ShowMajorLabels)
        this._majorLabelStyle.Paint(g, layer, styleinfo, _axisLineStyle, false);
      if (ShowMinorLabels)
        this._minorLabelStyle.Paint(g, layer, styleinfo, _axisLineStyle, true);
      if (ShowTitle)
        _axisTitle.Paint(g, layer);
    }

    #region Properties
    /// <summary>
    /// Determines whether or not the axis line and ticks should be drawn.
    /// </summary>
    public bool ShowAxisLine
    {
      get
      {
        return _axisLineStyle != null;
      }
      set
      {
        if (value == false)
          AxisLineStyle = null;
        else if (_axisLineStyle == null)
          AxisLineStyle = new AxisLineStyle();
      }
    }

    /// <summary>
    /// Determines whether or not the major labels should be shown.
    /// </summary>
    public bool ShowMajorLabels
    {
      get
      {
        return _majorLabelStyle != null;
      }
      set
      {
        if (value == false)
          MajorLabelStyle = null;
        else if (_majorLabelStyle == null)
          MajorLabelStyle = new AxisLabelStyle();
      }
    }

    /// <summary>
    /// Determines whether or not the minor labels should be shown.
    /// </summary>
    public bool ShowMinorLabels
    {
      get
      {
        return _minorLabelStyle != null;
      }
      set
      {
        if (value == false)
          MinorLabelStyle = null;
        else if (_minorLabelStyle == null)
          MinorLabelStyle = new AxisLabelStyle();
      }
    }

    /// <summary>Style of axis. Determines the line width and color of the axis and the ticks.</summary>
    public AxisLineStyle AxisLineStyle
    {
      get
      {
        return _axisLineStyle;
      }
      set
      {
        AxisLineStyle oldvalue = _axisLineStyle;
        _axisLineStyle = value;

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
    public AxisLabelStyleBase MajorLabelStyle
    {
      get
      {
        if (null == _majorLabelStyle)
          this.MajorLabelStyle = new AxisLabelStyle();

        return _majorLabelStyle;
      }
      set
      {
        AxisLabelStyleBase oldvalue = _majorLabelStyle;
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
    public AxisLabelStyleBase MinorLabelStyle
    {
      get
      {
        if (_minorLabelStyle == null)
          this.MinorLabelStyle = new AxisLabelStyle();

        return _minorLabelStyle;
      }
      set
      {
        AxisLabelStyleBase oldvalue = _minorLabelStyle;
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

    /// <summary>
    /// Determines whether or not the title is shown.
    /// </summary>
    public bool ShowTitle
    {
      get
      {
        return this._axisTitle != null;
      }
      set
      {
        if (value == false)
          Title = null;
        else if (_axisTitle == null)
        {
          Title = new TextGraphics();
          Title.Text = "axis title";
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

    public string TitleText
    {
      get { return null==_axisTitle ? string.Empty : _axisTitle.Text; }
      set
      {
        string oldvalue = TitleText;
        if (value!=oldvalue)
        {
          if (string.IsNullOrEmpty(value))
          {
            _axisTitle = null;
          }
          else
          {
            if (_axisTitle == null)
              _axisTitle = new TextGraphics();

            _axisTitle.Text = value;
          }


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
      AxisStyle res = new AxisStyle(_styleID);
      res.CopyFrom(this);
      return res;
    }

    #endregion
  }

}

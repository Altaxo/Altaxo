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
using Altaxo.Graph.Gdi.Shapes;

namespace Altaxo.Graph.Gdi.Axis
{
  /// <summary>
  /// This class summarizes all members that are belonging to one edge of the layer.
  /// </summary>
  public class AxisStyle 
    :
    Main.IChangedEventSource,
    Main.IChildChangedEventSink, 
    Main.IDocumentNode,
    ICloneable
  {
    /// <summary>
    /// Identifies the axis style.
    /// </summary>
    CSLineID _styleID;

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
    TextGraphic _axisTitle;

    CSAxisInformation _cachedAxisInfo;

    [field:NonSerialized]
    event EventHandler _changed;

    [NonSerialized]
    object _parent;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLayerAxisStyleProperties", 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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
        else s.MajorLabelStyle = null;

        bool showMinorLabels = info.GetBoolean("ShowMinorLabels");
        if (showMinorLabels)
          s.MinorLabelStyle = (AxisLabelStyleBase)info.GetValue("MinorLabelStyle", s);
        else
          s.MinorLabelStyle = null;

        s.Title = (TextGraphic)info.GetValue("AxisTitle", s);

        if (!showAxis)
        {
          s.MajorLabelStyle = null;
          s.MinorLabelStyle = null;
          s.AxisLineStyle = null;
          s.Title = null;
        }


                  double offset = 0;
                  if (s.AxisLineStyle != null && s.AxisLineStyle.Position.IsRelative)
                  {
                    offset = s.AxisLineStyle.Position.Value;
                    // Note here: Absolute values are no longer supported
                    // and so this problem can not be fixed here.
                  }

        switch (edge)
        {
          case EdgeType.Bottom:
            s._styleID = new CSLineID(0, -offset);
            break;
          case EdgeType.Top:
            s._styleID = new CSLineID(0, 1+offset);
            break;
          case EdgeType.Left:
            s._styleID = new CSLineID(1, -offset);
            break;
          case EdgeType.Right:
            s._styleID = new CSLineID(1, 1+offset);
            break;
        }


        return s;
      }
    }

    // 2006-09-06 renaming to G2DAxisStyle
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisStyle), 1)]
    class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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
        s._styleID = (CSLineID)info.GetValue("StyleID", s);
        s.AxisLineStyle = (AxisLineStyle)info.GetValue("AxisStyle", s);
        s.MajorLabelStyle = (AxisLabelStyleBase)info.GetValue("MajorLabelStyle", s);
        s.MinorLabelStyle = (AxisLabelStyleBase)info.GetValue("MinorLabelStyle", s);
        s.Title = (TextGraphic)info.GetValue("AxisTitle", s);


        return s;
      }
    }
    #endregion

    protected AxisStyle()
    {
    }

    void CopyFrom(AxisStyle from)
    {
      this._styleID = from._styleID.Clone();
      CopyWithoutIdFrom(from);
      this._cachedAxisInfo = from._cachedAxisInfo;
    }
    public void CopyWithoutIdFrom(AxisStyle from)
    {
      this.AxisLineStyle = from._axisLineStyle == null ? null : (AxisLineStyle)from._axisLineStyle.Clone();
      this.MajorLabelStyle = from._majorLabelStyle == null ? null : (AxisLabelStyleBase)from._majorLabelStyle.Clone();
      this.MinorLabelStyle = from._minorLabelStyle == null ? null : (AxisLabelStyleBase)from._minorLabelStyle.Clone();
      this.Title = from._axisTitle == null ? null : (TextGraphic)from._axisTitle.Clone();
    }

    public AxisStyle(CSLineID id)
    {
      _styleID = id;
    }

    /// <summary>
    /// Identifies the axis style.
    /// </summary>
    public CSLineID StyleID
    {
      get
      {
        return _styleID;
      }
    }

    public CSAxisInformation CachedAxisInformation
    {
      get
      {
        return _cachedAxisInfo;
      }
      set
      {
        _cachedAxisInfo = value;
        if (_axisLineStyle != null)
          _axisLineStyle.CachedAxisInformation = value;
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
    public bool Remove(GraphicBase go)
    {
      // test our own objects for removal (only that that _are_ removable)
      if (object.ReferenceEquals(go, this._axisTitle))
      {
        _axisTitle = null;
        return true;
      }
      return false;
    }

    public void Paint(Graphics g, XYPlotLayer layer)
    {
      // update the logical values of the physical axes before
        if (_styleID.UsePhysicalValueOtherFirst)
        {
          // then update the logical value of this identifier
          double logicalValue = layer.Scales(_styleID.AxisNumberOtherFirst).PhysicalVariantToNormal(_styleID.PhysicalValueOtherFirst);
          _styleID.LogicalValueOtherFirst = logicalValue;
        }
        if (_styleID.UsePhysicalValueOtherSecond)
        {
          // then update the logical value of this identifier
          double logicalValue = layer.Scales(_styleID.AxisNumberOtherSecond).PhysicalVariantToNormal(_styleID.PhysicalValueOtherSecond);
          _styleID.LogicalValueOtherSecond = logicalValue;
        }

      int axisnumber = _styleID.ParallelAxisNumber;
      CSAxisInformation styleinfo = layer.CoordinateSystem.GetAxisStyleInformation(_styleID);
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


        if (null != value)
        {
          value.ParentObject = this;
          value.CachedAxisInformation = this._cachedAxisInfo;
        }

        if (!object.ReferenceEquals(value, oldvalue))
        {
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
        return _majorLabelStyle;
      }
      set
      {
        AxisLabelStyleBase oldvalue = _majorLabelStyle;
        _majorLabelStyle = value;

        if (null != value)
          value.ParentObject = this;

        if (!object.ReferenceEquals(value, oldvalue))
        {
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
        return _minorLabelStyle;
      }
      set
      {
        AxisLabelStyleBase oldvalue = _minorLabelStyle;
        _minorLabelStyle = value;

        if (null != value)
          value.ParentObject = this;

        if (!object.ReferenceEquals(value, oldvalue))
        {
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
          Title = new TextGraphic();
          Title.Text = "axis title";
        }
      }
    }

    public TextGraphic Title
    {
      get { return _axisTitle; }
      set
      {
        TextGraphic oldvalue = _axisTitle;
        _axisTitle = value;

        if (null != value)
          value.ParentObject = this;

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
              this.Title = new TextGraphic();

            _axisTitle.Text = value;
          }


          OnChanged();
        }
      }
    }

    #endregion

    #region IChangedEventSource Members

    public event EventHandler Changed
    {
      add { _changed += value; }
      remove { _changed -= value; }
    }

    protected void OnChanged()
    {
      if (_parent is Main.IChildChangedEventSink)
        ((Main.IChildChangedEventSink)_parent).EhChildChanged(this, EventArgs.Empty);

      if (_changed != null)
        _changed(this, EventArgs.Empty);
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

    #region IDocumentNode Members

    public object ParentObject
    {
      get { return _parent; }
      set { _parent = value; }
    }

    public string Name
    {
      get { return "AxisStyle"; }
    }

    #endregion

   


   
  }

}

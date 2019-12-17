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

using System;
using System.Collections.Generic;
using System.Drawing;
using Altaxo.Geometry;
using Altaxo.Graph.Gdi.Shapes;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Ticks;

namespace Altaxo.Graph.Gdi.Axis
{
  /// <summary>
  /// This class summarizes all members that are belonging to one edge of the layer.
  /// </summary>
  public class AxisStyle
    :
    Main.SuspendableDocumentNodeWithSetOfEventArgs,
    Main.ICopyFrom
  {
    /// <summary>
    /// Identifies the axis style.
    /// </summary>
    private CSLineID _styleID;

    /// <summary>If not <c>null</c>, this is a custom tick spacing for the axis line that overrides the default tick spacing of the scaleWithTicks.</summary>
    protected TickSpacing _customTickSpacing;

    /// <summary>Style of axis. Determines the line width and color of the axis and the ticks.</summary>
    protected AxisLineStyle _axisLineStyle;

    /// <summary>
    /// Determines the style of the major labels.
    /// </summary>
    private AxisLabelStyle _majorLabelStyle;

    /// <summary>
    /// Determines the style of the minor labels.
    /// </summary>
    private AxisLabelStyle _minorLabelStyle;

    /// <summary>
    /// The title of the axis.
    /// </summary>
    private TextGraphic _axisTitle;

    private CSAxisInformation _cachedAxisInfo;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLayerAxisStyleProperties", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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
        var edge = (EdgeType)info.GetEnum("Edge", typeof(EdgeType));
        s.AxisLineStyle = (AxisLineStyle)info.GetValue("AxisStyle", s);
        bool showMajorLabels = info.GetBoolean("ShowMajorLabels");
        if (showMajorLabels)
          s.MajorLabelStyle = (AxisLabelStyle)info.GetValue("MajorLabelStyle", s);
        else
          s.MajorLabelStyle = null;

        bool showMinorLabels = info.GetBoolean("ShowMinorLabels");
        if (showMinorLabels)
          s.MinorLabelStyle = (AxisLabelStyle)info.GetValue("MinorLabelStyle", s);
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
            s._styleID = new CSLineID(0, 1 + offset);
            break;

          case EdgeType.Left:
            s._styleID = new CSLineID(1, -offset);
            break;

          case EdgeType.Right:
            s._styleID = new CSLineID(1, 1 + offset);
            break;
        }

        return s;
      }
    }

    // 2006-09-06 renaming to G2DAxisStyle
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Axis.AxisStyle", 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AxisStyle)obj;

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
        s.MajorLabelStyle = (AxisLabelStyle)info.GetValue("MajorLabelStyle", s);
        s.MinorLabelStyle = (AxisLabelStyle)info.GetValue("MinorLabelStyle", s);
        s.Title = (TextGraphic)info.GetValue("AxisTitle", s);

        return s;
      }
    }

    // 2013-09-20 addition of _customTickSpacing
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisStyle), 2)]
    private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AxisStyle)obj;

        info.AddValue("StyleID", s._styleID);
        info.AddValue("TickSpacing", s._customTickSpacing);
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
        s.TickSpacing = (TickSpacing)info.GetValue("TickSpacing", s);
        s.AxisLineStyle = (AxisLineStyle)info.GetValue("AxisStyle", s);
        s.MajorLabelStyle = (AxisLabelStyle)info.GetValue("MajorLabelStyle", s);
        s.MinorLabelStyle = (AxisLabelStyle)info.GetValue("MinorLabelStyle", s);
        s.Title = (TextGraphic)info.GetValue("AxisTitle", s);

        return s;
      }
    }

    #endregion Serialization

    protected AxisStyle()
    {
    }

    public bool CopyFrom(object obj)
    {
      var from = obj as AxisStyle;
      if (null == from)
        return false;

      if (!object.ReferenceEquals(this, from))
      {
        _styleID = from._styleID; // immutable
        _cachedAxisInfo = from._cachedAxisInfo; // attention - have to appear _before_ CopyWithoutIdFrom, since the _cachedAxisInfo is used when cloning AxisLineStyle!
        CopyWithoutIdFrom(from);
      }
      return true;
    }

    public void CopyWithoutIdFrom(AxisStyle from)
    {
      TickSpacing = from._customTickSpacing == null ? null : (TickSpacing)from._customTickSpacing.Clone();
      AxisLineStyle = from._axisLineStyle == null ? null : (AxisLineStyle)from._axisLineStyle.Clone();
      MajorLabelStyle = from._majorLabelStyle == null ? null : (AxisLabelStyle)from._majorLabelStyle.Clone();
      MinorLabelStyle = from._minorLabelStyle == null ? null : (AxisLabelStyle)from._minorLabelStyle.Clone();
      Title = from._axisTitle == null ? null : (TextGraphic)from._axisTitle.Clone();
    }

    public AxisStyle(CSLineID id, bool isAxisLineEnabled, bool areMajorTicksEnabled, bool areMinorTicksEnabled, string axisTitleOrNull, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      _styleID = id;

      if (isAxisLineEnabled)
      {
        ShowAxisLine(context);
      }

      if (areMajorTicksEnabled)
      {
        ShowMajorLabels(context);
      }

      if (areMinorTicksEnabled)
      {
        ShowMinorLabels(context);
      }

      if (null != axisTitleOrNull)
      {
        ShowTitle(context);
        _axisTitle.Text = axisTitleOrNull;
      }
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (null != _axisLineStyle)
        yield return new Main.DocumentNodeAndName(_axisLineStyle, "LineStyle");

      if (null != _majorLabelStyle)
        yield return new Main.DocumentNodeAndName(_majorLabelStyle, "MajorLabelStyle");

      if (null != _minorLabelStyle)
        yield return new Main.DocumentNodeAndName(_minorLabelStyle, "MinorLabelStyle");

      if (null != _axisTitle)
        yield return new Main.DocumentNodeAndName(_axisTitle, "Title");

      if (null != _customTickSpacing)
        yield return new Main.DocumentNodeAndName(_customTickSpacing, "CustomTickSpacing");
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
        if (_majorLabelStyle is AxisLabelStyle)
          _majorLabelStyle.CachedAxisInformation = value;
        if (_minorLabelStyle is AxisLabelStyle)
          _minorLabelStyle.CachedAxisInformation = value;
      }
    }

    public bool IsEmpty
    {
      get
      {
        bool r = IsAxisLineEnabled | IsTitleEnabled | AreMajorLabelsEnabled | AreMinorLabelsEnabled;
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
      if (object.ReferenceEquals(go, _axisTitle))
      {
        _axisTitle = null;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Gets or sets the tick spacing. This is a custom tick spacing, thus this property can be set to <c>null</c>. If set to <c>null</c>, the default tick spacing of the appropriate scale is used.
    /// If not <c>null</c>, the tick spacing provided is used, but this tick spacing does not influence the Org or the End of the scale.
    /// </summary>
    /// <value>
    /// The tick spacing.
    /// </value>
    public TickSpacing TickSpacing
    {
      get
      {
        return _customTickSpacing;
      }
      set
      {
        if (ChildSetMember(ref _customTickSpacing, value))
          EhSelfChanged(EventArgs.Empty);
      }
    }

    public void FixupInternalDataStructures(IPlotArea layer)
    {
      FixupInternalDataStructures(layer, layer.CoordinateSystem.GetAxisStyleInformation);
    }

    public void FixupInternalDataStructures(IPlotArea layer, Func<CSLineID, CSAxisInformation> GetAxisStyleInformation)
    {
      // update the logical values of the physical axes before
      if (_styleID.UsePhysicalValueOtherFirst)
      {
        // then update the logical value of this identifier
        double logicalValue = layer.Scales[_styleID.AxisNumberOtherFirst].PhysicalVariantToNormal(_styleID.PhysicalValueOtherFirst);
        _styleID = _styleID.WithLogicalValueOtherFirst(logicalValue);
      }
      if (_styleID.UsePhysicalValueOtherSecond)
      {
        // then update the logical value of this identifier
        double logicalValue = layer.Scales[_styleID.AxisNumberOtherSecond].PhysicalVariantToNormal(_styleID.PhysicalValueOtherSecond);
        _styleID = _styleID.WithLogicalValueOtherSecond(logicalValue);
      }

      CachedAxisInformation = GetAxisStyleInformation(_styleID);

      if (null != _customTickSpacing)
      {
        CSLineID styleID = _cachedAxisInfo.Identifier;
        Scale scale = layer.Scales[styleID.ParallelAxisNumber];
        Altaxo.Data.AltaxoVariant org = scale.OrgAsVariant, end = scale.EndAsVariant;
        _customTickSpacing.PreProcessScaleBoundaries(ref org, ref end, false, false);
        _customTickSpacing.FinalProcessScaleBoundaries(org, end, scale);
      }

      if (null != _axisTitle)
        _axisTitle.SetParentSize(layer.Size, false);
    }

    public void PaintPreprocessing(IPlotArea layer)
    {
    }

    public void Paint(Graphics g, IPaintContext paintContext, IPlotArea layer)
    {
      Paint(g, paintContext, layer, layer.CoordinateSystem.GetAxisStyleInformation);
    }

    public void Paint(Graphics g, IPaintContext paintContext, IPlotArea layer, Func<CSLineID, CSAxisInformation> GetAxisStyleInformation)
    {
      PaintLine(g, layer);
      PaintMajorLabels(g, layer);
      PaintMinorLabels(g, layer);
      PaintTitle(g, paintContext, layer);
    }

    public void PaintLine(Graphics g, IPlotArea layer)
    {
      if (IsAxisLineEnabled)
      {
        _axisLineStyle.Paint(g, layer, _cachedAxisInfo, _customTickSpacing);
      }
    }

    public void PaintMajorLabels(Graphics g, IPlotArea layer)
    {
      if (AreMajorLabelsEnabled)
      {
        var labelSide = _majorLabelStyle.PredictLabelSide(_cachedAxisInfo);
        var outerDistance = null == _axisLineStyle ? 0 : _axisLineStyle.GetOuterDistance(labelSide);
        var scaleWithTicks = layer.Scales[_cachedAxisInfo.Identifier.ParallelAxisNumber];
        _majorLabelStyle.Paint(g, layer.CoordinateSystem, scaleWithTicks, _customTickSpacing ?? scaleWithTicks.TickSpacing, _cachedAxisInfo, outerDistance, false);
      }
    }

    public void PaintMinorLabels(Graphics g, IPlotArea layer)
    {
      if (AreMinorLabelsEnabled)
      {
        var labelSide = _minorLabelStyle.PredictLabelSide(_cachedAxisInfo);
        var outerDistance = null == _axisLineStyle ? 0 : _axisLineStyle.GetOuterDistance(labelSide);
        var scaleWithTicks = layer.Scales[_cachedAxisInfo.Identifier.ParallelAxisNumber];
        _minorLabelStyle.Paint(g, layer.CoordinateSystem, scaleWithTicks, _customTickSpacing ?? scaleWithTicks.TickSpacing, _cachedAxisInfo, outerDistance, true);
      }
    }

    public void PaintTitle(Graphics g, IPaintContext paintContext, IPlotArea layer)
    {
      if (IsTitleEnabled)
      {
        _axisTitle.Paint(g, paintContext);
      }
    }

    public void PaintPostprocessing()
    {
    }

    #region Properties

    /// <summary>
    /// Determines whether or not the axis line and ticks should be drawn.
    /// </summary>
    public bool IsAxisLineEnabled
    {
      get
      {
        return _axisLineStyle != null;
      }
    }

    public void ShowAxisLine(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      if (_axisLineStyle == null)
        AxisLineStyle = new AxisLineStyle(context);
    }

    public void HideAxisLine()
    {
      AxisLineStyle = null;
    }

    /// <summary>
    /// Determines whether or not the major labels should be shown.
    /// </summary>
    public bool AreMajorLabelsEnabled
    {
      get
      {
        return _majorLabelStyle != null;
      }
    }

    public void ShowMajorLabels(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      if (_majorLabelStyle == null)
        MajorLabelStyle = new AxisLabelStyle(context) { CachedAxisInformation = _cachedAxisInfo };
    }

    public void HideMajorLabels()
    {
      MajorLabelStyle = null;
    }

    /// <summary>
    /// Determines whether or not the minor labels should be shown.
    /// </summary>
    public bool AreMinorLabelsEnabled
    {
      get
      {
        return _minorLabelStyle != null;
      }
    }

    public void ShowMinorLabels(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      if (_minorLabelStyle == null)
        MinorLabelStyle = new AxisLabelStyle(context) { CachedAxisInformation = _cachedAxisInfo };
    }

    public void HideMinorLabels()
    {
      MinorLabelStyle = null;
    }

    /// <summary>
    /// Determines whether or not the title is shown.
    /// </summary>
    public bool IsTitleEnabled
    {
      get
      {
        return _axisTitle != null;
      }
    }

    public void ShowTitle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      if (_axisTitle == null)
      {
        Title = new TextGraphic(context)
        {
          Text = "axis title"
        };
      }
    }

    public void HideTitle()
    {
      Title = null;
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
          value.CachedAxisInformation = _cachedAxisInfo;
        }

        if (!object.ReferenceEquals(value, oldvalue))
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Determines the style of the major labels.
    /// </summary>
    public AxisLabelStyle MajorLabelStyle
    {
      get
      {
        return _majorLabelStyle;
      }
      set
      {
        var oldvalue = _majorLabelStyle;
        _majorLabelStyle = value;

        if (null != _majorLabelStyle)
        {
          _majorLabelStyle.ParentObject = this;
        }

        if (!object.ReferenceEquals(value, oldvalue))
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Determines the style of the minor labels.
    /// </summary>
    public AxisLabelStyle MinorLabelStyle
    {
      get
      {
        return _minorLabelStyle;
      }
      set
      {
        var oldvalue = _minorLabelStyle;
        _minorLabelStyle = value;

        if (null != value)
          value.ParentObject = this;

        if (!object.ReferenceEquals(value, oldvalue))
        {
          EhSelfChanged(EventArgs.Empty);
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
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public string TitleText
    {
      get { return null == _axisTitle ? string.Empty : _axisTitle.Text; }
      set
      {
        string oldvalue = TitleText;
        if (value != oldvalue)
        {
          if (string.IsNullOrEmpty(value))
          {
            _axisTitle = null;
          }
          else
          {
            if (_axisTitle == null)
            {
              Title = new TextGraphic(this.GetPropertyContext()) { ParentObject = this };
            }

            _axisTitle.Text = value;
          }

          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    #endregion Properties

    #region ICloneable Members

    public object Clone()
    {
      var res = new AxisStyle();
      res.CopyFrom(this);
      return res;
    }

    #endregion ICloneable Members
  }
}

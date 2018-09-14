#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Ticks;

namespace Altaxo.Graph.Graph3D.Axis
{
  using GraphicsContext;
  using Main.Properties;
  using Shapes;

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

    // 2015-09-1 Initial version
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisStyle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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

    /// <summary>
    /// Changes the style identifier.
    /// </summary>
    /// <param name="newIdentifier">The new identifier.</param>
    /// <param name="GetNewAxisSideFromOldAxisSide">Functions that uses the old axis side as parameter1, and returns the corresponding axis side of the new coordinate system, or null if no such side could be found.</param>
    /// <exception cref="System.ArgumentNullException"></exception>
    public void ChangeStyleIdentifier(CSLineID newIdentifier, Func<CSAxisSide, CSAxisSide?> GetNewAxisSideFromOldAxisSide)
    {
      if (null == newIdentifier)
        throw new ArgumentNullException(nameof(newIdentifier));
      _styleID = newIdentifier;

      if (null != _axisLineStyle)
        _axisLineStyle.ChangeTickPositionsWhenChangingCoordinateSystem(GetNewAxisSideFromOldAxisSide);

      if (null != _majorLabelStyle && _majorLabelStyle.LabelSide.HasValue)
        _majorLabelStyle.LabelSide = GetNewAxisSideFromOldAxisSide(_majorLabelStyle.LabelSide.Value);

      if (null != _minorLabelStyle && _minorLabelStyle.LabelSide.HasValue)
      {
        _minorLabelStyle.LabelSide = GetNewAxisSideFromOldAxisSide(_minorLabelStyle.LabelSide.Value);
      }
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

    public AxisStyle(CSAxisInformation info, bool isAxisLineEnabled, bool areMajorTicksEnabled, bool areMinorTicksEnabled, string axisTitleOrNull, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      _styleID = info.Identifier;

      if (isAxisLineEnabled)
      {
        ShowAxisLine(info.HasTicksByDefault, info.PreferredTickSide, context);
      }

      if (info.HasLabelsByDefault)
      {
        ShowMajorLabels(info.PreferredLabelSide, context);
      }

      if (info.HasLabelsByDefault && areMinorTicksEnabled)
      {
        ShowMinorLabels(info.PreferredLabelSide, context);
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
    public bool Remove(IGraphicBase go)
    {
      // test our own objects for removal (only that that _are_ removable)
      if (object.ReferenceEquals(go, _axisTitle))
      {
        _axisTitle = null;
        EhSelfChanged(EventArgs.Empty);
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

    public IHitTestObject HitTest(HitTestPointData parentCoord, DoubleClickHandler AxisScaleEditorMethod, DoubleClickHandler AxisStyleEditorMethod, DoubleClickHandler AxisLabelMajorStyleEditorMethod, DoubleClickHandler AxisLabelMinorStyleEditorMethod)
    {
      IHitTestObject hit;
      if (null != (hit = _axisTitle?.HitTest(parentCoord)))
        return hit;

      if (IsAxisLineEnabled && null != (hit = _axisLineStyle.HitTest(parentCoord, false)))
      {
        hit.DoubleClick = AxisScaleEditorMethod;
        return hit;
      }

      // hit testing the axes - secondly now with the ticks
      // in this case the TitleAndFormat editor for the axis should be shown
      if (IsAxisLineEnabled && null != (hit = _axisLineStyle.HitTest(parentCoord, true)))
      {
        hit.DoubleClick = AxisStyleEditorMethod;
        return hit;
      }

      if (null != (hit = _majorLabelStyle?.HitTest(parentCoord)))
      {
        hit.DoubleClick = AxisLabelMajorStyleEditorMethod;
        return hit;
      }

      if (null != (hit = _minorLabelStyle?.HitTest(parentCoord)))
      {
        hit.DoubleClick = AxisLabelMinorStyleEditorMethod;
        return hit;
      }

      return null;
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

    public void Paint(IGraphicsContext3D g, Altaxo.Graph.IPaintContext paintContext, IPlotArea layer)
    {
      Paint(g, paintContext, layer, layer.CoordinateSystem.GetAxisStyleInformation);
    }

    public void Paint(IGraphicsContext3D g, Altaxo.Graph.IPaintContext paintContext, IPlotArea layer, Func<CSLineID, CSAxisInformation> GetAxisStyleInformation)
    {
      PaintLine(g, layer);
      PaintMajorLabels(g, layer);
      PaintMinorLabels(g, layer);
      PaintTitle(g, paintContext, layer);
    }

    public void PaintLine(IGraphicsContext3D g, IPlotArea layer)
    {
      if (IsAxisLineEnabled)
      {
        _axisLineStyle.Paint(g, layer, _cachedAxisInfo, _customTickSpacing);
      }
    }

    public void PaintMajorLabels(IGraphicsContext3D g, IPlotArea layer)
    {
      if (AreMajorLabelsEnabled)
      {
        var labelSide = _majorLabelStyle.PredictLabelSide(_cachedAxisInfo);
        var outerDistance = null == _axisLineStyle ? 0 : _axisLineStyle.GetOuterDistance(labelSide);
        var scaleWithTicks = layer.Scales[_cachedAxisInfo.Identifier.ParallelAxisNumber];
        _majorLabelStyle.Paint(g, layer.CoordinateSystem, scaleWithTicks, _customTickSpacing ?? scaleWithTicks.TickSpacing, _cachedAxisInfo, outerDistance, false);
      }
    }

    public void PaintMinorLabels(IGraphicsContext3D g, IPlotArea layer)
    {
      if (AreMinorLabelsEnabled)
      {
        var labelSide = _minorLabelStyle.PredictLabelSide(_cachedAxisInfo);
        var outerDistance = null == _axisLineStyle ? 0 : _axisLineStyle.GetOuterDistance(labelSide);
        var scaleWithTicks = layer.Scales[_cachedAxisInfo.Identifier.ParallelAxisNumber];
        _minorLabelStyle.Paint(g, layer.CoordinateSystem, scaleWithTicks, _customTickSpacing ?? scaleWithTicks.TickSpacing, _cachedAxisInfo, outerDistance, true);
      }
    }

    public void PaintTitle(IGraphicsContext3D g, Altaxo.Graph.IPaintContext paintContext, IPlotArea layer)
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

    private void ShowAxisLine(bool hasTicksByDefault, CSAxisSide preferredTickSide, IReadOnlyPropertyBag context)
    {
      if (_axisLineStyle == null)
        AxisLineStyle = new AxisLineStyle(hasTicksByDefault, preferredTickSide, context);
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

    public void ShowMajorLabels(CSAxisSide preferredLabelSide, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      if (_majorLabelStyle == null)
        MajorLabelStyle = new AxisLabelStyle(preferredLabelSide, context) { CachedAxisInformation = _cachedAxisInfo };
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

    public void ShowMinorLabels(CSAxisSide preferredLabelSide, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      if (_minorLabelStyle == null)
        MinorLabelStyle = new AxisLabelStyle(preferredLabelSide, context) { CachedAxisInformation = _cachedAxisInfo };
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

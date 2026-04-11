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

#nullable enable
using System;
using System.Collections.Generic;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Ticks;

namespace Altaxo.Graph.Graph3D.Axis
{
  using System.Diagnostics.CodeAnalysis;
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
    protected TickSpacing? _customTickSpacing;

    /// <summary>Style of axis. Determines the line width and color of the axis and the ticks.</summary>
    protected AxisLineStyle? _axisLineStyle;

    /// <summary>
    /// Determines the style of the major labels.
    /// </summary>
    private AxisLabelStyle? _majorLabelStyle;

    /// <summary>
    /// Determines the style of the minor labels.
    /// </summary>
    private AxisLabelStyle? _minorLabelStyle;

    /// <summary>
    /// The title of the axis.
    /// </summary>
    private TextGraphic? _axisTitle;

    private CSAxisInformation? _cachedAxisInfo;

    #region Serialization

    // 2015-09-1 Initial version
    /// <summary>
    /// Serializes <see cref="AxisStyle"/> instances.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisStyle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AxisStyle)obj;

        info.AddValue("StyleID", s._styleID);
        info.AddValueOrNull("TickSpacing", s._customTickSpacing);
        info.AddValueOrNull("AxisStyle", s._axisLineStyle);
        info.AddValueOrNull("MajorLabelStyle", s._majorLabelStyle);
        info.AddValueOrNull("MinorLabelStyle", s._minorLabelStyle);
        info.AddValueOrNull("AxisTitle", s._axisTitle);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (AxisStyle?)o ?? new AxisStyle(info);

        // Styles
        s._styleID = (CSLineID)info.GetValue("StyleID", s);
        s.TickSpacing = info.GetValueOrNull<TickSpacing>("TickSpacing", s);
        s.AxisLineStyle = info.GetValueOrNull<AxisLineStyle>("AxisStyle", s);
        s.MajorLabelStyle = info.GetValueOrNull<AxisLabelStyle>("MajorLabelStyle", s);
        s.MinorLabelStyle = info.GetValueOrNull<AxisLabelStyle>("MinorLabelStyle", s);
        s.Title = info.GetValueOrNull<TextGraphic>("AxisTitle", s);

        return s;
      }


    }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    /// <summary>
    /// Initializes a new instance of the <see cref="AxisStyle"/> class during deserialization.
    /// </summary>
    /// <param name="info">The deserialization info.</param>
    protected AxisStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.


    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="AxisStyle"/> class by copying from another instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    public AxisStyle(AxisStyle from)
    {
      CopyFrom(from);
    }

    /// <summary>
    /// Copies values from another <see cref="AxisStyle"/> instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    [MemberNotNull(nameof(_styleID))]
    public void CopyFrom(AxisStyle from)
    {
      if (ReferenceEquals(this, from))
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
        return;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.

      _styleID = from._styleID; // immutable
      _cachedAxisInfo = from._cachedAxisInfo; // attention - have to appear _before_ CopyWithoutIdFrom, since the _cachedAxisInfo is used when cloning AxisLineStyle!
      CopyWithoutIdFrom(from);
    }


    /// <summary>
    /// Copies all values except the style identifier from another <see cref="AxisStyle"/> instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    public void CopyWithoutIdFrom(AxisStyle from)
    {
      ChildCloneToMember(ref _customTickSpacing, from._customTickSpacing);
      ChildCloneToMember(ref _axisLineStyle, from._axisLineStyle);
      ChildCloneToMember(ref _majorLabelStyle, from._majorLabelStyle);
      ChildCloneToMember(ref _minorLabelStyle, from._minorLabelStyle);
      ChildCloneToMember(ref _axisTitle, from._axisTitle);
    }

    /// <inheritdoc/>
    public bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;
      if (obj is AxisStyle from)
      {
        CopyFrom(from);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Changes the style identifier.
    /// </summary>
    /// <param name="newIdentifier">The new identifier.</param>
    /// <param name="GetNewAxisSideFromOldAxisSide">Functions that uses the old axis side as parameter1, and returns the corresponding axis side of the new coordinate system, or null if no such side could be found.</param>
    /// <exception cref="System.ArgumentNullException"></exception>
    public void ChangeStyleIdentifier(CSLineID newIdentifier, Func<CSAxisSide, CSAxisSide?> GetNewAxisSideFromOldAxisSide)
    {
      if (newIdentifier is null)
        throw new ArgumentNullException(nameof(newIdentifier));

      _styleID = newIdentifier;

      if (_axisLineStyle is not null)
        _axisLineStyle.ChangeTickPositionsWhenChangingCoordinateSystem(GetNewAxisSideFromOldAxisSide);

      if (_majorLabelStyle is not null && _majorLabelStyle.LabelSide.HasValue)
        _majorLabelStyle.LabelSide = GetNewAxisSideFromOldAxisSide(_majorLabelStyle.LabelSide.Value);

      if (_minorLabelStyle is not null && _minorLabelStyle.LabelSide.HasValue)
      {
        _minorLabelStyle.LabelSide = GetNewAxisSideFromOldAxisSide(_minorLabelStyle.LabelSide.Value);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AxisStyle"/> class.
    /// </summary>
    /// <param name="id">The style identifier.</param>
    /// <param name="isAxisLineEnabled">If set to <c>true</c>, an axis line style is created.</param>
    /// <param name="areMajorTicksEnabled">If set to <c>true</c>, major labels are enabled.</param>
    /// <param name="areMinorTicksEnabled">If set to <c>true</c>, minor labels are enabled.</param>
    /// <param name="axisTitleOrNull">The axis title, or <c>null</c>.</param>
    /// <param name="context">The property context.</param>
    public AxisStyle(CSLineID id, bool isAxisLineEnabled, bool areMajorTicksEnabled, bool areMinorTicksEnabled, string? axisTitleOrNull, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
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

      if (axisTitleOrNull is not null)
      {
        ShowTitle(context);
        _axisTitle.Text = axisTitleOrNull;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AxisStyle"/> class using axis metadata.
    /// </summary>
    /// <param name="info">The axis information.</param>
    /// <param name="isAxisLineEnabled">If set to <c>true</c>, an axis line style is created.</param>
    /// <param name="areMajorTicksEnabled">If set to <c>true</c>, major labels are enabled.</param>
    /// <param name="areMinorTicksEnabled">If set to <c>true</c>, minor labels are enabled.</param>
    /// <param name="axisTitleOrNull">The axis title, or <c>null</c>.</param>
    /// <param name="context">The property context.</param>
    public AxisStyle(CSAxisInformation info, bool isAxisLineEnabled, bool areMajorTicksEnabled, bool areMinorTicksEnabled, string? axisTitleOrNull, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
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

      if (axisTitleOrNull is not null)
      {
        ShowTitle(context);
        _axisTitle.Text = axisTitleOrNull;
      }
    }

    /// <inheritdoc/>
    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_axisLineStyle is not null)
        yield return new Main.DocumentNodeAndName(_axisLineStyle, "LineStyle");

      if (_majorLabelStyle is not null)
        yield return new Main.DocumentNodeAndName(_majorLabelStyle, "MajorLabelStyle");

      if (_minorLabelStyle is not null)
        yield return new Main.DocumentNodeAndName(_minorLabelStyle, "MinorLabelStyle");

      if (_axisTitle is not null)
        yield return new Main.DocumentNodeAndName(_axisTitle, "Title");

      if (_customTickSpacing is not null)
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

    /// <summary>
    /// Gets or sets the cached axis information associated with this style.
    /// </summary>
    public CSAxisInformation? CachedAxisInformation
    {
      get
      {
        return _cachedAxisInfo;
      }
      set
      {
        _cachedAxisInfo = value;
        if (_axisLineStyle is not null)
          _axisLineStyle.CachedAxisInformation = value;
        if (_majorLabelStyle is not null)
          _majorLabelStyle.CachedAxisInformation = value;
        if (_minorLabelStyle is not null)
          _minorLabelStyle.CachedAxisInformation = value;
      }
    }

    /// <summary>
    /// Gets a value indicating whether no axis line, title, or labels are enabled.
    /// </summary>
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
    public TickSpacing? TickSpacing
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

    /// <summary>
    /// Updates cached data structures using the specified plot area.
    /// </summary>
    /// <param name="layer">The plot area.</param>
    public void FixupInternalDataStructures(IPlotArea layer)
    {
      FixupInternalDataStructures(layer, layer.CoordinateSystem.GetAxisStyleInformation);
    }

    /// <summary>
    /// Performs hit testing for the title, axis line, and labels.
    /// </summary>
    /// <param name="parentCoord">The hit-test data in parent coordinates.</param>
    /// <param name="AxisScaleEditorMethod">Unused scale editor callback.</param>
    /// <param name="AxisStyleEditorMethod">The double-click handler for the axis line.</param>
    /// <param name="AxisLabelMajorStyleEditorMethod">The double-click handler for major labels.</param>
    /// <param name="AxisLabelMinorStyleEditorMethod">The double-click handler for minor labels.</param>
    /// <returns>The matching hit-test object, or <see langword="null"/>.</returns>
    public IHitTestObject? HitTest(HitTestPointData parentCoord, DoubleClickHandler? AxisScaleEditorMethod, DoubleClickHandler? AxisStyleEditorMethod, DoubleClickHandler? AxisLabelMajorStyleEditorMethod, DoubleClickHandler? AxisLabelMinorStyleEditorMethod)
    {
      IHitTestObject? hit;
      if ((hit = _axisTitle?.HitTest(parentCoord)) is not null)
        return hit;

      // hit testing the axes - secondly now with the ticks
      // in this case the TitleAndFormat editor for the axis should be shown
      if ((hit = _axisLineStyle?.HitTest(parentCoord, true)) is not null)
      {
        hit.DoubleClick = AxisStyleEditorMethod;
        return hit;
      }

      if ((hit = _majorLabelStyle?.HitTest(parentCoord)) is not null)
      {
        hit.DoubleClick = AxisLabelMajorStyleEditorMethod;
        return hit;
      }

      if ((hit = _minorLabelStyle?.HitTest(parentCoord)) is not null)
      {
        hit.DoubleClick = AxisLabelMinorStyleEditorMethod;
        return hit;
      }

      return null;
    }

    /// <summary>
    /// Updates internal data structures of all contained axis styles.
    /// </summary>
    /// <param name="layer">The plot area.</param>
    /// <param name="GetAxisStyleInformation">The resolver for axis information.</param>
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
      if (_cachedAxisInfo is null)
        throw new InvalidOperationException($"{nameof(_cachedAxisInfo)} is null");


      if (_customTickSpacing is not null)
      {
        CSLineID styleID = _cachedAxisInfo.Identifier;
        Scale scale = layer.Scales[styleID.ParallelAxisNumber];
        Altaxo.Data.AltaxoVariant org = scale.OrgAsVariant, end = scale.EndAsVariant;
        _customTickSpacing.PreProcessScaleBoundaries(ref org, ref end, false, false);
        _customTickSpacing.FinalProcessScaleBoundaries(org, end, scale);
      }

      if (_axisTitle is not null)
        _axisTitle.SetParentSize(layer.Size, false);
    }

    /// <summary>
    /// Performs preprocessing for all contained axis styles.
    /// </summary>
    /// <param name="layer">The plot area.</param>
    public void PaintPreprocessing(IPlotArea layer)
    {
    }

    /// <summary>
    /// Paints the complete axis style.
    /// </summary>
    /// <param name="g">The graphics context.</param>
    /// <param name="paintContext">The paint context.</param>
    /// <param name="layer">The plot area.</param>
    public void Paint(IGraphicsContext3D g, Altaxo.Graph.IPaintContext paintContext, IPlotArea layer)
    {
      Paint(g, paintContext, layer, layer.CoordinateSystem.GetAxisStyleInformation);
    }

    /// <summary>
    /// Paints the complete axis style using the supplied axis-information resolver.
    /// </summary>
    /// <param name="g">The graphics context.</param>
    /// <param name="paintContext">The paint context.</param>
    /// <param name="layer">The plot area.</param>
    /// <param name="GetAxisStyleInformation">The axis-information resolver.</param>
    public void Paint(IGraphicsContext3D g, Altaxo.Graph.IPaintContext paintContext, IPlotArea layer, Func<CSLineID, CSAxisInformation> GetAxisStyleInformation)
    {
      PaintLine(g, layer);
      PaintMajorLabels(g, layer);
      PaintMinorLabels(g, layer);
      PaintTitle(g, paintContext, layer);
    }

    /// <summary>
    /// Paints the axis line.
    /// </summary>
    /// <param name="g">The graphics context.</param>
    /// <param name="layer">The plot area.</param>
    public void PaintLine(IGraphicsContext3D g, IPlotArea layer)
    {
      if (_cachedAxisInfo is null)
        throw new InvalidOperationException($"{nameof(_cachedAxisInfo)} is null");

      if (_axisLineStyle is not null)
      {
        _axisLineStyle.Paint(g, layer, _cachedAxisInfo, _customTickSpacing);
      }
    }

    /// <summary>
    /// Paints the major labels.
    /// </summary>
    /// <param name="g">The graphics context.</param>
    /// <param name="layer">The plot area.</param>
    public void PaintMajorLabels(IGraphicsContext3D g, IPlotArea layer)
    {
      if (_cachedAxisInfo is null)
        throw new InvalidOperationException($"{nameof(_cachedAxisInfo)} is null");

      if (_majorLabelStyle is not null)
      {
        var labelSide = _majorLabelStyle.PredictLabelSide(_cachedAxisInfo);
        var outerDistance = _axisLineStyle is null ? 0 : _axisLineStyle.GetOuterDistance(labelSide);
        var scaleWithTicks = layer.Scales[_cachedAxisInfo.Identifier.ParallelAxisNumber];
        _majorLabelStyle.Paint(g, layer.CoordinateSystem, scaleWithTicks, _customTickSpacing ?? scaleWithTicks.TickSpacing, _cachedAxisInfo, outerDistance, false);
      }
    }

    /// <summary>
    /// Paints the minor labels.
    /// </summary>
    /// <param name="g">The graphics context.</param>
    /// <param name="layer">The plot area.</param>
    public void PaintMinorLabels(IGraphicsContext3D g, IPlotArea layer)
    {
      if (_cachedAxisInfo is null)
        throw new InvalidOperationException($"{nameof(_cachedAxisInfo)} is null");

      if (_minorLabelStyle is not null)
      {
        var labelSide = _minorLabelStyle.PredictLabelSide(_cachedAxisInfo);
        var outerDistance = _axisLineStyle is null ? 0 : _axisLineStyle.GetOuterDistance(labelSide);
        var scaleWithTicks = layer.Scales[_cachedAxisInfo.Identifier.ParallelAxisNumber];
        _minorLabelStyle.Paint(g, layer.CoordinateSystem, scaleWithTicks, _customTickSpacing ?? scaleWithTicks.TickSpacing, _cachedAxisInfo, outerDistance, true);
      }
    }

    /// <summary>
    /// Paints the axis title.
    /// </summary>
    /// <param name="g">The graphics context.</param>
    /// <param name="paintContext">The paint context.</param>
    /// <param name="layer">The plot area.</param>
    public void PaintTitle(IGraphicsContext3D g, Altaxo.Graph.IPaintContext paintContext, IPlotArea layer)
    {
      if (_axisTitle is not null)
      {
        _axisTitle.Paint(g, paintContext);
      }
    }

    /// <summary>
    /// Performs cleanup after painting.
    /// </summary>
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
        return _axisLineStyle is not null;
      }
    }

    /// <summary>
    /// Ensures that the axis line exists.
    /// </summary>
    /// <param name="context">The property context.</param>
    public void ShowAxisLine(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      if (_axisLineStyle is null)
        AxisLineStyle = new AxisLineStyle(context);
    }

    private void ShowAxisLine(bool hasTicksByDefault, CSAxisSide preferredTickSide, IReadOnlyPropertyBag context)
    {
      if (_axisLineStyle is null)
        AxisLineStyle = new AxisLineStyle(hasTicksByDefault, preferredTickSide, context);
    }

    /// <summary>
    /// Removes the axis line.
    /// </summary>
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
        return _majorLabelStyle is not null;
      }
    }

    /// <summary>
    /// Ensures that major labels exist.
    /// </summary>
    /// <param name="context">The property context.</param>
    public void ShowMajorLabels(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      if (_majorLabelStyle is null)
        MajorLabelStyle = new AxisLabelStyle(context) { CachedAxisInformation = _cachedAxisInfo };
    }

    /// <summary>
    /// Ensures that major labels exist on the specified side.
    /// </summary>
    /// <param name="preferredLabelSide">The preferred label side.</param>
    /// <param name="context">The property context.</param>
    public void ShowMajorLabels(CSAxisSide preferredLabelSide, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      if (_majorLabelStyle is null)
        MajorLabelStyle = new AxisLabelStyle(preferredLabelSide, context) { CachedAxisInformation = _cachedAxisInfo };
    }

    /// <summary>
    /// Removes the major labels.
    /// </summary>
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
        return _minorLabelStyle is not null;
      }
    }

    /// <summary>
    /// Ensures that minor labels exist.
    /// </summary>
    /// <param name="context">The property context.</param>
    public void ShowMinorLabels(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      if (_minorLabelStyle is null)
        MinorLabelStyle = new AxisLabelStyle(context) { CachedAxisInformation = _cachedAxisInfo };
    }

    /// <summary>
    /// Ensures that minor labels exist on the specified side.
    /// </summary>
    /// <param name="preferredLabelSide">The preferred label side.</param>
    /// <param name="context">The property context.</param>
    public void ShowMinorLabels(CSAxisSide preferredLabelSide, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      if (_minorLabelStyle is null)
        MinorLabelStyle = new AxisLabelStyle(preferredLabelSide, context) { CachedAxisInformation = _cachedAxisInfo };
    }

    /// <summary>
    /// Removes the minor labels.
    /// </summary>
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
        return _axisTitle is not null;
      }
    }

    /// <summary>
    /// Ensures that an axis title exists.
    /// </summary>
    /// <param name="context">The property context.</param>
    [MemberNotNull(nameof(_axisTitle))]
    public void ShowTitle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      if (_axisTitle is null)
      {
        ChildSetMember(ref _axisTitle, new TextGraphic(context) { Text = "axis title" });
        EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Removes the axis title.
    /// </summary>
    public void HideTitle()
    {
      Title = null;
    }

    /// <summary>Style of axis. Determines the line width and color of the axis and the ticks.</summary>
    public AxisLineStyle? AxisLineStyle
    {
      get
      {
        return _axisLineStyle;
      }
      set
      {
        if (ChildSetMember(ref _axisLineStyle, value))
        {
          if (_axisLineStyle is not null)
            _axisLineStyle.CachedAxisInformation = _cachedAxisInfo;

          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Determines the style of the major labels.
    /// </summary>
    public AxisLabelStyle? MajorLabelStyle
    {
      get
      {
        return _majorLabelStyle;
      }
      set
      {
        if (ChildSetMember(ref _majorLabelStyle, value))
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Determines the style of the minor labels.
    /// </summary>
    public AxisLabelStyle? MinorLabelStyle
    {
      get
      {
        return _minorLabelStyle;
      }
      set
      {
        if (ChildSetMember(ref _minorLabelStyle, value))
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Gets or sets the axis title graphic.
    /// </summary>
    public TextGraphic? Title
    {
      get { return _axisTitle; }
      set
      {
        if (ChildSetMember(ref _axisTitle, value))
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Gets or sets the axis-title text.
    /// </summary>
    public string TitleText
    {
      get { return _axisTitle?.Text ?? string.Empty; }
      set
      {
        string oldvalue = TitleText;

        if (!(TitleText == value))
        {
          if (string.IsNullOrEmpty(value))
          {
            _axisTitle = null;
          }
          else
          {
            if (_axisTitle is null)
              ChildSetMember(ref _axisTitle, new TextGraphic(this.GetPropertyContext()));
            _axisTitle.Text = value;
          }
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    #endregion Properties

    #region ICloneable Members



    /// <summary>
    /// Creates a copy of this axis style.
    /// </summary>
    /// <returns>A cloned axis style.</returns>
    public object Clone()
    {
      return new AxisStyle(this);
    }

    #endregion ICloneable Members
  }
}

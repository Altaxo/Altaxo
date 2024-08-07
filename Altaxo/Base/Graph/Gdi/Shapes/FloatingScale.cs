﻿#region Copyright

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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Data;
using Altaxo.Drawing;
using Altaxo.Geometry;
using Altaxo.Graph.Gdi.Axis;
using Altaxo.Graph.Gdi.Background;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Rescaling;
using Altaxo.Graph.Scales.Ticks;

namespace Altaxo.Graph.Gdi.Shapes
{
  /// <summary>Enumerates the kind of span that determines the length of the floating scale.</summary>
  public enum FloatingScaleSpanType
  {
    /// <summary>
    /// The span value is a logical value. This is the ratio corresponding to the length of the underlying scale. Thus, a value of 0.5 means half the length of the underlying scale.
    /// </summary>
    IsLogicalValue,

    /// <summary>
    /// The span value is a physical value, and is given as difference of end and org of the floating scale. Thus, if the span value is for example 3 and the org of the floating scale is 2, then the end of the floating scale will be 2 + 3 = 5.
    /// </summary>
    IsPhysicalEndOrgDifference,

    /// <summary>
    /// The span value is a physical value, and is given as ratio of end to org of the floating scale. Thus, if the span value is for example 3 and the org of the floating scale is 2, then the end of the floating scale will be 2 * 3 =6.
    /// </summary>
    IsPhysicalEndOrgRatio
  }

  [Serializable]
  public class FloatingScale : GraphicBase
  {
    /// <summary>Number of the scale to measure (0: x-axis, 1: y-axis, 2: z-axis).</summary>
    private int _scaleNumber;

    /// <summary>Designates the type of scale span value, i.e. whether it is interpreted as a logical value, or a physical value (either as a span difference or as an end/org ratio).</summary>
    private FloatingScaleSpanType _scaleSpanType;

    /// <summary>The span this scale should show. It is either a physical or a logical value, depending on <see cref="_scaleSpanType"/>.</summary>
    private double _scaleSpanValue;

    private ScaleSegmentType _scaleSegmentType;

    private TickSpacing _tickSpacing;

    private AxisStyle _axisStyle;

    private Margin2D _backgroundPadding;

    private IBackgroundStyle? _background;

    // Cached members
    /// <summary>Cached path of the isoline.</summary>
    private GraphicsPath? _cachedPath;

    /// <summary>
    /// A segment of the underlying X-X layer, that has the position and the size of the floating scale we want to draw.
    /// </summary>
    private LayerSegment? _cachedLayerSegment;

    /// <summary>
    /// A scale of the same type as the scale of the underlying X-Y layer, which is used for drawing this floating scale.
    /// </summary>
    private ScaleSegment? _cachedScale;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FloatingScale), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (FloatingScale)obj;
        info.AddBaseValueEmbedded(s, typeof(FloatingScale).BaseType!);

        info.AddValue("ScaleNumber", s._scaleNumber);
        info.AddEnum("ScaleSpanType", s._scaleSpanType);
        info.AddValue("ScaleSpanValue", s._scaleSpanValue);
        info.AddEnum("ScaleType", s._scaleSegmentType);
        info.AddValue("TickSpacing", s._tickSpacing);
        info.AddValue("AxisStyle", s._axisStyle);

        info.AddValueOrNull("Background", s._background);
        if (s._background is not null)
          info.AddValue("BackgroundPadding", s._backgroundPadding);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (FloatingScale?)o ?? new FloatingScale(info);
        info.GetBaseValueEmbedded(s, typeof(FloatingScale).BaseType!, parent);

        s._scaleNumber = info.GetInt32("ScaleNumber");
        s._scaleSpanType = (FloatingScaleSpanType)info.GetEnum("ScaleSpanType", typeof(FloatingScaleSpanType));
        s._scaleSpanValue = info.GetDouble("ScaleSpanValue");
        s._scaleSegmentType = (ScaleSegmentType)info.GetEnum("ScaleType", typeof(ScaleSegmentType));

        s.ChildSetMember(ref s._tickSpacing, (TickSpacing)info.GetValue("TickSpacing", s));
        s.ChildSetMember(ref s._axisStyle, (AxisStyle)info.GetValue("AxisStyle", s));
        s.ChildSetMember(ref s._background, info.GetValueOrNull<IBackgroundStyle>("Background", s));
        if (s._background is not null)
        {
          s._backgroundPadding = (Margin2D)info.GetValue("BackgroundPadding", s);
        }

        return s;
      }
    }

    #endregion Serialization

    #region Constructors

    /// <summary>Constructor only for deserialization purposes.</summary>
    /// <param name="info">Not used here.</param>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    private FloatingScale(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
      : base(new ItemLocationDirectAutoSize())
    {
    }

    public FloatingScale(Main.Properties.IReadOnlyPropertyBag context)
      : base(new ItemLocationDirectAutoSize())
    {
      _scaleSpanValue = 0.25;
      _tickSpacing = new SpanTickSpacing();
      _axisStyle = new AxisStyle(new CSLineID(0, 0), true, false, true, null, context);
    }

    public FloatingScale(FloatingScale from)
      : base(from)
    {
      CopyFrom(from, false);
    }

    [MemberNotNull(nameof(_scaleSpanValue), nameof(_tickSpacing), nameof(_axisStyle))]
    protected void CopyFrom(FloatingScale from, bool withBaseMembers)
    {
      if (withBaseMembers)
        base.CopyFrom(from, withBaseMembers);

      _cachedPath = null;

      _scaleSpanValue = from._scaleSpanValue;
      _scaleSpanType = from._scaleSpanType;
      _scaleNumber = from._scaleNumber;
      _scaleSegmentType = from._scaleSegmentType;

      CopyHelper.Copy(ref _tickSpacing, from._tickSpacing);
      CopyHelper.Copy(ref _axisStyle, from._axisStyle);

      _backgroundPadding = from._backgroundPadding;
      CopyHelper.Copy(ref _background, from._background);
    }

    public override bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;
      if (obj is FloatingScale from)
      {
        using (var suspendToken = SuspendGetToken())
        {
          CopyFrom(from, true);
          EhSelfChanged(EventArgs.Empty);
        }
        return true;
      }
      else
      {
        return base.CopyFrom(obj);
      }
    }

    public override object Clone()
    {
      return new FloatingScale(this);
    }

    protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_tickSpacing is not null)
        yield return new Main.DocumentNodeAndName(_tickSpacing, "TickSpacing");
      if (_axisStyle is not null)
        yield return new Main.DocumentNodeAndName(_axisStyle, "AxisStyle");
      if (_background is not null)
        yield return new Main.DocumentNodeAndName(_background, "Background");
    }

    #endregion Constructors

    public override void FixupInternalDataStructures()
    {
      base.FixupInternalDataStructures();

      var layer = Main.AbsoluteDocumentPath.GetRootNodeImplementing<XYPlotLayer>(this);
      if (layer is null)
      {
        _cachedLayerSegment = null;

        _cachedScale?.Dispose();
        _cachedScale = null;
        return;
      }

      layer.CoordinateSystem.LayerToLogicalCoordinates(X, Y, out var rBegin);

      Logical3D rEnd = rBegin;
      switch (_scaleSpanType)
      {
        case FloatingScaleSpanType.IsLogicalValue:
          rEnd[_scaleNumber] = rBegin[_scaleNumber] + _scaleSpanValue;
          break;

        case FloatingScaleSpanType.IsPhysicalEndOrgDifference:
          {
            var physValue = layer.Scales[_scaleNumber].NormalToPhysicalVariant(rBegin[_scaleNumber]);
            physValue += _scaleSpanValue; // to be replaced by the scale span
            var logValue = layer.Scales[_scaleNumber].PhysicalVariantToNormal(physValue);
            rEnd[_scaleNumber] = logValue;
          }
          break;

        case FloatingScaleSpanType.IsPhysicalEndOrgRatio:
          {
            var physValue = layer.Scales[_scaleNumber].NormalToPhysicalVariant(rBegin[_scaleNumber]);
            physValue *= _scaleSpanValue; // to be replaced by the scale span
            var logValue = layer.Scales[_scaleNumber].PhysicalVariantToNormal(physValue);
            rEnd[_scaleNumber] = logValue;
          }
          break;
      }

      // axis style
      var csLineId = new CSLineID(_scaleNumber, rBegin);
      if (_axisStyle.StyleID != csLineId)
      {
        var propertyContext = this.GetPropertyContext();
        var axStyle = new AxisStyle(new CSLineID(_scaleNumber, rBegin), false, false, false, null, propertyContext);
        axStyle.CopyWithoutIdFrom(_axisStyle);
        _axisStyle = axStyle;
      }

      _cachedScale?.Dispose();
      _cachedScale = new ScaleSegment(layer.Scales[_scaleNumber], rBegin[_scaleNumber], rEnd[_scaleNumber], _scaleSegmentType);
      _tickSpacing.FinalProcessScaleBoundaries(_cachedScale.OrgAsVariant, _cachedScale.EndAsVariant, _cachedScale);
      _cachedScale.TickSpacing = _tickSpacing;
      _cachedLayerSegment = new LayerSegment(layer, _cachedScale, rBegin, rEnd, _scaleNumber);

      _axisStyle.FixupInternalDataStructures(_cachedLayerSegment, _cachedLayerSegment.GetAxisStyleInformation); // we use here special AxisStyleInformation not provided by the underlying CS, but by the layer segment
    }

    public override bool IsCompatibleWithParent(object parentObject)
    {
      return parentObject is XYPlotLayer;
    }

    public AxisStyle AxisStyle
    {
      get
      {
        return _axisStyle;
      }
    }

    public ScaleSegmentType ScaleType
    {
      get
      {
        return _scaleSegmentType;
      }
      set
      {
        var oldValue = _scaleSegmentType;
        _scaleSegmentType = value;
        if (oldValue != value)
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public TickSpacing TickSpacing
    {
      get
      {
        return _tickSpacing;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException();

        _tickSpacing = (TickSpacing)value.Clone();

        EhSelfChanged(EventArgs.Empty);
      }
    }

    public int ScaleNumber
    {
      get
      {
        return _scaleNumber;
      }
      set
      {
        _scaleNumber = value;
      }
    }

    public double ScaleSpanValue
    {
      get
      {
        return _scaleSpanValue;
      }
      set
      {
        _scaleSpanValue = value;
      }
    }

    public FloatingScaleSpanType ScaleSpanType
    {
      get
      {
        return _scaleSpanType;
      }
      set
      {
        _scaleSpanType = value;
      }
    }

    public Margin2D BackgroundPadding
    {
      get
      {
        return _backgroundPadding;
      }
      set
      {
        var oldValue = _backgroundPadding;
        _backgroundPadding = value;
        if (!value.Equals(oldValue))
          EhSelfChanged(EventArgs.Empty);
      }
    }

    public IBackgroundStyle? Background
    {
      get
      {
        return _background;
      }
      set
      {
        var oldValue = _background;
        _background = value;
        if (!object.ReferenceEquals(value, oldValue))
          EhSelfChanged(EventArgs.Empty);
      }
    }

    public override bool AllowNegativeSize
    {
      get
      {
        return true;
      }
    }

    public override bool AutoSize
    {
      get
      {
        return true;
      }
    }

    protected override void SetPosition(PointD2D value, Main.EventFiring eventFiring)
    {
      var oldPosition = GetPosition();
      base.SetPosition(value, eventFiring);

      if (_axisStyle.Title is not null)
      {
        var oldTitlePos = _axisStyle.Title.Position;
        _axisStyle.Title.SilentSetPosition(oldTitlePos + (GetPosition() - oldPosition));
      }
    }

    public override void SilentSetPosition(PointD2D newPosition)
    {
      var oldPosition = GetPosition();
      base.SilentSetPosition(newPosition);
      if (_axisStyle.Title is not null)
      {
        var oldTitlePos = _axisStyle.Title.Position;
        _axisStyle.Title.SilentSetPosition(oldTitlePos + (GetPosition() - oldPosition));
      }
    }

    public GraphicsPath GetSelectionPath()
    {
      return (GraphicsPath?)_cachedPath?.Clone() ?? throw new InvalidOperationException("Path not set yet!");
    }

    public override GraphicsPath GetObjectOutlineForArrangements()
    {
      return (GraphicsPath?)_cachedPath?.Clone() ?? throw new InvalidOperationException("Path not set yet!");
    }

    protected GraphicsPath? GetPath(double minWidth)
    {
      return (GraphicsPath?)_cachedPath?.Clone() ?? throw new InvalidOperationException("Path not set yet!");
    }

    public override IHitTestObject? HitTest(HitTestPointData htd)
    {
      if (_axisStyle.Title is not null)
      {
        var titleResult = _axisStyle.Title.HitTest(htd);
        if (titleResult is not null)
        {
          titleResult.Remove = EhTitleRemove;
          return titleResult;
        }
      }

      var pt = htd.GetHittedPointInWorldCoord();
      HitTestObjectBase? result = null;
      GraphicsPath gp = GetSelectionPath();
      if (gp.IsVisible(pt.ToGdi()))
      {
        result = new MyHitTestObject(this);
      }

      if (result is not null)
        result.DoubleClick = EhHitDoubleClick;

      return result;
    }

    private static bool EhHitDoubleClick(IHitTestObject o)
    {
      object hitted = o.HittedObject;
      Current.Gui.ShowDialog(ref hitted, "Floating scale properties", true);
      ((FloatingScale)hitted).EhSelfChanged(EventArgs.Empty);
      return true;
    }

    private static bool EhTitleRemove(IHitTestObject o)
    {
      object hitted = o.HittedObject;
      if (((TextGraphic)hitted).ParentObject is AxisStyle axStyle)
        axStyle.Title = null;
      return true;
    }

    public override void Paint(Graphics g, IPaintContext paintContext)
    {
      if (_cachedLayerSegment is null) // _privLayer should be set before in FixupInternalDataStructures
      {
        PaintErrorInvalidLayerType(g, paintContext);
        return;
      }

      if (_background is null)
      {
        _axisStyle.Paint(g, paintContext, _cachedLayerSegment, _cachedLayerSegment.GetAxisStyleInformation);
      }
      else
      {
        // if we have a background, we paint in a dummy bitmap in order to measure all items
        // the real painting is done later on after painting the background.
        using (var bmp = new Bitmap(4, 4))
        {
          using (var gg = Graphics.FromImage(bmp))
          {
            _axisStyle.Paint(gg, paintContext, _cachedLayerSegment, _cachedLayerSegment.GetAxisStyleInformation);
          }
        }
      }

      _cachedPath = _axisStyle.AxisLineStyle?.GetObjectPath(_cachedLayerSegment, true) ?? new GraphicsPath();

      // calculate size information
      RectangleD2D? bounds1 = _cachedPath.PointCount > 0 ? _cachedPath.GetBounds().ToAxo() : (RectangleD2D?)null;

      if (_axisStyle.AreMinorLabelsEnabled)
      {
        var path = _axisStyle.MinorLabelStyle?.GetSelectionPath();
        if (path is not null && path.PointCount > 0)
        {
          _cachedPath.AddPath(path, false);
          RectangleD2D bounds2 = path.GetBounds().ToAxo();
          bounds1 = RectangleD2D.ExpandToInclude(bounds1, bounds2);
        }
      }
      if (_axisStyle.AreMajorLabelsEnabled)
      {
        var path = _axisStyle.MajorLabelStyle?.GetSelectionPath();
        if (path is not null && path.PointCount > 0)
        {
          _cachedPath.AddPath(path, false);
          RectangleD2D bounds2 = path.GetBounds().ToAxo();
          bounds1 = RectangleD2D.ExpandToInclude(bounds1, bounds2);
        }
      }

      if (bounds1.HasValue)
      {
        ((ItemLocationDirectAutoSize)_location).SetSizeInAutoSizeMode(bounds1.Value.Size, false); // size here is important only for selection, thus we set size silently

        if (_background is not null)
        {
          bounds1.Value.Expand(_backgroundPadding);
          _background.Draw(g, bounds1.Value);
          _axisStyle.Paint(g, paintContext, _cachedLayerSegment, _cachedLayerSegment.GetAxisStyleInformation);
        }
      }
    }

    private void PaintErrorInvalidLayerType(Graphics g, object obj)
    {
      string errorMsg = "FloatingScale:Error: Invalid layer type";
      var font = GdiFontManager.ToGdi(GdiFontManager.GetFontXGenericSansSerif(10, FontXStyle.Regular));
      var size = g.MeasureString(errorMsg, font);
      if (obj is HostLayer)
      {
        var destSizeX = 0.2 * ((HostLayer)obj).Size.X;
        var factor = destSizeX / size.Width;
        font = GdiFontManager.ToGdi(GdiFontManager.GetFontXGenericSansSerif(font.Size * factor, FontXStyle.Regular));
      }

      g.DrawString(errorMsg, font, Brushes.Red, Position.ToGdi());
      size = g.MeasureString(errorMsg, font);

      _cachedPath = new GraphicsPath();
      _cachedPath.AddRectangle(new RectangleF(Position.ToGdi(), size));

      ((ItemLocationDirectAutoSize)_location).SetSizeInAutoSizeMode(size.ToPointD2D());
    }

    #region Inner classes

    private class LayerSegment : IPlotArea
    {
      private IPlotArea _underlyingArea;
      private Logical3D _org;
      private Logical3D _end;
      private int _scaleNumber;

      private ScaleCollection _scaleCollection = new ScaleCollection();

      public LayerSegment(IPlotArea underlyingArea, Scale scale, Logical3D org, Logical3D end, int scaleNumber)
      {
        _underlyingArea = underlyingArea;
        _org = org;
        _end = end;
        _scaleNumber = scaleNumber;

        for (int i = 0; i < _underlyingArea.Scales.Count; ++i)
        {
          if (i == _scaleNumber)
            _scaleCollection[i] = scale;
          else
            _scaleCollection[i] = (Scale)underlyingArea.Scales[i].Clone();
        }
      }

      public CSAxisInformation GetAxisStyleInformation(CSLineID lineId)
      {
        var result = _underlyingArea.CoordinateSystem.GetAxisStyleInformation(lineId).WithIdentifier(new CSLineID(lineId.ParallelAxisNumber, _org));
        result = result.WithLogicalValuesForAxisOrgAndEnd(
                  LogicalValueAxisOrg: _org[_scaleNumber],
                  LogicalValueAxisEnd: _end[_scaleNumber]);

        return result;
      }

      public bool Is3D
      {
        get { return _underlyingArea.Is3D; }
      }

      public Scale XAxis
      {
        get { return _scaleCollection[0]; }
      }

      public Scale YAxis
      {
        get { return _scaleCollection[1]; }
      }

      public ScaleCollection Scales
      {
        get { return _scaleCollection; }
      }

      public G2DCoordinateSystem CoordinateSystem
      {
        get { return _underlyingArea.CoordinateSystem; }
      }

      public PointD2D Size
      {
        get { throw new NotImplementedException(); }
      }

      public Logical3D GetLogical3D(I3DPhysicalVariantAccessor acc, int idx)
      {
        throw new NotImplementedException();
      }

      public Logical3D GetLogical3D(AltaxoVariant x, AltaxoVariant y)
      {
        throw new NotImplementedException();
      }

      public System.Collections.Generic.IEnumerable<CSLineID> AxisStyleIDs
      {
        get { throw new NotImplementedException(); }
      }

      public CSPlaneID UpdateCSPlaneID(CSPlaneID id)
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Enumerates the type of scale segment
    /// </summary>
    public enum ScaleSegmentType
    {
      /// <summary>Scale segment corresponds to the segment of the parent scale.</summary>
      Normal,

      /// <summary>Measures differences from org, thus the physical value of org is evaluated to zero (0).</summary>
      DifferenceToOrg,

      /// <summary>Measures ratios to org, thus the physical value of org is evaluated to one (1).</summary>
      RatioToOrg
    }

    [DisplayName("${res:ClassNames.Altaxo.Graph.Gdi.Shapes.FloatingScale+ScaleSegment}")]
    private class ScaleSegment : Scale
    {
      private double _relOrg;
      private double _relEnd;
      private Scale _underlyingScale;
      private ScaleSegmentType _segmentScaling;
      private TickSpacing? _tickSpacing;

      public ScaleSegment(Scale underlyingScale, double relOrg, double relEnd, ScaleSegmentType scaling)
      {
        if (underlyingScale is null)
          throw new ArgumentNullException("underlyingScale");

        _underlyingScale = underlyingScale;
        _relOrg = relOrg;
        _relEnd = relEnd;
        _segmentScaling = scaling;
      }

      public override bool CopyFrom(object obj)
      {
        if (ReferenceEquals(this, obj))
          return true;

        var from = obj as ScaleSegment;

        if (from is null)
          return false;

        using (var suspendToken = SuspendGetToken())
        {
          _relOrg = from._relOrg;
          _relEnd = from._relEnd;
          _underlyingScale = from._underlyingScale;
          _segmentScaling = from._segmentScaling;

          EhSelfChanged(EventArgs.Empty);
          suspendToken.Resume();
        }

        return true;
      }

      public override object Clone()
      {
        return new ScaleSegment(_underlyingScale, _relOrg, _relEnd, _segmentScaling);
      }

      protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
      {
        yield break; // do not dispose _underlyingScale !! we are not the owner (the owner is the layer the scale belongs to)
      }

      public override double PhysicalVariantToNormal(Altaxo.Data.AltaxoVariant x)
      {
        switch (_segmentScaling)
        {
          case ScaleSegmentType.DifferenceToOrg:
            x += _underlyingScale.NormalToPhysicalVariant(_relOrg);
            break;

          case ScaleSegmentType.RatioToOrg:
            x *= _underlyingScale.NormalToPhysicalVariant(_relOrg);
            break;
        }

        double r = _underlyingScale.PhysicalVariantToNormal(x);
        return (r - _relOrg) / (_relEnd - _relOrg);
      }

      public override Altaxo.Data.AltaxoVariant NormalToPhysicalVariant(double x)
      {
        double r = _relOrg * (1 - x) + _relEnd * x;
        var y = _underlyingScale.NormalToPhysicalVariant(r);
        switch (_segmentScaling)
        {
          case ScaleSegmentType.DifferenceToOrg:
            y -= _underlyingScale.NormalToPhysicalVariant(_relOrg);
            break;

          case ScaleSegmentType.RatioToOrg:
            y /= _underlyingScale.NormalToPhysicalVariant(_relOrg);
            break;
        }
        return y;
      }

      public override IScaleRescaleConditions? RescalingObject
      {
        get { return _underlyingScale.RescalingObject; }
      }

      public override Altaxo.Graph.Scales.Boundaries.IPhysicalBoundaries DataBoundsObject
      {
        get { return _underlyingScale.DataBoundsObject; }
      }

      public override Altaxo.Data.AltaxoVariant OrgAsVariant
      {
        get
        {
          return NormalToPhysicalVariant(0);
        }
      }

      public override Altaxo.Data.AltaxoVariant EndAsVariant
      {
        get
        {
          return NormalToPhysicalVariant(1);
        }
      }

      protected override string? SetScaleOrgEnd(Altaxo.Data.AltaxoVariant org, Altaxo.Data.AltaxoVariant end)
      {
        _relOrg = _underlyingScale.PhysicalVariantToNormal(org);
        _relEnd = _underlyingScale.PhysicalVariantToNormal(end);
        return null;
      }

      public override void OnUserRescaled()
      {
      }

      public override void OnUserZoomed(Data.AltaxoVariant newZoomOrg, Data.AltaxoVariant newZoomEnd)
      {
      }

      public override TickSpacing TickSpacing
      {
        get
        {
          return _tickSpacing ?? throw new InvalidOperationException();
        }
        set
        {

          ChildSetMember(ref _tickSpacing, value ?? throw new ArgumentNullException(nameof(TickSpacing)));
        }
      }
    }

    #endregion Inner classes

    #region HitTestObject

    /// <summary>Creates a new hit test object. Here, a special hit test object is constructed, which suppresses the resize, rotate, scale and shear grips.</summary>
    /// <returns>A newly created hit test object.</returns>
    protected override IHitTestObject GetNewHitTestObject()
    {
      return new MyHitTestObject(this);
    }

    private class MyHitTestObject : GraphicBaseHitTestObject
    {
      public MyHitTestObject(FloatingScale obj)
        : base(obj)
      {
      }

      public override IGripManipulationHandle[] GetGrips(double pageScale, int gripLevel)
      {
        return ((FloatingScale)_hitobject).GetGrips(this, pageScale, GripKind.Move);
      }
    }

    #endregion HitTestObject
  } // End Class
} // end Namespace

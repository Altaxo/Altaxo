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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Gdi.Shapes
{
  using Altaxo.Graph;
  using Altaxo.Graph.Gdi.Axis;
  using Altaxo.Graph.Gdi.Plot;
  using Altaxo.Graph.Scales;
  using Altaxo.Graph.Scales.Ticks;
  using Data;
  using Geometry;

  [Serializable]
  public class DensityImageLegend : GraphicBase, Main.IChildChangedEventSink
  {
    private const int _bitmapPixelsAcross = 2;
    private const int _bitmapPixelsAlong = 1024;

    /// <summary>The proxy for the plot item this legend is intended for.</summary>
    private Altaxo.Main.RelDocNodeProxy _plotItemProxy;

    /// <summary>
    /// Axis styles for both sides of the density plot item.
    /// </summary>
    protected AxisStyleCollection _axisStyles;

    // Cached members
    private Bitmap _bitmap;

    private DensityLegendArea _cachedArea;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DensityImageLegend), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DensityImageLegend)obj;
        info.AddBaseValueEmbedded(s, typeof(DensityImageLegend).BaseType);

        info.AddValue("PlotItem", s._plotItemProxy);
        info.AddValue("IsOrientationVertical", s.IsOrientationVertical);
        info.AddValue("IsScaleReversed", s.IsScaleReversed);
        info.AddValue("Scale", s.ScaleWithTicks);
        info.AddValue("TickSpacing", s.ScaleWithTicks.TickSpacing);
        info.AddValue("AxisStyles", s._axisStyles);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        DensityImageLegend s = null != o ? (DensityImageLegend)o : new DensityImageLegend();
        info.GetBaseValueEmbedded(s, typeof(DensityImageLegend).BaseType, parent);

        s._plotItemProxy = (Main.RelDocNodeProxy)info.GetValue("PlotItem", s);
        bool isOrientationVertical = info.GetBoolean("IsOrientationVertical");
        bool isScaleReversed = info.GetBoolean("IsScaleReversed");

        var cachedScale = (NumericalScale)info.GetValue("Scale", s); // TODO replace next 3 lines with serialization / deserialization of ScaleWithTicks
        var scaleTickSpacing = (TickSpacing)info.GetValue("TickSpacing", s);
        scaleTickSpacing.FinalProcessScaleBoundaries(cachedScale.OrgAsVariant, cachedScale.EndAsVariant, cachedScale);

        s._axisStyles = (AxisStyleCollection)info.GetValue("AxisStyles", s);
        s._axisStyles.ParentObject = s;

        s._cachedArea = new DensityLegendArea(s.Size, isOrientationVertical, isScaleReversed, cachedScale, scaleTickSpacing) { ParentObject = s };
        s._axisStyles.UpdateCoordinateSystem(s._cachedArea.CoordinateSystem);

        return s;
      }
    }

    private DensityImageLegend()
      : base(new ItemLocationDirect())
    {
    }

    #endregion Serialization

    public DensityImageLegend(DensityImagePlotItem plotItem, Main.IDocumentNode futureParentObject, PointD2D initialLocation, PointD2D graphicSize, Main.Properties.IReadOnlyPropertyBag context)
      : base(new ItemLocationDirect())
    {
      if (null == plotItem)
        throw new ArgumentNullException("plotItem");
      if (null == futureParentObject)
        throw new ArgumentNullException("futureParentObject");

      ParentObject = futureParentObject;
      PlotItem = plotItem;
      if (null == _plotItemProxy.DocumentPath)
        throw new ArgumentException("No path could be found between plotItem and futureParentObject. This is an indication that one of the objects is not rooted.");

      SetSize(graphicSize.X, graphicSize.Y, Main.EventFiring.Suppressed);
      SetPosition(initialLocation, Main.EventFiring.Suppressed);

      // _orientationIsVertical = true;
      // _scaleIsReversed = false;

      var cachedScale = (NumericalScale)PlotItem.Style.Scale.Clone();
      var scaleTickSpacing = Altaxo.Graph.Scales.Scale.CreateDefaultTicks(cachedScale.GetType());
      _cachedArea = new DensityLegendArea(Size, true, false, cachedScale, scaleTickSpacing);
      //_cachedArea.ParentObject = this; // --> moved to the end of this function

      _axisStyles = new AxisStyleCollection();
      _axisStyles.UpdateCoordinateSystem(_cachedArea.CoordinateSystem);
      // _axisStyles.ParentObject = this; --> see below

      var sx0 = new AxisStyle(CSLineID.X0, true, true, false, "Z values", context);
      sx0.AxisLineStyle.FirstDownMajorTicks = true;
      sx0.AxisLineStyle.FirstUpMajorTicks = false;
      sx0.AxisLineStyle.FirstDownMinorTicks = true;
      sx0.AxisLineStyle.FirstUpMinorTicks = false;

      var sx1 = new AxisStyle(CSLineID.X1, true, false, false, null, context);
      sx1.AxisLineStyle.FirstDownMajorTicks = false;
      sx1.AxisLineStyle.FirstUpMajorTicks = false;
      sx1.AxisLineStyle.FirstDownMinorTicks = false;
      sx1.AxisLineStyle.FirstUpMinorTicks = false;

      var sy0 = new AxisStyle(CSLineID.Y0, true, false, false, "Color map", context);
      var sy1 = new AxisStyle(CSLineID.Y1, true, false, false, null, context);
      _axisStyles.Add(sx0);
      _axisStyles.Add(sx1);
      _axisStyles.Add(sy0);
      _axisStyles.Add(sy1);

      sx0.Title.Rotation = 90;
      sx0.Title.Location.ParentAnchorX = RADouble.NewRel(0); // Left
      sx0.Title.Location.ParentAnchorY = RADouble.NewRel(0.5); // Center
      sx0.Title.Location.LocalAnchorX = RADouble.NewRel(0.5); // Center
      sx0.Title.Location.LocalAnchorY = RADouble.NewRel(1); // Bottom
      sx0.Title.X = -Width / 3;
      sx0.Title.Y = 0;

      sy0.Title.Location.ParentAnchorX = RADouble.NewRel(0.5); // Center
      sy0.Title.Location.ParentAnchorY = RADouble.NewRel(0); // Top
      sy0.Title.Location.LocalAnchorX = RADouble.NewRel(0.5); // Center
      sy0.Title.Location.LocalAnchorY = RADouble.NewRel(1); // Bottom
      sy0.Title.X = 0;
      sy0.Title.Y = sy0.Title.Height / 2;

      // set the parent objects
      _axisStyles.ParentObject = this;
      _cachedArea.ParentObject = this;
      UpdateTransformationMatrix();
    }

    public DensityImageLegend(DensityImageLegend from)
      : base(from)  // all is done here, since CopyFrom is virtual!
    {
    }

    public override bool CopyFrom(object obj)
    {
      var isCopied = base.CopyFrom(obj);
      if (isCopied && !object.ReferenceEquals(this, obj))
      {
        var from = obj as DensityImageLegend;
        if (null != from)
        {
          _cachedArea = new DensityLegendArea(from._cachedArea) { ParentObject = this };

          ChildCloneToMember(ref _axisStyles, from._axisStyles);
          _axisStyles.UpdateCoordinateSystem(_cachedArea.CoordinateSystem);
          _axisStyles.ParentObject = this;

          _bitmap = null != from._bitmap ? (Bitmap)from._bitmap.Clone() : null;

          _plotItemProxy = new Main.RelDocNodeProxy(from._plotItemProxy, true, this);
        }
      }
      return isCopied;
    }

    private IEnumerable<Main.DocumentNodeAndName> GetMyDocumentNodeChildrenWithName()
    {
      if (null != _axisStyles)
        yield return new Main.DocumentNodeAndName(_axisStyles, "AxisStyles");

      if (null != _cachedArea)
        yield return new Main.DocumentNodeAndName(_cachedArea, "LegendArea");

      if (null != _plotItemProxy)
        yield return new Main.DocumentNodeAndName(_plotItemProxy, "PlotItem");
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      return base.GetDocumentNodeChildrenWithName().Concat(GetMyDocumentNodeChildrenWithName());
    }

    public override object Clone()
    {
      return new DensityImageLegend(this);
    }

    private DensityImagePlotItem PlotItem
    {
      get
      {
        return null == _plotItemProxy ? null : _plotItemProxy.Document as DensityImagePlotItem;
      }
      set
      {
        if (object.ReferenceEquals(PlotItem, value))
          return;
        if (null == value)
          throw new ArgumentNullException("value");

        if (null != _plotItemProxy)
        {
          _plotItemProxy.Document = value;
        }
        else
        {
          ChildSetMember(ref _plotItemProxy, new Main.RelDocNodeProxy(value, this));
        }
      }
    }

    public AxisStyleCollection AxisStyles
    {
      get
      {
        return _axisStyles;
      }
    }

    public G2DCoordinateSystem CoordinateSystem
    {
      get
      {
        return _cachedArea.CoordinateSystem;
      }
    }

    public Scale ScaleWithTicks
    {
      get
      {
        return _cachedArea.Scales[0];
      }
    }

    public bool IsOrientationVertical
    {
      get
      {
        return ((CS.G2DCartesicCoordinateSystem)_cachedArea.CoordinateSystem).IsXYInterchanged;
      }
    }

    public bool IsScaleReversed
    {
      get
      {
        return ((CS.G2DCartesicCoordinateSystem)_cachedArea.CoordinateSystem).IsXReverse;
      }
    }

    /// <summary>
    /// Updates the internal transformation matrix to reflect the settings for position, rotation, scaleX, scaleY and shear. It is designed here by default so that
    /// the local anchor point of the object is located at the world coordinates (0,0). The transformation matrix update can be overridden in derived classes so
    /// that for instance the left upper corner of the object is located at (0,0).
    /// </summary>
    protected override void UpdateTransformationMatrix()
    {
      var locD = _location;
      _transformation.SetTranslationRotationShearxScale(locD.AbsolutePivotPositionX, locD.AbsolutePivotPositionY, -locD.Rotation, locD.ShearX, locD.ScaleX, locD.ScaleY);
      _transformation.TranslatePrepend(locD.AbsoluteVectorPivotToLeftUpper.X, locD.AbsoluteVectorPivotToLeftUpper.Y);
    }

    /// <summary>
    /// Transforms the graphics context is such a way, that the object can be drawn in local coordinates.
    /// </summary>
    /// <param name="g">Graphics context (should be saved beforehand).</param>
    protected override void TransformGraphics(Graphics g)
    {
      g.MultiplyTransform(_transformation);
    }

    /// <summary>
    /// Gets the bound of the object. The X and Y positions depend on the transformation model chosen for this graphic object: if the transformation takes into account the local anchor point,
    /// then the X and Y of the bounds are always 0 (which is the case here).
    /// </summary>
    /// <value>
    /// The bounds of the graphical object.
    /// </value>
    public override RectangleD2D Bounds
    {
      get
      {
        return new RectangleD2D(0, 0, Size.X, Size.Y);
      }
    }

    /// <summary>
    /// Get the object outline for arrangements in object world coordinates.
    /// </summary>
    /// <returns>Object outline for arrangements in object world coordinates</returns>
    public override GraphicsPath GetObjectOutlineForArrangements()
    {
      var result = new GraphicsPath();
      result.AddRectangle((RectangleF)Bounds);
      return result;
    }

    public override void FixupInternalDataStructures()
    {
      base.FixupInternalDataStructures();

      if (null == _cachedArea)
        return;

      if (null == PlotItem)
      {
        // search for the first density plot item in the layer
        var layer = Main.AbsoluteDocumentPath.GetRootNodeImplementing<XYPlotLayer>(this);
        if (null != layer)
        {
          foreach (var item in layer.PlotItems)
          {
            if (item is DensityImagePlotItem)
            {
              PlotItem = item as DensityImagePlotItem;
              break;
            }
          }
        }
      }

      UpdateIfPlotItemChanged();

      _cachedArea.Size = Size; // Update the coordinate system size to meet the size of the graph item
      _axisStyles.FixupInternalDataStructures(_cachedArea); // make sure the AxisStyles know about the size of the parent
    }

    public override void PaintPreprocessing(IPaintContext context)
    {
      base.PaintPreprocessing(context);
      _axisStyles.PaintPreprocessing(_cachedArea); // make sure the AxisStyles know about the size of the parent
    }

    public override void Paint(System.Drawing.Graphics g, IPaintContext paintContext)
    {
      if (null == _cachedArea)
        return;

      bool orientationIsVertical = IsOrientationVertical;
      bool scaleIsReversed = IsScaleReversed;

      int pixelH = orientationIsVertical ? _bitmapPixelsAcross : _bitmapPixelsAlong;
      int pixelV = orientationIsVertical ? _bitmapPixelsAlong : _bitmapPixelsAcross;

      if (null == _bitmap || _bitmap.Width != pixelH || _bitmap.Height != pixelV)
      {
        if (null != _bitmap)
          _bitmap.Dispose();

        _bitmap = new Bitmap(pixelH, pixelV, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
      }

      Data.AltaxoVariant porg;
      Data.AltaxoVariant pend;
      NumericalScale originalZScale;
      Plot.IColorProvider colorProvider;

      if (null != PlotItem)
      {
        porg = PlotItem.Style.Scale.OrgAsVariant;
        pend = PlotItem.Style.Scale.EndAsVariant;
        originalZScale = PlotItem.Style.Scale;
        colorProvider = PlotItem.Style.ColorProvider;
      }
      else
      {
        porg = 0;
        pend = 1;
        originalZScale = new LinearScale();
        colorProvider = new Plot.ColorProvider.ColorProviderBGRY();
      }

      var legendScale = (NumericalScale)ScaleWithTicks;
      var legendTickSpacing = ScaleWithTicks.TickSpacing;

      // Fill the bitmap

      for (int i = 0; i < _bitmapPixelsAlong; i++)
      {
        double r = (scaleIsReversed ^ orientationIsVertical) ? 1 - i / (double)(_bitmapPixelsAlong - 1) : i / (double)(_bitmapPixelsAlong - 1);
        double l = originalZScale.PhysicalToNormal(legendScale.NormalToPhysical(r));
        var color = colorProvider.GetColor(l);
        if (orientationIsVertical)
        {
          for (int j = 0; j < _bitmapPixelsAcross; j++)
            _bitmap.SetPixel(j, i, color);
        }
        else
        {
          for (int j = 0; j < _bitmapPixelsAcross; j++)
            _bitmap.SetPixel(i, j, color);
        }
      }

      var graphicsState = g.Save();
      TransformGraphics(g);

      {
        // Three tricks are neccessary to get the color legend (which is the bitmap) drawn smooth and uniformly:
        // Everything other than this will result in distorted image, or soft (unsharp) edges
        var graphicsState2 = g.Save(); // Of course, save the graphics state so we can make our tricks undone afterwards
        g.InterpolationMode = InterpolationMode.Default; // Trick1: Set the interpolation mode, whatever it was before, back to default
        g.PixelOffsetMode = PixelOffsetMode.Default;  // Trick2: Set the PixelOffsetMode, whatever it was before, back to default

        g.DrawImage(_bitmap,
          new RectangleF(0, 0, (float)Size.X, (float)Size.Y),
          new Rectangle(0, 0, pixelH - 1, pixelV - 1), GraphicsUnit.Pixel); // Trick3: Paint both in X and Y direction one pixel less than the source bitmap acually has, this prevents soft edges

        g.Restore(graphicsState2); // make our tricks undone here
      }
      _axisStyles.Paint(g, paintContext, _cachedArea);

      g.Restore(graphicsState);
    }

    private bool EhAxisTitleRemove(IHitTestObject o)
    {
      foreach (var axstyle in _axisStyles)
      {
        if (object.ReferenceEquals(o.HittedObject, axstyle.Title))
        {
          axstyle.Title = null;
          return true;
        }
      }
      return false;
    }

    public override IHitTestObject HitTest(HitTestPointData htd)
    {
      var myHitTestData = htd.NewFromAdditionalTransformation(_transformation);

      IHitTestObject result = null;
      foreach (var axstyle in _axisStyles)
      {
        if (null != axstyle.Title && null != (result = axstyle.Title.HitTest(myHitTestData)))
        {
          result.Remove = EhAxisTitleRemove;
          result.Transform(_transformation);
          return result;
        }
      }

      result = base.HitTest(htd);
      if (result != null)
        result.DoubleClick = EhDoubleClick;

      return result;
    }

    private bool EhDoubleClick(IHitTestObject o)
    {
      Current.Gui.ShowDialog(new object[] { o.HittedObject }, "Edit density image legend", true);
      return false;
    }

    #region IChildChangedEventSink Members

    private void UpdateIfPlotItemChanged()
    {
      if (null == PlotItem)
        return;

      // Test whether the scale type is still the same than the scale of the plot item
      // If not, we must create a new cachedArea
      if (_cachedArea.Scales[0].GetType() != PlotItem.Style.Scale.GetType())
      {
        var cachedScale = (NumericalScale)PlotItem.Style.Scale.Clone();
        var scaleTickSpacing = Altaxo.Graph.Scales.Scale.CreateDefaultTicks(cachedScale.GetType()); // we have to use CreateDefaultTicks because the scale of the DensityImagePlotStyle has NoTickspacing as TickSpacing
        _cachedArea = new DensityLegendArea(Size, true, false, cachedScale, scaleTickSpacing) { ParentObject = this };
      }

      // We set the boundaries of our legend scale to the org and end of the z-scale of the density image PlotItem.
      using (var suspendToken = ScaleWithTicks.DataBoundsObject.SuspendGetToken())
      {
        ScaleWithTicks.DataBoundsObject.Reset();

        // Note: we do not use PlotItem.Style.Scale.DataBoundsObject here to merge with ScaleWithTicks.DataBoundsObject
        // this is because in the PlotItem we can limit the scale to show only relevant data and to eliminate outliers
        // that's why we bind our DataBoundsObject to the resulting Scale org and end
        ScaleWithTicks.DataBoundsObject.Add(PlotItem.Style.Scale.Org);
        ScaleWithTicks.DataBoundsObject.Add(PlotItem.Style.Scale.End);

        suspendToken.Resume();
      }
    }

    protected override bool HandleHighPriorityChildChangeCases(object sender, ref EventArgs e)
    {
      if (object.ReferenceEquals(sender, _plotItemProxy))
      {
        UpdateIfPlotItemChanged();
      }

      return base.HandleHighPriorityChildChangeCases(sender, ref e);
    }

    #endregion IChildChangedEventSink Members

    #region Inner classes

    [Serializable]
    private class DensityLegendArea : Main.SuspendableDocumentNodeWithSetOfEventArgs, IPlotArea
    {
      private PointD2D _size;
      private ScaleCollection _scales;
      private CS.G2DCartesicCoordinateSystem _coordinateSystem;

      public DensityLegendArea(PointD2D size, bool isXYInterchanged, bool isXReversed, Scale scale, TickSpacing tickSpacing)
      {
        _size = size;
        _scales = new ScaleCollection() { ParentObject = this };
        _scales[0] = scale;
        scale.TickSpacing = tickSpacing;
        _scales[1] = new LinearScale() { TickSpacing = new NoTickSpacing() };
        _coordinateSystem = new Altaxo.Graph.Gdi.CS.G2DCartesicCoordinateSystem() { ParentObject = this };
        _coordinateSystem.IsXYInterchanged = isXYInterchanged;
        _coordinateSystem.IsXReverse = isXReversed;
        _coordinateSystem.UpdateAreaSize(_size);
      }

      public DensityLegendArea(DensityLegendArea from)
      {
        _size = from._size;

        _scales = from._scales.Clone();
        _scales.ParentObject = this;

        _coordinateSystem = (CS.G2DCartesicCoordinateSystem)from._coordinateSystem.Clone();
        _coordinateSystem.ParentObject = this;
      }

      protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
      {
        if (null != _scales)
          yield return new Main.DocumentNodeAndName(_scales, "Scales");

        if (null != _coordinateSystem)
          yield return new Main.DocumentNodeAndName(_coordinateSystem, "CoordinateSystem");
      }

      #region IPlotArea Members

      public bool Is3D
      {
        get { return false; }
      }

      public Scale XAxis
      {
        get { return _scales[0]; }
      }

      public Scale YAxis
      {
        get { return _scales[1]; }
      }

      public ScaleCollection Scales
      {
        get { return _scales; }
      }

      public G2DCoordinateSystem CoordinateSystem
      {
        get { return _coordinateSystem; }
      }

      public PointD2D Size
      {
        get { return _size; }
        set
        {
          var chg = _size != value;
          _size = value;
          if (chg)
          {
            _coordinateSystem.UpdateAreaSize(_size);
          }
        }
      }

      public Logical3D GetLogical3D(I3DPhysicalVariantAccessor acc, int idx)
      {
        Logical3D r;
        r.RX = XAxis.PhysicalVariantToNormal(acc.GetXPhysical(idx));
        r.RY = YAxis.PhysicalVariantToNormal(acc.GetYPhysical(idx));
        r.RZ = 0;
        return r;
      }

      public Logical3D GetLogical3D(AltaxoVariant x, AltaxoVariant y)
      {
        Logical3D r;
        r.RX = XAxis.PhysicalVariantToNormal(x);
        r.RY = YAxis.PhysicalVariantToNormal(y);
        r.RZ = 0;
        return r;
      }

      public IEnumerable<CSLineID> AxisStyleIDs
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public CSPlaneID UpdateCSPlaneID(CSPlaneID id)
      {
        throw new NotImplementedException();
      }

      #endregion IPlotArea Members
    }

    #endregion Inner classes
  }
}

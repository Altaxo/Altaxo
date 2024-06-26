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
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Calc.Interpolation;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Data;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;

namespace Altaxo.Graph.Gdi.Plot.Styles
{
  using System.ComponentModel;
  using System.Diagnostics.CodeAnalysis;
  using Altaxo.Calc;
  using Altaxo.Graph.Scales.Ticks;
  using ColorProvider;
  using Drawing;
  using Graph.Plot.Data;

  /// <summary>
  /// This plot style is responsible for showing density plots as pixel image. Because of the limitation to a pixel image, each pixel is correlated
  /// with a single data point in the table. Splining of data is not implemented here. Beause of this limitation, the image can only be shown
  /// on linear axes.
  /// </summary>
  [Serializable]
  [DisplayName("${res:ClassNames.Altaxo.Graph.Gdi.Plot.Styles.DensityImagePlotStyle}")]
  public class DensityImagePlotStyle
    :
    Main.SuspendableDocumentNodeWithEventArgs,
    Main.ICopyFrom
  {
    [Serializable]
    private enum CachedImageType { None, LinearEquidistant, Other };

    /// <summary>
    /// Deprecated: The kind of scaling of the values between from and to.
    /// </summary>
    private enum ScalingStyle
    {
      /// <summary>Linear scale, i.e. color changes linear between from and to.</summary>
      Linear,

      /// <summary>Logarithmic style, i.e. color changes with log(value) between log(from) and log(to).</summary>
      Logarithmic
    };

    /// <summary>
    /// The image which is shown during paint.
    /// </summary>
    [NonSerialized]
    private System.Drawing.Bitmap? _cachedImage;

    /// <summary>If true, the image is clipped to the layer boundaries.</summary>
    private bool _clipToLayer = true;

    /// <summary>Calculates the color from the relative value.</summary>
    private IColorProvider _colorProvider;

    /// <summary>
    /// Converts the numerical values into logical values.
    /// </summary>
    private NumericalScale _scale;

    /// <summary>The lower bound of the y plot range</summary>
    [NonSerialized]
    private AltaxoVariant _yRangeFrom = double.NaN;

    /// <summary>The upper bound of the y plot range</summary>
    [NonSerialized]
    private AltaxoVariant _yRangeTo = double.NaN;

    [NonSerialized]
    private CachedImageType _imageType;

    /// <summary>
    /// Stores the conditions under which the image is valid. Depends on the type of image.
    /// </summary>
    [NonSerialized]
    private object? _imageConditionMemento;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.DensityImagePlotStyle", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DensityImagePlotStyle)obj;

        // nothing to save up to now
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (DensityImagePlotStyle?)o ?? new DensityImagePlotStyle();

        // Nothing to deserialize in the moment

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.DensityImagePlotStyle", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.Plot.Styles.DensityImagePlotStyle", 2)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotImplementedException("This function should not be called, since a newer serialization version is available");
        /*
                DensityImagePlotStyle s = (DensityImagePlotStyle)obj;

                info.AddEnum("ScalingStyle", s._scalingStyle);
                info.AddValue("RangeFrom", s._vRangeFrom);
                info.AddValue("RangeTo", s._vRangeTo);
                info.AddValue("ClipToLayer", s._clipToLayer);
                info.AddValue("ColorBelow", s._colorBelow);
                info.AddValue("ColorAbove", s._colorAbove);
                info.AddValue("ColorInvalid", s._colorInvalid);
                */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (DensityImagePlotStyle?)o ?? new DensityImagePlotStyle();

        var scalingStyle = (ScalingStyle)info.GetEnum("ScalingStyle", typeof(ScalingStyle));
        var vRangeFrom = info.GetDouble("RangeFrom");
        var vRangeTo = info.GetDouble("RangeTo");
        s._clipToLayer = info.GetBoolean("ClipToLayer");
        var colorBelow = (NamedColor)info.GetValue("ColorBelow", s);
        var colorAbove = (NamedColor)info.GetValue("ColorAbove", s);
        var colorInvalid = (NamedColor)info.GetValue("ColorInvalid", s);

        var colorProvider = ColorProviderBGMYR.NewFromColorBelowAboveInvalidAndTransparency(colorBelow, colorAbove, colorInvalid, 0);
        var scale = scalingStyle == ScalingStyle.Logarithmic ? new Log10Scale() : (NumericalScale)new LinearScale();

        scale.Rescaling?.SetUserParameters(
          double.IsNaN(vRangeFrom) ? Altaxo.Graph.Scales.Rescaling.BoundaryRescaling.Auto : Altaxo.Graph.Scales.Rescaling.BoundaryRescaling.Fixed,
          vRangeFrom,
          double.IsNaN(vRangeTo) ? Altaxo.Graph.Scales.Rescaling.BoundaryRescaling.Auto : Altaxo.Graph.Scales.Rescaling.BoundaryRescaling.Fixed,
          vRangeTo);

        s.Scale = scale;
        s.ColorProvider = colorProvider;

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DensityImagePlotStyle), 3)]
    private class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DensityImagePlotStyle)obj;

        info.AddValue("ClipToLayer", s._clipToLayer);
        info.AddValue("Scale", s._scale);
        info.AddValue("Colorization", s._colorProvider);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (DensityImagePlotStyle?)o ?? new DensityImagePlotStyle();

        s._clipToLayer = info.GetBoolean("ClipToLayer");
        s.Scale = (NumericalScale)info.GetValue("Scale", s);
        s.ColorProvider = (IColorProvider)info.GetValue("Colorization", s);

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initialized the member variables to default values.
    /// </summary>
    protected void InitializeMembers()
    {
      _cachedImage = null;
    }

    /// <summary>
    /// Initializes the style to default values.
    /// </summary>
    public DensityImagePlotStyle()
    {
      ColorProvider = new ColorProvider.ColorProviderBGRY();
      Scale = new LinearScale() { TickSpacing = new NoTickSpacing() }; // Ticks are not needed here, they will only disturb the bounds of the scale
      InitializeMembers();
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">The style to copy from.</param>
    public DensityImagePlotStyle(DensityImagePlotStyle from)
    {
      InitializeMembers();
      CopyFrom(from);
    }

    [MemberNotNull(nameof(_colorProvider), nameof(_scale))]
    protected void CopyFrom(DensityImagePlotStyle from)
    {
      _clipToLayer = from._clipToLayer;
      ColorProvider = from._colorProvider;
      Scale = (NumericalScale)from._scale.Clone();
      _imageType = CachedImageType.None;
    }

    public bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      if (obj is DensityImagePlotStyle from)
      {
        using (var suspendToken = SuspendGetToken())
        {
          CopyFrom(from);

          EhSelfChanged();
          suspendToken.Resume();
        }
        return true;
      }

      return false;
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_scale is not null)
        yield return new Main.DocumentNodeAndName(_scale, "Scale");
    }

    public object Clone()
    {
      return new DensityImagePlotStyle(this);
    }

    public NumericalScale Scale
    {
      get
      {
        return _scale;
      }
      [MemberNotNull(nameof(_scale))]
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(_scale));

        if (ChildSetMember(ref _scale, value))
        {
          if (!(_scale.TickSpacing is NoTickSpacing))
            _scale.TickSpacing = new NoTickSpacing(); // strip the old tickspacing, use NoTickspacing, since Ticks are not needed in the density image plot style

          EhChildChanged(_scale, EventArgs.Empty);
        }
      }
    }

    public IColorProvider ColorProvider
    {
      get { return _colorProvider; }
      [MemberNotNull(nameof(_colorProvider))]
      set
      {
        if (value is null)
          throw new ArgumentNullException("value");

        if (!object.ReferenceEquals(value, _colorProvider))
        {
          _colorProvider = value;
          EhChildChanged(_colorProvider, EventArgs.Empty);
        }
      }
    }

    public bool ClipToLayer
    {
      get { return _clipToLayer; }
      set
      {
        bool oldValue = _clipToLayer;
        _clipToLayer = value;

        if (_clipToLayer != oldValue)
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    private bool NaNEqual(double a, double b)
    {
      if (double.IsNaN(a) && double.IsNaN(b))
        return true;

      return a == b;
    }

    /// <summary>
    /// Called by the parent plot item to indicate that the associated data has changed. Used to invalidate the cached bitmap to force
    /// rebuilding the bitmap from new data.
    /// </summary>
    /// <param name="sender">The sender of the message.</param>
    public void EhDataChanged(object sender)
    {
      _imageType = CachedImageType.None;
      EhSelfChanged(EventArgs.Empty);
    }

    /// <summary>
    /// This routine ensures that the plot item updates all its cached data and send the appropriate
    /// events if something has changed. Called before the layer paint routine paints the axes because
    /// it must be ensured that the axes are scaled correctly before the plots are painted.
    /// </summary>
    /// <param name="layer">The plot layer.</param>
    /// <param name="plotData">The plot data which are here used to determine the bounds of the independent data column.</param>
    public void PrepareScales(IPlotArea layer, XYZMeshedColumnPlotData plotData)
    {
      NumericalBoundaries pb = _scale.DataBounds;
      plotData.SetVBoundsFromTemplate(pb); // ensure that the right v-boundary type is set
      using (var suspendToken = pb.SuspendGetToken())
      {
        pb.Reset();
        plotData.MergeVBoundsInto(pb);
        suspendToken.Resume();
      }
    }

    /// <summary>
    /// Paint the density image in the layer.
    /// </summary>
    /// <param name="gfrx">The graphics context painting in.</param>
    /// <param name="gl">The layer painting in.</param>
    /// <param name="plotObject">The data to plot.</param>
    public void Paint(Graphics gfrx, IPlotArea gl, object plotObject) // plots the curve with the choosen style
    {
      if (!(plotObject is XYZMeshedColumnPlotData))
        return; // we cannot plot any other than a TwoDimMeshDataAssociation now

      var myPlotAssociation = (XYZMeshedColumnPlotData)plotObject;
      myPlotAssociation.DataTableMatrix.GetWrappers(
        gl.XAxis.PhysicalVariantToNormal, // transformation function for row header values
        Precision.IsFinite,       // selection functiton for row header values
        gl.YAxis.PhysicalVariantToNormal, // transformation function for column header values
        Precision.IsFinite,       // selection functiton for column header values
        out var matrix,
        out var logicalRowHeaderValues,
        out var logicalColumnHeaderValues
        );

      int cols = matrix.ColumnCount;
      int rows = matrix.RowCount;

      if (cols <= 0 || rows <= 0)
        return; // we cannot show a picture if one length is zero

      // there is a need for rebuilding the bitmap only if the data are invalid for some reason
      // if the cached image is valid, then test if the conditions hold any longer
      switch (_imageType)
      {
        case CachedImageType.LinearEquidistant:
          {
            var memento = new ImageTypeEquiLinearMemento(gl);
            if (!memento.Equals(_imageConditionMemento))
              _imageType = CachedImageType.None;
          }
          break;

        case CachedImageType.Other:
          {
            var memento = new ImageTypeOtherMemento(gl);
            if (!memento.Equals(_imageConditionMemento))
              _imageType = CachedImageType.None;
          }
          break;
      }

      // now build the image
      // note that the image type can change during the call of BuildImage
      if (_imageType == CachedImageType.None || _cachedImage is null)
      {
        BuildImage(gfrx, gl, myPlotAssociation, matrix, logicalRowHeaderValues, logicalColumnHeaderValues);
        switch (_imageType)
        {
          case CachedImageType.LinearEquidistant:
            _imageConditionMemento = new ImageTypeEquiLinearMemento(gl);
            break;

          case CachedImageType.Other:
            _imageConditionMemento = new ImageTypeOtherMemento(gl);
            break;
        }
      }

      // and now draw the image
      {
        // Three tricks are neccessary to get the bitmap drawn smooth and uniformly:
        // Everything other than this will result in distorted image, or soft (unsharp) edges
        var graphicsState = gfrx.Save(); // Of course, save the graphics state so we can make our tricks undone afterwards
        gfrx.InterpolationMode = InterpolationMode.Default; // Trick1: Set the interpolation mode, whatever it was before, back to default
        gfrx.PixelOffsetMode = PixelOffsetMode.Default;  // Trick2: Set the PixelOffsetMode, whatever it was before, back to default

        switch (_imageType)
        {
          case CachedImageType.LinearEquidistant:
            {
              double xOrgRel = logicalRowHeaderValues[0];
              double xEndRel = logicalRowHeaderValues[rows - 1];
              double yOrgRel = logicalColumnHeaderValues[0];
              double yEndRel = logicalColumnHeaderValues[cols - 1];
              bool isConvertible = true;
              isConvertible &= gl.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(xOrgRel, yEndRel), out var x0, out var y0);
              isConvertible &= gl.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(xOrgRel, yOrgRel), out var x1, out var y1);
              isConvertible &= gl.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(xEndRel, yEndRel), out var x2, out var y2);

              if (isConvertible)
              {
                if (_clipToLayer)
                  gfrx.Clip = gl.CoordinateSystem.GetRegion();

                var pts = new PointF[] { new PointF((float)x0, (float)y0), new PointF((float)x2, (float)y2), new PointF((float)x1, (float)y1) };
                gfrx.DrawImage(_cachedImage, pts, new RectangleF(0, 0, _cachedImage.Width - 1, _cachedImage.Height - 1), GraphicsUnit.Pixel); // Trick3: Paint both in X and Y direction one pixel less than the source bitmap acually has, this prevents soft edges
              }
            }
            break;

          case CachedImageType.Other:
            {
              gfrx.DrawImage(_cachedImage, 0, 0, (float)gl.Size.X, (float)gl.Size.Y);
            }
            break;
        }

        gfrx.Restore(graphicsState);
      }
    }

    private class ImageTypeEquiLinearMemento
    {
      private Type xtype, ytype;
      private Type cstype;

      public ImageTypeEquiLinearMemento(IPlotArea gl)
      {
        xtype = gl.XAxis.GetType();
        ytype = gl.YAxis.GetType();
        cstype = gl.CoordinateSystem.GetType();
      }

      public override bool Equals(object? obj)
      {
        return obj is ImageTypeEquiLinearMemento from &&
            xtype == from.xtype &&
            ytype == from.ytype &&
            cstype == from.cstype
          ;
      }

      public override int GetHashCode()
      {
        return base.GetHashCode() + 13 * xtype.GetHashCode() + 31 * ytype.GetHashCode();
      }
    }

    private class ImageTypeOtherMemento
    {
      private Type xtype, ytype;
      private AltaxoVariant xorg, xend, yorg, yend;

      private Type cstype;
      private double x00, x10, x01, x11, x32;
      private double y00, y10, y01, y11, y32;

      public ImageTypeOtherMemento(IPlotArea gl)
      {
        xtype = gl.XAxis.GetType();
        xorg = gl.XAxis.OrgAsVariant;
        xend = gl.XAxis.EndAsVariant;

        ytype = gl.YAxis.GetType();
        yorg = gl.YAxis.OrgAsVariant;
        yend = gl.YAxis.EndAsVariant;

        cstype = gl.CoordinateSystem.GetType();
        gl.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(0, 0), out x00, out y00);
        gl.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(1, 0), out x10, out y10);
        gl.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(0, 1), out x01, out y01);
        gl.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(1, 1), out x11, out y11);
        gl.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(0.3, 0.2), out x32, out y32);
      }

      public override bool Equals(object? obj)
      {
        var from = obj as ImageTypeOtherMemento;
        if (from is null)
          return false;
        else
          return
            xtype == from.xtype &&
            ytype == from.ytype &&
            xorg == from.xorg &&
            xend == from.xend &&
            yend == from.yend &&
            cstype == from.cstype &&
            x00 == from.x00 &&
            x10 == from.x10 &&
            x01 == from.x01 &&
            x11 == from.x11 &&
            x32 == from.x32 &&
            y00 == from.y00 &&
            y10 == from.y10 &&
            y01 == from.y01 &&
            y11 == from.y11 &&
            y32 == from.y32;
      }

      public override int GetHashCode()
      {
        return base.GetHashCode() + 13 * xtype.GetHashCode() + 31 * ytype.GetHashCode();
      }
    }

    [MemberNotNull(nameof(_cachedImage))]
    private void BuildImage(Graphics gfrx, IPlotArea gl, XYZMeshedColumnPlotData myPlotAssociation, IROMatrix<double> matrix, IReadOnlyList<double> logicalRowHeaderValues, IReadOnlyList<double> logicalColumnHeaderValues)
    {
      // ---------------- prepare the color scaling -------------------------------------

      // --------------- end preparation of color scaling ------------------------------

      // test if the coordinate system is affine and the scale is linear
      if (!gl.CoordinateSystem.IsAffine)
      {
        BuildImageV3(gfrx, gl, logicalRowHeaderValues, logicalColumnHeaderValues, matrix);
      }
      else // Coordinate System is affine
      {
        // now test lx and ly (only the valid indices for equidistantness
        bool isEquististantX = IsEquidistant(logicalRowHeaderValues, 0.2);
        bool isEquististantY = IsEquidistant(logicalColumnHeaderValues, 0.2);

        bool areLinearScales = (gl.XAxis is LinearScale) && (gl.YAxis is LinearScale);

        if (areLinearScales && isEquististantX && isEquististantY)
        {
          BuildImageV1(matrix); // affine, linear scales, and equidistant points
        }
        else if (areLinearScales)
        {
          BuildImageV3(gfrx, gl, logicalRowHeaderValues, logicalColumnHeaderValues, matrix); // affine and linear, but nonequidistant scale
        }
        else
        {
          BuildImageV3(gfrx, gl, logicalRowHeaderValues, logicalColumnHeaderValues, matrix); // affine, but nonlinear scales
        }
      }
    }

    private static bool IsEquidistant(double[] x, Altaxo.Collections.IAscendingIntegerCollection indices, double relthreshold)
    {
      if (indices.Count <= 1)
        return true;
      int N = indices.Count;
      double first = x[indices[0]];
      double last = x[indices[N - 1]];
      double spanByNM1 = (last - first) / (N - 1);
      double threshold = Math.Abs(relthreshold * spanByNM1);

      for (int i = 0; i < N; i++)
      {
        if (Math.Abs((x[indices[i]] - first) - i * spanByNM1) > threshold)
          return false;
      }
      return true;
    }

    private static bool IsEquidistant(IReadOnlyList<double> x, double relthreshold)
    {
      int NM1 = x.Count - 1;
      if (NM1 <= 0)
        return true;
      double first = x[0];
      double last = x[NM1];
      double spanByNM1 = (last - first) / (NM1);
      double threshold = Math.Abs(relthreshold * spanByNM1);

      for (int i = 0; i <= NM1; i++)
      {
        if (Math.Abs((x[i] - first) - i * spanByNM1) > threshold)
          return false;
      }
      return true;
    }

    private Color GetColor(double val)
    {
      double relval = _scale.PhysicalToNormal(val);
      return _colorProvider.GetColor(relval);
    }

    // CoordinateSystem is not affine, or scales are non-linear
    [MemberNotNull(nameof(_cachedImage))]
    private void BuildImageV3(Graphics gfrx, IPlotArea gl,
      IReadOnlyList<double> lx,
      IReadOnlyList<double> ly,
      IROMatrix<double> vcolumns)
    {
      // allocate a bitmap of same dimensions than the underlying layer
      _imageType = CachedImageType.Other;

      int dimX = (int)Math.Ceiling(gl.Size.X / 72.0 * gfrx.DpiX);
      int dimY = (int)Math.Ceiling(gl.Size.Y / 72.0 * gfrx.DpiY);

      dimX = Math.Min(2048, dimX);
      dimY = Math.Min(2048, dimY);

      // look if the image has the right dimensions
      if (_cachedImage is null || _cachedImage.Width != dimX || _cachedImage.Height != dimY)
      {
        if (_cachedImage is not null)
          _cachedImage.Dispose();

        // please notice: the horizontal direction of the image is related to the row index!!! (this will turn the image in relation to the table)
        // and the vertical direction of the image is related to the column index
        _cachedImage = new System.Drawing.Bitmap(dimX, dimY, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
      }

      byte[] imageBytes = new byte[dimX * dimY * 4];

      double widthByDimX = gl.Size.X / dimX;
      double heightByDimY = gl.Size.Y / dimY;
      var rel = new Logical3D();

      double minRX = lx[0];
      double maxRX = lx[lx.Count - 1];
      double minRY = ly[0];
      double maxRY = ly[ly.Count - 1];

      if (minRX > maxRX)
      {
        double h = minRX;
        minRX = maxRX;
        maxRX = h;
      }
      if (minRY > maxRY)
      {
        double h = minRY;
        minRY = maxRY;
        maxRY = h;
      }

      var interpol = new BivariateLinearSpline(lx, ly, vcolumns);

      Color colorInvalid = _colorProvider.GetColor(double.NaN);

      int addr = 0;
      for (int ny = 0; ny < dimY; ny++)
      {
        double py = (ny + 0.5) * heightByDimY;

        for (int nx = 0; nx < dimX; nx++, addr += 4)
        {
          double px = (nx + 0.5) * widthByDimX;

          if (false == gl.CoordinateSystem.LayerToLogicalCoordinates(px, py, out rel))
          {
            _cachedImage.SetPixel(nx, ny, colorInvalid);
          }
          else // conversion to relative coordinates was possible
          {
            double rx = rel.RX;
            double ry = rel.RY;

            if (rx < minRX || rx > maxRX || ry < minRY || ry > maxRY)
            {
              //_cachedImage.SetPixel(nx, ny, _colorInvalid);
              imageBytes[addr + 0] = colorInvalid.B;
              imageBytes[addr + 1] = colorInvalid.G;
              imageBytes[addr + 2] = colorInvalid.R;
              imageBytes[addr + 3] = colorInvalid.A;
            }
            else
            {
              double val = interpol.Interpolate(rx, ry);
              if (double.IsNaN(val))
              {
                //_cachedImage.SetPixel(nx, ny, _colorInvalid);
                imageBytes[addr + 0] = colorInvalid.B;
                imageBytes[addr + 1] = colorInvalid.G;
                imageBytes[addr + 2] = colorInvalid.R;
                imageBytes[addr + 3] = colorInvalid.A;
              }
              else
              {
                //_cachedImage.SetPixel(nx, ny, GetColor(val));
                Color c = GetColor(val);
                imageBytes[addr + 0] = c.B;
                imageBytes[addr + 1] = c.G;
                imageBytes[addr + 2] = c.R;
                imageBytes[addr + 3] = c.A;
              }
            }
          }
        }
      }

      // Lock the bitmap's bits.
      var rect = new Rectangle(0, 0, _cachedImage.Width, _cachedImage.Height);
      System.Drawing.Imaging.BitmapData bmpData =
          _cachedImage.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly,
          _cachedImage.PixelFormat);

      // Get the address of the first line.
      IntPtr ptr = bmpData.Scan0;

      // Copy the RGB values back to the bitmap
      System.Runtime.InteropServices.Marshal.Copy(imageBytes, 0, ptr, imageBytes.Length);

      _cachedImage.UnlockBits(bmpData);
    }

    [MemberNotNull(nameof(_cachedImage))]
    private void BuildImageV1(IROMatrix<double> matrix)
    {
      _imageType = CachedImageType.LinearEquidistant;
      // look if the image has the right dimensions
      if (_cachedImage is not null && (_cachedImage.Width != matrix.ColumnCount || _cachedImage.Height != matrix.RowCount))
      {
        _cachedImage.Dispose();
        _cachedImage = null;
      }

      GetPixelwiseImage(matrix, ref _cachedImage);
    }

    /// <summary>
    /// Gets a pixelwise image of the matrix data, i.e. each element of the matrix is converted to a pixel of the resulting bitmap.
    /// </summary>
    /// <param name="matrix">The matrix data.</param>
    /// <param name="image">Bitmap to fill with the pixelwise image. If null, a new image is created.</param>
    /// <exception cref="ArgumentException">An exception will be thrown if the provided image is smaller than the required dimensions.</exception>
    public void GetPixelwiseImage(IROMatrix<double> matrix, [AllowNull] ref System.Drawing.Bitmap image)
    {
      // look if the image has the right dimensions

      int numberOfRows = matrix.RowCount;
      int numberOfColumns = matrix.ColumnCount;

      if (image is not null && (image.Width < matrix.ColumnCount || image.Height < matrix.RowCount))
        throw new ArgumentException("The provided image is smaller than required");

      if (image is null)
      {
        // please notice: the horizontal direction of the image is related to the row index!!! (this will turn the image in relation to the table)
        // and the vertical direction of the image is related to the column index
        image = new System.Drawing.Bitmap(numberOfRows, numberOfColumns, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
      }

      // now we can fill the image with our data
      for (int i = 0; i < numberOfColumns; i++)
      {
        for (int j = 0; j < numberOfRows; j++)
        {
          image.SetPixel(j, numberOfColumns - i - 1, GetColor(matrix[j, i]));
        } // for all pixel of a column
      } // for all columns
    }

    #region Changed event handling

    protected override bool HandleHighPriorityChildChangeCases(object? sender, ref EventArgs e)
    {
      if (object.ReferenceEquals(sender, _scale))
      {
        _imageType = CachedImageType.None;
      }
      else if (object.ReferenceEquals(sender, _colorProvider))
      {
        _imageType = CachedImageType.None;
      }

      return base.HandleHighPriorityChildChangeCases(sender, ref e);
    }

    #endregion Changed event handling
  } // end of class DensityImagePlotStyle
}

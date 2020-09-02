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
using System.Drawing;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Data;
using Altaxo.Geometry;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;

namespace Altaxo.Graph.Graph3D.Plot.Styles
{
  using System.Diagnostics.CodeAnalysis;
  using Altaxo.Graph;
  using Altaxo.Graph.Gdi.Plot;
  using Altaxo.Graph.Gdi.Plot.ColorProvider;
  using Altaxo.Graph.Scales.Ticks;
  using Drawing.D3D;
  using Drawing.D3D.Material;
  using Graph.Plot.Data;
  using GraphicsContext;

  /// <summary>
  /// This plot style is responsible for showing density plots as pixel image. Because of the limitation to a pixel image, each pixel is correlated
  /// with a single data point in the table. Splining of data is not implemented here. Beause of this limitation, the image can only be shown
  /// on linear axes.
  /// </summary>
  [Serializable]
  public class DataMeshPlotStyle
      :
      Main.SuspendableDocumentNodeWithEventArgs,
      Main.ICopyFrom
  {
    [Serializable]
    private enum CachedImageType { None, LinearEquidistant, Other };

    /// <summary>If true, the image is clipped to the layer boundaries.</summary>
    private bool _clipToLayer = true;

    /// <summary>Calculates the color from the relative value.</summary>
    private IColorProvider _colorProvider;

    /// <summary>
    /// Converts the numerical height values into logical values used for color calculation.
    /// This member can be null. In this case the z-scale of the parent coordinate system is used for coloring.
    /// </summary>
    private NumericalScale? _colorScale;

    /// <summary>
    /// The material used to show the surface. Here only the specular properties of the material are used, because the color is provided by the color provider
    /// </summary>
    private IMaterial _material;

    #region Serialization

    /// <summary>
    /// 2015-11-14 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DataMeshPlotStyle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DataMeshPlotStyle)obj;

        info.AddValue("ClipToLayer", s._clipToLayer);
        info.AddValue("Colorization", s._colorProvider);
        info.AddValueOrNull("ColorScale", s._colorScale);
        info.AddValue("Material", s._material);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (DataMeshPlotStyle?)o ?? new DataMeshPlotStyle();

        s._clipToLayer = info.GetBoolean("ClipToLayer");
        s.ColorProvider = (IColorProvider)info.GetValue("Colorization", s);
        s.ColorScale = info.GetValueOrNull<NumericalScale>("ColorScale", s);
        s._material = (IMaterial)info.GetValue("Material", s);

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initialized the member variables to default values.
    /// </summary>
    protected void InitializeMembers()
    {
    }

    /// <summary>
    /// Initializes the style to default values.
    /// </summary>
    public DataMeshPlotStyle()
    {
      ChildSetMemberAlt(ref _colorProvider, new ColorProviderBGRY());
      _material = new MaterialWithoutColorOrTexture();

      InitializeMembers();
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">The style to copy from.</param>
    public DataMeshPlotStyle(DataMeshPlotStyle from)
    {
      InitializeMembers();
      CopyFrom(from);
    }

    [MemberNotNull(nameof(_colorProvider), nameof(_material))]
    protected void CopyFrom(DataMeshPlotStyle from)
    {
      _clipToLayer = from._clipToLayer;
      _colorProvider = from._colorProvider;
      ChildCloneToMember(ref _colorScale, from._colorScale);
      _material = from._material; // Material is immutable
    }

    public bool CopyFrom(object obj)
    {
      if (object.ReferenceEquals(this, obj))
        return true;

      if (obj is DataMeshPlotStyle from)
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
      if (_colorScale is not null)
        yield return new Main.DocumentNodeAndName(_colorScale, "ColorScale");
    }

    public object Clone()
    {
      return new DataMeshPlotStyle(this);
    }

    /// <summary>
    /// Converts the numerical height values into logical values used for color calculation.
    /// This member can be null, in this case the z-scale of the parent coordinate system is used for coloring.
    /// </summary>
    public NumericalScale? ColorScale
    {
      get
      {
        return _colorScale;
      }
      set
      {
        if (ChildSetMember(ref _colorScale, value))
        {
          if (_colorScale is not null && !(_colorScale.TickSpacing is NoTickSpacing))
            _colorScale.TickSpacing = new NoTickSpacing(); // strip the old tickspacing, use NoTickspacing, since Ticks are not needed in the density image plot style

          if (_colorScale is not null)
            EhChildChanged(_colorScale, EventArgs.Empty);
          else
            EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public IColorProvider ColorProvider
    {
      get { return _colorProvider; }
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

    public IMaterial Material
    {
      get { return _material; }
      set
      {
        var oldValue = _material;
        _material = value;
        if (!object.ReferenceEquals(oldValue, value))
          EhSelfChanged();
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
      EhSelfChanged(EventArgs.Empty);
    }

    /// <summary>
    /// This routine ensures that the plot item updates all its cached data and send the appropriate
    /// events if something has changed. Called before the layer paint routine paints the axes because
    /// it must be ensured that the axes are scaled correctly before the plots are painted.
    /// </summary>
    /// <param name="layer">The plot layer.</param>
    /// <param name="plotData">The plot data.</param>
    public void PrepareScales(IPlotArea layer, XYZMeshedColumnPlotData plotData)
    {
      if (_colorScale is not null)
      {
        // in case we use our own scale for coloring, we need to calculate the data bounds
        NumericalBoundaries pb = _colorScale.DataBounds;
        plotData.SetVBoundsFromTemplate(pb); // ensure that the right v-boundary type is set
        using (var suspendToken = pb.SuspendGetToken())
        {
          pb.Reset();
          plotData.MergeVBoundsInto(pb);
          suspendToken.Resume();
        }
      }
    }

    /// <summary>
    /// Paint the density image in the layer.
    /// </summary>
    /// <param name="gfrx">The graphics context painting in.</param>
    /// <param name="gl">The layer painting in.</param>
    /// <param name="plotObject">The data to plot.</param>
    public void Paint(IGraphicsContext3D gfrx, IPlotArea gl, object plotObject) // plots the curve with the choosen style
    {
      if (!(plotObject is XYZMeshedColumnPlotData))
        return; // we cannot plot any other than a TwoDimMeshDataAssociation now

      var myPlotAssociation = (XYZMeshedColumnPlotData)plotObject;
      myPlotAssociation.DataTableMatrix.GetWrappers(
          gl.XAxis.PhysicalVariantToNormal, // transformation function for row header values
          Altaxo.Calc.RMath.IsFinite,       // selection functiton for row header values
          gl.YAxis.PhysicalVariantToNormal, // transformation function for column header values
          Altaxo.Calc.RMath.IsFinite,       // selection functiton for column header values
          out var matrix,
          out var logicalRowHeaderValues,
          out var logicalColumnHeaderValues
          );

      int cols = matrix.ColumnCount;
      int rows = matrix.RowCount;

      if (cols <= 1 || rows <= 1)
        return; // we cannot plot anything  if one length is zero or one

      BuildImage(gfrx, gl, myPlotAssociation, matrix, logicalRowHeaderValues, logicalColumnHeaderValues);
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

    private void BuildImage(IGraphicsContext3D gfrx, IPlotArea gl, XYZMeshedColumnPlotData myPlotAssociation, IROMatrix<double> matrix, IReadOnlyList<double> logicalRowHeaderValues, IReadOnlyList<double> logicalColumnHeaderValues)
    {
      BuildImageWithUColor(gfrx, gl, logicalRowHeaderValues, logicalColumnHeaderValues, matrix); // affine, linear scales, and equidistant points
    }

    private void BuildImageWithUColor(
        IGraphicsContext3D g,
        IPlotArea gl,
        IReadOnlyList<double> lx,
        IReadOnlyList<double> ly,
        IROMatrix<double> matrix)
    {
      IPositionNormalUIndexedTriangleBuffer buffers;

      if (gl.ClipDataToFrame == LayerDataClipping.None && !_clipToLayer)
      {
        buffers = g.GetPositionNormalUIndexedTriangleBuffer(_material, null, _colorProvider);
      }
      else
      {
        var clipPlanes = new PlaneD3D[6];
        clipPlanes[0] = new PlaneD3D(1, 0, 0, 0);
        clipPlanes[1] = new PlaneD3D(-1, 0, 0, -gl.Size.X);
        clipPlanes[2] = new PlaneD3D(0, 1, 0, 0);
        clipPlanes[3] = new PlaneD3D(0, -1, 0, -gl.Size.Y);
        clipPlanes[4] = new PlaneD3D(0, 0, 1, 0);
        clipPlanes[5] = new PlaneD3D(0, 0, -1, -gl.Size.Z);

        buffers = g.GetPositionNormalUIndexedTriangleBuffer(_material, clipPlanes, _colorProvider);
      }

      var buf = buffers;
      var offs = buf.VertexCount;

      int lxl = lx.Count;
      int lyl = ly.Count;
      int lxlm1 = lx.Count - 1;
      int lylm1 = ly.Count - 1;

      var vertexPoints = new PointD3D[lxl, lyl];
      var isValid = new bool[lxl, lyl]; // array which stores for every point[i, j], if it is valid, to speed up calculations

      var zScale = gl.ZAxis;
      for (int i = 0; i < lx.Count; ++i)
      {
        for (int j = 0; j < ly.Count; ++j)
        {
          double lz = zScale.PhysicalVariantToNormal(matrix[i, j]);
          gl.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(lx[i], ly[j], lz), out var pt);

          isValid[i, j] = !pt.IsNaN;
          vertexPoints[i, j] = pt;
        }
      }

      // ------------------------------------------------------------------
      // ------------------ Calculation of normals ------------------------
      // (this can be laborious, if both neighboring points are invalid)
      // ------------------------------------------------------------------
      for (int i = 0; i < lxl; ++i)
      {
        for (int j = 0; j < lyl; ++j)
        {
          if (isValid[i, j])
          {
            var pm = vertexPoints[i, j];

            // Strategy here: we calculate the vectors (right-left) and (upper-lower) and calculate the cross product. This is our normal vector.

            // right - left
            var vec1 = vertexPoints[(i < lxlm1 && isValid[i + 1, j]) ? i + 1 : i, j] - // right side
                        vertexPoints[(i > 0 && isValid[i - 1, j]) ? i - 1 : i, j]; // left side

            if (vec1.IsEmpty) // if vector 1 is empty (because both the right _and_ the left neighbor points are invalid), then we have to try the diagonals
            {
              bool rightup = (i < lxlm1 && j < lylm1 && isValid[i + 1, j + 1]); // right-up neighbor valid?
              bool leftlow = (i > 0 && j > 0 && isValid[i - 1, j - 1]); // left-lower neighbor valid?
              var vec1a = vertexPoints[rightup ? i + 1 : i, rightup ? j + 1 : j] - // right / upper side
                        vertexPoints[leftlow ? i - 1 : i, leftlow ? j - 1 : j]; // left / lower side

              bool rightlow = (i < lxlm1 && j > 0 && isValid[i + 1, j - 1]); // right-lower neighbor valid?
              bool leftup = (i > 0 && j < lylm1 && isValid[i - 1, j + 1]); // left-upper neighbor valid?
              var vec1b = vertexPoints[rightlow ? i + 1 : i, rightlow ? j - 1 : j] - // right / lower side
                          vertexPoints[leftup ? i - 1 : i, leftup ? j + 1 : j]; // left / upper side

              vec1 = vec1a + vec1b; // if one of these two vectors is empty, it doesn't matter for the addition
            }

            // upper - lower
            var vec2 = vertexPoints[i, (j < lylm1 && isValid[i, j + 1]) ? j + 1 : j] - // upper side
                        vertexPoints[i, (j > 0 && isValid[i, j - 1]) ? j - 1 : j]; // lower side

            if (vec2.IsEmpty) // if vector 2 is empty (because both the upper _and_ the lower neighbor points are invalid, then we have to try the diagonals
            {
              bool rightup = (i < lxlm1 && j < lylm1 && isValid[i + 1, j + 1]); // right-up neighbor valid?
              bool leftlow = (i > 0 && j > 0 && isValid[i - 1, j - 1]); // left-lower neighbor valid?
              var vec2a = vertexPoints[rightup ? i + 1 : i, rightup ? j + 1 : j] - // upper side / right
                        vertexPoints[leftlow ? i - 1 : i, leftlow ? j - 1 : j]; // lower side / left

              bool leftup = (i > 0 && j < lylm1 && isValid[i - 1, j + 1]); // left-upper neighbor valid?
              bool rightlow = (i < lxlm1 && j > 0 && isValid[i + 1, j - 1]); // right-lower neighbor valid?
              var vec2b = vertexPoints[leftup ? i - 1 : i, leftup ? j + 1 : j] - // upper side / left
                        vertexPoints[rightlow ? i + 1 : i, rightlow ? j - 1 : j]; // lower side / right

              vec2 = vec2a + vec2b; // if one of these two vectors is empty, it doesn't matter for the addition
            }

            var normal = VectorD3D.CrossProduct(vec1, vec2).Normalized;
            double lz = _colorScale is not null ? _colorScale.PhysicalVariantToNormal(matrix[i, j]) : zScale.PhysicalVariantToNormal(matrix[i, j]);
            buf.AddTriangleVertex(pm.X, pm.Y, pm.Z, normal.X, normal.Y, normal.Z, lz);
            buf.AddTriangleVertex(pm.X, pm.Y, pm.Z, -normal.X, -normal.Y, -normal.Z, lz);
          }
          else // if this point is not valid, we still add triangle vertices to keep the order of points
          {
            buf.AddTriangleVertex(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
            buf.AddTriangleVertex(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
          }
        }
      }

      // now add the triangle indices
      // we don't make the effort to sort out the invalid point, because they are suppressed anyways
      for (int i = 0; i < lxlm1; ++i)
      {
        for (int j = 0; j < lylm1; ++j)
        {
          // upper side
          buf.AddTriangleIndices(offs + 0, offs + 2 * lyl, offs + 2);
          buf.AddTriangleIndices(offs + 2, offs + 2 * lyl, offs + 2 * lyl + 2);

          // from below
          buf.AddTriangleIndices(offs + 0 + 1, offs + 2 + 1, offs + 2 * lyl + 1);
          buf.AddTriangleIndices(offs + 2 + 1, offs + 2 * lyl + 2 + 1, offs + 2 * lyl + 1);

          offs += 2;
        }
        offs += 2; // one extra increment because inner loop ends at one less than array size
      }
    }

    #region Changed event handling

    protected override bool HandleHighPriorityChildChangeCases(object? sender, ref EventArgs e)
    {
      if (object.ReferenceEquals(sender, _colorScale))
      {
      }
      else if (object.ReferenceEquals(sender, _colorProvider))
      {
      }

      return base.HandleHighPriorityChildChangeCases(sender, ref e);
    }

    #endregion Changed event handling
  } // end of class DensityImagePlotStyle
}

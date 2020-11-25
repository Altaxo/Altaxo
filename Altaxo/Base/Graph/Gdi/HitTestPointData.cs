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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Geometry;

namespace Altaxo.Graph.Gdi
{
  /// <summary>
  /// Holds information about a hitted point on the screen.
  /// </summary>
  public class HitTestPointData
  {
    /// <summary>Hitted point in page coordinates.</summary>
    private PointD2D _hittedPointInPageCoord;

    /// <summary>The ratio between displayed sizes and page scale sizes, i.e. the zoom factor on the display.</summary>
    private double _pageScale;

    /// <summary>Transformation of this item that transform world coordinates to page coordinates.</summary>
    private MatrixD2D _transformation;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="hitPointPageCoord">Page coordinates (unit: points).</param>
    /// <param name="pageScale">Current zoom factor, i.e. ration between displayed size on the screen and given size.</param>
    public HitTestPointData(PointD2D hitPointPageCoord, double pageScale)
    {
      _hittedPointInPageCoord = hitPointPageCoord;
      _pageScale = pageScale;
      _transformation = new MatrixD2D();
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">Another HitTestData object to copy from.</param>
    public HitTestPointData(HitTestPointData from)
    {
      _hittedPointInPageCoord = from._hittedPointInPageCoord;
      _pageScale = from._pageScale;
      _transformation = new MatrixD2D(from._transformation);
    }

    /// <summary>
    /// Returns the hitted point in page coordinates (unit: Points).
    /// </summary>
    public PointD2D HittedPointInPageCoord
    {
      get { return _hittedPointInPageCoord; }
    }

    /// <summary>
    /// The ratio between displayed sizes and page scale sizes, i.e. the zoom factor on the display.
    /// </summary>
    private double PageScale
    {
      get { return _pageScale; }
    }

    /// <summary>
    /// Transformation of this item that transform world coordinates to page coordinates.
    /// </summary>
    public MatrixD2D Transformation
    {
      get
      {
        return _transformation;
      }
    }

    /// <summary>
    /// Gets the transformation of this item plus an additional transformation. Both together transform world coordinates to page coordinates.
    /// </summary>
    /// <param name="additionalTransformation">The additional transformation matrix.</param>
    /// <returns></returns>
    public MatrixD2D GetTransformation(MatrixD2D additionalTransformation)
    {
      var result = new MatrixD2D(_transformation);
      result.PrependTransform(additionalTransformation);
      return result;
    }

    public HitTestPointData NewFromTranslationRotationScaleShear(double x, double y, double rotation, double scaleX, double scaleY, double shear)
    {
      var result = new HitTestPointData(this);
      result.Transformation.TranslatePrepend(x, y);
      if (0 != rotation)
        result.Transformation.RotatePrepend(rotation);
      if (1 != scaleX || 1 != scaleY)
        result.Transformation.ScalePrepend(scaleX, scaleY);
      if (0 != shear)
        result.Transformation.ShearPrepend(shear, 0);

      return result;
    }

    public HitTestPointData NewFromAdditionalTransformation(MatrixD2D additionalTransformation)
    {
      var result = new HitTestPointData(this);
      result.Transformation.PrependTransform(additionalTransformation);
      return result;
    }

    /// <summary>
    /// Returns the hitted point in world coordinated by applying the inverse current coordinate transformation.
    /// </summary>
    /// <returns>Hitted point in world coordinates.</returns>
    public PointD2D GetHittedPointInWorldCoord()
    {
      return Transformation.InverseTransformPoint(HittedPointInPageCoord);
    }

    /// <summary>
    /// Returns the hitted point in world coordinated by applying the inverse current coordinate transformation and then the provided inverse coordinate transformation.
    /// </summary>
    /// <returns>Hitted point in world coordinates.</returns>
    public PointD2D GetHittedPointInWorldCoord(MatrixD2D additionalTransform)
    {
      var result = GetHittedPointInWorldCoord();
      result = additionalTransform.InverseTransformPoint(result);
      return result;
    }
  }
}

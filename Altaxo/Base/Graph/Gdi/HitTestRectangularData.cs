using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Drawing2D;


namespace Altaxo.Graph.Gdi
{
  /// <summary>
  /// Holds information about a hitted point on the screen.
  /// </summary>
  public class HitTestRectangularData
  {
    /// <summary>Hitted area in page coordinates.</summary>
    RectangleD _hittedAreaInPageCoord;

    /// <summary>The ratio between displayed sizes and page scale sizes, i.e. the zoom factor on the display.</summary>
    double _pageScale;

    /// <summary>Transformation of this item that transform world coordinates to page coordinates.</summary>
    TransformationMatrix2D _transformation;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="hitAreaPageCoord">Page coordinates (unit: points).</param>
    /// <param name="pageScale">Current zoom factor, i.e. ration between displayed size on the screen and given size.</param>
    public HitTestRectangularData(RectangleD hitAreaPageCoord, double pageScale)
    {
      _hittedAreaInPageCoord = hitAreaPageCoord;
      _pageScale = pageScale;
      _transformation = new TransformationMatrix2D();
    }


    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">Another HitTestData object to copy from.</param>
    public HitTestRectangularData(HitTestRectangularData from)
    {
      _hittedAreaInPageCoord = from._hittedAreaInPageCoord;
      this._pageScale = from._pageScale;
      this._transformation = new TransformationMatrix2D(from._transformation);
    }

    /// <summary>
    /// Returns the hitted point in page coordinates (unit: Points).
    /// </summary>
    public RectangleD HittedAreaInPageCoord
    {
      get { return _hittedAreaInPageCoord; }
    }

    /// <summary>
    /// The ratio between displayed sizes and page scale sizes, i.e. the zoom factor on the display.
    /// </summary>
    double PageScale
    {
      get { return _pageScale; }
    }


    /// <summary>
    /// Transformation of this item that transform world coordinates to page coordinates.
    /// </summary>
    public TransformationMatrix2D Transformation
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
    public TransformationMatrix2D GetTransformation(TransformationMatrix2D additionalTransformation)
    {
      var result = new TransformationMatrix2D(_transformation);
      result.PrependTransform(additionalTransformation);
      return result;
    }




    public HitTestRectangularData NewFromTranslationRotationScaleShear(double x, double y, double rotation, double scaleX, double scaleY, double shear)
    {
      var result = new HitTestRectangularData(this);
      result.Transformation.TranslatePrepend(x, y);
      if (0 != rotation)
        result.Transformation.RotatePrepend(rotation);
      if (1 != scaleX || 1 != scaleY)
        result.Transformation.ScalePrepend(scaleX, scaleY);
      if (0 != shear)
        result.Transformation.ShearPrepend(shear, 0);

      return result;
    }



    public HitTestRectangularData NewFromAdditionalTransformation(TransformationMatrix2D additionalTransformation)
    {
      var result = new HitTestRectangularData(this);
      result.Transformation.PrependTransform(additionalTransformation);
      return result;
    }




    /// <summary>
    /// Returns the hitted area in world coordinated by applying the inverse current coordinate transformation.
    /// </summary>
    /// <returns>Hitted point in world coordinates.</returns>
    public TransformationMatrix2D GetHittedAreaInWorldCoord()
    {
      var pt0 = _transformation.InverseTransformPoint(_hittedAreaInPageCoord.Location);
      var pt1 = _transformation.InverseTransformVector(new PointD2D(0,_hittedAreaInPageCoord.Height));
      var pt2 = _transformation.InverseTransformPoint(new PointD2D(_hittedAreaInPageCoord.Width,0));

      var result = new TransformationMatrix2D(pt1.X, pt1.Y, pt2.X, pt2.Y, pt0.X, pt0.Y);
      return result;
    }

    /// <summary>
    /// Returns the hitted area in world coordinated by applying the inverse current coordinate transformation and then the provided inverse coordinate transformation.
    /// </summary>
    /// <returns>Hitted point in world coordinates.</returns>
    public TransformationMatrix2D GetHittedAreaInWorldCoord(TransformationMatrix2D additionalTransform)
    {
      var pt0 = _transformation.InverseTransformPoint(_hittedAreaInPageCoord.Location);
      var pt1 = _transformation.InverseTransformVector(new PointD2D(0,_hittedAreaInPageCoord.Height));
      var pt2 = _transformation.InverseTransformPoint(new PointD2D(_hittedAreaInPageCoord.Width,0));

      pt0 = additionalTransform.InverseTransformPoint(pt0);
      pt1 = additionalTransform.InverseTransformVector(pt1);
      pt2 = additionalTransform.InverseTransformVector(pt2);

      var result = new TransformationMatrix2D(pt1.X, pt1.Y, pt2.X, pt2.Y, pt0.X, pt0.Y);
      return result;
    }



    public bool IsCovering(PointD2D pt)
    {
      pt = _transformation.TransformPoint(pt);
      return _hittedAreaInPageCoord.Contains(pt);
    }

    public bool IsCovering(PointD2D pt, TransformationMatrix2D additionalTransform)
    {
      pt = _transformation.TransformPoint(additionalTransform.TransformPoint(pt));
      return _hittedAreaInPageCoord.Contains(pt);
    }

    public bool IsCovering(RectangleD rect)
    {
      PointD2D pt;
      pt = _transformation.TransformPoint(new PointD2D(rect.X,rect.Y));
      if (!_hittedAreaInPageCoord.Contains(pt))
        return false;

      pt = _transformation.TransformPoint(new PointD2D(rect.Right, rect.Y));
      if (!_hittedAreaInPageCoord.Contains(pt))
        return false;

      pt = _transformation.TransformPoint(new PointD2D(rect.X, rect.Bottom));
      if (!_hittedAreaInPageCoord.Contains(pt))
        return false;

      pt = _transformation.TransformPoint(new PointD2D(rect.Right, rect.Bottom));
      if (!_hittedAreaInPageCoord.Contains(pt))
        return false;


      return true;
    }


    public bool IsCovering(RectangleD rect, TransformationMatrix2D additionalTransform)
    {
      PointD2D pt;
      pt = _transformation.TransformPoint(additionalTransform.TransformPoint(new PointD2D(rect.X, rect.Y)));
      if (!_hittedAreaInPageCoord.Contains(pt))
        return false;

      pt = _transformation.TransformPoint(additionalTransform.TransformPoint(new PointD2D(rect.Right, rect.Y)));
      if (!_hittedAreaInPageCoord.Contains(pt))
        return false;

      pt = _transformation.TransformPoint(additionalTransform.TransformPoint(new PointD2D(rect.X, rect.Bottom)));
      if (!_hittedAreaInPageCoord.Contains(pt))
        return false;

      pt = _transformation.TransformPoint(additionalTransform.TransformPoint(new PointD2D(rect.Right, rect.Bottom)));
      if (!_hittedAreaInPageCoord.Contains(pt))
        return false;


      return true;
    }

  }
}

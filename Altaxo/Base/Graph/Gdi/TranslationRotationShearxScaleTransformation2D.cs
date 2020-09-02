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
  /// Class that holds the location, rotation, shear and scale of an graphical item.
  /// </summary>
  [Serializable]
  public class LocationRotationShearxScaleTransformation2D : Main.SuspendableDocumentLeafNodeWithEventArgs
  {
    private double _x;
    private double _y;
    private double _rotationDeg;
    private double _shearX;
    private double _scaleX = 1;
    private double _scaleY = 1;
    private MatrixD2D _transformation = new MatrixD2D();

    /// <summary>
    /// Translation (or location) of this transformation.
    /// </summary>
    public PointD2D Location
    {
      get
      {
        return new PointD2D(_x, _y);
      }
      set
      {
        var chg = !(_x == value.X && _y == value.Y);
        _x = value.X;
        _y = value.Y;
        if (chg)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// X value of the translation (location).
    /// </summary>
    public double X
    {
      get
      {
        return _x;
      }
      set
      {
        var chg = !(_x == value);
        _x = value;
        if (chg)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Y value of the translation (location).
    /// </summary>
    public double Y
    {
      get
      {
        return _y;
      }
      set
      {
        var chg = !(_y == value);
        _y = value;
        if (chg)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Rotation value in degree counterclockwise.
    /// </summary>
    public double RotationDeg
    {
      get
      {
        return _rotationDeg;
      }
      set
      {
        var chg = !(_rotationDeg == value);
        _rotationDeg = value;
        if (chg)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Rotation value in rad counterclockwise.
    /// </summary>
    public double RotationRad
    {
      get
      {
        return _rotationDeg * (Math.PI / 180);
      }
      set
      {
        value *= (180 / Math.PI);
        var chg = !(_rotationDeg == value);
        _rotationDeg = value;
        if (chg)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Shear value. This is x shear, meaning that the x value is shifted along the y axis by y*shear.
    /// </summary>
    public double ShearX
    {
      get
      {
        return _shearX;
      }
      set
      {
        var chg = !(_shearX == value);
        _shearX = value;
        if (chg)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Scale in x and y direction. Both values are normally 1.
    /// </summary>
    public PointD2D Scale
    {
      get
      {
        return new PointD2D(_scaleX, _scaleY);
      }
      set
      {
        var chg = !(_scaleX == value.X && _scaleY == value.Y);
        _scaleX = value.X;
        _scaleY = value.Y;
        if (chg)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Scale in x direction. The default value is 1.
    /// </summary>
    public double ScaleX
    {
      get
      {
        return _scaleX;
      }
      set
      {
        var chg = !(_scaleX == value);
        _scaleX = value;
        if (chg)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Scale in y direction. The default value is 1.
    /// </summary>
    public double ScaleY
    {
      get
      {
        return _scaleY;
      }
      set
      {
        var chg = !(_scaleY == value);
        _scaleY = value;
        if (chg)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Returns the transformation matrix. For performance reasons, this is the value stored in this instance.
    /// If you intend to change the transformation, consider using <see cref="TransformationClone"/> instead.
    /// </summary>
    public MatrixD2D Transformation
    {
      get
      {
        return _transformation;
      }
    }

    /// <summary>
    /// Returns a clone of the transformation matrix.
    /// </summary>
    public MatrixD2D TransformationClone
    {
      get
      {
        return _transformation.Clone();
      }
    }

    /// <summary>
    /// Sets all transformation values and updates the transformation matrix.
    /// </summary>
    /// <param name="x">Translation in x direction.</param>
    /// <param name="y">Translation in y direction.</param>
    /// <param name="rotation">Roation in degrees counterclockwise.</param>
    /// <param name="shearX">Shear in x-direction.</param>
    /// <param name="scaleX">X scale value.</param>
    /// <param name="scaleY">Y scale value.</param>
    public void SetTranslationRotationShearxScale(double x, double y, double rotation, double shearX, double scaleX, double scaleY)
    {
      _x = x;
      _y = y;
      _rotationDeg = rotation;
      _shearX = shearX;
      _scaleX = scaleX;
      _scaleY = scaleY;

      EhSelfChanged(EventArgs.Empty);
    }

    /// <summary>
    /// Sets all provided transformation values and updates the transformation matrix. You can leave out
    /// multiple parameters by setting them to null.
    /// </summary>
    /// <param name="x">Translation in x direction.</param>
    /// <param name="y">Translation in y direction.</param>
    /// <param name="rotation">Roation in degrees counterclockwise.</param>
    /// <param name="shearX">Shear in x-direction.</param>
    /// <param name="scaleX">X scale value.</param>
    /// <param name="scaleY">Y scale value.</param>
    /// <param name="eventFiring">Designates whether or not the change event should be fired if the value has changed.</param>
    public void SetTranslationRotationShearxScale(double? x, double? y, double? rotation, double? shearX, double? scaleX, double? scaleY, Main.EventFiring eventFiring)
    {
      if (x is not null)
        _x = (double)x;
      if (y is not null)
        _y = (double)y;
      if (rotation is not null)
        _rotationDeg = (double)rotation;
      if (shearX is not null)
        _shearX = (double)shearX;
      if (scaleX is not null)
        _scaleX = (double)scaleX;
      if (scaleY is not null)
        _scaleY = (double)scaleY;

      _transformation.SetTranslationRotationShearxScale(_x, _y, _rotationDeg, _shearX, _scaleX, _scaleY);

      if (eventFiring == Main.EventFiring.Enabled)
        EhSelfChanged(EventArgs.Empty);
    }

    /// <summary>
    /// Sets the value for translation, roation, shear and scale from a transformation matrix.
    /// </summary>
    /// <param name="transformation"></param>
    public void SetFrom(MatrixD2D transformation)
    {
      SetTranslationRotationShearxScale(transformation.X, transformation.Y, transformation.Rotation, transformation.Shear, transformation.ScaleX, transformation.ScaleY);
    }

    public override void EhSelfChanged()
    {
      _transformation.SetTranslationRotationShearxScale(_x, _y, _rotationDeg, _shearX, _scaleX, _scaleY);
      base.EhSelfChanged();
    }
  }
}

#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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
using Altaxo.Geometry;

#nullable enable

namespace Altaxo.Drawing
{
  /// <summary>
  /// Represents an immutable <see cref="BrushX"/> in its environment (rectangle and resolution). This structure is immutable itself.
  /// </summary>
  public struct BrushXEnv : IEquatable<BrushXEnv>
  {
    /// <summary>
    /// Gets the brush.
    /// </summary>
    /// <value>
    /// The brush.
    /// </value>
    public BrushX BrushX { get; }

    /// <summary>
    /// Gets the bounding rectangle of the brush.
    /// </summary>
    public RectangleD2D BrushBoundingRectangle { get; }

    /// <summary>Gets the effective maximum resolution in dots per inch. Important for repeateable texture brushes only.</summary>
    public double EffectiveMaximumResolutionDpi { get; }

    private int _cachedHashCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="BrushXEnv"/> struct.
    /// </summary>
    /// <param name="brushX">The brush.</param>
    /// <param name="boundingRectangle">The bounding rectangle of the brush.</param>
    /// <param name="effectiveMaximumResolutionDpi">The effective maximum resolution in dots per inch. Important for repeateable texture brushes only.</param>
    public BrushXEnv(BrushX brushX, RectangleD2D boundingRectangle, double effectiveMaximumResolutionDpi)
    {
      BrushX = brushX ?? throw new ArgumentNullException(nameof(brushX));

      // avoid keys with different hashes but the same outcome. For example, for a SolidBrush, the boundingRectangle is meaningless, so set it to Empty.
      if (brushX.BrushType == BrushType.SolidBrush)
      {
        boundingRectangle = RectangleD2D.Empty;
      }
      else
      {
        if (boundingRectangle.IsEmpty)
        {
          boundingRectangle = new RectangleD2D(0, 0, 1000, 1000);
        }

        // fix: in order that a gradient is shown, the bounding rectangle's width and height must always be positive (this is not the case for instance for pens)
        if (boundingRectangle.Width < 0)
        {
          boundingRectangle.X += boundingRectangle.Width;
          boundingRectangle.Width = -boundingRectangle.Width;
        }
        else if (boundingRectangle.Width == 0)
        {
          boundingRectangle.Width = 1;
        }

        if (boundingRectangle.Height < 0)
        {
          boundingRectangle.Y += boundingRectangle.Height;
          boundingRectangle.Height = -boundingRectangle.Height;
        }
        else if (boundingRectangle.Height == 0)
        {
          boundingRectangle.Height = 1;
        }
      }

      // avoid keys with different hashes but the same outcome. For example, for SolidBrush and many other brushes, the effectiveMaximumResolution is meaningless, so set it to 96.
      switch (brushX.BrushType)
      {
        case BrushType.HatchBrush:
        case BrushType.SyntheticTextureBrush:
        case BrushType.TextureBrush:
          break;
        default:
          effectiveMaximumResolutionDpi = 96;
          break;
      }

      EffectiveMaximumResolutionDpi = effectiveMaximumResolutionDpi;
      BrushBoundingRectangle = boundingRectangle;

      unchecked
      {
        _cachedHashCode = BrushX.GetHashCode() + 61 * BrushBoundingRectangle.GetHashCode() + 67 * EffectiveMaximumResolutionDpi.GetHashCode();
      }
    }

    public override int GetHashCode()
    {
      return _cachedHashCode;
    }

    public bool Equals(BrushXEnv other)
    {
      return
        EffectiveMaximumResolutionDpi == other.EffectiveMaximumResolutionDpi &&
        BrushBoundingRectangle == other.BrushBoundingRectangle &&
        Equals(BrushX, other.BrushX);
    }

    public override bool Equals(object? obj)
    {
      return obj is BrushXEnv other ? Equals(other) : false;
    }

    public static bool operator ==(BrushXEnv x, BrushXEnv y)
    {
      return x.Equals(y);
    }
    public static bool operator !=(BrushXEnv x, BrushXEnv y)
    {
      return !x.Equals(y);
    }
  }
} // end of namespace

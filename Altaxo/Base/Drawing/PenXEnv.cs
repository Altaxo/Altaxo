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
  /// Represents an immutable <see cref="PenX"/> in its environment (rectangle and resolution). This structure is immutable itself.
  /// </summary>
  public struct PenXEnv : IEquatable<PenXEnv>
  {
    /// <summary>
    /// Gets the <see cref="PenX"/> object.
    /// </summary>
    public PenX PenX { get; }

    private BrushXEnv BrushXEnv;

    private int _cachedHashCode;

    /// <summary>
    /// Gets the bounding rectangle of the pen's brush.
    /// </summary>
    public RectangleD2D BrushBoundingRectangle { get => BrushXEnv.BrushBoundingRectangle; }

    /// <summary>Gets the effective maximum resolution in dots per inch. Important for pens, whose underlying brush has a repeateable texture.</summary>
    public double EffectiveMaximumResolutionDpi { get => BrushXEnv.EffectiveMaximumResolutionDpi; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PenXEnv"/> struct.
    /// </summary>
    /// <param name="penX">The pen.</param>
    /// <param name="boundingRectangle">The bounding rectangle of the pen's brush.</param>
    /// <param name="effectiveMaximumResolutionDpi">The effective maximum resolution in dots per inch. Important for repeateable texture brushes only.</param>
    public PenXEnv(PenX penX, RectangleD2D boundingRectangle, double effectiveMaximumResolutionDpi)
    {
      PenX = penX ?? throw new ArgumentNullException(nameof(penX));

      BrushXEnv = new BrushXEnv(penX.Brush, boundingRectangle, effectiveMaximumResolutionDpi);
      unchecked
      {
        _cachedHashCode = BrushXEnv.GetHashCode() + 113 * PenX.GetHashCode();
      }
    }

    public override int GetHashCode()
    {
      return _cachedHashCode;
    }

    public bool Equals(PenXEnv other)
    {
      return
        BrushXEnv == other.BrushXEnv &&
        PenX == other.PenX;
    }

    public override bool Equals(object? obj)
    {
      return obj is PenXEnv other ? Equals(other) : false;
    }

    public static bool operator ==(PenXEnv x, PenXEnv y)
    {
      return x.Equals(y);
    }
    public static bool operator !=(PenXEnv x, PenXEnv y)
    {
      return !x.Equals(y);
    }
  }
} // end of namespace

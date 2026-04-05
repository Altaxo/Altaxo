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
using System.Drawing.Drawing2D;

namespace Altaxo.Graph.Gdi
{
  #region DashStyleEx

  /// <summary>
  /// Represents either a predefined or custom dash style.
  /// </summary>
  [Serializable]
  public class DashStyleEx : Main.IImmutable, IEquatable<DashStyleEx>
  {
    private static readonly float[] _emptyArrayFloat = new float[0];
    private DashStyle _knownStyle;

    private float[] _customStyle = _emptyArrayFloat;

    /// <summary>
    /// Initializes a new instance of the <see cref="DashStyleEx"/> class from a predefined dash style.
    /// </summary>
    /// <param name="style">The predefined dash style.</param>
    public DashStyleEx(DashStyle style)
    {
      if (style == DashStyle.Custom)
        throw new ArgumentOutOfRangeException("Style must not be a custom style, use the other constructor instead");

      _knownStyle = style;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DashStyleEx"/> class from a custom dash pattern.
    /// </summary>
    /// <param name="customStyle">The custom dash pattern.</param>
    public DashStyleEx(float[] customStyle)
    {
      _customStyle = (float[])customStyle.Clone();
      _knownStyle = DashStyle.Custom;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DashStyleEx"/> class from a custom dash pattern.
    /// </summary>
    /// <param name="customStyle">The custom dash pattern.</param>
    public DashStyleEx(double[] customStyle)
    {
      _customStyle = new float[customStyle.Length];
      for (int i = 0; i < customStyle.Length; ++i)
        _customStyle[i] = (float)customStyle[i];

      _knownStyle = DashStyle.Custom;
    }

    /// <summary>
    /// Gets a value indicating whether this instance represents a predefined dash style.
    /// </summary>
    public bool IsKnownStyle
    {
      get
      {
        return _knownStyle != DashStyle.Custom;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance represents a custom dash pattern.
    /// </summary>
    public bool IsCustomStyle
    {
      get
      {
        return _knownStyle == DashStyle.Custom;
      }
    }

    /// <summary>
    /// Gets the predefined dash style.
    /// </summary>
    public DashStyle KnownStyle
    {
      get
      {
        return _knownStyle;
      }
    }

    /// <summary>
    /// Gets a copy of the custom dash pattern.
    /// </summary>
    public float[] CustomStyle
    {
      get
      {
        return (float[])_customStyle.Clone();
      }
    }





    /// <inheritdoc />
    public bool Equals(DashStyleEx? from)
    {
      if (from is null)
      {
        return false;
      }
      else
      {
        if (this.IsKnownStyle && from.IsKnownStyle && this._knownStyle == from._knownStyle)
          return true;
        else if (this.IsCustomStyle && from.IsCustomStyle && IsEqual(this._customStyle, from._customStyle))
          return true;
        else
          return false;
      }
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
      return Equals(obj as DashStyleEx);
    }

    /// <summary>
    /// Determines whether two dash style instances are equal.
    /// </summary>
    /// <param name="x">The first instance.</param>
    /// <param name="y">The second instance.</param>
    /// <returns><c>true</c> if both instances are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(DashStyleEx x, DashStyleEx y)
    {
      if (x is { } _)
        return x.Equals(x);
      else if (y is { } _)
        return y.Equals(x);
      else
        return true;
    }

    /// <summary>
    /// Determines whether two dash style instances are not equal.
    /// </summary>
    /// <param name="x">The first instance.</param>
    /// <param name="y">The second instance.</param>
    /// <returns><c>true</c> if both instances are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(DashStyleEx x, DashStyleEx y)
    {
      return !(x == y);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
      if (IsCustomStyle && _customStyle is not null)
        return _customStyle.GetHashCode();
      else
        return _knownStyle.GetHashCode();
    }

    /// <inheritdoc />
    public override string ToString()
    {
      if (_knownStyle != DashStyle.Custom)
        return _knownStyle.ToString();
      else
      {
        var stb = new System.Text.StringBuilder();
        foreach (float f in _customStyle)
        {
          stb.Append(Altaxo.Serialization.GUIConversion.ToString(f));
          stb.Append(";");
        }
        return stb.ToString(0, stb.Length - 1);
      }
    }

    /// <summary>
    /// Gets a solid dash style.
    /// </summary>
    public static DashStyleEx Solid
    {
      get { return new DashStyleEx(DashStyle.Solid); }
    }

    /// <summary>
    /// Gets a dotted dash style.
    /// </summary>
    public static DashStyleEx Dot
    {
      get { return new DashStyleEx(DashStyle.Dot); }
    }

    /// <summary>
    /// Gets a dashed style.
    /// </summary>
    public static DashStyleEx Dash
    {
      get { return new DashStyleEx(DashStyle.Dash); }
    }

    /// <summary>
    /// Gets a dash-dot style.
    /// </summary>
    public static DashStyleEx DashDot
    {
      get { return new DashStyleEx(DashStyle.DashDot); }
    }

    /// <summary>
    /// Gets a dash-dot-dot style.
    /// </summary>
    public static DashStyleEx DashDotDot
    {
      get { return new DashStyleEx(DashStyle.DashDotDot); }
    }

    /// <summary>
    /// Gets a long-dash style.
    /// </summary>
    public static DashStyleEx LongDash
    {
      get { return new DashStyleEx(new float[] { 5.0f, 3.0f }); }
    }

    private static bool IsEqual(float[] a, float[] b)
    {
      if (a is null || b is null)
        return false;
      if (a.Length != b.Length)
        return false;
      for (int i = a.Length - 1; i >= 0; i--)
        if (a[i] != b[i])
          return false;

      return true;
    }
  }

  #endregion DashStyleEx
}

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

namespace Altaxo.Geometry
{
  /// <summary>
  /// Represents a margin in 2D space, defined by left, top, right, and bottom values.
  /// </summary>
  [Serializable]
  public struct Margin2D : IEquatable<Margin2D>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Margin2D"/> struct.
    /// </summary>
    /// <param name="left">The left margin.</param>
    /// <param name="top">The top margin.</param>
    /// <param name="right">The right margin.</param>
    /// <param name="bottom">The bottom margin.</param>
    public Margin2D(double left, double top, double right, double bottom)
    {
      Left = left;
      Top = top;
      Right = right;
      Bottom = bottom;
    }

    /// <summary>
    /// Gets or sets the left margin.
    /// </summary>
    public double Left { get; set; }

    /// <summary>
    /// Gets or sets the top margin.
    /// </summary>
    public double Top { get; set; }

    /// <summary>
    /// Gets or sets the right margin.
    /// </summary>
    public double Right { get; set; }

    /// <summary>
    /// Gets or sets the bottom margin.
    /// </summary>
    public double Bottom { get; set; }

    #region Serialization

    /// <summary>
    /// XML serialization surrogate for <see cref="Margin2D"/>.
    /// V1: 2015-11-15 Move to Altaxo.Geometry namespace.
    /// V2: 2023-01-14 Move from assembly AltaxoBase to AltaxoCore
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Margin2D", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Geometry.Margin2D", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Margin2D), 2)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (Margin2D)obj;

        info.AddValue("Left", s.Left);
        info.AddValue("Top", s.Top);
        info.AddValue("Right", s.Right);
        info.AddValue("Bottom", s.Bottom);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = o is not null ? (Margin2D)o : new Margin2D();

        s.Left = info.GetDouble("Left");
        s.Top = info.GetDouble("Top");
        s.Right = info.GetDouble("Right");
        s.Bottom = info.GetDouble("Bottom");

        return s;
      }
    }

    #endregion Serialization

    /// <inheritdoc/>
    public bool Equals(Margin2D other)
    {
      return Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;
    }

    /// <summary>
    /// Determines whether two <see cref="Margin2D"/> instances are equal.
    /// </summary>
    /// <param name="a">The first <see cref="Margin2D"/>.</param>
    /// <param name="b">The second <see cref="Margin2D"/>.</param>
    /// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(Margin2D a, Margin2D b)
    {
      return a.Equals(b);
    }

    /// <summary>
    /// Determines whether two <see cref="Margin2D"/> instances are not equal.
    /// </summary>
    /// <param name="a">The first <see cref="Margin2D"/>.</param>
    /// <param name="b">The second <see cref="Margin2D"/>.</param>
    /// <returns><c>true</c> if the instances are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(Margin2D a, Margin2D b)
    {
      return !(a.Equals(b));
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      if (obj is Margin2D from)
      {
        return Equals(from);
      }
      else
      {
        return false;
      }
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return Left.GetHashCode() + 3 * Right.GetHashCode() + 7 * Top.GetHashCode() + 11 * Bottom.GetHashCode();
    }
  }
}

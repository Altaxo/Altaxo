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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Geometry;

namespace Altaxo.Drawing
{
  /// <summary>
  /// Designates how a texture image should be scaled.
  /// </summary>
  public enum TextureScalingMode
  {
    /// <summary>Size of the texture scales with the original size of the texture. X and Y in <see cref="TextureScaling"/> are scaling factors.</summary>
    Source = 0,

    /// <summary>Size of the texture scales with the destination size of the texture (e.g. the bounding rectangle of a shape to fill). X and Y in <see cref="TextureScaling"/> are scaling factors.</summary>
    Destination = 1,

    /// <summary>Size of the texture is given absolutely in Points (1/72 inch). X and Y in <see cref="TextureScaling"/> are the horizontal and vertical size in Points (1/72 inch).</summary>
    Absolute = 2
  }

  /// <summary>
  /// Designates whether the aspect ratio of an image or a texture should be preserved.
  /// </summary>
  public enum AspectRatioPreservingMode
  {
    /// <summary>Aspect ratio is not preserved.</summary>
    None = 0,

    /// <summary>Aspect ratio is preserved. The horizontal size or scaling is used to calculate the final size.</summary>
    PreserveXPriority = 1,

    /// <summary>Aspect ratio is preserved. The vertical size or scaling is used to calculate the final size.</summary>
    PreserveYPriority = 2
  }

  /// <summary>
  /// Designates how and how much a texture image should be scaled.
  /// </summary>
  public readonly struct TextureScaling : IEquatable<TextureScaling>, Main.IImmutable
  {
    /// <summary>Gets or sets the scaling mode, i.e. how the texture image should be scaled.</summary>
    /// <value>The scaling mode.</value>
    public TextureScalingMode ScalingMode { get; }

    /// <summary>Gets or sets whether the aspect ratio of the texture image should be preserved.</summary>
    /// <value>The aspect ratio preserving mode.</value>
    public AspectRatioPreservingMode SourceAspectRatioPreserving { get; }

    /// <summary>If <see cref="ScalingMode"/> is Absolute, this value is the horizontal size of the texture (repeat length) in Points (1/72 inch).
    /// Otherwise, it is the horizontal scaling factor related either to the source image width or the destination image width.</summary>
    /// <value>The X value.</value>
    public double X { get; }

    /// <summary>If <see cref="ScalingMode"/> is Absolute, this value is the vertical size of the texture (repeat length) in Points (1/72 inch).
    /// Otherwise, it is the vertical scaling factor related either to the source image width or the destination image height.</summary>
    /// <value>The Y value.</value>
    public double Y { get; }

    #region Serialization

    /// <summary>
    /// 2020-03-30 Moved from Altaxo.Graph.TextureScaling to Altaxo.Drawing.TextureScaling.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.TextureScaling", 0)]
    [Serialization.Xml.XmlSerializationSurrogateFor(typeof(TextureScaling), 1)]
    private class XmlSerializationSurrogate0 : Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (TextureScaling)obj;
        info.AddEnum("Mode", s.ScalingMode);
        info.AddEnum("AspectPreserving", s.SourceAspectRatioPreserving);
        info.AddValue("X", s.X);
        info.AddValue("Y", s.Y);
      }

      public object Deserialize(object? o, Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var scalingMode = (TextureScalingMode)info.GetEnum("Mode", typeof(TextureScalingMode));
        var aspectPreserving = (AspectRatioPreservingMode)info.GetEnum("AspectPreserving", typeof(AspectRatioPreservingMode));
        var x = info.GetDouble("X");
        var y = info.GetDouble("Y");

        return new TextureScaling(scalingMode, aspectPreserving, x, y);
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureScaling"/> struct.
    /// </summary>
    /// <param name="mode">The scaling mode.</param>
    /// <param name="aspectPreserving">The aspect preserving mode.</param>
    /// <param name="x">The x parameter. See <see cref="X"/> for explanations.</param>
    /// <param name="y">The y parameter. See <see cref="Y"/> for explanations.</param>
    public TextureScaling(TextureScalingMode mode, AspectRatioPreservingMode aspectPreserving, double x, double y)
    {
      ScalingMode = mode;
      SourceAspectRatioPreserving = aspectPreserving;
      X = x;
      Y = y;
    }

    /// <summary>Returns a new <see cref="TextureScaling"/> instance with the <see cref="ScalingMode"/> value set to the provided value.</summary>
    /// <param name="scalingMode">The scaling mode, i.e. how the texture image should be scaled.</param>
    public TextureScaling WithScalingMode(TextureScalingMode scalingMode)
    {
      if (!(ScalingMode == scalingMode))
      {
        return new TextureScaling(scalingMode, SourceAspectRatioPreserving, X, Y);
      }
      else
      {
        return this;
      }
    }



    /// <summary>Returns a new <see cref="TextureScaling"/> instance with the <see cref="SourceAspectRatioPreserving"/> value set to the provided value.</summary>
    /// <param name="aspectPreserving">The aspect ratio preserving mode.</param>
    public TextureScaling WithSourceAspectRatioPreserving(AspectRatioPreservingMode aspectPreserving)
    {
      if (!(SourceAspectRatioPreserving == aspectPreserving))
      {
        return new TextureScaling(ScalingMode, aspectPreserving, X, Y);
      }
      else
      {
        return this;
      }
    }

    /// <summary>Returns a new <see cref="TextureScaling"/> instance with the <see cref="X"/> value set to the provided value.</summary>
    /// <param name="x">If <see cref="ScalingMode"/> is Absolute, this value is the horizontal size of the texture (repeat length) in Points (1/72 inch).
    /// Otherwise, it is the horizontal scaling factor related either to the source image width or the destination image width.</param>
    public TextureScaling WithX(double x)
    {
      if (!(X == x))
      {
        return new TextureScaling(ScalingMode, SourceAspectRatioPreserving, x, Y);
      }
      else
      {
        return this;
      }
    }


    /// <summary>Returns a new <see cref="TextureScaling"/> instance with the <see cref="Y"/> value set to the provided value.</summary>
    /// <param name="y">If <see cref="ScalingMode"/> is Absolute, this value is the vertical size of the texture (repeat length) in Points (1/72 inch).
    /// Otherwise, it is the vertical scaling factor related either to the source image width or the destination image height.</param>
    public TextureScaling WithY(double y)
    {
      if (!(Y == y))
      {
        return new TextureScaling(ScalingMode, SourceAspectRatioPreserving, X, y);
      }
      else
      {
        return this;
      }
    }

    /// <summary>Gets the default texture scaling.</summary>
    public static TextureScaling Default
    {
      get
      {
        return new TextureScaling(TextureScalingMode.Source, AspectRatioPreservingMode.PreserveXPriority, 1, 1);
      }
    }

    /// <summary>Gets the resulting size of the texture (for one repeat unit).</summary>
    /// <param name="sourceSize">Size of the source image (texture image) in points (1/72 inch).</param>
    /// <param name="destinationSize">Size of the destination rectangle in points (1/72 inch).</param>
    /// <returns>The resulting size of the texture repeat unit in points (1/72 inch).</returns>
    public VectorD2D GetResultingSize(VectorD2D sourceSize, VectorD2D destinationSize)
    {
      switch (ScalingMode)
      {
        default:
        case TextureScalingMode.Source:
          switch (SourceAspectRatioPreserving)
          {
            default:
            case AspectRatioPreservingMode.None:
              return new VectorD2D(X * sourceSize.X, Y * sourceSize.Y);

            case AspectRatioPreservingMode.PreserveXPriority:
              return new VectorD2D(X * sourceSize.X, X * sourceSize.Y);

            case AspectRatioPreservingMode.PreserveYPriority:
              return new VectorD2D(Y * sourceSize.X, Y * sourceSize.Y);
          }
        case TextureScalingMode.Destination:
          switch (SourceAspectRatioPreserving)
          {
            default:
            case AspectRatioPreservingMode.None:
              return new VectorD2D(destinationSize.X * X, destinationSize.Y * Y);

            case AspectRatioPreservingMode.PreserveXPriority: // we use X as scaling factor, and we adjust y so that the source aspect ratio is preserved
              return new VectorD2D(X * destinationSize.X, X * destinationSize.X * (sourceSize.Y / sourceSize.X));

            case AspectRatioPreservingMode.PreserveYPriority:
              return new VectorD2D(Y * destinationSize.Y * (sourceSize.X / sourceSize.Y), Y * destinationSize.Y);
          }
        case TextureScalingMode.Absolute:
          switch (SourceAspectRatioPreserving)
          {
            default:
            case AspectRatioPreservingMode.None:
              return new VectorD2D(X, Y);

            case AspectRatioPreservingMode.PreserveXPriority: // we use X as scaling factor, and we adjust y so that the source aspect ratio is preserved
              return new VectorD2D(X, X * (sourceSize.Y / sourceSize.X));

            case AspectRatioPreservingMode.PreserveYPriority:
              return new VectorD2D(Y * (sourceSize.X / sourceSize.Y), Y);
          }
      }
    }

    /// <summary>Compares this value with another value.</summary>
    /// <param name="other">The other value.</param>
    /// <returns><c>True</c> if this value is equal to the other value.</returns>
    public bool Equals(TextureScaling other)
    {
      return ScalingMode == other.ScalingMode && SourceAspectRatioPreserving == other.SourceAspectRatioPreserving && X == other.X && Y == other.Y;
    }

    public override bool Equals(object? obj)
    {
      return obj is TextureScaling other ? Equals(other) : false;
    }

    public override int GetHashCode()
    {
      return 17 * ScalingMode.GetHashCode() + 31 * SourceAspectRatioPreserving.GetHashCode() + 61 * X.GetHashCode() + 127 * Y.GetHashCode();
    }

    /// <summary>Implements the operator ==.</summary>
    /// <param name="t1">First value.</param>
    /// <param name="t2">Second value.</param>
    /// <returns><c>True</c> if both values are equal.</returns>
    public static bool operator ==(TextureScaling t1, TextureScaling t2)
    {
      return t1.Equals(t2);
    }

    /// <summary>Implements the operator !=.</summary>
    /// <param name="t1">First value.</param>
    /// <param name="t2">Second value.</param>
    /// <returns><c>True</c> if both values are not equal.</returns>
    public static bool operator !=(TextureScaling t1, TextureScaling t2)
    {
      return !t1.Equals(t2);
    }
  }
}

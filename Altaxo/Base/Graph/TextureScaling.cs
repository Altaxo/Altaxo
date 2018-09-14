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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Geometry;

namespace Altaxo.Graph
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
  /// Designates if the aspect ratio of an image or a texture should be preserved.
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
  public struct TextureScaling : System.IEquatable<TextureScaling>
  {
    private TextureScalingMode _scalingMode;
    private AspectRatioPreservingMode _aspectPreserving;
    private double _x;
    private double _y;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(TextureScaling), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (TextureScaling)obj;
        info.AddEnum("Mode", s._scalingMode);
        info.AddEnum("AspectPreserving", s._aspectPreserving);
        info.AddValue("X", s._x);
        info.AddValue("Y", s._y);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = null != o ? (TextureScaling)o : new TextureScaling();
        s._scalingMode = (TextureScalingMode)info.GetEnum("Mode", typeof(TextureScalingMode));
        s._aspectPreserving = (AspectRatioPreservingMode)info.GetEnum("AspectPreserving", typeof(AspectRatioPreservingMode));
        s._x = info.GetDouble("X");
        s._y = info.GetDouble("Y");

        return s;
      }
    }

    #endregion Serialization

    public TextureScaling(TextureScalingMode mode, AspectRatioPreservingMode aspectPreserving, double x, double y)
    {
      _scalingMode = mode;
      _aspectPreserving = aspectPreserving;
      _x = x;
      _y = y;
    }

    /// <summary>Gets or sets the scaling mode, i.e. how the texture image should be scaled.</summary>
    /// <value>The scaling mode.</value>
    public TextureScalingMode ScalingMode
    {
      get { return _scalingMode; }
      set { _scalingMode = value; }
    }

    /// <summary>Gets or sets whether the aspect ratio of the texture image should be preserved.</summary>
    /// <value>The aspect ratio preserving mode.</value>
    public AspectRatioPreservingMode SourceAspectRatioPreserving
    {
      get { return _aspectPreserving; }
      set { _aspectPreserving = value; }
    }

    /// <summary>If <see cref="ScalingMode"/> is Absolute, this value is the horizontal size of the texture (repeat length) in Points (1/72 inch).
    /// Otherwise, it is the horizontal scaling factor related either to the source image width or the destination image width.</summary>
    /// <value>The X value.</value>
    public double X
    {
      get { return _x; }
      set { _x = value; }
    }

    /// <summary>If <see cref="ScalingMode"/> is Absolute, this value is the vertical size of the texture (repeat length) in Points (1/72 inch).
    /// Otherwise, it is the vertical scaling factor related either to the source image width or the destination image height.</summary>
    /// <value>The X value.</value>
    public double Y
    {
      get { return _y; }
      set { _y = value; }
    }

    /// <summary>Gets the default texture scaling.</summary>
    public static TextureScaling Default
    {
      get
      {
        return new TextureScaling() { _scalingMode = TextureScalingMode.Source, _aspectPreserving = AspectRatioPreservingMode.PreserveXPriority, _x = 1, _y = 1 };
      }
    }

    /// <summary>Gets the resulting size of the texture (for one repeat unit).</summary>
    /// <param name="sourceSize">Size of the source image (texture image) in points (1/72 inch).</param>
    /// <param name="destinationSize">Size of the destination rectangle in points (1/72 inch).</param>
    /// <returns>The resulting size of the texture repeat unit in points (1/72 inch).</returns>
    public PointD2D GetResultingSize(PointD2D sourceSize, PointD2D destinationSize)
    {
      switch (_scalingMode)
      {
        default:
        case TextureScalingMode.Source:
          switch (_aspectPreserving)
          {
            default:
            case AspectRatioPreservingMode.None:
              return new PointD2D(_x * sourceSize.X, _y * sourceSize.Y);

            case AspectRatioPreservingMode.PreserveXPriority:
              return new PointD2D(_x * sourceSize.X, _x * sourceSize.Y);

            case AspectRatioPreservingMode.PreserveYPriority:
              return new PointD2D(_y * sourceSize.X, _y * sourceSize.Y);
          }
        case TextureScalingMode.Destination:
          switch (_aspectPreserving)
          {
            default:
            case AspectRatioPreservingMode.None:
              return new PointD2D(destinationSize.X * _x, destinationSize.Y * _y);

            case AspectRatioPreservingMode.PreserveXPriority: // we use _x as scaling factor, and we adjust y so that the source aspect ratio is preserved
              return new PointD2D(_x * destinationSize.X, _x * destinationSize.X * (sourceSize.Y / sourceSize.X));

            case AspectRatioPreservingMode.PreserveYPriority:
              return new PointD2D(_y * destinationSize.Y * (sourceSize.X / sourceSize.Y), _y * destinationSize.Y);
          }
        case TextureScalingMode.Absolute:
          switch (_aspectPreserving)
          {
            default:
            case AspectRatioPreservingMode.None:
              return new PointD2D(_x, _y);

            case AspectRatioPreservingMode.PreserveXPriority: // we use _x as scaling factor, and we adjust y so that the source aspect ratio is preserved
              return new PointD2D(_x, _x * (sourceSize.Y / sourceSize.X));

            case AspectRatioPreservingMode.PreserveYPriority:
              return new PointD2D(_y * (sourceSize.X / sourceSize.Y), _y);
          }
      }
    }

    /// <summary>Compares this value with another value.</summary>
    /// <param name="other">The other value.</param>
    /// <returns><c>True</c> if this value is equal to the other value.</returns>
    public bool Equals(TextureScaling other)
    {
      return _scalingMode == other._scalingMode && _aspectPreserving == other._aspectPreserving && _x == other._x && _y == other._y;
    }

    public override bool Equals(object obj)
    {
      if (obj is TextureScaling)
        return Equals((TextureScaling)obj);
      else
        return false;
    }

    public override int GetHashCode()
    {
      return 17 * _scalingMode.GetHashCode() + 31 * _aspectPreserving.GetHashCode() + 61 * _x.GetHashCode() + 127 * _y.GetHashCode();
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
      return !(t1.Equals(t2));
    }
  }
}

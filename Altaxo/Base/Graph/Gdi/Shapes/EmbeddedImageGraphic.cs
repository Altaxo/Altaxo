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
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Drawing;
using Altaxo.Geometry;

#nullable enable

namespace Altaxo.Graph.Gdi.Shapes
{
  /// <summary>
  /// Represents an embedded image graphic.
  /// </summary>
  [Serializable]
  public class EmbeddedImageGraphic : ImageGraphic
  {
    /// <summary>
    /// The embedded image proxy.
    /// </summary>
    protected ImageProxy? _imageProxy;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.EmbeddedImageGraphic", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(EmbeddedImageGraphic), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (EmbeddedImageGraphic)o;
        info.AddBaseValueEmbedded(s, typeof(EmbeddedImageGraphic).BaseType!);
        info.AddValueOrNull("Image", s._imageProxy);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (EmbeddedImageGraphic?)o ?? new EmbeddedImageGraphic();
        info.GetBaseValueEmbedded(s, typeof(EmbeddedImageGraphic).BaseType!, parent);
        s.Image = info.GetValueOrNull<ImageProxy>("Image", s);
        return s;
      }
    }

    #endregion Serialization

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddedImageGraphic"/> class.
    /// </summary>
    public EmbeddedImageGraphic()
      :
      base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddedImageGraphic"/> class.
    /// </summary>
    /// <param name="graphicPosition">The graphic position.</param>
    /// <param name="startingImage">The starting image.</param>
    public EmbeddedImageGraphic(PointD2D graphicPosition, ImageProxy startingImage)
      :
      this()
    {
      SetPosition(graphicPosition, Main.EventFiring.Suppressed);
      Image = startingImage;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddedImageGraphic"/> class.
    /// </summary>
    /// <param name="posX">The X position.</param>
    /// <param name="posY">The Y position.</param>
    /// <param name="startingImage">The starting image.</param>
    public EmbeddedImageGraphic(double posX, double posY, ImageProxy startingImage)
      :
      this(new PointD2D(posX, posY), startingImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddedImageGraphic"/> class.
    /// </summary>
    /// <param name="graphicPosition">The graphic position.</param>
    /// <param name="graphicSize">The graphic size.</param>
    /// <param name="startingImage">The starting image.</param>
    public EmbeddedImageGraphic(PointD2D graphicPosition, PointD2D graphicSize, ImageProxy startingImage)
      :
      this(graphicPosition, startingImage)
    {
      SetSize(graphicSize.X, graphicSize.Y, Main.EventFiring.Suppressed);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddedImageGraphic"/> class.
    /// </summary>
    /// <param name="posX">The x-position of the graphic.</param>
    /// <param name="posY">The y-position of the graphic.</param>
    /// <param name="graphicSize">The graphic size.</param>
    /// <param name="startingImage">The starting image.</param>
    public EmbeddedImageGraphic(double posX, double posY, PointD2D graphicSize, ImageProxy startingImage)
      :
      this(new PointD2D(posX, posY), graphicSize, startingImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddedImageGraphic"/> class.
    /// </summary>
    /// <param name="posX">The x-position of the graphic.</param>
    /// <param name="posY">The y-position of the graphic.</param>
    /// <param name="width">The width of the graphic.</param>
    /// <param name="height">The height of the graphic.</param>
    /// <param name="startingImage">The starting image.</param>
    public EmbeddedImageGraphic(double posX, double posY, double width, double height, ImageProxy startingImage)
      :
      this(new PointD2D(posX, posY), new PointD2D(width, height), startingImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddedImageGraphic"/> class.
    /// </summary>
    /// <param name="graphicPosition">The graphic position.</param>
    /// <param name="Rotation">The rotation angle.</param>
    /// <param name="startingImage">The starting image.</param>
    public EmbeddedImageGraphic(PointD2D graphicPosition, double Rotation, ImageProxy startingImage)
      :
      this(graphicPosition, startingImage)
    {
      this.Rotation = Rotation;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddedImageGraphic"/> class.
    /// </summary>
    /// <param name="posX">The x-position of the graphic.</param>
    /// <param name="posY">The y-position of the graphic.</param>
    /// <param name="Rotation">The rotation angle.</param>
    /// <param name="startingImage">The starting image.</param>
    public EmbeddedImageGraphic(double posX, double posY, double Rotation, ImageProxy startingImage)
      :
      this(new PointD2D(posX, posY), Rotation, startingImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddedImageGraphic"/> class.
    /// </summary>
    /// <param name="graphicPosition">The graphic position.</param>
    /// <param name="graphicSize">The graphic size.</param>
    /// <param name="Rotation">The rotation angle.</param>
    /// <param name="startingImage">The starting image.</param>
    public EmbeddedImageGraphic(PointD2D graphicPosition, PointD2D graphicSize, double Rotation, ImageProxy startingImage)
      :
      this(graphicPosition, Rotation, startingImage)
    {
      SetSize(graphicSize.X, graphicSize.Y, Main.EventFiring.Suppressed);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddedImageGraphic"/> class.
    /// </summary>
    /// <param name="posX">The x-position of the graphic.</param>
    /// <param name="posY">The y-position of the graphic.</param>
    /// <param name="graphicSize">The graphic size.</param>
    /// <param name="Rotation">The rotation angle.</param>
    /// <param name="startingImage">The starting image.</param>
    public EmbeddedImageGraphic(double posX, double posY, PointD2D graphicSize, double Rotation, ImageProxy startingImage)
      :
      this(new PointD2D(posX, posY), graphicSize, Rotation, startingImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddedImageGraphic"/> class.
    /// </summary>
    /// <param name="posX">The x-position of the graphic.</param>
    /// <param name="posY">The y-position of the graphic.</param>
    /// <param name="width">The width of the graphic.</param>
    /// <param name="height">The height of the graphic.</param>
    /// <param name="Rotation">The rotation angle.</param>
    /// <param name="startingImage">The starting image.</param>
    public EmbeddedImageGraphic(double posX, double posY, double width, double height, double Rotation, ImageProxy startingImage)
      :
      this(new PointD2D(posX, posY), new PointD2D(width, height), Rotation, startingImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddedImageGraphic"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    public EmbeddedImageGraphic(EmbeddedImageGraphic from)
      :
      base(from) // all is done here, since CopyFrom is virtual!
    {
      CopyFrom(from, false);
    }

    /// <summary>
    /// Copies the values from another <see cref="EmbeddedImageGraphic"/> instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    /// <param name="withBaseMembers">If set to <see langword="true"/>, base class members are copied as well.</param>
    protected void CopyFrom(EmbeddedImageGraphic from, bool withBaseMembers)
    {
      if (withBaseMembers)
        base.CopyFrom(from, withBaseMembers);

      Image = from._imageProxy;
    }

    /// <inheritdoc />
    public override bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;
      if (obj is ImageGraphic from)
      {
        using (var suspendToken = SuspendGetToken())
        {
          CopyFrom(from, true);
          EhSelfChanged(EventArgs.Empty);
        }
        return true;
      }
      else
      {
        return base.CopyFrom(obj);
      }
    }

    #endregion Constructors

    /// <inheritdoc />
    public override object Clone()
    {
      return new EmbeddedImageGraphic(this);
    }

    /// <summary>
    /// Gets or sets the image.
    /// </summary>
    /// <value>
    /// The image.
    /// </value>
    public ImageProxy? Image
    {
      get
      {
        return _imageProxy;
      }
      set
      {
        _imageProxy = value;
        var originalItemSize = _imageProxy is null ? new VectorD2D(10, 10) : _imageProxy.Size;
        ((ItemLocationDirectAspectPreserving)_location).OriginalItemSize = (PointD2D)originalItemSize;
      }
    }

    /// <inheritdoc />
    public override PointD2D GetImageSizePt()
    {
      return _imageProxy is null ? new PointD2D(1, 1) : (PointD2D)_imageProxy.Size;
    }

    /// <inheritdoc />
    public override Image? GetImage()
    {
      if (_imageProxy is { } imgproxy)
      {
        var str = imgproxy.GetContentStream();
        return SystemDrawingImageProxyExtensions.GetImage(str, disposeStream: true);
      }
      else
      {
        return null;
      }
    }

    /// <inheritdoc />
    public override void Paint(Graphics g, IPaintContext context)
    {
      GraphicsState gs = g.Save();
      TransformGraphics(g);

      if (GetImage() is { } img)
      {
        var bounds = Bounds;

        g.DrawImage(img, (float)bounds.X, (float)bounds.Y, (float)bounds.Width, (float)bounds.Height);
      }

      g.Restore(gs);
    }
  } // End Class
}

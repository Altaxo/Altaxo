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
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Drawing;
using Altaxo.Geometry;

#nullable enable

namespace Altaxo.Graph.Gdi.Shapes
{
  [Serializable]
  public class EmbeddedImageGraphic : ImageGraphic
  {
    protected ImageProxy? _imageProxy;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.EmbeddedImageGraphic", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(EmbeddedImageGraphic), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (EmbeddedImageGraphic)obj;
        info.AddBaseValueEmbedded(s, typeof(EmbeddedImageGraphic).BaseType);
        info.AddValue("Image", s._imageProxy);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        EmbeddedImageGraphic s = null != o ? (EmbeddedImageGraphic)o : new EmbeddedImageGraphic();
        info.GetBaseValueEmbedded(s, typeof(EmbeddedImageGraphic).BaseType, parent);
        s.Image = (ImageProxy)info.GetValue("Image", s);
        return s;
      }
    }

    #endregion Serialization

    #region Constructors

    public EmbeddedImageGraphic()
      :
      base()
    {
    }

    public EmbeddedImageGraphic(PointD2D graphicPosition, ImageProxy startingImage)
      :
      this()
    {
      SetPosition(graphicPosition, Main.EventFiring.Suppressed);
      Image = startingImage;
    }

    public EmbeddedImageGraphic(double posX, double posY, ImageProxy startingImage)
      :
      this(new PointD2D(posX, posY), startingImage)
    {
    }

    public EmbeddedImageGraphic(PointD2D graphicPosition, PointD2D graphicSize, ImageProxy startingImage)
      :
      this(graphicPosition, startingImage)
    {
      SetSize(graphicSize.X, graphicSize.Y, Main.EventFiring.Suppressed);
    }

    public EmbeddedImageGraphic(double posX, double posY, PointD2D graphicSize, ImageProxy startingImage)
      :
      this(new PointD2D(posX, posY), graphicSize, startingImage)
    {
    }

    public EmbeddedImageGraphic(double posX, double posY, double width, double height, ImageProxy startingImage)
      :
      this(new PointD2D(posX, posY), new PointD2D(width, height), startingImage)
    {
    }

    public EmbeddedImageGraphic(PointD2D graphicPosition, double Rotation, ImageProxy startingImage)
      :
      this(graphicPosition, startingImage)
    {
      this.Rotation = Rotation;
    }

    public EmbeddedImageGraphic(double posX, double posY, double Rotation, ImageProxy startingImage)
      :
      this(new PointD2D(posX, posY), Rotation, startingImage)
    {
    }

    public EmbeddedImageGraphic(PointD2D graphicPosition, PointD2D graphicSize, double Rotation, ImageProxy startingImage)
      :
      this(graphicPosition, Rotation, startingImage)
    {
      SetSize(graphicSize.X, graphicSize.Y, Main.EventFiring.Suppressed);
    }

    public EmbeddedImageGraphic(double posX, double posY, PointD2D graphicSize, double Rotation, ImageProxy startingImage)
      :
      this(new PointD2D(posX, posY), graphicSize, Rotation, startingImage)
    {
    }

    public EmbeddedImageGraphic(double posX, double posY, double width, double height, double Rotation, ImageProxy startingImage)
      :
      this(new PointD2D(posX, posY), new PointD2D(width, height), Rotation, startingImage)
    {
    }

    public EmbeddedImageGraphic(EmbeddedImageGraphic from)
      :
      base(from) // all is done here, since CopyFrom is virtual!
    {
    }

    public override bool CopyFrom(object obj)
    {
      var isCopied = base.CopyFrom(obj);
      if (isCopied && !object.ReferenceEquals(this, obj))
      {
        var from = obj as EmbeddedImageGraphic;
        if (null != from)
        {
          Image = from._imageProxy;
        }
      }
      return isCopied;
    }

    #endregion Constructors

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

    public override PointD2D GetImageSizePt()
    {
      return _imageProxy is null ? new PointD2D(1, 1) : (PointD2D)_imageProxy.Size;
    }

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

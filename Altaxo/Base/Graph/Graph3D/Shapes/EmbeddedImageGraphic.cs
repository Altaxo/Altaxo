#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D.GraphicsContext;

namespace Altaxo.Graph.Graph3D.Shapes
{
  [Serializable]
  public class EmbeddedImageGraphic : ImageGraphic
  {
    protected ImageProxy _imageProxy;

    #region Serialization

    protected EmbeddedImageGraphic(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    :
    base(info)
    {
    }

    /// <summary>
    /// 2016-02-16 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(EmbeddedImageGraphic), 0)]
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
        var s = (EmbeddedImageGraphic)o ?? new EmbeddedImageGraphic(info);
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

    public EmbeddedImageGraphic(PointD3D graphicPosition, ImageProxy startingImage)
      :
      this()
    {
      SetPosition(graphicPosition, Main.EventFiring.Suppressed);
      Image = startingImage;
    }

    public EmbeddedImageGraphic(double posX, double posY, double posZ, ImageProxy startingImage)
      :
      this(new PointD3D(posX, posY, posZ), startingImage)
    {
    }

    public EmbeddedImageGraphic(PointD3D graphicPosition, VectorD3D graphicSize, ImageProxy startingImage)
      :
      this(graphicPosition, startingImage)
    {
      SetSize(graphicSize.X, graphicSize.Y, graphicSize.Z, Main.EventFiring.Suppressed);
    }

    public EmbeddedImageGraphic(double posX, double posY, double posZ, VectorD3D graphicSize, ImageProxy startingImage)
      :
      this(new PointD3D(posX, posY, posZ), graphicSize, startingImage)
    {
    }

    public EmbeddedImageGraphic(double posX, double posY, double posZ, double width, double height, ImageProxy startingImage)
      :
      this(new PointD3D(posX, posY, posZ), new VectorD3D(width, height, 0), startingImage)
    {
    }

    public EmbeddedImageGraphic(PointD3D graphicPosition, double Rotation, ImageProxy startingImage)
      :
      this(graphicPosition, startingImage)
    {
      RotationZ = Rotation;
    }

    public EmbeddedImageGraphic(double posX, double posY, double posZ, double Rotation, ImageProxy startingImage)
      :
      this(new PointD3D(posX, posY, posZ), Rotation, startingImage)
    {
    }

    public EmbeddedImageGraphic(PointD3D graphicPosition, VectorD3D graphicSize, double Rotation, ImageProxy startingImage)
      :
      this(graphicPosition, Rotation, startingImage)
    {
      SetSize(graphicSize.X, graphicSize.Y, graphicSize.Z, Main.EventFiring.Suppressed);
    }

    public EmbeddedImageGraphic(double posX, double posY, double posZ, VectorD3D graphicSize, double Rotation, ImageProxy startingImage)
      :
      this(new PointD3D(posX, posY, posZ), graphicSize, Rotation, startingImage)
    {
    }

    public EmbeddedImageGraphic(double posX, double posY, double posZ, double width, double height, double Rotation, ImageProxy startingImage)
      :
      this(new PointD3D(posX, posY, posZ), new VectorD3D(width, height, 0), Rotation, startingImage)
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
          Image = null == from._imageProxy ? null : (ImageProxy)from._imageProxy.Clone();
        }
      }
      return isCopied;
    }

    #endregion Constructors

    public override object Clone()
    {
      return new EmbeddedImageGraphic(this);
    }

    public ImageProxy Image
    {
      get
      {
        return _imageProxy;
      }
      set
      {
        _imageProxy = value;
        var originalItemSize = new PointD2D(10, 10);
        if (null != _imageProxy)
        {
          Image img = _imageProxy == null ? null : _imageProxy.GetImage();
          if (null != img)
            originalItemSize = new PointD2D((72.0 * img.Width / img.HorizontalResolution), (72.0 * img.Height / img.VerticalResolution));
        }
        ((ItemLocationDirectAspectPreserving)_location).OriginalItemSize = new VectorD3D(originalItemSize.X, originalItemSize.Y, 0);
      }
    }

    public override PointD2D GetImageSizePt()
    {
      return _imageProxy == null ? new PointD2D(1, 1) : _imageProxy.Size;
    }

    public override Image GetImage()
    {
      return _imageProxy == null ? null : _imageProxy.GetImage();
    }

    public override void Paint(IGraphicsContext3D g, IPaintContext context)
    {
      throw new NotImplementedException();

      /*
			GraphicsState gs = g.Save();
			TransformGraphics(g);

			Image img = _imageProxy == null ? null : _imageProxy.GetImage();

			if (null != img)
			{
				var bounds = this.Bounds;

				g.DrawImage(img, (float)bounds.X, (float)bounds.Y, (float)bounds.Width, (float)bounds.Height);
			}

			g.Restore(gs);
			*/
    }
  } // End Class
}

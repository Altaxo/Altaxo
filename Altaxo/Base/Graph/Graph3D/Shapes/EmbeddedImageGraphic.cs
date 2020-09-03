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

#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Drawing;
using Altaxo.Geometry;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Graph3D.GraphicsContext;

namespace Altaxo.Graph.Graph3D.Shapes
{
  [Serializable]
  public class EmbeddedImageGraphic : ImageGraphic
  {
    protected ImageProxy _imageProxy;

    #region Serialization

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected EmbeddedImageGraphic(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
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
        info.AddBaseValueEmbedded(s, typeof(EmbeddedImageGraphic).BaseType!);
        info.AddValue("Image", s._imageProxy);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (EmbeddedImageGraphic?)o ?? new EmbeddedImageGraphic(info);
        info.GetBaseValueEmbedded(s, typeof(EmbeddedImageGraphic).BaseType!, parent);
        s.Image = (ImageProxy)info.GetValue("Image", s);
        return s;
      }
    }

    #endregion Serialization

    #region Constructors



    public EmbeddedImageGraphic(PointD3D graphicPosition, ImageProxy startingImage)
      :
      base()
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
      : base(from)
    {
      CopyFrom(from, false);
    }

    [MemberNotNull(nameof(_imageProxy))]
    protected void CopyFrom(EmbeddedImageGraphic from, bool withBaseMembers)
    {
      if (withBaseMembers)
        base.CopyFrom(from, withBaseMembers);

      Image = from._imageProxy;
    }

    public override bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;
      if (obj is EmbeddedImageGraphic from)
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
      [MemberNotNull(nameof(_imageProxy))]
      set
      {
        _imageProxy = value ?? throw new ArgumentNullException(nameof(Image));
        var originalItemSize = _imageProxy.Size;
        ((ItemLocationDirectAspectPreserving)_location).OriginalItemSize = new VectorD3D(originalItemSize.X, originalItemSize.Y, 0);
      }
    }

    public override PointD2D GetImageSizePt()
    {
      return _imageProxy is null ? new PointD2D(1, 1) : (PointD2D)_imageProxy.Size;
    }

    public override Image GetImage()
    {
      var str = _imageProxy.GetContentStream();
      return SystemDrawingImageProxyExtensions.GetImage(str, disposeStream: true);

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

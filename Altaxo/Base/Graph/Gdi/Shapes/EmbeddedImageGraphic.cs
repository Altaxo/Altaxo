#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;
using System.IO;


namespace Altaxo.Graph.Gdi.Shapes
{
  [Serializable]
  public class EmbeddedImageGraphic : ImageGraphic
  {
    protected ImageProxy _imageProxy;

    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.EmbeddedImageGraphic", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(EmbeddedImageGraphic), 1)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        EmbeddedImageGraphic s = (EmbeddedImageGraphic)obj;
        info.AddBaseValueEmbedded(s, typeof(EmbeddedImageGraphic).BaseType);
        info.AddValue("Image", s._imageProxy);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        EmbeddedImageGraphic s = null != o ? (EmbeddedImageGraphic)o : new EmbeddedImageGraphic();
        info.GetBaseValueEmbedded(s, typeof(EmbeddedImageGraphic).BaseType, parent);
        s._imageProxy = (ImageProxy)info.GetValue("Image", s);
        return s;
      }
    }

    /// <summary>
    /// Finale measures after deserialization.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public override void OnDeserialization(object obj)
    {
    }
    #endregion

    #region Constructors

    public EmbeddedImageGraphic()
      :
      base()
    {
    }


    public EmbeddedImageGraphic(PointF graphicPosition, ImageProxy startingImage)
      :
      this()
    {
      this.SetPosition(graphicPosition);
      this.Image = startingImage;
    }

    public EmbeddedImageGraphic(float posX, float posY, ImageProxy startingImage)
      :
      this(new PointF(posX, posY), startingImage)
    {
    }

    public EmbeddedImageGraphic(PointF graphicPosition, SizeF graphicSize, ImageProxy startingImage)
      :
      this(graphicPosition, startingImage)
    {
      this.SetSize(graphicSize);
      this.AutoSize = false;
    }

    public EmbeddedImageGraphic(float posX, float posY, SizeF graphicSize, ImageProxy startingImage)
      :
      this(new PointF(posX, posY), graphicSize, startingImage)
    {
    }
    public EmbeddedImageGraphic(float posX, float posY, float width, float height, ImageProxy startingImage)
      :
      this(new PointF(posX, posY), new SizeF(width, height), startingImage)
    {
    }

    public EmbeddedImageGraphic(PointF graphicPosition, float Rotation, ImageProxy startingImage)
      :
      this(graphicPosition, startingImage)
    {
      this.Rotation = Rotation;
    }

    public EmbeddedImageGraphic(float posX, float posY, float Rotation, ImageProxy startingImage)
      :
      this(new PointF(posX, posY), Rotation, startingImage)
    {
    }

    public EmbeddedImageGraphic(PointF graphicPosition, SizeF graphicSize, float Rotation, ImageProxy startingImage)
      :
      this(graphicPosition, Rotation, startingImage)
    {
      this.SetSize(graphicSize);
      this.AutoSize = false;
    }
    public EmbeddedImageGraphic(float posX, float posY, SizeF graphicSize, float Rotation, ImageProxy startingImage)
      :
      this(new PointF(posX, posY), graphicSize, Rotation, startingImage)
    {
    }

    public EmbeddedImageGraphic(float posX, float posY, float width, float height, float Rotation, ImageProxy startingImage)
      :
      this(new PointF(posX, posY), new SizeF(width, height), Rotation, startingImage)
    {
    }

    public EmbeddedImageGraphic(EmbeddedImageGraphic from)
      :
      base(from) // all is done here, since CopyFrom is overridden
    {
    }

    protected override void CopyFrom(GraphicBase bfrom)
    {
      EmbeddedImageGraphic from = bfrom as EmbeddedImageGraphic;
      if (from != null)
      {
        this._imageProxy = null == from._imageProxy ? null : (ImageProxy)from._imageProxy.Clone();
      }
      base.CopyFrom(bfrom);
    }

    #endregion

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
      }
    }



    public override Image GetImage()
    {
      return this._imageProxy == null ? null : this._imageProxy.GetImage();
    }

    public override void Paint(Graphics g, object obj)
    {
      GraphicsState gs = g.Save();
      g.TranslateTransform(X, Y);
      if (_rotation != 0)
        g.RotateTransform(-_rotation);

      Image img = _imageProxy == null ? null : _imageProxy.GetImage();

      if (null != img)
      {
        if (this.AutoSize)
        {
          this.Width = (img.Width / img.HorizontalResolution) * g.DpiX;
          this.Height = (img.Height / img.VerticalResolution) * g.DpiY;
        }

        g.DrawImage(img, 0, 0, Width, Height);

      }

      g.Restore(gs);
    }

  } // End Class
}

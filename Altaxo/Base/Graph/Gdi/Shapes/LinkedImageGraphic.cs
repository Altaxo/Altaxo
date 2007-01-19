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
  public class LinkedImageGraphic : ImageGraphic
  {
    protected string _imagePath;
    [NonSerialized()]
    protected Image _cachedImage;


    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.LinkedImageGraphic", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinkedImageGraphic), 1)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        LinkedImageGraphic s = (LinkedImageGraphic)obj;
        info.AddBaseValueEmbedded(s, typeof(LinkedImageGraphic).BaseType);
        info.AddValue("ImagePath", s._imagePath);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        LinkedImageGraphic s = null != o ? (LinkedImageGraphic)o : new LinkedImageGraphic();
        info.GetBaseValueEmbedded(s, typeof(LinkedImageGraphic).BaseType, parent);
        s._imagePath = info.GetString("ImagePath");
        return s;
      }
    }


    /// <summary>
    /// Finale measures after deserialization.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public override void OnDeserialization(object obj)
    {
      // load the image into memory here
      GetImage();
    }
    #endregion



    #region Constructors
    public LinkedImageGraphic()
      :
      base()
    {
    }

    public LinkedImageGraphic(PointF graphicPosition, string ImagePath)
      :
      this()
    {
      this.SetPosition(graphicPosition);
      this.ImagePath = ImagePath;
    }

    public LinkedImageGraphic(float posX, float posY, string ImagePath)
      :
      this(new PointF(posX, posY), ImagePath)
    {
    }
    public LinkedImageGraphic(PointF graphicPosition, SizeF graphicSize, string ImagePath)
      :
      this(graphicPosition, ImagePath)
    {
      this.SetSize(graphicSize);
      this.AutoSize = false;
    }

    public LinkedImageGraphic(float posX, float posY, SizeF graphicSize, string ImagePath)
      :
      this(new PointF(posX, posY), graphicSize, ImagePath)
    {
    }
    public LinkedImageGraphic(float posX, float posY, float width, float height, string ImagePath)
      :
      this(new PointF(posX, posY), new SizeF(width, height), ImagePath)
    {
    }

    public LinkedImageGraphic(PointF graphicPosition, float Rotation, string ImagePath)
      :
      this(graphicPosition, ImagePath)
    {
      this.Rotation = Rotation;
    }

    public LinkedImageGraphic(float posX, float posY, float Rotation, string ImagePath)
      :
      this(new PointF(posX, posY), Rotation, ImagePath)
    {
    }

    public LinkedImageGraphic(PointF graphicPosition, SizeF graphicSize, float Rotation, string ImagePath)
      :
      this(graphicPosition, Rotation, ImagePath)
    {
      this.SetSize(graphicSize);
      this.AutoSize = false;
    }

    public LinkedImageGraphic(float posX, float posY, SizeF graphicSize, float Rotation, string ImagePath)
      :
      this(new PointF(posX, posY), graphicSize, Rotation, ImagePath)
    {
    }

    public LinkedImageGraphic(float posX, float posY, float width, float height, float Rotation, string ImagePath)
      :
      this(new PointF(posX, posY), new SizeF(width, height), Rotation, ImagePath)
    {
    }

    public LinkedImageGraphic(LinkedImageGraphic from)
      :
      base(from)
    {
    }
    protected override void CopyFrom(GraphicBase bfrom)
    {
      LinkedImageGraphic from = bfrom as LinkedImageGraphic;
      if (from != null)
      {
        this._imagePath = from._imagePath;
        this._cachedImage = null == from._cachedImage ? null : (Image)from._cachedImage.Clone();
      }
      base.CopyFrom(bfrom);
    }

    #endregion

    public override object Clone()
    {
      return new LinkedImageGraphic(this);
    }


    public override Image GetImage()
    {
      try
      {
        if (_cachedImage == null)
          _cachedImage = new Bitmap(_imagePath);
        return _cachedImage;
      }
      catch (System.Exception)
      {
        return null;
      }
    }

    public string ImagePath
    {
      get
      {
        return _imagePath;
      }
      set
      {
        if (value != _imagePath)
        {
          _imagePath = value;
          _cachedImage = null;
        }
      }
    }

    public override void Paint(Graphics g, object obj)
    {
      GraphicsState gs = g.Save();
      g.TranslateTransform(X, Y);
      if (_rotation != 0)
        g.RotateTransform(-_rotation);

      Image myImage = this.GetImage();

      if (null != myImage)
      {
        if (this.AutoSize)
        {
          float myNewWidth = (myImage.Width / myImage.HorizontalResolution) * g.DpiX;
          float myNewHeight = (myImage.Height / myImage.VerticalResolution) * g.DpiY;
          this.Height = myNewHeight;
          this.Width = myNewWidth;
        }
        g.DrawImage(myImage, 0, 0, Width, Height);
      }

      g.Restore(gs);
    }
  } // End Class
}

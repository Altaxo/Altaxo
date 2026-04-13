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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Geometry;

namespace Altaxo.Graph.Gdi.Shapes
{
  /// <summary>
  /// Represents an image graphic that loads its image from an external path.
  /// </summary>
  [Serializable]
  public class LinkedImageGraphic : ImageGraphic
  {
    /// <summary>
    /// The path to the linked image.
    /// </summary>
    protected string _imagePath;

    /// <summary>
    /// The cached linked image.
    /// </summary>
    [NonSerialized()]
    protected Image? _cachedImage;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.LinkedImageGraphic", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinkedImageGraphic), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc />
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (LinkedImageGraphic)o;
        info.AddBaseValueEmbedded(s, typeof(LinkedImageGraphic).BaseType!);
        info.AddValue("ImagePath", s._imagePath);
      }

      /// <inheritdoc />
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (LinkedImageGraphic?)o ?? new LinkedImageGraphic(info);
        info.GetBaseValueEmbedded(s, typeof(LinkedImageGraphic).BaseType!, parent);
        s._imagePath = info.GetString("ImagePath");
        return s;
      }
    }

    #endregion Serialization

    #region Constructors

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    /// <summary>
    /// Initializes a new instance of the <see cref="LinkedImageGraphic"/> class for deserialization.
    /// </summary>
    /// <param name="info">The deserialization info.</param>
    public LinkedImageGraphic(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
      :
      base(info)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinkedImageGraphic"/> class.
    /// </summary>
    /// <param name="graphicPosition">The graphic position.</param>
    /// <param name="ImagePath">The image path.</param>
    public LinkedImageGraphic(PointD2D graphicPosition, string ImagePath)
      : base()
    {
      SetPosition(graphicPosition, Main.EventFiring.Suppressed);
      this.ImagePath = ImagePath;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinkedImageGraphic"/> class.
    /// </summary>
    /// <param name="posX">The x-position of the graphic.</param>
    /// <param name="posY">The y-position of the graphic.</param>
    /// <param name="ImagePath">The image path.</param>
    public LinkedImageGraphic(double posX, double posY, string ImagePath)
      :
      this(new PointD2D(posX, posY), ImagePath)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinkedImageGraphic"/> class.
    /// </summary>
    /// <param name="graphicPosition">The graphic position.</param>
    /// <param name="graphicSize">The graphic size.</param>
    /// <param name="ImagePath">The image path.</param>
    public LinkedImageGraphic(PointD2D graphicPosition, PointD2D graphicSize, string ImagePath)
      :
      this(graphicPosition, ImagePath)
    {
      SetSize(graphicSize.X, graphicSize.X, Main.EventFiring.Suppressed);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinkedImageGraphic"/> class.
    /// </summary>
    /// <param name="posX">The x-position of the graphic.</param>
    /// <param name="posY">The y-position of the graphic.</param>
    /// <param name="graphicSize">The graphic size.</param>
    /// <param name="ImagePath">The image path.</param>
    public LinkedImageGraphic(double posX, double posY, PointD2D graphicSize, string ImagePath)
      :
      this(new PointD2D(posX, posY), graphicSize, ImagePath)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinkedImageGraphic"/> class.
    /// </summary>
    /// <param name="posX">The x-position of the graphic.</param>
    /// <param name="posY">The y-position of the graphic.</param>
    /// <param name="width">The width of the graphic.</param>
    /// <param name="height">The height of the graphic.</param>
    /// <param name="ImagePath">The image path.</param>
    public LinkedImageGraphic(double posX, double posY, double width, double height, string ImagePath)
      :
      this(new PointD2D(posX, posY), new PointD2D(width, height), ImagePath)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinkedImageGraphic"/> class.
    /// </summary>
    /// <param name="graphicPosition">The graphic position.</param>
    /// <param name="Rotation">The rotation angle.</param>
    /// <param name="ImagePath">The image path.</param>
    public LinkedImageGraphic(PointD2D graphicPosition, double Rotation, string ImagePath)
      :
      this(graphicPosition, ImagePath)
    {
      this.Rotation = Rotation;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinkedImageGraphic"/> class.
    /// </summary>
    /// <param name="posX">The x-position of the graphic.</param>
    /// <param name="posY">The y-position of the graphic.</param>
    /// <param name="Rotation">The rotation angle.</param>
    /// <param name="ImagePath">The image path.</param>
    public LinkedImageGraphic(double posX, double posY, double Rotation, string ImagePath)
      :
      this(new PointD2D(posX, posY), Rotation, ImagePath)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinkedImageGraphic"/> class.
    /// </summary>
    /// <param name="graphicPosition">The graphic position.</param>
    /// <param name="graphicSize">The graphic size.</param>
    /// <param name="Rotation">The rotation angle.</param>
    /// <param name="ImagePath">The image path.</param>
    public LinkedImageGraphic(PointD2D graphicPosition, PointD2D graphicSize, double Rotation, string ImagePath)
      :
      this(graphicPosition, Rotation, ImagePath)
    {
      SetSize(graphicSize.X, graphicSize.X, Main.EventFiring.Suppressed);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinkedImageGraphic"/> class.
    /// </summary>
    /// <param name="posX">The x-position of the graphic.</param>
    /// <param name="posY">The y-position of the graphic.</param>
    /// <param name="graphicSize">The graphic size.</param>
    /// <param name="Rotation">The rotation angle.</param>
    /// <param name="ImagePath">The image path.</param>
    public LinkedImageGraphic(double posX, double posY, PointD2D graphicSize, double Rotation, string ImagePath)
      :
      this(new PointD2D(posX, posY), graphicSize, Rotation, ImagePath)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinkedImageGraphic"/> class.
    /// </summary>
    /// <param name="posX">The x-position of the graphic.</param>
    /// <param name="posY">The y-position of the graphic.</param>
    /// <param name="width">The width of the graphic.</param>
    /// <param name="height">The height of the graphic.</param>
    /// <param name="Rotation">The rotation angle.</param>
    /// <param name="ImagePath">The image path.</param>
    public LinkedImageGraphic(double posX, double posY, double width, double height, double Rotation, string ImagePath)
      :
      this(new PointD2D(posX, posY), new PointD2D(width, height), Rotation, ImagePath)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinkedImageGraphic"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    public LinkedImageGraphic(LinkedImageGraphic from)
      :
      base(from)
    {
      CopyFrom(from, false);
    }

    /// <summary>
    /// Copies the values from another <see cref="LinkedImageGraphic"/> instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    /// <param name="withBaseMembers">If set to <see langword="true"/>, base class members are copied as well.</param>
    [MemberNotNull(nameof(_imagePath))]
    protected void CopyFrom(LinkedImageGraphic from, bool withBaseMembers)
    {
      if (withBaseMembers)
        base.CopyFrom(from, withBaseMembers);

      _imagePath = from._imagePath;
      _cachedImage = from._cachedImage is null ? null : (Image)from._cachedImage.Clone();
    }

    /// <inheritdoc />
    public override bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;
      if (obj is LinkedImageGraphic from)
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
      return new LinkedImageGraphic(this);
    }

    /// <inheritdoc />
    public override Image? GetImage()
    {
      try
      {
        if (_cachedImage is null)
        {
          _cachedImage = new Bitmap(_imagePath);
          ((ItemLocationDirectAspectPreserving)_location).OriginalItemSize = new PointD2D(72.0 * _cachedImage.Width / _cachedImage.HorizontalResolution, 72.0 * _cachedImage.Height / _cachedImage.HorizontalResolution);
        }

        return _cachedImage;
      }
      catch (System.Exception)
      {
        return null;
      }
    }

    /// <inheritdoc />
    public override PointD2D GetImageSizePt()
    {
      var img = GetImage();
      return img is null ? PointD2D.Empty : new PointD2D(img.Width * 72.0 / img.HorizontalResolution, img.Height * 72.0 / img.VerticalResolution);
    }

    /// <summary>
    /// Gets or sets the external image path.
    /// </summary>
    public string ImagePath
    {
      get
      {
        return _imagePath;
      }
      [MemberNotNull(nameof(_imagePath))]
      set
      {
        if (value != _imagePath)
        {
          _imagePath = value;
          _cachedImage = null;
        }
      }
    }

    /// <inheritdoc />
    public override void Paint(Graphics g, IPaintContext paintContext)
    {
      GraphicsState gs = g.Save();
      TransformGraphics(g);

      Image? myImage = GetImage();

      if (myImage is not null)
      {
        var bounds = Bounds;
        g.DrawImage(myImage, (float)bounds.X, (float)bounds.Y, (float)bounds.Width, (float)bounds.Height);
      }

      g.Restore(gs);
    }
  } // End Class
}

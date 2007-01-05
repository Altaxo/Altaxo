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
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Resources;

namespace Altaxo.Graph
{
  /// <summary>
  /// Holds an image, either from a resource or from a file stream or from the clipboard.
  /// </summary>
  [Serializable]
  public class ImageProxy : ICloneable
  {
    /// <summary>
    /// Either the name of a resource, or the original file name.
    /// </summary>
    string _url;
    string _name;
    MemoryStream _stream;
    
    // Cached objects
    [NonSerialized]
    Image _image;

    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Drawing.ImageProxy", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ImageProxy), 1)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        ImageProxy s = (ImageProxy)obj;
        info.AddValue("Url", s._url);
        info.AddValue("Name", s._name);
        info.AddValue("Stream", s._stream);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        ImageProxy s = SDeserialize(o, info, parent);
        return s;
      }

      public virtual ImageProxy SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        ImageProxy s = null != o ? (ImageProxy)o : new ImageProxy();
        s._url = info.GetString("Url");
        s._name = info.GetString("Name");
        s._stream = info.GetMemoryStream("Stream");

        return s;
      }
    }
    #endregion

    public override string ToString()
    {
      return _name;
    }

    private ImageProxy()
    {
    }

    private void CopyFrom(ImageProxy from)
    {
      this._url = from._url;
      this._name = from._name;
      this._stream = from._stream;
      this._image = from._image;
    }

    public static ImageProxy FromFile(string fullpath)
    {
      ImageProxy img = new ImageProxy();
      img._url = "file://" + fullpath;
      img._name = System.IO.Path.GetFileName(fullpath);
      img.LoadStreamBuffer(fullpath);
      return img;
    }

    public static ImageProxy FromResource(string fullpath)
    {
      ImageProxy img = new ImageProxy();
      img._url = "res://" + fullpath;
      img._name = System.IO.Path.GetFileName(fullpath);
      return img;
    }

    public static ImageProxy FromImage(System.Drawing.Image image)
    {
      return FromImage(image, "unnamed");
    }

    public static ImageProxy FromImage(System.Drawing.Image image, string name)
    {
      ImageProxy img = new ImageProxy();
      img._url = "image://" + name;
      img._name = name;
      img._image = image;
      MemoryStream str1;
      MemoryStream str2;
      if (image is Metafile)
      {
        Metafile mf = (Metafile)image;
       
        str1 = ImageToStream(image, ImageFormat.Emf);
        str2 = ImageToStream(image, ImageFormat.Png);
      }
      else
      {
        str1 = ImageToStream(image, ImageFormat.Jpeg);
        str2 = ImageToStream(image, ImageFormat.Png);
      }

      if (str2.Length < str1.Length)
      {
        img._stream = str2;
        str1.Dispose();
      }
      else
      {
        img._stream = str1;
        str2.Dispose();
      }

      return img;
    }

    public static ImageProxy FromStream(Stream istr)
    {
      return FromStream(istr, "unnamed");
    }

    public static ImageProxy FromStream(Stream istr, string name)
    {
      ImageProxy img = new ImageProxy();
      img._url = "image://" + name;
      img._name = name;
      img.CopyFromStream(istr);
      return img;
    }

    private static MemoryStream ImageToStream(Image image, ImageFormat format)
    {
      MemoryStream str = new MemoryStream();
      image.Save(str, format);
      str.Flush();
      return str;
    }

    private void CopyFromStream(Stream istr)
    {
        if (null == _stream)
        _stream = new MemoryStream();
      _stream.SetLength(0);

       byte[] buffer = new byte[4096];
       int readed;
       while (0 != (readed = istr.Read(buffer, 0, buffer.Length)))
          _stream.Write(buffer, 0, readed);


      _stream.Flush();
      _stream.Seek(0, SeekOrigin.Begin); ;
    }
    
    private void LoadStreamBuffer(string fullpath)
    {

      // Copy the file into the stream
      using (FileStream istr = new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        CopyFromStream(istr);
        istr.Close();
      }
    }


    public bool IsValid
    {
      get
      {
        if (_image != null)
          return true;

        try
        {
          GetImage();
        }
        catch (Exception)
        {
        }

        return _image != null;
      }
    }


    public static implicit operator System.Drawing.Image(ImageProxy ip)
    {
      return ip.GetImage();
    }

    public System.Drawing.Image GetImage()
    {
      if (_image != null)
        return _image;

      if (_stream != null)
      {
        _stream.Seek(0, SeekOrigin.Begin);
        _image = Image.FromStream(_stream);
      }

      if (_url != null)
      {
        if (_url.StartsWith("file://"))
        {
          string fullpath = _url.Substring("file://".Length);
          LoadStreamBuffer(fullpath);
          if (_stream != null)
          {
            _stream.Seek(0, SeekOrigin.Begin);
            _image = Image.FromStream(_stream);
          }
        }
        else if (_url.StartsWith("res://"))
        {
          string fullpath = _url.Substring("res://".Length);
        }
      }

      return _image;
    }

    #region ICloneable Members

    public ImageProxy Clone()
    {
      ImageProxy result = new ImageProxy();
      result.CopyFrom(this);
      return result;
    }

    object ICloneable.Clone()
    {
      ImageProxy result = new ImageProxy();
      result.CopyFrom(this);
      return result;
    }

    #endregion
  }
}

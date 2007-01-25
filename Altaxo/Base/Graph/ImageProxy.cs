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
  #region ImageProxy
  /// <summary>
  /// Holds an image, either from a resource or from a file stream or from the clipboard.
  /// </summary>
  [Serializable]
  public abstract class ImageProxy : ICloneable
  {
    public override string ToString()
    {
      return Name;
    }

    public static ImageProxy FromFile(string fullpath)
    {
      return MemoryStreamImageProxy.FromFile(fullpath);
    }

    public static ImageProxy FromResource(string fullpath)
    {
      return ResourceImageProxy.FromResource(fullpath);
    }

    public static ImageProxy FromImage(System.Drawing.Image image)
    {
      return MemoryStreamImageProxy.FromImage(image);
    }

    public static ImageProxy FromImage(System.Drawing.Image image, string name)
    {
      return MemoryStreamImageProxy.FromImage(image, name);
    }

    public static ImageProxy FromStream(Stream istr)
    {
      return MemoryStreamImageProxy.FromStream(istr);
    }

    public static ImageProxy FromStream(Stream istr, string name)
    {
      return MemoryStreamImageProxy.FromStream(istr,name);
    }

    public static implicit operator System.Drawing.Image(ImageProxy ip)
    {
      return ip.GetImage();
    }

    public abstract System.Drawing.Image GetImage();

    public abstract string ContentHash { get; }

    public abstract bool IsValid { get; }

    public abstract string Name { get; }

    public bool HasSameContentAs(ImageProxy from)
    {
      return this.ContentHash == from.ContentHash;
    }


    #region ICloneable Members


    public abstract object Clone();

    #endregion
  }
  #endregion

  #region ResourceImageProxy
  /// <summary>
  /// Holds an image, either from a resource or from a file stream or from the clipboard.
  /// </summary>
  [Serializable]
  public class ResourceImageProxy : ImageProxy
  {
    /// <summary>
    /// Either the name of a resource, or the original file name.
    /// </summary>
    string _url;
    string _name;

    // Cached objects
    [NonSerialized]
    Image _image;



    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ResourceImageProxy), 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        ResourceImageProxy s = (ResourceImageProxy)obj;
        info.AddValue("Url", s._url);
        info.AddValue("Name", s._name);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        ResourceImageProxy s = SDeserialize(o, info, parent);
        return s;
      }

      public virtual ResourceImageProxy SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        ResourceImageProxy s = null != o ? (ResourceImageProxy)o : new ResourceImageProxy();
        s._url = info.GetString("Url");
        s._name = info.GetString("Name");
        return s;
      }
    }
    #endregion

    public override string ToString()
    {
      return _name;
    }

    private ResourceImageProxy()
    {
    }

    private void CopyFrom(ResourceImageProxy from)
    {
      this._url = from._url;
      this._name = from._name;
      this._image = from._image;
    }

    public new static ImageProxy FromResource(string fullpath)
    {
      if (string.IsNullOrEmpty(fullpath))
        throw new ArgumentException("Path is null or empty");

      ResourceImageProxy img = new ResourceImageProxy();
      img._url = fullpath;
      img._name = System.IO.Path.GetFileName(fullpath);
      return img;
    }

    public new static ImageProxy FromResource(string name, string fullpath)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentException("Name is null or empty");
      if (string.IsNullOrEmpty(fullpath))
        throw new ArgumentException("Path is null or empty");

      ResourceImageProxy img = new ResourceImageProxy();
      img._url = fullpath;
      img._name = name;
      return img;
    }


    public override string Name { get { return _name; } }


    public override bool IsValid
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

   

    public override System.Drawing.Image GetImage()
    {
      if (_image != null)
        return _image;

      if (_url != null)
      {
       _image = Current.ResourceService.GetBitmap(_url);
      }

      return _image;
    }

    public override string ContentHash 
    {
      get
      {
        return _url+"."+_name;
      }
    }



    #region ICloneable Members

    public override object Clone()
    {
      ResourceImageProxy result = new ResourceImageProxy();
      result.CopyFrom(this);
      return result;
    }

    #endregion
  }
  #endregion

  #region MemoryStreamImageProxy
  /// <summary>
  /// Holds an image, either from a resource or from a file stream or from the clipboard.
  /// </summary>
  [Serializable]
  public class MemoryStreamImageProxy : ImageProxy
  {
    /// <summary>
    /// Either the name of a resource, or the original file name.
    /// </summary>
    string _url;
    string _name;
    MemoryStream _stream;
    string _hash;

    // Cached objects
    [NonSerialized]
    Image _image;

    // Static helpers
    static System.Security.Cryptography.MD5 _md5 = System.Security.Cryptography.MD5CryptoServiceProvider.Create();


    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(MemoryStreamImageProxy), 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        MemoryStreamImageProxy s = (MemoryStreamImageProxy)obj;
        info.AddValue("Url", s._url);
        info.AddValue("Name", s._name);
        info.AddValue("Hash", s._hash);
        info.AddValue("Stream", s._stream);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        MemoryStreamImageProxy s = SDeserialize(o, info, parent);
        return s;
      }

      public virtual MemoryStreamImageProxy SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        MemoryStreamImageProxy s = null != o ? (MemoryStreamImageProxy)o : new MemoryStreamImageProxy();
        s._url = info.GetString("Url");
        s._name = info.GetString("Name");
        s._hash = info.GetString("Hash");
        s._stream = info.GetMemoryStream("Stream");

        return s;
      }
    }
    #endregion

    public override string ToString()
    {
      return _name;
    }

    private MemoryStreamImageProxy()
    {
    }

    private void CopyFrom(MemoryStreamImageProxy from)
    {
      this._url = from._url;
      this._name = from._name;
      this._stream = from._stream;
      this._image = from._image;
      this._hash = from._hash;
    }

    public static MemoryStreamImageProxy FromFile(string fullpath)
    {
      MemoryStreamImageProxy img = new MemoryStreamImageProxy();
      img._url = fullpath;
      img._name = System.IO.Path.GetFileName(fullpath);
      img.LoadStreamBuffer(fullpath);
      return img;
    }

    public static MemoryStreamImageProxy FromImage(System.Drawing.Image image)
    {
      return FromImage(image, null);
    }

    public static MemoryStreamImageProxy FromImage(System.Drawing.Image image, string name)
    {
      MemoryStreamImageProxy img = new MemoryStreamImageProxy();
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

      img.ComputeStreamHash();
      return img;
    }

    public static MemoryStreamImageProxy FromStream(Stream istr)
    {
      return FromStream(istr, null);
    }

    public static MemoryStreamImageProxy FromStream(Stream istr, string name)
    {
      MemoryStreamImageProxy img = new MemoryStreamImageProxy();
      img._url =  name;
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

    private void ComputeStreamHash()
    {
      byte[] hash = _md5.ComputeHash(_stream);
      SetHash(hash);
    }
    private void SetHash(byte[] hash)
    {
      StringBuilder stb = new StringBuilder();
      for (int i = 0; i < hash.Length; i++)
        stb.Append(hash[i].ToString("X2"));

      _hash = stb.ToString();
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
      _stream.Seek(0, SeekOrigin.Begin);

      ComputeStreamHash();
      _stream.Seek(0, SeekOrigin.Begin);
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

    public override string Name { get { return _name; } }


    public override bool IsValid
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

    public override System.Drawing.Image GetImage()
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
          LoadStreamBuffer(_url);
          if (_stream != null)
          {
            _stream.Seek(0, SeekOrigin.Begin);
            _image = Image.FromStream(_stream);
          }
      }
      return _image;
    }

    public override string  ContentHash
{
	get { return _hash; }
}


    #region ICloneable Members

  

    public override object Clone()
    {
      MemoryStreamImageProxy result = new MemoryStreamImageProxy();
      result.CopyFrom(this);
      return result;
    }

    #endregion
  }
  #endregion
}

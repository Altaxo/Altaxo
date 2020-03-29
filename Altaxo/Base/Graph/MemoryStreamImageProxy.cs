#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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
using System.IO;
using System.Text;
using Altaxo.Geometry;

namespace Altaxo.Graph
{
  /// <summary>
  /// Holds an image, either from a resource or from a file stream or from the clipboard.
  /// </summary>
  /// <seealso cref="Altaxo.Graph.ImageProxy" />
  [Serializable]
  public class MemoryStreamImageProxy : ImageProxy, Main.IImmutable
  {
    private byte[] _streamBuffer;
    public override string ContentHash { get; }

    /// <summary>
    /// Gets the extension that designates the type of memory stream, such as '.png' or '.jpg'.
    /// </summary>
    public string Extension { get; }


    /// <summary>
    /// Either the name of a resource, or the original file name.
    /// </summary>
    public string Url { get; }

    public override string Name { get; }


    /// <summary>Image size in points (1/72 inch).</summary>
    private VectorD2D _imageSizePt;
    private VectorD2D _imageSizePixels;
    private VectorD2D _imageResolutionDpi;

    // Static helpers
    private static System.Security.Cryptography.MD5 _md5 = System.Security.Cryptography.MD5CryptoServiceProvider.Create();

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.MemoryStreamImageProxy", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version");
        /*
                MemoryStreamImageProxy s = (MemoryStreamImageProxy)obj;
                info.AddValue("Url", s._url);
                info.AddValue("Name", s._name);
                info.AddValue("Hash", s._hash);
                info.AddValue("Stream", s._stream);
                */
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        MemoryStreamImageProxy s = SDeserialize(o, info, parent);
        return s;
      }

      public virtual MemoryStreamImageProxy SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var url = info.GetString("Url");
        var name = info.GetString("Name");
        var hash = info.GetString("Hash");
        var stream = info.GetMemoryStream("Stream");
        var extension = ".png";

        return new MemoryStreamImageProxy(
          stream: stream,
          useStreamDirectly: true,
          hash: hash,
          extension: extension,
          name: name,
          url: url);
      }
    }

    /// <summary>
    /// Extended 2018-03-27 with _extension;
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(MemoryStreamImageProxy), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (MemoryStreamImageProxy)obj;
        info.AddValue("Url", s.Url);
        info.AddValue("Name", s.Name);
        info.AddValue("Hash", s.ContentHash);
        info.AddValue("Extension", s.Extension);
        var s_stream = new MemoryStream(s._streamBuffer, writable: false);
        info.AddValue("Stream", s_stream);
        s_stream.Dispose();
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        MemoryStreamImageProxy s = SDeserialize(o, info, parent);
        return s;
      }

      public virtual MemoryStreamImageProxy SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var url = info.GetString("Url");
        var name = info.GetString("Name");
        var hash = info.GetString("Hash");
        var extension = info.GetString("Extension");
        var stream = info.GetMemoryStream("Stream");

        return new MemoryStreamImageProxy(
          stream: stream,
          useStreamDirectly: true,
          hash: hash,
          extension: extension,
          name: name,
          url: url);
      }
    }

    #endregion Serialization

    public override string ToString()
    {
      return Name;
    }

    private MemoryStreamImageProxy()
    {
    }

    public MemoryStreamImageProxy(Stream stream, bool useStreamDirectly, string hash, string extension, string name, string url, VectorD2D? imageSizePoint = null, VectorD2D? imageSizePixel = null, VectorD2D? imageResolutionDpi = null)
    {
      if (stream is null)
        throw new ArgumentNullException(nameof(stream));
      if (string.IsNullOrEmpty(extension))
        throw new ArgumentNullException(nameof(extension));
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof(name));
      if (string.IsNullOrEmpty(url))
        throw new ArgumentNullException(nameof(url));

      stream.Seek(0, SeekOrigin.Begin);
      if (stream is MemoryStream memstream)
      {
        _streamBuffer = memstream.ToArray();
      }
      else
      {
        _streamBuffer = new byte[stream.Length];
        stream.Read(_streamBuffer, 0, _streamBuffer.Length);
      }

      ContentHash = ComputeStreamHash(_streamBuffer);
      Extension = extension;
      Name = name;
      Url = url;


      if (imageSizePoint.HasValue && imageSizePixel.HasValue && imageResolutionDpi.HasValue)
        (_imageSizePt, _imageSizePixels, _imageResolutionDpi) = (imageSizePoint.Value, imageSizePixel.Value, imageResolutionDpi.Value);
      else
        (_imageSizePt, _imageSizePixels, _imageResolutionDpi) = GetImageInformation(stream);
    }


    public static new MemoryStreamImageProxy FromFile(string fullpath)
    {
      var stream = new MemoryStream();
      using (var istr = new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        istr.CopyTo(stream);
      }

      var hash = ComputeStreamHash(stream);

      var img = new MemoryStreamImageProxy(
        stream,
        useStreamDirectly: true,
        hash: hash,
        extension: System.IO.Path.GetExtension(fullpath),
        name: System.IO.Path.GetFileName(fullpath),
        url: fullpath
        );


      return img;
    }




    /// <summary>
    /// Creates a <see cref="MemoryStreamImageProxy"/> from a stream. A file name must be provided in order
    /// to deduce the kind of stream (.png for .png stream, .jpg for jpg stream and so on).
    /// </summary>
    /// <param name="istr">The image stream to copy from.</param>
    /// <param name="name">The name. The kind of image is deduced from the extension of this name.</param>
    /// <returns>A memory stream image proxy holding the image.</returns>
    /// <exception cref="ArgumentNullException">
    /// istr
    /// or
    /// name - Name must be provided in order to deduce the file extension
    /// </exception>
    public static new MemoryStreamImageProxy FromStream(Stream istr, string name)
    {
      if (istr == null)
        throw new ArgumentNullException(nameof(istr));
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof(name), "Name must be provided in order to deduce the file extension");

      var stream = new MemoryStream();
      istr.CopyTo(stream);

      var img = new MemoryStreamImageProxy(
        stream: stream,
        useStreamDirectly: true,
        hash: ComputeStreamHash(stream),
        extension: System.IO.Path.GetExtension(name),
        name: name,
        url: name);

      return img;
    }

    /// <summary>
    /// Computes the hash of the content of the given stream. The stream must be seekable, since it is positioned to
    /// the beginning before calculating the hash. Afterwards, the stream position is undefined.
    /// </summary>
    /// <param name="stream">The stream to compute the hash for.</param>
    /// <returns>String with the MD5 hash of the stream.</returns>
    public static string ComputeStreamHash(Stream stream)
    {
      stream.Seek(0, SeekOrigin.Begin);
      byte[] hash = _md5.ComputeHash(stream);
      stream.Seek(0, SeekOrigin.Begin);
      return HashBytesToString(hash);
    }

    /// <summary>
    /// Computes the hash of the content of the given stream. The stream must be seekable, since it is positioned to
    /// the beginning before calculating the hash. Afterwards, the stream position is undefined.
    /// </summary>
    /// <param name="buffer">The byte buffer to compute the hash for.</param>
    /// <returns>String with the MD5 hash of the stream.</returns>
    public static string ComputeStreamHash(byte[] buffer)
    {
      byte[] hash = _md5.ComputeHash(buffer, 0, buffer.Length); ;
      return HashBytesToString(hash);
    }

    private static string HashBytesToString(byte[] hash)
    {
      var stb = new StringBuilder();
      for (int i = 0; i < hash.Length; i++)
        stb.Append(hash[i].ToString("X2"));
      return stb.ToString();
    }


    public override bool IsValid
    {
      get
      {
        return true;
      }
    }

    /// <summary>Returns the original size of the image in points (1/72 inch).</summary>
    public override VectorD2D Size
    {
      get
      {
        return _imageSizePt;
      }
    }

    /// <summary>
    /// Gets the image resolution in dpi.
    /// </summary>
    /// <value>
    /// The resolution in dpi.
    /// </value>
    public VectorD2D ResolutionDpi
    {
      get
      {
        return _imageResolutionDpi;
      }
    }

    /// <summary>
    /// Gets the image size in pixels.
    /// </summary>
    /// <value>
    /// The image size in pixels.
    /// </value>
    public VectorD2D ImageSizePixels
    {
      get
      {
        return _imageSizePixels;
      }
    }

    public override Stream GetContentStream()
    {
      return new MemoryStream(_streamBuffer, writable: false);
    }
  }

}

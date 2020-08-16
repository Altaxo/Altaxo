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
using System.IO;
using Altaxo.Geometry;

#nullable enable

namespace Altaxo.Drawing
{
  /// <summary>
  /// Holds an image, either from a resource or from a file stream or from the clipboard.
  /// </summary>
  [Serializable]
  public class ResourceImageProxy : ImageProxy
  {
    /// <summary>
    /// Either the name of a resource, or the original file name.
    /// </summary>
    private string _url;

    private string _name;

    // Cached values

    [NonSerialized]
    private VectorD2D? _imageSizePt;

    [NonSerialized]
    private byte[]? _streamBuffer;

    #region Serialization

    /// <summary>
    /// 2020-04-01: moved to namespace Altaxo.Drawing
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.ResourceImageProxy", 0)]
    [Serialization.Xml.XmlSerializationSurrogateFor(typeof(ResourceImageProxy), 1)]
    private class XmlSerializationSurrogate0 : Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ResourceImageProxy)obj;
        info.AddValue("Url", s._url);
        info.AddValue("Name", s._name);
      }

      public object Deserialize(object? o, Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }

      public virtual ResourceImageProxy SDeserialize(object? o, Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var url = info.GetString("Url");
        var name = info.GetString("Name");
        return new ResourceImageProxy(url: url, name: name);
      }
    }

    #endregion Serialization

    private static byte[] DefaultImageSubstituteBuffer;

    static ResourceImageProxy()
    {
      // Create a red cross image for the case that the resource could not be resolved
      var img = new System.Drawing.Bitmap(16, 16);
      using (var g = System.Drawing.Graphics.FromImage(img))
      {
        g.DrawLine(System.Drawing.Pens.Red, 0, 0, 16, 16);
        g.DrawLine(System.Drawing.Pens.Red, 0, 16, 16, 0);
      }
      using (var memStr = new MemoryStream())
      {
        img.Save(memStr, System.Drawing.Imaging.ImageFormat.Png);
        memStr.Flush();
        DefaultImageSubstituteBuffer = memStr.ToArray();
      }
    }


    public override string ToString()
    {
      return _name;
    }

    private ResourceImageProxy(string url, string name)
    {
      _url = url;
      _name = name;
    }



    public static new ImageProxy FromResource(string resourceKey)
    {
      if (string.IsNullOrEmpty(resourceKey))
        throw new ArgumentException("Path is null or empty");

      var url = resourceKey;
      var name = Path.GetFileName(resourceKey); // misuse GetFileName to get the last part of the resource key

      return new ResourceImageProxy(url: url, name: name);
    }

    public static ImageProxy FromResource(string name, string resourceKey)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(name);
      if (string.IsNullOrEmpty(resourceKey))
        throw new ArgumentNullException(resourceKey);

      return new ResourceImageProxy(url: resourceKey, name: name);
    }

    public override string Name { get { return _name; } }

    public override VectorD2D Size
    {
      get
      {
        if (!_imageSizePt.HasValue)
        {
          var (imageSizePt, _, _) = GetImageInformation(GetContentStream());
          _imageSizePt = imageSizePt;
        }
        return _imageSizePt.Value;
      }
    }

    public override bool IsValid
    {
      get
      {
        TryResolveStreamBuffer();
        return _streamBuffer?.Length > 0;
      }
    }

    public override Stream GetContentStream()
    {
      TryResolveStreamBuffer();
      return _streamBuffer is null ? new MemoryStream(DefaultImageSubstituteBuffer, writable: false) : new MemoryStream(_streamBuffer, writable: false);
    }

    private void TryResolveStreamBuffer()
    {
      if (_streamBuffer is null)
      {
        var imgObject = Current.ResourceService.GetImageResource(_url);
        if (imgObject is null)
          imgObject = Current.ResourceService.GetResourceStream(_url);

        if (imgObject is Stream stream)
        {
          var streamBuffer = new byte[stream.Length];
          stream.Read(streamBuffer, 0, streamBuffer.Length);
          _streamBuffer ??= streamBuffer;
          stream.Dispose();
        }
        else if (imgObject is byte[] buffer)
        {
          _streamBuffer = buffer;
        }
      }
    }

    public override string ContentHash
    {
      get
      {
        return _url + "." + _name;
      }
    }
  }
}

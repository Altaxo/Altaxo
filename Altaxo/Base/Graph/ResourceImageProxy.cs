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
using System.IO;
using Altaxo.Geometry;

namespace Altaxo.Graph
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
    private byte[] _streamBuffer;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ResourceImageProxy), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ResourceImageProxy)obj;
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

    private ResourceImageProxy()
    {
    }



    public static new ImageProxy FromResource(string fullpath)
    {
      if (string.IsNullOrEmpty(fullpath))
        throw new ArgumentException("Path is null or empty");

      var img = new ResourceImageProxy
      {
        _url = fullpath,
        _name = System.IO.Path.GetFileName(fullpath)
      };
      return img;
    }

    public static ImageProxy FromResource(string name, string fullpath)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentException("Name is null or empty");
      if (string.IsNullOrEmpty(fullpath))
        throw new ArgumentException("Path is null or empty");

      var img = new ResourceImageProxy
      {
        _url = fullpath,
        _name = name
      };
      return img;
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
        var stream = Current.ResourceService.GetResourceStream(_url);
        if (stream is { } _)
        {
          var streamBuffer = new byte[stream.Length];
          stream.Read(streamBuffer, 0, streamBuffer.Length);
          _streamBuffer ??= streamBuffer;
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

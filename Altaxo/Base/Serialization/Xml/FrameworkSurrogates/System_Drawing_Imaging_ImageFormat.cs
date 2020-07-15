#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using System.Linq;

namespace Altaxo.Serialization.Xml.FrameworkSurrogates
{
  using System.Drawing.Imaging;

  internal class System_Drawing_Imaging_ImageFormat
  {
    private static readonly ImageFormat[] ImageFormats = { ImageFormat.Bmp, ImageFormat.Emf, ImageFormat.Exif, ImageFormat.Gif, ImageFormat.Icon, ImageFormat.Jpeg, ImageFormat.MemoryBmp, ImageFormat.Png, ImageFormat.Tiff, ImageFormat.Wmf };

    #region Serialization

    /// <summary>
    /// Initial version (2014-01-18)
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("System.Drawing", "System.Drawing.Imaging.ImageFormat", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(System.Drawing.Imaging.ImageFormat), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ImageFormat)obj;

        info.AddValue("Guid", System.Xml.XmlConvert.ToString(s.Guid));
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var guid = System.Xml.XmlConvert.ToGuid(info.GetString("Guid"));

        var s = ImageFormats.FirstOrDefault(x => x.Guid == guid);

        if (null == s)
          s = new ImageFormat(guid);

        return s;
      }
    }

    #endregion Serialization
  }
}

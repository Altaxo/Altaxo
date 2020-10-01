#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2019 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Main.Properties;

namespace Altaxo.Serialization
{
  public class StorageSettings
  {
    public bool AllowProgressiveStorage { get; set; }

    /// <summary>
    /// Gets or sets the compression level. Valid values range from 0 (no compression) up to 9 (optimal compression).
    /// </summary>
    /// <value>
    /// The compression level.
    /// </value>
    public int CompressionLevel { get; set; } = 5;


    /// <summary>
    /// Get or sets the compression level for zip operations.
    /// </summary>
    /// <value>
    /// The zip compression level.
    /// </value>
    public System.IO.Compression.CompressionLevel ZipCompressionLevel
    {
      get
      {
        return CompressionLevel switch
        {
          <= 3 => System.IO.Compression.CompressionLevel.NoCompression,
          <= 7 => System.IO.Compression.CompressionLevel.Fastest,
          _ => System.IO.Compression.CompressionLevel.Optimal
        };
      }
      set
      {
        var oldValue = CompressionLevel;
        switch (value)
        {
          case System.IO.Compression.CompressionLevel.Optimal:
            CompressionLevel = 9;
            break;
          case System.IO.Compression.CompressionLevel.Fastest:
            CompressionLevel = 5;
            break;
          case System.IO.Compression.CompressionLevel.NoCompression:
            CompressionLevel = 0;
            break;
          default:
            throw new NotImplementedException();
        }
      }
    }


    public static readonly PropertyKey<StorageSettings> PropertyKeyStorageSettings = new PropertyKey<StorageSettings>(
      "AB367EE6-1E6A-4995-A84B-29DD8046E01B",
      "Project\\StorageSettings",
      PropertyLevel.Application | PropertyLevel.Document,
      typeof(Main.IProject),
      () => new StorageSettings()
      );

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(StorageSettings), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (StorageSettings)obj;
        info.AddValue("AllowProgressiveStorage", s.AllowProgressiveStorage);
        info.AddValue("CompressionLevel", s.CompressionLevel);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = (StorageSettings)o ?? new StorageSettings();

        s.AllowProgressiveStorage = info.GetBoolean("AllowProgressiveStorage");
        s.CompressionLevel = info.GetInt32("CompressionLevel");

        return s;
      }
    }


    #endregion Serialization

    public override string ToString()
    {
      return $"AllowProgressive: {AllowProgressiveStorage}, Level: {CompressionLevel}";
    }

  }
}

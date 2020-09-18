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
    public int CompressionLevel { get; set; } = 9;


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



  }
}

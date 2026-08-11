#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

namespace Altaxo.Serialization.NamePropertyExtraction
{

  /// <summary>
  /// Represents an action to put a property into a property bag at a specified level during the import process. The level indicates the hierarchy of the property bag, where 0 is the property bag of the target table, -1 is the property bag of the parent folder, -2 is the property bag of the grandparent folder, and so on.
  /// </summary>
  public record ActionPutToPropertyBag
  {
    /// <summary>
    /// Gets or sets the name of the property to be put into the property bag.
    /// </summary>
    public string PropertyName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the level of the property bag where the property should be put. A level of 0 indicates the property bag of the target table, -1 indicates the property bag of the parent folder, -2 indicates the property bag of the grandparent folder, and so on.
    /// </summary>
    public int Level
    {
      get => field;
      init
      {
        if (field > 0)
        {
          throw new ArgumentOutOfRangeException(nameof(Level), "Level must be less than or equal to 0.");
        }
        field = value;
      }
    }

    #region Serialization

    /// <summary>
    /// V0: 2026-08-10 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ActionPutToPropertyBag), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ActionPutToPropertyBag)o;
        info.AddValue("PropertyName", s.PropertyName);
        info.AddValue("Level", s.Level);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var propertyName = info.GetString("PropertyName");
        var level = info.GetInt32("Level");

        return o is null ? new ActionPutToPropertyBag
        {
          PropertyName = propertyName,
          Level = level,
        } : ((ActionPutToPropertyBag)o) with
        {
          PropertyName = propertyName,
          Level = level,
        };
      }
    }
    #endregion

  }
}



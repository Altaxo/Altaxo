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

namespace Altaxo.Units
{
  /// <summary>
  /// Special case of <see cref="ChangeableRelativeUnit"/> which represents 'percent of some quantity'.
  /// </summary>
  public class ChangeableRelativePercentUnit : ChangeableRelativeUnit
  {
    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ChangeableRelativePercentUnit), 0)]
    public class SerializationSurrogateB0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ChangeableRelativePercentUnit)obj;

        info.AddValue("Name", s.Name);
        info.AddValue("ShortCut", s.ShortCut);
        info.AddValue("Reference", s.ReferenceQuantity);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var value = info.GetString("Name");
        var shortCut = info.GetString("ShortCut");
        var reference = info.GetValue<DimensionfulQuantity>("Reference", null);
        return new ChangeableRelativePercentUnit(value, shortCut, reference);
      }
    }
    #endregion

    /// <summary>Initializes a new instance of the <see cref="ChangeableRelativePercentUnit"/> class.</summary>
    /// <param name="name">The full name of the unit (e.g. 'percent of page with').</param>
    /// <param name="shortcut">The shortcut of the unit (e.g. %PW).</param>
    /// <param name="valueForHundredPercent">The quantity that corresponds to a value of hundred percent.</param>
    public ChangeableRelativePercentUnit(string name, string shortcut, DimensionfulQuantity valueForHundredPercent)
      : base(name, shortcut, 100, valueForHundredPercent)
    {
    }
  }
}

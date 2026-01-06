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

namespace Altaxo.Units.Angle
{
  [UnitDescription("Angular measure", 0, 0, 0, 0, 0, 0, 0)]
  public class Radian : SIUnit
  {
    private static readonly Radian _instance = new Radian();

    /// <summary>List with only the prefix <see cref="SIPrefix.None"/>.</summary>
    private static SIPrefixList _prefixList = new SIPrefixList(new SIPrefix[] { SIPrefix.None, SIPrefix.Micro, SIPrefix.Nano, SIPrefix.Pico });

    /// <summary>
    /// Gets the singleton instance of the <see cref="Radian"/> unit.
    /// </summary>
    public static Radian Instance { get { return _instance; } }

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Radian), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return Radian.Instance;
      }
    }
    #endregion

    private Radian()
        : base(0, 0, 0, 0, 0, 0, 0)
    {
    }

    /// <inheritdoc/>
    public override string Name
    {
      get { return "Radian"; }
    }

    /// <inheritdoc/>
    public override string ShortCut
    {
      get { return "rad"; }
    }

    /// <inheritdoc/>
    public override ISIPrefixList Prefixes
    {
      get { return _prefixList; }
    }
  }
}

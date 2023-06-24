#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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

using System.Collections.Generic;

namespace Altaxo.Serialization.Ascii
{
  /// <summary>
  /// This separation strategy returns the entire line as a single token. This strategy can be used for instance when you have text containing spaces, that should be imported in one single text column.
  /// </summary>
  public record SingleLineSeparationStrategy : IAsciiSeparationStrategy
  {
    #region Serialization

    /// <summary>2014-08-03 initial version.</summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SingleLineSeparationStrategy), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (SingleLineSeparationStrategy?)o ?? new SingleLineSeparationStrategy();
        return s;
      }
    }

    #endregion Serialization

    public IEnumerable<string> GetTokens(string line)
    {
      yield return line;
    }

    public override string ToString()
    {
      return $"{GetType().Name}";
    }
  }
}

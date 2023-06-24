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

using System;
using System.Collections.Generic;

namespace Altaxo.Serialization.Ascii
{
  /// <summary>
  /// This stategy assumes that the tokens are separated by one or more whitespace chars (tabs and spaces).
  /// </summary>
  public record SkipWhiteSpaceSeparationStrategy : IAsciiSeparationStrategy
  {
    private static char[] _sSeparators = new[] { ' ', '\t' };

    #region Serialization

    /// <summary>2014-08-03 initial version.</summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SkipWhiteSpaceSeparationStrategy), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SkipWhiteSpaceSeparationStrategy)obj;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (SkipWhiteSpaceSeparationStrategy?)o ?? new SkipWhiteSpaceSeparationStrategy();
        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="SkipWhiteSpaceSeparationStrategy"/> class.
    /// </summary>
    public SkipWhiteSpaceSeparationStrategy()
    {
    }

    public IEnumerable<string> GetTokens(string line)
    {
      return line.Split(_sSeparators, StringSplitOptions.RemoveEmptyEntries);
    }

    public override string ToString()
    {
      return $"{GetType().Name}";
    }
  }
}

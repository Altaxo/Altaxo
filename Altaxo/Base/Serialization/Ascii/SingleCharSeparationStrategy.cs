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
  /// This strategy assumes that the tokens are separated by exactly one (!) separation char. The separation character has to
  /// be provided in the constructor.
  /// </summary>
  public record SingleCharSeparationStrategy : IAsciiSeparationStrategy
  {
    public char SeparatorChar { get; init; }

    #region Serialization

    /// <summary>2014-08-03 initial version.</summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SingleCharSeparationStrategy), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SingleCharSeparationStrategy)obj;
        info.AddValue("SeparatorChar", s.SeparatorChar);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var separatorChar = info.GetChar("SeparatorChar");
        return o is SingleCharSeparationStrategy s ?
          s with { SeparatorChar = separatorChar } :
          new SingleCharSeparationStrategy(separatorChar);
      }
    }

    #endregion Serialization

    /// <summary>
    /// Constructor for this strategy. You must provide a separator char.
    /// </summary>
    /// <param name="separator">The separator char used.</param>
    public SingleCharSeparationStrategy(char separator)
    {
      SeparatorChar = separator;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleCharSeparationStrategy"/> class with tabulator as the separator char.
    /// </summary>
    public SingleCharSeparationStrategy()
      : this('\t')
    {
    }

    public IEnumerable<string> GetTokens(string line)
    {
      int len = line.Length;
      int ix = 0;
      for (int start = 0; start <= len; start = ix + 1)
      {
        ix = line.IndexOf(SeparatorChar, start, len - start);
        if (ix == -1)
        {
          ix = len;
        }
        yield return line.Substring(start, ix - start);
      }
    }

    public string GetSeparatorCharReadable()
    {
      return SeparatorChar switch
      {
        '\t' => "TAB",
        ' ' => "SPACE",
        _ => "" + SeparatorChar,
      };
    }

    public override string ToString()
    {
      return $"{this.GetType().Name}[{GetSeparatorCharReadable()}]";
    }
  }
}

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
using System.Collections.Immutable;
using System.Linq;

namespace Altaxo.Serialization.Ascii
{
  /// <summary>
  /// This strategy assumes that the tokens fill the string at fixed positions in the string and have a fixed length.
  /// The starting position of the first token is always zero. The starting positions of each subsequent token (beginning with the second token) has to be provided in the constructor.
  /// </summary>
  public record FixedColumnWidthWithoutTabSeparationStrategy : IAsciiSeparationStrategy
  {
    public ImmutableArray<int> StartPositions { get; init; }

    #region Serialization

    /// <summary>2014-08-03 initial version.</summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FixedColumnWidthWithoutTabSeparationStrategy), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (FixedColumnWidthWithoutTabSeparationStrategy)obj;
        info.AddArray("StartPositions", s.StartPositions.ToArray(), s.StartPositions.Length);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        info.GetArray("StartPositions", out int[] startPositions);

        return o is FixedColumnWidthWithoutTabSeparationStrategy s ?
          s with { StartPositions = startPositions.ToImmutableArray() } :
          new FixedColumnWidthWithoutTabSeparationStrategy(startPositions);
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="FixedColumnWidthWithoutTabSeparationStrategy"/> class with an empty list of start positions.
    /// </summary>
    public FixedColumnWidthWithoutTabSeparationStrategy()
      : this(new int[] { })
    {
    }

    /// <summary>
    /// Constructor for this strategy. You must provide the start positions of the tokens. The first token implicitely has position 0.
    /// </summary>
    /// <param name="startPositions">List of starting positions.</param>
    public FixedColumnWidthWithoutTabSeparationStrategy(IReadOnlyList<int> startPositions)
    {
      StartPositions = startPositions.ToImmutableArray();
    }



    public IEnumerable<string> GetTokens(string line)
    {
      int len = line.Length;
      int stringPos = 0;
      for (int i = 0; i <= StartPositions.Length; i++)
      {
        int startStringPos = stringPos;
        stringPos = i < StartPositions.Length ? StartPositions[i] : len;

        if (stringPos < len)
        {
          yield return line.Substring(startStringPos, stringPos - startStringPos);
        }
        else if (stringPos >= len && startStringPos < len)
        {
          yield return line.Substring(startStringPos, len - startStringPos);
          break;
        }
        else
        {
          break;
        }
      }
    }

    public override string ToString()
    {
      return $"{GetType().Name}:StartPos={StartPositions}";
    }

  }
}

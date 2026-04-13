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
    /// <summary>
    /// Gets the start positions of tokens (beginning with the second token).
    /// </summary>
    /// <remarks>
    /// The start position of the first token is implicitly 0.
    /// </remarks>
    public ImmutableArray<int> StartPositions { get; init; }

    #region Serialization

    /// <summary>
    /// V0: 2014-08-03 initial version.
    /// V1: 2026-03-13 Moved from AltaxoBase to AltaxoCore
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Serialization.Ascii.FixedColumnWidthWithoutTabSeparationStrategy", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FixedColumnWidthWithoutTabSeparationStrategy), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (FixedColumnWidthWithoutTabSeparationStrategy)o;
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



    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public override string ToString()
    {
      return $"{GetType().Name}:StartPos={StartPositions}";
    }

  }
}

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
using System.Collections.Immutable;
using System.Linq;

namespace Altaxo.Serialization.Ascii
{
  /// <summary>
  /// This strategy assumes that the tokens fill the printout (!) at fixed positions and have a fixed length.
  /// For the printout position, we have to assume a certain tabulator with. Each tabulator char in the string advances the printout position by a certain amount depending on the current printout
  /// position.
  /// The starting printout position of the first token is always zero. The starting printout positions of each subsequent token (beginning with the second token) has to be provided in the constructor.
  /// </summary>
  /// <remarks>For a tab width of 1, this strategy is identical to the <see cref="FixedColumnWidthWithoutTabSeparationStrategy" />.</remarks>
  public record FixedColumnWidthWithTabSeparationStrategy : IAsciiSeparationStrategy
  {
    public ImmutableArray<int> StartPositions { get; init; }
    public int TabSize { get; init; }

    #region Serialization

    /// <summary>2014-08-03 initial version.</summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FixedColumnWidthWithTabSeparationStrategy), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (FixedColumnWidthWithTabSeparationStrategy)obj;
        info.AddArray("StartPositions", s.StartPositions.ToArray(), s.StartPositions.Length);
        info.AddValue("TabSize", s.TabSize);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        info.GetArray("StartPositions", out int[] startPositions);
        var tabSize = info.GetInt32("TabSize");
        return o is FixedColumnWidthWithTabSeparationStrategy s ?
          s with { StartPositions = startPositions.ToImmutableArray(), TabSize = tabSize } :
          new FixedColumnWidthWithTabSeparationStrategy(startPositions, tabSize);
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="FixedColumnWidthWithTabSeparationStrategy"/> class with a TabSize of 8 and an empty list of start positions.
    /// </summary>
    public FixedColumnWidthWithTabSeparationStrategy()
      : this(new int[] { }, 8)
    {
    }

    /// <summary>
    /// Constructor for this strategy. You must provide the start positions of the tokens. The first token implicitely has position 0.
    /// Furthermore you must provide a tab size that is used to calculate the tabbed positions.
    /// </summary>
    /// <param name="startPositions">List of starting tabbed positions.</param>
    /// <param name="tabSize">Size of the tabulator (i.e. how many spaces at maximum substitutes one tabulator)</param>
    public FixedColumnWidthWithTabSeparationStrategy(IReadOnlyList<int> startPositions, int tabSize)
    {
      if (tabSize < 1)
        throw new ArgumentOutOfRangeException("TabSize have to be >=1");

      StartPositions = startPositions.ToImmutableArray();
      TabSize = tabSize;
    }

    public IEnumerable<string> GetTokens(string line)
    {
      int len = line.Length;
      int stringPos = 0;
      int tabbedPos = 0;

      for (int i = 0; i < StartPositions.Length; i++)
      {
        int startStringPos = stringPos;
        int tabbedEndPos = StartPositions[i];

        // now we have to look for the string position corresponding to the tabbedend
        for (; (tabbedPos < tabbedEndPos) && (stringPos < len); stringPos++)
        {
          if (line[stringPos] == '\t')
            tabbedPos += TabSize - (tabbedPos % TabSize);
          else
            tabbedPos++;
        }

        if ((stringPos - startStringPos) > 0)
          yield return line.Substring(startStringPos, stringPos - startStringPos);
      }

      if (line.Length - stringPos > 0)
        yield return line.Substring(stringPos, line.Length - stringPos);
    }

    public override string ToString()
    {
      return $"{GetType().Name}:TabSize={TabSize}:StartPos={StartPositions}";
    }
  }
}

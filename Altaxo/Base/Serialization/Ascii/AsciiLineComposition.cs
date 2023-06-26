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

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Altaxo.Serialization.Ascii
{
  /// <summary>
  /// Represents the structure of one single line of imported ascii text.
  /// </summary>
  public class AsciiLineComposition : Main.IImmutable, IEquatable<AsciiLineComposition>
  {
    /// <summary>
    /// The structure of the line. This list holds <see cref="System.Type" /> values that represent the recognized items in the line.
    /// </summary>
    public ImmutableArray<AsciiColumnInfo> Columns { get; } = ImmutableArray<AsciiColumnInfo>.Empty;
    public string ShortCuts { get; }

    protected int _hashValue;
    protected int _priorityValue;

    public AsciiLineComposition(ImmutableArray<AsciiColumnInfo> columns)
    {
      Columns = columns;
      var stb = new StringBuilder(columns.Length);
      for (int i = 0; i < columns.Length; ++i)
      {
        stb.Append(columns[i].ShortCut);
      }
      ShortCuts = stb.ToString();
      _hashValue = ShortCuts.GetHashCode();
      _priorityValue = Columns.Sum(x => x.ScoreValue);
    }

    public AsciiLineComposition(IEnumerable<AsciiColumnType> columnTypes, int? count)
    {
      var b = count.HasValue ? ImmutableArray.CreateBuilder<AsciiColumnInfo>(count.Value) : ImmutableArray.CreateBuilder<AsciiColumnInfo>();
      int priorityValue = 0;
      foreach (var columnType in columnTypes)
      {
        var columnInfo = AsciiColumnInfo.ColumnTypeToColumnInfo[columnType];
        b.Add(columnInfo);
        priorityValue += columnInfo.ScoreValue;
      }
      Columns = b.ToImmutable();
      var stb = new StringBuilder(Columns.Length);
      for (int i = 0; i < Columns.Length; ++i)
      {
        stb.Append(Columns[i].ShortCut);
      }
      ShortCuts = stb.ToString();
      _hashValue = ShortCuts.GetHashCode(); _priorityValue = priorityValue;
    }


    public int LineStructureScoring => _priorityValue;

    #region Serialization

    /// <summary>
    /// 2014-08-03 initial version.
    /// 2023-06-26 Renane AsciiLineStructure to AsciiLineComposition
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Serialization.Ascii.AsciiLineStructure", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AsciiLineComposition), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AsciiLineComposition)obj;

        info.CreateArray("ColumnTypes", s.Columns.Length);
        for (int i = 0; i < s.Columns.Length; ++i)
        {
          info.AddEnum("e", s.Columns[i].ColumnType);
        }
        info.CommitArray();
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var count = info.OpenArray("ColumnTypes");
        var b = new AsciiColumnType[count];
        for (int i = 0; i < count; ++i)
        {
          b[i] = (AsciiColumnType)info.GetEnum("e", typeof(AsciiColumnType));
        }
        info.CloseArray(count);
        return new AsciiLineComposition(b, b.Length);
      }
    }

    #endregion Serialization


    /// <summary>
    /// Number of recognized items in the line.
    /// </summary>
    public int Count
    {
      get
      {
        return Columns.Length;
      }
    }

    /// <summary>
    /// Getter / setter of the items of the line.
    /// </summary>
    public AsciiColumnInfo this[int i]
    {
      get
      {
        return Columns[i];
      }
    }

    public override int GetHashCode()
    {
      return _hashValue;
    }

    public bool Equals(AsciiLineComposition? other)
    {
      return
        (other is { } o) &&
        (ShortCuts == o.ShortCuts);
    }

    public override bool Equals(object? other)
    {
      return
        (other is AsciiLineComposition o) &&
        (ShortCuts == o.ShortCuts);
    }

    /// <summary>
    /// Determines whether this line structure is is compatible with another line structure.
    /// </summary>
    /// <param name="ano">The other line structure to compare with.</param>
    /// <returns><c>True</c> if this line structure is compatible with the line structure specified in <paramref name="ano"/>; otherwise, <c>false</c>.
    /// It is compatible if the values of all columns of this line structure could be stored in the columns specified by the other line structure.
    /// </returns>
    public bool IsCompatibleWith(AsciiLineComposition ano)
    {
      // our structure can have more columns, but not lesser than ano
      if (Count < ano.Count)
        return false;

      for (int i = 0; i < ano.Count; i++)
      {
        if (!IsCompatibleWith(Columns[i].ColumnType, ano.Columns[i].ColumnType))
          return false;
      }
      return true;
    }

    /// <summary>
    /// Determines whether the <see cref="AsciiColumnType"/> <paramref name="a"/> is compatible with <paramref name="b"/>.
    /// </summary>
    /// <param name="a">First column type.</param>
    /// <param name="b">Second column type.</param>
    /// <returns><c>True</c> if  <see cref="AsciiColumnType"/> <paramref name="a"/> is compatible with <paramref name="b"/>, i.e. values of type <paramref name="a"/> could be stored in columns of type <paramref name="b"/>; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// <para>The column type <see cref="AsciiColumnType.DBNull"/> is compatible to all other column types.</para>
    /// <para>Since all numeric types will be stored in Double format, all numeric column types are compatible with each other.</para>
    /// </remarks>
    public static bool IsCompatibleWith(AsciiColumnType a, AsciiColumnType b)
    {
      if (a == AsciiColumnType.DBNull || b == AsciiColumnType.DBNull)
        return true;

      if ((a == AsciiColumnType.Double || a == AsciiColumnType.Int64) &&
        (b == AsciiColumnType.Double || b == AsciiColumnType.Int64))
        return true;

      return a == b;
    }

    public override string ToString()
    {
      var stb = new StringBuilder();

      stb.AppendFormat("C={0} H={1:X8}", Count, GetHashCode());
      foreach (var entry in Columns)
      {
        stb.Append(' ');
        stb.Append(entry.ShortCut);
      }
      return stb.ToString();
    }


  } // end class AsciiLineStructure
}

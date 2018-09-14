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

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Serialization.Ascii
{
  public enum AsciiColumnType
  {
    DBNull,
    Int64,
    Double,
    DateTime,
    Text
  }

  public struct AsciiColumnInfo
  {
    private AsciiColumnType _columnType;
    private int _scoreValue;
    private char _shortCut;

    private AsciiColumnInfo(AsciiColumnType t, int scoreValue, char shortCut)
    {
      _columnType = t;
      _scoreValue = scoreValue;
      _shortCut = shortCut;
    }

    public AsciiColumnType ColumnType { get { return _columnType; } }

    public int ScoreValue { get { return _scoreValue; } }

    public char ShortCut { get { return _shortCut; } }

    static AsciiColumnInfo()
    {
      _instDBNull = new AsciiColumnInfo(AsciiColumnType.DBNull, 1, 'N');
      _instText = new AsciiColumnInfo(AsciiColumnType.Text, 2, 'T');
      _instFloatWithDecimalSeparator = new AsciiColumnInfo(AsciiColumnType.Double, 8, 'F');
      _instFloatWithoutDecimalSeparator = new AsciiColumnInfo(AsciiColumnType.Double, 4, 'E');
      _instInteger = new AsciiColumnInfo(AsciiColumnType.Int64, 3, 'I');
      _instGeneralNumber = new AsciiColumnInfo(AsciiColumnType.Double, 3, 'N');
      _instDateTime = new AsciiColumnInfo(AsciiColumnType.DateTime, 17, 'D');
    }

    private static AsciiColumnInfo _instDBNull;

    public static AsciiColumnInfo DBNull { get { return _instDBNull; } }

    private static AsciiColumnInfo _instText;

    public static AsciiColumnInfo Text { get { return _instText; } }

    private static AsciiColumnInfo _instFloatWithDecimalSeparator;

    public static AsciiColumnInfo FloatWithDecimalSeparator { get { return _instFloatWithDecimalSeparator; } }

    private static AsciiColumnInfo _instFloatWithoutDecimalSeparator;

    public static AsciiColumnInfo FloatWithoutDecimalSeparator { get { return _instFloatWithoutDecimalSeparator; } }

    private static AsciiColumnInfo _instInteger;

    public static AsciiColumnInfo Integer { get { return _instInteger; } }

    private static AsciiColumnInfo _instGeneralNumber;

    public static AsciiColumnInfo GeneralNumber { get { return _instGeneralNumber; } }

    private static AsciiColumnInfo _instDateTime;

    public static AsciiColumnInfo DateTime { get { return _instDateTime; } }
  }

  /// <summary>
  /// Represents the structure of one single line of imported ascii text.
  /// </summary>
  public class AsciiLineStructure : IList<AsciiColumnInfo>
  {
    #region Inner items

    private class CollectionWrapper : ICollection<AsciiColumnType>
    {
      private AsciiLineStructure _parent;

      public CollectionWrapper(AsciiLineStructure parent)
      {
        _parent = parent;
      }

      public void Add(AsciiColumnType item)
      {
        switch (item)
        {
          case AsciiColumnType.DBNull:
            _parent.Add(AsciiColumnInfo.DBNull);
            break;

          case AsciiColumnType.Int64:
            _parent.Add(AsciiColumnInfo.Integer);
            break;

          case AsciiColumnType.Double:
            _parent.Add(AsciiColumnInfo.FloatWithDecimalSeparator);
            break;

          case AsciiColumnType.DateTime:
            _parent.Add(AsciiColumnInfo.DateTime);
            break;

          case AsciiColumnType.Text:
            _parent.Add(AsciiColumnInfo.Text);
            break;

          default:
            throw new ArgumentOutOfRangeException("item");
        }
      }

      public void Clear()
      {
        _parent.Clear();
      }

      public bool Contains(AsciiColumnType item)
      {
        throw new NotImplementedException();
      }

      public void CopyTo(AsciiColumnType[] array, int arrayIndex)
      {
        throw new NotImplementedException();
      }

      public int Count
      {
        get { return _parent.Count; }
      }

      public bool IsReadOnly
      {
        get { return false; }
      }

      public bool Remove(AsciiColumnType item)
      {
        throw new NotImplementedException();
      }

      public IEnumerator<AsciiColumnType> GetEnumerator()
      {
        foreach (var entry in _parent._recognizedTypes)
          yield return entry.ColumnType;
      }

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
        foreach (var entry in _parent._recognizedTypes)
          yield return entry.ColumnType;
      }
    }

    #endregion Inner items

    /// <summary>
    /// The structure of the line. This list holds <see cref="System.Type" /> values that represent the recognized items in the line.
    /// </summary>
    protected List<AsciiColumnInfo> _recognizedTypes = new List<AsciiColumnInfo>();

    private CollectionWrapper _columnTypeCollectionWrapper;

    /// <summary>
    /// If true, the cached data in this class are invalid and needs to be recalculated.
    /// </summary>
    protected bool _isCachedDataInvalid = true;

    protected int _priorityValue;
    protected int _hashValue;

    #region Serialization

    /// <summary>2014-08-03 initial version.</summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AsciiLineStructure), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AsciiLineStructure)obj;

        info.CreateArray("ColumnTypes", s._recognizedTypes.Count);
        for (int i = 0; i < s._recognizedTypes.Count; ++i)
        {
          info.AddEnum("e", s._recognizedTypes[i].ColumnType);
        }
        info.CommitArray();
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = (o == null ? new AsciiLineStructure() : (AsciiLineStructure)o);
        var count = info.OpenArray("ColumnTypes");
        for (int i = 0; i < count; ++i)
        {
          s.ColumnTypes.Add((AsciiColumnType)info.GetEnum("e", typeof(AsciiColumnType)));
        }
        info.CloseArray(count);

        return s;
      }
    }

    #endregion Serialization

    public ICollection<AsciiColumnType> ColumnTypes
    {
      get
      {
        if (null == _columnTypeCollectionWrapper)
          _columnTypeCollectionWrapper = new CollectionWrapper(this);
        return _columnTypeCollectionWrapper;
      }
    }

    /// <summary>
    /// Number of recognized items in the line.
    /// </summary>
    public int Count
    {
      get
      {
        return _recognizedTypes.Count;
      }
    }

    /// <summary>
    /// Adds a recognized item.
    /// </summary>
    /// <param name="o">The recognized item represented by its type, i.e. typeof(double) represents a recognized double number.</param>
    public void Add(AsciiColumnInfo o)
    {
      _recognizedTypes.Add(o);
      _isCachedDataInvalid = true;
    }

    /// <summary>
    /// Getter / setter of the items of the line.
    /// </summary>
    public AsciiColumnInfo this[int i]
    {
      get
      {
        return _recognizedTypes[i];
      }
      set
      {
        _recognizedTypes[i] = value;
        _isCachedDataInvalid = true;
      }
    }

    public int LineStructureScoring
    {
      get
      {
        if (_isCachedDataInvalid)
          CalculateCachedData();
        return _priorityValue;
      }
    }

    protected void CalculateCachedData()
    {
      // Calculate priority and hash
      var stb = new StringBuilder(); // for calculating the hash
      stb.Append(Count.ToString());

      int priorityValue = 0;
      foreach (var entry in _recognizedTypes)
      {
        priorityValue += entry.ScoreValue;
        stb.Append(entry.ShortCut);
      } // foreach

      _priorityValue = priorityValue;
      // calculate hash
      _hashValue = stb.ToString().GetHashCode();

      _isCachedDataInvalid = false;
    }

    public override int GetHashCode()
    {
      if (_isCachedDataInvalid)
        CalculateCachedData();
      return _hashValue;
    }

    /// <summary>
    /// Determines whether this line structure is is compatible with another line structure.
    /// </summary>
    /// <param name="ano">The other line structure to compare with.</param>
    /// <returns><c>True</c> if this line structure is compatible with the line structure specified in <paramref name="ano"/>; otherwise, <c>false</c>.
    /// It is compatible if the values of all columns of this line structure could be stored in the columns specified by the other line structure.
    /// </returns>
    public bool IsCompatibleWith(AsciiLineStructure ano)
    {
      // our structure can have more columns, but not lesser than ano
      if (Count < ano.Count)
        return false;

      for (int i = 0; i < ano.Count; i++)
      {
        if (!IsCompatibleWith(_recognizedTypes[i].ColumnType, ano._recognizedTypes[i].ColumnType))
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
      foreach (var entry in _recognizedTypes)
      {
        stb.Append(' ');
        stb.Append(entry.ShortCut);
      }
      return stb.ToString();
    }

    public int IndexOf(AsciiColumnInfo item)
    {
      return _recognizedTypes.IndexOf(item);
    }

    public void Insert(int index, AsciiColumnInfo item)
    {
      _recognizedTypes.Insert(index, item);
      _isCachedDataInvalid = true;
    }

    public void RemoveAt(int index)
    {
      _recognizedTypes.RemoveAt(index);
      _isCachedDataInvalid = true;
    }

    public void Clear()
    {
      _recognizedTypes.Clear();
      _isCachedDataInvalid = true;
    }

    public bool Contains(AsciiColumnInfo item)
    {
      return _recognizedTypes.Contains(item);
    }

    public void CopyTo(AsciiColumnInfo[] array, int arrayIndex)
    {
      _recognizedTypes.CopyTo(array, arrayIndex);
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    public bool Remove(AsciiColumnInfo item)
    {
      var r = _recognizedTypes.Remove(item);
      if (r)
        _isCachedDataInvalid = true;
      return r;
    }

    public IEnumerator<AsciiColumnInfo> GetEnumerator()
    {
      return _recognizedTypes.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _recognizedTypes.GetEnumerator();
    }
  } // end class AsciiLineStructure

  /// <summary>
  /// Helper structure to count how many lines have the same structure.
  /// </summary>
  public struct NumberAndStructure
  {
    /// <summary>Number of lines that have the same structure.</summary>
    public int NumberOfLines;

    /// <summary>The structure that these lines have.</summary>
    public AsciiLineStructure LineStructure;
  } // end class
}

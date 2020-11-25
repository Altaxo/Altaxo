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
using System.Collections.Generic;

namespace Altaxo.Gui.Common.MultiRename
{
  /// <summary>
  /// Extends the parser generated from the PEG grammar, so that the keywords are no longer fixed, but can be stored into collections.
  /// </summary>
  public class MultiRenameParser : MultiRenameParserBase
  {
    #region specific string comparer

    /// <summary>
    /// Specific comparer, that orders string so that the longest strings come first. This is essential for looking up of parser literals.
    /// </summary>
    private class LongestFirstComparer : IComparer<string>
    {
      public int Compare(string? x, string? y)
      {
        int lenx = x is null ? 0 : x.Length;
        int leny = y is null ? 0 : y.Length;

        if (lenx != leny) // make sorting so that the longest strings come first
          return lenx < leny ? 1 : -1;
        else
          return string.Compare(x, y);
      }
    }

    #endregion specific string comparer

    private static LongestFirstComparer _comparer = new LongestFirstComparer();
    private SortedSet<string> _integerTChars;
    private SortedSet<string> _stringTChars;
    private SortedSet<string> _arrayTChars;
    private SortedSet<string> _dateTimeTChars;

    /// <summary>
    /// Contain all keywords that can be used as integer number, like counter, row number and so on.
    /// </summary>
    /// <example>
    /// If [C] and [R] can be used to insert a counter and a row number, then the set contains the keywords C and K.
    /// </example>
    public SortedSet<string> IntegerTChars
    {
      get { return _integerTChars; }
    }

    /// <summary>
    /// Contain all keywords that can be used as strings, like name.
    /// </summary>
    /// <example>
    /// If [N]  can be used to insert the item name, then this set contains the keyword N.
    /// </example>
    public SortedSet<string> StringTChars
    {
      get { return _stringTChars; }
    }

    /// <summary>
    /// Contain all keywords that can be used as arrays, like for instance the path parts, which can be considered as an array of strings.
    /// </summary>
    /// <example>
    /// If [P"\\"]  can be used to insert the path with a backslash between the path parts, then this set contains the keyword P.
    /// </example>
    public SortedSet<string> ArrayTChars
    {
      get { return _arrayTChars; }
    }

    /// <summary>
    /// Contain all keywords that can be used as DateTime values, like for instance the creation date of an item.
    /// </summary>
    /// <example>
    /// If [CD]  can be used to insert the creation date, then this set contains the keyword CD.
    /// </example>
    public SortedSet<string> DateTimeTChars
    {
      get { return _dateTimeTChars; }
    }

    public MultiRenameParser(MultiRenameData renameData)
      : base()
    {
      _integerTChars = new SortedSet<string>(renameData.GetIntegerShortcuts(), _comparer);
      _stringTChars = new SortedSet<string>(renameData.GetStringShortcuts(), _comparer);
      _arrayTChars = new SortedSet<string>(renameData.GetArrayShortcuts(), _comparer);
      _dateTimeTChars = new SortedSet<string>(renameData.GetDateTimeShortcuts(), _comparer);
    }

    private bool IsOneOf(SortedSet<string> list)
    {
      foreach (string s in list)
      {
        if (Char(s))
          return true;
      }
      return false;
    }

    public override bool IntegerTChar()
    {
      return TreeAST((int)EAltaxo_MultiRename.StringTChar, () => IsOneOf(_integerTChars));
    }

    public override bool StringTChar()
    {
      return TreeAST((int)EAltaxo_MultiRename.StringTChar, () => IsOneOf(_stringTChars));
    }

    public override bool ArrayTChar()
    {
      return TreeAST((int)EAltaxo_MultiRename.StringTChar, () => IsOneOf(_arrayTChars));
    }

    public override bool DateTimeTChar()
    {
      return TreeAST((int)EAltaxo_MultiRename.StringTChar, () => IsOneOf(_dateTimeTChars));
    }
  }
}

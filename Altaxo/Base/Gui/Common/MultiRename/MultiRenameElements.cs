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
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Gui.Common.MultiRename
{
  /// <summary>
  /// Represents one element of a multi-rename template.
  /// </summary>
  public interface IMultiRenameElement
  {
    /// <summary>Retrieves the content for this element by providing the original object index of the object and a sort order.</summary>
    /// <param name="originalListIndex">Index in the original object list of <see cref="MultiRenameData"/>.</param>
    /// <param name="currentSortIndex">Index of the current sort order (for instance when sorted in a view).</param>
    /// <returns>The string content of this element.</returns>
    public string GetContent(int originalListIndex, int currentSortIndex);
  }

  /// <summary>
  /// Base class for multi-rename elements that access shortcut-based rename data.
  /// </summary>
  public abstract class MultiRenameBaseElement : IMultiRenameElement
  {
    /// <summary>
    /// The rename data used by this element.
    /// </summary>
    protected MultiRenameData _renameData;

    /// <summary>
    /// The shortcut that identifies the data source.
    /// </summary>
    protected string _shortCut;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiRenameBaseElement"/> class.
    /// </summary>
    /// <param name="multiRenameData">The rename data.</param>
    /// <param name="shortCut">The shortcut identifying the source value.</param>
    public MultiRenameBaseElement(MultiRenameData multiRenameData, string shortCut)
    {
      _renameData = multiRenameData;
      _shortCut = shortCut;
    }

    /// <summary>Retrieves the content for this element by providing the original object index of the object and a sort order.</summary>
    /// <param name="originalListIndex">Index in the original object list of <see cref="MultiRenameData"/>.</param>
    /// <param name="currentSortIndex">Index of the current sort order (for instance when sorted in a view).</param>
    /// <returns>The string content of this element.</returns>
    public abstract string GetContent(int originalListIndex, int currentSortIndex);
  }

  /// <summary>
  /// Represents a literal text element in a multi-rename template.
  /// </summary>
  public class MultiRenameLiteralElement : IMultiRenameElement
  {
    private string _literal;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiRenameLiteralElement"/> class.
    /// </summary>
    /// <param name="literal">The literal text.</param>
    public MultiRenameLiteralElement(string literal)
    {
      _literal = literal;
    }

    /// <inheritdoc/>
    public string GetContent(int originalListIndex, int currentSortIndex)
    {
      return _literal;
    }
  }

  /// <summary>
  /// Represents a collection of multi-rename elements.
  /// </summary>
  public class MultiRenameElementCollection : IMultiRenameElement
  {
    private List<IMultiRenameElement> _elements = new List<IMultiRenameElement>();

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiRenameElementCollection"/> class.
    /// </summary>
    public MultiRenameElementCollection()
    {
    }

    /// <summary>
    /// Gets the contained elements.
    /// </summary>
    public List<IMultiRenameElement> Elements { get { return _elements; } }

    /// <inheritdoc/>
    public string GetContent(int originalListIndex, int currentSortIndex)
    {
      var stb = new StringBuilder();
      foreach (var ele in _elements)
      {
        stb.Append(ele.GetContent(originalListIndex, currentSortIndex));
      }

      return stb.ToString();
    }
  }

  /// <summary>
  /// Represents a string element with substring extraction.
  /// </summary>
  public class MultiRenameStringElement : MultiRenameBaseElement
  {
    private int _start, _last;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiRenameStringElement"/> class.
    /// </summary>
    public MultiRenameStringElement(MultiRenameData multiRenameData, string shortCut, int start, int last)
      : base(multiRenameData, shortCut)
    {
      _start = start;
      _last = last;
    }

    /// <inheritdoc/>
    public override string GetContent(int originalListIndex, int currentSortIndex)
    {
      string stringValue = _renameData.GetStringValueOfShortcut(_shortCut, originalListIndex, currentSortIndex);
      if (string.IsNullOrEmpty(stringValue))
        return string.Empty;

      int start = _start;
      int last = _last;

      if (start < 0)
        start += stringValue.Length;
      if (last < 0)
        last += stringValue.Length;

      start = Math.Max(start, 0);
      last = Math.Max(last, 0);
      start = Math.Min(start, stringValue.Length - 1);
      last = Math.Min(last, stringValue.Length - 1);

      if (start <= last)
        return stringValue.Substring(start, last - start + 1);
      else
        return string.Empty;
    }
  }

  /// <summary>
  /// Represents an integer element with formatting and offset logic.
  /// </summary>
  public class MultiRenameIntegerElement : MultiRenameBaseElement
  {
    private int _numberOfDigits;
    private int _offset;
    private int _step;
    private string _formatString;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiRenameIntegerElement"/> class.
    /// </summary>
    public MultiRenameIntegerElement(MultiRenameData multiRenameData, string shortCut, int numberOfDigits, int offset, int step)
      : base(multiRenameData, shortCut)
    {
      _numberOfDigits = numberOfDigits;
      _offset = offset;
      _step = step;
      _formatString = "D" + numberOfDigits.ToString();
    }

    /// <inheritdoc/>
    /// <inheritdoc/>
    public override string GetContent(int originalListIndex, int currentSortIndex)
    {
      int val = _renameData.GetIntegerValueOfShortcut(_shortCut, originalListIndex, currentSortIndex);

      int result = val * _step + _offset;

      if (_numberOfDigits <= 0)
      {
        return result.ToString();
      }
      else
      {
        return result.ToString(_formatString);
      }
    }
  }

  /// <summary>
  /// Represents a date-time element with formatting options.
  /// </summary>
  public class MultiRenameDateTimeElement : MultiRenameBaseElement
  {
    private const string DefaultDateTimeFormat = "yyyy-dd-MM HH-mm-ss";
    private string _dateTimeFormat;
    private bool _useUtcTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiRenameDateTimeElement"/> class.
    /// </summary>
    public MultiRenameDateTimeElement(MultiRenameData multiRenameData, string shortCut, string? dateTimeFormat, bool useUtcTime)
      : base(multiRenameData, shortCut)
    {
      _dateTimeFormat = string.IsNullOrEmpty(dateTimeFormat) ? DefaultDateTimeFormat : dateTimeFormat;

      _useUtcTime = useUtcTime;
    }

    /// <inheritdoc/>
    public override string GetContent(int originalListIndex, int currentSortIndex)
    {
      var val = _renameData.GetDateTimeValueOfShortcut(_shortCut, originalListIndex, currentSortIndex);
      if (_useUtcTime)
        val = val.ToUniversalTime();
      else
        val = val.ToLocalTime();

      try
      {
        return val.ToString(_dateTimeFormat);
      }
      catch (Exception)
      {
      }

      return val.ToString();
    }
  }

  /// <summary>
  /// Represents an array element with range and separator options.
  /// </summary>
  public class MultiRenameArrayElement : MultiRenameBaseElement
  {
    private int _start, _last;
    private string _separator;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiRenameArrayElement"/> class.
    /// </summary>
    /// <param name="multiRenameData">The rename data provider.</param>
    /// <param name="shortCut">The shortcut that identifies the source array.</param>
    /// <param name="start">The first element index to include.</param>
    /// <param name="last">The last element index to include.</param>
    /// <param name="separator">The separator inserted between the selected elements.</param>
    public MultiRenameArrayElement(MultiRenameData multiRenameData, string shortCut, int start, int last, string separator)
      : base(multiRenameData, shortCut)
    {
      _start = start;
      _last = last;
      _separator = separator;
    }

    /// <inheritdoc/>
    public override string GetContent(int originalListIndex, int currentSortIndex)
    {
      string[] arr = _renameData.GetStringArrayValueOfShortcut(_shortCut, originalListIndex, currentSortIndex);

      var start = _start;
      var last = _last;

      if (start < 0)
        start += arr.Length;
      if (last < 0)
        last += arr.Length;

      start = Math.Max(start, 0);
      last = Math.Max(last, 0);
      start = Math.Min(start, arr.Length - 1);
      last = Math.Min(last, arr.Length - 1);

      if (start <= last)
        return string.Join(_separator, arr, start, last - start + 1);
      else
        return string.Empty;
    }
  }
}

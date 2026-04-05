#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using System.Diagnostics.CodeAnalysis;
using Altaxo.Scripting;

namespace Altaxo.Data
{
  /// <summary>
  /// Collection that maps data columns to their associated column scripts.
  /// </summary>
  public class ColumnScriptCollection
    :
    Main.SuspendableDocumentNodeWithSetOfEventArgs,
    IDictionary<DataColumn, IColumnScriptText>
  {
    /// <summary>
    /// ColumnScripts, key is the corresponding column, value is of type WorksheetColumnScript
    /// </summary>
    protected Dictionary<DataColumn, IColumnScriptText> _innerDict = new Dictionary<DataColumn, IColumnScriptText>();

    /// <inheritdoc />
    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_innerDict is not null)
      {
        foreach (var item in _innerDict)
        {
          yield return new Main.DocumentNodeAndName(item.Value, item.Key.Name);
        }
      }
    }

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
      var d = _innerDict;
      if (d is not null && d.Count > 0)
      {
        _innerDict = new Dictionary<DataColumn, IColumnScriptText>();

        foreach (var item in d)
          item.Value?.Dispose();
      }

      base.Dispose(isDisposing);
    }

    #region IDictionary<DataColumn, IColumnScriptText>

    /// <inheritdoc />
    public void Add(DataColumn key, IColumnScriptText value)
    {
      _innerDict.Add(key, value);
      value.ParentObject = this;
      EhSelfChanged(EventArgs.Empty);
    }

    /// <inheritdoc />
    public bool ContainsKey(DataColumn key)
    {
      return _innerDict.ContainsKey(key);
    }

    /// <inheritdoc />
    public ICollection<DataColumn> Keys
    {
      get { return _innerDict.Keys; }
    }

    /// <inheritdoc />
    public bool Remove(DataColumn key)
    {
      if (_innerDict.TryGetValue(key, out var oldValue))
      {
        _innerDict.Remove(key);
        oldValue.Dispose();
        EhSelfChanged(EventArgs.Empty);
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <inheritdoc />
    public bool TryGetValue(DataColumn key, [MaybeNullWhen(false)] out IColumnScriptText value)
    {
      return _innerDict.TryGetValue(key, out value);
    }

    /// <inheritdoc />
    public ICollection<IColumnScriptText> Values
    {
      get { return _innerDict.Values; }
    }

    /// <inheritdoc />
    public IColumnScriptText this[DataColumn key]
    {
      get
      {
        return _innerDict[key];
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException("value");

        _innerDict.TryGetValue(key, out var oldValue);

        if (object.ReferenceEquals(oldValue, value))
          return;

        _innerDict[key] = value;
        value.ParentObject = this;

        if (oldValue is not null)
          oldValue.Dispose();
      }
    }

    /// <inheritdoc />
    public void Add(KeyValuePair<DataColumn, IColumnScriptText> item)
    {
      Add(item.Key, item.Value);
    }

    /// <inheritdoc />
    public void Clear()
    {
      var d = _innerDict;
      _innerDict = new Dictionary<DataColumn, IColumnScriptText>();

      foreach (var item in d)
      {
        item.Value.Dispose();
      }

      EhSelfChanged(EventArgs.Empty);
    }

    /// <inheritdoc />
    public bool Contains(KeyValuePair<DataColumn, IColumnScriptText> item)
    {
      return _innerDict.ContainsKey(item.Key);
    }

    /// <inheritdoc />
    public void CopyTo(KeyValuePair<DataColumn, IColumnScriptText>[] array, int arrayIndex)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc />
    public int Count
    {
      get { return _innerDict.Count; }
    }

    /// <inheritdoc />
    public bool IsReadOnly
    {
      get { return false; }
    }

    /// <inheritdoc />
    public bool Remove(KeyValuePair<DataColumn, IColumnScriptText> item)
    {
      return Remove(item.Key);
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<DataColumn, IColumnScriptText>> GetEnumerator()
    {
      return _innerDict.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _innerDict.GetEnumerator();
    }

    #endregion IDictionary<DataColumn, IColumnScriptText>
  }
}

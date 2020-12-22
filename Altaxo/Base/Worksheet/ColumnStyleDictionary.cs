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
using System.Diagnostics.CodeAnalysis;
using Altaxo.Data;

namespace Altaxo.Worksheet
{
  public class ColumnStyleDictionary
    :
    Main.SuspendableDocumentNodeWithSetOfEventArgs,
    IDictionary<Data.DataColumn, ColumnStyle>
  {
    /// <summary>Column styles. Key is the column instance, value is the column style.</summary>
    private Dictionary<Data.DataColumn, ColumnStyle> _columnStyles;

    /// <summary>Default column styles. Key is the type of the column, value is the default column style for this type of columns.</summary>
    private Dictionary<System.Type, ColumnStyle> _defaultColumnStyles;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ColumnStyleDictionary), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      protected ColumnStyleDictionary? _deserializedInstance;
      protected Dictionary<Main.AbsoluteDocumentPath, ColumnStyle>? _unresolvedColumns;

      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ColumnStyleDictionary)obj;

        info.CreateArray("DefaultColumnStyles", s._defaultColumnStyles.Count);
        foreach (var style in s._defaultColumnStyles)
        {
          info.CreateElement("e");
          info.AddValue("Type", style.Key.FullName);
          info.AddValue("Style", style.Value);
          info.CommitElement(); // "e"
        }
        info.CommitArray();

        info.CreateArray("ColumnStyles", s._columnStyles.Count);
        foreach (var dictentry in s._columnStyles)
        {
          info.CreateElement("e");
          info.AddValue("Column", Main.AbsoluteDocumentPath.GetAbsolutePath(dictentry.Key));
          info.AddValue("Style", dictentry.Value);
          info.CommitElement(); // "e"
        }
        info.CommitArray();
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        ColumnStyleDictionary s = (ColumnStyleDictionary?)o ?? new ColumnStyleDictionary();
        Deserialize(s, info, parent);
        return s;
      }

      protected virtual void Deserialize(ColumnStyleDictionary s, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var surr = new XmlSerializationSurrogate0
        {
          _unresolvedColumns = new Dictionary<Main.AbsoluteDocumentPath, ColumnStyle>(),
          _deserializedInstance = s
        };
        info.DeserializationFinished += surr.EhDeserializationFinished;

        int count;

        count = info.OpenArray(); // DefaultColumnStyles
        for (int i = 0; i < count; i++)
        {
          info.OpenElement(); // "e"
          string typeName = info.GetString("Type");
          //Type t = Type.ReflectionOnlyGetType(typeName, false, false);
          var t = Type.GetType(typeName, false, false);
          var style = (ColumnStyle)info.GetValue("Style", s);
          style.ParentObject = s;

          if (!(t is null || style is null))
            s._defaultColumnStyles[t] = style;
          info.CloseElement(); // "e"
        }
        info.CloseArray(count);

        // deserialize the columnstyles
        // this must be deserialized in a new instance of this surrogate, since we can not resolve it immediately
        count = info.OpenArray();
        if (count > 0)
        {
          for (int i = 0; i < count; i++)
          {
            info.OpenElement(); // "e"
            var key = (Main.AbsoluteDocumentPath)info.GetValue("Column", s);
            var val = (ColumnStyle)info.GetValue("Style", s);
            surr._unresolvedColumns.Add(key, val);
            info.CloseElement();
          }
        }
        info.CloseArray(count);
      }

      public void EhDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot, bool isFinallyCall)
      {
        var resolvedStyles = new List<Main.AbsoluteDocumentPath>();
        foreach (var entry in _unresolvedColumns!)
        {
          object? resolvedobj = Main.AbsoluteDocumentPath.GetObject(entry.Key, _deserializedInstance!, (Main.IDocumentNode)documentRoot);
          if (!(resolvedobj is null))
          {
            _deserializedInstance!._columnStyles.Add((DataColumn)resolvedobj, entry.Value);
            resolvedStyles.Add(entry.Key);
          }
        }

        foreach (var resstyle in resolvedStyles)
          _unresolvedColumns.Remove(resstyle);

        // if all columns have resolved, we can close the event link
        if (_unresolvedColumns.Count == 0)
          info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(EhDeserializationFinished);
      }
    }

    #endregion Serialization

    public ColumnStyleDictionary()
    {
      _defaultColumnStyles = new Dictionary<Type, ColumnStyle>();
      _columnStyles = new Dictionary<Altaxo.Data.DataColumn, ColumnStyle>();
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      foreach (var entry in _defaultColumnStyles)
        yield return new Main.DocumentNodeAndName(entry.Value, "DefaultColumnStyle_" + entry.Key.FullName);

      foreach (var entry in _columnStyles)
      {
        // TODO we rely on the name of the data column here, but it can already be disposed off
        // instead, we should maintain the name separately in the dictionary
        if (entry.Key.IsDisposeInProgress)
          yield return new Main.DocumentNodeAndName(entry.Value, "ColumnStyle_UnknownName");
        else
          yield return new Main.DocumentNodeAndName(entry.Value, "ColumnStyle_" + entry.Key.FullName);
      }
    }

    protected override void Dispose(bool isDisposing)
    {
      if (isDisposing)
      {
        var tmpDefaultColumnStyles = _defaultColumnStyles;
        var tmpColumnStyles = _columnStyles;

        if (tmpDefaultColumnStyles is not null || tmpColumnStyles is not null)
        {
          _defaultColumnStyles = new Dictionary<Type, ColumnStyle>();
          _columnStyles = new Dictionary<DataColumn, ColumnStyle>();
        }

        if (tmpDefaultColumnStyles is not null)
        {
          foreach (var entry in tmpDefaultColumnStyles)
          {
            entry.Value?.Dispose();
          }
        }

        if (tmpColumnStyles is not null)
        {
          foreach (var entry in tmpColumnStyles)
          {
            if (entry.Key is not null)
              DetachKey(entry.Key);
            if (entry.Value is not null)
              entry.Value.Dispose();
          }
        }
      }
      base.Dispose(isDisposing);
    }

    private void AttachKey(DataColumn key)
    {
      key.TunneledEvent += EhKey_TunneledEvent;
    }

    private void DetachKey(DataColumn key)
    {
      key.TunneledEvent -= EhKey_TunneledEvent;
    }

    private void EhKey_TunneledEvent(object sender, object source, Main.TunnelingEventArgs e)
    {
      if (e is Main.DisposeEventArgs)
      {
        var c = source as DataColumn;
        if (c is not null)
          Remove(c); // do not use direct remove, as the event handler has to be detached also
      }
    }

    public void SetDefaultColumnStyle(System.Type key, ColumnStyle value)
    {
      bool isOldStylePresent = _defaultColumnStyles.TryGetValue(key, out var oldStyle);
      _defaultColumnStyles[key] = value;
      value.ParentObject = this;

      if (isOldStylePresent)
        oldStyle?.Dispose();
    }

    internal Dictionary<System.Type, ColumnStyle> DefaultColumnStyles
    {
      get
      {
        return _defaultColumnStyles;
      }
    }

    #region IDictionary<DataColumn,ColumnStyle> Members

    public void Add(DataColumn key, ColumnStyle value)
    {
      if (value is null)
        throw new ArgumentNullException("value");

      _columnStyles.Add(key, value);
      value.ParentObject = this;
      AttachKey(key);
    }

    public bool ContainsKey(DataColumn key)
    {
      return _columnStyles.ContainsKey(key);
    }

    public ICollection<DataColumn> Keys
    {
      get { return _columnStyles.Keys; }
    }

    public bool Remove(DataColumn key)
    {
      if (TryGetValue(key, out var value))
      {
        _columnStyles.Remove(key);
        DetachKey(key);
        value.Dispose();
        return true;
      }
      else
      {
        return false;
      }
    }

    public bool TryGetValue(DataColumn key, [MaybeNullWhen(false)] out ColumnStyle value)
    {
      return _columnStyles.TryGetValue(key, out value);
    }

    public ICollection<ColumnStyle> Values
    {
      get { return _columnStyles.Values; }
    }

    public ColumnStyle this[DataColumn key]
    {
      get
      {
        // first look at the column styles hash table, column itself is the key
        if (_columnStyles.TryGetValue(key, out var colstyle))
          return colstyle;

        if (_defaultColumnStyles.TryGetValue(key.GetType(), out colstyle))
          return colstyle;

        // second look to the defaultcolumnstyles hash table, key is the type of the column style
        var searchstyletype = key.GetColumnStyleType();
        if (searchstyletype is null)
        {
          throw new ApplicationException("Error: Column of type +" + key.GetType() + " returns no associated ColumnStyleType, you have to overload the method GetColumnStyleType.");
        }
        else
        {
          // if not successfull yet, we will create a new defaultColumnStyle
          colstyle = (ColumnStyle?)Activator.CreateInstance(searchstyletype) ?? throw new InvalidProgramException($"Can not create a instance of type {searchstyletype}. Is there a public constructor?"); ;
          colstyle.ParentObject = this;
          _defaultColumnStyles.Add(key.GetType(), colstyle);
          return colstyle;
        }
      }
      set
      {
        bool hadOldValue = _columnStyles.TryGetValue(key, out var oldStyle);
        _columnStyles[key] = value ?? throw new ArgumentNullException("value");
        value.ParentObject = this;

        if (hadOldValue)
        {
          oldStyle?.Dispose();
        }
        else
        {
          AttachKey(key);
        }
      }
    }

    #endregion IDictionary<DataColumn,ColumnStyle> Members

    #region ICollection<KeyValuePair<DataColumn,ColumnStyle>> Members

    public void Add(KeyValuePair<DataColumn, ColumnStyle> item)
    {
      ((ICollection<KeyValuePair<DataColumn, ColumnStyle>>)_columnStyles).Add(item);
      AttachKey(item.Key);
      item.Value.ParentObject = this;
    }

    public void Clear()
    {
      var columnStyles = _columnStyles;
      _columnStyles = new Dictionary<DataColumn, ColumnStyle>();
      foreach (var entry in columnStyles)
      {
        entry.Value.Dispose();
        DetachKey(entry.Key);
      }
    }

    public bool Contains(KeyValuePair<DataColumn, ColumnStyle> item)
    {
      return ((ICollection<KeyValuePair<DataColumn, ColumnStyle>>)_columnStyles).Contains(item);
    }

    public void CopyTo(KeyValuePair<DataColumn, ColumnStyle>[] array, int arrayIndex)
    {
      ((ICollection<KeyValuePair<DataColumn, ColumnStyle>>)_columnStyles).CopyTo(array, arrayIndex);
    }

    public int Count
    {
      get { return _columnStyles.Count; }
    }

    public bool IsReadOnly
    {
      get { return ((ICollection<KeyValuePair<DataColumn, ColumnStyle>>)_columnStyles).IsReadOnly; }
    }

    public bool Remove(KeyValuePair<DataColumn, ColumnStyle> item)
    {
      bool result = ((ICollection<KeyValuePair<DataColumn, ColumnStyle>>)_columnStyles).Remove(item);
      if (result)
      {
        DetachKey(item.Key);
        item.Value.Dispose();
      }
      return result;
    }

    #endregion ICollection<KeyValuePair<DataColumn,ColumnStyle>> Members

    #region IEnumerable<KeyValuePair<DataColumn,ColumnStyle>> Members

    public IEnumerator<KeyValuePair<DataColumn, ColumnStyle>> GetEnumerator()
    {
      return ((ICollection<KeyValuePair<DataColumn, ColumnStyle>>)_columnStyles).GetEnumerator();
    }

    #endregion IEnumerable<KeyValuePair<DataColumn,ColumnStyle>> Members

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return ((ICollection<KeyValuePair<DataColumn, ColumnStyle>>)_columnStyles).GetEnumerator();
    }

    #endregion IEnumerable Members
  }
}

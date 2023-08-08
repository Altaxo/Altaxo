#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Altaxo.Main.Properties;

namespace Altaxo.Gui.Units
{
  /// <summary>
  /// Collection of unit environments defined by the user.
  /// Key is a quantity that is not neccessarily a unit quantity (for instance: unit quanity is 'Length' but quantities can be 'CapSize', 'LineThickness' etc).
  /// Value is the user defined unit environment for that quantity.
  /// </summary>
  public class UserDefinedUnitEnvironments : IDictionary<string, UserDefinedUnitEnvironment>, INotifyCollectionChanged
  {
    private IDictionary<string, UserDefinedUnitEnvironment> _dictionary;

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>
    /// The property key used to store the default instance of this class in the property service.
    /// </summary>
    public static readonly PropertyKey<UserDefinedUnitEnvironments> PropertyKeyDefaultInstance =
     new PropertyKey<UserDefinedUnitEnvironments>(
     "DC4BAAC5-440E-46CA-9A63-1747FAFFB32C",
     "Units\\UserDefinedUnitEnvironments",
     PropertyLevel.Application,
     () => new UserDefinedUnitEnvironments());


    #region Serialization

    /// <summary>
    /// 2017-09-26 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(UserDefinedUnitEnvironments), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (UserDefinedUnitEnvironments)obj;

        info.CreateArray("Environments", s.Count);
        foreach (var env in s.Values)
          info.AddValue("e", env);
        info.CommitArray();
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (UserDefinedUnitEnvironments?)o ?? new UserDefinedUnitEnvironments(info);

        var count = info.OpenArray("Environments");
        for (int i = 0; i < count; ++i)
        {
          var env = (UserDefinedUnitEnvironment)info.GetValue("e", null);
          s.Add(env.Name, env);
        }
        info.CloseArray(count);

        return s;
      }
    }

    #endregion Serialization

    protected UserDefinedUnitEnvironments(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      _dictionary = new Dictionary<string, UserDefinedUnitEnvironment>();
    }

    public UserDefinedUnitEnvironments()
    {
      _dictionary = new Dictionary<string, UserDefinedUnitEnvironment>();
    }

    private void OnContentsChanged()
    {
      CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public UserDefinedUnitEnvironment this[string key] { get => _dictionary[key]; set => _dictionary[key] = value; }

    public ICollection<string> Keys => _dictionary.Keys;

    public ICollection<UserDefinedUnitEnvironment> Values => _dictionary.Values;

    public int Count => _dictionary.Count;

    public bool IsReadOnly => _dictionary.IsReadOnly;

    public void Add(string key, UserDefinedUnitEnvironment value)
    {
      _dictionary.Add(key, value);
      OnContentsChanged();
    }

    public void Add(KeyValuePair<string, UserDefinedUnitEnvironment> item)
    {
      _dictionary.Add(item);
      OnContentsChanged();
    }

    public void Clear()
    {
      _dictionary.Clear();
      OnContentsChanged();
    }

    public bool Contains(KeyValuePair<string, UserDefinedUnitEnvironment> item)
    {
      return _dictionary.Contains(item);
    }

    public bool ContainsKey(string key)
    {
      return _dictionary.ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<string, UserDefinedUnitEnvironment>[] array, int arrayIndex)
    {
      _dictionary.CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<string, UserDefinedUnitEnvironment>> GetEnumerator()
    {
      return _dictionary.GetEnumerator();
    }

    public bool Remove(string key)
    {
      var success = _dictionary.Remove(key);
      if (success)
        OnContentsChanged();
      return success;
    }

    public bool Remove(KeyValuePair<string, UserDefinedUnitEnvironment> item)
    {
      var success = _dictionary.Remove(item);
      if (success)
        OnContentsChanged();
      return success;
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out UserDefinedUnitEnvironment value)
    {
      return _dictionary.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _dictionary.GetEnumerator();
    }
  }
}

#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;
using Altaxo.Serialization.Xml;

namespace Altaxo.Main.Properties
{
  /// <summary>
  /// Default implementation of a property bag <see cref="IPropertyBag"/>.
  /// </summary>
  public class PropertyBag
    :
    Main.SuspendableDocumentNodeWithEventArgs,
    IPropertyBag
  {
    /// <summary>
    /// Dictionary that hold the properties. Key is the Guid of the property key (or any other string). Value is the property value.
    /// </summary>
    protected Dictionary<string, object?> _properties;

    /// <summary>
    /// The properties lazy loaded. All keys that are included here are not yet deserialized, i.e in the dictionary <see cref="_properties"/> the value should be a string.
    /// </summary>
    private HashSet<string> _propertiesLazyLoaded = new HashSet<string>();


    /// <summary>
    /// Get a string that designates a temporary property (i.e. a property that is not stored permanently). If any property key starts with this prefix,
    /// the propery is not serialized when storing the project.
    /// </summary>
    public const string TemporaryPropertyPrefixString = "tmp/";

    /// <summary>
    /// Gets the assembly version this property bag was loaded from, i.e. the version of Altaxo that has serialized this bag before it was loaded again.
    /// </summary>
    /// <value>
    /// The assembly version of Altaxo this bag was loaded from, or null if this is the default bag.
    /// </value>
    public Version? AssemblyVersionLoadedFrom { get; protected set; }

    #region Serialization

    /// <summary>
    /// 2014-01-22 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PropertyBag), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PropertyBag)obj;
        var keyList = new List<string>(s._properties.Count);
        foreach (var entry in s._properties)
        {
          if (entry.Key.StartsWith(TemporaryPropertyPrefixString))
            continue;
          if (!info.IsSerializable(entry.Value))
            continue;
          keyList.Add(entry.Key);
        }

        info.CreateArray("Properties", keyList.Count);
        info.AddAttributeValue("AssemblyVersion", s.GetType().Assembly.GetName().Version?.ToString() ?? throw new InvalidProgramException($"No assembly version available for assembly {s.GetType().Assembly}"));
        foreach (var key in keyList)
        {
          var value = s._properties[key];

          info.CreateElement("e");
          info.AddValue("Key", key);

          if (s._propertiesLazyLoaded.Contains(key) && value is string rawXml)
            info.WriteRaw(rawXml);
          else
            info.AddValueOrNull("Value", value);

          info.CommitElement();
        }
        info.CommitArray();
      }

      public void Deserialize(PropertyBag s, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var assemblyVersionString = info.GetStringAttribute("AssemblyVersion");
        if (!string.IsNullOrEmpty(assemblyVersionString))
          s.AssemblyVersionLoadedFrom = Version.Parse(assemblyVersionString);


        int count = info.OpenArray("Properties");

        for (int i = 0; i < count; ++i)
        {
          info.OpenElement(); // "e"
          string propkey = info.GetString("Key");
          object? value = info.GetValueOrOuterXml("Value", s, out var isOuterXml);
          info.CloseElement(); // "e"

          if (isOuterXml)
          {
            s._properties[propkey] = value;
            s._propertiesLazyLoaded.Add(propkey);
          }
          else
          {
            s._properties[propkey] = value;
            s._propertiesLazyLoaded.Remove(propkey);

            if (value is IDocumentLeafNode valueLeafNode)
              valueLeafNode.ParentObject = s;

          }
        }
        info.CloseArray(count);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = o is not null ? (PropertyBag)o : new PropertyBag();
        Deserialize(s, info, parent);
        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyBag"/> class.
    /// </summary>
    public PropertyBag()
    {
      _properties = new Dictionary<string, object?>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyBag"/> class by copying the properties from another instance.
    /// </summary>
    /// <param name="from">From.</param>
    public PropertyBag(PropertyBag from)
    {
      _properties = new Dictionary<string, object?>();
      CopyFrom(from);
    }

    /// <summary>
    /// Copies the properties from another instance.
    /// </summary>
    /// <param name="obj">The object to copy from.</param>
    /// <returns>True if anything could be copyied.</returns>
    public virtual bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      if (obj is PropertyBag fromBag)
      {
        _properties.Clear();
        _propertiesLazyLoaded.Clear();
        MergePropertiesFrom(fromBag, true);
        return true;
      }
      else if (obj is IPropertyBag from)
      {
        _properties.Clear();
        _propertiesLazyLoaded.Clear();
        foreach (var entry in from)
        {
          object? value;
          if (entry.Value is ICloneable)
          {
            value = ((ICloneable)entry.Value).Clone();
            var propValAsNode = value as IDocumentLeafNode;
            if (propValAsNode is not null)
              propValAsNode.ParentObject = this;
          }
          else
          {
            value = entry.Value;
          }
          _properties.Add(entry.Key, value);
        }
        EhSelfChanged(EventArgs.Empty);
        return true;
      }
      return false;
    }

    protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      foreach (var entry in _properties)
      {
        var doc = entry.Value as Main.IDocumentLeafNode;
        if (doc is not null)
          yield return new Main.DocumentNodeAndName(doc, entry.Key);
      }
    }

    object ICloneable.Clone()
    {
      return new PropertyBag(this);
    }

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>Clone of this instance.</returns>
    public PropertyBag Clone()
    {
      return new PropertyBag(this);
    }

    /// <summary>
    /// Get a string that designates a temporary property (i.e. a property that is not stored permanently). If any property key starts with this prefix,
    /// the propery is not serialized when saving the project to file.
    /// </summary>
    /// <value>
    /// Temporary property prefix.
    /// </value>
    public string TemporaryPropertyPrefix
    {
      get
      {
        return TemporaryPropertyPrefixString;
      }
    }

    #region Conversion from Lazy

    /// <summary>
    /// Converts a property from the lazy representation as Xml string into the real value. Then the lazy property entry
    /// is removed, and the property is stored in the regular property dictionary.
    /// </summary>
    /// <param name="propName">Name of the property.</param>
    /// <param name="xml">The xml element to convert from.</param>
    private bool ConvertFromLazy(string propName, string xml)
    {
      using (var info = new Altaxo.Serialization.Xml.XmlStreamDeserializationInfo())
      {
        object propval;
        try // deserialization may fail, e.g. because the DLL the type is in is not loaded (e.g. if it is an addin DLL)
        {
          info.BeginReading(xml);
          propval = info.GetValue("Value", this);
        }
        catch (Exception)
        {
          // if deserialization fails, we leave everything as it is.
          return false;
        }

        if (propval is IDocumentLeafNode documentLeafNode)
          documentLeafNode.ParentObject = this;

        _propertiesLazyLoaded.Remove(propName);
        _properties[propName] = propval;
        return true;
      }
    }



    #endregion Conversion from Lazy


    /// <summary>
    /// Removes all properties in this instance.
    /// </summary>
    public virtual void Clear()
    {
      _properties.Clear();
      _propertiesLazyLoaded.Clear();
    }

    /// <summary>
    /// Gets the number of properties in this instance.
    /// </summary>
    /// <value>
    /// Number of properties in this instance.
    /// </value>
    public virtual int Count
    {
      get
      {
        return _properties.Count;
      }
    }

    #region Document property accessors

    /// <summary>
    /// Gets the value of a property.
    /// </summary>
    /// <typeparam name="T">The of the property.</typeparam>
    /// <param name="p">The property key.</param>
    /// <returns>
    /// The property.
    /// </returns>
    [return: MaybeNull]
    public virtual T GetValue<T>(PropertyKey<T> p)
    {
      if (_propertiesLazyLoaded.Contains(p.GuidString) && _properties.TryGetValue(p.GuidString, out var obj) && obj is string xml && !ConvertFromLazy(p.GuidString, xml))
        return default;

      if (_properties.ContainsKey(p.GuidString))
        return (T)_properties[p.GuidString];
      else
        throw new KeyNotFoundException(string.Format("The property key {0} was not found in this collection", p.PropertyName));
    }

    [return: MaybeNull]
    public virtual T GetValue<T>(PropertyKey<T> p, [MaybeNull] T defaultValue)
    {
      if (_propertiesLazyLoaded.Contains(p.GuidString) && _properties.TryGetValue(p.GuidString, out var obj) && obj is string xml && !ConvertFromLazy(p.GuidString, xml))
        return defaultValue;

      if (_properties.ContainsKey(p.GuidString))
        return (T)_properties[p.GuidString];
      else
        return defaultValue;
    }

    /// <summary>
    /// Tries to get the value of a property.
    /// </summary>
    /// <typeparam name="T">Type of the property.</typeparam>
    /// <param name="p">The property key.</param>
    /// <param name="value">If successfull, on return this value contains the property value.</param>
    /// <returns>
    ///   <c>True</c> if the property could be successfully retrieved, otherwise <c>false</c>.
    /// </returns>
    public virtual bool TryGetValue<T>(PropertyKey<T> p, [MaybeNullWhen(false)] out T value)
    {
      if (_propertiesLazyLoaded.Contains(p.GuidString) && _properties.TryGetValue(p.GuidString, out var obj) && obj is string xml && !ConvertFromLazy(p.GuidString, xml))
      {
        value = default;
        return false;
      }

      var isPresent = _properties.TryGetValue(p.GuidString, out var o);
      if (isPresent && o is T to)
      {
        value = to;
        return true;
      }
      else
      {
        value = default;
        return false;
      }
    }

    /// <summary>
    /// Sets the value of a property.
    /// </summary>
    /// <typeparam name="T">Type of the property.</typeparam>
    /// <param name="p">The property key.</param>
    /// <param name="value">The value of the property.</param>
    /// <exception cref="System.ArgumentException">Thrown if the type of the provided value is not compatible with the registered property.</exception>
    public virtual void SetValue<T>(PropertyKey<T> p, T value)
    {
      if (Altaxo.Main.Services.ReflectionService.IsSubClassOfOrImplements(typeof(T), p.PropertyType))
      {
        _properties[p.GuidString] = value;
        _propertiesLazyLoaded.Remove(p.GuidString);
        if (value is IDocumentLeafNode propValAsNode)
          propValAsNode.ParentObject = this;

        EhSelfChanged(EventArgs.Empty);
      }
      else
      {
        throw new ArgumentException(string.Format("Type of the provided value is not compatible with the registered property"));
      }
    }

    /// <summary>
    /// Removes a property from this instance.
    /// </summary>
    /// <typeparam name="T">Type of the property value.</typeparam>
    /// <param name="p">The property key.</param>
    /// <returns><c>True</c> if the property has been successful removed, <c>false</c> if the property has not been found in this collection.</returns>
    public virtual bool RemoveValue<T>(PropertyKey<T> p)
    {
      _propertiesLazyLoaded.Remove(p.GuidString);
      var removed = _properties.Remove(p.GuidString);

      if (removed)
        EhSelfChanged(EventArgs.Empty);

      return removed;
    }

    #endregion Document property accessors

    #region string property name accessors

    /// <summary>
    /// Gets the value of a property.
    /// </summary>
    /// <typeparam name="T">Type of the property value.</typeparam>
    /// <param name="propName">The property name.</param>
    /// <returns>
    /// The property.
    /// </returns>
    [return: MaybeNull]
    public virtual T GetValue<T>(string propName)
    {
      if (_propertiesLazyLoaded.Contains(propName) && _properties.TryGetValue(propName, out var obj) && obj is string xml && !ConvertFromLazy(propName, xml))
        return default;

      var result = _properties[propName];
      return (T)result;
    }

    /// <summary>
    /// Tries to get the value of a property.
    /// </summary>
    /// <typeparam name="T">Type of the property value.</typeparam>
    /// <param name="propName">The property name.</param>
    /// <param name="value">If successfull, on return this value contains the property value.</param>
    /// <returns>
    ///   <c>True</c> if the property could be successfully retrieved, otherwise <c>false</c>.
    /// </returns>
    public virtual bool TryGetValue<T>(string propName, [MaybeNull] out T value)
    {
      if (_propertiesLazyLoaded.Contains(propName) && _properties.TryGetValue(propName, out var obj) && obj is string xml && !ConvertFromLazy(propName, xml))
      {
        value = default;
        return false;
      }


      var isPresent = _properties.TryGetValue(propName, out var o);
      if (isPresent)
      {
        value = (T)o;
        return true;
      }
      else
      {
        value = default;
        return false;
      }
    }

    /// <summary>
    /// Sets the value of a property.
    /// </summary>
    /// <typeparam name="T">Type of the property value.</typeparam>
    /// <param name="propName">The property name.</param>
    /// <param name="value">The value of the property.</param>
    public virtual void SetValue<T>(string propName, T value)
    {
      if (string.IsNullOrEmpty(propName))
        throw new ArgumentNullException("propName is null or empty");

      _propertiesLazyLoaded.Remove(propName);
      _properties[propName] = value;
      if (value is IDocumentLeafNode propValAsNode)
        propValAsNode.ParentObject = this;

      if (!(propName.StartsWith(TemporaryPropertyPrefixString)))
        EhSelfChanged(EventArgs.Empty);
    }

    /// <summary>
    /// Removes a property from this instance.
    /// </summary>
    /// <param name="propName">The property name.</param>
    /// <returns>
    ///   <c>True</c> if the property has been successful removed, <c>false</c> if the property has not been found in this collection.
    /// </returns>
    public virtual bool RemoveValue(string propName)
    {
      _propertiesLazyLoaded.Remove(propName);
      bool removed = _properties.Remove(propName);

      if (removed && !(propName.StartsWith(TemporaryPropertyPrefixString)))
        EhSelfChanged(EventArgs.Empty);

      return removed;
    }

    #endregion string property name accessors

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
    /// </returns>
    public virtual IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
      // we yield only those properties which could be deserialized
      foreach (var entry in _properties)
      {
        if (_propertiesLazyLoaded.Contains(entry.Key) && _properties.TryGetValue(entry.Key, out var obj) && obj is string xml)
        {
          if (ConvertFromLazy(entry.Key, xml))
            yield return new KeyValuePair<string, object?>(entry.Key, _properties[entry.Key]);
          else
            continue;
        }
        else
        {
          yield return entry;
        }
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }


    /// <summary>
    /// Gets the property keys.
    /// </summary>
    /// <value>
    /// The property keys.
    /// </value>
    public virtual IEnumerable<string> Keys
    {
      get
      {
        return _properties.Keys;
      }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    protected override void Dispose(bool isDisposing)
    {
      if (_parent is not null)
      {
        foreach (var pr in _properties)
        {
          if (pr.Value is IDisposable disposable)
          {
            disposable.Dispose();
          }
        }

        _properties.Clear();
        _propertiesLazyLoaded.Clear();

        base.Dispose(isDisposing);
      }
    }

    /// <summary>
    /// Merges the properties from another bag into this property bag. If the same property exists already in this bag, it will be overriden by the property in the other bag.
    /// </summary>
    /// <param name="from">The bag from which to take the property values that should be merged into.</param>
    /// <param name="overrideExistingProperties">If <c>true</c>, a property that already exist in this bag will be overriden by the property in the other bag. Otherwise, the existing
    /// property is left untouched.</param>
    public void MergePropertiesFrom(PropertyBag? from, bool overrideExistingProperties)
    {
      if (from is null)
        return;

      bool anythingChanged = false;

      foreach (var entry in from._properties)
      {
        if (overrideExistingProperties | !_properties.ContainsKey(entry.Key))
        {
          var value = entry.Value is ICloneable cloneableValue ? cloneableValue.Clone() : entry.Value;
          if (value is IDocumentLeafNode propValAsNode)
            propValAsNode.ParentObject = this;

          _properties[entry.Key] = value;
          if (from._propertiesLazyLoaded.Contains(entry.Key))
            _propertiesLazyLoaded.Add(entry.Key);
          else
            _propertiesLazyLoaded.Remove(entry.Key);

          anythingChanged = true;
        }
      }

      if (anythingChanged)
        EhSelfChanged(EventArgs.Empty);
    }
  }
}

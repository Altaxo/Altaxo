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
using System.Linq;
using System.Text;

namespace Altaxo.Graph
{
  /// <summary>
  /// Provides routed properties exposed by an object.
  /// </summary>
  public interface IRoutedPropertyReceiver
  {
    /// <summary>
    /// Gets the routed properties of this object, along with an action to set this property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>Enumeration of all properties (with the provided name) of this object.</returns>
    IEnumerable<(string PropertyName, object PropertyValue, Action<object> PropertySetter)> GetRoutedProperties(string propertyName);
  }

  /// <summary>
  /// Represents a routed property that provides a value to be set.
  /// </summary>
  public interface IRoutedSetterProperty
  {
    /// <summary>
    /// Gets the property name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets a value indicating whether the property is inherited by children.
    /// </summary>
    bool InheritToChilds { get; }

    /// <summary>
    /// Gets the type of the property value.
    /// </summary>
    System.Type TypeOfValue { get; }

    /// <summary>
    /// Gets the property value as an object.
    /// </summary>
    object? ValueAsObject { get; }
  }

  /// <summary>
  /// Represents a routed property that requests a value.
  /// </summary>
  public interface IRoutedGetterProperty
  {
    /// <summary>
    /// Gets the property name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the type of the property value.
    /// </summary>
    System.Type TypeOfValue { get; }
  }

  /// <summary>
  /// Represents a routed property with a setter value.
  /// </summary>
  /// <typeparam name="T">The type of the property value.</typeparam>
  public class RoutedSetterProperty<T> : IRoutedSetterProperty
  {
    private T _value;
    private string _name;
    private bool _inheritToChilds;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutedSetterProperty{T}"/> class.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="value">The property value.</param>
    public RoutedSetterProperty(string name, T value)
    {
      _name = name;
      _value = value;
    }

    /// <inheritdoc/>
    public virtual string Name { get { return _name; } }

    /// <summary>
    /// Gets the strongly typed value.
    /// </summary>
    public T Value { get { return _value; } }

    /// <inheritdoc/>
    public System.Type TypeOfValue { get { return typeof(T); } }

    /// <inheritdoc/>
    public bool InheritToChilds { get { return _inheritToChilds; } set { _inheritToChilds = value; } }

    /// <inheritdoc/>
    public object? ValueAsObject
    {
      get
      {
        return _value;
      }
    }
  }

  /// <summary>
  /// Represents a routed property that collects getter values.
  /// </summary>
  /// <typeparam name="T">The type of the property value.</typeparam>
  public class RoutedGetterProperty<T> : IRoutedGetterProperty
  {
    /// <inheritdoc/>
    public string Name { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutedGetterProperty{T}"/> class.
    /// </summary>
    /// <param name="name">The property name.</param>
    public RoutedGetterProperty(string name)
    {
      Name = name;
    }

    /// <inheritdoc/>
    public System.Type TypeOfValue { get { return typeof(T); } }

    [AllowNull]
    [MaybeNull]
    private T _value;

    /// <summary>
    /// Gets the merged value.
    /// </summary>
    [AllowNull]
    [MaybeNull]
    public T Value
    {
      get => _value;
      private set
      {
        _value = value;
      }
    }
    /// <summary>
    /// Gets a value indicating whether a value has already been set.
    /// </summary>
    public bool WasSet { get; private set; }
    /// <summary>
    /// Gets a value indicating whether merged values do not match.
    /// </summary>
    public bool DoNotMatch { get; private set; }

    /// <summary>
    /// Merges a value into this property.
    /// </summary>
    /// <param name="t">The value to merge.</param>
    public void Merge(T t)
    {
      if (!WasSet)
      {
        Value = t;
        WasSet = true;
      }
      else
      {
        if (!DoNotMatch && !object.Equals(t, Value))
          DoNotMatch = true;
      }
    }
  }
}

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
  public interface IRoutedPropertyReceiver
  {
    /// <summary>
    /// Gets the routed properties of this object, along with an action to set this property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>Enumeration of all properties (with the provided name) of this object.</returns>
    IEnumerable<(string PropertyName, object PropertyValue, Action<object> PropertySetter)> GetRoutedProperties(string propertyName);
  }

  public interface IRoutedSetterProperty
  {
    string Name { get; }

    bool InheritToChilds { get; }

    System.Type TypeOfValue { get; }

    object? ValueAsObject { get; }
  }

  public interface IRoutedGetterProperty
  {
    string Name { get; }

    System.Type TypeOfValue { get; }
  }

  public class RoutedSetterProperty<T> : IRoutedSetterProperty
  {
    private T _value;
    private string _name;
    private bool _inheritToChilds;

    public RoutedSetterProperty(string name, T value)
    {
      _name = name;
      _value = value;
    }

    public virtual string Name { get { return _name; } }

    public T Value { get { return _value; } }

    public System.Type TypeOfValue { get { return typeof(T); } }

    public bool InheritToChilds { get { return _inheritToChilds; } set { _inheritToChilds = value; } }

    public object? ValueAsObject
    {
      get
      {
        return _value;
      }
    }
  }

  public class RoutedGetterProperty<T> : IRoutedGetterProperty
  {
    public string Name { get; private set; }

    public RoutedGetterProperty(string name)
    {
      Name = name;
    }

    public System.Type TypeOfValue { get { return typeof(T); } }

    [AllowNull]
    [MaybeNull]
    private T _value;

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
    public bool WasSet { get; private set; }
    public bool DoNotMatch { get; private set; }

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

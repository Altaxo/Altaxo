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
using System.ComponentModel;
using System.Text;

namespace Altaxo.Main.Services.PropertyReflection
{
  /// <summary>
  /// Represents a reflected property that can be displayed and edited through the property reflection infrastructure.
  /// </summary>
  /// <remarks>
  /// <para>This class originated from the 'WPG Property Grid' project (<see href="http://wpg.codeplex.com"/>), licensed under Ms-PL.</para>
  /// </remarks>
  public class Property : Item, IDisposable, INotifyPropertyChanged
  {
    #region Fields

    /// <summary>
    /// The object instance that owns the reflected property.
    /// </summary>
    protected object _instance;

    /// <summary>
    /// The reflected property descriptor.
    /// </summary>
    protected PropertyDescriptor _property;

    #endregion Fields

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the <see cref="Property"/> class.
    /// </summary>
    /// <param name="instance">The object instance that owns the property.</param>
    /// <param name="property">The reflected property descriptor.</param>
    public Property(object instance, PropertyDescriptor property)
    {
      _instance = instance is ICustomTypeDescriptor descriptor ? descriptor.GetPropertyOwner(property) : instance;

      _property = property;

      _property.AddValueChanged(_instance, EhInstance_PropertyChanged);

      NotifyPropertyChanged("PropertyType");
    }

    #endregion Initialization

    #region Properties

    /// <summary>
    /// Gets or sets the current property value.
    /// </summary>
    /// <exception cref="NotSupportedException">
    /// The conversion cannot be performed
    /// </exception>
    public object Value
    {
      get { return _property.GetValue(_instance); }
      set
      {
        object currentValue = _property.GetValue(_instance);
        if (value is not null && value.Equals(currentValue))
        {
          return;
        }
        Type propertyType = _property.PropertyType;
        if (propertyType == typeof(object) ||
                    value is null && propertyType.IsClass ||
                    value is not null && propertyType.IsAssignableFrom(value.GetType()))
        {
          _property.SetValue(_instance, value);
        }
        else
        {
          TypeConverter converter = TypeDescriptor.GetConverter(_property.PropertyType);
          try
          {
            object convertedValue = converter.ConvertFrom(value);
            _property.SetValue(_instance, convertedValue);
          }
          catch (Exception)
          { }
        }
        NotifyPropertyChanged("Value");
      }
    }

    /// <summary>
    /// Gets the display name of the property.
    /// </summary>
    public string Name
    {
      get { return _property.DisplayName ?? _property.Name; }
    }

    /// <summary>
    /// Gets the property description.
    /// </summary>
    public string Description
    {
      get { return _property.Description; }
    }

    /// <summary>
    /// Gets a value indicating whether the property can be written.
    /// </summary>
    public bool IsWriteable
    {
      get { return !IsReadOnly; }
    }

    /// <summary>
    /// Gets a value indicating whether the property is read-only.
    /// </summary>
    public bool IsReadOnly
    {
      get { return _property.IsReadOnly; }
    }

    /// <summary>
    /// Gets the property type.
    /// </summary>
    public Type PropertyType
    {
      get { return _property.PropertyType; }
    }

    /// <summary>
    /// Gets the category of the property.
    /// </summary>
    public string Category
    {
      get { return _property.Category; }
    }

    /// <summary>
    /// Gets the attributes associated with the property.
    /// </summary>
    public AttributeCollection Attributes
    {
      get { return _property.Attributes; }
    }

    #endregion Properties

    #region Event Handlers

    private void EhInstance_PropertyChanged(object? sender, EventArgs e)
    {
      NotifyPropertyChanged("Value");
    }

    #endregion Event Handlers

    #region IDisposable Members

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
      if (Disposed)
      {
        return;
      }
      if (disposing)
      {
        _property.RemoveValueChanged(_instance, EhInstance_PropertyChanged);
      }
      base.Dispose(disposing);
    }

    #endregion IDisposable Members

    #region Comparer for Sorting

    private class ByCategoryThenByNameComparer : IComparer<Property>
    {
      public int Compare(Property? x, Property? y)
      {
        if (x is null || y is null)
          return 0;
        if (ReferenceEquals(x, y))
          return 0;
        int val = x.Category.CompareTo(y.Category);
        return val == 0 ? x.Name.CompareTo(y.Name) : val;
      }
    }

    private class ByNameComparer : IComparer<Property>
    {
      public int Compare(Property? x, Property? y)
      {
        return x is null || y is null ? 0 : ReferenceEquals(x, y) ? 0 : x.Name.CompareTo(y.Name);
      }
    }

    /// <summary>
    /// Compares properties first by category and then by name.
    /// </summary>
    public static readonly IComparer<Property> CompareByCategoryThenByName = new ByCategoryThenByNameComparer();
    /// <summary>
    /// Compares properties by name.
    /// </summary>
    public static readonly IComparer<Property> CompareByName = new ByNameComparer();

    #endregion Comparer for Sorting
  }
}

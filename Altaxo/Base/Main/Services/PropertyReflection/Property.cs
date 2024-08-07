﻿#region Copyright

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
  ///
  /// </summary>
  /// <remarks>
  /// <para>This class originated from the 'WPG Property Grid' project (<see href="http://wpg.codeplex.com"/>), licensed under Ms-PL.</para>
  /// </remarks>
  public class Property : Item, IDisposable, INotifyPropertyChanged
  {
    #region Fields

    protected object _instance;
    protected PropertyDescriptor _property;

    #endregion Fields

    #region Initialization

    public Property(object instance, PropertyDescriptor property)
    {
      _instance = instance is ICustomTypeDescriptor descriptor ? descriptor.GetPropertyOwner(property) : instance;

      _property = property;

      _property.AddValueChanged(_instance, EhInstance_PropertyChanged);

      NotifyPropertyChanged("PropertyType");
    }

    #endregion Initialization

    #region Properties

    /// <value>
    /// Initializes the reflected instance property
    /// </value>
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

    public string Name
    {
      get { return _property.DisplayName ?? _property.Name; }
    }

    public string Description
    {
      get { return _property.Description; }
    }

    public bool IsWriteable
    {
      get { return !IsReadOnly; }
    }

    public bool IsReadOnly
    {
      get { return _property.IsReadOnly; }
    }

    public Type PropertyType
    {
      get { return _property.PropertyType; }
    }

    public string Category
    {
      get { return _property.Category; }
    }

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

    public static readonly IComparer<Property> CompareByCategoryThenByName = new ByCategoryThenByNameComparer();
    public static readonly IComparer<Property> CompareByName = new ByNameComparer();

    #endregion Comparer for Sorting
  }
}

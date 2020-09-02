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

namespace Altaxo.Main.Properties
{
  /// <summary>
  /// Strongly typed version of a property key.
  /// </summary>
  /// <typeparam name="T">Type of the property value that this key can be used for.</typeparam>
  public class PropertyKey<T> : PropertyKeyBase
  {
    /// <summary>
    /// Procedure to apply the property value.
    /// </summary>
    protected Action<T>? _applicationOfProperty;

    /// <summary>
    /// If not null, this function is called if the property needs to be edited. The argument is the original property value, the return al
    /// </summary>
    protected Func<T, Gui.IMVCANController>? _editingControllerCreation;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyKey{T}"/> class.
    /// </summary>
    /// <param name="guidString">The unique identifier string used as a key string for this property.</param>
    /// <param name="propertyName">Name of the property. This name can contain backslashes, so that the property keys can be grouped by categories.</param>
    /// <param name="applicationLevel">The application level of this property.</param>
    public PropertyKey(string guidString, string propertyName, PropertyLevel applicationLevel)
      : this(guidString, propertyName, applicationLevel, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyKey{T}"/> class.
    /// </summary>
    /// <param name="guidString">The unique identifier string used as a key string for this property.</param>
    /// <param name="propertyName">Name of the property. This name can contain backslashes, so that the property keys can be grouped by categories.</param>
    /// <param name="applicationLevel">The application level of this property.</param>
    /// <param name="CreateBuiltinValue">Procedure to create the value that is stored in the BuiltinSettings when this constructor is called.</param>
    public PropertyKey(string guidString, string propertyName, PropertyLevel applicationLevel, Func<T> CreateBuiltinValue)
      : this(guidString, propertyName, applicationLevel, null, CreateBuiltinValue)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyKey{T}"/> class.
    /// </summary>
    /// <param name="guidString">The unique identifier string used as a key string for this property.</param>
    /// <param name="propertyName">Name of the property. This name can contain backslashes, so that the property keys can be grouped by categories.</param>
    /// <param name="applicationLevel">The application level of this property.</param>
    /// <param name="applicationItemType">Type of the application item (only useful if the application level contains <see cref="PropertyLevel.Document"/>). Can be <c>null</c> otherwise.</param>
    public PropertyKey(string guidString, string propertyName, PropertyLevel applicationLevel, Type applicationItemType)
      : this(guidString, propertyName, applicationLevel, applicationItemType, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyKey{T}"/> class.
    /// </summary>
    /// <param name="guidString">The unique identifier string used as a key string for this property.</param>
    /// <param name="propertyName">Name of the property. This name can contain backslashes, so that the property keys can be grouped by categories.</param>
    /// <param name="applicationLevel">The application level of this property.</param>
    /// <param name="applicationItemType">Type of the application item (only useful if the application level contains <see cref="PropertyLevel.Document"/>). Can be <c>null</c> otherwise.</param>
    /// <param name="CreateBuiltinValue">Procedure to create the value that is stored in the BuiltinSettings when this constructor is called.</param>
    public PropertyKey(string guidString, string propertyName, PropertyLevel applicationLevel, Type? applicationItemType, Func<T>? CreateBuiltinValue)
      : base(typeof(T), guidString, propertyName, applicationLevel, applicationItemType)
    {
      if (CreateBuiltinValue is not null && Current.PropertyService is not null)
      {
        T value = CreateBuiltinValue();
        Current.PropertyService.BuiltinSettings.SetValue(this, value);
      }
    }

    /// <summary>
    /// Applies the value given in the argument by calling a procedure stored in this property key.
    /// </summary>
    /// <param name="value">The property value that should be applied.</param>
    public void ApplyProperty(T value)
    {
      if (_applicationOfProperty is not null)
        _applicationOfProperty(value);
    }

    /// <summary>
    /// Applies the value given in the argument by calling a procedure stored in this property key.
    /// </summary>
    /// <param name="o">The property value that should be applied.</param>
    protected override void ApplyProperty(object o)
    {
      var prop = (T)o;
      ApplyProperty(prop);
    }

    /// <summary>
    /// Gets a value indicating whether this key contains a function that returns a Gui controller to edit the property value.
    /// </summary>
    /// <value>
    /// <c>true</c> if this key has edit property function; otherwise, <c>false</c>.
    /// </value>
    public override bool CanCreateEditingController
    {
      get { return _editingControllerCreation is not null; }
    }

    /// <summary>
    /// Function to get a Gui controller in order to edit a property value.
    /// </summary>
    /// <param name="originalValue">The orignal property value.</param>
    /// <returns>The Gui controller used to edit this value, or null if such a controller could not be created, or the <see cref="EditingControllerCreation"/> value was not set.</returns>
    public virtual Gui.IMVCANController? CreateEditingController(T originalValue)
    {
      if (_editingControllerCreation is not null)
      {
        return _editingControllerCreation(originalValue);
      }
      else
      {
        return null;
      }
    }

    /// <summary>
    /// Function to get a Gui controller in order to edit a property value.
    /// </summary>
    /// <param name="originalValue">The orignal property value.</param>
    /// <returns>The Gui controller used to edit this value, or null if such a controller could not be created, or the <see cref="EditingControllerCreation"/> value was not set.</returns>
    public override Gui.IMVCANController? CreateEditingController(object originalValue)
    {
      return CreateEditingController((T)originalValue);
    }

    /// <summary>
    /// Sets a function, that provides a Gui controller for the property value.
    /// </summary>
    /// <value>
    /// The edit property function.
    /// </value>
    public Func<T, Gui.IMVCANController> EditingControllerCreation
    {
      set
      {
        _editingControllerCreation = value;
      }
    }

    public Action<T>? ApplicationAction
    {
      get
      {
        return _applicationOfProperty;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException();

        if (_applicationOfProperty is not null)
          throw new InvalidOperationException("Application action was already set. It is not allowed to set it again!");

        _applicationOfProperty = value;
      }
    }
  }
}

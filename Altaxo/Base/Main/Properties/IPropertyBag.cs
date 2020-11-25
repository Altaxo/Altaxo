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
  /// A collection of properties that can be read from the bag.
  /// </summary>
  public interface IReadOnlyPropertyBag
  {
    /// <summary>
    /// Gets the value of a property. This method will throw an exception if the property is not found in the bag.
    /// </summary>
    /// <typeparam name="T">Type of the property value.</typeparam>
    /// <param name="p">The property key.</param>
    /// <returns>The property.</returns>
    T GetValue<T>(PropertyKey<T> p);

    /// <summary>
    /// Gets the value of a property.  If the property is not found in the bag, the provided default value is returned.
    /// </summary>
    /// <typeparam name="T">Type of the property value.</typeparam>
    /// <param name="p">The property key.</param>
    /// <param name="defaultValue">Default value that is returned if no property value is found in the bag.</param>
    /// <returns>The property.</returns>
    [return: MaybeNull]
    [return: NotNullIfNotNull("defaultValue")]
    T GetValue<T>(PropertyKey<T> p, [MaybeNull] T defaultValue);

    /// <summary>
    /// Tries to get the value of a property.
    /// </summary>
    /// <typeparam name="T">Type of the property.</typeparam>
    /// <param name="p">The property key.</param>
    /// <param name="value">If successfull, on return this value contains the property value.</param>
    /// <returns><c>True</c> if the property could be successfully retrieved, otherwise <c>false</c>.</returns>
    bool TryGetValue<T>(PropertyKey<T> p, out T value);

    /// <summary>
    /// Gets the property keys in this bag.
    /// </summary>
    /// <value>
    /// The property keys.
    /// </value>
    IEnumerable<string> Keys { get; }
  }

  /// <summary>
  /// Bag with properties. The recommended method is to access the properties by a property key (<see cref="PropertyKeyBase"/>). For compatibility, properties can also
  /// be stored and retrieved with strings.
  /// </summary>
  public interface IPropertyBag
    :
    IReadOnlyPropertyBag,
    Main.IChangedEventSource,
    IEnumerable<KeyValuePair<string, object?>>,
    IDisposable,
    Main.ICopyFrom
  {
    /// <summary>Removes all properties in this instance.</summary>
    void Clear();

    /// <summary>
    /// Gets the number of properties in this instance.
    /// </summary>
    /// <value>
    /// Number of properties in this instance.
    /// </value>
    int Count { get; }

    /// <summary>
    /// Gets the value of a property.
    /// </summary>
    /// <typeparam name="T">Type of the property value.</typeparam>
    /// <param name="propName">The property name.</param>
    /// <returns>The property.</returns>
    T GetValue<T>(string propName);

    /// <summary>
    /// Removes a property from this instance.
    /// </summary>
    /// <param name="p">The property key.</param>
    /// <returns>True if the property has been successful removed, fale if the property has not been found in this collection.</returns>
    bool RemoveValue<T>(PropertyKey<T> p);

    /// <summary>
    /// Removes a property from this instance.
    /// </summary>
    /// <param name="propName">The property name.</param>
    /// <returns><c>True</c> if the property has been successful removed, <c>false</c> if the property has not been found in this collection.</returns>
    bool RemoveValue(string propName);

    /// <summary>
    /// Sets the value of a property.
    /// </summary>
    /// <typeparam name="T">Type of the property.</typeparam>
    /// <param name="p">The property key.</param>
    /// <param name="value">The value of the property.</param>
    void SetValue<T>(PropertyKey<T> p, T value);

    /// <summary>
    /// Sets the value of a property.
    /// </summary>
    /// <typeparam name="T">Type of the property.</typeparam>
    /// <param name="propName">The property name.</param>
    /// <param name="value">The value of the property.</param>
    void SetValue<T>(string propName, T value);

    /// <summary>
    /// Tries to get the value of a property.
    /// </summary>
    /// <typeparam name="T">Type of the property.</typeparam>
    /// <param name="propName">The property name.</param>
    /// <param name="value">If successfull, on return this value contains the property value.</param>
    /// <returns><c>True</c> if the property could be successfully retrieved, otherwise <c>false</c>.</returns>
    bool TryGetValue<T>(string propName, out T value);

    /// <summary>
    /// Get a string that designates a temporary property (i.e. a property that is not stored permanently). If any property key starts with this prefix,
    /// the propery is not serialized when saving the project to file.
    /// </summary>
    /// <value>
    /// Temporary property prefix.
    /// </value>
    string TemporaryPropertyPrefix { get; }
  }
}

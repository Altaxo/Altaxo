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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Altaxo.Main.Properties;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Designates the location at where to look for property values.
  /// </summary>
  public enum RuntimePropertyKind
  {
    /// <summary>Look in user settings, then application settings, and finally built-in settings.</summary>
    UserAndApplicationAndBuiltin,

    /// <summary>Look application settings, and finally built-in settings.</summary>
    ApplicationAndBuiltin,

    /// <summary>Look int built-in settings.</summary>
    Builtin
  }

  /// <summary>
  /// Service for getting global and user defined properties for the application.
  /// </summary>
  [GlobalService(FallbackImplementation = typeof(PropertyServiceFallbackImplementation))]
  public interface IPropertyService
  {
    /// <summary>
    /// Absolute path to the application's config directory.
    /// </summary>
    DirectoryName ConfigDirectory { get; }

    /// <summary>
    /// Absolute path to the application's data directory.
    /// This is the directory where e.g. resources for the application are stored.
    /// </summary>
    DirectoryName DataDirectory { get; }

    /// <summary>Occurs when a property has changed. Argument is the property key.</summary>
    event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Returns the property bag with user settings. These are typically stored in the user's application settings directory.
    /// </summary>
    /// <value>
    /// The user settings.
    /// </value>
    Main.Properties.IPropertyBag UserSettings { get; }

    /// <summary>
    /// Gets the property bag with application settings. These are typically stored in the .addin file.
    /// </summary>
    /// <value>
    /// The application settings.
    /// </value>
    Main.Properties.IPropertyBag ApplicationSettings { get; }

    /// <summary>
    /// Gets the builtin settings. These are typically hard-coded in the program.
    /// </summary>
    /// <value>
    /// The builtin settings.
    /// </value>
    Main.Properties.IPropertyBag BuiltinSettings { get; }

    /// <summary>
    /// Gets the property value from UserSettings, then ApplicationSettings, then BuiltinSettings.
    /// </summary>
    /// <typeparam name="T">Type of the property value.</typeparam>
    /// <param name="property">The property key (as a string).</param>
    /// <param name="defaultValue">The default value that is returned if the property is not found.</param>
    /// <returns>The property value (if the property was found), or the default value provided in the argument (if the property was not found).</returns>
    T GetValue<T>(string property, T defaultValue);

    /// <summary>
    /// Gets the property value from UserSettings, then ApplicationSettings, then BuiltinSettings.
    /// </summary>
    /// <typeparam name="T">Type of the property value.</typeparam>
    /// <param name="p">The property key.</param>
    /// <param name="kind">Designates the kind of property to search.</param>
    /// <returns>The property value (if the property was found), or the default value defined by the property key (if not found).</returns>
    T GetValue<T>(PropertyKey<T> p, RuntimePropertyKind kind);

    /// <summary>
    /// Gets the property value from UserSettings, then ApplicationSettings, then BuiltinSettings.
    /// </summary>
    /// <typeparam name="T">Type of the property value.</typeparam>
    /// <param name="p">The property key.</param>
    /// <param name="kind">Kind of search.</param>
    /// <param name="ValueCreationIfNotFound">Function used to create a default value if the property value was not found.</param>
    /// <returns></returns>
    T GetValue<T>(PropertyKey<T> p, RuntimePropertyKind kind, Func<T> ValueCreationIfNotFound);

    /// <summary>
    /// Sets a value associated with a property key.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="property">The property key (as string).</param>
    /// <param name="value">The property value.</param>
    void SetValue<T>(string property, T value);

    /// <summary>
    /// Sets a value belonging to a property key
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="p">The property key.</param>
    /// <param name="value">The value.</param>
    void SetValue<T>(PropertyKey<T> p, T value);

    /// <summary>
    /// Saves the user settings.
    /// </summary>
    void Save();
  }
}

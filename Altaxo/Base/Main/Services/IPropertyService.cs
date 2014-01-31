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
using System.Text;

namespace Altaxo.Main.Services
{
	public enum RuntimePropertyKind
	{
		UserAndApplicationAndBuiltin,
		ApplicationAndBuiltin,
		Builtin
	}

	/// <summary>
	/// Service for getting global and user defined properties for the application.
	/// </summary>
	public interface IPropertyService
	{
		/// <summary>
		/// Absolute path to the application's config directory.
		/// </summary>
		string ConfigDirectory { get; }

		string Get(string property);

		T Get<T>(string property, T defaultValue);

		void Set<T>(string property, T value);

		/// <summary>Occurs when a property has changed. Argument is the property key.</summary>
		event Action<string> PropertyChanged;

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
		/// <param name="p">The property key.</param>
		/// <returns></returns>
		T GetValue<T>(Altaxo.Main.Properties.PropertyKey<T> p, RuntimePropertyKind kind);

		/// <summary>
		/// Gets the property value from UserSettings, then ApplicationSettings, then BuiltinSettings.
		/// </summary>
		/// <typeparam name="T">Type of the property value.</typeparam>
		/// <param name="p">The property key.</param>
		/// <param name="kind">Kind of search.</param>
		/// <param name="ValueCreationIfNotFound">Function used to create a default value if the property value was not found.</param>
		/// <returns></returns>
		T GetValue<T>(Altaxo.Main.Properties.PropertyKey<T> p, RuntimePropertyKind kind, Func<T> ValueCreationIfNotFound);
	}
}
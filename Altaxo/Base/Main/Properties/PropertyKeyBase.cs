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

using Altaxo.Main.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main.Properties
{
	/// <summary>
	/// Base class for all property keys. Property keys are used to retrieve property values.
	/// This implementation contains members to check the usage of the property key, for instance whether the retrieved type of the property value matches the type stored in the key.
	/// </summary>
	public abstract class PropertyKeyBase
	{
		private System.Type _type;
		private string _guidString;
		private string _propertyName;
		private PropertyLevel _applicationLevel;
		private Type _applicationItemType;

		private static Dictionary<string, PropertyKeyBase> _allRegisteredProperties = new Dictionary<string, PropertyKeyBase>();

		/// <summary>
		/// Gets the unique identifier string that identifies the property key. This value is used in the dictionaries as a key to store the property values.
		/// </summary>
		/// <value>
		/// The unique identifier string.
		/// </value>
		public string GuidString { get { return _guidString; } }

		/// <summary>
		/// Gets the type of the property value that can be accessed with this key.
		/// </summary>
		/// <value>
		/// The type of the property value.
		/// </value>
		public Type PropertyType { get { return _type; } }

		/// <summary>
		/// Gets the name of the property. The name can contain backslashes, which are used in the Gui system to group the items by category.
		/// </summary>
		/// <value>
		/// The name of the property.
		/// </value>
		public string PropertyName { get { return _propertyName; } }

		/// <summary>
		/// Gets the application level this property is intended for. See <see cref="PropertyLevel"/> for details.
		/// </summary>
		/// <value>
		/// The application level.
		/// </value>
		public PropertyLevel ApplicationLevel { get { return _applicationLevel; } }

		/// <summary>
		/// Gets the type of the application item. This value is used only if <see cref="ApplicationLevel"/>  contains the flag <see cref="PropertyLevel.Document"/>.
		/// </summary>
		/// <value>
		/// The type of the application item.
		/// </value>
		public Type ApplicationItemType { get { return _applicationItemType; } }

		protected PropertyKeyBase(System.Type typeOfProperty, string guidString, string propertyName, PropertyLevel applicationLevel)
			: this(typeOfProperty, guidString, propertyName, applicationLevel, null)
		{
		}

		protected PropertyKeyBase(System.Type typeOfProperty, string guidString, string propertyName, PropertyLevel applicationLevel, Type applicationItemType)
		{
			if (applicationLevel.HasFlag(PropertyLevel.Document) && applicationItemType == null)
				throw new ArgumentNullException("applicationItemType is mandatory since applicationLevel has flag 'Document'");

			_type = typeOfProperty;
			_guidString = guidString;
			_propertyName = propertyName;
			_applicationLevel = applicationLevel;
			_applicationItemType = applicationItemType;

			_allRegisteredProperties[_guidString] = this;
		}

		/// <summary>
		/// Applies the property.
		/// </summary>
		/// <param name="o">The property value to apply.</param>
		protected virtual void ApplyProperty(object o)
		{
		}

		/// <summary>
		/// Gets a value indicating whether this key contains a function that returns a Gui controller to edit the property value.
		/// </summary>
		/// <value>
		/// <c>true</c> if this key has edit property function; otherwise, <c>false</c>.
		/// </value>
		public virtual bool CanCreateEditingController
		{
			get { return false; }
		}

		/// <summary>
		/// Function to get a Gui controller in order to edit a property value.
		/// </summary>
		/// <param name="originalValue">The orignal property value.</param>
		/// <returns>The Gui controller used to edit this value, or null if such a controller could not be created, or the <see cref="PropertyKey{T}.EditingControllerCreation"/> value was not set.</returns>
		public virtual Gui.IMVCANController CreateEditingController(object originalValue)
		{
			return null;
		}

		/// <summary>
		/// Gets the name of the property.
		/// </summary>
		/// <param name="propertyKeyString">The property key as string value.</param>
		/// <returns>The name of the property</returns>
		public static string GetPropertyName(string propertyKeyString)
		{
			PropertyKeyBase result;
			if (_allRegisteredProperties.TryGetValue(propertyKeyString, out result))
				return result._propertyName;
			else
				return null;
		}

		/// <summary>
		/// Gets the type of the property value.
		/// </summary>
		/// <param name="propertyKeyString">The property key as string.</param>
		/// <returns>Type of the property value associated with the provided key.</returns>
		public static Type GetPropertyValueType(string propertyKeyString)
		{
			PropertyKeyBase result;
			if (_allRegisteredProperties.TryGetValue(propertyKeyString, out result))
				return result._type;
			else
				return null;
		}

		/// <summary>
		/// Gets the property key by providing the properties key string.
		/// </summary>
		/// <param name="propertyKeyString">The properties key string.</param>
		/// <returns>The property key.</returns>
		public static PropertyKeyBase GetPropertyKey(string propertyKeyString)
		{
			PropertyKeyBase result;
			if (_allRegisteredProperties.TryGetValue(propertyKeyString, out result))
				return result;
			else
				return null;
		}

		/// <summary>
		/// Gets all registered property keys.
		/// </summary>
		/// <value>
		/// All registered property keys.
		/// </value>
		public static IEnumerable<PropertyKeyBase> AllRegisteredPropertyKeys
		{
			get
			{
				ReflectionService.ForceRegisteringOfAllPropertyKeys();
				return _allRegisteredProperties.Values;
			}
		}
	}
}
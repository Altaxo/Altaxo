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

using System;
using System.Collections.Generic;
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
		protected Action<T> _applicationOfProperty;

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
		public PropertyKey(string guidString, string propertyName, PropertyLevel applicationLevel, Type applicationItemType, Func<T> CreateBuiltinValue)
			: base(typeof(T), guidString, propertyName, applicationLevel, applicationItemType)
		{
			if (null != CreateBuiltinValue)
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
			if (null != _applicationOfProperty)
				_applicationOfProperty(value);
		}

		/// <summary>
		/// Applies the value given in the argument by calling a procedure stored in this property key.
		/// </summary>
		/// <param name="o">The property value that should be applied.</param>
		protected override void ApplyProperty(object o)
		{
			T prop = (T)o;
			ApplyProperty(prop);
		}

		public Action<T> ApplicationAction
		{
			get
			{
				return _applicationOfProperty;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException();

				if (null != _applicationOfProperty)
					throw new InvalidOperationException("Application action was already set. It is not allowed to set it again!");

				_applicationOfProperty = value;
			}
		}
	}
}
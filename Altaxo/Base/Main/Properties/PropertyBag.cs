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
	/// Default implementation of a property bag <see cref="IPropertyBag"/>.
	/// </summary>
	public class PropertyBag : IPropertyBag
	{
		/// <summary>
		/// Dictionary that hold the properties. Key is the Guid of the property key (or any other string). Value is the property value.
		/// </summary>
		protected Dictionary<string, object> _properties;

		#region Serialization

		/// <summary>
		/// 2014-01-22 Initial version
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PropertyBag), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (PropertyBag)obj;
				// Add graph properties
				int numberproperties = s.Count;
				info.CreateArray("Properties", numberproperties);
				foreach (var entry in s._properties)
				{
					if (entry.Key.StartsWith("tmp/"))
						continue;
					if (!info.IsSerializable(entry.Value))
						continue;
					info.CreateElement("e");
					info.AddValue("Key", entry.Key);
					info.AddValue("Value", entry.Value);
					info.CommitElement();
				}
				info.CommitArray();
			}

			public void Deserialize(PropertyBag s, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				int count;
				count = info.OpenArray("Properties");
				for (int i = 0; i < count; i++)
				{
					info.OpenElement(); // "e"
					string propkey = info.GetString("Key");
					object propval = info.GetValue("Value", parent);
					info.CloseElement(); // "e"
					s._properties[propkey] = propval;
				}
				info.CloseArray(count);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (PropertyBag)o : new PropertyBag();
				Deserialize(s, info, parent);
				return s;
			}
		}

		#endregion Serialization

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyBag"/> class.
		/// </summary>
		public PropertyBag()
		{
			_properties = new Dictionary<string, object>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyBag"/> class by copying the properties from another instance.
		/// </summary>
		/// <param name="from">From.</param>
		public PropertyBag(PropertyBag from)
		{
			_properties = new Dictionary<string, object>();
			CopyFrom(from);
		}

		/// <summary>
		/// Copies the properties from another instance.
		/// </summary>
		/// <param name="obj">The object to copy from.</param>
		/// <returns>True if anything could be copyied.</returns>
		public virtual bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = (IPropertyBag)obj;
			if (null != from)
			{
				this._properties.Clear();
				foreach (var entry in from)
				{
					if (entry.Value is ICloneable)
					{
						this._properties.Add(entry.Key, ((ICloneable)entry.Value).Clone());
					}
					else
					{
						this._properties.Add(entry.Key, entry.Value);
					}
				}
				return true;
			}
			return false;
		}

		object ICloneable.Clone()
		{
			return new PropertyBag(this);
		}

		/// <summary>
		/// Clones this instance.
		/// </summary>
		/// <returns>Clone of this instance.</returns>
		public PropertyBag Clone()
		{
			return new PropertyBag(this);
		}

		/// <summary>
		/// Removes all properties in this instance.
		/// </summary>
		public void Clear()
		{
			_properties.Clear();
		}

		/// <summary>
		/// Gets the number of properties in this instance.
		/// </summary>
		/// <value>
		/// Number of properties in this instance.
		/// </value>
		public int Count
		{
			get
			{
				return _properties.Count;
			}
		}

		#region Document property accessors

		/// <summary>
		/// Gets the value of a property.
		/// </summary>
		/// <typeparam name="T">The of the property.</typeparam>
		/// <param name="p">The property key.</param>
		/// <returns>
		/// The property.
		/// </returns>
		public T GetValue<T>(PropertyKey<T> p)
		{
			var result = _properties[p.GuidString];
			return (T)result;
		}

		/// <summary>
		/// Tries to get the value of a property.
		/// </summary>
		/// <typeparam name="T">Type of the property.</typeparam>
		/// <param name="p">The property key.</param>
		/// <param name="value">If successfull, on return this value contains the property value.</param>
		/// <returns>
		///   <c>True</c> if the property could be successfully retrieved, otherwise <c>false</c>.
		/// </returns>
		public bool TryGetValue<T>(PropertyKey<T> p, out T value)
		{
			object o;
			var isPresent = _properties.TryGetValue(p.GuidString, out o);
			if (isPresent)
			{
				value = (T)o;
				return true;
			}
			else
			{
				value = default(T);
				return false;
			}
		}

		/// <summary>
		/// Sets the value of a property.
		/// </summary>
		/// <typeparam name="T">Type of the property.</typeparam>
		/// <param name="p">The property key.</param>
		/// <param name="value">The value of the property.</param>
		/// <exception cref="System.ArgumentException">Thrown if the type of the provided value is not compatible with the registered property.</exception>
		public void SetValue<T>(PropertyKey<T> p, T value)
		{
			if (Altaxo.Main.Services.ReflectionService.IsSubClassOfOrImplements(typeof(T), p.PropertyType))
			{
				_properties[p.GuidString] = value;
			}
			else
			{
				throw new ArgumentException(string.Format("Type of the provided value is not compatible with the registered property"));
			}
		}

		/// <summary>
		/// Removes a property from this instance.
		/// </summary>
		/// <typeparam name="T">Type of the property value.</typeparam>
		/// <param name="p">The property key.</param>
		/// <returns><c>True</c> if the property has been successful removed, <c>false</c> if the property has not been found in this collection.</returns>
		public bool RemoveValue<T>(PropertyKey<T> p)
		{
			return _properties.Remove(p.GuidString);
		}

		#endregion Document property accessors

		#region string property name accessors

		/// <summary>
		/// Gets the value of a property.
		/// </summary>
		/// <typeparam name="T">Type of the property value.</typeparam>
		/// <param name="propName">The property name.</param>
		/// <returns>
		/// The property.
		/// </returns>
		public T GetValue<T>(string propName)
		{
			var result = _properties[propName];
			return (T)result;
		}

		/// <summary>
		/// Tries to get the value of a property.
		/// </summary>
		/// <typeparam name="T">Type of the property value.</typeparam>
		/// <param name="propName">The property name.</param>
		/// <param name="value">If successfull, on return this value contains the property value.</param>
		/// <returns>
		///   <c>True</c> if the property could be successfully retrieved, otherwise <c>false</c>.
		/// </returns>
		public bool TryGetValue<T>(string propName, out T value)
		{
			object o;
			var isPresent = _properties.TryGetValue(propName, out o);
			if (isPresent)
			{
				value = (T)o;
				return true;
			}
			else
			{
				value = default(T);
				return false;
			}
		}

		/// <summary>
		/// Sets the value of a property.
		/// </summary>
		/// <typeparam name="T">Type of the property value.</typeparam>
		/// <param name="propName">The property name.</param>
		/// <param name="value">The value of the property.</param>
		public void SetValue<T>(string propName, T value)
		{
			_properties[propName] = value;
		}

		/// <summary>
		/// Removes a property from this instance.
		/// </summary>
		/// <param name="propName">The property name.</param>
		/// <returns>
		///   <c>True</c> if the property has been successful removed, <c>false</c> if the property has not been found in this collection.
		/// </returns>
		public bool RemoveValue(string propName)
		{
			return _properties.Remove(propName);
		}

		#endregion string property name accessors

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return _properties.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _properties.GetEnumerator();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			foreach (var pr in _properties)
				if (pr.Value is IDisposable)
					((IDisposable)pr.Value).Dispose();

			_properties.Clear();
		}
	}
}
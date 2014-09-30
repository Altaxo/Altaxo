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

using Altaxo.Main.Properties;
using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main.Services
{
	public class PropertyService : Altaxo.Main.Services.IPropertyService
	{
		/// <summary>Occurs when a property changed, argument is the key to the property that changed.</summary>
		public event Action<string> PropertyChanged;

		#region Inner classes

		private class PropertyBagWrapper : IPropertyBag
		{
			public PropertyService _parent;

			protected Dictionary<string, object> _properties;
			protected Dictionary<string, string> _guidToName;

			public event EventHandler Changed;

			public PropertyBagWrapper(PropertyService s)
			{
				_parent = s;
				_guidToName = new Dictionary<string, string>();
				_properties = new Dictionary<string, object>();
			}

			/// <summary>
			/// Get a string that designates a temporary property (i.e. a property that is not stored permanently). If any property key starts with this prefix,
			/// the propery is not serialized when saving the project to file.
			/// </summary>
			/// <value>
			/// Temporary property prefix.
			/// </value>
			public string TemporaryPropertyPrefix
			{
				get
				{
					return PropertyBag.TemporaryPropertyPrefixString;
				}
			}

			public void Clear()
			{
				_guidToName.Clear();
				_properties.Clear();
			}

			public int Count
			{
				get { return _properties.Count; }
			}

			public bool TryGetValue<T>(PropertyKey<T> p, out T value)
			{
				return TryGetValue<T>(p.GuidString, out value);
			}

			public bool TryGetValue<T>(string propName, out T value)
			{
				object resultObject;
				if (_properties.TryGetValue(propName, out resultObject))
				{
					value = (T)resultObject;
					return true;
				}

				if (_parent.TryGet<T>(propName, out value))
				{
					_properties[propName] = value;
					return true;
				}
				return false;
			}

			public T GetValue<T>(PropertyKey<T> p)
			{
				T result;
				if (TryGetValue<T>(p, out result))
					return result;
				else
					return default(T);
			}

			public T GetValue<T>(PropertyKey<T> p, T defaultValue)
			{
				T result;
				if (TryGetValue<T>(p, out result))
					return result;
				else
					return defaultValue;
			}

			public T GetValue<T>(string propName)
			{
				T result;
				if (TryGetValue<T>(propName, out result))
					return result;
				else
					return default(T);
			}

			public bool RemoveValue<T>(PropertyKey<T> p)
			{
				return RemoveValue(p.GuidString);
			}

			public bool RemoveValue(string propName)
			{
				bool b1 = _properties.Remove(propName);
				bool b2 = ICSharpCode.Core.PropertyService.Remove(propName);
				return b1 | b2;
			}

			public void SetValue<T>(PropertyKey<T> p, T value)
			{
				if (Altaxo.Main.Services.ReflectionService.IsSubClassOfOrImplements(typeof(T), p.PropertyType))
				{
					SetValue<T>(p.GuidString, value);
				}
				else
				{
					throw new ArgumentException(string.Format("Type of the provided value is not compatible with the registered property"));
				}
			}

			public void SetValue<T>(string propName, T value)
			{
				_properties[propName] = value;

				if (!propName.StartsWith(TemporaryPropertyPrefix))
					_parent.Set<T>(propName, value);
			}

			public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
			{
				return _properties.GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return _properties.GetEnumerator();
			}

			public void Dispose()
			{
			}

			public bool CopyFrom(object from)
			{
				return false;
			}

			public object Clone()
			{
				return this;
			}

			public object ParentObject
			{
				get
				{
					return _parent;
				}
				set
				{
					_parent = value as PropertyService;
				}
			}

			#region Change handling

			protected virtual void OnChanged()
			{
				var p = _parent as Main.IChildChangedEventSink;
				if (null != p)
					p.EhChildChanged(this, EventArgs.Empty);

				var ev = Changed;
				if (null != ev)
					ev(this, EventArgs.Empty);
			}

			#endregion Change handling
		}

		#endregion Inner classes

		public IPropertyBag UserSettings { get; private set; }

		public IPropertyBag ApplicationSettings { get; private set; }

		public IPropertyBag BuiltinSettings { get; private set; }

		public PropertyService()
		{
			ICSharpCode.Core.PropertyService.PropertyChanged += new PropertyChangedEventHandler(PropertyService_PropertyChanged);

			UserSettings = new PropertyBagWrapper(this);
			ApplicationSettings = new PropertyBag();
			BuiltinSettings = new PropertyBag();
		}

		private void PropertyService_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (null != PropertyChanged)
				PropertyChanged(e.Key);
		}

		public string ConfigDirectory
		{
			get
			{
				return ICSharpCode.Core.PropertyService.ConfigDirectory;
			}
		}

		public string Get(string property)
		{
			return ICSharpCode.Core.PropertyService.Get(property);
		}

		public bool TryGet<T>(string property, out T result)
		{
			// we first have to try if it is an Altaxo object
			try
			{
				var wrapp = ICSharpCode.Core.PropertyService.Get<Altaxo.Serialization.Xml.FrameworkXmlSerializationWrapper>(property, null);
				if (wrapp != null && wrapp.WrappedObject != null)
				{
					result = (T)wrapp.WrappedObject;
					return true;
				}
				else
				{
					result = default(T);
					return false;
				}
			}
			catch (Exception)
			{
			}

			// obviously our try for an Altaxo object failed, thus we try it the SharpDevelop way...
			{
				T defau = default(T);
				result = ICSharpCode.Core.PropertyService.Get<T>(property, defau);
				if (null == result)
					return false;
				if (null == defau)
					return true;
				bool returnValue = !defau.Equals(result);

				if (result is Altaxo.Serialization.Xml.FrameworkXmlSerializationWrapper)
				{
					var wrapper = result as Altaxo.Serialization.Xml.FrameworkXmlSerializationWrapper;
					result = (T)wrapper.WrappedObject;
				}

				return returnValue;
			}
		}

		public T Get<T>(string property, T defaultValue)
		{
			T result;
			if (TryGet<T>(property, out result))
				return result;
			else
				return defaultValue;
		}

		public void Set<T>(string property, T value)
		{
			if (null != value && Altaxo.Serialization.Xml.FrameworkXmlSerializationWrapper.IsSerializableType(value.GetType()))
			{
				ICSharpCode.Core.PropertyService.Set(property, new Altaxo.Serialization.Xml.FrameworkXmlSerializationWrapper(value));
			}
			else if (null != value)
			{
				ICSharpCode.Core.PropertyService.Set(property, value);
			}
			else
			{
				ICSharpCode.Core.PropertyService.Remove(property);
			}
		}

		/// <summary>
		/// Gets a value. First UserSettings, then ApplicationSettings, and then BuiltinSettings is searched.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="p">The p.</param>
		/// <param name="propKind">Designates where to search for the property value.</param>
		/// <returns></returns>
		public T GetValue<T>(PropertyKey<T> p, RuntimePropertyKind propKind)
		{
			return GetValue<T>(p, propKind, null);
		}

		/// <summary>
		/// Gets a value. First UserSettings, then ApplicationSettings, and then BuiltinSettings is searched.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="p">The p.</param>
		/// <param name="propKind">Designates where to search for the property value.</param>
		/// <param name="ValueCreationIfNotFound">Function to create a new property value if no property value was found. May be <c>null</c>, in this case the default(T) operator is used to create the default value.</param>
		/// <returns></returns>
		public T GetValue<T>(PropertyKey<T> p, RuntimePropertyKind propKind, Func<T> ValueCreationIfNotFound)
		{
			T result;
			IPropertyBag[] bags;

			switch (propKind)
			{
				case RuntimePropertyKind.UserAndApplicationAndBuiltin:
					bags = new IPropertyBag[] { UserSettings, ApplicationSettings, BuiltinSettings };
					break;

				case RuntimePropertyKind.ApplicationAndBuiltin:
					bags = new IPropertyBag[] { ApplicationSettings, BuiltinSettings };
					break;

				case RuntimePropertyKind.Builtin:
					bags = new IPropertyBag[] { BuiltinSettings };
					break;

				default:
					throw new NotImplementedException();
			}

			foreach (var pb in bags)
			{
				if (pb.TryGetValue<T>(p, out result))
					return result;
			}

			if (null != ValueCreationIfNotFound)
				return ValueCreationIfNotFound();
			else
				return default(T);
		}
	}
}
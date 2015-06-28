using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Gdi
{
	/// <summary>
	/// Interface to a paint context that must be used by the objects to be painted to store temporary object valid only during painting.
	/// </summary>
	public interface IPaintContext
	{
		/// <summary>
		/// Adds a specified object under a specified key.
		/// </summary>
		/// <param name="key">The key (usually the owner of the value).</param>
		/// <param name="value">The value.</param>
		void AddValue(object key, object value);

		/// <summary>
		/// Gets an object stored under a specified key (usually the owner).
		/// </summary>
		/// <typeparam name="T">Type of the value.</typeparam>
		/// <param name="key">The key.</param>
		/// <returns>The value. An exception will be thrown if the specified key does not exist or a value with another type than the specified type is stored under the key.</returns>
		T GetValue<T>(object key);

		/// <summary>
		/// Gets an object stored under a specified key (usually the owner). If the value is not available, the default value of the specified type is returned.
		/// </summary>
		/// <typeparam name="T">Type of the value.</typeparam>
		/// <param name="key">The key.</param>
		/// <returns>The value, or the default value. An exception will be thrown if the specified key does not exist or a value with another type than the specified type is stored under the key.</returns>
		T GetValueOrDefault<T>(object key);
	}

	/// <summary>
	/// Implementation of <see cref="IPaintContext"/> for Gdi paint operations.
	/// </summary>
	public class GdiPaintContext : IPaintContext
	{
		Dictionary<object, object> _dictionary = new Dictionary<object, object>();

		///<inheritdoc/>
		public void AddValue(object key, object value)
		{
			_dictionary.Add(key, value);
		}

		///<inheritdoc/>
		public T GetValue<T>(object key)
		{
			return (T)_dictionary[key];
		}

		///<inheritdoc/>
		public T GetValueOrDefault<T>(object key)
		{
			object o;
			if (_dictionary.TryGetValue(key, out o))
			{
				return (T)o;
			}
			else
			{
				return default(T);
			}
		}
	}
}
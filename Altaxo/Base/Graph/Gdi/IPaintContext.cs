using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Gdi
{
	/// <summary>
	/// Implementation of <see cref="IPaintContext"/> for Gdi paint operations.
	/// </summary>
	public class GdiPaintContext : IPaintContext
	{
		private Dictionary<object, object> _dictionary = new Dictionary<object, object>();

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

		#region Hierarchical values

		private Dictionary<string, object> _hierarchicalData = new Dictionary<string, object>();

		public void PushHierarchicalValue<T>(string name, T value)
		{
			object existing;
			if (_hierarchicalData.TryGetValue(name, out existing))
			{
				var existingStack = existing as Stack<T>;
				if (null != existingStack)
					existingStack.Push(value);
				else
					throw new InvalidOperationException(string.Format("Expected stored type: {0}, but was {1}", typeof(Stack<T>), existing.GetType()));
			}
			else
			{
				var newStack = new Stack<T>();
				newStack.Push(value);
				_hierarchicalData.Add(name, newStack);
			}
		}

		public T PopHierarchicalValue<T>(string name)
		{
			object existing;
			if (_hierarchicalData.TryGetValue(name, out existing))
			{
				var existingStack = existing as Stack<T>;
				if (null != existingStack)
				{
					return existingStack.Pop();
				}
				else
				{
					throw new InvalidOperationException(string.Format("Expected stored type: {0}, but was {1}", typeof(Stack<T>), existing.GetType()));
				}
			}
			else
			{
				throw new InvalidOperationException(string.Format("Key {0} was not found in the dictionary.", name));
			}
		}

		public T GetHierarchicalValue<T>(string name)
		{
			object existing;
			if (_hierarchicalData.TryGetValue(name, out existing))
			{
				var existingStack = existing as Stack<T>;
				if (null != existingStack)
					return existingStack.Peek();
				else
					throw new InvalidOperationException(string.Format("Expected stored type: {0}, but was {1}", typeof(Stack<T>), existing.GetType()));
			}
			else
			{
				throw new InvalidOperationException(string.Format("Key {0} was not found in the dictionary.", name));
			}
		}

		#endregion Hierarchical values
	}
}
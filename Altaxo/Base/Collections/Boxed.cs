using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Collections
{
	/// <summary>
	/// Boxes a single value into a class. The value can be get/set by the property <see cref="Value"/>. Since the value is accessible by a property, this class can be used in collections as data sources for data grids, etc.
	/// For this, it additionally implements both <see cref="System.ComponentModel.INotifyPropertyChanged"/> and <see cref="System.ComponentModel.IEditableObject"/>
	/// </summary>
	/// <typeparam name="T">The type of the value to be boxed.</typeparam>
	public class Boxed<T> : System.ComponentModel.INotifyPropertyChanged, System.ComponentModel.IEditableObject
	{
		/// <summary>Event arg for the property changed event.</summary>
		static readonly System.ComponentModel.PropertyChangedEventArgs _valueChangedEventArg = new System.ComponentModel.PropertyChangedEventArgs("Value");

		/// <summary>The boxed value.</summary>
		protected T _value;

		/// <summary>Occurs when the boxed value changed.</summary>
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;


		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>
		/// The value.
		/// </value>
		public T Value
		{
			get { return _value; }
			set
			{
				T oldValue = _value;
				_value = value;
				if (null != PropertyChanged && !object.Equals(_value, oldValue))
					PropertyChanged(this, _valueChangedEventArg);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Boxed&lt;T&gt;"/> class with the value set to the default value of  <see cref="T:T"/>.
		/// </summary>
		public Boxed()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Boxed&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="val">The value to be boxed.</param>
		public Boxed(T val)
		{
			_value = val;
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="Altaxo.Collections.Boxed&lt;T&gt;"/> to <see cref="T:T"/>.
		/// </summary>
		/// <param name="val">The boxed value</param>
		/// <returns>
		/// The unboxed value.
		/// </returns>
		public static implicit operator T(Boxed<T> val)
		{
			return val.Value;
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="T:T"/> to <see cref="Altaxo.Collections.Boxed&lt;T&gt;"/>.
		/// </summary>
		/// <param name="val">The value to be boxed.</param>
		/// <returns>
		/// The value wrapped in an instance of this class.
		/// </returns>
		public static implicit operator Boxed<T>(T val)
		{
			return new Boxed<T>(val);
		}


		/// <summary>
		/// Begins an edit on an object.
		/// </summary>
		public virtual void BeginEdit()
		{

		}

		/// <summary>
		/// Discards changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> call.
		/// </summary>
		public virtual void CancelEdit()
		{

		}

		/// <summary>
		/// Pushes changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> or <see cref="M:System.ComponentModel.IBindingList.AddNew"/> call into the underlying object.
		/// </summary>
		public virtual void EndEdit()
		{

		}

		public static IEnumerable<Boxed<T>> ToBoxedItems(IEnumerable<T> items)
		{
			foreach (var item in items)
				yield return new Boxed<T>(item);
		}

		public static void AddRange(ICollection<Boxed<T>> destination, IEnumerable<T> sourceItems)
		{
			foreach (var item in sourceItems)
				destination.Add(new Boxed<T>(item));
		}

		public static IEnumerable<T> ToUnboxedItems(IEnumerable<Boxed<T>> boxedItems)
		{
			foreach (var item in boxedItems)
				yield return item.Value;
		}

		public static void AddRange(ICollection<T> destination, IEnumerable<Boxed<T>> sourceItems)
		{
			foreach (var item in sourceItems)
				destination.Add(item.Value);
		}


		public static T[] ToUnboxedArray(ICollection<Boxed<T>> boxedItems)
		{
			var arr = new T[boxedItems.Count];
			int i = 0;
			foreach (var item in boxedItems)
			{
				arr[i] = item.Value;
				++i;
			}
			return arr;
		}
	}

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui
{
	/// <summary>
	/// Helper class that wraps a single value of type T and implements the <see cref="System.ComponentModel.INotifyPropertyChanged"/> interface, so that changes to the value can be monitored by the Gui.
	/// </summary>
	/// <typeparam name="T">Type of the value to wrap.</typeparam>
	public class NotifyChangedValue<T> : System.ComponentModel.INotifyPropertyChanged
	{
		private T _value;
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Empty constructor, which sets the value to the defaulting one.
		/// </summary>
		public NotifyChangedValue()
		{
		}

		/// <summary>
		/// Constructs the class with the given value.
		/// </summary>
		/// <param name="value">Initial value.</param>
		public NotifyChangedValue(T value)
		{
			_value = value;
		}

		/// <summary>
		/// Gets/sets the value. If the value set is different from the old one, the PropertyChanged event is triggered.
		/// </summary>
		public T Value
		{
			get
			{
				return _value;
			}
			set
			{
				T oldValue = _value;
				_value = value;
				if (!object.Equals(oldValue, value) && null != PropertyChanged)
					PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Value"));
			}
		}
	}
}

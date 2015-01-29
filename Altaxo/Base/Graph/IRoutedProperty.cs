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
using System.Linq;
using System.Text;

namespace Altaxo.Graph
{
	public interface IRoutedPropertyReceiver
	{
		void SetRoutedProperty(IRoutedSetterProperty property);

		void GetRoutedProperty(IRoutedGetterProperty property);
	}

	public interface IRoutedSetterProperty
	{
		string Name { get; }

		bool InheritToChilds { get; }

		System.Type TypeOfValue { get; }

		object ValueAsObject { get; }
	}

	public interface IRoutedGetterProperty
	{
		string Name { get; }

		System.Type TypeOfValue { get; }
	}

	public class RoutedSetterProperty<T> : IRoutedSetterProperty
	{
		private T _value;
		private string _name;
		private bool _inheritToChilds;

		public RoutedSetterProperty(string name, T value)
		{
			_name = name;
			_value = value;
		}

		public virtual string Name { get { return _name; } }

		public T Value { get { return _value; } }

		public System.Type TypeOfValue { get { return typeof(T); } }

		public bool InheritToChilds { get { return _inheritToChilds; } set { _inheritToChilds = value; } }

		public object ValueAsObject
		{
			get
			{
				return _value;
			}
		}
	}

	public class RoutedGetterProperty<T> : IRoutedGetterProperty
	{
		public string Name { get; set; }

		public System.Type TypeOfValue { get { return typeof(T); } }

		private T _value;
		private bool _wasSet;
		private bool _doNotMatch;

		public T Value { get { return _value; } }

		public void Merge(T t)
		{
			if (!_wasSet)
			{
				_value = t;
				_wasSet = true;
			}
			else
			{
				if (!_doNotMatch && !object.Equals(t, _value))
					_doNotMatch = true;
			}
		}
	}
}
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

namespace Altaxo.Calc
{
	/// <summary>
	/// This structure holds a value, which is either absolute or relative to another value.
	/// </summary>
	[Serializable]
	public struct RelativeOrAbsoluteValue : IEquatable<RelativeOrAbsoluteValue>
	{
		/// <summary>True if the value m_Value is relative, false if m_Value is absolute.</summary>
		private bool _isRelative; // per default, m_bRelative is false, so the value is interpreted as absolute

		/// <summary>
		/// The value to hold, either absolute, or relative. If relative, the value 1 means the same value
		/// as the value it is relative to.
		/// </summary>
		private double _value;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RelativeOrAbsoluteValue), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				RelativeOrAbsoluteValue s = (RelativeOrAbsoluteValue)obj;

				info.AddValue("IsRelative", s._isRelative);
				info.AddValue("Value", s._value);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				bool rel = info.GetBoolean("IsRelative");
				double val = info.GetDouble("Value");
				return new RelativeOrAbsoluteValue(val, rel);
			}
		}

		#endregion Serialization

		/// <summary>
		/// This creates the structure with the absolute value absval.
		/// </summary>
		/// <param name="absval"></param>
		public RelativeOrAbsoluteValue(double absval)
		{
			_isRelative = false;
			_value = absval;
		}

		/// <summary>
		/// Constructs the structure with provided value and the information,
		/// if it is a absolute or a relative value.
		/// </summary>
		/// <param name="val">The value, either absolute or relative.</param>
		/// <param name="isRelative">True if the value is relative, else false.</param>
		public RelativeOrAbsoluteValue(double val, bool isRelative)
		{
			_isRelative = isRelative;
			_value = val;
		}

		public static RelativeOrAbsoluteValue NewAbsoluteValue(double val)
		{
			return new RelativeOrAbsoluteValue(val, false);
		}

		public static RelativeOrAbsoluteValue NewRelativeValue(double val)
		{
			return new RelativeOrAbsoluteValue(val, true);
		}

		/// <summary>
		/// Get / set the information, if the value is relative or absolute.
		/// </summary>
		public bool IsRelative
		{
			get { return _isRelative; }
		}

		/// <summary>
		/// Get / set the information, if the value is relative or absolute.
		/// </summary>
		public bool IsAbsolute
		{
			get { return !_isRelative; }
		}

		/// <summary>
		///  Get / set the raw value. Careful! the value you get is not relative to another, even
		///  in the case that the sructure holds a relative value, it is the raw value in m_Value instead.
		/// </summary>
		public double Value
		{
			get { return _value; }
		}

		/// <summary>
		/// This is the function to get out the value. In case it is a absolute value, simply
		/// Value is returned, regardless of the argument <paramref name="r"/>. In case it
		/// is a absolute value, the product of r with the value is returned.
		/// </summary>
		/// <param name="r">The value to which this value is relative to.</param>
		/// <returns>If absolute, the stored value; if relative, the product of the stored value with <paramref name="r"/></returns>
		public double GetValueRelativeTo(double r)
		{
			return _isRelative ? r * _value : _value;
		}

		public static bool operator ==(RelativeOrAbsoluteValue a, RelativeOrAbsoluteValue b)
		{
			return a._isRelative == b._isRelative && a._value == b._value;
		}

		public static bool operator !=(RelativeOrAbsoluteValue a, RelativeOrAbsoluteValue b)
		{
			return !(a == b);
		}

		public override bool Equals(object o)
		{
			if (!(o is RelativeOrAbsoluteValue))
			{
				return false;
			}
			else
			{
				var other = (RelativeOrAbsoluteValue)o;
				return _value == other._value && _isRelative == other._isRelative;
			}
		}

		public override int GetHashCode()
		{
			return _value.GetHashCode() + _isRelative.GetHashCode();
		}

		public bool Equals(RelativeOrAbsoluteValue other)
		{
			return _value == other._value && _isRelative == other._isRelative;
		}
	}
}
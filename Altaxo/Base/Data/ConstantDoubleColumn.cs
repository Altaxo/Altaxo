#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

namespace Altaxo.Data
{
	/// <summary>
	/// A column whose rows all have the same value (of type Double).
	/// </summary>
	public class ConstantDoubleColumn : INumericColumn, IReadableColumn, ICloneable
	{
		private double _value;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ConstantDoubleColumn), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (ConstantDoubleColumn)obj;
				info.AddValue("Value", s._value);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				double value = info.GetDouble("Value");
				return new ConstantDoubleColumn(value);
			}
		}

		#endregion Serialization

		public ConstantDoubleColumn(double value)
		{
			_value = value;
		}

		public ConstantDoubleColumn()
		{
			_value = 0;
		}

		/// <summary>
		/// Creates a cloned instance of this object.
		/// </summary>
		/// <returns>The cloned instance of this object.</returns>
		public object Clone()
		{
			return new ConstantDoubleColumn(_value);
		}

		/// <summary>
		/// Simply returns the value i.
		/// </summary>
		/// <param name="i">The index i.</param>
		/// <returns>The index i.</returns>
		public double this[int i]
		{
			get
			{
				return _value;
			}
		}

		/// <summary>
		/// This returns always true.
		/// </summary>
		/// <param name="i">The index i.</param>
		/// <returns>Always true.</returns>
		public bool IsElementEmpty(int i)
		{
			return double.IsNaN(_value);
		}

		/// <summary>
		/// Returns the index i as AltaxoVariant.
		/// </summary>
		AltaxoVariant IReadableColumn.this[int i]
		{
			get
			{
				return new AltaxoVariant(_value);
			}
		}

		/// <summary>
		/// The full name of this column (Gui culture)
		/// </summary>
		public string FullName
		{
			get { return string.Format(Altaxo.Settings.GuiCulture.Instance, "Constant, value = {0}", _value); }
		}

		public override string ToString()
		{
			return string.Format(Altaxo.Settings.GuiCulture.Instance, "Constant, value = {0}", _value);
		}
	}
}
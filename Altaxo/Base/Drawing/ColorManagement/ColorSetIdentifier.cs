#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

namespace Altaxo.Drawing.ColorManagement
{
	/// <summary>
	/// Structure that stores <see cref="ColorSetLevel"/> and name of a color set. This is used as key value in the internal dictionaries.
	/// </summary>
	[System.ComponentModel.ImmutableObject(true)]
	public class ColorSetIdentifier : IEquatable<ColorSetIdentifier>, IComparable<ColorSetIdentifier>, Main.IImmutable
	{
		private ColorSetLevel _level;
		private string _name;

		#region Serialization

		/// <summary>
		/// 2015-11-14 Version 1 moved to Altaxo.Drawing.ColorManagement.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.ColorManagement.ColorSetIdentifier", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ColorSetIdentifier), 1)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (ColorSetIdentifier)obj;
				info.AddEnum("Level", s.Level);
				info.AddValue("Name", s.Name);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var colorSetLevel = (ColorManagement.ColorSetLevel)info.GetEnum("Level", typeof(ColorManagement.ColorSetLevel));
				string colorSetName = info.GetString("Name");
				return new ColorSetIdentifier(colorSetLevel, colorSetName);
			}
		}

		#endregion Serialization

		public ColorSetIdentifier(ColorSetLevel colorSetLevel, string colorSetName)
		{
			if (string.IsNullOrEmpty(colorSetName))
				throw new ArgumentOutOfRangeException("colorSetName is null or is empty");

			_level = colorSetLevel;
			_name = colorSetName;
		}

		/// <summary>
		/// Gets the color set level.
		/// </summary>
		/// <value>
		/// The level of the color set.
		/// </value>
		public ColorSetLevel Level { get { return _level; } }

		/// <summary>
		/// Gets the name of the color set.
		/// </summary>
		/// <value>
		/// The name of the color set.
		/// </value>
		public string Name { get { return _name; } }

		public override int GetHashCode()
		{
			return _level.GetHashCode() * 29 + _name.GetHashCode() * 31;
		}

		public bool Equals(ColorSetIdentifier other)
		{
			return this._level == other._level && 0 == string.Compare(this._name, other._name);
		}

		public override bool Equals(object obj)
		{
			if (obj is ColorSetIdentifier)
			{
				var other = (ColorSetIdentifier)obj;
				return this._level == other._level && 0 == string.Compare(this._name, other._name);
			}
			return false;
		}

		public int CompareTo(ColorSetIdentifier other)
		{
			int result;
			result = Comparer<int>.Default.Compare((int)this._level, (int)other._level);
			if (0 != result)
				return result;
			else
				return string.Compare(this._name, other._name);
		}

		public override string ToString()
		{
			return string.Format("{0} ({1})", _name, _level);
		}
	}
}
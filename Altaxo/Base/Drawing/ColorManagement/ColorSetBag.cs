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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Drawing.ColorManagement
{
	/// <summary>
	/// Used to store user color sets in the user's settings.
	/// </summary>
	public class ColorSetBag
	{
		/// <summary>
		/// The color sets. One tupe consist of the color set and a bool indication whether this is a plot color set.
		/// </summary>
		private Tuple<IColorSet, bool>[] _colorSets;

		/// <summary>
		/// 2016-08-19 Initial version
		/// </summary>
		/// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ColorSetBag), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (ColorSetBag)obj;

				info.CreateArray("ColorSets", s._colorSets.Length);

				foreach (var c in s._colorSets)
				{
					info.CreateElement("e");
					info.AddValue("ColorSet", c.Item1);
					info.AddValue("IsPlotColorSet", c.Item2);
					info.CommitElement();
				}

				info.CommitArray();
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				int count = info.OpenArray("ColorSets");
				var colorSets = new Tuple<IColorSet, bool>[count];
				for (int i = 0; i < count; ++i)
				{
					info.OpenElement(); // e
					colorSets[i] = new Tuple<IColorSet, bool>((IColorSet)info.GetValue("ColorSet", null), info.GetBoolean("IsPlotColorSet"));
					info.CloseElement();
				}

				info.CloseArray(count);

				return new ColorSetBag(colorSets);
			}
		}

		public ColorSetBag(IEnumerable<Tuple<IColorSet, bool>> colorSets)
		{
			_colorSets = colorSets.ToArray();
		}

		public IEnumerable<Tuple<IColorSet, bool>> ColorSets
		{
			get
			{
				return _colorSets;
			}
		}
	}
}
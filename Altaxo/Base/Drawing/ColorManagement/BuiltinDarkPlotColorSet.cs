#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2012 Dr. Dirk Lellinger
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
	public class BuiltinDarkPlotColorSet : BuiltinColorSet
	{
		private static BuiltinDarkPlotColorSet _instance = new BuiltinDarkPlotColorSet();

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.BuiltinPlotColorSet", 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Serialization of old versions not supported");
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				info.GetString("Name");
				return BuiltinDarkPlotColorSet.Instance;
			}
		}

		/// <summary>
		/// <para>Date: 2012-10-25</para>
		/// <para>This is the initial version of serialization of this instance. It is labelled with version 1, because this class is the predecessor of the original Altaxo.Graph.BuiltinPlotColorSet.</para>
		/// <para>2015-11-15 Version 2 moved to Altaxo.Drawing.ColorManagement namespace.</para>
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.ColorManagement.BuiltinDarkPlotColorSet", 1)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BuiltinDarkPlotColorSet), 2)]
		private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				return BuiltinDarkPlotColorSet.Instance;
			}
		}

		#endregion Serialization

		/// <summary>
		/// Initializes a new instance of the <see cref="BuiltinDarkPlotColorSet"/> class.
		/// </summary>
		public BuiltinDarkPlotColorSet()
			: base("PlotColorsDark", ColorsVersion0())
		{
		}

		/// <summary>
		/// Gets the instance of this color set.
		/// </summary>
		public static BuiltinDarkPlotColorSet Instance { get { return _instance; } }

		private static NamedColor[] ColorsVersion0() // Version 2012-09-10
		{
			return new NamedColor[]{
			NamedColors.Black,
			NamedColors.Red,
			NamedColors.Green,
			NamedColors.Blue,
			NamedColors.Magenta,
			NamedColors.Goldenrod,
			NamedColors.Coral
			};
		}

		/// <summary>
		/// Gets a value indicating whether this instance is used as a plot color set.
		/// </summary>
		/// <value>Always <c>true</c> for this set.
		/// </value>
		public override bool IsPlotColorSet
		{
			get { return true; }
		}
	}
}
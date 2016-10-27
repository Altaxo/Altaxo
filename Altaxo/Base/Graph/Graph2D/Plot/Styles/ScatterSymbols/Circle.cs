﻿#region Copyright

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

using Altaxo.Drawing;
using Altaxo.Geometry;
using ClipperLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols
{
	public class Circle : ScatterSymbolBase
	{
		private const double Sqrt1By2 = 0.70710678118654752440084436210485;

		#region Serialization

		/// <summary>
		/// 2016-10-27 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Circle), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				info.AddBaseValueEmbedded(obj, obj.GetType().BaseType);

				SerializeSetV0((IScatterSymbol)obj, info);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (Circle)o ?? new Circle();
				info.GetBaseValueEmbedded(s, s.GetType().BaseType, parent);

				return DeserializeSetV0(s, info, parent);
			}
		}

		#endregion Serialization

		public Circle()
		{
		}

		public Circle(NamedColor fillColor, bool isFillColorInfluencedByPlotColor)
			: base(fillColor, isFillColorInfluencedByPlotColor)
		{
		}

		public override List<List<ClipperLib.IntPoint>> GetCopyOfOuterPolygon()
		{
			double radius = ClipperScalingDouble * 0.797884560802865; // we decrease the radius a little, so that the size of this symbol "feels" roughly the same as for the square
			var list = new List<ClipperLib.IntPoint>(360);
			for (int i = 0; i < 360; ++i)
			{
				var phi = Math.PI * i / 180.0;
				list.Add(new IntPoint((int)(radius * Math.Cos(phi)), (int)(radius * Math.Sin(phi))));
			}

			return new List<List<ClipperLib.IntPoint>>(1) { list };
		}
	}
}
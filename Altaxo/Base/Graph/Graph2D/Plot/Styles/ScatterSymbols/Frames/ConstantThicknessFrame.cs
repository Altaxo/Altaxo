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

using Altaxo.Drawing;
using ClipperLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols.Frames
{
	public class ConstantThicknessFrame : FrameBase
	{
		#region Serialization

		/// <summary>
		/// 2016-10-27 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ConstantThicknessFrame), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				info.AddBaseValueEmbedded(obj, obj.GetType().BaseType);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (ConstantThicknessFrame)o ?? new ConstantThicknessFrame();
				info.GetBaseValueEmbedded(s, s.GetType().BaseType, parent);
				return s;
			}
		}

		#endregion Serialization

		public override List<List<IntPoint>> GetCopyOfClipperPolygon(double relativeWidth, List<List<IntPoint>> outerPolygon)
		{
			var delta = (-2 * relativeWidth) * ScatterSymbolBase.ClipperScalingInt;
			var clipper = new ClipperOffset();
			clipper.AddPaths(outerPolygon, JoinType.jtMiter, EndType.etClosedPolygon);
			var result = new List<List<IntPoint>>();
			clipper.Execute(ref result, delta);
			return result;
		}
	}
}
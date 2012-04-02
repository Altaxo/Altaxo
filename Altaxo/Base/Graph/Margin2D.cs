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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph
{
	[Serializable]
	public struct Margin2D : IEquatable<Margin2D>
	{
		public double Left { get; set; }
		public double Top { get; set; }
		public double Right { get; set; }
		public double Bottom { get; set; }


		#region Serialization


		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Margin2D), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (Margin2D)obj;

				info.AddValue("Left", s.Left);
				info.AddValue("Top", s.Top);
				info.AddValue("Right", s.Right);
				info.AddValue("Bottom", s.Bottom);

			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{

				var s = null != o ? (Margin2D)o : new Margin2D();

				s.Left = info.GetDouble("Left");
				s.Top = info.GetDouble("Top");
				s.Right = info.GetDouble("Right");
				s.Bottom = info.GetDouble("Bottom");

				return s;
			}
		}


		#endregion



		public bool Equals(Margin2D other)
		{
			return this.Left == other.Left && this.Top == other.Top && this.Right == other.Right && this.Bottom == other.Bottom;
		}
	}
}

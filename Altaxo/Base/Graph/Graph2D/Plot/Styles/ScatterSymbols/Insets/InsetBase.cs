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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols.Insets
{
	public abstract class InsetBase : IScatterSymbolInset
	{
		protected const double ClipperScalingDouble = ScatterSymbolBase.ClipperScalingDouble;
		protected const int ClipperScalingInt = ScatterSymbolBase.ClipperScalingInt;

		protected NamedColor _color = NamedColors.White;

		#region Serialization

		/// <summary>
		/// 2016-10-27 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(InsetBase), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (InsetBase)obj;
				info.AddValue("Color", s._color);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (InsetBase)o;
				s._color = (NamedColor)info.GetValue("Color", s);
				return s;
			}
		}

		#endregion Serialization

		public abstract List<List<ClipperLib.IntPoint>> GetCopyOfClipperPolygon(double relativeWidth);

		public NamedColor Color
		{
			get
			{
				return _color;
			}
		}

		public InsetBase WithColor(NamedColor value)
		{
			if (_color == value)
			{
				return this;
			}
			else
			{
				var result = (InsetBase)this.MemberwiseClone();
				result._color = value;
				return result;
			}
		}

		public override bool Equals(object obj)
		{
			return this.GetType() == obj?.GetType() && this._color == ((InsetBase)obj)._color;
		}

		public override int GetHashCode()
		{
			return this.GetType().GetHashCode() + 17 * _color.GetHashCode();
		}
	}
}
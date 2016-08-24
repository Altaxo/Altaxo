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
using Altaxo.Drawing.D3D;
using Altaxo.Graph;
using Altaxo.Graph.Graph3D.Plot.Styles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Drawing.DashPatternManagement
{
	public class DashPatternList : StyleListBase<IDashPattern>
	{
		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DashPatternList), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (DashPatternList)obj;
				info.AddValue("Name", s._name);
				info.CreateArray("Elements", s._list.Count);
				foreach (var ele in s)
					info.AddValue("e", ele);
				info.CommitArray();
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				string name = info.GetString("Name");
				int count = info.OpenArray("Elements");
				var list = new List<IDashPattern>(count);
				for (int i = 0; i < count; ++i)
					list.Add((IDashPattern)info.GetValue("e", null));
				info.CloseArray(count);

				var result = new DashPatternList(name, list);
				return result;
			}
		}

		#endregion Serialization

		public DashPatternList(string name, IEnumerable<IDashPattern> symbols)
			: base(name, symbols.Select(instance => (IDashPattern)instance.Clone()))
		{
		}
	}
}
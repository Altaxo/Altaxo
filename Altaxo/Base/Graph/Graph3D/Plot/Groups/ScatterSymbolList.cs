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

using Altaxo.Graph.Graph3D.Plot.Styles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph3D.Plot.Groups
{
	public class ScatterSymbolList : StyleListBase<IScatterSymbol>
	{
		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ScatterSymbolList), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (ScatterSymbolList)obj;
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
				var list = new List<IScatterSymbol>(count);
				for (int i = 0; i < count; ++i)
					list.Add((IScatterSymbol)info.GetValue("e", null));
				info.CloseArray(count);

				var result = new ScatterSymbolList(name, list);
				ScatterSymbolList existingList;
				ScatterSymbolListManager.Instance.TryRegisterList(result, Main.ItemDefinitionLevel.Project, out existingList);
				return result;
			}
		}

		#endregion Serialization

		public ScatterSymbolList(string name, IEnumerable<IScatterSymbol> symbols)
			: base(name, symbols)
		{
		}
	}
}
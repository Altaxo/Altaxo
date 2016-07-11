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
	public class ScatterSymbolList : Main.IImmutable // TODO NET45 replace IList with IReadonlyList
	{
		private string _name;
		private IList<IScatterSymbol> _list;

		#region Serialization

		private ScatterSymbolList(string name, List<IScatterSymbol> listToTakeDirectly, Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
			_name = name;
			_list = listToTakeDirectly.AsReadOnly();
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ScatterSymbolList), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (ScatterSymbolList)obj;
				info.AddValue("Name", s._name);
				info.CreateArray("Elements", s._list.Count);
				foreach (var ele in s._list)
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
				ScatterSymbolListManager.Instance.TryRegisterInstance(result, out existingList);
				return result;
			}
		}

		#endregion Serialization

		public ScatterSymbolList(string name, IEnumerable<IScatterSymbol> symbols)
		{
			_name = name;
			_list = new List<IScatterSymbol>(symbols).AsReadOnly();
		}

		public string Name { get { return _name; } }

		public IList<IScatterSymbol> Items { get { return _list; } }

		public static bool AreListsStructuralEquivalent(IList<IScatterSymbol> l1, IList<IScatterSymbol> l2)
		{
			if (l1 == null || l2 == null)
				return false;

			if (l1.Count != l2.Count)
				return false;

			for (int i = l1.Count - 1; i >= 0; --i)
			{
				if (!l1[i].Equals(l2[i]))
					return false;
			}

			return true;
		}

		public static bool AreListsStructuralEquivalent(IEnumerable<IScatterSymbol> l1, IList<IScatterSymbol> l2)
		{
			if (l1 == null || l2 == null)
				return false;

			int i = 0;
			int len2 = l2.Count;
			foreach (var item1 in l1)
			{
				if (i >= len2)
					return false;

				if (!item1.Equals(l2[i]))
					return false;
				++i;
			}

			if (i != l2.Count)
				return false;

			return true;
		}
	}
}
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

namespace Altaxo.Graph
{
	/// <summary>
	/// Used to store user color sets in the user's settings.
	/// </summary>
	public class StyleListBag<TList, TItem>
		where TList : IStyleList<TItem>
		where TItem : Altaxo.Main.IImmutable
	{
		/// <summary>
		/// The color sets. One tupe consist of the color set and a bool indication whether this is a plot color set.
		/// </summary>
		protected TList[] _styleLists;

		public StyleListBag(IEnumerable<TList> styleLists)
		{
			_styleLists = styleLists.ToArray();
		}

		public IEnumerable<TList> StyleLists
		{
			get
			{
				return _styleLists;
			}
		}

		#region Serialization Helper

		public void Serialize(Altaxo.Serialization.Xml.IXmlSerializationInfo info)
		{
			info.CreateArray("StyleLists", _styleLists.Length);
			foreach (var list in _styleLists)
			{
				info.AddValue("e", list);
			}

			info.CommitArray();
		}

		protected StyleListBag(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
		{
			int count = info.OpenArray("StyleLists");
			var lists = new TList[count];
			for (int i = 0; i < count; ++i)
			{
				lists[i] = (TList)info.GetValue("e", null);
			}
			info.CloseArray(count);
			_styleLists = lists;
		}

		#endregion Serialization Helper
	}
}
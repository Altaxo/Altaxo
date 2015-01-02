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

#endregion Copyright

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Scripting
{
	public class FitFunctionScriptCollection
		:
		Main.SuspendableDocumentNodeWithSetOfEventArgs,
		System.Collections.ICollection
	{
		private Hashtable _InnerList = new Hashtable();

		public FitFunctionScriptCollection(Main.IDocumentNode parent)
		{
			_parent = parent;
		}

		public List<FitFunctionScript> Find(string category, string name)
		{
			List<FitFunctionScript> result = new List<FitFunctionScript>();

			foreach (FitFunctionScript script in this)
			{
				if (name != null && script.FitFunctionName != name)
					continue;
				if (category != null && script.FitFunctionCategory != category)
					continue;
				result.Add(script);
			}

			result.Sort(FitFunctionScriptDateTimeComparer);

			return result;
		}

		private int FitFunctionScriptDateTimeComparer(FitFunctionScript x, FitFunctionScript y)
		{
			return DateTime.Compare(x.CreationTime, y.CreationTime);
		}

		public void Add(FitFunctionScript script)
		{
			if (!Contains(script))
				_InnerList.Add(script, null);
		}

		public bool Contains(FitFunctionScript script)
		{
			return _InnerList.Contains(script);
		}

		public void Remove(FitFunctionScript script)
		{
			if (_InnerList.Contains(script))
				_InnerList.Remove(script);
		}

		#region ICollection Members

		public void CopyTo(Array array, int index)
		{
			_InnerList.Keys.CopyTo(array, index);
		}

		public int Count
		{
			get { return _InnerList.Count; }
		}

		public bool IsSynchronized
		{
			get { return _InnerList.IsSynchronized; }
		}

		public object SyncRoot
		{
			get { return _InnerList.SyncRoot; }
		}

		#endregion ICollection Members

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return _InnerList.Keys.GetEnumerator();
		}

		#endregion IEnumerable Members
	}
}
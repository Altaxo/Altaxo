#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

using Altaxo.Scripting;
using System;
using System.Collections.Generic;

namespace Altaxo.Data
{
	public class ColumnScriptCollection
		:
		Main.SuspendableDocumentNodeWithSetOfEventArgs,
		IDictionary<DataColumn, IColumnScriptText>
	{
		/// <summary>
		/// ColumnScripts, key is the corresponding column, value is of type WorksheetColumnScript
		/// </summary>
		protected Dictionary<DataColumn, IColumnScriptText> _innerDict = new Dictionary<DataColumn, IColumnScriptText>();

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _innerDict)
			{
				foreach (var item in _innerDict)
				{
					yield return new Main.DocumentNodeAndName(item.Value, item.Key.Name);
				}
			}
		}

		protected override void Dispose(bool isDisposing)
		{
			var d = _innerDict;
			if (null != d && d.Count > 0)
			{
				_innerDict = new Dictionary<DataColumn, IColumnScriptText>();

				foreach (var item in d)
					item.Value?.Dispose();
			}

			base.Dispose(isDisposing);
		}

		#region IDictionary<DataColumn, IColumnScriptText>

		public void Add(DataColumn key, IColumnScriptText value)
		{
			_innerDict.Add(key, value);
			value.ParentObject = this;
			EhSelfChanged(EventArgs.Empty);
		}

		public bool ContainsKey(DataColumn key)
		{
			return _innerDict.ContainsKey(key);
		}

		public ICollection<DataColumn> Keys
		{
			get { return _innerDict.Keys; }
		}

		public bool Remove(DataColumn key)
		{
			IColumnScriptText oldValue;
			if (_innerDict.TryGetValue(key, out oldValue))
			{
				_innerDict.Remove(key);
				oldValue.Dispose();
				EhSelfChanged(EventArgs.Empty);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool TryGetValue(DataColumn key, out IColumnScriptText value)
		{
			return _innerDict.TryGetValue(key, out value);
		}

		public ICollection<IColumnScriptText> Values
		{
			get { return _innerDict.Values; }
		}

		public IColumnScriptText this[DataColumn key]
		{
			get
			{
				return _innerDict[key];
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException("value");

				IColumnScriptText oldValue;
				_innerDict.TryGetValue(key, out oldValue);

				if (object.ReferenceEquals(oldValue, value))
					return;

				_innerDict[key] = value;
				value.ParentObject = this;

				if (null != oldValue)
					oldValue.Dispose();
			}
		}

		public void Add(KeyValuePair<DataColumn, IColumnScriptText> item)
		{
			Add(item.Key, item.Value);
		}

		public void Clear()
		{
			var d = _innerDict;
			_innerDict = new Dictionary<DataColumn, IColumnScriptText>();

			foreach (var item in d)
			{
				item.Value.Dispose();
			}

			EhSelfChanged(EventArgs.Empty);
		}

		public bool Contains(KeyValuePair<DataColumn, IColumnScriptText> item)
		{
			return _innerDict.ContainsKey(item.Key);
		}

		public void CopyTo(KeyValuePair<DataColumn, IColumnScriptText>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get { return _innerDict.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(KeyValuePair<DataColumn, IColumnScriptText> item)
		{
			return Remove(item.Key);
		}

		public IEnumerator<KeyValuePair<DataColumn, IColumnScriptText>> GetEnumerator()
		{
			return _innerDict.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _innerDict.GetEnumerator();
		}

		#endregion IDictionary<DataColumn, IColumnScriptText>
	}
}
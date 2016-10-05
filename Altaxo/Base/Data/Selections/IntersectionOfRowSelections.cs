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

using Altaxo.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Data.Selections
{
	public class IntersectionOfRowSelections : Main.SuspendableDocumentNodeWithEventArgs, IRowSelection, IRowSelectionCollection
	{
		private List<IRowSelection> _rowSelections = new List<IRowSelection>();

		#region Serialization

		/// <summary>
		/// 2016-09-26 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(IntersectionOfRowSelections), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (IntersectionOfRowSelections)obj;

				info.CreateArray("RowSelections", s._rowSelections.Count);

				for (int i = 0; i < s._rowSelections.Count; ++i)
					info.AddValue("e", s._rowSelections[i]);

				info.CommitArray();
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				int count = info.OpenArray("RowSelections");

				var list = new List<IRowSelection>();

				for (int i = 0; i < count; ++i)
				{
					list.Add((IRowSelection)info.GetValue("e", parent));
				}

				info.CloseArray(count);

				return new IntersectionOfRowSelections(info, list);
			}
		}

		#endregion Serialization

		protected IntersectionOfRowSelections(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, List<IRowSelection> list)
		{
			_rowSelections = list;
		}

		public IntersectionOfRowSelections()
		{
			_rowSelections = new List<IRowSelection>();
		}

		public IntersectionOfRowSelections(IEnumerable<IRowSelection> rowSelections)
		{
			_rowSelections = new List<IRowSelection>(rowSelections.Select(item => { var result = (IRowSelection)item.Clone(); item.ParentObject = this; return item; }));
		}

		public IntersectionOfRowSelections(IEnumerable<IRowSelection> rowSelectionsHead, IRowSelection selection, IEnumerable<IRowSelection> rowSelectionTail)
		{
			_rowSelections = new List<IRowSelection>(rowSelectionsHead.Select(item => { var result = (IRowSelection)item.Clone(); item.ParentObject = this; return item; }));

			{
				var item = (IRowSelection)selection.Clone();
				item.ParentObject = this;
				_rowSelections.Add(item);
			}

			_rowSelections.AddRange(rowSelectionTail.Select(item => { var result = (IRowSelection)item.Clone(); item.ParentObject = this; return item; }));
		}

		public object Clone()
		{
			return new IntersectionOfRowSelections(this);
		}

		/// <inheritdoc/>
		public IEnumerable<int> GetSelectedRowIndicesFromTo(int startIndex, int maxIndex, DataColumnCollection table, int totalRowCount)
		{
			IEnumerator<int>[] _enumerators = new IEnumerator<int>[_rowSelections.Count];

			int maxCurrentIndex = 0;
			int currentIndex0 = 0;
			bool isUniform = false;

			for (int i = 0; i < _rowSelections.Count; ++i)
			{
				_enumerators[i] = _rowSelections[i].GetSelectedRowIndicesFromTo(startIndex, maxIndex, table, totalRowCount).GetEnumerator();
			}

			for (;;)
			{
				for (int i = 0; i < _rowSelections.Count; ++i)
				{
					if (!_enumerators[i].MoveNext())
						goto BreakEnumeration; // if one enumerator has no more items, we can end this enumeration

					if (i == 0)
					{
						currentIndex0 = maxCurrentIndex = _enumerators[i].Current;
						isUniform = true;
					}
					else
					{
						isUniform &= (_enumerators[i].Current == currentIndex0);
						maxCurrentIndex = Math.Max(_enumerators[i].Current, maxCurrentIndex);
					}
				}

				if (!(maxCurrentIndex <= maxIndex))
					goto BreakEnumeration; // end of range is reached

				if (isUniform) // all enumerators have the same current value -> thus we can yield this value;
				{
					yield return currentIndex0;
					continue; // and we try to move all enumerators to the next item.
				}
				else // not uniform
				{
					for (int i = 0; i < _rowSelections.Count; ++i)
					{
						_enumerators[i].Dispose(); // dispose old enumerators
						_enumerators[i] = _rowSelections[i].GetSelectedRowIndicesFromTo(maxCurrentIndex, maxIndex, table, totalRowCount).GetEnumerator(); // use new enumerators which start at maxCurrentItem
					}
				}
			}

		BreakEnumeration:

			for (int i = 0; i < _rowSelections.Count; ++i)
			{
				_enumerators[i].Dispose();
			}
		}

		public IRowSelectionCollection WithAdditionalItem(IRowSelection item)
		{
			return new IntersectionOfRowSelections(_rowSelections.Concat(new[] { item }));
		}

		public IRowSelectionCollection WithChangedItem(int idx, IRowSelection item)
		{
			return new IntersectionOfRowSelections(_rowSelections.Take(idx), item, _rowSelections.Skip(idx + 1));
		}

		public IEnumerator<IRowSelection> GetEnumerator()
		{
			return _rowSelections.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _rowSelections.GetEnumerator();
		}

		public IRowSelectionCollection NewWithItems(IEnumerable<IRowSelection> items)
		{
			return new IntersectionOfRowSelections(items);
		}

		protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			for (int i = 0; i < _rowSelections.Count; ++i)
			{
				yield return new DocumentNodeAndName(_rowSelections[i], () => _rowSelections[i] = null, string.Format("RowSelection[{i}], i"));
			}
		}

		/// <summary>
		/// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
		/// to change a plot so that the plot items refer to another table.
		/// </summary>
		/// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
		public void VisitDocumentReferences(DocNodeProxyReporter Report)
		{
			for (int i = 0; i < _rowSelections.Count; ++i)
			{
				_rowSelections[i].VisitDocumentReferences(Report);
			}
		}
	}
}
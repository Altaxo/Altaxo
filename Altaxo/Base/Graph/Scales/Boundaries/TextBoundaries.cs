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

using Altaxo.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Scales.Boundaries
{
	[Serializable]
	public class TextBoundaries : Main.SuspendableDocumentLeafNodeWithSingleAccumulatedData<BoundariesChangedEventArgs>, IPhysicalBoundaries
	{
		private AltaxoSet<string> _itemList;

		[NonSerialized]
		protected int _savedNumberOfItems;

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(TextBoundaries), 10)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				TextBoundaries s = (TextBoundaries)obj;
				info.CreateArray("Items", s._itemList.Count);
				foreach (string name in s._itemList)
					info.AddValue("e", name);
				info.CommitArray();
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				TextBoundaries s = null != o ? (TextBoundaries)o : new TextBoundaries();

				int count = info.OpenArray("Items");
				for (int i = 0; i < count; i++)
					s._itemList.Add(info.GetString("e"));
				info.CloseArray(count);

				return s;
			}
		}

		public TextBoundaries()
		{
			_itemList = new AltaxoSet<string>();
		}

		public TextBoundaries(TextBoundaries from)
		{
			_itemList = new AltaxoSet<string>();
			using (var suspendToken = SuspendGetToken())
			{
				foreach (string s in from._itemList)
					_itemList.Add(s);

				suspendToken.Resume();
			}
		}

		/// <summary>
		/// Try to find the text item and returns the index in the collection. If the
		/// item is not found, the function returns -1.
		/// </summary>
		/// <param name="item">The text item to find.</param>
		/// <returns>The ordinal number  or double.NaN if the item is not found.</returns>
		public int IndexOf(string item)
		{
			return _itemList.IndexOf(item);
		}

		public string GetItem(int i)
		{
			return _itemList[i];
		}

		#region AbstractPhysicalBoundaries implementation

		/// <summary>
		/// Processes a single value from a data column.
		/// If the data value is text, the boundaries are
		/// updated and the number of items is increased by one (if not contained already). The function returns true
		/// in this case. On the other hand, if the value is outside the range, the function has to
		/// return false.
		/// </summary>
		/// <param name="col">The data column</param>
		/// <param name="idx">The index into this data column where the data value is located.</param>
		/// <returns>True if data is in the tracked range, false if the data is not in the tracked range.</returns>
		public bool Add(Altaxo.Data.IReadableColumn col, int idx)
		{
			return Add(col[idx]);
		}

		/// <summary>
		/// Processes a single value .
		/// If the data value is text, the boundaries are
		/// updated and the number of items is increased by one (if not contained already). The function returns true
		/// in this case. On the other hand, if the value is outside the range, the function has to
		/// return false.
		/// </summary>
		/// <param name="item">The data item.</param>
		/// <returns>True if data is in the tracked range, false if the data is not in the tracked range.</returns>
		public bool Add(Altaxo.Data.AltaxoVariant item)
		{
			if (!(item.IsType(Altaxo.Data.AltaxoVariant.Content.VString)))
				return false;

			string s = item.ToString();

			if (IsSuspended) // when suspended: performance tweak, see overrides OnSuspended and OnResume for details (if suspended, we have saved the state of the instance for comparison when we resume).
			{
				if (!_itemList.Contains(s))
					_itemList.Add(s);
			}
			else  // not suspended: normal behaviour with change notification
			{
				if (!_itemList.Contains(s))
				{
					_itemList.Add(s);
					EhSelfChanged(new BoundariesChangedEventArgs(BoundariesChangedData.NumberOfItemsChanged | BoundariesChangedData.UpperBoundChanged));
				}
			}
			return true;
		}

		public void Add(IPhysicalBoundaries b)
		{
			if (b is TextBoundaries)
			{
				using (var suspendToken = SuspendGetToken())
				{
					TextBoundaries from = (TextBoundaries)b;
					foreach (string s in from._itemList)
					{
						if (!_itemList.Contains(s))
							_itemList.Add(s);
					}
					suspendToken.Resume();
				}
			}
		}

		public object Clone()
		{
			return new TextBoundaries(this);
		}

		#endregion AbstractPhysicalBoundaries implementation

		#region IPhysicalBoundaries Members

		public void Reset()
		{
			_itemList.Clear();
		}

		public int NumberOfItems
		{
			get
			{
				return _itemList.Count;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return _itemList.Count == 0;
			}
		}

		#endregion IPhysicalBoundaries Members

		#region Changed event handling

		/// <summary>
		/// For performance reasons, we save the current state of this instance here if the item is suspended. When the item is resumed, we compare the saved state
		/// with the current state and set our accumulated data accordingly.
		/// </summary>
		protected override void OnSuspended()
		{
			this._savedNumberOfItems = this._itemList.Count;

			base.OnSuspended();
		}

		/// <summary>
		/// For performance reasons, we don't call EhSelfChanged during the suspended state. Instead, when we resume here, we compare the saved state of this instance with the current state of the instance
		/// and and set our accumulated data accordingly.
		/// </summary>
		protected override void OnResume()
		{
			BoundariesChangedData data = 0;
			// if anything changed in the meantime, fire the event
			if (this._savedNumberOfItems != this._itemList.Count)
			{
				data |= BoundariesChangedData.NumberOfItemsChanged;
				data |= BoundariesChangedData.UpperBoundChanged;
			}

			if (0 != data)
				_accumulatedEventData = new BoundariesChangedEventArgs(data);

			base.OnResume();
		}

		protected override void AccumulateChangeData(object sender, EventArgs e)
		{
			var eAsBCEA = e as BoundariesChangedEventArgs;
			if (null == eAsBCEA)
				throw new ArgumentOutOfRangeException(string.Format("Argument e should be of type {0}, but is {1}", typeof(BoundariesChangedEventArgs), e.GetType()));

			if (null == _accumulatedEventData)
				_accumulatedEventData = eAsBCEA;
			else
				_accumulatedEventData.Add(eAsBCEA);
		}

		#endregion Changed event handling
	}
}
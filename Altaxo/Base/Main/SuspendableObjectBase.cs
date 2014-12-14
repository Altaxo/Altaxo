#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
	/// <summary>
	/// Stores a set of event data. A special function exist to either set the item, if it is not accumulateable, or to accumulate the event data.
	/// </summary>
	public interface ISetOfEventData : ICollection<EventArgs>
	{
		/// <summary>
		/// Puts the specified item in the collection, regardless whether it is already contained or not. If it is not already contained, it is added to the collection.
		/// If it is already contained, and is of type <see cref="SelfAccumulateableEventArgs"/>, the <see cref="SelfAccumulateableEventArgs.Add"/> function is used to add the item to the already contained item.
		/// </summary>
		/// <param name="item">The <see cref="EventArgs"/> instance containing the event data.</param>
		void SetOrAccumulate(EventArgs item);
	}

	public class SuspendableObjectBase
	{
		#region Implementation of a set of accumulated event data

		protected class SetOfEventData : Dictionary<EventArgs, EventArgs>, ISetOfEventData
		{
			/// <summary>
			/// Puts the specified item in the collection, regardless whether it is already contained or not. If it is not already contained, it is added to the collection.
			/// If it is already contained, and is of type <see cref="SelfAccumulateableEventArgs"/>, the <see cref="SelfAccumulateableEventArgs.Add"/> function is used to add the item to the already contained item.
			/// </summary>
			/// <param name="item">The <see cref="EventArgs"/> instance containing the event data.</param>
			public void SetOrAccumulate(EventArgs item)
			{
				EventArgs containedItem;
				if (base.TryGetValue(item, out containedItem))
				{
					var containedAsSelf = containedItem as SelfAccumulateableEventArgs;
					if (null != containedAsSelf)
						containedAsSelf.Add((SelfAccumulateableEventArgs)item);
				}
				else // not in the collection already
				{
					base.Add(item, item);
				}
			}

			public void Add(EventArgs item)
			{
				this.Add(item, item);
			}

			public bool Contains(EventArgs item)
			{
				return base.ContainsKey(item);
			}

			public void CopyTo(EventArgs[] array, int arrayIndex)
			{
				base.Values.CopyTo(array, arrayIndex);
			}

			public bool IsReadOnly
			{
				get { return false; }
			}

			public new IEnumerator<EventArgs> GetEnumerator()
			{
				return base.Values.GetEnumerator();
			}
		}

		#endregion Implementation of a set of accumulated event data
	}
}
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

using Altaxo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Scales.Ticks
{
	public class SuppressedTicks
		:
		Main.SuspendableDocumentLeafNodeWithEventArgs,
		Main.ICopyFrom
	{
		private List<AltaxoVariant> _suppressedTickValues;
		private List<int> _suppressedTicksByNumber;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SuppressedTicks), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				SuppressedTicks s = (SuppressedTicks)obj;

				info.CreateArray("ByValues", s._suppressedTickValues.Count);
				foreach (AltaxoVariant v in s._suppressedTickValues)
					info.AddValue("e", (object)v);
				info.CommitArray();

				info.CreateArray("ByNumbers", s._suppressedTicksByNumber.Count);
				foreach (int v in s._suppressedTicksByNumber)
					info.AddValue("e", v);
				info.CommitArray();
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				SuppressedTicks s = SDeserialize(o, info, parent);
				return s;
			}

			protected virtual SuppressedTicks SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				SuppressedTicks s = null != o ? (SuppressedTicks)o : new SuppressedTicks();

				int count;

				count = info.OpenArray("ByValues");
				for (int i = 0; i < count; i++)
					s._suppressedTickValues.Add((AltaxoVariant)info.GetValue("e", s));
				info.CloseArray(count);

				count = info.OpenArray("ByNumbers");
				for (int i = 0; i < count; i++)
					s._suppressedTicksByNumber.Add(info.GetInt32("e"));
				info.CloseArray(count);

				return s;
			}
		}

		#endregion Serialization

		public SuppressedTicks()
		{
			_suppressedTickValues = new List<AltaxoVariant>();
			_suppressedTicksByNumber = new List<int>();
		}

		public SuppressedTicks(SuppressedTicks from)
		{
			CopyFrom(from);
		}

		public virtual bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as SuppressedTicks;
			if (null != from)
			{
				_suppressedTickValues = new List<AltaxoVariant>(from._suppressedTickValues);
				_suppressedTicksByNumber = new List<int>(from._suppressedTicksByNumber);
				return true;
			}
			else
			{
				return false;
			}
		}

		public object Clone()
		{
			return new SuppressedTicks(this);
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			else if (!(obj is SuppressedTicks))
				return false;
			else
			{
				var from = (SuppressedTicks)obj;
				if (!_suppressedTicksByNumber.SequenceEqual(from._suppressedTicksByNumber))
					return false;

				if (!_suppressedTickValues.SequenceEqual(from._suppressedTickValues))
					return false;
			}
			return true;
		}

		public bool IsEmpty
		{
			get
			{
				return _suppressedTickValues.Count == 0 && _suppressedTicksByNumber.Count == 0;
			}
		}

		public IList<AltaxoVariant> ByValues
		{
			get
			{
				return _suppressedTickValues;
			}
		}

		public IList<int> ByNumbers
		{
			get
			{
				return _suppressedTicksByNumber;
			}
		}

		private class DescendingIntComparer : IComparer<int>
		{
			private Comparer<int> _comparer = Comparer<int>.Default;

			public int Compare(int x, int y)
			{
				return _comparer.Compare(y, x);
			}
		}

		/// <summary>Gets all suppressed tick numbers in descending order with negative numbers transformed to positive numbers.</summary>
		/// <param name="totalNumberOfTicks">The total number of ticks. This is used to transform negative numbers to real, positive, tick numbers.</param>
		/// <returns>A list of suppressed tick numbers (all positive) in descending order.</returns>
		public ICollection<int> GetValidTickNumbersDecendingWithNegativeNumbersTransformed(int totalNumberOfTicks)
		{
			SortedSet<int> result = new SortedSet<int>(new DescendingIntComparer());
			foreach (int idx in _suppressedTicksByNumber)
			{
				if (idx >= 0 && idx < totalNumberOfTicks)
					result.Add(idx);
				else if (idx < 0 && totalNumberOfTicks + idx >= 0)
					result.Add(totalNumberOfTicks + idx);
			}
			return result;
		}

		/// <summary>Removes the suppressed ticks from the list given as argument.</summary>
		/// <param name="ticks">The tick list. At return, the suppressed ticks are removed from that list.</param>
		public void RemoveSuppressedTicks(IList<double> ticks)
		{
			// Remove suppressed ticks
			if (_suppressedTicksByNumber.Count > 0)
			{
				var suppressedTicksDescending = GetValidTickNumbersDecendingWithNegativeNumbersTransformed(ticks.Count);
				foreach (var i in suppressedTicksDescending)
				{
					ticks.RemoveAt(i);
				}
			}

			if (_suppressedTickValues.Count > 0)
			{
				for (int i = ticks.Count - 1; i >= 0; --i)
				{
					if (_suppressedTickValues.Contains(ticks[i]))
						ticks.RemoveAt(i);
				}
			}
		}

		/// <summary>Removes the suppressed ticks from the list given as argument.</summary>
		/// <param name="ticks">The tick list. At return, the suppressed ticks are removed from that list.</param>
		public void RemoveSuppressedTicks(IList<AltaxoVariant> ticks)
		{
			// Remove suppressed ticks
			if (_suppressedTicksByNumber.Count > 0)
			{
				var suppressedTicksDescending = GetValidTickNumbersDecendingWithNegativeNumbersTransformed(ticks.Count);
				foreach (var i in suppressedTicksDescending)
				{
					ticks.RemoveAt(i);
				}
			}

			if (_suppressedTickValues.Count > 0)
			{
				for (int i = ticks.Count - 1; i >= 0; --i)
				{
					if (_suppressedTickValues.Contains(ticks[i]))
						ticks.RemoveAt(i);
				}
			}
		}
	}
}
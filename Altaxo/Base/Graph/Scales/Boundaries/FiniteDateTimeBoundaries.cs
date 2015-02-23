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

namespace Altaxo.Graph.Scales.Boundaries
{
	/// <summary>
	/// Provides a class for tracking the boundaries of a plot association where the x-axis is a DateTime axis.
	/// </summary>
	[Serializable]
	public class FiniteDateTimeBoundaries : AbstractPhysicalBoundaries
	{
		protected DateTime _minValue = DateTime.MaxValue;
		protected DateTime _maxValue = DateTime.MinValue;

		[NonSerialized]
		private DateTime _cachedMinValue, _cachedMaxValue; // stores the minValue and MaxValue in the moment if the events where disabled

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Axes.Boundaries.FiniteDateTimeBoundaries", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FiniteDateTimeBoundaries), 1)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				FiniteDateTimeBoundaries s = (FiniteDateTimeBoundaries)obj;
				info.AddValue("NumberOfItems", s._numberOfItems);
				info.AddValue("MinValue", s._minValue);
				info.AddValue("MaxValue", s._maxValue);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				FiniteDateTimeBoundaries s = null != o ? (FiniteDateTimeBoundaries)o : new FiniteDateTimeBoundaries();

				s._numberOfItems = info.GetInt32("NumberOfItems");
				s._minValue = info.GetDateTime("MinValue");
				s._maxValue = info.GetDateTime("MaxValue");

				return s;
			}
		}

		#endregion Serialization

		public FiniteDateTimeBoundaries()
		{
			_minValue = DateTime.MaxValue;
			_maxValue = DateTime.MinValue;
		}

		public FiniteDateTimeBoundaries(FiniteDateTimeBoundaries x)
			: base(x)
		{
			_minValue = x._minValue;
			_maxValue = x._maxValue;
		}

		/// <summary>
		/// Reset the internal data to the initialized state
		/// </summary>
		public override void Reset()
		{
			base.Reset();
			_minValue = DateTime.MaxValue;
			_maxValue = DateTime.MinValue;
		}

		public virtual DateTime LowerBound { get { return _minValue; } }

		public virtual DateTime UpperBound { get { return _maxValue; } }

		/// <summary>
		/// merged boundaries of another object into this object
		/// </summary>
		/// <param name="b">another physical boundary object of the same type as this</param>
		public virtual void Add(FiniteDateTimeBoundaries b)
		{
			if (this.GetType() == b.GetType())
			{
				BoundariesChangedData data = 0;
				if (b._numberOfItems > 0)
				{
					_numberOfItems += b._numberOfItems;

					data |= BoundariesChangedData.NumberOfItemsChanged;
					if (b._minValue < _minValue) { _minValue = b._minValue; data |= BoundariesChangedData.LowerBoundChanged; }
					if (b._maxValue > _maxValue) { _maxValue = b._maxValue; data |= BoundariesChangedData.UpperBoundChanged; }
				}

				if (!IsSuspended && 0 != data) // performance tweak, see overrides OnSuspended and OnResume for details (if suspended, we have saved the state of the instance for comparison when we resume).
				{
					EhSelfChanged(new BoundariesChangedEventArgs(data));
				}
			}
			else
			{
				throw new ArgumentException("Argument has not the same type as this, argument type: " + b.GetType().ToString() + ", this type: " + this.GetType().ToString());
			}
		}

		#region IPhysicalBoundaries Members

		public override void Add(IPhysicalBoundaries b)
		{
			if (b is FiniteDateTimeBoundaries)
				Add((FiniteDateTimeBoundaries)b);
		}

		public override bool Add(Altaxo.Data.IReadableColumn col, int idx)
		{
			// if column is not numeric, use the index instead
			if (!(col is Altaxo.Data.DateTimeColumn))
				return false;
			else
				return Add(((Altaxo.Data.DateTimeColumn)col)[idx]);
		}

		public override bool Add(Altaxo.Data.AltaxoVariant item)
		{
			return Add((DateTime)item);
		}

		public bool Add(DateTime d)
		{
			if (IsSuspended)  // when suspended: performance tweak, see overrides OnSuspended and OnResume for details (if suspended, we have saved the state of the instance for comparison when we resume).
			{
				if (DateTime.MinValue != d)
				{
					if (d < _minValue) _minValue = d;
					if (d > _maxValue) _maxValue = d;
					_numberOfItems++;
					return true;
				}
			}
			else // not suspended: normal behaviour with change notification
			{
				if (DateTime.MinValue != d)
				{
					var data = BoundariesChangedData.NumberOfItemsChanged;

					if (d < _minValue) { _minValue = d; data |= BoundariesChangedData.LowerBoundChanged; }
					if (d > _maxValue) { _maxValue = d; data |= BoundariesChangedData.UpperBoundChanged; }
					_numberOfItems++;

					EhSelfChanged(new BoundariesChangedEventArgs(data));

					return true;
				}
			}

			return false;
		}

		public override object Clone()
		{
			return new FiniteDateTimeBoundaries(this);
		}

		#endregion IPhysicalBoundaries Members

		#region Changed event handling

		protected override void OnSuspended()
		{
			this._savedNumberOfItems = this._numberOfItems;
			this._cachedMinValue = this._minValue;
			this._cachedMaxValue = this._maxValue;

			base.OnSuspended();
		}

		protected override void OnResume()
		{
			BoundariesChangedData data = 0;
			// if anything changed in the meantime, fire the event
			if (this._savedNumberOfItems != this._numberOfItems)
				data |= BoundariesChangedData.NumberOfItemsChanged;

			if (this._cachedMinValue != this._minValue)
				data |= BoundariesChangedData.LowerBoundChanged;
			if (this._cachedMaxValue != this._maxValue)
				data |= BoundariesChangedData.UpperBoundChanged;

			if (0 != data)
				_accumulatedEventData = new BoundariesChangedEventArgs(data);

			base.OnResume();
		}

		#endregion Changed event handling
	}
}
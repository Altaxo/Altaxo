/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002 Dr. Dirk Lellinger
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

using System;
using Altaxo.Serialization;


namespace Altaxo.Graph
{

	public class BoundariesChangedEventArgs : System.EventArgs
	{
		protected bool m_bLowerBoundChanged, m_bUpperBoundChanged;

		public BoundariesChangedEventArgs(bool bLowerBound, bool bUpperBound)
		{
			this.m_bLowerBoundChanged = bLowerBound;
			this.m_bUpperBoundChanged = bUpperBound;
		}

		public bool LowerBoundChanged 
		{
			get { return this.m_bLowerBoundChanged; }
		}

		public bool UpperBoundChanged 
		{
			get { return this.m_bUpperBoundChanged; }
		}
	}

	/// <summary>
	/// PhysicalBoundaries provides a abstract class for tracking the physical
	/// boundaries of a plot association. Every plot association has two of these objects
	/// that help tracking the boundaries of X and Y axis
	/// </summary>
	[SerializationSurrogate(0,typeof(PhysicalBoundaries.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public abstract class PhysicalBoundaries : ICloneable, System.Runtime.Serialization.IDeserializationCallback
	{
		protected int numberOfItems=0;
		protected double minValue=double.MaxValue;
		protected double maxValue=double.MinValue;
	
		private int 		m_EventsSuspendCount=0;
		private double	m_SavedMinValue, m_SavedMaxValue; // stores the minValue and MaxValue in the moment if the events where disabled
		private int			m_SavedNumberOfItems; // stores the number of items when events are disabled


		public delegate void BoundaryChangedHandler(object sender, BoundariesChangedEventArgs args);
		public delegate void ItemNumberChangedHandler(object sender, System.EventArgs args);

		public event BoundaryChangedHandler		BoundaryChanged;
		public event ItemNumberChangedHandler NumberOfItemsChanged;

		#region Serialization
		/// <summary>Used to serialize the PhysicalBoundaries Version 0.</summary>
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			/// <summary>
			/// Serializes PhysicalBoundaries Version 0.
			/// </summary>
			/// <param name="obj">The PhysicalBoundaries to serialize.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				PhysicalBoundaries s = (PhysicalBoundaries)obj;
				info.AddValue("NumberOfItems",s.numberOfItems);
				info.AddValue("MinValue",s.minValue);
				info.AddValue("MaxValue",s.maxValue);
			}
			/// <summary>
			/// Deserializes the PhysicalBoundaries Version 0.
			/// </summary>
			/// <param name="obj">The empty PhysicalBoundaries object to deserialize into.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <param name="selector">The deserialization surrogate selector.</param>
			/// <returns>The deserialized PhysicalBoundaries.</returns>
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				PhysicalBoundaries s = (PhysicalBoundaries)obj;

				s.numberOfItems = info.GetInt32("NumberOfItems");  
				s.minValue = info.GetDouble("MinValue");
				s.maxValue = info.GetDouble("MaxValue");

				return s;
			}
		}

		/// <summary>
		/// Finale measures after deserialization.
		/// </summary>
		/// <param name="obj">Not used.</param>
		public virtual void OnDeserialization(object obj)
		{
		}
		#endregion



		public PhysicalBoundaries()
		{
			numberOfItems = 0;
			minValue = double.MaxValue;
			maxValue = double.MinValue;
		}

		public PhysicalBoundaries(PhysicalBoundaries x)
		{
			numberOfItems = x.numberOfItems;
			minValue      = x.minValue;
			maxValue      = x.maxValue;
		}

		public bool EventsEnabled
		{
			get { return m_EventsSuspendCount<=0; }
		}


		public void EndUpdate()
		{
			if(m_EventsSuspendCount>0)
			{
				--m_EventsSuspendCount;
				// if anything changed in the meantime, fire the event
				if(this.m_SavedNumberOfItems!=this.numberOfItems)
					OnNumberOfItemsChanged();

				bool bLower = (this.m_SavedMinValue!=this.minValue);
				bool bUpper = (this.m_SavedMaxValue!=this.maxValue);

				if(bLower || bUpper)
					OnBoundaryChanged(bLower,bUpper);
			}
		}

		public void BeginUpdate()
		{
			++m_EventsSuspendCount;
			if(m_EventsSuspendCount==1) // events are freshly disabled
			{
					this.m_SavedNumberOfItems = this.numberOfItems;
					this.m_SavedMinValue = this.minValue;
					this.m_SavedMaxValue = this.maxValue;
			}
		}
	
		/// <summary>
		/// Processes a single value from a numeric column <paramref name="col"/>[<paramref name="idx"/>].
		/// If the data value is inside the considered value range, the boundaries are
		/// updated and the number of items is increased by one. The function has to return true
		/// in this case. On the other hand, if the value is outside the range, the function has to
		/// return false.
		/// </summary>
		/// <param name="col">The numeric data column</param>
		/// <param name="idx">The index into this numeric column where the data value is located</param>
		/// <returns>True if data is in the tracked range, false if the data is not in the tracked range.</returns>
		public abstract bool Add(Altaxo.Data.IReadableColumn col, int idx);

		public abstract object Clone();

		/// <summary>
		/// Reset the internal data to the initialized state
		/// </summary>
		public virtual void Reset()
		{
			numberOfItems = 0;
			minValue = Double.MaxValue;
			maxValue = Double.MinValue;
		}


		public virtual int NumberOfItems	{	get	{	return numberOfItems;	}	}
		public virtual double LowerBound { get { return minValue; } }
		public virtual double UpperBound { get { return maxValue; } }
		public virtual bool IsEmpty { get { return numberOfItems==0; } }

		/// <summary>
		/// merged boundaries of another object into this object
		/// </summary>
		/// <param name="b">another physical boundary object of the same type as this</param>
		public virtual void Add(PhysicalBoundaries b)
		{
			if(this.GetType()==b.GetType())
			{
				if(b.numberOfItems>0)
				{
					bool bLower=false,bUpper=false;
					numberOfItems += b.numberOfItems;
					if(b.minValue < minValue) 
					{
						minValue = b.minValue;
						bLower=true;
					}
					if(b.maxValue > maxValue)
					{
						maxValue = b.maxValue;
						bUpper=true;
					}
					
					if(EventsEnabled)
					{
						OnNumberOfItemsChanged(); // fire item number event
						if(bLower||bUpper)
							OnBoundaryChanged(bLower,bUpper);
					}

				}
			}
			else
			{
				throw new ArgumentException("Argument has not the same type as this, argument type: " + b.GetType().ToString() + ", this type: " +this.GetType().ToString());
			}
		}

		protected void OnBoundaryChanged(bool bLowerBoundChanged, bool bUpperBoundChanged)
		{
			if(null!=BoundaryChanged)
				BoundaryChanged(this, new BoundariesChangedEventArgs(bLowerBoundChanged,bUpperBoundChanged));
		}

		protected void OnNumberOfItemsChanged()
		{
			if(null!=NumberOfItemsChanged)
				NumberOfItemsChanged(this, new System.EventArgs());
		}
	}

	/// <summary>
	/// FinitePhysicalBoundaries is intended to use for LinearAxis
	/// it keeps track of the most negative and most positive value
	/// </summary>
	[SerializationSurrogate(0,typeof(FinitePhysicalBoundaries.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class FinitePhysicalBoundaries : PhysicalBoundaries
	{
		#region Serialization
		/// <summary>Used to serialize the FinitePhysicalBoundaries Version 0.</summary>
		public new class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			/// <summary>
			/// Serializes FinitePhysicalBoundaries Version 0.
			/// </summary>
			/// <param name="obj">The FinitePhysicalBoundaries to serialize.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				FinitePhysicalBoundaries s = (FinitePhysicalBoundaries)obj;
				// get the surrogate selector of the base class
				System.Runtime.Serialization.ISurrogateSelector ss;
				System.Runtime.Serialization.ISerializationSurrogate surr =
					App.m_SurrogateSelector.GetSurrogate(obj.GetType().BaseType,context, out ss);

				// serialize the base class
				surr.GetObjectData(obj,info,context); // stream the data of the base object
			}
			/// <summary>
			/// Deserializes the FinitePhysicalBoundaries Version 0.
			/// </summary>
			/// <param name="obj">The empty FinitePhysicalBoundaries object to deserialize into.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <param name="selector">The deserialization surrogate selector.</param>
			/// <returns>The deserialized FinitePhysicalBoundaries.</returns>
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				FinitePhysicalBoundaries s = (FinitePhysicalBoundaries)obj;
				// get the surrogate selector of the base class
				System.Runtime.Serialization.ISurrogateSelector ss;
				System.Runtime.Serialization.ISerializationSurrogate surr =
					App.m_SurrogateSelector.GetSurrogate(obj.GetType().BaseType,context, out ss);
				// deserialize the base class
				surr.SetObjectData(obj,info,context,selector);

				return s;
			}
		}

		/// <summary>
		/// Finale measures after deserialization.
		/// </summary>
		/// <param name="obj">Not used.</param>
		public override void OnDeserialization(object obj)
		{
		}
		#endregion

		public FinitePhysicalBoundaries()
			: base()
		{
		}

		public FinitePhysicalBoundaries(FinitePhysicalBoundaries c)
			: base(c)
		{
		}

		public override object Clone()
		{
			return new FinitePhysicalBoundaries(this);
		}


		public override bool Add(Altaxo.Data.IReadableColumn col, int idx)
		{
			// if column is not numeric, use the index instead
			double d = (col is Altaxo.Data.INumericColumn) ? ((Altaxo.Data.INumericColumn)col).GetDoubleAt(idx) : idx;
	
			if(EventsEnabled)
			{
				if(!double.IsInfinity(d))
				{
					bool bLower=false, bUpper=false;
					if(d<minValue) { minValue = d; bLower=true; }
					if(d>maxValue) { maxValue = d; bUpper=true; }
					numberOfItems++;
	
					OnNumberOfItemsChanged();

					if(bLower || bUpper) 
						OnBoundaryChanged(bLower,bUpper);
	
					return true;
				}
			}
			else // Events not enabled
			{
				if(!double.IsInfinity(d))
				{
					if(d<minValue) minValue = d;
					if(d>maxValue) maxValue = d;
					numberOfItems++;
					return true;
				}
			}
		
	
			return false;
		}
	}


	/// <summary>
	/// PositiveFinitePhysicalBoundaries is intended to use for logarithmic axis
	/// it keeps track of the smallest positive and biggest positive value
	/// </summary>
	[SerializationSurrogate(0,typeof(PositiveFinitePhysicalBoundaries.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class PositiveFinitePhysicalBoundaries : PhysicalBoundaries
	{
		#region Serialization
		/// <summary>Used to serialize the PositiveFinitePhysicalBoundaries Version 0.</summary>
		public new class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			/// <summary>
			/// Serializes PositiveFinitePhysicalBoundaries Version 0.
			/// </summary>
			/// <param name="obj">The PositiveFinitePhysicalBoundaries to serialize.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				PositiveFinitePhysicalBoundaries s = (PositiveFinitePhysicalBoundaries)obj;
				// get the surrogate selector of the base class
				System.Runtime.Serialization.ISurrogateSelector ss;
				System.Runtime.Serialization.ISerializationSurrogate surr =
					App.m_SurrogateSelector.GetSurrogate(obj.GetType().BaseType,context, out ss);

				// serialize the base class
				surr.GetObjectData(obj,info,context); // stream the data of the base object
			}
			/// <summary>
			/// Deserializes the PositiveFinitePhysicalBoundaries Version 0.
			/// </summary>
			/// <param name="obj">The empty PositiveFinitePhysicalBoundaries object to deserialize into.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <param name="selector">The PositiveFinitePhysicalBoundaries surrogate selector.</param>
			/// <returns>The deserialized PositiveFinitePhysicalBoundaries.</returns>
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				PositiveFinitePhysicalBoundaries s = (PositiveFinitePhysicalBoundaries)obj;
				// get the surrogate selector of the base class
				System.Runtime.Serialization.ISurrogateSelector ss;
				System.Runtime.Serialization.ISerializationSurrogate surr =
					App.m_SurrogateSelector.GetSurrogate(obj.GetType().BaseType,context, out ss);
				// deserialize the base class
				surr.SetObjectData(obj,info,context,selector);

				return s;
			}
		}

		/// <summary>
		/// Finale measures after deserialization.
		/// </summary>
		/// <param name="obj">Not used.</param>
		public override void OnDeserialization(object obj)
		{
		}
		#endregion

		public PositiveFinitePhysicalBoundaries()
			: base()
		{
		}

		public PositiveFinitePhysicalBoundaries(PositiveFinitePhysicalBoundaries c)
			: base(c)
		{
		}

		public override object Clone()
		{
			return new PositiveFinitePhysicalBoundaries(this);
		}


		public override bool Add(Altaxo.Data.IReadableColumn col, int idx)
		{
			double d = (col is Altaxo.Data.INumericColumn) ? ((Altaxo.Data.INumericColumn)col).GetDoubleAt(idx) : idx;

			if(EventsEnabled)
			{
				if(d>0 && !double.IsInfinity(d))
				{
					bool bLower=false, bUpper=false;
					if(d<minValue) { minValue = d; bLower=true; }
					if(d>maxValue) { maxValue = d; bUpper=true; }
					numberOfItems++;

					OnNumberOfItemsChanged();

					if(bLower || bUpper) 
						OnBoundaryChanged(bLower,bUpper);
	
					return true;
				}
			}
			else // events disabled
			{
				if(d>0 && !double.IsInfinity(d))
				{
					if(d<minValue) minValue = d;
					if(d>maxValue) maxValue = d;
					numberOfItems++;
					return true;
				}
			}
			return false;
		}

	}
}

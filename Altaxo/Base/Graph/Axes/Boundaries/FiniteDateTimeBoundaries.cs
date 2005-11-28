#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
#endregion

using System;
using Altaxo.Serialization;

namespace Altaxo.Graph.Axes.Boundaries
{
  /// <summary>
  /// Provides a class for tracking the boundaries of a plot association where the x-axis is a DateTime axis.
  /// </summary>
  [SerializationSurrogate(0,typeof(FiniteDateTimeBoundaries.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class FiniteDateTimeBoundaries : AbstractPhysicalBoundaries, System.Runtime.Serialization.IDeserializationCallback
  {
    
    protected DateTime minValue = DateTime.MaxValue;
    protected DateTime maxValue = DateTime.MinValue;
  
    private DateTime  m_SavedMinValue, m_SavedMaxValue; // stores the minValue and MaxValue in the moment if the events where disabled

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
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        FiniteDateTimeBoundaries s = (FiniteDateTimeBoundaries)obj;
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
        FiniteDateTimeBoundaries s = (FiniteDateTimeBoundaries)obj;

        s.numberOfItems = info.GetInt32("NumberOfItems");  
        s.minValue = info.GetDateTime("MinValue");
        s.maxValue = info.GetDateTime("MaxValue");

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FiniteDateTimeBoundaries),0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        FiniteDateTimeBoundaries s = (FiniteDateTimeBoundaries)obj;
        info.AddValue("NumberOfItems",s.numberOfItems);
        info.AddValue("MinValue",s.minValue);
        info.AddValue("MaxValue",s.maxValue);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        FiniteDateTimeBoundaries s = (FiniteDateTimeBoundaries)o;

        s.numberOfItems = info.GetInt32("NumberOfItems");  
        s.minValue = info.GetDateTime("MinValue");
        s.maxValue = info.GetDateTime("MaxValue");

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



    public FiniteDateTimeBoundaries()
    {
      minValue = DateTime.MaxValue;
      maxValue = DateTime.MinValue;
    }

    public FiniteDateTimeBoundaries(FiniteDateTimeBoundaries x)
      : base(x)
    {
      minValue      = x.minValue;
      maxValue      = x.maxValue;
    }

    public override void EndUpdate()
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

    public override void BeginUpdate()
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
    /// Reset the internal data to the initialized state
    /// </summary>
    public override void Reset()
    {
      base.Reset();
      minValue = DateTime.MaxValue;
      maxValue = DateTime.MinValue;
    }


    public virtual DateTime LowerBound { get { return minValue; } }
    public virtual DateTime UpperBound { get { return maxValue; } }

    /// <summary>
    /// merged boundaries of another object into this object
    /// </summary>
    /// <param name="b">another physical boundary object of the same type as this</param>
    public virtual void Add(FiniteDateTimeBoundaries b)
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


    #region IPhysicalBoundaries Members

    public override void Add(IPhysicalBoundaries b)
    {
      if(b is FiniteDateTimeBoundaries)
        Add((FiniteDateTimeBoundaries)b);
    }


    public override bool Add(Altaxo.Data.IReadableColumn col, int idx)
    {
      // if column is not numeric, use the index instead
      if(!(col is Altaxo.Data.DateTimeColumn))
        return false;

      DateTime d = ((Altaxo.Data.DateTimeColumn)col)[idx];
  
      if(EventsEnabled)
      {
        if(DateTime.MinValue!=d)
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
        if(DateTime.MinValue!=d)
        {
          if(d<minValue) minValue = d;
          if(d>maxValue) maxValue = d;
          numberOfItems++;
          return true;
        }
      }
    
  
      return false;
    }

    public override object Clone()
    {
      return new FiniteDateTimeBoundaries(this);
    }

    #endregion
  }


}

#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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

namespace Altaxo.Graph.Scales.Boundaries
{
  /// <summary>
  /// Provides a class for tracking the boundaries of a plot association where the x-axis is a DateTime axis.
  /// </summary>
  [Serializable]
  public class FiniteDateTimeBoundaries : AbstractPhysicalBoundaries, System.Runtime.Serialization.IDeserializationCallback
  {
    
    protected DateTime _minValue = DateTime.MaxValue;
    protected DateTime _maxValue = DateTime.MinValue;
  
    [NonSerialized]
    private DateTime  _cachedMinValue, _cachedMaxValue; // stores the minValue and MaxValue in the moment if the events where disabled

    #region Serialization


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.Axes.Boundaries.FiniteDateTimeBoundaries", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FiniteDateTimeBoundaries),1)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        FiniteDateTimeBoundaries s = (FiniteDateTimeBoundaries)obj;
        info.AddValue("NumberOfItems",s._numberOfItems);
        info.AddValue("MinValue",s._minValue);
        info.AddValue("MaxValue",s._maxValue);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        FiniteDateTimeBoundaries s = null!=o ? (FiniteDateTimeBoundaries)o : new FiniteDateTimeBoundaries();

        s._numberOfItems = info.GetInt32("NumberOfItems");  
        s._minValue = info.GetDateTime("MinValue");
        s._maxValue = info.GetDateTime("MaxValue");

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
      _minValue = DateTime.MaxValue;
      _maxValue = DateTime.MinValue;
    }

    public FiniteDateTimeBoundaries(FiniteDateTimeBoundaries x)
      : base(x)
    {
      _minValue      = x._minValue;
      _maxValue      = x._maxValue;
    }

    public override void EndUpdate()
    {
      if(_eventSuspendCount>0)
      {
        --_eventSuspendCount;
        // if anything changed in the meantime, fire the event
        if(this._savedNumberOfItems!=this._numberOfItems)
          OnNumberOfItemsChanged();

        bool bLower = (this._cachedMinValue!=this._minValue);
        bool bUpper = (this._cachedMaxValue!=this._maxValue);

        if(bLower || bUpper)
          OnBoundaryChanged(bLower,bUpper);
      }
    }

    public override void BeginUpdate()
    {
      ++_eventSuspendCount;
      if(_eventSuspendCount==1) // events are freshly disabled
      {
        this._savedNumberOfItems = this._numberOfItems;
        this._cachedMinValue = this._minValue;
        this._cachedMaxValue = this._maxValue;
      }
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
      if(this.GetType()==b.GetType())
      {
        if(b._numberOfItems>0)
        {
          bool bLower=false,bUpper=false;
          _numberOfItems += b._numberOfItems;
          if(b._minValue < _minValue) 
          {
            _minValue = b._minValue;
            bLower=true;
          }
          if(b._maxValue > _maxValue)
          {
            _maxValue = b._maxValue;
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
      if(EventsEnabled)
      {
        if(DateTime.MinValue!=d)
        {
          bool bLower=false, bUpper=false;
          if(d<_minValue) { _minValue = d; bLower=true; }
          if(d>_maxValue) { _maxValue = d; bUpper=true; }
          _numberOfItems++;
  
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
          if(d<_minValue) _minValue = d;
          if(d>_maxValue) _maxValue = d;
          _numberOfItems++;
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

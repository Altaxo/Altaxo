#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
  /// PositiveFinitePhysicalBoundaries is intended to use for logarithmic axis
  /// it keeps track of the smallest positive and biggest positive value
  /// </summary>
  [SerializationSurrogate(0,typeof(PositiveFiniteNumericalBoundaries.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class PositiveFiniteNumericalBoundaries : NumericalBoundaries
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
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        PositiveFiniteNumericalBoundaries s = (PositiveFiniteNumericalBoundaries)obj;
        // get the surrogate selector of the base class
        System.Runtime.Serialization.ISurrogateSelector ss = AltaxoStreamingContext.GetSurrogateSelector(context);
        if(null!=ss)
        {
          System.Runtime.Serialization.ISerializationSurrogate surr =
            ss.GetSurrogate(obj.GetType().BaseType,context, out ss);

          // serialize the base class
          surr.GetObjectData(obj,info,context); // stream the data of the base object
        }
        else 
        {
          throw new NotImplementedException(string.Format("Serializing a {0} without surrogate not implemented yet!",obj.GetType()));
        }
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
        PositiveFiniteNumericalBoundaries s = (PositiveFiniteNumericalBoundaries)obj;
        // get the surrogate selector of the base class
        System.Runtime.Serialization.ISurrogateSelector ss = AltaxoStreamingContext.GetSurrogateSelector(context);
        if(null!=ss)
        {
          System.Runtime.Serialization.ISerializationSurrogate surr =
            ss.GetSurrogate(obj.GetType().BaseType,context, out ss);
          // deserialize the base class
          surr.SetObjectData(obj,info,context,selector);
        }
        else 
        {
          throw new NotImplementedException(string.Format("Serializing a {0} without surrogate not implemented yet!",obj.GetType()));
        }
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.PositiveFinitePhysicalBoundaries",0)]
      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PositiveFiniteNumericalBoundaries),1)]
      public new class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        PositiveFiniteNumericalBoundaries s = (PositiveFiniteNumericalBoundaries)obj;
        info.AddBaseValueEmbedded(s,typeof(PositiveFiniteNumericalBoundaries).BaseType);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        PositiveFiniteNumericalBoundaries s = null!=o ? (PositiveFiniteNumericalBoundaries)o : new PositiveFiniteNumericalBoundaries();
        info.GetBaseValueEmbedded(s,typeof(PositiveFiniteNumericalBoundaries).BaseType,parent);
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

    public PositiveFiniteNumericalBoundaries()
      : base()
    {
    }

    public PositiveFiniteNumericalBoundaries(PositiveFiniteNumericalBoundaries c)
      : base(c)
    {
    }

    public override object Clone()
    {
      return new PositiveFiniteNumericalBoundaries(this);
    }


    public override bool Add(Altaxo.Data.IReadableColumn col, int idx)
    {
      double d = (col is Altaxo.Data.INumericColumn) ? ((Altaxo.Data.INumericColumn)col)[idx] : idx;

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

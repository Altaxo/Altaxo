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

namespace Altaxo.Graph.Scales.Rescaling
{
  /// <summary>
  /// Summary description for AxisRescaleConditions.
  /// </summary>
  [Serializable]
  public class DateTimeAxisRescaleConditions : ICloneable, Altaxo.Main.IChangedEventSource
  {
    protected BoundaryRescaling _orgRescaling;
    protected BoundaryRescaling _endRescaling;
    protected BoundaryRescaling _spanRescaling;
    protected DateTime _org;
    protected DateTime _end;
    protected TimeSpan _span;

    [field:NonSerialized]
    public event EventHandler Changed;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.Axes.Scaling.DateTimeAxisRescaleConditions",0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DateTimeAxisRescaleConditions),1)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        DateTimeAxisRescaleConditions s = (DateTimeAxisRescaleConditions)obj;
        info.AddEnum("OrgRescaling",s._orgRescaling);
        info.AddValue("Org",s._org);  
        info.AddEnum("EndRescaling",s._endRescaling);
        info.AddValue("End",s._end);  
        info.AddEnum("SpanRescaling",s._spanRescaling);
        info.AddValue("Span",s._span);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        DateTimeAxisRescaleConditions s = null!=o ? (DateTimeAxisRescaleConditions)o : new DateTimeAxisRescaleConditions();

        s._orgRescaling = (BoundaryRescaling)info.GetEnum("OrgRescaling",typeof(BoundaryRescaling));
        s._org  = info.GetDateTime("Org");
        s._endRescaling = (BoundaryRescaling)info.GetEnum("EndRescaling",typeof(BoundaryRescaling));
        s._end  = info.GetDateTime("End");
        s._spanRescaling = (BoundaryRescaling)info.GetEnum("SpanRescaling",typeof(BoundaryRescaling));
        s._span = info.GetTimeSpan("Span");
        return s;
      }
    }
   
    #endregion


    /// <summary>
    /// Copies the data from another object.
    /// </summary>
    /// <param name="from">The object to copy the data from.</param>
    public void CopyFrom(DateTimeAxisRescaleConditions from)
    {
      bool bEqual = this.IsEqualTo(from);
      this._orgRescaling = from._orgRescaling;
      this._endRescaling = from._endRescaling;
      this._spanRescaling = from._spanRescaling;
      this._org = from._org;
      this._end = from._end;
      this._span = from._span;

      if(!bEqual)
        OnChanged();
    }

    #region Accessors

    public DateTime Org
    {
      get
      {
        return _org;
      }
    }

    public DateTime End
    {
      get
      {
        return _end;
      }
    }
    public TimeSpan Span
    {
      get
      {
        return _span;
      }
    }

    public BoundaryRescaling OrgRescaling
    {
      get
      {
        return _orgRescaling;
      }
    }

    public BoundaryRescaling EndRescaling
    {
      get
      {
        return _endRescaling;
      }
    }

    public BoundaryRescaling SpanRescaling
    {
      get
      {
        return _spanRescaling;
      }
    }


    #endregion


    public bool IsEqualTo( DateTimeAxisRescaleConditions b)
    {
      return
        this._orgRescaling == b._orgRescaling &&
        this._org == b._org &&
        this._endRescaling == b._endRescaling &&
        this._end == b._end &&
        this._spanRescaling == b._spanRescaling &&
        this._span == b._span;
    }
   

    public virtual void SetOrgEndSpan(BoundaryRescaling orgRescaling, DateTime org, BoundaryRescaling endRescaling, DateTime end, BoundaryRescaling spanRescaling, TimeSpan span)
    {
      _orgRescaling = orgRescaling;
      _org = org;
      _endRescaling = endRescaling;
      _end = end;
      _spanRescaling = spanRescaling;
      _span = span;
      Normalize(ref _orgRescaling, ref _endRescaling, ref _spanRescaling);
      OnChanged();
    }

    /// <summary>
    /// Sets the scaling behaviour of the axis by providing org and end values.
    /// </summary>
    /// <param name="orgRescaling">Type of scaling behaviour for the axis origin.</param>
    /// <param name="org">The axis org value. If orgRescaling is Auto, this value is ignored.</param>
    /// <param name="endRescaling">Type of scaling behaviour for the axis end.</param>
    /// <param name="end">The axis end value. If endRescaling is Auto, this value is ignored.</param>
    public virtual void SetOrgAndEnd(BoundaryRescaling orgRescaling, DateTime org, BoundaryRescaling endRescaling, DateTime end)
    {
      _orgRescaling = orgRescaling;
      _org = org;
      _endRescaling = endRescaling;
      _end = end;

      _spanRescaling = BoundaryRescaling.Auto;

      OnChanged();
    }

    /// <summary>
    /// Sets the scaling behavior so, that a certain size (end-org) of the axis is maintained.
    /// </summary>
    /// <param name="spanRescaling">The type of scaling for the axis span.</param>
    /// <param name="span">The axis span value. If spanRescaling is Auto, this value is ignored.</param>
    public virtual void SetSpan(BoundaryRescaling spanRescaling, TimeSpan span)
    {
      _spanRescaling = spanRescaling;
      _span = span;

      _orgRescaling = BoundaryRescaling.Auto;
      _endRescaling = BoundaryRescaling.Auto;

      OnChanged();
    }

    /// <summary>
    /// Sets the scaling behaviour to auto for both ends of the axis.
    /// </summary>
    public virtual void SetAuto()
    {
      _orgRescaling = BoundaryRescaling.Auto;
      _endRescaling = BoundaryRescaling.Auto;
      _spanRescaling = BoundaryRescaling.Auto;

      OnChanged();
    }

    /// <summary>
    /// Restricts the values of orgRescaling, endRescaling and spanRescaling to allowed combinations.
    /// </summary>
    /// <param name="orgRes">Org rescaling.</param>
    /// <param name="endRes">End rescaling.</param>
    /// <param name="spanRes">Span Rescaling.</param>
    public static void Normalize(
      ref BoundaryRescaling orgRes,
      ref BoundaryRescaling endRes,
      ref BoundaryRescaling spanRes)
    {
      NumericAxisRescaleConditions.Normalize(ref orgRes, ref endRes, ref spanRes);
    }



    /// <summary>
    /// This will process the temporary values for the axis origin and axis end. Depending on the rescaling conditions,
    /// the values of org and end are changed.
    /// </summary>
    /// <param name="org">The temporary axis origin (usually the lower boundary of the data set. On return, this value may be modified, depending on the rescale conditions.</param>
    /// <param name="isAutoOrg">On return, this value is true if the org value was not modified.</param>
    /// <param name="end">The temporary axis end (usually the upper boundary of the data set. On return, this value may be modified, depending on the rescale conditions.</param>
    /// <param name="isAutoEnd">On return, this value is true if the end value was not modified.</param>
    public virtual void Process(ref DateTime org, out bool isAutoOrg, ref DateTime end, out bool isAutoEnd)
    {
      DateTime oorg = org;
      DateTime oend = end;
      isAutoOrg = true;
      isAutoEnd = true;

      if(_spanRescaling!=BoundaryRescaling.Auto)
      {
        switch(_spanRescaling)
        {
          case BoundaryRescaling.Fixed:
            TimeSpan ospan = oend - oorg;
            TimeSpan diff = TimeSpan.FromSeconds((ospan - _span).TotalSeconds*0.5);
            org = oorg + diff;
            end = oend - diff;
            isAutoOrg = false;
            isAutoEnd = false;
            break;
          case BoundaryRescaling.GreaterOrEqual:
            if((oend-oorg)<_span)
              goto case BoundaryRescaling.Fixed;
            break;
          case BoundaryRescaling.LessOrEqual:
            if((oend-oorg)>_span)
              goto case BoundaryRescaling.Fixed;
            break;
        } // switch
      }
      else // spanRescaling is Auto
      {
        switch(_orgRescaling)
        {
          case BoundaryRescaling.Fixed:
            org = _org;
            isAutoOrg = false;
            break;
          case BoundaryRescaling.GreaterOrEqual:
            if(oorg<_org)
              goto case BoundaryRescaling.Fixed;
            break;
          case BoundaryRescaling.LessOrEqual:
            if(oorg>_org)
              goto case BoundaryRescaling.Fixed;
            break;
        }
        switch(_endRescaling)
        {
          case BoundaryRescaling.Fixed:
            end = _end;
            isAutoEnd = false;
            break;
          case BoundaryRescaling.GreaterOrEqual:
            if(oend<_end)
              goto case BoundaryRescaling.Fixed;
            break;
          case BoundaryRescaling.LessOrEqual:
            if(oend>_end)
              goto case BoundaryRescaling.Fixed;
            break;
        }
      }
    }
    #region ICloneable Members

    public virtual object Clone()
    {
      DateTimeAxisRescaleConditions result = new DateTimeAxisRescaleConditions();
      result.CopyFrom(this);
      return result;
    }

    #endregion

    protected virtual void OnChanged()
    {
      if(Changed!=null)
        Changed(this,EventArgs.Empty);

    }
  }
}

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
  public class NumericAxisRescaleConditions : ICloneable, Altaxo.Main.IChangedEventSource
  {
    protected BoundaryRescaling _orgRescaling;
    protected BoundaryRescaling _endRescaling;
    protected BoundaryRescaling _spanRescaling;
    protected double _org;
    protected double _end;
    protected double _span;

    protected double _minGrace=0.1;
    protected double _maxGrace=0.1;

    [field:NonSerialized]
    public event EventHandler Changed;

    #region Serialization


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Axes.Scaling.NumericAxisRescaleConditions", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NumericAxisRescaleConditions),1)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        NumericAxisRescaleConditions s = (NumericAxisRescaleConditions)obj;
        info.AddEnum("OrgRescaling",s._orgRescaling);
        info.AddValue("Org",s._org);  
        info.AddEnum("EndRescaling",s._endRescaling);
        info.AddValue("End",s._end);  
        info.AddEnum("SpanRescaling",s._spanRescaling);
        info.AddValue("Span",s._span);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        NumericAxisRescaleConditions s = null!=o ? (NumericAxisRescaleConditions)o : new NumericAxisRescaleConditions();

        s._orgRescaling = (BoundaryRescaling)info.GetEnum("OrgRescaling",typeof(BoundaryRescaling));
        s._org  = (double)info.GetDouble("Org");
        s._endRescaling = (BoundaryRescaling)info.GetEnum("EndRescaling",typeof(BoundaryRescaling));
        s._end  = (double)info.GetDouble("End");
        s._spanRescaling = (BoundaryRescaling)info.GetEnum("SpanRescaling",typeof(BoundaryRescaling));
        s._span = (double)info.GetDouble("Span");
        return s;
      }
    }
   
    #endregion


    /// <summary>
    /// Copies the data from another object.
    /// </summary>
    /// <param name="from">The object to copy the data from.</param>
    public void CopyFrom(NumericAxisRescaleConditions from)
    {
      bool bEqual = this.IsEqualTo(from);
      this._orgRescaling = from._orgRescaling;
      this._endRescaling = from._endRescaling;
      this._spanRescaling = from._spanRescaling;
      this._org = from._org;
      this._end = from._end;
      this._span = from._span;

      this._minGrace = from._minGrace;
      this._maxGrace = from._maxGrace;

      if(!bEqual)
        OnChanged();
    }

    #region Accessors

    public double Org
    {
      get
      {
        return _org;
      }
    }

    public double End
    {
      get
      {
        return _end;
      }
    }
    public double Span
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


    public double MinGrace
    {
      get
      {
        return _minGrace;
      }
    }

    public double MaxGrace
    {
      get
      {
        return _maxGrace;
      }
    }

    #endregion


    public bool IsEqualTo( NumericAxisRescaleConditions b)
    {
      return
        this._orgRescaling == b._orgRescaling &&
        this._org == b._org &&
        this._endRescaling == b._endRescaling &&
        this._end == b._end &&
        this._spanRescaling == b._spanRescaling &&
        this._span == b._span;
    }
   
    public virtual void SetOrgEndSpan(BoundaryRescaling orgRescaling, double org, BoundaryRescaling endRescaling, double end, BoundaryRescaling spanRescaling, double span)
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
    public virtual void SetOrgAndEnd(BoundaryRescaling orgRescaling, double org, BoundaryRescaling endRescaling, double end)
    {
      _orgRescaling = orgRescaling;
      _org = org;
      _endRescaling = endRescaling;
      _end = end;

      _spanRescaling = BoundaryRescaling.Auto;

      Normalize(ref _orgRescaling, ref _endRescaling, ref _spanRescaling);

      OnChanged();
    }

    /// <summary>
    /// Sets the scaling behavior so, that a certain size (end-org) of the axis is maintained.
    /// </summary>
    /// <param name="spanRescaling">The type of scaling for the axis span.</param>
    /// <param name="span">The axis span value. If spanRescaling is Auto, this value is ignored.</param>
    public virtual void SetSpan(BoundaryRescaling spanRescaling, double span)
    {
      _spanRescaling = spanRescaling;
      _span = span;

      _orgRescaling = BoundaryRescaling.Auto;
      _endRescaling = BoundaryRescaling.Auto;

      Normalize(ref _orgRescaling,ref _endRescaling, ref _spanRescaling);

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
      if(spanRes==BoundaryRescaling.UseSpan)
        spanRes=BoundaryRescaling.Fixed;

      if(orgRes==BoundaryRescaling.UseSpan && endRes==BoundaryRescaling.UseSpan)
      {
        orgRes=BoundaryRescaling.Auto;
        endRes=BoundaryRescaling.Auto;
      }


      if(orgRes==BoundaryRescaling.UseSpan && spanRes==BoundaryRescaling.Auto)
        orgRes=BoundaryRescaling.Auto;

      if(endRes==BoundaryRescaling.UseSpan && spanRes==BoundaryRescaling.Auto)
        endRes=BoundaryRescaling.Auto;
    }


    protected virtual bool ProcessOrg(ref double org)
    {
      bool isAutoOrg=true;
      double oorg = org;
      
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
      return isAutoOrg;
    }

    protected virtual bool ProcessOrgFromEndAndSpan(double end, ref double org)
    {
      bool isAutoOrg=true;
      double prp_org = end - _span;
      
      switch(_spanRescaling)
      {
        case BoundaryRescaling.Fixed:
          org = prp_org;
          isAutoOrg = false;
          break;
        case BoundaryRescaling.GreaterOrEqual:
          if(org>prp_org)
            goto case BoundaryRescaling.Fixed;
          break;
        case BoundaryRescaling.LessOrEqual:
          if(org<prp_org)
            goto case BoundaryRescaling.Fixed;
          break;
      }
      return isAutoOrg;
    }

    protected virtual bool ProcessEnd(ref double end)
    {
      bool isAutoEnd=true;
      double oend = end;
      
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

      return isAutoEnd;
    }


    protected virtual bool ProcessEndFromOrgAndSpan(double org, ref double end)
    {
      bool isAutoEnd=true;
      double prp_end = org + _span;

      switch(_spanRescaling)
      {
        case BoundaryRescaling.Fixed:
          end = prp_end;
          isAutoEnd = false;
          break;
        case BoundaryRescaling.GreaterOrEqual:
          if(end<prp_end)
            goto case BoundaryRescaling.Fixed;
          break;
        case BoundaryRescaling.LessOrEqual:
          if(end>prp_end)
            goto case BoundaryRescaling.Fixed;
          break;
      }

      return isAutoEnd;
    }


    protected virtual void ProcessSpanOnly(ref double org, ref bool isAutoOrg, ref double end, ref bool isAutoEnd)
    {
      double oorg = org;
      double oend = end;
      isAutoOrg = true;
      isAutoEnd = true;

      switch(_spanRescaling)
      {
        case BoundaryRescaling.Fixed:
        case BoundaryRescaling.UseSpan:
          org = ((oorg+oend)-_span)*0.5;
          end = ((oorg+oend)+_span)*0.5;
          isAutoOrg = false;
          isAutoEnd = false;
          break;
        case BoundaryRescaling.GreaterOrEqual:
          if(Math.Abs(oorg-oend)<_span)
            goto case BoundaryRescaling.Fixed;
          break;
        case BoundaryRescaling.LessOrEqual:
          if(Math.Abs(oorg-oend)>_span)
            goto case BoundaryRescaling.Fixed;
          break;
      } // switch
    }
    /// <summary>
    /// This will process the temporary values for the axis origin and axis end. Depending on the rescaling conditions,
    /// the values of org and end are changed.
    /// </summary>
    /// <param name="org">The temporary axis origin (usually the lower boundary of the data set. On return, this value may be modified, depending on the rescale conditions.</param>
    /// <param name="isAutoOrg">On return, this value is true if the org value was not modified.</param>
    /// <param name="end">The temporary axis end (usually the upper boundary of the data set. On return, this value may be modified, depending on the rescale conditions.</param>
    /// <param name="isAutoEnd">On return, this value is true if the end value was not modified.</param>
    public virtual void Process(ref double org, out bool isAutoOrg, ref double end, out bool isAutoEnd)
    {
      double oorg = org;
      double oend = end;
      isAutoOrg = true;
      isAutoEnd = true;


      if(_orgRescaling==BoundaryRescaling.UseSpan)
      {
        isAutoEnd = ProcessEnd(ref end);
        isAutoOrg = ProcessOrgFromEndAndSpan(end, ref org);
      }
      else if(_endRescaling==BoundaryRescaling.UseSpan)
      {
        isAutoOrg = ProcessOrg(ref end);
        isAutoEnd = ProcessEndFromOrgAndSpan(org, ref end);
        return;
      }
      else if(_spanRescaling!=BoundaryRescaling.Auto)
      {
        ProcessSpanOnly(ref org, ref isAutoOrg, ref end, ref isAutoEnd);
      }
      else
      {
        isAutoOrg = ProcessOrg(ref org);
        isAutoEnd = ProcessEnd(ref end);
      }
    }
    #region ICloneable Members

    public virtual object Clone()
    {
      NumericAxisRescaleConditions result = new NumericAxisRescaleConditions();
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

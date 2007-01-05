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
using System.Collections;
using Altaxo.Serialization;
using Altaxo.Graph.Scales.Rescaling;
using Altaxo.Graph.Scales.Boundaries;


namespace Altaxo.Graph.Scales
{
  /// <summary>
  /// Represents a logarithmic axis, i.e. the physical values v correspond to logical values l by v=a*10^(b*l).
  /// </summary>
  [Serializable]
  public class Log10Scale : NumericalScale, System.Runtime.Serialization.IDeserializationCallback
  {
    /// <summary>Decimal logarithm of axis org.</summary>
    double _log10Org=0; // Log10 of physical axis org
    /// <summary>Decimal logarithm of axis end.</summary>
    double _log10End=1; // Log10 of physical axis end

    /// <summary>Number of decades per major tick.</summary>
    int    _decadesPerMajorTick=1; // how many decades is one major tick

    /// <summary>The boundary object. It collectes only positive values for the axis is logarithmic.</summary>
    protected NumericalBoundaries _dataBounds = null;

    protected LogarithmicAxisRescaleConditions _rescaling = new LogarithmicAxisRescaleConditions();

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.Log10Axis",0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        Log10Scale s = (Log10Scale)obj;
        info.AddValue("Log10Org",s._log10Org);  
        info.AddValue("Log10End",s._log10End);  
        info.AddValue("DecadesPerMajor",s._decadesPerMajorTick);

        //info.AddValue("OrgFixed",s.m_AxisOrgFixed);
        //info.AddValue("EndFixed",s.m_AxisEndFixed);

        info.AddValue("Bounds",s._dataBounds);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        Log10Scale s = null!=o ? (Log10Scale)o : new Log10Scale();

        s._log10Org = (double)info.GetDouble("Log10Org");
        s._log10End = (double)info.GetDouble("Log10End");

        s._decadesPerMajorTick = (int)info.GetInt32("DecadesPerMajor");

        bool AxisOrgFixed = (bool)info.GetBoolean("OrgFixed");
        bool AxisEndFixed = (bool)info.GetBoolean("EndFixed");

        s._dataBounds = (PositiveFiniteNumericalBoundaries)info.GetValue("Bounds",typeof(PositiveFiniteNumericalBoundaries));
    
        s._dataBounds.BoundaryChanged += new BoundaryChangedHandler(s.OnBoundariesChanged);


        s._rescaling = new LogarithmicAxisRescaleConditions();
        s._rescaling.SetOrgAndEnd(AxisOrgFixed ? BoundaryRescaling.Fixed : BoundaryRescaling.Auto, s.Org, AxisEndFixed ? BoundaryRescaling.Fixed:BoundaryRescaling.Auto, s.End);

        LogarithmicAxisRescaleConditions rescaling = new LogarithmicAxisRescaleConditions();
        rescaling.SetOrgAndEnd(AxisOrgFixed ? BoundaryRescaling.Fixed : BoundaryRescaling.Auto, s.Org, AxisEndFixed ? BoundaryRescaling.Fixed:BoundaryRescaling.Auto, s.End);
        s._rescaling = rescaling;

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Axes.Log10Axis", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Log10Scale),2)]
      class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        Log10Scale s = (Log10Scale)obj;
        info.AddValue("Log10Org",s._log10Org);  
        info.AddValue("Log10End",s._log10End);  
        info.AddValue("DecadesPerMajor",s._decadesPerMajorTick);

        //info.AddValue("OrgFixed",s.m_AxisOrgFixed);
        //info.AddValue("EndFixed",s.m_AxisEndFixed);

        info.AddValue("Bounds",s._dataBounds);
        
        // new in version 1:
        info.AddValue("Rescaling",s._rescaling);

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        Log10Scale s = null!=o ? (Log10Scale)o : new Log10Scale();

        s._log10Org = (double)info.GetDouble("Log10Org");
        s._log10End = (double)info.GetDouble("Log10End");

        s._decadesPerMajorTick = (int)info.GetInt32("DecadesPerMajor");

        // s.m_AxisOrgFixed = (bool)info.GetBoolean("OrgFixed");
        // s.m_AxisEndFixed = (bool)info.GetBoolean("EndFixed");

        s._dataBounds = (PositiveFiniteNumericalBoundaries)info.GetValue("Bounds",typeof(PositiveFiniteNumericalBoundaries));
    
        s._dataBounds.BoundaryChanged += new BoundaryChangedHandler(s.OnBoundariesChanged);

        // new in version 1
        s._rescaling = (LogarithmicAxisRescaleConditions)info.GetValue("Rescaling",s);

        return s;
      }
    }

    /// <summary>
    /// Finale measures after deserialization of the linear axis.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public virtual void OnDeserialization(object obj)
    {
      // restore the event chain
      _dataBounds.BoundaryChanged += new BoundaryChangedHandler(this.OnBoundariesChanged);
    }
    #endregion



    /// <summary>
    /// Creates a default logarithmic axis with org=1 and end=10.
    /// </summary>
    public Log10Scale()
    {
      _dataBounds = new PositiveFiniteNumericalBoundaries();
      _dataBounds.BoundaryChanged += new BoundaryChangedHandler(this.OnBoundariesChanged);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">The axis to copy from.</param>
    public Log10Scale(Log10Scale from)
    {
      this.IsLinked = from.IsLinked;
   
      this._dataBounds   = null==from._dataBounds ? new PositiveFiniteNumericalBoundaries() : (NumericalBoundaries)from._dataBounds.Clone();
      _dataBounds.BoundaryChanged += new BoundaryChangedHandler(this.OnBoundariesChanged);
      this._decadesPerMajorTick = from._decadesPerMajorTick;
      this._log10End = from._log10End;
      this._log10Org = from._log10Org;

      this._rescaling = null==from.Rescaling ? new LogarithmicAxisRescaleConditions() : (LogarithmicAxisRescaleConditions)from.Rescaling.Clone();

    }

    /// <summary>
    /// Creates a clone copy of this axis.
    /// </summary>
    /// <returns>The cloned copy.</returns>
    public override object Clone()
    {
      return new Log10Scale(this);
    }

    /// <summary>
    /// Returns the rescaling conditions for this axis
    /// </summary>
    public override NumericAxisRescaleConditions Rescaling 
    {
      get 
      {
        return _rescaling; 

      }
    }

    /// <summary>
    /// PhysicalToNormal translates physical values into a normal value linear along the axis
    /// a physical value of the axis origin must return a value of zero
    /// a physical value of the axis end must return a value of one
    /// the function physicalToNormal must be provided by any derived class
    /// </summary>
    /// <param name="x">the physical value</param>
    /// <returns>
    /// the normalized value linear along the axis,
    /// 0 for axis origin, 1 for axis end</returns>
    public override double PhysicalToNormal(double x)
    {
      if(x<=0)
        return Double.NaN;

      double log10x = Math.Log10(x);
      return (log10x-_log10Org)/(_log10End-_log10Org);
    }
    /// <summary>
    /// NormalToPhysical is the inverse function to PhysicalToNormal
    /// It translates a normalized value (0 for the axis origin, 1 for the axis end)
    /// into the physical value
    /// </summary>
    /// <param name="x">the normal value (0 for axis origin, 1 for axis end</param>
    /// <returns>the corresponding physical value</returns>
    public override double NormalToPhysical(double x)
    {
      double log10x = _log10Org + (_log10End-_log10Org)*x;
      return Math.Pow(10,log10x);
    }

    /// <summary>
    /// PhysicalVariantToNormal translates physical values into a normal value linear along the axis
    /// a physical value of the axis origin must return a value of zero
    /// a physical value of the axis end must return a value of one
    /// the function physicalToNormal must be provided by any derived class
    /// </summary>
    /// <param name="x">the physical value</param>
    /// <returns>
    /// the normalized value linear along the axis,
    /// 0 for axis origin, 1 for axis end</returns>
    public override double PhysicalVariantToNormal(Altaxo.Data.AltaxoVariant x)
    {
      return PhysicalToNormal(x.ToDouble());
    }

    /// <summary>
    /// NormalToPhysicalVariant is the inverse function to PhysicalToNormal
    /// It translates a normalized value (0 for the axis origin, 1 for the axis end)
    /// into the physical value
    /// </summary>
    /// <param name="x">the normal value (0 for axis origin, 1 for axis end</param>
    /// <returns>the corresponding physical value</returns>
    public override Altaxo.Data.AltaxoVariant NormalToPhysicalVariant(double x)
    {
      return new Altaxo.Data.AltaxoVariant(NormalToPhysical(x));
    }

    /// <summary>
    /// GetMajorTicks returns the physical values
    /// at which major ticks should occur
    /// </summary>
    /// <returns>physical values for the major ticks</returns>
    public override double[] GetMajorTicks()
    {
      double log10org;
      double log10end;

      // ensure that log10org<log10end
      if(_log10Org<_log10End)
      {
        log10org = _log10Org;
        log10end = _log10End;
      }
      else
      {
        log10org = _log10End;
        log10end = _log10Org;
      }

      // calculate the number of major ticks

      int nFullDecades = (int)(1+Math.Floor(log10end)-Math.Ceiling(log10org));
      int nMajorTicks = (int)Math.Floor((nFullDecades+_decadesPerMajorTick-1)/(double)_decadesPerMajorTick);


      double[] retval = new double[nMajorTicks];
      int beg=(int)Math.Ceiling(log10org);
      int end=(int)Math.Floor(log10end);

      int i,j;
      for(i=beg,j=0 ;(i<=end) && (j<nMajorTicks) ; i+=_decadesPerMajorTick,j++)
      {
        retval[j] = Calc.RMath.Pow(10,i);
      }
      return retval;
    }

  
    /// <summary>
    /// GetMinorTicks returns the physical values
    /// at which minor ticks should occur
    /// </summary>
    /// <returns>physical values for the minor ticks</returns>
    public override double[] GetMinorTicks()
    {
      double log10org;
      double log10end;

      // ensure that log10org<log10end
      if(_log10Org<_log10End)
      {
        log10org = _log10Org;
        log10end = _log10End;
      }
      else
      {
        log10org = _log10End;
        log10end = _log10Org;
      }

      double decadespan = Math.Abs(_log10Org-_log10End);

      // now get the major ticks
      double[] majorticks = GetMajorTicks();
      // and calculate begin and end of minor ticks

      int majorcount = majorticks.Length;


      // guess from the span the tickiness (i.e. the increment of the multiplicator)
      // so that not more than 50 minor ticks are visible
      double minorsperdecade = 50.0/decadespan;
      
      // when there is more than one decade per major tick, then allow only minor ticks onto a full decade
      if (this._decadesPerMajorTick > 1)
      {
        int decadesPerMinor;
        for (decadesPerMinor = 1; decadesPerMinor<_decadesPerMajorTick; decadesPerMinor++)
        {
          if (0 != (_decadesPerMajorTick % decadesPerMinor))
            continue;
          double resultingMinors = decadespan / decadesPerMinor;
          if (resultingMinors < 50)
            break;
        }
        if (decadesPerMinor == _decadesPerMajorTick)
          return new double[] { };

        ArrayList minorlist = new ArrayList();

        int log10firstmajor = (int)Math.Floor(Math.Log10(majorticks[0]) + 0.125);
        int beg = (int)Math.Ceiling((log10org-log10firstmajor) / decadesPerMinor);
        int end = (int)Math.Floor((log10end-log10firstmajor) / decadesPerMinor);

        
        for (int i = beg, j=0 ; i <= end; i++)
        {
          double result = Calc.RMath.Pow(10, log10firstmajor + i * decadesPerMinor);
          double logdiff = Math.Log10(result) - Math.Log10(majorticks[j]);
          if(Math.Abs(logdiff)<0.125)
          {
            if((j+1)<majorticks.Length)
              j++;
            continue;
          }
          minorlist.Add(result);
        }
        return (double[])minorlist.ToArray(typeof(double));
      }


      // do not allow more than 10 minors per decade than 
      if(decadespan>0.3 && minorsperdecade>9) 
        minorsperdecade=9;

      // if minorsperdecade is lesser than one, we dont have minors, so we can
      // return an empty field
      if(minorsperdecade<=1)
        return new double[0];


      // ensure the minorsperdecade are one of the following values
      // 3,9,..
      double dec=1;
      double minormax=1;
      for(int i=0;;i++)
      {
        double val;
        switch(i%2)
        {
          default:
          case 0: val=3*dec; break;
          case 1: val=9*dec; dec*=10; break;
        }
        if (val <= minorsperdecade)
        {
          minormax = val;
        }
        else
          break;
      }
      minorsperdecade = minormax;
      // now if minorsperdecade is at least 2, it is a good "even" value

     

      // of cause this increment is only valid in the decade between 1 and 10
      double minorincrement = 9/(minorsperdecade);

      // there are two cases now, either we have at least one major tick,
      // then we have two different decades on left and right of the axis,
      // or there is no major tick, so the whole axis is in the same decade
      if(majorcount>=1) // the "normal" case
      {
        int i,j,k;
        // count the ticks on left of the axis
        // note: we normalized so that the "lesser values" are on the left
        double org = Math.Pow(10,log10org);
        double firstmajor = majorticks[0];
        for(i=1;firstmajor*(1-i*minorincrement/10)>=org;i++) {}
        int leftminorticks = i-1;

        // count the ticks on the right of the axis
        double end = Math.Pow(10,log10end);
        double lastmajor = majorticks[majorcount-1];
        for(i=1;lastmajor*(1+i*minorincrement)<=end;i++){}
        int rightminorticks = i-1;


        // calculate the total minorticks count
        double [] minors = new double[leftminorticks+rightminorticks+(majorcount-1)*((int)minorsperdecade-1)];

        // now fill the array
        for(j=0,i=leftminorticks;i>0;j++,i--)
          minors[j] = firstmajor*(1-i*minorincrement/10); 

        for(k=0;k<(majorcount-1);k++)
        {
          for(i=1;i<minorsperdecade;j++,i++)
            minors[j] = majorticks[k]*(1+i*minorincrement);
        }
        for(i=1;i<=rightminorticks;j++,i++)
          minors[j] = lastmajor*(1+i*minorincrement);
      
        return minors;
      }
      else // in case there is no major tick
      {

        // determine the upper decade (major tick)
        double firstmajor = Math.Pow(10,Math.Floor(log10org));
        double groundpow = Math.Floor(log10org);
        double norg = Math.Pow(10,log10org-groundpow);
        double nend = Math.Pow(10,log10end-groundpow);

        // norg and nend now is between 1 and 10
        // so calculate directly the indices
        double firstidx = Math.Ceiling(norg/minorincrement);
        double lastidx  = Math.Floor(nend/minorincrement);


        // do not do anything if something goes wrong
        if((lastidx<firstidx) || ((lastidx-firstidx)>100))
        {
          return new double[0];
        }

        double[] minors = new double[(int)(1+lastidx-firstidx)];
        
        // fill the array
        int j;
        double di;
        for(j=0,di=firstidx;di<=lastidx;j++,di+=1)
          minors[j] = firstmajor*(di*minorincrement);

        return minors; // return a empty array per default
      }



    }


    public override NumericalBoundaries DataBounds
    {
      get { return this._dataBounds; }
    } // return a PhysicalBoundarie object that is associated with that axis

    public override double Org
    {
      get { return Math.Pow(10,_log10Org); } 
      set 
      {
        if(value>0)
        {
          ProcessDataBounds(value,true,Math.Pow(10,_log10End),true);
        }
      }
    }
    public override double End 
    {
      get { return Math.Pow(10,_log10End); } 
      set
      {
        if(value>0)
        {
          ProcessDataBounds(Math.Pow(10,_log10Org),true,value,true);
        }
      }
    }

  

    /// <summary>
    /// calculates the axis org and end using the databounds
    /// the org / end is adjusted only if it is not fixed
    /// and the DataBound object contains valid data
    /// </summary>
    public override void ProcessDataBounds(double org, bool orgfixed, double end, bool endfixed)
    {
      if(IsLinked)
        return;
      // if one of the bounds is not valid, use the old bounds instead

      double log10org = org>0 ? Math.Log10(org) : _log10Org;
      double log10end = end>0 ? Math.Log10(end) : _log10End;


      // do something if org and end are the same
      if(log10org==log10end)
      {
        log10org -= 1;
        log10end += 1;
      }

      // calculate the number of decades between end and org
      double decades = Math.Abs(log10end-log10org);

      // limit the number of major ticks to about 10
      _decadesPerMajorTick = (int)Math.Ceiling(decades/10.0);


      _log10Org = log10org;
      _log10End = log10end;

    }
    public override void ProcessDataBounds()
    {
      if(null==this._dataBounds || this._dataBounds.IsEmpty)
        return;
    
      ProcessDataBounds(_dataBounds.LowerBound,_dataBounds.UpperBound,_rescaling); 
    }

    public void ProcessDataBounds(double xorg, double xend, NumericAxisRescaleConditions rescaling)
    {
      bool isAutoOrg, isAutoEnd;
      rescaling.Process(ref xorg, out isAutoOrg, ref xend, out isAutoEnd);
      ProcessDataBounds(xorg,!isAutoOrg,xend,!isAutoEnd);
    }

    protected void OnBoundariesChanged(object sender, BoundariesChangedEventArgs e)
    {
      bool bIsRelevant=true;
     

      if(bIsRelevant) // if something really relevant changed
      {
        ProcessDataBounds(); // calculate new bounds and fire AxisChanged event
      }
    }


  } // end of class Log10Axis
}

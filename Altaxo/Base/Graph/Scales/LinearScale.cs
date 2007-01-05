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
using Altaxo.Graph.Scales.Rescaling;
using Altaxo.Graph.Scales.Boundaries;

namespace Altaxo.Graph.Scales
{
  /// <summary>
  /// A linear axis, i.e a axis where physical values v can be translated to logical values l by v=a+b*l.
  /// </summary>
  
  [Serializable]
  public class LinearScale : NumericalScale, System.Runtime.Serialization.IDeserializationCallback
  {
    // primary values
    /// <summary>Proposed value of axis origin, proposed either by the lower physical boundary or by the user (if axis org is fixed).</summary>
    protected double _baseOrg=0; // proposed value of org
    /// <summary>Proposed value of axis end, proposed either by the upper physical boundary or by the user (if axis end is fixed).</summary>
    protected double _baseEnd=1; // proposed value of end
    /// <summary>Current axis origin divided by the major tick span value.</summary>
    protected double _axisOrgByMajor=0;
    /// <summary>Current axis end divided by the major tick span value.</summary>
    protected double _axisEndByMajor=5;
    /// <summary>Physical span value between two major ticks.</summary>
    protected double _majorSpan=0.2; // physical span value between two major ticks
    /// <summary>Minor ticks per Major tick ( if there is one minor tick between two major ticks m_minorticks is 2!</summary>
    protected int    _minorTicks=2;
   
    /// <summary>Holds the <see cref="NumericalBoundaries"/> for that axis.</summary>
    protected NumericalBoundaries _dataBounds = new FiniteNumericalBoundaries();

    protected NumericAxisRescaleConditions _rescaling = new NumericAxisRescaleConditions();

    // cached values
    /// <summary>Current axis origin (cached value).</summary>
    protected double _cachedAxisOrg=0;
    /// <summary>Current axis end (cached value).</summary>
    protected double _cachedAxisEnd=1;
    /// <summary>Current axis span (i.e. end-org) (cached value).</summary>
    protected double _cachedAxisSpan=1;
    /// <summary>Current inverse of axis span (cached value).</summary>
    protected double _cachedOneByAxisSpan=1;


    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.LinearAxis",0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        LinearScale s = (LinearScale)obj;
        info.AddValue("BaseOrg",s._baseOrg);  
        info.AddValue("BaseEnd",s._baseEnd);  
        info.AddValue("MajorSpan",s._majorSpan);
        info.AddValue("MinorTicks",s._minorTicks);
        info.AddValue("OrgByMajor",s._axisOrgByMajor);
        info.AddValue("EndByMajor",s._axisEndByMajor);

        // info.AddValue("OrgFixed",s.m_AxisOrgFixed);
        // info.AddValue("EndFixed",s.m_AxisEndFixed);

        info.AddValue("Bounds",s._dataBounds);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        LinearScale s = null!=o ? (LinearScale)o : new LinearScale();

        s._baseOrg = (double)info.GetDouble("BaseOrg");
        s._baseEnd = (double)info.GetDouble("BaseEnd");

        s._majorSpan = (double)info.GetDouble("MajorSpan");
        s._minorTicks = (int)info.GetInt32("MinorTicks");

        s._axisOrgByMajor = (double)info.GetDouble("OrgByMajor");
        s._axisEndByMajor = (double)info.GetDouble("EndByMajor");

        bool AxisOrgFixed = (bool)info.GetBoolean("OrgFixed");
        bool AxisEndFixed = (bool)info.GetBoolean("EndFixed");

        s._dataBounds = (FiniteNumericalBoundaries)info.GetValue("Bounds",s);
  
        s.SetCachedValues();
        // restore the event chain
        s._dataBounds.BoundaryChanged += new BoundaryChangedHandler(s.OnBoundariesChanged);
  
        s._rescaling = new NumericAxisRescaleConditions();
        s._rescaling.SetOrgAndEnd(AxisOrgFixed ? BoundaryRescaling.Fixed : BoundaryRescaling.Auto, s.Org, AxisEndFixed ? BoundaryRescaling.Fixed:BoundaryRescaling.Auto, s.End);

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Axes.LinearAxis", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinearScale),2)]
      class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        LinearScale s = (LinearScale)obj;
        info.AddValue("BaseOrg",s._baseOrg);  
        info.AddValue("BaseEnd",s._baseEnd);  
        info.AddValue("MajorSpan",s._majorSpan);
        info.AddValue("MinorTicks",s._minorTicks);
        info.AddValue("OrgByMajor",s._axisOrgByMajor);
        info.AddValue("EndByMajor",s._axisEndByMajor);

        // info.AddValue("OrgFixed",s.m_AxisOrgFixed); // removed in version 1
        // info.AddValue("EndFixed",s.m_AxisEndFixed); // removed in version 1

        info.AddValue("Bounds",s._dataBounds);

        // new in version 1:
        info.AddValue("Rescaling",s._rescaling);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        LinearScale s = null!=o ? (LinearScale)o : new LinearScale();

        s._baseOrg = (double)info.GetDouble("BaseOrg");
        s._baseEnd = (double)info.GetDouble("BaseEnd");

        s._majorSpan = (double)info.GetDouble("MajorSpan");
        s._minorTicks = (int)info.GetInt32("MinorTicks");

        s._axisOrgByMajor = (double)info.GetDouble("OrgByMajor");
        s._axisEndByMajor = (double)info.GetDouble("EndByMajor");

        //s.m_AxisOrgFixed = (bool)info.GetBoolean("OrgFixed");
        //s.m_AxisEndFixed = (bool)info.GetBoolean("EndFixed");

        s._dataBounds = (FiniteNumericalBoundaries)info.GetValue("Bounds",s);
  
        s.SetCachedValues();
        // restore the event chain
        s._dataBounds.BoundaryChanged += new BoundaryChangedHandler(s.OnBoundariesChanged);
  
        // new in version 1
        s._rescaling = (NumericAxisRescaleConditions)info.GetValue("Rescaling",s);

        return s;
      }
    }

    /// <summary>
    /// Finale measures after deserialization of the linear axis.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public virtual void OnDeserialization(object obj)
    {
      // restore the cached values
      SetCachedValues();
      // restore the event chain
      _dataBounds.BoundaryChanged += new BoundaryChangedHandler(this.OnBoundariesChanged);
    }
    #endregion


    /// <summary>
    /// Creates a default linear axis with org=0 and end=1.
    /// </summary>
    public LinearScale()
    {
      _dataBounds = new FiniteNumericalBoundaries();
      _dataBounds.BoundaryChanged += new BoundaryChangedHandler(this.OnBoundariesChanged);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">A other linear axis from which to copy from.</param>
    public LinearScale(LinearScale from)
    {
      this.IsLinked = from.IsLinked;

      this._cachedAxisEnd        = from._cachedAxisEnd;
      this._axisEndByMajor = from._axisEndByMajor;
      this._cachedAxisOrg        = from._cachedAxisOrg;
      this._axisOrgByMajor = from._axisOrgByMajor;
      this._cachedAxisSpan       = from._cachedAxisSpan;
      this._baseEnd        = from._baseEnd;
      this._baseOrg        = from._baseOrg;
      this._dataBounds     = null==from._dataBounds ? new FiniteNumericalBoundaries() : (NumericalBoundaries)from._dataBounds.Clone(); 
      _dataBounds.BoundaryChanged += new BoundaryChangedHandler(this.OnBoundariesChanged);
      this._majorSpan      = from._majorSpan;
      this._minorTicks     = from._minorTicks;
      this._cachedOneByAxisSpan  = from._cachedOneByAxisSpan;

      this._rescaling = null==from.Rescaling ? new NumericAxisRescaleConditions() : (NumericAxisRescaleConditions)from.Rescaling.Clone();

    }

    public virtual void CopyFrom(LinearScale from)
    {
      this._cachedAxisEnd        = from._cachedAxisEnd;
      this._axisEndByMajor = from._axisEndByMajor;
      this._cachedAxisOrg        = from._cachedAxisOrg;
      this._axisOrgByMajor = from._axisOrgByMajor;
      this._cachedAxisSpan       = from._cachedAxisSpan;
      this._baseEnd        = from._baseEnd;
      this._baseOrg        = from._baseOrg;
      if(null!=_dataBounds)
        _dataBounds.BoundaryChanged -= new BoundaryChangedHandler(this.OnBoundariesChanged);
      this._dataBounds     = null==from._dataBounds ? new FiniteNumericalBoundaries() : (NumericalBoundaries)from._dataBounds.Clone(); 
      _dataBounds.BoundaryChanged += new BoundaryChangedHandler(this.OnBoundariesChanged);
      this._majorSpan      = from._majorSpan;
      this._minorTicks     = from._minorTicks;
      this._cachedOneByAxisSpan  = from._cachedOneByAxisSpan;

      this._rescaling = null==from.Rescaling ? new NumericAxisRescaleConditions() : (NumericAxisRescaleConditions)from.Rescaling.Clone();
    }

    public override object Clone()
    {
      return new LinearScale(this);
    }

    /// <summary>
    /// Get/sets the axis origin (physical units).
    /// </summary>
    public override double Org
    {
      get { return _cachedAxisOrg; } 
      set 
      {
        _cachedAxisOrg = value;
        ProcessDataBounds(_cachedAxisOrg,true,_cachedAxisEnd,true);
      }
    }

    /// <summary>
    /// Get/sets the axis end (physical units).
    /// </summary>
    public override double End 
    {
      get { return _cachedAxisEnd; } 
      set
      { 
        _cachedAxisEnd = value;
        ProcessDataBounds(_cachedAxisOrg,true,_cachedAxisEnd,true);
      }
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
    /// Get the internal DataBound object (mostly for merging).
    /// </summary>
    public override NumericalBoundaries DataBounds 
    {
      get { return _dataBounds; }
    }

    /// <summary>
    /// Converts a value in axis (physical) units to a "normalized" value, which is 0 for the axis org and 1 for the axis end.
    /// </summary>
    /// <param name="x">Value to convert (physical units).</param>
    /// <returns>Normalized value.</returns>
    public override double PhysicalToNormal(double x)
    {
      return (x- _cachedAxisOrg ) * _cachedOneByAxisSpan; 
    }

    public override double NormalToPhysical(double x)
    {
      return _cachedAxisOrg + x * _cachedAxisSpan;
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

    public override double[] GetMajorTicks()
    {
      int j;
      double i,beg,end;
      double[] retv;
      if(_axisOrgByMajor<=_axisEndByMajor) // normal case org<end
      {
        beg=System.Math.Ceiling(_axisOrgByMajor);
        end=System.Math.Floor(_axisEndByMajor);
        retv = new double[1+(int)(end-beg)];
        for(j=0,i=beg;i<=end;i+=1,j++)
          retv[j]=i*_majorSpan;
      }
      else
      {
        beg=System.Math.Floor(_axisOrgByMajor);
        end=System.Math.Ceiling(_axisEndByMajor);
        retv = new double[1+(int)(beg-end)];
        for(j=0,i=beg;i>=end;i-=1,j++)
          retv[j]=i*_majorSpan;
      }
      return retv;
    }

    public override double[] GetMinorTicks()
    {
      int j;
      double i,beg,end;
      double[] retv;
      if(_minorTicks<2)
        return new double[]{}; // below 2 there are no minor ticks per definition

      if(_axisOrgByMajor<=_axisEndByMajor) // normal case org<end
      {
        beg=System.Math.Ceiling(_axisOrgByMajor);
        end=System.Math.Floor(_axisEndByMajor);
        int majorticks = 1+(int)(end-beg);
        beg = System.Math.Ceiling(_axisOrgByMajor*_minorTicks);
        end = System.Math.Floor(_axisEndByMajor*_minorTicks);
        int minorticks = 1+(int)(end-beg) - majorticks;
        retv = new double[minorticks];
        for(j=0,i=beg;i<=end && j<minorticks;i+=1)
        {
          if(i%_minorTicks!=0)
          {
            retv[j]=i*_majorSpan/_minorTicks;
            j++;
          }
        }
      }
      else
      {
        beg=System.Math.Floor(_axisOrgByMajor);
        end=System.Math.Ceiling(_axisEndByMajor);
        retv = new double[1+(int)(beg-end)];
        for(j=0,i=beg;i>=end;i-=1,j++)
          retv[j]=i*_majorSpan;
      }
      return retv;
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


    public  override void ProcessDataBounds(double xorg, bool xorgfixed, double xend, bool xendfixed)
    {
      if(IsLinked)
        return;

      double oldAxisOrgByMajor = _axisOrgByMajor;
      double oldAxisEndByMajor = _axisEndByMajor;
      double oldMajorSpan      = _majorSpan;
      int    oldMinorTicks     = _minorTicks;

      _baseOrg = xorg;
      _baseEnd = xend;


      if (!(xorgfixed && xendfixed))
      {
        CalculateTicks(xorg, xend, out _majorSpan, out _minorTicks);
        double orgByMajor, endByMajor;
        CalculateActualLimits(xorg, xorgfixed, xend, xendfixed, _majorSpan, _minorTicks, out orgByMajor, out endByMajor);
        xorg = orgByMajor * _majorSpan;
        xend = endByMajor * _majorSpan;
      }

      CalculateTicks(xorg, xend, out _majorSpan, out _minorTicks);
      _axisOrgByMajor = xorg / _majorSpan;
      _axisEndByMajor = xend / _majorSpan;
      
      /*
      if (xend == xorg)
      {
        if(xorgfixed)
          _axisOrgByMajor = xorg / _majorSpan;
        else
          _axisOrgByMajor = System.Math.Floor(_minorTicks * xorg / _majorSpan) / _minorTicks - 3 ;

        if(xendfixed)
          _axisEndByMajor = xend / _majorSpan;
        else
          _axisEndByMajor = System.Math.Ceiling(_minorTicks * xend / _majorSpan) / _minorTicks +3;
      }
      else if(xend>xorg)
      {
        if(xorgfixed)
          _axisOrgByMajor = xorg/_majorSpan;
        else
          _axisOrgByMajor = System.Math.Floor(_minorTicks * xorg/_majorSpan)/_minorTicks;

        if(xendfixed)
          _axisEndByMajor = xend/_majorSpan;
        else
          _axisEndByMajor = System.Math.Ceiling(_minorTicks * xend /_majorSpan)/_minorTicks;
      }
      else // org is greater than end !
      {
        if(xorgfixed)
          _axisOrgByMajor = xorg/_majorSpan;
        else
          _axisOrgByMajor = System.Math.Ceiling(_minorTicks * xorg/_majorSpan)/_minorTicks;

        if(xendfixed)
          _axisEndByMajor = xend/_majorSpan;
        else
          _axisEndByMajor = System.Math.Floor(_minorTicks * xend /_majorSpan)/_minorTicks;
      }
      */

      SetCachedValues();

      // compare with the saved values to find out whether or not something changed
      if(oldAxisOrgByMajor!=_axisOrgByMajor ||
        oldAxisEndByMajor!=_axisEndByMajor ||
        oldMajorSpan != _majorSpan ||
        oldMinorTicks != _minorTicks)
      {
        OnChanged();
      }
    }

    protected void SetCachedValues()
    {
      _cachedAxisOrg = _axisOrgByMajor * _majorSpan;
      _cachedAxisEnd = _axisEndByMajor * _majorSpan;
      _cachedAxisSpan = _cachedAxisEnd - _cachedAxisOrg;
      _cachedOneByAxisSpan = 1/_cachedAxisSpan;
    }

    protected void OnBoundariesChanged(object sender, BoundariesChangedEventArgs e)
    {
     

      bool bIsRelevant=true;

      if(bIsRelevant) // if something really relevant changed
      {
        ProcessDataBounds(); // calculate new bounds and fire AxisChanged event
      }
    }


    void CalculateActualLimits(
      double xorg, bool xorgfixed, double xend, bool xendfixed, 
      double majorSpan, int minorTicks,
      out double axisOrgByMajor, out double axisEndByMajor)
    {
      const double ZeroLever = 0.25;
      const double minGrace = 1/16.0;
      const double maxGrace = 1/16.0;

      if (xend < xorg)
        throw new ArgumentOutOfRangeException("xorg is greater than xend");

      double range = xend - xorg;

      // This is the zero-lever test.  If xorg is within the zero lever fraction
      // of the data range, then use zero.

      if (!xorgfixed && xorg > 0 && Math.Abs(xorg / range) < ZeroLever)
        xorg = 0;

      // Zero-lever test for cases where the xend value is less than zero
      if (!xendfixed && xend < 0 && Math.Abs(xend / range) < ZeroLever)
        xend = 0;


      range = xend - xorg;

      if (range==0)
      {
        if (xorgfixed)
        {
          axisOrgByMajor = xorg / majorSpan;
        }
        else
        {
          axisOrgByMajor = System.Math.Floor(minorTicks * xorg / majorSpan) / minorTicks - 3;
        }

        if (xendfixed)
        {
          axisEndByMajor = xend / majorSpan;
        }
        else
        {
          axisEndByMajor = System.Math.Ceiling(minorTicks * xend / majorSpan) / minorTicks + 3;
        }
      }
      else // normal case where xorg < xend
      {
        if (xorgfixed)
        {
          axisOrgByMajor = xorg / majorSpan;
        }
        else
        {
          axisOrgByMajor = System.Math.Floor(minorTicks * xorg / majorSpan) / minorTicks;
          double norg = axisOrgByMajor * majorSpan;

          // Compare this new org with a value adjusted by grace
           // Do not let the grace value extend the axis below zero when all the values were positive
          double gorg = xorg;  
          if ((xorg < 0 || xorg - minGrace * range >= 0.0))
              gorg = xorg - minGrace * range;
            
          if(gorg<norg) // if the grace adjusted value exceeds the tick adjusted limit, than we adjust the limit further
            axisOrgByMajor = System.Math.Floor(minorTicks * gorg / majorSpan) / minorTicks;
        }
      

        if (xendfixed)
        {
          axisEndByMajor = xend / majorSpan;
        }
        else
        {
          axisEndByMajor = System.Math.Ceiling(minorTicks * xend / majorSpan) / minorTicks;
          double nend = axisEndByMajor * majorSpan;

          // Compare this new end with a value adjusted by grace
          // Do not let the grace value extend the axis above zero when all the values are negative
          double gend = xend;
          if (xend > 0 || xend + maxGrace * range <= 0.0)
            gend = xend + maxGrace * range;

          if (gend > nend) // if the grace adjusted value exceeds the tick adjusted limit, than we adjust the limit further
            axisEndByMajor = System.Math.Ceiling(minorTicks * gend / majorSpan) / minorTicks;
        }
      }
    }

    static double TenToThePowerOf(int ii)
    {
      if(ii==0)
        return 1;
      else if(ii==1)
        return 10;
      else
      {
        int i = System.Math.Abs(ii);
        int halfi = i/2;
        double hret = TenToThePowerOf(halfi);
        double ret =(halfi+halfi)==i ? hret*hret : 10*hret*hret; 
        return ii<0 ? 1/ret : ret;
      }
    }



    static void CalculateTicks( 
      double min,                // Minimum of data 
      double max,                // Maximum of data
      out double majorspan,      // the span between two major ticks
      out int    minorticks      // number of ticks in a major tick span 
      )
    {
      // Make sure that minVal and maxVal are legitimate values
      if (Double.IsInfinity(min) || Double.IsNaN(min) || min == Double.MaxValue)
        min = 0.0;
      if (Double.IsInfinity(max) || Double.IsNaN(max) || max == Double.MaxValue)
        max = 0.0;

      if(min>max) // should not happen, but can happen when there are no data and min and max are uninitialized 
      {
        min=max=0;
      }

      double span = max - min; // span width between max and min

      if(0==span)
      {
        double diff;
        // if span width is zero, then 1% of the velue, in case of min==max==0 we use 1
        if(0==max || 0==min) // if one is null, the other should also be null, but to be secure...
          diff = 1;
        else
          diff = System.Math.Abs(min/100); // wir can be sure, that min==max, because span==0

        min -= diff;
        max += diff;
          
        span = max - min;
      } // if 0==span


      // we have to norm span in that way, that 100<=normspan<1000
      int nSpanPotCorr = (int)(System.Math.Floor(System.Math.Log10(span)))-2; // nSpanPotCorr will be 0 if 100<=span<1000 
      double normspan = span / TenToThePowerOf(nSpanPotCorr);

      // we divide normspan by 10, 20, 25, 50, 100, 200 and calculate the
      // number of major ticks this will give
      // we can break if the number of major ticks is below 10
      int majornormspan=1;
      int minornormspan=1;
      for(int finep=0;finep<=5;finep++)
      {
        switch(finep)
        {
          case 0:
            majornormspan = 10;
            minornormspan = 5;
            break;
          case 1:
            majornormspan = 20;
            minornormspan = 10;
            break;
          case 2:
            majornormspan = 25;
            minornormspan = 5;
            break;
          case 3:
            majornormspan = 50;
            minornormspan = 25;
            break;
          case 4:
            majornormspan = 100;
            minornormspan = 50;
            break;
          case 5:
          default:
            majornormspan = 200;
            minornormspan = 100;
            break;
        } // end of switch
        double majorticks = 1+System.Math.Floor(normspan/majornormspan);
        if(majorticks<=10)
          break;
      }
      majorspan = majornormspan * TenToThePowerOf(nSpanPotCorr);
      minorticks = (int)(majornormspan / minornormspan);
    } // end of function
  } // end of class LinearAxis


}

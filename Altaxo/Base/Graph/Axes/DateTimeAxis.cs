using System;
using Altaxo.Data;

#if true
namespace Altaxo.Graph.Axes
{
  using Scaling;
  using Boundaries;

  /// <summary>
  /// Summary description for DateTimeAxis.
  /// </summary>
  public class DateTimeAxis : Axis
  {
    // cached values
    /// <summary>Current axis origin (cached value).</summary>
    protected DateTime m_AxisOrg=DateTime.MinValue;
    /// <summary>Current axis end (cached value).</summary>
    protected DateTime m_AxisEnd=DateTime.MaxValue;

    /// <summary>Holds the <see cref="NumericalBoundaries"/> for that axis.</summary>
    protected FiniteDateTimeBoundaries m_DataBounds;

    protected DateTimeAxisRescaleConditions _rescaling;

    #region ICloneable Members
    public void CopyFrom(DateTimeAxis from)
    {
      this.IsLinked = from.IsLinked;

      this.m_AxisOrg = from.m_AxisOrg;
      this.m_AxisEnd = from.m_AxisEnd;
 
      this.InternalSetDataBounds((FiniteDateTimeBoundaries)from.m_DataBounds.Clone());
      this.InternalSetRescaling((DateTimeAxisRescaleConditions)from._rescaling.Clone());

    }

    public DateTimeAxis(DateTimeAxis from)
    {
      CopyFrom(from);
    }

    public DateTimeAxis()
    {
      this.InternalSetDataBounds(new FiniteDateTimeBoundaries());
      this.InternalSetRescaling(new DateTimeAxisRescaleConditions());
    }

    /// <summary>
    /// Creates a copy of the axis.
    /// </summary>
    /// <returns>The cloned copy of the axis.</returns>
    public override object Clone()
    {
      return new DateTimeAxis(this);
    }

   
    #endregion
    

    protected void InternalSetDataBounds(FiniteDateTimeBoundaries bounds)
    {
      if(this.m_DataBounds!=null)
      {
        this.m_DataBounds.BoundaryChanged -= new BoundaryChangedHandler(this.EhBoundariesChanged);
        this.m_DataBounds = null;
      }
      this.m_DataBounds = bounds;
      this.m_DataBounds.BoundaryChanged += new BoundaryChangedHandler(this.EhBoundariesChanged);
    }

    protected void InternalSetRescaling(DateTimeAxisRescaleConditions rescaling)
    {
      this._rescaling = rescaling;
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
    public double PhysicalToNormal(DateTime x)
    {
      return (x-m_AxisOrg).TotalSeconds / (m_AxisEnd-m_AxisOrg).TotalSeconds; 
    }
    /// <summary>
    /// NormalToPhysical is the inverse function to PhysicalToNormal
    /// It translates a normalized value (0 for the axis origin, 1 for the axis end)
    /// into the physical value
    /// </summary>
    /// <param name="x">the normal value (0 for axis origin, 1 for axis end</param>
    /// <returns>the corresponding physical value</returns>
    public DateTime NormalToPhysical(double x)
    {
      return m_AxisOrg.AddSeconds(x*(m_AxisEnd-m_AxisOrg).TotalSeconds);
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
      if(x.IsType(AltaxoVariant.Content.VDateTime))
        return PhysicalToNormal((DateTime)x);
      else if(x.CanConvertedToDouble)
        return PhysicalToNormal(new DateTime((long)(x.ToDouble()*10000000)));
      else throw new ArgumentException("Variant x is neither DateTime nor numeric");
    }
    /// <summary>
    /// NormalToPhysicalVariant is the inverse function to PhysicalToNormal
    /// It translates a normalized value (0 for the axis origin, 1 for the axis end)
    /// into the physical value
    /// </summary>
    /// <param name="x">the normal value (0 for axis origin, 1 for axis end</param>
    /// <returns>the corresponding physical value</returns>
    public  override Altaxo.Data.AltaxoVariant NormalToPhysicalVariant(double x)
    {
      return new Altaxo.Data.AltaxoVariant(NormalToPhysical(x));
    }



    public override AltaxoVariant OrgAsVariant
    {
      get
      {
        return new AltaxoVariant (Org);
      }
      set
      {
        Org = (DateTime)value;
      }
    }

    public override AltaxoVariant EndAsVariant
    {
      get
      {
        return new AltaxoVariant (End);
      }
      set
      {
        End = (DateTime)value;
      }
    }


    /// <summary>
    /// GetMajorTicks returns the physical values
    /// at which major ticks should occur
    /// </summary>
    /// <returns>physical values for the major ticks</returns>
    public DateTime[] GetMajorTicks()
    {
      return (DateTime[])GetMajorTicksAsList().ToArray(typeof(DateTime));
    }

    public override AltaxoVariant[] GetMajorTicksAsVariant()
    {
      DateTime[] ticks = GetMajorTicks();
      AltaxoVariant[] vticks = new AltaxoVariant[ticks.Length];
      for(int i=0;i<ticks.Length;++i)
        vticks[i] = ticks[i];
      return vticks;
    }
    public override AltaxoVariant[] GetMinorTicksAsVariant()
    {
      DateTime[] ticks = GetMinorTicks();
      AltaxoVariant[] vticks = new AltaxoVariant[ticks.Length];
      for(int i=0;i<ticks.Length;++i)
        vticks[i] = ticks[i];
      return vticks;
    }

    System.Collections.ArrayList GetMajorTicksAsList()
    {
      System.Collections.ArrayList arr = new System.Collections.ArrayList();
    
      if(m_AxisOrg<=m_AxisEnd)
      {
        for(DateTime d=m_AxisOrg;;)
        {

          DateTime r = m_MajorSpan.RoundUp(d);
          if(r>m_AxisEnd)
            break;
          arr.Add(r);

          d = r.AddTicks(1);
        }
        
      }
      else // downwards
      {
        for(DateTime d=m_AxisOrg;;)
        {

          DateTime r = m_MajorSpan.RoundDown(d);
          if(r<m_AxisEnd)
            break;
          arr.Add(r);

          d=r.AddTicks(-1);
        }
      }

      return arr;
     
    }

    /// <summary>
    /// Returns the physical values
    /// at which minor ticks should occur
    /// </summary>
    /// <returns>physical values for the minor ticks</returns>
    public DateTime[] GetMinorTicks()
    {
      if(this.m_MinorTicks==0)
        return new DateTime[0];

      System.Collections.ArrayList major = GetMajorTicksAsList();

      return new DateTime[0];

    }


    public override double[] GetMajorTicksNormal()
    {
      DateTime[] ticks = GetMajorTicks();
      double[] rticks = new double[ticks.Length];
      for(int i=0;i<ticks.Length;i++)
      {
        rticks[i] = PhysicalToNormal(ticks[i]);
      }
      return rticks;
    }
    public override double[] GetMinorTicksNormal()
    {
      DateTime[] ticks = GetMinorTicks();
      double[] rticks = new double[ticks.Length];
      for(int i=0;i<ticks.Length;i++)
      {
        rticks[i] = PhysicalToNormal(ticks[i]);
      }
      return rticks;
    }

    /// <summary>
    /// Returns the rescaling conditions for this axis
    /// </summary>
    public DateTimeAxisRescaleConditions Rescaling 
    {
      get
      {
        return this._rescaling;
      }
    }
    /// <summary>
    /// Returns the rescaling conditions for this axis
    /// </summary>
    public override object RescalingObject
    {
      get
      {
        return this._rescaling;
      }
    }
    /// <summary>
    /// Returns the <see cref="FiniteDateTimeBoundaries" /> object that is associated with that axis.
    /// </summary>
    public FiniteDateTimeBoundaries DataBounds 
    {
      get
      {
        return this.m_DataBounds;
      }
    } // return a PhysicalBoundarie object that is associated with that axis
    
    /// <summary>
    /// Returns the <see cref="IPhysicalBoundaries"/> object that is associated with that axis.
    /// </summary>
    public override IPhysicalBoundaries DataBoundsObject 
    {
      get
      {
        return this.m_DataBounds;
      }
    } // return a PhysicalBoundarie object that is associated with that axis

    /// <summary>The axis origin, i.e. the first point in physical units.</summary>
    public DateTime Org 
    {
      get
      {
        return this.m_AxisOrg;
      }
      set
      {
        this.m_AxisOrg = value;
      }
    }
    /// <summary>The axis end point in physical units.</summary>
    public DateTime End
    {
      get
      {
        return m_AxisEnd;
      }
      set
      {
        m_AxisEnd = value;
      }
    }
    // /// <summary>Indicates that the axis origin is fixed to a certain value.</summary>
    // public abstract bool   OrgFixed { get; set; }
    // /// <summary>Indicates that the axis end is fixed to a certain value.</summary>
    // public abstract bool   EndFixed { get; set; }


    SpanCompound m_MajorSpan;
    int m_MinorTicks;

    /// <summary>
    /// calculates the axis org and end using the databounds
    /// the org / end is adjusted only if it is not fixed
    /// and the DataBound object contains valid data
    /// </summary>
    
    public  void ProcessDataBounds(DateTime org, bool orgfixed, DateTime end, bool endfixed)
    {
      if(IsLinked)
        return;


      DateTime oldAxisOrg = this.m_AxisOrg;
      DateTime oldAxisEnd = this.m_AxisEnd;
    
     


      CalculateTicks(org, end, out m_MajorSpan, out m_MinorTicks);
      
      if(end>=org)
      {
        if(orgfixed)
          m_AxisOrg = org;
        else
          m_AxisOrg = m_MajorSpan.RoundDown(org);

        if(endfixed)
          m_AxisEnd = end;
        else
          m_AxisEnd = m_MajorSpan.RoundUp(end);
      }
      else // org is greater than end !
      {
        if(orgfixed)
          m_AxisOrg = org;
        else
          m_AxisOrg = m_MajorSpan.RoundUp(org);

        if(endfixed)
          m_AxisEnd = end;
        else
          m_AxisEnd = m_MajorSpan.RoundDown(end);
      }

      // compare with the saved values to find out whether or not something changed
      if( m_AxisOrg!=oldAxisOrg || m_AxisEnd!=oldAxisEnd)
      {
        OnChanged();
      }
    }

    /// <summary>
    /// This will set the axis to certain values in case there are no data available.
    /// </summary>
    void SetDefaultAxisValues()
    {
      ProcessDataBounds(DateTime.MinValue,true, DateTime.MinValue.AddDays(1),true);
    }


    public override void ProcessDataBounds()
    {
      if(null==this.m_DataBounds || this.m_DataBounds.IsEmpty)
        SetDefaultAxisValues();
      else
        ProcessDataBounds(m_DataBounds.LowerBound,m_DataBounds.UpperBound,_rescaling); 
    }


    public void ProcessDataBounds(DateTime xorg, DateTime xend, DateTimeAxisRescaleConditions rescaling)
    {
      bool isAutoOrg, isAutoEnd;
      rescaling.Process(ref xorg, out isAutoOrg, ref xend, out isAutoEnd);
      ProcessDataBounds(xorg,!isAutoOrg,xend,!isAutoEnd);
    }

    /// <summary>
    /// calculates the axis org and end using the databounds
    /// the org / end is adjusted only if it is not fixed
    /// and the DataBound object contains valid data
    /// </summary>
    public override void ProcessDataBounds(AltaxoVariant org, bool orgfixed, AltaxoVariant end, bool endfixed)
    {
      DateTime dorg;  
      DateTime dend; 
      if(org.IsType(AltaxoVariant.Content.VDateTime))
        dorg = (DateTime)org;
      else if(org.CanConvertedToDouble)
        dorg = new DateTime((long)(org.ToDouble()*1E7));
      else 
        throw new ArgumentException("Variant org is not a DateTime nor a numeric value");

      if(end.IsType(AltaxoVariant.Content.VDateTime))
        dend = (DateTime)end;
      else if(end.CanConvertedToDouble)
        dend = new DateTime((long)(end.ToDouble()*1E7));
      else 
        throw new ArgumentException("Variant end is not a DateTime nor a numeric value");


      ProcessDataBounds(dorg,orgfixed,dend,endfixed);
    }

    protected void EhBoundariesChanged(object sender, BoundariesChangedEventArgs e)
    {

      bool bIsRelevant=true;

      if(bIsRelevant) // if something really relevant changed
      {
        ProcessDataBounds(); // calculate new bounds and fire AxisChanged event
      }
    }
  
    readonly static TimeSpan[] _fixedSpan =
      {
        TimeSpan.FromTicks(1),
        TimeSpan.FromTicks(2),
        TimeSpan.FromTicks(4),
        TimeSpan.FromTicks(5),
        TimeSpan.FromTicks(10),
        TimeSpan.FromTicks(20),
        TimeSpan.FromTicks(40),
        TimeSpan.FromTicks(50),
        TimeSpan.FromTicks(100),
        TimeSpan.FromTicks(200),
        TimeSpan.FromTicks(400),
        TimeSpan.FromTicks(500),
        TimeSpan.FromTicks(1000),
        TimeSpan.FromTicks(2000),
        TimeSpan.FromTicks(4000),
        TimeSpan.FromTicks(5000),
        TimeSpan.FromMilliseconds(1),
        TimeSpan.FromMilliseconds(2),
        TimeSpan.FromMilliseconds(4),
        TimeSpan.FromMilliseconds(5),
        TimeSpan.FromMilliseconds(10),
        TimeSpan.FromMilliseconds(20),
        TimeSpan.FromMilliseconds(40),
        TimeSpan.FromMilliseconds(50),
        TimeSpan.FromMilliseconds(100),
        TimeSpan.FromMilliseconds(200),
        TimeSpan.FromMilliseconds(400),
        TimeSpan.FromMilliseconds(500),
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(4),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10),
        TimeSpan.FromSeconds(20),
        TimeSpan.FromSeconds(30),
        TimeSpan.FromMinutes(1),
        TimeSpan.FromMinutes(2),
        TimeSpan.FromMinutes(4),
        TimeSpan.FromMinutes(5),
        TimeSpan.FromMinutes(10),
        TimeSpan.FromMinutes(20),
        TimeSpan.FromMinutes(30),
        TimeSpan.FromHours(1),
        TimeSpan.FromHours(2),
        TimeSpan.FromHours(4),
        TimeSpan.FromHours(6),
        TimeSpan.FromHours(6),
        TimeSpan.FromHours(12),
        TimeSpan.FromDays(1),
        TimeSpan.FromDays(2),
        TimeSpan.FromDays(4),
        TimeSpan.FromDays(5),
        TimeSpan.FromDays(10),
        TimeSpan.FromDays(20)
      };


    enum Unit { Span, Month, Years }
    struct SpanCompound
    {
      public Unit _unit;
      public TimeSpan _span;
     

      public SpanCompound(Unit unit, TimeSpan span)
      {
        _unit = unit;
        _span = span;
      }

      public SpanCompound(Unit unit, long val)
      {
        _unit  = unit;
        _span = TimeSpan.FromTicks(val);
      }


      public DateTime RoundUp(DateTime d)
      {
        switch(_unit)
        {
          case Unit.Span:
            return RoundUpSpan(d);
          case Unit.Month:
            return RoundUpMonths(d);
          case Unit.Years:
            return RoundUpYears(d);
        }
        return d;  
      }

      public DateTime RoundDown(DateTime d)
      {
        switch(_unit)
        {
          case Unit.Span:
            return RoundDownSpan(d);
          case Unit.Month:
            return RoundDownMonths(d);
          case Unit.Years:
            return RoundDownYears(d);
        }
        return d;  
      }


      DateTime RoundUpSpan(DateTime d)
      {
        long dd = (d-DateTime.MinValue).Ticks;
        long rn = _span.Ticks;
        long re = dd % rn;
        return DateTime.MinValue + TimeSpan.FromTicks(re==0 ? dd : dd+rn-re);
      }
      DateTime RoundDownSpan(DateTime d)
      {
        long dd = (d-DateTime.MinValue).Ticks;
        long rn = _span.Ticks;
        long re = dd % rn;
        return DateTime.MinValue + TimeSpan.FromTicks(re==0 ? dd : dd-re);
      }
      DateTime RoundUpMonths(DateTime d)
      {
        int m = (int)_span.Ticks;
        System.Diagnostics.Debug.Assert(m>0 && m<=12);
        for( DateTime td = new DateTime(d.Year,d.Month,1);;td=td.AddMonths(1))
        {
          if(td>=d && 0==((td.Month-1)%m))
            return td;
        }
      }
   
      DateTime RoundDownMonths(DateTime d)
      {
        int m = (int)_span.Ticks;
        System.Diagnostics.Debug.Assert(m>0 && m<=12);
        for( DateTime td = new DateTime(d.Year,d.Month,1);;td=td.AddMonths(-1))
        {
          if(td<=d && 0==((td.Month-1)%m))
            return td;
        }
      }

      DateTime RoundUpYears(DateTime d)
      {
        int m = (int)_span.Ticks;
        return new DateTime(Altaxo.Calc.Rounding.RoundUp(d.Year+1,m),1,1);
      }
      DateTime RoundDownYears(DateTime d)
      {
        int m = (int)_span.Ticks;
        return new DateTime(Altaxo.Calc.Rounding.RoundDown(d.Year,m),1,1);
      }
    }
  

    static void CalculateTicks( 
      DateTime min,                // Minimum of data 
      DateTime max,                // Maximum of data
      out SpanCompound majorspan,      // the span between two major ticks
      out int minorticks      // number of ticks in a major tick span 
      )
    {
      if(min>max) // should not happen, but can happen when there are no data and min and max are uninitialized 
      {
        min=max=DateTime.MinValue;
      }

      TimeSpan span = max - min; // span width between max and min

      if(0==span.Ticks)
      {
        TimeSpan diff;
        // if span width is zero, then 1% of the velue, in case of min==max==0 we use 1
        if(DateTime.MinValue==max || DateTime.MinValue==min) // if one is null, the other should also be null, but to be secure...
          diff = TimeSpan.FromSeconds(1);
        else
          diff = TimeSpan.FromDays(1); // wir can be sure, that min==max, because span==0

        min -= diff;
        max += diff;
          
        span = max - min;
      } // if 0==span


      if(span>_fixedSpan[_fixedSpan.Length-1])
      {
        if(span>=TimeSpan.FromDays(365*4+1))
        {
          minorticks=0;
          majorspan = CalculateYearTicks(span);
        }
        else
        {
          minorticks=0;
          majorspan = CalculateMonthTicks(span);
        }
      }
      else
      {
        int i=_fixedSpan.Length-1;
        TimeSpan quarterspan = new TimeSpan(span.Ticks/4);
        for(i=_fixedSpan.Length-1;i>=0;i--)
        {
          if(_fixedSpan[i]<quarterspan)
            break;
        }
        minorticks=0;
        majorspan = new SpanCompound(Unit.Span,_fixedSpan[Math.Max(i,0)]);
        if(span<TimeSpan.FromTicks(6*majorspan._span.Ticks))
          minorticks=1;
      }
    } // end of function


    static SpanCompound CalculateYearTicks(TimeSpan span)
    {
      double years = span.TotalDays/(4*365+1);
      long yearexpo = 1;
      long yearmantissa=1;
      for(;;)
      {
        if(years<=4)
        {
          yearmantissa = 1;
          break;
        }
        else if(years<=8)
        {
          yearmantissa = 2;
          break;
        }
        else if(years<=16)
        {
          yearmantissa = 4;
          break;
        }
        else if(years<=20)
        {
          yearmantissa = 4;
          break;
        }

        else
        {
          years/=10;
          yearexpo*=10;
        }
      }
      return new SpanCompound(Unit.Years,yearexpo*yearmantissa);
    }

    const double _DaysPerMonth = 31;
    static SpanCompound CalculateMonthTicks(TimeSpan span)
    {
      double months = span.TotalDays/_DaysPerMonth;
      long m;

      if(months<=4)
        m = 1;
      else if(months<=8)
        m = 2;
      else if(months<=12)
        m = 3;
      else if(months<=16)
        m = 4;
      else if(months<=24)
        m = 6;
      else if(months<=48)
        m = 12;
      else
        return CalculateYearTicks(span);

      return new SpanCompound(Unit.Month,m);
    }



  } // end of class 
}

#endif
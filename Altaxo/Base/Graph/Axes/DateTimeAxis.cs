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
      return new DateTime[]{};
    }

    /// <summary>
    /// Returns the physical values
    /// at which minor ticks should occur
    /// </summary>
    /// <returns>physical values for the minor ticks</returns>
    public DateTime[] GetMinorTicks()
    {
      return new DateTime[]{};
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
    /// Returns the <see cref="PhysicalBoundaries"/> object that is associated with that axis.
    /// </summary>
    public FiniteDateTimeBoundaries DataBounds 
    {
      get
      {
        return this.m_DataBounds;
      }
    } // return a PhysicalBoundarie object that is associated with that axis
    
    /// <summary>
    /// Returns the <see cref="PhysicalBoundaries"/> object that is associated with that axis.
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

    /// <summary>
    /// calculates the axis org and end using the databounds
    /// the org / end is adjusted only if it is not fixed
    /// and the DataBound object contains valid data
    /// </summary>
    public  void ProcessDataBounds(DateTime org, bool orgfixed, DateTime end, bool endfixed)
    {
      this.m_AxisOrg = org;
      this.m_AxisEnd = end;
    }

    public override void ProcessDataBounds()
    {
      if(null==this.m_DataBounds || this.m_DataBounds.IsEmpty)
        return;
    
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
  
  } // end of class Axis
}

#endif
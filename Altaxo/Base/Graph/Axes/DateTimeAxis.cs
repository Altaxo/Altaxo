using System;

#if false
namespace Altaxo.Graph.Axes
{
	/// <summary>
	/// Summary description for DateTimeAxis.
	/// </summary>
	public class DateTimeAxis
  {
    // cached values
    /// <summary>Current axis origin (cached value).</summary>
    protected DateTime m_AxisOrg=DateTime.MinValue;
    /// <summary>Current axis end (cached value).</summary>
    protected DateTime m_AxisEnd=DateTime.MaxValue;

    #region ICloneable Members
    /// <summary>
    /// Creates a copy of the axis.
    /// </summary>
    /// <returns>The cloned copy of the axis.</returns>
    public abstract object Clone();
    #endregion
    
    #region IChangedEventSource Members

    /// <summary>
    /// Fired when the data of the axis has changed, for instance end point, org point, or tick spacing.
    /// </summary>
    public event System.EventHandler Changed;

    /// <summary>
    /// Used to fire the axis changed event, can be overriden in child classes.
    /// </summary>
    protected virtual void OnChanged()
    {
      OnChanged(new Main.ChangedEventArgs(this,null));
    }

    protected virtual void OnChanged(EventArgs e)
    {
      if(null!=Changed)
        Changed(this,e);
    }

    #endregion

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
      return PhysicalToNormal((DateTime)x);
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
    public  Altaxo.Data.AltaxoVariant[] GetMajorTicks()
    {
      return new AltaxoVariant[]{};
    }


    /// <summary>
    /// Returns the rescaling conditions for this axis
    /// </summary>
    public abstract NumericAxisRescaleConditions Rescaling { get; }

  
    /// <summary>
    /// GetMinorTicks returns the physical values
    /// at which minor ticks should occur
    /// </summary>
    /// <returns>physical values for the minor ticks</returns>
    public virtual Altaxo.Data.AltaxoVariant[] GetMinorTicks()
    {
      return new double[]{}; // return a empty array per default
    }


    /// <summary>
    /// Returns the <see cref="PhysicalBoundaries"/> object that is associated with that axis.
    /// </summary>
    public abstract PhysicalBoundaries DataBounds { get; } // return a PhysicalBoundarie object that is associated with that axis

    /// <summary>The axis origin, i.e. the first point in physical units.</summary>
    public abstract double Org { get; set;}
    /// <summary>The axis end point in physical units.</summary>
    public abstract double End { get; set;}
    // /// <summary>Indicates that the axis origin is fixed to a certain value.</summary>
    // public abstract bool   OrgFixed { get; set; }
    // /// <summary>Indicates that the axis end is fixed to a certain value.</summary>
    // public abstract bool   EndFixed { get; set; }

    /// <summary>
    /// calculates the axis org and end using the databounds
    /// the org / end is adjusted only if it is not fixed
    /// and the DataBound object contains valid data
    /// </summary>
    public abstract void ProcessDataBounds(double org, bool orgfixed, double end, bool endfixed); 
    public abstract void ProcessDataBounds();


  

    /// <summary>
    /// Static collection that holds all available axis types.
    /// </summary>
    protected static System.Collections.Hashtable sm_AvailableAxes;
    
    /// <summary>
    /// Static constructor that initializes the collection of available axis types by searching in the current assembly for child classes of axis.
    /// </summary>
    static Axis()
    {
      sm_AvailableAxes = new System.Collections.Hashtable();

      System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
      foreach(System.Reflection.Assembly assembly in assemblies)
      {
        // test if the assembly supports Serialization
        
        Type[] definedtypes = assembly.GetTypes();
        foreach(Type definedtype in definedtypes)
        {
          if(definedtype.IsSubclassOf(typeof(Axis)) && !definedtype.IsAbstract)
            sm_AvailableAxes.Add(definedtype.Name,definedtype);
        }
      }
    }


    /// <summary>Returns the collection of available axes.</summary>
    public static System.Collections.Hashtable AvailableAxes 
    {
      get { return sm_AvailableAxes; }
    }
  } // end of class Axis
}

#endif
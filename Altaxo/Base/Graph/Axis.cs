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


namespace Altaxo.Graph
{
  /// <summary>
  /// Axis is the abstract base class of all axis types including linear axis, logarithmic axis and so on.
  /// </summary>
  public abstract class Axis : ICloneable, Main.IChangedEventSource
  {
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
    public abstract double PhysicalToNormal(double x);
    /// <summary>
    /// NormalToPhysical is the inverse function to PhysicalToNormal
    /// It translates a normalized value (0 for the axis origin, 1 for the axis end)
    /// into the physical value
    /// </summary>
    /// <param name="x">the normal value (0 for axis origin, 1 for axis end</param>
    /// <returns>the corresponding physical value</returns>
    public abstract double NormalToPhysical(double x);

    /// <summary>
    /// GetMajorTicks returns the physical values
    /// at which major ticks should occur
    /// </summary>
    /// <returns>physical values for the major ticks</returns>
    public abstract double[] GetMajorTicks();

  
    /// <summary>
    /// GetMinorTicks returns the physical values
    /// at which minor ticks should occur
    /// </summary>
    /// <returns>physical values for the minor ticks</returns>
    public virtual double[] GetMinorTicks()
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
    /// <summary>Indicates that the axis origin is fixed to a certain value.</summary>
    public abstract bool   OrgFixed { get; set; }
    /// <summary>Indicates that the axis end is fixed to a certain value.</summary>
    public abstract bool   EndFixed { get; set; }

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



  /// <summary>
  /// A linear axis, i.e a axis where physical values v can be translated to logical values l by v=a+b*l.
  /// </summary>
  [SerializationSurrogate(0,typeof(LinearAxis.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class LinearAxis : Axis, System.Runtime.Serialization.IDeserializationCallback
  {
    // primary values
    /// <summary>Proposed value of axis origin, proposed either by the lower physical boundary or by the user (if axis org is fixed).</summary>
    protected double m_BaseOrg=0; // proposed value of org
    /// <summary>Proposed value of axis end, proposed either by the upper physical boundary or by the user (if axis end is fixed).</summary>
    protected double m_BaseEnd=1; // proposed value of end
    /// <summary>Current axis origin divided by the major tick span value.</summary>
    protected double m_AxisOrgByMajor=0;
    /// <summary>Current axis end divided by the major tick span value.</summary>
    protected double m_AxisEndByMajor=5;
    /// <summary>Physical span value between two major ticks.</summary>
    protected double m_MajorSpan=0.2; // physical span value between two major ticks
    /// <summary>Minor ticks per Major tick ( if there is one minor tick between two major ticks m_minorticks is 2!</summary>
    protected int    m_MinorTicks=2;
    /// <summary>True if the axis org is fixed to m_BaseOrg.</summary>
    protected bool   m_AxisOrgFixed = false;
    /// <summary>True if the axis end is fixed to m_BaseEnd.</summary>
    protected bool   m_AxisEndFixed = false;
    /// <summary>Holds the <see cref="PhysicalBoundaries"/> for that axis.</summary>
    protected PhysicalBoundaries m_DataBounds = new FinitePhysicalBoundaries();

    // cached values
    /// <summary>Current axis origin (cached value).</summary>
    protected double m_AxisOrg=0;
    /// <summary>Current axis end (cached value).</summary>
    protected double m_AxisEnd=1;
    /// <summary>Current axis span (i.e. end-org) (cached value).</summary>
    protected double m_AxisSpan=1;
    /// <summary>Current inverse of axis span (cached value).</summary>
    protected double m_OneByAxisSpan=1;


    #region Serialization
    /// <summary>Used to serialize the LinearAxis Version 0.</summary>
    public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      /// <summary>
      /// Serializes LinearAxis Version 0.
      /// </summary>
      /// <param name="obj">The axis to serialize.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        LinearAxis s = (LinearAxis)obj;
        info.AddValue("BaseOrg",s.m_BaseOrg);  
        info.AddValue("BaseEnd",s.m_BaseEnd);  
        info.AddValue("MajorSpan",s.m_MajorSpan);
        info.AddValue("MinorTicks",s.m_MinorTicks);
        info.AddValue("OrgByMajor",s.m_AxisOrgByMajor);
        info.AddValue("EndByMajor",s.m_AxisEndByMajor);

        info.AddValue("OrgFixed",s.m_AxisOrgFixed);
        info.AddValue("EndFixed",s.m_AxisEndFixed);

        info.AddValue("Bounds",s.m_DataBounds);
      }
      /// <summary>
      /// Deserializes the Linear Axis Version 0.
      /// </summary>
      /// <param name="obj">The empty axis object to deserialize into.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      /// <param name="selector">The deserialization surrogate selector.</param>
      /// <returns>The deserialized linear axis.</returns>
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        LinearAxis s = (LinearAxis)obj;

        s.m_BaseOrg = (double)info.GetDouble("BaseOrg");
        s.m_BaseEnd = (double)info.GetDouble("BaseEnd");

        s.m_MajorSpan = (double)info.GetDouble("MajorSpan");
        s.m_MinorTicks = (int)info.GetInt32("MinorTicks");

        s.m_AxisOrgByMajor = (double)info.GetDouble("OrgByMajor");
        s.m_AxisEndByMajor = (double)info.GetDouble("EndByMajor");

        s.m_AxisOrgFixed = (bool)info.GetBoolean("OrgFixed");
        s.m_AxisEndFixed = (bool)info.GetBoolean("EndFixed");

        s.m_DataBounds = (FinitePhysicalBoundaries)info.GetValue("Bounds",typeof(FinitePhysicalBoundaries));
    
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinearAxis),0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        LinearAxis s = (LinearAxis)obj;
        info.AddValue("BaseOrg",s.m_BaseOrg);  
        info.AddValue("BaseEnd",s.m_BaseEnd);  
        info.AddValue("MajorSpan",s.m_MajorSpan);
        info.AddValue("MinorTicks",s.m_MinorTicks);
        info.AddValue("OrgByMajor",s.m_AxisOrgByMajor);
        info.AddValue("EndByMajor",s.m_AxisEndByMajor);

        info.AddValue("OrgFixed",s.m_AxisOrgFixed);
        info.AddValue("EndFixed",s.m_AxisEndFixed);

        info.AddValue("Bounds",s.m_DataBounds);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        LinearAxis s = null!=o ? (LinearAxis)o : new LinearAxis();

        s.m_BaseOrg = (double)info.GetDouble("BaseOrg");
        s.m_BaseEnd = (double)info.GetDouble("BaseEnd");

        s.m_MajorSpan = (double)info.GetDouble("MajorSpan");
        s.m_MinorTicks = (int)info.GetInt32("MinorTicks");

        s.m_AxisOrgByMajor = (double)info.GetDouble("OrgByMajor");
        s.m_AxisEndByMajor = (double)info.GetDouble("EndByMajor");

        s.m_AxisOrgFixed = (bool)info.GetBoolean("OrgFixed");
        s.m_AxisEndFixed = (bool)info.GetBoolean("EndFixed");

        s.m_DataBounds = (FinitePhysicalBoundaries)info.GetValue("Bounds",s);
  
        s.SetCachedValues();
        // restore the event chain
        s.m_DataBounds.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(s.OnBoundariesChanged);
  
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
      m_DataBounds.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(this.OnBoundariesChanged);
    }
    #endregion


    /// <summary>
    /// Creates a default linear axis with org=0 and end=1.
    /// </summary>
    public LinearAxis()
    {
      m_DataBounds = new FinitePhysicalBoundaries();
      m_DataBounds.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(this.OnBoundariesChanged);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">A other linear axis from which to copy from.</param>
    public LinearAxis(LinearAxis from)
    {
      this.m_AxisEnd        = from.m_AxisEnd;
      this.m_AxisEndByMajor = from.m_AxisEndByMajor;
      this.m_AxisEndFixed   = from.m_AxisEndFixed;
      this.m_AxisOrg        = from.m_AxisOrg;
      this.m_AxisOrgByMajor = from.m_AxisOrgByMajor;
      this.m_AxisOrgFixed   = from.m_AxisOrgFixed;
      this.m_AxisSpan       = from.m_AxisSpan;
      this.m_BaseEnd        = from.m_BaseEnd;
      this.m_BaseOrg        = from.m_BaseOrg;
      this.m_DataBounds     = null==from.m_DataBounds ? new FinitePhysicalBoundaries() : (PhysicalBoundaries)from.m_DataBounds.Clone(); 
      m_DataBounds.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(this.OnBoundariesChanged);
      this.m_MajorSpan      = from.m_MajorSpan;
      this.m_MinorTicks     = from.m_MinorTicks;
      this.m_OneByAxisSpan  = from.m_OneByAxisSpan;
    }

    public override object Clone()
    {
      return new LinearAxis(this);
    }

    /// <summary>
    /// Get/sets the axis origin (physical units).
    /// </summary>
    public override double Org
    {
      get { return m_AxisOrg; } 
      set 
      {
        m_AxisOrg = value;
        ProcessDataBounds(m_AxisOrg,true,m_AxisEnd,true);
      }
    }

    /// <summary>
    /// Get/sets the axis end (physical units).
    /// </summary>
    public override double End 
    {
      get { return m_AxisEnd; } 
      set
      { 
        m_AxisEnd = value;
        ProcessDataBounds(m_AxisOrg,true,m_AxisEnd,true);
      }
    }

    /// <summary>
    /// Get/sets the OrgFixed property. If true, the axis origin is fixed to a specific value.
    /// </summary>
    public override bool OrgFixed
    {
      get { return m_AxisOrgFixed; } 
      set { m_AxisOrgFixed = value; }
    }
  
    
    /// <summary>
    /// Get/sets the EndFixed property. If true, the axis end is fixed to a specific value.
    /// </summary>
    public override bool EndFixed 
    {
      get { return m_AxisEndFixed; } 
      set { m_AxisEndFixed = value; }
    }


    /// <summary>
    /// Get the internal DataBound object (mostly for merging).
    /// </summary>
    public override PhysicalBoundaries DataBounds 
    {
      get { return m_DataBounds; }
    }

    /// <summary>
    /// Converts a value in axis (physical) units to a "normalized" value, which is 0 for the axis org and 1 for the axis end.
    /// </summary>
    /// <param name="x">Value to convert (physical units).</param>
    /// <returns>Normalized value.</returns>
    public override double PhysicalToNormal(double x)
    {
      return (x- m_AxisOrg ) * m_OneByAxisSpan; 
    }

    public override double NormalToPhysical(double x)
    {
      return m_AxisOrg + x * m_AxisSpan;
    }

    public override double[] GetMajorTicks()
    {
      int j;
      double i,beg,end;
      double[] retv;
      if(m_AxisOrgByMajor<=m_AxisEndByMajor) // normal case org<end
      {
        beg=System.Math.Ceiling(m_AxisOrgByMajor);
        end=System.Math.Floor(m_AxisEndByMajor);
        retv = new double[1+(int)(end-beg)];
        for(j=0,i=beg;i<=end;i+=1,j++)
          retv[j]=i*m_MajorSpan;
      }
      else
      {
        beg=System.Math.Floor(m_AxisOrgByMajor);
        end=System.Math.Ceiling(m_AxisEndByMajor);
        retv = new double[1+(int)(beg-end)];
        for(j=0,i=beg;i>=end;i-=1,j++)
          retv[j]=i*m_MajorSpan;
      }
      return retv;
    }

    public override double[] GetMinorTicks()
    {
      int j;
      double i,beg,end;
      double[] retv;
      if(m_MinorTicks<2)
        return new double[]{}; // below 2 there are no minor ticks per definition

      if(m_AxisOrgByMajor<=m_AxisEndByMajor) // normal case org<end
      {
        beg=System.Math.Ceiling(m_AxisOrgByMajor);
        end=System.Math.Floor(m_AxisEndByMajor);
        int majorticks = 1+(int)(end-beg);
        beg = System.Math.Ceiling(m_AxisOrgByMajor*m_MinorTicks);
        end = System.Math.Floor(m_AxisEndByMajor*m_MinorTicks);
        int minorticks = 1+(int)(end-beg) - majorticks;
        retv = new double[minorticks];
        for(j=0,i=beg;i<=end && j<minorticks;i+=1)
        {
          if(i%m_MinorTicks!=0)
          {
            retv[j]=i*m_MajorSpan/m_MinorTicks;
            j++;
          }
        }
      }
      else
      {
        beg=System.Math.Floor(m_AxisOrgByMajor);
        end=System.Math.Ceiling(m_AxisEndByMajor);
        retv = new double[1+(int)(beg-end)];
        for(j=0,i=beg;i>=end;i-=1,j++)
          retv[j]=i*m_MajorSpan;
      }
      return retv;
    }

    public override void ProcessDataBounds()
    {
      if(null==this.m_DataBounds || this.m_DataBounds.IsEmpty)
        return;
    
      ProcessDataBounds(m_DataBounds.LowerBound,this.m_AxisOrgFixed,m_DataBounds.UpperBound,this.m_AxisEndFixed); 
    }


    public  override void ProcessDataBounds(double xorg, bool xorgfixed, double xend, bool xendfixed)
    {
      double oldAxisOrgByMajor = m_AxisOrgByMajor;
      double oldAxisEndByMajor = m_AxisEndByMajor;
      double oldMajorSpan      = m_MajorSpan;
      int    oldMinorTicks     = m_MinorTicks;

      m_BaseOrg = xorg;
      m_BaseEnd = xend;
      
      CalculateTicks(xorg, xend, out m_MajorSpan, out m_MinorTicks);
      if(xend>=xorg)
      {
        if(xorgfixed)
          m_AxisOrgByMajor = xorg/m_MajorSpan;
        else
          m_AxisOrgByMajor = System.Math.Floor(m_MinorTicks * xorg/m_MajorSpan)/m_MinorTicks;

        if(xendfixed)
          m_AxisEndByMajor = xend/m_MajorSpan;
        else
          m_AxisEndByMajor = System.Math.Ceiling(m_MinorTicks * xend /m_MajorSpan)/m_MinorTicks;
      }
      else // org is greater than end !
      {
        if(xorgfixed)
          m_AxisOrgByMajor = xorg/m_MajorSpan;
        else
          m_AxisOrgByMajor = System.Math.Ceiling(m_MinorTicks * xorg/m_MajorSpan)/m_MinorTicks;

        if(xendfixed)
          m_AxisEndByMajor = xend/m_MajorSpan;
        else
          m_AxisEndByMajor = System.Math.Floor(m_MinorTicks * xend /m_MajorSpan)/m_MinorTicks;
      }

      SetCachedValues();

      // compare with the saved values to find out whether or not something changed
      if(oldAxisOrgByMajor!=m_AxisOrgByMajor ||
        oldAxisEndByMajor!=m_AxisEndByMajor ||
        oldMajorSpan != m_MajorSpan ||
        oldMinorTicks != m_MinorTicks)
      {
        OnChanged();
      }
    }

    protected void SetCachedValues()
    {
      m_AxisOrg = m_AxisOrgByMajor * m_MajorSpan;
      m_AxisEnd = m_AxisEndByMajor * m_MajorSpan;
      m_AxisSpan = m_AxisEnd - m_AxisOrg;
      m_OneByAxisSpan = 1/m_AxisSpan;
    }

    protected void OnBoundariesChanged(object sender, BoundariesChangedEventArgs e)
    {
      bool bIsRelevant=false;
      bIsRelevant |= !this.m_AxisOrgFixed && e.LowerBoundChanged;
      bIsRelevant |= !this.m_AxisEndFixed && e.UpperBoundChanged;

      if(bIsRelevant) // if something really relevant changed
      {
        ProcessDataBounds(); // calculate new bounds and fire AxisChanged event
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


  /// <summary>
  /// Represents a logarithmic axis, i.e. the physical values v correspond to logical values l by v=a*10^(b*l).
  /// </summary>
  [SerializationSurrogate(0,typeof(Log10Axis.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class Log10Axis : Axis, System.Runtime.Serialization.IDeserializationCallback
  {
    /// <summary>Decimal logarithm of axis org.</summary>
    double m_Log10Org=0; // Log10 of physical axis org
    /// <summary>Decimal logarithm of axis end.</summary>
    double m_Log10End=1; // Log10 of physical axis end

    /// <summary>Number of decades per major tick.</summary>
    int    m_DecadesPerMajorTick=1; // how many decades is one major tick
    /// <summary>True when the axis org is fixed to a specific value.</summary>
    protected bool   m_AxisOrgFixed = false;
    /// <summary>True when the axis end is fixed to a specific value.</summary>
    protected bool   m_AxisEndFixed = false;

    /// <summary>The boundary object. It collectes only positive values for the axis is logarithmic.</summary>
    protected PhysicalBoundaries m_DataBounds = null;


    #region Serialization
    /// <summary>Used to serialize the Log10Axis Version 0.</summary>
    public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      /// <summary>
      /// Serializes Log10Axis Version 0.
      /// </summary>
      /// <param name="obj">The axis to serialize.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        Log10Axis s = (Log10Axis)obj;
        info.AddValue("Log10Org",s.m_Log10Org);  
        info.AddValue("Log10End",s.m_Log10End);  
        info.AddValue("DecadesPerMajor",s.m_DecadesPerMajorTick);

        info.AddValue("OrgFixed",s.m_AxisOrgFixed);
        info.AddValue("EndFixed",s.m_AxisEndFixed);

        info.AddValue("Bounds",s.m_DataBounds);
      }
      /// <summary>
      /// Deserializes the Linear Axis Version 0.
      /// </summary>
      /// <param name="obj">The empty axis object to deserialize into.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      /// <param name="selector">The deserialization surrogate selector.</param>
      /// <returns>The deserialized linear axis.</returns>
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        Log10Axis s = (Log10Axis)obj;

        s.m_Log10Org = (double)info.GetDouble("Log10Org");
        s.m_Log10End = (double)info.GetDouble("Log10End");

        s.m_DecadesPerMajorTick = (int)info.GetInt32("DecadesPerMajor");

        s.m_AxisOrgFixed = (bool)info.GetBoolean("OrgFixed");
        s.m_AxisEndFixed = (bool)info.GetBoolean("EndFixed");

        s.m_DataBounds = (PositiveFinitePhysicalBoundaries)info.GetValue("Bounds",typeof(PositiveFinitePhysicalBoundaries));
    
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Log10Axis),0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        Log10Axis s = (Log10Axis)obj;
        info.AddValue("Log10Org",s.m_Log10Org);  
        info.AddValue("Log10End",s.m_Log10End);  
        info.AddValue("DecadesPerMajor",s.m_DecadesPerMajorTick);

        info.AddValue("OrgFixed",s.m_AxisOrgFixed);
        info.AddValue("EndFixed",s.m_AxisEndFixed);

        info.AddValue("Bounds",s.m_DataBounds);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        Log10Axis s = null!=o ? (Log10Axis)o : new Log10Axis();

        s.m_Log10Org = (double)info.GetDouble("Log10Org");
        s.m_Log10End = (double)info.GetDouble("Log10End");

        s.m_DecadesPerMajorTick = (int)info.GetInt32("DecadesPerMajor");

        s.m_AxisOrgFixed = (bool)info.GetBoolean("OrgFixed");
        s.m_AxisEndFixed = (bool)info.GetBoolean("EndFixed");

        s.m_DataBounds = (PositiveFinitePhysicalBoundaries)info.GetValue("Bounds",typeof(PositiveFinitePhysicalBoundaries));
    
        s.m_DataBounds.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(s.OnBoundariesChanged);

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
      m_DataBounds.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(this.OnBoundariesChanged);
    }
    #endregion



    /// <summary>
    /// Creates a default logarithmic axis with org=1 and end=10.
    /// </summary>
    public Log10Axis()
    {
      m_DataBounds = new PositiveFinitePhysicalBoundaries();
      m_DataBounds.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(this.OnBoundariesChanged);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">The axis to copy from.</param>
    public Log10Axis(Log10Axis from)
    {
      this.m_AxisEndFixed = from.m_AxisEndFixed;
      this.m_AxisOrgFixed = from.m_AxisOrgFixed;
      this.m_DataBounds   = null==from.m_DataBounds ? new PositiveFinitePhysicalBoundaries() : (PhysicalBoundaries)from.m_DataBounds.Clone();
      m_DataBounds.BoundaryChanged += new PhysicalBoundaries.BoundaryChangedHandler(this.OnBoundariesChanged);
      this.m_DecadesPerMajorTick = from.m_DecadesPerMajorTick;
      this.m_Log10End = from.m_Log10End;
      this.m_Log10Org = from.m_Log10Org;
    }

    /// <summary>
    /// Creates a clone copy of this axis.
    /// </summary>
    /// <returns>The cloned copy.</returns>
    public override object Clone()
    {
      return new Log10Axis(this);
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
      return (log10x-m_Log10Org)/(m_Log10End-m_Log10Org);
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
      double log10x = m_Log10Org + (m_Log10End-m_Log10Org)*x;
      return Math.Pow(10,log10x);
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
      if(m_Log10Org<m_Log10End)
      {
        log10org = m_Log10Org;
        log10end = m_Log10End;
      }
      else
      {
        log10org = m_Log10End;
        log10end = m_Log10Org;
      }

      // calculate the number of major ticks

      int nFullDecades = (int)(1+Math.Floor(log10end)-Math.Ceiling(log10org));
      int nMajorTicks = (int)Math.Floor((nFullDecades+m_DecadesPerMajorTick-1)/m_DecadesPerMajorTick);


      double[] retval = new double[nMajorTicks];
      int beg=(int)Math.Ceiling(log10org);
      int end=(int)Math.Floor(log10end);

      int i,j;
      for(i=beg,j=0 ;(i<=end) && (j<nMajorTicks) ; i+=m_DecadesPerMajorTick,j++)
      {
        retval[j] = Math.Pow(10,i);
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
      if(m_Log10Org<m_Log10End)
      {
        log10org = m_Log10Org;
        log10end = m_Log10End;
      }
      else
      {
        log10org = m_Log10End;
        log10end = m_Log10Org;
      }

      double decadespan = Math.Abs(m_Log10Org-m_Log10End);

      // guess from the span the tickiness (i.e. the increment of the multiplicator)
      // so that not more than 50 minor ticks are visible
      double minorsperdecade = 50.0/decadespan;
      
      // do not allow more than 10 minors per decade than 
      if(decadespan>0.3 && minorsperdecade>10) minorsperdecade=10;

      // if minorsperdecade is lesser than one, we dont have minors, so we can
      // return an empty field
      if(minorsperdecade<=1)
        return new double[0];


      // ensure the minorsperdecade are one of the following values
      // 1,2,4,5,8,10,20,40,50,80,100 usw.
      double dec=1;
      for(int i=0;;i++)
      {
        double val;
        switch(i%5)
        {
          default:
          case 0: val=1*dec; break;
          case 1: val=2*dec; break;
          case 2: val=4*dec; break;
          case 3: val=5*dec; break;
          case 4: val=8*dec; dec*=10; break;
        }
        if(val>=minorsperdecade)
        {
          minorsperdecade=val;
          break;
        }
      }
      // now if minorsperdecade is at least 2, it is a good "even" value

      // now get the major ticks
      double [] majorticks = GetMajorTicks();
      // and calculate begin and end of minor ticks

      int majorcount = majorticks.Length;

      // of cause this increment is only valid in the decade between 1 and 10
      double minorincrement = 10/minorsperdecade;

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


    public override PhysicalBoundaries DataBounds
    {
      get { return this.m_DataBounds; }
    } // return a PhysicalBoundarie object that is associated with that axis

    public override double Org
    {
      get { return Math.Pow(10,m_Log10Org); } 
      set 
      {
        if(value>0)
        {
          ProcessDataBounds(value,true,Math.Pow(10,m_Log10End),true);
        }
      }
    }
    public override double End 
    {
      get { return Math.Pow(10,m_Log10End); } 
      set
      {
        if(value>0)
        {
          ProcessDataBounds(Math.Pow(10,m_Log10Org),true,value,true);
        }
      }
    }

    public override bool OrgFixed
    {
      get { return m_AxisOrgFixed; } 
      set { m_AxisOrgFixed = value; }
    }
    public override bool EndFixed 
    {
      get { return m_AxisEndFixed; } 
      set { m_AxisEndFixed = value; }
    }

    /// <summary>
    /// calculates the axis org and end using the databounds
    /// the org / end is adjusted only if it is not fixed
    /// and the DataBound object contains valid data
    /// </summary>
    public override void ProcessDataBounds(double org, bool orgfixed, double end, bool endfixed)
    {

      // if one of the bounds is not valid, use the old bounds instead

      double log10org = org>0 ? Math.Log10(org) : m_Log10Org;
      double log10end = end>0 ? Math.Log10(end) : m_Log10End;


      // do something if org and end are the same
      if(log10org==log10end)
      {
        log10org += 1;
        log10end -= 1;
      }

      // calculate the number of decades between end and org
      double decades = Math.Abs(log10end-log10org);

      // limit the number of major ticks to about 10
      m_DecadesPerMajorTick = (int)Math.Ceiling(decades/10.0);


      m_Log10Org = log10org;
      m_Log10End = log10end;

    }
    public override void ProcessDataBounds()
    {
      if(null==this.m_DataBounds || this.m_DataBounds.IsEmpty)
        return;
    
      ProcessDataBounds(m_DataBounds.LowerBound,this.m_AxisOrgFixed,m_DataBounds.UpperBound,this.m_AxisEndFixed); 
    }

    protected void OnBoundariesChanged(object sender, BoundariesChangedEventArgs e)
    {
      bool bIsRelevant=false;
      bIsRelevant |= !this.m_AxisOrgFixed && e.LowerBoundChanged;
      bIsRelevant |= !this.m_AxisEndFixed && e.UpperBoundChanged;

      if(bIsRelevant) // if something really relevant changed
      {
        ProcessDataBounds(); // calculate new bounds and fire AxisChanged event
      }
    }


  } // end of class Log10Axis
}

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
using Altaxo.Graph.Axes.Scaling;
using Altaxo.Graph.Axes.Boundaries;

namespace Altaxo.Graph.Axes
{
  /// <summary>
  /// A linear axis, i.e a axis where physical values v can be translated to logical values l by v=a+b*l.
  /// </summary>
  [SerializationSurrogate(0,typeof(LinearAxis.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class LinearAxis : NumericalAxis, System.Runtime.Serialization.IDeserializationCallback
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
   
    /// <summary>Holds the <see cref="NumericalBoundaries"/> for that axis.</summary>
    protected NumericalBoundaries m_DataBounds = new FiniteNumericalBoundaries();

    protected NumericAxisRescaleConditions _rescaling = new NumericAxisRescaleConditions();

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

        // info.AddValue("OrgFixed",s.m_AxisOrgFixed);
        // info.AddValue("EndFixed",s.m_AxisEndFixed);

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

        // s.m_AxisOrgFixed = (bool)info.GetBoolean("OrgFixed");
        // s.m_AxisEndFixed = (bool)info.GetBoolean("EndFixed");

        s.m_DataBounds = (FiniteNumericalBoundaries)info.GetValue("Bounds",typeof(FiniteNumericalBoundaries));
    
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.LinearAxis",0)]
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

        // info.AddValue("OrgFixed",s.m_AxisOrgFixed);
        // info.AddValue("EndFixed",s.m_AxisEndFixed);

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

        bool AxisOrgFixed = (bool)info.GetBoolean("OrgFixed");
        bool AxisEndFixed = (bool)info.GetBoolean("EndFixed");

        s.m_DataBounds = (FiniteNumericalBoundaries)info.GetValue("Bounds",s);
  
        s.SetCachedValues();
        // restore the event chain
        s.m_DataBounds.BoundaryChanged += new BoundaryChangedHandler(s.OnBoundariesChanged);
  
        s._rescaling = new NumericAxisRescaleConditions();
        s._rescaling.SetOrgAndEnd(AxisOrgFixed ? BoundaryRescaling.Fixed : BoundaryRescaling.Auto, s.Org, AxisEndFixed ? BoundaryRescaling.Fixed:BoundaryRescaling.Auto, s.End);

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinearAxis),1)]
      public class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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

        // info.AddValue("OrgFixed",s.m_AxisOrgFixed); // removed in version 1
        // info.AddValue("EndFixed",s.m_AxisEndFixed); // removed in version 1

        info.AddValue("Bounds",s.m_DataBounds);

        // new in version 1:
        info.AddValue("Rescaling",s._rescaling);
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

        //s.m_AxisOrgFixed = (bool)info.GetBoolean("OrgFixed");
        //s.m_AxisEndFixed = (bool)info.GetBoolean("EndFixed");

        s.m_DataBounds = (FiniteNumericalBoundaries)info.GetValue("Bounds",s);
  
        s.SetCachedValues();
        // restore the event chain
        s.m_DataBounds.BoundaryChanged += new BoundaryChangedHandler(s.OnBoundariesChanged);
  
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
      m_DataBounds.BoundaryChanged += new BoundaryChangedHandler(this.OnBoundariesChanged);
    }
    #endregion


    /// <summary>
    /// Creates a default linear axis with org=0 and end=1.
    /// </summary>
    public LinearAxis()
    {
      m_DataBounds = new FiniteNumericalBoundaries();
      m_DataBounds.BoundaryChanged += new BoundaryChangedHandler(this.OnBoundariesChanged);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="from">A other linear axis from which to copy from.</param>
    public LinearAxis(LinearAxis from)
    {
      this.IsLinked = from.IsLinked;

      this.m_AxisEnd        = from.m_AxisEnd;
      this.m_AxisEndByMajor = from.m_AxisEndByMajor;
      this.m_AxisOrg        = from.m_AxisOrg;
      this.m_AxisOrgByMajor = from.m_AxisOrgByMajor;
      this.m_AxisSpan       = from.m_AxisSpan;
      this.m_BaseEnd        = from.m_BaseEnd;
      this.m_BaseOrg        = from.m_BaseOrg;
      this.m_DataBounds     = null==from.m_DataBounds ? new FiniteNumericalBoundaries() : (NumericalBoundaries)from.m_DataBounds.Clone(); 
      m_DataBounds.BoundaryChanged += new BoundaryChangedHandler(this.OnBoundariesChanged);
      this.m_MajorSpan      = from.m_MajorSpan;
      this.m_MinorTicks     = from.m_MinorTicks;
      this.m_OneByAxisSpan  = from.m_OneByAxisSpan;

      this._rescaling = null==from.Rescaling ? new NumericAxisRescaleConditions() : (NumericAxisRescaleConditions)from.Rescaling.Clone();

    }

    public virtual void CopyFrom(LinearAxis from)
    {
      this.m_AxisEnd        = from.m_AxisEnd;
      this.m_AxisEndByMajor = from.m_AxisEndByMajor;
      this.m_AxisOrg        = from.m_AxisOrg;
      this.m_AxisOrgByMajor = from.m_AxisOrgByMajor;
      this.m_AxisSpan       = from.m_AxisSpan;
      this.m_BaseEnd        = from.m_BaseEnd;
      this.m_BaseOrg        = from.m_BaseOrg;
      if(null!=m_DataBounds)
        m_DataBounds.BoundaryChanged -= new BoundaryChangedHandler(this.OnBoundariesChanged);
      this.m_DataBounds     = null==from.m_DataBounds ? new FiniteNumericalBoundaries() : (NumericalBoundaries)from.m_DataBounds.Clone(); 
      m_DataBounds.BoundaryChanged += new BoundaryChangedHandler(this.OnBoundariesChanged);
      this.m_MajorSpan      = from.m_MajorSpan;
      this.m_MinorTicks     = from.m_MinorTicks;
      this.m_OneByAxisSpan  = from.m_OneByAxisSpan;

      this._rescaling = null==from.Rescaling ? new NumericAxisRescaleConditions() : (NumericAxisRescaleConditions)from.Rescaling.Clone();
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
    
      ProcessDataBounds(m_DataBounds.LowerBound,m_DataBounds.UpperBound,_rescaling); 
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
     

      bool bIsRelevant=true;

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


}

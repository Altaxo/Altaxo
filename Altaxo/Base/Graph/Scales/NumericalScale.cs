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
using Altaxo.Data;

namespace Altaxo.Graph.Scales
{
  using Rescaling;
  using Boundaries;

  /// <summary>
  /// Axis is the abstract base class of all axis types including linear axis, logarithmic axis and so on.
  /// </summary>
  [Serializable]
  public abstract class NumericalScale : Scale
  {
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
    /// Returns the rescaling conditions for this axis
    /// </summary>
    public abstract NumericAxisRescaleConditions Rescaling { get; }
    public override object RescalingObject
    {
      get
      {
        return Rescaling;
      }
    }
  
    /// <summary>
    /// GetMinorTicks returns the physical values
    /// at which minor ticks should occur
    /// </summary>
    /// <returns>physical values for the minor ticks</returns>
    public virtual double[] GetMinorTicks()
    {
      return new double[]{}; // return a empty array per default
    }


    public override double[] GetMajorTicksNormal()
    {
      double[] ticks = GetMajorTicks();
      for(int i=0;i<ticks.Length;i++)
      {
        ticks[i] = PhysicalToNormal(ticks[i]);
      }
      return ticks;
    }
    public override double[] GetMinorTicksNormal()
    {
      double[] ticks = GetMinorTicks();
      for(int i=0;i<ticks.Length;i++)
      {
        ticks[i] = PhysicalToNormal(ticks[i]);
      }
      return ticks;
    }
    public override AltaxoVariant[] GetMajorTicksAsVariant()
    {
      double[] ticks = GetMajorTicks();
      AltaxoVariant[] vticks = new AltaxoVariant[ticks.Length];
      for(int i=0;i<ticks.Length;++i)
        vticks[i] = ticks[i];
      return vticks;
    }
    public override AltaxoVariant[] GetMinorTicksAsVariant()
    {
      double[] ticks = GetMinorTicks();
      AltaxoVariant[] vticks = new AltaxoVariant[ticks.Length];
      for(int i=0;i<ticks.Length;++i)
        vticks[i] = ticks[i];
      return vticks;
    }


    /// <summary>
    /// Returns the <see cref="NumericalBoundaries"/> object that is associated with that axis.
    /// </summary>
    public abstract NumericalBoundaries DataBounds { get; } // return a PhysicalBoundarie object that is associated with that axis
    public override IPhysicalBoundaries DataBoundsObject
    {
      get { return DataBounds; }
    }

    /// <summary>The axis origin, i.e. the first point in physical units.</summary>
    public abstract double Org { get; set;}
    /// <summary>The axis end point in physical units.</summary>
    public abstract double End { get; set;}

    public override AltaxoVariant OrgAsVariant
    {
      get
      {
        return new AltaxoVariant (Org);
      }
      set
      {
        Org = (double)value;
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
        End = (double)value;
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
    public abstract void ProcessDataBounds(double org, bool orgfixed, double end, bool endfixed); 

    /// <summary>
    /// calculates the axis org and end using the databounds
    /// the org / end is adjusted only if it is not fixed
    /// and the DataBound object contains valid data
    /// </summary>
    public override void ProcessDataBounds(AltaxoVariant org, bool orgfixed, AltaxoVariant end, bool endfixed)
    {
      double dorg = org.ToDouble();
      double dend = end.ToDouble();

      
      ProcessDataBounds(dorg,orgfixed,dend,endfixed);
    }
  
  } // end of class

}

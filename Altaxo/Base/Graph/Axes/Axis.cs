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


namespace Altaxo.Graph.Axes
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
    /// PhysicalVariantToNormal translates physical values into a normal value linear along the axis
    /// a physical value of the axis origin must return a value of zero
    /// a physical value of the axis end must return a value of one
    /// the function physicalToNormal must be provided by any derived class
    /// </summary>
    /// <param name="x">the physical value</param>
    /// <returns>
    /// the normalized value linear along the axis,
    /// 0 for axis origin, 1 for axis end</returns>
    public abstract double PhysicalVariantToNormal(Altaxo.Data.AltaxoVariant x);
    /// <summary>
    /// NormalToPhysicalVariant is the inverse function to PhysicalToNormal
    /// It translates a normalized value (0 for the axis origin, 1 for the axis end)
    /// into the physical value
    /// </summary>
    /// <param name="x">the normal value (0 for axis origin, 1 for axis end</param>
    /// <returns>the corresponding physical value</returns>
    public abstract Altaxo.Data.AltaxoVariant NormalToPhysicalVariant(double x);

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

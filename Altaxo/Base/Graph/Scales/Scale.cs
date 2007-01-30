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
  public abstract class Scale : ICloneable, Main.IChangedEventSource
  {
    /// <summary>
    /// Fired when the data of the axis has changed, for instance end point, org point, or tick spacing.
    /// </summary>
    [field:NonSerialized]
    public event System.EventHandler Changed;



    #region ICloneable Members
    /// <summary>
    /// Creates a copy of the axis.
    /// </summary>
    /// <returns>The cloned copy of the axis.</returns>
    public abstract object Clone();
    #endregion
    
    #region IChangedEventSource Members

   
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
    /// This will return the the major ticks as <see cref="AltaxoVariant" />.
    /// </summary>
    /// <returns>The array with major tick values.</returns>
    public abstract AltaxoVariant[] GetMajorTicksAsVariant();

    /// <summary>
    /// This will return the location of the major ticks relative on this axis, that mean
    /// the value range is 0..1.
    /// </summary>
    /// <returns>The array with relative (normal) major tick values.</returns>
    public abstract double[] GetMajorTicksNormal();

    /// <summary>
    /// This will return the minor ticks as array of <see cref="AltaxoVariant" />.
    /// </summary>
    /// <returns>The array with minor tick values.</returns>
    public abstract AltaxoVariant[] GetMinorTicksAsVariant();


    /// <summary>
    /// This will return the location of the minor ticks relative on this axis, that mean
    /// the value range is 0..1.
    /// </summary>
    /// <returns>The array with relative (normal) minor tick values.</returns>
    public abstract double[] GetMinorTicksNormal();


    /// <summary>
    /// Returns the rescaling conditions for this axis as object.
    /// </summary>
    public abstract object RescalingObject { get; }

    /// <summary>
    /// Returns the <see cref="NumericalBoundaries"/> object that is associated with that axis.
    /// </summary>
    public abstract IPhysicalBoundaries DataBoundsObject { get; } // return a PhysicalBoundarie object that is associated with that axis


    /// <summary>The axis origin, i.e. the first point in physical units.</summary>
    public abstract AltaxoVariant OrgAsVariant { get; set;}
    /// <summary>The axis end point in physical units.</summary>
    public abstract AltaxoVariant EndAsVariant { get; set;}

    public abstract void ProcessDataBounds();

    
    /// <summary>
    /// calculates the axis org and end using the databounds
    /// the org / end is adjusted only if it is not fixed
    /// and the DataBound object contains valid data
    /// </summary>
    public abstract void ProcessDataBounds(AltaxoVariant org, bool orgfixed, AltaxoVariant end, bool endfixed); 

    /// <summary>
    /// True if the axis is linked to another. In this case, do not process the data bounds.
    /// The layer that controls this axis has to set IsLinked temporarily to false in order
    /// to be able to set the data bounds of this axis.
    /// </summary>
    public bool IsLinked;


    /// <summary>
    /// Static collection that holds all available axis types.
    /// </summary>
    protected static System.Collections.Hashtable sm_AvailableAxes;
    
    /// <summary>
    /// Static constructor that initializes the collection of available axis types by searching in the current assembly for child classes of axis.
    /// </summary>
    static Scale()
    {
      sm_AvailableAxes = new System.Collections.Hashtable();

      System.Type[] types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Scale));
      foreach (System.Type definedtype in types)
      {
        if(definedtype.IsVisible)
          sm_AvailableAxes.Add(definedtype.Name, definedtype);
      }
      
    }


    /// <summary>Returns the collection of available axes.</summary>
    public static System.Collections.Hashtable AvailableAxes 
    {
      get { return sm_AvailableAxes; }
    }
  } // end of class Axis

}

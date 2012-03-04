#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

		private static int _instanceCounter;
		private readonly int _instance = _instanceCounter++;


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
    /// Returns the rescaling conditions for this axis as object.
    /// </summary>
    public abstract object RescalingObject { get; }

    /// <summary>
		/// Returns the <see cref="IPhysicalBoundaries"/> object that is associated with that axis.
    /// </summary>
    public abstract IPhysicalBoundaries DataBoundsObject { get; } // return a PhysicalBoundarie object that is associated with that axis


    /// <summary>The axis origin, i.e. the first point in physical units.</summary>
    public abstract AltaxoVariant OrgAsVariant { get; }

    /// <summary>The axis end point in physical units.</summary>
    public abstract AltaxoVariant EndAsVariant { get;}

		/// <summary>Returns true if it is allowed to extend the origin (to lower values).</summary>
		public abstract bool IsOrgExtendable { get; }

		/// <summary>Returns true if it is allowed to extend the scale end (to higher values).</summary>
		public abstract bool IsEndExtendable { get; }

		/// <summary>
		/// Sets the orgin and the end of the scale temporarily (until the next DataBoundaryChanged event).
		/// </summary>
		/// <param name="org">The scale origin.</param>
		/// <param name="end">The scale end.</param>
		/// <returns>Null when the settings where applied. An string describing the problem otherwise.</returns>
		/// <remarks>Settings like fixed boundaries or the data bounds will be ignored by this function. However, the next call
		/// to <see cref="Rescale"/> will override the scale bounds.</remarks>
		public abstract string SetScaleOrgEnd(AltaxoVariant org, AltaxoVariant end);

    
		/// <summary>
		/// Adjusts org and end considering fixed org and end values and the data boundaries.
		/// </summary>
		public abstract void Rescale();

    
    

    /// <summary>
    /// Static collection that holds all available axis types.
    /// </summary>
    protected static System.Collections.Generic.Dictionary<string,Type> sm_AvailableScales;
    
    /// <summary>
    /// Static constructor that initializes the collection of available axis types by searching in the current assembly for child classes of axis.
    /// </summary>
    static Scale()
    {
			sm_AvailableScales = new System.Collections.Generic.Dictionary<string, Type>();

      System.Type[] types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Scale));
      foreach (System.Type definedtype in types)
      {
        if(definedtype.IsVisible)
          sm_AvailableScales.Add(definedtype.Name, definedtype);
      }
      
    }


    /// <summary>Returns the collection of available axes.</summary>
		public static System.Collections.Generic.Dictionary<string, Type> AvailableAxes 
    {
      get { return sm_AvailableScales; }
    }

		#region IDocumentNode Members

		public object ParentObject
		{
			get { throw new NotImplementedException(); }
		}

		public string Name
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	} // end of class Axis

}

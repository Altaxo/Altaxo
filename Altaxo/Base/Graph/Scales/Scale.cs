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

#endregion Copyright

#nullable enable
using System;
using System.Collections.Generic;
using Altaxo.Data;

namespace Altaxo.Graph.Scales
{
  using Boundaries;
  using Ticks;

  /// <summary>
  /// Axis is the abstract base class of all axis types including linear axis, logarithmic axis and so on.
  /// </summary>
  [Serializable]
  public abstract class Scale
    :
    Main.SuspendableDocumentNodeWithSetOfEventArgs,
    Main.ICopyFrom
  {
    private static int _instanceCounter;
    private readonly int _instance = _instanceCounter++;

    /// <summary>
    /// Static collection that holds all available axis types.
    /// </summary>
    protected static System.Collections.Generic.Dictionary<string, Type> sm_AvailableScales;

    #region ICloneable Members

    /// <summary>
    /// Creates a copy of the axis.
    /// </summary>
    /// <returns>The cloned copy of the axis.</returns>
    public abstract object Clone();

    public abstract bool CopyFrom(object obj);

    #endregion ICloneable Members

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
    public abstract Rescaling.IScaleRescaleConditions? RescalingObject { get; }

    public abstract TickSpacing TickSpacing { get; set; }

    /// <summary>
    /// Returns the <see cref="IPhysicalBoundaries"/> object that is associated with that axis.
    /// </summary>
    public abstract IPhysicalBoundaries DataBoundsObject { get; } // return a PhysicalBoundaries object that is associated with that axis

    /// <summary>The axis origin, i.e. the first point in physical units.</summary>
    public abstract AltaxoVariant OrgAsVariant { get; }

    /// <summary>The axis end point in physical units.</summary>
    public abstract AltaxoVariant EndAsVariant { get; }

    /// <summary>
    /// Sets the orgin and the end of the scale temporarily (until the next DataBoundaryChanged event).
    /// </summary>
    /// <param name="org">The scale origin.</param>
    /// <param name="end">The scale end.</param>
    /// <returns>Null when the settings where applied. An string describing the problem otherwise.</returns>
    /// <remarks>Settings like fixed boundaries or the data bounds will be ignored by this function. However, the next call
    /// to <see cref="OnUserRescaled"/> will override the scale bounds.</remarks>
    protected abstract string? SetScaleOrgEnd(AltaxoVariant org, AltaxoVariant end);

    /// <summary>
    /// Called when user has pressed the rescale button.
    /// </summary>
    public abstract void OnUserRescaled();

    /// <summary>
    /// Called when user zoomed has used the zoom tool to zoom into the data.
    /// </summary>
    /// <param name="newZoomOrg">The new zoom orgigin.</param>
    /// <param name="newZoomEnd">The new zoom end.</param>
    public abstract void OnUserZoomed(AltaxoVariant newZoomOrg, AltaxoVariant newZoomEnd);

    /// <summary>
    /// Static constructor that initializes the collection of available axis types by searching in the current assembly for child classes of axis.
    /// </summary>
    static Scale()
    {
      sm_AvailableScales = new System.Collections.Generic.Dictionary<string, Type>();

      System.Type[] types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Scale));
      foreach (System.Type definedtype in types)
      {
        if (definedtype.IsVisible)
          sm_AvailableScales.Add(definedtype.Name, definedtype);
      }

      RegisterDefaultTicking();
    }

    /// <summary>Returns the collection of available axes.</summary>
    public static System.Collections.Generic.Dictionary<string, Type> AvailableAxes
    {
      get { return sm_AvailableScales; }
    }

    #region Default ticking

    private static Dictionary<System.Type, SortedDictionary<int, System.Type>> _scaleToTickSpacingTypes = new Dictionary<Type, SortedDictionary<int, Type>>();

    public static void RegisterDefaultTicking(System.Type scaleType, System.Type tickSpacingType, int priority)
    {
      if (!_scaleToTickSpacingTypes.ContainsKey(scaleType))
        _scaleToTickSpacingTypes.Add(scaleType, new SortedDictionary<int, Type>());
      _scaleToTickSpacingTypes[scaleType].Add(priority, tickSpacingType);
    }

    public static TickSpacing CreateDefaultTicks(System.Type type)
    {
      if (_scaleToTickSpacingTypes.ContainsKey(type))
      {
        SortedDictionary<int, Type> dict = _scaleToTickSpacingTypes[type];

        foreach (KeyValuePair<int, System.Type> entry in dict)
        {
          var result = (TickSpacing?)System.Activator.CreateInstance(entry.Value);
          if (result is not null)
            return result;
        }
      }

      return new NoTickSpacing();
    }

    private static void RegisterDefaultTicking()
    {
      RegisterDefaultTicking(typeof(DateTimeScale), typeof(DateTimeTickSpacing), 100);

      RegisterDefaultTicking(typeof(AngularDegreeScale), typeof(AngularDegreeTickSpacing), 100);

      RegisterDefaultTicking(typeof(AngularRadianScale), typeof(AngularRadianTickSpacing), 100);

      RegisterDefaultTicking(typeof(TextScale), typeof(TextTickSpacing), 100);

      RegisterDefaultTicking(typeof(Log10Scale), typeof(Log10TickSpacing), 100);

      RegisterDefaultTicking(typeof(LinearScale), typeof(LinearTickSpacing), 100);

      RegisterDefaultTicking(typeof(InverseScale), typeof(InverseTickSpacing), 100);
    }

    #endregion Default ticking
  }
}

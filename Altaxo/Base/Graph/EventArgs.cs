#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;

namespace Altaxo.Graph
{
  /// <summary>
  /// Provides event data for changes in plot item data.
  /// </summary>
  public class PlotItemDataChangedEventArgs : EventArgs
  {
    /// <summary>
    /// Gets the shared empty instance.
    /// </summary>
    public static new readonly PlotItemDataChangedEventArgs Empty = new PlotItemDataChangedEventArgs();

    private PlotItemDataChangedEventArgs()
    {
    }
  }

  /// <summary>
  /// Provides event data for changes in plot item style.
  /// </summary>
  public class PlotItemStyleChangedEventArgs : EventArgs
  {
    /// <summary>
    /// Gets the shared empty instance.
    /// </summary>
    public static new readonly PlotItemStyleChangedEventArgs Empty = new PlotItemStyleChangedEventArgs();

    private PlotItemStyleChangedEventArgs()
    {
    }
  }

  /// <summary>
  /// Designates what has changed in the boundaries of a plot item.
  /// </summary>
  [Flags]
  public enum BoundariesChangedData
  {
    /// <summary>The number of data points changed.</summary>
    NumberOfItemsChanged = 0x01,

    /// <summary>The lower boundary has changed.</summary>
    LowerBoundChanged = 0x02,

    /// <summary>The upper boundary has changed.</summary>
    UpperBoundChanged = 0x04,

    /// <summary>The boundaries related to the x-scale (1st independent variable) have changed.</summary>
    XBoundariesChanged = 0x10,

    /// <summary>The boundaries related to the y-scale (2nd independent variable) have changed.</summary>
    YBoundariesChanged = 0x20,

    /// <summary>The boundaries related to the z-scale (3rd independent variable) have changed.</summary>
    ZBoundariesChanged = 0x40,

    /// <summary>The boundaries related to the v-scale (dependent variable) have changed.</summary>
    VBoundariesChanged = 0x80,

    /// <summary>A complex change, including all possible kind of changes (for instance, if a new plot item was added to the layer).</summary>
    ComplexChange = 0xFF
  }

  /// <summary>
  /// Provides event data for changes in plot boundaries.
  /// </summary>
  public class BoundariesChangedEventArgs : Main.SelfAccumulateableEventArgs
  {
    /// <summary>
    /// The accumulated boundary change flags.
    /// </summary>
    protected BoundariesChangedData _data;

    /// <summary>
    /// Gets the kind of boundary changes contained in this instance.
    /// </summary>
    public BoundariesChangedData Data { get { return _data; } }

    /// <summary>
    /// Creates an instance from flags indicating whether the lower or upper bound changed.
    /// </summary>
    public static T FromLowerAndUpperBoundChanged<T>(bool haslowerBoundChanged, bool hasUpperBoundChanged) where T : BoundariesChangedEventArgs, new()
    {
      var result = new T();

      if (haslowerBoundChanged)
        result._data |= BoundariesChangedData.LowerBoundChanged;
      if (hasUpperBoundChanged)
        result._data |= BoundariesChangedData.UpperBoundChanged;

      return result;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundariesChangedEventArgs"/> class.
    /// </summary>
    public BoundariesChangedEventArgs()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundariesChangedEventArgs"/> class.
    /// </summary>
    /// <param name="data">The boundary change flags.</param>
    public BoundariesChangedEventArgs(BoundariesChangedData data)
    {
      _data = data;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundariesChangedEventArgs"/> class.
    /// </summary>
    /// <param name="bLowerBound"><see langword="true"/> if the lower bound changed.</param>
    /// <param name="bUpperBound"><see langword="true"/> if the upper bound changed.</param>
    public BoundariesChangedEventArgs(bool bLowerBound, bool bUpperBound)
    {
      if (bLowerBound)
        _data |= BoundariesChangedData.LowerBoundChanged;
      if (bUpperBound)
        _data |= BoundariesChangedData.UpperBoundChanged;
    }

    /// <summary>
    /// Gets a value indicating whether the lower bound changed.
    /// </summary>
    public bool LowerBoundChanged
    {
      get { return _data.HasFlag(BoundariesChangedData.LowerBoundChanged); }
    }

    /// <summary>
    /// Gets a value indicating whether the upper bound changed.
    /// </summary>
    public bool UpperBoundChanged
    {
      get { return _data.HasFlag(BoundariesChangedData.UpperBoundChanged); }
    }

    /// <summary>
    /// Sets the flag that indicates a change of the x-boundary.
    /// </summary>
    public void SetXBoundaryChangedFlag()
    {
      _data |= BoundariesChangedData.XBoundariesChanged;
    }

    /// <summary>
    /// Sets the flag that indicates a change of the y-boundary.
    /// </summary>
    public void SetYBoundaryChangedFlag()
    {
      _data |= BoundariesChangedData.YBoundariesChanged;
    }

    /// <summary>
    /// Sets the flag that indicates a change of the z-boundary.
    /// </summary>
    public void SetZBoundaryChangedFlag()
    {
      _data |= BoundariesChangedData.ZBoundariesChanged;
    }

    /// <summary>
    /// Sets the flag that indicates a change of the v-boundary.
    /// </summary>
    public void SetVBoundaryChangedFlag()
    {
      _data |= BoundariesChangedData.VBoundariesChanged;
    }

    /// <summary>
    /// Adds the changes from another <see cref="BoundariesChangedEventArgs"/> instance.
    /// </summary>
    public void Add(BoundariesChangedEventArgs other)
    {
      _data |= other._data;
    }

    /// <inheritdoc/>
    public override void Add(Main.SelfAccumulateableEventArgs e)
    {
      var other = e as BoundariesChangedEventArgs;
      if (other is null)
        throw new ArgumentException(string.Format("Argument e should be of type {0}, but is: {1}", typeof(BoundariesChangedEventArgs), e.GetType()));

      _data |= other._data;
    }

    /// <summary>
    /// Adds additional boundary change flags.
    /// </summary>
    public void Add(BoundariesChangedData other)
    {
      _data |= other;
    }
  }

  /// <summary>
  /// Provides event data for a scale instance change.
  /// </summary>
  public class ScaleInstanceChangedEventArgs : Main.SelfAccumulateableEventArgs
  {
    private int _scaleIndex = -1;
    private Altaxo.Graph.Scales.Scale? _oldScale;
    private Altaxo.Graph.Scales.Scale _newScale;

    /// <summary>
    /// Gets or sets the index of the scale that changed.
    /// </summary>
    public int ScaleIndex { get { return _scaleIndex; } set { _scaleIndex = value; } }

    /// <summary>
    /// Gets the previous scale instance.
    /// </summary>
    public Altaxo.Graph.Scales.Scale? OldScale { get { return _oldScale; } }

    /// <summary>
    /// Gets the new scale instance.
    /// </summary>
    public Altaxo.Graph.Scales.Scale NewScale { get { return _newScale; } }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScaleInstanceChangedEventArgs"/> class.
    /// </summary>
    /// <param name="oldScale">The previous scale instance.</param>
    /// <param name="newScale">The new scale instance.</param>
    public ScaleInstanceChangedEventArgs(Altaxo.Graph.Scales.Scale? oldScale, Altaxo.Graph.Scales.Scale newScale)
    {
      _oldScale = oldScale;
      _newScale = newScale;
    }

    /// <inheritdoc/>
    public override void Add(Main.SelfAccumulateableEventArgs e)
    {
      var other = e as ScaleInstanceChangedEventArgs;
      if (other is null)
        throw new ArgumentException("Expect event args of type: " + typeof(ScaleInstanceChangedEventArgs).ToString());

      if (ScaleIndex != other.ScaleIndex)
        throw new InvalidProgramException("This should not happen, because the overrides for GetHashCode and Equals should prevent this.");

      _newScale = other._newScale;
    }

    /// <summary>
    /// Override to ensure that only one instance with a given ScaleIndex is contained in the event args collection.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return 17 * GetType().GetHashCode() + 31 * _scaleIndex;
    }

    /// <summary>
    /// Override to ensure that only one instance with a given ScaleIndex is contained in the event args collection.
    /// </summary>
    /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      return obj is ScaleInstanceChangedEventArgs other && _scaleIndex == other._scaleIndex;
    }
  }
}

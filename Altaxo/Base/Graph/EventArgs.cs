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
  public class PlotItemDataChangedEventArgs : EventArgs
  {
    public static new readonly PlotItemDataChangedEventArgs Empty = new PlotItemDataChangedEventArgs();

    private PlotItemDataChangedEventArgs()
    {
    }
  }

  public class PlotItemStyleChangedEventArgs : EventArgs
  {
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

    /// <summary>The boundaries related to the x-scale (1st independent (!) variable)have changed.</summary>
    XBoundariesChanged = 0x10,

    /// <summary>The boundaries related to the y-scale (2nd independent (!) variable) have changed.</summary>
    YBoundariesChanged = 0x20,

    /// <summary>The boundaries related to the y-scale (3rd independent (!) variable) have changed.</summary>
    ZBoundariesChanged = 0x40,

    /// <summary>The boundaries related to the v-scale (depended variable) have changed.</summary>
    VBoundariesChanged = 0x80,

    /// <summary>A complex change, including all possible kind of changes (for instance, if a new plot item was added to the layer).</summary>
    ComplexChange = 0xFF
  }

  public class BoundariesChangedEventArgs : Main.SelfAccumulateableEventArgs
  {
    protected BoundariesChangedData _data;

    public BoundariesChangedData Data { get { return _data; } }

    public static T FromLowerAndUpperBoundChanged<T>(bool haslowerBoundChanged, bool hasUpperBoundChanged) where T : BoundariesChangedEventArgs, new()
    {
      var result = new T();

      if (haslowerBoundChanged)
        result._data |= BoundariesChangedData.LowerBoundChanged;
      if (hasUpperBoundChanged)
        result._data |= BoundariesChangedData.UpperBoundChanged;

      return result;
    }

    public BoundariesChangedEventArgs()
    {
    }

    public BoundariesChangedEventArgs(BoundariesChangedData data)
    {
      _data = data;
    }

    public BoundariesChangedEventArgs(bool bLowerBound, bool bUpperBound)
    {
      if (bLowerBound)
        _data |= BoundariesChangedData.LowerBoundChanged;
      if (bUpperBound)
        _data |= BoundariesChangedData.UpperBoundChanged;
    }

    public bool LowerBoundChanged
    {
      get { return _data.HasFlag(BoundariesChangedData.LowerBoundChanged); }
    }

    public bool UpperBoundChanged
    {
      get { return _data.HasFlag(BoundariesChangedData.UpperBoundChanged); }
    }

    public void SetXBoundaryChangedFlag()
    {
      _data |= BoundariesChangedData.XBoundariesChanged;
    }

    public void SetYBoundaryChangedFlag()
    {
      _data |= BoundariesChangedData.YBoundariesChanged;
    }

    public void SetZBoundaryChangedFlag()
    {
      _data |= BoundariesChangedData.ZBoundariesChanged;
    }

    public void SetVBoundaryChangedFlag()
    {
      _data |= BoundariesChangedData.VBoundariesChanged;
    }

    public void Add(BoundariesChangedEventArgs other)
    {
      _data |= other._data;
    }

    public override void Add(Main.SelfAccumulateableEventArgs e)
    {
      var other = e as BoundariesChangedEventArgs;
      if (other == null)
        throw new ArgumentException(string.Format("Argument e should be of type {0}, but is: {1}", typeof(BoundariesChangedEventArgs), e.GetType()));

      _data |= other._data;
    }

    public void Add(BoundariesChangedData other)
    {
      _data |= other;
    }
  }

  public class ScaleInstanceChangedEventArgs : Main.SelfAccumulateableEventArgs
  {
    private int _scaleIndex = -1;
    private Altaxo.Graph.Scales.Scale _oldScale;
    private Altaxo.Graph.Scales.Scale _newScale;

    public int ScaleIndex { get { return _scaleIndex; } set { _scaleIndex = value; } }

    public Altaxo.Graph.Scales.Scale OldScale { get { return _oldScale; } }

    public Altaxo.Graph.Scales.Scale NewScale { get { return _newScale; } }

    public ScaleInstanceChangedEventArgs(Altaxo.Graph.Scales.Scale oldScale, Altaxo.Graph.Scales.Scale newScale)
    {
      _oldScale = oldScale;
      _newScale = newScale;
    }

    public override void Add(Main.SelfAccumulateableEventArgs e)
    {
      var other = e as ScaleInstanceChangedEventArgs;
      if (null == other)
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
    public override bool Equals(object? obj)
    {
      return obj is ScaleInstanceChangedEventArgs other && _scaleIndex == other._scaleIndex;
    }
  }
}

#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

namespace Altaxo.Graph.Plot.Groups
{
  /// <summary>
  /// Group style that manages 2D bar sizes and horizontal positions within a cluster.
  /// </summary>
  public class BarSizePosition2DGroupStyle
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    IPlotGroupStyle,
    IShiftLogicalXYGroupStyle
  {
    private bool _isInitialized;
    private bool _isStepEnabled;

    /// <summary>Is set to true if a BarPlotStyle has touched the group style during a prepare step.
    /// Helps to prevent the counting of more than one item per step (in case there is more than one
    /// BarStyle in a PlotItem.</summary>
    private bool _wasTouchedInThisPrepareStep;

    private int _numberOfItems;

    /// <summary>
    /// Relative gap between the bars belonging to the same x-value.
    /// A value of 0.5 means that the gap has half of the width of one bar.
    /// </summary>
    private double _relInnerGapX;

    /// <summary>
    /// Relative gap between the bars between two consecutive x-values.
    /// A value of 1 means that the gap has the same width than one bar.
    /// </summary>
    private double _relOuterGapX;

    /// <summary>
    /// The width of one cluster of bars (including the gaps) in units of logical scale values.
    /// </summary>
    private double _logicalClusterSizeX;

    /// <summary>The x-size of a bar in logical units.</summary>
    private double _logicalItemSizeX;

    /// <summary>The running x offset from the real data point to the center of the bar in logical units.</summary>
    private double _logicalItemOffsetX;

    /// <summary>The number of items in x-direction in a bar cluster.</summary>
    private int _cachedNumberOfItemsX;

    /// <summary>The running number of the item which is processed in the Step() call.</summary>
    private int _cachedCurrentItemIndex;

    #region Serialization

    /// <summary>
    /// 2016-11-05 Renaming from BarWidthPositionGroupStyle to BarSizePosition2DGroupStyle
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Plot.Groups.BarWidthPositionGroupStyle", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BarSizePosition2DGroupStyle), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (BarSizePosition2DGroupStyle)o;
        info.AddValue("StepEnabled", s._isStepEnabled);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (BarSizePosition2DGroupStyle?)o ?? new BarSizePosition2DGroupStyle();
        s._isStepEnabled = info.GetBoolean("StepEnabled");
        return s;
      }
    }

    #endregion Serialization

    private void CopyFrom(BarSizePosition2DGroupStyle from)
    {
      if (ReferenceEquals(this, from))
        return;

      _isStepEnabled = from._isStepEnabled;

      _relInnerGapX = from._relInnerGapX;
      _relOuterGapX = from._relOuterGapX;

      _logicalClusterSizeX = from._logicalClusterSizeX;
      _logicalItemSizeX = from._logicalItemSizeX;
      _logicalItemOffsetX = from._logicalItemOffsetX;
    }

    /// <inheritdoc/>
    public void TransferFrom(IPlotGroupStyle from)
    {
      var fromX = (BarSizePosition2DGroupStyle)from;

      _relInnerGapX = fromX._relInnerGapX;
      _relOuterGapX = fromX._relOuterGapX;
      _logicalClusterSizeX = fromX._logicalClusterSizeX;
      _logicalItemSizeX = fromX._logicalItemSizeX;
      _logicalItemOffsetX = fromX._logicalItemOffsetX;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BarSizePosition2DGroupStyle"/> class.
    /// </summary>
    public BarSizePosition2DGroupStyle()
    {
      _isStepEnabled = true;
    }

    #region ICloneable Members

    /// <summary>
    /// Creates a strongly typed clone of this group style.
    /// </summary>
    /// <returns>A cloned group style.</returns>
    public BarSizePosition2DGroupStyle Clone()
    {
      var result = new BarSizePosition2DGroupStyle();
      result.CopyFrom(this);
      return result;
    }

    /// <inheritdoc />
    object ICloneable.Clone()
    {
      var result = new BarSizePosition2DGroupStyle();
      result.CopyFrom(this);
      return result;
    }

    #endregion ICloneable Members

    #region IPlotGroupStyle Members

    /// <inheritdoc/>
    public void BeginPrepare()
    {
      _isInitialized = false;
      _numberOfItems = 0;
      _wasTouchedInThisPrepareStep = false;
      _logicalClusterSizeX = 0.5; // in case there is only one item, it takes half of the width of the x-scale
    }

    /// <inheritdoc/>
    public void PrepareStep()
    {
      if (_wasTouchedInThisPrepareStep)
      {
        if (_isStepEnabled)
          _numberOfItems++;
        else
          _numberOfItems = 1;
      }

      _wasTouchedInThisPrepareStep = false;
    }

    /// <inheritdoc/>
    public void EndPrepare()
    {
      _wasTouchedInThisPrepareStep = false;

      int totalNumberOfItems = 1;
      if (_isStepEnabled)
        totalNumberOfItems = Math.Max(totalNumberOfItems, _numberOfItems);

      // partition the total number of items in items in x-direction is easy for 2D: it is the number of items
      PartitionItems(totalNumberOfItems, out _cachedNumberOfItemsX);

      _logicalItemSizeX = 1.0 / (_cachedNumberOfItemsX + (_cachedNumberOfItemsX - 1) * _relInnerGapX + _relOuterGapX);
      _logicalItemSizeX *= _logicalClusterSizeX;

      _cachedCurrentItemIndex = 0;
      SetPositionXY_AccordingToCachedCurrentItemIndex(); // sets the position of the first item (according to the _cachedCurrentItemIndex)
    }

    /// <summary>
    /// Partitions the total number of items in rows and columns. This is easy here in 2D: it is simply the total number of items
    /// </summary>
    /// <param name="totalNumberOfItems">The total number of items.</param>
    /// <param name="numberOfItemsX">The number of items in x-direction.</param>
    private void PartitionItems(int totalNumberOfItems, out int numberOfItemsX)
    {
      numberOfItemsX = totalNumberOfItems;
    }

    /// <summary>
    /// Sets the positions <see cref="_logicalItemOffsetX"/>  according to the <see cref="_cachedCurrentItemIndex"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void SetPositionXY_AccordingToCachedCurrentItemIndex()
    {
      int itemIndexX = _cachedCurrentItemIndex;

      // leftmost position is 1/2 cluster size to the left, then 1/2 outer gap to the right, and 1/2 size to the right
      _logicalItemOffsetX = 0.5 * (_logicalItemSizeX * (1 + _relOuterGapX) - _logicalClusterSizeX); // x-position of the first item (leftmost)
      _logicalItemOffsetX += itemIndexX * _logicalItemSizeX * (1 + _relInnerGapX);
    }

    /// <inheritdoc/>
    public bool CanCarryOver
    {
      get
      {
        return false;
      }
    }

    /// <inheritdoc/>
    public bool CanStep
    {
      get
      {
        return true;
      }
    }

    /// <inheritdoc/>
    public int Step(int step)
    {
      _cachedCurrentItemIndex += step;

      SetPositionXY_AccordingToCachedCurrentItemIndex();

      return 0;
    }

    /// <inheritdoc/>
    public bool IsStepEnabled
    {
      get
      {
        return _isStepEnabled;
      }
      set
      {
        _isStepEnabled = value;
      }
    }

    #endregion IPlotGroupStyle Members

    /// <summary>
    /// Gets a value indicating whether this style was initialized.
    /// </summary>
    public bool IsInitialized
    {
      get
      {
        return _isInitialized;
      }
    }

    /// <summary>
    /// Call this function during a prepare step in case the plot item has a BarGraphPlotStyle.
    /// You can safely call it more than once in each prepare step. Only one item is counted per prepare step.
    /// </summary>
    private void IntendToApply(
      int numberOfClusterItems,
      double minimumLogicalXValue,
      double maximumLogicalXValue)
    {
      _wasTouchedInThisPrepareStep = true;

      if (numberOfClusterItems > 1)
      {
        double logicalClusterSizeX = (maximumLogicalXValue - minimumLogicalXValue) / (numberOfClusterItems - 1);
        if (logicalClusterSizeX < _logicalClusterSizeX)
          _logicalClusterSizeX = logicalClusterSizeX;
      }
    }

    /// <summary>
    /// Initializes the bar-spacing parameters.
    /// </summary>
    /// <param name="relInnerGapX">The relative gap between bars inside a cluster.</param>
    /// <param name="relOuterGapX">The relative gap between neighboring clusters.</param>
    public void Initialize(double relInnerGapX, double relOuterGapX)
    {
      _isInitialized = true;
      _relInnerGapX = relInnerGapX;
      _relOuterGapX = relOuterGapX;
    }

    /// <summary>
    /// Applies the current size and position information.
    /// </summary>
    /// <param name="relInnerGapX">The relative inner gap.</param>
    /// <param name="relOuterGapX">The relative outer gap.</param>
    /// <param name="sizeX">The bar width.</param>
    /// <param name="posX">The bar offset.</param>
    public void Apply(out double relInnerGapX, out double relOuterGapX, out double sizeX, out double posX)
    {
      relInnerGapX = _relInnerGapX;
      relOuterGapX = _relOuterGapX;
      sizeX = _logicalItemSizeX;
      posX = _logicalItemOffsetX;
    }

    /// <inheritdoc />
    bool IShiftLogicalXYGroupStyle.IsConstant { get { return true; } }

    /// <inheritdoc />
    void IShiftLogicalXYGroupStyle.Apply(out double logicalShiftX, out double logicalShiftY)
    {
      logicalShiftX = _logicalItemOffsetX;
      logicalShiftY = 0;
    }

    /// <inheritdoc />
    void IShiftLogicalXYGroupStyle.Apply(out Func<int, double> logicalShiftX, out Func<int, double> logicalShiftY)
    {
      throw new NotImplementedException("Use this function only if IsConstant returns false");
    }

    #region Static Helpers

    /// <summary>
    /// Adds this style as an external group style when required.
    /// </summary>
    /// <param name="externalGroups">Collection of external plot group styles.</param>
    public static void AddExternalGroupStyle(IPlotGroupStyleCollection externalGroups)
    {
      if (PlotGroupStyle.ShouldAddExternalGroupStyle(externalGroups, typeof(BarSizePosition2DGroupStyle)))
      {
        var gstyle = new BarSizePosition2DGroupStyle
        {
          IsStepEnabled = true
        };
        externalGroups.Add(gstyle);
      }
    }

    /// <summary>
    /// Adds a local BarWidthPositionGroupStyle in case there is no external one. In this case also BeginPrepare is called on
    /// this newly created group style.
    /// </summary>
    /// <param name="externalGroups">Collection of external plot group styles.</param>
    /// <param name="localGroups">Collection of plot group styles of the plot item.</param>
    public static void AddLocalGroupStyle(
     IPlotGroupStyleCollection externalGroups,
     IPlotGroupStyleCollection localGroups)
    {
      if (PlotGroupStyle.ShouldAddLocalGroupStyle(externalGroups, localGroups, typeof(BarSizePosition2DGroupStyle)))
      {
        var styleToAdd = new BarSizePosition2DGroupStyle();
        localGroups.Add(styleToAdd);
      }
    }

    /// <summary>
    /// Announces that a plot item intends to use bar-size positioning.
    /// </summary>
    /// <param name="externalGroups">Collection of external plot group styles.</param>
    /// <param name="localGroups">Collection of local plot group styles.</param>
    /// <param name="numberOfItems">The number of items in the cluster.</param>
    /// <param name="minimumLogicalXValue">The minimum logical x-value of the cluster.</param>
    /// <param name="maximumLogicalXValue">The maximum logical x-value of the cluster.</param>
    public static void IntendToApply(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      int numberOfItems,
      double minimumLogicalXValue,
      double maximumLogicalXValue
      )
    {
      if (externalGroups is not null && externalGroups.ContainsType(typeof(BarSizePosition2DGroupStyle)))
      {
        ((BarSizePosition2DGroupStyle)externalGroups.GetPlotGroupStyle(typeof(BarSizePosition2DGroupStyle))).IntendToApply(numberOfItems, minimumLogicalXValue, maximumLogicalXValue);
      }
      else if (localGroups is not null && localGroups.ContainsType(typeof(BarSizePosition2DGroupStyle)))
      {
        ((BarSizePosition2DGroupStyle)localGroups.GetPlotGroupStyle(typeof(BarSizePosition2DGroupStyle))).IntendToApply(numberOfItems, minimumLogicalXValue, maximumLogicalXValue);
      }
    }

    #endregion Static Helpers
  }
}

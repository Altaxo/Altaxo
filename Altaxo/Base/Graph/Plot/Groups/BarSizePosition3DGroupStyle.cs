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
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Plot.Groups
{
  public class BarSizePosition3DGroupStyle
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    IPlotGroupStyle,
    IShiftLogicalXYZGroupStyle
  {
    private bool _isInitialized;
    private bool _isStepEnabled;

    /// <summary>Is set to true if a BarPlotStyle has touched the group style during a prepare step.
    /// Helps to prevent the counting of more than one item per step (in case there is more than one
    /// BarStyle in a PlotItem.</summary>
    private bool _wasTouchedInThisPrepareStep;

    private int _numberOfItems;

    private BarShiftStrategy3D _barShiftStrategy;

    /// <summary>The number of items in one direction. This field is used in Step() if bar shift strategy is one of the manual values, to switch in y-Direction after every this number of items Steps in x-Direction.</summary>
    private int _barShiftMaxNumberOfItemsInOneDirection;

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
    /// Relative gap between the bars belonging to the same y-value.
    /// A value of 0.5 means that the gap has half of the width of one bar.
    /// </summary>
    private double _relInnerGapY;

    /// <summary>
    /// Relative gap between the bars between two consecutive y-values.
    /// A value of 1 means that the gap has the same width than one bar.
    /// </summary>
    private double _relOuterGapY;

    /// <summary>
    /// The width of one cluster of bars (including the gaps) in units of logical scale values.
    /// </summary>
    private double _logicalClusterSizeX;

    /// <summary>
    /// The depth of one cluster of bars (including the gaps) in units of logical scale values.
    /// </summary>
    private double _logicalClusterSizeY;

    /// <summary>The x-size of a bar in logical units.</summary>
    private double _logicalItemSizeX;

    /// <summary>The y-size of a bar in logical units.</summary>
    private double _logicalItemSizeY;

    /// <summary>The running x offset from the real data point to the center of the bar in logical units.</summary>
    private double _logicalItemOffsetX;

    /// <summary>The running y offset from the real data point to the center of the bar in logical units.</summary>
    private double _logicalItemOffsetY;

    /// <summary>The number of items in x-direction in a bar cluster.</summary>
    private int _cachedNumberOfItemsX;

    /// <summary>The number of items in y-direction in a bar cluster.</summary>
    private int _cachedNumberOfItemsY;

    /// <summary>The running number of the item which is processed in the Step() call.</summary>
    private int _cachedCurrentItemIndex;

    #region Serialization

    /// <summary>
    /// Initial version 2016-09-06.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BarSizePosition3DGroupStyle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (BarSizePosition3DGroupStyle)obj;
        info.AddValue("StepEnabled", s._isStepEnabled);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (BarSizePosition3DGroupStyle?)o ?? new BarSizePosition3DGroupStyle();
        s._isStepEnabled = info.GetBoolean("StepEnabled");
        return s;
      }
    }

    #endregion Serialization

    private void CopyFrom(BarSizePosition3DGroupStyle from)
    {
      if (object.ReferenceEquals(this, from))
        return;

      _isStepEnabled = from._isStepEnabled;

      _barShiftStrategy = from._barShiftStrategy;
      _barShiftMaxNumberOfItemsInOneDirection = from._barShiftMaxNumberOfItemsInOneDirection;
      _relInnerGapX = from._relInnerGapX;
      _relOuterGapX = from._relOuterGapX;
      _relInnerGapY = from._relInnerGapY;
      _relOuterGapY = from._relOuterGapY;

      _logicalClusterSizeX = from._logicalClusterSizeX;
      _logicalClusterSizeY = from._logicalClusterSizeY;
      _logicalItemSizeX = from._logicalItemSizeX;
      _logicalItemOffsetX = from._logicalItemOffsetX;
      _logicalItemSizeY = from._logicalItemSizeY;
      _logicalItemOffsetY = from._logicalItemOffsetY;
    }

    public void TransferFrom(IPlotGroupStyle fromb)
    {
      var from = (BarSizePosition3DGroupStyle)fromb;

      _barShiftStrategy = from._barShiftStrategy;
      _barShiftMaxNumberOfItemsInOneDirection = from._barShiftMaxNumberOfItemsInOneDirection;
      _relInnerGapX = from._relInnerGapX;
      _relOuterGapX = from._relOuterGapX;
      _relInnerGapY = from._relInnerGapY;
      _relOuterGapY = from._relOuterGapY;

      _logicalClusterSizeX = from._logicalClusterSizeX;
      _logicalClusterSizeY = from._logicalClusterSizeY;
      _logicalItemSizeX = from._logicalItemSizeX;
      _logicalItemOffsetX = from._logicalItemOffsetX;
      _logicalItemSizeY = from._logicalItemSizeY;
      _logicalItemOffsetY = from._logicalItemOffsetY;
    }

    public BarSizePosition3DGroupStyle()
    {
      _isStepEnabled = true;
    }

    #region ICloneable Members

    public BarSizePosition3DGroupStyle Clone()
    {
      var result = new BarSizePosition3DGroupStyle();
      result.CopyFrom(this);
      return result;
    }

    object ICloneable.Clone()
    {
      var result = new BarSizePosition3DGroupStyle();
      result.CopyFrom(this);
      return result;
    }

    #endregion ICloneable Members

    #region IPlotGroupStyle Members

    public void BeginPrepare()
    {
      _isInitialized = false;
      _numberOfItems = 0;
      _wasTouchedInThisPrepareStep = false;
      _logicalClusterSizeX = 0.5; // in case there is only one item, it takes half of the width of the x-scale
      _logicalClusterSizeY = 0.5; // in case there is only one item, it takes half of the width of the x-scale
    }

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

    public void EndPrepare()
    {
      _wasTouchedInThisPrepareStep = false;

      int totalNumberOfItems = 1;
      if (_isStepEnabled)
        totalNumberOfItems = Math.Max(totalNumberOfItems, _numberOfItems);

      // partition the total number of items in items in x-direction and in items in y-direction

      PartitionItems(totalNumberOfItems, out _cachedNumberOfItemsX, out _cachedNumberOfItemsY);

      _logicalItemSizeX = 1.0 / (_cachedNumberOfItemsX + (_cachedNumberOfItemsX - 1) * _relInnerGapX + _relOuterGapX);
      _logicalItemSizeX *= _logicalClusterSizeX;

      _logicalItemSizeY = 1.0 / (_cachedNumberOfItemsY + (_cachedNumberOfItemsY - 1) * _relInnerGapY + _relOuterGapY);
      _logicalItemSizeY *= _logicalClusterSizeY;

      _cachedCurrentItemIndex = 0;
      SetPositionXY_AccordingToCachedCurrentItemIndex(); // sets the position of the first item (according to the _cachedCurrentItemIndex)
    }

    /// <summary>
    /// Partitions the total number of items in rows and columns. The strategy how to partition is stored in _barShiftStrategy.
    /// </summary>
    /// <param name="totalNumberOfItems">The total number of items.</param>
    /// <param name="numberOfItemsX">The number of items in x-direction.</param>
    /// <param name="numberOfItemsY">The number of items in y-direction.</param>
    private void PartitionItems(int totalNumberOfItems, out int numberOfItemsX, out int numberOfItemsY)
    {
      if (0 == totalNumberOfItems)
      {
        numberOfItemsX = 0;
        numberOfItemsY = 0;
      }
      else
      {
        switch (_barShiftStrategy)
        {
          case BarShiftStrategy3D.ManualFirstXThenY:
            numberOfItemsX = Math.Min(totalNumberOfItems, _barShiftMaxNumberOfItemsInOneDirection);
            numberOfItemsY = (int)Math.Ceiling(totalNumberOfItems / (double)numberOfItemsX);
            break;

          case BarShiftStrategy3D.ManualFirstYThenX:
            numberOfItemsY = Math.Min(totalNumberOfItems, _barShiftMaxNumberOfItemsInOneDirection);
            numberOfItemsX = (int)Math.Ceiling(totalNumberOfItems / (double)numberOfItemsY);
            break;

          case BarShiftStrategy3D.UniformFirstXThenY:
            numberOfItemsX = (int)Math.Ceiling(Math.Sqrt(totalNumberOfItems));
            numberOfItemsY = (int)Math.Ceiling(totalNumberOfItems / (double)numberOfItemsX);
            break;

          case BarShiftStrategy3D.UniformFirstYThenX:
            numberOfItemsY = (int)Math.Ceiling(Math.Sqrt(totalNumberOfItems));
            numberOfItemsX = (int)Math.Ceiling(totalNumberOfItems / (double)numberOfItemsY);
            break;

          default:
            throw new ArgumentOutOfRangeException(nameof(_barShiftStrategy));
        }
      }
    }

    /// <summary>
    /// Sets the positions <see cref="_logicalItemOffsetX"/> and <see cref="_logicalItemOffsetY"/> according to the <see cref="_cachedCurrentItemIndex"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void SetPositionXY_AccordingToCachedCurrentItemIndex()
    {
      int itemIndexX, itemIndexY;

      switch (_barShiftStrategy)
      {
        case BarShiftStrategy3D.ManualFirstXThenY:
        case BarShiftStrategy3D.UniformFirstXThenY:
          itemIndexX = _cachedCurrentItemIndex % _cachedNumberOfItemsX;
          itemIndexY = _cachedCurrentItemIndex / _cachedNumberOfItemsX;
          break;

        case BarShiftStrategy3D.ManualFirstYThenX:
        case BarShiftStrategy3D.UniformFirstYThenX:
          itemIndexY = _cachedCurrentItemIndex % _cachedNumberOfItemsY;
          itemIndexX = _cachedCurrentItemIndex / _cachedNumberOfItemsY;
          break;

        default:
          throw new ArgumentOutOfRangeException(nameof(_barShiftStrategy));
      }

      // leftmost position is 1/2 cluster size to the left, then 1/2 outer gap to the right, and 1/2 size to the right
      _logicalItemOffsetX = 0.5 * (_logicalItemSizeX * (1 + _relOuterGapX) - _logicalClusterSizeX); // x-position of the first item (leftmost)
      _logicalItemOffsetX += itemIndexX * _logicalItemSizeX * (1 + _relInnerGapX);

      // frontmost position is 1/2 cluster size to the front, then 1/2 outer gap to the back, and 1/2 size to the back
      _logicalItemOffsetY = 0.5 * (_logicalItemSizeY * (1 + _relOuterGapY) - _logicalClusterSizeY); // y-position of the first item (frontmost)
      _logicalItemOffsetY += itemIndexY * _logicalItemSizeY * (1 + _relInnerGapY);
    }

    public bool CanCarryOver
    {
      get
      {
        return false;
      }
    }

    public bool CanStep
    {
      get
      {
        return true;
      }
    }

    public int Step(int step)
    {
      _cachedCurrentItemIndex += step;

      SetPositionXY_AccordingToCachedCurrentItemIndex();

      return 0;
    }

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
      double maximumLogicalXValue,
      double minimumLogicalYValue,
      double maximumLogicalYValue)
    {
      _wasTouchedInThisPrepareStep = true;

      if (numberOfClusterItems > 1)
      {
        double logicalClusterSizeX = (maximumLogicalXValue - minimumLogicalXValue) / (numberOfClusterItems - 1);
        if (logicalClusterSizeX < _logicalClusterSizeX)
          _logicalClusterSizeX = logicalClusterSizeX;

        double logicalClusterSizeY = (maximumLogicalYValue - minimumLogicalYValue) / (numberOfClusterItems - 1);
        if (logicalClusterSizeY < _logicalClusterSizeY)
          _logicalClusterSizeY = logicalClusterSizeY;
      }
    }

    /// <summary>
    /// Is initialized is called the first time a BarGraphPlotStyle.PrepareStyle was called.
    /// The BarGraphPlotStyle has stored two properties relGap and relBound, which are transferred
    /// to the group style in this process.
    /// </summary>
    /// <param name="barShiftStrategy">Strategy how to shift the bars belonging to one group.</param>
    /// <param name="barShiftMaxNumberOfItemsInOneDirection">If barShiftStrategy is set to a manual value, this parameter designates the maximum number of items in one direction. Ignored if bar shift strategy is set to automatic.</param>
    /// <param name="relInnerGapX">Gap x between to bars in a group in units of one bar width.</param>
    /// <param name="relOuterGapX">Gap x between the items of two groups in units of one bar width.</param>
    /// <param name="relInnerGapY">Gap y between to bars in a group in units of one bar width.</param>
    /// <param name="relOuterGapY">Gap y between the items of two groups in units of one bar width.</param>
    public void Initialize(BarShiftStrategy3D barShiftStrategy, int barShiftMaxNumberOfItemsInOneDirection, double relInnerGapX, double relOuterGapX, double relInnerGapY, double relOuterGapY)
    {
      if (barShiftMaxNumberOfItemsInOneDirection <= 0)
        throw new ArgumentOutOfRangeException(nameof(barShiftMaxNumberOfItemsInOneDirection), "value should be >= 1");

      _isInitialized = true;
      _barShiftStrategy = barShiftStrategy;
      _barShiftMaxNumberOfItemsInOneDirection = barShiftMaxNumberOfItemsInOneDirection;
      _relInnerGapX = relInnerGapX;
      _relOuterGapX = relOuterGapX;
      _relInnerGapY = relInnerGapY;
      _relOuterGapY = relOuterGapY;
    }

    public void Apply(
      out BarShiftStrategy3D barShiftStrategy, out int barShiftMaxNumberOfItemsInOneDirection,
      out double relInnerGapX, out double relOuterGapX, out double sizeX, out double posX,
      out double relInnerGapY, out double relOuterGapY, out double sizeY, out double posY)
    {
      barShiftStrategy = _barShiftStrategy;
      barShiftMaxNumberOfItemsInOneDirection = _barShiftMaxNumberOfItemsInOneDirection;
      relInnerGapX = _relInnerGapX;
      relOuterGapX = _relOuterGapX;
      sizeX = _logicalItemSizeX;
      posX = _logicalItemOffsetX;

      relInnerGapY = _relInnerGapY;
      relOuterGapY = _relOuterGapY;
      sizeY = _logicalItemSizeY;
      posY = _logicalItemOffsetY;
    }

    bool IShiftLogicalXYZGroupStyle.IsConstant { get { return true; } }

    void IShiftLogicalXYZGroupStyle.Apply(out double logicalShiftX, out double logicalShiftY, out double logicalShiftZ)
    {
      logicalShiftX = _logicalItemOffsetX;
      logicalShiftY = _logicalItemOffsetY;
      logicalShiftZ = 0;
    }

    void IShiftLogicalXYZGroupStyle.Apply(out Func<int, double> logicalShiftX, out Func<int, double> logicalShiftY, out Func<int, double> logicalShiftZ)
    {
      throw new NotImplementedException("Use this function only if IsConstant returns false");
    }

    #region Static Helpers

    public static void AddExternalGroupStyle(IPlotGroupStyleCollection externalGroups)
    {
      if (PlotGroupStyle.ShouldAddExternalGroupStyle(externalGroups, typeof(BarSizePosition3DGroupStyle)))
      {
        var gstyle = new BarSizePosition3DGroupStyle
        {
          IsStepEnabled = true
        };
        externalGroups.Add(gstyle);
      }
    }

    /// <summary>
    /// Adds a local BarSizePosition3DGroupStyle in case there is no external one. In this case also BeginPrepare is called on
    /// this newly created group style.
    /// </summary>
    /// <param name="externalGroups">Collection of external plot group styles.</param>
    /// <param name="localGroups">Collection of plot group styles of the plot item.</param>
    public static void AddLocalGroupStyle(
     IPlotGroupStyleCollection externalGroups,
     IPlotGroupStyleCollection localGroups)
    {
      if (PlotGroupStyle.ShouldAddLocalGroupStyle(externalGroups, localGroups, typeof(BarSizePosition3DGroupStyle)))
      {
        var styleToAdd = new BarSizePosition3DGroupStyle();
        localGroups.Add(styleToAdd);
      }
    }

    public static void IntendToApply(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      int numberOfItems,
      double minimumLogicalXValue,
      double maximumLogicalXValue,
      double minimumLogicalYValue,
      double maximumLogicalYValue
      )
    {
      if (externalGroups != null && externalGroups.ContainsType(typeof(BarSizePosition3DGroupStyle)))
      {
        ((BarSizePosition3DGroupStyle)externalGroups.GetPlotGroupStyle(typeof(BarSizePosition3DGroupStyle))).IntendToApply(numberOfItems, minimumLogicalXValue, maximumLogicalXValue, minimumLogicalYValue, maximumLogicalYValue);
      }
      else if (localGroups != null && localGroups.ContainsType(typeof(BarSizePosition3DGroupStyle)))
      {
        ((BarSizePosition3DGroupStyle)localGroups.GetPlotGroupStyle(typeof(BarSizePosition3DGroupStyle))).IntendToApply(numberOfItems, minimumLogicalXValue, maximumLogicalXValue, minimumLogicalYValue, maximumLogicalYValue);
      }
    }

    #endregion Static Helpers
  }
}

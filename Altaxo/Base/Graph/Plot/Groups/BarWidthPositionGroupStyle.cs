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
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Plot.Groups
{
  using Gdi.Plot.Groups;

  public class BarWidthPositionGroupStyle : IPlotGroupStyle
  {
    bool _isInitialized;
    bool _isStepEnabled;

    /// <summary>Is set to true if a BarPlotStyle has touched the group style during a prepare step.
    /// Helps to prevent the counting of more than one item per step (in case there is more than one
    /// BarStyle in a PlotItem.</summary>
    bool _wasTouchedInThisPrepareStep;
    
    int _numberOfItems;

    /// <summary>
    /// Relative gap between the bars belonging to the same x-value.
    /// A value of 0.5 means that the gap has half of the width of one bar.
    /// </summary>
    double _relInnerGapWidth;
    
    /// <summary>
    /// Relative gap between the bars between two consecutive x-values.
    /// A value of 1 means that the gap has the same width than one bar.
    /// </summary>
    double _relOuterGapWidth;

    /// <summary>
    /// The width of one cluster of bars (including the gaps) in units of logical scale values.
    /// </summary>
    double _logicalClusterWidth;

    double _width;
    double _positionX;


    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BarWidthPositionGroupStyle), 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        BarWidthPositionGroupStyle s = (BarWidthPositionGroupStyle)obj;
        info.AddValue("StepEnabled", s._isStepEnabled);
      }


      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        BarWidthPositionGroupStyle s = null != o ? (BarWidthPositionGroupStyle)o : new BarWidthPositionGroupStyle();
        s._isStepEnabled = info.GetBoolean("StepEnabled");
        return s;
      }
    }

    #endregion

    void CopyFrom(BarWidthPositionGroupStyle from)
    {
      _isInitialized = from._isInitialized;
      _isStepEnabled = from._isStepEnabled;
      _wasTouchedInThisPrepareStep = from._wasTouchedInThisPrepareStep;
      _numberOfItems = from._numberOfItems;
      _relInnerGapWidth = from._relInnerGapWidth;
      _relOuterGapWidth = from._relOuterGapWidth;
      _logicalClusterWidth = from._logicalClusterWidth;
      _width = from._width;
      _positionX = from._positionX;
    }

    public void TransferFrom(IPlotGroupStyle fromb)
    {
      BarWidthPositionGroupStyle from = (BarWidthPositionGroupStyle)fromb;
      _isInitialized = from._isInitialized;
      _numberOfItems = from._numberOfItems;
      _relInnerGapWidth = from._relInnerGapWidth;
      _relOuterGapWidth = from._relOuterGapWidth;
      _logicalClusterWidth = from._logicalClusterWidth;
      _width = from._width;
      _positionX = from._positionX;
    }

    public BarWidthPositionGroupStyle()
    {
      _isStepEnabled = true;
    }


    #region ICloneable Members

    public BarWidthPositionGroupStyle Clone()
    {
      BarWidthPositionGroupStyle result = new BarWidthPositionGroupStyle();
      result.CopyFrom(this);
      return result;
    }

    object ICloneable.Clone()
    {
      BarWidthPositionGroupStyle result = new BarWidthPositionGroupStyle();
      result.CopyFrom(this);
      return result;
    }


    #endregion


    #region IPlotGroupStyle Members

    public void BeginPrepare()
    {
      _isInitialized = false;
      _numberOfItems = 0;
      _wasTouchedInThisPrepareStep = false;
      _logicalClusterWidth = 0.5; // in case there is only one item, it takes half of the width of the x-scale
    }

    public void EndPrepare()
    {
      _wasTouchedInThisPrepareStep = false;

      int tnumberOfItems = 1;
      if(this._isStepEnabled)
        tnumberOfItems = Math.Max(tnumberOfItems, _numberOfItems);

      _width = 1.0 / (tnumberOfItems + (tnumberOfItems - 1) * _relInnerGapWidth + _relOuterGapWidth);
      _width *= _logicalClusterWidth;

      _positionX = 0.5 * (_width * _relOuterGapWidth - _logicalClusterWidth);

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

    public bool CanHaveChilds()
    {
      return false;
    }

    public int Step(int step)
    {
      _positionX += step * _width * (1 + _relInnerGapWidth);
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

    #endregion

  
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
    void IntendToApply(
      int numberOfClusterItems,
      double minimumLogicalXValue,
      double maximumLogicalXValue)
    {
      _wasTouchedInThisPrepareStep = true;

      if (numberOfClusterItems > 1)
      {
        double logicalclusterwidth = (maximumLogicalXValue - minimumLogicalXValue) / (numberOfClusterItems - 1);
        if (logicalclusterwidth < _logicalClusterWidth)
          _logicalClusterWidth = logicalclusterwidth;
      }
    }

    /// <summary>
    /// Is initialized is called the first time a BarGraphPlotStyle.PrepareStyle was called.
    /// The BarGraphPlotStyle has stored two properties relGap and relBound, which are transferred
    /// to the group style in this process.
    /// </summary>
    /// <param name="relInnerGapWidth">Gap between to bars in a group in units of one bar width.</param>
    /// <param name="relOuterGapWidth">Gap between the items of two groups in units of one bar width.</param>
    public void Initialize(double relInnerGapWidth, double relOuterGapWidth)
    {
      _isInitialized = true;
      _relInnerGapWidth = relInnerGapWidth;
      _relOuterGapWidth = relOuterGapWidth;
    }

    public void Apply(out double relInnerGapWidth, out double relOuterGapWidth, out double width, out double pos)
    {
      relInnerGapWidth = _relInnerGapWidth;
      relOuterGapWidth = _relOuterGapWidth;
      width = _width;
      pos = _positionX;
    }


  

    #region Static Helpers

    public static void AddExternalGroupStyle(IPlotGroupStyleCollection externalGroups)
    {
      if (PlotGroupStyle.ShouldAddExternalGroupStyle(externalGroups, typeof(BarWidthPositionGroupStyle)))
      {
        BarWidthPositionGroupStyle gstyle = new BarWidthPositionGroupStyle();
        gstyle.IsStepEnabled = true;
        externalGroups.Add(gstyle);
      }
    }

    public static void AddLocalGroupStyle(
     IPlotGroupStyleCollection externalGroups,
     IPlotGroupStyleCollection localGroups)
    {
      if (PlotGroupStyle.ShouldAddLocalGroupStyle(externalGroups, localGroups, typeof(BarWidthPositionGroupStyle)))
        localGroups.Add(new BarWidthPositionGroupStyle());
    }

    public static void IntendToApply(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      int numberOfItems,
      double minimumLogicalXValue,
      double maximumLogicalXValue
      )
    {
      if (externalGroups != null && externalGroups.ContainsType(typeof(BarWidthPositionGroupStyle)))
      {
        ((BarWidthPositionGroupStyle)externalGroups.GetPlotGroupStyle(typeof(BarWidthPositionGroupStyle))).IntendToApply(numberOfItems,minimumLogicalXValue,maximumLogicalXValue);
      }
    }


   

   

    #endregion
  }
}

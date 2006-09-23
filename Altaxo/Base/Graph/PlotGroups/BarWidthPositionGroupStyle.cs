using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.PlotGroups
{
  using G2D.Plot.Groups;

  public class BarWidthPositionGroupStyle : IPlotGroupStyle
  {
    bool _isInitialized;
    bool _isStepEnabled;


    bool _intendToApply;
    int _numberOfItems;


    double _relGapWidth;
    double _relBoundWidth;
    double _width;
    double _positionX;

    #region IPlotGroupStyle Members

    public void BeginPrepare()
    {
      _isInitialized = false;
      _numberOfItems = 0;
      _intendToApply = false;
    }

    public void EndPrepare()
    {
      _intendToApply = false;
    }

    public void PrepareStep()
    {
      if (_intendToApply)
        _numberOfItems++;

      _intendToApply = false;
    }

    public bool CanHaveChilds()
    {
      return false;
    }

    public int Step(int step)
    {
      _positionX += step * _width * (1 + _relGapWidth);
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

    #region ICloneable Members

    public object Clone()
    {
      throw new Exception("The method or operation is not implemented.");
    }

    #endregion

    public bool IsInitialized
    {
      get
      {
        return _isInitialized;
      }
    }

    void IntendToApply()
    {
      _intendToApply = true;
    }

    public void Initialize(double relGap, double relBound)
    {
      _isInitialized = true;
      _relGapWidth = relGap;
      _relBoundWidth = relBound;
      _width = 1.0 / (_numberOfItems + (_numberOfItems - 1) * _relGapWidth + _relBoundWidth);
      _positionX = _relBoundWidth*_width / 2;
    }

    public void Apply(out double relGap, out double relBound, out double width, out double pos)
    {
      relGap = _relGapWidth;
      relBound = _relBoundWidth;
      width = _width;
      pos = _positionX;
    }


    #region ICloneable Members

    object ICloneable.Clone()
    {
      throw new Exception("The method or operation is not implemented.");
    }




    #endregion

    #region Static Helpers
  

    public static void AddLocalGroupStyle(
     PlotGroupStyleCollection externalGroups,
     PlotGroupStyleCollection localGroups)
    {
      if (PlotGroupStyle.ShouldAddLocalGroupStyle(externalGroups, localGroups, typeof(BarWidthPositionGroupStyle)))
        localGroups.Add(new BarWidthPositionGroupStyle());
    }

    public static void IntendToApply(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      if (externalGroups != null && externalGroups.ContainsType(typeof(BarWidthPositionGroupStyle)))
      {
        ((BarWidthPositionGroupStyle)externalGroups.GetPlotGroupStyle(typeof(BarWidthPositionGroupStyle))).IntendToApply();
      }
    }


   

   

    #endregion
  }
}

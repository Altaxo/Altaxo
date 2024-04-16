#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using Altaxo.Science.Spectroscopy.BaselineEstimation;

namespace Altaxo.Gui.Science.Spectroscopy.BaselineEstimation
{
  public interface ISSProbView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(SSProbBase))]
  [ExpectedTypeOfView(typeof(ISSProbView))]
  public class SSProbController : MVCANControllerEditImmutableDocBase<SSProbBase, ISSProbView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private double _numberOfFeatures;

    public double NumberOfFeatures
    {
      get => _numberOfFeatures;
      set
      {
        if (!(_numberOfFeatures == value))
        {
          _numberOfFeatures = value;
          OnPropertyChanged(nameof(NumberOfFeatures));
        }
      }
    }

    private double _averageSpan;

    public double AveragingSpan
    {
      get => _averageSpan;
      set
      {
        if (!(_averageSpan == value))
        {
          _averageSpan = value;
          OnPropertyChanged(nameof(AveragingSpan));
        }
      }
    }

    private bool _isAverageSpanInXUnits;

    public bool IsAveragingSpanInXUnits
    {
      get => _isAverageSpanInXUnits;
      set
      {
        if (!(_isAverageSpanInXUnits == value))
        {
          _isAverageSpanInXUnits = value;
          OnPropertyChanged(nameof(IsAveragingSpanInXUnits));
        }
      }
    }

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        NumberOfFeatures = _doc.NumberOfFeatures;
        AveragingSpan = _doc.AveragingSpan;
        IsAveragingSpanInXUnits = _doc.IsAveragingSpanInXUnits;
      }
    }

    public override bool Apply(bool disposeController)
    {
      try
      {
        _doc = _doc with
        {
          NumberOfFeatures = NumberOfFeatures,
          AveragingSpan = AveragingSpan,
          IsAveragingSpanInXUnits = IsAveragingSpanInXUnits,
        };
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox(ex.Message);
        return ApplyEnd(false, disposeController);
      }

      return ApplyEnd(true, disposeController);
    }


  }
}

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

    private double _smoothnessValue;

    public double SmoothnessValue
    {
      get => _smoothnessValue;
      set
      {
        if (!(_smoothnessValue == value))
        {
          _smoothnessValue = value;
          OnPropertyChanged(nameof(SmoothnessValue));
        }
      }
    }

    private SmoothnessSpecification _smoothnessSpecificiedBy;

    public SmoothnessSpecification SmoothnessSpecificiedBy
    {
      get => _smoothnessSpecificiedBy;
      set
      {
        if (!(_smoothnessSpecificiedBy == value))
        {
          _smoothnessSpecificiedBy = value;
          OnPropertyChanged(nameof(SmoothnessSpecificiedBy));
          OnPropertyChanged(nameof(IsSpecifiedNumberOfFeatures));
          OnPropertyChanged(nameof(IsSpecifiedNumberOfPoints));
          OnPropertyChanged(nameof(IsSpecifiedXSpan));
        }
      }
    }

    public bool IsSpecifiedNumberOfFeatures
    {
      get => SmoothnessSpecificiedBy == SmoothnessSpecification.ByNumberOfFeatures;
      set
      {
        if (value)
          SmoothnessSpecificiedBy = SmoothnessSpecification.ByNumberOfFeatures;
      }
    }

    public bool IsSpecifiedNumberOfPoints
    {
      get => SmoothnessSpecificiedBy == SmoothnessSpecification.ByNumberOfPoints;
      set
      {
        if (value)
          SmoothnessSpecificiedBy = SmoothnessSpecification.ByNumberOfPoints;
      }
    }
    public bool IsSpecifiedXSpan
    {
      get => SmoothnessSpecificiedBy == SmoothnessSpecification.ByXSpan;
      set
      {
        if (value)
          SmoothnessSpecificiedBy = SmoothnessSpecification.ByXSpan;
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        SmoothnessSpecificiedBy = _doc.SmoothnessSpecifiedBy;
        SmoothnessValue = _doc.SmoothnessValue;

      }
    }

    public override bool Apply(bool disposeController)
    {
      try
      {
        _doc = _doc with
        {
          SmoothnessSpecifiedBy = SmoothnessSpecificiedBy,
          SmoothnessValue = SmoothnessValue,
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

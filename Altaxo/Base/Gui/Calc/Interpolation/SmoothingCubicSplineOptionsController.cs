#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2019 Dr. Dirk Lellinger
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

#nullable disable
using System;
using System.Collections.Generic;
using Altaxo.Calc.Interpolation;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Calc.Interpolation
{
  public interface ISmoothingCubicSplineOptionsView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controls the Smoothing parameter of a rational cubic spline.
  /// </summary>
  [UserControllerForObject(typeof(Altaxo.Calc.Interpolation.SmoothingCubicSplineOptions), 100)]
  [ExpectedTypeOfView(typeof(ISmoothingCubicSplineOptionsView))]
  public class SmoothingCubicSplineOptionsController : MVCANControllerEditImmutableDocBase<SmoothingCubicSplineOptions, ISmoothingCubicSplineOptionsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings


    public double SmoothnessValue
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(SmoothnessValue));
        }
      }
    }


    public ItemsController<SmoothnessSpecification> SmoothnessSpecifiedBy
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field?.Dispose();
          field = value;
          OnPropertyChanged(nameof(SmoothnessSpecifiedBy));
        }
      }
    }

    public string SmoothnessValueToolTip
    {
      get
      {
        return SmoothnessSpecifiedBy.SelectedValue switch
        {
          SmoothnessSpecification.Direct => "A value of 0 corresponds to a cubic spline, a value of infinity to a linear interpolation. Note that the smoothing effect not only depends on the smoothing value, but also on the number of points of the signal.",
          SmoothnessSpecification.ByNumberOfFeatures => "Specify the number of features. For a sine signal with #NumberOfFeature periods, the sine signal is attenuated by a factor of 1/e",
          SmoothnessSpecification.ByNumberOfPoints => "Specify the number of points in one feature. If the feature is a sine period, the sine signal is attenuated by a factor of 1/e",
          SmoothnessSpecification.ByXSpan => "Specify the x-width of one feature. If the feature is a sine period, the sine signal is attenuated by a factor of 1/e",
          _ => throw new NotSupportedException("Unknown SmoothnessSpecification"),
        };
      }
    }

    #endregion Bindings

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);
      if (initData)
      {
        SmoothnessValue = _doc.Smoothness;
        SmoothnessSpecifiedBy = new ItemsController<SmoothnessSpecification>(new Collections.SelectableListNodeList(_doc.SmoothnessSpecifiedBy), EhSmoothnessSpecificationChanged);
      }
    }

    private void EhSmoothnessSpecificationChanged(SmoothnessSpecification specification)
    {
      OnPropertyChanged(nameof(SmoothnessValueToolTip));
    }

    public override bool Apply(bool disposeController)
    {
      try
      {
        _doc = _doc with
        {
          SmoothnessSpecifiedBy = this.SmoothnessSpecifiedBy.SelectedValue,
          Smoothness = SmoothnessValue,
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

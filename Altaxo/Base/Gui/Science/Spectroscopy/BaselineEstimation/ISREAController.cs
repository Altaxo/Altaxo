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
using Altaxo.Calc.Interpolation;
using Altaxo.Science.Spectroscopy.BaselineEstimation;

namespace Altaxo.Gui.Science.Spectroscopy.BaselineEstimation
{
  /// <summary>
  /// View interface for ISREA baseline estimation settings.
  /// </summary>
  public interface IISREAView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for <see cref="ISREABase"/>.
  /// </summary>
  [UserControllerForObject(typeof(ISREABase))]
  [ExpectedTypeOfView(typeof(IISREAView))]
  public class ISREAController : MVCANControllerEditImmutableDocBase<ISREABase, IISREAView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    /// <summary>
    /// Gets or sets the smoothness value.
    /// </summary>
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

    /// <summary>
    /// Gets or sets how the smoothness is specified.
    /// </summary>
    public SmoothnessSpecification SmoothnessSpecificiedBy
    {
      get => field;
      set
      {
        if (value == SmoothnessSpecification.Direct)
          throw new ArgumentException("Direct specification of smoothness is not supported in ISREA.");

        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(SmoothnessSpecificiedBy));
          OnPropertyChanged(nameof(IsSpecifiedNumberOfFeatures));
          OnPropertyChanged(nameof(IsSpecifiedNumberOfPoints));
          OnPropertyChanged(nameof(IsSpecifiedXSpan));
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the smoothness is specified by the number of features.
    /// </summary>
    public bool IsSpecifiedNumberOfFeatures
    {
      get => SmoothnessSpecificiedBy == SmoothnessSpecification.ByNumberOfFeatures;
      set
      {
        if (value)
          SmoothnessSpecificiedBy = SmoothnessSpecification.ByNumberOfFeatures;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the smoothness is specified by the number of points.
    /// </summary>
    public bool IsSpecifiedNumberOfPoints
    {
      get => SmoothnessSpecificiedBy == SmoothnessSpecification.ByNumberOfPoints;
      set
      {
        if (value)
          SmoothnessSpecificiedBy = SmoothnessSpecification.ByNumberOfPoints;
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether the smoothness is specified by the X span.
    /// </summary>
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

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        SmoothnessSpecificiedBy = _doc.SmoothnessSpecifiedBy;
        SmoothnessValue = _doc.SmoothnessValue;
      }
    }

    /// <inheritdoc/>
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

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

namespace Altaxo.Gui.Calc.Interpolation
{
  /// <summary>
  /// Defines the view contract for editing <see cref="PronySeriesInterpolationBase"/> options.
  /// </summary>
  public interface IPronySeriesInterpolationBaseView : IDataContextAwareView { }

  /// <summary>
  /// Controller for <see cref="PronySeriesInterpolationBase"/>.
  /// </summary>
  [ExpectedTypeOfView(typeof(IPronySeriesInterpolationBaseView))]
  [UserControllerForObject(typeof(PronySeriesInterpolationBase))]
  public class PronySeriesInterpolationBaseController : MVCANControllerEditImmutableDocBase<PronySeriesInterpolationBase, IPronySeriesInterpolationBaseView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private bool _isManuallySpecifiedXMinXMax;

    /// <summary>
    /// Gets or sets a value indicating whether the x minimum and maximum are specified manually.
    /// </summary>
    public bool IsManuallySpecifiedXMinXMax
    {
      get => _isManuallySpecifiedXMinXMax;
      set
      {
        if (!(_isManuallySpecifiedXMinXMax == value))
        {
          _isManuallySpecifiedXMinXMax = value;
          OnPropertyChanged(nameof(IsManuallySpecifiedXMinXMax));
          OnPropertyChanged(nameof(IsNumberOfPointsVisible));
          OnPropertyChanged(nameof(IsNumberOfPointsLabelVisible));
          OnPropertyChanged(nameof(IsMaximumNumberOfPointsLabelVisible));
        }
      }
    }

    private bool _isSpecificationPointsPerDecade;

    /// <summary>
    /// Gets or sets a value indicating whether the point density is specified in points per decade.
    /// </summary>
    public bool IsSpecificationPointsPerDecade
    {
      get => _isSpecificationPointsPerDecade;
      set
      {
        if (!(_isSpecificationPointsPerDecade == value))
        {
          _isSpecificationPointsPerDecade = value;

          IsSpecificationPointsPerDecadeChanged(value);

          OnPropertyChanged(nameof(IsSpecificationPointsPerDecade));
          OnPropertyChanged(nameof(IsNumberOfPointsVisible));
          OnPropertyChanged(nameof(IsNumberOfPointsLabelVisible));
          OnPropertyChanged(nameof(IsMaximumNumberOfPointsLabelVisible));
        }
      }
    }

    private void IsSpecificationPointsPerDecadeChanged(bool value)
    {
      if (value == true)
      {
        if (PointsPerDecade == 0)
          PointsPerDecade = 1;

        if (!IsManuallySpecifiedXMinXMax)
          NumberOfPoints = int.MaxValue;
      }
      else
      {
        PointsPerDecade = 0;

        if (NumberOfPoints == int.MaxValue)
        {
          if (IsManuallySpecifiedXMinXMax && XMaximum > XMinimum)
          {
            NumberOfPoints = 1 + (int)Math.Max(1, Math.Ceiling(Math.Log10(XMaximum) - Math.Log10(XMinimum)));
          }
          else
          {
            NumberOfPoints = 10;
          }
        }
      }
    }

    /// <summary>
    /// Gets a value indicating whether the number-of-points field is visible.
    /// </summary>
    public bool IsNumberOfPointsVisible
    {
      get
      {
        return !(_isManuallySpecifiedXMinXMax && IsSpecificationPointsPerDecade);
      }
    }
    /// <summary>
    /// Gets a value indicating whether the number-of-points label is visible.
    /// </summary>
    public bool IsNumberOfPointsLabelVisible
    {
      get
      {
        return !IsSpecificationPointsPerDecade;
      }
    }
    /// <summary>
    /// Gets a value indicating whether the maximum-number-of-points label is visible.
    /// </summary>
    public bool IsMaximumNumberOfPointsLabelVisible
    {
      get
      {
        return !IsManuallySpecifiedXMinXMax && IsSpecificationPointsPerDecade;
      }
    }



    private double _XMinimum;

    /// <summary>
    /// Gets or sets the minimum x value.
    /// </summary>
    public double XMinimum
    {
      get => _XMinimum;
      set
      {
        if (!(_XMinimum == value))
        {
          _XMinimum = value;
          OnPropertyChanged(nameof(XMinimum));
        }
      }
    }
    private double _XMaximum;

    /// <summary>
    /// Gets or sets the maximum x value.
    /// </summary>
    public double XMaximum
    {
      get => _XMaximum;
      set
      {
        if (!(_XMaximum == value))
        {
          _XMaximum = value;
          OnPropertyChanged(nameof(XMaximum));
        }
      }
    }

    private int _NumberOfPoints;

    /// <summary>
    /// Gets or sets the number of interpolation points.
    /// </summary>
    public int NumberOfPoints
    {
      get => _NumberOfPoints;
      set
      {
        if (!(_NumberOfPoints == value))
        {
          _NumberOfPoints = value;
          OnPropertyChanged(nameof(NumberOfPoints));
        }
      }
    }

    private double _PointsPerDecade;

    /// <summary>
    /// Gets or sets the number of points per decade.
    /// </summary>
    public double PointsPerDecade
    {
      get => _PointsPerDecade;
      set
      {
        if (!(_PointsPerDecade == value))
        {
          _PointsPerDecade = value;
          OnPropertyChanged(nameof(PointsPerDecade));
        }
      }
    }




    private bool _isRelaxation;

    /// <summary>
    /// Gets or sets a value indicating whether the interpolation describes a relaxation spectrum.
    /// </summary>
    public bool IsRelaxation
    {
      get => _isRelaxation;
      set
      {
        if (!(_isRelaxation == value))
        {
          _isRelaxation = value;
          OnPropertyChanged(nameof(IsRelaxation));
        }
      }
    }

    private bool _UseIntercept;

    /// <summary>
    /// Gets or sets a value indicating whether an intercept is used.
    /// </summary>
    public bool UseIntercept
    {
      get => _UseIntercept;
      set
      {
        if (!(_UseIntercept == value))
        {
          _UseIntercept = value;
          OnPropertyChanged(nameof(UseIntercept));
        }
      }
    }
    private bool _allowNegativePronyCoefficients;

    /// <summary>
    /// Gets or sets a value indicating whether negative Prony coefficients are allowed.
    /// </summary>
    public bool AllowNegativePronyCoefficients
    {
      get => _allowNegativePronyCoefficients;
      set
      {
        if (!(_allowNegativePronyCoefficients == value))
        {
          _allowNegativePronyCoefficients = value;
          OnPropertyChanged(nameof(AllowNegativePronyCoefficients));
        }
      }
    }

    private double _regularizationParameter;

    /// <summary>
    /// Gets or sets the regularization parameter.
    /// </summary>
    public double RegularizationParameter
    {
      get => _regularizationParameter;
      set
      {
        if (!(_regularizationParameter == value))
        {
          _regularizationParameter = value;
          OnPropertyChanged(nameof(RegularizationParameter));
        }
      }
    }




    #endregion

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        IsRelaxation = _doc.IsRelaxation;
        NumberOfPoints = _doc.NumberOfPoints;
        PointsPerDecade = _doc.PointsPerDecade;
        IsSpecificationPointsPerDecade = PointsPerDecade > 0;
        if (_doc.XMinimumMaximum is { } minmax)
        {
          XMinimum = minmax.xMinimum;
          XMaximum = minmax.xMaximum;
          IsManuallySpecifiedXMinXMax = true;
        }
        else
        {
          IsManuallySpecifiedXMinXMax = false;
        }

        UseIntercept = _doc.UseIntercept;
        AllowNegativePronyCoefficients |= _doc.AllowNegativePronyCoefficients;
        RegularizationParameter = _doc.RegularizationParameter;
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      if (IsManuallySpecifiedXMinXMax)
      {
        if (!(XMaximum > XMinimum))
        {
          Current.Gui.ErrorMessageBox("XMaximum and XMinimum must be numbers and XMaximum has to be greater than XMinimum");
          return ApplyEnd(false, disposeController);
        }
      }

      _doc = _doc with
      {
        IsRelaxation = IsRelaxation,
        XMinimumMaximum = IsManuallySpecifiedXMinXMax ? (XMinimum, XMaximum) : null,
        PointsPerDecade = IsSpecificationPointsPerDecade ? PointsPerDecade : 0,
        NumberOfPoints = NumberOfPoints,
        UseIntercept = UseIntercept,
        AllowNegativePronyCoefficients = AllowNegativePronyCoefficients,
        RegularizationParameter = RegularizationParameter,
      };

      return ApplyEnd(true, disposeController);
    }


  }
}

#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Altaxo.Collections;
using Altaxo.Science.Spectroscopy;

namespace Altaxo.Gui.Science.Spectroscopy
{
    /// <summary>
/// Defines the contract for peak Searching And Fitting Output Options View.
/// </summary>
public interface IPeakSearchingAndFittingOutputOptionsView : IDataContextAwareView
  {
  }

  /// <summary>
/// Represents a controller for peak Searching And Fitting Output Options.
/// </summary>
[ExpectedTypeOfView(typeof(IPeakSearchingAndFittingOutputOptionsView))]
  [UserControllerForObject(typeof(PeakSearchingAndFittingOutputOptions))]
  public class PeakSearchingAndFittingOutputOptionsController : MVCANControllerEditImmutableDocBase<PeakSearchingAndFittingOutputOptions, IPeakSearchingAndFittingOutputOptionsView>
  {
        /// <inheritdoc />
public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private bool _outputPreprocessedCurve;

        /// <summary>
/// Gets or sets the output Preprocessed Curve.
/// </summary>
/// <value>
/// <c>true</c> if output Preprocessed Curve; otherwise, <c>false</c>.
/// </value>
public bool OutputPreprocessedCurve
    {
      get => _outputPreprocessedCurve;
      set
      {
        if (!(_outputPreprocessedCurve == value))
        {
          _outputPreprocessedCurve = value;
          OnPropertyChanged(nameof(OutputPreprocessedCurve));
        }
      }
    }

    private bool _outputFitCurve;

        /// <summary>
/// Gets or sets the output Fit Curve.
/// </summary>
/// <value>
/// <c>true</c> if output Fit Curve; otherwise, <c>false</c>.
/// </value>
public bool OutputFitCurve
    {
      get => _outputFitCurve;
      set
      {
        if (!(_outputFitCurve == value))
        {
          _outputFitCurve = value;
          OnPropertyChanged(nameof(OutputFitCurve));
        }
      }
    }

    private bool _outputFitCurveAsSeparatePeaks;

        /// <summary>
/// Gets or sets the output Fit Curve As Separate Peaks.
/// </summary>
/// <value>
/// <c>true</c> if output Fit Curve As Separate Peaks; otherwise, <c>false</c>.
/// </value>
public bool OutputFitCurveAsSeparatePeaks
    {
      get => _outputFitCurveAsSeparatePeaks;
      set
      {
        if (!(_outputFitCurveAsSeparatePeaks == value))
        {
          _outputFitCurveAsSeparatePeaks = value;
          OnPropertyChanged(nameof(OutputFitCurveAsSeparatePeaks));
        }
      }
    }

    private int _outputFitCurveSamplingFactor;

        /// <summary>
/// Gets or sets the output Fit Curve Sampling Factor.
/// </summary>
/// <value>
/// The output Fit Curve Sampling Factor.
/// </value>
public int OutputFitCurveSamplingFactor
    {
      get => _outputFitCurveSamplingFactor;
      set
      {
        if (!(_outputFitCurveSamplingFactor == value))
        {
          _outputFitCurveSamplingFactor = value;
          OnPropertyChanged(nameof(OutputFitCurveSamplingFactor));
        }
      }
    }

    private int _outputFitCurveAsSeparatePeaksSamplingFactor;

        /// <summary>
/// Gets or sets the output Fit Curve As Separate Peaks Sampling Factor.
/// </summary>
/// <value>
/// The output Fit Curve As Separate Peaks Sampling Factor.
/// </value>
public int OutputFitCurveAsSeparatePeaksSamplingFactor
    {
      get => _outputFitCurveAsSeparatePeaksSamplingFactor;
      set
      {
        if (!(_outputFitCurveAsSeparatePeaksSamplingFactor == value))
        {
          _outputFitCurveAsSeparatePeaksSamplingFactor = value;
          OnPropertyChanged(nameof(OutputFitCurveAsSeparatePeaksSamplingFactor));
        }
      }
    }

    private bool _outputBaselineCurve;

        /// <summary>
/// Gets or sets the output Baseline Curve.
/// </summary>
/// <value>
/// <c>true</c> if output Baseline Curve; otherwise, <c>false</c>.
/// </value>
public bool OutputBaselineCurve
    {
      get => _outputBaselineCurve;
      set
      {
        if (!(_outputBaselineCurve == value))
        {
          _outputBaselineCurve = value;
          OnPropertyChanged(nameof(OutputBaselineCurve));
        }
      }
    }

    private bool _outputFitResidualCurve;

        /// <summary>
/// Gets or sets the output Fit Residual Curve.
/// </summary>
/// <value>
/// <c>true</c> if output Fit Residual Curve; otherwise, <c>false</c>.
/// </value>
public bool OutputFitResidualCurve
    {
      get => _outputFitResidualCurve;
      set
      {
        if (!(_outputFitResidualCurve == value))
        {
          _outputFitResidualCurve = value;
          OnPropertyChanged(nameof(OutputFitResidualCurve));
        }
      }
    }


    /// <summary>
/// Represents the property.
/// </summary>
public class Property : IEditableObject, INotifyPropertyChanged
    {
      /// <inheritdoc/>
      public event PropertyChangedEventHandler? PropertyChanged;

            /// <summary>
/// Initializes a new instance of the <see cref="Altaxo.Gui.Science.Spectroscopy.PeakSearchingAndFittingOutputOptionsController.Property" /> class.
/// </summary>
public Property() { }
            /// <summary>
/// Initializes a new instance of the <see cref="Altaxo.Gui.Science.Spectroscopy.PeakSearchingAndFittingOutputOptionsController.Property" /> class.
/// </summary>
/// <param name="name">The name.</param>
public Property(string name)
      {
        _propertyName = name ?? string.Empty;
      }


            /// <inheritdoc />
public void BeginEdit()
      {
      }

            /// <inheritdoc />
public void CancelEdit()
      {
      }

            /// <inheritdoc />
public void EndEdit()
      {
      }

      private string _propertyName = string.Empty;
            /// <summary>
/// Gets or sets the property Name.
/// </summary>
/// <value>
/// The property Name.
/// </value>
public string PropertyName
      {
        get => _propertyName;
        set
        {
          if (_propertyName != value)
          {
            _propertyName = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PropertyName)));
          }
        }
      }
    }

        /// <summary>
/// Gets the property Names.
/// </summary>
/// <value>
/// The property Names.
/// </value>
public ObservableCollection<Property> PropertyNames { get; } = new();


    #endregion

        /// <inheritdoc />
protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        OutputPreprocessedCurve = _doc.OutputPreprocessedCurve;
        OutputFitCurve = _doc.OutputFitCurve;
        OutputFitCurveSamplingFactor = _doc.OutputFitCurveSamplingFactor;
        OutputFitCurveAsSeparatePeaks = _doc.OutputFitCurveAsSeparatePeaks;
        OutputFitCurveAsSeparatePeaksSamplingFactor = _doc.OutputFitCurveAsSeparatePeaksSamplingFactor;
        OutputBaselineCurve = _doc.OutputBaselineCurve;
        OutputFitResidualCurve = _doc.OutputFitResidualCurve;
        PropertyNames.AddRange(_doc.PropertyNames.Select(x => new Property(x)));
      }
    }

        /// <inheritdoc />
public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        OutputPreprocessedCurve = OutputPreprocessedCurve,
        OutputFitCurve = OutputFitCurve,
        OutputFitCurveSamplingFactor = _doc.OutputFitCurveSamplingFactor,
        OutputFitCurveAsSeparatePeaks = OutputFitCurveAsSeparatePeaks,
        OutputFitCurveAsSeparatePeaksSamplingFactor = OutputFitCurveAsSeparatePeaksSamplingFactor,
        OutputBaselineCurve = OutputBaselineCurve,
        OutputFitResidualCurve = OutputFitResidualCurve,
        PropertyNames = PropertyNames.Select(x => x.PropertyName).Distinct().ToArray(),
      };

      return ApplyEnd(true, disposeController);
    }

  }
}

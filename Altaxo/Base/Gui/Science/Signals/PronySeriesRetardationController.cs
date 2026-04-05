#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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
using Altaxo.Science.Signals;

namespace Altaxo.Gui.Science.Signals
{
  /// <summary>
  /// View interface for Prony-series retardation settings.
  /// </summary>
  public interface IPronySeriesRetardationView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for <see cref="PronySeriesRetardation"/>.
  /// </summary>
  [UserControllerForObject(typeof(PronySeriesRetardation))]
  [ExpectedTypeOfView(typeof(IPronySeriesRetardationView))]
  public class PronySeriesRetardationController : MVCANControllerEditImmutableDocBase<PronySeriesRetardation, IPronySeriesRetardationView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private double _timeMinimum;

    /// <summary>
    /// Gets or sets the minimum retardation time.
    /// </summary>
    public double TimeMinimum
    {
      get => _timeMinimum;
      set
      {
        if (!(_timeMinimum == value))
        {
          _timeMinimum = value;
          OnPropertyChanged(nameof(TimeMinimum));
        }
      }
    }

    private double _timeMaximum;

    /// <summary>
    /// Gets or sets the maximum retardation time.
    /// </summary>
    public double TimeMaximum
    {
      get => _timeMaximum;
      set
      {
        if (!(_timeMaximum == value))
        {
          _timeMaximum = value;
          OnPropertyChanged(nameof(TimeMaximum));
        }
      }
    }


    private int _numberOfRelaxationTimes;

    /// <summary>
    /// Gets or sets the number of retardation times.
    /// </summary>
    public int NumberOfRelaxationTimes
    {
      get => _numberOfRelaxationTimes;
      set
      {
        if (!(_numberOfRelaxationTimes == value))
        {
          _numberOfRelaxationTimes = value;
          OnPropertyChanged(nameof(NumberOfRelaxationTimes));
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

    private bool _useIntercept;

    /// <summary>
    /// Gets or sets a value indicating whether an intercept is used.
    /// </summary>
    public bool UseIntercept
    {
      get => _useIntercept;
      set
      {
        if (!(_useIntercept == value))
        {
          _useIntercept = value;
          OnPropertyChanged(nameof(UseIntercept));
        }
      }
    }

    private bool _useFlowTerm;

    /// <summary>
    /// Gets or sets a value indicating whether the flow term is used.
    /// </summary>
    public bool UseFlowTerm
    {
      get => _useFlowTerm;
      set
      {
        if (!(_useFlowTerm == value))
        {
          _useFlowTerm = value;
          OnPropertyChanged(nameof(UseFlowTerm));
        }
      }
    }


    private bool _isDielectricSpectrum;

    /// <summary>
    /// Gets or sets a value indicating whether the data represent a dielectric spectrum.
    /// </summary>
    public bool IsDielectricSpectrum
    {
      get => _isDielectricSpectrum;
      set
      {
        if (!(_isDielectricSpectrum == value))
        {
          _isDielectricSpectrum = value;
          OnPropertyChanged(nameof(IsDielectricSpectrum));
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
        TimeMinimum = _doc.MinimalRetardationTime;
        TimeMaximum = _doc.MaximalRetardationTime;
        NumberOfRelaxationTimes = _doc.NumberOfRetardationTimes;
        RegularizationParameter = _doc.RegularizationParameter;
        UseIntercept = _doc.UseIntercept;
        UseFlowTerm = _doc.UseFlowTerm;
        IsDielectricSpectrum = _doc.IsDielectricSpectrum;
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      try
      {
        _doc = _doc with
        {
          MinimalRetardationTime = TimeMinimum,
          MaximalRetardationTime = TimeMaximum,
          NumberOfRetardationTimes = NumberOfRelaxationTimes,
          RegularizationParameter = RegularizationParameter,
          UseIntercept = UseIntercept,
          UseFlowTerm = UseFlowTerm,
          IsDielectricSpectrum = IsDielectricSpectrum,
        };
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox($"The changes could not be applied: {ex.Message}", "Apply failed");
        return ApplyEnd(false, disposeController);
      }

      return ApplyEnd(true, disposeController);
    }


  }
}

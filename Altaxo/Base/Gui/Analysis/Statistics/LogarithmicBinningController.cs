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

#nullable disable
using System;
using System.Collections.Generic;
using Altaxo.Analysis.Statistics.Histograms;

namespace Altaxo.Gui.Analysis.Statistics
{
  /// <summary>
  /// Defines the view contract for editing logarithmic binning options.
  /// </summary>
  public interface ILogarithmicBinningView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for <see cref="LogarithmicBinning"/>.
  /// </summary>
  [UserControllerForObject(typeof(LogarithmicBinning))]
  [ExpectedTypeOfView(typeof(ILogarithmicBinningView))]
  public class LogarithmicBinningController : MVCANControllerEditOriginalDocBase<LogarithmicBinning, ILogarithmicBinningView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private bool _isUserDefinedBinOffset;

    /// <summary>
    /// Gets or sets a value indicating whether the bin offset is user-defined.
    /// </summary>
    public bool IsUserDefinedBinOffset
    {
      get => _isUserDefinedBinOffset;
      set
      {
        if (!(_isUserDefinedBinOffset == value))
        {
          _isUserDefinedBinOffset = value;
          OnPropertyChanged(nameof(IsUserDefinedBinOffset));
        }
      }
    }
    private double _binOffset;

    /// <summary>
    /// Gets or sets the bin offset.
    /// </summary>
    public double BinOffset
    {
      get => _binOffset;
      set
      {
        if (!(_binOffset == value))
        {
          _binOffset = value;
          OnPropertyChanged(nameof(BinOffset));
        }
      }
    }
    private bool _isUserDefinedBinWidth;

    /// <summary>
    /// Gets or sets a value indicating whether the bin width is user-defined.
    /// </summary>
    public bool IsUserDefinedBinWidth
    {
      get => _isUserDefinedBinWidth;
      set
      {
        if (!(_isUserDefinedBinWidth == value))
        {
          _isUserDefinedBinWidth = value;
          OnPropertyChanged(nameof(IsUserDefinedBinWidth));
        }
      }
    }
    private double _binWidth;

    /// <summary>
    /// Gets or sets the bin width.
    /// </summary>
    public double BinWidth
    {
      get => _binWidth;
      set
      {
        if (!(_binWidth == value))
        {
          _binWidth = value;
          OnPropertyChanged(nameof(BinWidth));
        }
      }
    }
    private double _resultingBinCount;

    /// <summary>
    /// Gets or sets the resulting number of bins.
    /// </summary>
    public double ResultingBinCount
    {
      get => _resultingBinCount;
      set
      {
        if (!(_resultingBinCount == value))
        {
          _resultingBinCount = value;
          OnPropertyChanged(nameof(ResultingBinCount));
        }
      }
    }


    #endregion Bindings


    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        IsUserDefinedBinOffset = _doc.IsUserDefinedBinOffset;
        IsUserDefinedBinWidth = _doc.IsUserDefinedBinWidth;

        BinOffset = _doc.BinOffset;
        BinWidth = Math.Pow(10, _doc.BinWidthInDecades);
        ResultingBinCount = _doc.NumberOfBins;
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      _doc.IsUserDefinedBinOffset = IsUserDefinedBinOffset;
      _doc.IsUserDefinedBinWidth = IsUserDefinedBinWidth;

      if (_doc.IsUserDefinedBinOffset)
        _doc.BinOffset = BinOffset;

      if (_doc.IsUserDefinedBinWidth)
        _doc.BinWidthInDecades = Math.Log10(BinWidth);

      return ApplyEnd(true, disposeController);
    }
  }
}

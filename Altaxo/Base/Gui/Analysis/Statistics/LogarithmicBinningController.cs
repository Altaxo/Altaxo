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
  public interface ILogarithmicBinningView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(LogarithmicBinning))]
  [ExpectedTypeOfView(typeof(ILogarithmicBinningView))]
  public class LogarithmicBinningController : MVCANControllerEditOriginalDocBase<LogarithmicBinning, ILogarithmicBinningView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private bool _isUserDefinedBinOffset;

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

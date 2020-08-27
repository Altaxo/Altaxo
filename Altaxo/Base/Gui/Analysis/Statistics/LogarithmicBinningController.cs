#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;
using Altaxo.Analysis.Statistics.Histograms;
using Altaxo.Data;

namespace Altaxo.Gui.Analysis.Statistics
{
  public interface ILogarithmicBinningView
  {
    bool IsUserDefinedBinOffset { get; set; }

    double BinOffset { get; set; }

    bool IsUserDefinedBinWidth { get; set; }

    double BinWidth { get; set; }

    double ResultingBinCount { set; }
  }

  [UserControllerForObject(typeof(LogarithmicBinning))]
  [ExpectedTypeOfView(typeof(ILogarithmicBinningView))]
  public class LogarithmicBinningController : MVCANControllerEditOriginalDocBase<LogarithmicBinning, ILogarithmicBinningView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
      }

      if (null != _view)
      {
        _view.IsUserDefinedBinOffset = _doc.IsUserDefinedBinOffset;
        _view.IsUserDefinedBinWidth = _doc.IsUserDefinedBinWidth;

        _view.BinOffset = _doc.BinOffset;
        _view.BinWidth = Math.Pow(10, _doc.BinWidthInDecades);
        _view.ResultingBinCount = _doc.NumberOfBins;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc.IsUserDefinedBinOffset = _view.IsUserDefinedBinOffset;
      _doc.IsUserDefinedBinWidth = _view.IsUserDefinedBinWidth;

      if (_doc.IsUserDefinedBinOffset)
        _doc.BinOffset = _view.BinOffset;

      if (_doc.IsUserDefinedBinWidth)
        _doc.BinWidthInDecades = Math.Log10(_view.BinWidth);

      return ApplyEnd(true, disposeController);
    }
  }
}

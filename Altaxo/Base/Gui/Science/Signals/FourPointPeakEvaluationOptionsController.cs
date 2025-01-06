#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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
using Altaxo.Science.Signals;

namespace Altaxo.Gui.Science.Signals
{
  public interface IFourPointPeakEvaluationOptionsView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(FourPointPeakEvaluationOptions))]
  [ExpectedTypeOfView(typeof(IFourPointPeakEvaluationOptionsView))]
  public class FourPointPeakEvaluationOptionsController : MVCANControllerEditImmutableDocBase<FourPointPeakEvaluationOptions, IFourPointPeakEvaluationOptionsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings



    private bool _IncludeOriginalPointsInOutput;

    public bool IncludeOriginalPointsInOutput
    {
      get => _IncludeOriginalPointsInOutput;
      set
      {
        if (!(_IncludeOriginalPointsInOutput == value))
        {
          _IncludeOriginalPointsInOutput = value;
          OnPropertyChanged(nameof(IncludeOriginalPointsInOutput));
        }
      }
    }


    private double _IndexLeftOuter;

    public double IndexLeftOuter
    {
      get => _IndexLeftOuter;
      set
      {
        if (!(_IndexLeftOuter == value))
        {
          _IndexLeftOuter = value;
          OnPropertyChanged(nameof(IndexLeftOuter));
        }
      }
    }

    private double _IndexLeftInner;

    public double IndexLeftInner
    {
      get => _IndexLeftInner;
      set
      {
        if (!(_IndexLeftInner == value))
        {
          _IndexLeftInner = value;
          OnPropertyChanged(nameof(IndexLeftInner));
        }
      }
    }

    private double _IndexRightOuter;

    public double IndexRightOuter
    {
      get => _IndexRightOuter;
      set
      {
        if (!(_IndexRightOuter == value))
        {
          _IndexRightOuter = value;
          OnPropertyChanged(nameof(IndexRightOuter));
        }
      }
    }

    private double _IndexRightInner;

    public double IndexRightInner
    {
      get => _IndexRightInner;
      set
      {
        if (!(_IndexRightInner == value))
        {
          _IndexRightInner = value;
          OnPropertyChanged(nameof(IndexRightInner));
        }
      }
    }

    #endregion


    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        IncludeOriginalPointsInOutput = _doc.IncludeOriginalPointsInOutput;
        IndexLeftOuter = _doc.IndexLeftOuter;
        IndexLeftInner = _doc.IndexLeftInner;
        IndexRightOuter = _doc.IndexRightOuter;
        IndexRightInner = _doc.IndexRightInner;
      }
    }
    public override bool Apply(bool disposeController)
    {


      _doc = new FourPointPeakEvaluationOptions()
      {
        IncludeOriginalPointsInOutput = IncludeOriginalPointsInOutput,
        IndexLeftOuter = IndexLeftOuter,
        IndexLeftInner = IndexLeftInner,
        IndexRightOuter = IndexRightOuter,
        IndexRightInner = IndexRightInner,
      };

      return ApplyEnd(true, disposeController);

    }

  }
}

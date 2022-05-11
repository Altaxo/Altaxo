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

using System;
using System.Collections.Generic;
using Altaxo.Science.Spectroscopy.BaselineEstimation;

namespace Altaxo.Gui.Analysis.Spectroscopy.BaselineEstimation
{
  public interface IArPLSView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(ArPLS))]
  [ExpectedTypeOfView(typeof(IArPLSView))]
  public class ArPLSController : MVCANControllerEditImmutableDocBase<ArPLS, IArPLSView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private double _lambda;

    public double Lambda
    {
      get => _lambda;
      set
      {
        if (!(_lambda == value))
        {
          _lambda = value;
          OnPropertyChanged(nameof(Lambda));
        }
      }
    }

    private bool _scaleLambdaWithXUnits;

    public bool ScaleLambdaWithXUnits
    {
      get => _scaleLambdaWithXUnits;
      set
      {
        if (!(_scaleLambdaWithXUnits == value))
        {
          _scaleLambdaWithXUnits = value;
          OnPropertyChanged(nameof(ScaleLambdaWithXUnits));
        }
      }
    }

    private double _terminationRatio;

    public double TerminationRatio
    {
      get => _terminationRatio;
      set
      {
        if (!(_terminationRatio == value))
        {
          _terminationRatio = value;
          OnPropertyChanged(nameof(TerminationRatio));
        }
      }
    }


    private int _maximalNumberOfIterations;

    public int MaximalNumberOfIterations
    {
      get => _maximalNumberOfIterations;
      set
      {
        if (!(_maximalNumberOfIterations == value))
        {
          _maximalNumberOfIterations = value;
          OnPropertyChanged(nameof(MaximalNumberOfIterations));
        }
      }
    }

    private int _order;

    public int Order
    {
      get => _order;
      set
      {
        if (!(_order == value))
        {
          _order = value;
          OnPropertyChanged(nameof(Order));
        }
      }
    }

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        Lambda = _doc.Lambda;
        ScaleLambdaWithXUnits = _doc.ScaleLambdaWithXUnits;
        TerminationRatio = _doc.TerminationRatio;
        MaximalNumberOfIterations = _doc.MaximumNumberOfIterations;
        Order = _doc.Order;
      }
    }

    public override bool Apply(bool disposeController)
    {
      try
      {
        _doc = _doc with
        {
          Lambda = Lambda,
          ScaleLambdaWithXUnits = ScaleLambdaWithXUnits,
          TerminationRatio = TerminationRatio,
          MaximumNumberOfIterations = MaximalNumberOfIterations,
          Order = Order,
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

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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Calc.FitFunctions.Peaks;
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Main.Properties;
using Altaxo.Science.Spectroscopy.PeakFitting;
using Altaxo.Science.Spectroscopy.PeakSearching;
using Altaxo.Units;

namespace Altaxo.Gui.Science.Spectroscopy.PeakFitting
{
  public interface IPeakFittingByIncrementalPeakAdditionView : IDataContextAwareView { }

  [UserControllerForObject(typeof(PeakFittingByIncrementalPeakAddition))]
  [ExpectedTypeOfView(typeof(IPeakFittingByIncrementalPeakAdditionView))]
  public class PeakFittingByIncrementalPeakAdditionController : PeakFittingBaseController<PeakFittingByIncrementalPeakAddition, IPeakFittingByIncrementalPeakAdditionView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private ItemsController<Type> _fitFunctions;

    public ItemsController<Type> FitFunctions
    {
      get => _fitFunctions;
      set
      {
        if (!(_fitFunctions == value))
        {
          _fitFunctions = value;
          OnPropertyChanged(nameof(FitFunctions));
        }
      }
    }

    private int _orderOfBaselinePolynomial;

    public int OrderOfBaselinePolynomial
    {
      get => _orderOfBaselinePolynomial;
      set
      {
        if (!(_orderOfBaselinePolynomial == value))
        {
          _orderOfBaselinePolynomial = value;
          OnPropertyChanged(nameof(OrderOfBaselinePolynomial));
        }
      }
    }

    private int _maximumNumberOfPeaks;

    public int MaximumNumberOfPeaks
    {
      get => _maximumNumberOfPeaks;
      set
      {
        if (!(_maximumNumberOfPeaks == value))
        {
          _maximumNumberOfPeaks = value;
          OnPropertyChanged(nameof(MaximumNumberOfPeaks));
        }
      }
    }




    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if(initData)
      {
        var ftypeList = new SelectableListNodeList(
          Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IFitFunctionPeak))
            .Select(t => new SelectableListNode(t.Name, t, false))
            );
        FitFunctions = new ItemsController<Type>(ftypeList);
        FitFunctions.SelectedValue = _doc.FitFunction.GetType();

        OrderOfBaselinePolynomial = _doc.OrderOfBaselinePolynomial;

        MaximumNumberOfPeaks = _doc.MaximumNumberOfPeaks;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        FitFunction = (IFitFunctionPeak)Activator.CreateInstance(FitFunctions.SelectedValue),
        OrderOfBaselinePolynomial = OrderOfBaselinePolynomial,
        MaximumNumberOfPeaks = MaximumNumberOfPeaks,
      };

      return ApplyEnd(true, disposeController);
    }

   
  }
}

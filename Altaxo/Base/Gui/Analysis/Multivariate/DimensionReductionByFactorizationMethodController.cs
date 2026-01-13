#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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
using Altaxo.Calc.LinearAlgebra.Double.Factorization;
using Altaxo.Calc.Regression.Multivariate;
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Analysis.Multivariate
{
  public interface IDimensionReductionByFactorizationMethodView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for <see cref="DimensionReductionByLowRankFactorization"/>
  /// </summary>
  [ExpectedTypeOfView(typeof(IDimensionReductionByFactorizationMethodView))]
  [UserControllerForObject(typeof(DimensionReductionByLowRankFactorization))]

  public class DimensionReductionByFactorizationMethodController : MVCANControllerEditImmutableDocBase<DimensionReductionByLowRankFactorization, IDimensionReductionByFactorizationMethodView>
  {

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(DetailsController, () => DetailsController = null!);
    }

    #region Bindings


    public ItemsController<ILowRankMatrixFactorization> Method
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(Method));
        }
      }
    }

    public int MaximumNumberOfFactors
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(MaximumNumberOfFactors));
        }
      }
    }


    public IMVCANController? DetailsController
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field?.Dispose();
          field = value;
          OnPropertyChanged(nameof(DetailsController));
        }
      }
    }

    #endregion Bindings

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var methodTypes = ReflectionService.GetNonAbstractSubclassesOf(typeof(ILowRankMatrixFactorization));
        var methodList = new SelectableListNodeList(
          methodTypes.Select(t =>
          {
            var instance = t == _doc.Method.GetType() ? _doc.Method : (ILowRankMatrixFactorization)Activator.CreateInstance(t)!;
            return new SelectableListNode(DimensionReductionByLowRankFactorization.GetDisplayName(instance.GetType()), instance, false);
          })
        );
        Method = new ItemsController<ILowRankMatrixFactorization>(methodList, EhMethodChanged);
        Method.SelectedValue = _doc.Method;
        MaximumNumberOfFactors = _doc.MaximumNumberOfFactors;
      }
    }

    private void EhMethodChanged(ILowRankMatrixFactorization factorization)
    {
      if (DetailsController is { } controller && controller.Apply(disposeController: true))
      {
        var mo = controller.ModelObject;
        Method.Items.FirstOrDefault(n => n.Tag?.GetType() == mo.GetType())?.Tag = mo;
      }

      DetailsController = (IMVCANController?)Current.Gui.GetControllerAndControl([factorization], typeof(IMVCANController));
    }

    public override bool Apply(bool disposeController)
    {
      if (DetailsController is { } controller)
      {
        if (!controller.Apply(disposeController: false))
          return ApplyEnd(false, disposeController);

        var mo = controller.ModelObject;
        Method.Items.FirstOrDefault(n => n.Tag?.GetType() == mo.GetType())?.Tag = mo;
      }

      _doc = _doc with
      {
        Method = Method.SelectedValue,
        MaximumNumberOfFactors = MaximumNumberOfFactors
      };

      return ApplyEnd(true, disposeController);
    }
  }
}

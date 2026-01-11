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

using System.Collections.Generic;
using Altaxo.Calc.Regression.Multivariate;

namespace Altaxo.Gui.Analysis.Multivariate
{
  public interface IDimensionReductionByFactorizationMethodView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for <see cref="DimensionReductionByFactorizationMethod"/>
  /// </summary>
  [ExpectedTypeOfView(typeof(IDimensionReductionByFactorizationMethodView))]
  [UserControllerForObject(typeof(DimensionReductionByFactorizationMethod))]

  public class DimensionReductionByFactorizationMethodController : MVCANControllerEditImmutableDocBase<DimensionReductionByFactorizationMethod, IDimensionReductionByFactorizationMethodView>
  {

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings


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


    #endregion Bindings

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        MaximumNumberOfFactors = _doc.MaximumNumberOfFactors;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc = _doc with { MaximumNumberOfFactors = MaximumNumberOfFactors };
      return ApplyEnd(true, disposeController);
    }
  }
}

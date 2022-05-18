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
using Altaxo.Gui.Common;
using Altaxo.Units;

namespace Altaxo.Gui.Analysis.Spectroscopy.PeakFitting
{
  public abstract class PeakFittingBaseController<TModel, TView> : MVCANControllerEditImmutableDocBase<TModel, TView> where TView : class
  {

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


    public QuantityWithUnitGuiEnvironment FitWidthScalingFactorEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _fitWidthScalingFactor;

    public DimensionfulQuantity FitWidthScalingFactor
    {
      get => _fitWidthScalingFactor;
      set
      {
        if (!(_fitWidthScalingFactor == value))
        {
          _fitWidthScalingFactor = value;
          OnPropertyChanged(nameof(FitWidthScalingFactor));
        }
      }
    }



    #endregion


  }
}

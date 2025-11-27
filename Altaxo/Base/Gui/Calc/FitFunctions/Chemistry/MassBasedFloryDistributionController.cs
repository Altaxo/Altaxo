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
using Altaxo.Calc.FitFunctions.Chemistry;

namespace Altaxo.Gui.Calc.FitFunctions.Chemistry
{
  public interface IMassBasedFloryDistributionView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(MassBasedFloryDistribution))]
  [ExpectedTypeOfView(typeof(IMassBasedFloryDistributionView))]
  public class MassBasedFloryDistributionController : MVCANControllerEditImmutableDocBase<MassBasedFloryDistribution, IMassBasedFloryDistributionView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings


    public double MolecularWeightOfMonomerUnit
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(MolecularWeightOfMonomerUnit));
        }
      }
    }


    public bool IndependentVariableIsDecadicLogarithm
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(IndependentVariableIsDecadicLogarithm));
        }
      }
    }








    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        MolecularWeightOfMonomerUnit = _doc.MolecularWeightOfMonomerUnit;
        IndependentVariableIsDecadicLogarithm = _doc.IndependentVariableIsDecadicLogarithm;
      }
    }


    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        MolecularWeightOfMonomerUnit = MolecularWeightOfMonomerUnit,
        IndependentVariableIsDecadicLogarithm = IndependentVariableIsDecadicLogarithm,
      };

      return ApplyEnd(true, disposeController);
    }
  }
}


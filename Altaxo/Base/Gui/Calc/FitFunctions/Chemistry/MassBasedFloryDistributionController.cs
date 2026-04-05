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
  /// <summary>
  /// Defines the view contract for editing <see cref="MassBasedFloryDistribution"/>.
  /// </summary>
  public interface IMassBasedFloryDistributionView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for <see cref="MassBasedFloryDistribution"/>.
  /// </summary>
  [UserControllerForObject(typeof(MassBasedFloryDistribution))]
  [ExpectedTypeOfView(typeof(IMassBasedFloryDistributionView))]
  public class MassBasedFloryDistributionController : MVCANControllerEditImmutableDocBase<MassBasedFloryDistribution, IMassBasedFloryDistributionView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings


    /// <summary>
    /// Gets or sets the baseline polynomial order.
    /// </summary>
    public int OrderOfBaselinePolynominal
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(OrderOfBaselinePolynominal));
        }
      }
    }


    /// <summary>
    /// Gets or sets the number of terms.
    /// </summary>
    public int NumberOfTerms
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(NumberOfTerms));
        }
      }
    }

    /// <summary>
    /// Gets or sets the molecular weight of the monomer unit.
    /// </summary>
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


    /// <summary>
    /// Gets or sets a value indicating whether the independent variable is the decadic logarithm.
    /// </summary>
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

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        NumberOfTerms = _doc.NumberOfTerms;
        OrderOfBaselinePolynominal = _doc.OrderOfBaselinePolynomial;
        MolecularWeightOfMonomerUnit = _doc.MolecularWeightOfMonomerUnit;
        IndependentVariableIsDecadicLogarithm = _doc.IndependentVariableIsDecadicLogarithm;
      }
    }


    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        NumberOfTerms = NumberOfTerms,
        OrderOfBaselinePolynomial = OrderOfBaselinePolynominal,
        MolecularWeightOfMonomerUnit = MolecularWeightOfMonomerUnit,
        IndependentVariableIsDecadicLogarithm = IndependentVariableIsDecadicLogarithm,
      };

      return ApplyEnd(true, disposeController);
    }
  }
}


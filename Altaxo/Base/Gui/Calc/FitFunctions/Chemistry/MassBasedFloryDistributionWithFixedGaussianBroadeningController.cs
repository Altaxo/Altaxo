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
  public interface IMassBasedFloryDistributionWithFixedGaussianBroadeningView : IDataContextAwareView
  {
  }


  [UserControllerForObject(typeof(MassBasedFloryDistributionWithFixedGaussianBroadening))]
  [ExpectedTypeOfView(typeof(IMassBasedFloryDistributionWithFixedGaussianBroadeningView))]
  public class MassBasedFloryDistributionWithFixedGaussianBroadeningController : MVCANControllerEditImmutableDocBase<MassBasedFloryDistributionWithFixedGaussianBroadening, IMassBasedFloryDistributionWithFixedGaussianBroadeningView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

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


    public double Accuracy
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(Accuracy));
        }
      }
    }


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


    public int OrderOfPolynomial
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(OrderOfPolynomial));
          OnPropertyChanged(nameof(Coeff0IsEnabled));
          OnPropertyChanged(nameof(Coeff1IsEnabled));
          OnPropertyChanged(nameof(Coeff2IsEnabled));
          OnPropertyChanged(nameof(Coeff3IsEnabled));
          OnPropertyChanged(nameof(Coeff4IsEnabled));
          OnPropertyChanged(nameof(Coeff5IsEnabled));
          OnPropertyChanged(nameof(Coeff6IsEnabled));
          OnPropertyChanged(nameof(Coeff7IsEnabled));
          OnPropertyChanged(nameof(Coeff8IsEnabled));
          OnPropertyChanged(nameof(Coeff9IsEnabled));
        }
      }
    }

    public bool Coeff0IsEnabled => OrderOfPolynomial >= 0;
    public bool Coeff1IsEnabled => OrderOfPolynomial >= 1;
    public bool Coeff2IsEnabled => OrderOfPolynomial >= 2;
    public bool Coeff3IsEnabled => OrderOfPolynomial >= 3;
    public bool Coeff4IsEnabled => OrderOfPolynomial >= 4;
    public bool Coeff5IsEnabled => OrderOfPolynomial >= 5;
    public bool Coeff6IsEnabled => OrderOfPolynomial >= 6;
    public bool Coeff7IsEnabled => OrderOfPolynomial >= 7;
    public bool Coeff8IsEnabled => OrderOfPolynomial >= 8;
    public bool Coeff9IsEnabled => OrderOfPolynomial >= 9;


    public double Coeff0
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(Coeff0));
        }
      }
    }


    public double Coeff1
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(Coeff1));
        }
      }
    }


    public double Coeff2
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(Coeff2));
        }
      }
    }


    public double Coeff3
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(Coeff3));
        }
      }
    }


    public double Coeff4
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(Coeff4));
        }
      }
    }

    public double Coeff5
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(Coeff5));
        }
      }
    }

    public double Coeff6
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(Coeff6));
        }
      }
    }

    public double Coeff7
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(Coeff7));
        }
      }
    }

    public double Coeff8
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(Coeff8));
        }
      }
    }
    public double Coeff9
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(Coeff9));
        }
      }
    }





    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        NumberOfTerms = _doc.NumberOfTerms;
        OrderOfBaselinePolynominal = _doc.OrderOfBaselinePolynomial;
        Accuracy = _doc.Accuracy;
        MolecularWeightOfMonomerUnit = _doc.MolecularWeightOfMonomerUnit;
        IndependentVariableIsDecadicLogarithm = _doc.IndependentVariableIsDecadicLogarithm;
        OrderOfPolynomial = _doc.PolynomialCoefficientsForSigma.Length - 1;

        var pc = _doc.PolynomialCoefficientsForSigma;
        int l = pc.Length;

        Coeff0 = l > 0 ? pc[0] : 0.0;
        Coeff1 = l > 1 ? pc[1] : 0.0;
        Coeff2 = l > 2 ? pc[2] : 0.0;
        Coeff3 = l > 3 ? pc[3] : 0.0;
        Coeff4 = l > 4 ? pc[4] : 0.0;
        Coeff5 = l > 5 ? pc[5] : 0.0;
        Coeff6 = l > 6 ? pc[6] : 0.0;
        Coeff7 = l > 7 ? pc[7] : 0.0;
        Coeff8 = l > 8 ? pc[8] : 0.0;
        Coeff9 = l > 9 ? pc[9] : 0.0;
      }
    }


    public override bool Apply(bool disposeController)
    {
      var arr = new double[]
      {
        Coeff0,
        Coeff1,
        Coeff2,
        Coeff3,
        Coeff4,
        Coeff5,
        Coeff6,
        Coeff7,
        Coeff8,
        Coeff9
      };

      _doc = _doc with
      {
        NumberOfTerms = NumberOfTerms,
        OrderOfBaselinePolynomial = OrderOfBaselinePolynominal,
        Accuracy = Accuracy,
        MolecularWeightOfMonomerUnit = MolecularWeightOfMonomerUnit,
        IndependentVariableIsDecadicLogarithm = IndependentVariableIsDecadicLogarithm,
        PolynomialCoefficientsForSigma = arr[0..(OrderOfPolynomial + 1)],
      };

      return ApplyEnd(true, disposeController);
    }
  }
}


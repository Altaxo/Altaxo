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

#nullable disable
using System;
using System.Collections.Generic;
using Altaxo.Calc.Regression;

namespace Altaxo.Gui.Worksheet
{
  #region Interfaces

   /// <summary>
   /// View contract for editing <see cref="SavitzkyGolayParameters"/>.
   /// </summary>
   public interface ISavitzkyGolayParameterView : IDataContextAwareView
  {
  }

  #endregion Interfaces

  /// <summary>
  /// Controller for editing <see cref="SavitzkyGolayParameters"/>.
  /// </summary>
  [UserControllerForObject(typeof(SavitzkyGolayParameters), 100)]
  [ExpectedTypeOfView(typeof(ISavitzkyGolayParameterView))]
  public class SavitzkyGolayParameterController : MVCANControllerEditImmutableDocBase<SavitzkyGolayParameters, ISavitzkyGolayParameterView>
  {

    /// <summary>
    /// Initializes a new instance of the <see cref="SavitzkyGolayParameterController"/> class.
    /// </summary>
    public SavitzkyGolayParameterController()
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SavitzkyGolayParameterController"/> class.
    /// </summary>
    /// <param name="parameters">The parameters to edit.</param>
    public SavitzkyGolayParameterController(SavitzkyGolayParameters parameters)
    {
      _originalDoc =  _doc = parameters;
      Initialize(true);
    }

    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }


    #region Bindings

    private int _numberOfPoints;

    /// <summary>
    /// Gets or sets the number of points used for the filter window.
    /// </summary>
    public int NumberOfPoints
    {
      get => _numberOfPoints;
      set
      {
        if (!(_numberOfPoints == value))
        {
          _numberOfPoints = value;
          OnPropertyChanged(nameof(NumberOfPoints));
        }
      }
    }

    private int _polynomialOrder;

    /// <summary>
    /// Gets or sets the polynomial order.
    /// </summary>
    public int PolynomialOrder
    {
      get => _polynomialOrder;
      set
      {
        if (!(_polynomialOrder == value))
        {
          _polynomialOrder = value;
          OnPropertyChanged(nameof(PolynomialOrder));
        }
      }
    }

    private int _derivativeOrder;

    /// <summary>
    /// Gets or sets the derivative order.
    /// </summary>
    public int DerivativeOrder
    {
      get => _derivativeOrder;
      set
      {
        if (!(_derivativeOrder == value))
        {
          _derivativeOrder = value;
          OnPropertyChanged(nameof(DerivativeOrder));
        }
      }
    }

    #endregion


    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);
      if(initData)
      {
        NumberOfPoints = _doc.NumberOfPoints;
        DerivativeOrder = _doc.DerivativeOrder;
        PolynomialOrder = _doc.PolynomialOrder;
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        NumberOfPoints = NumberOfPoints,
        DerivativeOrder = DerivativeOrder,
        PolynomialOrder = PolynomialOrder,
      };

      return ApplyEnd(true, disposeController);
    }

  }
}

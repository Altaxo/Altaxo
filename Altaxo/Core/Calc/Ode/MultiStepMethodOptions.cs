#region Copyright

/////////////////////////////////////////////////////////////////////////////
//
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
//    This source file is licensed under the MIT license.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Calc.Ode
{
  /// <summary>
  /// Options for multistep Ode methods.
  /// </summary>
  /// <seealso cref="Altaxo.Calc.Ode.OdeMethodOptions" />
  public class MultiStepMethodOptions : OdeMethodOptions
  {
    private int _maxOrder = int.MaxValue;

    /// <summary>
    /// Gets or sets the maximum order the method can use.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Value must be greater than or equal to 1.</exception>
    public int MaxOrder
    {
      get => _maxOrder;
      set
      {
        if (!(value >= 1))
          throw new ArgumentOutOfRangeException("Value must be >= 1");
        _maxOrder = value;
      }
    }


    private int _minOrder;

    /// <summary>
    /// Gets or sets the minimum order the method can use (except in the startup phase, in which the method has to start with an order of 1).
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Value must be greater than or equal to 1.</exception>
    public int MinOrder
    {
      get => _minOrder;
      set
      {
        if (!(value >= 1))
          throw new ArgumentOutOfRangeException("Value must be >= 1");
        _minOrder = value;
      }
    }

    /// <summary>
    /// Gets or sets the iteration method (see values of <see cref="OdeIterationMethod"/> for explanations). 
    /// </summary>
    public OdeIterationMethod IterationMethod { get; set; }
  }

  /// <summary>
  /// Iteration method for implicit Ode method (Gear's, implicit Adam's, and so on).
  /// </summary>
  public enum OdeIterationMethod
  {
    /// <summary>Use a function to calculate the jacobian. This function has either to be provided by the user,
    /// or, if the user does not provide such a function, an approximation by finite differences is used.</summary>
    UseJacobian,

    /// <summary>Set the jacobian matrix to zero. This will not allow Newton-Raphson iterations,
    /// that's why the iteration converges only slowly.</summary>
    DoNotUseJacobian
  }
}

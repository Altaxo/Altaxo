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
  /// Options for multistep ODE methods.
  /// </summary>
  /// <seealso cref="Altaxo.Calc.Ode.OdeMethodOptions" />
  public class MultiStepMethodOptions : OdeMethodOptions
  {
    private int _maxOrder = int.MaxValue;

    /// <summary>
    /// Gets or sets the maximum order the method can use.
    /// </summary>
    /// <value>The maximum order.</value>
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
    /// Gets or sets the minimum order the method can use.
    /// </summary>
    /// <remarks>
    /// In the startup phase, the method has to start with an order of 1.
    /// </remarks>
    /// <value>The minimum order.</value>
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
    /// Gets or sets the iteration method.
    /// </summary>
    /// <remarks>
    /// See values of <see cref="OdeIterationMethod"/> for details.
    /// </remarks>
    public OdeIterationMethod IterationMethod { get; set; }
  }

  /// <summary>
  /// Iteration method for implicit ODE methods (Gear's, implicit Adams, and so on).
  /// </summary>
  public enum OdeIterationMethod
  {
    /// <summary>
    /// Use a function to calculate the Jacobian.
    /// </summary>
    /// <remarks>
    /// This function either has to be provided by the user or, if the user does not provide such a function,
    /// an approximation by finite differences is used.
    /// </remarks>
    UseJacobian,

    /// <summary>
    /// Set the Jacobian matrix to zero.
    /// </summary>
    /// <remarks>
    /// This does not allow Newton-Raphson iterations; therefore, the iteration converges only slowly.
    /// </remarks>
    DoNotUseJacobian
  }
}

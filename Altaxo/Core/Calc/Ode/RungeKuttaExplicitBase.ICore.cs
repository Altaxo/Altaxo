#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.Ode
{
  public abstract partial class RungeKuttaExplicitBase
  {
    /// <summary>
    /// Interface for the core of the explicit Runge-Kutta methods.
    /// </summary>
    protected interface ICore
    {
      /// <summary>
      /// Gets or sets the absolute tolerance.
      /// </summary>
      /// <value>
      /// The absolute tolerance.
      /// </value>
      /// <exception cref="ArgumentException">Must be &gt;= 0 - AbsoluteTolerance</exception>
      double AbsoluteTolerance { get; set; }

      /// <summary>
      /// Gets or sets the relative tolerance.
      /// </summary>
      /// <value>
      /// The relative tolerance.
      /// </value>
      /// <exception cref="ArgumentException">Must be >= 0 - RelativeTolerance</exception>
      double RelativeTolerance { get; set; }


      /// <summary>
      /// Sets the stiffness detection threshold value.
      /// </summary>
      /// <value>
      /// The stiffness detection threshold value.
      /// </value>
      double StiffnessDetectionThresholdValue { set; }


      /// <summary>
      /// Sets the coefficients for the local error evaluation (used in order to guess the error).
      /// If set to null, the error is not evaluated.
      /// </summary>
      /// <value>
      /// The difference between high order and low order bottom side coefficients of the Runge-Kutta scheme.
      /// </value>
      double[]? BL { set; }


      /// <summary>
      /// Gets or sets the interpolation coefficients for dense output.
      /// </summary>
      /// <value>
      /// The interpolation coefficients.
      /// </value>
      double[][]? InterpolationCoefficients { get; set; }

      bool IsInitialized { get; }
      double X { get; }
      double X_previous { get; }
      double[] Y_volatile { get; }

      /// <summary>
      /// Evaluates the next solution point in one step. To get the results, see <see cref="X"/> and <see cref="Y_volatile"/>.
      /// </summary>
      /// <param name="stepSize">Size of the step.</param>
      void EvaluateNextSolutionPoint(double stepSize);

      /// <summary>
      /// Gets the initial step size. The absolute and relative tolerances must be set before the call to this function.
      /// </summary>
      /// <returns>The initial step size in the context of the absolute and relative tolerances.</returns>
      /// <exception cref="InvalidOperationException">Either absolute tolerance or relative tolerance is required to be &gt; 0</exception>
      double GetInitialStepSize();

      /// <summary>Get an interpolated point in the last evaluated interval.
      /// Please use the result immediately, or else make a copy of the result, since a internal array
      /// is returned, which is overwritten at the next operation.</summary>
      /// <param name="theta">Relative location (0..1) in the last evaluated interval.</param>
      /// <returns>Interpolated y values at the relative point of the last evaluated interval <paramref name="theta"/>.</returns>
      /// <remarks>This method is intended for FSAL methods only. We assume here, that k[_stages-1] contains
      /// the derivative of x_current.</remarks>
      double[] GetInterpolatedY_volatile(double theta);


      /// <summary>
      /// Gets the recommended step size.  
      /// </summary>
      /// <param name="error_current">The relative error of the current step.</param>
      /// <param name="error_previous">The relative error of the previous step.</param>
      /// <returns>The recommended step size in the context of the absolute and relative tolerances.</returns>
      double GetRecommendedStepSize(double error_current, double error_previous);

      /// <summary>
      /// Gets the relative error, which should be in the order of 1, if the step size is optimally chosen.
      /// </summary>
      /// <returns>The relative error (relative to the absolute and relative tolerance).</returns>
      double GetRelativeError();

      /// <summary>
      /// Reverts the state of the instance to the former solution point, by
      /// setting <see cref="X"/> to <see cref="X_previous"/> and <see cref="Y_volatile"/> y_previous.
      /// </summary>
      void Revert();

      /// <summary>
      /// Function that is been called after every <b>successfull</b> step.
      /// Detects a stiffness condition. If it founds one, an exception will be thrown.
      /// </summary>
      void ThrowIfStiffnessDetected();
    }
  }
}

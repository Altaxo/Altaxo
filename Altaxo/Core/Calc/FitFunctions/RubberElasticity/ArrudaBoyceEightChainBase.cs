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

namespace Altaxo.Calc.FitFunctions.RubberElasticity
{

  /// <summary>
  /// Implements the Arruda-Boyce eight-chain model for incompressible
  /// hyperelastic rubber-like materials.
  ///
  /// Strain energy density (statistical form):
  ///     W = mu * lambdaM^2 * ( (lambdaChain/lambdaM) * InverseLangevin(lambdaChain/lambdaM)
  ///                             + ln( (InverseLangevin(lambdaChain/lambdaM)) / sinh(InverseLangevin(lambdaChain/lambdaM)) ) )
  ///
  /// with the chain stretch lambdaChain = sqrt(I1 / 3).
  ///
  /// Differentiating W with respect to the principal stretch for each
  /// deformation mode gives the nominal (engineering) stress:
  ///
  ///     P = (mu / 3) * (lambdaM / lambdaChain) * InverseLangevin(lambdaChain / lambdaM) * dI1/dLambda
  ///
  /// where dI1/dLambda depends on the loading mode (incompressibility assumed, J = 1).
  ///
  /// Parameters:
  ///   mu      - initial shear modulus (small-strain), e.g. in MPa
  ///   lambdaM - locking stretch (chain limiting stretch), typically 1.5 - 8, must be > 1
  ///   lambda  - applied principal stretch (lambda = 1 + engineering strain)
  /// </summary>
  public abstract record ArrudaBoyceEightChainBase
  {
    /// <summary>
    /// Core evaluation shared by all loading modes.
    /// </summary>
    /// <param name="mu">Initial shear modulus.</param>
    /// <param name="lambdaM">Locking stretch (chain limit), must be &gt; 1.</param>
    /// <param name="i1">First invariant of the left Cauchy-Green tensor for the given mode.</param>
    /// <param name="dI1dLambda">Derivative of I1 with respect to lambda for the given mode.</param>
    /// <returns>Nominal (engineering) stress.</returns>
    protected static double NominalStress(double mu, double lambdaM, double i1, double dI1dLambda)
    {
      if (lambdaM <= 1.0)
        throw new ArgumentOutOfRangeException(nameof(lambdaM), "Locking stretch must be greater than 1.");

      double lambdaChain = Math.Sqrt(i1 / 3.0);
      double x = lambdaChain / lambdaM;

      if (x >= 1.0)
        throw new ArgumentOutOfRangeException(nameof(lambdaM),
            "Chain stretch has reached the locking stretch (x = lambdaChain/lambdaM >= 1). " +
            "Reduce lambda or increase lambdaM.");

      double invLangevin = Hyperbolic.InverseLangevin(x);

      return (mu / 3.0) * (lambdaM / lambdaChain) * invLangevin * dI1dLambda;
    }


    /// <summary>
    /// Core gradient (dP/dmu, dP/dlambdaM) shared by all loading modes.
    ///
    /// dP/dmu is simply P/mu, since P is linear in mu:
    ///     dP/dmu = (1/3) * (lambdaM/lambdaChain) * L(x) * dI1dLambda
    ///
    /// dP/dlambdaM follows from differentiating lambdaM * L(lambdaChain/lambdaM)
    /// with respect to lambdaM at fixed lambdaChain (lambdaChain depends only on
    /// lambda, not on lambdaM). Using L'(x) = (3+x^4)/(1-x^2)^2, this simplifies to:
    ///     d/dlambdaM [ lambdaM * L(x) ] = L(x) - x * L'(x) = -4*x^3 / (1-x^2)^2
    /// so:
    ///     dP/dlambdaM = (mu / (3*lambdaChain)) * dI1dLambda * ( -4*x^3 / (1-x^2)^2 )
    /// </summary>
    private static (double dPdMu, double dPdLambdaM) NominalStressGradient(
        double mu, double lambdaM, double i1, double dI1dLambda)
    {
      if (lambdaM <= 1.0)
        throw new ArgumentOutOfRangeException(nameof(lambdaM), "Locking stretch must be greater than 1.");

      double lambdaChain = Math.Sqrt(i1 / 3);
      double x = lambdaChain / lambdaM;

      if (x >= 1.0)
        throw new ArgumentOutOfRangeException(nameof(lambdaM),
            "Chain stretch has reached the locking stretch (x = lambdaChain/lambdaM >= 1). " +
            "Reduce lambda or increase lambdaM.");

      double invLangevin = Hyperbolic.InverseLangevin(x);
      double dPdMu = (1.0 / 3.0) * (lambdaM / lambdaChain) * invLangevin * dI1dLambda;

      double bracket = invLangevin - x / Hyperbolic.LangevinFirstDerivative(invLangevin);
      double dPdLambdaM = (mu / (3.0 * lambdaChain)) * dI1dLambda * bracket;

      return (dPdMu, dPdLambdaM);
    }

    /// <summary>
    /// Nominal (engineering) stress for uniaxial loading.
    /// Deformation: lambda1 = lambda, lambda2 = lambda3 = 1/sqrt(lambda) (incompressible).
    /// I1 = lambda^2 + 2/lambda
    /// </summary>
    public static double UniaxialStress(double lambda, double mu, double lambdaM)
    {
      if (lambda <= 0.0) throw new ArgumentOutOfRangeException(nameof(lambda));

      double i1 = lambda * lambda + 2.0 / lambda;
      double dI1dLambda = 2.0 * lambda - 2.0 / (lambda * lambda);

      return NominalStress(mu, lambdaM, i1, dI1dLambda);
    }

    /// <summary>
    /// Gradient (d(sigma)/d(mu), d(sigma)/d(lambdaM)) of the uniaxial nominal stress,
    /// for use as the Jacobian in a nonlinear least-squares fit.
    /// </summary>
    public static (double dSigmaDMu, double dSigmaDLambdaM) UniaxialStressGradient(
        double lambda, double mu, double lambdaM)
    {
      if (lambda <= 0.0) throw new ArgumentOutOfRangeException(nameof(lambda));

      double i1 = lambda * lambda + 2.0 / lambda;
      double dI1dLambda = 2.0 * lambda - 2.0 / (lambda * lambda);

      return NominalStressGradient(mu, lambdaM, i1, dI1dLambda);
    }

    /// <summary>
    /// Nominal (engineering) stress for equibiaxial loading.
    /// Deformation: lambda1 = lambda2 = lambda, lambda3 = 1/lambda^2 (incompressible).
    /// I1 = 2*lambda^2 + 1/lambda^4
    /// </summary>
    public static double BiaxialStress(double lambda, double mu, double lambdaM)
    {
      if (lambda <= 0.0) throw new ArgumentOutOfRangeException(nameof(lambda));

      double i1 = 2.0 * lambda * lambda + 1.0 / Math.Pow(lambda, 4);
      double dI1dLambda = 4.0 * lambda - 4.0 / Math.Pow(lambda, 5);

      return NominalStress(mu, lambdaM, i1, dI1dLambda);
    }

    /// <summary>
    /// Gradient (d(sigma)/d(mu), d(sigma)/d(lambdaM)) of the equibiaxial nominal stress,
    /// for use as the Jacobian in a nonlinear least-squares fit.
    /// </summary>
    public static (double dSigmaDMu, double dSigmaDLambdaM) BiaxialStressGradient(
        double lambda, double mu, double lambdaM)
    {
      if (lambda <= 0.0) throw new ArgumentOutOfRangeException(nameof(lambda));

      double i1 = 2.0 * lambda * lambda + 1.0 / Math.Pow(lambda, 4);
      double dI1dLambda = 4.0 * lambda - 4.0 / Math.Pow(lambda, 5);

      return NominalStressGradient(mu, lambdaM, i1, dI1dLambda);
    }


    /// <summary>
    /// Nominal (engineering) stress for pure shear (planar tension).
    /// Deformation: lambda1 = lambda, lambda2 = 1, lambda3 = 1/lambda (incompressible).
    /// I1 = lambda^2 + 1 + 1/lambda^2
    /// </summary>
    public static double PureShearStress(double lambda, double mu, double lambdaM)
    {
      if (lambda <= 0.0) throw new ArgumentOutOfRangeException(nameof(lambda));

      double i1 = lambda * lambda + 1.0 + 1.0 / (lambda * lambda);
      double dI1dLambda = 2.0 * lambda - 2.0 / Math.Pow(lambda, 3);

      return NominalStress(mu, lambdaM, i1, dI1dLambda);
    }

    /// <summary>
    /// Gradient (d(sigma)/d(mu), d(sigma)/d(lambdaM)) of the pure shear nominal stress,
    /// for use as the Jacobian in a nonlinear least-squares fit.
    /// </summary>
    public static (double dSigmaDMu, double dSigmaDLambdaM) PureShearStressGradient(
        double lambda, double mu, double lambdaM)
    {
      if (lambda <= 0.0) throw new ArgumentOutOfRangeException(nameof(lambda));

      double i1 = lambda * lambda + 1.0 + 1.0 / (lambda * lambda);
      double dI1dLambda = 2.0 * lambda - 2.0 / Math.Pow(lambda, 3);

      return NominalStressGradient(mu, lambdaM, i1, dI1dLambda);
    }
  }
}

using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization.TrustRegion.Subproblems
{
  /// <summary>
  /// Solves the trust-region subproblem using the dog-leg strategy.
  /// </summary>
  internal class DogLegSubproblem : ITrustRegionSubproblem
  {
    /// <inheritdoc />
    public Vector<double> Pstep { get; private set; }

    /// <inheritdoc />
    public bool HitBoundary { get; private set; }

    /// <inheritdoc />
    public void Solve(IObjectiveModel objective, double radius)
    {
      var Gradient = objective.Gradient;
      var Hessian = objective.Hessian;

      // newton point, the Gauss–Newton step by solving the normal equations
      var Pgn = -Hessian.PseudoInverse() * Gradient; // Hessian.Solve(Gradient) fails so many times...

      // cauchy point, steepest descent direction is given by
      var alpha = Gradient.DotProduct(Gradient) / (Hessian * Gradient).DotProduct(Gradient);
      var Psd = -alpha * Gradient;

      // update step and prectted reduction
      if (Pgn.L2Norm() <= radius)
      {
        // Pgn is inside trust region radius
        HitBoundary = false;
        Pstep = Pgn;
      }
      else if (alpha * Psd.L2Norm() >= radius)
      {
        // Psd is outside trust region radius
        HitBoundary = true;
        Pstep = radius / Psd.L2Norm() * Psd;
      }
      else
      {
        // Pstep is intersection of the trust region boundary
        HitBoundary = true;
        var beta = Util.FindBeta(alpha, Psd, Pgn, radius).Item2;
        Pstep = alpha * Psd + beta * (Pgn - alpha * Psd);
      }
    }
  }
}

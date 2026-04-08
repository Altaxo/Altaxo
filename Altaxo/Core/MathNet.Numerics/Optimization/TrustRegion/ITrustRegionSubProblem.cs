using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization.TrustRegion
{
  /// <summary>
  /// Defines a trust-region subproblem solver.
  /// </summary>
  public interface ITrustRegionSubproblem
  {
    /// <summary>
    /// Gets the computed trust-region step.
    /// </summary>
    public Vector<double> Pstep { get; }
    /// <summary>
    /// Gets a value indicating whether the trust-region boundary was hit.
    /// </summary>
    public bool HitBoundary { get; }

    /// <summary>
    /// Solves the trust-region subproblem.
    /// </summary>
    /// <param name="objective">The objective model.</param>
    /// <param name="radius">The trust-region radius.</param>
    public void Solve(IObjectiveModel objective, double radius);
  }
}

using Altaxo.Calc.Optimization.TrustRegion.Subproblems;

namespace Altaxo.Calc.Optimization.TrustRegion
{
  /// <summary>
  /// Provides factory methods for trust-region subproblem solvers.
  /// </summary>
  public static class TrustRegionSubproblem
  {
    /// <summary>
    /// Creates a dogleg trust-region subproblem solver.
    /// </summary>
    /// <returns>The dogleg subproblem solver.</returns>
    public static ITrustRegionSubproblem DogLeg()
    {
      return new DogLegSubproblem();
    }

    /// <summary>
    /// Creates a Newton-CG trust-region subproblem solver.
    /// </summary>
    /// <returns>The Newton-CG subproblem solver.</returns>
    public static ITrustRegionSubproblem NewtonCG()
    {
      return new NewtonCGSubproblem();
    }
  }
}

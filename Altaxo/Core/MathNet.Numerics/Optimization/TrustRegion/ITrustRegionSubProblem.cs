using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization.TrustRegion
{
  public interface ITrustRegionSubproblem
  {
    Vector<double> Pstep { get; }
    bool HitBoundary { get; }

    void Solve(IObjectiveModel objective, double radius);
  }
}

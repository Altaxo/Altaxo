using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization.TrustRegion
{
  public interface ITrustRegionSubproblem
  {
    public Vector<double> Pstep { get; }
    public bool HitBoundary { get; }

    public void Solve(IObjectiveModel objective, double radius);
  }
}

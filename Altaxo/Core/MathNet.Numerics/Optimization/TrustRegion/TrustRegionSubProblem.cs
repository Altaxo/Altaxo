using Altaxo.Calc.Optimization.TrustRegion.Subproblems;

namespace Altaxo.Calc.Optimization.TrustRegion
{
  public static class TrustRegionSubproblem
  {
    public static ITrustRegionSubproblem DogLeg()
    {
      return new DogLegSubproblem();
    }

    public static ITrustRegionSubproblem NewtonCG()
    {
      return new NewtonCGSubproblem();
    }
  }
}

namespace Altaxo.Calc.Optimization.TrustRegion
{
  /// <summary>
  /// Minimizes nonlinear least-squares objectives with a trust-region Newton-CG method.
  /// </summary>
  public sealed class TrustRegionNewtonCGMinimizer : TrustRegionMinimizerBase
  {
    /// <summary>
    /// Non-linear least square fitting by the trust region Newton-Conjugate-Gradient algorithm.
    /// </summary>
    /// <summary>
    /// Initializes a new instance of the <see cref="TrustRegionNewtonCGMinimizer"/> class.
    /// </summary>
    /// <param name="gradientTolerance">The stopping threshold for the gradient norm.</param>
    /// <param name="stepTolerance">The stopping threshold for the parameter step norm.</param>
    /// <param name="functionTolerance">The stopping threshold for the function value.</param>
    /// <param name="radiusTolerance">The stopping threshold for the trust-region radius.</param>
    /// <param name="maximumIterations">The maximum number of iterations.</param>
    public TrustRegionNewtonCGMinimizer(double gradientTolerance = 1E-8, double stepTolerance = 1E-8, double functionTolerance = 1E-8, double radiusTolerance = 1E-8, int maximumIterations = -1)
        : base(TrustRegionSubproblem.NewtonCG(), gradientTolerance, stepTolerance, functionTolerance, radiusTolerance, maximumIterations)
    { }
  }
}

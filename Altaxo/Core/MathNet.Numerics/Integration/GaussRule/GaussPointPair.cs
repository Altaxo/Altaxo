namespace Altaxo.Calc.Integration.GaussRule
{
  /// <summary>
  /// Contains two GaussPoint.
  /// </summary>
  internal class GaussPointPair
  {
    /// <summary>
    /// Gets the order of the primary quadrature rule.
    /// </summary>
    internal int Order { get; }

    /// <summary>
    /// Gets the abscissas of the primary quadrature rule.
    /// </summary>
    internal double[] Abscissas { get; }

    /// <summary>
    /// Gets the weights of the primary quadrature rule.
    /// </summary>
    internal double[] Weights { get; }

    /// <summary>
    /// Gets the order of the secondary quadrature rule.
    /// </summary>
    internal int SecondOrder { get; }

    /// <summary>
    /// Gets the abscissas of the secondary quadrature rule.
    /// </summary>
    internal double[] SecondAbscissas { get; }

    /// <summary>
    /// Gets the weights of the secondary quadrature rule.
    /// </summary>
    internal double[] SecondWeights { get; }

    /// <summary>
    /// Gets the beginning of the integration interval.
    /// </summary>
    internal double IntervalBegin { get; }

    /// <summary>
    /// Gets the end of the integration interval.
    /// </summary>
    internal double IntervalEnd { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GaussPointPair"/> class.
    /// </summary>
    /// <param name="intervalBegin">The beginning of the integration interval.</param>
    /// <param name="intervalEnd">The end of the integration interval.</param>
    /// <param name="order">The order of the primary quadrature rule.</param>
    /// <param name="abscissas">The abscissas of the primary quadrature rule.</param>
    /// <param name="weights">The weights of the primary quadrature rule.</param>
    /// <param name="secondOrder">The order of the secondary quadrature rule.</param>
    /// <param name="secondAbscissas">The abscissas of the secondary quadrature rule.</param>
    /// <param name="secondWeights">The weights of the secondary quadrature rule.</param>
    internal GaussPointPair(double intervalBegin, double intervalEnd, int order, double[] abscissas, double[] weights, int secondOrder, double[] secondAbscissas, double[] secondWeights)
    {
      IntervalBegin = intervalBegin;
      IntervalEnd = intervalEnd;
      Order = order;
      Abscissas = abscissas;
      Weights = weights;
      SecondOrder = secondOrder;
      SecondAbscissas = secondAbscissas;
      SecondWeights = secondWeights;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GaussPointPair"/> class on the default interval [-1, 1].
    /// </summary>
    /// <param name="order">The order of the primary quadrature rule.</param>
    /// <param name="abscissas">The abscissas of the primary quadrature rule.</param>
    /// <param name="weights">The weights of the primary quadrature rule.</param>
    /// <param name="secondOrder">The order of the secondary quadrature rule.</param>
    /// <param name="secondWeights">The weights of the secondary quadrature rule.</param>
    internal GaussPointPair(int order, double[] abscissas, double[] weights, int secondOrder, double[] secondWeights)
        : this(-1, 1, order, abscissas, weights, secondOrder, null, secondWeights)
    { }
  }
}

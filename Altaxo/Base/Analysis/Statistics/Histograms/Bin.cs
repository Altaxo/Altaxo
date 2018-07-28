using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Analysis.Statistics.Histograms
{
  /// <summary>
  /// Represents one element of the binning collection.
  /// </summary>
  public struct Bin
  {
    private double _lowerBound;
    private double _upperBound;
    private double _centerPosition;
    private int _count;

    /// <summary>
    /// Gets the width of the bin. i.e. the difference between the <see cref="UpperBound"/> and the <see cref="LowerBound"/> of the bin.
    /// </summary>
    /// <value>
    /// The width of the bin.
    /// </value>
    public double Width { get { return _upperBound - _lowerBound; } }

    /// <summary>
    /// Gets the lower boundary of the bin.
    /// </summary>
    /// <value>
    /// The lower boundary of the bin.
    /// </value>
    public double LowerBound { get { return _lowerBound; } }

    /// <summary>
    /// Gets the upper boundary of the bin.
    /// </summary>
    /// <value>
    /// The upper boundary of the bin.
    /// </value>
    public double UpperBound { get { return _upperBound; } }

    /// <summary>
    /// Gets the center position of the bin.
    /// </summary>
    /// <value>
    /// The center position of the bin.
    /// </value>
    public double CenterPosition { get { return _centerPosition; } }

    /// <summary>
    /// Gets the number of ensenble data that were sorted into this bin.
    /// </summary>
    /// <value>
    /// The number of data in this bin.
    /// </value>
    public int ValueCount { get { return _count; } }

    /// <summary>
    /// Initializes a new instance of the <see cref="Bin"/> struct.
    /// </summary>
    /// <param name="lowerBound">The lower boundary of the bin.</param>
    /// <param name="centerPosition">The center position of the bin.</param>
    /// <param name="upperBound">The upper boundary of the bin.</param>
    /// <param name="count">The number of data in this bin.</param>
    public Bin(double lowerBound, double centerPosition, double upperBound, int count)
    {
      _lowerBound = lowerBound;
      _centerPosition = centerPosition;
      _upperBound = upperBound;
      _count = count;
    }
  }
}

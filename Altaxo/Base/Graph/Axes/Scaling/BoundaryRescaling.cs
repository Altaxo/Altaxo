using System;

namespace Altaxo.Graph.Axes.Scaling
{
  /// <summary>
  /// Denotes what happens with one side of an axis when the data are changed.
  /// </summary>
  public enum BoundaryRescaling
  {
    /// <summary>
    /// Scale this boundary so that the data fits.
    /// </summary>
    Auto = 0,

    /// <summary>
    /// This axis boundary is set to a fixed value.
    /// </summary>
    Fixed = 1,
    /// <summary>
    /// This axis boundary is set to a fixed value.
    /// </summary>
    Equal = 1,

    /// <summary>
    /// The axis boundary is set to fit the data, but is set not greater than a certain value.
    /// </summary>
    NotGreater  = 2,
    /// <summary>
    /// The axis boundary is set to fit the data, but is set not greater than a certain value.
    /// </summary>
    LessOrEqual = 2,

    /// <summary>
    /// The axis boundary is set to fit the data, but is set not lesser than a certain value.
    /// </summary>
    GreaterOrEqual = 3,

    /// <summary>
    /// The axis boundary is set to fit the data, but is set not lesser than a certain value.
    /// </summary>
    NotLess = 3,

  }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph
{
  /// <summary>
  /// Enumerates the sides of an axis line.
  /// </summary>
  [Serializable]
  public enum CSAxisSide
  {
    /// <summary>Direction to lower logical values of the first alternate axis.</summary>
    FirstDown = 0,
    /// <summary>Direction to higher logical values of the first alternate axis.</summary>
    FirstUp = 1,
    /// <summary>Direction to lower logical values of the second alternate axis.</summary>
    SecondDown = 2,
    /// <summary>Direction to higher logical values of the second alternate axis.</summary>
    SecondUp = 3
  };
}
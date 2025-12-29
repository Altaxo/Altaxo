using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Interface defining how to scale the differences between real quantities (dependent variables)
  /// and fitted values.
  /// </summary>
  public interface IVarianceScaling : ICloneable
  {
    /// <summary>
    /// Gets the weight depending on the real data (roughly speaking: inverse of variance).
    /// </summary>
    /// <param name="yreal">The real (measured) data.</param>
    /// <param name="i">The index of the measured data point in the table.</param>
    /// <returns>
    /// The weight used to scale the fit difference (<c>yreal - yfit</c>). If a variance is given for the
    /// current data point, return <c>1 / variance</c>.
    /// </returns>
    double GetWeight(double yreal, int i);

    /// <summary>
    /// Gets a short name for the scaling method.
    /// </summary>
    /// <remarks>
    /// Used to display this short name in the fit function dialog box.
    /// </remarks>
    string ShortName { get; }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Interface to how to scale the differences between real quantities (dependent variables) and fitted values.
  /// </summary>
  public interface IVarianceScaling : ICloneable
  {
    /// <summary>
    /// Gets the weight in dependence of the real data (roughly spoken: inverse of variance).
    /// </summary>
    /// <param name="yreal">The real (measured) data.</param>
    /// <param name="i">The index of the measured data point in the table.</param>
    /// <returns>The weight used to scale the fit difference (yreal-yfit). In case a variance is given for the current data,
    /// you should return (1/variance). </returns>
    double GetWeight(double yreal, int i);

    /// <summary>
    /// Returns a short name for the scaling method. Used to display this short name in
    /// the fit function dialog box.
    /// </summary>
    string ShortName { get; }
  }
}

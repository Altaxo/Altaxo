using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Data.Transformations
{
  /// <summary>
  /// Transforms a double value to another double value.
  /// </summary>
  /// <seealso cref="Altaxo.Data.IVariantToVariantTransformation" />
  public interface IDoubleToDoubleTransformation : IVariantToVariantTransformation
  {
    /// <summary>
    /// Transforms the specified y value.
    /// </summary>
    /// <param name="y">The y value.</param>
    /// <returns>The transformed y value.</returns>
    double Transform(double y);

    /// <summary>
    /// Evaluates the derivative of the transformed y value.
    /// </summary>
    /// <param name="y">The y value.</param>
    /// <param name="dydx">The derivative of y with respect to x.</param>
    /// <returns>The transformed y value and the derivative of the transformed y value with respect to x.</returns>
    (double ytrans, double dydxtrans) Derivative(double y, double dydx);
  }
}

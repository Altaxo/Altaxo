using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Complex64T = System.Numerics.Complex;

namespace Altaxo.Calc
{
  /// <summary>
  /// Provides extension methods for <see cref="System.Numerics.Complex"/>.
  /// </summary>
  public static class SystemNumericsComplexExtensions
  {
    /// <summary>
    /// Gets the complex conjugate of <paramref name="z"/>.
    /// </summary>
    /// <param name="z">The complex value.</param>
    /// <returns>The complex conjugate of <paramref name="z"/>.</returns>
    public static Complex64T GetConjugate(this Complex64T z)
    {
      return new Complex64T(z.Real, -z.Imaginary);
    }

    /// <summary>
    /// Calculates <c>z²</c>.
    /// </summary>
    /// <param name="z">The complex value.</param>
    /// <returns><paramref name="z"/> squared.</returns>
    public static Complex64T Pow2(this Complex64T z)
    {
      return z * z;
    }

    /// <summary>
    /// Calculates <c>z³</c>.
    /// </summary>
    /// <param name="z">The complex value.</param>
    /// <returns><paramref name="z"/> cubed.</returns>
    public static Complex64T Pow3(this Complex64T z)
    {
      return z * z * z;
    }
  }
}

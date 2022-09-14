using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Complex64T = System.Numerics.Complex;

namespace Altaxo.Calc
{
  public static class SystemNumericsComplexExtensions
  {
    public static Complex64T GetConjugate(this Complex64T z)
    {
      return new Complex64T(z.Real, -z.Imaginary);
    }

    public static Complex64T Pow2(this Complex64T z)
    {
      return z * z;
    }
    public static Complex64T Pow3(this Complex64T z)
    {
      return z * z * z;
    }
  }
}

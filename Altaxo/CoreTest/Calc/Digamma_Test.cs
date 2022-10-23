using Xunit;
using Complex64 = System.Numerics.Complex;

namespace Altaxo.Calc
{
  public class Digamma_Test
  {
    private ((double Re, double Im) Arg, (double Re, double Im) Res)[] _testValues = new ((double Re, double Im) Arg, (double Re, double Im) Res)[]
      {
        ((0.5, 0), (-1.96351002602142347944098, 0)),
        ((0.5, 1), (-0.05176165099441254279260, 1.56494051781587928263812)),
        ((0.5, 1000), (6.90775523731546309371680, 1.57079632679489661923132)),
        ((2, 0), (0.422784335098467139393488, 0)),
        ((2, 3), (1.20798071071015088078664, 1.10412968058757620966198)),
        ((2, 1000), (6.90775636231447871972461, 1.56929632779489561923232 )),
        ((1000, 0), (6.90725519564881205205001, 0)),
        ((1000, 3), (6.90725970013077163941413, 0.00300149148653962131255 )),
        ((1000, 1000), (7.25407886926210762342926, 0.78564820506411497628183)),
      };

    [Fact]
    public void TestDigamma()
    {
      foreach (var testPairs in _testValues)
      {
        var z = new Complex64(testPairs.Arg.Re, testPairs.Arg.Im);
        var result = SpecialDigamma.Digamma(z);

        AssertEx.AreEqual(testPairs.Res.Re, result.Real, 1E-16, 1E-14);
        AssertEx.AreEqual(testPairs.Res.Im, result.Imaginary, 1E-16, 1E-14);

        z = new Complex64(testPairs.Arg.Re, -testPairs.Arg.Im);
        result = SpecialDigamma.Digamma(z);
        AssertEx.AreEqual(testPairs.Res.Re, result.Real, 1E-16, 1E-14);
        AssertEx.AreEqual(-testPairs.Res.Im, result.Imaginary, 1E-16, 1E-14);

      }
    }
  }
}

#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
//
//    The source code of this source code file (and only of this source file) is licensed under the MIT licence:
//    Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
//    to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
//    and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
//    DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using Xunit;
using Complex64 = System.Numerics.Complex;


namespace Altaxo.Calc
{
  public class SpecialFunctions_Hypotenuse_Tests
  {
    [Fact]
    public void TestHypotenuseDouble()
    {
      const double TwoToThePowerOfMinus25 = 2.98023223876953125E-8;
      const double SqrtOnePlusTwoToThePowerOfMinus50 = 1.000000000000000444089210;

      Assert.True(double.IsNaN(SpecialFunctions.Hypotenuse(double.NaN, 0d)));
      Assert.True(double.IsNaN(SpecialFunctions.Hypotenuse(0d, double.NaN)));
      Assert.True(double.IsNaN(SpecialFunctions.Hypotenuse(double.NaN, double.NaN)));
      Assert.True(double.IsPositiveInfinity(SpecialFunctions.Hypotenuse(double.NegativeInfinity, 0d)));
      Assert.True(double.IsPositiveInfinity(SpecialFunctions.Hypotenuse(0d, double.NegativeInfinity)));
      Assert.True(double.IsPositiveInfinity(SpecialFunctions.Hypotenuse(double.NegativeInfinity, double.NegativeInfinity)));
      Assert.True(double.IsPositiveInfinity(SpecialFunctions.Hypotenuse(double.PositiveInfinity, 0d)));
      Assert.True(double.IsPositiveInfinity(SpecialFunctions.Hypotenuse(0d, double.NegativeInfinity)));
      Assert.True(double.IsPositiveInfinity(SpecialFunctions.Hypotenuse(double.PositiveInfinity, double.PositiveInfinity)));
      Assert.True(double.IsPositiveInfinity(SpecialFunctions.Hypotenuse(double.NegativeInfinity, double.PositiveInfinity)));
      Assert.True(double.IsPositiveInfinity(SpecialFunctions.Hypotenuse(double.PositiveInfinity, double.NegativeInfinity)));
      Assert.Equal(0, SpecialFunctions.Hypotenuse(0d, 0d));
      Assert.Equal(double.Epsilon, SpecialFunctions.Hypotenuse(double.Epsilon, 0d));
      Assert.Equal(double.Epsilon, SpecialFunctions.Hypotenuse(0d, double.Epsilon));
      Assert.Equal(7, SpecialFunctions.Hypotenuse(7d, 0d));
      Assert.Equal(7, SpecialFunctions.Hypotenuse(0d, 7d));
      Assert.Equal(double.MaxValue, SpecialFunctions.Hypotenuse(double.MaxValue, 0d));
      Assert.Equal(double.MaxValue, SpecialFunctions.Hypotenuse(0d, double.MaxValue));
      Assert.Equal(-double.MinValue, SpecialFunctions.Hypotenuse(double.MinValue, 0d));
      Assert.Equal(-double.MinValue, SpecialFunctions.Hypotenuse(0d, double.MinValue));
      Assert.Equal(5, SpecialFunctions.Hypotenuse(3d, 4d));
      Assert.Equal(5, SpecialFunctions.Hypotenuse(4d, 3d));
      Assert.Equal(SqrtOnePlusTwoToThePowerOfMinus50, SpecialFunctions.Hypotenuse(1, TwoToThePowerOfMinus25));
      Assert.Equal(SqrtOnePlusTwoToThePowerOfMinus50, SpecialFunctions.Hypotenuse(TwoToThePowerOfMinus25, 1));
    }

    [Fact]
    public void TestHypotenuseSingle()
    {
      const float TwoToThePowerOfMinus11 = 0.00048828125f;
      const float SqrtOnePlusTwoToThePowerOfMinus22 = 1.000000119209282445354739f;

      Assert.True(float.IsNaN(SpecialFunctions.Hypotenuse(float.NaN, 0f)));
      Assert.True(float.IsNaN(SpecialFunctions.Hypotenuse(0f, float.NaN)));
      Assert.True(float.IsNaN(SpecialFunctions.Hypotenuse(float.NaN, float.NaN)));
      Assert.True(float.IsPositiveInfinity(SpecialFunctions.Hypotenuse(float.NegativeInfinity, 0f)));
      Assert.True(float.IsPositiveInfinity(SpecialFunctions.Hypotenuse(0, float.NegativeInfinity)));
      Assert.True(float.IsPositiveInfinity(SpecialFunctions.Hypotenuse(float.NegativeInfinity, float.NegativeInfinity)));
      Assert.True(float.IsPositiveInfinity(SpecialFunctions.Hypotenuse(float.PositiveInfinity, 0f)));
      Assert.True(float.IsPositiveInfinity(SpecialFunctions.Hypotenuse(0, float.NegativeInfinity)));
      Assert.True(float.IsPositiveInfinity(SpecialFunctions.Hypotenuse(float.PositiveInfinity, float.PositiveInfinity)));
      Assert.True(float.IsPositiveInfinity(SpecialFunctions.Hypotenuse(float.NegativeInfinity, float.PositiveInfinity)));
      Assert.True(float.IsPositiveInfinity(SpecialFunctions.Hypotenuse(float.PositiveInfinity, float.NegativeInfinity)));
      Assert.Equal(0, SpecialFunctions.Hypotenuse(0f, 0f));
      Assert.Equal(float.Epsilon, SpecialFunctions.Hypotenuse(float.Epsilon, 0f));
      Assert.Equal(float.Epsilon, SpecialFunctions.Hypotenuse(0f, float.Epsilon));
      Assert.Equal(7, SpecialFunctions.Hypotenuse(7f, 0f));
      Assert.Equal(7, SpecialFunctions.Hypotenuse(0f, 7f));
      Assert.Equal(float.MaxValue, SpecialFunctions.Hypotenuse(float.MaxValue, 0f));
      Assert.Equal(float.MaxValue, SpecialFunctions.Hypotenuse(0f, float.MaxValue));
      Assert.Equal(-float.MinValue, SpecialFunctions.Hypotenuse(float.MinValue, 0f));
      Assert.Equal(-float.MinValue, SpecialFunctions.Hypotenuse(0f, float.MinValue));
      Assert.Equal(5, SpecialFunctions.Hypotenuse(3f, 4f));
      Assert.Equal(5, SpecialFunctions.Hypotenuse(4f, 3f));
      Assert.Equal(SqrtOnePlusTwoToThePowerOfMinus22, SpecialFunctions.Hypotenuse(1, TwoToThePowerOfMinus11));
      Assert.Equal(SqrtOnePlusTwoToThePowerOfMinus22, SpecialFunctions.Hypotenuse(TwoToThePowerOfMinus11, 1));
    }

    [Fact]
    public void TestComplex64Magnitude()
    {
      const double TwoToThePowerOfMinus25 = 2.98023223876953125E-8;
      const double SqrtOnePlusTwoToThePowerOfMinus50 = 1.000000000000000444089210;

      Assert.True(double.IsNaN(new Complex64(double.NaN, 0).Magnitude));
      Assert.True(double.IsNaN(new Complex64(0, double.NaN).Magnitude));
      Assert.True(double.IsNaN(new Complex64(double.NaN, double.NaN).Magnitude));
      Assert.True(double.IsPositiveInfinity(new Complex64(double.NegativeInfinity, 0).Magnitude));
      Assert.True(double.IsPositiveInfinity(new Complex64(0, double.NegativeInfinity).Magnitude));
      Assert.True(double.IsPositiveInfinity(new Complex64(double.NegativeInfinity, double.NegativeInfinity).Magnitude));
      Assert.True(double.IsPositiveInfinity(new Complex64(double.PositiveInfinity, 0).Magnitude));
      Assert.True(double.IsPositiveInfinity(new Complex64(0, double.NegativeInfinity).Magnitude));
      Assert.True(double.IsPositiveInfinity(new Complex64(double.PositiveInfinity, double.PositiveInfinity).Magnitude));
      Assert.True(double.IsPositiveInfinity(new Complex64(double.NegativeInfinity, double.PositiveInfinity).Magnitude));
      Assert.True(double.IsPositiveInfinity(new Complex64(double.PositiveInfinity, double.NegativeInfinity).Magnitude));
      Assert.Equal(0, new Complex64(0, 0).Magnitude);
      Assert.Equal(double.Epsilon, new Complex64(double.Epsilon, 0).Magnitude);
      Assert.Equal(double.Epsilon, new Complex64(0, double.Epsilon).Magnitude);
      Assert.Equal(7, new Complex64(7, 0).Magnitude);
      Assert.Equal(7, new Complex64(0, 7).Magnitude);
      Assert.Equal(double.MaxValue, new Complex64(double.MaxValue, 0).Magnitude);
      Assert.Equal(double.MaxValue, new Complex64(0, double.MaxValue).Magnitude);
      Assert.Equal(-double.MinValue, new Complex64(double.MinValue, 0).Magnitude);
      Assert.Equal(-double.MinValue, new Complex64(0, double.MinValue).Magnitude);
      Assert.Equal(5, new Complex64(3, 4).Magnitude);
      Assert.Equal(5, new Complex64(4, 3).Magnitude);
      Assert.Equal(SqrtOnePlusTwoToThePowerOfMinus50, new Complex64(1, TwoToThePowerOfMinus25).Magnitude);
      Assert.Equal(SqrtOnePlusTwoToThePowerOfMinus50, new Complex64(TwoToThePowerOfMinus25, 1).Magnitude);
    }
  }
}

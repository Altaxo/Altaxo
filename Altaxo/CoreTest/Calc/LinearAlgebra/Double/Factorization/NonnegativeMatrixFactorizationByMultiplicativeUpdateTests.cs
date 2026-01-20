#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion
using Xunit;

namespace Altaxo.Calc.LinearAlgebra.Double.Factorization
{
  // Test matrices see https://resources.wolframcloud.com/FunctionRepository/resources/NonNegativeMatrixFactorization/
  // https://github.com/trigeorgis/Deep-Semi-NMF/blob/master/matlab/NNDSVD.m

  public class NonnegativeMatrixFactorizationByMultiplicativeUpdateTests : FactorizationTestBase
  {
    [Fact]
    public void Test()
    {
      var originalLoadings = GetThreeSpectra();
      var originalScores = GetScores3D(NumberOfSpectra);
      var originalMatrix = originalScores * originalLoadings;
      var matrixX = originalMatrix.Clone();
      var nmf = new NonnegativeMatrixFactorizationByMultiplicativeUpdate()
      {
        InitializationMethod = new NNDSVDa(),
        NumberOfAdditionalTrials = 5,
        MaximumNumberOfIterations = 200,
      };
      var (mfactors, mloads) = nmf.FactorizeOneTrial(matrixX, NumberOfComponents);
      var relError = RelativeError(mfactors, mloads, originalMatrix);
      Assert.True(relError < 0.022);
    }

    [Fact]
    public void CanHandleDifferentScales()
    {
      foreach (var scale in new double[] { 1, 1e40 })
      {
        var originalLoadings = GetThreeSpectra();
        var originalScores = GetScores3D(NumberOfSpectra);
        var originalMatrix = originalScores * originalLoadings * scale;
        var matrixX = originalMatrix.Clone();

        var nmf = new NonnegativeMatrixFactorizationByMultiplicativeUpdate()
        {
          InitializationMethod = new NNDSVDa(),
          NumberOfAdditionalTrials = 5,
          MaximumNumberOfIterations = 200,
        };
        var (mfactors, mloads) = nmf.FactorizeOneTrial(matrixX, NumberOfComponents);
        var relError = RelativeError(mfactors, mloads, originalMatrix);
        Assert.True(relError < 0.022);
      }
    }
  }
}


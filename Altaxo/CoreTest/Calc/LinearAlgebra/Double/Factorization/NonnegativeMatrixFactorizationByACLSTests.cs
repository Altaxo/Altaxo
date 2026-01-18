#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

#endregion Copyright

using Xunit;

namespace Altaxo.Calc.LinearAlgebra.Double.Factorization
{
  public class NonnegativeMatrixFactorizationByACLSTests : FactorizationTestBase
  {
    [Fact]
    public void Test()
    {
      var originalLoadings = GetThreeSpectra();
      var originalScores = GetScores3D(NumberOfSpectra);
      var originalMatrix = originalScores * originalLoadings;
      var matrixX = originalMatrix.Clone();

      var nmf = new NonnegativeMatrixFactorizationByACLS() { InitializationMethod = new NNDSVDa() };
      var (mfactors, mloads) = nmf.FactorizeOneTrial(matrixX, 3);
      var relError = RelativeError(mfactors, mloads, originalMatrix);
      Assert.True(relError < 0.0035);
    }

    [Fact]
    public void CanHandleDifferentScalesForNNSVDaInitialization()
    {
      foreach (var scale in new double[] { 1, 1e40, 1e-40 })
      {
        var originalLoadings = GetThreeSpectra();
        var originalScores = GetScores3D(NumberOfSpectra);
        var originalMatrix = originalScores * originalLoadings * scale;
        var matrixX = originalMatrix.Clone();

        var nmf = new NonnegativeMatrixFactorizationByACLS() { InitializationMethod = new NNDSVDa() };
        var (mfactors, mloads) = nmf.FactorizeOneTrial(matrixX, 3);
        var relError = RelativeError(mfactors, mloads, originalMatrix);
        Assert.True(relError < 0.0035);
      }
    }

    [Fact]
    public void CanHandleDifferentScalesForNNDSVDarInitialization()
    {
      foreach (var scale in new double[] { 1, 1e40, 1e-40 })
      {
        var originalLoadings = GetThreeSpectra();
        var originalScores = GetScores3D(NumberOfSpectra);
        var originalMatrix = originalScores * originalLoadings * scale;
        var matrixX = originalMatrix.Clone();

        var nmf = new NonnegativeMatrixFactorizationByACLS() { InitializationMethod = new NNDSVDar() };
        var (mfactors, mloads) = nmf.FactorizeOneTrial(matrixX, 3);
        var relError = RelativeError(mfactors, mloads, originalMatrix);
        Assert.True(relError < 0.022);
      }
    }

    [Fact]
    public void CanHandleDifferentScalesForRandomInitialization()
    {
      foreach (var scale in new double[] { 1, 1e40, 1e-40 })
      {
        var originalLoadings = GetThreeSpectra();
        var originalScores = GetScores3D(NumberOfSpectra);
        var originalMatrix = originalScores * originalLoadings * scale;
        var matrixX = originalMatrix.Clone();

        var nmf = new NonnegativeMatrixFactorizationByACLS() { InitializationMethod = new NMFInitializationRandom() };
        var (mfactors, mloads) = nmf.FactorizeOneTrial(matrixX, 3);
        var relError = RelativeError(mfactors, mloads, originalMatrix);
        Assert.True(relError < 0.022);
      }
    }


  }
}

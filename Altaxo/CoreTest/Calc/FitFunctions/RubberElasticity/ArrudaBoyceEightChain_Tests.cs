using Altaxo.Calc.LinearAlgebra;
using Xunit;

namespace Altaxo.Calc.FitFunctions.RubberElasticity
{
  public class ArrudaBoyceEightChain_Tests
  {
    [Fact]
    public void TestBiaxial()
    {

      var lambda = 3;
      var G = 13;
      var lambdaM = 7;
      var expectedFunctionValue = 168.75581601770347514;
      var expectedDerivativeWrtG = 12.981216616746421164;
      var expectedDerivativeWrtLambdaM = -4.2024229943945278002;

      // test function value y of one term
      var v = new ArrudaBoyceEightChainBiaxial();
      var y = ArrudaBoyceEightChainBiaxial.Evaluate(lambda - 1, G, lambdaM);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 1e-14);


      var parameters = new double[] { G, lambdaM };

      var X = Matrix<double>.Build.Dense(1, 1);
      X[0, 0] = lambda - 1;

      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 2);

      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtG, DY[0, 0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtLambdaM, DY[0, 1], 0, 1e-14);
    }

    [Fact]
    public void TestPlanar()
    {

      var lambda = 3;
      var G = 13;
      var lambdaM = 7;
      var expectedFunctionValue = 80.438388540457666284;
      var expectedDerivativeWrtG = 6.1875683492659743295;
      var expectedDerivativeWrtLambdaM = -1.04014528865992076485;

      // test function value y of one term
      var v = new ArrudaBoyceEightChainPlanar();
      var y = ArrudaBoyceEightChainPlanar.Evaluate(lambda - 1, G, lambdaM);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 1e-14);


      var parameters = new double[] { G, lambdaM };

      var X = Matrix<double>.Build.Dense(1, 1);
      X[0, 0] = lambda - 1;

      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 2);

      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtG, DY[0, 0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtLambdaM, DY[0, 1], 0, 1e-14);
    }

    [Fact]
    public void TestUniaxial()
    {

      var lambda = 3;
      var G = 13;
      var lambdaM = 7;
      var expectedFunctionValue = 78.271890703356687203;
      var expectedDerivativeWrtG = 6.0209146694889759387;
      var expectedDerivativeWrtLambdaM = -0.96356285843549942155;

      // test function value y of one term
      var v = new ArrudaBoyceEightChainUniaxial();
      var y = ArrudaBoyceEightChainUniaxial.Evaluate(lambda - 1, G, lambdaM);
      AssertEx.AreEqual(expectedFunctionValue, y, 0, 1e-14);


      var parameters = new double[] { G, lambdaM };

      var X = Matrix<double>.Build.Dense(1, 1);
      X[0, 0] = lambda - 1;

      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 2);

      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtG, DY[0, 0], 0, 1e-14);
      AssertEx.AreEqual(expectedDerivativeWrtLambdaM, DY[0, 1], 0, 1e-14);
    }

  }
}

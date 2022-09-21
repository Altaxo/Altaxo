using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression.Nonlinear
{
  internal class WrapperToFitFunction
  {
    public IFitFunction FitFunction { get; private set; }

    private double[] _x = new double[1];
    private double[] _y = new double[1];

    public WrapperToFitFunction(IFitFunction f)
    {
      FitFunction = f;
    }

    public Vector<double> Evaluate(Vector<double> parameter, Vector<double> x)
    {
      var yR = Vector<double>.Build.Dense(x.Count);
      FitFunction.EvaluateMultiple(MatrixMath.ToROMatrixWithOneColumn(x), parameter, null, yR);
      return yR;
    }

    public Matrix<double> EvaluateDerivative(Vector<double> parameter, Vector<double> x)
    {
      var yR = Matrix<double>.Build.Dense(x.Count, parameter.Count);
      ((IFitFunctionWithGradient)FitFunction).EvaluateGradient(MatrixMath.ToROMatrixWithOneColumn(x), parameter, null, yR);
      return yR;
    }
  }
}

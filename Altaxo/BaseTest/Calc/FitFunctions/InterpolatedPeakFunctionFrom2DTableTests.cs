using System;
using Altaxo.Calc.FitFunctions.Peaks;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Data;

namespace Altaxo.Calc.FitFunctions
{
  public class InterpolatedPeakFunctionFrom2DTableTests
  {
    public static (IFitFunctionWithDerivative fitFunction, Func<double, double, double, double, (double z, double dzdheight, double dzdposition, double dzdwidth)> expectedFunction, double[] x, double[] widths, Matrix<double> z)
      GetFitFunctionAndExpected_WidthDependent()
    {
      (var fitFunction, var expectedFunction, var x, var widths, var matrix) = InterpolatedPeakFunctionFromMatrixTests.GetFitFunctionAndExpected_WidthDependent();



      var table = new DataTable();
      table.Name = "Table";

      var widthsColumn = table.PropertyColumns.EnsureExistence("Widths", typeof(DoubleColumn), ColumnKind.X, 0);

      table.DataColumns.EnsureExistence("X", typeof(DoubleColumn), ColumnKind.X, 0).Data = x;

      for (int i = 0; i < widths.Length; i++)
      {
        var yCol = table.DataColumns.EnsureExistence($"Width{i}", typeof(DoubleColumn), ColumnKind.V, 0);

        yCol.Data = matrix.Row(i).ToArray();
        widthsColumn[table.DataColumns.GetColumnNumber(yCol)] = widths[i];
      }

      var superFitFunction = new InterpolatedPeakFunctionFrom2DTable(1, -1, table, 0, "Widths", propertyIsPeakWidth: true);
      return (superFitFunction, expectedFunction, x, widths, matrix);
    }

    public static IFitFunctionWithDerivative GetFitFunctionOnly_WidthDependent()
    {
      return GetFitFunctionAndExpected_WidthDependent().fitFunction;
    }
  }
}

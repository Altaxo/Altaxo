#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

#nullable enable
using System;
using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression;
using Altaxo.Data;
using Altaxo.Gui.Worksheet;
using Altaxo.Gui.Worksheet.Viewing;

namespace Altaxo.Worksheet.Commands.Analysis
{
  /// <summary>
  /// Summary description for CalculusCommands.
  /// </summary>
  public class CalculusCommands
  {
    #region SavitzkyGolay

    public static void SavitzkyGolayFiltering(IWorksheetController ctrl)
    {
      if (ctrl.SelectedDataColumns.Count == 0)
        return;

      object paramobject = new SavitzkyGolayParameters();

      if (!Current.Gui.ShowDialog(ref paramobject, "Savitzky-Golay parameters"))
        return;

      var parameters = (SavitzkyGolayParameters)paramobject;

      var yCol = ctrl.DataTable.DataColumns[ctrl.SelectedDataColumns[0]];
      var xCol = ctrl.DataTable.DataColumns.FindXColumnOf(yCol);

      SavitzkyGolay(parameters, yCol, xCol);
    }

    public static void SavitzkyGolay(SavitzkyGolayParameters parameters, Altaxo.Data.DataColumn yCol)
    {
      SavitzkyGolay(parameters, yCol, null);
    }

    public static void SavitzkyGolay(SavitzkyGolayParameters parameters, Altaxo.Data.DataColumn yCol, Altaxo.Data.DataColumn? xCol)
    {
      double spacing = 1;
      if (xCol is Data.INumericColumn)
      {
        var calcspace = new Calc.LinearAlgebra.VectorSpacingEvaluator(Calc.LinearAlgebra.DataColumnWrapper.ToROVector(xCol));
        if (!calcspace.HasValidSpaces || calcspace.HasInvalidSpaces)
        {
          Current.Gui.ErrorMessageBox(string.Format("The x-column {0} contains invalid spaces (is not equally spaced)", xCol.Name));
          return;
        }
        if (calcspace.RelativeSpaceDeviation > 1E-2)
        {
          if (!Current.Gui.YesNoMessageBox(
            string.Format("The x-column {0} is not equally spaced, the deviation is {1}, the mean spacing is {2}. Continue anyway?", xCol.Name, calcspace.RelativeSpaceDeviation, calcspace.SpaceMeanValue),
            "Continue?", true))
            return;
        }

        spacing = calcspace.SpaceMeanValue;
      }

      var filter = new SavitzkyGolay(parameters);

      using (var suspendToken = yCol.SuspendGetToken())
      {
        filter.Apply(DataColumnWrapper.ToROVectorCopy(yCol), DataColumnWrapper.ToVector(yCol));

        if (parameters.DerivativeOrder > 0)
        {
          double factor = Math.Pow(1 / spacing, parameters.DerivativeOrder) * Calc.GammaRelated.Fac(parameters.DerivativeOrder);
          yCol.Data = yCol * factor;
        }
        suspendToken.Dispose();
      }
    }

    #endregion SavitzkyGolay

    #region Interpolation

    public static void Interpolation(IWorksheetController ctrl)
    {
      if (ctrl.SelectedDataColumns.Count == 0)
        return;

      object paramobject = new InterpolationParameters();

      if (!Current.Gui.ShowDialog(ref paramobject, "Interpolation"))
        return;

      var parameters = (InterpolationParameters)paramobject;

      Interpolation(ctrl, parameters);
    }

    public static void Interpolation(IWorksheetController ctrl, InterpolationParameters parameters)
    {
      var _columnToGroupNumber = new Dictionary<DataColumn, int>();

      for (int nSel = 0; nSel < ctrl.SelectedDataColumns.Count; nSel++)
      {
        var yCol = ctrl.DataTable.DataColumns[ctrl.SelectedDataColumns[nSel]];
        var xCol = ctrl.DataTable.DataColumns.FindXColumnOf(yCol);

        if (!(yCol is INumericColumn))
        {
          Current.Gui.ErrorMessageBox("The selected column is not numeric!");
          return;
        }
        if (!(xCol is INumericColumn))
        {
          Current.Gui.ErrorMessageBox("The x-column of the selected column is not numeric!");
          return;
        }

        var xRes = new DoubleColumn();
        var yRes = new DoubleColumn();
        if (!_columnToGroupNumber.TryGetValue(xCol, out var newgroup))
        {
          newgroup = ctrl.DataTable.DataColumns.GetUnusedColumnGroupNumber();
          ctrl.DataTable.DataColumns.Add(xRes, xCol.Name + ".I", ColumnKind.X, newgroup);
          _columnToGroupNumber.Add(xCol, newgroup);
        }
        ctrl.DataTable.DataColumns.Add(yRes, yCol.Name + ".I", ColumnKind.V, newgroup);

        Interpolation(xCol, yCol, parameters, xRes, yRes);
      }
    }

    public static void Interpolation(Altaxo.Data.DataColumn xCol, Altaxo.Data.DataColumn yCol,
    InterpolationParameters parameters,
    Altaxo.Data.DataColumn xRes, Altaxo.Data.DataColumn yRes)
    {
      Interpolation(
        xCol, yCol,
        parameters.InterpolationInstance,
        VectorMath.CreateEquidistantSequenceByStartEndLength(parameters.XOrg, parameters.XEnd, parameters.NumberOfPoints),
        xRes, yRes);
    }

    public static void Interpolation(Altaxo.Data.DataColumn xCol, Altaxo.Data.DataColumn yCol,
      Calc.Interpolation.IInterpolationFunction interpolInstance, IReadOnlyList<double> samplePoints,
      Altaxo.Data.DataColumn xRes, Altaxo.Data.DataColumn yRes)
    {
      int rows = Math.Min(xCol.Count, yCol.Count);
      var yVec = DataColumnWrapper.ToROVector((INumericColumn)yCol, rows);
      var xVec = DataColumnWrapper.ToROVector((INumericColumn)xCol, rows);

      interpolInstance.Interpolate(xVec, yVec);

      using (var suspendToken_xRes = xRes.SuspendGetToken())
      {
        using (var suspendToken_yRes = yRes.SuspendGetToken())
        {
          for (int i = 0; i < samplePoints.Count; i++)
          {
            //double r = i / (double)(parameters.NumberOfPoints - 1);
            //double x = parameters.XOrg * (1 - r) + parameters.XEnd * (r);
            double x = samplePoints[i];
            double y = interpolInstance.GetYOfX(x);
            xRes[i] = x;
            yRes[i] = y;
          }
          suspendToken_yRes.Resume();
        }
        suspendToken_xRes.Resume();
      }
    }

    #endregion Interpolation

    #region Multivariate linear fit

    public static LinearFitBySvd MultivariateLinearFit(IWorksheetController ctrl)
    {
      return Calc.Regression.Multivariate.MultivariateLinearRegression.ShowDialogAndRegress(ctrl.DataTable.DataColumns, ctrl.SelectedDataColumns);
    }

    #endregion Multivariate linear fit
  }
}

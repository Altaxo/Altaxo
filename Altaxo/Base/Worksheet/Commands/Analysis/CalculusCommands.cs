#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;

using Altaxo.Worksheet.GUI;
using Altaxo.Calc;
using Altaxo.Calc.Regression;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Interpolation;
using Altaxo.Calc.Probability;
using Altaxo.Data;
using Altaxo.Collections;

namespace Altaxo.Worksheet.Commands.Analysis
{
  /// <summary>
  /// Summary description for CalculusCommands.
  /// </summary>
  public class CalculusCommands
  {

    #region SavitzkyGolay

    public static void SavitzkyGolayFiltering(WorksheetController ctrl)
    {
      if(ctrl.SelectedDataColumns.Count==0)
        return;

      object paramobject = new SavitzkyGolayParameters();

      if(!Current.Gui.ShowDialog(ref paramobject,"Savitzky-Golay parameters"))
        return;

      SavitzkyGolayParameters parameters = (SavitzkyGolayParameters)paramobject;

      Altaxo.Data.DataColumn yCol = ctrl.Doc.DataColumns[ctrl.SelectedDataColumns[0]];
      Altaxo.Data.DataColumn xCol = ctrl.Doc.DataColumns.FindXColumnOf(yCol);

      double spacing = 1;
      if(xCol is Data.INumericColumn)
      {
        Calc.LinearAlgebra.VectorSpacingEvaluator calcspace = new Calc.LinearAlgebra.VectorSpacingEvaluator(Calc.LinearAlgebra.DataColumnWrapper.ToROVector(xCol));
        if(!calcspace.HasValidSpaces || calcspace.HasInvalidSpaces)
        {
          Current.Gui.ErrorMessageBox(string.Format("The x-column {0} contains invalid spaces (is not equally spaced)",xCol.Name));
          return;
        }
        if(calcspace.RelativeSpaceDeviation>1E-2)
        {
          System.Windows.Forms.DialogResult dlgresult = 
            System.Windows.Forms.MessageBox.Show(Current.MainWindow,
            string.Format("The x-column {0} is not equally spaced, the deviation is {1}, the mean spacing is {2}. Continue anyway?", xCol.Name, calcspace.RelativeSpaceDeviation, calcspace.SpaceMeanValue),
            "Continue?",System.Windows.Forms.MessageBoxButtons.YesNo,
            System.Windows.Forms.MessageBoxIcon.Question,
            System.Windows.Forms.MessageBoxDefaultButton.Button1);

          if(dlgresult==System.Windows.Forms.DialogResult.No)
            return;
        }

        spacing = calcspace.SpaceMeanValue;
      }

      Calc.Regression.SavitzkyGolay filter = new SavitzkyGolay(parameters);

      yCol.Suspend();
      filter.Apply(DataColumnWrapper.ToROVectorCopy(yCol),DataColumnWrapper.ToVector(yCol));

      if(parameters.DerivativeOrder>0)
      {
        double factor = Math.Pow(1/spacing,parameters.DerivativeOrder)*Calc.GammaRelated.Fac(parameters.DerivativeOrder);
        yCol.Data = yCol*factor; 
      }

      yCol.Resume();
    }

    #endregion

    #region Interpolation

    public static void Interpolation(WorksheetController ctrl)
    {
      if(ctrl.SelectedDataColumns.Count==0)
        return;

      object paramobject = new InterpolationParameters();

      if(!Current.Gui.ShowDialog(ref paramobject,"Interpolation"))
        return;

      InterpolationParameters parameters = (InterpolationParameters)paramobject;


      Altaxo.Data.DataColumn yCol = ctrl.Doc.DataColumns[ctrl.SelectedDataColumns[0]];
      Altaxo.Data.DataColumn xCol = ctrl.Doc.DataColumns.FindXColumnOf(yCol);

      if(!(yCol is INumericColumn))
      {
        Current.Gui.ErrorMessageBox("The selected column is not numeric!");
        return;
      }
      if(!(xCol is INumericColumn))
      {
        Current.Gui.ErrorMessageBox("The x-column of the selected column is not numeric!");
        return;
      }

      int rows = Math.Min(xCol.Count,yCol.Count);
      IROVector yVec = DataColumnWrapper.ToROVector((INumericColumn)yCol,rows);
      IROVector xVec = DataColumnWrapper.ToROVector((INumericColumn)xCol,rows);

      parameters.InterpolationInstance.Interpolate(xVec,yVec);

      DoubleColumn xRes = new DoubleColumn();
      DoubleColumn yRes = new DoubleColumn();

      for(int i=0; i<parameters.NumberOfPoints;i++)
      {
        double r = i/(double)(parameters.NumberOfPoints-1);
        double x = parameters.XOrg*(1-r) + parameters.XEnd*(r);
        double y = ((IInterpolationFunction)parameters.InterpolationInstance).GetYOfX(x);
        xRes[i]=x;
        yRes[i]=y;
      }

      int newgroup = ctrl.DataTable.DataColumns.GetUnusedColumnGroupNumber();
      ctrl.DataTable.DataColumns.Add(xRes,xCol.Name+".I",ColumnKind.X,newgroup);
      ctrl.DataTable.DataColumns.Add(yRes,yCol.Name+".I",ColumnKind.V,newgroup);

    }

    #endregion

    #region Multivariate linear fit

 

    public static LinearFitBySvd MultivariateLinearFit(WorksheetController ctrl)
    {
      return Calc.Regression.Multivariate.MultivariateLinearRegression.ShowDialogAndRegress(ctrl.Doc.DataColumns,ctrl.SelectedDataColumns);
    }



    #endregion
  }
}

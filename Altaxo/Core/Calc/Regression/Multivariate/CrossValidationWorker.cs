#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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

using System;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression.Multivariate
{
  public class CrossValidationWorker
  {
    protected IROVector _xOfX;
    protected int       _numFactors;
  
    protected ICrossValidationGroupingStrategy _groupingStrategy;
    protected SpectralPreprocessingOptions _preprocessOptions;
    protected MultivariateRegression _analysis;
      
    protected IMatrix predY;

    

    public int NumberOfFactors { get { return NumberOfFactors; }}

    public CrossValidationWorker(
      IROVector xOfX,
      int numFactors,
      ICrossValidationGroupingStrategy groupingStrategy,
      SpectralPreprocessingOptions preprocessOptions,
      MultivariateRegression analysis
      )
    {
      _xOfX = xOfX;
      _numFactors = numFactors;
      _groupingStrategy = groupingStrategy;
      _preprocessOptions = preprocessOptions;
      _analysis = analysis;
    }
  }

  public class CrossPRESSEvaluator : CrossValidationWorker
  {
    double[] _crossPRESS;
    public double[] CrossPRESS { get { return _crossPRESS; }}

    public CrossPRESSEvaluator(
      IROVector xOfX,
      int numFactors,
      ICrossValidationGroupingStrategy groupingStrategy,
      SpectralPreprocessingOptions preprocessOptions,
      MultivariateRegression analysis
      )
      : base(xOfX,numFactors,groupingStrategy,preprocessOptions,analysis)
    {
    }

    public void EhCrossPRESS(int[]group, IMatrix XX, IMatrix YY, IMatrix XU, IMatrix YU)
    {
      IVector meanX,scaleX,meanY,scaleY;
      if(predY==null || predY.Rows!=YU.Rows || predY.Columns!=YU.Columns)
        predY = new MatrixMath.BEMatrix(YU.Rows,YU.Columns);

      MultivariateRegression.PreprocessForAnalysis(_preprocessOptions,_xOfX, XX, YY, out meanX, out scaleX, out meanY, out scaleY);
      _analysis.AnalyzeFromPreprocessed(XX,YY,_numFactors);
      _numFactors = Math.Min(_numFactors,_analysis.NumberOfFactors);
   
      MultivariateRegression.PreprocessSpectraForPrediction(_preprocessOptions,XU,meanX,scaleX);


      // allocate the crossPRESS vector here, since now we know about the number of factors a bit more
      if(null==_crossPRESS)
        _crossPRESS = new double[_numFactors+1]; // one more since we want to have the value at factors=0 (i.e. the variance of the y-matrix)

      // for all factors do now a prediction of the remaining spectra
      _crossPRESS[0] += MatrixMath.SumOfSquares(YU);
      for(int nFactor=1;nFactor<=_numFactors;nFactor++)
      {
        _analysis.PredictYFromPreprocessed(XU,nFactor,predY);
        MultivariateRegression.PostprocessY(predY,meanY,scaleY);
        _crossPRESS[nFactor] += MatrixMath.SumOfSquaredDifferences(YU,predY);
      }
    }
  }

  public class CrossPredictedYEvaluator : CrossValidationWorker
  {
    public IMatrix _YCrossValidationPrediction;

    public CrossPredictedYEvaluator(
      IROVector xOfX,
      int numFactors,
      ICrossValidationGroupingStrategy groupingStrategy,
      SpectralPreprocessingOptions preprocessOptions,
      MultivariateRegression analysis,
      IMatrix YCrossValidationPrediction
      )
      : base(xOfX,numFactors,groupingStrategy,preprocessOptions,analysis)
    {
      _YCrossValidationPrediction = YCrossValidationPrediction;
    }
     
    public void EhYCrossPredicted(int[]group, IMatrix XX, IMatrix YY, IMatrix XU, IMatrix YU)
    {
      IVector meanX,scaleX,meanY,scaleY;
      if(predY==null || predY.Rows!=YU.Rows || predY.Columns!=YU.Columns)
        predY = new MatrixMath.BEMatrix(YU.Rows,YU.Columns);

      MultivariateRegression.PreprocessForAnalysis(_preprocessOptions,_xOfX, XX, YY,
        out meanX, out scaleX, out meanY, out scaleY);
        
      _analysis.AnalyzeFromPreprocessed(XX,YY,_numFactors);
      _numFactors = Math.Min(_numFactors,_analysis.NumberOfFactors);

      MultivariateRegression.PreprocessSpectraForPrediction(_preprocessOptions,XU,meanX,scaleX);
      _analysis.PredictYFromPreprocessed(XU,_numFactors,predY);
      MultivariateRegression.PostprocessY(predY,meanY,scaleY);
        
      for(int i=0;i<group.Length;i++)
        MatrixMath.SetRow(predY,i,_YCrossValidationPrediction,group[i]); 
    }
  }
}


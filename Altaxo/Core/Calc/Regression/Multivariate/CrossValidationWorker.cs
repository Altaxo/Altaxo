#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
    protected int[] _spectralRegions;
    protected int       _numFactors;
  
    protected ICrossValidationGroupingStrategy _groupingStrategy;
    protected SpectralPreprocessingOptions _preprocessOptions;
    protected MultivariateRegression _analysis;
      


    

    public int NumberOfFactors { get { return _numFactors; }}

    public CrossValidationWorker(
      int[] spectralRegions,
      int numFactors,
      ICrossValidationGroupingStrategy groupingStrategy,
      SpectralPreprocessingOptions preprocessOptions,
      MultivariateRegression analysis
      )
    {
      _spectralRegions = spectralRegions;
      _numFactors = numFactors;
      _groupingStrategy = groupingStrategy;
      _preprocessOptions = preprocessOptions;
      _analysis = analysis;
    }
  }

  public class CrossPRESSEvaluator : CrossValidationWorker
  {
    protected IMatrix _predictedY;

    double[] _crossPRESS;
    public double[] CrossPRESS { get { return _crossPRESS; }}

    public CrossPRESSEvaluator(
      int[] spectralRegions,
      int numFactors,
      ICrossValidationGroupingStrategy groupingStrategy,
      SpectralPreprocessingOptions preprocessOptions,
      MultivariateRegression analysis
      )
      : base(spectralRegions,numFactors,groupingStrategy,preprocessOptions,analysis)
    {
    }

    public void EhCrossPRESS(int[]group, IMatrix XX, IMatrix YY, IMatrix XU, IMatrix YU)
    {
      IVector meanX,scaleX,meanY,scaleY;
      if(_predictedY==null || _predictedY.Rows!=YU.Rows || _predictedY.Columns!=YU.Columns)
        _predictedY = new MatrixMath.BEMatrix(YU.Rows,YU.Columns);

      MultivariateRegression.PreprocessForAnalysis(_preprocessOptions,_spectralRegions, XX, YY, out meanX, out scaleX, out meanY, out scaleY);
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
        _analysis.PredictYFromPreprocessed(XU,nFactor,_predictedY);
        MultivariateRegression.PostprocessY(_predictedY,meanY,scaleY);
        _crossPRESS[nFactor] += MatrixMath.SumOfSquaredDifferences(YU,_predictedY);
      }
    }
  }

  public class CrossPredictedYEvaluator : CrossValidationWorker
  {
    protected IMatrix _predictedY;
    public IMatrix _YCrossValidationPrediction;

    public CrossPredictedYEvaluator(
      int[] spectralRegions,
      int numFactors,
      ICrossValidationGroupingStrategy groupingStrategy,
      SpectralPreprocessingOptions preprocessOptions,
      MultivariateRegression analysis,
      IMatrix YCrossValidationPrediction
      )
      : base(spectralRegions,numFactors,groupingStrategy,preprocessOptions,analysis)
    {
      _YCrossValidationPrediction = YCrossValidationPrediction;
    }
     
    public void EhYCrossPredicted(int[]group, IMatrix XX, IMatrix YY, IMatrix XU, IMatrix YU)
    {
      IVector meanX,scaleX,meanY,scaleY;
      if(_predictedY==null || _predictedY.Rows!=YU.Rows || _predictedY.Columns!=YU.Columns)
        _predictedY = new MatrixMath.BEMatrix(YU.Rows,YU.Columns);

      MultivariateRegression.PreprocessForAnalysis(_preprocessOptions,_spectralRegions, XX, YY,
        out meanX, out scaleX, out meanY, out scaleY);
        
      _analysis.AnalyzeFromPreprocessed(XX,YY,_numFactors);
      _numFactors = Math.Min(_numFactors,_analysis.NumberOfFactors);

      MultivariateRegression.PreprocessSpectraForPrediction(_preprocessOptions,XU,meanX,scaleX);
      _analysis.PredictYFromPreprocessed(XU,_numFactors,_predictedY);
      MultivariateRegression.PostprocessY(_predictedY,meanY,scaleY);
        
      for(int i=0;i<group.Length;i++)
        MatrixMath.SetRow(_predictedY,i,_YCrossValidationPrediction,group[i]); 
    }
  }

  


  public class CrossPredictedXResidualsEvaluator : CrossValidationWorker
  {
    int _numberOfPoints;
     
    public IMatrix _XCrossResiduals;

    public IROMatrix XCrossResiduals { get { return _XCrossResiduals; }}

    public CrossPredictedXResidualsEvaluator(
      int numberOfPoints,
      int[] spectralRegions,
      int numFactors,
      ICrossValidationGroupingStrategy groupingStrategy,
      SpectralPreprocessingOptions preprocessOptions,
      MultivariateRegression analysis
      )
      : base(spectralRegions,numFactors,groupingStrategy,preprocessOptions,analysis)
    {
      _numberOfPoints = numberOfPoints;
    }
     
    public void EhCrossValidationWorker(int[]group, IMatrix XX, IMatrix YY, IMatrix XU, IMatrix YU)
    {
      IVector meanX,scaleX,meanY,scaleY;

      MultivariateRegression.PreprocessForAnalysis(_preprocessOptions,_spectralRegions, XX, YY,
        out meanX, out scaleX, out meanY, out scaleY);
        
      _analysis.AnalyzeFromPreprocessed(XX,YY,_numFactors);
      _numFactors = Math.Min(_numFactors,_analysis.NumberOfFactors);

      MultivariateRegression.PreprocessSpectraForPrediction(_preprocessOptions,XU,meanX,scaleX);
      IROMatrix xResidual = _analysis.SpectralResidualsFromPreprocessed(XU,_numFactors);
     

      
      if(this._XCrossResiduals==null)
        this._XCrossResiduals = new MatrixMath.BEMatrix(_numberOfPoints,xResidual.Columns);
  
      for(int i=0;i<group.Length;i++)
        MatrixMath.SetRow(xResidual,i,this._XCrossResiduals,group[i]); 
    }
  }


  public class CrossValidationResultEvaluator : CrossValidationWorker
  {
  
    CrossValidationResult _result;

    protected IMatrix _predictedY;
    protected IMatrix _spectralResidual;


    public CrossValidationResultEvaluator(
      int[] spectralRegions,
      int numFactors,
      ICrossValidationGroupingStrategy groupingStrategy,
      SpectralPreprocessingOptions preprocessOptions,
      MultivariateRegression analysis,
      CrossValidationResult result
      )
      : base(spectralRegions,numFactors,groupingStrategy,preprocessOptions,analysis)
    {
      _result = result;
    }
     
    public void EhCrossValidationWorker(int[]group, IMatrix XX, IMatrix YY, IMatrix XU, IMatrix YU)
    {
      IVector meanX,scaleX,meanY,scaleY;

      MultivariateRegression.PreprocessForAnalysis(_preprocessOptions,_spectralRegions, XX, YY,
        out meanX, out scaleX, out meanY, out scaleY);
        
      _analysis.AnalyzeFromPreprocessed(XX,YY,_numFactors);
      _numFactors = Math.Min(_numFactors,_analysis.NumberOfFactors);

      MultivariateRegression.PreprocessSpectraForPrediction(_preprocessOptions,XU,meanX,scaleX);

      if(_predictedY==null || _predictedY.Rows!=YU.Rows || _predictedY.Columns!=YU.Columns)
        _predictedY = new MatrixMath.BEMatrix(YU.Rows,YU.Columns);
      if(_spectralResidual==null || _spectralResidual.Rows!=XU.Rows || _spectralResidual.Columns!=_analysis.NumberOfSpectralResiduals)
        _spectralResidual = new MatrixMath.BEMatrix(XU.Rows,_analysis.NumberOfSpectralResiduals);



      for(int nFactor=0;nFactor<=_numFactors;nFactor++)
      {
        _analysis.PredictedYAndSpectralResidualsFromPreprocessed(XU,nFactor,_predictedY,_spectralResidual);
        MultivariateRegression.PostprocessY(_predictedY,meanY,scaleY);
        
        for(int i=0;i<group.Length;i++)
        {
          MatrixMath.SetRow(_predictedY,i,_result.GetPredictedYW(NumberOfFactors),group[i]); 
          MatrixMath.SetRow(_spectralResidual,i,_result.GetSpectralResidualW(NumberOfFactors),group[i]);
        }
      }
    }
  }
}


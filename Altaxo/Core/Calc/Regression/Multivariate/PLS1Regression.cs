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
	/// <summary>
	/// Summary description for PLS1Regression.
	/// </summary>
	public class PLS1Regression : MultivariateRegression
	{
    PLS1CalibrationModel _calib;

   

    protected override MultivariateCalibrationModel InternalCalibrationModel { get { return _calib; }}

    public override void SetCalibrationModel(IMultivariateCalibrationModel calib)
    {
      if(calib is PLS1CalibrationModel)
        _calib = (PLS1CalibrationModel) calib;
      else
        throw new ArgumentException("Expecting argument of type PLS1CalibrationModel, but actual type is " + calib.GetType().ToString());
    }


    public override int NumberOfSpectralResiduals { get { return _calib.NumberOfY; }}


    public override void Reset()
    {
      _calib = new PLS1CalibrationModel();
      base.Reset();
    }

    /// <summary>
    /// Creates an analyis from preprocessed spectra and preprocessed concentrations.
    /// </summary>
    /// <param name="matrixX">The spectral matrix (each spectrum is a row in the matrix). They must at least be centered.</param>
    /// <param name="matrixY">The matrix of concentrations (each experiment is a row in the matrix). They must at least be centered.</param>
    /// <param name="maxFactors">Maximum number of factors for analysis.</param>
    /// <returns>A regression object, which holds all the loads and weights neccessary for further calculations.</returns>
    public static PLS1Regression CreateFromPreprocessed(IROMatrix matrixX, IROMatrix matrixY, int maxFactors)
    {
      PLS1Regression result = new PLS1Regression();
      result.AnalyzeFromPreprocessed(matrixX,matrixY,maxFactors);
      return result;
    }

    /// <summary>
    /// Creates an analyis from preprocessed spectra and preprocessed concentrations.
    /// </summary>
    /// <param name="matrixX">The spectral matrix (each spectrum is a row in the matrix). They must at least be centered.</param>
    /// <param name="matrixY">The matrix of concentrations (each experiment is a row in the matrix). They must at least be centered.</param>
    /// <param name="maxFactors">Maximum number of factors for analysis.</param>
    /// <returns>A regression object, which holds all the loads and weights neccessary for further calculations.</returns>
    protected override void AnalyzeFromPreprocessedWithoutReset(IROMatrix matrixX, IROMatrix matrixY, int maxFactors)
    {
      int numberOfFactors = _calib.NumberOfFactors = Math.Min(matrixX.Columns, maxFactors);
      IMatrix helperY = new MatrixMath.BEMatrix(matrixY.Rows,1);
      
      for(int i=0;i<matrixY.Columns;i++)
      {
        MatrixMath.Submatrix(matrixY,helperY,0,i);
       
        PLS2Regression r = PLS2Regression.CreateFromPreprocessed(matrixX, helperY, maxFactors);

        IPLS2CalibrationModel cal = r.CalibrationModel;
        _calib.NumberOfFactors = Math.Min(_calib.NumberOfFactors,cal.NumberOfFactors);
        _calib.XLoads[i] = cal.XLoads;
        _calib.YLoads[i] = cal.YLoads;
        _calib.XWeights[i] = cal.XWeights;
        _calib.CrossProduct[i] = cal.CrossProduct;
      }
    }

    /// <summary>
    /// This predicts concentrations of unknown spectra.
    /// </summary>
    /// <param name="XU">Matrix of unknown spectra (preprocessed the same way as the calibration spectra).</param>
    /// <param name="numFactors">Number of factors used for prediction.</param>
    /// <param name="predictedY">On return, holds the predicted y values. (They are centered).</param>
    /// <param name="spectralResiduals">On return, holds the spectral residual values.</param>
    public override void PredictedYAndSpectralResidualsFromPreprocessed(
      IROMatrix XU, // unknown spectrum or spectra,  horizontal oriented
      int numFactors, // number of factors to use for prediction
      IMatrix predictedY, // Matrix of predicted y-values, must be same number of rows as spectra
      IMatrix spectralResiduals // Matrix of spectral residuals, n rows x 1 column, can be zero
        )
    {
      if(numFactors>NumberOfFactors)
        throw new ArgumentOutOfRangeException(string.Format("Required numFactors (={0}) is higher than numFactors of analysis (={1})",numFactors,NumberOfFactors));

      IMatrix helperY = new MatrixMath.BEMatrix(_calib.NumberOfY,1);
      IMatrix helperS = new MatrixMath.BEMatrix(XU.Rows,1);
      for(int i=0;i<_calib.NumberOfY;i++)
      {
        PLS2Regression.Predict(
          XU, // unknown spectrum or spectra,  horizontal oriented
          _calib.XLoads[i], // x-loads matrix
          _calib.YLoads[i], // y-loads matrix
          _calib.XWeights[i], // weighting matrix
          _calib.CrossProduct[i],  // Cross product vector
          numFactors, // number of factors to use for prediction
          helperY, // Matrix of predicted y-values, must be same number of rows as spectra
          helperS // Matrix of spectral residuals, n rows x 1 column, can be zero
          );

        MatrixMath.Copy(helperY,predictedY,0,i);
        MatrixMath.Copy(helperS,spectralResiduals,0,i);
      }
    }

  
	}
}

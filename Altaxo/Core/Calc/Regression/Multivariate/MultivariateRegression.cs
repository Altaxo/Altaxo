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
using Altaxo.Calc;
using Altaxo.Calc.LinearAlgebra;
using System.Xml;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Contains method common for all multivariate analysis.
  /// </summary>
  public abstract class MultivariateRegression
  {


    /// <summary>
    /// Creates an analyis from preprocessed spectra and preprocessed concentrations.
    /// </summary>
    /// <param name="matrixX">The spectral matrix (each spectrum is a row in the matrix). They must at least be centered.</param>
    /// <param name="matrixY">The matrix of concentrations (each experiment is a row in the matrix). They must at least be centered.</param>
    /// <param name="maxFactors">Maximum number of factors for analysis.</param>
    /// <returns>A regression object, which holds all the loads and weights neccessary for further calculations.</returns>
    public abstract void AnalyzeFromPreprocessed(IROMatrix matrixX, IROMatrix matrixY, int maxFactors);
   

    /// <summary>
    /// This predicts concentrations of unknown spectra.
    /// </summary>
    /// <param name="XU">Matrix of unknown spectra (preprocessed the same way as the calibration spectra).</param>
    /// <param name="numFactors">Number of factors used for prediction.</param>
    /// <param name="predictedY">On return, holds the predicted y values. (They are centered).</param>
    public abstract void PredictYFromPreprocessed(
      IROMatrix XU, // unknown spectrum or spectra,  horizontal oriented
      int numFactors, // number of factors to use for prediction
      IMatrix predictedY // Matrix of predicted y-values, must be same number of rows as spectra
      );


    public abstract int NumberOfFactors { get; }


    /// <summary>
    /// Preprocesses the x and y matrices before usage in multivariate calibrations.
    /// </summary>
    /// <param name="preprocessOptions">Information how to preprocess the data.</param>
    /// <param name="xOfX"></param>
    /// <param name="matrixX"></param>
    /// <param name="matrixY"></param>
    /// <param name="meanX"></param>
    /// <param name="scaleX"></param>
    /// <param name="meanY"></param>
    /// <param name="scaleY"></param>
    public static void PreprocessForAnalysis(
      SpectralPreprocessingOptions preprocessOptions,
      IROVector xOfX,
      IMatrix matrixX, 
      IMatrix matrixY,
      out IVector meanX, out IVector scaleX,
      out IVector meanY, out IVector scaleY)
    {
      
      PreprocessSpectraForAnalysis(preprocessOptions,xOfX,matrixX,out meanX, out scaleX);

      PreprocessYForAnalysis(matrixY,out meanY, out scaleY);
    }


    
    /// <summary>
    /// This will process the spectra before analysis in multivariate calibration.
    /// </summary>
    /// <param name="preprocessOptions">Contains the information how to preprocess the spectra.</param>
    /// <param name="xOfX"></param>
    /// <param name="matrixX">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    /// <param name="meanX"></param>
    /// <param name="scaleX"></param>
    public static void PreprocessSpectraForAnalysis(
      SpectralPreprocessingOptions preprocessOptions,
      IROVector xOfX,
      IMatrix matrixX,
      out IVector meanX, out IVector scaleX
      )
    {
      // Before we can apply PLS, we have to center the x and y matrices
      meanX = new MatrixMath.HorizontalVector(matrixX.Columns);
      scaleX = new MatrixMath.HorizontalVector(matrixX.Columns);
      //  MatrixMath.HorizontalVector scaleX = new MatrixMath.HorizontalVector(matrixX.Cols);

      preprocessOptions.IdentifyRegions(xOfX);
      preprocessOptions.Process(matrixX,meanX,scaleX);
    }



    /// <summary>
    /// This will convert the raw spectra (horizontally in matrixX) to preprocessed spectra according to the calibration model.
    /// </summary>
    /// <param name="calib">The calibration model containing the instructions to process the spectra.</param>
    /// <param name="preprocessOptions">Contains the information how to preprocess the spectra.</param>
    /// <param name="matrixX">The matrix of spectra. Each spectrum is a row of the matrix.</param>
    public static void PreprocessSpectraForPrediction(
      IMultivariateCalibrationModel calib, 
      SpectralPreprocessingOptions preprocessOptions,
      IMatrix matrixX)
    {
      preprocessOptions.ProcessForPrediction(matrixX,calib.XMean,calib.XScale);
    }

    /// <summary>
    /// This will convert the raw spectra (horizontally in matrixX) to preprocessed spectra according to the calibration model.
    /// </summary>
    /// <param name="preprocessOptions">Information how to preprocess the spectra.</param>
    /// <param name="matrixX">Matrix of raw spectra. On return, this matrix contains the preprocessed spectra.</param>
    /// <param name="meanX">Mean spectrum.</param>
    /// <param name="scaleX">Scale spectrum.</param>
    public static void PreprocessSpectraForPrediction(
      SpectralPreprocessingOptions preprocessOptions,
      IMatrix matrixX,
      IROVector meanX,
      IROVector scaleX)
    {
      preprocessOptions.ProcessForPrediction(matrixX,meanX,scaleX);
    }


    public static void PreprocessYForAnalysis(IMatrix matrixY,
      out IVector meanY, out IVector scaleY)
    {
      meanY = new MatrixMath.HorizontalVector(matrixY.Columns);
      scaleY = new MatrixMath.HorizontalVector(matrixY.Columns);
      VectorMath.Fill(scaleY,1);
      MatrixMath.ColumnsToZeroMean(matrixY, meanY);
    }

    /// <summary>
    /// This centers and scales the y values in exactly the way it was done in the multivariate analysis.
    /// </summary>
    /// <param name="matrixY">The concentration matrix. Constituents are horizontally oriented, different experiments vertically.</param>
    /// <param name="meanY">Vector of concentration mean values.</param>
    /// <param name="scaleY">Vector of concentration scale values.</param>
    public static void PreprocessYForPrediction(IMatrix matrixY,
      IROVector meanY, IROVector scaleY)
    {
      for(int j=0;j<matrixY.Columns;j++)
      {
        for(int i=0;i<matrixY.Rows;i++)
          matrixY[i,j] = (matrixY[i,j]-meanY[j])*scaleY[j];
      }
    }

    public static void PostprocessY(IMatrix matrixY,
      IROVector meanY, IROVector scaleY)
    {
      for(int j=0;j<matrixY.Columns;j++)
      {
        for(int i=0;i<matrixY.Rows;i++)
          matrixY[i,j] = (matrixY[i,j]/scaleY[j])+meanY[j];
      }
    }

 

    /// <summary>
    /// Function used for cross validation iteration. During cross validation, the original spectral matrix is separated into
    /// a spectral group used for prediction and the remaining calibration spectra. Parameters here:
    /// XX: remaining calibration spectra.
    /// YY: corresponding concentration data.
    /// XU: spectra used for prediction.
    /// YU: corresponding concentration data.
    /// </summary>
    public delegate void CrossValidationIterationFunction(int[]group, IMatrix XX, IMatrix YY, IMatrix XU, IMatrix YU);


    
    public static double CrossValidationIteration(
      IROMatrix X, // matrix of spectra (a spectra is a row of this matrix)
      IROMatrix Y, // matrix of concentrations (a mixture is a row of this matrix)
      ICrossValidationGroupingStrategy groupingStrategy,
      CrossValidationIterationFunction crossFunction
      )
    {

      //      int[][] groups = bExcludeGroups ? new ExcludeGroupsGroupingStrategy().Group(Y) : new ExcludeSingleMeasurementsGroupingStrategy().Group(Y);
      int[][] groups = groupingStrategy.Group(Y);


      IMatrix XX=null; 
      IMatrix YY=null; 
      IMatrix XU=null; 
      IMatrix YU=null; 
      

      for(int nGroup=0 ,prevNumExcludedSpectra = int.MinValue ;nGroup < groups.Length;nGroup++)
      {
        int[] spectralGroup = groups[nGroup];
        int numberOfExcludedSpectraOfGroup = spectralGroup.Length;

        if(prevNumExcludedSpectra != numberOfExcludedSpectraOfGroup)
        {
          XX = new MatrixMath.BEMatrix(X.Rows-numberOfExcludedSpectraOfGroup,X.Columns);
          YY = new MatrixMath.BEMatrix(Y.Rows-numberOfExcludedSpectraOfGroup,Y.Columns);
          XU = new MatrixMath.BEMatrix(numberOfExcludedSpectraOfGroup,X.Columns);
          YU = new MatrixMath.BEMatrix(numberOfExcludedSpectraOfGroup,Y.Columns);
          prevNumExcludedSpectra = numberOfExcludedSpectraOfGroup;
        }


        // build a new x and y matrix with the group information
        // fill XX and YY with values
        for(int i=0,j=0;i<X.Rows;i++)
        {
          if(Array.IndexOf(spectralGroup,i)>=0) // if spectral group contains i
            continue; // Exclude this row from the spectra
          MatrixMath.SetRow(X,i,XX,j);
          MatrixMath.SetRow(Y,i,YY,j);
          j++;
        }

        // fill XU (unknown spectra) with values
        for(int i=0;i<spectralGroup.Length;i++)
        {
          int j = spectralGroup[i];
          MatrixMath.SetRow(X,j,XU,i); // x-unkown (unknown spectra)
          MatrixMath.SetRow(Y,j,YU,i); // y-unkown (unknown concentration)
        }


        // now do the analysis
        crossFunction(spectralGroup,XX,YY,XU,YU);
        

      } // for all groups

      // calculate the mean number of excluded spectras
      return ((double)X.Rows)/groups.Length;
    }






  }
}

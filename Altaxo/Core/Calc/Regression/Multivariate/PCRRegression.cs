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
	/// PCRRegression contains static methods for doing principal component regression analysis and prediction of the data.
	/// </summary>
	public class PCRRegression
	{
	
    public static void ExecuteAnalysis(
      IROMatrix X, // matrix of spectra (a spectra is a row of this matrix)
      IROMatrix Y, // matrix of concentrations (a mixture is a row of this matrix)
      ref int numFactors,
      out IROMatrix xLoads, // out: the loads of the X matrix
      out IROMatrix xScores, // matrix of weighting values
      out IROVector V  // vector of cross products
       )
    {
      IMatrix matrixX = new MatrixMath.BEMatrix(X.Rows,X.Columns);
      MatrixMath.Copy(X,matrixX);
      MatrixMath.SingularValueDecomposition decompose = new MatrixMath.SingularValueDecomposition(matrixX);

      numFactors = Math.Min(numFactors,matrixX.Columns);
      numFactors = Math.Min(numFactors,matrixX.Rows);


      xLoads = JaggedArrayMath.ToTransposedROMatrix(decompose.V,Y.Rows,X.Columns);
      xScores = JaggedArrayMath.ToMatrix(decompose.U,Y.Rows,Y.Rows);
      V       = VectorMath.ToROVector(decompose.Diagonal,numFactors);
    }

    static void CalculatePRESS(
      IROMatrix Y, // matrix of concentrations (a mixture is a row of this matrix)
      IROMatrix xLoads, // out: the loads of the X matrix
      IROMatrix xScores, // matrix of weighting values
      IROVector V,  // vector of cross products
      int maxNumberOfFactors,
      IVector PRESS //vector of Y PRESS values
      )
    {

      IROMatrix U = xScores;
      MatrixMath.BEMatrix UtY  = new MatrixMath.BEMatrix(Y.Rows,Y.Columns);
      MatrixMath.MultiplyFirstTransposed(U,Y,UtY);

      MatrixMath.BEMatrix predictedY  = new MatrixMath.BEMatrix(Y.Rows,Y.Columns);
      MatrixMath.BEMatrix subU  = new MatrixMath.BEMatrix(Y.Rows,1);
      MatrixMath.BEMatrix subY  = new MatrixMath.BEMatrix(Y.Rows,Y.Columns);

      PRESS[0] = MatrixMath.SumOfSquares(Y);

      int numFactors = Math.Min(maxNumberOfFactors,V.Length);

      // now calculate PRESS by predicting the y
      // using yp = U (w*(1/w)) U' y
      // of course w*1/w is the identity matrix, but we use only the first factors, so using a cutted identity matrix
      // we precalculate the last term U'y = UtY
      // and multiplying with one row of U in every factor step, summing up the predictedY 
      for(int nf=0;nf<numFactors;nf++)
      {
        for(int cn=0;cn<Y.Columns;cn++)
        {
          for(int k=0;k<Y.Rows;k++)
            predictedY[k,cn] += U[k,nf]*UtY[nf,cn];
        }
        PRESS[nf+1] = MatrixMath.SumOfSquaredDifferences(Y,predictedY);
      }
    }


    public static void Predict(
      IROMatrix matrixX,
      IROMatrix xLoads,
      IROMatrix yLoads,
      IROMatrix xScores,
      IROVector crossProduct,
      int numberOfFactors,
      IMatrix predictedY,
      IMatrix spectralResiduals)
    {
      int numX = xLoads.Columns;
      int numY = yLoads.Columns;
      int numM = yLoads.Rows;

      MatrixMath.BEMatrix predictionScores  = new MatrixMath.BEMatrix(numX,numY);
      GetPredictionScoreMatrix(xLoads,yLoads,xScores,crossProduct,numberOfFactors,predictionScores);
      MatrixMath.Multiply(matrixX,predictionScores,predictedY);

      if(null!=spectralResiduals)
        GetSpectralResiduals(matrixX,xLoads,yLoads,xScores,crossProduct,numberOfFactors,spectralResiduals);
    }

    public static void GetPredictionScoreMatrix(
      IROMatrix xLoads,
      IROMatrix yLoads,
      IROMatrix xScores,
      IROVector crossProduct,
      int numberOfFactors,
      IMatrix predictionScores)
    {
      int numX = xLoads.Columns;
      int numY = yLoads.Columns;
      int numM = yLoads.Rows;

      MatrixMath.BEMatrix UtY  = new MatrixMath.BEMatrix(xScores.Columns,yLoads.Columns);
      MatrixMath.MultiplyFirstTransposed(xScores,yLoads,UtY);

      MatrixMath.ZeroMatrix(predictionScores);

      for(int nf=0;nf<numberOfFactors;nf++)
      {
          double scale = 1/crossProduct[nf];
        for(int cn=0;cn<numY;cn++)
        {
          for(int k=0;k<numX;k++)
            predictionScores[k,cn] += scale*xLoads[nf,k]*UtY[nf,cn];
        }
      }
    }

    public static void CalculatePRESS(
      IROMatrix yLoads,
      IROMatrix xScores,
      int numberOfFactors,
      out IROVector press)
    {
      int numMeasurements = yLoads.Rows;

       IExtensibleVector PRESS   = VectorMath.CreateExtensibleVector(numberOfFactors+1);
       MatrixMath.BEMatrix UtY  = new MatrixMath.BEMatrix(yLoads.Rows,yLoads.Columns);
      MatrixMath.BEMatrix predictedY  = new MatrixMath.BEMatrix(yLoads.Rows,yLoads.Columns);
      press = PRESS;

      
      MatrixMath.MultiplyFirstTransposed(xScores,yLoads,UtY);



        // now calculate PRESS by predicting the y
        // using yp = U (w*(1/w)) U' y
        // of course w*1/w is the identity matrix, but we use only the first factors, so using a cutted identity matrix
        // we precalculate the last term U'y = UtY
        // and multiplying with one row of U in every factor step, summing up the predictedY 
      PRESS[0] = MatrixMath.SumOfSquares(yLoads);
      for(int nf=0;nf<numberOfFactors;nf++)
        {
          for(int cn=0;cn<yLoads.Columns;cn++)
          {
            for(int k=0;k<yLoads.Rows;k++)
              predictedY[k,cn] += xScores[k,nf]*UtY[nf,cn];
          }
          PRESS[nf+1] = MatrixMath.SumOfSquaredDifferences(yLoads,predictedY);
        }
     
    }

    public static void CalculatePRESS(
      IROMatrix matrixX,
      IROMatrix xLoads,
      IROMatrix yLoads,
      IROMatrix xScores,
      IROVector crossProduct,
      int numberOfFactors,
      out IROVector PRESS)
    {
      IMatrix predictedY = new JaggedArrayMatrix(yLoads.Rows,yLoads.Columns);
      IVector press = VectorMath.CreateExtensibleVector(numberOfFactors+1);
      PRESS = press;

      press[0] = MatrixMath.SumOfSquares(yLoads);
      for(int nf=0;nf<numberOfFactors;nf++)
      {
        Predict(matrixX,xLoads,yLoads,xScores,crossProduct,nf,predictedY,null);
        press[nf+1] = MatrixMath.SumOfSquaredDifferences(yLoads,predictedY);
      }
    }


    public static void GetSpectralResiduals(
      IROMatrix matrixX,
      IROMatrix xLoads,
      IROMatrix yLoads,
      IROMatrix xScores,
      IROVector crossProduct,
      int numberOfFactors,
      IMatrix spectralResiduals)
    {
      int numX = xLoads.Columns;
      int numY = yLoads.Columns;
      int numM = yLoads.Rows;

      MatrixMath.BEMatrix reconstructedSpectra  = new MatrixMath.BEMatrix(matrixX.Rows,matrixX.Columns);
      MatrixMath.ZeroMatrix(reconstructedSpectra);

      for(int nf=0;nf<numberOfFactors;nf++)
      {
        double scale = crossProduct[nf];
        for(int m=0;m<numM;m++)
        {
          for(int k=0;k<numX;k++)
            reconstructedSpectra[m,k] += scale*xScores[m,nf]*xLoads[nf,k];
        }
      }
      for(int m=0;m<numM;m++)
        spectralResiduals[m,0] = MatrixMath.SumOfSquaredDifferences(
          MatrixMath.ToROSubMatrix(matrixX,m,0,1,matrixX.Columns),
          MatrixMath.ToROSubMatrix(reconstructedSpectra,m,0,1,matrixX.Columns));

    }


    


    /// <summary>
    /// Performs a PLS cross validation.
    /// </summary>
    /// <param name="X">Matrix of spectra (a spectra is a row of this matrix).</param>
    /// <param name="Y">Matrix of concentrations (one sample is one row of this matrix).</param>
    /// <param name="numFactors">Maximal number of factors to use for PLS.</param>
    /// <param name="bExcludeGroups">If true, groups of samples with the same Y values are exluded. If false, every single sample is excluded to perform cross validation.</param>
    /// <param name="crossPRESSMatrix">Output: This is a k*1 matrix of resulting PRESS values, k being the max. number of factors.</param>
    /// <param name="meanNumberOfExcludedSpectra">On return, this gives the mean number of spectra that where excluded during the cross validation.</param>
    public static void CrossValidation(
      IROMatrix X, // matrix of spectra (a spectra is a row of this matrix)
      IROMatrix Y, // matrix of concentrations (a mixture is a row of this matrix)
      int numFactors,
      ICrossValidationGroupingStrategy groupingStrategy,
      out IROVector crossPRESSMatrix, // vertical value of PRESS values for the cross validation
      out double meanNumberOfExcludedSpectra
      )
    {
 
      double[] crossPRESS = null;

      


      IMatrix XX=null; 
      IMatrix YY=null; 
      IMatrix XU=null; 
      IMatrix YU=null; 
      IMatrix predY=null; 

      int[][] groups = groupingStrategy.Group(Y);

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
          predY = new MatrixMath.BEMatrix(numberOfExcludedSpectraOfGroup,Y.Columns);
          prevNumExcludedSpectra = numberOfExcludedSpectraOfGroup;
        }


        // build a new x and y matrix with the group information
        // fill XX and YY with values
        for(int i=0,j=0;i<X.Rows;i++)
        {
          if(Array.IndexOf(spectralGroup,i)>=0)  // if spectral group contains i
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

        // do a PLS with the builded matrices
        IROMatrix xLoads;
        IROMatrix xScores;
        IROVector V;
        int actnumfactors=numFactors;


        ExecuteAnalysis(XX, YY, ref actnumfactors, out xLoads, out xScores, out V); 
        numFactors = Math.Min(numFactors,actnumfactors); // if we have here a lesser number of factors, use it for all further calculations
                            

        // allocate the crossPRESS vector here, since now we know about the number of factors a bit more
        if(null==crossPRESS)
          crossPRESS = new double[numFactors+1]; // one more since we want to have the value at factors=0 (i.e. the variance of the y-matrix)

        // for all factors do now a prediction of the remaining spectra
        crossPRESS[0] += MatrixMath.SumOfSquares(YU);
        for(int nFactor=1;nFactor<=numFactors;nFactor++)
        {
          Predict(XU,xLoads,YY,xScores,V,nFactor,predY,null);
          crossPRESS[nFactor] += MatrixMath.SumOfSquaredDifferences(YU,predY);
        }
      } // for all groups


      // copy the resulting crossPRESS values into a matrix
      crossPRESSMatrix = VectorMath.ToROVector(crossPRESS,numFactors+1);

      // calculate the mean number of excluded spectras
      meanNumberOfExcludedSpectra = ((double)X.Rows)/groups.Length;
    }

	}
}

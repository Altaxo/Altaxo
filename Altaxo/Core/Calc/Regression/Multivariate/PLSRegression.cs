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
  /// PLSRegression contains static methods for doing partial least squares regression analysis and prediction of the data.
  /// </summary>
  public class PLSRegression
  {
  
    /// <summary>
    /// Partial least squares (PLS) decomposition of the matrizes X and Y.
    /// </summary>
    /// <param name="_X">The X ("spectrum") matrix.</param>
    /// <param name="_Y">The Y ("concentration") matrix.</param>
    /// <param name="numFactors">Number of factors to calculate.</param>
    /// <param name="xLoads">Returns the matrix of eigenvectors of X. Should be initially empty.</param>
    /// <param name="yLoads">Returns the matrix of eigenvectors of Y. Should be initially empty. </param>
    /// <param name="W">Returns the matrix of weighting values. Should be initially empty.</param>
    /// <param name="V">Returns the vector of cross products. Should be initially empty.</param>
    /// <param name="PRESS">If not null, the PRESS value of each factor is stored (vertically) here. </param>
    public static void ExecuteAnalysis(
      IROMatrix _X, // matrix of spectra (a spectra is a row of this matrix)
      IROMatrix _Y, // matrix of concentrations (a mixture is a row of this matrix)
      ref int numFactors,
      IBottomExtensibleMatrix xLoads, // out: the loads of the X matrix
      IBottomExtensibleMatrix yLoads, // out: the loads of the Y matrix
      IBottomExtensibleMatrix W, // matrix of weighting values
      IRightExtensibleMatrix V,  // matrix of cross products
      IExtensibleVector PRESS //vector of Y PRESS values
      )
    {
      // used variables:
      // n: number of spectra (number of tests, number of experiments)
      // p: number of slots (frequencies, ..) in each spectrum
      // m: number of constitutents (number of y values in each measurement)
      
      // X : n-p matrix of spectra (each spectra is a horizontal row)
      // Y : n-m matrix of concentrations


      const int maxIterations = 1500; // max number of iterations in one factorization step
      const double accuracy = 1E-12; // accuracy that should be reached between subsequent calculations of the u-vector



      // use the mean spectrum as first row of the W matrix
      MatrixMath.HorizontalVector mean = new MatrixMath.HorizontalVector(_X.Columns);
      //  MatrixMath.ColumnsToZeroMean(X,mean);
      //W.AppendBottom(mean);

      IMatrix X = new MatrixMath.BEMatrix(_X.Rows,_X.Columns);
      MatrixMath.Copy(_X,X);
      IMatrix Y = new MatrixMath.BEMatrix(_Y.Rows,_Y.Columns);
      MatrixMath.Copy(_Y,Y);

      IMatrix u_prev = null;
      IMatrix w = new MatrixMath.HorizontalVector(X.Columns); // horizontal vector of X (spectral) weighting
      IMatrix t = new MatrixMath.VerticalVector(X.Rows); // vertical vector of X  scores
      IMatrix u = new MatrixMath.VerticalVector(X.Rows); // vertical vector of Y scores
      IMatrix p = new MatrixMath.HorizontalVector(X.Columns); // horizontal vector of X loads
      IMatrix q = new MatrixMath.HorizontalVector(Y.Columns); // horizontal vector of Y loads

      int maxFactors = Math.Min(X.Columns,X.Rows);
      numFactors = numFactors<=0 ? maxFactors : Math.Min(numFactors,maxFactors);

      if(PRESS!=null)
      {
        PRESS.Append(new MatrixMath.Scalar(MatrixMath.SumOfSquares(Y))); // Press value for not decomposed Y
      }

      for(int nFactor=0; nFactor<numFactors; nFactor++)
      {
        //Console.WriteLine("Factor_{0}:",nFactor);
        //Console.WriteLine("X:"+X.ToString());
        //Console.WriteLine("Y:"+Y.ToString());

  
        // 1. Use as start vector for the y score the first column of the 
        // y-matrix
        MatrixMath.Submatrix(Y,u); // u is now a vertical vector of concentrations of the first constituents

        for(int iter=0;iter<maxIterations;iter++)
        {
          // 2. Calculate the X (spectrum) weighting vector
          MatrixMath.MultiplyFirstTransposed(u,X,w); // w is a horizontal vector

          // 3. Normalize w to unit length
          MatrixMath.NormalizeRows(w); // w now has unit length

          // 4. Calculate X (spectral) scores
          MatrixMath.MultiplySecondTransposed(X,w,t); // t is a vertical vector of n numbers

          // 5. Calculate the Y (concentration) loading vector
          MatrixMath.MultiplyFirstTransposed(t,Y,q); // q is a horizontal vector of m (number of constitutents)

          // 5.1 Normalize q to unit length
          MatrixMath.NormalizeRows(q);

          // 6. Calculate the Y (concentration) score vector u
          MatrixMath.MultiplySecondTransposed(Y,q,u); // u is a vertical vector of n numbers

          // 6.1 Compare
          // Compare this with the previous one 
          if(u_prev!=null && MatrixMath.IsEqual(u_prev,u,accuracy))
            break;
          if(u_prev==null)
            u_prev = new MatrixMath.VerticalVector(X.Rows);
          MatrixMath.Copy(u,u_prev); // stores the content of u in u_prev
        } // for all iterations

        // Store the scores of X
        //factors.AppendRight(t);


        // 7. Calculate the inner scalar (cross product)
        double length_of_t = MatrixMath.LengthOf(t); 
        MatrixMath.Scalar v = new MatrixMath.Scalar(0);
        MatrixMath.MultiplyFirstTransposed(u,t,v);
        if(length_of_t!=0)
          v = v/MatrixMath.Square(length_of_t); 
      
        // 8. Calculate the new loads for the X (spectral) matrix
        MatrixMath.MultiplyFirstTransposed(t,X,p); // p is a horizontal vector of loads
        // Normalize p by the spectral scores

        if(length_of_t!=0)
          MatrixMath.MultiplyScalar(p,1/MatrixMath.Square(length_of_t),p);

        // 9. Calculate the new residua for the X (spectral) and Y (concentration) matrix
        //MatrixMath.MultiplyScalar(t,length_of_t*v,t); // original t times the cross product

        MatrixMath.SubtractProductFromSelf(t,p,X);
        
        MatrixMath.MultiplyScalar(t,v,t); // original t times the cross product
        MatrixMath.SubtractProductFromSelf(t,q,Y); // to calculate residual Y

        // Store the loads of X and Y in the output result matrix
        xLoads.AppendBottom(p);
        yLoads.AppendBottom(q);
        W.AppendBottom(w);
        V.AppendRight(v);
    
        if(PRESS!=null)
        {
          double pressValue=MatrixMath.SumOfSquares(Y);
          PRESS.Append(new MatrixMath.Scalar(pressValue));
        }
        // Calculate SEPcv. If SEPcv is greater than for the actual number of factors,
        // break since the optimal number of factors was found. If not, repeat the calculations
        // with the residual matrizes for the next factor.
      } // for all factors
    }


    public static void Predict(
      IROMatrix XU, // unknown spectrum or spectra,  horizontal oriented
      IROMatrix xLoads, // x-loads matrix
      IROMatrix yLoads, // y-loads matrix
      IROMatrix W, // weighting matrix
      IROMatrix V,  // Cross product vector
      int numFactors, // number of factors to use for prediction
      IMatrix predictedY, // Matrix of predicted y-values, must be same number of rows as spectra
      IMatrix spectralResiduals // Matrix of spectral residuals, n rows x 1 column, can be zero
      )
    {

      // now predicting a "unkown" spectra
      MatrixMath.Scalar si = new MatrixMath.Scalar(0);
      MatrixMath.HorizontalVector Cu = new MatrixMath.HorizontalVector(yLoads.Columns);

      MatrixMath.HorizontalVector wi = new MatrixMath.HorizontalVector(XU.Columns);
      MatrixMath.HorizontalVector cuadd = new MatrixMath.HorizontalVector(yLoads.Columns);
      
      // xu holds a single spectrum extracted out of XU
      MatrixMath.HorizontalVector xu = new MatrixMath.HorizontalVector(XU.Columns);

      // xl holds temporarily a row of the xLoads matrix+
      MatrixMath.HorizontalVector xl = new MatrixMath.HorizontalVector(xLoads.Columns);


      int maxFactors = Math.Min(yLoads.Rows,numFactors);
      

      for(int nSpectrum=0;nSpectrum<XU.Rows;nSpectrum++)
      {
        MatrixMath.Submatrix(XU,xu,nSpectrum,0); // extract one spectrum to predict
        MatrixMath.ZeroMatrix(Cu); // Set Cu=0
        for(int i=0;i<maxFactors;i++)
        {
          //1. Calculate the unknown spectral score for a weighting vector
          MatrixMath.Submatrix(W,wi,i,0);
          MatrixMath.MultiplySecondTransposed(wi,xu,si);
          // take the y loading vector
          MatrixMath.Submatrix(yLoads,cuadd,i,0);
          // and multiply it with the cross product and the score
          MatrixMath.MultiplyScalar(cuadd,si*V[0,i],cuadd);
          // Add it to the predicted y-values
          MatrixMath.Add(Cu,cuadd,Cu);
          // remove the spectral contribution of the factor from the spectrum
          // TODO this is quite ineffective: in every loop we extract the xl vector, we have to find a shortcut for this!
          MatrixMath.Submatrix(xLoads,xl,i,0);
          MatrixMath.SubtractProductFromSelf(xl,(double)si,xu);
        }
        // xu now contains the spectral residual,
        // Cu now contains the predicted y values
        MatrixMath.SetRow(Cu,0,predictedY,nSpectrum);

        if(null!=spectralResiduals)
        {
          spectralResiduals[nSpectrum,0] = MatrixMath.SumOfSquares(xu);
        }

      } // for each spectrum in XU
    } // end partial-least-squares-predict


    /// <summary>
    /// Get the prediction score matrix. To get the predictions, you have to multiply
    /// the spectras to predict by this prediction score matrix (in case of a single y-variable), this is
    /// simply the dot product.
    /// </summary>
    /// <param name="xLoads">Matrix of spectral loads [factors,spectral bins].</param>
    /// <param name="yLoads">Matrix of concentration loads[factors, number of concentrations].</param>
    /// <param name="W">Matrix of spectral weightings [factors,spectral bins].</param>
    /// <param name="V">Cross product matrix[1,factors].</param>
    /// <param name="numFactors">Number of factors to use to calculate the score matrix.</param>
    /// <param name="predictionScores">Output: the resulting score matrix[numberOfConcentrations, spectral bins]</param>
    public static void GetPredictionScoreMatrix(
      IROMatrix xLoads, // x-loads matrix
      IROMatrix yLoads, // y-loads matrix
      IROMatrix W, // weighting matrix
      IROMatrix V,  // Cross product vector
      int numFactors, // number of factors to use for prediction
      IMatrix predictionScores // Matrix of predicted y-values, must be same number of rows as spectra
      )
    {

      Matrix bidiag = new Matrix(numFactors,numFactors);
      IROMatrix subweights = MatrixMath.ToROSubMatrix(W,0,0,numFactors,W.Columns);
      IROMatrix subxloads  = MatrixMath.ToROSubMatrix(xLoads,0,0,numFactors,xLoads.Columns);
      MatrixMath.MultiplySecondTransposed(subxloads,subweights,bidiag);
      IMatrix invbidiag = bidiag.Inverse;

      
      Matrix subyloads = new Matrix(numFactors,yLoads.Columns);
      MatrixMath.Submatrix(yLoads,subyloads,0,0);
      // multiply each row of the subyloads matrix by the appropriate weight
      for(int i=0;i<subyloads.Rows;i++)
        for(int j=0;j<subyloads.Columns;j++)
          subyloads[i,j] *= V[0,i];

      Matrix helper = new Matrix(numFactors,yLoads.Columns);
      MatrixMath.Multiply(invbidiag,subyloads,helper);

      //Matrix scores = new Matrix(yLoads.Columns,xLoads.Columns);
      MatrixMath.MultiplyFirstTransposed(helper,subweights,predictionScores); // we calculate the transpose of scores (i.e. scores are horizontal oriented here)

      // now calculate the ys from the scores and the spectra

      //MultiplySecondTransposed(XU,scores,predictedY);
    } // end partial-least-squares-predict


    /// <summary>
    /// Performs a PLS cross validation.
    /// </summary>
    /// <param name="X">Matrix of spectra (a spectra is a row of this matrix).</param>
    /// <param name="Y">Matrix of concentrations (one sample is one row of this matrix).</param>
    /// <param name="numFactors">Maximal number of factors to use for PLS.</param>
    /// <param name="groupingStrategy">Instance of a class that is used to group the measurements for cross validation.</param>
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

      //      int[][] groups = bExcludeGroups ? new ExcludeGroupsGroupingStrategy().Group(Y) : new ExcludeSingleMeasurementsGroupingStrategy().Group(Y);
      int[][] groups = groupingStrategy.Group(Y);


      IBottomExtensibleMatrix xLoads = new MatrixMath.BEMatrix(0,0); // out: the loads of the X matrix
      IBottomExtensibleMatrix yLoads = new MatrixMath.BEMatrix(0,0); // out: the loads of the Y matrix
      IBottomExtensibleMatrix W = new MatrixMath.BEMatrix(0,0); // matrix of weighting values
      IRightExtensibleMatrix V = new MatrixMath.REMatrix(0,0); // matrix of cross products

      double[] crossPRESS = null;

      IMatrix XX=null; 
      IMatrix YY=null; 
      IMatrix XU=null; 
      IMatrix YU=null; 
      IMatrix predY=null; 

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

        // do a PLS with the builded matrices
        xLoads = new MatrixMath.BEMatrix(0,0); // clear xLoads
        yLoads = new MatrixMath.BEMatrix(0,0); // clear yLoads
        W      = new MatrixMath.BEMatrix(0,0); // clear W
        V      = new MatrixMath.REMatrix(0,0); // clear V
        int actnumfactors=numFactors;
        ExecuteAnalysis(XX, YY, ref actnumfactors, xLoads,yLoads,W,V,null); 
        numFactors = Math.Min(numFactors,actnumfactors); // if we have here a lesser number of factors, use it for all further calculations
                            

        // allocate the crossPRESS vector here, since now we know about the number of factors a bit more
        if(null==crossPRESS)
          crossPRESS = new double[numFactors+1]; // one more since we want to have the value at factors=0 (i.e. the variance of the y-matrix)

        // for all factors do now a prediction of the remaining spectra
        crossPRESS[0] += MatrixMath.SumOfSquares(YU);
        for(int nFactor=1;nFactor<=numFactors;nFactor++)
        {
          Predict(XU,xLoads,yLoads,W,V,nFactor,predY,null);
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

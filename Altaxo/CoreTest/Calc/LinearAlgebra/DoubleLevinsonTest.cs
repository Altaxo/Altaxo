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

#region Using Directive

using System;
using NUnit.Framework;
using Altaxo.Calc.LinearAlgebra;

#endregion Using Directive

namespace AltaxoTest.Calc.LinearAlgebra
{

  // suite of tests for double symmetric Levinson algorithm
  [TestFixture]
  public class DoubleLevinsonTest
  {

    #region Fields

    // unit testing - order 1
    
    DoubleVector TR1;     // Toeplitz matrix
    DoubleVector LC1;     // Toeplitz matrix
    DoubleMatrix A1;      // Lower triangle matrix
    DoubleVector D1;      // diagonal vector
    DoubleMatrix B1;      // upper triangle matrix
    DoubleMatrix I1;      // inverse matrix
    double Det1;        // exact determinant
    DoubleVector X1;      // RHS vector
    DoubleVector Y1;      // LHS vector
    double Tolerance1;      // allowable tolerance

    // unit testing - order 2
    
    DoubleVector TR2;     // Toeplitz matrix
    DoubleVector LC2;     // Toeplitz matrix
    DoubleMatrix A2;      // Lower triangle matrix
    DoubleVector D2;      // diagonal vector
    DoubleMatrix B2;      // upper triangle matrix
    DoubleMatrix I2;      // inverse matrix
    double Det2;        // exact determinant
    DoubleVector X2;      // RHS vector
    DoubleVector Y2;      // LHS vector
    double Tolerance2;      // allowable tolerance

    // unit testing - order 3
    
    DoubleVector TR3;     // Toeplitz matrix
    DoubleVector LC3;     // Toeplitz matrix
    DoubleMatrix A3;      // Lower triangle matrix
    DoubleVector D3;      // diagonal vector
    DoubleMatrix B3;      // upper triangle matrix
    DoubleMatrix I3;      // inverse matrix
    double Det3;        // exact determinant
    DoubleVector X3;      // RHS vector
    DoubleVector Y3;      // LHS vector
    double Tolerance3;      // allowable tolerance

    // unit testing - order 4
    
    DoubleVector TR4;     // Toeplitz matrix
    DoubleVector LC4;     // Toeplitz matrix
    DoubleMatrix A4;      // Lower triangle matrix
    DoubleVector D4;      // diagonal vector
    DoubleMatrix B4;      // upper triangle matrix
    DoubleMatrix I4;      // inverse matrix
    double Det4;        // exact determinant
    DoubleVector X4;      // RHS vector
    DoubleVector Y4;      // LHS vector
    double Tolerance4;      // allowable tolerance

    // unit testing - order 5
    
    DoubleVector TR5;     // Toeplitz matrix
    DoubleVector LC5;     // Toeplitz matrix
    DoubleMatrix A5;      // Lower triangle matrix
    DoubleVector D5;      // diagonal vector
    DoubleMatrix B5;      // upper triangle matrix
    DoubleMatrix I5;      // inverse matrix
    double Det5;        // exact determinant
    DoubleVector X5;      // RHS vector
    DoubleVector Y5;      // LHS vector
    double Tolerance5;      // allowable tolerance

    // unit testing - order 10
    
    DoubleVector TR10;      // Toeplitz matrix
    DoubleVector LC10;      // Toeplitz matrix
    DoubleMatrix A10;     // Lower triangle matrix
    DoubleVector D10;     // diagonal vector
    DoubleMatrix B10;     // upper triangle matrix
    DoubleMatrix I10;     // inverse matrix
    double Det10;       // exact determinant
    DoubleVector X10;     // RHS vector
    DoubleVector Y10;     // LHS vector
    double Tolerance10;     // allowable tolerance

    #endregion Fields

    #region Test Fixture Setup

    [TestFixtureSetUp]
    public void SetupTestCases()
    {

      // unit testing values - order 1

      TR1 = new DoubleVector(1);
      TR1[0] = +3.0000000000000000E+000;

      LC1 = new DoubleVector(1);
      LC1[0] = +3.0000000000000000E+000;

      A1 = new DoubleMatrix(1);
      A1[0, 0] = +1.0000000000000000E+000;

      D1 = new DoubleVector(1);
      D1[0] = +3.3333333333333331E-001;

      B1 = new DoubleMatrix(1);
      B1[0, 0] = +1.0000000000000000E+000;

      Det1 = +3.0000000000000000E+000;

      I1 = new DoubleMatrix(1);
      I1[0, 0] = +3.3333333333333331E-001;

      X1 = new DoubleVector(1);
      X1[0] = +1.0000000000000000E+000;

      Y1 = new DoubleVector(1);
      Y1[0] = +3.0000000000000000E+000;


      // unit testing values - order 2

      TR2 = new DoubleVector(2);
      TR2[0] = +3.0000000000000000E+000;
      TR2[1] = +2.0000000000000000E+000;

      LC2 = new DoubleVector(2);
      LC2[0] = +3.0000000000000000E+000;
      LC2[1] = +1.0000000000000000E+000;

      A2 = new DoubleMatrix(2);
      A2[0, 0] = +1.0000000000000000E+000;
      A2[1, 0] = -3.3333333333333331E-001;
      A2[1, 1] = +1.0000000000000000E+000;

      D2 = new DoubleVector(2);
      D2[0] = +3.3333333333333331E-001;
      D2[1] = +4.2857142857142855E-001;

      B2 = new DoubleMatrix(2);
      B2[0, 0] = +1.0000000000000000E+000;
      B2[0, 1] = -6.6666666666666663E-001;
      B2[1, 1] = +1.0000000000000000E+000;

      Det2 = +7.0000000000000000E+000;

      I2 = new DoubleMatrix(2);
      I2[0, 0] = +4.2857142857142855E-001;
      I2[0, 1] = -2.8571428571428570E-001;
      I2[1, 0] = -1.4285714285714285E-001;
      I2[1, 1] = +4.2857142857142855E-001;

      X2 = new DoubleVector(2);
      X2[0] = +1.0000000000000000E+000;
      X2[1] = +2.0000000000000000E+000;

      Y2 = new DoubleVector(2);
      Y2[0] = +7.0000000000000000E+000;
      Y2[1] = +7.0000000000000000E+000;


      // unit testing values - order 3

      TR3 = new DoubleVector(3);
      TR3[0] = +3.0000000000000000E+000;
      TR3[1] = +2.0000000000000000E+000;
      TR3[2] = +1.0000000000000000E+000;

      LC3 = new DoubleVector(3);
      LC3[0] = +3.0000000000000000E+000;
      LC3[1] = +1.0000000000000000E+000;
      LC3[2] = +0.0000000000000000E+000;

      A3 = new DoubleMatrix(3);
      A3[0, 0] = +1.0000000000000000E+000;
      A3[1, 0] = -3.3333333333333331E-001;
      A3[1, 1] = +1.0000000000000000E+000;
      A3[2, 0] = +1.4285714285714285E-001;
      A3[2, 1] = -4.2857142857142855E-001;
      A3[2, 2] = +1.0000000000000000E+000;

      D3 = new DoubleVector(3);
      D3[0] = +3.3333333333333331E-001;
      D3[1] = +4.2857142857142855E-001;
      D3[2] = +4.3750000000000000E-001;

      B3 = new DoubleMatrix(3);
      B3[0, 0] = +1.0000000000000000E+000;
      B3[0, 1] = -6.6666666666666663E-001;
      B3[1, 1] = +1.0000000000000000E+000;
      B3[0, 2] = +1.4285714285714285E-001;
      B3[1, 2] = -7.1428571428571430E-001;
      B3[2, 2] = +1.0000000000000000E+000;

      Det3 = +1.6000000000000000E+001;

      I3 = new DoubleMatrix(3);
      I3[0, 0] = +4.3750000000000000E-001;
      I3[0, 1] = -3.1250000000000000E-001;
      I3[0, 2] = +6.2500000000000000E-002;
      I3[1, 0] = -1.8750000000000000E-001;
      I3[1, 1] = +5.6250000000000000E-001;
      I3[1, 2] = -3.1250000000000000E-001;
      I3[2, 0] = +6.2500000000000000E-002;
      I3[2, 1] = -1.8750000000000000E-001;
      I3[2, 2] = +4.3750000000000000E-001;

      X3 = new DoubleVector(3);
      X3[0] = +1.0000000000000000E+000;
      X3[1] = +2.0000000000000000E+000;
      X3[2] = +3.0000000000000000E+000;

      Y3 = new DoubleVector(3);
      Y3[0] = +1.0000000000000000E+001;
      Y3[1] = +1.3000000000000000E+001;
      Y3[2] = +1.1000000000000000E+001;


      // unit testing values - order 4

      TR4 = new DoubleVector(4);
      TR4[0] = +4.0000000000000000E+000;
      TR4[1] = +3.0000000000000000E+000;
      TR4[2] = +2.0000000000000000E+000;
      TR4[3] = +1.0000000000000000E+000;

      LC4 = new DoubleVector(4);
      LC4[0] = +4.0000000000000000E+000;
      LC4[1] = +1.0000000000000000E+000;
      LC4[2] = +2.0000000000000000E+000;
      LC4[3] = +3.0000000000000000E+000;

      A4 = new DoubleMatrix(4);
      A4[0, 0] = +1.0000000000000000E+000;
      A4[1, 0] = -2.5000000000000000E-001;
      A4[1, 1] = +1.0000000000000000E+000;
      A4[2, 0] = -5.3846153846153844E-001;
      A4[2, 1] = +1.5384615384615385E-001;
      A4[2, 2] = +1.0000000000000000E+000;
      A4[3, 0] = -8.1818181818181823E-001;
      A4[3, 1] = +9.0909090909090912E-002;
      A4[3, 2] = +9.0909090909090912E-002;
      A4[3, 3] = +1.0000000000000000E+000;

      D4 = new DoubleVector(4);
      D4[0] = +2.5000000000000000E-001;
      D4[1] = +3.0769230769230771E-001;
      D4[2] = +2.9545454545454547E-001;
      D4[3] = +2.7500000000000002E-001;

      B4 = new DoubleMatrix(4);
      B4[0, 0] = +1.0000000000000000E+000;
      B4[0, 1] = -7.5000000000000000E-001;
      B4[1, 1] = +1.0000000000000000E+000;
      B4[0, 2] = +7.6923076923076927E-002;
      B4[1, 2] = -7.6923076923076927E-001;
      B4[2, 2] = +1.0000000000000000E+000;
      B4[0, 3] = +9.0909090909090912E-002;
      B4[1, 3] = +9.0909090909090912E-002;
      B4[2, 3] = -8.1818181818181823E-001;
      B4[3, 3] = +1.0000000000000000E+000;

      Det4 = +1.6000000000000000E+002;

      I4 = new DoubleMatrix(4);
      I4[0, 0] = +2.7500000000000002E-001;
      I4[0, 1] = -2.2500000000000001E-001;
      I4[0, 2] = +2.5000000000000001E-002;
      I4[0, 3] = +2.5000000000000001E-002;
      I4[1, 0] = +2.5000000000000001E-002;
      I4[1, 1] = +2.7500000000000002E-001;
      I4[1, 2] = -2.2500000000000001E-001;
      I4[1, 3] = +2.5000000000000001E-002;
      I4[2, 0] = +2.5000000000000001E-002;
      I4[2, 1] = +2.5000000000000001E-002;
      I4[2, 2] = +2.7500000000000002E-001;
      I4[2, 3] = -2.2500000000000001E-001;
      I4[3, 0] = -2.2500000000000001E-001;
      I4[3, 1] = +2.5000000000000001E-002;
      I4[3, 2] = +2.5000000000000001E-002;
      I4[3, 3] = +2.7500000000000002E-001;

      X4 = new DoubleVector(4);
      X4[0] = +1.0000000000000000E+000;
      X4[1] = +2.0000000000000000E+000;
      X4[2] = +3.0000000000000000E+000;
      X4[3] = +4.0000000000000000E+000;

      Y4 = new DoubleVector(4);
      Y4[0] = +2.0000000000000000E+001;
      Y4[1] = +2.6000000000000000E+001;
      Y4[2] = +2.8000000000000000E+001;
      Y4[3] = +2.6000000000000000E+001;


      // unit testing values - order 5

      TR5 = new DoubleVector(5);
      TR5[0] = +5.0000000000000000E+000;
      TR5[1] = +4.0000000000000000E+000;
      TR5[2] = +3.0000000000000000E+000;
      TR5[3] = +2.0000000000000000E+000;
      TR5[4] = +1.0000000000000000E+000;

      LC5 = new DoubleVector(5);
      LC5[0] = +5.0000000000000000E+000;
      LC5[1] = +1.0000000000000000E+000;
      LC5[2] = +2.0000000000000000E+000;
      LC5[3] = +3.0000000000000000E+000;
      LC5[4] = +4.0000000000000000E+000;

      A5 = new DoubleMatrix(5);
      A5[0, 0] = +1.0000000000000000E+000;
      A5[1, 0] = -2.0000000000000001E-001;
      A5[1, 1] = +1.0000000000000000E+000;
      A5[2, 0] = -4.2857142857142855E-001;
      A5[2, 1] = +1.4285714285714285E-001;
      A5[2, 2] = +1.0000000000000000E+000;
      A5[3, 0] = -6.6666666666666663E-001;
      A5[3, 1] = +1.1111111111111110E-001;
      A5[3, 2] = +1.1111111111111110E-001;
      A5[3, 3] = +1.0000000000000000E+000;
      A5[4, 0] = -8.7500000000000000E-001;
      A5[4, 1] = +6.2500000000000000E-002;
      A5[4, 2] = +6.2500000000000000E-002;
      A5[4, 3] = +6.2500000000000000E-002;
      A5[4, 4] = +1.0000000000000000E+000;

      D5 = new DoubleVector(5);
      D5[0] = +2.0000000000000001E-001;
      D5[1] = +2.3809523809523808E-001;
      D5[2] = +2.3333333333333334E-001;
      D5[3] = +2.2500000000000001E-001;
      D5[4] = +2.1333333333333335E-001;

      B5 = new DoubleMatrix(5);
      B5[0, 0] = +1.0000000000000000E+000;
      B5[0, 1] = -8.0000000000000004E-001;
      B5[1, 1] = +1.0000000000000000E+000;
      B5[0, 2] = +4.7619047619047616E-002;
      B5[1, 2] = -8.0952380952380953E-001;
      B5[2, 2] = +1.0000000000000000E+000;
      B5[0, 3] = +5.5555555555555552E-002;
      B5[1, 3] = +5.5555555555555552E-002;
      B5[2, 3] = -8.3333333333333337E-001;
      B5[3, 3] = +1.0000000000000000E+000;
      B5[0, 4] = +6.2500000000000000E-002;
      B5[1, 4] = +6.2500000000000000E-002;
      B5[2, 4] = +6.2500000000000000E-002;
      B5[3, 4] = -8.7500000000000000E-001;
      B5[4, 4] = +1.0000000000000000E+000;

      Det5 = +1.8750000000000000E+003;

      I5 = new DoubleMatrix(5);
      I5[0, 0] = +2.1333333333333335E-001;
      I5[0, 1] = -1.8666666666666668E-001;
      I5[0, 2] = +1.3333333333333334E-002;
      I5[0, 3] = +1.3333333333333334E-002;
      I5[0, 4] = +1.3333333333333334E-002;
      I5[1, 0] = +1.3333333333333334E-002;
      I5[1, 1] = +2.1333333333333335E-001;
      I5[1, 2] = -1.8666666666666668E-001;
      I5[1, 3] = +1.3333333333333334E-002;
      I5[1, 4] = +1.3333333333333334E-002;
      I5[2, 0] = +1.3333333333333334E-002;
      I5[2, 1] = +1.3333333333333334E-002;
      I5[2, 2] = +2.1333333333333335E-001;
      I5[2, 3] = -1.8666666666666668E-001;
      I5[2, 4] = +1.3333333333333334E-002;
      I5[3, 0] = +1.3333333333333334E-002;
      I5[3, 1] = +1.3333333333333334E-002;
      I5[3, 2] = +1.3333333333333334E-002;
      I5[3, 3] = +2.1333333333333335E-001;
      I5[3, 4] = -1.8666666666666668E-001;
      I5[4, 0] = -1.8666666666666668E-001;
      I5[4, 1] = +1.3333333333333334E-002;
      I5[4, 2] = +1.3333333333333334E-002;
      I5[4, 3] = +1.3333333333333334E-002;
      I5[4, 4] = +2.1333333333333335E-001;

      X5 = new DoubleVector(5);
      X5[0] = +1.0000000000000000E+000;
      X5[1] = +2.0000000000000000E+000;
      X5[2] = +3.0000000000000000E+000;
      X5[3] = +4.0000000000000000E+000;
      X5[4] = +5.0000000000000000E+000;

      Y5 = new DoubleVector(5);
      Y5[0] = +3.5000000000000000E+001;
      Y5[1] = +4.5000000000000000E+001;
      Y5[2] = +5.0000000000000000E+001;
      Y5[3] = +5.0000000000000000E+001;
      Y5[4] = +4.5000000000000000E+001;


      // unit testing values - order 10

      TR10 = new DoubleVector(10);
      TR10[0] = +1.0000000000000000E+001;
      TR10[1] = +9.0000000000000000E+000;
      TR10[2] = +8.0000000000000000E+000;
      TR10[3] = +7.0000000000000000E+000;
      TR10[4] = +6.0000000000000000E+000;
      TR10[5] = +5.0000000000000000E+000;
      TR10[6] = +4.0000000000000000E+000;
      TR10[7] = +3.0000000000000000E+000;
      TR10[8] = +2.0000000000000000E+000;
      TR10[9] = +1.0000000000000000E+000;

      LC10 = new DoubleVector(10);
      LC10[0] = +1.0000000000000000E+001;
      LC10[1] = +1.0000000000000000E+000;
      LC10[2] = +2.0000000000000000E+000;
      LC10[3] = +3.0000000000000000E+000;
      LC10[4] = +4.0000000000000000E+000;
      LC10[5] = +5.0000000000000000E+000;
      LC10[6] = +6.0000000000000000E+000;
      LC10[7] = +7.0000000000000000E+000;
      LC10[8] = +8.0000000000000000E+000;
      LC10[9] = +9.0000000000000000E+000;

      A10 = new DoubleMatrix(10);
      A10[0, 0] = +1.0000000000000000E+000;
      A10[1, 0] = -1.0000000000000001E-001;
      A10[1, 1] = +1.0000000000000000E+000;
      A10[2, 0] = -2.0879120879120880E-001;
      A10[2, 1] = +8.7912087912087919E-002;
      A10[2, 2] = +1.0000000000000000E+000;
      A10[3, 0] = -3.2530120481927710E-001;
      A10[3, 1] = +8.4337349397590355E-002;
      A10[3, 2] = +8.4337349397590355E-002;
      A10[3, 3] = +1.0000000000000000E+000;
      A10[4, 0] = -4.4736842105263158E-001;
      A10[4, 1] = +7.8947368421052627E-002;
      A10[4, 2] = +7.8947368421052627E-002;
      A10[4, 3] = +7.8947368421052627E-002;
      A10[4, 4] = +1.0000000000000000E+000;
      A10[5, 0] = -5.7142857142857140E-001;
      A10[5, 1] = +7.1428571428571425E-002;
      A10[5, 2] = +7.1428571428571425E-002;
      A10[5, 3] = +7.1428571428571425E-002;
      A10[5, 4] = +7.1428571428571425E-002;
      A10[5, 5] = +1.0000000000000000E+000;
      A10[6, 0] = -6.9230769230769229E-001;
      A10[6, 1] = +6.1538461538461542E-002;
      A10[6, 2] = +6.1538461538461542E-002;
      A10[6, 3] = +6.1538461538461542E-002;
      A10[6, 4] = +6.1538461538461542E-002;
      A10[6, 5] = +6.1538461538461542E-002;
      A10[6, 6] = +1.0000000000000000E+000;
      A10[7, 0] = -8.0327868852459017E-001;
      A10[7, 1] = +4.9180327868852458E-002;
      A10[7, 2] = +4.9180327868852458E-002;
      A10[7, 3] = +4.9180327868852458E-002;
      A10[7, 4] = +4.9180327868852458E-002;
      A10[7, 5] = +4.9180327868852458E-002;
      A10[7, 6] = +4.9180327868852458E-002;
      A10[7, 7] = +1.0000000000000000E+000;
      A10[8, 0] = -8.9655172413793105E-001;
      A10[8, 1] = +3.4482758620689655E-002;
      A10[8, 2] = +3.4482758620689655E-002;
      A10[8, 3] = +3.4482758620689655E-002;
      A10[8, 4] = +3.4482758620689655E-002;
      A10[8, 5] = +3.4482758620689655E-002;
      A10[8, 6] = +3.4482758620689655E-002;
      A10[8, 7] = +3.4482758620689655E-002;
      A10[8, 8] = +1.0000000000000000E+000;
      A10[9, 0] = -9.6428571428571430E-001;
      A10[9, 1] = +1.7857142857142856E-002;
      A10[9, 2] = +1.7857142857142856E-002;
      A10[9, 3] = +1.7857142857142856E-002;
      A10[9, 4] = +1.7857142857142856E-002;
      A10[9, 5] = +1.7857142857142856E-002;
      A10[9, 6] = +1.7857142857142856E-002;
      A10[9, 7] = +1.7857142857142856E-002;
      A10[9, 8] = +1.7857142857142856E-002;
      A10[9, 9] = +1.0000000000000000E+000;

      D10 = new DoubleVector(10);
      D10[0] = +1.0000000000000001E-001;
      D10[1] = +1.0989010989010989E-001;
      D10[2] = +1.0963855421686747E-001;
      D10[3] = +1.0921052631578948E-001;
      D10[4] = +1.0857142857142857E-001;
      D10[5] = +1.0769230769230770E-001;
      D10[6] = +1.0655737704918032E-001;
      D10[7] = +1.0517241379310345E-001;
      D10[8] = +1.0357142857142858E-001;
      D10[9] = +1.0181818181818182E-001;

      B10 = new DoubleMatrix(10);
      B10[0, 0] = +1.0000000000000000E+000;
      B10[0, 1] = -9.0000000000000002E-001;
      B10[1, 1] = +1.0000000000000000E+000;
      B10[0, 2] = +1.0989010989010990E-002;
      B10[1, 2] = -9.0109890109890112E-001;
      B10[2, 2] = +1.0000000000000000E+000;
      B10[0, 3] = +1.2048192771084338E-002;
      B10[1, 3] = +1.2048192771084338E-002;
      B10[2, 3] = -9.0361445783132532E-001;
      B10[3, 3] = +1.0000000000000000E+000;
      B10[0, 4] = +1.3157894736842105E-002;
      B10[1, 4] = +1.3157894736842105E-002;
      B10[2, 4] = +1.3157894736842105E-002;
      B10[3, 4] = -9.0789473684210531E-001;
      B10[4, 4] = +1.0000000000000000E+000;
      B10[0, 5] = +1.4285714285714285E-002;
      B10[1, 5] = +1.4285714285714285E-002;
      B10[2, 5] = +1.4285714285714285E-002;
      B10[3, 5] = +1.4285714285714285E-002;
      B10[4, 5] = -9.1428571428571426E-001;
      B10[5, 5] = +1.0000000000000000E+000;
      B10[0, 6] = +1.5384615384615385E-002;
      B10[1, 6] = +1.5384615384615385E-002;
      B10[2, 6] = +1.5384615384615385E-002;
      B10[3, 6] = +1.5384615384615385E-002;
      B10[4, 6] = +1.5384615384615385E-002;
      B10[5, 6] = -9.2307692307692313E-001;
      B10[6, 6] = +1.0000000000000000E+000;
      B10[0, 7] = +1.6393442622950821E-002;
      B10[1, 7] = +1.6393442622950821E-002;
      B10[2, 7] = +1.6393442622950821E-002;
      B10[3, 7] = +1.6393442622950821E-002;
      B10[4, 7] = +1.6393442622950821E-002;
      B10[5, 7] = +1.6393442622950821E-002;
      B10[6, 7] = -9.3442622950819676E-001;
      B10[7, 7] = +1.0000000000000000E+000;
      B10[0, 8] = +1.7241379310344827E-002;
      B10[1, 8] = +1.7241379310344827E-002;
      B10[2, 8] = +1.7241379310344827E-002;
      B10[3, 8] = +1.7241379310344827E-002;
      B10[4, 8] = +1.7241379310344827E-002;
      B10[5, 8] = +1.7241379310344827E-002;
      B10[6, 8] = +1.7241379310344827E-002;
      B10[7, 8] = -9.4827586206896552E-001;
      B10[8, 8] = +1.0000000000000000E+000;
      B10[0, 9] = +1.7857142857142856E-002;
      B10[1, 9] = +1.7857142857142856E-002;
      B10[2, 9] = +1.7857142857142856E-002;
      B10[3, 9] = +1.7857142857142856E-002;
      B10[4, 9] = +1.7857142857142856E-002;
      B10[5, 9] = +1.7857142857142856E-002;
      B10[6, 9] = +1.7857142857142856E-002;
      B10[7, 9] = +1.7857142857142856E-002;
      B10[8, 9] = -9.6428571428571430E-001;
      B10[9, 9] = +1.0000000000000000E+000;

      Det10 = +5.5000000000000000E+009;

      I10 = new DoubleMatrix(10);
      I10[0, 0] = +1.0181818181818182E-001;
      I10[0, 1] = -9.8181818181818176E-002;
      I10[0, 2] = +1.8181818181818182E-003;
      I10[0, 3] = +1.8181818181818182E-003;
      I10[0, 4] = +1.8181818181818182E-003;
      I10[0, 5] = +1.8181818181818182E-003;
      I10[0, 6] = +1.8181818181818182E-003;
      I10[0, 7] = +1.8181818181818182E-003;
      I10[0, 8] = +1.8181818181818182E-003;
      I10[0, 9] = +1.8181818181818182E-003;
      I10[1, 0] = +1.8181818181818182E-003;
      I10[1, 1] = +1.0181818181818182E-001;
      I10[1, 2] = -9.8181818181818176E-002;
      I10[1, 3] = +1.8181818181818182E-003;
      I10[1, 4] = +1.8181818181818182E-003;
      I10[1, 5] = +1.8181818181818182E-003;
      I10[1, 6] = +1.8181818181818182E-003;
      I10[1, 7] = +1.8181818181818182E-003;
      I10[1, 8] = +1.8181818181818182E-003;
      I10[1, 9] = +1.8181818181818182E-003;
      I10[2, 0] = +1.8181818181818182E-003;
      I10[2, 1] = +1.8181818181818182E-003;
      I10[2, 2] = +1.0181818181818182E-001;
      I10[2, 3] = -9.8181818181818176E-002;
      I10[2, 4] = +1.8181818181818182E-003;
      I10[2, 5] = +1.8181818181818182E-003;
      I10[2, 6] = +1.8181818181818182E-003;
      I10[2, 7] = +1.8181818181818182E-003;
      I10[2, 8] = +1.8181818181818182E-003;
      I10[2, 9] = +1.8181818181818182E-003;
      I10[3, 0] = +1.8181818181818182E-003;
      I10[3, 1] = +1.8181818181818182E-003;
      I10[3, 2] = +1.8181818181818182E-003;
      I10[3, 3] = +1.0181818181818182E-001;
      I10[3, 4] = -9.8181818181818176E-002;
      I10[3, 5] = +1.8181818181818182E-003;
      I10[3, 6] = +1.8181818181818182E-003;
      I10[3, 7] = +1.8181818181818182E-003;
      I10[3, 8] = +1.8181818181818182E-003;
      I10[3, 9] = +1.8181818181818182E-003;
      I10[4, 0] = +1.8181818181818182E-003;
      I10[4, 1] = +1.8181818181818182E-003;
      I10[4, 2] = +1.8181818181818182E-003;
      I10[4, 3] = +1.8181818181818182E-003;
      I10[4, 4] = +1.0181818181818182E-001;
      I10[4, 5] = -9.8181818181818176E-002;
      I10[4, 6] = +1.8181818181818182E-003;
      I10[4, 7] = +1.8181818181818182E-003;
      I10[4, 8] = +1.8181818181818182E-003;
      I10[4, 9] = +1.8181818181818182E-003;
      I10[5, 0] = +1.8181818181818182E-003;
      I10[5, 1] = +1.8181818181818182E-003;
      I10[5, 2] = +1.8181818181818182E-003;
      I10[5, 3] = +1.8181818181818182E-003;
      I10[5, 4] = +1.8181818181818182E-003;
      I10[5, 5] = +1.0181818181818182E-001;
      I10[5, 6] = -9.8181818181818176E-002;
      I10[5, 7] = +1.8181818181818182E-003;
      I10[5, 8] = +1.8181818181818182E-003;
      I10[5, 9] = +1.8181818181818182E-003;
      I10[6, 0] = +1.8181818181818182E-003;
      I10[6, 1] = +1.8181818181818182E-003;
      I10[6, 2] = +1.8181818181818182E-003;
      I10[6, 3] = +1.8181818181818182E-003;
      I10[6, 4] = +1.8181818181818182E-003;
      I10[6, 5] = +1.8181818181818182E-003;
      I10[6, 6] = +1.0181818181818182E-001;
      I10[6, 7] = -9.8181818181818176E-002;
      I10[6, 8] = +1.8181818181818182E-003;
      I10[6, 9] = +1.8181818181818182E-003;
      I10[7, 0] = +1.8181818181818182E-003;
      I10[7, 1] = +1.8181818181818182E-003;
      I10[7, 2] = +1.8181818181818182E-003;
      I10[7, 3] = +1.8181818181818182E-003;
      I10[7, 4] = +1.8181818181818182E-003;
      I10[7, 5] = +1.8181818181818182E-003;
      I10[7, 6] = +1.8181818181818182E-003;
      I10[7, 7] = +1.0181818181818182E-001;
      I10[7, 8] = -9.8181818181818176E-002;
      I10[7, 9] = +1.8181818181818182E-003;
      I10[8, 0] = +1.8181818181818182E-003;
      I10[8, 1] = +1.8181818181818182E-003;
      I10[8, 2] = +1.8181818181818182E-003;
      I10[8, 3] = +1.8181818181818182E-003;
      I10[8, 4] = +1.8181818181818182E-003;
      I10[8, 5] = +1.8181818181818182E-003;
      I10[8, 6] = +1.8181818181818182E-003;
      I10[8, 7] = +1.8181818181818182E-003;
      I10[8, 8] = +1.0181818181818182E-001;
      I10[8, 9] = -9.8181818181818176E-002;
      I10[9, 0] = -9.8181818181818176E-002;
      I10[9, 1] = +1.8181818181818182E-003;
      I10[9, 2] = +1.8181818181818182E-003;
      I10[9, 3] = +1.8181818181818182E-003;
      I10[9, 4] = +1.8181818181818182E-003;
      I10[9, 5] = +1.8181818181818182E-003;
      I10[9, 6] = +1.8181818181818182E-003;
      I10[9, 7] = +1.8181818181818182E-003;
      I10[9, 8] = +1.8181818181818182E-003;
      I10[9, 9] = +1.0181818181818182E-001;

      X10 = new DoubleVector(10);
      X10[0] = +1.0000000000000000E+000;
      X10[1] = +2.0000000000000000E+000;
      X10[2] = +3.0000000000000000E+000;
      X10[3] = +4.0000000000000000E+000;
      X10[4] = +5.0000000000000000E+000;
      X10[5] = +6.0000000000000000E+000;
      X10[6] = +7.0000000000000000E+000;
      X10[7] = +8.0000000000000000E+000;
      X10[8] = +9.0000000000000000E+000;
      X10[9] = +1.0000000000000000E+001;

      Y10 = new DoubleVector(10);
      Y10[0] = +2.2000000000000000E+002;
      Y10[1] = +2.6500000000000000E+002;
      Y10[2] = +3.0000000000000000E+002;
      Y10[3] = +3.2500000000000000E+002;
      Y10[4] = +3.4000000000000000E+002;
      Y10[5] = +3.4500000000000000E+002;
      Y10[6] = +3.4000000000000000E+002;
      Y10[7] = +3.2500000000000000E+002;
      Y10[8] = +3.0000000000000000E+002;
      Y10[9] = +2.6500000000000000E+002;

      // Tolerances
      Tolerance1 = 1.000E-015;
      Tolerance2 = 1.000E-015;
      Tolerance3 = 2.200E-015;
      Tolerance4 = 1.000E-014;
      Tolerance5 = 2.000E-014;
      Tolerance10 = 2.000E-013;
    }

    #endregion Test Fixture Setup

    #region Null Parameter Tests for Constructor 
    
    // Test constructor with a null parameter
    [Test]
    [ExpectedException(typeof(System.ArgumentNullException))]
    public void NullParameterTestforConstructor1()
    {
      DoubleLevinson dl = new DoubleLevinson(null as DoubleVector, TR5);
    }

    // Test constructor with a null parameter
    [Test]
    [ExpectedException(typeof(System.ArgumentNullException))]
    public void NullParameterTestforConstructor2()
    {
      DoubleLevinson dl = new DoubleLevinson(LC5, null as DoubleVector);
    }
    
    [Test]
    [ExpectedException(typeof(System.ArgumentNullException))]
    public void NullParameterTestforConstructor3()
    {
      DoubleLevinson dl = new DoubleLevinson(null as RODoubleVector, TR5.ToArray());
    }

    // Test constructor with a null parameter
    [Test]
    [ExpectedException(typeof(System.ArgumentNullException))]
    public void NullParameterTestforConstructor4()
    {
      DoubleLevinson dl = new DoubleLevinson(LC5.ToArray(), null as RODoubleVector);
    }
    
    #endregion  Null Parameter Tests for Constructor 

    #region Zero Length Vector Tests for Constructor
    
    // Test constructor with a zero length vector parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void ZeroLengthVectorTestsforConstructor1()
    {
      DoubleVector dv = new DoubleVector(1, 0.0);
      dv.RemoveAt(0);
      DoubleLevinson  dl = new DoubleLevinson(dv, dv);
    }

    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void ZeroLengthVectorTestsforConstructor2()
    {
      double[] dv = new double[0];
      DoubleLevinson  dl = new DoubleLevinson(dv, dv);
    }
    
    #endregion Zero Length Vector Tests for Constructor

    #region Mismatching Vector Length Tests for Constructor

    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchVectorLengthTestsforConstructor1()
    {
      DoubleLevinson  dl = new DoubleLevinson(LC2, TR3);
    }

    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchVectorLengthTestsforConstructor2()
    {
      DoubleLevinson  dl = new DoubleLevinson(LC2.ToArray(), TR3.ToArray());
    }
    
    #endregion Mismatching Vector Length Tests for Constructor

    #region First Element Test for Constructor

    [Test]
    [ExpectedException(typeof(System.ArithmeticException))]
    public void FirstElementTestforConstructor1()
    {
      DoubleVector dv = new DoubleVector(3, 1.0);
      DoubleLevinson  dl = new DoubleLevinson(LC3, dv);
    }

    [Test]
    [ExpectedException(typeof(System.ArithmeticException))]
    public void FirstElementTestforConstructor2()
    {
      DoubleVector dv = new DoubleVector(3, 1.0);
      DoubleLevinson  dl = new DoubleLevinson(LC3.ToArray(), dv.ToArray());
    }

    #endregion First Element Test for Constructor

    #region Get Vector Member Tests

    // check get vector
    [Test]
    public void GetLeftColumnTest()
    {
      DoubleLevinson  dl = new DoubleLevinson(LC5, TR5);
      DoubleVector LC = dl.GetLeftColumn();
      Assert.IsTrue(LC5.Equals(LC));
    }
    
    // check get vector
    [Test]
    public void GetTopRowTest()
    {
      DoubleLevinson  dl = new DoubleLevinson(LC5, TR5);
      DoubleVector TR = dl.GetTopRow();
      Assert.IsTrue(TR5.Equals(TR));
    }
    
    #endregion Get Vector Member Tests

    #region GetMatrix Member Test
    
    // check get matrix
    [Test]
    public void GetMatrixMemberTest()
    {
      DoubleLevinson dl = new DoubleLevinson(LC5, TR5);
      DoubleMatrix dldm = dl.GetMatrix();
      for (int row = 0; row < TR5.Length; row++)
      {
        for (int column = 0; column < TR5.Length; column++)
        {
          if (column < row)
          {
            Assert.IsTrue(dldm[row, column] == LC5[row - column]);
          }
          else
          {
            Assert.IsTrue(dldm[row, column] == TR5[column - row]);
          }
        }
      }
    }
    
    #endregion GetMatrix Member Test

    #region Order Property Test

    // test order property
    [Test]
    public void OrderPropertyTest()
    {
      DoubleLevinson  dl = new DoubleLevinson(LC5, TR5);
      Assert.IsTrue(dl.Order == 5);
    }

    #endregion Order Property Test

    #region Decomposition Test 1

    // test the UDL factorisation for case 1

    [Test]
    public void DecompositionTest1()
    {
      int i, j;
      double e, me;
      DoubleLevinson dl = new DoubleLevinson(LC1, TR1);
      DoubleMatrix U = dl.U;
      DoubleMatrix D = dl.D;
      DoubleMatrix L = dl.L;
      
      // check the upper triangle
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        for (j = 0; j < dl.Order; j++)
        {
          if (B1[i, j] != U[i, j])
          {
            e = System.Math.Abs((B1[i, j] - U[i, j]) / B1[i, j]);
            if (e > me)
            {
              me = e;
            }
          }
        }
      }
      Assert.IsTrue(me < Tolerance1, "Maximum Error = " + me.ToString());

      // check the lower triangle
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        for (j = 0; j < dl.Order; j++)
        {
          if (A1[i, j] != L[i, j])
          {
            e = System.Math.Abs((A1[i, j] - L[i, j]) / A1[i, j]);
            if (e > me)
            {
              me = e;
            }
          }
        }
      }
      Assert.IsTrue(me < Tolerance1, "Maximum Error = " + me.ToString());

      // check the diagonal
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        e = System.Math.Abs((D1[i] - D[i, i]) / D1[i]);
        if (e > me)
        {
          me = e;
        }
      }

      Assert.IsTrue(me < Tolerance1, "Maximum Error = " + me.ToString());
    }
    
    #endregion Decomposition Test 1

    #region Decomposition Test 2

    // test the UDL factorisation for case 2

    [Test]
    public void DecompositionTest2()
    {
      int i, j;
      double e, me;
      DoubleLevinson dl = new DoubleLevinson(LC2, TR2);
      DoubleMatrix U = dl.U;
      DoubleMatrix D = dl.D;
      DoubleMatrix L = dl.L;
      
      // check the upper triangle
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        for (j = 0; j < dl.Order; j++)
        {
          if (B2[i, j] != U[i, j])
          {
            e = System.Math.Abs((B2[i, j] - U[i, j]) / B2[i, j]);
            if (e > me)
            {
              me = e;
            }
          }
        }
      }
      Assert.IsTrue(me < Tolerance2, "Maximum Error = " + me.ToString());

      // check the lower triangle
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        for (j = 0; j < dl.Order; j++)
        {
          if (A2[i, j] != L[i, j])
          {
            e = System.Math.Abs((A2[i, j] - L[i, j]) / A2[i, j]);
            if (e > me)
            {
              me = e;
            }
          }
        }
      }
      Assert.IsTrue(me < Tolerance2, "Maximum Error = " + me.ToString());

      // check the diagonal
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        e = System.Math.Abs((D2[i] - D[i, i]) / D2[i]);
        if (e > me)
        {
          me = e;
        }
      }

      Assert.IsTrue(me < Tolerance2, "Maximum Error = " + me.ToString());
    }
    
    #endregion Decomposition Test 2

    #region Decomposition Test 3

    // test the UDL factorisation for case 3

    [Test]
    public void DecompositionTest3()
    {
      int i, j;
      double e, me;
      DoubleLevinson dl = new DoubleLevinson(LC3, TR3);
      DoubleMatrix U = dl.U;
      DoubleMatrix D = dl.D;
      DoubleMatrix L = dl.L;
      
      // check the upper triangle
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        for (j = 0; j < dl.Order; j++)
        {
          if (B3[i, j] != U[i, j])
          {
            e = System.Math.Abs((B3[i, j] - U[i, j]) / B3[i, j]);
            if (e > me)
            {
              me = e;
            }
          }
        }
      }
      Assert.IsTrue(me < Tolerance3, "Maximum Error = " + me.ToString());

      // check the lower triangle
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        for (j = 0; j < dl.Order; j++)
        {
          if (A3[i, j] != L[i, j])
          {
            e = System.Math.Abs((A3[i, j] - L[i, j]) / A3[i, j]);
            if (e > me)
            {
              me = e;
            }
          }
        }
      }
      Assert.IsTrue(me < Tolerance3, "Maximum Error = " + me.ToString());

      // check the diagonal
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        e = System.Math.Abs((D3[i] - D[i, i]) / D3[i]);
        if (e > me)
        {
          me = e;
        }
      }

      Assert.IsTrue(me < Tolerance3, "Maximum Error = " + me.ToString());
    }
    
    #endregion Decomposition Test 3

    #region Decomposition Test 4

    // test the UDL factorisation for case 4

    [Test]
    public void DecompositionTest4()
    {
      int i, j;
      double e, me;
      DoubleLevinson dl = new DoubleLevinson(LC4, TR4);
      DoubleMatrix U = dl.U;
      DoubleMatrix D = dl.D;
      DoubleMatrix L = dl.L;
      
      // check the upper triangle
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        for (j = 0; j < dl.Order; j++)
        {
          if (B4[i, j] != U[i, j])
          {
            e = System.Math.Abs((B4[i, j] - U[i, j]) / B4[i, j]);
            if (e > me)
            {
              me = e;
            }
          }
        }
      }
      Assert.IsTrue(me < Tolerance4, "Maximum Error = " + me.ToString());

      // check the lower triangle
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        for (j = 0; j < dl.Order; j++)
        {
          if (A4[i, j] != L[i, j])
          {
            e = System.Math.Abs((A4[i, j] - L[i, j]) / A4[i, j]);
            if (e > me)
            {
              me = e;
            }
          }
        }
      }
      Assert.IsTrue(me < Tolerance4, "Maximum Error = " + me.ToString());

      // check the diagonal
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        e = System.Math.Abs((D4[i] - D[i, i]) / D4[i]);
        if (e > me)
        {
          me = e;
        }
      }

      Assert.IsTrue(me < Tolerance4, "Maximum Error = " + me.ToString());
    }
    
    #endregion Decomposition Test 4

    #region Decomposition Test 5

    // test the UDL factorisation for case 5

    [Test]
    public void DecompositionTest5()
    {
      int i, j;
      double e, me;
      DoubleLevinson dl = new DoubleLevinson(LC5, TR5);
      DoubleMatrix U = dl.U;
      DoubleMatrix D = dl.D;
      DoubleMatrix L = dl.L;
      
      // check the upper triangle
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        for (j = 0; j < dl.Order; j++)
        {
          if (B5[i, j] != U[i, j])
          {
            e = System.Math.Abs((B5[i, j] - U[i, j]) / B5[i, j]);
            if (e > me)
            {
              me = e;
            }
          }
        }
      }
      Assert.IsTrue(me < Tolerance5, "Maximum Error = " + me.ToString());

      // check the lower triangle
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        for (j = 0; j < dl.Order; j++)
        {
          if (A5[i, j] != L[i, j])
          {
            e = System.Math.Abs((A5[i, j] - L[i, j]) / A5[i, j]);
            if (e > me)
            {
              me = e;
            }
          }
        }
      }
      Assert.IsTrue(me < Tolerance5, "Maximum Error = " + me.ToString());

      // check the diagonal
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        e = System.Math.Abs((D5[i] - D[i, i]) / D5[i]);
        if (e > me)
        {
          me = e;
        }
      }

      Assert.IsTrue(me < Tolerance5, "Maximum Error = " + me.ToString());
    }
    
    #endregion Decomposition Test 5

    #region Decomposition Test 10

    // test the UDL factorisation for case 10

    [Test]
    public void DecompositionTest10()
    {
      int i, j;
      double e, me;
      DoubleLevinson dl = new DoubleLevinson(LC10, TR10);
      DoubleMatrix U = dl.U;
      DoubleMatrix D = dl.D;
      DoubleMatrix L = dl.L;
      
      // check the upper triangle
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        for (j = 0; j < dl.Order; j++)
        {
          if (B10[i, j] != U[i, j])
          {
            e = System.Math.Abs((B10[i, j] - U[i, j]) / B10[i, j]);
            if (e > me)
            {
              me = e;
            }
          }
        }
      }
      Assert.IsTrue(me < Tolerance10, "Maximum Error = " + me.ToString());

      // check the lower triangle
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        for (j = 0; j < dl.Order; j++)
        {
          if (A10[i, j] != L[i, j])
          {
            e = System.Math.Abs((A10[i, j] - L[i, j]) / A10[i, j]);
            if (e > me)
            {
              me = e;
            }
          }
        }
      }
      Assert.IsTrue(me < Tolerance10, "Maximum Error = " + me.ToString());

      // check the diagonal
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        e = System.Math.Abs((D10[i] - D[i, i]) / D10[i]);
        if (e > me)
        {
          me = e;
        }
      }

      Assert.IsTrue(me < Tolerance10, "Maximum Error = " + me.ToString());
    }
    
    #endregion Decomposition Test 10

    #region Singularity Property Test 1

    // check that non-singular matrix is detected
    [Test]
    public void SingularityPropertyTest1()
    {
      DoubleLevinson dl = new DoubleLevinson(LC4,TR4);
      Assert.IsFalse(dl.IsSingular);
    }

    #endregion Singularity Property Test 1

    #region Singularity Property Test 2
    // check that singular matrix is detected
    [Test]
    public void SingularityPropertyTest2()
    {
      DoubleVector LC = new DoubleVector(new double[]{4.0, 2.0, 1.0, 0.0});
      DoubleVector TR = new DoubleVector(new double[]{4.0, 8.0, 2.0, 1.0});

      DoubleLevinson dl = new DoubleLevinson(LC,TR);
      Assert.IsTrue(dl.IsSingular);
    }

    #endregion Singularity Property Test 2

    #region GetDeterminant Method Test 1
    
    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest1()
    {
      // calculate determinant from diagonal
      DoubleLevinson  dl = new DoubleLevinson(LC1, TR1);

      // check results match
      Double e = System.Math.Abs( (dl.GetDeterminant() - Det1)/Det1 );
      Assert.IsTrue(e < Tolerance1);
    }
    
    #endregion GetDeterminant Method Test 1

    #region GetDeterminant Method Test 2
    
    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest2()
    {
      // calculate determinant from diagonal
      DoubleLevinson  dl = new DoubleLevinson(LC2, TR2);

      // check results match
      Double e = System.Math.Abs( (dl.GetDeterminant() - Det2)/Det2 );
      Assert.IsTrue(e < Tolerance2);
    }
    
    #endregion GetDeterminant Method Test 2

    #region GetDeterminant Method Test 3
    
    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest3()
    {
      // calculate determinant from diagonal
      DoubleLevinson  dl = new DoubleLevinson(LC3, TR3);

      // check results match
      Double e = System.Math.Abs( (dl.GetDeterminant() - Det3)/Det3 );
      Assert.IsTrue(e < Tolerance3);
    }
    
    #endregion GetDeterminant Method Test 3

    #region GetDeterminant Method Test 4
    
    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest4()
    {
      // calculate determinant from diagonal
      DoubleLevinson  dl = new DoubleLevinson(LC4, TR4);

      // check results match
      Double e = System.Math.Abs( (dl.GetDeterminant() - Det4)/Det4 );
      Assert.IsTrue(e < Tolerance4);
    }
    
    #endregion GetDeterminant Method Test 4

    #region GetDeterminant Method Test 5
    
    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest5()
    {
      // calculate determinant from diagonal
      DoubleLevinson  dl = new DoubleLevinson(LC5, TR5);

      // check results match
      Double e = System.Math.Abs( (dl.GetDeterminant() - Det5)/Det5 );
      Assert.IsTrue(e < Tolerance5);
    }
    
    #endregion GetDeterminant Method Test 5

    #region GetDeterminant Method Test 10
    
    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest10()
    {
      // calculate determinant from diagonal
      DoubleLevinson  dl = new DoubleLevinson(LC10, TR10);

      // check results match
      Double e = System.Math.Abs( (dl.GetDeterminant() - Det10)/Det10 );
      Assert.IsTrue(e < Tolerance10);
    }
    
    #endregion GetDeterminant Method Test 10

    #region Null Parameter Test for SolveVector

    [Test]
    [ExpectedException(typeof(System.ArgumentNullException))]
    public void NullParameterTestforSolveVector()
    {
      DoubleLevinson dl = new DoubleLevinson(LC10, TR10);
      DoubleVector X = dl.Solve(null as DoubleVector);
    }

    #endregion Null Parameter Test for SolveVector

    #region Mismatch Rows Test for SolveVector

    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchRowsTestforSolveVector()
    {
      DoubleLevinson dl = new DoubleLevinson(LC10, TR10);
      DoubleVector X = dl.Solve(X5);
    }

    #endregion Mismatch Rows Test for SolveVector

    #region SolveVector 1

    // Test solving a linear system
    [Test]
    public void SolveVector1()
    {
      int i;
      double e, me;
      DoubleLevinson  dl = new DoubleLevinson(LC1, TR1);
      DoubleVector X = dl.Solve(Y1);
      
      // determine the maximum error
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        e = System.Math.Abs((X1[i] - X[i]) / X1[i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.IsTrue(me < Tolerance1, "Maximum Error = " + me.ToString());
    }
    
    #endregion SolveVector 1

    #region SolveVector 2

    // Test solving a linear system
    [Test]
    public void SolveVector2()
    {
      int i;
      double e, me;
      DoubleLevinson  dl = new DoubleLevinson(LC2, TR2);
      DoubleVector X = dl.Solve(Y2);
      
      // determine the maximum error
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        e = System.Math.Abs((X2[i] - X[i]) / X2[i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.IsTrue(me < Tolerance2, "Maximum Error = " + me.ToString());
    }
    
    #endregion SolveVector 2

    #region SolveVector 3

    // Test solving a linear system
    [Test]
    public void SolveVector3()
    {
      int i;
      double e, me;
      DoubleLevinson  dl = new DoubleLevinson(LC3, TR3);
      DoubleVector X = dl.Solve(Y3);
      
      // determine the maximum error
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        e = System.Math.Abs((X3[i] - X[i]) / X3[i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.IsTrue(me < Tolerance3, "Maximum Error = " + me.ToString());
    }
    
    #endregion SolveVector 3

    #region SolveVector 4

    // Test solving a linear system
    [Test]
    public void SolveVector4()
    {
      int i;
      double e, me;
      DoubleLevinson  dl = new DoubleLevinson(LC4, TR4);
      DoubleVector X = dl.Solve(Y4);
      
      // determine the maximum error
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        e = System.Math.Abs((X4[i] - X[i]) / X4[i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.IsTrue(me < Tolerance4, "Maximum Error = " + me.ToString());
    }
    
    #endregion SolveVector 4

    #region SolveVector 5

    // Test solving a linear system
    [Test]
    public void SolveVector5()
    {
      int i;
      double e, me;
      DoubleLevinson  dl = new DoubleLevinson(LC5, TR5);
      DoubleVector X = dl.Solve(Y5);
      
      // determine the maximum error
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        e = System.Math.Abs((X5[i] - X[i]) / X5[i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.IsTrue(me < Tolerance5, "Maximum Error = " + me.ToString());
    }
    
    #endregion SolveVector 5

    #region SolveVector 10

    // Test solving a linear system
    [Test]
    public void SolveVector10()
    {
      int i;
      double e, me;
      DoubleLevinson  dl = new DoubleLevinson(LC10, TR10);
      DoubleVector X = dl.Solve(Y10);
      
      // determine the maximum error
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        e = System.Math.Abs((X10[i] - X[i]) / X10[i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.IsTrue(me < Tolerance10, "Maximum Error = " + me.ToString());
    }
    
    #endregion SolveVector 10

    #region Null Parameter Test for SolveMatrix

    [Test]
    [ExpectedException(typeof(System.ArgumentNullException))]
    public void NullParameterTestforSolveMatrix()
    {
      DoubleLevinson  dl = new DoubleLevinson(LC10, TR10);
      DoubleMatrix X = dl.Solve(null as DoubleMatrix);
    }

    #endregion Null Parameter Test for SolveMatrix

    #region Mismatch Rows Test for SolveMatrix

    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchRowsTestforSolveMatrix()
    {
      DoubleLevinson  dl = new DoubleLevinson(LC10, TR10);
      DoubleMatrix X = dl.Solve(I5);
    }

    #endregion Mismatch Rows Test for SolveMatrix

    #region Solve Matrix 1

    // calculate inverse by solving linear equations with identity RHS
    [Test]
    public void SolveMatrix1()
    {
      int i, j;
      double e, me;
      DoubleLevinson dl = new DoubleLevinson(LC1, TR1);

      // check inverse
      DoubleMatrix I = dl.Solve(DoubleMatrix.CreateIdentity(1));
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        for (j = 0; j < dl.Order; j++)
        {
          e = System.Math.Abs((I1[i, j] - I[i, j]) / I1[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance1, "Maximum Error = " + me.ToString());
    }
    
    #endregion Solve Matrix 1

    #region Solve Matrix 2

    // calculate inverse by solving linear equations with identity RHS
    [Test]
    public void SolveMatrix2()
    {
      int i, j;
      double e, me;
      DoubleLevinson dl = new DoubleLevinson(LC2, TR2);

      // check inverse
      DoubleMatrix I = dl.Solve(DoubleMatrix.CreateIdentity(2));
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        for (j = 0; j < dl.Order; j++)
        {
          e = System.Math.Abs((I2[i, j] - I[i, j]) / I2[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance2, "Maximum Error = " + me.ToString());
    }
    
    #endregion Solve Matrix 2

    #region Solve Matrix 3

    // calculate inverse by solving linear equations with identity RHS
    [Test]
    public void SolveMatrix3()
    {
      int i, j;
      double e, me;
      DoubleLevinson dl = new DoubleLevinson(LC3, TR3);

      // check inverse
      DoubleMatrix I = dl.Solve(DoubleMatrix.CreateIdentity(3));
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        for (j = 0; j < dl.Order; j++)
        {
          e = System.Math.Abs((I3[i, j] - I[i, j]) / I3[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance3, "Maximum Error = " + me.ToString());
    }
    
    #endregion Solve Matrix 3

    #region Solve Matrix 4

    // calculate inverse by solving linear equations with identity RHS
    [Test]
    public void SolveMatrix4()
    {
      int i, j;
      double e, me;
      DoubleLevinson dl = new DoubleLevinson(LC4, TR4);

      // check inverse
      DoubleMatrix I = dl.Solve(DoubleMatrix.CreateIdentity(4));
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        for (j = 0; j < dl.Order; j++)
        {
          e = System.Math.Abs((I4[i, j] - I[i, j]) / I4[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance4, "Maximum Error = " + me.ToString());
    }
    
    #endregion Solve Matrix 4

    #region Solve Matrix 5

    // calculate inverse by solving linear equations with identity RHS
    [Test]
    public void SolveMatrix5()
    {
      int i, j;
      double e, me;
      DoubleLevinson dl = new DoubleLevinson(LC5, TR5);

      // check inverse
      DoubleMatrix I = dl.Solve(DoubleMatrix.CreateIdentity(5));
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        for (j = 0; j < dl.Order; j++)
        {
          e = System.Math.Abs((I5[i, j] - I[i, j]) / I5[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance5, "Maximum Error = " + me.ToString());
    }
    
    #endregion Solve Matrix 5

    #region Solve Matrix 10

    // calculate inverse by solving linear equations with identity RHS
    [Test]
    public void SolveMatrix10()
    {
      int i, j;
      double e, me;
      DoubleLevinson dl = new DoubleLevinson(LC10, TR10);

      // check inverse
      DoubleMatrix I = dl.Solve(DoubleMatrix.CreateIdentity(10));
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        for (j = 0; j < dl.Order; j++)
        {
          e = System.Math.Abs((I10[i, j] - I[i, j]) / I10[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance10, "Maximum Error = " + me.ToString());
    }
    
    #endregion Solve Matrix 10

    #region Get Inverse 1

    // calculate inverse using GetInverse member
    [Test]
    public void GetInverse1()
    {
      int i, j;
      double e, me;
      DoubleLevinson dl = new DoubleLevinson(LC1, TR1);

      // check inverse
      DoubleMatrix I = dl.GetInverse();
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        for (j = 0; j < dl.Order; j++)
        {
          e = System.Math.Abs((I1[i, j] - I[i, j]) / I1[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance1, "Maximum Error = " + me.ToString());
    }
    
    #endregion Get Inverse 1

    #region Get Inverse 2

    // calculate inverse using GetInverse member
    [Test]
    public void GetInverse2()
    {
      int i, j;
      double e, me;
      DoubleLevinson dl = new DoubleLevinson(LC2, TR2);

      // check inverse
      DoubleMatrix I = dl.GetInverse();
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        for (j = 0; j < dl.Order; j++)
        {
          e = System.Math.Abs((I2[i, j] - I[i, j]) / I2[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance2, "Maximum Error = " + me.ToString());
    }
    
    #endregion Get Inverse 2

    #region Get Inverse 3

    // calculate inverse using GetInverse member
    [Test]
    public void GetInverse3()
    {
      int i, j;
      double e, me;
      DoubleLevinson dl = new DoubleLevinson(LC3, TR3);

      // check inverse
      DoubleMatrix I = dl.GetInverse();
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        for (j = 0; j < dl.Order; j++)
        {
          e = System.Math.Abs((I3[i, j] - I[i, j]) / I3[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance3, "Maximum Error = " + me.ToString());
    }
    
    #endregion Get Inverse 3

    #region Get Inverse 4

    // calculate inverse using GetInverse member
    [Test]
    public void GetInverse4()
    {
      int i, j;
      double e, me;
      DoubleLevinson dl = new DoubleLevinson(LC4, TR4);

      // check inverse
      DoubleMatrix I = dl.GetInverse();
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        for (j = 0; j < dl.Order; j++)
        {
          e = System.Math.Abs((I4[i, j] - I[i, j]) / I4[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance4, "Maximum Error = " + me.ToString());
    }
    
    #endregion Get Inverse 4

    #region Get Inverse 5

    // calculate inverse using GetInverse member
    [Test]
    public void GetInverse5()
    {
      int i, j;
      double e, me;
      DoubleLevinson dl = new DoubleLevinson(LC5, TR5);

      // check inverse
      DoubleMatrix I = dl.GetInverse();
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        for (j = 0; j < dl.Order; j++)
        {
          e = System.Math.Abs((I5[i, j] - I[i, j]) / I5[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance5, "Maximum Error = " + me.ToString());
    }
    
    #endregion Get Inverse 5

    #region Get Inverse 10

    // calculate inverse using GetInverse member
    [Test]
    public void GetInverse10()
    {
      int i, j;
      double e, me;
      DoubleLevinson dl = new DoubleLevinson(LC10, TR10);

      // check inverse
      DoubleMatrix I = dl.GetInverse();
      me = 0.0;
      for (i = 0; i < dl.Order; i++)
      {
        for (j = 0; j < dl.Order; j++)
        {
          e = System.Math.Abs((I10[i, j] - I[i, j]) / I10[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance10, "Maximum Error = " + me.ToString());
    }
    
    #endregion Get Inverse 10

    #region Null Parameter Test 1 for Static SolveVector
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullParameterTestforStaticSolveVector1()
    {
      DoubleVector X = DoubleLevinson.Solve(null, TR10, Y10);
    }

    #endregion Null Parameter Test 1 for Static SolveVector

    #region Null Parameter Test 2 for Static SolveVector
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullParameterTestforStaticSolveVector2()
    {
      DoubleVector X = DoubleLevinson.Solve(LC10, null, Y10);
    }

    #endregion Null Parameter Test 2 for Static SolveVector

    #region Null Parameter Test 3 for Static SolveVector
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullParameterTestforStaticSolveVector3()
    {
      DoubleVector X = DoubleLevinson.Solve(LC10, TR10, null as DoubleVector);
    }

    #endregion Null Parameter Test 3 for Static SolveVector

    #region Zero Vector Length Test for Static SolveVector
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void ZeroVectorLengthTestforStaticSolveVector()
    {
      DoubleVector LC = new DoubleVector(1, 0.0);
      LC.RemoveAt(0);
      DoubleVector X = DoubleLevinson.Solve(LC, TR10, Y10);
    }

    #endregion Zero Vector Length Test for Static SolveVector

    #region Mismatch Dimension Test 1 for Static SolveVector
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchDimensionTestforStaticSolveVector1()
    {
      DoubleVector X = DoubleLevinson.Solve(LC10, TR5, Y5);
    }

    #endregion Mismatch Dimension Test 1 for Static SolveVector

    #region Mismatch Dimension Test 2 for Static SolveVector
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchDimensionTestforStaticSolveVector2()
    {
      DoubleVector X = DoubleLevinson.Solve(LC10, TR10, Y5);
    }

    #endregion Mismatch Dimension Test 2 for Static SolveVector

    #region First Element Test for Static SolveVector

    [Test]
    [ExpectedException(typeof(System.ArithmeticException))]
    public void FirstElementTestforStaticSolveVector()
    {
      DoubleVector dv = new DoubleVector(3, 1.0);
      DoubleVector X = DoubleLevinson.Solve(dv, TR3, Y3);
    }

    #endregion First Element Test for Static SolveVector

    #region Singular Test for Static SolveVector
    
    // test with Toeplitz matrix which has a singular principal sub-matrix
    [Test]
    [ExpectedException(typeof(SingularMatrixException))]
    public void SingularTestforStaticSolveVector()
    {
      DoubleVector dv = new DoubleVector(3, 1.0);
      DoubleVector X = DoubleLevinson.Solve(dv, dv, Y3);
    }
    
    #endregion Singular Test for Static SolveVector

    #region Static Solve Vector 1
    
    [Test]
    public void StaticSolveVector1()
    {
      int i;
      double e, me;
      DoubleVector X = DoubleLevinson.Solve(LC1, TR1, Y1);
      
      // determine the maximum error
      me = 0.0;
      for (i = 0; i < X.Length; i++)
      {
        e = System.Math.Abs((X1[i] - X[i]) / X1[i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.IsTrue(me < Tolerance1, "Maximum Error = " + me.ToString());
    }
    
    #endregion Static Solve Vector 1

    #region Static Solve Vector 2
    
    [Test]
    public void StaticSolveVector2()
    {
      int i;
      double e, me;
      DoubleVector X = DoubleLevinson.Solve(LC2, TR2, Y2);
      
      // determine the maximum error
      me = 0.0;
      for (i = 0; i < X.Length; i++)
      {
        e = System.Math.Abs((X2[i] - X[i]) / X2[i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.IsTrue(me < Tolerance2, "Maximum Error = " + me.ToString());
    }
    
    #endregion Static Solve Vector 2

    #region Static Solve Vector 3
    
    [Test]
    public void StaticSolveVector3()
    {
      int i;
      double e, me;
      DoubleVector X = DoubleLevinson.Solve(LC3, TR3, Y3);
      
      // determine the maximum error
      me = 0.0;
      for (i = 0; i < X.Length; i++)
      {
        e = System.Math.Abs((X3[i] - X[i]) / X3[i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.IsTrue(me < Tolerance3, "Maximum Error = " + me.ToString());
    }
    
    #endregion Static Solve Vector 3

    #region Static Solve Vector 4
    
    [Test]
    public void StaticSolveVector4()
    {
      int i;
      double e, me;
      DoubleVector X = DoubleLevinson.Solve(LC4, TR4, Y4);
      
      // determine the maximum error
      me = 0.0;
      for (i = 0; i < X.Length; i++)
      {
        e = System.Math.Abs((X4[i] - X[i]) / X4[i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.IsTrue(me < Tolerance4, "Maximum Error = " + me.ToString());
    }
    
    #endregion Static Solve Vector 4

    #region Static Solve Vector 5
    
    [Test]
    public void StaticSolveVector5()
    {
      int i;
      double e, me;
      DoubleVector X = DoubleLevinson.Solve(LC5, TR5, Y5);
      
      // determine the maximum error
      me = 0.0;
      for (i = 0; i < X.Length; i++)
      {
        e = System.Math.Abs((X5[i] - X[i]) / X5[i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.IsTrue(me < Tolerance5, "Maximum Error = " + me.ToString());
    }
    
    #endregion Static Solve Vector 5

    #region Static Solve Vector 10
    
    [Test]
    public void StaticSolveVector10()
    {
      int i;
      double e, me;
      DoubleVector X = DoubleLevinson.Solve(LC10, TR10, Y10);
      
      // determine the maximum error
      me = 0.0;
      for (i = 0; i < X.Length; i++)
      {
        e = System.Math.Abs((X10[i] - X[i]) / X10[i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.IsTrue(me < Tolerance10, "Maximum Error = " + me.ToString());
    }
    
    #endregion Static Solve Vector 10

    #region Null Parameter Test 1 for Static SolveMatrix
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullParameterTestforStaticSolveMatrix1()
    {
      DoubleMatrix X = DoubleLevinson.Solve(null, TR10, DoubleMatrix.CreateIdentity(10));
    }

    #endregion Null Parameter Test 1 for Static SolveMatrix

    #region Null Parameter Test 2 for Static SolveMatrix
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullParameterTestforStaticSolveMatrix2()
    {
      DoubleMatrix X = DoubleLevinson.Solve(LC10, null, DoubleMatrix.CreateIdentity(10));
    }

    #endregion Null Parameter Test 2 for Static SolveMatrix

    #region Null Parameter Test 3 for Static SolveMatrix
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullParameterTestforStaticSolveMatrix3()
    {
      DoubleMatrix X = DoubleLevinson.Solve(LC10, TR10, null as DoubleMatrix);
    }

    #endregion Null Parameter Test 3 for Static SolveMatrix

    #region Zero Vector Length Test for Static SolveMatrix
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void ZeroVectorLengthTestforStaticSolveMatrix()
    {
      DoubleVector LC = new DoubleVector(1, 0.0);
      LC.RemoveAt(0);
      DoubleMatrix X = DoubleLevinson.Solve(LC, TR10, DoubleMatrix.CreateIdentity(10));
    }

    #endregion Zero Vector Length Test for Static SolveMatrix

    #region Mismatch Dimension Test 1 for Static SolveMatrix
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchDimensionTestforStaticSolveMatrix1()
    {
      DoubleMatrix X = DoubleLevinson.Solve(LC10, TR5, DoubleMatrix.CreateIdentity(5));
    }

    #endregion Mismatch Dimension Test 1 for Static SolveMatrix

    #region Mismatch Dimension Test 2 for Static SolveMatrix
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchDimensionTestforStaticSolveMatrix2()
    {
      DoubleMatrix X = DoubleLevinson.Solve(LC10, TR10, DoubleMatrix.CreateIdentity(5));
    }

    #endregion Mismatch Dimension Test 2 for Static SolveMatrix

    #region First Element Test for Static SolveMatrix

    [Test]
    [ExpectedException(typeof(System.ArithmeticException))]
    public void FirstElementTestforStaticSolveMatrix()
    {
      DoubleVector dv = new DoubleVector(3, 1.0);
      DoubleMatrix X = DoubleLevinson.Solve(dv, TR3, DoubleMatrix.CreateIdentity(3));
    }

    #endregion First Element Test for Static SolveMatrix

    #region Singular Test for Static SolveMatrix
    
    // test with Toeplitz matrix which has a singular principal sub-matrix
    [Test]
    [ExpectedException(typeof(SingularMatrixException))]
    public void SingularTestforStaticSolveMatrix()
    {
      DoubleVector dv = new DoubleVector(3, 1.0);
      DoubleMatrix X = DoubleLevinson.Solve(dv, dv, DoubleMatrix.CreateIdentity(3));
    }
    
    #endregion Singular Test for Static SolveMatrix

    #region Static Solve Matrix 1
    
    [Test]
    public void StaticSolveMatrix1()
    {
      int i, j;
      double e, me;
      DoubleMatrix I = DoubleLevinson.Solve(LC1, TR1, DoubleMatrix.CreateIdentity(1));
      
      // determine the maximum error
      me = 0.0;
      for (i = 0; i < I.ColumnLength; i++)
      {
        for (j = 0; j < I.RowLength; j++)
        {
          e = System.Math.Abs((I1[i, j] - I[i, j]) / I1[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance1, "Maximum Error = " + me.ToString());
    }
    
    #endregion Static Solve Matrix 1

    #region Static Solve Matrix 2
    
    [Test]
    public void StaticSolveMatrix2()
    {
      int i, j;
      double e, me;
      DoubleMatrix I = DoubleLevinson.Solve(LC2, TR2, DoubleMatrix.CreateIdentity(2));
      
      // determine the maximum error
      me = 0.0;
      for (i = 0; i < I.ColumnLength; i++)
      {
        for (j = 0; j < I.RowLength; j++)
        {
          e = System.Math.Abs((I2[i, j] - I[i, j]) / I2[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance2, "Maximum Error = " + me.ToString());
    }
    
    #endregion Static Solve Matrix 2

    #region Static Solve Matrix 3
    
    [Test]
    public void StaticSolveMatrix3()
    {
      int i, j;
      double e, me;
      DoubleMatrix I = DoubleLevinson.Solve(LC3, TR3, DoubleMatrix.CreateIdentity(3));
      
      // determine the maximum error
      me = 0.0;
      for (i = 0; i < I.ColumnLength; i++)
      {
        for (j = 0; j < I.RowLength; j++)
        {
          e = System.Math.Abs((I3[i, j] - I[i, j]) / I3[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance3, "Maximum Error = " + me.ToString());
    }
    
    #endregion Static Solve Matrix 3

    #region Static Solve Matrix 4
    
    [Test]
    public void StaticSolveMatrix4()
    {
      int i, j;
      double e, me;
      DoubleMatrix I = DoubleLevinson.Solve(LC4, TR4, DoubleMatrix.CreateIdentity(4));
      
      // determine the maximum error
      me = 0.0;
      for (i = 0; i < I.ColumnLength; i++)
      {
        for (j = 0; j < I.RowLength; j++)
        {
          e = System.Math.Abs((I4[i, j] - I[i, j]) / I4[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance4, "Maximum Error = " + me.ToString());
    }
    
    #endregion Static Solve Matrix 4

    #region Static Solve Matrix 5
    
    [Test]
    public void StaticSolveMatrix5()
    {
      int i, j;
      double e, me;
      DoubleMatrix I = DoubleLevinson.Solve(LC5, TR5, DoubleMatrix.CreateIdentity(5));
      
      // determine the maximum error
      me = 0.0;
      for (i = 0; i < I.ColumnLength; i++)
      {
        for (j = 0; j < I.RowLength; j++)
        {
          e = System.Math.Abs((I5[i, j] - I[i, j]) / I5[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance5, "Maximum Error = " + me.ToString());
    }
    
    #endregion Static Solve Matrix 5

    #region Static Solve Matrix 10
    
    [Test]
    public void StaticSolveMatrix10()
    {
      int i, j;
      double e, me;
      DoubleMatrix I = DoubleLevinson.Solve(LC10, TR10, DoubleMatrix.CreateIdentity(10));
      
      // determine the maximum error
      me = 0.0;
      for (i = 0; i < I.ColumnLength; i++)
      {
        for (j = 0; j < I.RowLength; j++)
        {
          e = System.Math.Abs((I10[i, j] - I[i, j]) / I10[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance10, "Maximum Error = " + me.ToString());
    }
    
    #endregion Static Solve Matrix 10

    #region Null Parameter Test 1 for Static Inverse
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullParameterTestforStaticInverse1()
    {
      DoubleMatrix X = DoubleLevinson.Inverse(null, TR10);
    }

    #endregion Null Parameter Test 1 for Static Inverse

    #region Null Parameter Test 2 for Static Inverse
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullParameterTestforStaticInverse2()
    {
      DoubleMatrix X = DoubleLevinson.Inverse(LC10, null);
    }

    #endregion Null Parameter Test 2 for Static Inverse

    #region Zero Vector Length Test for Static Inverse
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void ZeroVectorLengthTestforStaticInverse()
    {
      DoubleVector LC = new DoubleVector(1, 0.0);
      LC.RemoveAt(0);
      DoubleMatrix X = DoubleLevinson.Inverse(LC, LC);
    }

    #endregion Zero Vector Length Test for Static Inverse

    #region Mismatch Dimension Test for Static Inverse
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchDimensionTestforStaticInverse()
    {
      DoubleMatrix X = DoubleLevinson.Inverse(LC10, TR5);
    }

    #endregion Mismatch Dimension Test for Static Inverse

    #region First Element Test for Static Inverse

    [Test]
    [ExpectedException(typeof(System.ArithmeticException))]
    public void FirstElementTestforStaticInverse()
    {
      DoubleVector dv = new DoubleVector(3, 1.0);
      DoubleMatrix X = DoubleLevinson.Inverse(dv, TR3);
    }

    #endregion First Element Test for Static  Inverse

    #region Singular Test for Static Inverse
    
    // test with Toeplitz matrix which has a singular principal sub-matrix
    [Test]
    [ExpectedException(typeof(SingularMatrixException))]
    public void SingularTestforStaticInverse()
    {
      DoubleVector dv = new DoubleVector(3, 1.0);
      DoubleMatrix X = DoubleLevinson.Inverse(dv, dv);
    }
    
    #endregion Singular Test for Static Inverse

    #region Static Inverse 1

    [Test]
    public void StaticInverse1()
    {
      int i, j;
      double e, me;

      // calculate the inverse
      DoubleMatrix I = DoubleLevinson.Inverse(LC1, TR1);

      // determine the maximum relative error
      me = 0.0;
      for (i = 0; i < I.ColumnLength; i++)
      {
        for (j = 0; j < I.RowLength; j++)
        {
          e = System.Math.Abs((I1[i, j] - I[i, j]) / I1[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance1, "Maximum Error = " + me.ToString());
    }

    #endregion Static Inverse 1

    #region Static Inverse 2

    [Test]
    public void StaticInverse2()
    {
      int i, j;
      double e, me;

      // calculate the inverse
      DoubleMatrix I = DoubleLevinson.Inverse(LC2, TR2);

      // determine the maximum relative error
      me = 0.0;
      for (i = 0; i < I.ColumnLength; i++)
      {
        for (j = 0; j < I.RowLength; j++)
        {
          e = System.Math.Abs((I2[i, j] - I[i, j]) / I2[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance2, "Maximum Error = " + me.ToString());
    }

    #endregion Static Inverse 2

    #region Static Inverse 3

    [Test]
    public void StaticInverse3()
    {
      int i, j;
      double e, me;

      // calculate the inverse
      DoubleMatrix I = DoubleLevinson.Inverse(LC3, TR3);

      // determine the maximum relative error
      me = 0.0;
      for (i = 0; i < I.ColumnLength; i++)
      {
        for (j = 0; j < I.RowLength; j++)
        {
          e = System.Math.Abs((I3[i, j] - I[i, j]) / I3[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance3, "Maximum Error = " + me.ToString());
    }

    #endregion Static Inverse 3

    #region Static Inverse 4

    [Test]
    public void StaticInverse4()
    {
      int i, j;
      double e, me;

      // calculate the inverse
      DoubleMatrix I = DoubleLevinson.Inverse(LC4, TR4);

      // determine the maximum relative error
      me = 0.0;
      for (i = 0; i < I.ColumnLength; i++)
      {
        for (j = 0; j < I.RowLength; j++)
        {
          e = System.Math.Abs((I4[i, j] - I[i, j]) / I4[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance4, "Maximum Error = " + me.ToString());
    }

    #endregion Static Inverse 4

    #region Static Inverse 5

    [Test]
    public void StaticInverse5()
    {
      int i, j;
      double e, me;

      // calculate the inverse
      DoubleMatrix I = DoubleLevinson.Inverse(LC5, TR5);

      // determine the maximum relative error
      me = 0.0;
      for (i = 0; i < I.ColumnLength; i++)
      {
        for (j = 0; j < I.RowLength; j++)
        {
          e = System.Math.Abs((I5[i, j] - I[i, j]) / I5[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance5, "Maximum Error = " + me.ToString());
    }

    #endregion Static Inverse 5

    #region Static Inverse 10

    [Test]
    public void StaticInverse10()
    {
      int i, j;
      double e, me;

      // calculate the inverse
      DoubleMatrix I = DoubleLevinson.Inverse(LC10, TR10);

      // determine the maximum relative error
      me = 0.0;
      for (i = 0; i < I.ColumnLength; i++)
      {
        for (j = 0; j < I.RowLength; j++)
        {
          e = System.Math.Abs((I10[i, j] - I[i, j]) / I10[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance10, "Maximum Error = " + me.ToString());
    }

    #endregion Static Inverse 10

  }

}

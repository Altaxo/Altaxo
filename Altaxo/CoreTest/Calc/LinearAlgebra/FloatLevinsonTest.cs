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

  // suite of tests for float symmetric Levinson algorithm
  [TestFixture]
  public class FloatLevinsonTest
  {

    #region Fields

    // unit testing - order 1
    
    FloatVector TR1;      // Toeplitz matrix
    FloatVector LC1;      // Toeplitz matrix
    FloatMatrix A1;     // Lower triangle matrix
    FloatVector D1;     // diagonal vector
    FloatMatrix B1;     // upper triangle matrix
    FloatMatrix I1;     // inverse matrix
    float Det1;       // exact determinant
    FloatVector X1;     // RHS vector
    FloatVector Y1;     // LHS vector
    float Tolerance1;     // allowable tolerance

    // unit testing - order 2
    
    FloatVector TR2;      // Toeplitz matrix
    FloatVector LC2;      // Toeplitz matrix
    FloatMatrix A2;     // Lower triangle matrix
    FloatVector D2;     // diagonal vector
    FloatMatrix B2;     // upper triangle matrix
    FloatMatrix I2;     // inverse matrix
    float Det2;       // exact determinant
    FloatVector X2;     // RHS vector
    FloatVector Y2;     // LHS vector
    float Tolerance2;     // allowable tolerance

    // unit testing - order 3
    
    FloatVector TR3;      // Toeplitz matrix
    FloatVector LC3;      // Toeplitz matrix
    FloatMatrix A3;     // Lower triangle matrix
    FloatVector D3;     // diagonal vector
    FloatMatrix B3;     // upper triangle matrix
    FloatMatrix I3;     // inverse matrix
    float Det3;       // exact determinant
    FloatVector X3;     // RHS vector
    FloatVector Y3;     // LHS vector
    float Tolerance3;     // allowable tolerance

    // unit testing - order 4
    
    FloatVector TR4;      // Toeplitz matrix
    FloatVector LC4;      // Toeplitz matrix
    FloatMatrix A4;     // Lower triangle matrix
    FloatVector D4;     // diagonal vector
    FloatMatrix B4;     // upper triangle matrix
    FloatMatrix I4;     // inverse matrix
    float Det4;       // exact determinant
    FloatVector X4;     // RHS vector
    FloatVector Y4;     // LHS vector
    float Tolerance4;     // allowable tolerance

    // unit testing - order 5
    
    FloatVector TR5;      // Toeplitz matrix
    FloatVector LC5;      // Toeplitz matrix
    FloatMatrix A5;     // Lower triangle matrix
    FloatVector D5;     // diagonal vector
    FloatMatrix B5;     // upper triangle matrix
    FloatMatrix I5;     // inverse matrix
    float Det5;       // exact determinant
    FloatVector X5;     // RHS vector
    FloatVector Y5;     // LHS vector
    float Tolerance5;     // allowable tolerance

    // unit testing - order 10
    
    FloatVector TR10;     // Toeplitz matrix
    FloatVector LC10;     // Toeplitz matrix
    FloatMatrix A10;      // Lower triangle matrix
    FloatVector D10;      // diagonal vector
    FloatMatrix B10;      // upper triangle matrix
    FloatMatrix I10;      // inverse matrix
    float Det10;        // exact determinant
    FloatVector X10;      // RHS vector
    FloatVector Y10;      // LHS vector
    float Tolerance10;      // allowable tolerance

    #endregion Fields

    #region Test Fixture Setup

    [TestFixtureSetUp]
    public void SetupTestCases()
    {
      // unit testing values - order 1

      TR1 = new FloatVector(1);
      TR1[0] = +3.0000000E+000f;

      LC1 = new FloatVector(1);
      LC1[0] = +3.0000000E+000f;

      A1 = new FloatMatrix(1);
      A1[0, 0] = +1.0000000E+000f;

      D1 = new FloatVector(1);
      D1[0] = +3.3333334E-001f;

      B1 = new FloatMatrix(1);
      B1[0, 0] = +1.0000000E+000f;

      Det1 = +3.0000000E+000f;

      I1 = new FloatMatrix(1);
      I1[0, 0] = +3.3333334E-001f;

      X1 = new FloatVector(1);
      X1[0] = +1.0000000E+000f;

      Y1 = new FloatVector(1);
      Y1[0] = +3.0000000E+000f;


      // unit testing values - order 2

      TR2 = new FloatVector(2);
      TR2[0] = +3.0000000E+000f;
      TR2[1] = +2.0000000E+000f;

      LC2 = new FloatVector(2);
      LC2[0] = +3.0000000E+000f;
      LC2[1] = +1.0000000E+000f;

      A2 = new FloatMatrix(2);
      A2[0, 0] = +1.0000000E+000f;
      A2[1, 0] = -3.3333333E-001f;
      A2[1, 1] = +1.0000000E+000f;

      D2 = new FloatVector(2);
      D2[0] = +3.3333333E-001f;
      D2[1] = +4.2857143E-001f;

      B2 = new FloatMatrix(2);
      B2[0, 0] = +1.0000000E+000f;
      B2[0, 1] = -6.6666667E-001f;
      B2[1, 1] = +1.0000000E+000f;

      Det2 = +7.0000000E+000f;

      I2 = new FloatMatrix(2);
      I2[0, 0] = +4.2857143E-001f;
      I2[0, 1] = -2.8571429E-001f;
      I2[1, 0] = -1.4285714E-001f;
      I2[1, 1] = +4.2857143E-001f;

      X2 = new FloatVector(2);
      X2[0] = +1.0000000E+000f;
      X2[1] = +2.0000000E+000f;

      Y2 = new FloatVector(2);
      Y2[0] = +7.0000000E+000f;
      Y2[1] = +7.0000000E+000f;


      // unit testing values - order 3

      TR3 = new FloatVector(3);
      TR3[0] = +3.0000000E+000f;
      TR3[1] = +2.0000000E+000f;
      TR3[2] = +1.0000000E+000f;

      LC3 = new FloatVector(3);
      LC3[0] = +3.0000000E+000f;
      LC3[1] = +1.0000000E+000f;
      LC3[2] = +0.0000000E+000f;

      A3 = new FloatMatrix(3);
      A3[0, 0] = +1.0000000E+000f;
      A3[1, 0] = -3.3333333E-001f;
      A3[1, 1] = +1.0000000E+000f;
      A3[2, 0] = +1.4285714E-001f;
      A3[2, 1] = -4.2857143E-001f;
      A3[2, 2] = +1.0000000E+000f;

      D3 = new FloatVector(3);
      D3[0] = +3.3333333E-001f;
      D3[1] = +4.2857143E-001f;
      D3[2] = +4.3750000E-001f;

      B3 = new FloatMatrix(3);
      B3[0, 0] = +1.0000000E+000f;
      B3[0, 1] = -6.6666667E-001f;
      B3[1, 1] = +1.0000000E+000f;
      B3[0, 2] = +1.4285714E-001f;
      B3[1, 2] = -7.1428571E-001f;
      B3[2, 2] = +1.0000000E+000f;

      Det3 = +1.6000000E+001f;

      I3 = new FloatMatrix(3);
      I3[0, 0] = +4.3750000E-001f;
      I3[0, 1] = -3.1250000E-001f;
      I3[0, 2] = +6.2500000E-002f;
      I3[1, 0] = -1.8750000E-001f;
      I3[1, 1] = +5.6250000E-001f;
      I3[1, 2] = -3.1250000E-001f;
      I3[2, 0] = +6.2500000E-002f;
      I3[2, 1] = -1.8750000E-001f;
      I3[2, 2] = +4.3750000E-001f;

      X3 = new FloatVector(3);
      X3[0] = +1.0000000E+000f;
      X3[1] = +2.0000000E+000f;
      X3[2] = +3.0000000E+000f;

      Y3 = new FloatVector(3);
      Y3[0] = +1.0000000E+001f;
      Y3[1] = +1.3000000E+001f;
      Y3[2] = +1.1000000E+001f;


      // unit testing values - order 4

      TR4 = new FloatVector(4);
      TR4[0] = +4.0000000E+000f;
      TR4[1] = +3.0000000E+000f;
      TR4[2] = +2.0000000E+000f;
      TR4[3] = +1.0000000E+000f;

      LC4 = new FloatVector(4);
      LC4[0] = +4.0000000E+000f;
      LC4[1] = +1.0000000E+000f;
      LC4[2] = +2.0000000E+000f;
      LC4[3] = +3.0000000E+000f;

      A4 = new FloatMatrix(4);
      A4[0, 0] = +1.0000000E+000f;
      A4[1, 0] = -2.5000000E-001f;
      A4[1, 1] = +1.0000000E+000f;
      A4[2, 0] = -5.3846154E-001f;
      A4[2, 1] = +1.5384615E-001f;
      A4[2, 2] = +1.0000000E+000f;
      A4[3, 0] = -8.1818182E-001f;
      A4[3, 1] = +9.0909091E-002f;
      A4[3, 2] = +9.0909091E-002f;
      A4[3, 3] = +1.0000000E+000f;

      D4 = new FloatVector(4);
      D4[0] = +2.5000000E-001f;
      D4[1] = +3.0769231E-001f;
      D4[2] = +2.9545455E-001f;
      D4[3] = +2.7500000E-001f;

      B4 = new FloatMatrix(4);
      B4[0, 0] = +1.0000000E+000f;
      B4[0, 1] = -7.5000000E-001f;
      B4[1, 1] = +1.0000000E+000f;
      B4[0, 2] = +7.6923077E-002f;
      B4[1, 2] = -7.6923077E-001f;
      B4[2, 2] = +1.0000000E+000f;
      B4[0, 3] = +9.0909091E-002f;
      B4[1, 3] = +9.0909091E-002f;
      B4[2, 3] = -8.1818182E-001f;
      B4[3, 3] = +1.0000000E+000f;

      Det4 = +1.6000000E+002f;

      I4 = new FloatMatrix(4);
      I4[0, 0] = +2.7500000E-001f;
      I4[0, 1] = -2.2500000E-001f;
      I4[0, 2] = +2.5000000E-002f;
      I4[0, 3] = +2.5000000E-002f;
      I4[1, 0] = +2.5000000E-002f;
      I4[1, 1] = +2.7500000E-001f;
      I4[1, 2] = -2.2500000E-001f;
      I4[1, 3] = +2.5000000E-002f;
      I4[2, 0] = +2.5000000E-002f;
      I4[2, 1] = +2.5000000E-002f;
      I4[2, 2] = +2.7500000E-001f;
      I4[2, 3] = -2.2500000E-001f;
      I4[3, 0] = -2.2500000E-001f;
      I4[3, 1] = +2.5000000E-002f;
      I4[3, 2] = +2.5000000E-002f;
      I4[3, 3] = +2.7500000E-001f;

      X4 = new FloatVector(4);
      X4[0] = +1.0000000E+000f;
      X4[1] = +2.0000000E+000f;
      X4[2] = +3.0000000E+000f;
      X4[3] = +4.0000000E+000f;

      Y4 = new FloatVector(4);
      Y4[0] = +2.0000000E+001f;
      Y4[1] = +2.6000000E+001f;
      Y4[2] = +2.8000000E+001f;
      Y4[3] = +2.6000000E+001f;


      // unit testing values - order 5

      TR5 = new FloatVector(5);
      TR5[0] = +5.0000000E+000f;
      TR5[1] = +4.0000000E+000f;
      TR5[2] = +3.0000000E+000f;
      TR5[3] = +2.0000000E+000f;
      TR5[4] = +1.0000000E+000f;

      LC5 = new FloatVector(5);
      LC5[0] = +5.0000000E+000f;
      LC5[1] = +1.0000000E+000f;
      LC5[2] = +2.0000000E+000f;
      LC5[3] = +3.0000000E+000f;
      LC5[4] = +4.0000000E+000f;

      A5 = new FloatMatrix(5);
      A5[0, 0] = +1.0000000E+000f;
      A5[1, 0] = -2.0000000E-001f;
      A5[1, 1] = +1.0000000E+000f;
      A5[2, 0] = -4.2857143E-001f;
      A5[2, 1] = +1.4285714E-001f;
      A5[2, 2] = +1.0000000E+000f;
      A5[3, 0] = -6.6666667E-001f;
      A5[3, 1] = +1.1111111E-001f;
      A5[3, 2] = +1.1111111E-001f;
      A5[3, 3] = +1.0000000E+000f;
      A5[4, 0] = -8.7500000E-001f;
      A5[4, 1] = +6.2500000E-002f;
      A5[4, 2] = +6.2500000E-002f;
      A5[4, 3] = +6.2500000E-002f;
      A5[4, 4] = +1.0000000E+000f;

      D5 = new FloatVector(5);
      D5[0] = +2.0000000E-001f;
      D5[1] = +2.3809524E-001f;
      D5[2] = +2.3333333E-001f;
      D5[3] = +2.2500000E-001f;
      D5[4] = +2.1333333E-001f;

      B5 = new FloatMatrix(5);
      B5[0, 0] = +1.0000000E+000f;
      B5[0, 1] = -8.0000000E-001f;
      B5[1, 1] = +1.0000000E+000f;
      B5[0, 2] = +4.7619048E-002f;
      B5[1, 2] = -8.0952381E-001f;
      B5[2, 2] = +1.0000000E+000f;
      B5[0, 3] = +5.5555556E-002f;
      B5[1, 3] = +5.5555556E-002f;
      B5[2, 3] = -8.3333333E-001f;
      B5[3, 3] = +1.0000000E+000f;
      B5[0, 4] = +6.2500000E-002f;
      B5[1, 4] = +6.2500000E-002f;
      B5[2, 4] = +6.2500000E-002f;
      B5[3, 4] = -8.7500000E-001f;
      B5[4, 4] = +1.0000000E+000f;

      Det5 = +1.8750000E+003f;

      I5 = new FloatMatrix(5);
      I5[0, 0] = +2.1333333E-001f;
      I5[0, 1] = -1.8666667E-001f;
      I5[0, 2] = +1.3333333E-002f;
      I5[0, 3] = +1.3333333E-002f;
      I5[0, 4] = +1.3333333E-002f;
      I5[1, 0] = +1.3333333E-002f;
      I5[1, 1] = +2.1333333E-001f;
      I5[1, 2] = -1.8666667E-001f;
      I5[1, 3] = +1.3333333E-002f;
      I5[1, 4] = +1.3333333E-002f;
      I5[2, 0] = +1.3333333E-002f;
      I5[2, 1] = +1.3333333E-002f;
      I5[2, 2] = +2.1333333E-001f;
      I5[2, 3] = -1.8666667E-001f;
      I5[2, 4] = +1.3333333E-002f;
      I5[3, 0] = +1.3333333E-002f;
      I5[3, 1] = +1.3333333E-002f;
      I5[3, 2] = +1.3333333E-002f;
      I5[3, 3] = +2.1333333E-001f;
      I5[3, 4] = -1.8666667E-001f;
      I5[4, 0] = -1.8666667E-001f;
      I5[4, 1] = +1.3333333E-002f;
      I5[4, 2] = +1.3333333E-002f;
      I5[4, 3] = +1.3333333E-002f;
      I5[4, 4] = +2.1333333E-001f;

      X5 = new FloatVector(5);
      X5[0] = +1.0000000E+000f;
      X5[1] = +2.0000000E+000f;
      X5[2] = +3.0000000E+000f;
      X5[3] = +4.0000000E+000f;
      X5[4] = +5.0000000E+000f;

      Y5 = new FloatVector(5);
      Y5[0] = +3.5000000E+001f;
      Y5[1] = +4.5000000E+001f;
      Y5[2] = +5.0000000E+001f;
      Y5[3] = +5.0000000E+001f;
      Y5[4] = +4.5000000E+001f;


      // unit testing values - order 10

      TR10 = new FloatVector(10);
      TR10[0] = +1.0000000E+001f;
      TR10[1] = +9.0000000E+000f;
      TR10[2] = +8.0000000E+000f;
      TR10[3] = +7.0000000E+000f;
      TR10[4] = +6.0000000E+000f;
      TR10[5] = +5.0000000E+000f;
      TR10[6] = +4.0000000E+000f;
      TR10[7] = +3.0000000E+000f;
      TR10[8] = +2.0000000E+000f;
      TR10[9] = +1.0000000E+000f;

      LC10 = new FloatVector(10);
      LC10[0] = +1.0000000E+001f;
      LC10[1] = +1.0000000E+000f;
      LC10[2] = +2.0000000E+000f;
      LC10[3] = +3.0000000E+000f;
      LC10[4] = +4.0000000E+000f;
      LC10[5] = +5.0000000E+000f;
      LC10[6] = +6.0000000E+000f;
      LC10[7] = +7.0000000E+000f;
      LC10[8] = +8.0000000E+000f;
      LC10[9] = +9.0000000E+000f;

      A10 = new FloatMatrix(10);
      A10[0, 0] = +1.0000000E+000f;
      A10[1, 0] = -1.0000000E-001f;
      A10[1, 1] = +1.0000000E+000f;
      A10[2, 0] = -2.0879121E-001f;
      A10[2, 1] = +8.7912090E-002f;
      A10[2, 2] = +1.0000000E+000f;
      A10[3, 0] = -3.2530120E-001f;
      A10[3, 1] = +8.4337346E-002f;
      A10[3, 2] = +8.4337346E-002f;
      A10[3, 3] = +1.0000000E+000f;
      A10[4, 0] = -4.4736841E-001f;
      A10[4, 1] = +7.8947365E-002f;
      A10[4, 2] = +7.8947365E-002f;
      A10[4, 3] = +7.8947365E-002f;
      A10[4, 4] = +1.0000000E+000f;
      A10[5, 0] = -5.7142860E-001f;
      A10[5, 1] = +7.1428575E-002f;
      A10[5, 2] = +7.1428575E-002f;
      A10[5, 3] = +7.1428575E-002f;
      A10[5, 4] = +7.1428575E-002f;
      A10[5, 5] = +1.0000000E+000f;
      A10[6, 0] = -6.9230771E-001f;
      A10[6, 1] = +6.1538462E-002f;
      A10[6, 2] = +6.1538462E-002f;
      A10[6, 3] = +6.1538462E-002f;
      A10[6, 4] = +6.1538462E-002f;
      A10[6, 5] = +6.1538462E-002f;
      A10[6, 6] = +1.0000000E+000f;
      A10[7, 0] = -8.0327868E-001f;
      A10[7, 1] = +4.9180329E-002f;
      A10[7, 2] = +4.9180329E-002f;
      A10[7, 3] = +4.9180329E-002f;
      A10[7, 4] = +4.9180329E-002f;
      A10[7, 5] = +4.9180329E-002f;
      A10[7, 6] = +4.9180329E-002f;
      A10[7, 7] = +1.0000000E+000f;
      A10[8, 0] = -8.9655173E-001f;
      A10[8, 1] = +3.4482758E-002f;
      A10[8, 2] = +3.4482758E-002f;
      A10[8, 3] = +3.4482758E-002f;
      A10[8, 4] = +3.4482758E-002f;
      A10[8, 5] = +3.4482758E-002f;
      A10[8, 6] = +3.4482758E-002f;
      A10[8, 7] = +3.4482758E-002f;
      A10[8, 8] = +1.0000000E+000f;
      A10[9, 0] = -9.6428573E-001f;
      A10[9, 1] = +1.7857144E-002f;
      A10[9, 2] = +1.7857144E-002f;
      A10[9, 3] = +1.7857144E-002f;
      A10[9, 4] = +1.7857144E-002f;
      A10[9, 5] = +1.7857144E-002f;
      A10[9, 6] = +1.7857144E-002f;
      A10[9, 7] = +1.7857144E-002f;
      A10[9, 8] = +1.7857144E-002f;
      A10[9, 9] = +1.0000000E+000f;

      D10 = new FloatVector(10);
      D10[0] = +1.0000000E-001f;
      D10[1] = +1.0989011E-001f;
      D10[2] = +1.0963856E-001f;
      D10[3] = +1.0921053E-001f;
      D10[4] = +1.0857143E-001f;
      D10[5] = +1.0769231E-001f;
      D10[6] = +1.0655738E-001f;
      D10[7] = +1.0517241E-001f;
      D10[8] = +1.0357143E-001f;
      D10[9] = +1.0181818E-001f;

      B10 = new FloatMatrix(10);
      B10[0, 0] = +1.0000000E+000f;
      B10[0, 1] = -8.9999998E-001f;
      B10[1, 1] = +1.0000000E+000f;
      B10[0, 2] = +1.0989011E-002f;
      B10[1, 2] = -9.0109891E-001f;
      B10[2, 2] = +1.0000000E+000f;
      B10[0, 3] = +1.2048192E-002f;
      B10[1, 3] = +1.2048192E-002f;
      B10[2, 3] = -9.0361446E-001f;
      B10[3, 3] = +1.0000000E+000f;
      B10[0, 4] = +1.3157895E-002f;
      B10[1, 4] = +1.3157895E-002f;
      B10[2, 4] = +1.3157895E-002f;
      B10[3, 4] = -9.0789473E-001f;
      B10[4, 4] = +1.0000000E+000f;
      B10[0, 5] = +1.4285714E-002f;
      B10[1, 5] = +1.4285714E-002f;
      B10[2, 5] = +1.4285714E-002f;
      B10[3, 5] = +1.4285714E-002f;
      B10[4, 5] = -9.1428572E-001f;
      B10[5, 5] = +1.0000000E+000f;
      B10[0, 6] = +1.5384615E-002f;
      B10[1, 6] = +1.5384615E-002f;
      B10[2, 6] = +1.5384615E-002f;
      B10[3, 6] = +1.5384615E-002f;
      B10[4, 6] = +1.5384615E-002f;
      B10[5, 6] = -9.2307693E-001f;
      B10[6, 6] = +1.0000000E+000f;
      B10[0, 7] = +1.6393442E-002f;
      B10[1, 7] = +1.6393442E-002f;
      B10[2, 7] = +1.6393442E-002f;
      B10[3, 7] = +1.6393442E-002f;
      B10[4, 7] = +1.6393442E-002f;
      B10[5, 7] = +1.6393442E-002f;
      B10[6, 7] = -9.3442625E-001f;
      B10[7, 7] = +1.0000000E+000f;
      B10[0, 8] = +1.7241379E-002f;
      B10[1, 8] = +1.7241379E-002f;
      B10[2, 8] = +1.7241379E-002f;
      B10[3, 8] = +1.7241379E-002f;
      B10[4, 8] = +1.7241379E-002f;
      B10[5, 8] = +1.7241379E-002f;
      B10[6, 8] = +1.7241379E-002f;
      B10[7, 8] = -9.4827586E-001f;
      B10[8, 8] = +1.0000000E+000f;
      B10[0, 9] = +1.7857144E-002f;
      B10[1, 9] = +1.7857144E-002f;
      B10[2, 9] = +1.7857144E-002f;
      B10[3, 9] = +1.7857144E-002f;
      B10[4, 9] = +1.7857144E-002f;
      B10[5, 9] = +1.7857144E-002f;
      B10[6, 9] = +1.7857144E-002f;
      B10[7, 9] = +1.7857144E-002f;
      B10[8, 9] = -9.6428573E-001f;
      B10[9, 9] = +1.0000000E+000f;

      Det10 = +5.5000003E+009f;

      I10 = new FloatMatrix(10);
      I10[0, 0] = +1.0181818E-001f;
      I10[0, 1] = -9.8181821E-002f;
      I10[0, 2] = +1.8181818E-003f;
      I10[0, 3] = +1.8181818E-003f;
      I10[0, 4] = +1.8181818E-003f;
      I10[0, 5] = +1.8181818E-003f;
      I10[0, 6] = +1.8181818E-003f;
      I10[0, 7] = +1.8181818E-003f;
      I10[0, 8] = +1.8181818E-003f;
      I10[0, 9] = +1.8181818E-003f;
      I10[1, 0] = +1.8181818E-003f;
      I10[1, 1] = +1.0181818E-001f;
      I10[1, 2] = -9.8181821E-002f;
      I10[1, 3] = +1.8181818E-003f;
      I10[1, 4] = +1.8181818E-003f;
      I10[1, 5] = +1.8181818E-003f;
      I10[1, 6] = +1.8181818E-003f;
      I10[1, 7] = +1.8181818E-003f;
      I10[1, 8] = +1.8181818E-003f;
      I10[1, 9] = +1.8181818E-003f;
      I10[2, 0] = +1.8181818E-003f;
      I10[2, 1] = +1.8181818E-003f;
      I10[2, 2] = +1.0181818E-001f;
      I10[2, 3] = -9.8181821E-002f;
      I10[2, 4] = +1.8181818E-003f;
      I10[2, 5] = +1.8181818E-003f;
      I10[2, 6] = +1.8181818E-003f;
      I10[2, 7] = +1.8181818E-003f;
      I10[2, 8] = +1.8181818E-003f;
      I10[2, 9] = +1.8181818E-003f;
      I10[3, 0] = +1.8181818E-003f;
      I10[3, 1] = +1.8181818E-003f;
      I10[3, 2] = +1.8181818E-003f;
      I10[3, 3] = +1.0181818E-001f;
      I10[3, 4] = -9.8181821E-002f;
      I10[3, 5] = +1.8181818E-003f;
      I10[3, 6] = +1.8181818E-003f;
      I10[3, 7] = +1.8181818E-003f;
      I10[3, 8] = +1.8181818E-003f;
      I10[3, 9] = +1.8181818E-003f;
      I10[4, 0] = +1.8181818E-003f;
      I10[4, 1] = +1.8181818E-003f;
      I10[4, 2] = +1.8181818E-003f;
      I10[4, 3] = +1.8181818E-003f;
      I10[4, 4] = +1.0181818E-001f;
      I10[4, 5] = -9.8181821E-002f;
      I10[4, 6] = +1.8181818E-003f;
      I10[4, 7] = +1.8181818E-003f;
      I10[4, 8] = +1.8181818E-003f;
      I10[4, 9] = +1.8181818E-003f;
      I10[5, 0] = +1.8181818E-003f;
      I10[5, 1] = +1.8181818E-003f;
      I10[5, 2] = +1.8181818E-003f;
      I10[5, 3] = +1.8181818E-003f;
      I10[5, 4] = +1.8181818E-003f;
      I10[5, 5] = +1.0181818E-001f;
      I10[5, 6] = -9.8181821E-002f;
      I10[5, 7] = +1.8181818E-003f;
      I10[5, 8] = +1.8181818E-003f;
      I10[5, 9] = +1.8181818E-003f;
      I10[6, 0] = +1.8181818E-003f;
      I10[6, 1] = +1.8181818E-003f;
      I10[6, 2] = +1.8181818E-003f;
      I10[6, 3] = +1.8181818E-003f;
      I10[6, 4] = +1.8181818E-003f;
      I10[6, 5] = +1.8181818E-003f;
      I10[6, 6] = +1.0181818E-001f;
      I10[6, 7] = -9.8181821E-002f;
      I10[6, 8] = +1.8181818E-003f;
      I10[6, 9] = +1.8181818E-003f;
      I10[7, 0] = +1.8181818E-003f;
      I10[7, 1] = +1.8181818E-003f;
      I10[7, 2] = +1.8181818E-003f;
      I10[7, 3] = +1.8181818E-003f;
      I10[7, 4] = +1.8181818E-003f;
      I10[7, 5] = +1.8181818E-003f;
      I10[7, 6] = +1.8181818E-003f;
      I10[7, 7] = +1.0181818E-001f;
      I10[7, 8] = -9.8181821E-002f;
      I10[7, 9] = +1.8181818E-003f;
      I10[8, 0] = +1.8181818E-003f;
      I10[8, 1] = +1.8181818E-003f;
      I10[8, 2] = +1.8181818E-003f;
      I10[8, 3] = +1.8181818E-003f;
      I10[8, 4] = +1.8181818E-003f;
      I10[8, 5] = +1.8181818E-003f;
      I10[8, 6] = +1.8181818E-003f;
      I10[8, 7] = +1.8181818E-003f;
      I10[8, 8] = +1.0181818E-001f;
      I10[8, 9] = -9.8181821E-002f;
      I10[9, 0] = -9.8181821E-002f;
      I10[9, 1] = +1.8181818E-003f;
      I10[9, 2] = +1.8181818E-003f;
      I10[9, 3] = +1.8181818E-003f;
      I10[9, 4] = +1.8181818E-003f;
      I10[9, 5] = +1.8181818E-003f;
      I10[9, 6] = +1.8181818E-003f;
      I10[9, 7] = +1.8181818E-003f;
      I10[9, 8] = +1.8181818E-003f;
      I10[9, 9] = +1.0181818E-001f;

      X10 = new FloatVector(10);
      X10[0] = +1.0000000E+000f;
      X10[1] = +2.0000000E+000f;
      X10[2] = +3.0000000E+000f;
      X10[3] = +4.0000000E+000f;
      X10[4] = +5.0000000E+000f;
      X10[5] = +6.0000000E+000f;
      X10[6] = +7.0000000E+000f;
      X10[7] = +8.0000000E+000f;
      X10[8] = +9.0000000E+000f;
      X10[9] = +1.0000000E+001f;

      Y10 = new FloatVector(10);
      Y10[0] = +2.2000000E+002f;
      Y10[1] = +2.6500000E+002f;
      Y10[2] = +3.0000000E+002f;
      Y10[3] = +3.2500000E+002f;
      Y10[4] = +3.4000000E+002f;
      Y10[5] = +3.4500000E+002f;
      Y10[6] = +3.4000000E+002f;
      Y10[7] = +3.2500000E+002f;
      Y10[8] = +3.0000000E+002f;
      Y10[9] = +2.6500000E+002f;

      // Tolerances
      Tolerance1 = 1.000E-06f;
      Tolerance2 = 2.000E-06f;
      Tolerance3 = 3.000E-06f;
      Tolerance4 = 4.000E-06f;
      Tolerance5 = 5.000E-05f;
      Tolerance10 = 4.000E-05f;
    }

    #endregion Test Fixture Setup

    #region Null Parameter Tests for Constructor 
    
    // Test constructor with a null parameter
    [Test]
    [ExpectedException(typeof(System.ArgumentNullException))]
    public void NullParameterTestforConstructor1()
    {
      FloatLevinson fl = new FloatLevinson(null as FloatVector, TR5);
    }

    // Test constructor with a null parameter
    [Test]
    [ExpectedException(typeof(System.ArgumentNullException))]
    public void NullParameterTestforConstructor2()
    {
      FloatLevinson fl = new FloatLevinson(LC5, null as FloatVector);
    }
    
    [Test]
    [ExpectedException(typeof(System.ArgumentNullException))]
    public void NullParameterTestforConstructor3()
    {
      FloatLevinson fl = new FloatLevinson(null as ROFloatVector, TR5.ToArray());
    }

    // Test constructor with a null parameter
    [Test]
    [ExpectedException(typeof(System.ArgumentNullException))]
    public void NullParameterTestforConstructor4()
    {
      FloatLevinson fl = new FloatLevinson(LC5.ToArray(), null as ROFloatVector);
    }
    
    #endregion  Null Parameter Tests for Constructor 

    #region Zero Length Vector Tests for Constructor
    
    // Test constructor with a zero length vector parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void ZeroLengthVectorTestsforConstructor1()
    {
      FloatVector fv = new FloatVector(1, 0.0f);
      fv.RemoveAt(0);
      FloatLevinson fl = new FloatLevinson(fv, fv);
    }

    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void ZeroLengthVectorTestsforConstructor2()
    {
      float[] fv = new float[0];
      FloatLevinson fl = new FloatLevinson(fv, fv);
    }
    
    #endregion Zero Length Vector Tests for Constructor

    #region Mismatching Vector Length Tests for Constructor

    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchVectorLengthTestsforConstructor1()
    {
      FloatLevinson fl = new FloatLevinson(LC2, TR3);
    }

    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchVectorLengthTestsforConstructor2()
    {
      FloatLevinson fl = new FloatLevinson(LC2.ToArray(), TR3.ToArray());
    }
    
    #endregion Mismatching Vector Length Tests for Constructor

    #region First Element Test for Constructor

    [Test]
    [ExpectedException(typeof(System.ArithmeticException))]
    public void FirstElementTestforConstructor1()
    {
      FloatVector fv = new FloatVector(3, 1.0f);
      FloatLevinson fl = new FloatLevinson(LC3, fv);
    }

    [Test]
    [ExpectedException(typeof(System.ArithmeticException))]
    public void FirstElementTestforConstructor2()
    {
      FloatVector fv = new FloatVector(3, 1.0f);
      FloatLevinson fl = new FloatLevinson(LC3.ToArray(), fv.ToArray());
    }

    #endregion First Element Test for Constructor

    #region Get Vector Member Tests

    // check get vector
    [Test]
    public void GetLeftColumnTest()
    {
      FloatLevinson fl = new FloatLevinson(LC5, TR5);
      FloatVector LC = fl.GetLeftColumn();
      Assert.IsTrue(LC5.Equals(LC));
    }
    
    // check get vector
    [Test]
    public void GetTopRowTest()
    {
      FloatLevinson fl = new FloatLevinson(LC5, TR5);
      FloatVector TR = fl.GetTopRow();
      Assert.IsTrue(TR5.Equals(TR));
    }
    
    #endregion Get Vector Member Tests

    #region GetMatrix Member Test
    
    // check get matrix
    [Test]
    public void GetMatrixMemberTest()
    {
      FloatLevinson fl = new FloatLevinson(LC5, TR5);
      FloatMatrix flfm = fl.GetMatrix();
      for (int row = 0; row < TR5.Length; row++)
      {
        for (int column = 0; column < TR5.Length; column++)
        {
          if (column < row)
          {
            Assert.IsTrue(flfm[row, column] == LC5[row - column]);
          }
          else
          {
            Assert.IsTrue(flfm[row, column] == TR5[column - row]);
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
      FloatLevinson fl = new FloatLevinson(LC5, TR5);
      Assert.IsTrue(fl.Order == 5);
    }

    #endregion Order Property Test

    #region Decomposition Test 1

    // test the UDL factorisation for case 1

    [Test]
    public void DecompositionTest1()
    {
      int i, j;
      float e, me;
      FloatLevinson fl = new FloatLevinson(LC1, TR1);
      FloatMatrix U = fl.U;
      FloatMatrix D = fl.D;
      FloatMatrix L = fl.L;
      
      // check the upper triangle
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
      {
        for (j = 0; j < fl.Order; j++)
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
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
      {
        for (j = 0; j < fl.Order; j++)
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
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
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
      float e, me;
      FloatLevinson fl = new FloatLevinson(LC2, TR2);
      FloatMatrix U = fl.U;
      FloatMatrix D = fl.D;
      FloatMatrix L = fl.L;
      
      // check the upper triangle
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
      {
        for (j = 0; j < fl.Order; j++)
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
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
      {
        for (j = 0; j < fl.Order; j++)
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
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
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
      float e, me;
      FloatLevinson fl = new FloatLevinson(LC3, TR3);
      FloatMatrix U = fl.U;
      FloatMatrix D = fl.D;
      FloatMatrix L = fl.L;
      
      // check the upper triangle
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
      {
        for (j = 0; j < fl.Order; j++)
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
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
      {
        for (j = 0; j < fl.Order; j++)
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
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
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
      float e, me;
      FloatLevinson fl = new FloatLevinson(LC4, TR4);
      FloatMatrix U = fl.U;
      FloatMatrix D = fl.D;
      FloatMatrix L = fl.L;
      
      // check the upper triangle
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
      {
        for (j = 0; j < fl.Order; j++)
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
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
      {
        for (j = 0; j < fl.Order; j++)
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
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
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
      float e, me;
      FloatLevinson fl = new FloatLevinson(LC5, TR5);
      FloatMatrix U = fl.U;
      FloatMatrix D = fl.D;
      FloatMatrix L = fl.L;
      
      // check the upper triangle
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
      {
        for (j = 0; j < fl.Order; j++)
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
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
      {
        for (j = 0; j < fl.Order; j++)
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
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
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
      float e, me;
      FloatLevinson fl = new FloatLevinson(LC10, TR10);
      FloatMatrix U = fl.U;
      FloatMatrix D = fl.D;
      FloatMatrix L = fl.L;
      
      // check the upper triangle
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
      {
        for (j = 0; j < fl.Order; j++)
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
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
      {
        for (j = 0; j < fl.Order; j++)
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
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
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
      FloatLevinson fl = new FloatLevinson(LC4,TR4);
      Assert.IsFalse(fl.IsSingular);
    }

    #endregion Singularity Property Test 1

    #region Singularity Property Test 2
    // check that singular matrix is detected
    [Test]
    public void SingularityPropertyTest2()
    {
      FloatVector LC = new FloatVector(new float[]{4.0f, 2.0f, 1.0f, 0.0f});
      FloatVector TR = new FloatVector(new float[]{4.0f, 8.0f, 2.0f, 1.0f});

      FloatLevinson fl = new FloatLevinson(LC,TR);
      Assert.IsTrue(fl.IsSingular);
    }

    #endregion Singularity Property Test 2

    #region GetDeterminant Method Test 1
    
    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest1()
    {
      // calculate determinant from diagonal
      FloatLevinson fl = new FloatLevinson(LC1, TR1);

      // check results match
      float e = System.Math.Abs( (fl.GetDeterminant() - Det1)/Det1 );
      Assert.IsTrue(e < Tolerance1);
    }
    
    #endregion GetDeterminant Method Test 1

    #region GetDeterminant Method Test 2
    
    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest2()
    {
      // calculate determinant from diagonal
      FloatLevinson fl = new FloatLevinson(LC2, TR2);

      // check results match
      float e = System.Math.Abs( (fl.GetDeterminant() - Det2)/Det2 );
      Assert.IsTrue(e < Tolerance2);
    }
    
    #endregion GetDeterminant Method Test 2

    #region GetDeterminant Method Test 3
    
    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest3()
    {
      // calculate determinant from diagonal
      FloatLevinson fl = new FloatLevinson(LC3, TR3);

      // check results match
      float e = System.Math.Abs( (fl.GetDeterminant() - Det3)/Det3 );
      Assert.IsTrue(e < Tolerance3);
    }
    
    #endregion GetDeterminant Method Test 3

    #region GetDeterminant Method Test 4
    
    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest4()
    {
      // calculate determinant from diagonal
      FloatLevinson fl = new FloatLevinson(LC4, TR4);

      // check results match
      float e = System.Math.Abs( (fl.GetDeterminant() - Det4)/Det4 );
      Assert.IsTrue(e < Tolerance4);
    }
    
    #endregion GetDeterminant Method Test 4

    #region GetDeterminant Method Test 5
    
    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest5()
    {
      // calculate determinant from diagonal
      FloatLevinson fl = new FloatLevinson(LC5, TR5);

      // check results match
      float e = System.Math.Abs( (fl.GetDeterminant() - Det5)/Det5 );
      Assert.IsTrue(e < Tolerance5);
    }
    
    #endregion GetDeterminant Method Test 5

    #region GetDeterminant Method Test 10
    
    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest10()
    {
      // calculate determinant from diagonal
      FloatLevinson fl = new FloatLevinson(LC10, TR10);

      // check results match
      float e = System.Math.Abs( (fl.GetDeterminant() - Det10)/Det10 );
      Assert.IsTrue(e < Tolerance10);
    }
    
    #endregion GetDeterminant Method Test 10

    #region Null Parameter Test for SolveVector

    [Test]
    [ExpectedException(typeof(System.ArgumentNullException))]
    public void NullParameterTestforSolveVector()
    {
      FloatLevinson fl = new FloatLevinson(LC10, TR10);
      FloatVector X = fl.Solve(null as FloatVector);
    }

    #endregion Null Parameter Test for SolveVector

    #region Mismatch Rows Test for SolveVector

    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchRowsTestforSolveVector()
    {
      FloatLevinson fl = new FloatLevinson(LC10, TR10);
      FloatVector X = fl.Solve(X5);
    }

    #endregion Mismatch Rows Test for SolveVector

    #region SolveVector 1

    // Test solving a linear system
    [Test]
    public void SolveVector1()
    {
      int i;
      float e, me;
      FloatLevinson fl = new FloatLevinson(LC1, TR1);
      FloatVector X = fl.Solve(Y1);
      
      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
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
      float e, me;
      FloatLevinson fl = new FloatLevinson(LC2, TR2);
      FloatVector X = fl.Solve(Y2);
      
      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
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
      float e, me;
      FloatLevinson fl = new FloatLevinson(LC3, TR3);
      FloatVector X = fl.Solve(Y3);
      
      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
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
      float e, me;
      FloatLevinson fl = new FloatLevinson(LC4, TR4);
      FloatVector X = fl.Solve(Y4);
      
      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
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
      float e, me;
      FloatLevinson fl = new FloatLevinson(LC5, TR5);
      FloatVector X = fl.Solve(Y5);
      
      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
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
      float e, me;
      FloatLevinson fl = new FloatLevinson(LC10, TR10);
      FloatVector X = fl.Solve(Y10);
      
      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
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
      FloatLevinson fl = new FloatLevinson(LC10, TR10);
      FloatMatrix X = fl.Solve(null as FloatMatrix);
    }

    #endregion Null Parameter Test for SolveMatrix

    #region Mismatch Rows Test for SolveMatrix

    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchRowsTestforSolveMatrix()
    {
      FloatLevinson fl = new FloatLevinson(LC10, TR10);
      FloatMatrix X = fl.Solve(I5);
    }

    #endregion Mismatch Rows Test for SolveMatrix

    #region Solve Matrix 1

    // calculate inverse by solving linear equations with identity RHS
    [Test]
    public void SolveMatrix1()
    {
      int i, j;
      float e, me;
      FloatLevinson fl = new FloatLevinson(LC1, TR1);

      // check inverse
      FloatMatrix I = fl.Solve(FloatMatrix.CreateIdentity(1));
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
      {
        for (j = 0; j < fl.Order; j++)
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
      float e, me;
      FloatLevinson fl = new FloatLevinson(LC2, TR2);

      // check inverse
      FloatMatrix I = fl.Solve(FloatMatrix.CreateIdentity(2));
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
      {
        for (j = 0; j < fl.Order; j++)
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
      float e, me;
      FloatLevinson fl = new FloatLevinson(LC3, TR3);

      // check inverse
      FloatMatrix I = fl.Solve(FloatMatrix.CreateIdentity(3));
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
      {
        for (j = 0; j < fl.Order; j++)
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
      float e, me;
      FloatLevinson fl = new FloatLevinson(LC4, TR4);

      // check inverse
      FloatMatrix I = fl.Solve(FloatMatrix.CreateIdentity(4));
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
      {
        for (j = 0; j < fl.Order; j++)
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
      float e, me;
      FloatLevinson fl = new FloatLevinson(LC5, TR5);

      // check inverse
      FloatMatrix I = fl.Solve(FloatMatrix.CreateIdentity(5));
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
      {
        for (j = 0; j < fl.Order; j++)
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
      float e, me;
      FloatLevinson fl = new FloatLevinson(LC10, TR10);

      // check inverse
      FloatMatrix I = fl.Solve(FloatMatrix.CreateIdentity(10));
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
      {
        for (j = 0; j < fl.Order; j++)
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
      float e, me;
      FloatLevinson fl = new FloatLevinson(LC1, TR1);

      // check inverse
      FloatMatrix I = fl.GetInverse();
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
      {
        for (j = 0; j < fl.Order; j++)
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
      float e, me;
      FloatLevinson fl = new FloatLevinson(LC2, TR2);

      // check inverse
      FloatMatrix I = fl.GetInverse();
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
      {
        for (j = 0; j < fl.Order; j++)
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
      float e, me;
      FloatLevinson fl = new FloatLevinson(LC3, TR3);

      // check inverse
      FloatMatrix I = fl.GetInverse();
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
      {
        for (j = 0; j < fl.Order; j++)
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
      float e, me;
      FloatLevinson fl = new FloatLevinson(LC4, TR4);

      // check inverse
      FloatMatrix I = fl.GetInverse();
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
      {
        for (j = 0; j < fl.Order; j++)
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
      float e, me;
      FloatLevinson fl = new FloatLevinson(LC5, TR5);

      // check inverse
      FloatMatrix I = fl.GetInverse();
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
      {
        for (j = 0; j < fl.Order; j++)
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
      float e, me;
      FloatLevinson fl = new FloatLevinson(LC10, TR10);

      // check inverse
      FloatMatrix I = fl.GetInverse();
      me = 0.0f;
      for (i = 0; i < fl.Order; i++)
      {
        for (j = 0; j < fl.Order; j++)
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
      FloatVector X = FloatLevinson.Solve(null, TR10, Y10);
    }

    #endregion Null Parameter Test 1 for Static SolveVector

    #region Null Parameter Test 2 for Static SolveVector
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullParameterTestforStaticSolveVector2()
    {
      FloatVector X = FloatLevinson.Solve(LC10, null, Y10);
    }

    #endregion Null Parameter Test 2 for Static SolveVector

    #region Null Parameter Test 3 for Static SolveVector
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullParameterTestforStaticSolveVector3()
    {
      FloatVector X = FloatLevinson.Solve(LC10, TR10, null as FloatVector);
    }

    #endregion Null Parameter Test 3 for Static SolveVector

    #region Zero Vector Length Test for Static SolveVector
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void ZeroVectorLengthTestforStaticSolveVector()
    {
      FloatVector LC = new FloatVector(1, 0.0f);
      LC.RemoveAt(0);
      FloatVector X = FloatLevinson.Solve(LC, TR10, Y10);
    }

    #endregion Zero Vector Length Test for Static SolveVector

    #region Mismatch Dimension Test 1 for Static SolveVector
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchDimensionTestforStaticSolveVector1()
    {
      FloatVector X = FloatLevinson.Solve(LC10, TR5, Y5);
    }

    #endregion Mismatch Dimension Test 1 for Static SolveVector

    #region Mismatch Dimension Test 2 for Static SolveVector
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchDimensionTestforStaticSolveVector2()
    {
      FloatVector X = FloatLevinson.Solve(LC10, TR10, Y5);
    }

    #endregion Mismatch Dimension Test 2 for Static SolveVector

    #region First Element Test for Static SolveVector

    [Test]
    [ExpectedException(typeof(System.ArithmeticException))]
    public void FirstElementTestforStaticSolveVector()
    {
      FloatVector fv = new FloatVector(3, 1.0f);
      FloatVector X = FloatLevinson.Solve(fv, TR3, Y3);
    }

    #endregion First Element Test for Static SolveVector

    #region Singular Test for Static SolveVector
    
    // test with Toeplitz matrix which has a singular principal sub-matrix
    [Test]
    [ExpectedException(typeof(SingularMatrixException))]
    public void SingularTestforStaticSolveVector()
    {
      FloatVector fv = new FloatVector(3, 1.0f);
      FloatVector X = FloatLevinson.Solve(fv, fv, Y3);
    }
    
    #endregion Singular Test for Static SolveVector

    #region Static Solve Vector 1
    
    [Test]
    public void StaticSolveVector1()
    {
      int i;
      float e, me;
      FloatVector X = FloatLevinson.Solve(LC1, TR1, Y1);
      
      // determine the maximum error
      me = 0.0f;
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
      float e, me;
      FloatVector X = FloatLevinson.Solve(LC2, TR2, Y2);
      
      // determine the maximum error
      me = 0.0f;
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
      float e, me;
      FloatVector X = FloatLevinson.Solve(LC3, TR3, Y3);
      
      // determine the maximum error
      me = 0.0f;
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
      float e, me;
      FloatVector X = FloatLevinson.Solve(LC4, TR4, Y4);
      
      // determine the maximum error
      me = 0.0f;
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
      float e, me;
      FloatVector X = FloatLevinson.Solve(LC5, TR5, Y5);
      
      // determine the maximum error
      me = 0.0f;
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
      float e, me;
      FloatVector X = FloatLevinson.Solve(LC10, TR10, Y10);
      
      // determine the maximum error
      me = 0.0f;
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
      FloatMatrix X = FloatLevinson.Solve(null, TR10, FloatMatrix.CreateIdentity(10));
    }

    #endregion Null Parameter Test 1 for Static SolveMatrix

    #region Null Parameter Test 2 for Static SolveMatrix
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullParameterTestforStaticSolveMatrix2()
    {
      FloatMatrix X = FloatLevinson.Solve(LC10, null, FloatMatrix.CreateIdentity(10));
    }

    #endregion Null Parameter Test 2 for Static SolveMatrix

    #region Null Parameter Test 3 for Static SolveMatrix
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullParameterTestforStaticSolveMatrix3()
    {
      FloatMatrix X = FloatLevinson.Solve(LC10, TR10, null as FloatMatrix);
    }

    #endregion Null Parameter Test 3 for Static SolveMatrix

    #region Zero Vector Length Test for Static SolveMatrix
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void ZeroVectorLengthTestforStaticSolveMatrix()
    {
      FloatVector LC = new FloatVector(1, 0.0f);
      LC.RemoveAt(0);
      FloatMatrix X = FloatLevinson.Solve(LC, TR10, FloatMatrix.CreateIdentity(10));
    }

    #endregion Zero Vector Length Test for Static SolveMatrix

    #region Mismatch Dimension Test 1 for Static SolveMatrix
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchDimensionTestforStaticSolveMatrix1()
    {
      FloatMatrix X = FloatLevinson.Solve(LC10, TR5, FloatMatrix.CreateIdentity(5));
    }

    #endregion Mismatch Dimension Test 1 for Static SolveMatrix

    #region Mismatch Dimension Test 2 for Static SolveMatrix
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchDimensionTestforStaticSolveMatrix2()
    {
      FloatMatrix X = FloatLevinson.Solve(LC10, TR10, FloatMatrix.CreateIdentity(5));
    }

    #endregion Mismatch Dimension Test 2 for Static SolveMatrix

    #region First Element Test for Static SolveMatrix

    [Test]
    [ExpectedException(typeof(System.ArithmeticException))]
    public void FirstElementTestforStaticSolveMatrix()
    {
      FloatVector fv = new FloatVector(3, 1.0f);
      FloatMatrix X = FloatLevinson.Solve(fv, TR3, FloatMatrix.CreateIdentity(3));
    }

    #endregion First Element Test for Static SolveMatrix

    #region Singular Test for Static SolveMatrix
    
    // test with Toeplitz matrix which has a singular principal sub-matrix
    [Test]
    [ExpectedException(typeof(SingularMatrixException))]
    public void SingularTestforStaticSolveMatrix()
    {
      FloatVector fv = new FloatVector(3, 1.0f);
      FloatMatrix X = FloatLevinson.Solve(fv, fv, FloatMatrix.CreateIdentity(3));
    }
    
    #endregion Singular Test for Static SolveMatrix

    #region Static Solve Matrix 1
    
    [Test]
    public void StaticSolveMatrix1()
    {
      int i, j;
      float e, me;
      FloatMatrix I = FloatLevinson.Solve(LC1, TR1, FloatMatrix.CreateIdentity(1));
      
      // determine the maximum error
      me = 0.0f;
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
      float e, me;
      FloatMatrix I = FloatLevinson.Solve(LC2, TR2, FloatMatrix.CreateIdentity(2));
      
      // determine the maximum error
      me = 0.0f;
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
      float e, me;
      FloatMatrix I = FloatLevinson.Solve(LC3, TR3, FloatMatrix.CreateIdentity(3));
      
      // determine the maximum error
      me = 0.0f;
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
      float e, me;
      FloatMatrix I = FloatLevinson.Solve(LC4, TR4, FloatMatrix.CreateIdentity(4));
      
      // determine the maximum error
      me = 0.0f;
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
      float e, me;
      FloatMatrix I = FloatLevinson.Solve(LC5, TR5, FloatMatrix.CreateIdentity(5));
      
      // determine the maximum error
      me = 0.0f;
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
      float e, me;
      FloatMatrix I = FloatLevinson.Solve(LC10, TR10, FloatMatrix.CreateIdentity(10));
      
      // determine the maximum error
      me = 0.0f;
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
      FloatMatrix X = FloatLevinson.Inverse(null, TR10);
    }

    #endregion Null Parameter Test 1 for Static Inverse

    #region Null Parameter Test 2 for Static Inverse
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullParameterTestforStaticInverse2()
    {
      FloatMatrix X = FloatLevinson.Inverse(LC10, null);
    }

    #endregion Null Parameter Test 2 for Static Inverse

    #region Zero Vector Length Test for Static Inverse
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void ZeroVectorLengthTestforStaticInverse()
    {
      FloatVector LC = new FloatVector(1, 0.0f);
      LC.RemoveAt(0);
      FloatMatrix X = FloatLevinson.Inverse(LC, LC);
    }

    #endregion Zero Vector Length Test for Static Inverse

    #region Mismatch Dimension Test for Static Inverse
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchDimensionTestforStaticInverse()
    {
      FloatMatrix X = FloatLevinson.Inverse(LC10, TR5);
    }

    #endregion Mismatch Dimension Test for Static Inverse

    #region First Element Test for Static Inverse

    [Test]
    [ExpectedException(typeof(System.ArithmeticException))]
    public void FirstElementTestforStaticInverse()
    {
      FloatVector fv = new FloatVector(3, 1.0f);
      FloatMatrix X = FloatLevinson.Inverse(fv, TR3);
    }

    #endregion First Element Test for Static  Inverse

    #region Singular Test for Static Inverse
    
    // test with Toeplitz matrix which has a singular principal sub-matrix
    [Test]
    [ExpectedException(typeof(SingularMatrixException))]
    public void SingularTestforStaticInverse()
    {
      FloatVector fv = new FloatVector(3, 1.0f);
      FloatMatrix X = FloatLevinson.Inverse(fv, fv);
    }
    
    #endregion Singular Test for Static Inverse

    #region Static Inverse 1

    [Test]
    public void StaticInverse1()
    {
      int i, j;
      float e, me;

      // calculate the inverse
      FloatMatrix I = FloatLevinson.Inverse(LC1, TR1);

      // determine the maximum relative error
      me = 0.0f;
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
      float e, me;

      // calculate the inverse
      FloatMatrix I = FloatLevinson.Inverse(LC2, TR2);

      // determine the maximum relative error
      me = 0.0f;
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
      float e, me;

      // calculate the inverse
      FloatMatrix I = FloatLevinson.Inverse(LC3, TR3);

      // determine the maximum relative error
      me = 0.0f;
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
      float e, me;

      // calculate the inverse
      FloatMatrix I = FloatLevinson.Inverse(LC4, TR4);

      // determine the maximum relative error
      me = 0.0f;
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
      float e, me;

      // calculate the inverse
      FloatMatrix I = FloatLevinson.Inverse(LC5, TR5);

      // determine the maximum relative error
      me = 0.0f;
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
      float e, me;

      // calculate the inverse
      FloatMatrix I = FloatLevinson.Inverse(LC10, TR10);

      // determine the maximum relative error
      me = 0.0f;
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

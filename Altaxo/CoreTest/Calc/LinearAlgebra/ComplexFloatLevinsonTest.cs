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
using Altaxo.Calc;
using Altaxo.Calc.LinearAlgebra;

#endregion Using Directive

namespace AltaxoTest.Calc.LinearAlgebra
{

  // suite of tests for complex float Levinson algorithm
  [TestFixture]
  public class ComplexFloatLevinsonTest
  {
    #region Fields

    // unit testing - order 1
    
    ComplexFloatVector TR1;   // Toeplitz matrix
    ComplexFloatVector LC1;   // Toeplitz matrix
    ComplexFloatMatrix L1;      // Lower triangle matrix
    ComplexFloatVector D1;      // diagonal vector
    ComplexFloatMatrix U1;      // upper triangle matrix
    ComplexFloatMatrix I1;      // inverse matrix
    ComplexFloat Det1;        // exact determinant
    ComplexFloatVector X1;      // RHS vector
    ComplexFloatVector Y1;      // LHS vector

    // unit testing - order 2
    
    ComplexFloatVector TR2;   // Toeplitz matrix
    ComplexFloatVector LC2;   // Toeplitz matrix
    ComplexFloatMatrix L2;      // Lower triangle matrix
    ComplexFloatVector D2;      // diagonal vector
    ComplexFloatMatrix U2;      // upper triangle matrix
    ComplexFloatMatrix I2;      // inverse matrix
    ComplexFloat Det2;        // exact determinant
    ComplexFloatVector X2;      // RHS vector
    ComplexFloatVector Y2;      // LHS vector

    // unit testing - order 3
    
    ComplexFloatVector TR3;   // Toeplitz matrix
    ComplexFloatVector LC3;   // Toeplitz matrix
    ComplexFloatMatrix L3;      // Lower triangle matrix
    ComplexFloatVector D3;      // diagonal vector
    ComplexFloatMatrix U3;      // upper triangle matrix
    ComplexFloatMatrix I3;      // inverse matrix
    ComplexFloat Det3;        // exact determinant
    ComplexFloatVector X3;      // RHS vector
    ComplexFloatVector Y3;      // LHS vector

    // unit testing - order 4
    
    ComplexFloatVector TR4;   // Toeplitz matrix
    ComplexFloatVector LC4;   // Toeplitz matrix
    ComplexFloatMatrix L4;      // Lower triangle matrix
    ComplexFloatVector D4;      // diagonal vector
    ComplexFloatMatrix U4;      // upper triangle matrix
    ComplexFloatMatrix I4;      // inverse matrix
    ComplexFloat Det4;        // exact determinant
    ComplexFloatVector X4;      // RHS vector
    ComplexFloatVector Y4;      // LHS vector

    // unit testing - order 5
    
    ComplexFloatVector TR5;   // Toeplitz matrix
    ComplexFloatVector LC5;   // Toeplitz matrix
    ComplexFloatMatrix L5;      // Lower triangle matrix
    ComplexFloatVector D5;      // diagonal vector
    ComplexFloatMatrix U5;      // upper triangle matrix
    ComplexFloatMatrix I5;      // inverse matrix
    ComplexFloat Det5;        // exact determinant
    ComplexFloatVector X5;      // RHS vector
    ComplexFloatVector Y5;      // LHS vector

    // unit testing - order 10
    
    ComplexFloatVector TR10;    // Toeplitz matrix
    ComplexFloatVector LC10;    // Toeplitz matrix
    ComplexFloatMatrix L10;   // Lower triangle matrix
    ComplexFloatVector D10;   // diagonal vector
    ComplexFloatMatrix U10;   // upper triangle matrix
    ComplexFloatMatrix I10;   // inverse matrix
    ComplexFloat Det10;     // exact determinant
    ComplexFloatVector X10;   // RHS vector
    ComplexFloatVector Y10;   // LHS vector

    // tolerance for error
    float Tolerance1;
    float Tolerance2;
    float Tolerance3;
    float Tolerance4;
    float Tolerance5;
    float Tolerance10;

    #endregion Fields

    #region Test Fixture Setup

    [TestFixtureSetUp]
    public void SetupTestCases()
    {
      // unit testing values - order 1

      LC1 = new ComplexFloatVector(1);
      LC1[0] = new ComplexFloat(+1.0000000E+000f, +1.0000000E+000f);

      TR1 = new ComplexFloatVector(1);
      TR1[0] = new ComplexFloat(+1.0000000E+000f, +1.0000000E+000f);

      L1 = new ComplexFloatMatrix(1);
      L1[0, 0] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);

      D1 = new ComplexFloatVector(1);
      D1[0] = new ComplexFloat(+5.0000000E-001f, -5.0000000E-001f);

      U1 = new ComplexFloatMatrix(1);
      U1[0, 0] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);

      Det1 = new ComplexFloat(+1.0000000E+000f, +1.0000000E+000f);

      I1 = new ComplexFloatMatrix(1);
      I1[0, 0] = new ComplexFloat(+5.0000000E-001f, -5.0000000E-001f);

      X1 = new ComplexFloatVector(1);
      X1[0] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);

      Y1 = new ComplexFloatVector(1);
      Y1[0] = new ComplexFloat(+1.0000000E+000f, +1.0000000E+000f);

      // unit testing values - order 2

      LC2 = new ComplexFloatVector(2);
      LC2[0] = new ComplexFloat(+3.0000000E+000f, +3.0000000E+000f);
      LC2[1] = new ComplexFloat(+2.0000000E+000f, +0.0000000E+000f);

      TR2 = new ComplexFloatVector(2);
      TR2[0] = new ComplexFloat(+3.0000000E+000f, +3.0000000E+000f);
      TR2[1] = new ComplexFloat(+2.0000000E+000f, +0.0000000E+000f);

      L2 = new ComplexFloatMatrix(2);
      L2[0, 0] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      L2[1, 0] = new ComplexFloat(-3.3333333E-001f, +3.3333333E-001f);
      L2[1, 1] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);

      D2 = new ComplexFloatVector(2);
      D2[0] = new ComplexFloat(+1.6666667E-001f, -1.6666667E-001f);
      D2[1] = new ComplexFloat(+1.2352941E-001f, -1.9411765E-001f);

      U2 = new ComplexFloatMatrix(2);
      U2[0, 0] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      U2[0, 1] = new ComplexFloat(-3.3333333E-001f, +3.3333333E-001f);
      U2[1, 1] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);

      Det2 = new ComplexFloat(-4.0000000E+000f, +1.8000000E+001f);

      I2 = new ComplexFloatMatrix(2);
      I2[0, 0] = new ComplexFloat(+1.2352941E-001f, -1.9411765E-001f);
      I2[0, 1] = new ComplexFloat(+2.3529412E-002f, +1.0588235E-001f);
      I2[1, 0] = new ComplexFloat(+2.3529412E-002f, +1.0588235E-001f);
      I2[1, 1] = new ComplexFloat(+1.2352941E-001f, -1.9411765E-001f);

      X2 = new ComplexFloatVector(2);
      X2[0] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      X2[1] = new ComplexFloat(+2.0000000E+000f, +0.0000000E+000f);

      Y2 = new ComplexFloatVector(2);
      Y2[0] = new ComplexFloat(+7.0000000E+000f, +3.0000000E+000f);
      Y2[1] = new ComplexFloat(+8.0000000E+000f, +6.0000000E+000f);

      // unit testing values - order 3

      LC3 = new ComplexFloatVector(3);
      LC3[0] = new ComplexFloat(+3.0000000E+000f, +3.0000000E+000f);
      LC3[1] = new ComplexFloat(+2.0000000E+000f, +0.0000000E+000f);
      LC3[2] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);

      TR3 = new ComplexFloatVector(3);
      TR3[0] = new ComplexFloat(+3.0000000E+000f, +3.0000000E+000f);
      TR3[1] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      TR3[2] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);

      L3 = new ComplexFloatMatrix(3);
      L3[0, 0] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      L3[1, 0] = new ComplexFloat(-3.3333333E-001f, +3.3333333E-001f);
      L3[1, 1] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      L3[2, 0] = new ComplexFloat(-1.7073171E-001f, -3.6585366E-002f);
      L3[2, 1] = new ComplexFloat(-2.9878049E-001f, +3.1097561E-001f);
      L3[2, 2] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);

      D3 = new ComplexFloatVector(3);
      D3[0] = new ComplexFloat(+1.6666667E-001f, -1.6666667E-001f);
      D3[1] = new ComplexFloat(+1.4634146E-001f, -1.8292683E-001f);
      D3[2] = new ComplexFloat(+1.4776571E-001f, -1.9120527E-001f);

      U3 = new ComplexFloatMatrix(3);
      U3[0, 0] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      U3[0, 1] = new ComplexFloat(-1.6666667E-001f, +1.6666667E-001f);
      U3[1, 1] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      U3[0, 2] = new ComplexFloat(-1.5243902E-001f, +1.2804878E-001f);
      U3[1, 2] = new ComplexFloat(-1.5853659E-001f, +7.3170732E-002f);
      U3[2, 2] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);

      Det3 = new ComplexFloat(-6.4000000E+001f, +3.9000000E+001f);

      I3 = new ComplexFloatMatrix(3);
      I3[0, 0] = new ComplexFloat(+1.4776571E-001f, -1.9120527E-001f);
      I3[0, 1] = new ComplexFloat(-9.4356418E-003f, +4.1125156E-002f);
      I3[0, 2] = new ComplexFloat(+1.9583408E-003f, +4.8068364E-002f);
      I3[1, 0] = new ComplexFloat(+1.5310664E-002f, +1.0307994E-001f);
      I3[1, 1] = new ComplexFloat(+1.3637173E-001f, -1.9814848E-001f);
      I3[1, 2] = new ComplexFloat(-9.4356418E-003f, +4.1125156E-002f);
      I3[2, 0] = new ComplexFloat(-3.2223607E-002f, +2.7238740E-002f);
      I3[2, 1] = new ComplexFloat(+1.5310664E-002f, +1.0307994E-001f);
      I3[2, 2] = new ComplexFloat(+1.4776571E-001f, -1.9120527E-001f);

      X3 = new ComplexFloatVector(3);
      X3[0] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      X3[1] = new ComplexFloat(+2.0000000E+000f, +0.0000000E+000f);
      X3[2] = new ComplexFloat(+3.0000000E+000f, +0.0000000E+000f);

      Y3 = new ComplexFloatVector(3);
      Y3[0] = new ComplexFloat(+8.0000000E+000f, +3.0000000E+000f);
      Y3[1] = new ComplexFloat(+1.1000000E+001f, +6.0000000E+000f);
      Y3[2] = new ComplexFloat(+1.4000000E+001f, +9.0000000E+000f);

      // unit testing values - order 4

      LC4 = new ComplexFloatVector(4);
      LC4[0] = new ComplexFloat(+4.0000000E+000f, +4.0000000E+000f);
      LC4[1] = new ComplexFloat(+3.0000000E+000f, +3.0000000E+000f);
      LC4[2] = new ComplexFloat(+2.0000000E+000f, +2.0000000E+000f);
      LC4[3] = new ComplexFloat(+1.0000000E+000f, +1.0000000E+000f);

      TR4 = new ComplexFloatVector(4);
      TR4[0] = new ComplexFloat(+4.0000000E+000f, +4.0000000E+000f);
      TR4[1] = new ComplexFloat(+1.0000000E+000f, +1.0000000E+000f);
      TR4[2] = new ComplexFloat(+2.0000000E+000f, +2.0000000E+000f);
      TR4[3] = new ComplexFloat(+3.0000000E+000f, +3.0000000E+000f);

      L4 = new ComplexFloatMatrix(4);
      L4[0, 0] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      L4[1, 0] = new ComplexFloat(-7.5000000E-001f, +0.0000000E+000f);
      L4[1, 1] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      L4[2, 0] = new ComplexFloat(+7.6923077E-002f, +0.0000000E+000f);
      L4[2, 1] = new ComplexFloat(-7.6923077E-001f, +0.0000000E+000f);
      L4[2, 2] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      L4[3, 0] = new ComplexFloat(+9.0909091E-002f, +0.0000000E+000f);
      L4[3, 1] = new ComplexFloat(+9.0909091E-002f, +0.0000000E+000f);
      L4[3, 2] = new ComplexFloat(-8.1818182E-001f, +0.0000000E+000f);
      L4[3, 3] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);

      D4 = new ComplexFloatVector(4);
      D4[0] = new ComplexFloat(+1.2500000E-001f, -1.2500000E-001f);
      D4[1] = new ComplexFloat(+1.5384615E-001f, -1.5384615E-001f);
      D4[2] = new ComplexFloat(+1.4772727E-001f, -1.4772727E-001f);
      D4[3] = new ComplexFloat(+1.3750000E-001f, -1.3750000E-001f);

      U4 = new ComplexFloatMatrix(4);
      U4[0, 0] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      U4[0, 1] = new ComplexFloat(-2.5000000E-001f, +0.0000000E+000f);
      U4[1, 1] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      U4[0, 2] = new ComplexFloat(-5.3846154E-001f, +0.0000000E+000f);
      U4[1, 2] = new ComplexFloat(+1.5384615E-001f, +0.0000000E+000f);
      U4[2, 2] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      U4[0, 3] = new ComplexFloat(-8.1818182E-001f, +0.0000000E+000f);
      U4[1, 3] = new ComplexFloat(+9.0909091E-002f, +0.0000000E+000f);
      U4[2, 3] = new ComplexFloat(+9.0909091E-002f, +0.0000000E+000f);
      U4[3, 3] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);

      Det4 = new ComplexFloat(-6.4000000E+002f, +0.0000000E+000f);

      I4 = new ComplexFloatMatrix(4);
      I4[0, 0] = new ComplexFloat(+1.3750000E-001f, -1.3750000E-001f);
      I4[0, 1] = new ComplexFloat(+1.2500000E-002f, -1.2500000E-002f);
      I4[0, 2] = new ComplexFloat(+1.2500000E-002f, -1.2500000E-002f);
      I4[0, 3] = new ComplexFloat(-1.1250000E-001f, +1.1250000E-001f);
      I4[1, 0] = new ComplexFloat(-1.1250000E-001f, +1.1250000E-001f);
      I4[1, 1] = new ComplexFloat(+1.3750000E-001f, -1.3750000E-001f);
      I4[1, 2] = new ComplexFloat(+1.2500000E-002f, -1.2500000E-002f);
      I4[1, 3] = new ComplexFloat(+1.2500000E-002f, -1.2500000E-002f);
      I4[2, 0] = new ComplexFloat(+1.2500000E-002f, -1.2500000E-002f);
      I4[2, 1] = new ComplexFloat(-1.1250000E-001f, +1.1250000E-001f);
      I4[2, 2] = new ComplexFloat(+1.3750000E-001f, -1.3750000E-001f);
      I4[2, 3] = new ComplexFloat(+1.2500000E-002f, -1.2500000E-002f);
      I4[3, 0] = new ComplexFloat(+1.2500000E-002f, -1.2500000E-002f);
      I4[3, 1] = new ComplexFloat(+1.2500000E-002f, -1.2500000E-002f);
      I4[3, 2] = new ComplexFloat(-1.1250000E-001f, +1.1250000E-001f);
      I4[3, 3] = new ComplexFloat(+1.3750000E-001f, -1.3750000E-001f);

      X4 = new ComplexFloatVector(4);
      X4[0] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      X4[1] = new ComplexFloat(+2.0000000E+000f, +0.0000000E+000f);
      X4[2] = new ComplexFloat(+3.0000000E+000f, +0.0000000E+000f);
      X4[3] = new ComplexFloat(+4.0000000E+000f, +0.0000000E+000f);

      Y4 = new ComplexFloatVector(4);
      Y4[0] = new ComplexFloat(+2.4000000E+001f, +2.4000000E+001f);
      Y4[1] = new ComplexFloat(+2.2000000E+001f, +2.2000000E+001f);
      Y4[2] = new ComplexFloat(+2.4000000E+001f, +2.4000000E+001f);
      Y4[3] = new ComplexFloat(+3.0000000E+001f, +3.0000000E+001f);

      // unit testing values - order 5

      LC5 = new ComplexFloatVector(5);
      LC5[0] = new ComplexFloat(+5.0000000E+000f, +1.0000000E+000f);
      LC5[1] = new ComplexFloat(+4.0000000E+000f, +1.0000000E+000f);
      LC5[2] = new ComplexFloat(+3.0000000E+000f, +1.0000000E+000f);
      LC5[3] = new ComplexFloat(+1.0000000E+000f, +1.0000000E+000f);
      LC5[4] = new ComplexFloat(+0.0000000E+000f, +0.0000000E+000f);

      TR5 = new ComplexFloatVector(5);
      TR5[0] = new ComplexFloat(+5.0000000E+000f, +1.0000000E+000f);
      TR5[1] = new ComplexFloat(+1.0000000E+000f, +4.0000000E+000f);
      TR5[2] = new ComplexFloat(+1.0000000E+000f, +3.0000000E+000f);
      TR5[3] = new ComplexFloat(+1.0000000E+000f, +1.0000000E+000f);
      TR5[4] = new ComplexFloat(+0.0000000E+000f, +0.0000000E+000f);

      L5 = new ComplexFloatMatrix(5);
      L5[0, 0] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      L5[1, 0] = new ComplexFloat(-8.0769231E-001f, -3.8461538E-002f);
      L5[1, 1] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      L5[2, 0] = new ComplexFloat(+3.8400000E-002f, +1.1200000E-002f);
      L5[2, 1] = new ComplexFloat(-8.1280000E-001f, -7.0400000E-002f);
      L5[2, 2] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      L5[3, 0] = new ComplexFloat(+2.2603093E-001f, +9.7680412E-002f);
      L5[3, 1] = new ComplexFloat(+8.8659794E-002f, -4.9484536E-002f);
      L5[3, 2] = new ComplexFloat(-8.9149485E-001f, -2.3788660E-001f);
      L5[3, 3] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      L5[4, 0] = new ComplexFloat(-1.1100961E-001f, +5.9189712E-002f);
      L5[4, 1] = new ComplexFloat(+2.3807881E-001f, +1.3619525E-001f);
      L5[4, 2] = new ComplexFloat(+1.0797070E-001f, +5.4774906E-003f);
      L5[4, 3] = new ComplexFloat(-8.0625940E-001f, -2.8594254E-001f);
      L5[4, 4] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);

      D5 = new ComplexFloatVector(5);
      D5[0] = new ComplexFloat(+1.9230769E-001f, -3.8461538E-002f);
      D5[1] = new ComplexFloat(+1.8080000E-001f, +9.4400000E-002f);
      D5[2] = new ComplexFloat(+1.8015464E-001f, +8.8402062E-002f);
      D5[3] = new ComplexFloat(+1.5698660E-001f, +6.5498707E-002f);
      D5[4] = new ComplexFloat(+1.6335863E-001f, +5.3379923E-002f);

      U5 = new ComplexFloatMatrix(5);
      U5[0, 0] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      U5[0, 1] = new ComplexFloat(-3.4615385E-001f, -7.3076923E-001f);
      U5[1, 1] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      U5[0, 2] = new ComplexFloat(-5.6320000E-001f, -4.9760000E-001f);
      U5[1, 2] = new ComplexFloat(+8.9600000E-002f, -3.0720000E-001f);
      U5[2, 2] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      U5[0, 3] = new ComplexFloat(-7.7757732E-001f, +1.8298969E-002f);
      U5[1, 3] = new ComplexFloat(+7.0103093E-002f, -4.5773196E-001f);
      U5[2, 3] = new ComplexFloat(+5.9536082E-002f, -3.1520619E-001f);
      U5[3, 3] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      U5[0, 4] = new ComplexFloat(-3.8732272E-001f, +5.0102819E-001f);
      U5[1, 4] = new ComplexFloat(-3.1309322E-001f, -3.3622620E-001f);
      U5[2, 4] = new ComplexFloat(+6.0556288E-002f, -3.9414442E-001f);
      U5[3, 4] = new ComplexFloat(-7.6951473E-002f, -2.3979216E-001f);
      U5[4, 4] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);

      Det5 = new ComplexFloat(+5.0900000E+002f, -4.2310000E+003f);

      I5 = new ComplexFloatMatrix(5);
      I5[0, 0] = new ComplexFloat(+1.6335863E-001f, +5.3379923E-002f);
      I5[0, 1] = new ComplexFloat(+2.2939970E-004f, -4.3279784E-002f);
      I5[0, 2] = new ComplexFloat(+3.0931791E-002f, -6.1154404E-002f);
      I5[0, 3] = new ComplexFloat(-3.3198751E-002f, -7.1638344E-002f);
      I5[0, 4] = new ComplexFloat(-9.0017358E-002f, +6.1172024E-002f);
      I5[1, 0] = new ComplexFloat(-1.1644584E-001f, -8.9749247E-002f);
      I5[1, 1] = new ComplexFloat(+1.4442611E-001f, +1.0032784E-001f);
      I5[1, 2] = new ComplexFloat(-1.2433728E-002f, -5.1220119E-003f);
      I5[1, 3] = new ComplexFloat(+4.7268453E-002f, -1.4096573E-005f);
      I5[1, 4] = new ComplexFloat(-3.3198751E-002f, -7.1638344E-002f);
      I5[2, 0] = new ComplexFloat(+1.7345558E-002f, +6.6582631E-003f);
      I5[2, 1] = new ComplexFloat(-1.2410964E-001f, -1.0040846E-001f);
      I5[2, 2] = new ComplexFloat(+1.4624793E-001f, +1.1547147E-001f);
      I5[2, 3] = new ComplexFloat(-1.2433728E-002f, -5.1220119E-003f);
      I5[2, 4] = new ComplexFloat(+3.0931791E-002f, -6.1154404E-002f);
      I5[3, 0] = new ComplexFloat(+3.1622138E-002f, +3.4957299E-002f);
      I5[3, 1] = new ComplexFloat(+2.3108689E-002f, -1.2234063E-002f);
      I5[3, 2] = new ComplexFloat(-1.2410964E-001f, -1.0040846E-001f);
      I5[3, 3] = new ComplexFloat(+1.4442611E-001f, +1.0032784E-001f);
      I5[3, 4] = new ComplexFloat(+2.2939970E-004f, -4.3279784E-002f);
      I5[4, 0] = new ComplexFloat(-2.1293920E-002f, +3.7434662E-003f);
      I5[4, 1] = new ComplexFloat(+3.1622138E-002f, +3.4957299E-002f);
      I5[4, 2] = new ComplexFloat(+1.7345558E-002f, +6.6582631E-003f);
      I5[4, 3] = new ComplexFloat(-1.1644584E-001f, -8.9749247E-002f);
      I5[4, 4] = new ComplexFloat(+1.6335863E-001f, +5.3379923E-002f);

      X5 = new ComplexFloatVector(5);
      X5[0] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      X5[1] = new ComplexFloat(+2.0000000E+000f, +0.0000000E+000f);
      X5[2] = new ComplexFloat(+3.0000000E+000f, +0.0000000E+000f);
      X5[3] = new ComplexFloat(+4.0000000E+000f, +0.0000000E+000f);
      X5[4] = new ComplexFloat(+5.0000000E+000f, +0.0000000E+000f);

      Y5 = new ComplexFloatVector(5);
      Y5[0] = new ComplexFloat(+1.4000000E+001f, +2.2000000E+001f);
      Y5[1] = new ComplexFloat(+2.6000000E+001f, +3.2000000E+001f);
      Y5[2] = new ComplexFloat(+3.5000000E+001f, +3.7000000E+001f);
      Y5[3] = new ComplexFloat(+4.4000000E+001f, +3.0000000E+001f);
      Y5[4] = new ComplexFloat(+5.2000000E+001f, +1.4000000E+001f);

      // unit testing values - order 10

      LC10 = new ComplexFloatVector(10);
      LC10[0] = new ComplexFloat(+1.0000000E+001f, +1.0000000E+000f);
      LC10[1] = new ComplexFloat(+9.0000000E+000f, +1.0000000E+000f);
      LC10[2] = new ComplexFloat(+8.0000000E+000f, +1.0000000E+000f);
      LC10[3] = new ComplexFloat(+7.0000000E+000f, +1.0000000E+000f);
      LC10[4] = new ComplexFloat(+6.0000000E+000f, +1.0000000E+000f);
      LC10[5] = new ComplexFloat(+5.0000000E+000f, +1.0000000E+000f);
      LC10[6] = new ComplexFloat(+4.0000000E+000f, +1.0000000E+000f);
      LC10[7] = new ComplexFloat(+3.0000000E+000f, +1.0000000E+000f);
      LC10[8] = new ComplexFloat(+2.0000000E+000f, +1.0000000E+000f);
      LC10[9] = new ComplexFloat(+1.0000000E+000f, +1.0000000E+000f);

      TR10 = new ComplexFloatVector(10);
      TR10[0] = new ComplexFloat(+1.0000000E+001f, +1.0000000E+000f);
      TR10[1] = new ComplexFloat(+1.0000000E+000f, +9.0000000E+000f);
      TR10[2] = new ComplexFloat(+1.0000000E+000f, +8.0000000E+000f);
      TR10[3] = new ComplexFloat(+1.0000000E+000f, +2.0000000E+000f);
      TR10[4] = new ComplexFloat(+1.0000000E+000f, +1.0000000E+000f);
      TR10[5] = new ComplexFloat(+1.0000000E+000f, +1.0000000E+000f);
      TR10[6] = new ComplexFloat(+1.0000000E+000f, +4.0000000E+000f);
      TR10[7] = new ComplexFloat(+1.0000000E+000f, +3.0000000E+000f);
      TR10[8] = new ComplexFloat(+1.0000000E+000f, +2.0000000E+000f);
      TR10[9] = new ComplexFloat(+1.0000000E+000f, +1.0000000E+000f);

      L10 = new ComplexFloatMatrix(10);
      L10[0, 0] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      L10[1, 0] = new ComplexFloat(-9.0099010E-001f, -9.9009901E-003f);
      L10[1, 1] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      L10[2, 0] = new ComplexFloat(+7.2554049E-003f, +4.5437889E-003f);
      L10[2, 1] = new ComplexFloat(-8.9835104E-001f, -1.7149139E-002f);
      L10[2, 2] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      L10[3, 0] = new ComplexFloat(+8.1190931E-003f, +4.8283630E-003f);
      L10[3, 1] = new ComplexFloat(+8.5500220E-003f, +3.8783605E-003f);
      L10[3, 2] = new ComplexFloat(-8.9685128E-001f, -2.5375839E-002f);
      L10[3, 3] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      L10[4, 0] = new ComplexFloat(+9.0797285E-003f, +5.0413564E-003f);
      L10[4, 1] = new ComplexFloat(+9.5223197E-003f, +3.9833209E-003f);
      L10[4, 2] = new ComplexFloat(+1.3594954E-002f, +1.3510004E-003f);
      L10[4, 3] = new ComplexFloat(-9.0106887E-001f, -3.2599942E-002f);
      L10[4, 4] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      L10[5, 0] = new ComplexFloat(+1.0110174E-002f, +5.1587582E-003f);
      L10[5, 1] = new ComplexFloat(+1.0553084E-002f, +3.9861933E-003f);
      L10[5, 2] = new ComplexFloat(+1.4900585E-002f, +9.5517949E-004f);
      L10[5, 3] = new ComplexFloat(+1.4574245E-002f, -1.1129242E-003f);
      L10[5, 4] = new ComplexFloat(-9.0776628E-001f, -3.8281015E-002f);
      L10[5, 5] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      L10[6, 0] = new ComplexFloat(+1.1163789E-002f, +5.1884795E-003f);
      L10[6, 1] = new ComplexFloat(+1.1597112E-002f, +3.8999115E-003f);
      L10[6, 2] = new ComplexFloat(+1.6188452E-002f, +4.4138654E-004f);
      L10[6, 3] = new ComplexFloat(+1.5752309E-002f, -1.7871732E-003f);
      L10[6, 4] = new ComplexFloat(+1.4568519E-002f, -5.3100094E-003f);
      L10[6, 5] = new ComplexFloat(-9.1612446E-001f, -3.9858108E-002f);
      L10[6, 6] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      L10[7, 0] = new ComplexFloat(+1.2250407E-002f, +5.1493111E-003f);
      L10[7, 1] = new ComplexFloat(+1.2666180E-002f, +3.7419578E-003f);
      L10[7, 2] = new ComplexFloat(+1.7480233E-002f, -1.7281679E-004f);
      L10[7, 3] = new ComplexFloat(+1.6920428E-002f, -2.5592884E-003f);
      L10[7, 4] = new ComplexFloat(+1.5502251E-002f, -6.3119298E-003f);
      L10[7, 5] = new ComplexFloat(+1.0175586E-002f, -6.2706520E-003f);
      L10[7, 6] = new ComplexFloat(-9.2129653E-001f, -4.0086723E-002f);
      L10[7, 7] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      L10[8, 0] = new ComplexFloat(+1.3361099E-002f, +5.0636594E-003f);
      L10[8, 1] = new ComplexFloat(+1.3753926E-002f, +3.5354454E-003f);
      L10[8, 2] = new ComplexFloat(+1.8776835E-002f, -8.5571402E-004f);
      L10[8, 3] = new ComplexFloat(+1.8083822E-002f, -3.3986502E-003f);
      L10[8, 4] = new ComplexFloat(+1.6416076E-002f, -7.3767089E-003f);
      L10[8, 5] = new ComplexFloat(+1.0693868E-002f, -7.1281839E-003f);
      L10[8, 6] = new ComplexFloat(+8.4803223E-003f, -7.7622936E-003f);
      L10[8, 7] = new ComplexFloat(-9.2544568E-001f, -3.8329752E-002f);
      L10[8, 8] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      L10[9, 0] = new ComplexFloat(+1.4514517E-002f, +4.9086455E-003f);
      L10[9, 1] = new ComplexFloat(+1.4876264E-002f, +3.2557272E-003f);
      L10[9, 2] = new ComplexFloat(+2.0088847E-002f, -1.6446645E-003f);
      L10[9, 3] = new ComplexFloat(+1.9247642E-002f, -4.3429207E-003f);
      L10[9, 4] = new ComplexFloat(+1.7306259E-002f, -8.5413384E-003f);
      L10[9, 5] = new ComplexFloat(+1.1183742E-002f, -8.0532599E-003f);
      L10[9, 6] = new ComplexFloat(+8.7870452E-003f, -8.6470170E-003f);
      L10[9, 7] = new ComplexFloat(+6.7700213E-003f, -5.2124680E-003f);
      L10[9, 8] = new ComplexFloat(-9.2836000E-001f, -3.8752385E-002f);
      L10[9, 9] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);

      D10 = new ComplexFloatVector(10);
      D10[0] = new ComplexFloat(+9.9009901E-002f, -9.9009901E-003f);
      D10[1] = new ComplexFloat(+6.8010260E-002f, +5.2693294E-002f);
      D10[2] = new ComplexFloat(+6.8503012E-002f, +5.2264825E-002f);
      D10[3] = new ComplexFloat(+6.8598237E-002f, +5.1618595E-002f);
      D10[4] = new ComplexFloat(+6.8466983E-002f, +5.0945485E-002f);
      D10[5] = new ComplexFloat(+6.8034270E-002f, +5.0441605E-002f);
      D10[6] = new ComplexFloat(+6.7730052E-002f, +5.0175369E-002f);
      D10[7] = new ComplexFloat(+6.7391349E-002f, +5.0080104E-002f);
      D10[8] = new ComplexFloat(+6.7234262E-002f, +4.9912171E-002f);
      D10[9] = new ComplexFloat(+6.7024327E-002f, +4.9751098E-002f);

      U10 = new ComplexFloatMatrix(10);
      U10[0, 0] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      U10[0, 1] = new ComplexFloat(-1.8811881E-001f, -8.8118812E-001f);
      U10[1, 1] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      U10[0, 2] = new ComplexFloat(-3.0868450E-001f, -8.2968120E-001f);
      U10[1, 2] = new ComplexFloat(+8.1788201E-002f, -1.3059729E-001f);
      U10[2, 2] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      U10[0, 3] = new ComplexFloat(-6.9271338E-001f, -4.1101317E-001f);
      U10[1, 3] = new ComplexFloat(+3.0656677E-001f, -4.4856765E-001f);
      U10[2, 3] = new ComplexFloat(+7.8629842E-002f, -1.3672690E-001f);
      U10[3, 3] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      U10[0, 4] = new ComplexFloat(-7.5308973E-001f, -1.7764936E-001f);
      U10[1, 4] = new ComplexFloat(-2.1811893E-002f, -2.3257783E-001f);
      U10[2, 4] = new ComplexFloat(+3.0081682E-001f, -4.5300731E-001f);
      U10[3, 4] = new ComplexFloat(+7.3373192E-002f, -1.4180544E-001f);
      U10[4, 4] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      U10[0, 5] = new ComplexFloat(-6.6968846E-001f, +1.6997563E-001f);
      U10[1, 5] = new ComplexFloat(-1.4411312E-001f, -3.0897730E-001f);
      U10[2, 5] = new ComplexFloat(-3.1145914E-002f, -2.3117177E-001f);
      U10[3, 5] = new ComplexFloat(+2.9376277E-001f, -4.5405633E-001f);
      U10[4, 5] = new ComplexFloat(+6.6435695E-002f, -1.4363825E-001f);
      U10[5, 5] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      U10[0, 6] = new ComplexFloat(-3.6546783E-001f, +1.3495818E-001f);
      U10[1, 6] = new ComplexFloat(-3.3276275E-001f, +6.1455619E-002f);
      U10[2, 6] = new ComplexFloat(-1.4928934E-001f, -3.0660365E-001f);
      U10[3, 6] = new ComplexFloat(-3.6720508E-002f, -2.2950990E-001f);
      U10[4, 6] = new ComplexFloat(+2.8936799E-001f, -4.5408893E-001f);
      U10[5, 6] = new ComplexFloat(+6.2044535E-002f, -1.4415916E-001f);
      U10[6, 6] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      U10[0, 7] = new ComplexFloat(-2.2796156E-001f, +2.1789305E-001f);
      U10[1, 7] = new ComplexFloat(-1.4794186E-001f, -5.5572852E-002f);
      U10[2, 7] = new ComplexFloat(-3.3492680E-001f, +6.5840476E-002f);
      U10[3, 7] = new ComplexFloat(-1.5249085E-001f, -3.0276392E-001f);
      U10[4, 7] = new ComplexFloat(-4.0507028E-002f, -2.2608317E-001f);
      U10[5, 7] = new ComplexFloat(+2.8587453E-001f, -4.5245103E-001f);
      U10[6, 7] = new ComplexFloat(+5.8369087E-002f, -1.4290942E-001f);
      U10[7, 7] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      U10[0, 8] = new ComplexFloat(-1.8901586E-001f, +3.4805079E-002f);
      U10[1, 8] = new ComplexFloat(-5.2426683E-002f, +1.9340427E-001f);
      U10[2, 8] = new ComplexFloat(-1.4964696E-001f, -5.4033437E-002f);
      U10[3, 8] = new ComplexFloat(-3.3763728E-001f, +6.7573088E-002f);
      U10[4, 8] = new ComplexFloat(-1.5560000E-001f, -3.0169126E-001f);
      U10[5, 8] = new ComplexFloat(-4.3805054E-002f, -2.2544210E-001f);
      U10[6, 8] = new ComplexFloat(+2.8335018E-001f, -4.5271747E-001f);
      U10[7, 8] = new ComplexFloat(+5.5874343E-002f, -1.4345634E-001f);
      U10[8, 8] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      U10[0, 9] = new ComplexFloat(-1.9701952E-001f, +6.3155749E-002f);
      U10[1, 9] = new ComplexFloat(-4.2642564E-003f, -1.6090427E-002f);
      U10[2, 9] = new ComplexFloat(-5.3607239E-002f, +1.9546918E-001f);
      U10[3, 9] = new ComplexFloat(-1.5130367E-001f, -5.1953666E-002f);
      U10[4, 9] = new ComplexFloat(-3.4040569E-001f, +7.0063213E-002f);
      U10[5, 9] = new ComplexFloat(-1.5894822E-001f, -2.9987956E-001f);
      U10[6, 9] = new ComplexFloat(-4.7450414E-002f, -2.2408765E-001f);
      U10[7, 9] = new ComplexFloat(+2.8041710E-001f, -4.5254539E-001f);
      U10[8, 9] = new ComplexFloat(+5.2922147E-002f, -1.4361015E-001f);
      U10[9, 9] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);

      Det10 = new ComplexFloat(+3.6568548E+010f, +2.4768517E+010f);

      I10 = new ComplexFloatMatrix(10);
      I10[0, 0] = new ComplexFloat(+6.7024327E-002f, +4.9751098E-002f);
      I10[0, 1] = new ComplexFloat(+1.0691834E-002f, -6.9924390E-003f);
      I10[0, 2] = new ComplexFloat(+4.1309398E-002f, -1.6380491E-002f);
      I10[0, 3] = new ComplexFloat(+7.9682744E-003f, -1.7380034E-002f);
      I10[0, 4] = new ComplexFloat(+4.2659401E-003f, -2.8007075E-002f);
      I10[0, 5] = new ComplexFloat(-2.6301184E-002f, -1.2239617E-002f);
      I10[0, 6] = new ComplexFloat(-7.5562748E-003f, -1.1009683E-002f);
      I10[0, 7] = new ComplexFloat(-1.3317795E-002f, +1.0434171E-002f);
      I10[0, 8] = new ComplexFloat(+5.1470750E-004f, -1.2906015E-003f);
      I10[0, 9] = new ComplexFloat(-1.6347168E-002f, -5.5689657E-003f);
      I10[1, 0] = new ComplexFloat(-6.0294731E-002f, -4.8784282E-002f);
      I10[1, 1] = new ComplexFloat(+5.7037418E-002f, +5.5989338E-002f);
      I10[1, 2] = new ComplexFloat(-2.8067888E-002f, +6.7497836E-003f);
      I10[1, 3] = new ComplexFloat(+3.3576008E-002f, -4.6936404E-004f);
      I10[1, 4] = new ComplexFloat(+3.2614353E-003f, +8.4914935E-003f);
      I10[1, 5] = new ComplexFloat(+2.8539068E-002f, -1.5668319E-002f);
      I10[1, 6] = new ComplexFloat(-1.9485222E-002f, -1.7952100E-003f);
      I10[1, 7] = new ComplexFloat(+5.4035811E-003f, -2.0272674E-002f);
      I10[1, 8] = new ComplexFloat(-1.3705944E-002f, +1.1564861E-002f);
      I10[1, 9] = new ComplexFloat(+5.1470750E-004f, -1.2906015E-003f);
      I10[2, 0] = new ComplexFloat(+7.1308213E-004f, -1.2546164E-005f);
      I10[2, 1] = new ComplexFloat(-6.0272601E-002f, -4.8871146E-002f);
      I10[2, 2] = new ComplexFloat(+5.7219842E-002f, +5.5680641E-002f);
      I10[2, 3] = new ComplexFloat(-2.8112753E-002f, +6.6173592E-003f);
      I10[2, 4] = new ComplexFloat(+3.3454600E-002f, -6.5413224E-004f);
      I10[2, 5] = new ComplexFloat(+3.0216929E-003f, +8.5724569E-003f);
      I10[2, 6] = new ComplexFloat(+2.8435161E-002f, -1.5684889E-002f);
      I10[2, 7] = new ComplexFloat(-1.9514358E-002f, -1.6393606E-003f);
      I10[2, 8] = new ComplexFloat(+5.4035811E-003f, -2.0272674E-002f);
      I10[2, 9] = new ComplexFloat(-1.3317795E-002f, +1.0434171E-002f);
      I10[3, 0] = new ComplexFloat(+1.0191444E-003f, -1.4239535E-004f);
      I10[3, 1] = new ComplexFloat(+9.9108704E-004f, -2.5251613E-004f);
      I10[3, 2] = new ComplexFloat(-5.9819166E-002f, -4.9484148E-002f);
      I10[3, 3] = new ComplexFloat(+5.7389952E-002f, +5.5227507E-002f);
      I10[3, 4] = new ComplexFloat(-2.8106424E-002f, +6.0757008E-003f);
      I10[3, 5] = new ComplexFloat(+3.3259014E-002f, -8.2858379E-004f);
      I10[3, 6] = new ComplexFloat(+2.9250084E-003f, +8.3171088E-003f);
      I10[3, 7] = new ComplexFloat(+2.8435161E-002f, -1.5684889E-002f);
      I10[3, 8] = new ComplexFloat(-1.9485222E-002f, -1.7952100E-003f);
      I10[3, 9] = new ComplexFloat(-7.5562748E-003f, -1.1009683E-002f);
      I10[4, 0] = new ComplexFloat(+1.1502413E-003f, +1.6639121E-005f);
      I10[4, 1] = new ComplexFloat(+1.1380402E-003f, -1.0980979E-004f);
      I10[4, 2] = new ComplexFloat(+1.3977289E-003f, -5.8000250E-004f);
      I10[4, 3] = new ComplexFloat(-5.9700112E-002f, -4.9533948E-002f);
      I10[4, 4] = new ComplexFloat(+5.7405368E-002f, +5.5059022E-002f);
      I10[4, 5] = new ComplexFloat(-2.8274330E-002f, +6.2766221E-003f);
      I10[4, 6] = new ComplexFloat(+3.3259014E-002f, -8.2858379E-004f);
      I10[4, 7] = new ComplexFloat(+3.0216929E-003f, +8.5724569E-003f);
      I10[4, 8] = new ComplexFloat(+2.8539068E-002f, -1.5668319E-002f);
      I10[4, 9] = new ComplexFloat(-2.6301184E-002f, -1.2239617E-002f);
      I10[5, 0] = new ComplexFloat(+1.5848813E-003f, +2.8852793E-004f);
      I10[5, 1] = new ComplexFloat(+1.5972212E-003f, +1.1105891E-004f);
      I10[5, 2] = new ComplexFloat(+2.0644546E-003f, -4.7842310E-004f);
      I10[5, 3] = new ComplexFloat(+1.9356717E-003f, -7.4622243E-004f);
      I10[5, 4] = new ComplexFloat(-5.9306111E-002f, -4.9933722E-002f);
      I10[5, 5] = new ComplexFloat(+5.7405368E-002f, +5.5059022E-002f);
      I10[5, 6] = new ComplexFloat(-2.8106424E-002f, +6.0757008E-003f);
      I10[5, 7] = new ComplexFloat(+3.3454600E-002f, -6.5413224E-004f);
      I10[5, 8] = new ComplexFloat(+3.2614353E-003f, +8.4914935E-003f);
      I10[5, 9] = new ComplexFloat(+4.2659401E-003f, -2.8007075E-002f);
      I10[6, 0] = new ComplexFloat(+1.5061253E-003f, +6.6650996E-004f);
      I10[6, 1] = new ComplexFloat(+1.5609115E-003f, +4.9307536E-004f);
      I10[6, 2] = new ComplexFloat(+2.1665459E-003f, +1.9121555E-005f);
      I10[6, 3] = new ComplexFloat(+2.1027094E-003f, -2.7790747E-004f);
      I10[6, 4] = new ComplexFloat(+1.9356717E-003f, -7.4622243E-004f);
      I10[6, 5] = new ComplexFloat(-5.9700112E-002f, -4.9533948E-002f);
      I10[6, 6] = new ComplexFloat(+5.7389952E-002f, +5.5227507E-002f);
      I10[6, 7] = new ComplexFloat(-2.8112753E-002f, +6.6173592E-003f);
      I10[6, 8] = new ComplexFloat(+3.3576008E-002f, -4.6936404E-004f);
      I10[6, 9] = new ComplexFloat(+7.9682744E-003f, -1.7380034E-002f);
      I10[7, 0] = new ComplexFloat(+1.4282653E-003f, +8.8920966E-004f);
      I10[7, 1] = new ComplexFloat(+1.5084436E-003f, +7.2160481E-004f);
      I10[7, 2] = new ComplexFloat(+2.1887064E-003f, +3.2867753E-004f);
      I10[7, 3] = new ComplexFloat(+2.1665459E-003f, +1.9121555E-005f);
      I10[7, 4] = new ComplexFloat(+2.0644546E-003f, -4.7842310E-004f);
      I10[7, 5] = new ComplexFloat(+1.3977289E-003f, -5.8000250E-004f);
      I10[7, 6] = new ComplexFloat(-5.9819166E-002f, -4.9484148E-002f);
      I10[7, 7] = new ComplexFloat(+5.7219842E-002f, +5.5680641E-002f);
      I10[7, 8] = new ComplexFloat(-2.8067888E-002f, +6.7497836E-003f);
      I10[7, 9] = new ComplexFloat(+4.1309398E-002f, -1.6380491E-002f);
      I10[8, 0] = new ComplexFloat(+8.3509562E-004f, +9.5832342E-004f);
      I10[8, 1] = new ComplexFloat(+9.3009336E-004f, +8.5497971E-004f);
      I10[8, 2] = new ComplexFloat(+1.5084436E-003f, +7.2160481E-004f);
      I10[8, 3] = new ComplexFloat(+1.5609115E-003f, +4.9307536E-004f);
      I10[8, 4] = new ComplexFloat(+1.5972212E-003f, +1.1105891E-004f);
      I10[8, 5] = new ComplexFloat(+1.1380402E-003f, -1.0980979E-004f);
      I10[8, 6] = new ComplexFloat(+9.9108704E-004f, -2.5251613E-004f);
      I10[8, 7] = new ComplexFloat(-6.0272601E-002f, -4.8871146E-002f);
      I10[8, 8] = new ComplexFloat(+5.7037418E-002f, +5.5989338E-002f);
      I10[8, 9] = new ComplexFloat(+1.0691834E-002f, -6.9924390E-003f);
      I10[9, 0] = new ComplexFloat(+7.2861524E-004f, +1.0511118E-003f);
      I10[9, 1] = new ComplexFloat(+8.3509562E-004f, +9.5832342E-004f);
      I10[9, 2] = new ComplexFloat(+1.4282653E-003f, +8.8920966E-004f);
      I10[9, 3] = new ComplexFloat(+1.5061253E-003f, +6.6650996E-004f);
      I10[9, 4] = new ComplexFloat(+1.5848813E-003f, +2.8852793E-004f);
      I10[9, 5] = new ComplexFloat(+1.1502413E-003f, +1.6639121E-005f);
      I10[9, 6] = new ComplexFloat(+1.0191444E-003f, -1.4239535E-004f);
      I10[9, 7] = new ComplexFloat(+7.1308213E-004f, -1.2546164E-005f);
      I10[9, 8] = new ComplexFloat(-6.0294731E-002f, -4.8784282E-002f);
      I10[9, 9] = new ComplexFloat(+6.7024327E-002f, +4.9751098E-002f);

      X10 = new ComplexFloatVector(10);
      X10[0] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f);
      X10[1] = new ComplexFloat(+2.0000000E+000f, +0.0000000E+000f);
      X10[2] = new ComplexFloat(+3.0000000E+000f, +0.0000000E+000f);
      X10[3] = new ComplexFloat(+4.0000000E+000f, +0.0000000E+000f);
      X10[4] = new ComplexFloat(+5.0000000E+000f, +0.0000000E+000f);
      X10[5] = new ComplexFloat(+6.0000000E+000f, +0.0000000E+000f);
      X10[6] = new ComplexFloat(+7.0000000E+000f, +0.0000000E+000f);
      X10[7] = new ComplexFloat(+8.0000000E+000f, +0.0000000E+000f);
      X10[8] = new ComplexFloat(+9.0000000E+000f, +0.0000000E+000f);
      X10[9] = new ComplexFloat(+1.0000000E+001f, +0.0000000E+000f);

      Y10 = new ComplexFloatVector(10);
      Y10[0] = new ComplexFloat(+6.4000000E+001f, +1.4200000E+002f);
      Y10[1] = new ComplexFloat(+8.1000000E+001f, +1.6400000E+002f);
      Y10[2] = new ComplexFloat(+1.0500000E+002f, +1.7500000E+002f);
      Y10[3] = new ComplexFloat(+1.3500000E+002f, +1.7400000E+002f);
      Y10[4] = new ComplexFloat(+1.7000000E+002f, +1.6000000E+002f);
      Y10[5] = new ComplexFloat(+2.0900000E+002f, +1.7600000E+002f);
      Y10[6] = new ComplexFloat(+2.5100000E+002f, +1.9200000E+002f);
      Y10[7] = new ComplexFloat(+2.9500000E+002f, +1.9700000E+002f);
      Y10[8] = new ComplexFloat(+3.4000000E+002f, +1.3500000E+002f);
      Y10[9] = new ComplexFloat(+3.8500000E+002f, +5.5000000E+001f);

      // Tolerances
      Tolerance1 = 1.000E-06f;
      Tolerance2 = 2.000E-06f;
      Tolerance3 = 1.000E-06f;
      Tolerance4 = 1.000E-05f;
      Tolerance5 = 1.000E-05f;
      Tolerance10 = 5.000E-05f;
    }

    #endregion Test Fixture Setup

    #region Null Parameter Tests for Constructor 
    
    // Test constructor with a null parameter
    [Test]
    [ExpectedException(typeof(System.ArgumentNullException))]
    public void NullParameterTestforConstructor1()
    {
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(null as ComplexFloatVector, TR5);
    }

    // Test constructor with a null parameter
    [Test]
    [ExpectedException(typeof(System.ArgumentNullException))]
    public void NullParameterTestforConstructor2()
    {
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC5, null as ComplexFloatVector);
    }
    
    #endregion  Null Parameter Tests for Constructor 

    #region Zero Length Vector Tests for Constructor
    
    // Test constructor with a zero length vector parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void ZeroLengthVectorTestsforConstructor1()
    {
      ComplexFloatVector cfv = new ComplexFloatVector(1);
      cfv.RemoveAt(0);
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(cfv, cfv);
    }

    #endregion Zero Length Vector Tests for Constructor

    #region Mismatching Vector Length Tests for Constructor

    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchVectorLengthTestsforConstructor1()
    {
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC2, TR3);
    }

    #endregion Mismatching Vector Length Tests for Constructor

    #region First Element Test for Constructor

    [Test]
    [ExpectedException(typeof(System.ArithmeticException))]
    public void FirstElementTestforConstructor1()
    {
      ComplexFloatVector cfv = new ComplexFloatVector(3, ComplexFloat.One);
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC3, cfv);
    }

    #endregion First Element Test for Constructor

    #region Get Vector Member Tests

    // check get vector
    [Test]
    public void GetLeftColumnTest()
    {
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC5, TR5);
      ComplexFloatVector LC = cfl.GetLeftColumn();
      Assert.IsTrue(LC5.Equals(LC));
    }
    
    // check get vector
    [Test]
    public void GetTopRowTest()
    {
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC5, TR5);
      ComplexFloatVector TR = cfl.GetTopRow();
      Assert.IsTrue(TR5.Equals(TR));
    }
    
    #endregion Get Vector Member Tests

    #region GetMatrix Member Test
    
    // check get matrix
    [Test]
    public void GetMatrixMemberTest()
    {
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC5, TR5);
      ComplexFloatMatrix cflfm = cfl.GetMatrix();
      for (int row = 0; row < TR5.Length; row++)
      {
        for (int column = 0; column < TR5.Length; column++)
        {
          if (column < row)
          {
            Assert.IsTrue(cflfm[row, column] == LC5[row - column]);
          }
          else
          {
            Assert.IsTrue(cflfm[row, column] == TR5[column - row]);
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
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC5, TR5);
      Assert.IsTrue(cfl.Order == 5);
    }

    #endregion Order Property Test

    #region Decomposition Test 1

    // test the UDL factorisation for case 1

    [Test]
    public void DecompositionTest1()
    {
      int i, j;
      float e, me;
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC1, TR1);
      ComplexFloatMatrix U = cfl.U;
      ComplexFloatMatrix D = cfl.D;
      ComplexFloatMatrix L = cfl.L;
      
      // check the upper triangle
      me = 0.0f;
      for (i = 0; i < cfl.Order; i++)
      {
        for (j = 0; j < cfl.Order; j++)
        {
          if (U1[i, j] != U[i, j])
          {
            e = ComplexMath.Absolute((U1[i, j] - U[i, j]) / U1[i, j]);
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
      for (i = 0; i < cfl.Order; i++)
      {
        for (j = 0; j < cfl.Order; j++)
        {
          if (L1[i, j] != L[i, j])
          {
            e = ComplexMath.Absolute((L1[i, j] - L[i, j]) / L1[i, j]);
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
      for (i = 0; i < cfl.Order; i++)
      {
        e = ComplexMath.Absolute((D1[i] - D[i, i]) / D1[i]);
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
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC2, TR2);
      ComplexFloatMatrix U = cfl.U;
      ComplexFloatMatrix D = cfl.D;
      ComplexFloatMatrix L = cfl.L;
      
      // check the upper triangle
      me = 0.0f;
      for (i = 0; i < cfl.Order; i++)
      {
        for (j = 0; j < cfl.Order; j++)
        {
          if (U2[i, j] != U[i, j])
          {
            e = ComplexMath.Absolute((U2[i, j] - U[i, j]) / U2[i, j]);
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
      for (i = 0; i < cfl.Order; i++)
      {
        for (j = 0; j < cfl.Order; j++)
        {
          if (L2[i, j] != L[i, j])
          {
            e = ComplexMath.Absolute((L2[i, j] - L[i, j]) / L2[i, j]);
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
      for (i = 0; i < cfl.Order; i++)
      {
        e = ComplexMath.Absolute((D2[i] - D[i, i]) / D2[i]);
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
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC3, TR3);
      ComplexFloatMatrix U = cfl.U;
      ComplexFloatMatrix D = cfl.D;
      ComplexFloatMatrix L = cfl.L;
      
      // check the upper triangle
      me = 0.0f;
      for (i = 0; i < cfl.Order; i++)
      {
        for (j = 0; j < cfl.Order; j++)
        {
          if (U3[i, j] != U[i, j])
          {
            e = ComplexMath.Absolute((U3[i, j] - U[i, j]) / U3[i, j]);
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
      for (i = 0; i < cfl.Order; i++)
      {
        for (j = 0; j < cfl.Order; j++)
        {
          if (L3[i, j] != L[i, j])
          {
            e = ComplexMath.Absolute((L3[i, j] - L[i, j]) / L3[i, j]);
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
      for (i = 0; i < cfl.Order; i++)
      {
        e = ComplexMath.Absolute((D3[i] - D[i, i]) / D3[i]);
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
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC4, TR4);
      ComplexFloatMatrix U = cfl.U;
      ComplexFloatMatrix D = cfl.D;
      ComplexFloatMatrix L = cfl.L;
      
      // check the upper triangle
      me = 0.0f;
      for (i = 0; i < cfl.Order; i++)
      {
        for (j = 0; j < cfl.Order; j++)
        {
          if (U4[i, j] != U[i, j])
          {
            e = ComplexMath.Absolute((U4[i, j] - U[i, j]) / U4[i, j]);
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
      for (i = 0; i < cfl.Order; i++)
      {
        for (j = 0; j < cfl.Order; j++)
        {
          if (L4[i, j] != L[i, j])
          {
            e = ComplexMath.Absolute((L4[i, j] - L[i, j]) / L4[i, j]);
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
      for (i = 0; i < cfl.Order; i++)
      {
        e = ComplexMath.Absolute((D4[i] - D[i, i]) / D4[i]);
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
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC5, TR5);
      ComplexFloatMatrix U = cfl.U;
      ComplexFloatMatrix D = cfl.D;
      ComplexFloatMatrix L = cfl.L;
      
      // check the upper triangle
      me = 0.0f;
      for (i = 0; i < cfl.Order; i++)
      {
        for (j = 0; j < cfl.Order; j++)
        {
          if (U5[i, j] != U[i, j])
          {
            e = ComplexMath.Absolute((U5[i, j] - U[i, j]) / U5[i, j]);
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
      for (i = 0; i < cfl.Order; i++)
      {
        for (j = 0; j < cfl.Order; j++)
        {
          if (L5[i, j] != L[i, j])
          {
            e = ComplexMath.Absolute((L5[i, j] - L[i, j]) / L5[i, j]);
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
      for (i = 0; i < cfl.Order; i++)
      {
        e = ComplexMath.Absolute((D5[i] - D[i, i]) / D5[i]);
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
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC10, TR10);
      ComplexFloatMatrix U = cfl.U;
      ComplexFloatMatrix D = cfl.D;
      ComplexFloatMatrix L = cfl.L;
      
      // check the upper triangle
      me = 0.0f;
      for (i = 0; i < cfl.Order; i++)
      {
        for (j = 0; j < cfl.Order; j++)
        {
          if (U10[i, j] != U[i, j])
          {
            e = ComplexMath.Absolute((U10[i, j] - U[i, j]) / U10[i, j]);
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
      for (i = 0; i < cfl.Order; i++)
      {
        for (j = 0; j < cfl.Order; j++)
        {
          if (L10[i, j] != L[i, j])
          {
            e = ComplexMath.Absolute((L10[i, j] - L[i, j]) / L10[i, j]);
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
      for (i = 0; i < cfl.Order; i++)
      {
        e = ComplexMath.Absolute((D10[i] - D[i, i]) / D10[i]);
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
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC4,TR4);
      Assert.IsFalse(cfl.IsSingular);
    }

    #endregion Singularity Property Test 1

    #region Singularity Property Test 2

    // check that singular matrix is detected
    [Test]
    public void SingularityPropertyTest2()
    {
      ComplexFloatVector LC = new ComplexFloatVector(4);
      LC[0] = new ComplexFloat(4.0f);
      LC[1] = new ComplexFloat(2.0f);
      LC[2] = new ComplexFloat(1.0f);
      LC[3] = new ComplexFloat(0.0f);

      ComplexFloatVector TR = new ComplexFloatVector(4);
      TR[0] = new ComplexFloat(4.0f);
      TR[1] = new ComplexFloat(8.0f);
      TR[2] = new ComplexFloat(2.0f);
      TR[3] = new ComplexFloat(1.0f);

      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC,TR);
      Assert.IsTrue(cfl.IsSingular);
    }

    #endregion Singularity Property Test 2

    #region GetDeterminant Method Test 1
    
    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest1()
    {
      // calculate determinant from diagonal
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC1, TR1);

      // check results match
      float e = ComplexMath.Absolute((cfl.GetDeterminant() - Det1)/Det1);
      Assert.IsTrue(e < Tolerance1);
    }
    
    #endregion GetDeterminant Method Test 1

    #region GetDeterminant Method Test 2
    
    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest2()
    {
      // calculate determinant from diagonal
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC2, TR2);

      // check results match
      Double e = ComplexMath.Absolute((cfl.GetDeterminant() - Det2)/Det2);
      Assert.IsTrue(e < Tolerance2);
    }
    
    #endregion GetDeterminant Method Test 2

    #region GetDeterminant Method Test 3
    
    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest3()
    {
      // calculate determinant from diagonal
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC3, TR3);

      // check results match
      Double e = ComplexMath.Absolute((cfl.GetDeterminant() - Det3)/Det3);
      Assert.IsTrue(e < Tolerance3);
    }
    
    #endregion GetDeterminant Method Test 3

    #region GetDeterminant Method Test 4
    
    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest4()
    {
      // calculate determinant from diagonal
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC4, TR4);

      // check results match
      Double e = ComplexMath.Absolute((cfl.GetDeterminant() - Det4)/Det4);
      Assert.IsTrue(e < Tolerance4);
    }
    
    #endregion GetDeterminant Method Test 4

    #region GetDeterminant Method Test 5
    
    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest5()
    {
      // calculate determinant from diagonal
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC5, TR5);

      // check results match
      Double e = ComplexMath.Absolute((cfl.GetDeterminant() - Det5)/Det5);
      Assert.IsTrue(e < Tolerance5);
    }
    
    #endregion GetDeterminant Method Test 5

    #region GetDeterminant Method Test 10
    
    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest10()
    {
      // calculate determinant from diagonal
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC10, TR10);

      // check results match
      Double e = ComplexMath.Absolute((cfl.GetDeterminant() - Det10)/Det10);
      Assert.IsTrue(e < Tolerance10);
    }
    
    #endregion GetDeterminant Method Test 10

    #region Null Parameter Test for SolveVector

    [Test]
    [ExpectedException(typeof(System.ArgumentNullException))]
    public void NullParameterTestforSolveVector()
    {
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC10, TR10);
      ComplexFloatVector X = cfl.Solve(null as ComplexFloatVector);
    }

    #endregion Null Parameter Test for SolveVector

    #region Mismatch Rows Test for SolveVector

    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchRowsTestforSolveVector()
    {
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC10, TR10);
      ComplexFloatVector X = cfl.Solve(X5);
    }

    #endregion Mismatch Rows Test for SolveVector

    #region SolveVector 1

    // Test solving a linear system
    [Test]
    public void SolveVector1()
    {
      int i;
      float e, me;
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC1, TR1);
      ComplexFloatVector X = cfl.Solve(Y1);
      
      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < cfl.Order; i++)
      {
        e = ComplexMath.Absolute((X1[i] - X[i]) / X1[i]);
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
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC2, TR2);
      ComplexFloatVector X = cfl.Solve(Y2);
      
      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < cfl.Order; i++)
      {
        e = ComplexMath.Absolute((X2[i] - X[i]) / X2[i]);
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
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC3, TR3);
      ComplexFloatVector X = cfl.Solve(Y3);
      
      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < cfl.Order; i++)
      {
        e = ComplexMath.Absolute((X3[i] - X[i]) / X3[i]);
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
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC4, TR4);
      ComplexFloatVector X = cfl.Solve(Y4);
      
      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < cfl.Order; i++)
      {
        e = ComplexMath.Absolute((X4[i] - X[i]) / X4[i]);
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
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC5, TR5);
      ComplexFloatVector X = cfl.Solve(Y5);
      
      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < cfl.Order; i++)
      {
        e = ComplexMath.Absolute((X5[i] - X[i]) / X5[i]);
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
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC10, TR10);
      ComplexFloatVector X = cfl.Solve(Y10);
      
      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < cfl.Order; i++)
      {
        e = ComplexMath.Absolute((X10[i] - X[i]) / X10[i]);
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
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC10, TR10);
      ComplexFloatMatrix X = cfl.Solve(null as ComplexFloatMatrix);
    }

    #endregion Null Parameter Test for SolveMatrix

    #region Mismatch Rows Test for SolveMatrix

    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchRowsTestforSolveMatrix()
    {
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC10, TR10);
      ComplexFloatMatrix X = cfl.Solve(I5);
    }

    #endregion Mismatch Rows Test for SolveMatrix

    #region Solve Matrix 1

    // calculate inverse by solving linear equations with identity RHS
    [Test]
    public void SolveMatrix1()
    {
      int i, j;
      float e, me;
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC1, TR1);

      // check inverse
      ComplexFloatMatrix I = cfl.Solve(ComplexFloatMatrix.CreateIdentity(1));
      me = 0.0f;
      for (i = 0; i < cfl.Order; i++)
      {
        for (j = 0; j < cfl.Order; j++)
        {
          e = ComplexMath.Absolute((I1[i, j] - I[i, j]) / I1[i, j]);
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
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC2, TR2);

      // check inverse
      ComplexFloatMatrix I = cfl.Solve(ComplexFloatMatrix.CreateIdentity(2));
      me = 0.0f;
      for (i = 0; i < cfl.Order; i++)
      {
        for (j = 0; j < cfl.Order; j++)
        {
          e = ComplexMath.Absolute((I2[i, j] - I[i, j]) / I2[i, j]);
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
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC3, TR3);

      // check inverse
      ComplexFloatMatrix I = cfl.Solve(ComplexFloatMatrix.CreateIdentity(3));
      me = 0.0f;
      for (i = 0; i < cfl.Order; i++)
      {
        for (j = 0; j < cfl.Order; j++)
        {
          e = ComplexMath.Absolute((I3[i, j] - I[i, j]) / I3[i, j]);
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
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC4, TR4);

      // check inverse
      ComplexFloatMatrix I = cfl.Solve(ComplexFloatMatrix.CreateIdentity(4));
      me = 0.0f;
      for (i = 0; i < cfl.Order; i++)
      {
        for (j = 0; j < cfl.Order; j++)
        {
          e = ComplexMath.Absolute((I4[i, j] - I[i, j]) / I4[i, j]);
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
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC5, TR5);

      // check inverse
      ComplexFloatMatrix I = cfl.Solve(ComplexFloatMatrix.CreateIdentity(5));
      me = 0.0f;
      for (i = 0; i < cfl.Order; i++)
      {
        for (j = 0; j < cfl.Order; j++)
        {
          e = ComplexMath.Absolute((I5[i, j] - I[i, j]) / I5[i, j]);
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
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC10, TR10);

      // check inverse
      ComplexFloatMatrix I = cfl.Solve(ComplexFloatMatrix.CreateIdentity(10));
      me = 0.0f;
      for (i = 0; i < cfl.Order; i++)
      {
        for (j = 0; j < cfl.Order; j++)
        {
          e = ComplexMath.Absolute((I10[i, j] - I[i, j]) / I10[i, j]);
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
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC1, TR1);

      // check inverse
      ComplexFloatMatrix I = cfl.GetInverse();
      me = 0.0f;
      for (i = 0; i < cfl.Order; i++)
      {
        for (j = 0; j < cfl.Order; j++)
        {
          e = ComplexMath.Absolute((I1[i, j] - I[i, j]) / I1[i, j]);
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
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC2, TR2);

      // check inverse
      ComplexFloatMatrix I = cfl.GetInverse();
      me = 0.0f;
      for (i = 0; i < cfl.Order; i++)
      {
        for (j = 0; j < cfl.Order; j++)
        {
          e = ComplexMath.Absolute((I2[i, j] - I[i, j]) / I2[i, j]);
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
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC3, TR3);

      // check inverse
      ComplexFloatMatrix I = cfl.GetInverse();
      me = 0.0f;
      for (i = 0; i < cfl.Order; i++)
      {
        for (j = 0; j < cfl.Order; j++)
        {
          e = ComplexMath.Absolute((I3[i, j] - I[i, j]) / I3[i, j]);
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
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC4, TR4);

      // check inverse
      ComplexFloatMatrix I = cfl.GetInverse();
      me = 0.0f;
      for (i = 0; i < cfl.Order; i++)
      {
        for (j = 0; j < cfl.Order; j++)
        {
          e = ComplexMath.Absolute((I4[i, j] - I[i, j]) / I4[i, j]);
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
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC5, TR5);

      // check inverse
      ComplexFloatMatrix I = cfl.GetInverse();
      me = 0.0f;
      for (i = 0; i < cfl.Order; i++)
      {
        for (j = 0; j < cfl.Order; j++)
        {
          e = ComplexMath.Absolute((I5[i, j] - I[i, j]) / I5[i, j]);
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
      ComplexFloatLevinson cfl = new ComplexFloatLevinson(LC10, TR10);

      // check inverse
      ComplexFloatMatrix I = cfl.GetInverse();
      me = 0.0f;
      for (i = 0; i < cfl.Order; i++)
      {
        for (j = 0; j < cfl.Order; j++)
        {
          e = ComplexMath.Absolute((I10[i, j] - I[i, j]) / I10[i, j]);
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
      ComplexFloatVector X = ComplexFloatLevinson.Solve(null, TR10, Y10);
    }

    #endregion Null Parameter Test 1 for Static SolveVector

    #region Null Parameter Test 2 for Static SolveVector
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullParameterTestforStaticSolveVector2()
    {
      ComplexFloatVector X = ComplexFloatLevinson.Solve(LC10, null, Y10);
    }

    #endregion Null Parameter Test 2 for Static SolveVector

    #region Null Parameter Test 3 for Static SolveVector
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullParameterTestforStaticSolveVector3()
    {
      ComplexFloatVector X = ComplexFloatLevinson.Solve(LC10, TR10, null as ComplexFloatVector);
    }

    #endregion Null Parameter Test 3 for Static SolveVector

    #region Zero Vector Length Test for Static SolveVector
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void ZeroVectorLengthTestforStaticSolveVector()
    {
      ComplexFloatVector LC = new ComplexFloatVector(1);
      LC.RemoveAt(0);
      ComplexFloatVector X = ComplexFloatLevinson.Solve(LC, TR10, Y10);
    }

    #endregion Zero Vector Length Test for Static SolveVector

    #region Mismatch Dimension Test 1 for Static SolveVector
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchDimensionTestforStaticSolveVector1()
    {
      ComplexFloatVector X = ComplexFloatLevinson.Solve(LC10, TR5, Y5);
    }

    #endregion Mismatch Dimension Test 1 for Static SolveVector

    #region Mismatch Dimension Test 2 for Static SolveVector
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchDimensionTestforStaticSolveVector2()
    {
      ComplexFloatVector X = ComplexFloatLevinson.Solve(LC10, TR10, Y5);
    }

    #endregion Mismatch Dimension Test 2 for Static SolveVector

    #region First Element Test for Static SolveVector

    [Test]
    [ExpectedException(typeof(System.ArithmeticException))]
    public void FirstElementTestforStaticSolveVector()
    {
      ComplexFloatVector cfv = new ComplexFloatVector(3, 1.0f);
      ComplexFloatVector X = ComplexFloatLevinson.Solve(cfv, TR3, Y3);
    }

    #endregion First Element Test for Static SolveVector

    #region Singular Test for Static SolveVector
    
    // test with Toeplitz matrix which has a singular principal sub-matrix
    [Test]
    [ExpectedException(typeof(SingularMatrixException))]
    public void SingularTestforStaticSolveVector()
    {
      ComplexFloatVector cfv = new ComplexFloatVector(3, 1.0f);
      ComplexFloatVector X = ComplexFloatLevinson.Solve(cfv, cfv, Y3);
    }
    
    #endregion Singular Test for Static SolveVector

    #region Static Solve Vector 1
    
    [Test]
    public void StaticSolveVector1()
    {
      int i;
      float e, me;
      ComplexFloatVector X = ComplexFloatLevinson.Solve(LC1, TR1, Y1);
      
      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < X.Length; i++)
      {
        e = ComplexMath.Absolute((X1[i] - X[i]) / X1[i]);
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
      ComplexFloatVector X = ComplexFloatLevinson.Solve(LC2, TR2, Y2);
      
      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < X.Length; i++)
      {
        e = ComplexMath.Absolute((X2[i] - X[i]) / X2[i]);
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
      ComplexFloatVector X = ComplexFloatLevinson.Solve(LC3, TR3, Y3);
      
      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < X.Length; i++)
      {
        e = ComplexMath.Absolute((X3[i] - X[i]) / X3[i]);
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
      ComplexFloatVector X = ComplexFloatLevinson.Solve(LC4, TR4, Y4);
      
      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < X.Length; i++)
      {
        e = ComplexMath.Absolute((X4[i] - X[i]) / X4[i]);
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
      ComplexFloatVector X = ComplexFloatLevinson.Solve(LC5, TR5, Y5);
      
      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < X.Length; i++)
      {
        e = ComplexMath.Absolute((X5[i] - X[i]) / X5[i]);
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
      ComplexFloatVector X = ComplexFloatLevinson.Solve(LC10, TR10, Y10);
      
      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < X.Length; i++)
      {
        e = ComplexMath.Absolute((X10[i] - X[i]) / X10[i]);
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
      ComplexFloatMatrix X = ComplexFloatLevinson.Solve(null, TR10, ComplexFloatMatrix.CreateIdentity(10));
    }

    #endregion Null Parameter Test 1 for Static SolveMatrix

    #region Null Parameter Test 2 for Static SolveMatrix
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullParameterTestforStaticSolveMatrix2()
    {
      ComplexFloatMatrix X = ComplexFloatLevinson.Solve(LC10, null, ComplexFloatMatrix.CreateIdentity(10));
    }

    #endregion Null Parameter Test 2 for Static SolveMatrix

    #region Null Parameter Test 3 for Static SolveMatrix
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullParameterTestforStaticSolveMatrix3()
    {
      ComplexFloatMatrix X = ComplexFloatLevinson.Solve(LC10, TR10, null as ComplexFloatMatrix);
    }

    #endregion Null Parameter Test 3 for Static SolveMatrix

    #region Zero Vector Length Test for Static SolveMatrix
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void ZeroVectorLengthTestforStaticSolveMatrix()
    {
      ComplexFloatVector LC = new ComplexFloatVector(1);
      LC.RemoveAt(0);
      ComplexFloatMatrix X = ComplexFloatLevinson.Solve(LC, TR10, ComplexFloatMatrix.CreateIdentity(10));
    }

    #endregion Zero Vector Length Test for Static SolveMatrix

    #region Mismatch Dimension Test 1 for Static SolveMatrix
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchDimensionTestforStaticSolveMatrix1()
    {
      ComplexFloatMatrix X = ComplexFloatLevinson.Solve(LC10, TR5, ComplexFloatMatrix.CreateIdentity(5));
    }

    #endregion Mismatch Dimension Test 1 for Static SolveMatrix

    #region Mismatch Dimension Test 2 for Static SolveMatrix
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchDimensionTestforStaticSolveMatrix2()
    {
      ComplexFloatMatrix X = ComplexFloatLevinson.Solve(LC10, TR10, ComplexFloatMatrix.CreateIdentity(5));
    }

    #endregion Mismatch Dimension Test 2 for Static SolveMatrix

    #region First Element Test for Static SolveMatrix

    [Test]
    [ExpectedException(typeof(System.ArithmeticException))]
    public void FirstElementTestforStaticSolveMatrix()
    {
      ComplexFloatVector cfv = new ComplexFloatVector(3, 1.0f);
      ComplexFloatMatrix X = ComplexFloatLevinson.Solve(cfv, TR3, ComplexFloatMatrix.CreateIdentity(3));
    }

    #endregion First Element Test for Static SolveMatrix

    #region Singular Test for Static SolveMatrix
    
    // test with Toeplitz matrix which has a singular principal sub-matrix
    [Test]
    [ExpectedException(typeof(SingularMatrixException))]
    public void SingularTestforStaticSolveMatrix()
    {
      ComplexFloatVector cfv = new ComplexFloatVector(3, 1.0f);
      ComplexFloatMatrix X = ComplexFloatLevinson.Solve(cfv, cfv, ComplexFloatMatrix.CreateIdentity(3));
    }
    
    #endregion Singular Test for Static SolveMatrix

    #region Static Solve Matrix 1
    
    [Test]
    public void StaticSolveMatrix1()
    {
      int i, j;
      float e, me;
      ComplexFloatMatrix I = ComplexFloatLevinson.Solve(LC1, TR1, ComplexFloatMatrix.CreateIdentity(1));
      
      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < I.ColumnLength; i++)
      {
        for (j = 0; j < I.RowLength; j++)
        {
          e = ComplexMath.Absolute((I1[i, j] - I[i, j]) / I1[i, j]);
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
      ComplexFloatMatrix I = ComplexFloatLevinson.Solve(LC2, TR2, ComplexFloatMatrix.CreateIdentity(2));
      
      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < I.ColumnLength; i++)
      {
        for (j = 0; j < I.RowLength; j++)
        {
          e = ComplexMath.Absolute((I2[i, j] - I[i, j]) / I2[i, j]);
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
      ComplexFloatMatrix I = ComplexFloatLevinson.Solve(LC3, TR3, ComplexFloatMatrix.CreateIdentity(3));
      
      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < I.ColumnLength; i++)
      {
        for (j = 0; j < I.RowLength; j++)
        {
          e = ComplexMath.Absolute((I3[i, j] - I[i, j]) / I3[i, j]);
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
      ComplexFloatMatrix I = ComplexFloatLevinson.Solve(LC4, TR4, ComplexFloatMatrix.CreateIdentity(4));
      
      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < I.ColumnLength; i++)
      {
        for (j = 0; j < I.RowLength; j++)
        {
          e = ComplexMath.Absolute((I4[i, j] - I[i, j]) / I4[i, j]);
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
      ComplexFloatMatrix I = ComplexFloatLevinson.Solve(LC5, TR5, ComplexFloatMatrix.CreateIdentity(5));
      
      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < I.ColumnLength; i++)
      {
        for (j = 0; j < I.RowLength; j++)
        {
          e = ComplexMath.Absolute((I5[i, j] - I[i, j]) / I5[i, j]);
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
      ComplexFloatMatrix I = ComplexFloatLevinson.Solve(LC10, TR10, ComplexFloatMatrix.CreateIdentity(10));
      
      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < I.ColumnLength; i++)
      {
        for (j = 0; j < I.RowLength; j++)
        {
          e = ComplexMath.Absolute((I10[i, j] - I[i, j]) / I10[i, j]);
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
      ComplexFloatMatrix X = ComplexFloatLevinson.Inverse(null, TR10);
    }

    #endregion Null Parameter Test 1 for Static Inverse

    #region Null Parameter Test 2 for Static Inverse
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullParameterTestforStaticInverse2()
    {
      ComplexFloatMatrix X = ComplexFloatLevinson.Inverse(LC10, null);
    }

    #endregion Null Parameter Test 2 for Static Inverse

    #region Zero Vector Length Test for Static Inverse
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void ZeroVectorLengthTestforStaticInverse()
    {
      ComplexFloatVector LC = new ComplexFloatVector(1);
      LC.RemoveAt(0);
      ComplexFloatMatrix X = ComplexFloatLevinson.Inverse(LC, LC);
    }

    #endregion Zero Vector Length Test for Static Inverse

    #region Mismatch Dimension Test for Static Inverse
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchDimensionTestforStaticInverse()
    {
      ComplexFloatMatrix X = ComplexFloatLevinson.Inverse(LC10, TR5);
    }

    #endregion Mismatch Dimension Test for Static Inverse

    #region First Element Test for Static Inverse

    [Test]
    [ExpectedException(typeof(System.ArithmeticException))]
    public void FirstElementTestforStaticInverse()
    {
      ComplexFloatVector cfv = new ComplexFloatVector(3, 1.0f);
      ComplexFloatMatrix X = ComplexFloatLevinson.Inverse(cfv, TR3);
    }

    #endregion First Element Test for Static  Inverse

    #region Singular Test for Static Inverse
    
    // test with Toeplitz matrix which has a singular principal sub-matrix
    [Test]
    [ExpectedException(typeof(SingularMatrixException))]
    public void SingularTestforStaticInverse()
    {
      ComplexFloatVector cfv = new ComplexFloatVector(3, 1.0f);
      ComplexFloatMatrix X = ComplexFloatLevinson.Inverse(cfv, cfv);
    }
    
    #endregion Singular Test for Static Inverse

    #region Static Inverse 1

    [Test]
    public void StaticInverse1()
    {
      int i, j;
      float e, me;

      // calculate the inverse
      ComplexFloatMatrix I = ComplexFloatLevinson.Inverse(LC1, TR1);

      // determine the maximum relative error
      me = 0.0f;
      for (i = 0; i < I.ColumnLength; i++)
      {
        for (j = 0; j < I.RowLength; j++)
        {
          e = ComplexMath.Absolute((I1[i, j] - I[i, j]) / I1[i, j]);
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
      ComplexFloatMatrix I = ComplexFloatLevinson.Inverse(LC2, TR2);

      // determine the maximum relative error
      me = 0.0f;
      for (i = 0; i < I.ColumnLength; i++)
      {
        for (j = 0; j < I.RowLength; j++)
        {
          e = ComplexMath.Absolute((I2[i, j] - I[i, j]) / I2[i, j]);
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
      ComplexFloatMatrix I = ComplexFloatLevinson.Inverse(LC3, TR3);

      // determine the maximum relative error
      me = 0.0f;
      for (i = 0; i < I.ColumnLength; i++)
      {
        for (j = 0; j < I.RowLength; j++)
        {
          e = ComplexMath.Absolute((I3[i, j] - I[i, j]) / I3[i, j]);
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
      ComplexFloatMatrix I = ComplexFloatLevinson.Inverse(LC4, TR4);

      // determine the maximum relative error
      me = 0.0f;
      for (i = 0; i < I.ColumnLength; i++)
      {
        for (j = 0; j < I.RowLength; j++)
        {
          e = ComplexMath.Absolute((I4[i, j] - I[i, j]) / I4[i, j]);
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
      ComplexFloatMatrix I = ComplexFloatLevinson.Inverse(LC5, TR5);

      // determine the maximum relative error
      me = 0.0f;
      for (i = 0; i < I.ColumnLength; i++)
      {
        for (j = 0; j < I.RowLength; j++)
        {
          e = ComplexMath.Absolute((I5[i, j] - I[i, j]) / I5[i, j]);
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
      ComplexFloatMatrix I = ComplexFloatLevinson.Inverse(LC10, TR10);

      // determine the maximum relative error
      me = 0.0f;
      for (i = 0; i < I.ColumnLength; i++)
      {
        for (j = 0; j < I.RowLength; j++)
        {
          e = ComplexMath.Absolute((I10[i, j] - I[i, j]) / I10[i, j]);
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

#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#endregion Copyright

#region Using Directive

using System;
using Altaxo.Calc.LinearAlgebra;
using Xunit;

#endregion Using Directive

namespace AltaxoTest.Calc.LinearAlgebra
{
  // suite of tests for float symmetric Levinson algorithm


  public class FloatSymmetricLevinsonTest
  {
    #region Fields

    // unit testing - order 1

    private FloatVector T1;     // Toeplitz matrix
    private FloatMatrix A1;     // Lower triangle matrix
    private FloatVector D1;     // diagonal vector
    private FloatMatrix I1;     // inverse matrix
    private float Det1;       // exact determinant
    private FloatVector X1;     // RHS vector
    private FloatVector Y1;     // LHS vector
    private float Tolerance1;     // allowable tolerance

    // unit testing - order 2

    private FloatVector T2;     // Toeplitz matrix
    private FloatMatrix A2;     // Lower triangle matrix
    private FloatVector D2;     // diagonal vector
    private FloatMatrix I2;     // inverse matrix
    private float Det2;       // exact determinant
    private FloatVector X2;     // RHS vector
    private FloatVector Y2;     // LHS vector
    private float Tolerance2;     // allowable tolerance

    // unit testing - order 3

    private FloatVector T3;     // Toeplitz matrix
    private FloatMatrix A3;     // Lower triangle matrix
    private FloatVector D3;     // diagonal vector
    private FloatMatrix I3;     // inverse matrix
    private float Det3;       // exact determinant
    private FloatVector X3;     // RHS vector
    private FloatVector Y3;     // LHS vector
    private float Tolerance3;     // allowable tolerance

    // unit testing - order 4

    private FloatVector T4;     // Toeplitz matrix
    private FloatMatrix A4;     // Lower triangle matrix
    private FloatVector D4;     // diagonal vector
    private FloatMatrix I4;     // inverse matrix
    private float Det4;       // exact determinant
    private FloatVector X4;     // RHS vector
    private FloatVector Y4;     // LHS vector
    private float Tolerance4;     // allowable tolerance

    // unit testing - order 5

    private FloatVector T5;     // Toeplitz matrix
    private FloatMatrix A5;     // Lower triangle matrix
    private FloatVector D5;     // diagonal vector
    private FloatMatrix I5;     // inverse matrix
    private float Det5;       // exact determinant
    private FloatVector X5;     // RHS vector
    private FloatVector Y5;     // LHS vector
    private float Tolerance5;     // allowable tolerance

    // unit testing - order 10

    private FloatVector T10;      // Toeplitz matrix
    private FloatMatrix A10;      // Lower triangle matrix
    private FloatVector D10;      // diagonal vector
    private FloatMatrix I10;      // inverse matrix
    private float Det10;        // exact determinant
    private FloatVector X10;      // RHS vector
    private FloatVector Y10;      // LHS vector
    private float Tolerance10;      // allowable tolerance

    #endregion Fields

    #region Test Fixture Setup

    // [OneTimeSetUp]
    public FloatSymmetricLevinsonTest()
    {
      // unit testing values - order 1

      T1 = new FloatVector(1)
      {
        [0] = +3.0000000E+000f
      };

      A1 = new FloatMatrix(1)
      {
        [0, 0] = +1.0000000E+000f
      };

      D1 = new FloatVector(1)
      {
        [0] = +3.3333334E-001f
      };

      Det1 = +3.0000000E+000f;

      I1 = new FloatMatrix(1)
      {
        [0, 0] = +3.3333334E-001f
      };

      X1 = new FloatVector(1)
      {
        [0] = +1.0000000E+000f
      };

      Y1 = new FloatVector(1)
      {
        [0] = +3.0000000E+000f
      };

      Tolerance1 = 1.0E-006f;

      // unit testing values - order 2

      T2 = new FloatVector(2)
      {
        [0] = +1.0000000E+001f,
        [1] = +4.5000000E+000f
      };

      A2 = new FloatMatrix(2)
      {
        [0, 0] = +1.0000000E+000f,
        [1, 0] = -4.4999999E-001f,
        [1, 1] = +1.0000000E+000f
      };

      D2 = new FloatVector(2)
      {
        [0] = +1.0000000E-001f,
        [1] = +1.2539186E-001f
      };

      Det2 = +7.9750000E+001f;

      I2 = new FloatMatrix(2)
      {
        [0, 0] = +1.2539186E-001f,
        [0, 1] = -5.6426331E-002f,
        [1, 0] = -5.6426331E-002f,
        [1, 1] = +1.2539186E-001f
      };

      X2 = new FloatVector(2)
      {
        [0] = +1.0000000E+000f,
        [1] = +2.0000000E+000f
      };

      Y2 = new FloatVector(2)
      {
        [0] = +1.9000000E+001f,
        [1] = +2.4500000E+001f
      };

      Tolerance2 = 1.0E-006f;

      // unit testing values - order 3

      T3 = new FloatVector(3)
      {
        [0] = +4.0000000E+000f,
        [1] = +1.5000000E+000f,
        [2] = +6.6666669E-001f
      };

      A3 = new FloatMatrix(3)
      {
        [0, 0] = +1.0000000E+000f,
        [1, 0] = -3.7500000E-001f,
        [1, 1] = +1.0000000E+000f,
        [2, 0] = -3.0303031E-002f,
        [2, 1] = -3.6363637E-001f,
        [2, 2] = +1.0000000E+000f
      };

      D3 = new FloatVector(3)
      {
        [0] = +2.5000000E-001f,
        [1] = +2.9090908E-001f,
        [2] = +2.9117647E-001f
      };

      Det3 = +4.7222221E+001f;

      I3 = new FloatMatrix(3)
      {
        [0, 0] = +2.9117647E-001f,
        [0, 1] = -1.0588235E-001f,
        [0, 2] = -8.8235298E-003f,
        [1, 0] = -1.0588235E-001f,
        [1, 1] = +3.2941177E-001f,
        [1, 2] = -1.0588235E-001f,
        [2, 0] = -8.8235298E-003f,
        [2, 1] = -1.0588235E-001f,
        [2, 2] = +2.9117647E-001f
      };

      X3 = new FloatVector(3)
      {
        [0] = +1.0000000E+000f,
        [1] = +2.0000000E+000f,
        [2] = +3.0000000E+000f
      };

      Y3 = new FloatVector(3)
      {
        [0] = +9.0000000E+000f,
        [1] = +1.4000000E+001f,
        [2] = +1.5666667E+001f
      };

      Tolerance3 = 1.5E-006f;

      // unit testing values - order 4

      T4 = new FloatVector(4)
      {
        [0] = +4.0000000E+000f,
        [1] = +1.5000000E+000f,
        [2] = +6.6666669E-001f,
        [3] = +2.5000000E-001f
      };

      A4 = new FloatMatrix(4)
      {
        [0, 0] = +1.0000000E+000f,
        [1, 0] = -3.7500000E-001f,
        [1, 1] = +1.0000000E+000f,
        [2, 0] = -3.0303031E-002f,
        [2, 1] = -3.6363637E-001f,
        [2, 2] = +1.0000000E+000f,
        [3, 0] = +1.1029412E-002f,
        [3, 1] = -3.4313727E-002f,
        [3, 2] = -3.6397058E-001f,
        [3, 3] = +1.0000000E+000f
      };

      D4 = new FloatVector(4)
      {
        [0] = +2.5000000E-001f,
        [1] = +2.9090908E-001f,
        [2] = +2.9117647E-001f,
        [3] = +2.9121190E-001f
      };

      Det4 = +1.6215759E+002f;

      I4 = new FloatMatrix(4)
      {
        [0, 0] = +2.9121190E-001f,
        [0, 1] = -1.0599256E-001f,
        [0, 2] = -9.9925650E-003f,
        [0, 3] = +3.2118959E-003f,
        [1, 0] = -1.0599256E-001f,
        [1, 1] = +3.2975465E-001f,
        [1, 2] = -1.0224535E-001f,
        [1, 3] = -9.9925650E-003f,
        [2, 0] = -9.9925650E-003f,
        [2, 1] = -1.0224535E-001f,
        [2, 2] = +3.2975465E-001f,
        [2, 3] = -1.0599256E-001f,
        [3, 0] = +3.2118959E-003f,
        [3, 1] = -9.9925650E-003f,
        [3, 2] = -1.0599256E-001f,
        [3, 3] = +2.9121190E-001f
      };

      X4 = new FloatVector(4)
      {
        [0] = +1.0000000E+000f,
        [1] = +2.0000000E+000f,
        [2] = +3.0000000E+000f,
        [3] = +4.0000000E+000f
      };

      Y4 = new FloatVector(4)
      {
        [0] = +1.0000000E+001f,
        [1] = +1.6666666E+001f,
        [2] = +2.1666666E+001f,
        [3] = +2.2083334E+001f
      };

      Tolerance4 = 2.5E-006f;

      // unit testing values - order 5

      T5 = new FloatVector(5)
      {
        [0] = +1.0000000E+000f,
        [1] = +5.0000000E-001f,
        [2] = +3.3333334E-001f,
        [3] = +2.5000000E-001f,
        [4] = +2.0000000E-001f
      };

      A5 = new FloatMatrix(5)
      {
        [0, 0] = +1.0000000E+000f,
        [1, 0] = -5.0000000E-001f,
        [1, 1] = +1.0000000E+000f,
        [2, 0] = -1.1111111E-001f,
        [2, 1] = -4.4444445E-001f,
        [2, 2] = +1.0000000E+000f,
        [3, 0] = -6.2500000E-002f,
        [3, 1] = -8.3333336E-002f,
        [3, 2] = -4.3750000E-001f,
        [3, 3] = +1.0000000E+000f,
        [4, 0] = -4.2823531E-002f,
        [4, 1] = -4.3764707E-002f,
        [4, 2] = -7.9764709E-002f,
        [4, 3] = -4.3482354E-001f,
        [4, 4] = +1.0000000E+000f
      };

      D5 = new FloatVector(5)
      {
        [0] = +1.0000000E+000f,
        [1] = +1.3333334E+000f,
        [2] = +1.3500000E+000f,
        [3] = +1.3552941E+000f,
        [4] = +1.3577842E+000f
      };

      Det5 = +3.0190009E-001f;

      I5 = new FloatMatrix(5)
      {
        [0, 0] = +1.3577842E+000f,
        [0, 1] = -5.9039646E-001f,
        [0, 2] = -1.0830325E-001f,
        [0, 3] = -5.9423022E-002f,
        [0, 4] = -5.8145106E-002f,
        [1, 0] = -5.9039646E-001f,
        [1, 1] = +1.6120124E+000f,
        [1, 2] = -5.4584837E-001f,
        [1, 3] = -8.7102652E-002f,
        [1, 4] = -5.9423022E-002f,
        [2, 0] = -1.0830325E-001f,
        [2, 1] = -5.4584837E-001f,
        [2, 2] = +1.6180506E+000f,
        [2, 3] = -5.4584837E-001f,
        [2, 4] = -1.0830325E-001f,
        [3, 0] = -5.9423022E-002f,
        [3, 1] = -8.7102652E-002f,
        [3, 2] = -5.4584837E-001f,
        [3, 3] = +1.6120124E+000f,
        [3, 4] = -5.9039646E-001f,
        [4, 0] = -5.8145106E-002f,
        [4, 1] = -5.9423022E-002f,
        [4, 2] = -1.0830325E-001f,
        [4, 3] = -5.9039646E-001f,
        [4, 4] = +1.3577842E+000f
      };

      X5 = new FloatVector(5)
      {
        [0] = +1.0000000E+000f,
        [1] = +2.0000000E+000f,
        [2] = +3.0000000E+000f,
        [3] = +4.0000000E+000f,
        [4] = +5.0000000E+000f
      };

      Y5 = new FloatVector(5)
      {
        [0] = +5.0000000E+000f,
        [1] = +6.5833335E+000f,
        [2] = +8.0000000E+000f,
        [3] = +8.9166670E+000f,
        [4] = +8.6999998E+000f
      };

      Tolerance5 = 2.5E-006f;

      // unit testing values - order 10

      T10 = new FloatVector(10)
      {
        [0] = +1.0000000E+001f,
        [1] = +5.0000000E+000f,
        [2] = +3.3333333E+000f,
        [3] = +2.5000000E+000f,
        [4] = +2.0000000E+000f,
        [5] = +1.6666666E+000f,
        [6] = +1.4285715E+000f,
        [7] = +1.2500000E+000f,
        [8] = +1.1111112E+000f,
        [9] = +1.0000000E+000f
      };

      A10 = new FloatMatrix(10)
      {
        [0, 0] = +1.0000000E+000f,
        [1, 0] = -5.0000000E-001f,
        [1, 1] = +1.0000000E+000f,
        [2, 0] = -1.1111111E-001f,
        [2, 1] = -4.4444445E-001f,
        [2, 2] = +1.0000000E+000f,
        [3, 0] = -6.2500000E-002f,
        [3, 1] = -8.3333336E-002f,
        [3, 2] = -4.3750000E-001f,
        [3, 3] = +1.0000000E+000f,
        [4, 0] = -4.2823531E-002f,
        [4, 1] = -4.3764707E-002f,
        [4, 2] = -7.9764709E-002f,
        [4, 3] = -4.3482354E-001f,
        [4, 4] = +1.0000000E+000f,
        [5, 0] = -3.2262016E-002f,
        [5, 1] = -2.8795246E-002f,
        [5, 2] = -4.1191336E-002f,
        [5, 3] = -7.8352772E-002f,
        [5, 4] = -4.3344197E-001f,
        [5, 5] = +1.0000000E+000f,
        [6, 0] = -2.5714690E-002f,
        [6, 1] = -2.1116190E-002f,
        [6, 2] = -2.6780428E-002f,
        [6, 3] = -4.0132113E-002f,
        [6, 4] = -7.7612311E-002f,
        [6, 5] = -4.3261236E-001f,
        [6, 6] = +1.0000000E+000f,
        [7, 0] = -2.1279071E-002f,
        [7, 1] = -1.6509103E-002f,
        [7, 2] = -1.9464672E-002f,
        [7, 3] = -2.5926454E-002f,
        [7, 4] = -3.9562251E-002f,
        [7, 5] = -7.7162974E-002f,
        [7, 6] = -4.3206516E-001f,
        [7, 7] = +1.0000000E+000f,
        [8, 0] = -1.8086541E-002f,
        [8, 1] = -1.3464506E-002f,
        [8, 2] = -1.5113491E-002f,
        [8, 3] = -1.8749127E-002f,
        [8, 4] = -2.5457535E-002f,
        [8, 5] = -3.9210200E-002f,
        [8, 6] = -7.6864384E-002f,
        [8, 7] = -4.3168029E-001f,
        [8, 8] = +1.0000000E+000f,
        [9, 0] = -1.5685264E-002f,
        [9, 1] = -1.1315523E-002f,
        [9, 2] = -1.2258868E-002f,
        [9, 3] = -1.4498468E-002f,
        [9, 4] = -1.8349819E-002f,
        [9, 5] = -2.5163449E-002f,
        [9, 6] = -3.8973141E-002f,
        [9, 7] = -7.6653190E-002f,
        [9, 8] = -4.3139660E-001f,
        [9, 9] = +1.0000000E+000f
      };

      D10 = new FloatVector(10)
      {
        [0] = +1.0000000E-001f,
        [1] = +1.3333334E-001f,
        [2] = +1.3500001E-001f,
        [3] = +1.3552941E-001f,
        [4] = +1.3577841E-001f,
        [5] = +1.3591988E-001f,
        [6] = +1.3600981E-001f,
        [7] = +1.3607143E-001f,
        [8] = +1.3611595E-001f,
        [9] = +1.3614945E-001f
      };

      Det10 = +6.4761683E+008f;

      I10 = new FloatMatrix(10)
      {
        [0, 0] = +1.3614945E-001f,
        [0, 1] = -5.8734413E-002f,
        [0, 2] = -1.0436290E-002f,
        [0, 3] = -5.3061722E-003f,
        [0, 4] = -3.4259900E-003f,
        [0, 5] = -2.4983177E-003f,
        [0, 6] = -1.9739585E-003f,
        [0, 7] = -1.6690382E-003f,
        [0, 8] = -1.5406023E-003f,
        [0, 9] = -2.1355401E-003f,
        [1, 0] = -5.8734413E-002f,
        [1, 1] = +1.6145378E-001f,
        [1, 2] = -5.4256398E-002f,
        [1, 3] = -8.1734043E-003f,
        [1, 4] = -3.8591737E-003f,
        [1, 5] = -2.3874110E-003f,
        [1, 6] = -1.7004963E-003f,
        [1, 7] = -1.3371698E-003f,
        [1, 8] = -1.1681236E-003f,
        [1, 9] = -1.5406023E-003f,
        [2, 0] = -1.0436290E-002f,
        [2, 1] = -5.4256398E-002f,
        [2, 2] = +1.6223632E-001f,
        [2, 3] = -5.3868547E-002f,
        [2, 4] = -7.9331277E-003f,
        [2, 5] = -3.6959394E-003f,
        [2, 6] = -2.2748676E-003f,
        [2, 7] = -1.6326014E-003f,
        [2, 8] = -1.3371698E-003f,
        [2, 9] = -1.6690382E-003f,
        [3, 0] = -5.3061722E-003f,
        [3, 1] = -8.1734043E-003f,
        [3, 2] = -5.3868547E-002f,
        [3, 3] = +1.6242266E-001f,
        [3, 4] = -5.3759225E-002f,
        [3, 5] = -7.8663863E-003f,
        [3, 6] = -3.6610069E-003f,
        [3, 7] = -2.2748676E-003f,
        [3, 8] = -1.7004963E-003f,
        [3, 9] = -1.9739585E-003f,
        [4, 0] = -3.4259900E-003f,
        [4, 1] = -3.8591737E-003f,
        [4, 2] = -7.9331277E-003f,
        [4, 3] = -5.3759225E-002f,
        [4, 4] = +1.6248025E-001f,
        [4, 5] = -5.3732581E-002f,
        [4, 6] = -7.8663863E-003f,
        [4, 7] = -3.6959394E-003f,
        [4, 8] = -2.3874110E-003f,
        [4, 9] = -2.4983177E-003f,
        [5, 0] = -2.4983177E-003f,
        [5, 1] = -2.3874110E-003f,
        [5, 2] = -3.6959394E-003f,
        [5, 3] = -7.8663863E-003f,
        [5, 4] = -5.3732581E-002f,
        [5, 5] = +1.6248025E-001f,
        [5, 6] = -5.3759225E-002f,
        [5, 7] = -7.9331277E-003f,
        [5, 8] = -3.8591737E-003f,
        [5, 9] = -3.4259900E-003f,
        [6, 0] = -1.9739585E-003f,
        [6, 1] = -1.7004963E-003f,
        [6, 2] = -2.2748676E-003f,
        [6, 3] = -3.6610069E-003f,
        [6, 4] = -7.8663863E-003f,
        [6, 5] = -5.3759225E-002f,
        [6, 6] = +1.6242266E-001f,
        [6, 7] = -5.3868547E-002f,
        [6, 8] = -8.1734043E-003f,
        [6, 9] = -5.3061722E-003f,
        [7, 0] = -1.6690382E-003f,
        [7, 1] = -1.3371698E-003f,
        [7, 2] = -1.6326014E-003f,
        [7, 3] = -2.2748676E-003f,
        [7, 4] = -3.6959394E-003f,
        [7, 5] = -7.9331277E-003f,
        [7, 6] = -5.3868547E-002f,
        [7, 7] = +1.6223632E-001f,
        [7, 8] = -5.4256398E-002f,
        [7, 9] = -1.0436290E-002f,
        [8, 0] = -1.5406023E-003f,
        [8, 1] = -1.1681236E-003f,
        [8, 2] = -1.3371698E-003f,
        [8, 3] = -1.7004963E-003f,
        [8, 4] = -2.3874110E-003f,
        [8, 5] = -3.8591737E-003f,
        [8, 6] = -8.1734043E-003f,
        [8, 7] = -5.4256398E-002f,
        [8, 8] = +1.6145378E-001f,
        [8, 9] = -5.8734413E-002f,
        [9, 0] = -2.1355401E-003f,
        [9, 1] = -1.5406023E-003f,
        [9, 2] = -1.6690382E-003f,
        [9, 3] = -1.9739585E-003f,
        [9, 4] = -2.4983177E-003f,
        [9, 5] = -3.4259900E-003f,
        [9, 6] = -5.3061722E-003f,
        [9, 7] = -1.0436290E-002f,
        [9, 8] = -5.8734413E-002f,
        [9, 9] = +1.3614945E-001f
      };

      X10 = new FloatVector(10)
      {
        [0] = +1.0000000E+000f,
        [1] = +2.0000000E+000f,
        [2] = +3.0000000E+000f,
        [3] = +4.0000000E+000f,
        [4] = +5.0000000E+000f,
        [5] = +6.0000000E+000f,
        [6] = +7.0000000E+000f,
        [7] = +8.0000000E+000f,
        [8] = +9.0000000E+000f,
        [9] = +1.0000000E+001f
      };

      Y10 = new FloatVector(10)
      {
        [0] = +1.0000000E+002f,
        [1] = +1.2328968E+002f,
        [2] = +1.4769048E+002f,
        [3] = +1.7195238E+002f,
        [4] = +1.9500000E+002f,
        [5] = +2.1566667E+002f,
        [6] = +2.3242857E+002f,
        [7] = +2.4294048E+002f,
        [8] = +2.4289682E+002f,
        [9] = +2.2218651E+002f
      };

      Tolerance10 = 1.0E-005f;
    }

    #endregion Test Fixture Setup

    #region Null Parameter Tests for Constructor

    // Test constructor with a null parameter
    [Fact]
    public void NullParameterTestforConstructor1()
    {
      Assert.Throws<System.ArgumentNullException>(() =>
      {
        var fsl = new FloatSymmetricLevinson(null as FloatVector);
      });
    }

    // Test constructor with a null parameter
    [Fact]
    public void NullParameterTestforConstructor2()
    {
      Assert.Throws<System.ArgumentNullException>(() =>
      {
        var fsl = new FloatSymmetricLevinson(null as float[]);
      });
    }

    #endregion Null Parameter Tests for Constructor

    #region Zero Length Vector Tests for Constructor

    // Test constructor with a zero length vector parameter
    [Fact]
    public void ZeroLengthVectorTestsforConstructor1()
    {
      Assert.Throws<System.RankException>(() =>
      {
        var dv = new FloatVector();
        var fsl = new FloatSymmetricLevinson(dv);
      });
    }

    [Fact]
    public void ZeroLengthVectorTestsforConstructor2()
    {
      Assert.Throws<System.RankException>(() =>
      {
        float[] dv = new float[0];
        var fsl = new FloatSymmetricLevinson(dv);
      });
    }

    #endregion Zero Length Vector Tests for Constructor

    #region GetVector Member Test

    // check get vector
    [Fact]
    public void GetVectorMemberTest()
    {
      var fsl = new FloatSymmetricLevinson(T5);
      FloatVector TT = fsl.GetVector();
      Assert.True(T5.Equals(TT));
    }

    #endregion GetVector Member Test

    #region GetMatrix Member Test

    // check get matrix
    [Fact]
    public void GetMatrixMemberTest()
    {
      var fsl = new FloatSymmetricLevinson(T5);
      FloatMatrix fsldm = fsl.GetMatrix();
      for (int row = 0; row < T5.Length; row++)
      {
        for (int column = 0; column < T5.Length; column++)
        {
          if (column < row)
          {
            Assert.True(fsldm[row, column] == T5[row - column]);
          }
          else
          {
            Assert.True(fsldm[row, column] == T5[column - row]);
          }
        }
      }
    }

    #endregion GetMatrix Member Test

    #region Order Property Test

    // test order property
    [Fact]
    public void OrderPropertyTest()
    {
      var fsl = new FloatSymmetricLevinson(T5);
      Assert.True(fsl.Order == 5);
    }

    #endregion Order Property Test

    #region Decomposition Test 1

    // test the UDL factorisation for case 1
    [Fact]
    public void DecompositionTest1()
    {
      int i, j;
      float e, me;
      var fsl = new FloatSymmetricLevinson(T1);
      FloatMatrix U = fsl.U;
      FloatMatrix D = fsl.D;
      FloatMatrix L = fsl.L;

      // check U is the transpose of L
      Assert.True(U.Equals(L.GetTranspose()));

      // check the lower triangle
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        for (j = 0; j <= i; j++)
        {
          e = System.Math.Abs((A1[i, j] - L[i, j]) / A1[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.True(me < Tolerance1, "Maximum Error = " + me.ToString());

      // check the diagonal
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        e = System.Math.Abs((D1[i] - D[i, i]) / D1[i]);
        if (e > me)
        {
          me = e;
        }
      }

      Assert.True(me < Tolerance1, "Maximum Error = " + me.ToString());
    }

    #endregion Decomposition Test 1

    #region Decomposition Test 2

    // test the UDL factorisation for case 2
    [Fact]
    public void DecompositionTest2()
    {
      int i, j;
      float e, me;
      var fsl = new FloatSymmetricLevinson(T2);
      FloatMatrix U = fsl.U;
      FloatMatrix D = fsl.D;
      FloatMatrix L = fsl.L;

      // check U is the transpose of L
      Assert.True(U.Equals(L.GetTranspose()));

      // check the lower triangle
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        for (j = 0; j <= i; j++)
        {
          e = System.Math.Abs((A2[i, j] - L[i, j]) / A2[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.True(me < Tolerance2, "Maximum Error = " + me.ToString());

      // check the diagonal
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        e = System.Math.Abs((D2[i] - D[i, i]) / D2[i]);
        if (e > me)
        {
          me = e;
        }
      }

      Assert.True(me < Tolerance2, "Maximum Error = " + me.ToString());
    }

    #endregion Decomposition Test 2

    #region Decomposition Test 3

    // test the UDL factorisation for case 3
    [Fact]
    public void DecompositionTest3()
    {
      int i, j;
      float e, me;
      var fsl = new FloatSymmetricLevinson(T3);
      FloatMatrix U = fsl.U;
      FloatMatrix D = fsl.D;
      FloatMatrix L = fsl.L;

      // check U is the transpose of L
      Assert.True(U.Equals(L.GetTranspose()));

      // check the lower triangle
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        for (j = 0; j <= i; j++)
        {
          e = System.Math.Abs((A3[i, j] - L[i, j]) / A3[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.True(me < Tolerance3, "Maximum Error = " + me.ToString());

      // check the diagonal
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        e = System.Math.Abs((D3[i] - D[i, i]) / D3[i]);
        if (e > me)
        {
          me = e;
        }
      }

      Assert.True(me < Tolerance3, "Maximum Error = " + me.ToString());
    }

    #endregion Decomposition Test 3

    #region Decomposition Test 4

    // test the UDL factorisation for case 4
    [Fact]
    public void DecompositionTest4()
    {
      int i, j;
      float e, me;
      var fsl = new FloatSymmetricLevinson(T4);
      FloatMatrix U = fsl.U;
      FloatMatrix D = fsl.D;
      FloatMatrix L = fsl.L;

      // check U is the transpose of L
      Assert.True(U.Equals(L.GetTranspose()));

      // check the lower triangle
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        for (j = 0; j <= i; j++)
        {
          e = System.Math.Abs((A4[i, j] - L[i, j]) / A4[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.True(me < Tolerance4, "Maximum Error = " + me.ToString());

      // check the diagonal
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        e = System.Math.Abs((D4[i] - D[i, i]) / D4[i]);
        if (e > me)
        {
          me = e;
        }
      }

      Assert.True(me < Tolerance4, "Maximum Error = " + me.ToString());
    }

    #endregion Decomposition Test 4

    #region Decomposition Test 5

    // test the UDL factorisation for case 5
    [Fact]
    public void DecompositionTest5()
    {
      int i, j;
      float e, me;
      var fsl = new FloatSymmetricLevinson(T5);
      FloatMatrix U = fsl.U;
      FloatMatrix D = fsl.D;
      FloatMatrix L = fsl.L;

      // check U is the transpose of L
      Assert.True(U.Equals(L.GetTranspose()));

      // check the lower triangle
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        for (j = 0; j <= i; j++)
        {
          e = System.Math.Abs((A5[i, j] - L[i, j]) / A5[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.True(me < Tolerance5, "Maximum Error = " + me.ToString());

      // check the diagonal
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        e = System.Math.Abs((D5[i] - D[i, i]) / D5[i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.True(me < Tolerance5, "Maximum Error = " + me.ToString());
    }

    #endregion Decomposition Test 5

    #region Decomposition Test 10

    // test the UDL factorisation for case 10
    [Fact]
    public void DecompositionTest10()
    {
      int i, j;
      float e, me;
      var fsl = new FloatSymmetricLevinson(T10);
      FloatMatrix U = fsl.U;
      FloatMatrix D = fsl.D;
      FloatMatrix L = fsl.L;

      // check U is the transpose of L
      Assert.True(U.Equals(L.GetTranspose()));

      // check the lower triangle
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        for (j = 0; j <= i; j++)
        {
          e = System.Math.Abs((A10[i, j] - L[i, j]) / A10[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.True(me < Tolerance10, "Maximum Error = " + me.ToString());

      // check the diagonal
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        e = System.Math.Abs((D10[i] - D[i, i]) / D10[i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.True(me < Tolerance10, "Maximum Error = " + me.ToString());
    }

    #endregion Decomposition Test 10

    #region Positve Definite Property Test 1

    // check the matrix is not positve definite
    [Fact]
    public void PositveDefinitePropertyTest1()
    {
      var fsl = new FloatSymmetricLevinson(T1);
      Assert.True(fsl.IsPositiveDefinite);
    }

    #endregion Positve Definite Property Test 1

    #region Positve Definite Property Test 2

    // check the matrix is not positve definite
    [Fact]
    public void PositveDefinitePropertyTest2()
    {
      var fsl = new FloatSymmetricLevinson(T2);
      Assert.True(fsl.IsPositiveDefinite);
    }

    #endregion Positve Definite Property Test 2

    #region Positve Definite Property Test 3

    // check the matrix is not positve definite
    [Fact]
    public void PositveDefinitePropertyTest3()
    {
      var fsl = new FloatSymmetricLevinson(T3);
      Assert.True(fsl.IsPositiveDefinite);
    }

    #endregion Positve Definite Property Test 3

    #region Positve Definite Property Test 4

    // check the matrix is not positve definite
    [Fact]
    public void PositveDefinitePropertyTest4()
    {
      var fsl = new FloatSymmetricLevinson(T4);
      Assert.True(fsl.IsPositiveDefinite);
    }

    #endregion Positve Definite Property Test 4

    #region Positve Definite Property Test 5

    // check the matrix is not positve definite
    [Fact]
    public void PositveDefinitePropertyTest5()
    {
      var fsl = new FloatSymmetricLevinson(T5);
      Assert.True(fsl.IsPositiveDefinite);
    }

    #endregion Positve Definite Property Test 5

    #region Positve Definite Property Test 10

    // check the matrix is not positve definite
    [Fact]
    public void PositveDefinitePropertyTest10()
    {
      var fsl = new FloatSymmetricLevinson(T10);
      Assert.True(fsl.IsPositiveDefinite);
    }

    #endregion Positve Definite Property Test 10

    #region Positve Definite Property Test - Negative Case

    // check the matrix is not positve definite
    [Fact]
    public void PositveDefinitePropertyTest()
    {
      var fsl = new FloatSymmetricLevinson(1.0f, 2.0f, 3.0f, 4.0f, 5.0f);
      Assert.False(fsl.IsPositiveDefinite);
    }

    #endregion Positve Definite Property Test - Negative Case

    #region Singularity Property Test 1

    // check that singular matrix is detected
    [Fact]
    public void SingularityPropertyTest1()
    {
      var fsl = new FloatSymmetricLevinson(T1);
      Assert.False(fsl.IsSingular);
    }

    #endregion Singularity Property Test 1

    #region Singularity Property Test 2

    // check that singular matrix is detected
    [Fact]
    public void SingularityPropertyTest2()
    {
      var fsl = new FloatSymmetricLevinson(T2);
      Assert.False(fsl.IsSingular);
    }

    #endregion Singularity Property Test 2

    #region Singularity Property Test 3

    // check that singular matrix is detected
    [Fact]
    public void SingularityPropertyTest3()
    {
      var fsl = new FloatSymmetricLevinson(T3);
      Assert.False(fsl.IsSingular);
    }

    #endregion Singularity Property Test 3

    #region Singularity Property Test 4

    // check that singular matrix is detected
    [Fact]
    public void SingularityPropertyTest4()
    {
      var fsl = new FloatSymmetricLevinson(T4);
      Assert.False(fsl.IsSingular);
    }

    #endregion Singularity Property Test 4

    #region Singularity Property Test 5

    // check that singular matrix is detected
    [Fact]
    public void SingularityPropertyTest5()
    {
      var fsl = new FloatSymmetricLevinson(T5);
      Assert.False(fsl.IsSingular);
    }

    #endregion Singularity Property Test 5

    #region Singularity Property Test 10

    // check that singular matrix is detected
    [Fact]
    public void SingularityPropertyTest10()
    {
      var fsl = new FloatSymmetricLevinson(T10);
      Assert.False(fsl.IsSingular);
    }

    #endregion Singularity Property Test 10

    #region Singularity Property Test - Negative Case

    // check that singular matrix is detected
    [Fact]
    public void SingularityPropertyTest()
    {
      var T = new FloatVector(10);
      for (int i = 1; i < 10; i++)
      {
        T[i] = i + 1;
      }
      T[0] = -2.0f;

      var fsl = new FloatSymmetricLevinson(T);
      Assert.True(fsl.IsSingular);
    }

    #endregion Singularity Property Test - Negative Case

    #region GetDeterminant Method Test 1

    // Test the Determinant
    [Fact]
    public void GetDeterminantMethodTest1()
    {
      // calculate determinant from diagonal
      var fsl = new FloatSymmetricLevinson(T1);

      // check results match
      float e = System.Math.Abs((fsl.GetDeterminant() - Det1) / Det1);
      Assert.True(e < Tolerance1);
    }

    #endregion GetDeterminant Method Test 1

    #region GetDeterminant Method Test 2

    // Test the Determinant
    [Fact]
    public void GetDeterminantMethodTest2()
    {
      // calculate determinant from diagonal
      var fsl = new FloatSymmetricLevinson(T2);

      // check results match
      float e = System.Math.Abs((fsl.GetDeterminant() - Det2) / Det2);
      Assert.True(e < Tolerance2);
    }

    #endregion GetDeterminant Method Test 2

    #region GetDeterminant Method Test 3

    // Test the Determinant
    [Fact]
    public void GetDeterminantMethodTest3()
    {
      // calculate determinant from diagonal
      var fsl = new FloatSymmetricLevinson(T3);

      // check results match
      float e = System.Math.Abs((fsl.GetDeterminant() - Det3) / Det3);
      Assert.True(e < Tolerance3);
    }

    #endregion GetDeterminant Method Test 3

    #region GetDeterminant Method Test 4

    // Test the Determinant
    [Fact]
    public void GetDeterminantMethodTest4()
    {
      // calculate determinant from diagonal
      var fsl = new FloatSymmetricLevinson(T4);

      // check results match
      float e = System.Math.Abs((fsl.GetDeterminant() - Det4) / Det4);
      Assert.True(e < Tolerance4);
    }

    #endregion GetDeterminant Method Test 4

    #region GetDeterminant Method Test 5

    // Test the Determinant
    [Fact]
    public void GetDeterminantMethodTest5()
    {
      // calculate determinant from diagonal
      var fsl = new FloatSymmetricLevinson(T5);

      // check results match
      float e = System.Math.Abs((fsl.GetDeterminant() - Det5) / Det5);
      Assert.True(e < Tolerance5);
    }

    #endregion GetDeterminant Method Test 5

    #region GetDeterminant Method Test 10

    // Test the Determinant
    [Fact]
    public void GetDeterminantMethodTest10()
    {
      // calculate determinant from diagonal
      var fsl = new FloatSymmetricLevinson(T10);

      // check results match
      float e = System.Math.Abs((fsl.GetDeterminant() - Det10) / Det10);
      Assert.True(e < Tolerance10);
    }

    #endregion GetDeterminant Method Test 10

    #region Null Parameter Test for SolveVector

    [Fact]
    public void NullParameterTestforSolveVector()
    {
      Assert.Throws<System.ArgumentNullException>(() =>
      {
        var fsl = new FloatSymmetricLevinson(T10);
        FloatVector X = fsl.Solve(null as FloatVector);
      });
    }

    #endregion Null Parameter Test for SolveVector

    #region Mismatch Rows Test for SolveVector

    [Fact]
    public void MismatchRowsTestforSolveVector()
    {
      Assert.Throws<System.RankException>(() =>
      {
        var fsl = new FloatSymmetricLevinson(T10);
        FloatVector X = fsl.Solve(X5);
      });
    }

    #endregion Mismatch Rows Test for SolveVector

    #region SolveVector 1

    // Test solving a linear system
    [Fact]
    public void SolveVector1()
    {
      int i;
      float e, me;
      var fsl = new FloatSymmetricLevinson(T1);
      FloatVector X = fsl.Solve(Y1);

      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        e = System.Math.Abs((X1[i] - X[i]) / X1[i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.True(me < Tolerance1, "Maximum Error = " + me.ToString());
    }

    #endregion SolveVector 1

    #region SolveVector 2

    // Test solving a linear system
    [Fact]
    public void SolveVector2()
    {
      int i;
      float e, me;
      var fsl = new FloatSymmetricLevinson(T2);
      FloatVector X = fsl.Solve(Y2);

      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        e = System.Math.Abs((X2[i] - X[i]) / X2[i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.True(me < Tolerance2, "Maximum Error = " + me.ToString());
    }

    #endregion SolveVector 2

    #region SolveVector 3

    // Test solving a linear system
    [Fact]
    public void SolveVector3()
    {
      int i;
      float e, me;
      var fsl = new FloatSymmetricLevinson(T3);
      FloatVector X = fsl.Solve(Y3);

      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        e = System.Math.Abs((X3[i] - X[i]) / X3[i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.True(me < Tolerance3, "Maximum Error = " + me.ToString());
    }

    #endregion SolveVector 3

    #region SolveVector 4

    // Test solving a linear system
    [Fact]
    public void SolveVector4()
    {
      int i;
      float e, me;
      var fsl = new FloatSymmetricLevinson(T4);
      FloatVector X = fsl.Solve(Y4);

      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        e = System.Math.Abs((X4[i] - X[i]) / X4[i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.True(me < Tolerance4, "Maximum Error = " + me.ToString());
    }

    #endregion SolveVector 4

    #region SolveVector 5

    // Test solving a linear system
    [Fact]
    public void SolveVector5()
    {
      int i;
      float e, me;
      var fsl = new FloatSymmetricLevinson(T5);
      FloatVector X = fsl.Solve(Y5);

      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        e = System.Math.Abs((X5[i] - X[i]) / X5[i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.True(me < Tolerance5, "Maximum Error = " + me.ToString());
    }

    #endregion SolveVector 5

    #region SolveVector 10

    // Test solving a linear system
    [Fact]
    public void SolveVector10()
    {
      int i;
      float e, me;
      var fsl = new FloatSymmetricLevinson(T10);
      FloatVector X = fsl.Solve(Y10);

      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        e = System.Math.Abs((X10[i] - X[i]) / X10[i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.True(me < Tolerance10, "Maximum Error = " + me.ToString());
    }

    #endregion SolveVector 10

    #region Null Parameter Test for SolveMatrix

    [Fact]
    public void NullParameterTestforSolveMatrix()
    {
      Assert.Throws<System.ArgumentNullException>(() =>
      {
        var fsl = new FloatSymmetricLevinson(T10);
        FloatMatrix X = fsl.Solve(null as FloatMatrix);
      });
    }

    #endregion Null Parameter Test for SolveMatrix

    #region Mismatch Rows Test for SolveMatrix

    [Fact]
    public void MismatchRowsTestforSolveMatrix()
    {
      Assert.Throws<System.RankException>(() =>
      {
        var fsl = new FloatSymmetricLevinson(T10);
        FloatMatrix X = fsl.Solve(I5);
      });
    }

    #endregion Mismatch Rows Test for SolveMatrix

    #region Solve Matrix 1

    // calculate inverse by solving linear equations with identity RHS
    [Fact]
    public void SolveMatrix1()
    {
      int i, j;
      float e, me;
      var fsl = new FloatSymmetricLevinson(T1);

      // check inverse
      FloatMatrix I = fsl.Solve(FloatMatrix.CreateIdentity(1));
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        for (j = 0; j < fsl.Order; j++)
        {
          e = System.Math.Abs((I1[i, j] - I[i, j]) / I1[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.True(me < Tolerance1, "Maximum Error = " + me.ToString());
    }

    #endregion Solve Matrix 1

    #region Solve Matrix 2

    // calculate inverse by solving linear equations with identity RHS
    [Fact]
    public void SolveMatrix2()
    {
      int i, j;
      float e, me;
      var fsl = new FloatSymmetricLevinson(T2);

      // check inverse
      FloatMatrix I = fsl.Solve(FloatMatrix.CreateIdentity(2));
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        for (j = 0; j < fsl.Order; j++)
        {
          e = System.Math.Abs((I2[i, j] - I[i, j]) / I2[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.True(me < Tolerance2, "Maximum Error = " + me.ToString());
    }

    #endregion Solve Matrix 2

    #region Solve Matrix 3

    // calculate inverse by solving linear equations with identity RHS
    [Fact]
    public void SolveMatrix3()
    {
      int i, j;
      float e, me;
      var fsl = new FloatSymmetricLevinson(T3);

      // check inverse
      FloatMatrix I = fsl.Solve(FloatMatrix.CreateIdentity(3));
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        for (j = 0; j < fsl.Order; j++)
        {
          e = System.Math.Abs((I3[i, j] - I[i, j]) / I3[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.True(me < Tolerance3, "Maximum Error = " + me.ToString());
    }

    #endregion Solve Matrix 3

    #region Solve Matrix 4

    // calculate inverse by solving linear equations with identity RHS
    [Fact]
    public void SolveMatrix4()
    {
      int i, j;
      float e, me;
      var fsl = new FloatSymmetricLevinson(T4);

      // check inverse
      FloatMatrix I = fsl.Solve(FloatMatrix.CreateIdentity(4));
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        for (j = 0; j < fsl.Order; j++)
        {
          e = System.Math.Abs((I4[i, j] - I[i, j]) / I4[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.True(me < Tolerance4, "Maximum Error = " + me.ToString());
    }

    #endregion Solve Matrix 4

    #region Solve Matrix 5

    // calculate inverse by solving linear equations with identity RHS
    [Fact]
    public void SolveMatrix5()
    {
      int i, j;
      float e, me;
      var fsl = new FloatSymmetricLevinson(T5);

      // check inverse
      FloatMatrix I = fsl.Solve(FloatMatrix.CreateIdentity(5));
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        for (j = 0; j < fsl.Order; j++)
        {
          e = System.Math.Abs((I5[i, j] - I[i, j]) / I5[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.True(me < Tolerance5, "Maximum Error = " + me.ToString());
    }

    #endregion Solve Matrix 5

    #region Solve Matrix 10

    // calculate inverse by solving linear equations with identity RHS
    [Fact]
    public void SolveMatrix10()
    {
      int i, j;
      float e, me;
      var fsl = new FloatSymmetricLevinson(T10);

      // check inverse
      FloatMatrix I = fsl.Solve(FloatMatrix.CreateIdentity(10));
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        for (j = 0; j < fsl.Order; j++)
        {
          e = System.Math.Abs((I10[i, j] - I[i, j]) / I10[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.True(me < Tolerance10, "Maximum Error = " + me.ToString());
    }

    #endregion Solve Matrix 10

    #region Get Inverse 1

    // calculate inverse using GetInverse member
    [Fact]
    public void GetInverse1()
    {
      int i, j;
      float e, me;
      var fsl = new FloatSymmetricLevinson(T1);

      // check inverse
      FloatMatrix I = fsl.GetInverse();
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        for (j = 0; j < fsl.Order; j++)
        {
          e = System.Math.Abs((I1[i, j] - I[i, j]) / I1[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.True(me < Tolerance1, "Maximum Error = " + me.ToString());
    }

    #endregion Get Inverse 1

    #region Get Inverse 2

    // calculate inverse using GetInverse member
    [Fact]
    public void GetInverse2()
    {
      int i, j;
      float e, me;
      var fsl = new FloatSymmetricLevinson(T2);

      // check inverse
      FloatMatrix I = fsl.GetInverse();
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        for (j = 0; j < fsl.Order; j++)
        {
          e = System.Math.Abs((I2[i, j] - I[i, j]) / I2[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.True(me < Tolerance2, "Maximum Error = " + me.ToString());
    }

    #endregion Get Inverse 2

    #region Get Inverse 3

    // calculate inverse using GetInverse member
    [Fact]
    public void GetInverse3()
    {
      int i, j;
      float e, me;
      var fsl = new FloatSymmetricLevinson(T3);

      // check inverse
      FloatMatrix I = fsl.GetInverse();
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        for (j = 0; j < fsl.Order; j++)
        {
          e = System.Math.Abs((I3[i, j] - I[i, j]) / I3[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.True(me < Tolerance3, "Maximum Error = " + me.ToString());
    }

    #endregion Get Inverse 3

    #region Get Inverse 4

    // calculate inverse using GetInverse member
    [Fact]
    public void GetInverse4()
    {
      int i, j;
      float e, me;
      var fsl = new FloatSymmetricLevinson(T4);

      // check inverse
      FloatMatrix I = fsl.GetInverse();
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        for (j = 0; j < fsl.Order; j++)
        {
          e = System.Math.Abs((I4[i, j] - I[i, j]) / I4[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.True(me < Tolerance4, "Maximum Error = " + me.ToString());
    }

    #endregion Get Inverse 4

    #region Get Inverse 5

    // calculate inverse using GetInverse member
    [Fact]
    public void GetInverse5()
    {
      int i, j;
      float e, me;
      var fsl = new FloatSymmetricLevinson(T5);

      // check inverse
      FloatMatrix I = fsl.GetInverse();
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        for (j = 0; j < fsl.Order; j++)
        {
          e = System.Math.Abs((I5[i, j] - I[i, j]) / I5[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.True(me < Tolerance5, "Maximum Error = " + me.ToString());
    }

    #endregion Get Inverse 5

    #region Get Inverse 10

    // calculate inverse using GetInverse member
    [Fact]
    public void GetInverse10()
    {
      int i, j;
      float e, me;
      var fsl = new FloatSymmetricLevinson(T10);

      // check inverse
      FloatMatrix I = fsl.GetInverse();
      me = 0.0f;
      for (i = 0; i < fsl.Order; i++)
      {
        for (j = 0; j < fsl.Order; j++)
        {
          e = System.Math.Abs((I10[i, j] - I[i, j]) / I10[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.True(me < Tolerance10, "Maximum Error = " + me.ToString());
    }

    #endregion Get Inverse 10

    #region Null Parameter Test 1 for Static SolveVector

    // test null parameter
    [Fact]
    public void NullParameterTestforStaticSolveVector1()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        FloatVector X = FloatSymmetricLevinson.Solve(null, new float[] { 1.0f, 1.0f });
      });
    }

    #endregion Null Parameter Test 1 for Static SolveVector

    #region Null Parameter Test for 2 Static SolveVector

    // test null parameter
    [Fact]
    public void NullParameterTestforStaticSolveVector2()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        FloatVector X = FloatSymmetricLevinson.Solve(new float[] { 1.0f, 1.0f }, null as IROVector<float>);
      });
    }

    #endregion Null Parameter Test for 2 Static SolveVector

    #region Row Mismatch Test for Static SolveVector

    // test mismatching dimensions
    [Fact]
    public void RowMismatchTestforStaticSolveVector()
    {
      Assert.Throws<RankException>(() =>
      {
        FloatVector X = FloatSymmetricLevinson.Solve(new float[] { 1.0f, 1.0f, 1.0f }, new float[] { 1.0f, 1.0f });
      });
    }

    #endregion Row Mismatch Test for Static SolveVector

    #region Singular Test for Static SolveVector

    // test with Toeplitz matrix which has a singular principal sub-matrix
    [Fact]
    public void SingularTestforStaticSolveVector()
    {
      Assert.Throws<SingularMatrixException>(() =>
      {
        FloatVector X = FloatSymmetricLevinson.Solve(new float[] { 1.0f, 1.0f, 1.0f }, new float[] { 1.0f, 1.0f, 1.0f });
      });
    }

    #endregion Singular Test for Static SolveVector

    #region Static Solve Vector 1

    [Fact]
    public void StaticSolveVector1()
    {
      int i;
      float e, me;
      FloatVector X = FloatSymmetricLevinson.Solve(T1, Y1);

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
      Assert.True(me < Tolerance1, "Maximum Error = " + me.ToString());
    }

    #endregion Static Solve Vector 1

    #region Static Solve Vector 2

    [Fact]
    public void StaticSolveVector2()
    {
      int i;
      float e, me;
      FloatVector X = FloatSymmetricLevinson.Solve(T2, Y2);

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
      Assert.True(me < Tolerance2, "Maximum Error = " + me.ToString());
    }

    #endregion Static Solve Vector 2

    #region Static Solve Vector 3

    [Fact]
    public void StaticSolveVector3()
    {
      int i;
      float e, me;
      FloatVector X = FloatSymmetricLevinson.Solve(T3, Y3);

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
      Assert.True(me < Tolerance3, "Maximum Error = " + me.ToString());
    }

    #endregion Static Solve Vector 3

    #region Static Solve Vector 4

    [Fact]
    public void StaticSolveVector4()
    {
      int i;
      float e, me;
      FloatVector X = FloatSymmetricLevinson.Solve(T4, Y4);

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
      Assert.True(me < Tolerance4, "Maximum Error = " + me.ToString());
    }

    #endregion Static Solve Vector 4

    #region Static Solve Vector 5

    [Fact]
    public void StaticSolveVector5()
    {
      int i;
      float e, me;
      FloatVector X = FloatSymmetricLevinson.Solve(T5, Y5);

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
      Assert.True(me < Tolerance5, "Maximum Error = " + me.ToString());
    }

    #endregion Static Solve Vector 5

    #region Static Solve Vector 10

    [Fact]
    public void StaticSolveVector10()
    {
      int i;
      float e, me;
      FloatVector X = FloatSymmetricLevinson.Solve(T10, Y10);

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
      Assert.True(me < Tolerance10, "Maximum Error = " + me.ToString());
    }

    #endregion Static Solve Vector 10

    #region Null Parameter Test 1 for Static SolveMatrix

    // test null parameter
    [Fact]
    public void NullParameterTestforStaticSolveMatrix1()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        var Y = new FloatMatrix(2);
        Y[0, 0] = Y[1, 1] = 2.0f;
        Y[0, 1] = Y[1, 0] = 1.0f;
        FloatMatrix X = FloatSymmetricLevinson.Solve(null, Y);
      });
    }

    #endregion Null Parameter Test 1 for Static SolveMatrix

    #region Null Parameter Test 2 for Static SolveMatrix

    // test null parameter
    [Fact]
    public void NullParameterTestforStaticSolveMatrix2()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        FloatMatrix Y = FloatSymmetricLevinson.Solve(new float[] { 1.0f, 1.0f }, null as FloatMatrix);
      });
    }

    #endregion Null Parameter Test 2 for Static SolveMatrix

    #region Row Mismatch Test for Static SolveMatrix

    // test mismatching dimensions
    [Fact]
    public void RowMismatchTestforStaticSolveMatrix()
    {
      Assert.Throws<RankException>(() =>
      {
        var Y = new FloatMatrix(2);
        Y[0, 0] = Y[1, 1] = 2.0f;
        Y[0, 1] = Y[1, 0] = 1.0f;

        FloatMatrix X = FloatSymmetricLevinson.Solve(new float[] { 1.0f, 1.0f, 1.0f }, Y);
      });
    }

    #endregion Row Mismatch Test for Static SolveMatrix

    #region Singular Test for Static SolveMatrix

    // test with Toeplitz matrix which has a singular principal sub-matrix
    [Fact]
    public void SingularTestforStaticSolveMatrix()
    {
      Assert.Throws<SingularMatrixException>(() =>
      {
        FloatMatrix X = FloatSymmetricLevinson.Solve(new float[] { 1.0f, 1.0f, 1.0f }, FloatMatrix.CreateIdentity(3));
      });
    }

    #endregion Singular Test for Static SolveMatrix

    #region Static Solve Matrix 1

    // calculate inverse by solving linear system with identity RHS
    [Fact]
    public void StaticSolveMatrix1()
    {
      int i, j;
      float e, me;

      // calculate inverse by solving the linear system
      FloatMatrix I = FloatSymmetricLevinson.Solve(T1, FloatMatrix.CreateIdentity(1));

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
      Assert.True(me < Tolerance1, "Maximum Error = " + me.ToString());
    }

    #endregion Static Solve Matrix 1

    #region Static Solve Matrix 2

    // calculate inverse by solving linear system with identity RHS
    [Fact]
    public void StaticSolveMatrix2()
    {
      int i, j;
      float e, me;

      // calculate inverse by solving the linear system
      FloatMatrix I = FloatSymmetricLevinson.Solve(T2, FloatMatrix.CreateIdentity(2));

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
      Assert.True(me < Tolerance2, "Maximum Error = " + me.ToString());
    }

    #endregion Static Solve Matrix 2

    #region Static Solve Matrix 3

    // calculate inverse by solving linear system with identity RHS
    [Fact]
    public void StaticSolveMatrix3()
    {
      int i, j;
      float e, me;

      // calculate inverse by solving the linear system
      FloatMatrix I = FloatSymmetricLevinson.Solve(T3, FloatMatrix.CreateIdentity(3));

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
      Assert.True(me < Tolerance3, "Maximum Error = " + me.ToString());
    }

    #endregion Static Solve Matrix 3

    #region Static Solve Matrix 4

    // calculate inverse by solving linear system with identity RHS
    [Fact]
    public void StaticSolveMatrix4()
    {
      int i, j;
      float e, me;

      // calculate inverse by solving the linear system
      FloatMatrix I = FloatSymmetricLevinson.Solve(T4, FloatMatrix.CreateIdentity(4));

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
      Assert.True(me < Tolerance4, "Maximum Error = " + me.ToString());
    }

    #endregion Static Solve Matrix 4

    #region Static Solve Matrix 5

    // calculate inverse by solving linear system with identity RHS
    [Fact]
    public void StaticSolveMatrix5()
    {
      int i, j;
      float e, me;

      // calculate inverse by solving the linear system
      FloatMatrix I = FloatSymmetricLevinson.Solve(T5, FloatMatrix.CreateIdentity(5));

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
      Assert.True(me < Tolerance5, "Maximum Error = " + me.ToString());
    }

    #endregion Static Solve Matrix 5

    #region Static Solve Matrix 10

    // calculate inverse by solving linear system with identity RHS
    [Fact]
    public void StaticSolveMatrix10()
    {
      int i, j;
      float e, me;

      // calculate inverse by solving the linear system
      FloatMatrix I = FloatSymmetricLevinson.Solve(T10, FloatMatrix.CreateIdentity(10));

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
      Assert.True(me < Tolerance10, "Maximum Error = " + me.ToString());
    }

    #endregion Static Solve Matrix 10

    #region Null Parameter Test for Static YuleWalker

    // test Yule-Walker with a null reference
    [Fact]
    public void NullParameterTestforStaticYuleWalker()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        FloatVector Y = FloatSymmetricLevinson.YuleWalker(null);
      });
    }

    #endregion Null Parameter Test for Static YuleWalker

    #region Row Test for Static YuleWalker

    // test Yule-Walker with order 1 matrix
    [Fact]
    public void RowTestforStaticYuleWalker()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() =>
      {
        FloatVector Y = FloatSymmetricLevinson.YuleWalker(new float[] { 1.0f });
      });
    }

    #endregion Row Test for Static YuleWalker

    #region Singular Test for Static YuleWalker

    // test Yule-Walker with matrix with singular principal sub-matrix
    [Fact]
    public void SingularTestforStaticYuleWalker()
    {
      Assert.Throws<SingularMatrixException>(() =>
      {
        FloatVector Y = FloatSymmetricLevinson.YuleWalker(new float[] { 1.0f, 1.0f, 1.0f });
      });
    }

    #endregion Singular Test for Static YuleWalker

    #region Static Yule Walker 2

    [Fact]
    public void StaticYuleWalker2()
    {
      int i;
      float e, me;
      int N = T2.Length;
      FloatVector A = FloatSymmetricLevinson.YuleWalker(T2);

      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < A.Length; i++)
      {
        e = System.Math.Abs((A2[N - 1, N - 2 - i] - A[i]) / A2[N - 1, N - 2 - i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.True(me < Tolerance2, "Maximum Error = " + me.ToString());
    }

    #endregion Static Yule Walker 2

    #region Static Yule Walker 3

    [Fact]
    public void StaticYuleWalker3()
    {
      int i;
      float e, me;
      int N = T3.Length;
      FloatVector A = FloatSymmetricLevinson.YuleWalker(T3);

      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < A.Length; i++)
      {
        e = System.Math.Abs((A3[N - 1, N - 2 - i] - A[i]) / A3[N - 1, N - 2 - i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.True(me < Tolerance3, "Maximum Error = " + me.ToString());
    }

    #endregion Static Yule Walker 3

    #region Static Yule Walker 4

    [Fact]
    public void StaticYuleWalker4()
    {
      int i;
      float e, me;
      int N = T4.Length;
      FloatVector A = FloatSymmetricLevinson.YuleWalker(T4);

      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < A.Length; i++)
      {
        e = System.Math.Abs((A4[N - 1, N - 2 - i] - A[i]) / A4[N - 1, N - 2 - i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.True(me < Tolerance4, "Maximum Error = " + me.ToString());
    }

    #endregion Static Yule Walker 4

    #region Static Yule Walker 5

    [Fact]
    public void StaticYuleWalker5()
    {
      int i;
      float e, me;
      int N = T5.Length;
      FloatVector A = FloatSymmetricLevinson.YuleWalker(T5);

      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < A.Length; i++)
      {
        e = System.Math.Abs((A5[N - 1, N - 2 - i] - A[i]) / A5[N - 1, N - 2 - i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.True(me < Tolerance5, "Maximum Error = " + me.ToString());
    }

    #endregion Static Yule Walker 5

    #region Static Yule Walker 10

    [Fact]
    public void StaticYuleWalker10()
    {
      int i;
      float e, me;
      int N = T10.Length;
      FloatVector A = FloatSymmetricLevinson.YuleWalker(T10);

      // determine the maximum error
      me = 0.0f;
      for (i = 0; i < A.Length; i++)
      {
        e = System.Math.Abs((A10[N - 1, N - 2 - i] - A[i]) / A10[N - 1, N - 2 - i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.True(me < Tolerance10, "Maximum Error = " + me.ToString());
    }

    #endregion Static Yule Walker 10

    #region Null Prameter Test for Static Inverse

    [Fact]
    public void NullPrameterTestforStaticInverse()
    {
      Assert.Throws<System.ArgumentNullException>(() =>
      {
        FloatMatrix Y = FloatSymmetricLevinson.Inverse(null);
      });
    }

    #endregion Null Prameter Test for Static Inverse

    #region Singular Test for Static Inverse

    [Fact]
    public void SingularTestforStaticInverse()
    {
      Assert.Throws<SingularMatrixException>(() =>
      {
        // setup an ill-conditioned system (second order principal submatrix is singular)
        var T = new FloatVector(3)
        {
          [0] = 1.0f,
          [1] = 1.0f,
          [2] = 1.0f
        };

        FloatMatrix Y = FloatSymmetricLevinson.Inverse(T);
      });
    }

    #endregion Singular Test for Static Inverse

    #region Static Inverse 1

    [Fact]
    public void StaticInverse1()
    {
      int i, j;
      float e, me;

      // calculate the inverse
      FloatMatrix I = FloatSymmetricLevinson.Inverse(T1);

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
      Assert.True(me < Tolerance1, "Maximum Error = " + me.ToString());
    }

    #endregion Static Inverse 1

    #region Static Inverse 2

    [Fact]
    public void StaticInverse2()
    {
      int i, j;
      float e, me;

      // calculate the inverse
      FloatMatrix I = FloatSymmetricLevinson.Inverse(T2);

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
      Assert.True(me < Tolerance2, "Maximum Error = " + me.ToString());
    }

    #endregion Static Inverse 2

    #region Static Inverse 3

    [Fact]
    public void StaticInverse3()
    {
      int i, j;
      float e, me;

      // calculate the inverse
      FloatMatrix I = FloatSymmetricLevinson.Inverse(T3);

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
      Assert.True(me < Tolerance3, "Maximum Error = " + me.ToString());
    }

    #endregion Static Inverse 3

    #region Static Inverse 4

    [Fact]
    public void StaticInverse4()
    {
      int i, j;
      float e, me;

      // calculate the inverse
      FloatMatrix I = FloatSymmetricLevinson.Inverse(T4);

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
      Assert.True(me < Tolerance4, "Maximum Error = " + me.ToString());
    }

    #endregion Static Inverse 4

    #region Static Inverse 5

    [Fact]
    public void StaticInverse5()
    {
      int i, j;
      float e, me;

      // calculate the inverse
      FloatMatrix I = FloatSymmetricLevinson.Inverse(T5);

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
      Assert.True(me < Tolerance5, "Maximum Error = " + me.ToString());
    }

    #endregion Static Inverse 5

    #region Static Inverse 10

    [Fact]
    public void StaticInverse10()
    {
      int i, j;
      float e, me;

      // calculate the inverse
      FloatMatrix I = FloatSymmetricLevinson.Inverse(T10);

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
      Assert.True(me < Tolerance10, "Maximum Error = " + me.ToString());
    }

    #endregion Static Inverse 10
  }
}

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
using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;
using NUnit.Framework;

#endregion Using Directive

namespace AltaxoTest.Calc.LinearAlgebra
{
  // suite of tests for double symmetric Levinson algorithm
  [TestFixture]
  public class DoubleSymmetricLevinsonTest
  {
    #region Fields

    // unit testing - order 1

    private DoubleVector T1;      // Toeplitz matrix
    private DoubleMatrix A1;      // Lower triangle matrix
    private DoubleVector D1;      // diagonal vector
    private DoubleMatrix I1;      // inverse matrix
    private double Det1;        // exact determinant
    private DoubleVector X1;      // RHS vector
    private DoubleVector Y1;      // LHS vector
    private double Tolerance1;      // allowable tolerance

    // unit testing - order 2

    private DoubleVector T2;      // Toeplitz matrix
    private DoubleMatrix A2;      // Lower triangle matrix
    private DoubleVector D2;      // diagonal vector
    private DoubleMatrix I2;      // inverse matrix
    private double Det2;        // exact determinant
    private DoubleVector X2;      // RHS vector
    private DoubleVector Y2;      // LHS vector
    private double Tolerance2;      // allowable tolerance

    // unit testing - order 3

    private DoubleVector T3;      // Toeplitz matrix
    private DoubleMatrix A3;      // Lower triangle matrix
    private DoubleVector D3;      // diagonal vector
    private DoubleMatrix I3;      // inverse matrix
    private double Det3;        // exact determinant
    private DoubleVector X3;      // RHS vector
    private DoubleVector Y3;      // LHS vector
    private double Tolerance3;      // allowable tolerance

    // unit testing - order 4

    private DoubleVector T4;      // Toeplitz matrix
    private DoubleMatrix A4;      // Lower triangle matrix
    private DoubleVector D4;      // diagonal vector
    private DoubleMatrix I4;      // inverse matrix
    private double Det4;        // exact determinant
    private DoubleVector X4;      // RHS vector
    private DoubleVector Y4;      // LHS vector
    private double Tolerance4;      // allowable tolerance

    // unit testing - order 5

    private DoubleVector T5;      // Toeplitz matrix
    private DoubleMatrix A5;      // Lower triangle matrix
    private DoubleVector D5;      // diagonal vector
    private DoubleMatrix I5;      // inverse matrix
    private double Det5;        // exact determinant
    private DoubleVector X5;      // RHS vector
    private DoubleVector Y5;      // LHS vector
    private double Tolerance5;      // allowable tolerance

    // unit testing - order 10

    private DoubleVector T10;     // Toeplitz matrix
    private DoubleMatrix A10;     // Lower triangle matrix
    private DoubleVector D10;     // diagonal vector
    private DoubleMatrix I10;     // inverse matrix
    private double Det10;       // exact determinant
    private DoubleVector X10;     // RHS vector
    private DoubleVector Y10;     // LHS vector
    private double Tolerance10;     // allowable tolerance

    #endregion Fields

    #region Test Fixture Setup

    [OneTimeSetUp]
    public void SetupTestCases()
    {
      // unit testing values - order 1

      T1 = new DoubleVector(1)
      {
        [0] = +3.0000000000000000E+000
      };

      A1 = new DoubleMatrix(1)
      {
        [0, 0] = +1.0000000000000000E+000
      };

      D1 = new DoubleVector(1)
      {
        [0] = +3.3333333333333331E-001
      };

      Det1 = +3.0000000000000000E+000;

      I1 = new DoubleMatrix(1)
      {
        [0, 0] = +3.3333333333333331E-001
      };

      X1 = new DoubleVector(1)
      {
        [0] = +1.0000000000000000E+000
      };

      Y1 = new DoubleVector(1)
      {
        [0] = +3.0000000000000000E+000
      };

      Tolerance1 = 1.0E-015;

      // unit testing values - order 2

      T2 = new DoubleVector(2)
      {
        [0] = +1.0000000000000000E+001,
        [1] = +4.5000000000000000E+000
      };

      A2 = new DoubleMatrix(2)
      {
        [0, 0] = +1.0000000000000000E+000,
        [1, 0] = -4.5000000000000001E-001,
        [1, 1] = +1.0000000000000000E+000
      };

      D2 = new DoubleVector(2)
      {
        [0] = +1.0000000000000001E-001,
        [1] = +1.2539184952978055E-001
      };

      Det2 = +7.9750000000000000E+001;

      I2 = new DoubleMatrix(2)
      {
        [0, 0] = +1.2539184952978055E-001,
        [0, 1] = -5.6426332288401257E-002,
        [1, 0] = -5.6426332288401257E-002,
        [1, 1] = +1.2539184952978055E-001
      };

      X2 = new DoubleVector(2)
      {
        [0] = +1.0000000000000000E+000,
        [1] = +2.0000000000000000E+000
      };

      Y2 = new DoubleVector(2)
      {
        [0] = +1.9000000000000000E+001,
        [1] = +2.4500000000000000E+001
      };

      Tolerance2 = 1.0E-015;

      // unit testing values - order 3

      T3 = new DoubleVector(3)
      {
        [0] = +4.0000000000000000E+000,
        [1] = +1.5000000000000000E+000,
        [2] = +6.6666666666666663E-001
      };

      A3 = new DoubleMatrix(3)
      {
        [0, 0] = +1.0000000000000000E+000,
        [1, 0] = -3.7500000000000000E-001,
        [1, 1] = +1.0000000000000000E+000,
        [2, 0] = -3.0303030303030304E-002,
        [2, 1] = -3.6363636363636365E-001,
        [2, 2] = +1.0000000000000000E+000
      };

      D3 = new DoubleVector(3)
      {
        [0] = +2.5000000000000000E-001,
        [1] = +2.9090909090909089E-001,
        [2] = +2.9117647058823531E-001
      };

      Det3 = +4.7222222222222221E+001;

      I3 = new DoubleMatrix(3)
      {
        [0, 0] = +2.9117647058823531E-001,
        [0, 1] = -1.0588235294117647E-001,
        [0, 2] = -8.8235294117647058E-003,
        [1, 0] = -1.0588235294117647E-001,
        [1, 1] = +3.2941176470588235E-001,
        [1, 2] = -1.0588235294117647E-001,
        [2, 0] = -8.8235294117647058E-003,
        [2, 1] = -1.0588235294117647E-001,
        [2, 2] = +2.9117647058823531E-001
      };

      X3 = new DoubleVector(3)
      {
        [0] = +1.0000000000000000E+000,
        [1] = +2.0000000000000000E+000,
        [2] = +3.0000000000000000E+000
      };

      Y3 = new DoubleVector(3)
      {
        [0] = +9.0000000000000000E+000,
        [1] = +1.4000000000000000E+001,
        [2] = +1.5666666666666666E+001
      };

      Tolerance3 = 3.0E-015;

      // unit testing values - order 4

      T4 = new DoubleVector(4)
      {
        [0] = +4.0000000000000000E+000,
        [1] = +1.5000000000000000E+000,
        [2] = +6.6666666666666663E-001,
        [3] = +2.5000000000000000E-001
      };

      A4 = new DoubleMatrix(4)
      {
        [0, 0] = +1.0000000000000000E+000,
        [1, 0] = -3.7500000000000000E-001,
        [1, 1] = +1.0000000000000000E+000,
        [2, 0] = -3.0303030303030304E-002,
        [2, 1] = -3.6363636363636365E-001,
        [2, 2] = +1.0000000000000000E+000,
        [3, 0] = +1.1029411764705883E-002,
        [3, 1] = -3.4313725490196081E-002,
        [3, 2] = -3.6397058823529410E-001,
        [3, 3] = +1.0000000000000000E+000
      };

      D4 = new DoubleVector(4)
      {
        [0] = +2.5000000000000000E-001,
        [1] = +2.9090909090909089E-001,
        [2] = +2.9117647058823531E-001,
        [3] = +2.9121189591078067E-001
      };

      Det4 = +1.6215760030864197E+002;

      I4 = new DoubleMatrix(4)
      {
        [0, 0] = +2.9121189591078067E-001,
        [0, 1] = -1.0599256505576209E-001,
        [0, 2] = -9.9925650557620826E-003,
        [0, 3] = +3.2118959107806690E-003,
        [1, 0] = -1.0599256505576209E-001,
        [1, 1] = +3.2975464684014871E-001,
        [1, 2] = -1.0224535315985130E-001,
        [1, 3] = -9.9925650557620826E-003,
        [2, 0] = -9.9925650557620826E-003,
        [2, 1] = -1.0224535315985130E-001,
        [2, 2] = +3.2975464684014871E-001,
        [2, 3] = -1.0599256505576209E-001,
        [3, 0] = +3.2118959107806690E-003,
        [3, 1] = -9.9925650557620826E-003,
        [3, 2] = -1.0599256505576209E-001,
        [3, 3] = +2.9121189591078067E-001
      };

      X4 = new DoubleVector(4)
      {
        [0] = +1.0000000000000000E+000,
        [1] = +2.0000000000000000E+000,
        [2] = +3.0000000000000000E+000,
        [3] = +4.0000000000000000E+000
      };

      Y4 = new DoubleVector(4)
      {
        [0] = +1.0000000000000000E+001,
        [1] = +1.6666666666666668E+001,
        [2] = +2.1666666666666668E+001,
        [3] = +2.2083333333333332E+001
      };

      Tolerance4 = 4.0E-015;

      // unit testing values - order 5

      T5 = new DoubleVector(5)
      {
        [0] = +1.0000000000000000E+000,
        [1] = +5.0000000000000000E-001,
        [2] = +3.3333333333333331E-001,
        [3] = +2.5000000000000000E-001,
        [4] = +2.0000000000000001E-001
      };

      A5 = new DoubleMatrix(5)
      {
        [0, 0] = +1.0000000000000000E+000,
        [1, 0] = -5.0000000000000000E-001,
        [1, 1] = +1.0000000000000000E+000,
        [2, 0] = -1.1111111111111110E-001,
        [2, 1] = -4.4444444444444442E-001,
        [2, 2] = +1.0000000000000000E+000,
        [3, 0] = -6.2500000000000000E-002,
        [3, 1] = -8.3333333333333329E-002,
        [3, 2] = -4.3750000000000000E-001,
        [3, 3] = +1.0000000000000000E+000,
        [4, 0] = -4.2823529411764705E-002,
        [4, 1] = -4.3764705882352942E-002,
        [4, 2] = -7.9764705882352946E-002,
        [4, 3] = -4.3482352941176472E-001,
        [4, 4] = +1.0000000000000000E+000
      };

      D5 = new DoubleVector(5)
      {
        [0] = +1.0000000000000000E+000,
        [1] = +1.3333333333333333E+000,
        [2] = +1.3500000000000001E+000,
        [3] = +1.3552941176470588E+000,
        [4] = +1.3577840963547489E+000
      };

      Det5 = +3.0190007716049383E-001;

      I5 = new DoubleMatrix(5)
      {
        [0, 0] = +1.3577840963547489E+000,
        [0, 1] = -5.9039647295613562E-001,
        [0, 2] = -1.0830324909747292E-001,
        [0, 3] = -5.9423021628701958E-002,
        [0, 4] = -5.8145107185073958E-002,
        [1, 0] = -5.9039647295613562E-001,
        [1, 1] = +1.6120123957701031E+000,
        [1, 2] = -5.4584837545126352E-001,
        [1, 3] = -8.7102648477684425E-002,
        [1, 4] = -5.9423021628701958E-002,
        [2, 0] = -1.0830324909747292E-001,
        [2, 1] = -5.4584837545126352E-001,
        [2, 2] = +1.6180505415162454E+000,
        [2, 3] = -5.4584837545126352E-001,
        [2, 4] = -1.0830324909747292E-001,
        [3, 0] = -5.9423021628701958E-002,
        [3, 1] = -8.7102648477684425E-002,
        [3, 2] = -5.4584837545126352E-001,
        [3, 3] = +1.6120123957701031E+000,
        [3, 4] = -5.9039647295613562E-001,
        [4, 0] = -5.8145107185073958E-002,
        [4, 1] = -5.9423021628701958E-002,
        [4, 2] = -1.0830324909747292E-001,
        [4, 3] = -5.9039647295613562E-001,
        [4, 4] = +1.3577840963547489E+000
      };

      X5 = new DoubleVector(5)
      {
        [0] = +1.0000000000000000E+000,
        [1] = +2.0000000000000000E+000,
        [2] = +3.0000000000000000E+000,
        [3] = +4.0000000000000000E+000,
        [4] = +5.0000000000000000E+000
      };

      Y5 = new DoubleVector(5)
      {
        [0] = +5.0000000000000000E+000,
        [1] = +6.5833333333333330E+000,
        [2] = +8.0000000000000000E+000,
        [3] = +8.9166666666666661E+000,
        [4] = +8.6999999999999993E+000
      };

      Tolerance5 = 5.0E-015;

      // unit testing values - order 10

      T10 = new DoubleVector(10)
      {
        [0] = +1.0000000000000000E+001,
        [1] = +5.0000000000000000E+000,
        [2] = +3.3333333333333335E+000,
        [3] = +2.5000000000000000E+000,
        [4] = +2.0000000000000000E+000,
        [5] = +1.6666666666666667E+000,
        [6] = +1.4285714285714286E+000,
        [7] = +1.2500000000000000E+000,
        [8] = +1.1111111111111112E+000,
        [9] = +1.0000000000000000E+000
      };

      A10 = new DoubleMatrix(10)
      {
        [0, 0] = +1.0000000000000000E+000,
        [1, 0] = -5.0000000000000000E-001,
        [1, 1] = +1.0000000000000000E+000,
        [2, 0] = -1.1111111111111110E-001,
        [2, 1] = -4.4444444444444442E-001,
        [2, 2] = +1.0000000000000000E+000,
        [3, 0] = -6.2500000000000000E-002,
        [3, 1] = -8.3333333333333329E-002,
        [3, 2] = -4.3750000000000000E-001,
        [3, 3] = +1.0000000000000000E+000,
        [4, 0] = -4.2823529411764705E-002,
        [4, 1] = -4.3764705882352942E-002,
        [4, 2] = -7.9764705882352946E-002,
        [4, 3] = -4.3482352941176472E-001,
        [4, 4] = +1.0000000000000000E+000,
        [5, 0] = -3.2262015058091863E-002,
        [5, 1] = -2.8795246158269703E-002,
        [5, 2] = -4.1191335740072200E-002,
        [5, 3] = -7.8352768282163504E-002,
        [5, 4] = -4.3344195606104169E-001,
        [5, 5] = +1.0000000000000000E+000,
        [6, 0] = -2.5714690745207582E-002,
        [6, 1] = -2.1116189201984319E-002,
        [6, 2] = -2.6780428952862961E-002,
        [6, 3] = -4.0132113280134232E-002,
        [6, 4] = -7.7612307432271474E-002,
        [6, 5] = -4.3261234832100565E-001,
        [6, 6] = +1.0000000000000000E+000,
        [7, 0] = -2.1279070818048949E-002,
        [7, 1] = -1.6509101948522443E-002,
        [7, 2] = -1.9464671415780829E-002,
        [7, 3] = -2.5926454872297021E-002,
        [7, 4] = -3.9562250635908527E-002,
        [7, 5] = -7.7162974546835128E-002,
        [7, 6] = -4.3206516359557418E-001,
        [7, 7] = +1.0000000000000000E+000,
        [8, 0] = -1.8086542121485090E-002,
        [8, 1] = -1.3464506037451252E-002,
        [8, 2] = -1.5113490559162029E-002,
        [8, 3] = -1.8749127103233718E-002,
        [8, 4] = -2.5457534954188437E-002,
        [8, 5] = -3.9210202036466142E-002,
        [8, 6] = -7.6864381979055291E-002,
        [8, 7] = -4.3168029878491743E-001,
        [8, 8] = +1.0000000000000000E+000,
        [9, 0] = -1.5685263462054830E-002,
        [9, 1] = -1.1315522903665111E-002,
        [9, 2] = -1.2258867955261750E-002,
        [9, 3] = -1.4498468209819659E-002,
        [9, 4] = -1.8349818960382801E-002,
        [9, 5] = -2.5163449955890666E-002,
        [9, 6] = -3.8973142955214410E-002,
        [9, 7] = -7.6653187654471433E-002,
        [9, 8] = -4.3139660660662438E-001,
        [9, 9] = +1.0000000000000000E+000
      };

      D10 = new DoubleVector(10)
      {
        [0] = +1.0000000000000001E-001,
        [1] = +1.3333333333333333E-001,
        [2] = +1.3500000000000001E-001,
        [3] = +1.3552941176470587E-001,
        [4] = +1.3577840963547491E-001,
        [5] = +1.3591988015945386E-001,
        [6] = +1.3600981601378359E-001,
        [7] = +1.3607142900101707E-001,
        [8] = +1.3611595566158571E-001,
        [9] = +1.3614945216955071E-001
      };

      Det10 = +6.4761680151808619E+008;

      I10 = new DoubleMatrix(10)
      {
        [0, 0] = +1.3614945216955071E-001,
        [0, 1] = -5.8734411657295094E-002,
        [0, 2] = -1.0436289506206054E-002,
        [0, 3] = -5.3061720626780265E-003,
        [0, 4] = -3.4259899261904190E-003,
        [0, 5] = -2.4983177988665531E-003,
        [0, 6] = -1.9739585040645931E-003,
        [0, 7] = -1.6690381563277477E-003,
        [0, 8] = -1.5406022443460088E-003,
        [0, 9] = -2.1355400274938358E-003,
        [1, 0] = -5.8734411657295094E-002,
        [1, 1] = +1.6145378154157938E-001,
        [1, 2] = -5.4256396530846282E-002,
        [1, 3] = -8.1734041875061048E-003,
        [1, 4] = -3.8591736934504169E-003,
        [1, 5] = -2.3874108784216197E-003,
        [1, 6] = -1.7004963532414416E-003,
        [1, 7] = -1.3371698139059260E-003,
        [1, 8] = -1.1681235264574500E-003,
        [1, 9] = -1.5406022443460088E-003,
        [2, 0] = -1.0436289506206054E-002,
        [2, 1] = -5.4256396530846282E-002,
        [2, 2] = +1.6223632367953364E-001,
        [2, 3] = -5.3868547567483924E-002,
        [2, 4] = -7.9331275114551281E-003,
        [2, 5] = -3.6959394426671018E-003,
        [2, 6] = -2.2748675342649503E-003,
        [2, 7] = -1.6326013697479989E-003,
        [2, 8] = -1.3371698139059260E-003,
        [2, 9] = -1.6690381563277477E-003,
        [3, 0] = -5.3061720626780265E-003,
        [3, 1] = -8.1734041875061048E-003,
        [3, 2] = -5.3868547567483924E-002,
        [3, 3] = +1.6242266136350664E-001,
        [3, 4] = -5.3759224468977879E-002,
        [3, 5] = -7.8663867627389306E-003,
        [3, 6] = -3.6610068338217565E-003,
        [3, 7] = -2.2748675342649503E-003,
        [3, 8] = -1.7004963532414416E-003,
        [3, 9] = -1.9739585040645931E-003,
        [4, 0] = -3.4259899261904190E-003,
        [4, 1] = -3.8591736934504169E-003,
        [4, 2] = -7.9331275114551281E-003,
        [4, 3] = -5.3759224468977879E-002,
        [4, 4] = +1.6248025171494504E-001,
        [4, 5] = -5.3732579955257077E-002,
        [4, 6] = -7.8663867627389306E-003,
        [4, 7] = -3.6959394426671018E-003,
        [4, 8] = -2.3874108784216197E-003,
        [4, 9] = -2.4983177988665531E-003,
        [5, 0] = -2.4983177988665531E-003,
        [5, 1] = -2.3874108784216197E-003,
        [5, 2] = -3.6959394426671018E-003,
        [5, 3] = -7.8663867627389306E-003,
        [5, 4] = -5.3732579955257077E-002,
        [5, 5] = +1.6248025171494504E-001,
        [5, 6] = -5.3759224468977879E-002,
        [5, 7] = -7.9331275114551281E-003,
        [5, 8] = -3.8591736934504169E-003,
        [5, 9] = -3.4259899261904190E-003,
        [6, 0] = -1.9739585040645931E-003,
        [6, 1] = -1.7004963532414416E-003,
        [6, 2] = -2.2748675342649503E-003,
        [6, 3] = -3.6610068338217565E-003,
        [6, 4] = -7.8663867627389306E-003,
        [6, 5] = -5.3759224468977879E-002,
        [6, 6] = +1.6242266136350664E-001,
        [6, 7] = -5.3868547567483924E-002,
        [6, 8] = -8.1734041875061048E-003,
        [6, 9] = -5.3061720626780265E-003,
        [7, 0] = -1.6690381563277477E-003,
        [7, 1] = -1.3371698139059260E-003,
        [7, 2] = -1.6326013697479989E-003,
        [7, 3] = -2.2748675342649503E-003,
        [7, 4] = -3.6959394426671018E-003,
        [7, 5] = -7.9331275114551281E-003,
        [7, 6] = -5.3868547567483924E-002,
        [7, 7] = +1.6223632367953364E-001,
        [7, 8] = -5.4256396530846282E-002,
        [7, 9] = -1.0436289506206054E-002,
        [8, 0] = -1.5406022443460088E-003,
        [8, 1] = -1.1681235264574500E-003,
        [8, 2] = -1.3371698139059260E-003,
        [8, 3] = -1.7004963532414416E-003,
        [8, 4] = -2.3874108784216197E-003,
        [8, 5] = -3.8591736934504169E-003,
        [8, 6] = -8.1734041875061048E-003,
        [8, 7] = -5.4256396530846282E-002,
        [8, 8] = +1.6145378154157938E-001,
        [8, 9] = -5.8734411657295094E-002,
        [9, 0] = -2.1355400274938358E-003,
        [9, 1] = -1.5406022443460088E-003,
        [9, 2] = -1.6690381563277477E-003,
        [9, 3] = -1.9739585040645931E-003,
        [9, 4] = -2.4983177988665531E-003,
        [9, 5] = -3.4259899261904190E-003,
        [9, 6] = -5.3061720626780265E-003,
        [9, 7] = -1.0436289506206054E-002,
        [9, 8] = -5.8734411657295094E-002,
        [9, 9] = +1.3614945216955071E-001
      };

      X10 = new DoubleVector(10)
      {
        [0] = +1.0000000000000000E+000,
        [1] = +2.0000000000000000E+000,
        [2] = +3.0000000000000000E+000,
        [3] = +4.0000000000000000E+000,
        [4] = +5.0000000000000000E+000,
        [5] = +6.0000000000000000E+000,
        [6] = +7.0000000000000000E+000,
        [7] = +8.0000000000000000E+000,
        [8] = +9.0000000000000000E+000,
        [9] = +1.0000000000000000E+001
      };

      Y10 = new DoubleVector(10)
      {
        [0] = +1.0000000000000000E+002,
        [1] = +1.2328968253968254E+002,
        [2] = +1.4769047619047620E+002,
        [3] = +1.7195238095238096E+002,
        [4] = +1.9500000000000000E+002,
        [5] = +2.1566666666666666E+002,
        [6] = +2.3242857142857142E+002,
        [7] = +2.4294047619047620E+002,
        [8] = +2.4289682539682539E+002,
        [9] = +2.2218650793650792E+002
      };

      Tolerance10 = 2.0E-014;
    }

    #endregion Test Fixture Setup

    #region Null Parameter Tests for Constructor

    // Test constructor with a null parameter
    [Test]
    public void NullParameterTestforConstructor1()
    {
      Assert.Throws(typeof(System.ArgumentNullException), () =>
      {
        var dsl = new DoubleSymmetricLevinson(null as DoubleVector);
      });
    }

    // Test constructor with a null parameter
    [Test]
    public void NullParameterTestforConstructor2()
    {
      Assert.Throws(typeof(System.ArgumentNullException), () =>
      {
        var dsl = new DoubleSymmetricLevinson(null as double[]);
      });
    }

    #endregion Null Parameter Tests for Constructor

    #region Zero Length Vector Tests for Constructor

    // Test constructor with a zero length vector parameter
    [Test]
    public void ZeroLengthVectorTestsforConstructor1()
    {
      Assert.Throws(typeof(System.RankException), () =>
      {
        var dv = new DoubleVector();

        var dsl = new DoubleSymmetricLevinson(dv);
      });
    }

    [Test]
    public void ZeroLengthVectorTestsforConstructor2()
    {
      Assert.Throws(typeof(System.RankException), () =>
      {
        double[] dv = new double[0];
        var dsl = new DoubleSymmetricLevinson(dv);
      });
    }

    #endregion Zero Length Vector Tests for Constructor

    #region GetVector Member Test

    // check get vector
    [Test]
    public void GetVectorMemberTest()
    {
      var dsl = new DoubleSymmetricLevinson(T5);
      DoubleVector TT = dsl.GetVector();
      Assert.IsTrue(T5.Equals(TT));
    }

    #endregion GetVector Member Test

    #region GetMatrix Member Test

    // check get matrix
    [Test]
    public void GetMatrixMemberTest()
    {
      var dsl = new DoubleSymmetricLevinson(T5);
      DoubleMatrix dsldm = dsl.GetMatrix();
      for (int row = 0; row < T5.Length; row++)
      {
        for (int column = 0; column < T5.Length; column++)
        {
          if (column < row)
          {
            Assert.IsTrue(dsldm[row, column] == T5[row - column]);
          }
          else
          {
            Assert.IsTrue(dsldm[row, column] == T5[column - row]);
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
      var dsl = new DoubleSymmetricLevinson(T5);
      Assert.IsTrue(dsl.Order == 5);
    }

    #endregion Order Property Test

    #region Decomposition Test 1

    // test the UDL factorisation for case 1
    [Test]
    public void DecompositionTest1()
    {
      int i, j;
      double e, me;
      var dsl = new DoubleSymmetricLevinson(T1);
      DoubleMatrix U = dsl.U;
      DoubleMatrix D = dsl.D;
      DoubleMatrix L = dsl.L;

      // check U is the transpose of L
      Assert.IsTrue(U.Equals(L.GetTranspose()));

      // check the lower triangle
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
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
      Assert.IsTrue(me < Tolerance1, "Maximum Error = " + me.ToString());

      // check the diagonal
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
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
      var dsl = new DoubleSymmetricLevinson(T2);
      DoubleMatrix U = dsl.U;
      DoubleMatrix D = dsl.D;
      DoubleMatrix L = dsl.L;

      // check U is the transpose of L
      Assert.IsTrue(U.Equals(L.GetTranspose()));

      // check the lower triangle
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
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
      Assert.IsTrue(me < Tolerance2, "Maximum Error = " + me.ToString());

      // check the diagonal
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
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
      var dsl = new DoubleSymmetricLevinson(T3);
      DoubleMatrix U = dsl.U;
      DoubleMatrix D = dsl.D;
      DoubleMatrix L = dsl.L;

      // check U is the transpose of L
      Assert.IsTrue(U.Equals(L.GetTranspose()));

      // check the lower triangle
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
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
      Assert.IsTrue(me < Tolerance3, "Maximum Error = " + me.ToString());

      // check the diagonal
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
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
      var dsl = new DoubleSymmetricLevinson(T4);
      DoubleMatrix U = dsl.U;
      DoubleMatrix D = dsl.D;
      DoubleMatrix L = dsl.L;

      // check U is the transpose of L
      Assert.IsTrue(U.Equals(L.GetTranspose()));

      // check the lower triangle
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
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
      Assert.IsTrue(me < Tolerance4, "Maximum Error = " + me.ToString());

      // check the diagonal
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
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
      var dsl = new DoubleSymmetricLevinson(T5);
      DoubleMatrix U = dsl.U;
      DoubleMatrix D = dsl.D;
      DoubleMatrix L = dsl.L;

      // check U is the transpose of L
      Assert.IsTrue(U.Equals(L.GetTranspose()));

      // check the lower triangle
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
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
      Assert.IsTrue(me < Tolerance5, "Maximum Error = " + me.ToString());

      // check the diagonal
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
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
      var dsl = new DoubleSymmetricLevinson(T10);
      DoubleMatrix U = dsl.U;
      DoubleMatrix D = dsl.D;
      DoubleMatrix L = dsl.L;

      // check U is the transpose of L
      Assert.IsTrue(U.Equals(L.GetTranspose()));

      // check the lower triangle
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
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
      Assert.IsTrue(me < Tolerance10, "Maximum Error = " + me.ToString());

      // check the diagonal
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
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

    #region Positve Definite Property Test 1

    // check the matrix is not positve definite
    [Test]
    public void PositveDefinitePropertyTest1()
    {
      var dsl = new DoubleSymmetricLevinson(T1);
      Assert.IsTrue(dsl.IsPositiveDefinite);
    }

    #endregion Positve Definite Property Test 1

    #region Positve Definite Property Test 2

    // check the matrix is not positve definite
    [Test]
    public void PositveDefinitePropertyTest2()
    {
      var dsl = new DoubleSymmetricLevinson(T2);
      Assert.IsTrue(dsl.IsPositiveDefinite);
    }

    #endregion Positve Definite Property Test 2

    #region Positve Definite Property Test 3

    // check the matrix is not positve definite
    [Test]
    public void PositveDefinitePropertyTest3()
    {
      var dsl = new DoubleSymmetricLevinson(T3);
      Assert.IsTrue(dsl.IsPositiveDefinite);
    }

    #endregion Positve Definite Property Test 3

    #region Positve Definite Property Test 4

    // check the matrix is not positve definite
    [Test]
    public void PositveDefinitePropertyTest4()
    {
      var dsl = new DoubleSymmetricLevinson(T4);
      Assert.IsTrue(dsl.IsPositiveDefinite);
    }

    #endregion Positve Definite Property Test 4

    #region Positve Definite Property Test 5

    // check the matrix is not positve definite
    [Test]
    public void PositveDefinitePropertyTest5()
    {
      var dsl = new DoubleSymmetricLevinson(T5);
      Assert.IsTrue(dsl.IsPositiveDefinite);
    }

    #endregion Positve Definite Property Test 5

    #region Positve Definite Property Test 10

    // check the matrix is not positve definite
    [Test]
    public void PositveDefinitePropertyTest10()
    {
      var dsl = new DoubleSymmetricLevinson(T10);
      Assert.IsTrue(dsl.IsPositiveDefinite);
    }

    #endregion Positve Definite Property Test 10

    #region Positve Definite Property Test - Negative Case

    // check the matrix is not positve definite
    [Test]
    public void PositveDefinitePropertyTest()
    {
      var dsl = new DoubleSymmetricLevinson(1.0, 2.0, 3.0, 4.0, 5.0);
      Assert.IsFalse(dsl.IsPositiveDefinite);
    }

    #endregion Positve Definite Property Test - Negative Case

    #region Singularity Property Test 1

    // check that singular matrix is detected
    [Test]
    public void SingularityPropertyTest1()
    {
      var dsl = new DoubleSymmetricLevinson(T1);
      Assert.IsFalse(dsl.IsSingular);
    }

    #endregion Singularity Property Test 1

    #region Singularity Property Test 2

    // check that singular matrix is detected
    [Test]
    public void SingularityPropertyTest2()
    {
      var dsl = new DoubleSymmetricLevinson(T2);
      Assert.IsFalse(dsl.IsSingular);
    }

    #endregion Singularity Property Test 2

    #region Singularity Property Test 3

    // check that singular matrix is detected
    [Test]
    public void SingularityPropertyTest3()
    {
      var dsl = new DoubleSymmetricLevinson(T3);
      Assert.IsFalse(dsl.IsSingular);
    }

    #endregion Singularity Property Test 3

    #region Singularity Property Test 4

    // check that singular matrix is detected
    [Test]
    public void SingularityPropertyTest4()
    {
      var dsl = new DoubleSymmetricLevinson(T4);
      Assert.IsFalse(dsl.IsSingular);
    }

    #endregion Singularity Property Test 4

    #region Singularity Property Test 5

    // check that singular matrix is detected
    [Test]
    public void SingularityPropertyTest5()
    {
      var dsl = new DoubleSymmetricLevinson(T5);
      Assert.IsFalse(dsl.IsSingular);
    }

    #endregion Singularity Property Test 5

    #region Singularity Property Test 10

    // check that singular matrix is detected
    [Test]
    public void SingularityPropertyTest10()
    {
      var dsl = new DoubleSymmetricLevinson(T10);
      Assert.IsFalse(dsl.IsSingular);
    }

    #endregion Singularity Property Test 10

    #region Singularity Property Test - Negative Case

    // check that singular matrix is detected
    [Test]
    public void SingularityPropertyTest()
    {
      var T = new DoubleVector(10);
      for (int i = 1; i < 10; i++)
      {
        T[i] = i + 1;
      }
      T[0] = -2.0;

      var dsl = new DoubleSymmetricLevinson(T);
      Assert.IsTrue(dsl.IsSingular);
    }

    #endregion Singularity Property Test - Negative Case

    #region GetDeterminant Method Test 1

    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest1()
    {
      // calculate determinant from diagonal
      var dsl = new DoubleSymmetricLevinson(T1);

      // check results match
      Double e = System.Math.Abs((dsl.GetDeterminant() - Det1) / Det1);
      Assert.IsTrue(e < Tolerance1);
    }

    #endregion GetDeterminant Method Test 1

    #region GetDeterminant Method Test 2

    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest2()
    {
      // calculate determinant from diagonal
      var dsl = new DoubleSymmetricLevinson(T2);

      // check results match
      Double e = System.Math.Abs((dsl.GetDeterminant() - Det2) / Det2);
      Assert.IsTrue(e < Tolerance2);
    }

    #endregion GetDeterminant Method Test 2

    #region GetDeterminant Method Test 3

    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest3()
    {
      // calculate determinant from diagonal
      var dsl = new DoubleSymmetricLevinson(T3);

      // check results match
      Double e = System.Math.Abs((dsl.GetDeterminant() - Det3) / Det3);
      Assert.IsTrue(e < Tolerance3);
    }

    #endregion GetDeterminant Method Test 3

    #region GetDeterminant Method Test 4

    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest4()
    {
      // calculate determinant from diagonal
      var dsl = new DoubleSymmetricLevinson(T4);

      // check results match
      Double e = System.Math.Abs((dsl.GetDeterminant() - Det4) / Det4);
      Assert.IsTrue(e < Tolerance4);
    }

    #endregion GetDeterminant Method Test 4

    #region GetDeterminant Method Test 5

    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest5()
    {
      // calculate determinant from diagonal
      var dsl = new DoubleSymmetricLevinson(T5);

      // check results match
      Double e = System.Math.Abs((dsl.GetDeterminant() - Det5) / Det5);
      Assert.IsTrue(e < Tolerance5);
    }

    #endregion GetDeterminant Method Test 5

    #region GetDeterminant Method Test 10

    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest10()
    {
      // calculate determinant from diagonal
      var dsl = new DoubleSymmetricLevinson(T10);

      // check results match
      Double e = System.Math.Abs((dsl.GetDeterminant() - Det10) / Det10);
      Assert.IsTrue(e < Tolerance10);
    }

    #endregion GetDeterminant Method Test 10

    #region Null Parameter Test for SolveVector

    [Test]
    public void NullParameterTestforSolveVector()
    {
      Assert.Throws(typeof(System.ArgumentNullException), () =>
      {
        var dsl = new DoubleSymmetricLevinson(T10);
        DoubleVector X = dsl.Solve(null as DoubleVector);
      });
    }

    #endregion Null Parameter Test for SolveVector

    #region Mismatch Rows Test for SolveVector

    [Test]
    public void MismatchRowsTestforSolveVector()
    {
      Assert.Throws(typeof(System.RankException), () =>
      {
        var dsl = new DoubleSymmetricLevinson(T10);
        DoubleVector X = dsl.Solve(X5);
      });
    }

    #endregion Mismatch Rows Test for SolveVector

    #region SolveVector 1

    // Test solving a linear system
    [Test]
    public void SolveVector1()
    {
      int i;
      double e, me;
      var dsl = new DoubleSymmetricLevinson(T1);
      DoubleVector X = dsl.Solve(Y1);

      // determine the maximum error
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
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
      var dsl = new DoubleSymmetricLevinson(T2);
      DoubleVector X = dsl.Solve(Y2);

      // determine the maximum error
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
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
      var dsl = new DoubleSymmetricLevinson(T3);
      DoubleVector X = dsl.Solve(Y3);

      // determine the maximum error
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
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
      var dsl = new DoubleSymmetricLevinson(T4);
      DoubleVector X = dsl.Solve(Y4);

      // determine the maximum error
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
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
      var dsl = new DoubleSymmetricLevinson(T5);
      DoubleVector X = dsl.Solve(Y5);

      // determine the maximum error
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
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
      var dsl = new DoubleSymmetricLevinson(T10);
      DoubleVector X = dsl.Solve(Y10);

      // determine the maximum error
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
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
    public void NullParameterTestforSolveMatrix()
    {
      Assert.Throws(typeof(System.ArgumentNullException), () =>
      {
        var dsl = new DoubleSymmetricLevinson(T10);
        DoubleMatrix X = dsl.Solve(null as DoubleMatrix);
      });
    }

    #endregion Null Parameter Test for SolveMatrix

    #region Mismatch Rows Test for SolveMatrix

    [Test]
    public void MismatchRowsTestforSolveMatrix()
    {
      Assert.Throws(typeof(System.RankException), () =>
      {
        var dsl = new DoubleSymmetricLevinson(T10);
        DoubleMatrix X = dsl.Solve(I5);
      });
    }

    #endregion Mismatch Rows Test for SolveMatrix

    #region Solve Matrix 1

    // calculate inverse by solving linear equations with identity RHS
    [Test]
    public void SolveMatrix1()
    {
      int i, j;
      double e, me;
      var dsl = new DoubleSymmetricLevinson(T1);

      // check inverse
      DoubleMatrix I = dsl.Solve(DoubleMatrix.CreateIdentity(1));
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
      {
        for (j = 0; j < dsl.Order; j++)
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
      var dsl = new DoubleSymmetricLevinson(T2);

      // check inverse
      DoubleMatrix I = dsl.Solve(DoubleMatrix.CreateIdentity(2));
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
      {
        for (j = 0; j < dsl.Order; j++)
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
      var dsl = new DoubleSymmetricLevinson(T3);

      // check inverse
      DoubleMatrix I = dsl.Solve(DoubleMatrix.CreateIdentity(3));
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
      {
        for (j = 0; j < dsl.Order; j++)
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
      var dsl = new DoubleSymmetricLevinson(T4);

      // check inverse
      DoubleMatrix I = dsl.Solve(DoubleMatrix.CreateIdentity(4));
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
      {
        for (j = 0; j < dsl.Order; j++)
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
      var dsl = new DoubleSymmetricLevinson(T5);

      // check inverse
      DoubleMatrix I = dsl.Solve(DoubleMatrix.CreateIdentity(5));
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
      {
        for (j = 0; j < dsl.Order; j++)
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
      var dsl = new DoubleSymmetricLevinson(T10);

      // check inverse
      DoubleMatrix I = dsl.Solve(DoubleMatrix.CreateIdentity(10));
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
      {
        for (j = 0; j < dsl.Order; j++)
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
      var dsl = new DoubleSymmetricLevinson(T1);

      // check inverse
      DoubleMatrix I = dsl.GetInverse();
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
      {
        for (j = 0; j < dsl.Order; j++)
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
      var dsl = new DoubleSymmetricLevinson(T2);

      // check inverse
      DoubleMatrix I = dsl.GetInverse();
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
      {
        for (j = 0; j < dsl.Order; j++)
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
      var dsl = new DoubleSymmetricLevinson(T3);

      // check inverse
      DoubleMatrix I = dsl.GetInverse();
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
      {
        for (j = 0; j < dsl.Order; j++)
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
      var dsl = new DoubleSymmetricLevinson(T4);

      // check inverse
      DoubleMatrix I = dsl.GetInverse();
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
      {
        for (j = 0; j < dsl.Order; j++)
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
      var dsl = new DoubleSymmetricLevinson(T5);

      // check inverse
      DoubleMatrix I = dsl.GetInverse();
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
      {
        for (j = 0; j < dsl.Order; j++)
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
      var dsl = new DoubleSymmetricLevinson(T10);

      // check inverse
      DoubleMatrix I = dsl.GetInverse();
      me = 0.0;
      for (i = 0; i < dsl.Order; i++)
      {
        for (j = 0; j < dsl.Order; j++)
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
    public void NullParameterTestforStaticSolveVector1()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        DoubleVector X = DoubleSymmetricLevinson.Solve(null, new double[] { 1.0, 1.0 });
      });
    }

    #endregion Null Parameter Test 1 for Static SolveVector

    #region Null Parameter Test for 2 Static SolveVector

    // test null parameter
    [Test]
    public void NullParameterTestforStaticSolveVector2()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        DoubleVector X = DoubleSymmetricLevinson.Solve(new double[] { 1.0, 1.0 }, null as IReadOnlyList<double>);
      });
    }

    #endregion Null Parameter Test for 2 Static SolveVector

    #region Row Mismatch Test for Static SolveVector

    // test mismatching dimensions
    [Test]
    public void RowMismatchTestforStaticSolveVector()
    {
      Assert.Throws(typeof(RankException), () =>
      {
        DoubleVector X = DoubleSymmetricLevinson.Solve(new double[] { 1.0, 1.0, 1.0 }, new double[] { 1.0, 1.0 });
      });
    }

    #endregion Row Mismatch Test for Static SolveVector

    #region Singular Test for Static SolveVector

    // test with Toeplitz matrix which has a singular principal sub-matrix
    [Test]
    public void SingularTestforStaticSolveVector()
    {
      Assert.Throws(typeof(SingularMatrixException), () =>
      {
        DoubleVector X = DoubleSymmetricLevinson.Solve(new double[] { 1.0, 1.0, 1.0 }, new double[] { 1.0, 1.0, 1.0 });
      });
    }

    #endregion Singular Test for Static SolveVector

    #region Static Solve Vector 1

    [Test]
    public void StaticSolveVector1()
    {
      int i;
      double e, me;
      DoubleVector X = DoubleSymmetricLevinson.Solve(T1, Y1);

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
      DoubleVector X = DoubleSymmetricLevinson.Solve(T2, Y2);

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
      DoubleVector X = DoubleSymmetricLevinson.Solve(T3, Y3);

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
      DoubleVector X = DoubleSymmetricLevinson.Solve(T4, Y4);

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
      DoubleVector X = DoubleSymmetricLevinson.Solve(T5, Y5);

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
      DoubleVector X = DoubleSymmetricLevinson.Solve(T10, Y10);

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
    public void NullParameterTestforStaticSolveMatrix1()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        var Y = new DoubleMatrix(2);
        Y[0, 0] = Y[1, 1] = 2.0;
        Y[0, 1] = Y[1, 0] = 1.0;
        DoubleMatrix X = DoubleSymmetricLevinson.Solve(null, Y);
      });
    }

    #endregion Null Parameter Test 1 for Static SolveMatrix

    #region Null Parameter Test 2 for Static SolveMatrix

    // test null parameter
    [Test]
    public void NullParameterTestforStaticSolveMatrix2()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        DoubleMatrix Y = DoubleSymmetricLevinson.Solve(new double[] { 1.0, 1.0 }, null as DoubleMatrix);
      });
    }

    #endregion Null Parameter Test 2 for Static SolveMatrix

    #region Row Mismatch Test for Static SolveMatrix

    // test mismatching dimensions
    [Test]
    public void RowMismatchTestforStaticSolveMatrix()
    {
      Assert.Throws(typeof(RankException), () =>
      {
        var Y = new DoubleMatrix(2);
        Y[0, 0] = Y[1, 1] = 2.0;
        Y[0, 1] = Y[1, 0] = 1.0;

        DoubleMatrix X = DoubleSymmetricLevinson.Solve(new double[] { 1.0, 1.0, 1.0 }, Y);
      });
    }

    #endregion Row Mismatch Test for Static SolveMatrix

    #region Singular Test for Static SolveMatrix

    // test with Toeplitz matrix which has a singular principal sub-matrix
    [Test]
    public void SingularTestforStaticSolveMatrix()
    {
      Assert.Throws(typeof(SingularMatrixException), () =>
      {
        DoubleMatrix X = DoubleSymmetricLevinson.Solve(new double[] { 1.0, 1.0, 1.0 }, DoubleMatrix.CreateIdentity(3));
      });
    }

    #endregion Singular Test for Static SolveMatrix

    #region Static Solve Matrix 1

    // calculate inverse by solving linear system with identity RHS
    [Test]
    public void StaticSolveMatrix1()
    {
      int i, j;
      double e, me;

      // calculate inverse by solving the linear system
      DoubleMatrix I = DoubleSymmetricLevinson.Solve(T1, DoubleMatrix.CreateIdentity(1));

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

    #endregion Static Solve Matrix 1

    #region Static Solve Matrix 2

    // calculate inverse by solving linear system with identity RHS
    [Test]
    public void StaticSolveMatrix2()
    {
      int i, j;
      double e, me;

      // calculate inverse by solving the linear system
      DoubleMatrix I = DoubleSymmetricLevinson.Solve(T2, DoubleMatrix.CreateIdentity(2));

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

    #endregion Static Solve Matrix 2

    #region Static Solve Matrix 3

    // calculate inverse by solving linear system with identity RHS
    [Test]
    public void StaticSolveMatrix3()
    {
      int i, j;
      double e, me;

      // calculate inverse by solving the linear system
      DoubleMatrix I = DoubleSymmetricLevinson.Solve(T3, DoubleMatrix.CreateIdentity(3));

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

    #endregion Static Solve Matrix 3

    #region Static Solve Matrix 4

    // calculate inverse by solving linear system with identity RHS
    [Test]
    public void StaticSolveMatrix4()
    {
      int i, j;
      double e, me;

      // calculate inverse by solving the linear system
      DoubleMatrix I = DoubleSymmetricLevinson.Solve(T4, DoubleMatrix.CreateIdentity(4));

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

    #endregion Static Solve Matrix 4

    #region Static Solve Matrix 5

    // calculate inverse by solving linear system with identity RHS
    [Test]
    public void StaticSolveMatrix5()
    {
      int i, j;
      double e, me;

      // calculate inverse by solving the linear system
      DoubleMatrix I = DoubleSymmetricLevinson.Solve(T5, DoubleMatrix.CreateIdentity(5));

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

    #endregion Static Solve Matrix 5

    #region Static Solve Matrix 10

    // calculate inverse by solving linear system with identity RHS
    [Test]
    public void StaticSolveMatrix10()
    {
      int i, j;
      double e, me;

      // calculate inverse by solving the linear system
      DoubleMatrix I = DoubleSymmetricLevinson.Solve(T10, DoubleMatrix.CreateIdentity(10));

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

    #endregion Static Solve Matrix 10

    #region Null Parameter Test for Static YuleWalker

    // test Yule-Walker with a null reference
    [Test]
    public void NullParameterTestforStaticYuleWalker()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        DoubleVector Y = DoubleSymmetricLevinson.YuleWalker(null);
      });
    }

    #endregion Null Parameter Test for Static YuleWalker

    #region Row Test for Static YuleWalker

    // test Yule-Walker with order 1 matrix
    [Test]
    public void RowTestforStaticYuleWalker()
    {
      Assert.Throws(typeof(ArgumentOutOfRangeException), () =>
      {
        DoubleVector Y = DoubleSymmetricLevinson.YuleWalker(new double[] { 1.0 });
      });
    }

    #endregion Row Test for Static YuleWalker

    #region Singular Test for Static YuleWalker

    // test Yule-Walker with matrix with singular principal sub-matrix
    [Test]
    public void SingularTestforStaticYuleWalker()
    {
      Assert.Throws(typeof(SingularMatrixException), () =>
      {
        DoubleVector Y = DoubleSymmetricLevinson.YuleWalker(new double[] { 1.0, 1.0, 1.0 });
      });
    }

    #endregion Singular Test for Static YuleWalker

    #region Static Yule Walker 2

    [Test]
    public void StaticYuleWalker2()
    {
      int i;
      double e, me;
      int N = T2.Length;
      DoubleVector A = DoubleSymmetricLevinson.YuleWalker(T2);

      // determine the maximum error
      me = 0.0;
      for (i = 0; i < A.Length; i++)
      {
        e = System.Math.Abs((A2[N - 1, N - 2 - i] - A[i]) / A2[N - 1, N - 2 - i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.IsTrue(me < Tolerance2, "Maximum Error = " + me.ToString());
    }

    #endregion Static Yule Walker 2

    #region Static Yule Walker 3

    [Test]
    public void StaticYuleWalker3()
    {
      int i;
      double e, me;
      int N = T3.Length;
      DoubleVector A = DoubleSymmetricLevinson.YuleWalker(T3);

      // determine the maximum error
      me = 0.0;
      for (i = 0; i < A.Length; i++)
      {
        e = System.Math.Abs((A3[N - 1, N - 2 - i] - A[i]) / A3[N - 1, N - 2 - i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.IsTrue(me < Tolerance3, "Maximum Error = " + me.ToString());
    }

    #endregion Static Yule Walker 3

    #region Static Yule Walker 4

    [Test]
    public void StaticYuleWalker4()
    {
      int i;
      double e, me;
      int N = T4.Length;
      DoubleVector A = DoubleSymmetricLevinson.YuleWalker(T4);

      // determine the maximum error
      me = 0.0;
      for (i = 0; i < A.Length; i++)
      {
        e = System.Math.Abs((A4[N - 1, N - 2 - i] - A[i]) / A4[N - 1, N - 2 - i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.IsTrue(me < Tolerance4, "Maximum Error = " + me.ToString());
    }

    #endregion Static Yule Walker 4

    #region Static Yule Walker 5

    [Test]
    public void StaticYuleWalker5()
    {
      int i;
      double e, me;
      int N = T5.Length;
      DoubleVector A = DoubleSymmetricLevinson.YuleWalker(T5);

      // determine the maximum error
      me = 0.0;
      for (i = 0; i < A.Length; i++)
      {
        e = System.Math.Abs((A5[N - 1, N - 2 - i] - A[i]) / A5[N - 1, N - 2 - i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.IsTrue(me < Tolerance5, "Maximum Error = " + me.ToString());
    }

    #endregion Static Yule Walker 5

    #region Static Yule Walker 10

    [Test]
    public void StaticYuleWalker10()
    {
      int i;
      double e, me;
      int N = T10.Length;
      DoubleVector A = DoubleSymmetricLevinson.YuleWalker(T10);

      // determine the maximum error
      me = 0.0;
      for (i = 0; i < A.Length; i++)
      {
        e = System.Math.Abs((A10[N - 1, N - 2 - i] - A[i]) / A10[N - 1, N - 2 - i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.IsTrue(me < Tolerance10, "Maximum Error = " + me.ToString());
    }

    #endregion Static Yule Walker 10

    #region Null Prameter Test for Static Inverse

    [Test]
    public void NullPrameterTestforStaticInverse()
    {
      Assert.Throws(typeof(System.ArgumentNullException), () =>
      {
        DoubleMatrix Y = DoubleSymmetricLevinson.Inverse(null);
      });
    }

    #endregion Null Prameter Test for Static Inverse

    #region Singular Test for Static Inverse

    [Test]
    public void SingularTestforStaticInverse()
    {
      Assert.Throws(typeof(SingularMatrixException), () =>
      {
        // setup an ill-conditioned system (second order principal submatrix is singular)
        var T = new DoubleVector(3)
        {
          [0] = 1.0,
          [1] = 1.0,
          [2] = 1.0
        };

        DoubleMatrix Y = DoubleSymmetricLevinson.Inverse(T);
      });
    }

    #endregion Singular Test for Static Inverse

    #region Static Inverse 1

    [Test]
    public void StaticInverse1()
    {
      int i, j;
      double e, me;

      // calculate the inverse
      DoubleMatrix I = DoubleSymmetricLevinson.Inverse(T1);

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
      DoubleMatrix I = DoubleSymmetricLevinson.Inverse(T2);

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
      DoubleMatrix I = DoubleSymmetricLevinson.Inverse(T3);

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
      DoubleMatrix I = DoubleSymmetricLevinson.Inverse(T4);

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
      DoubleMatrix I = DoubleSymmetricLevinson.Inverse(T5);

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
      DoubleMatrix I = DoubleSymmetricLevinson.Inverse(T10);

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

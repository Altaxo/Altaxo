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

#region Using Directives

using System;
using System.IO;
using NUnit.Framework;
using Altaxo.Calc;
using Altaxo.Calc.LinearAlgebra;

#endregion Using Directives

namespace AltaxoTest.Calc.LinearAlgebra
{
  
  // suite of tests for complex double symmetric algorithm
  [TestFixture]
  public class ComplexDoubleSymmetricLevinsonTest
  {

    #region Fields

    // unit testing - order 1
    
    ComplexDoubleVector T1;     // Toeplitz matrix
    ComplexDoubleMatrix L1;     // Lower triangle matrix
    ComplexDoubleVector D1;     // diagonal vector
    ComplexDoubleMatrix I1;     // inverse matrix
    Complex Det1;       // exact determinant
    ComplexDoubleVector X1;     // RHS vector
    ComplexDoubleVector Y1;     // LHS vector
    double Tolerance1;        // allowable tolerance

    // unit testing - order 2
    
    ComplexDoubleVector T2;     // Toeplitz matrix
    ComplexDoubleMatrix L2;     // Lower triangle matrix
    ComplexDoubleVector D2;     // diagonal vector
    ComplexDoubleMatrix I2;     // inverse matrix
    Complex Det2;       // exact determinant
    ComplexDoubleVector X2;     // RHS vector
    ComplexDoubleVector Y2;     // LHS vector
    double Tolerance2;        // allowable tolerance

    // unit testing - order 3
    
    ComplexDoubleVector T3;     // Toeplitz matrix
    ComplexDoubleMatrix L3;     // Lower triangle matrix
    ComplexDoubleVector D3;     // diagonal vector
    ComplexDoubleMatrix I3;     // inverse matrix
    Complex Det3;       // exact determinant
    ComplexDoubleVector X3;     // RHS vector
    ComplexDoubleVector Y3;     // LHS vector
    double Tolerance3;        // allowable tolerance

    // unit testing - order 4
    
    ComplexDoubleVector T4;     // Toeplitz matrix
    ComplexDoubleMatrix L4;     // Lower triangle matrix
    ComplexDoubleVector D4;     // diagonal vector
    ComplexDoubleMatrix I4;     // inverse matrix
    Complex Det4;       // exact determinant
    ComplexDoubleVector X4;     // RHS vector
    ComplexDoubleVector Y4;     // LHS vector
    double Tolerance4;        // allowable tolerance

    // unit testing - order 5
    
    ComplexDoubleVector T5;     // Toeplitz matrix
    ComplexDoubleMatrix L5;     // Lower triangle matrix
    ComplexDoubleVector D5;     // diagonal vector
    ComplexDoubleMatrix I5;     // inverse matrix
    Complex Det5;       // exact determinant
    ComplexDoubleVector X5;     // RHS vector
    ComplexDoubleVector Y5;     // LHS vector
    double Tolerance5;        // allowable tolerance

    #endregion Fields
    
    #region Text Fixture Setup

    [TestFixtureSetUp]
    public void SetupTestCases()
    {
      // unit testing values - order 1

      T1 = new ComplexDoubleVector(1);
      T1[0] = new Complex(+1.0000000000000000E+000, +1.0000000000000000E+000);

      L1 = new ComplexDoubleMatrix(1);
      L1[0, 0] = new Complex(+1.0000000000000000E+000, +0.0000000000000000E+000);

      D1 = new ComplexDoubleVector(1);
      D1[0] = new Complex(+5.0000000000000000E-001, -5.0000000000000000E-001);

      Det1 = new Complex(+1.0000000000000000E+000, +1.0000000000000000E+000);

      I1 = new ComplexDoubleMatrix(1);
      I1[0, 0] = new Complex(+5.0000000000000000E-001, -5.0000000000000000E-001);

      X1 = new ComplexDoubleVector(1);
      X1[0] = new Complex(+1.0000000000000000E+000, -1.0000000000000000E+000);

      Y1 = new ComplexDoubleVector(1);
      Y1[0] = new Complex(+2.0000000000000000E+000, +0.0000000000000000E+000);

      Tolerance1 = +3.000E-016;

      // unit testing values - order 2

      T2 = new ComplexDoubleVector(2);
      T2[0] = new Complex(+1.0000000000000000E+000, +1.0000000000000000E+000);
      T2[1] = new Complex(+5.0000000000000000E-001, +5.0000000000000000E-001);

      L2 = new ComplexDoubleMatrix(2);
      L2[0, 0] = new Complex(+1.0000000000000000E+000, +0.0000000000000000E+000);
      L2[1, 0] = new Complex(-5.0000000000000000E-001, +0.0000000000000000E+000);
      L2[1, 1] = new Complex(+1.0000000000000000E+000, +0.0000000000000000E+000);

      D2 = new ComplexDoubleVector(2);
      D2[0] = new Complex(+5.0000000000000000E-001, -5.0000000000000000E-001);
      D2[1] = new Complex(+6.6666666666666663E-001, -6.6666666666666663E-001);

      Det2 = new Complex(+0.0000000000000000E+000, +1.5000000000000000E+000);

      I2 = new ComplexDoubleMatrix(2);
      I2[0, 0] = new Complex(+6.6666666666666663E-001, -6.6666666666666663E-001);
      I2[0, 1] = new Complex(-3.3333333333333331E-001, +3.3333333333333331E-001);
      I2[1, 0] = new Complex(-3.3333333333333331E-001, +3.3333333333333331E-001);
      I2[1, 1] = new Complex(+6.6666666666666663E-001, -6.6666666666666663E-001);

      X2 = new ComplexDoubleVector(2);
      X2[0] = new Complex(+1.0000000000000000E+000, -1.0000000000000000E+000);
      X2[1] = new Complex(+2.0000000000000000E+000, +2.0000000000000000E+000);

      Y2 = new ComplexDoubleVector(2);
      Y2[0] = new Complex(+2.0000000000000000E+000, +2.0000000000000000E+000);
      Y2[1] = new Complex(+1.0000000000000000E+000, +4.0000000000000000E+000);

      Tolerance2 = +3.000E-016;

      // unit testing values - order 3

      T3 = new ComplexDoubleVector(3);
      T3[0] = new Complex(+2.0000000000000000E+000, +2.0000000000000000E+000);
      T3[1] = new Complex(+1.0000000000000000E+000, +0.0000000000000000E+000);
      T3[2] = new Complex(+5.0000000000000000E-001, +0.0000000000000000E+000);

      L3 = new ComplexDoubleMatrix(3);
      L3[0, 0] = new Complex(+1.0000000000000000E+000, +0.0000000000000000E+000);
      L3[1, 0] = new Complex(-2.5000000000000000E-001, +2.5000000000000000E-001);
      L3[1, 1] = new Complex(+1.0000000000000000E+000, +0.0000000000000000E+000);
      L3[2, 0] = new Complex(-1.2307692307692308E-001, +1.5384615384615385E-002);
      L3[2, 1] = new Complex(-2.2307692307692309E-001, +2.1538461538461540E-001);
      L3[2, 2] = new Complex(+1.0000000000000000E+000, +0.0000000000000000E+000);

      D3 = new ComplexDoubleVector(3);
      D3[0] = new Complex(+2.5000000000000000E-001, -2.5000000000000000E-001);
      D3[1] = new Complex(+2.1538461538461540E-001, -2.7692307692307694E-001);
      D3[2] = new Complex(+2.1756097560975610E-001, -2.8195121951219509E-001);

      Det3 = new Complex(-1.9500000000000000E+001, +1.1500000000000000E+001);

      I3 = new ComplexDoubleMatrix(3);
      I3[0, 0] = new Complex(+2.1756097560975610E-001, -2.8195121951219509E-001);
      I3[0, 1] = new Complex(+1.2195121951219513E-002, +1.0975609756097561E-001);
      I3[0, 2] = new Complex(-2.2439024390243902E-002, +3.8048780487804877E-002);
      I3[1, 0] = new Complex(+1.2195121951219513E-002, +1.0975609756097561E-001);
      I3[1, 1] = new Complex(+1.8902439024390244E-001, -2.9878048780487804E-001);
      I3[1, 2] = new Complex(+1.2195121951219513E-002, +1.0975609756097561E-001);
      I3[2, 0] = new Complex(-2.2439024390243902E-002, +3.8048780487804877E-002);
      I3[2, 1] = new Complex(+1.2195121951219513E-002, +1.0975609756097561E-001);
      I3[2, 2] = new Complex(+2.1756097560975610E-001, -2.8195121951219509E-001);

      X3 = new ComplexDoubleVector(3);
      X3[0] = new Complex(+1.0000000000000000E+000, -1.0000000000000000E+000);
      X3[1] = new Complex(+2.0000000000000000E+000, +2.0000000000000000E+000);
      X3[2] = new Complex(+3.0000000000000000E+000, -3.0000000000000000E+000);

      Y3 = new ComplexDoubleVector(3);
      Y3[0] = new Complex(+7.5000000000000000E+000, +5.0000000000000000E-001);
      Y3[1] = new Complex(+4.0000000000000000E+000, +4.0000000000000000E+000);
      Y3[2] = new Complex(+1.4500000000000000E+001, +1.5000000000000000E+000);

      Tolerance3 = +1.000E-015;

      // unit testing values - order 4

      T4 = new ComplexDoubleVector(4);
      T4[0] = new Complex(+4.0000000000000000E+000, +0.0000000000000000E+000);
      T4[1] = new Complex(+1.2000000000000000E+001, -1.3333333333333333E+000);
      T4[2] = new Complex(+2.6666666666666668E+001, +2.1333333333333332E+001);
      T4[3] = new Complex(+4.8000000000000000E+001, -5.3333333333333330E+000);

      L4 = new ComplexDoubleMatrix(4);
      L4[0, 0] = new Complex(+1.0000000000000000E+000, +0.0000000000000000E+000);
      L4[1, 0] = new Complex(-3.0000000000000000E+000, +3.3333333333333331E-001);
      L4[1, 1] = new Complex(+1.0000000000000000E+000, +0.0000000000000000E+000);
      L4[2, 0] = new Complex(-4.8611369990680336E-001, +8.0633737185461318E-001);
      L4[2, 1] = new Complex(-1.8104380242311278E+000, -2.2477166821994410E+000);
      L4[2, 2] = new Complex(+1.0000000000000000E+000, +0.0000000000000000E+000);
      L4[3, 0] = new Complex(+3.3484605496212611E-001, -1.9271448251604772E+000);
      L4[3, 1] = new Complex(-5.4240073026015514E+000, +3.5426745778183477E+000);
      L4[3, 2] = new Complex(-4.1928238540484636E-001, -1.0409062930503854E+000);
      L4[3, 3] = new Complex(+1.0000000000000000E+000, +0.0000000000000000E+000);

      D4 = new ComplexDoubleVector(4);
      D4[0] = new Complex(+2.5000000000000000E-001, +0.0000000000000000E+000);
      D4[1] = new Complex(-2.9776328052190121E-002, -7.5489282385834107E-003);
      D4[2] = new Complex(-1.8372281937366793E-002, +4.8476042525929675E-003);
      D4[3] = new Complex(-3.4274173601848597E-003, +2.0146632548174214E-003);

      Det4 = new Complex(-1.4775024197530865E+006, -8.8785224691358022E+005);

      I4 = new ComplexDoubleMatrix(4);
      I4[0, 0] = new Complex(-3.4274173601848597E-003, +2.0146632548174214E-003);
      I4[0, 1] = new Complex(+3.5341313868731158E-003, +2.7229074838592197E-003);
      I4[0, 2] = new Complex(+1.1453040494999565E-002, -2.3069772555912881E-002);
      I4[0, 3] = new Complex(+2.7348906841957528E-003, +7.2797316722982060E-003);
      I4[1, 0] = new Complex(+3.5341313868731158E-003, +2.7229074838592197E-003);
      I4[1, 1] = new Complex(-1.7019789440246038E-002, +2.7237506460653452E-005);
      I4[1, 2] = new Complex(+1.5342389186985224E-002, +3.0270604877556437E-002);
      I4[1, 3] = new Complex(+1.1453040494999565E-002, -2.3069772555912881E-002);
      I4[2, 0] = new Complex(+1.1453040494999565E-002, -2.3069772555912881E-002);
      I4[2, 1] = new Complex(+1.5342389186985224E-002, +3.0270604877556437E-002);
      I4[2, 2] = new Complex(-1.7019789440246038E-002, +2.7237506460653452E-005);
      I4[2, 3] = new Complex(+3.5341313868731158E-003, +2.7229074838592197E-003);
      I4[3, 0] = new Complex(+2.7348906841957528E-003, +7.2797316722982060E-003);
      I4[3, 1] = new Complex(+1.1453040494999565E-002, -2.3069772555912881E-002);
      I4[3, 2] = new Complex(+3.5341313868731158E-003, +2.7229074838592197E-003);
      I4[3, 3] = new Complex(-3.4274173601848597E-003, +2.0146632548174214E-003);

      X4 = new ComplexDoubleVector(4);
      X4[0] = new Complex(+1.0000000000000000E+000, -1.0000000000000000E+000);
      X4[1] = new Complex(+2.0000000000000000E+000, +2.0000000000000000E+000);
      X4[2] = new Complex(+3.0000000000000000E+000, -3.0000000000000000E+000);
      X4[3] = new Complex(+4.0000000000000000E+000, -4.0000000000000000E+000);

      Y4 = new ComplexDoubleVector(4);
      Y4[0] = new Complex(+3.4533333333333331E+002, -2.1200000000000000E+002);
      Y4[1] = new Complex(+2.4266666666666666E+002, -6.6666666666666671E+001);
      Y4[2] = new Complex(+1.2933333333333334E+002, -4.9333333333333336E+001);
      Y4[3] = new Complex(+1.0133333333333333E+002, -1.3333333333333334E+001);

      Tolerance4 = +3.000E-014;

      // unit testing values - order 5

      T5 = new ComplexDoubleVector(5);
      T5[0] = new Complex(+5.0000000000000000E+000, +0.0000000000000000E+000);
      T5[1] = new Complex(+0.0000000000000000E+000, +4.0000000000000000E+000);
      T5[2] = new Complex(+3.0000000000000000E+000, +0.0000000000000000E+000);
      T5[3] = new Complex(+0.0000000000000000E+000, +2.0000000000000000E+000);
      T5[4] = new Complex(+1.0000000000000000E+000, +0.0000000000000000E+000);

      L5 = new ComplexDoubleMatrix(5);
      L5[0, 0] = new Complex(+1.0000000000000000E+000, +0.0000000000000000E+000);
      L5[1, 0] = new Complex(+0.0000000000000000E+000, -8.0000000000000004E-001);
      L5[1, 1] = new Complex(+1.0000000000000000E+000, +0.0000000000000000E+000);
      L5[2, 0] = new Complex(-7.5609756097560976E-001, +0.0000000000000000E+000);
      L5[2, 1] = new Complex(+0.0000000000000000E+000, -1.9512195121951220E-001);
      L5[2, 2] = new Complex(+1.0000000000000000E+000, +0.0000000000000000E+000);
      L5[3, 0] = new Complex(+0.0000000000000000E+000, +4.5833333333333331E-001);
      L5[3, 1] = new Complex(-6.6666666666666663E-001, +0.0000000000000000E+000);
      L5[3, 2] = new Complex(+0.0000000000000000E+000, -5.4166666666666663E-001);
      L5[3, 3] = new Complex(+1.0000000000000000E+000, +0.0000000000000000E+000);
      L5[4, 0] = new Complex(+4.1176470588235292E-001, +0.0000000000000000E+000);
      L5[4, 1] = new Complex(+0.0000000000000000E+000, +2.3529411764705882E-001);
      L5[4, 2] = new Complex(-9.4117647058823528E-001, +0.0000000000000000E+000);
      L5[4, 3] = new Complex(+0.0000000000000000E+000, -3.5294117647058826E-001);
      L5[4, 4] = new Complex(+1.0000000000000000E+000, +0.0000000000000000E+000);

      D5 = new ComplexDoubleVector(5);
      D5[0] = new Complex(+2.0000000000000001E-001, +0.0000000000000000E+000);
      D5[1] = new Complex(+1.2195121951219512E-001, +0.0000000000000000E+000);
      D5[2] = new Complex(+2.8472222222222221E-001, +0.0000000000000000E+000);
      D5[3] = new Complex(+2.3529411764705882E-001, +0.0000000000000000E+000);
      D5[4] = new Complex(+2.8333333333333333E-001, +0.0000000000000000E+000);

      Det5 = new Complex(+2.1600000000000000E+003, +0.0000000000000000E+000);

      I5 = new ComplexDoubleMatrix(5);
      I5[0, 0] = new Complex(+2.8333333333333333E-001, +0.0000000000000000E+000);
      I5[0, 1] = new Complex(+0.0000000000000000E+000, -1.0000000000000001E-001);
      I5[0, 2] = new Complex(-2.6666666666666666E-001, +0.0000000000000000E+000);
      I5[0, 3] = new Complex(+0.0000000000000000E+000, +6.6666666666666666E-002);
      I5[0, 4] = new Complex(+1.1666666666666667E-001, +0.0000000000000000E+000);
      I5[1, 0] = new Complex(+0.0000000000000000E+000, -1.0000000000000001E-001);
      I5[1, 1] = new Complex(+2.0000000000000001E-001, +0.0000000000000000E+000);
      I5[1, 2] = new Complex(+0.0000000000000000E+000, -3.3333333333333333E-002);
      I5[1, 3] = new Complex(-1.3333333333333333E-001, +0.0000000000000000E+000);
      I5[1, 4] = new Complex(+0.0000000000000000E+000, +6.6666666666666666E-002);
      I5[2, 0] = new Complex(-2.6666666666666666E-001, +0.0000000000000000E+000);
      I5[2, 1] = new Complex(+0.0000000000000000E+000, -3.3333333333333333E-002);
      I5[2, 2] = new Complex(+4.6666666666666667E-001, +0.0000000000000000E+000);
      I5[2, 3] = new Complex(+0.0000000000000000E+000, -3.3333333333333333E-002);
      I5[2, 4] = new Complex(-2.6666666666666666E-001, +0.0000000000000000E+000);
      I5[3, 0] = new Complex(+0.0000000000000000E+000, +6.6666666666666666E-002);
      I5[3, 1] = new Complex(-1.3333333333333333E-001, +0.0000000000000000E+000);
      I5[3, 2] = new Complex(+0.0000000000000000E+000, -3.3333333333333333E-002);
      I5[3, 3] = new Complex(+2.0000000000000001E-001, +0.0000000000000000E+000);
      I5[3, 4] = new Complex(+0.0000000000000000E+000, -1.0000000000000001E-001);
      I5[4, 0] = new Complex(+1.1666666666666667E-001, +0.0000000000000000E+000);
      I5[4, 1] = new Complex(+0.0000000000000000E+000, +6.6666666666666666E-002);
      I5[4, 2] = new Complex(-2.6666666666666666E-001, +0.0000000000000000E+000);
      I5[4, 3] = new Complex(+0.0000000000000000E+000, -1.0000000000000001E-001);
      I5[4, 4] = new Complex(+2.8333333333333333E-001, +0.0000000000000000E+000);

      X5 = new ComplexDoubleVector(5);
      X5[0] = new Complex(+1.0000000000000000E+000, -1.0000000000000000E+000);
      X5[1] = new Complex(+2.0000000000000000E+000, +2.0000000000000000E+000);
      X5[2] = new Complex(+3.0000000000000000E+000, -3.0000000000000000E+000);
      X5[3] = new Complex(+4.0000000000000000E+000, -4.0000000000000000E+000);
      X5[4] = new Complex(+5.0000000000000000E+000, +5.0000000000000000E+000);

      Y5 = new ComplexDoubleVector(5);
      Y5[0] = new Complex(+1.9000000000000000E+001, +7.0000000000000000E+000);
      Y5[1] = new Complex(+2.8000000000000000E+001, +2.4000000000000000E+001);
      Y5[2] = new Complex(+4.1000000000000000E+001, +2.1000000000000000E+001);
      Y5[3] = new Complex(+2.0000000000000000E+001, +2.0000000000000000E+001);
      Y5[4] = new Complex(+4.7000000000000000E+001, +3.5000000000000000E+001);

      Tolerance5 = +5.000E-015;
    }
    
    #endregion Text Fixture Setup

    #region Null Parameter Tests for Constructor 
    
    // Test constructor with a null parameter
    [Test]
    [ExpectedException(typeof(System.ArgumentNullException))]
    public void NullParameterTestforConstructor()
    {
      ComplexDoubleSymmetricLevinson cdsl = new ComplexDoubleSymmetricLevinson(null as ComplexDoubleVector);
    }

    #endregion  Null Parameter Tests for Constructor 

    #region Zero Length Vector Tests for Constructor
    
    // Test constructor with a zero length vector parameter
    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void ZeroLengthVectorTestsforConstructor1()
    {
      ComplexDoubleVector cdv = new ComplexDoubleVector(1, 0.0);
      cdv.RemoveAt(0);
      ComplexDoubleSymmetricLevinson  cdsl = new ComplexDoubleSymmetricLevinson(cdv);
    }

    #endregion Zero Length Vector Tests for Constructor
    
    #region GetVector Member Test

    // check get vector
    [Test]
    public void GetVectorMemberTest()
    {
      ComplexDoubleSymmetricLevinson  cdsl = new ComplexDoubleSymmetricLevinson(T5);
      ComplexDoubleVector TT = cdsl.GetVector();
      Assert.IsTrue(T5.Equals(TT));
    }
    
    #endregion GetVector Member Test
    
    #region GetMatrix Member Test
    
    // check get matrix
    [Test]
    public void GetMatrixMemberTest()
    {
      ComplexDoubleSymmetricLevinson  cdsl = new ComplexDoubleSymmetricLevinson(T5);
      ComplexDoubleMatrix cdsldm = cdsl.GetMatrix();
      for (int row = 0; row < T5.Length; row++)
      {
        for (int column = 0; column < T5.Length; column++)
        {
          if (column < row)
          {
            Assert.IsTrue(cdsldm[row, column] == T5[row - column]);
          }
          else
          {
            Assert.IsTrue(cdsldm[row, column] == T5[column - row]);
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
      ComplexDoubleSymmetricLevinson  cdsl = new ComplexDoubleSymmetricLevinson(T5);
      Assert.IsTrue(cdsl.Order == 5);
    }

    #endregion Order Property Test

    #region Decomposition Test 1

    // test the UDL factorisation for case 1
    [Test]
    public void DecompositionTest1()
    {
      int i, j;
      double e, me;
      ComplexDoubleSymmetricLevinson cdsl = new ComplexDoubleSymmetricLevinson(T1);
      ComplexDoubleMatrix U = cdsl.U;
      ComplexDoubleMatrix D = cdsl.D;
      ComplexDoubleMatrix L = cdsl.L;
      
      // check U is the transpose of L
      Assert.IsTrue(U.Equals(L.GetTranspose()));

      // check the lower triangle
      me = 0.0;
      for (i = 0; i < cdsl.Order; i++)
      {
        for (j = 0; j <= i ; j++)
        {
          e = ComplexMath.Absolute((L1[i, j] - L[i, j]) / L1[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance1, "Maximum Error = " + me.ToString());

      // check the diagonal
      me = 0.0;
      for (i = 0; i < cdsl.Order; i++)
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
      double e, me;
      ComplexDoubleSymmetricLevinson cdsl = new ComplexDoubleSymmetricLevinson(T2);
      ComplexDoubleMatrix U = cdsl.U;
      ComplexDoubleMatrix D = cdsl.D;
      ComplexDoubleMatrix L = cdsl.L;
      
      // check U is the transpose of L
      Assert.IsTrue(U.Equals(L.GetTranspose()));

      // check the lower triangle
      me = 0.0;
      for (i = 0; i < cdsl.Order; i++)
      {
        for (j = 0; j <= i ; j++)
        {
          e = ComplexMath.Absolute((L2[i, j] - L[i, j]) / L2[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance2, "Maximum Error = " + me.ToString());

      // check the diagonal
      me = 0.0;
      for (i = 0; i < cdsl.Order; i++)
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
      double e, me;
      ComplexDoubleSymmetricLevinson cdsl = new ComplexDoubleSymmetricLevinson(T3);
      ComplexDoubleMatrix U = cdsl.U;
      ComplexDoubleMatrix D = cdsl.D;
      ComplexDoubleMatrix L = cdsl.L;
      
      // check U is the transpose of L
      Assert.IsTrue(U.Equals(L.GetTranspose()));

      // check the lower triangle
      me = 0.0;
      for (i = 0; i < cdsl.Order; i++)
      {
        for (j = 0; j <= i ; j++)
        {
          e = ComplexMath.Absolute((L3[i, j] - L[i, j]) / L3[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance3, "Maximum Error = " + me.ToString());

      // check the diagonal
      me = 0.0;
      for (i = 0; i < cdsl.Order; i++)
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
      double e, me;
      ComplexDoubleSymmetricLevinson cdsl = new ComplexDoubleSymmetricLevinson(T4);
      ComplexDoubleMatrix U = cdsl.U;
      ComplexDoubleMatrix D = cdsl.D;
      ComplexDoubleMatrix L = cdsl.L;
      
      // check U is the transpose of L
      Assert.IsTrue(U.Equals(L.GetTranspose()));

      // check the lower triangle
      me = 0.0;
      for (i = 0; i < cdsl.Order; i++)
      {
        for (j = 0; j <= i ; j++)
        {
          e = ComplexMath.Absolute((L4[i, j] - L[i, j]) / L4[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance4, "Maximum Error = " + me.ToString());

      // check the diagonal
      me = 0.0;
      for (i = 0; i < cdsl.Order; i++)
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
      double e, me;
      ComplexDoubleSymmetricLevinson cdsl = new ComplexDoubleSymmetricLevinson(T5);
      ComplexDoubleMatrix U = cdsl.U;
      ComplexDoubleMatrix D = cdsl.D;
      ComplexDoubleMatrix L = cdsl.L;
      
      // check U is the transpose of L
      Assert.IsTrue(U.Equals(L.GetTranspose()));

      // check the lower triangle
      me = 0.0;
      for (i = 0; i < cdsl.Order; i++)
      {
        for (j = 0; j <= i ; j++)
        {
          e = ComplexMath.Absolute((L5[i, j] - L[i, j]) / L5[i, j]);
          if (e > me)
          {
            me = e;
          }
        }
      }
      Assert.IsTrue(me < Tolerance5, "Maximum Error = " + me.ToString());

      // check the diagonal
      me = 0.0;
      for (i = 0; i < cdsl.Order; i++)
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

    #region Singularity Property Test 1

    // check that singular matrix is detected
    [Test]
    public void SingularityPropertyTest1()
    {
      ComplexDoubleSymmetricLevinson cdsl = new ComplexDoubleSymmetricLevinson(T1);
      Assert.IsFalse(cdsl.IsSingular);
    }

    #endregion Singularity Property Test 1

    #region Singularity Property Test 2

    // check that singular matrix is detected
    [Test]
    public void SingularityPropertyTest2()
    {
      ComplexDoubleSymmetricLevinson cdsl = new ComplexDoubleSymmetricLevinson(T2);
      Assert.IsFalse(cdsl.IsSingular);
    }

    #endregion Singularity Property Test 2

    #region Singularity Property Test 3

    // check that singular matrix is detected
    [Test]
    public void SingularityPropertyTest3()
    {
      ComplexDoubleSymmetricLevinson cdsl = new ComplexDoubleSymmetricLevinson(T3);
      Assert.IsFalse(cdsl.IsSingular);
    }

    #endregion Singularity Property Test 3

    #region Singularity Property Test 4

    // check that singular matrix is detected
    [Test]
    public void SingularityPropertyTest4()
    {
      ComplexDoubleSymmetricLevinson cdsl = new ComplexDoubleSymmetricLevinson(T4);
      Assert.IsFalse(cdsl.IsSingular);
    }

    #endregion Singularity Property Test 4

    #region Singularity Property Test 5

    // check that singular matrix is detected
    [Test]
    public void SingularityPropertyTest5()
    {
      ComplexDoubleSymmetricLevinson cdsl = new ComplexDoubleSymmetricLevinson(T5);
      Assert.IsFalse(cdsl.IsSingular);
    }

    #endregion Singularity Property Test 5

    #region Singularity Property Test - Negative Case

    // check that singular matrix is detected
    [Test]
    public void SingularityPropertyTest()
    {
      ComplexDoubleVector T = new ComplexDoubleVector(10);
      for (int i = 1; i < 10; i++)
      {
        T[i] = new Complex((double) (i + 1), (double) (i + 1));
      }
      T[0] = new Complex(2.0, 2.0);

      ComplexDoubleSymmetricLevinson cdsl = new ComplexDoubleSymmetricLevinson(T);
      Assert.IsTrue(cdsl.IsSingular);
    }

    #endregion Singularity Property Test - Negative Case

    #region GetDeterminant Method Test 1
    
    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest1()
    {
      // calculate determinant from diagonal
      ComplexDoubleSymmetricLevinson  cdsl = new ComplexDoubleSymmetricLevinson(T1);

      // check results match
      double e = ComplexMath.Absolute((cdsl.GetDeterminant() - Det1) / Det1);
      Assert.IsTrue(e < Tolerance1);
    }
    
    #endregion GetDeterminant Method Test 1

    #region GetDeterminant Method Test 2
    
    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest2()
    {
      // calculate determinant from diagonal
      ComplexDoubleSymmetricLevinson  cdsl = new ComplexDoubleSymmetricLevinson(T2);

      // check results match
      double e = ComplexMath.Absolute((cdsl.GetDeterminant() - Det2) / Det2);
      Assert.IsTrue(e < Tolerance2);
    }
    
    #endregion GetDeterminant Method Test 2

    #region GetDeterminant Method Test 3
    
    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest3()
    {
      // calculate determinant from diagonal
      ComplexDoubleSymmetricLevinson  cdsl = new ComplexDoubleSymmetricLevinson(T3);

      // check results match
      double e = ComplexMath.Absolute((cdsl.GetDeterminant() - Det3) / Det3);
      Assert.IsTrue(e < Tolerance3);
    }
    
    #endregion GetDeterminant Method Test 3

    #region GetDeterminant Method Test 4
    
    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest4()
    {
      // calculate determinant from diagonal
      ComplexDoubleSymmetricLevinson  cdsl = new ComplexDoubleSymmetricLevinson(T4);

      // check results match
      double e = ComplexMath.Absolute((cdsl.GetDeterminant() - Det4) / Det4);
      Assert.IsTrue(e < Tolerance4);
    }
    
    #endregion GetDeterminant Method Test 4

    #region GetDeterminant Method Test 5
    
    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest5()
    {
      // calculate determinant from diagonal
      ComplexDoubleSymmetricLevinson  cdsl = new ComplexDoubleSymmetricLevinson(T5);

      // check results match
      double e = ComplexMath.Absolute((cdsl.GetDeterminant() - Det5) / Det5);
      Assert.IsTrue(e < Tolerance5);
    }
    
    #endregion GetDeterminant Method Test 5

    #region Null Parameter Test for SolveVector

    [Test]
    [ExpectedException(typeof(System.ArgumentNullException))]
    public void NullParameterTestforSolveVector()
    {
      ComplexDoubleSymmetricLevinson  cdsl = new ComplexDoubleSymmetricLevinson(T5);
      ComplexDoubleVector X = cdsl.Solve(null as ComplexDoubleVector);
    }

    #endregion Null Parameter Test for SolveVector

    #region Mismatch Rows Test for SolveVector

    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchRowsTestforSolveVector()
    {
      ComplexDoubleSymmetricLevinson  cdsl = new ComplexDoubleSymmetricLevinson(T4);
      ComplexDoubleVector X = cdsl.Solve(X5);
    }

    #endregion Mismatch Rows Test for SolveVector

    #region SolveVector 1

    // Test solving a linear system
    [Test]
    public void SolveVector1()
    {
      int i;
      double e, me;
      ComplexDoubleSymmetricLevinson  cdsl = new ComplexDoubleSymmetricLevinson(T1);
      ComplexDoubleVector X = cdsl.Solve(Y1);
      
      // determine the maximum error
      me = 0.0;
      for (i = 0; i < cdsl.Order; i++)
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
      double e, me;
      ComplexDoubleSymmetricLevinson  cdsl = new ComplexDoubleSymmetricLevinson(T2);
      ComplexDoubleVector X = cdsl.Solve(Y2);
      
      // determine the maximum error
      me = 0.0;
      for (i = 0; i < cdsl.Order; i++)
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
      double e, me;
      ComplexDoubleSymmetricLevinson  cdsl = new ComplexDoubleSymmetricLevinson(T3);
      ComplexDoubleVector X = cdsl.Solve(Y3);
      
      // determine the maximum error
      me = 0.0;
      for (i = 0; i < cdsl.Order; i++)
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
      double e, me;
      ComplexDoubleSymmetricLevinson  cdsl = new ComplexDoubleSymmetricLevinson(T4);
      ComplexDoubleVector X = cdsl.Solve(Y4);
      
      // determine the maximum error
      me = 0.0;
      for (i = 0; i < cdsl.Order; i++)
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
      double e, me;
      ComplexDoubleSymmetricLevinson  cdsl = new ComplexDoubleSymmetricLevinson(T5);
      ComplexDoubleVector X = cdsl.Solve(Y5);
      
      // determine the maximum error
      me = 0.0;
      for (i = 0; i < cdsl.Order; i++)
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

    #region Null Parameter Test for SolveMatrix

    [Test]
    [ExpectedException(typeof(System.ArgumentNullException))]
    public void NullParameterTestforSolveMatrix()
    {
      ComplexDoubleSymmetricLevinson  cdsl = new ComplexDoubleSymmetricLevinson(T5);
      ComplexDoubleMatrix X = cdsl.Solve(null as ComplexDoubleMatrix);
    }

    #endregion Null Parameter Test for SolveMatrix

    #region Mismatch Rows Test for SolveMatrix

    [Test]
    [ExpectedException(typeof(System.RankException))]
    public void MismatchRowsTestforSolveMatrix()
    {
      ComplexDoubleSymmetricLevinson  cdsl = new ComplexDoubleSymmetricLevinson(T4);
      ComplexDoubleMatrix X = cdsl.Solve(I5);
    }

    #endregion Mismatch Rows Test for SolveMatrix

    #region Solve Matrix 1

    // calculate inverse by solving linear equations with identity RHS
    [Test]
    public void SolveMatrix1()
    {
      int i, j;
      double e, me;
      ComplexDoubleSymmetricLevinson cdsl = new ComplexDoubleSymmetricLevinson(T1);

      // check inverse
      ComplexDoubleMatrix I = cdsl.Solve(ComplexDoubleMatrix.CreateIdentity(1));
      me = 0.0;
      for (i = 0; i < cdsl.Order; i++)
      {
        for (j = 0; j < cdsl.Order; j++)
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
      double e, me;
      ComplexDoubleSymmetricLevinson cdsl = new ComplexDoubleSymmetricLevinson(T2);

      // check inverse
      ComplexDoubleMatrix I = cdsl.Solve(ComplexDoubleMatrix.CreateIdentity(2));
      me = 0.0;
      for (i = 0; i < cdsl.Order; i++)
      {
        for (j = 0; j < cdsl.Order; j++)
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
      double e, me;
      ComplexDoubleSymmetricLevinson cdsl = new ComplexDoubleSymmetricLevinson(T3);

      // check inverse
      ComplexDoubleMatrix I = cdsl.Solve(ComplexDoubleMatrix.CreateIdentity(3));
      me = 0.0;
      for (i = 0; i < cdsl.Order; i++)
      {
        for (j = 0; j < cdsl.Order; j++)
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
      double e, me;
      ComplexDoubleSymmetricLevinson cdsl = new ComplexDoubleSymmetricLevinson(T4);

      // check inverse
      ComplexDoubleMatrix I = cdsl.Solve(ComplexDoubleMatrix.CreateIdentity(4));
      me = 0.0;
      for (i = 0; i < cdsl.Order; i++)
      {
        for (j = 0; j < cdsl.Order; j++)
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
      double e, me;
      ComplexDoubleSymmetricLevinson cdsl = new ComplexDoubleSymmetricLevinson(T5);

      // check inverse
      ComplexDoubleMatrix I = cdsl.Solve(ComplexDoubleMatrix.CreateIdentity(5));
      me = 0.0;
      for (i = 0; i < cdsl.Order; i++)
      {
        for (j = 0; j < cdsl.Order; j++)
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

    #region Get Inverse 1

    // calculate inverse using GetInverse member
    [Test]
    public void GetInverse1()
    {
      int i, j;
      double e, me;
      ComplexDoubleSymmetricLevinson cdsl = new ComplexDoubleSymmetricLevinson(T1);

      // check inverse
      ComplexDoubleMatrix I = cdsl.GetInverse();
      me = 0.0;
      for (i = 0; i < cdsl.Order; i++)
      {
        for (j = 0; j < cdsl.Order; j++)
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
      double e, me;
      ComplexDoubleSymmetricLevinson cdsl = new ComplexDoubleSymmetricLevinson(T2);

      // check inverse
      ComplexDoubleMatrix I = cdsl.GetInverse();
      me = 0.0;
      for (i = 0; i < cdsl.Order; i++)
      {
        for (j = 0; j < cdsl.Order; j++)
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
      double e, me;
      ComplexDoubleSymmetricLevinson cdsl = new ComplexDoubleSymmetricLevinson(T3);

      // check inverse
      ComplexDoubleMatrix I = cdsl.GetInverse();
      me = 0.0;
      for (i = 0; i < cdsl.Order; i++)
      {
        for (j = 0; j < cdsl.Order; j++)
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
      double e, me;
      ComplexDoubleSymmetricLevinson cdsl = new ComplexDoubleSymmetricLevinson(T4);

      // check inverse
      ComplexDoubleMatrix I = cdsl.GetInverse();
      me = 0.0;
      for (i = 0; i < cdsl.Order; i++)
      {
        for (j = 0; j < cdsl.Order; j++)
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
      double e, me;
      ComplexDoubleSymmetricLevinson cdsl = new ComplexDoubleSymmetricLevinson(T5);

      // check inverse
      ComplexDoubleMatrix I = cdsl.GetInverse();
      me = 0.0;
      for (i = 0; i < cdsl.Order; i++)
      {
        for (j = 0; j < cdsl.Order; j++)
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

    #region Null Parameter Test 1 for Static SolveVector
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullParameterTestforStaticSolveVector1()
    {
      ComplexDoubleVector X = ComplexDoubleSymmetricLevinson.Solve(null, X5);
    }

    #endregion Null Parameter Test 1 for Static SolveVector
    
    #region Null Parameter Test for 2 Static SolveVector
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullParameterTestforStaticSolveVector2()
    {
      ComplexDoubleVector X = ComplexDoubleSymmetricLevinson.Solve(T5, null as ComplexDoubleVector);
    }

    #endregion Null Parameter Test 2 for Static SolveVector
    
    #region Row Mismatch Test for Static SolveVector

    // test mismatching dimensions
    [Test]
    [ExpectedException(typeof(RankException))]
    public void RowMismatchTestforStaticSolveVector()
    {
      ComplexDoubleVector X = ComplexDoubleSymmetricLevinson.Solve(T5, Y4);
    }
    
    #endregion Row Mismatch Test for Static SolveVector
    
    #region Singular Test for Static SolveVector
    
    // test with Toeplitz matrix which has a singular principal sub-matrix
    [Test]
    [ExpectedException(typeof(SingularMatrixException))]
    public void SingularTestforStaticSolveVector()
    {
      ComplexDoubleVector T = new ComplexDoubleVector(3);
      T[2] = T[1] = T[0] = new Complex(1.0, 1.0);
      ComplexDoubleVector X = ComplexDoubleSymmetricLevinson.Solve(T, T);
    }
    
    #endregion Singular Test for Static SolveVector
    
    #region Static Solve Vector 1
    
    [Test]
    public void StaticSolveVector1()
    {
      int i;
      double e, me;
      ComplexDoubleVector X = ComplexDoubleSymmetricLevinson.Solve(T1, Y1);
      
      // determine the maximum error
      me = 0.0;
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
      double e, me;
      ComplexDoubleVector X = ComplexDoubleSymmetricLevinson.Solve(T2, Y2);
      
      // determine the maximum error
      me = 0.0;
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
      double e, me;
      ComplexDoubleVector X = ComplexDoubleSymmetricLevinson.Solve(T3, Y3);
      
      // determine the maximum error
      me = 0.0;
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
      double e, me;
      ComplexDoubleVector X = ComplexDoubleSymmetricLevinson.Solve(T4, Y4);
      
      // determine the maximum error
      me = 0.0;
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
      double e, me;
      ComplexDoubleVector X = ComplexDoubleSymmetricLevinson.Solve(T5, Y5);
      
      // determine the maximum error
      me = 0.0;
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

    #region Null Parameter Test 1 for Static SolveMatrix

    // test null parameter
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullParameterTestforStaticSolveMatrix1()
    {
      ComplexDoubleMatrix X = ComplexDoubleSymmetricLevinson.Solve(null, L2);
    }
    
    #endregion Null Parameter Test 1 for Static SolveMatrix
    
    #region Null Parameter Test 2 for Static SolveMatrix
    
    // test null parameter
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullParameterTestforStaticSolveMatrix2()
    {
      ComplexDoubleMatrix Y = ComplexDoubleSymmetricLevinson.Solve(T2, null as ComplexDoubleMatrix);
    }
    
    #endregion Null Parameter Test 2 for Static SolveMatrix
    
    #region Row Mismatch Test for Static SolveMatrix

    // test mismatching dimensions
    [Test]
    [ExpectedException(typeof(RankException))]
    public void RowMismatchTestforStaticSolveMatrix()
    {
      ComplexDoubleMatrix X = ComplexDoubleSymmetricLevinson.Solve(T2, L3);
    }
    
    #endregion Row Mismatch Test for Static SolveMatrix
    
    #region Singular Test for Static SolveMatrix

    // test with Toeplitz matrix which has a singular principal sub-matrix
    [Test]
    [ExpectedException(typeof(SingularMatrixException))]
    public void SingularTestforStaticSolveMatrix()
    {
      ComplexDoubleVector T = new ComplexDoubleVector(3);
      T[2] = T[1] = T[0] = new Complex(1.0, 1.0);
      ComplexDoubleMatrix X = ComplexDoubleSymmetricLevinson.Solve(T, ComplexDoubleMatrix.CreateIdentity(3));
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
      ComplexDoubleMatrix I = ComplexDoubleSymmetricLevinson.Solve(T1, ComplexDoubleMatrix.CreateIdentity(1));

      // determine the maximum relative error
      me = 0.0;
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

    // calculate inverse by solving linear system with identity RHS
    [Test]
    public void StaticSolveMatrix2()
    {
      int i, j;
      double e, me;

      // calculate inverse by solving the linear system
      ComplexDoubleMatrix I = ComplexDoubleSymmetricLevinson.Solve(T2, ComplexDoubleMatrix.CreateIdentity(2));

      // determine the maximum relative error
      me = 0.0;
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

    // calculate inverse by solving linear system with identity RHS
    [Test]
    public void StaticSolveMatrix3()
    {
      int i, j;
      double e, me;

      // calculate inverse by solving the linear system
      ComplexDoubleMatrix I = ComplexDoubleSymmetricLevinson.Solve(T3, ComplexDoubleMatrix.CreateIdentity(3));

      // determine the maximum relative error
      me = 0.0;
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

    // calculate inverse by solving linear system with identity RHS
    [Test]
    public void StaticSolveMatrix4()
    {
      int i, j;
      double e, me;

      // calculate inverse by solving the linear system
      ComplexDoubleMatrix I = ComplexDoubleSymmetricLevinson.Solve(T4, ComplexDoubleMatrix.CreateIdentity(4));

      // determine the maximum relative error
      me = 0.0;
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

    // calculate inverse by solving linear system with identity RHS
    [Test]
    public void StaticSolveMatrix5()
    {
      int i, j;
      double e, me;

      // calculate inverse by solving the linear system
      ComplexDoubleMatrix I = ComplexDoubleSymmetricLevinson.Solve(T5, ComplexDoubleMatrix.CreateIdentity(5));

      // determine the maximum relative error
      me = 0.0;
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

    #region Null Parameter Test for Static YuleWalker
    
    // test Yule-Walker with a null reference
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullParameterTestforStaticYuleWalker()
    {
      ComplexDoubleVector Y = ComplexDoubleSymmetricLevinson.YuleWalker(null);
    }
    
    #endregion Null Parameter Test for Static YuleWalker
    
    #region Row Test for Static YuleWalker

    // test Yule-Walker with order 1 matrix
    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void RowTestforStaticYuleWalker()
    {
      ComplexDoubleVector T = new ComplexDoubleVector(1);
      T[0] = new Complex(1.0, 1.0);
      ComplexDoubleVector Y = ComplexDoubleSymmetricLevinson.YuleWalker(T);
    }
    
    #endregion Row Test for Static YuleWalker
    
    #region Singular Test for Static YuleWalker
    
    // test Yule-Walker with matrix with singular principal sub-matrix
    [Test]
    [ExpectedException(typeof(SingularMatrixException))]
    public void SingularTestforStaticYuleWalker()
    {
      ComplexDoubleVector T = new ComplexDoubleVector(3);
      T[2] = T[1] = T[0] = new Complex(1.0, 1.0);
      ComplexDoubleVector Y = ComplexDoubleSymmetricLevinson.YuleWalker(T);
    }
    
    #endregion Singular Test for Static YuleWalker
    
    #region Static Yule Walker 2
    
    [Test]
    public void StaticYuleWalker2()
    {
      int i;
      double e, me;
      int N = T2.Length;
      ComplexDoubleVector A = ComplexDoubleSymmetricLevinson.YuleWalker(T2);

      // determine the maximum error
      me = 0.0;
      for (i = 0; i < A.Length; i++)
      {
        e = ComplexMath.Absolute((L2[N - 1, N - 2 - i] - A[i]) / L2[N - 1, N - 2 - i]);
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
      ComplexDoubleVector A = ComplexDoubleSymmetricLevinson.YuleWalker(T3);

      // determine the maximum error
      me = 0.0;
      for (i = 0; i < A.Length; i++)
      {
        e = ComplexMath.Absolute((L3[N - 1, N - 2 - i] - A[i]) / L3[N - 1, N - 2 - i]);
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
      ComplexDoubleVector A = ComplexDoubleSymmetricLevinson.YuleWalker(T4);

      // determine the maximum error
      me = 0.0;
      for (i = 0; i < A.Length; i++)
      {
        e = ComplexMath.Absolute((L4[N - 1, N - 2 - i] - A[i]) / L4[N - 1, N - 2 - i]);
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
      ComplexDoubleVector A = ComplexDoubleSymmetricLevinson.YuleWalker(T5);

      // determine the maximum error
      me = 0.0;
      for (i = 0; i < A.Length; i++)
      {
        e = ComplexMath.Absolute((L5[N - 1, N - 2 - i] - A[i]) / L5[N - 1, N - 2 - i]);
        if (e > me)
        {
          me = e;
        }
      }
      Assert.IsTrue(me < Tolerance5, "Maximum Error = " + me.ToString());
    }

    #endregion Static Yule Walker 5

    #region Null Prameter Test for Static Inverse

    [Test]
    [ExpectedException(typeof(System.ArgumentNullException))]
    public void NullPrameterTestforStaticInverse()
    {
      ComplexDoubleMatrix Y = ComplexDoubleSymmetricLevinson.Inverse(null);
    }

    #endregion Null Prameter Test for Static Inverse

    #region Singular Test for Static Inverse

    [Test]
    [ExpectedException(typeof(SingularMatrixException))]
    public void SingularTestforStaticInverse()
    {

      // setup an ill-conditioned system (second order principal submatrix is singular)
      ComplexDoubleVector T = new ComplexDoubleVector(3);
      T[0] = Complex.One;
      T[1] = Complex.One;
      T[2] = Complex.One;

      ComplexDoubleMatrix Y = ComplexDoubleSymmetricLevinson.Inverse(T);
    }

    #endregion Singular Test for Static Inverse

    #region Static Inverse 1

    [Test]
    public void StaticInverse1()
    {
      int i, j;
      double e, me;

      // calculate the inverse
      ComplexDoubleMatrix I = ComplexDoubleSymmetricLevinson.Inverse(T1);

      // determine the maximum relative error
      me = 0.0;
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
      double e, me;

      // calculate the inverse
      ComplexDoubleMatrix I = ComplexDoubleSymmetricLevinson.Inverse(T2);

      // determine the maximum relative error
      me = 0.0;
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
      double e, me;

      // calculate the inverse
      ComplexDoubleMatrix I = ComplexDoubleSymmetricLevinson.Inverse(T3);

      // determine the maximum relative error
      me = 0.0;
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
      double e, me;

      // calculate the inverse
      ComplexDoubleMatrix I = ComplexDoubleSymmetricLevinson.Inverse(T4);

      // determine the maximum relative error
      me = 0.0;
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
      double e, me;

      // calculate the inverse
      ComplexDoubleMatrix I = ComplexDoubleSymmetricLevinson.Inverse(T5);

      // determine the maximum relative error
      me = 0.0;
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

  }
  
}

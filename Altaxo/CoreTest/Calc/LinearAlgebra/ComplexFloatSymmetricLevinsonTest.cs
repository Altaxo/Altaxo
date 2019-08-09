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

#region Using Directives

using System;
using Altaxo.Calc;
using Altaxo.Calc.LinearAlgebra;
using NUnit.Framework;

#endregion Using Directives

namespace AltaxoTest.Calc.LinearAlgebra
{
  // suite of tests for complex float symmetric algorithm
  [TestFixture]
  public class ComplexFloatSymmetricLevinsonTest
  {
    #region Fields

    // unit testing - order 1

    private ComplexFloatVector T1;      // Toeplitz matrix
    private ComplexFloatMatrix L1;      // Lower triangle matrix
    private ComplexFloatVector D1;      // diagonal vector
    private ComplexFloatMatrix I1;      // inverse matrix
    private ComplexFloat Det1;        // exact determinant
    private ComplexFloatVector X1;      // RHS vector
    private ComplexFloatVector Y1;      // LHS vector
    private float Tolerance1;       // allowable tolerance

    // unit testing - order 2

    private ComplexFloatVector T2;      // Toeplitz matrix
    private ComplexFloatMatrix L2;      // Lower triangle matrix
    private ComplexFloatVector D2;      // diagonal vector
    private ComplexFloatMatrix I2;      // inverse matrix
    private ComplexFloat Det2;        // exact determinant
    private ComplexFloatVector X2;      // RHS vector
    private ComplexFloatVector Y2;      // LHS vector
    private float Tolerance2;       // allowable tolerance

    // unit testing - order 3

    private ComplexFloatVector T3;      // Toeplitz matrix
    private ComplexFloatMatrix L3;      // Lower triangle matrix
    private ComplexFloatVector D3;      // diagonal vector
    private ComplexFloatMatrix I3;      // inverse matrix
    private ComplexFloat Det3;        // exact determinant
    private ComplexFloatVector X3;      // RHS vector
    private ComplexFloatVector Y3;      // LHS vector
    private float Tolerance3;       // allowable tolerance

    // unit testing - order 4

    private ComplexFloatVector T4;      // Toeplitz matrix
    private ComplexFloatMatrix L4;      // Lower triangle matrix
    private ComplexFloatVector D4;      // diagonal vector
    private ComplexFloatMatrix I4;      // inverse matrix
    private ComplexFloat Det4;        // exact determinant
    private ComplexFloatVector X4;      // RHS vector
    private ComplexFloatVector Y4;      // LHS vector
    private float Tolerance4;       // allowable tolerance

    // unit testing - order 5

    private ComplexFloatVector T5;      // Toeplitz matrix
    private ComplexFloatMatrix L5;      // Lower triangle matrix
    private ComplexFloatVector D5;      // diagonal vector
    private ComplexFloatMatrix I5;      // inverse matrix
    private ComplexFloat Det5;        // exact determinant
    private ComplexFloatVector X5;      // RHS vector
    private ComplexFloatVector Y5;      // LHS vector
    private float Tolerance5;       // allowable tolerance

    #endregion Fields

    #region Text Fixture Setup

    [OneTimeSetUp]
    public void SetupTestCases()
    {
      // unit testing values - order 1

      T1 = new ComplexFloatVector(1)
      {
        [0] = new ComplexFloat(+1.0000000E+000f, +1.0000000E+000f)
      };

      L1 = new ComplexFloatMatrix(1)
      {
        [0, 0] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f)
      };

      D1 = new ComplexFloatVector(1)
      {
        [0] = new ComplexFloat(+5.0000000E-001f, -5.0000000E-001f)
      };

      Det1 = new ComplexFloat(+1.0000000E+000f, +1.0000000E+000f);

      I1 = new ComplexFloatMatrix(1)
      {
        [0, 0] = new ComplexFloat(+5.0000000E-001f, -5.0000000E-001f)
      };

      X1 = new ComplexFloatVector(1)
      {
        [0] = new ComplexFloat(+1.0000000E+000f, -1.0000000E+000f)
      };

      Y1 = new ComplexFloatVector(1)
      {
        [0] = new ComplexFloat(+2.0000000E+000f, +0.0000000E+000f)
      };

      Tolerance1 = 5.0e-7F;

      // unit testing values - order 2

      T2 = new ComplexFloatVector(2)
      {
        [0] = new ComplexFloat(+1.0000000E+000f, +1.0000000E+000f),
        [1] = new ComplexFloat(+5.0000000E-001f, +5.0000000E-001f)
      };

      L2 = new ComplexFloatMatrix(2)
      {
        [0, 0] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f),
        [1, 0] = new ComplexFloat(-5.0000000E-001f, +0.0000000E+000f),
        [1, 1] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f)
      };

      D2 = new ComplexFloatVector(2)
      {
        [0] = new ComplexFloat(+5.0000000E-001f, -5.0000000E-001f),
        [1] = new ComplexFloat(+6.6666669E-001f, -6.6666669E-001f)
      };

      Det2 = new ComplexFloat(+0.0000000E+000f, +1.5000000E+000f);

      I2 = new ComplexFloatMatrix(2)
      {
        [0, 0] = new ComplexFloat(+6.6666669E-001f, -6.6666669E-001f),
        [0, 1] = new ComplexFloat(-3.3333334E-001f, +3.3333334E-001f),
        [1, 0] = new ComplexFloat(-3.3333334E-001f, +3.3333334E-001f),
        [1, 1] = new ComplexFloat(+6.6666669E-001f, -6.6666669E-001f)
      };

      X2 = new ComplexFloatVector(2)
      {
        [0] = new ComplexFloat(+1.0000000E+000f, -1.0000000E+000f),
        [1] = new ComplexFloat(+2.0000000E+000f, +2.0000000E+000f)
      };

      Y2 = new ComplexFloatVector(2)
      {
        [0] = new ComplexFloat(+2.0000000E+000f, +2.0000000E+000f),
        [1] = new ComplexFloat(+1.0000000E+000f, +4.0000000E+000f)
      };

      Tolerance2 = 5.0e-7F;

      // unit testing values - order 3

      T3 = new ComplexFloatVector(3)
      {
        [0] = new ComplexFloat(+2.0000000E+000f, +2.0000000E+000f),
        [1] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f),
        [2] = new ComplexFloat(+5.0000000E-001f, +0.0000000E+000f)
      };

      L3 = new ComplexFloatMatrix(3)
      {
        [0, 0] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f),
        [1, 0] = new ComplexFloat(-2.5000000E-001f, +2.5000000E-001f),
        [1, 1] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f),
        [2, 0] = new ComplexFloat(-1.2307692E-001f, +1.5384615E-002f),
        [2, 1] = new ComplexFloat(-2.2307692E-001f, +2.1538462E-001f),
        [2, 2] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f)
      };

      D3 = new ComplexFloatVector(3)
      {
        [0] = new ComplexFloat(+2.5000000E-001f, -2.5000000E-001f),
        [1] = new ComplexFloat(+2.1538462E-001f, -2.7692309E-001f),
        [2] = new ComplexFloat(+2.1756098E-001f, -2.8195122E-001f)
      };

      Det3 = new ComplexFloat(-1.9500000E+001f, +1.1500000E+001f);

      I3 = new ComplexFloatMatrix(3)
      {
        [0, 0] = new ComplexFloat(+2.1756098E-001f, -2.8195122E-001f),
        [0, 1] = new ComplexFloat(+1.2195121E-002f, +1.0975610E-001f),
        [0, 2] = new ComplexFloat(-2.2439023E-002f, +3.8048781E-002f),
        [1, 0] = new ComplexFloat(+1.2195121E-002f, +1.0975610E-001f),
        [1, 1] = new ComplexFloat(+1.8902439E-001f, -2.9878050E-001f),
        [1, 2] = new ComplexFloat(+1.2195121E-002f, +1.0975610E-001f),
        [2, 0] = new ComplexFloat(-2.2439023E-002f, +3.8048781E-002f),
        [2, 1] = new ComplexFloat(+1.2195121E-002f, +1.0975610E-001f),
        [2, 2] = new ComplexFloat(+2.1756098E-001f, -2.8195122E-001f)
      };

      X3 = new ComplexFloatVector(3)
      {
        [0] = new ComplexFloat(+1.0000000E+000f, -1.0000000E+000f),
        [1] = new ComplexFloat(+2.0000000E+000f, +2.0000000E+000f),
        [2] = new ComplexFloat(+3.0000000E+000f, -3.0000000E+000f)
      };

      Y3 = new ComplexFloatVector(3)
      {
        [0] = new ComplexFloat(+7.5000000E+000f, +5.0000000E-001f),
        [1] = new ComplexFloat(+4.0000000E+000f, +4.0000000E+000f),
        [2] = new ComplexFloat(+1.4500000E+001f, +1.5000000E+000f)
      };

      Tolerance3 = 5.0e-7F;

      // unit testing values - order 4

      T4 = new ComplexFloatVector(4)
      {
        [0] = new ComplexFloat(+4.0000000E+000f, +0.0000000E+000f),
        [1] = new ComplexFloat(+1.2000000E+001f, -1.3333334E+000f),
        [2] = new ComplexFloat(+2.6666666E+001f, +2.1333334E+001f),
        [3] = new ComplexFloat(+4.8000000E+001f, -5.3333335E+000f)
      };

      L4 = new ComplexFloatMatrix(4)
      {
        [0, 0] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f),
        [1, 0] = new ComplexFloat(-3.0000000E+000f, +3.3333334E-001f),
        [1, 1] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f),
        [2, 0] = new ComplexFloat(-4.8611370E-001f, +8.0633736E-001f),
        [2, 1] = new ComplexFloat(-1.8104380E+000f, -2.2477167E+000f),
        [2, 2] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f),
        [3, 0] = new ComplexFloat(+3.3484605E-001f, -1.9271448E+000f),
        [3, 1] = new ComplexFloat(-5.4240074E+000f, +3.5426745E+000f),
        [3, 2] = new ComplexFloat(-4.1928238E-001f, -1.0409063E+000f),
        [3, 3] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f)
      };

      D4 = new ComplexFloatVector(4)
      {
        [0] = new ComplexFloat(+2.5000000E-001f, +0.0000000E+000f),
        [1] = new ComplexFloat(-2.9776327E-002f, -7.5489283E-003f),
        [2] = new ComplexFloat(-1.8372282E-002f, +4.8476043E-003f),
        [3] = new ComplexFloat(-3.4274173E-003f, +2.0146633E-003f)
      };

      Det4 = new ComplexFloat(-1.4775024E+006f, -8.8785225E+005f);

      I4 = new ComplexFloatMatrix(4)
      {
        [0, 0] = new ComplexFloat(-3.4274173E-003f, +2.0146633E-003f),
        [0, 1] = new ComplexFloat(+3.5341315E-003f, +2.7229076E-003f),
        [0, 2] = new ComplexFloat(+1.1453041E-002f, -2.3069773E-002f),
        [0, 3] = new ComplexFloat(+2.7348907E-003f, +7.2797318E-003f),
        [1, 0] = new ComplexFloat(+3.5341315E-003f, +2.7229076E-003f),
        [1, 1] = new ComplexFloat(-1.7019790E-002f, +2.7237507E-005f),
        [1, 2] = new ComplexFloat(+1.5342389E-002f, +3.0270604E-002f),
        [1, 3] = new ComplexFloat(+1.1453041E-002f, -2.3069773E-002f),
        [2, 0] = new ComplexFloat(+1.1453041E-002f, -2.3069773E-002f),
        [2, 1] = new ComplexFloat(+1.5342389E-002f, +3.0270604E-002f),
        [2, 2] = new ComplexFloat(-1.7019790E-002f, +2.7237507E-005f),
        [2, 3] = new ComplexFloat(+3.5341315E-003f, +2.7229076E-003f),
        [3, 0] = new ComplexFloat(+2.7348907E-003f, +7.2797318E-003f),
        [3, 1] = new ComplexFloat(+1.1453041E-002f, -2.3069773E-002f),
        [3, 2] = new ComplexFloat(+3.5341315E-003f, +2.7229076E-003f),
        [3, 3] = new ComplexFloat(-3.4274173E-003f, +2.0146633E-003f)
      };

      X4 = new ComplexFloatVector(4)
      {
        [0] = new ComplexFloat(+1.0000000E+000f, -1.0000000E+000f),
        [1] = new ComplexFloat(+2.0000000E+000f, +2.0000000E+000f),
        [2] = new ComplexFloat(+3.0000000E+000f, -3.0000000E+000f),
        [3] = new ComplexFloat(+4.0000000E+000f, -4.0000000E+000f)
      };

      Y4 = new ComplexFloatVector(4)
      {
        [0] = new ComplexFloat(+3.4533334E+002f, -2.1200000E+002f),
        [1] = new ComplexFloat(+2.4266667E+002f, -6.6666664E+001f),
        [2] = new ComplexFloat(+1.2933333E+002f, -4.9333332E+001f),
        [3] = new ComplexFloat(+1.0133334E+002f, -1.3333333E+001f)
      };

      Tolerance4 = 8.0E-06f;

      // unit testing values - order 5

      T5 = new ComplexFloatVector(5)
      {
        [0] = new ComplexFloat(+5.0000000E+000f, +0.0000000E+000f),
        [1] = new ComplexFloat(+0.0000000E+000f, +4.0000000E+000f),
        [2] = new ComplexFloat(+3.0000000E+000f, +0.0000000E+000f),
        [3] = new ComplexFloat(+0.0000000E+000f, +2.0000000E+000f),
        [4] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f)
      };

      L5 = new ComplexFloatMatrix(5)
      {
        [0, 0] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f),
        [1, 0] = new ComplexFloat(+0.0000000E+000f, -8.0000001E-001f),
        [1, 1] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f),
        [2, 0] = new ComplexFloat(-7.5609756E-001f, +0.0000000E+000f),
        [2, 1] = new ComplexFloat(+0.0000000E+000f, -1.9512194E-001f),
        [2, 2] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f),
        [3, 0] = new ComplexFloat(+0.0000000E+000f, +4.5833334E-001f),
        [3, 1] = new ComplexFloat(-6.6666669E-001f, +0.0000000E+000f),
        [3, 2] = new ComplexFloat(+0.0000000E+000f, -5.4166669E-001f),
        [3, 3] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f),
        [4, 0] = new ComplexFloat(+4.1176471E-001f, +0.0000000E+000f),
        [4, 1] = new ComplexFloat(+0.0000000E+000f, +2.3529412E-001f),
        [4, 2] = new ComplexFloat(-9.4117647E-001f, +0.0000000E+000f),
        [4, 3] = new ComplexFloat(+0.0000000E+000f, -3.5294119E-001f),
        [4, 4] = new ComplexFloat(+1.0000000E+000f, +0.0000000E+000f)
      };

      D5 = new ComplexFloatVector(5)
      {
        [0] = new ComplexFloat(+2.0000000E-001f, +0.0000000E+000f),
        [1] = new ComplexFloat(+1.2195122E-001f, +0.0000000E+000f),
        [2] = new ComplexFloat(+2.8472221E-001f, +0.0000000E+000f),
        [3] = new ComplexFloat(+2.3529412E-001f, +0.0000000E+000f),
        [4] = new ComplexFloat(+2.8333333E-001f, +0.0000000E+000f)
      };

      Det5 = new ComplexFloat(+2.1600000E+003f, +0.0000000E+000f);

      I5 = new ComplexFloatMatrix(5)
      {
        [0, 0] = new ComplexFloat(+2.8333333E-001f, +0.0000000E+000f),
        [0, 1] = new ComplexFloat(+0.0000000E+000f, -1.0000000E-001f),
        [0, 2] = new ComplexFloat(-2.6666668E-001f, +0.0000000E+000f),
        [0, 3] = new ComplexFloat(+0.0000000E+000f, +6.6666670E-002f),
        [0, 4] = new ComplexFloat(+1.1666667E-001f, +0.0000000E+000f),
        [1, 0] = new ComplexFloat(+0.0000000E+000f, -1.0000000E-001f),
        [1, 1] = new ComplexFloat(+2.0000000E-001f, +0.0000000E+000f),
        [1, 2] = new ComplexFloat(+0.0000000E+000f, -3.3333335E-002f),
        [1, 3] = new ComplexFloat(-1.3333334E-001f, +0.0000000E+000f),
        [1, 4] = new ComplexFloat(+0.0000000E+000f, +6.6666670E-002f),
        [2, 0] = new ComplexFloat(-2.6666668E-001f, +0.0000000E+000f),
        [2, 1] = new ComplexFloat(+0.0000000E+000f, -3.3333335E-002f),
        [2, 2] = new ComplexFloat(+4.6666667E-001f, +0.0000000E+000f),
        [2, 3] = new ComplexFloat(+0.0000000E+000f, -3.3333335E-002f),
        [2, 4] = new ComplexFloat(-2.6666668E-001f, +0.0000000E+000f),
        [3, 0] = new ComplexFloat(+0.0000000E+000f, +6.6666670E-002f),
        [3, 1] = new ComplexFloat(-1.3333334E-001f, +0.0000000E+000f),
        [3, 2] = new ComplexFloat(+0.0000000E+000f, -3.3333335E-002f),
        [3, 3] = new ComplexFloat(+2.0000000E-001f, +0.0000000E+000f),
        [3, 4] = new ComplexFloat(+0.0000000E+000f, -1.0000000E-001f),
        [4, 0] = new ComplexFloat(+1.1666667E-001f, +0.0000000E+000f),
        [4, 1] = new ComplexFloat(+0.0000000E+000f, +6.6666670E-002f),
        [4, 2] = new ComplexFloat(-2.6666668E-001f, +0.0000000E+000f),
        [4, 3] = new ComplexFloat(+0.0000000E+000f, -1.0000000E-001f),
        [4, 4] = new ComplexFloat(+2.8333333E-001f, +0.0000000E+000f)
      };

      X5 = new ComplexFloatVector(5)
      {
        [0] = new ComplexFloat(+1.0000000E+000f, -1.0000000E+000f),
        [1] = new ComplexFloat(+2.0000000E+000f, +2.0000000E+000f),
        [2] = new ComplexFloat(+3.0000000E+000f, -3.0000000E+000f),
        [3] = new ComplexFloat(+4.0000000E+000f, -4.0000000E+000f),
        [4] = new ComplexFloat(+5.0000000E+000f, +5.0000000E+000f)
      };

      Y5 = new ComplexFloatVector(5)
      {
        [0] = new ComplexFloat(+1.9000000E+001f, +7.0000000E+000f),
        [1] = new ComplexFloat(+2.8000000E+001f, +2.4000000E+001f),
        [2] = new ComplexFloat(+4.1000000E+001f, +2.1000000E+001f),
        [3] = new ComplexFloat(+2.0000000E+001f, +2.0000000E+001f),
        [4] = new ComplexFloat(+4.7000000E+001f, +3.5000000E+001f)
      };

      Tolerance5 = 8.0E-06f;
    }

    #endregion Text Fixture Setup

    #region Null Parameter Tests for Constructor

    // Test constructor with a null parameter
    [Test]
    public void NullParameterTestforConstructor()
    {
      Assert.Throws(typeof(System.ArgumentNullException), () =>
      {
        var cdsl = new ComplexFloatSymmetricLevinson(null as ComplexFloatVector);
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
        var cdv = new ComplexFloatVector(1, 0.0f);
        cdv.RemoveAt(0);
        var cdsl = new ComplexFloatSymmetricLevinson(cdv);
      });
    }

    #endregion Zero Length Vector Tests for Constructor

    #region GetVector Member Test

    // check get vector
    [Test]
    public void GetVectorMemberTest()
    {
      var cdsl = new ComplexFloatSymmetricLevinson(T5);
      ComplexFloatVector TT = cdsl.GetVector();
      Assert.IsTrue(T5.Equals(TT));
    }

    #endregion GetVector Member Test

    #region GetMatrix Member Test

    // check get matrix
    [Test]
    public void GetMatrixMemberTest()
    {
      var cdsl = new ComplexFloatSymmetricLevinson(T5);
      ComplexFloatMatrix cdsldm = cdsl.GetMatrix();
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
      var cdsl = new ComplexFloatSymmetricLevinson(T5);
      Assert.IsTrue(cdsl.Order == 5);
    }

    #endregion Order Property Test

    #region Decomposition Test 1

    // test the UDL factorisation for case 1
    [Test]
    public void DecompositionTest1()
    {
      int i, j;
      float e, me;
      var cdsl = new ComplexFloatSymmetricLevinson(T1);
      ComplexFloatMatrix U = cdsl.U;
      ComplexFloatMatrix D = cdsl.D;
      ComplexFloatMatrix L = cdsl.L;

      // check U is the transpose of L
      Assert.IsTrue(U.Equals(L.GetTranspose()));

      // check the lower triangle
      me = 0.0f;
      for (i = 0; i < cdsl.Order; i++)
      {
        for (j = 0; j <= i; j++)
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
      me = 0.0f;
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
      float e, me;
      var cdsl = new ComplexFloatSymmetricLevinson(T2);
      ComplexFloatMatrix U = cdsl.U;
      ComplexFloatMatrix D = cdsl.D;
      ComplexFloatMatrix L = cdsl.L;

      // check U is the transpose of L
      Assert.IsTrue(U.Equals(L.GetTranspose()));

      // check the lower triangle
      me = 0.0f;
      for (i = 0; i < cdsl.Order; i++)
      {
        for (j = 0; j <= i; j++)
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
      me = 0.0f;
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
      float e, me;
      var cdsl = new ComplexFloatSymmetricLevinson(T3);
      ComplexFloatMatrix U = cdsl.U;
      ComplexFloatMatrix D = cdsl.D;
      ComplexFloatMatrix L = cdsl.L;

      // check U is the transpose of L
      Assert.IsTrue(U.Equals(L.GetTranspose()));

      // check the lower triangle
      me = 0.0f;
      for (i = 0; i < cdsl.Order; i++)
      {
        for (j = 0; j <= i; j++)
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
      me = 0.0f;
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
      float e, me;
      var cdsl = new ComplexFloatSymmetricLevinson(T4);
      ComplexFloatMatrix U = cdsl.U;
      ComplexFloatMatrix D = cdsl.D;
      ComplexFloatMatrix L = cdsl.L;

      // check U is the transpose of L
      Assert.IsTrue(U.Equals(L.GetTranspose()));

      // check the lower triangle
      me = 0.0f;
      for (i = 0; i < cdsl.Order; i++)
      {
        for (j = 0; j <= i; j++)
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
      me = 0.0f;
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
      float e, me;
      var cdsl = new ComplexFloatSymmetricLevinson(T5);
      ComplexFloatMatrix U = cdsl.U;
      ComplexFloatMatrix D = cdsl.D;
      ComplexFloatMatrix L = cdsl.L;

      // check U is the transpose of L
      Assert.IsTrue(U.Equals(L.GetTranspose()));

      // check the lower triangle
      me = 0.0f;
      for (i = 0; i < cdsl.Order; i++)
      {
        for (j = 0; j <= i; j++)
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
      me = 0.0f;
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
      var cdsl = new ComplexFloatSymmetricLevinson(T1);
      Assert.IsFalse(cdsl.IsSingular);
    }

    #endregion Singularity Property Test 1

    #region Singularity Property Test 2

    // check that singular matrix is detected
    [Test]
    public void SingularityPropertyTest2()
    {
      var cdsl = new ComplexFloatSymmetricLevinson(T2);
      Assert.IsFalse(cdsl.IsSingular);
    }

    #endregion Singularity Property Test 2

    #region Singularity Property Test 3

    // check that singular matrix is detected
    [Test]
    public void SingularityPropertyTest3()
    {
      var cdsl = new ComplexFloatSymmetricLevinson(T3);
      Assert.IsFalse(cdsl.IsSingular);
    }

    #endregion Singularity Property Test 3

    #region Singularity Property Test 4

    // check that singular matrix is detected
    [Test]
    public void SingularityPropertyTest4()
    {
      var cdsl = new ComplexFloatSymmetricLevinson(T4);
      Assert.IsFalse(cdsl.IsSingular);
    }

    #endregion Singularity Property Test 4

    #region Singularity Property Test 5

    // check that singular matrix is detected
    [Test]
    public void SingularityPropertyTest5()
    {
      var cdsl = new ComplexFloatSymmetricLevinson(T5);
      Assert.IsFalse(cdsl.IsSingular);
    }

    #endregion Singularity Property Test 5

    #region Singularity Property Test - Negative Case

    // check that singular matrix is detected
    [Test]
    public void SingularityPropertyTest()
    {
      var T = new ComplexFloatVector(10);
      for (int i = 1; i < 10; i++)
      {
        T[i] = new ComplexFloat(i + 1, i + 1);
      }
      T[0] = new ComplexFloat(2.0f, 2.0f);

      var cdsl = new ComplexFloatSymmetricLevinson(T);
      Assert.IsTrue(cdsl.IsSingular);
    }

    #endregion Singularity Property Test - Negative Case

    #region GetDeterminant Method Test 1

    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest1()
    {
      // calculate determinant from diagonal
      var cdsl = new ComplexFloatSymmetricLevinson(T1);

      // check results match
      float e = ComplexMath.Absolute((cdsl.GetDeterminant() - Det1) / Det1);
      Assert.IsTrue(e < Tolerance1);
    }

    #endregion GetDeterminant Method Test 1

    #region GetDeterminant Method Test 2

    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest2()
    {
      // calculate determinant from diagonal
      var cdsl = new ComplexFloatSymmetricLevinson(T2);

      // check results match
      float e = ComplexMath.Absolute((cdsl.GetDeterminant() - Det2) / Det2);
      Assert.IsTrue(e < Tolerance2);
    }

    #endregion GetDeterminant Method Test 2

    #region GetDeterminant Method Test 3

    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest3()
    {
      // calculate determinant from diagonal
      var cdsl = new ComplexFloatSymmetricLevinson(T3);

      // check results match
      float e = ComplexMath.Absolute((cdsl.GetDeterminant() - Det3) / Det3);
      Assert.IsTrue(e < Tolerance3);
    }

    #endregion GetDeterminant Method Test 3

    #region GetDeterminant Method Test 4

    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest4()
    {
      // calculate determinant from diagonal
      var cdsl = new ComplexFloatSymmetricLevinson(T4);

      // check results match
      float e = ComplexMath.Absolute((cdsl.GetDeterminant() - Det4) / Det4);
      Assert.IsTrue(e < Tolerance4);
    }

    #endregion GetDeterminant Method Test 4

    #region GetDeterminant Method Test 5

    // Test the Determinant
    [Test]
    public void GetDeterminantMethodTest5()
    {
      // calculate determinant from diagonal
      var cdsl = new ComplexFloatSymmetricLevinson(T5);

      // check results match
      float e = ComplexMath.Absolute((cdsl.GetDeterminant() - Det5) / Det5);
      Assert.IsTrue(e < Tolerance5);
    }

    #endregion GetDeterminant Method Test 5

    #region Null Parameter Test for SolveVector

    [Test]
    public void NullParameterTestforSolveVector()
    {
      Assert.Throws(typeof(System.ArgumentNullException), () =>
      {
        var cdsl = new ComplexFloatSymmetricLevinson(T5);
        ComplexFloatVector X = cdsl.Solve(null as ComplexFloatVector);
      });
    }

    #endregion Null Parameter Test for SolveVector

    #region Mismatch Rows Test for SolveVector

    [Test]
    public void MismatchRowsTestforSolveVector()
    {
      Assert.Throws(typeof(System.RankException), () =>
      {
        var cdsl = new ComplexFloatSymmetricLevinson(T4);
        ComplexFloatVector X = cdsl.Solve(X5);
      });
    }

    #endregion Mismatch Rows Test for SolveVector

    #region SolveVector 1

    // Test solving a linear system
    [Test]
    public void SolveVector1()
    {
      int i;
      float e, me;
      var cdsl = new ComplexFloatSymmetricLevinson(T1);
      ComplexFloatVector X = cdsl.Solve(Y1);

      // determine the maximum error
      me = 0.0f;
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
      float e, me;
      var cdsl = new ComplexFloatSymmetricLevinson(T2);
      ComplexFloatVector X = cdsl.Solve(Y2);

      // determine the maximum error
      me = 0.0f;
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
      float e, me;
      var cdsl = new ComplexFloatSymmetricLevinson(T3);
      ComplexFloatVector X = cdsl.Solve(Y3);

      // determine the maximum error
      me = 0.0f;
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
      float e, me;
      var cdsl = new ComplexFloatSymmetricLevinson(T4);
      ComplexFloatVector X = cdsl.Solve(Y4);

      // determine the maximum error
      me = 0.0f;
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
      float e, me;
      var cdsl = new ComplexFloatSymmetricLevinson(T5);
      ComplexFloatVector X = cdsl.Solve(Y5);

      // determine the maximum error
      me = 0.0f;
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
    public void NullParameterTestforSolveMatrix()
    {
      Assert.Throws(typeof(System.ArgumentNullException), () =>
      {
        var cdsl = new ComplexFloatSymmetricLevinson(T5);
        ComplexFloatMatrix X = cdsl.Solve(null as ComplexFloatMatrix);
      });
    }

    #endregion Null Parameter Test for SolveMatrix

    #region Mismatch Rows Test for SolveMatrix

    [Test]
    public void MismatchRowsTestforSolveMatrix()
    {
      Assert.Throws(typeof(System.RankException), () =>
      {
        var cdsl = new ComplexFloatSymmetricLevinson(T4);
        ComplexFloatMatrix X = cdsl.Solve(I5);
      });
    }

    #endregion Mismatch Rows Test for SolveMatrix

    #region Solve Matrix 1

    // calculate inverse by solving linear equations with identity RHS
    [Test]
    public void SolveMatrix1()
    {
      int i, j;
      float e, me;
      var cdsl = new ComplexFloatSymmetricLevinson(T1);

      // check inverse
      ComplexFloatMatrix I = cdsl.Solve(ComplexFloatMatrix.CreateIdentity(1));
      me = 0.0f;
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
      float e, me;
      var cdsl = new ComplexFloatSymmetricLevinson(T2);

      // check inverse
      ComplexFloatMatrix I = cdsl.Solve(ComplexFloatMatrix.CreateIdentity(2));
      me = 0.0f;
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
      float e, me;
      var cdsl = new ComplexFloatSymmetricLevinson(T3);

      // check inverse
      ComplexFloatMatrix I = cdsl.Solve(ComplexFloatMatrix.CreateIdentity(3));
      me = 0.0f;
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
      float e, me;
      var cdsl = new ComplexFloatSymmetricLevinson(T4);

      // check inverse
      ComplexFloatMatrix I = cdsl.Solve(ComplexFloatMatrix.CreateIdentity(4));
      me = 0.0f;
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
      float e, me;
      var cdsl = new ComplexFloatSymmetricLevinson(T5);

      // check inverse
      ComplexFloatMatrix I = cdsl.Solve(ComplexFloatMatrix.CreateIdentity(5));
      me = 0.0f;
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
      float e, me;
      var cdsl = new ComplexFloatSymmetricLevinson(T1);

      // check inverse
      ComplexFloatMatrix I = cdsl.GetInverse();
      me = 0.0f;
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
      float e, me;
      var cdsl = new ComplexFloatSymmetricLevinson(T2);

      // check inverse
      ComplexFloatMatrix I = cdsl.GetInverse();
      me = 0.0f;
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
      float e, me;
      var cdsl = new ComplexFloatSymmetricLevinson(T3);

      // check inverse
      ComplexFloatMatrix I = cdsl.GetInverse();
      me = 0.0f;
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
      float e, me;
      var cdsl = new ComplexFloatSymmetricLevinson(T4);

      // check inverse
      ComplexFloatMatrix I = cdsl.GetInverse();
      me = 0.0f;
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
      float e, me;
      var cdsl = new ComplexFloatSymmetricLevinson(T5);

      // check inverse
      ComplexFloatMatrix I = cdsl.GetInverse();
      me = 0.0f;
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
    public void NullParameterTestforStaticSolveVector1()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        ComplexFloatVector X = ComplexFloatSymmetricLevinson.Solve(null, X5);
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
        ComplexFloatVector X = ComplexFloatSymmetricLevinson.Solve(T5, null as ComplexFloatVector);
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
        ComplexFloatVector X = ComplexFloatSymmetricLevinson.Solve(T5, Y4);
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
        var T = new ComplexFloatVector(3);
        T[2] = T[1] = T[0] = new ComplexFloat(1.0f, 1.0f);
        ComplexFloatVector X = ComplexFloatSymmetricLevinson.Solve(T, T);
      });
    }

    #endregion Singular Test for Static SolveVector

    #region Static Solve Vector 1

    [Test]
    public void StaticSolveVector1()
    {
      int i;
      float e, me;
      ComplexFloatVector X = ComplexFloatSymmetricLevinson.Solve(T1, Y1);

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
      ComplexFloatVector X = ComplexFloatSymmetricLevinson.Solve(T2, Y2);

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
      ComplexFloatVector X = ComplexFloatSymmetricLevinson.Solve(T3, Y3);

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
      ComplexFloatVector X = ComplexFloatSymmetricLevinson.Solve(T4, Y4);

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
      ComplexFloatVector X = ComplexFloatSymmetricLevinson.Solve(T5, Y5);

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

    #region Null Parameter Test 1 for Static SolveMatrix

    // test null parameter
    [Test]
    public void NullParameterTestforStaticSolveMatrix1()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        ComplexFloatMatrix X = ComplexFloatSymmetricLevinson.Solve(null, L2);
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
        ComplexFloatMatrix Y = ComplexFloatSymmetricLevinson.Solve(T2, null as ComplexFloatMatrix);
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
        ComplexFloatMatrix X = ComplexFloatSymmetricLevinson.Solve(T2, L3);
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
        var T = new ComplexFloatVector(3);
        T[2] = T[1] = T[0] = new ComplexFloat(1.0f, 1.0f);
        ComplexFloatMatrix X = ComplexFloatSymmetricLevinson.Solve(T, ComplexFloatMatrix.CreateIdentity(3));
      });
    }

    #endregion Singular Test for Static SolveMatrix

    #region Static Solve Matrix 1

    // calculate inverse by solving linear system with identity RHS
    [Test]
    public void StaticSolveMatrix1()
    {
      int i, j;
      float e, me;

      // calculate inverse by solving the linear system
      ComplexFloatMatrix I = ComplexFloatSymmetricLevinson.Solve(T1, ComplexFloatMatrix.CreateIdentity(1));

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

    #endregion Static Solve Matrix 1

    #region Static Solve Matrix 2

    // calculate inverse by solving linear system with identity RHS
    [Test]
    public void StaticSolveMatrix2()
    {
      int i, j;
      float e, me;

      // calculate inverse by solving the linear system
      ComplexFloatMatrix I = ComplexFloatSymmetricLevinson.Solve(T2, ComplexFloatMatrix.CreateIdentity(2));

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

    #endregion Static Solve Matrix 2

    #region Static Solve Matrix 3

    // calculate inverse by solving linear system with identity RHS
    [Test]
    public void StaticSolveMatrix3()
    {
      int i, j;
      float e, me;

      // calculate inverse by solving the linear system
      ComplexFloatMatrix I = ComplexFloatSymmetricLevinson.Solve(T3, ComplexFloatMatrix.CreateIdentity(3));

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

    #endregion Static Solve Matrix 3

    #region Static Solve Matrix 4

    // calculate inverse by solving linear system with identity RHS
    [Test]
    public void StaticSolveMatrix4()
    {
      int i, j;
      float e, me;

      // calculate inverse by solving the linear system
      ComplexFloatMatrix I = ComplexFloatSymmetricLevinson.Solve(T4, ComplexFloatMatrix.CreateIdentity(4));

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

    #endregion Static Solve Matrix 4

    #region Static Solve Matrix 5

    // calculate inverse by solving linear system with identity RHS
    [Test]
    public void StaticSolveMatrix5()
    {
      int i, j;
      float e, me;

      // calculate inverse by solving the linear system
      ComplexFloatMatrix I = ComplexFloatSymmetricLevinson.Solve(T5, ComplexFloatMatrix.CreateIdentity(5));

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

    #endregion Static Solve Matrix 5

    #region Null Parameter Test for Static YuleWalker

    // test Yule-Walker with a null reference
    [Test]
    public void NullParameterTestforStaticYuleWalker()
    {
      Assert.Throws(typeof(ArgumentNullException), () =>
      {
        ComplexFloatVector Y = ComplexFloatSymmetricLevinson.YuleWalker(null);
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
        var T = new ComplexFloatVector(1)
        {
          [0] = new ComplexFloat(1.0f, 1.0f)
        };
        ComplexFloatVector Y = ComplexFloatSymmetricLevinson.YuleWalker(T);
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
        var T = new ComplexFloatVector(3);
        T[2] = T[1] = T[0] = new ComplexFloat(1.0f, 1.0f);
        ComplexFloatVector Y = ComplexFloatSymmetricLevinson.YuleWalker(T);
      });
    }

    #endregion Singular Test for Static YuleWalker

    #region Static Yule Walker 2

    [Test]
    public void StaticYuleWalker2()
    {
      int i;
      float e, me;
      int N = T2.Length;
      ComplexFloatVector A = ComplexFloatSymmetricLevinson.YuleWalker(T2);

      // determine the maximum error
      me = 0.0f;
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
      float e, me;
      int N = T3.Length;
      ComplexFloatVector A = ComplexFloatSymmetricLevinson.YuleWalker(T3);

      // determine the maximum error
      me = 0.0f;
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
      float e, me;
      int N = T4.Length;
      ComplexFloatVector A = ComplexFloatSymmetricLevinson.YuleWalker(T4);

      // determine the maximum error
      me = 0.0f;
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
      float e, me;
      int N = T5.Length;
      ComplexFloatVector A = ComplexFloatSymmetricLevinson.YuleWalker(T5);

      // determine the maximum error
      me = 0.0f;
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
    public void NullPrameterTestforStaticInverse()
    {
      Assert.Throws(typeof(System.ArgumentNullException), () =>
      {
        ComplexFloatMatrix Y = ComplexFloatSymmetricLevinson.Inverse(null);
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
        var T = new ComplexFloatVector(3)
        {
          [0] = ComplexFloat.One,
          [1] = ComplexFloat.One,
          [2] = ComplexFloat.One
        };

        ComplexFloatMatrix Y = ComplexFloatSymmetricLevinson.Inverse(T);
      });
    }

    #endregion Singular Test for Static Inverse

    #region Static Inverse 1

    [Test]
    public void StaticInverse1()
    {
      int i, j;
      float e, me;

      // calculate the inverse
      ComplexFloatMatrix I = ComplexFloatSymmetricLevinson.Inverse(T1);

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
      ComplexFloatMatrix I = ComplexFloatSymmetricLevinson.Inverse(T2);

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
      ComplexFloatMatrix I = ComplexFloatSymmetricLevinson.Inverse(T3);

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
      ComplexFloatMatrix I = ComplexFloatSymmetricLevinson.Inverse(T4);

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
      ComplexFloatMatrix I = ComplexFloatSymmetricLevinson.Inverse(T5);

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
  }
}

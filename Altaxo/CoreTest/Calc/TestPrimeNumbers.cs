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

using Altaxo.Calc;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace AltaxoTest.Calc
{
  [TestFixture]
  public class TestPrimeNumbers
  {
    [Test]
    public void TestPrimalityOf1235()
    {
      Assert.IsTrue(PrimeNumberMath.IsPrime(1));
      Assert.IsTrue(PrimeNumberMath.IsPrime(2));
      Assert.IsTrue(PrimeNumberMath.IsPrime(3));
      Assert.IsTrue(PrimeNumberMath.IsPrime(5));
      Assert.IsTrue(PrimeNumberMath.IsPrime(7));
      Assert.IsTrue(PrimeNumberMath.IsPrime(65537));
    }

    [Test]
    public void TestNonPrimalityOfExampleNumbers()
    {
      Assert.IsFalse(PrimeNumberMath.IsPrime(4), "4");
      Assert.IsFalse(PrimeNumberMath.IsPrime(9), "9");
      Assert.IsFalse(PrimeNumberMath.IsPrime(29209157L), "29209157L");
      Assert.IsFalse(PrimeNumberMath.IsPrime(6863624473L), "6863624473L");
    }

    [Test]
    public void TestPrimeFactorization()
    {
      var rnd = new System.Random(1);

      for (int i = 0; i < 1000; ++i)
      {
        int number = rnd.Next(1, 0x7FFFFFF);
        var list = PrimeNumberMath.PrimeFactorization(number);
        var number2 = PrimeNumberMath.GetNumberFromPrimeFactors(list);
        Assert.AreEqual(number, number2);
        Assert.IsTrue(PrimeNumberMath.IsPrime(list[list.Count - 1].PrimeNumber));
      }
    }

    [Test]
    public void TestGetAllDivisors()
    {
      int number = 2 * 2 * 3 * 5 * 5 * 7;

      var naivList = new List<int>();

      for (int i = 1; i <= number; ++i)
        if (0 == number % i)
          naivList.Add(i);

      var ourList = new List<int>(PrimeNumberMath.GetAllDivisors(number));
      ourList.Sort();

      Assert.AreEqual(naivList.Count, ourList.Count, "List count of naiv list and advanced list");

      for (int i = 0; i < naivList.Count; ++i)
      {
        Assert.AreEqual(naivList[i], ourList[i], string.Format("List entry [{0}]", i));
      }
    }

    [Test]
    public void TestNearestDivisors()
    {
      var list = PrimeNumberMath.GetNeighbouringDivisors(6803 * 6823, 6806, 6823);
      Assert.AreEqual(2, list.Count);
      Assert.AreEqual(6803, list[0]);
      Assert.AreEqual(6823, list[1]);
    }

    [Test]
    public void TestLeastCommonMultiple()
    {
      // primes
      Assert.AreEqual(1 * 1, PrimeNumberMath.LeastCommonMultiple(1, 1));
      Assert.AreEqual(1 * 2, PrimeNumberMath.LeastCommonMultiple(1, 2));
      Assert.AreEqual(2 * 1, PrimeNumberMath.LeastCommonMultiple(2, 1));
      Assert.AreEqual(2 * 3, PrimeNumberMath.LeastCommonMultiple(2, 3));
      Assert.AreEqual(3 * 2, PrimeNumberMath.LeastCommonMultiple(3, 2));
      Assert.AreEqual(3 * 5, PrimeNumberMath.LeastCommonMultiple(3, 5));
      Assert.AreEqual(5 * 3, PrimeNumberMath.LeastCommonMultiple(5, 3));
      Assert.AreEqual(3 * 17, PrimeNumberMath.LeastCommonMultiple(3, 17));
      Assert.AreEqual(17 * 3, PrimeNumberMath.LeastCommonMultiple(17, 3));

      // pure composites
      Assert.AreEqual(8, PrimeNumberMath.LeastCommonMultiple(1, 8));
      Assert.AreEqual(8, PrimeNumberMath.LeastCommonMultiple(2, 8));
      Assert.AreEqual(8, PrimeNumberMath.LeastCommonMultiple(4, 8));
      Assert.AreEqual(8, PrimeNumberMath.LeastCommonMultiple(8, 1));
      Assert.AreEqual(8, PrimeNumberMath.LeastCommonMultiple(8, 2));
      Assert.AreEqual(8, PrimeNumberMath.LeastCommonMultiple(8, 4));

      // with one other component
      Assert.AreEqual(8 * 5, PrimeNumberMath.LeastCommonMultiple(8, 5 * 1));
      Assert.AreEqual(8 * 5, PrimeNumberMath.LeastCommonMultiple(8, 5 * 2));
      Assert.AreEqual(8 * 5, PrimeNumberMath.LeastCommonMultiple(8, 5 * 4));
      Assert.AreEqual(8 * 5, PrimeNumberMath.LeastCommonMultiple(8, 5 * 8));
      Assert.AreEqual(8 * 5, PrimeNumberMath.LeastCommonMultiple(5 * 1, 8));
      Assert.AreEqual(8 * 5, PrimeNumberMath.LeastCommonMultiple(5 * 2, 8));
      Assert.AreEqual(8 * 5, PrimeNumberMath.LeastCommonMultiple(5 * 4, 8));
      Assert.AreEqual(8 * 5, PrimeNumberMath.LeastCommonMultiple(5 * 8, 8));

      //Composites
      Assert.AreEqual(2 * 2 * 2 * 2 * 2 * 3 * 3 * 3 * 5 * 5 * 7 * 7 * 7, PrimeNumberMath.LeastCommonMultiple(2 * 2 * 2 * 2 * 3 * 3 * 3 * 7 * 7 * 7, 2 * 2 * 2 * 2 * 2 * 5 * 5 * 7 * 7));
    }
  }
}

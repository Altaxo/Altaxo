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

using System;
using Altaxo.Calc;
using Altaxo.Calc.Probability;
using NUnit.Framework;


namespace AltaxoTest.Calc.Probability
{

  [TestFixture]
  public class TestProbabilityDistributions
  {
    [Test]
    public  void TestFDistributionQuantile()
    {
      // N[Quantile[FRatioDistribution[3, 4], 1 - 1/20] , 25]
      Assert.AreEqual(6.591382116425581280992118,FDistribution.Quantile(0.95,3,4),1E-14);
    }

    [Test]
    public  void TestFDistributionCDF()
    {
      // N[CDF[FRatioDistribution[3, 4], 1 - 1/20] , 25]
      Assert.AreEqual(0.5034356763953920149093067,FDistribution.CDF(0.95,3,4),1E-14);
    }

    [Test]
    public  void TestFDistributionPDF()
    {
      // N[PDF[FRatioDistribution[3, 4], 1 - 1/20] , 25]
      Assert.AreEqual(0.3612251124036590033894851,FDistribution.PDF(0.95,3,4),1E-14);
    }

    [Test]
    public  void TestStudentTDistributionQuantile()
    {
      // N[Quantile[StudentTDistribution[12], 1/10], 25]
      Assert.AreEqual(-1.356217334023205433796216, StudentTDistribution.Quantile(0.1,12),1e-14);
      
      // N[Quantile[StudentTDistribution[12], 5/10], 25]
      Assert.AreEqual(0, StudentTDistribution.Quantile(0.5,12));
     
      // N[Quantile[StudentTDistribution[12], 9/10], 25]
      Assert.AreEqual(1.356217334023205433796216, StudentTDistribution.Quantile(0.9,12),1e-14);
    }

    [Test]
    public  void TestStudentTDistributionCDF()
    {
      // N[CDF[StudentTDistribution[12], 1/10], 25
      Assert.AreEqual(0.5390022147715870702517874,StudentTDistribution.CDF(0.1,12),1e-14);

      // N[CDF[StudentTDistribution[12], 5/10], 25
      Assert.AreEqual(0.6869412618873379592985154,StudentTDistribution.CDF(0.5,12),1e-14);

      // N[CDF[StudentTDistribution[12], 9/10], 25
      Assert.AreEqual(0.8070872841025491107351673,StudentTDistribution.CDF(0.9,12),1e-14);

      // N[CDF[StudentTDistribution[12], -9/10], 25
      Assert.AreEqual(0.1929127158974508892648327,StudentTDistribution.CDF(-0.9,12),1e-14);

    }

    [Test]
    public  void TestStudentTDistributionPDF()
    {
      // N[PDF[StudentTDistribution[12], 1/10], 25]
      Assert.AreEqual(0.3886164693412969119684949,StudentTDistribution.PDF(0.1,12),1e-14);

      // N[PDF[StudentTDistribution[12], 5/10], 25]
      Assert.AreEqual(0.3417166761526545973157284,StudentTDistribution.PDF(0.5,12),1e-14);

      // N[PDF[StudentTDistribution[12], 9/10], 25]
      Assert.AreEqual(0.2555532488891510539167125,StudentTDistribution.PDF(0.9,12),1e-14);

      // N[PDF[StudentTDistribution[12], -9/10], 25]
      Assert.AreEqual(0.2555532488891510539167125,StudentTDistribution.PDF(-0.9,12),1e-14);

    }
  }
}

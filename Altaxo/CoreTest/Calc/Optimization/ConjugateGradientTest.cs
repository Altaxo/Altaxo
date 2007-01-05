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
using System.Collections;
using NUnit.Framework;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Optimization;

namespace AltaxoTest.Calc.Optimization 
{
  [TestFixture]
  public class ConjugateGradientTest
  {   
    //Test Rosenbrock
    [Test]
    public void TestRosenbrock() 
    {
      Rosenbrock cf = new Rosenbrock();
      EndCriteria ec = new EndCriteria();
      ConjugateGradient optim = new ConjugateGradient(cf,ec);
      //  new SecantLineSearch(cf,ec));
      
      DoubleVector x0 = new DoubleVector(new double[5]{1.3,0.7,0.8,1.9,1.2});
      
      optim.Minimize(x0);
      
      //Console.WriteLine(optim.IterationVectors[0].ToString());
      //Console.WriteLine(optim.IterationVectors[1].ToString());
      //Console.WriteLine(optim.IterationVectors[2].ToString());
      
      //Console.WriteLine(optim.SolutionVector.ToString());
      
      Assert.AreEqual(optim.SolutionValue,0.0,0.1);
      Assert.AreEqual(optim.SolutionVector[0],1.0,0.1);
      Assert.AreEqual(optim.SolutionVector[1],1.0,0.1);
      Assert.AreEqual(optim.SolutionVector[2],1.0,0.1);
      Assert.AreEqual(optim.SolutionVector[3],1.0,0.2);
      Assert.AreEqual(optim.SolutionVector[4],1.0,0.4); 
    }
  }
}

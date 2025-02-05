﻿#region Copyright

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

using System;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Optimization;
using Xunit;

namespace AltaxoTest.Calc.Optimization
{

  public class ConjugateGradientTest
  {
    //Test Rosenbrock
    [Fact]
    public void TestRosenbrock()
    {
      var cf = new Rosenbrock();
      var ec = new EndCriteria();
      var optim = new ConjugateGradient(cf, ec);
      //  new SecantLineSearch(cf,ec));

      var x0 = CreateVector.DenseOfArray(new double[5] { 1.3, 0.7, 0.8, 1.9, 1.2 });

      optim.Minimize(x0);

      AssertEx.Equal(optim.SolutionValue, 0.0, 0.1);
      AssertEx.Equal(optim.SolutionVector[0], 1.0, 0.1);
      AssertEx.Equal(optim.SolutionVector[1], 1.0, 0.1);
      AssertEx.Equal(optim.SolutionVector[2], 1.0, 0.1);
      AssertEx.Equal(optim.SolutionVector[3], 1.0, 0.2);
      AssertEx.Equal(optim.SolutionVector[4], 1.0, 0.4);
    }
  }
}

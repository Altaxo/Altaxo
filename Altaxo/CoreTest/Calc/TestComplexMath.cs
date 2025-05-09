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
using Altaxo.Calc;
using Xunit;
using Complex64T = System.Numerics.Complex;

namespace AltaxoTest.Calc
{

  public class TestComplexMath
  {
    [Fact]
    public void TestExp()
    {
      Complex64T result;

      result = ComplexMath.Exp(new Complex64T(0.5, 0.5));
      AssertEx.Equal(1.446889036584169158051583, result.Real, 1e-15);
      AssertEx.Equal(0.7904390832136149118432626, result.Imaginary, 1e-15);
    }

    [Fact]
    public void TestLog()
    {
      Complex64T arg;
      Complex64T result;

      arg = new Complex64T(1 / 2.0, 1 / 3.0);

      AssertEx.Equal(0.6009252125773315488532035, arg.Magnitude, 1e-15);

      result = ComplexMath.Log(arg);
      AssertEx.Equal(-0.5092847904972866327857336, result.Real, 1e-15);
      AssertEx.Equal(0.5880026035475675512456111, result.Imaginary, 1e-15);
    }
  }
}

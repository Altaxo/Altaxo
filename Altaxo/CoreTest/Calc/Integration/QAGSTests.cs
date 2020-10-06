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

using System;
using Altaxo.Calc.Integration;
using Xunit;

namespace AltaxoTest.Calc.Integration
{

  public class QAGSTests
  {
    [Fact]
    public void TestSin()
    {
      QagsIntegration.Integration(Math.Sin, 0, Math.PI, 0, 1E-6, 100, out var result, out var abserr);

      AssertEx.Equal(2.0, result, 2.0 * 1E-6);
    }

    [Fact]
    public void TestOneBySqrtX()
    {
      QagsIntegration.Integration(delegate (double x)
      { return 1 / Math.Sqrt(x); }, 0, 1, 0, 1E-6, 100, out var result, out var abserr);

      AssertEx.Equal(2.0, result, 2.0 * 1E-6);
    }
  }
}

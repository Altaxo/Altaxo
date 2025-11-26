#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using Xunit;

namespace Altaxo.Calc
{
  public class LambertWTests
  {
    [Fact]
    /// <summary>
    /// Test values for W0 branch, positive arguments
    /// </summary>
    public void LambertW_W0_TestValues_Positive()
    {
      var testValues = new Dictionary<double, double>
      {
        { 0, 0 },
        { 1E-12, 9.99999999999E-13},
        { 1E-4, 0.000099990001499733385406},
        { 1, 0.56714329040978387300 },
        { 2, 0.85260550201372549135 },
        { Math.E, 1 },
        { 10, 1.7455280027406993831 },
        { 100, 3.3856301402900501849 },
      };
      foreach (var kvp in testValues)
      {
        double z = kvp.Key;
        double expected = kvp.Value;
        double result = LambertW.W0(z);
        AssertEx.AreEqual(expected, result, 0, 1E-12);
      }
    }
  }
}

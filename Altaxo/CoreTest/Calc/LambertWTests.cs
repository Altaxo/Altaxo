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

    [Fact]
    /// <summary>
    /// Test values for W0 branch, positive arguments
    /// </summary>
    public void LambertW_W0_TestValues_Negative()
    {
      var testValues = new Dictionary<double, double>
      {
        { -1/1024d, -0.00097751757373022269454 },
        { -1/16d, -0.066818862915653493405 },
        {-93/1024d, -0.10041347836391072512},
        {-168/1024d, -0.20048325016956268995},
        {-228/1024d, -0.30079290613599957954},
        {-275/1024d, -0.40106236308787063016},
        {-311/1024d, -0.50147261583727274812},
        {-337/1024d, -0.59915660607126016647},
        {-355/1024d, -0.69383972007531217404},
        {-370/1024d, -0.82221296968055844588},
        {-374/1024d, -0.88462840745424410676},
        {-375/1024d, -0.90765646354922072264},
        {-376/1024d, -0.93988639805243454197},
        { -1/Math.E + 1/1048576d, -0.99772473035977414162},
        { -1/Math.E, -1 }
      };
      foreach (var kvp in testValues)
      {
        double z = kvp.Key;
        double expected = kvp.Value;
        double result = LambertW.W0(z);
        AssertEx.AreEqual(expected, result, 0, 1E-12);
      }
    }
    [Fact]
    /// <summary>
    /// Test values for W0 branch, positive arguments
    /// </summary>
    public void LambertW_W1_TestValues_Negative()
    {
      var testValues = new Dictionary<double, double>
      {
        { -1/1073741824d, -23.971271509716644856 },
        { -1/1048576d, -16.676972486021141677 },
        { -1/1024d, -9.1446396866250831925 },
        { -1/16d,    -4.2100673728608598371 },
        {-93/1024d, -3.7098688336215573458},
        {-168/1024d, -2.8574306855706828969},
        {-228/1024d, -2.3613667663163030671},
        {-275/1024d, -2.0156349804772059505},
        {-311/1024d, -1.7530174585523841407},
        {-337/1024d, -1.5489955890962905861},
        {-355/1024d, -1.3851947845101166740},
        {-370/1024d, -1.2017358689924431719},
        {-374/1024d, -1.1249913943084231164},
        {-375/1024d, -1.0984039637745947710},
        {-376/1024d, -1.0626237132134927642},
        { -1/Math.E, -1 }
      };
      foreach (var kvp in testValues)
      {
        double z = kvp.Key;
        double expected = kvp.Value;
        double result = LambertW.Wm1(z);
        AssertEx.AreEqual(expected, result, 0, 1E-12);
      }
    }
  }
}

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

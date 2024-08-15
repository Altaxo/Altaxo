#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.FitFunctions.Peaks
{
  public class PeakFunctionsGeneralTest
  {
    [Fact]
    public void TestParameterNamesAndDefaultValues()
    {
      var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Altaxo.Calc.FitFunctions.Peaks.IFitFunctionPeak));
      foreach (var type in types)
      {
        var ff = (IFitFunctionPeak)Activator.CreateInstance(type);
        ff = ff.WithNumberOfTerms(3).WithOrderOfBaselinePolynomial(5);

        var numParameters = ff.NumberOfParameters;


        // test if all parameters have a name
        // and if all parameters are unique
        var paraSet = new HashSet<string>();
        for (int i = 0; i < numParameters; ++i)
        {
          var paraName = ff.ParameterName(i);
          Assert.False(string.IsNullOrEmpty(paraName));
          Assert.DoesNotContain(paraName, paraSet);
          paraSet.Add(paraName);
        }

        // test if all parameters have a default value
        for (int i = 0; i < numParameters; ++i)
        {
          var paraValue = ff.DefaultParameterValue(i);
          Assert.True(paraValue.IsFinite());
        }
      }
    }

    /// <summary>
    /// Tests the number of parameters returned from <see cref="IFitFunctionPeak.GetInitialParametersFromHeightPositionAndWidthAtRelativeHeight(double, double, double, double)"/>.
    /// </summary>
    [Fact]
    public void TestNumberOfParametersForInitialGuess()
    {
      var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Altaxo.Calc.FitFunctions.Peaks.IFitFunctionPeak));
      foreach (var type in types)
      {
        var ff = (IFitFunctionPeak)Activator.CreateInstance(type);
        ff = ff.WithNumberOfTerms(1).WithOrderOfBaselinePolynomial(-1);
        var param = ff.GetInitialParametersFromHeightPositionAndWidthAtRelativeHeight(77.0, 23.0, 7.0, 0.5);

        Assert.Equal(ff.ParameterNamesForOnePeak.Length, param.Length);
        Assert.Equal(ff.NumberOfParameters, param.Length);
      }
    }
  }
}

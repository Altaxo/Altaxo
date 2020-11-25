#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Altaxo.Science.Thermodynamics.Fluids
{
  /// <summary>
  /// Consistency test(s) for classes derived from <see cref="BinaryMixtureDefinitionBase"/>.
  /// </summary>

  public class MixtureConsistencyTests
  {
    /// <summary>
    /// Tests the consistence between the presence/absence of a departure function
    /// and the value of F of the binary mixture.
    /// </summary>
    [Fact]
    public void TestPresenceOfDepartureFunction()
    {
      var assembly = typeof(MixtureOfFluids).Assembly;

      foreach (var type in assembly.GetTypes().Where(t => typeof(BinaryMixtureDefinitionBase).IsAssignableFrom(t) && !t.IsAbstract))
      {
        var m = type.GetProperty("Instance").GetGetMethod();
        var instance = (BinaryMixtureDefinitionBase)m.Invoke(null, new object[0]);

        if (instance.F == 0) // if F is null, then no departure function should be defined.
        {
          var d1 = instance.DepartureFunction_OfReducedVariables(1, 1);
          var d2 = instance.DepartureFunction_OfReducedVariables(2, 2);

          Assert.True(0 == d1 && 0 == d2, $"Mixture {instance.GetType()}: Since F is 0, there should be no departure function defined");
        }
        else // F F is not null, then there should be a departure function defined
        {
          var d1 = instance.DepartureFunction_OfReducedVariables(1, 1);
          var d2 = instance.DepartureFunction_OfReducedVariables(2, 2);

          Assert.True(0 != d1 || 0 != d2, $"Mixture {instance.GetType()}: Since F is not 0, there should be a departure function defined");
        }
      }
    }
  }
}

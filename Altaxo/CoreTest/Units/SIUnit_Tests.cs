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
using System.Linq;

using Xunit;

namespace Altaxo.Units
{
  public class SIUnit_Tests
  {
    public IEnumerable<Type> GetAllClasses()
    {
      // This method returns all types that have the CompareAttribute applied to them.
      return AppDomain.CurrentDomain
        .GetAssemblies()
        .SelectMany(assembly => assembly.GetTypes());
    }

    [Fact]
    public void Test_CompareAttributeWithClass()
    {
      // This test is to ensure that the CompareAttribute works correctly with classes.
      // The CompareAttribute should not throw an exception when applied to a class.
      var allSIUnitTypes = GetAllClasses().Where(t => typeof(SIUnit).IsAssignableFrom(t)).ToArray();

      Assert.True(allSIUnitTypes.Length > 0, "No SIUnit types found in the current domain.");

      foreach (var siUnitType in allSIUnitTypes)
      {
        if (siUnitType == typeof(SIUnit))
          continue; // SIUnit is a special case, we don't want to test it here

        var obj = siUnitType.GetProperty("Instance")?.GetGetMethod()?.Invoke(null, null);
        Assert.False(obj is null, $"Instance property of {siUnitType.Name} returned null.");
        var instance = (SIUnit)obj;

        // get the attribute UnitDescriptionAttribute from the class
        var unitDescriptionAttribute = (UnitDescriptionAttribute?)Attribute.GetCustomAttribute(siUnitType, typeof(UnitDescriptionAttribute));
        Assert.NotNull(unitDescriptionAttribute);

        // compare the unit exponents from the attribute with the class properties
        AssertEx.Equal(unitDescriptionAttribute.Metre, instance.ExponentMetre, $"{siUnitType}");
        AssertEx.Equal(unitDescriptionAttribute.Kilogram, instance.ExponentKilogram, $"{siUnitType}");
        AssertEx.Equal(unitDescriptionAttribute.Second, instance.ExponentSecond, $"{siUnitType}");
        AssertEx.Equal(unitDescriptionAttribute.Ampere, instance.ExponentAmpere, $"{siUnitType}");
        AssertEx.Equal(unitDescriptionAttribute.Kelvin, instance.ExponentKelvin, $"{siUnitType}");
        AssertEx.Equal(unitDescriptionAttribute.Mole, instance.ExponentMole, $"{siUnitType}");
        AssertEx.Equal(unitDescriptionAttribute.Candela, instance.ExponentCandela, $"{siUnitType}");

        if (instance.ShortCut.Length > 0 && instance.ShortCut[0] == 'k')
        {
          Assert.True(instance.Prefixes == SIPrefix.ListWithNonePrefixOnly);
        }
      }
    }
  }
}


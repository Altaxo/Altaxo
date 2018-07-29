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

using Altaxo.Main.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Units
{
  /// <summary>
  /// Extension to help finding all defined units and instantiating them.
  /// </summary>
  public static class UnitsExtensions
  {
    /// <summary>
    /// Gets all defined units in the currently loaded assemblies, i.e. all classes that are decorated with the <see cref="UnitDescriptionAttribute"/>.
    /// </summary>
    /// <returns>A dictionary with the type of the unit (key) and the <see cref="UnitDescriptionAttribute"/> of this type (value).</returns>
    public static IReadOnlyDictionary<Type, UnitDescriptionAttribute> GetAllDefinedUnits()
    {
      var result = new Dictionary<Type, UnitDescriptionAttribute>();

      var types = ReflectionService.GetSortedClassTypesHavingAttribute(typeof(UnitDescriptionAttribute), false);

      var list = new HashSet<string>();

      foreach (var ty in types)
      {
        var attribute = (UnitDescriptionAttribute)ty.GetCustomAttributes(typeof(UnitDescriptionAttribute), false).First();
        result.Add(ty, attribute);
      }

      return result;
    }

    /// <summary>
    /// Enumerates all defined quantities.
    /// </summary>
    /// <param name="allDefinedUnits">Dictionary with all defined units. If you provide null, all defined units will be searched in the assemblies, which may take some time.</param>
    /// <returns>Enumeration of all defined quantities in alphabetical order.</returns>
    public static IEnumerable<string> GetAllDefinedQuantities(IReadOnlyDictionary<Type, UnitDescriptionAttribute> allDefinedUnits = null)
    {
      if (null == allDefinedUnits)
      {
        allDefinedUnits = GetAllDefinedUnits();
      }

      var list = new HashSet<string>(allDefinedUnits.Values.Select(x => x.Quantity));
      var quantities = list.ToArray();
      Array.Sort(quantities);

      return quantities;
    }

    /// <summary>
    /// Gets the unit instance for a given type.
    /// </summary>
    /// <param name="type">The type of the unit.</param>
    /// <returns>The unit instance with the given type. It is presumed that the unit class has either a static property 'Instance' or a parameterless constructor.</returns>
    /// <exception cref="ArgumentException">If the type given in the argument is not derived from <see cref="IUnit"/>"</exception>
    public static IUnit GetUnitInstance(Type type)
    {
      if (!typeof(IUnit).IsAssignableFrom(type))
        throw new ArgumentException("Given type is not derived from " + nameof(IUnit), nameof(type));

      var propInfo = type.GetProperty("Instance");
      var propMethod = propInfo.GetGetMethod();
      if (null != propMethod)
      {
        var instance = (IUnit)propMethod.Invoke(null, null);
        return instance;
      }
      else
      {
        // try to construct this type
        return (IUnit)Activator.CreateInstance(type);
      }
    }

    /// <summary>
    /// Gets all available units for a given quantity.
    /// </summary>
    /// <param name="quantity">The quantity (e.g. 'Length', 'Time' etc).</param>
    /// <param name="allDefinedUnits">Dictionary with all defined units. If you provide null, all defined units will be searched in the assemblies, which may take some time.</param>
    /// <returns>All unit instances of the given quantity.</returns>
    public static IEnumerable<IUnit> GetAvailableUnitsForQuantity(string quantity, IReadOnlyDictionary<Type, UnitDescriptionAttribute> allDefinedUnits = null)
    {
      if (null == allDefinedUnits)
      {
        allDefinedUnits = GetAllDefinedUnits();
      }

      var unitTypes = allDefinedUnits.Where(x => x.Value.Quantity == quantity).Select(x => x.Key);
      foreach (var ty in unitTypes)
      {
        yield return GetUnitInstance(ty);
      }
    }
  }
}

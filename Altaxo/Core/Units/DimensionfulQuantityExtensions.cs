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

namespace Altaxo.Units
{
  /// <summary>
  /// Extension methods for <see cref="DimensionfulQuantity"/>.
  /// </summary>
  public static class DimensionfulQuantityExtensions
  {
    /// <summary>
    /// Checks whether the quantity <paramref name="x"/> has the expected <paramref name="expectedUnit"/>, and if not, throws an <see cref="ArgumentException"/>.
    /// </summary>
    /// <param name="x">The dimensionful quantity to check.</param>
    /// <param name="expectedUnit">The expected unit the quantity should have.</param>
    /// <param name="valueName">Name of the value <paramref name="x"/>. When compiled with C# 10 or later the caller argument expression will be supplied automatically.</param>
    /// <remarks>
    /// This helper is intended to validate that the runtime unit of <paramref name="x"/> is compatible with the provided
    /// <paramref name="expectedUnit"/> (i.e. both share the same SI base dimensions). If they are incompatible an
    /// <see cref="ArgumentException"/> is thrown identifying the argument name and the incompatible units.
    /// </remarks>
    /// <exception cref="System.ArgumentException">Argument '{valueName}' has a unit {x.Unit} that is not compatible with '{expectedUnit}'.</exception>
    public static void CheckUnitCompatibleWith(this DimensionfulQuantity x, IUnit expectedUnit,
    [System.Runtime.CompilerServices.CallerArgumentExpression(nameof(x))]
    string valueName = "")
    {
      if (!(x.Unit.SIUnit == expectedUnit.SIUnit))
      {
        throw new ArgumentException($"Argument '{valueName}' has a unit {x.Unit} that is not compatible with '{expectedUnit}'", valueName);
      }
    }
  }
}

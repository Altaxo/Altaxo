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
  /// Extensions methods for <see cref="DimensionfulQuantity"/>.
  /// </summary>
  public static class DimensionfulQuantityExtensions
  {
    /// <summary>
    /// Checks whether the quantity <paramref name="x"/> has the expected <paramref name="expectedUnit"/>, and if not, throws an <see cref="ArgumentException"/>.
    /// </summary>
    /// <param name="x">The dimensionful quantity x.</param>
    /// <param name="expectedUnit">The expected unit.</param>
    /// <param name="valueName">Name of the value <paramref name="x"/>.</param>
    /// <exception cref="System.ArgumentException">Argument '{valueName}' has a unit {x.Unit} that is not compatible with '{expectedUnit}'</exception>
    public static void CheckUnitCompatibleWith(this DimensionfulQuantity x, IUnit expectedUnit,
#if NET9_0_OR_GREATER
    [System.Runtime.CompilerServices.CallerArgumentExpression(nameof(x))]
#endif
    string valueName = "")
    {
      if (!(x.Unit.SIUnit == expectedUnit.SIUnit))
      {
        throw new ArgumentException($"Argument '{valueName}' has a unit {x.Unit} that is not compatible with '{expectedUnit}'", valueName);
      }
    }
  }
}

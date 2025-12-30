#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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

#nullable enable

namespace Altaxo.Units
{
  /// <summary>
  /// Base class for classes that implement <see cref="IUnit"/> and are not a <see cref="SIUnit"/>. Classes that are SI units should derive from
  /// <see cref="SIUnit"/>.
  /// </summary>
  public abstract class UnitBase
  {
    /// <summary>
    /// Determines whether the specified <see cref="SIUnit"/> is equal to this unit.
    /// </summary>
    /// <param name="obj">The SI unit to compare with this instance.</param>
    /// <returns><c>true</c> if the specified <see cref="SIUnit"/> is equal to this unit; otherwise, <c>false</c>.</returns>
    public bool Equals(SIUnit obj)
    {
      return obj is null ? false : obj.Equals(this);
    }

    /// <summary>
    /// Determines whether the specified <see cref="IUnit"/> is equal to this unit by comparing their runtime types.
    /// </summary>
    /// <param name="obj">The unit to compare with this instance.</param>
    /// <returns><c>true</c> if the specified <see cref="IUnit"/> has the same runtime type as this unit; otherwise, <c>false</c>.</returns>
    public bool Equals(IUnit obj)
    {
      return obj is null ? false : GetType() == obj.GetType();
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      return obj is IUnit other ? GetType() == other.GetType() : false;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return GetType().GetHashCode();
    }

    /// <summary>
    /// Determines whether two units are equal using the <see cref="Equals(IUnit)"/> implementation.
    /// </summary>
    /// <param name="a">The left operand.</param>
    /// <param name="b">The right operand.</param>
    /// <returns><c>true</c> if both operands are considered equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(UnitBase a, IUnit b)
    {
      if (a is { } aa)
        return aa.Equals(b);
      else if (b is { } bb)
        return b.Equals(a);
      else
        return true; // null==null
    }

    /// <summary>
    /// Determines whether two units are not equal using the <see cref="Equals(IUnit)"/> implementation.
    /// </summary>
    /// <param name="a">The left operand.</param>
    /// <param name="b">The right operand.</param>
    /// <returns><c>true</c> if the operands are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(UnitBase a, IUnit b)
    {
      return !(a == b);
    }
  }
}

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

namespace Altaxo.Units
{
  /// <summary>
  /// Base class for classes that implement <see cref="IUnit"/> and are not a <see cref="SIUnit"/>. Classes that are SI units should derive from
  /// <see cref="SIUnit"/>.
  /// </summary>
  public abstract class UnitBase
  {
    public bool Equals(SIUnit obj)
    {
      if (null == obj)
        return false;

      return obj.Equals(this);
    }

    public bool Equals(IUnit obj)
    {
      if (null == obj)
        return false;

      return GetType() == obj.GetType();
    }

    public override bool Equals(object obj)
    {
      if (!(obj is IUnit other))
        return false;

      return GetType() == obj.GetType();
    }

    public override int GetHashCode()
    {
      return GetType().GetHashCode();
    }

    public static bool operator ==(UnitBase a, IUnit b)
    {
      return a?.Equals(b) ?? false;
    }

    public static bool operator !=(UnitBase a, IUnit b)
    {
      return !(a == b);
    }
  }
}

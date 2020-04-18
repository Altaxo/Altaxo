#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#nullable enable

namespace Altaxo.Units
{
  /// <summary>
  /// Encapsulates an existing unit, but limits the set of possible prefixes.
  /// </summary>
  public class UnitWithLimitedPrefixes : IUnit
  {
    private SIPrefixList _prefixes;
    private IUnit _unit;

    public UnitWithLimitedPrefixes(IUnit unit, IEnumerable<SIPrefix> allowedPrefixes)
    {
      if (allowedPrefixes is null)
        throw new ArgumentNullException(nameof(allowedPrefixes));
      _unit = unit ?? throw new ArgumentNullException(nameof(unit));

      var l = new HashSet<SIPrefix>(_unit.Prefixes);
      l.IntersectWith(allowedPrefixes);
      _prefixes = new SIPrefixList(l);
    }

    public string Name
    {
      get { return _unit.Name; }
    }

    public string ShortCut
    {
      get { return _unit.ShortCut; }
    }

    public double ToSIUnit(double x)
    {
      return _unit.ToSIUnit(x);
    }

    public double FromSIUnit(double x)
    {
      return _unit.FromSIUnit(x);
    }

    /// <summary>
    /// Returns a list of possible prefixes for this unit (like µ, m, k, M, G..).
    /// </summary>
    public ISIPrefixList Prefixes
    {
      get { return _prefixes; }
    }

    /// <summary>
    /// Gets the underlying unit.
    /// </summary>
    /// <value>
    /// The underlying unit.
    /// </value>
    public IUnit Unit
    {
      get { return _unit; }
    }

    /// <summary>
    /// Returns the underlying unit, transformed to the corresponding SI unit.
    /// </summary>
    public SIUnit SIUnit
    {
      get { return _unit.SIUnit; }
    }
  }
}

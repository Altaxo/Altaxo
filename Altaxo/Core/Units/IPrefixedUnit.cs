#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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


#nullable enable

namespace Altaxo.Units
{
  /// <summary>
  /// Interface to a unit that is prefixed with an SI prefix. Example: Nanometer (Meter is the unit, Nano is the prefix).
  /// </summary>
  public interface IPrefixedUnit
  {
    /// <summary>
    /// Gets the SI prefix (e.g. nano, micro, mega, etc.).
    /// </summary>
    /// <value>The SI prefix applied to the unit.</value>
    SIPrefix Prefix { get; }

    /// <summary>
    /// Gets the underlying unit that the prefix is applied to.
    /// </summary>
    /// <value>The unit without the prefix (for example, meter for nanometer).</value>
    IUnit Unit { get; }
  }
}

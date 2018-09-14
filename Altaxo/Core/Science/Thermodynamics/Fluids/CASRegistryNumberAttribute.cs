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

namespace Altaxo.Science.Thermodynamics.Fluids
{
  /// <summary>
  /// Associates a CAS registry number to a (pure) fluid or two CAS registry numbers to a binary mixture.
  /// </summary>
  /// <seealso cref="System.Attribute" />
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public class CASRegistryNumberAttribute : Attribute
  {
    /// <summary>
    /// Gets the CAS registry number.
    /// </summary>
    /// <value>
    /// The CAS registry number.
    /// </value>
    public string CASRegistryNumber { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CASRegistryNumberAttribute"/> class.
    /// </summary>
    /// <param name="casRegistryNumber">The CAS registry number.</param>
    public CASRegistryNumberAttribute(string casRegistryNumber)
    {
      CASRegistryNumber = casRegistryNumber;
    }
  }
}

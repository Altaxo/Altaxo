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
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class UnitDescriptionAttribute : Attribute
  {
    private string _quantity;
    private sbyte _metre;
    private sbyte _kilogram;
    private sbyte _second;
    private sbyte _ampere;
    private sbyte _kelvin;
    private sbyte _mole;
    private sbyte _candela;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitDescriptionAttribute"/> class.
    /// </summary>
    /// <param name="quantity">The quantity description. Examples are Length, Temperature, Mass, Time, ElectricalVoltage, ElectricalCurrent, Force, etc.</param>
    /// <param name="metre">Power of 'Metre' units that the constructed unit will contain.</param>
    /// <param name="kilogram">Power of 'Kilogram' units that the constructed unit will contain.</param>
    /// <param name="second">Power of 'Second' units that the constructed unit will contain.</param>
    /// <param name="ampere">Power of 'Ampere' units that the constructed unit will contain.</param>
    /// <param name="kelvin">Power of 'Kelvin' units that the constructed unit will contain.</param>
    /// <param name="mole">Power of 'Mole' units that the constructed unit will contain.</param>
    /// <param name="candela">Power of 'Candela' units that the constructed unit will contain.</param>
    public UnitDescriptionAttribute(string quantity, sbyte metre, sbyte kilogram, sbyte second, sbyte ampere, sbyte kelvin, sbyte mole, sbyte candela)
    {
      _quantity = quantity;
      _metre = metre;
      _kilogram = kilogram;
      _second = second;
      _ampere = ampere;
      _kelvin = kelvin;
      _mole = mole;
      _candela = candela;
    }

    /// <summary>The quantity description. Examples are Length, Temperature, Mass, Time, ElectricalVoltage, ElectricalCurrent, Force, etc.</summary>
    public string Quantity { get { return _quantity; } }

    /// <summary>Power of 'Metre' units that the constructed unit will contain.</summary>
    public int Metre => _metre;

    /// <summary>Power of 'Kilogram' units that the constructed unit will contain.</summary>
    public int Kilogram => _kilogram;

    /// <summary>Power of 'Second' units that the constructed unit will contain.</summary>
    public int Second => _second;

    /// <summary>Power of 'Ampere' units that the constructed unit will contain.</summary>
    public int Ampere => _ampere;

    /// <summary>Power of 'Kelvin' units that the constructed unit will contain.</summary>
    public int Kelvin => _kelvin;

    /// <summary>Power of 'Mole' units that the constructed unit will contain.</summary>
    public int Mole => _mole;

    /// <summary>Power of 'Candela' units that the constructed unit will contain.</summary>
    public int Candela => _candela;
  }
}

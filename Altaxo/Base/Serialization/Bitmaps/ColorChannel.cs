#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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


namespace Altaxo.Serialization.Bitmaps
{
  /// <summary>
  /// Designates which part of the color of each pixel should be imported as value in the table.
  /// </summary>
  public enum ColorChannel
  {
    /// <summary>The HSL lightness value.</summary>
    HSLLightness,

    /// <summary>The HSL saturation value.</summary>
    HSLSaturation,

    /// <summary>The HSL hue value.</summary>
    HSLHue,

    /// <summary>The red channel value.</summary>
    Red,

    /// <summary>The green channel value.</summary>
    Green,

    /// <summary>The blue channel value.</summary>
    Blue,

    /// <summary>The alpha channel value.</summary>
    Alpha,
  };
}

#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using Altaxo.Drawing;

namespace Altaxo.Graph.Graph3D.Lighting
{
  /// <summary>
  /// Interface to discrete lights. All classes that implement this interface should be immutable.
  /// </summary>
  /// <seealso cref="Altaxo.Main.IImmutable" />
  public interface IDiscreteLight : Main.IImmutable
  {
    /// <summary>
    /// Gets a value indicating whether this light source is affixed to the camera coordinate system or to the world coordinate system.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is affixed to the camera coordinate system; <c>false</c> if this instance is affixed to the world coordinate system.
    /// </value>
    bool IsAffixedToCamera { get; }

    /// <summary>
    /// Gets the light amplitude. The default value is 1. This value is multiplied with the light <see cref="Color"/> to get the effective light's color.
    /// </summary>
    /// <value>
    /// The light amplitude.
    /// </value>
    double LightAmplitude { get; }

    /// <summary>
    /// Gets the color of the light.
    /// </summary>
    NamedColor Color { get; }
  }
}

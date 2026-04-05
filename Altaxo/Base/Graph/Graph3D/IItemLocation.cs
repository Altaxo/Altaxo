#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph3D
{
  /// <summary>
  /// Describes the position, orientation, shear, and scale of an item in a three-dimensional host layer.
  /// </summary>
  public interface IItemLocation
  :
  Altaxo.Main.IDocumentLeafNode,
  Altaxo.Main.ICopyFrom
  {
    /// <summary>
    /// Gets or sets the rotation around the x-axis in degrees.
    /// </summary>
    double RotationX { get; set; }
    /// <summary>
    /// Gets or sets the rotation around the y-axis in degrees.
    /// </summary>
    double RotationY { get; set; }
    /// <summary>
    /// Gets or sets the rotation around the z-axis in degrees.
    /// </summary>
    double RotationZ { get; set; }

    /// <summary>
    /// Gets or sets the shear in x direction.
    /// </summary>
    double ShearX { get; set; }

    /// <summary>
    /// Gets or sets the shear in y direction.
    /// </summary>
    double ShearY { get; set; }

    /// <summary>
    /// Gets or sets the shear in z direction.
    /// </summary>
    double ShearZ { get; set; }

    /// <summary>
    /// Gets or sets the scale in x direction.
    /// </summary>
    double ScaleX { get; set; }

    /// <summary>
    /// Gets or sets the scale in y direction.
    /// </summary>
    double ScaleY { get; set; }

    /// <summary>
    /// Gets or sets the scale in z direction.
    /// </summary>
    double ScaleZ { get; set; }
  }
}

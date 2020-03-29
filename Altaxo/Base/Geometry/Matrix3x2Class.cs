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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Geometry
{
  /// <summary>
  /// Class wrapper around the <see cref="Matrix3x2"/> struct. The wrapped matrix can be accessed by the <see cref="Matrix"/> property.
  /// </summary>
  public class Matrix3x2Class : IEquatable<Matrix3x2Class>, IEquatable<Matrix3x2>, Main.IImmutable
  {
    /// <summary>
    /// Gets the wrapped matrix.
    /// </summary>
    /// <value>
    /// The wrapped matrix.
    /// </value>
    public Matrix3x2 Matrix { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix3x2Class"/> class with the identity transformation.
    /// </summary>
    public Matrix3x2Class()
    {
      Matrix = Matrix3x2.Identity;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix3x2Class"/> class.
    /// </summary>
    /// <param name="matrix">The matrix to wrap.</param>
    public Matrix3x2Class(Matrix3x2 matrix)
    {
      Matrix = matrix;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix3x2Class"/> class.
    /// </summary>
    /// <param name="m11">The element M11.</param>
    /// <param name="m12">The element M12.</param>
    /// <param name="m21">The element M21.</param>
    /// <param name="m22">The element M22.</param>
    /// <param name="m31">The element M31.</param>
    /// <param name="m32">The element M32.</param>
    public Matrix3x2Class(
    double m11, double m12,
    double m21, double m22,
    double m31, double m32
    )
    {
      Matrix = new Matrix3x2(m11, m12, m21, m22, m31, m32);
    }

    public bool Equals(Matrix3x2Class other)
    {
      return other is null ? false : Matrix.Equals(other.Matrix);
    }

    public bool Equals(Matrix3x2 other)
    {
      return Matrix.Equals(other);
    }

    public override bool Equals(object obj)
    {
      return Equals(obj as Matrix3x2Class);
    }

    public override int GetHashCode()
    {
      return Matrix.GetHashCode();
    }
  }
}

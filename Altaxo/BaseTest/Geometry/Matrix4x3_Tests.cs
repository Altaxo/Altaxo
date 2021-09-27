#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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
using Xunit;

namespace Altaxo.Geometry
{
  public class Matrix4x3_Tests
  {
    

    [Fact]
    public void TestDecomposition1()
    {
      var m = Matrix4x3.FromScaleShearRotationRadianTranslation(7, 11, 13, 1 / 2d, 1 / 3d, 1 / 5d, Math.PI / 12, Math.PI / 6, Math.PI / 4,17,19,23);
      var (scale, shear, rot, trans) = m.DecomposeIntoScaleShearRotationRadianTranslation();

      AssertEx.AreEqual(1/2d, shear.X, 1E-8, 1E-8);
      AssertEx.AreEqual(1/3d, shear.Y, 1E-8, 1E-8);
      AssertEx.AreEqual(1/5d, shear.Z, 1E-8, 1E-8);

      AssertEx.AreEqual(7.0, scale.X, 1E-8, 1E-8);
      AssertEx.AreEqual(11.0, scale.Y, 1E-8, 1E-8);
      AssertEx.AreEqual(13.0, scale.Z, 1E-8, 1E-8);

      AssertEx.AreEqual(Math.PI/12, rot.X, 1E-8, 1E-8); 
      AssertEx.AreEqual(Math.PI/6, rot.Y, 1E-8, 1E-8);
      AssertEx.AreEqual(Math.PI/4, rot.Z, 1E-8, 1E-8);

      AssertEx.AreEqual(17, trans.X, 1E-8, 1E-8);
      AssertEx.AreEqual(19, trans.Y, 1E-8, 1E-8);
      AssertEx.AreEqual(23, trans.Z, 1E-8, 1E-8);
    }
  }
}

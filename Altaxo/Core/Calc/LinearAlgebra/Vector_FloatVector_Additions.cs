#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
//
//    modified for Altaxo:  a data processing and data plotting program
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Altaxo.Calc.LinearAlgebra
{
  ///<summary>
  /// Defines a Vector of floats.
  ///</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  [Serializable]
  public partial class FloatVector : Vector<float>
  {
    ///<summary>Implicit cast conversion to <c>FloatVector</c> from <c>float</c> array</summary>
    [return: NotNullIfNotNull("src")]
    public static implicit operator FloatVector?(float[]? src)
    {
      return src is null ? null : new FloatVector(src);
    }

    ///<summary>Implicit cast conversion to <c>FloatVector</c> from <c>float</c> array</summary>
    [return: NotNullIfNotNull("src")]
    public static FloatVector? ToFloatVector(float[]? src)
    {
      return src is null ? null : new FloatVector(src);
    }

    ///<summary>Explicit cast conversion to <c>FloatVector</c> from <c>DoubleVector</c></summary>
    [return: NotNullIfNotNull("src")]
    public static explicit operator FloatVector?(DoubleVector? src)
    {
      if (src is null)
      {
        return null;
      }
      var ret = new FloatVector(src.Length);
      // Can't use Array.Copy to implicitly copy from a double[] to a float[]
      for (int i = 0; i < src.Length; i++)
      {
        ret[i] = (float)src[i];
      }
      return ret;
    }
  }
}

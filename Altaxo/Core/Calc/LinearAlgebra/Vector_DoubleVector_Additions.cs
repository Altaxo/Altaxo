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

/*
 * DoubleVector.cs
 *
 * Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Altaxo.Calc.LinearAlgebra
{
  ///<summary>
  /// Defines a Vector of doubles.
  ///</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  [Serializable]
  public partial class DoubleVector : Vector<double>
  {
    /// <summary>
    /// Returns a wrapper object, so that the returned vector is read only. Please note, that changes to the elements
    /// of the underlying array in DoubleVector are reflected in the wrapper object, whereas when the array itself is changing,
    /// the wrapper object will not reflect the changed.
    /// </summary>
    /// <returns></returns>
    public IROVector<double> ToROVector()
    {
      return VectorMath.ToROVector(_array, _array.Length);
    }

    ///<summary>Implicit cast conversion to <c>DoubleVector</c> from <c>FloatVector</c></summary>
    [return: NotNullIfNotNull("src")]
    public static explicit operator DoubleVector?(FloatVector? src)
    {
      if (src is null)
      {
        return null;
      }
      var ret = new DoubleVector(src.Count);
      Array.Copy(src.GetInternalData(), ret._array, src.Length);
      return ret;
    }

    ///<summary>Implicit cast conversion to <c>DoubleVector</c> from <c>FloatVector</c></summary>
    [return: NotNullIfNotNull("src")]
    public static DoubleVector? ToDoubleVector(FloatVector? src)
    {
      if (src is null)
      {
        return null;
      }
      var ret = new DoubleVector(src.Length);
      Array.Copy(src.GetInternalData(), ret._array, src.Length);
      return ret;
    }

    ///<summary>Implicit cast conversion to <c>DoubleVector</c> from <c>float</c> array</summary>
    [return: NotNullIfNotNull("src")]
    public static explicit operator DoubleVector?(double[]? src)
    {
      if (src is null)
      {
        return null;
      }
      var ret = new DoubleVector(src.Length);
      Array.Copy(src, ret._array, src.Length);
      return ret;
    }

    ///<summary>Implicit cast conversion to <c>DoubleVector</c> from <c>float</c> array</summary>
    [return: NotNullIfNotNull("src")]
    public static explicit operator DoubleVector?(float[]? src)
    {
      if (src is null)
      {
        return null;
      }
      var ret = new DoubleVector(src.Length);
      Array.Copy(src, ret._array, src.Length);
      return ret;
    }

    ///<summary>Implicit cast conversion to <c>DoubleVector</c> from <c>float</c> array</summary>
    [return: NotNullIfNotNull("src")]
    public static DoubleVector? ToDoubleVector(float[]? src)
    {
      if (src is null)
      {
        return null;
      }
      var ret = new DoubleVector(src.Length);
      Array.Copy(src, ret._array, src.Length);
      return ret;
    }
  }
}

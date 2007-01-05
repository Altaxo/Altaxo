#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
//
//    modified for Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

/*
 * ComplexFloatVectorEnumerator.cs
 * 
 * Copyright (c) 2004, dnAnalytics Project. All rights reserved.
*/

using System;
using System.Collections;

namespace Altaxo.Calc.LinearAlgebra
{
  ///<summary>
  /// Defines an Enumerator for the ComplexFloatVector that supports 
  /// simple iteration over each vector component.
  ///</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  sealed internal class ComplexFloatVectorEnumerator : IEnumerator
  {
    private ComplexFloatVector v;
    private int index;
    private int length;

    ///<summary> Constructor </summary>
    public ComplexFloatVectorEnumerator(ComplexFloatVector vector)
    {
      v = vector;
      index = -1;
      length = v.Length;
    }

    ///<summary> Return the current <c>ComplexFloatVector</c> component</summary>
    public ComplexFloat Current
    {
      get
      {
        if (index < 0 || index >= length)
          throw new InvalidOperationException();
        return v[index];
      }
    }
    object IEnumerator.Current
    {
      get { return Current; }
    }

    ///<summary> Move the index to the next component </summary>
    public bool MoveNext()
    {
      if (length != v.Length)
        throw new InvalidOperationException();
      index++;
      if (index >= length)
      {
        index = length;
        return false;
      }
      else
      {
        return true;
      }
    }

    ///<summary> Set the enumerator to it initial position </summary>
    public void Reset()
    {
      index = -1;
    }
  }
}

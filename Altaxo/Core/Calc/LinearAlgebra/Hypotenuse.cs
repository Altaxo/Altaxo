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

using System;

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary></summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  internal class Hypotenuse 
  {
    private Hypotenuse() {}
    public static double Compute(double a, double b) 
    {
      double r;
      if (System.Math.Abs(a) > System.Math.Abs(b)) 
      {
        r = b/a;
        r = System.Math.Abs(a)*System.Math.Sqrt(1+r*r);
      } 
      else if (b != 0) 
      {
        r = a/b;
        r = System.Math.Abs(b)*System.Math.Sqrt(1+r*r);
      } 
      else 
      {
        r = 0.0;
      }
      return r;
    }  
    public static float Compute(float a, float b) 
    {
      float r;
      if (System.Math.Abs(a) > System.Math.Abs(b)) 
      {
        r = b/a;
        r = (float)(System.Math.Abs(a)*System.Math.Sqrt(1+r*r));
      } 
      else if (b != 0) 
      {
        r = a/b;
        r = (float)(System.Math.Abs(b)*System.Math.Sqrt(1+r*r));
      } 
      else 
      {
        r = 0.0f;
      }
      return r;
    }  
  
    public static float Compute(ComplexFloat a, ComplexFloat b)
    {
      ComplexFloat temp = a * ComplexMath.Conjugate(a);
      temp += (b * ComplexMath.Conjugate(b));
      float ret = ComplexMath.Absolute(temp);
      return (float)System.Math.Sqrt(ret);
    }

    public static double Compute(Complex a, Complex b)
    {
      Complex temp = a * ComplexMath.Conjugate(a);
      temp += (b * ComplexMath.Conjugate(b));
      double ret = ComplexMath.Absolute(temp);
      return System.Math.Sqrt(ret);
    }
  }
}

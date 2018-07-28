#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Copyright (C) bsargos, Software Developer, France
//    (see CodeProject article http://www.codeproject.com/Articles/16083/One-dimensional-root-finding-algorithms)
//    This source code file is licenced under the CodeProject open license (CPOL)
//
//    modified for Altaxo:  a data processing and data plotting program
//    Copyright (C) 2012 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.RootFinding
{
  public static class UnaryFunctions
  {
    public static Func<double, double> Identity()
    {
      return new Func<double, double>(delegate (double x)
      { return x; });
    }

    public static Func<double, double> Constant(double a)
    {
      return new Func<double, double>(delegate (double x)
      { return a; });
    }

    static public Func<double, double> Add(Func<double, double> f1, Func<double, double> f2)
    {
      return new Func<double, double>(delegate (double x)
      { return f1(x) + f2(x); });
    }

    static public Func<double, double> Multiply(Func<double, double> f, double lambda)
    {
      return new Func<double, double>(delegate (double x)
      { return lambda * f(x); });
    }

    static public Func<double, double> Minus(Func<double, double> f)
    {
      return new Func<double, double>(delegate (double x)
      { return -f(x); });
    }

    static public Func<double, double> Subtract(Func<double, double> f1, Func<double, double> f2)
    {
      return new Func<double, double>(delegate (double x)
      { return f1(x) - f2(x); });
    }

    static public Func<double, double> Compound(Func<double, double> f1, Func<double, double> f2)
    {
      return new Func<double, double>(delegate (double x)
      { return f1(f2(x)); });
    }
  }
}

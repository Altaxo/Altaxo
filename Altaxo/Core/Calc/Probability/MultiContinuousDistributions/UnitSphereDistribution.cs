#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
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

// This file was created by Dirk Lellinger Jan 2004 as a translation from Matpack 1.7.3 sources (Author B.Gammel) to C#
// The following Matpack files were used here:

// matpack-1.7.3\source\random\RanSphere.cc

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.Probability.ContinuousRNDs
{
  /// <summary>
  /// Vector of three random numbers distributed uniformly on the unit sphere.   
  /// </summary>
  /// <remarks><code>
  /// Uses the algorithm of Marsaglia, Ann. Math. Stat 43, 645 (1972).           
  /// On average requires 2.25 deviates per vector and a square root calculation 
  /// Vector of three random numbers (x,y,z) which are distributed uniformly     
  /// on the unit sphere.                                                        
  ///                            
  /// Uses the algorithm of Marsaglia, Ann. Math. Stat 43, 645 (1972).        
  /// On average requires 2.25 deviates per vector and a square root calculation 
  /// </code></remarks>                           

  public class UnitSphereDistribution : Distribution
  {
    protected double scale;
    public UnitSphereDistribution()
      :
      this(new StandardGenerator())
    { }

    public UnitSphereDistribution(Generator generator)
      : base(generator)
    {
      scale = 2.0 / (int.MaxValue - 1);;
    }

    public override double NextDouble()
    {
      throw new NotSupportedException("Use NextDoubles(out double x, out double y, out double z) instead of this method");
    }

    public void NextDoubles(out double x, out double y, out double z)
    {
      for (; ; )
      {
        double d1 = 1.0 - scale * Generator.Next(),
          d2 = 1.0 - scale * Generator.Next(),
          dd = d1 * d1 + d2 * d2;
        if (dd < 1.0)
        {
          z = 1 - 2 * dd;
          dd = 2 * Math.Sqrt(1.0 - dd);
          x = d1 * dd;
          y = d2 * dd;
          return;
        }
      }
    }

    public override double Minimum
    {
      get { throw new Exception("The method or operation is not implemented."); }
    }

    public override double Maximum
    {
      get { throw new Exception("The method or operation is not implemented."); }
    }

    public override double Mean
    {
      get { throw new Exception("The method or operation is not implemented."); }
    }

    public override double Median
    {
      get { throw new Exception("The method or operation is not implemented."); }
    }

    public override double Variance
    {
      get { throw new Exception("The method or operation is not implemented."); }
    }

    public override double[] Mode
    {
      get { throw new Exception("The method or operation is not implemented."); }
    }
  }
}

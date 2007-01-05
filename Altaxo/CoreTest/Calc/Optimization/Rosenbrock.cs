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

/*
 * RosenBrock.cs
 * 
 * Copyright (c) 2004, dnAnalytics Project. All rights reserved.
*/

using System;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Optimization;

namespace AltaxoTest.Calc.Optimization 
{
  ///<summary>Rosenbrock Function</summary>
  ///<remarks>The Rosenbrock Function is typically used to test optimization algorithms.  It has a
  /// global minimum of 0 at point (1,1). </remarks>
  public sealed class Rosenbrock : CostFunction
  {
    public override double Value (DoubleVector x) 
    {
      double retvalue=0;
      for (int i=1; i<x.Length; i++) 
      {
        retvalue = retvalue + 100*System.Math.Pow((x[i] - System.Math.Pow(x[i-1],2)),2) + System.Math.Pow((1-x[i-1]),2);
      }
      return retvalue;
    }
    
    public override DoubleVector Gradient(DoubleVector x) 
    {
      DoubleVector retvalue = new DoubleVector(x.Length,0.0);
      retvalue[0] = -400*x[0]*(x[1]-System.Math.Pow(x[0],2))-2*(1-x[0]);
      retvalue[x.Length-1] = 200*(x[x.Length-1]-System.Math.Pow(x[x.Length-2],2));
      if (x.Length>2) 
      {
        for (int i=1; i<x.Length-1; i++)
          retvalue[i] = 200*(x[i]-System.Math.Pow(x[i-1],2))-400*x[i]*(x[i+1]-System.Math.Pow(x[i],2))-2*(1-x[i]);
      }
      return retvalue;
    }
    
    public override DoubleMatrix Hessian(DoubleVector x) 
    {
      DoubleMatrix ret = new DoubleMatrix(x.Length,x.Length,0.0);
      
      for (int i=0; i<x.Length-1; i++)
      {
        ret[i,i+1] = -400*x[i];
        ret[i+1,i] = -400*x[i];
      }
      ret[0,0] = System.Math.Pow(1200*x[0],2)-400*x[1]+2;
      ret[x.Length-1,x.Length-1] = 200;
      for (int i=1; i<x.Length-1; i++)
        ret[i,i] = 202 + System.Math.Pow(1200*x[i],2) - 400*x[i+1];
      return ret;
    } 

  } 
}

#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
using Altaxo.Calc;
using Altaxo.Calc.Regression;

using NUnit.Framework;
namespace AltaxoTest.Calc.Regression
{

  [TestFixture]
  public class TestNonlinearFit2
  {
    const double ROSD = 105.0;

    /* Rosenbrock function, global minimum at (1, 1) */
    void ros(double[] p, double[] x, object data)
    {
      int i;
      int m=p.Length;
      int n=x.Length;

      for(i=0; i<n; ++i)
        x[i]=((1.0-p[0])*(1.0-p[0]) + ROSD*(p[1]-p[0]*p[0])*(p[1]-p[0]*p[0]));
    }

    void jacros(double[] p, double[] jac,  object data)
    {
      int i, j;
      int m=p.Length;
      int n=jac.Length/m;
  
      for(i=j=0; i<n; ++i)
      {
        jac[j++]=(-2 + 2*p[0]-4*ROSD*(p[1]-p[0]*p[0])*p[0]);
        jac[j++]=(2*ROSD*(p[1]-p[0]*p[0]));
      }
    }
    [Test]
    public void Rosenbrock_Der()
    {
      const double LM_INIT_MU = 1E-3;
      const double LM_DIFF_DELTA = 1E-6;
      double[] opts = new double[5];
      opts[0] = LM_INIT_MU; opts[1] = 1E-15; opts[2] = 1E-15; opts[3] = 1E-20;
      opts[4] = LM_DIFF_DELTA; // relevant only if the finite difference jacobian version is used 

      double[] info = new double[9];
      int m, n;
      object tempstorage=null;

      /* Rosenbrock function */
      m = 2; n = 2;
      double[] p = new double[m];
      double[] x = new double[n];
      p[0] = -1.2; p[1] = 1.0;
      for (int i = 0; i < n; i++) x[i] = 0.0;
      int ret = Altaxo.Calc.Regression.NonLinearFit2.LEVMAR_DER(
        new Altaxo.Calc.Regression.NonLinearFit2.FitFunction(ros),
        new Altaxo.Calc.Regression.NonLinearFit2.JacobianFunction(jacros),
        p, x, null, 1000, opts, info, ref tempstorage, null, null); // with analytic jacobian
      //ret=dlevmar_dif(modros, p, x, m, n, 1000, opts, info, NULL, NULL, NULL);  // no jacobian
      Assert.AreEqual(1, p[0], 0.1);
      Assert.AreEqual(1, p[1], 0.12);

    }


    /// <summary>
    /// Generates for values z=1,2,3..n the function p[0]+p[1]*z[i]
    /// </summary>
    /// <param name="p"></param>
    /// <param name="x"></param>
    /// <param name="data"></param>
    void poly(double[] p, double[] x, object data)
    {
      int i;
      int m=p.Length;
      int n=x.Length;

      for(i=0; i<n; ++i)
      {
        double z=1;
        double res=0;
        for(int k=0; k<m; ++k)
        {
          res += p[k] *z;
          z *= (i+1);
        }
        x[i] = res;
      }
    }

    /// <summary>
    /// Generates for values z=1,2,3..n the derivative df/dp[i] = 1, z, z^2, z^3
    /// </summary>
    /// <param name="p"></param>
    /// <param name="jac"></param>
    /// <param name="data"></param>
    void jacpoly(double[] p, double[] jac,  object data)
    {
      int i, j;
      int m=p.Length;
      int n=jac.Length/m;
  
      for(i=j=0; i<n; ++i)
      {
        double z=1;
        for(int k=0;k<m;++k)
        {
          jac[j++]=z;
          z *= (i+1);
        }
      }
    }

    [Test]
    public void Polynom2Der()
    {
      const double LM_INIT_MU = 1E-3;
      const double LM_DIFF_DELTA = 1E-6;
      double[] opts = new double[5];
      opts[0] = LM_INIT_MU; opts[1] = 1E-15; opts[2] = 1E-15; opts[3] = 1E-20;
      opts[4] = LM_DIFF_DELTA; // relevant only if the finite difference jacobian version is used 

      double[] info = new double[9];
      int m, n;
      object tempstorage=null;

      
      m = 2; n = 9;
      double[] p = new double[m];
      double[] x = new double[n];
      p[0] = 10; p[1] = 2.0;

      x[0] = 2; x[1] = 1;
      x[2] = 4; x[3] = 3;
      x[4] = 6; x[5] = 5;
      x[6] = 8; x[7] = 7;
      x[8] = 10;

      double[] covar = new double[m*m];

      int ret = Altaxo.Calc.Regression.NonLinearFit2.LEVMAR_DER(
        new Altaxo.Calc.Regression.NonLinearFit2.FitFunction(poly),
        new Altaxo.Calc.Regression.NonLinearFit2.JacobianFunction(jacpoly),
        p, x, null, 1000, opts, info, ref tempstorage, covar, null); // with analytic jacobian
      //ret=dlevmar_dif(modros, p, x, m, n, 1000, opts, info, NULL, NULL, NULL);  // no jacobian


      Assert.AreEqual(0.11111111, p[0], 0.001);
      Assert.AreEqual(1, p[1], 0.001);

      Assert.AreEqual(0.670194,covar[0],0.001);
      Assert.AreEqual(-0.10582,covar[1],0.001);
      Assert.AreEqual(-0.10582,covar[2],0.001);
      Assert.AreEqual(0.021164,covar[3],0.001);

    }

    [Test]
    public void Polynom2Diff()
    {
      const double LM_INIT_MU = 1E-3;
      const double LM_DIFF_DELTA = 1E-6;
      double[] opts = new double[5];
      opts[0] = LM_INIT_MU; opts[1] = 1E-15; opts[2] = 1E-15; opts[3] = 1E-20;
      opts[4] = LM_DIFF_DELTA; // relevant only if the finite difference jacobian version is used 

      double[] info = new double[9];
      int m, n;
      object tempstorage=null;

    
      m = 2; n = 9;
      double[] p = new double[m];
      double[] x = new double[n];
      p[0] = 0.1; p[1] = 1.01;

      x[0] = 2; x[1] = 1;
      x[2] = 4; x[3] = 3;
      x[4] = 6; x[5] = 5;
      x[6] = 8; x[7] = 7;
      x[8] = 10;

      double[] covar = new double[m*m];

      int ret = Altaxo.Calc.Regression.NonLinearFit2.LEVMAR_DIF(
        new Altaxo.Calc.Regression.NonLinearFit2.FitFunction(poly),
        p, x, 1000, opts, info, ref tempstorage, covar, null); // with analytic jacobian
      //ret=dlevmar_dif(modros, p, x, m, n, 1000, opts, info, NULL, NULL, NULL);  // no jacobian


      Assert.AreEqual(0.11111111, p[0], 0.001);
      Assert.AreEqual(1, p[1], 0.001);

      Assert.AreEqual(0.670194,covar[0],0.001);
      Assert.AreEqual(-0.10582,covar[1],0.001);
      Assert.AreEqual(-0.10582,covar[2],0.001);
      Assert.AreEqual(0.021164,covar[3],0.001);

    }


    /// <summary>
    /// Generates the values sum from i=1 to 9 of p[0]+p[1]*i - y
    /// </summary>
    /// <param name="p"></param>
    /// <param name="x"></param>
    /// <param name="data"></param>
    void poly3(double[] p, double[] x, object data)
    {
      if(p.Length!=2)
        throw new ArgumentException("p");

      double[] y = {2,1,4,3,6,5,8,7,10};

      double sum = 0;
      for(int i=0; i<9; ++i)
      {
        double diff = p[0] + p[1]*(i+1) - y[i];
        sum += diff*diff;
      }

      sum /= x.Length;
      sum = Math.Sqrt(sum);
      for(int i=0;i<x.Length;++i)
        x[i] = sum;

      System.Diagnostics.Debug.WriteLine(string.Format("p[0]={0}, p[1]={1}, sum={2}",p[0],p[1],sum));

    }

    /*
    [Test]
    public void Polynom3Diff()
    {
      const double LM_INIT_MU = 1E-3;
      const double LM_DIFF_DELTA = 1E-6;
      double[] opts = new double[5];
      opts[0] = LM_INIT_MU; opts[1] = 1E-15; opts[2] = 1E-15; opts[3] = 1E-20;
      opts[4] = LM_DIFF_DELTA; // relevant only if the finite difference jacobian version is used 

      double[] info = new double[9];
      int m, n;
      object tempstorage=null;

    
      m = 2; n = 2;
      double[] p = new double[m];
      double[] x = new double[n];
      p[0] = 0.1; p[1] = 1.01;

      x[0] = 0; x[1] = 0;
     

      double[] covar = new double[m*m];

      int ret = Altaxo.Calc.Regression.NonLinearFit2.LEVMAR_DIF(
        new Altaxo.Calc.Regression.NonLinearFit2.FitFunction(poly3),
        p, x, 1000, opts, info, ref tempstorage, covar, null); // with analytic jacobian
      //ret=dlevmar_dif(modros, p, x, m, n, 1000, opts, info, NULL, NULL, NULL);  // no jacobian


      Assert.AreEqual(0.11111111, p[0], 0.001);
      Assert.AreEqual(1, p[1], 0.001);

      Assert.AreEqual(0.670194,covar[0],0.001);
      Assert.AreEqual(-0.10582,covar[1],0.001);
      Assert.AreEqual(-0.10582,covar[2],0.001);
      Assert.AreEqual(0.021164,covar[3],0.001);

    }
    */

  }
}

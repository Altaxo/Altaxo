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
  public class Test_enorm
  {
    protected double[] arr = new double[90001];


    [Test]
    public void ZeroElements()
    {
      arr[0]=1;
      double r = NLFit.enorm(0,arr);
      NUnit.Framework.Assert.AreEqual(0.0,r,0);
    }

    [Test]
    public void OneElement()
    {
      for(int i=0;i<arr.Length;i++) 
        arr[i] = (i+1);
      
      for(int i=0;i<arr.Length;i++) 
      {
        double r = NLFit.enorm(1,arr,i);
        NUnit.Framework.Assert.AreEqual((double)(i+1),r,0);
      }
    }


    [Test]
    public void NormalElements()
    {
      // we use the law 3^2 + 4^2 = 5^2
      arr[0]=400;;
      for(int i=1;i<90001;i++) 
        arr[i] = 1;
      double r = NLFit.enorm(90001,arr);
      NUnit.Framework.Assert.AreEqual(500,r,0);
    }

    [Test]
    public void NormalAndBigElements()
    {
      // we use the law 3^2 + 4^2 = 5^2
      arr[0]=400E19;;
      for(int i=1;i<90001;i++) 
        arr[i] = 1E19;
      double r = NLFit.enorm(90001,arr);
      NUnit.Framework.Assert.AreEqual(500E19,r,100E10);
    }

    [Test]
    public void NormalAndSmallElements()
    {
      // we use the law 3^2 + 4^2 = 5^2
      arr[0]=400E-20;;
      for(int i=1;i<90001;i++) 
        arr[i] = 1E-20;
      double r = NLFit.enorm(90001,arr);
      NUnit.Framework.Assert.AreEqual(500E-20,r,0);
    }
  }



  [TestFixture]
  public class Test_LevenbergMarquardtFit
  {
    public void FitFunction2plus5x(int numberOfYs, int numberOfParameter, double[] parameter, double[] ys, ref int info)
    {
      Assert.IsTrue( numberOfParameter==2, "NumberOfParameter must be 2 in this test.");
      Assert.IsTrue(numberOfYs==2,"NumberOfYs must be 2 in this test.");
      Assert.IsTrue(parameter.Length==2,"Length of parameter array must be 2 in this test.");
      Assert.IsTrue(ys.Length==2,"Length of ys array must be 2 in this test.");

      // we use the simplest model y=a+bx in the range -10 ..10 with step size of 1 to
      // calculate the delta
      double a = parameter[0];
      double b = parameter[1];

      double sum_se = 0;
      double x;
      for(x=-10;x<=10;x+=1)
        sum_se += NLFit.sqr( (a+b*x) - (2 + 5*x));

      ys[0] = sum_se;
      ys[1] = sum_se;
    }


    public void FitFunction2plus5xMod(int numberOfYs, int numberOfParameter, double[] parameter, double[] ys, ref int info)
    {
      Assert.IsTrue(numberOfParameter==2,"NumberOfParameter must be 2 in this test.");
      Assert.IsTrue(numberOfYs==1, "NumberOfYs must be 1 in this test.");
      Assert.IsTrue(parameter.Length==2, "Length of parameter array must be 2 in this test.");
      Assert.IsTrue(ys.Length==1,"Length of ys array must be 1 in this test.");

      // we use the simplest model y=a+bx in the range -10 ..10 with step size of 1 to
      // calculate the delta
      double a = parameter[0];
      double b = parameter[1];

      double sum_se = 0;
      double x;
      for(x=-10;x<=10;x+=1)
        sum_se += NLFit.sqr( (a+b*x) - (2 + 5*x));

      ys[0] = sum_se;   
    }

  



    [Test]
    public void Test_2plus5x()
    {
      double[] param = new double[2];
      param[0]=1;
      param[1]=1;
      double[] ys = new double[2];
      int info = 0;

      NLFit.LevenbergMarquardtFit(new NLFit.LMFunction(this.FitFunction2plus5x),param,ys,1E-10,ref info);
      
      Assert.AreEqual(2,param[0],1E-5,"Fit parameter 0 should be 2 in this model");
      Assert.AreEqual(5,param[1],1E-5,"Fit parameter 1 should be 5 in this model");
    }


    [Test]
    public void Test_2plus5xMod()
    {
      double[] param = new double[2];
      param[0]=1;
      param[1]=1;
      double[] ys = new double[1];
      int info = 0;

      NLFit.LevenbergMarquardtFit(new NLFit.LMFunction(this.FitFunction2plus5xMod),param,ys,1E-10,ref info);
      
      Assert.AreEqual(0,info,"Info should be 0 due to inappropriate length of ys in this model");
    }

    public void FitFunction7malCos3xplus1(int numberOfYs, int numberOfParameter, double[] parameter, double[] ys, ref int info)
    {
      Assert.IsTrue(numberOfParameter==3,"NumberOfParameter must be 3 in this test.");
      Assert.IsTrue(numberOfYs==3,"NumberOfYs must be 3 in this test.");
      Assert.IsTrue(parameter.Length==3,"Length of parameter array must be 3 in this test.");
      Assert.IsTrue(ys.Length==3,"Length of ys array must be 3 in this test.");

      // we use the simplest model y=a+bx in the range -10 ..10 with step size of 1 to
      // calculate the delta
      double a = parameter[0];
      double b = parameter[1];
      double c = parameter[2];

      double sum_se = 0;
      double x;
      for(x=-10;x<=10;x+=1)
        sum_se += NLFit.sqr( (a*Math.Cos(b*x+c)) - (7*Math.Cos(3*x+1)));

      ys[0] = sum_se;
      ys[1] = 0;
      ys[2] = 0;
    }
  
    [Test]
    public void Test_7malCos3xplus1()
    {
      double[] param = new double[3];
      param[0]=1;
      param[1]=3.1;
      param[2]=1.1;

      double[] ys = new double[3];
      int info = 0;

      NLFit.LevenbergMarquardtFit(new NLFit.LMFunction(this.FitFunction7malCos3xplus1),param,ys,1E-10,ref info);
      
      Assert.AreEqual(7,param[0],1E-4,"Fit parameter 0 should be 7 in this model");
      Assert.AreEqual(3,param[1],1E-4,"Fit parameter 1 should be 3 in this model");
      Assert.AreEqual(1,param[2],1E-4,"Fit parameter 2 should be 1 in this model");
    }


    public void FitFunction7malCos3xplus1Mod(int numberOfYs, int numberOfParameter, double[] parameter, double[] ys, ref int info)
    {
      Assert.IsTrue(numberOfParameter==3,"NumberOfParameter must be 3 in this test.");
      Assert.IsTrue(numberOfYs==21,"NumberOfYs must be 21 in this test.");
      Assert.IsTrue(parameter.Length==3,"Length of parameter array must be 3 in this test.");
      Assert.IsTrue(ys.Length==21,"Length of ys array must be 21 in this test.");

      // we use the simplest model y=a+bx in the range -10 ..10 with step size of 1 to
      // calculate the delta
      double a = parameter[0];
      double b = parameter[1];
      double c = parameter[2];

      for(int i=-10;i<=10;i++)
        ys[i+10] = NLFit.sqr( (a*Math.Cos(b*i+c)) - (7*Math.Cos(3*i+1)));
    }
  
    [Test]
    public void Test_7malCos3xplus1Mod()
    {
      double[] param = new double[3];
      param[0]=1;
      param[1]=3.1;
      param[2]=1.1;

      double[] ys = new double[21];
      int info = 0;

      do
      {
        NLFit.LevenbergMarquardtFit(new NLFit.LMFunction(this.FitFunction7malCos3xplus1Mod),param,ys,1E-10,ref info);
      } while(info==5);

      Assert.AreEqual(7,param[0],1E-4,"Fit parameter 0 should be 7 in this model");
      Assert.AreEqual(3,param[1],1E-4,"Fit parameter 1 should be 3 in this model");
      Assert.AreEqual(1,param[2],1E-4,"Fit parameter 2 should be 1 in this model");
    }


  
    /// <summary>
    /// Generates for values z=1,2,3..n the function p[0]+p[1]*z[i]
    /// </summary>
    /// <param name="p"></param>
    /// <param name="x"></param>
    /// <param name="data"></param>
    void poly2(int numberOfYs, int numberOfParameter, double[] p, double[] x, ref int info)
    {
      Assert.IsTrue(p.Length==2);
      Assert.IsTrue(x.Length==9);

      double[] y = {2,1,4,3,6,5,8,7,10};

      for(int i=0;i<9;i++)
        x[i] = p[0] + p[1]*(i+1) - y[i];  
    
    }


    [Test]
    public void Test_Poly2()
    {
      double[] param = new double[2];
      param[0]=0.1;
      param[1]=1.01;
      

      double[] ys = new double[9];
      int info = 0;

      do
      {
        NLFit.LevenbergMarquardtFit(new NLFit.LMFunction(this.poly2),param,ys,1E-10,ref info);
      } while(info==5);

      Assert.AreEqual(0.1111111,param[0],1E-4,"Fit parameter 0 should be 0.11111 in this model");
      Assert.AreEqual(1,param[1],1E-4,"Fit parameter 1 should be 1 in this model");

      double[] covar = new double[2*2];
      double chisqr;
      NLFit.ComputeCovariances(new NLFit.LMFunction(this.poly2),param,9,2,covar, out chisqr);
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
    void poly3(int numberOfYs, int numberOfParameter, double[] p, double[] x, ref int info)
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

    [Test]
    public void Test_Poly3()
    {
      double[] param = new double[2];
      param[0]=1;
      param[1]=1;
      

      double[] ys = new double[2];
      int info = 0;

      do
      {
        NLFit.LevenbergMarquardtFit(new NLFit.LMFunction(this.poly3),param,ys,1E-10,ref info);
      } while(info==5);

      Assert.AreEqual(0.1111111,param[0],1E-4,"Fit parameter 0 should be 0.11111 in this model");
      Assert.AreEqual(1,param[1],1E-4,"Fit parameter 1 should be 1 in this model");

      double[] covar = new double[2*2];
      double chisqr;
      NLFit.ComputeCovariances(new NLFit.LMFunction(this.poly3),param,2,2,covar, out chisqr);
     
    }
  
  }


  /// <summary>
  /// Summary description for Class1.
  /// </summary>
  class MainMain
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
      double[] arr = { 1E20, 1, 1E-20 };
      double result = NLFit.enorm(3,arr);
    }
  }
}

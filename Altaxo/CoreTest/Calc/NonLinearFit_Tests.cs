#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
using NUnit.Framework;
namespace AltaxoTest.Calc.NLFIT
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
      NUnit.Framework.Assertion.AssertEquals(0.0,r,0);
    }

    [Test]
    public void OneElement()
    {
      for(int i=0;i<arr.Length;i++) 
        arr[i] = (i+1);
      
      for(int i=0;i<arr.Length;i++) 
      {
        double r = NLFit.enorm(1,arr,i);
        NUnit.Framework.Assertion.AssertEquals((double)(i+1),r,0);
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
      NUnit.Framework.Assertion.AssertEquals(500,r,0);
    }

    [Test]
    public void NormalAndBigElements()
    {
      // we use the law 3^2 + 4^2 = 5^2
      arr[0]=400E19;;
      for(int i=1;i<90001;i++) 
        arr[i] = 1E19;
      double r = NLFit.enorm(90001,arr);
      NUnit.Framework.Assertion.AssertEquals(500E19,r,100E10);
    }

    [Test]
    public void NormalAndSmallElements()
    {
      // we use the law 3^2 + 4^2 = 5^2
      arr[0]=400E-20;;
      for(int i=1;i<90001;i++) 
        arr[i] = 1E-20;
      double r = NLFit.enorm(90001,arr);
      NUnit.Framework.Assertion.AssertEquals(500E-20,r,0);
    }
  }



  [TestFixture]
  public class Test_LevenbergMarquardtFit
  {
    public void FitFunction2plus5x(int numberOfYs, int numberOfParameter, double[] parameter, double[] ys, ref int info)
    {
      Assertion.Assert("NumberOfParameter must be 2 in this test.",numberOfParameter==2);
      Assertion.Assert("NumberOfYs must be 2 in this test.",numberOfYs==2);
      Assertion.Assert("Length of parameter array must be 2 in this test.",parameter.Length==2);
      Assertion.Assert("Length of ys array must be 2 in this test.",ys.Length==2);

      // we use the simplest model y=a+bx in the range -10 ..10 with step size of 1 to
      // calculate the delta
      double a = parameter[0];
      double b = parameter[1];

      double sum_se = 0;
      double x;
      for(x=-10;x<=10;x+=1)
        sum_se += Altaxo.Calc.NLFit.sqr( (a+b*x) - (2 + 5*x));

      ys[0] = sum_se;
      ys[1] = sum_se;
    }


    public void FitFunction2plus5xMod(int numberOfYs, int numberOfParameter, double[] parameter, double[] ys, ref int info)
    {
      Assertion.Assert("NumberOfParameter must be 2 in this test.",numberOfParameter==2);
      Assertion.Assert("NumberOfYs must be 1 in this test.",numberOfYs==1);
      Assertion.Assert("Length of parameter array must be 2 in this test.",parameter.Length==2);
      Assertion.Assert("Length of ys array must be 1 in this test.",ys.Length==1);

      // we use the simplest model y=a+bx in the range -10 ..10 with step size of 1 to
      // calculate the delta
      double a = parameter[0];
      double b = parameter[1];

      double sum_se = 0;
      double x;
      for(x=-10;x<=10;x+=1)
        sum_se += Altaxo.Calc.NLFit.sqr( (a+b*x) - (2 + 5*x));

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

      Altaxo.Calc.NLFit.LevenbergMarquardtFit(new NLFit.LMFunction(this.FitFunction2plus5x),param,ys,1E-10,ref info);
      
      Assertion.AssertEquals("Fit parameter 0 should be 2 in this model",2,param[0],1E-5);
      Assertion.AssertEquals("Fit parameter 1 should be 5 in this model",5,param[1],1E-5);
    }


    [Test]
    public void Test_2plus5xMod()
    {
      double[] param = new double[2];
      param[0]=1;
      param[1]=1;
      double[] ys = new double[1];
      int info = 0;

      Altaxo.Calc.NLFit.LevenbergMarquardtFit(new NLFit.LMFunction(this.FitFunction2plus5xMod),param,ys,1E-10,ref info);
      
      Assertion.AssertEquals("Info should be 0 due to inappropriate length of ys in this model",0,info);
    }

    public void FitFunction7malCos3xplus1(int numberOfYs, int numberOfParameter, double[] parameter, double[] ys, ref int info)
    {
      Assertion.Assert("NumberOfParameter must be 3 in this test.",numberOfParameter==3);
      Assertion.Assert("NumberOfYs must be 3 in this test.",numberOfYs==3);
      Assertion.Assert("Length of parameter array must be 3 in this test.",parameter.Length==3);
      Assertion.Assert("Length of ys array must be 3 in this test.",ys.Length==3);

      // we use the simplest model y=a+bx in the range -10 ..10 with step size of 1 to
      // calculate the delta
      double a = parameter[0];
      double b = parameter[1];
      double c = parameter[2];

      double sum_se = 0;
      double x;
      for(x=-10;x<=10;x+=1)
        sum_se += Altaxo.Calc.NLFit.sqr( (a*Math.Cos(b*x+c)) - (7*Math.Cos(3*x+1)));

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

      Altaxo.Calc.NLFit.LevenbergMarquardtFit(new NLFit.LMFunction(this.FitFunction7malCos3xplus1),param,ys,1E-10,ref info);
      
      Assertion.AssertEquals("Fit parameter 0 should be 7 in this model",7,param[0],1E-4);
      Assertion.AssertEquals("Fit parameter 1 should be 3 in this model",3,param[1],1E-4);
      Assertion.AssertEquals("Fit parameter 2 should be 1 in this model",1,param[2],1E-4);
    }


    public void FitFunction7malCos3xplus1Mod(int numberOfYs, int numberOfParameter, double[] parameter, double[] ys, ref int info)
    {
      Assertion.Assert("NumberOfParameter must be 3 in this test.",numberOfParameter==3);
      Assertion.Assert("NumberOfYs must be 21 in this test.",numberOfYs==21);
      Assertion.Assert("Length of parameter array must be 3 in this test.",parameter.Length==3);
      Assertion.Assert("Length of ys array must be 21 in this test.",ys.Length==21);

      // we use the simplest model y=a+bx in the range -10 ..10 with step size of 1 to
      // calculate the delta
      double a = parameter[0];
      double b = parameter[1];
      double c = parameter[2];

      for(int i=-10;i<=10;i++)
        ys[i+10] = Altaxo.Calc.NLFit.sqr( (a*Math.Cos(b*i+c)) - (7*Math.Cos(3*i+1)));
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
        Altaxo.Calc.NLFit.LevenbergMarquardtFit(new NLFit.LMFunction(this.FitFunction7malCos3xplus1Mod),param,ys,1E-10,ref info);
      } while(info==5);

      Assertion.AssertEquals("Fit parameter 0 should be 7 in this model",7,param[0],1E-4);
      Assertion.AssertEquals("Fit parameter 1 should be 3 in this model",3,param[1],1E-4);
      Assertion.AssertEquals("Fit parameter 2 should be 1 in this model",1,param[2],1E-4);
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
      double result = Altaxo.Calc.NLFit.enorm(3,arr);
    }
  }
}

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
using NUnit.Framework;

namespace Altaxo.Calc.FFT
{

  [TestFixture]
  public class TestChirpFFT
  {
    const int nLowerLimit=5;
    const int nUpperLimit=100;
    const double maxTolerableEpsPerN=1E-15;

   
   


    private static void zzTestZero(uint n)
    {
      double[] re = new double[n];
      double[] im = new double[n];


      ChirpFFT.FFT(re,im,n,false);

      for(uint i=0;i<n;i++)
      {
        Assertion.AssertEquals("FFT of zero should give re=0", 0, re[i],0);
        Assertion.AssertEquals("FFT of zero should give im=0", 0, im[i],0);
      }
    }

    [Test]
    public void TestZero()
    {
      
      for(uint i=nLowerLimit;i<=nUpperLimit;i++)
        zzTestZero(i);
    }

  

  

    private static void zzTestReOne_ZeroPos(uint n)
    {
      double[] re = new double[n];
      double[] im = new double[n];

      re[0] = 1;

     

      ChirpFFT.FFT(re,im,n,false);
      
      for(uint i=0;i<n;i++)
      {
        Assertion.AssertEquals("FFT of 1 at pos 0 should give re=1", 1, re[i],n*maxTolerableEpsPerN);
        Assertion.AssertEquals("FFT of 1 at pos 0 should give im=0", 0, im[i],n*maxTolerableEpsPerN);
      }
    }


    [Test]
    public void TestReOne_ZeroPos()
    {
      // Testing 2^n
      for(uint i=nLowerLimit;i<=nUpperLimit;i++)
        zzTestReOne_ZeroPos(i);
    }


    private static void zzTestImOne_ZeroPos(uint n)
    {
      double[] re = new double[n];
      double[] im = new double[n];

      im[0] = 1;

      ChirpFFT.FFT(re,im,n,false);

      for(uint i=0;i<n;i++)
      {
        Assertion.AssertEquals("FFT of im 1 at pos 0 should give re=0", 0, re[i],n*maxTolerableEpsPerN);
        Assertion.AssertEquals("FFT of im 1 at pos 0 should give im=1", 1, im[i],n*maxTolerableEpsPerN);
      }
    }

    [Test]
    public void TestImOne_ZeroPos()
    {
      // Testing 2^n
      for(uint i=nLowerLimit;i<=nUpperLimit;i++)
        zzTestImOne_ZeroPos(i);
    }


    private static void zzTestReOne_OnePos(uint n)
    {
      double[] re = new double[n];
      double[] im = new double[n];

      re[1] = 1;

      ChirpFFT.FFT(re,im,n,false);

      for(uint i=0;i<n;i++)
      {
        Assertion.AssertEquals(string.Format("FFT({0}) of re 1 at pos 1 re[{1}]",n,i), Math.Cos((2*Math.PI*i)/n), re[i],n*maxTolerableEpsPerN);
        Assertion.AssertEquals(string.Format("FFT({0}) of re 1 at pos 1 im[{1}]",n,i), -Math.Sin((2*Math.PI*i)/n), im[i],n*maxTolerableEpsPerN);
      }
    }


    [Test]
    public void TestReOne_OnePos()
    {
      // Testing 2^n
      for(uint i=nLowerLimit;i<=nUpperLimit;i++)
        zzTestReOne_OnePos(i);
    }
  


    private static void zzTestImOne_OnePos(uint n)
    {
      double[] re = new double[n];
      double[] im = new double[n];

      im[1] = 1;

     
      ChirpFFT.FFT(re,im,n,false);

      for(uint i=0;i<n;i++)
      {
        Assertion.AssertEquals(string.Format("FFT({0}) of im 1 at pos 1 re[{1}]",n,i), Math.Sin((2*Math.PI*i)/n), re[i],n*maxTolerableEpsPerN);
        Assertion.AssertEquals(string.Format("FFT({0}) of im 1 at pos 1 im[{1}]",n,i), Math.Cos((2*Math.PI*i)/n), im[i],n*maxTolerableEpsPerN);
      }
    }

    
    [Test]
    public void TestImOne_OnePos()
    {
      // Testing 2^n
      for(uint i=nLowerLimit;i<=nUpperLimit;i++)
        zzTestImOne_OnePos(i);
    }

   



    private static void zzTestReImOne_RandomPos(uint n)
    {
      double[] re = new double[n];
      double[] im = new double[n];
      
      System.Random rnd = new System.Random();

      int repos = rnd.Next((int)n);
      int impos = rnd.Next((int)n);

      re[repos]=1;
      im[impos]=1;

     

      ChirpFFT.FFT(re,im,n,false);

      for(uint i=0;i<n;i++)
      {
        Assertion.AssertEquals(string.Format("FFT({0}) of im 1 at pos(re={1},im={2}) re[{3}]",n,repos,impos,i), 
          Math.Cos((2*Math.PI*i*(double)repos)/n) + Math.Sin((2*Math.PI*i*(double)impos)/n),
          re[i],n*1E-14);
        Assertion.AssertEquals(string.Format("FFT({0}) of im 1 at pos(re={1},im={2}) arb im[{3}]",n,repos,impos,i), 
          -Math.Sin((2*Math.PI*i*(double)repos)/n) + Math.Cos((2*Math.PI*i*(double)impos)/n), 
          im[i],n*1E-14);
      }
    }


    [Test]
    public void TestReImOne_RandomPos()
    {
      // Testing 2^n
      for(uint i=nLowerLimit;i<=nUpperLimit;i++)
        zzTestReImOne_RandomPos(i);
    }

    


  }



  
}

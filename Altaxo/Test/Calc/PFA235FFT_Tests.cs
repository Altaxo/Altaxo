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
	public class TestPFA235FFT_1D_Inverse
	{

		[Test]
		public void TestZero2N()
		{
			// Testing 2^n
			for(int i=2;i<=65536;i*=2)
				TestZero(i);
		}

		[Test]
		public void TestZero3N()
		{
			// Testing 3^n
			for(int i=3;i<100000;i*=3)
				TestZero(i);
		}
	
		[Test]
		public void TestZero5N()
		{
			// Testing 5^n
			for(int i=5;i<100000;i*=5)
				TestZero(i);
		}


		private static void TestZero(int n)
		{
			double[] re = new double[n];
			double[] im = new double[n];

			Pfa235FFT fft = new Pfa235FFT(n);

			fft.FFT(re,im,Pfa235FFT.Direction.Inverse);

			for(int i=0;i<n;i++)
			{
				Assertion.AssertEquals("FFT of zero should give re=0", 0, re[i],0);
				Assertion.AssertEquals("FFT of zero should give im=0", 0, im[i],0);
			}
		}
	

		[Test]
		public void TestReOne_ZeroPos2N()
		{
			// Testing 2^n
			for(int i=2;i<=65536;i*=2)
				TestReOne_ZeroPos(i);
		}

		[Test]
		public void TestReOne_ZeroPos3N()
		{
			// Testing 3^n
			for(int i=3;i<100000;i*=3)
				TestReOne_ZeroPos(i);
		}
	
		[Test]
		public void TestReOne_ZeroPos5N()
		{
			// Testing 5^n
			for(int i=5;i<100000;i*=5)
				TestReOne_ZeroPos(i);
		}

		private static void TestReOne_ZeroPos(int n)
		{
			double[] re = new double[n];
			double[] im = new double[n];

			re[0] = 1;

			Pfa235FFT fft = new Pfa235FFT(n);

			fft.FFT(re,im,Pfa235FFT.Direction.Inverse);
			
			for(int i=0;i<n;i++)
			{
				Assertion.AssertEquals("FFT of 1 at pos 0 should give re=1", 1, re[i],0);
				Assertion.AssertEquals("FFT of 1 at pos 0 should give im=0", 0, im[i],0);
			}
		}


		[Test]
		public void TestImOne_ZeroPos2N()
		{
			// Testing 2^n
			for(int i=2;i<=65536;i*=2)
				TestImOne_ZeroPos(i);
		}

		[Test]
		public void TestImOne_ZeroPos3N()
		{
			// Testing 3^n
			for(int i=3;i<100000;i*=3)
				TestImOne_ZeroPos(i);
		}
	
		[Test]
		public void TestImOne_ZeroPos5N()
		{
			// Testing 5^n
			for(int i=5;i<100000;i*=5)
				TestImOne_ZeroPos(i);
		}

		[Test]
		public void TestImOne_ZeroPosArbN()
		{
			TestImOne_ZeroPos(2*2*2*3*3*5);
			TestImOne_ZeroPos(2*2*3*3*3*5);
			TestImOne_ZeroPos(2*3*3*5*5*5);
		}

		private static void TestImOne_ZeroPos(int n)
		{
			double[] re = new double[n];
			double[] im = new double[n];

			im[0] = 1;

			Pfa235FFT fft = new Pfa235FFT(n);

			fft.FFT(re,im,Pfa235FFT.Direction.Inverse);

			for(int i=0;i<n;i++)
			{
				Assertion.AssertEquals("FFT of im 1 at pos 0 should give re=0", 0, re[i],0);
				Assertion.AssertEquals("FFT of im 1 at pos 0 should give im=1", 1, im[i],0);
			}
		}


		[Test]
		public void TestReOne_OnePos2N()
		{
			// Testing 2^n
			for(int i=2;i<=65536;i*=2)
				TestReOne_OnePos(i);
		}

		[Test]
		public void TestReOne_OnePos3N()
		{
			// Testing 3^n
			for(int i=3;i<100000;i*=3)
				TestReOne_OnePos(i);
		}
	
		[Test]
		public void TestReOne_OnePos5N()
		{
			// Testing 5^n
			for(int i=5;i<100000;i*=5)
				TestReOne_OnePos(i);
		}

		[Test]
		public void TestReOne_OnePosArbN()
		{
			TestReOne_OnePos(2*2*2*3*3*5);
			TestReOne_OnePos(2*2*3*3*3*5);
			TestReOne_OnePos(2*3*3*5*5*5);
		}

		private static void TestReOne_OnePos(int n)
		{
			double[] re = new double[n];
			double[] im = new double[n];

			re[1] = 1;

			Pfa235FFT fft = new Pfa235FFT(n);

			fft.FFT(re,im,Pfa235FFT.Direction.Inverse);

			for(int i=0;i<n;i++)
			{
				Assertion.AssertEquals(string.Format("FFT({0}) of re 1 at pos 1 re[{1}]",n,i), Math.Cos((2*Math.PI*i)/n), re[i],n*1E-15);
				Assertion.AssertEquals(string.Format("FFT({0}) of re 1 at pos 1 im[{1}]",n,i), -Math.Sin((2*Math.PI*i)/n), im[i],n*1E-15);
			}
		}
	

		[Test]
		public void TestImOne_OnePos2N()
		{
			// Testing 2^n
			for(int i=2;i<=65536;i*=2)
				TestImOne_OnePos(i);
		}

		[Test]
		public void TestImOne_OnePos3N()
		{
			// Testing 3^n
			for(int i=3;i<100000;i*=3)
				TestImOne_OnePos(i);
		}
	
		[Test]
		public void TestImOne_OnePos5N()
		{
			// Testing 5^n
			for(int i=5;i<100000;i*=5)
				TestImOne_OnePos(i);
		}

		[Test]
		public void TestImOne_OnePosArbN()
		{
			TestImOne_OnePos(2*2*2*3*3*5);
			TestImOne_OnePos(2*2*3*3*3*5);
			TestImOne_OnePos(2*3*3*5*5*5);
		}

		private static void TestImOne_OnePos(int n)
		{
			double[] re = new double[n];
			double[] im = new double[n];

			im[1] = 1;

			Pfa235FFT fft = new Pfa235FFT(n);

			fft.FFT(re,im,Pfa235FFT.Direction.Inverse);

			for(int i=0;i<n;i++)
			{
				Assertion.AssertEquals(string.Format("FFT({0}) of im 1 at pos 1 re[{1}]",n,i), Math.Sin((2*Math.PI*i)/n), re[i],n*1E-15);
				Assertion.AssertEquals(string.Format("FFT({0}) of im 1 at pos 1 im[{1}]",n,i), Math.Cos((2*Math.PI*i)/n), im[i],n*1E-15);
			}
		}


		[Test]
		public void TestReImOne_RandomPos2N()
		{
			// Testing 2^n
			for(int i=2;i<=65536;i*=2)
				TestReImOne_RandomPos(i);
		}

		[Test]
		public void TestReImOne_RandomPos3N()
		{
			// Testing 3^n
			for(int i=3;i<100000;i*=3)
				TestReImOne_RandomPos(i);
		}
	
		[Test]
		public void TestReImOne_RandomPos5N()
		{
			// Testing 5^n
			for(int i=5;i<100000;i*=5)
				TestReImOne_RandomPos(i);
		}

		[Test]
		public void TestReImOne_RandomPosArbN()
		{
			TestReImOne_RandomPos(2*2*2*3*3*5);
			TestReImOne_RandomPos(2*2*3*3*3*5);
			TestReImOne_RandomPos(2*3*3*5*5*5);
		}


		private static void TestReImOne_RandomPos(int n)
		{
			double[] re = new double[n];
			double[] im = new double[n];
			
			System.Random rnd = new System.Random();

			int repos = rnd.Next(n);
			int impos = rnd.Next(n);

			re[repos]=1;
			im[impos]=1;

			Pfa235FFT fft = new Pfa235FFT(n);

			fft.FFT(re,im,Pfa235FFT.Direction.Inverse);

			for(int i=0;i<n;i++)
			{
				Assertion.AssertEquals(string.Format("FFT({0}) of im 1 at pos(re={1},im={2}) re[{3}]",n,repos,impos,i), 
					 Math.Cos((2*Math.PI*i*(double)repos)/n) + Math.Sin((2*Math.PI*i*(double)impos)/n),
					re[i],n*1E-14);
				Assertion.AssertEquals(string.Format("FFT({0}) of im 1 at pos(re={1},im={2}) arb im[{3}]",n,repos,impos,i), 
					-Math.Sin((2*Math.PI*i*(double)repos)/n) + Math.Cos((2*Math.PI*i*(double)impos)/n), 
					im[i],n*1E-14);
			}
		}
	}


	[TestFixture]
	public class TestPFA235FFT_2D_Inverse
	{
		static System.Random rnd = new System.Random();
		
		
		static int GetRandomN(int max)
		{
			int[] pqr = new int[3];

			int n = rnd.Next(max);
			while(!Pfa235FFT.Factorize(n,pqr))
				++n;

			return n;
		}


		[Test]
		public void TestZero2N()
		{
			// Testing 2^n
			for(int i=2;i<=1000;i*=2)
				TestZero(i,i);
		}

		[Test]
		public void TestZero3N()
		{
			// Testing 3^n
			for(int i=3;i<1000;i*=3)
				TestZero(i,i);
		}
	
		[Test]
		public void TestZero5N()
		{
			// Testing 5^n
			for(int i=5;i<1000;i*=5)
				TestZero(i,i);
		}

		[Test]
		public void TestZero10N()
		{
			// Testing 5^n
			for(int i=10;i<=1000;i*=10)
				TestZero(i,i);
		}

		[Test]
		public void TestZeroRandomN()
		{
			// Testing 10 times random dimensions
			for(int i=0;i<10;i++)
			{
				int u = GetRandomN(1000);
				int v = GetRandomN(1000);
				Console.WriteLine("TestZero({0},{1})",u,v);
				TestZero(u,v);
			}
		}


		private static void TestZero(int u, int v)
		{
			int n = u*v;
			double[] re = new double[n];
			double[] im = new double[n];

			Pfa235FFT fft = new Pfa235FFT(u,v);

			fft.FFT(re,im,Pfa235FFT.Direction.Inverse);

			for(int i=0;i<n;i++)
			{
				Assertion.AssertEquals("FFT of zero should give re=0", 0, re[i],0);
				Assertion.AssertEquals("FFT of zero should give im=0", 0, im[i],0);
			}
		}

		[Test]
		public void TestReOne_OnePos1stDimRandomN()
		{
			// Testing 10 times random dimensions
			for(int i=0;i<10;i++)
			{
				int u = GetRandomN(1000);
				int v = GetRandomN(1000);
				TestReOne_OnePos1stDim(u,v);
			}
		}

		private static void TestReOne_OnePos1stDim(int u, int v)
		{
			Console.WriteLine("TestReOn_OnePos1stDim({0},{1})",u,v);

			int n=u*v;
			double[] re = new double[n];
			double[] im = new double[n];

			re[1] = 1;

			Pfa235FFT fft = new Pfa235FFT(u,v);

			fft.FFT(re,im,Pfa235FFT.Direction.Inverse);

			for(int i=0;i<u;i++)
			{
				for(int j=0;j<v;j++)
				{
					Assertion.AssertEquals(string.Format("FFT({0},{1}) of re 1 at pos 1 re[{2},{3}]",u,v,i,j), Math.Cos((2*Math.PI*j)/v), re[i*v+j],n*1E-15);
					Assertion.AssertEquals(string.Format("FFT({0},{1}) of re 1 at pos 1 im[{2},{3}]",u,v,i,j), -Math.Sin((2*Math.PI*j)/v), im[i*v+j],n*1E-15);
				}
			}
		}

		[Test]
		public void TestReOne_OnePos2ndDimRandomN()
		{
			// Testing 10 times random dimensions
			for(int i=0;i<10;i++)
			{
				int u = GetRandomN(1000);
				int v = GetRandomN(1000);
				TestReOne_OnePos2ndDim(u,v);
			}
		}

		private static void TestReOne_OnePos2ndDim(int u, int v)
		{
			Console.WriteLine("TestReOn_OnePos2ndDim({0},{1})",u,v);

			int n=u*v;
			double[] re = new double[n];
			double[] im = new double[n];

			re[1*v] = 1;

			Pfa235FFT fft = new Pfa235FFT(u,v);

			fft.FFT(re,im,Pfa235FFT.Direction.Inverse);

			for(int i=0;i<u;i++)
			{
				for(int j=0;j<v;j++)
				{
					Assertion.AssertEquals(string.Format("FFT({0},{1}) of re 1 at pos 1 re[{2},{3}]",u,v,i,j), Math.Cos((2*Math.PI*i)/u), re[i*v+j],n*1E-15);
					Assertion.AssertEquals(string.Format("FFT({0},{1}) of re 1 at pos 1 im[{2},{3}]",u,v,i,j), -Math.Sin((2*Math.PI*i)/u), im[i*v+j],n*1E-15);
				}
			}
		}


		[Test]
		public void TestReOne_OnePosBothDimRandomN()
		{
			// Testing 10 times random dimensions
			for(int i=0;i<10;i++)
			{
				int u = GetRandomN(1000);
				int v = GetRandomN(1000);
				TestReOne_OnePosBothDim(u,v);
			}
		}

		private static void TestReOne_OnePosBothDim(int u, int v)
		{
			Console.WriteLine("TestReOn_OnePosBothDim({0},{1})",u,v);

			int n=u*v;
			double[] re = new double[n];
			double[] im = new double[n];

			re[1*v+1] = 1;

			Pfa235FFT fft = new Pfa235FFT(u,v);

			fft.FFT(re,im,Pfa235FFT.Direction.Inverse);

			for(int i=0;i<u;i++)
			{
				for(int j=0;j<v;j++)
				{
					Assertion.AssertEquals(string.Format("FFT({0},{1}) of re 1 at pos 1 re[{2},{3}]",u,v,i,j), Math.Cos(2*Math.PI*(((double)i)/u + ((double)j)/v)), re[i*v+j],n*1E-15);
					Assertion.AssertEquals(string.Format("FFT({0},{1}) of re 1 at pos 1 im[{2},{3}]",u,v,i,j), -Math.Sin(2*Math.PI*(((double)i)/u + ((double)j)/v)),im[i*v+j],n*1E-15);
				}
			}
		}

		[Test]
		public void TestReOne_ArbPosBothDimRandomN()
		{
			// Testing 10 times random dimensions
			for(int i=0;i<10;i++)
			{
				int u = GetRandomN(1000);
				int v = GetRandomN(1000);
				TestReOne_ArbPosBothDim(u,v);
			}
		}

		private static void TestReOne_ArbPosBothDim(int u, int v)
		{
			int upos = rnd.Next(u);
			int vpos = rnd.Next(v);
			
			Console.WriteLine("TestReOn_ArbPosBothDim({0},{1}), pos({2},{3})",u,v,upos,vpos);

			int n=u*v;
			double[] re = new double[n];
			double[] im = new double[n];

	
			re[upos*v+vpos] = 1;

			Pfa235FFT fft = new Pfa235FFT(u,v);

			fft.FFT(re,im,Pfa235FFT.Direction.Inverse);

			for(int i=0;i<u;i++)
			{
				for(int j=0;j<v;j++)
				{
					Assertion.AssertEquals(string.Format("FFT({0},{1}) of re 1 at pos 1 re[{2},{3}]",u,v,i,j), Math.Cos(2*Math.PI*(((double)i)*upos/u + ((double)j)*vpos/v)), re[i*v+j],n*1E-15);
					Assertion.AssertEquals(string.Format("FFT({0},{1}) of re 1 at pos 1 im[{2},{3}]",u,v,i,j), -Math.Sin(2*Math.PI*(((double)i)*upos/u + ((double)j)*vpos/v)), im[i*v+j],n*1E-15);
				}
			}
		}


		[Test]
		public void TestImOne_ArbPosBothDimRandomN()
		{
			// Testing 10 times random dimensions
			for(int i=0;i<10;i++)
			{
				int u = GetRandomN(1000);
				int v = GetRandomN(1000);
				TestImOne_ArbPosBothDim(u,v);
			}
		}

		private static void TestImOne_ArbPosBothDim(int u, int v)
		{
			int upos = rnd.Next(u);
			int vpos = rnd.Next(v);
			Console.WriteLine("TestImOn_ArbPosBothDim({0},{1}), pos({2},{3})",u,v,upos,vpos);

			int n=u*v;
			double[] re = new double[n];
			double[] im = new double[n];



			im[upos*v+vpos] = 1;

			Pfa235FFT fft = new Pfa235FFT(u,v);

			fft.FFT(re,im,Pfa235FFT.Direction.Inverse);

			for(int i=0;i<u;i++)
			{
				for(int j=0;j<v;j++)
				{
					Assertion.AssertEquals(string.Format("FFT({0},{1}) of re 1 at pos 1 re[{2},{3}]",u,v,i,j), Math.Sin(2*Math.PI*(((double)i)*upos/u + ((double)j)*vpos/v)), re[i*v+j],n*1E-15);
					Assertion.AssertEquals(string.Format("FFT({0},{1}) of re 1 at pos 1 im[{2},{3}]",u,v,i,j), Math.Cos(2*Math.PI*(((double)i)*upos/u + ((double)j)*vpos/v)), im[i*v+j],n*1E-15);
				}
			}
		}

	}


	/// <summary>
	/// Summary description for main.
	/// </summary>
	public class MainClass
	{

		[STAThread]
		public static void Main()
		{
			
		//TestMpFFT1D.TestReOne_ZeroPos(8);

		}
	}
}

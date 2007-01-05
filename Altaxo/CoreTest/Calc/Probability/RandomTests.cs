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

using System;
using Altaxo.Calc.Probability;
using NUnit.Framework;

namespace AltaxoTest.Calc.Probability
{

  [TestFixture]
  public class RandomnessTests
  {

    [Test]
    public  void TestSystemRandom()
    {
      System.Random rand = new System.Random();

      EntCalc calc = new EntCalc(false);

      for(int i=0;i<100000;++i)
      {
        calc.AddSample(rand.Next(),false);
      }

      EntCalc.EntCalcResult result = calc.EndCalculation();
      Assert.IsTrue(result.ChiProbability<0.01);


    }
  

    [Test]
    public  void TestRan002()
    {
      Altaxo.Calc.Probability.Ran002 rand = new Ran002();

      EntCalc calc = new EntCalc(false);

      for(int i=0;i<100000;++i)
      {
        calc.AddSample((int)rand.Long(),false);
      }

      EntCalc.EntCalcResult result = calc.EndCalculation();
      Assert.IsTrue(result.ChiProbability<0.01);


    }
  }
  #region EntCalc
  /// <summary>
  /// Apply various randomness tests to a stream of bytes
  /// Original code by John Walker  --  September 1996
  /// http://www.fourmilab.ch/
  /// 
  /// C# port of ENT (ent - pseudorandom number sequence test)
  /// by Brett Trotter
  /// blt@iastate.edu
  /// </summary>

  public class EntCalc
  {
    static double[,] chsqt = new double[2, 10] 
      {
        {0.5, 0.25, 0.1, 0.05, 0.025, 0.01, 0.005, 0.001, 0.0005, 0.0001}, 
        {0.0, 0.6745, 1.2816, 1.6449, 1.9600, 2.3263, 2.5758, 3.0902, 3.2905, 3.7190}
      };

    private static int MONTEN = 6;        /* Bytes used as Monte Carlo co-ordinates
                           * This should be no more bits than the mantissa 
                           * of your "double" floating point type. */

    private uint[] monte = new uint[MONTEN];
    private double[] prob = new double[256];  /* Probabilities per bin for entropy */
    private long[] ccount = new long[256];    /* Bins to count occurrences of values */
    private long totalc = 0;          /* Total bytes counted */

    private int mp;
    private bool sccfirst;
    private long inmont, mcount;
    private double a;
    private double cexp;
    private double incirc;
    private double montex, montey, montepi;
    private double scc, sccun, sccu0, scclast, scct1, scct2, scct3;
    private double ent, chisq, datasum;

    private bool binary = false;        /* Treat input as a bitstream */

    public struct EntCalcResult 
    {
      public double Entropy;
      public double ChiSquare;
      public double Mean;
      public double MonteCarloPiCalc;
      public double SerialCorrelation;
      public long[] OccuranceCount;

      public double ChiProbability;
      public double MonteCarloErrorPct;
      public double OptimumCompressionReductionPct;
      public double ExpectedMeanForRandom;

      public long NumberOfSamples;
    }


    /*  Initialise random test counters.  */
    public EntCalc(bool binmode) 
    {
      int i;

      binary = binmode;       /* Set binary / byte mode */

      /* Initialise for calculations */

      ent = 0.0;            /* Clear entropy accumulator */
      chisq = 0.0;          /* Clear Chi-Square */
      datasum = 0.0;          /* Clear sum of bytes for arithmetic mean */

      mp = 0;             /* Reset Monte Carlo accumulator pointer */
      mcount = 0;           /* Clear Monte Carlo tries */
      inmont = 0;           /* Clear Monte Carlo inside count */
      incirc = 65535.0 * 65535.0;   /* In-circle distance for Monte Carlo */

      sccfirst = true;        /* Mark first time for serial correlation */
      scct1 = scct2 = scct3 = 0.0;  /* Clear serial correlation terms */

      incirc = Math.Pow(Math.Pow(256.0, (double) (MONTEN / 2)) - 1, 2.0);

      for (i = 0; i < 256; i++) 
      {
        ccount[i] = 0;
      }
      totalc = 0;
    }


    /*  AddSample  -- Add one or more bytes to accumulation.  */
    public void AddSample(int buf, bool Fold)
    {
      AddSample((byte) buf, Fold);
    }
    public void AddSample(byte buf, bool Fold)
    {
      byte[] tmpByte = new byte[1];
      tmpByte[0] = buf;
      AddSample(tmpByte, Fold);
    }
    public void AddSample(byte[] buf, bool Fold)
    {
      int c, bean;

      foreach(byte bufByte in buf) 
      {
        bean = 0;   // reset bean

        byte oc = bufByte;

        /*  Have not implemented folding yet. Plan to use System.Text.Encoding to do so.
         * 
         *        if (fold && isISOalpha(oc) && isISOupper(oc)) 
         *        {
         *          oc = toISOlower(oc);
         *        }
         */

        do 
        {
          if (binary) 
          {
            c = ((oc & 0x80));    // Get the MSB of the byte being read in
          } 
          else 
          {
            c = oc;
          }
          ccount[c]++;      /* Update counter for this bin */
          totalc++;

          /* Update inside / outside circle counts for Monte Carlo computation of PI */

          if (bean == 0) 
          {
            monte[mp++] = oc;       /* Save character for Monte Carlo */
            if (mp >= MONTEN) 
            {     /* Calculate every MONTEN character */
              int mj;

              mp = 0;
              mcount++;
              montex = montey = 0;
              for (mj = 0; mj < MONTEN / 2; mj++) 
              {
                montex = (montex * 256.0) + monte[mj];
                montey = (montey * 256.0) + monte[(MONTEN / 2) + mj];
              }
              if ((montex * montex + montey *  montey) <= incirc) 
              {
                inmont++;
              }
            }
          }

          /* Update calculation of serial correlation coefficient */

          sccun = c;
          if (sccfirst) 
          {
            sccfirst = false;
            scclast = 0;
            sccu0 = sccun;
          } 
          else 
          {
            scct1 = scct1 + scclast * sccun;
          }
          scct2 = scct2 + sccun;
          scct3 = scct3 + (sccun * sccun);
          scclast = sccun;
          oc <<= 1;           // left shift by one
        } while (binary && (++bean < 8));   // keep looping if we're in binary mode and while the bean counter is less than 8 (bits)
      }
    } // end foreach


    /*  EndCalculation  --  Complete calculation and return results.  */
    public EntCalcResult EndCalculation()
    {
      int i;

      /* Complete calculation of serial correlation coefficient */

      scct1 = scct1 + scclast * sccu0;
      scct2 = scct2 * scct2;
      scc = totalc * scct3 - scct2;
      if (scc == 0.0) 
      {
        scc = -100000;
      } 
      else 
      {
        scc = (totalc * scct1 - scct2) / scc;
      }

      /* Scan bins and calculate probability for each bin and
         Chi-Square distribution */

      cexp = totalc / (binary ? 2.0 : 256.0);  /* Expected count per bin */
      for (i = 0; i < (binary ? 2 : 256); i++) 
      {
        prob[i] = (double) ccount[i] / totalc;
        a = ccount[i] - cexp;
        chisq = chisq + (a * a) / cexp;
        datasum += ((double) i) * ccount[i];
      }

      /* Calculate entropy */

      for (i = 0; i < (binary ? 2 : 256); i++) 
      {
        if (prob[i] > 0.0) 
        {
          ent += prob[i] * log2(1 / prob[i]);
        }
      }

      /* Calculate Monte Carlo value for PI from percentage of hits
         within the circle */

      montepi = 4.0 * (((double) inmont) / mcount);


      /* Calculate probability of observed distribution occurring from
         the results of the Chi-Square test */

      double chip = Math.Sqrt(2.0 * chisq) - Math.Sqrt(2.0 * (binary ? 1 : 255.0) - 1.0);
      a = Math.Abs(chip);
      for (i = 9; i >= 0; i--) 
      {
        if (chsqt[1,i] < a) 
        {
          break;
        }
      }
      chip = (chip >= 0.0) ? chsqt[0,i] : 1.0 - chsqt[0,i];

      double compReductionPct = ((binary ? 1 : 8) - ent) / (binary ? 1.0 : 8.0);

      /* Return results */
      EntCalcResult result = new EntCalcResult();
      result.Entropy = ent;
      result.ChiSquare = chisq;
      result.ChiProbability = chip;
      result.Mean = datasum / totalc;
      result.ExpectedMeanForRandom = binary ? 0.5 : 127.5;
      result.MonteCarloPiCalc = montepi;
      result.MonteCarloErrorPct = (Math.Abs(Math.PI - montepi) / Math.PI);
      result.SerialCorrelation = scc;
      result.OptimumCompressionReductionPct = compReductionPct;
      result.OccuranceCount = this.ccount;
      result.NumberOfSamples = this.totalc;
      return result;
    }


    /*  LOG2  --  Calculate log to the base 2  */
    static double log2(double x)
    {
      return Math.Log(x, 2);    //can use this in C#
    }
  }
  public class EntFileCalc
  {
    public static EntCalc.EntCalcResult CalculateFile(ref System.IO.FileStream inStream)
    {

      EntCalc entCalc = new EntCalc(false);
      while (inStream.Position < inStream.Length)
      {
        entCalc.AddSample((byte) inStream.ReadByte(), false);
      }
      
      return entCalc.EndCalculation();
    }
    public static EntCalc.EntCalcResult CalculateFile(string inFileName) 
    {
      System.IO.FileStream instream = new System.IO.FileStream(inFileName,System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None);

      instream.Position = 0;

      EntCalc.EntCalcResult tmpRes = CalculateFile(ref instream);
      
      instream.Close();

      return tmpRes;
    }
  }
  #endregion


}



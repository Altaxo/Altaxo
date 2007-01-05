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
// matpack-1.7.3\include\mprandom.h
// matpack-1.7.3\include\ranclass.cc
// matpack-1.7.3\source\random\rand000.cc
// matpack-1.7.3\source\random\rand001.cc
// matpack-1.7.3\source\random\rand002.cc
// matpack-1.7.3\source\random\ran004.cc
// matpack-1.7.3\source\random\ran005.cc
// matpack-1.7.3\source\random\ran013.cc
// matpack-1.7.3\source\random\ran055.cc
// matpack-1.7.3\source\random\ran056.cc
// matpack-1.7.3\source\random\ran088.cc
// matpack-1.7.3\source\random\ran205.cc
// matpack-1.7.3\source\random\ran250.cc
// matpack-1.7.3\source\random\ran800.cc
// matpack-1.7.3\source\random\ran19937.cc
// matpack-1.7.3\source\random\rannormal.cc
// matpack-1.7.3\source\random\ranlognormal.cc
// matpack-1.7.3\source\random\ranexpo.cc
// matpack-1.7.3\source\random\ranerlang.cc
// matpack-1.7.3\source\random\rangamma.cc
// matpack-1.7.3\source\random\ranbeta.cc
// matpack-1.7.3\source\random\ranchisqr.cc
// matpack-1.7.3\source\random\ranfdistrib.cc
// matpack-1.7.3\source\random\ranpoisson.cc
// matpack-1.7.3\source\random\ranbinomial.cc
// matpack-1.7.3\source\random\ranmar.cc
// matpack-1.7.3\source\random\ranlux.cc
// matpack-1.7.3\source\random\Ran32k3a.cc
// matpack-1.7.3\source\random\RanSphere.cc

  
/*------------------------------------------------------------------------------\
| Matpack Library Release 1.7.3                                                 |
| Copyright (C) 1991-1997 by Berndt M. Gammel                                   |
|                                                                               |
| Permission to  use, copy, and  distribute  Matpack  in  its  entirety and its | 
| documentation for  non-commercial purpose and  without fee is hereby granted, | 
| provided that this license information and copyright notice appear unmodified | 
| in all copies. This software is provided 'as is'  without  express or implied | 
| warranty.  In no event will the author be held liable for any damages arising | 
| from the use of this software.            |
| Note that distributing Matpack 'bundled' in with any product is considered to | 
| be a 'commercial purpose'.              |
| The software may be modified for your own purposes, but modified versions may | 
| not be distributed without prior consent of the author.     |
|                                                                               |
| Read the  COPYRIGHT and  README files in this distribution about registration |
| and installation of Matpack.              |
|                                                                               |
\*-----------------------------------------------------------------------------*/



using System;
using Altaxo.Calc;

namespace Altaxo.Calc.Probability
{

  #region RandomGenerator

  /// <summary>
  /// Base class for all random generators    
  /// </summary>
  public abstract class RandomGenerator 
  {
    /// <summary>The seed of the random generator.</summary>
    protected uint seed;  

    /// <summary>Uniform long int values within [0...max_val].</summary>
    protected uint max_val; // 

    /// <summary>Used to generate seed values.</summary>
    protected static System.Security.Cryptography.RandomNumberGenerator seedGenerator =
      new System.Security.Cryptography.RNGCryptoServiceProvider();
    
    /// <summary>This is the default state of the art random genenerator to produce random numbers.</summary>
    protected static RandomGenerator defaultRandomGenerator = 
      new Ran002(UniqueSeed());

    #region Helper functions and constants

    /// <summary>Maximum value of a System.Double.</summary>
    protected const double DBL_MAX = double.MaxValue;
    /// <summary>Smallest positive value of a System.Double.</summary>
    protected const double DBL_MIN = double.Epsilon; // god saves microsoft!
    /// <summary>Maximum binary exponent of a <see cref="System.Double"/>.</summary>
    protected const int DBL_MAX_EXP = 1024;
    /// <summary>Natural logarithm of 2.</summary>
    protected const double M_LN2 = 0.69314718055994530941723212145818;


    /// <summary>
    /// Return first number with sign of second number
    /// </summary>
    /// <param name="x">The first number.</param>
    /// <param name="y">The second number whose sign is used.</param>
    /// <returns>The first number x with the sign of the second argument y.</returns>
    protected static double CopySign (double x, double y)
    {
      return (y < 0) ? ((x < 0) ? x : -x) : ((x > 0) ? x : -x);
    }

    #endregion


    #region UniqueSeed

    
    /// <summary>
    /// Generate an unique seed using the default seed generator, which is currently
    /// <see cref="System.Security.Cryptography.RNGCryptoServiceProvider"/>
    /// </summary>
    /// <returns>A seed value in the range [0, 2^31-1].</returns>
    public static uint UniqueSeed()
    {
      byte[] bytes = new byte[4];
      uint result;
      
      do
      {
        seedGenerator.GetBytes(bytes);

        result = 0x7FFFFFFFU & System.BitConverter.ToUInt32(bytes,0);
      
      } while(result==0); // extremly seldom
      
      return result; 
    }     

                                                                                                                                                
    
    #endregion

    /// <summary>The default random generator used to produce random values.</summary>
    /// <value>Default random generator.</value>
    public static RandomGenerator DefaultGenerator
    {
      get { return defaultRandomGenerator; }
    }

    /// <summary>Constructs the random generator with a seed value of 0.</summary>
    public RandomGenerator()
      : this(0)
    {
    }
    /// <summary>Constructs the random generator with a seed value.</summary>
    /// <param name="new_seed">The initial seed value.</param>
    public RandomGenerator(uint new_seed)
    {
      seed = ((new_seed == 0) ? UniqueSeed() : new_seed) & 0x7fffffff;
    }
    /// <summary>Constructs the random generator with a seed using the hash value of a given string.</summary>
    /// <param name="hashstring">The hash value of this string is used for the initial seed value.</param>
    public RandomGenerator (string hashstring)
    {
      seed = (uint)(0x7fffffff & hashstring.GetHashCode());
    }
  
    /// <summary>The seed value.</summary>
    /// <value>The seed value.</value>
    public uint Seed 
    {
      get { return seed; }
    }
    /// <summary>The maximum value of the random number which can be returned.</summary>
    /// <value>Maximal possible value of the generated random number.</value>
    public uint Maximum 
    {
      get { return max_val; }
    }

    /// <summary>The generation function.</summary>
    /// <returns>The next random value.</returns>
    public abstract uint Long();
  }

  #endregion

  #region Ran000


  
  /// <summary>
  /// <para>Ran000: minimal congruential</para>
  /// <para>Returns integer random numbers uniformly distributed within [0,2147483646].</para> 
  /// <para>NOT RECOMENDED FOR SERIOUS APPLICATIONS.</para>
  /// </summary>
  /// <remarks>
  /// <code>
  /// Notes: - NOT RECOMENDED FOR SERIOUS APPLICATIONS.
  ///     Just use it for comparison purposes, etc.
  ///
  ///        - This generator is fast
  ///
  ///        - "Minimal" random number generator of Park and Miller.
  ///         x(n) = 16807 * x(n-1) mod (2^31-1)
  ///          also known as GGL.
  ///
  ///   - Set seed to any integer value (except the unlikely value mask) to
  ///     initialize the sequence.
  ///
  ///        - The period is 2^31-2 = 2.1*10^9. If your application needs 
  ///     more numbers in sequence than 1% of the random generators period, 
  ///     i.e. 10^7, then use a more elaborate random generator.
  ///
  ///        - At least 32 bit long int is required.
  ///
  ///        - References:
  ///           o Algorithm from CACM 31 no. 10, pp. 1192-1201, October 1988. 
  ///           o Algorithm "ran0" from "Portable Random Number Generators",
  ///             William H. Press and Saul A Teukolsky
  ///             Computers in Phyics, Vol. 6, No. 5, Sep/Oct 1992
  ///           o According to a posting by Ed Taft on comp.lang.postscript,
  ///             Level 2 (Adobe) PostScript interpreters use this algorithm too.
  ///
  /// ----------------------------------------------------------------------------//
  /// </code>
  /// </remarks>
  public class Ran000: RandomGenerator 
  { // minimal congruential

    const int  IM = 2147483647;  // 2^31-1
    const int   IA = 16807;
    const int   IQ = 127773;
    const int   IR = 2836;
    const int   MASK  = 123459876;

    private int idum;

    public Ran000()
      : this(0)
    {
    }
    public Ran000 (uint the_seed)
      : base(the_seed)
    {
      // make shure that seed is not the MASK (otherwise zero is returned always)
      if (seed == (uint) MASK)
        throw new ArgumentException(string.Format("You selected the only illegal seed value ({0})",MASK));

      max_val = IM - 1; // initialize the maximum value max_val

      idum = (int)(seed^MASK);  // initialize idum using the seed: 
      // XORing with MASK allows use of zero
      // and other simple bit patterns for idum.
    }

    public override uint Long()
    {
      int k = idum/IQ;    // Compute idum=mod(IA*idum,IM)
      idum = IA*(idum-k*IQ)-IR*k; // without overflows by  
      if (idum < 0)
        idum += IM; // Schrage's method.
      
      return (uint)idum;    // return random number
    }
  }

  #endregion

  #region Ran001


  

  /// <summary>
  /// Ran001: combined congruential with shuffle.
  /// Returns integer random numbers uniformly distributed within [0,2147483646].
  /// </summary>
  /// <remarks><code>
  /// Notes: - Minimal random number generator of Park and Miller with
  ///          Bays-Durham shuffle and added safeguards. 
  ///
  ///        - The period is 2^31-2 = 2.1*10^9. If your application needs 
  ///     more numbers in sequence than 1% of the random generators period, 
  ///     i.e. 10^7, then use a more elaborate random generator.
  ///          There are no statistical tests known that it fails to pass, execpt 
  ///          when the number of calls starts to become on the order of the period.
  ///     When you need longer random sequences you should use
  ///          another random generator, for example Ran002 or Ran013.
  ///
  ///        - Reference:
  ///          Algorithm "ran1" published in "Portable Random Number Generators",
  ///          William H. Press and Saul A. Teukolsky
  ///          Computers in Phyics, Vol. 6, No. 5, Sep/Oct 1992
  ///
  ///        - At least 32 bit long int is required, but works with any larger
  ///     word lengths
  /// </code></remarks>
  public class Ran001 : RandomGenerator 
  { // Press's MLCG "ran1"

    
    const int NTAB = 32;

    const int IM = 2147483647,  // 2^31-1
      IA = 16807,
      IQ = 127773,
      IR = 2836,
      MASK = 123459876;

    private int idum, iy;
    int[] iv = new int[32];
    
    public Ran001() : this(0) {}
    public Ran001 (uint the_seed)
      : base(the_seed)
    { 
          
      // initialize the maximum value: max_val
      max_val = IM - 1;
    
      // initialize shuffle table using the seed: iv and idum,iy
      idum  = (int)((seed > 1) ? seed : 1);    // Be sure to prevent idum = 0 
      for (int j = NTAB+7; j >= 0; j--) 
      { // Load shuffle table (after 8 warm-ups)
        long k = idum/IQ;
        idum = (int)(IA * (idum - k * IQ) - IR * k);
        if (idum < 0) 
          idum += IM;
        if (j < NTAB)
          iv[j] = idum;
      }
      iy = iv[0];
    }
    public override uint Long()
    {
      long j, k;

      k = idum/IQ;      // Start here when not initializing
      idum = (int)(IA*(idum-k*IQ)-IR*k);    // Compute idum=mod(IA1*idum,IM1) without
      if (idum < 0) 
        idum += IM;   // overflows by Schrage's method 
      
      j = iy/(1+(IM-1)/NTAB);   // Will be in the range 0..NTAB-1
      iy = iv[j];       // Output previously stored value and 
      iv[j] = idum;     // refill the shuffle table
    
      return (uint)iy;
    }

  }
  #endregion

  #region Ran002

   

  /// <summary>
  /// Ran002: combined congruential with shuffle.
  /// Returns an integer random number uniformly distributed within [1,2147483562].  
  /// This generator is very slow. 
  /// </summary>
  /// <remarks><code>
  /// Reference:
  ///  (1) This is the long period ( > 2.3 * 10^18 ) random number generator of 
  ///      P. L'Ecuyer, Commun. ACM 31, 743 (1988), but with Bays-Durham shuffle 
  ///      and "added safeguards" as proposed by
  ///  (2) William H. Press and Saul A. Teukolsky,
  ///      "Portable Random Number Generators",
  ///      Computers in Phyics, Vol. 6, No. 5, Sep/Oct 1992, Algorithm "ran2"
  ///  (3) This generator has been validated also by 
  ///      G. Marsaglia and A. Zaman, 
  ///      "Some portable very-long-period random number generators",
  ///      Computers in Physics, Vol. 8, No. 1, Jan/Feb 1994.  
  ///
  /// Notes:
  ///   -  William Press and Saul Teukolsky think that this is a "perfect" 
  ///      random generator and will pay $1000 for the first one who convinces 
  ///      them otherwise.
  /// </code></remarks>
  public class Ran002 : RandomGenerator 
  { // Press's combined MLCG "ran2"

    
    const int NTAB = 32;

    const int IM1  = 2147483563,
      IM2  = 2147483399,
      IA1  = 40014,
      IA2  = 40692,
      IQ1  = 53668,
      IQ2  = 52774,
      IR1  = 12211,
      IR2  = 3791,
      IMM1 = IM1-1;


    private int idum, idum2, iy;
    private int[] iv = new int[32];
    public  Ran002 () : this(0) {}
    public  Ran002 (uint the_seed)
      : base(the_seed)
    { 
      // initialize the maximum value: max_val
      max_val = IMM1;
    
      // initialize shuffle table using the seed: iv and idum,idum2,iy
      idum  = (int)((seed > 1) ? seed : 1);    // Be sure to prevent idum = 0 
      idum2 = idum;     // was previously: idum2 = 123456789L;

      for (int j = NTAB+7; j >= 0; j--) 
      { // Load shuffle table (after 8 warm-ups)
        long k = idum/IQ1;
        idum = (int)(IA1 * (idum - k * IQ1) - IR1 * k);
        if (idum < 0)
          idum += IM1;
        if (j < NTAB)
          iv[j] = idum;
      }
      iy = iv[0];
    }

    public override uint Long()
    {
      long j, k;

      k = idum/IQ1;     // Start here when not initializing
      idum = (int)(IA1*(idum-k*IQ1)-IR1*k); // Compute idum=mod(IA1*idum,IM1) without
      // overflows by Schrage's method 
      if (idum < 0)
        idum += IM1;
    
      k = idum2/IQ2;      // Compute idum2=mod(IA2*idum2,IM2) likewise
      idum2 = (int)(IA2*(idum2-k*IQ2)-IR2*k); // Compute idum=mod(IA1*idum,IM1) without
      // overflows by Schrage's method
      if (idum2 < 0) idum2 += IM2;
    
      j = iy/(1L+IMM1/NTAB);    // Will be in the range 0..NTAB-1
      iy = iv[j]-idum2;     // Output previously stored value and 
      // refill the shuffle table
      iv[j] = idum;
      if (iy < 1) iy += IMM1;
    
      return (uint)iy;
    }

  }
  #endregion

  #region Ran004

  
  
  /// <summary>
  /// Ran004: W.H. Press/S.A. Teukolsky: Numerical recipies pseudo-DES ran4. 
  /// Returns an integer random number uniformly distributed within [0,4294967295].
  /// </summary>
  /// <remarks><code>
  /// The period length for one seed is 2^32, but the seed is incremented
  /// automatically if the series for one seed is exhausted. There are
  /// 2^32 possible seeds.
  ///
  /// Notes:
  ///
  /// a) The original version of Ref. (1) is not portable to machines with larger
  ///    word lengths. That means different random sequences are obtained for
  ///    32-bit long integers and 64-bit long integers. 
  ///    This version is made portable by using bit-masks. The run time penalty
  ///    is neglegible.
  /// b) The random sequence for one seed has only a period of maximally 2^32.
  ///    This is definitly to short for nowadays MC simulations. In this version
  ///    the seed is automatically incremented to jump to the next segment
  ///    when one segment is exhausted. 
  /// c) Also the extremely inconvenient interface of the original 
  ///    has been changed.
  /// 
  /// Reference:
  ///   (1) W.H. Press, S.A. Teukolsky, Vetterling, Teukolsky,
  ///       Numerical Recipies in C, 2nd edition, 1992.
  ///   (2) Major modifications a) to c) and inclusion into Matpack
  ///       by B. M. Gammel, Apr 1, 1997 (no joke!)
  /// </code></remarks>
  public class Ran004:  RandomGenerator 
  { // Press's pseudo-DES "ran4"
    private uint idums, idum; 
    public Ran004() : this(0) {}
    public Ran004 (uint the_seed)
      : base(the_seed)
    {
      // initialize the maximum value: 0xffffffff = 2^32-1 = 4294967295
      max_val = 0xffffffff;

      // initialize the seed
      idums = seed;

      // first value to be hashed for the sequence associated to seed
      idum = 1;
    }
    //----------------------------------------------------------------------------//
    // deliver next pseudorandom number
    //----------------------------------------------------------------------------//
    public override uint Long()
    {
      uint irword, lword;
      irword = idum;  // next idum
      lword  = idums;   // the seed
      psdes(ref lword, ref irword); // hash

      if (idum == 0xffffffff) 
      { // this sequence is exhausted
        idum = 0;     // reset to start 
        if (idums == 0xffffffff) 
          idums = 1; 
        else 
          idums++;      // increment seed to get next sequence
#if DEBUG
        System.Diagnostics.Trace.WriteLine("skip to next sequence: " + idums.ToString());
#endif
      }
      idum++;     // increment idum
      return irword;
    }

    //----------------------------------------------------------------------------//
    // reset the sequence to the n-th deviate for the given seed and return number
    //----------------------------------------------------------------------------//
    public virtual uint Long (uint new_seed, uint nth)
    {
      uint irword, lword;
      lword = idums = new_seed; // the new seed
      irword = idum = nth;    // reset idum to nth element od the sequence
      psdes(ref lword, ref irword);   // hash
      if (idum == 0xffffffff) 
      { // this sequence is exhausted
        idum = 0;     // reset to start 
        if (idums == 0xffffffff) 
          idums = 1; 
        else 
          idums++;      // increment seed to get next sequence
#if DEBUG
        System.Diagnostics.Trace.WriteLine("skip to next sequence: " + idums.ToString());
#endif
      }
      idum++;     // increment idum
      return irword;
    }


    //----------------------------------------------------------------------------//
    // pseudo-DES hashing algorithm
    // not yet portable to 64-bit machines - use masks !
    //----------------------------------------------------------------------------//
    readonly static uint[] c1 = {0xbaa96887,0x1e17d32c,0x03bcdc3c,0x0f33d1b2};
    readonly static uint[] c2 = {0x4b0f3b58,0xe874f0c3,0x6955c5a6,0x55a7ca46};
    static void psdes (ref uint lword, ref uint irword)
    {
  
      uint ia, ib, iswap, itmph = 0, itmpl = 0;

#if DEBUG
      System.Diagnostics.Trace.WriteLine(string.Format("before: lword = {0}, irword = {1}", lword, irword ));
#endif

      // four iterations of the non-linear pseude-DES mixing
      for (int i = 0; i < 4; i++) 
      {
        ia = (iswap = irword) ^ c1[i];
        itmpl = ia & 0xffff;
        itmph = ia >> 16;
        ib = (itmpl*itmpl + ~(itmph*itmph)) & 0xffffffff;
        irword = (lword ^ (((ia = (ib >> 16) | ((ib & 0xffff) << 16)) ^ c2[i]) 
          + itmpl*itmph)) & 0xffffffff;
        lword = iswap & 0xffffffff;
      }

#if DEBUG
      System.Diagnostics.Trace.WriteLine(string.Format("after : lword = {0} irword = {1}",lword , irword ));
#endif

    }
  }
  #endregion

  #region Ran005

  
  
  /// <summary>
  /// Ran005: congruential with shuffle.
  /// Returns an integer random number uniformly distributed within [0,714024]. 
  /// <para>Notes: - NOT RECOMENDED FOR SERIOUS APPLICATIONS.</para>
  /// </summary>
  /// <remarks><code>
  ///     Just use it for comparison purposes, etc.
  ///
  ///        - As you see the spacing of the numbers is very coarse.
  ///
  ///   - Low order correlations are present.
  ///
  ///        - The period is effectively infinite.
  ///
  ///        - At least 32 bit long int is required.
  ///
  ///        - Corresponds to generator "ran2" of
  ///          W. H. Press, B. P. Flannery, S. A. Teukolsky, W. T. Vetterling,
  ///          Numerical Recipies in C, Cambridge Univ. Press, 1988.
  ///
  ///        - The generator is based upon
  ///          D. H. Knuth: The Art of Computer Programming, Vol.2, 2nd ed., 1981.
  /// </code></remarks>
  public class Ran005:RandomGenerator 
  { // Press's old congruential with shuffle

    const int M = 714025, 
      A = 1366, 
      C = 150889;

    private int[] r = new int[98];
    private int x, y;
    public Ran005() : this(0) {}
    public Ran005 (uint the_seed)
      : base(the_seed)
    {
          
      // initialize the maximum value: max_val
      max_val = M - 1;

      // initialize shuffle table r and x, y
      x = (int)((seed > 0) ? -seed : seed);
      if ((x = (C - x) % M) < 0) x = -x;
      for (int j = 1; j <= 97; j++)
        r[j] = x = (A * x + C) % M;
      y = x = (A * x + C) % M;
    }
     

    public override uint Long()
    {
      int k = 1 + (97 * y) / M;
      y = r[k];
      r[k] = x = (A * x + C) % M;
      return (uint)y;
    }
  };
  #endregion

  #region Ran013

  
  /// <summary>
  /// Ran013: congruential combined.
  /// Returns  integer random numbers uniformly distributed within [0,4294967295]
  ///          (that means [0,2^32-1]. The period is about 2^125 > 4.25*10^37.
  /// </summary>
  /// <remarks><code>
  ///
  ///        - At least 32 bit long int is required, but works with any larger
  ///     word lengths
  ///
  ///        - Reference:
  ///          This is algorithm "mzran13" from
  ///          G. Marsaglia and A. Zaman, 
  ///          "Some portable very-long-period random number generators",
  ///          Computers in Physics, Vol. 8, No. 1, Jan/Feb 1994.  
  ///
  ///          In the original implementation the algorithm relies on 32-bit 
  ///          arithmetics with implicit modulo 2^32 on overflow. Since the
  ///          the size of unsigned longs may not always be 32 bit the 
  ///          modulo 2^32 is coded explicitly using masks. 
  ///     The performance loss is not very important.
  ///
  ///          The original code reads:
  ///
  ///              long s;
  ///              if (y > x+c) { 
  ///           s = y - (x+c); c = 0; 
  ///              } else {
  ///           s = y - (x+c) - 18; c = 1;
  ///              }
  ///              x = y;
  ///              y = z;
  ///              return ((z = s) + (n - 69069 * n + 1013904243));
  ///                                   ^
  ///                                   Here it contains a misprint
  ///                                   Should really be a "=" !
  /// </code></remarks>
  public class Ran013 : RandomGenerator 
  { // Marsaglia's combined congruential
    private uint x, y, z, n; 
    private uint c;
    public  Ran013 () : this(0) {}
    public  Ran013 (uint the_seed)
      : base(the_seed)
    {
      // initialize the maximum value: 0xffffffff = 2^32-1 = 4294967295
      max_val = (uint) 0xffffffff;

      // initialize the seeds
      x = (seed ^  521288629) & 0xffffffff;
      y = (seed ^  362436069) & 0xffffffff;
      z = (seed ^   16163801) & 0xffffffff;
      n = (seed ^ 1131199209) & 0xffffffff;
      c = y > z ? 1U : 0U; // test this, the original code here was:  c = y > z;
    }

    public override uint Long()
    {
      // The mask 0xffffffff is neccessary in some places to assure that arithmetics
      // is performed modulo 2^32 to make the generater portable to any word length 
      // larger than 2^32.

      uint s;
      if (y > x+c) 
      { 
        s = y - (x+c); 
        c = 0; 
      } 
      else 
      {
        s = (y - (x+c) - 18) & 0xffffffff; // mask is neccessary here
        c = 1;
      }
      x = y;
      y = z;   

      return (uint)(((z = s) + (n = 69069 * n + 1013904243)) & 0xffffffff);  // and here
    }
  };
  #endregion

  #region Ran055


  
  /// <summary>
  /// Ran055: Knuth's shift and add random generator.
  /// Returns integer random numbers uniformly distributed within [0,2147483647]  
  /// DON'T USE THIS GENERATOR IN SERIOUS APPLICATIONS BECAUSE IT HAS SERIOUS CORRELATIONS.
  /// </summary>
  /// <remarks><code>
  ///         - The period is 2^55 = 36 028 797 018 963 968 > 3.6*10^16.
  ///
  ///        - At least 32 bit long int is required, but works with any larger
  ///     word lengths
  ///        - This is a lagged Fibonacci generator:
  ///            x(n) =  ( x(n-55) - x(n-24) ) mod 2^31
  ///        - Reference:
  ///          A version of this pseudrandom number generator is described in
  ///          J. Bentley's column, "The Software Exploratorium", Unix Review 1991.
  ///          It is based on Algorithm A in D. E. Knuth, The Art of Computer-
  ///          Programming, Vol 2, Section 3.2.2, pp. 172 
  /// </code></remarks>
  public class Ran055:  RandomGenerator 
  { // Knuth's lagged Fibonacci generator

    
    const uint MAX  = 2147483647,  // 2^31-1
      MASK = 123459876;

    private uint[] s55 = new uint[55];
    private int j55, k55; 

    public Ran055() : this(0) {}

    public Ran055(uint the_seed)
      : base(the_seed)
    {
      int i, ii;
      uint j;
      int k;

          

      max_val = MAX;  // initialize the maximum value max_val

      // initialize table

      s55[0] = j = seed^MASK ;  // XORing with MASK allows use of zero
      // and other simple bit patterns for idum.
      k = 1;  
      for (i = 1; i < 55; i++) 
      {
        ii = (21 * i) % 55;
        s55[ii] = (uint)k;
        k = (int)(j - k);
        j = s55[ii] ;
      }

      j55 = 0;      // invariance (b-a-24)%55 = 0
      k55 = 24;

      for (i = 0; i < 165; i++) Long();  // warm up table three times
    }
    public override uint Long()
    {
      // The mask 0x7fffffff assures that the result
      // is a positive 32 bit signed long.
      if(k55!=0) k55--; else k55 = 54;
      if(j55!=0) j55--; else j55 = 54;
      
      return ( s55[j55] -= s55[k55] ) & 0x7fffffff; 
    }
  }
  #endregion

  #region Ran056
  
  

  /// <summary>
  /// Ran056: Knuth's lagged Fibonacci random generator with 3-decimation.
  /// Returns integer random numbers uniformly distributed within [0,2147483647].
  /// The period is 2^55/3 > 1.2*10^16.
  /// </summary>
  /// <remarks><code>
  ///
  ///        - At least 32 bit long int is required, but works with any larger
  ///     word lengths
  ///        - This is the same lagged Fibonacci generator as Ran055
  ///            x(n) =  ( x(n-55) - x(n-24) ) mod 2^31
  ///        - but a decimation strategy is applied to remove the known 
  ///          correlations. Only every 3rd number will be used: 
  ///        - Reference:
  ///            I. Vattulainen, T. Ala-Nissila, and K. Kankaala, 
  ///            Physical Tests for Random Numbers in Simulations, 
  ///            Phys. Rev. Lett. 73, 2513 (1994).
  /// </code></remarks>
  public class Ran056 :  RandomGenerator 
  { // like Ran055 but with 3-decimation
    const uint MAX  = 2147483647,  // 2^31-1
      MASK = 123459876;

    private uint[] s55 = new uint[55];
    private int j55, k55; 
    public  Ran056() : this(0) {}
    public  Ran056(uint the_seed)
      : base(the_seed)
    {
      int i, ii;
      uint j;
      int k;

      max_val = MAX;  // initialize the maximum value max_val

      // initialize table

      s55[0] = j = seed^MASK ;  // XORing with MASK allows use of zero
      // and other simple bit patterns for idum.
      k = 1;  
      for (i = 1; i < 55; i++) 
      {
        ii = (21 * i) % 55;
        s55[ii] = (uint)k;
        k = (int)(j - k);
        j = s55[ii] ;
      }

      j55 = 0;      // invariance (b-a-24)%55 = 0
      k55 = 24;

      for (i = 0; i < 55; i++) Long(); // warm up table three times
    }
    public override uint Long()
    {
      // The mask 0x7fffffff assures that the result
      // is a positive 32 bit signed long.

      // inline the Fibonacci step three times 
      // in order to remove correlations
    
      if(k55!=0) k55--; else k55 = 54; // one
      if(j55!=0) j55--; else j55 = 54;
      s55[j55] -= s55[k55];
      if(k55!=0) k55--; else k55 = 54; // two
      if(j55!=0) j55--; else j55 = 54;
      s55[j55] -= s55[k55];
      if(k55!=0) k55--; else k55 = 54; // three
      if(j55!=0) j55--; else j55 = 54;
      return ( s55[j55] -= s55[k55] ) & 0x7fffffff; 
    }

  }
  #endregion

  #region Ran088



  
  /// <summary>
  /// Ran088: L'Ecuyer's 1996 three-component Tausworthe generator "taus88".
  /// Returns an integer random number uniformly distributed within [0,4294967295].
  /// The period length is approximately 2^88 (which is 3*10^26). 
  /// This generator is very fast and passes all standard statistical tests.
  /// </summary>
  /// <remarks><code>
  ///
  /// Reference:
  ///   (1) P. L'Ecuyer, Maximally equidistributed combined Tausworthe generators,
  ///       Mathematics of Computation, 65, 203-213 (1996), see Figure 4.
  ///   (2) recommended in:
  ///       P. L'Ecuyer, Random number generation, chapter 4 of the
  ///       Handbook on Simulation, Ed. Jerry Banks, Wiley, 1997.
  /// </code></remarks>
  public class Ran088:  RandomGenerator 
  { // L'Ecuyer's 1996 Tausworthe "taus88"
    private uint s1,s2,s3;
    public  Ran088 () : this(0) {}
    public  Ran088 (uint the_seed)
      : base(the_seed)
    { 
        
  
      // initialize the maximum value: max_val
      max_val = 4294967295U; // which is 2^32-1
  
      // initialize seeds using the given seed value taking care of
      // the requrements. The constants below are arbitrary otherwise
      s1 = 1243598713U ^ seed; if (s1 < 2)  s1 = 1243598713U;
      s2 = 3093459404U ^ seed; if (s2 < 8)  s2 = 3093459404U;
      s3 = 1821928721U ^ seed; if (s3 < 16) s3 = 1821928721U;
    }
    public override uint Long()
    { 
      // use mask to make the generator portable for any word width >= 32 bit
      const uint mask = 0xffffffff;
      uint b;
      b  = (((s1 << 13) & mask) ^ s1) >> 19;
      s1 = (((s1 & 4294967294U) << 12) & mask) ^ b;
      b  = (((s2 << 2) & mask) ^ s2) >> 25;
      s2 = (((s2 & 4294967288U) << 4) & mask) ^ b;
      b  = (((s3 << 3) & mask) ^ s3) >> 11;
      s3 = (((s3 & 4294967280U) << 17) & mask) ^ b;
      return (s1 ^ s2 ^ s3); 
    }
  };
  #endregion

  #region Ran205
  

  /// <summary>
  /// Ran205: L'Ecuyer's 1996 combined multiple recursive PRNG.
  /// Returns an integer random number uniformly distributed within [0,2147483646].
  /// The period length is approximately 2^205 (=5*10^61). The generator
  /// returns uniformly distributed integers in the range [0,2^31-2].
  /// It passes all current standard statistical tests.
  /// </summary>
  /// <remarks><code>
  ///
  /// References:
  ///   (1) P. L'Ecuyer, "Combined Multiple Recursive Generators,"
  ///       Operations Research 44, 5, pp. 816-822 (1996), see figure 1.
  ///   (2) recommended in:
  ///       P. L'Ecuyer, "Random number generation", chapter 4 of the
  ///       Handbook on Simulation, Ed. Jerry Banks, Wiley, 1997.
  ///
  /// Notes:
  ///    -  This generator is very slow
  ///
  /// </code></remarks>
  public class Ran205: RandomGenerator 
  { // L'Ecuyer's 1996 MRG

    private int x10,x11,x12,x20,x21,x22;
    public Ran205 () : this(0) {}
    public Ran205 (uint the_seed)
      : base(the_seed)
    { 
            
  
      // initialize the maximum value: max_val
      max_val = 2147483646; // which is 2^31-2
  
      // initialize seeds using the given seed value:  
      // The seeds must be within [1,2145483478]
      const long smax = 2145483478, 
              mask = 0x7fffffff; // to get positive integer <= 2^31-1
      x10 = (int)(((2039845123 ^ seed) & mask) % smax + 1);
      x11 = (int)((( 182401045 ^ seed) & mask) % smax + 1);
      x12 = (int)(((1190945568 ^ seed) & mask) % smax + 1);
      x20 = (int)((( 943583831 ^ seed) & mask) % smax + 1);
      x21 = (int)(((1345908737 ^ seed) & mask) % smax + 1);
      x22 = (int)((( 723161013 ^ seed) & mask) % smax + 1);
    }
    public override uint Long()
    {
      // constant numbers
      const int  m1  = 2147483647,  m2  = 2145483479,
              a12 =      63308,  q12 =      33921,  r12 = 12979,
              a13 =    -183326,  q13 =      11714,  r13 =  2883,
              a21 =      86098,  q21 =      24919,  r21 =  7417,
              a23 =    -539608,  q23 =       3976,  r23 =  2071;

      long h, p12, p13, p21, p23;
      // component 1
      h = x10 / q13;  
      p13 = -a13 * (x10 - h * q13) - h * r13;
      h = x11 / q12;  
      p12 =  a12 * (x11 - h * q12) - h * r12;
      if (p13 < 0) p13 += m1;
      if (p12 < 0) p12 += m1;
      x10 = x11;
      x11 = x12;
      x12 = (int)(p12 - p13);
      if (x12 < 0) x12 += m1;
      // component 2
      h = x20 / q23;
      p23 = -a23 * (x20 - h * q23) - h * r23;
      h = x22 / q21;
      p21 =  a21 * (x22 - h * q21) - h * r21;
      if (p23 < 0) p23 += m2;
      if (p21 < 0) p21 += m2;
      x20 = x21;
      x21 = x22;
      x22 = (int)(p21 - p23);
      if (x22 < 0) x22 += m2;
      // combination
      return (uint)((x12 < x22) ? (x12 - x22 + m1) : (x12 - x22));
    }
  };

  #endregion

  #region Ran250
  

  /// <summary>
  /// Ran250: the Kirkpatrick-Stoll generator "R250".
  /// Returns integer random numbers uniformly distributed within [0,2147483646]. 
  /// Notes: - SERIOUS DEFICIENCIES IN SOME PHYSICAL SIMULATIONS HAVE BEEN FOUND!
  /// </summary>
  /// <remarks><code>
  /// Notes: - SERIOUS DEFICIENCIES IN SOME PHYSICAL SIMULATIONS HAVE BEEN FOUND
  ///     (e.g. in the Ising model using the Wolff cluster update algorithm)
  ///
  ///        - The period is 2^250
  ///
  ///        - At least 32 bit unsigned long is required.
  ///
  ///        - References:
  ///           o S. Kirkpatrick and E. Stoll, "A Very Fast
  ///             Shift-Register Sequence Random Number Generator",
  ///             Journal of Computational Physics 40, 517 (1981)
  ///           o W.L. Maier, "A Fast Pseudo Random Number Generator",
  ///             Dr. Dobb's Journal, May, 152-157 (1991) 
  /// </code></remarks>
  public class Ran250: RandomGenerator 
  { // Kirkpatrick-Stoll generator "R250"

    // Defines to allow for 16 or 31 or 32 bit integers
    // In the Matpack library the 32-bit version is used !

    const int N_BITS = 32;
    const uint MSB = 0x80000000;
    const uint ALL_BITS  =    0xffffffff;
    const uint HALF_RANGE =  0x40000000;
    const int  STEP  =  7;

    private uint[] r250_buffer = new uint[250];
    private int r250_index;
    public Ran250(uint the_seed)
      : base(the_seed)
    {

      // initialize the maximum value max_val
      max_val = ALL_BITS;

      // set initial state in the r250_buffer
      int j, k;
      uint mask, msb;

      // set seed for the auxilliary GGL generator
      int GGL_seed = (int)seed;
  
      r250_index = 0;
      for (j = 0; j < 250; j++)      // fill r250 buffer with N_BITS-1 bit values 
        r250_buffer[j] = (uint)GGL(ref GGL_seed);

      for (j = 0; j < 250; j++) // set some MSBs to 1
        if ( GGL(ref GGL_seed) > HALF_RANGE )
          r250_buffer[j] |= MSB;

      msb = MSB;      // turn on diagonal bit
      mask = ALL_BITS;    // turn off the leftmost bits

      for (j = 0; j < N_BITS; j++) 
      {
        k = STEP * j + 3;   // select a word to operate on
        r250_buffer[k] &= mask; // turn off bits left of the diagonal
        r250_buffer[k] |= msb;  // turn on the diagonal bit
        mask >>= 1;
        msb  >>= 1;
      }
    }

    public override uint Long()
    {
      int j;
      uint new_rand;

      if ( r250_index >= 147 )
        j = r250_index - 147; // wrap pointer around 
      else
        j = r250_index + 103;

      new_rand = r250_buffer[r250_index] ^ r250_buffer[j];
      r250_buffer[r250_index] = new_rand;

      if ( r250_index >= 249 )  // increment pointer for next time
        r250_index = 0;
      else
        r250_index++;

      return new_rand;    // return random number
    }

    static int GGL (ref int seed)
      // 
      // Park and Miller's minimal standard random generator 'GGL':
      // An algorithm from CACM 31 no. 10, pp. 1192-1201, October 1988.
      // x[n+1] = (16807 * x[n]) mod (2^31 - 1)
      //
    {
      const int A = 16807, M = 0x7fffffff, Q = 127773, R = 2836;
      seed = A * (seed % Q) - R * (seed / Q);
      if (seed <= 0) seed += M;
      return seed;
    }

  }
  #endregion

  #region Ran800


  
  /// <summary>
  /// Ran800: huge period generator TT800 of Matsumoto and Kurita.
  /// Returns  integer random numbers uniformly distributed within [0,4294967295]
  ///          (that means [0,2^32-1].
  /// </summary>
  /// <remarks><code>
  ///
  /// This is a twisted GFSR generator proposed by Matsumoto and
  /// Kurita in the ACM Transactions on Modelling and Computer
  /// Simulation, Vol. 4, No. 3, 1994, pp. 254-266. This generator has a
  /// period of 2^800 - 1 and excellent equidistribution properties up to
  /// dimension 25. A TGFSR with a period of more than 2^11000 is
  /// currently under construction by M. Matsumoto and T. Nishimura.
  ///
  /// The original code has been adapted to the general random generator class
  /// interface of Matpack, 1997.
  ///
  /// Original notes of the authors:
  ///   A C-program for TT800 : July 8th 1996 Version 
  ///   by M. Matsumoto, email: matumoto@math.keio.ac.jp 
  ///   genrand() generate one pseudorandom number with double precision 
  ///   which is uniformly distributed on [0,1]-interval 
  ///   for each call.  One may choose any initial 25 seeds 
  ///   except all zeros. 
  ///
  /// References: 
  ///   (1) ACM Transactions on Modelling and Computer Simulation, 
  ///       Vol. 4, No. 3, 1994, pages 254-266. 
  ///   (2) This is one of the recommended generators in:
  ///       Pierre L'Ecuyer, "Random Number Generation", Chapter 4
  ///       of the "Handbook on Simulation", Ed. Jerry Banks, Wiley, 1997.
  ///
  /// </code></remarks>
  public class Ran800 : RandomGenerator 
  { // Matsumoto's GFSR generator "TT800"

    const int N = 25, M = 7;    
    private int k;
    uint[] x = new uint[25];
    
    static uint[] x_data = 
   { 
     // initial 25 seeds, change as you wish 
     0x95f24dab, 0x0b685215, 0xe76ccae7, 0xaf3ec239, 0x715fad23,
     0x24a590ad, 0x69e4b5ef, 0xbf456141, 0x96bc1b7b, 0xa7bdf825,
     0xc1de75b7, 0x8858a9c9, 0x2da87693, 0xb657f9dd, 0xffdc8a9f,
     0x8121da71, 0x8b823ecb, 0x885d05f5, 0x4e20cd47, 0x5a9ad5d9,
     0x512c0c03, 0xea857ccd, 0x4cc1d30f, 0x8891a8a1, 0xa6b7aadb
   };

    // this is magic vector `a', don't change
    readonly uint[] mag01 = { 0x0, 0x8ebfd028 };
    public Ran800 () : this(0){}
    public Ran800 (uint the_seed)
      : base(the_seed)
    { 
             

      // initialize the maximum value: max_val
      max_val = (uint) 0xffffffff;

      // initialize seeds
      k = 0;
  
      // combine table with given seed
      for (int i = 0; i < N; i++) x[i] = x_data[i] ^ seed;
    }

    public override uint Long()
    {
 

      if (k == N) 
      { 
        // generate N words at one time
        int kk;
        for (kk = 0; kk < N-M; kk++) 
        {
          x[kk] = x[kk+M] ^ (x[kk] >> 1) ^ mag01[x[kk] % 2];
        }
        for ( ; kk < N; kk++) 
        {
          x[kk] = x[kk+(M-N)] ^ (x[kk] >> 1) ^ mag01[x[kk] % 2];
        }
        k = 0;
      }
  
      uint y;
      y = x[k];
      y ^= (y << 7) & 0x2b5b2500; // s and b, magic vectors
      y ^= (y << 15) & 0xdb8b0000;  // t and c, magic vectors 
      y &= 0xffffffff;    // you may delete this line if word size = 32
 
      // the following line was added by Makoto Matsumoto in the 1996 version
      // to improve lower bit's corellation.
      // Delete this line to use the code published in 1994.
      y ^= (y >> 16);     // added to the 1994 version
      k++;

      return y;
    }
  }
  #endregion

  #region Ran19937

  
    
  /// <summary>
  /// Ran19937: huge period generator MT19937B of Matsumoto and Nishimura
  /// Returns integer random numbers uniformly distributed within [0,4294967295]
  ///          (that means [0,2^32-1]
  /// </summary>
  /// <remarks><code>
  ///
  /// The Mersenne Twister, a new variant of the twisted GFSR (``TGFSR'') 
  /// by Matsumoto and Nishimura, sets new standards for the period, quality 
  /// and speed of random number generators. The incredible period
  /// is 2^19937 - 1, a number with about 6000 decimal digits; the 32-bit random 
  /// numbers exhibit best possible equidistribution properties in dimensions up
  /// to 623; and it's fast, very fast. 
  /// A paper on the Mersenne Twister has been submitted to ACM TOMACS.
  ///
  /// May 1997: First empirical results for this generator are available 
  /// on the news page of the pLab group at the University of Salzburg's
  /// Mathematics Department. WWW address: "http://random.mat.sbg.ac.at/news/".
  ///
  /// The original code has been adapted to the general random generator class
  /// interface of Matpack, 1997.
  ///
  ///----------------------------------------------------------------------------//
  ///
  /// Original Notes of the authors:    
  ///
  ///   A C-program for MT19937B: Integer Version
  ///   genrand() generate one pseudorandom integer which is
  ///   uniformly distributed among the 32bit unsigned integers
  ///   sgenrand(seed) set initial values to the working area of 624 words.
  ///   sgenrand(seed) must be called once before calling genrand()
  ///   (seed is any integer except 0).
  ///
  ///   LICENCE CONDITIONS: 
  ///
  ///                Matsumoto and Nishimura consent to GNU General 
  ///                Public Licence for this code.
  ///
  ///    NOTE: 
  ///                When you use it in your program, please let Matsumoto
  ///                (matumoto@math.keio.ac.jp) know it.
  ///
  /// </code></remarks>
  public class Ran19937: RandomGenerator 
  { // Matsumoto's GFSR generator "MT19937B"

    // Period parameters
    const uint  N = 624;
    const uint  M =397;
    const uint  MATRIX_A = 0x9908b0df;   // constant vector a
    const uint  UPPER_MASK = 0x80000000; // most significant w-r bits
    const uint  LOWER_MASK = 0x7fffffff; // least significant r bits

    // for tempering
    const uint TEMPERING_MASK_B = 0x9d2c5680;
    const uint TEMPERING_MASK_C = 0xefc60000;
    static uint TEMPERING_SHIFT_U(uint y) { return (y >> 11); }
    static uint  TEMPERING_SHIFT_S(uint y)  { return(y << 7); }
    static uint  TEMPERING_SHIFT_T(uint y)  { return(y << 15); }
    static uint  TEMPERING_SHIFT_L(uint y)  { return(y >> 18); }

    private int k;
    uint[] ptgfsr = new uint[624];
    public Ran19937(uint the_seed)
      : base(the_seed)
    { 
          

      // initialize the maximum value: max_val
      max_val = (uint) 0xffffffff;
  
      // setting initial seeds to ptgfsr[N] using
      // the generator Line 25 of Table 1 in
      // [KNUTH 1981, The Art of Computer Programming
      // Vol. 2 (2nd Ed.), pp102]

      ptgfsr[0]= seed & 0xffffffff;
      for (int i = 1; i < N; i++)
        ptgfsr[i] = (69069 * ptgfsr[i-1]) & 0xffffffff;
  
      // set initial index
      k = 1;
    }

    static readonly uint[] mag01 = { 0x0, MATRIX_A };

    public override uint Long()
    {
      
      uint y;
      unchecked
      {
        // mag01[x] = x * MATRIX_A  for x=0,1
  
        if (k == N) 
        { // generate N words at one time
          int kk;
          for (kk = 0; kk < N-M; kk++) 
          {
            y = (ptgfsr[kk]&UPPER_MASK)|(ptgfsr[kk+1]&LOWER_MASK);
            ptgfsr[kk] = ptgfsr[kk+M] ^ (y >> 1) ^ mag01[y & 0x1];
          }
          for (; kk < N-1; kk++) 
          {
            y = (ptgfsr[kk]&UPPER_MASK)|(ptgfsr[kk+1]&LOWER_MASK);
            ptgfsr[kk] = ptgfsr[kk+(M-N)] ^ (y >> 1) ^ mag01[y & 0x1];
          }
          y = (ptgfsr[N-1]&UPPER_MASK)|(ptgfsr[0]&LOWER_MASK);
          ptgfsr[N-1] = ptgfsr[M-1] ^ (y >> 1) ^ mag01[y & 0x1];
  
          k = 0;
        }
  
        y = ptgfsr[k++];
        y ^= TEMPERING_SHIFT_U(y);
        y ^= TEMPERING_SHIFT_S(y) & TEMPERING_MASK_B;
        y ^= TEMPERING_SHIFT_T(y) & TEMPERING_MASK_C;
        y &= 0xffffffff; // you may delete this line if word size = 32
        y ^= TEMPERING_SHIFT_L(y);

      }
      return y;

    }
  }
  #endregion

  #region ProbabilityDistribution

  /// <summary>
  /// Base class for all distributions.
  /// </summary>
  public abstract class ProbabilityDistribution : RandomGenerator
  {
    /// <summary>Pointer to generator.</summary>
    protected RandomGenerator gen; 

    /// <summary>Empty constructor. Use this only in inheritance hierarchies.</summary>
    protected ProbabilityDistribution()
    {
    }

    /// <summary>
    /// Generates a random value distributed according to the distribution.
    /// </summary>
    /// <returns></returns>
    public abstract double val();

    /// <summary>Initialize the distribution by providing the random generator used
    /// to produce the random values.</summary>
    /// <param name="ran">The random generator used to produce the random values.</param>
    public  ProbabilityDistribution(RandomGenerator ran)
    {
      gen = ran;
      max_val = ran.Maximum;
      seed = ran.Seed;
    }

    /// <summary>Returns the random generator used by the distribution to generate the random values.</summary>
    /// <value>The used random generator.</value>
    public RandomGenerator Generator
    {
      get { return gen; }
    }

    /// <summary>The generation function.</summary>
    /// <returns>The next random value.</returns>
    public override uint Long()
    {
      return gen.Long();
    }


    /// <summary>
    /// Gives the probability density at x.
    /// </summary>
    /// <param name="x">Location.</param>
    /// <returns>Probability density at x.</returns>
    public abstract double PDF(double x);
    
    /// <summary>
    /// Gives the cumulative probability at x.
    /// </summary>
    /// <param name="x">Location.</param>
    /// <returns>Cumulative probability at x.</returns>
    public abstract double CDF(double x);
    /// <summary>
    /// Gives the pth quantile of the distribution.
    /// </summary>
    /// <param name="p">The probability.</param>
    /// <returns>The pth quantile, that is the value x for with holds: p==CDF(x)</returns>
    public abstract double Quantile(double p);

  }
  #endregion

  #region UniformDistribution

  
  /// <summary>
  /// Generates uniformly distributed random numbers over [a,b]
  /// </summary>
  public class UniformDistribution : ProbabilityDistribution 
  {
    protected double scale, low, high;
    protected UniformDistribution() {}
    public UniformDistribution(double lo, double hi)
      : this(lo,hi,DefaultGenerator)
    {
    }
    public UniformDistribution (double lo, double hi, RandomGenerator ran) 
      : base(ran)
    {
      if (hi > lo) 
      { 
        low = lo; high = hi;
      } 
      else 
      { 
        low = hi; high = lo; 
      }
      scale = (high-low) / max_val;
    }

    public override double val() // we may not want a getter here, because debugging then changes the state of the generator
    {
      return (scale * gen.Long() + low);
    }
    
    public override uint Long()
    {
      return gen.Long();
    }
    public double LowerBound
    {
      get { return low; }
    }
    public double UpperBound 
    {
      get { return high; }
    }

  
    public override double PDF(double z)
    {
      if(z<low)
        return 0;
      else if(z>=high)
        return 0;
      else
        return 1.0/(high-low);
    }

    public override double CDF(double z)
    {
      if(z<low)
        return 0;
      else if(z>=high)
        return 1;
      else
        return (z-low)/(high-low);
    }

    public override double Quantile(double p)
    {
      if(p<0 || p>1)
        throw new ArgumentException("Probability p must be between 0 and 1");
      return low + p*(high-low);
    }

  
  }

  #endregion

  #region U01_Distribution
  

  /// <summary>
  /// Uniformly distributed random numbers over [0,1].  
  /// This is a special case and equivalent to class UniformDistribution(0,1).
  /// </summary>
  /// <remarks>It is included just because of speed considerations.</remarks>
  public class U01_Distribution : ProbabilityDistribution 
  {
    protected double scale;
    public U01_Distribution() : this(DefaultGenerator) {}
    public U01_Distribution(RandomGenerator ran)
      : base(ran)
    {
      scale = 1.0 / max_val;
    }
    public override double val() 
    {
      return scale * gen.Long();
    }
    public override uint Long()
    {
      return gen.Long();
    }

    public override double PDF(double z)
    {
      if(z<0)
        return 0;
      else if(z>=1)
        return 0;
      else
        return 1;
    }

    public override double CDF(double z)
    {
      if(z<0)
        return 0;
      else if(z>=1)
        return 1;
      else
        return z;
    }

    public override double Quantile(double p)
    {
      if(p<0 || p>1)
        throw new ArgumentException("Probability p must be between 0 and 1");
      return p;
    }
  }
  #endregion

  #region NormalDistribution

  

  /// <summary>
  /// Generates normal (Gaussian) distributed random numbers.
  /// </summary>
  /// <remarks><code>
  /// Return normal (Gaussian) distributed random deviates           
  /// with mean "m" and standard deviation  "s" according to the density:    
  ///                           
  ///                                           2                 
  ///                      1               (x-m)               
  ///  p   (x) dx =  ------------  exp( - ------- ) dx            
  ///   m,s          sqrt(2 pi) s          2 s*s           
  ///                          
  /// </code></remarks>
  public class NormalDistribution : ProbabilityDistribution 
  {
    protected double m, s, scale, cacheval;
    protected int cached;
    protected NormalDistribution() {}
    public NormalDistribution (double mean, double stdev, RandomGenerator ran)
      : base(ran)
    {
      cached = 0;
      m = mean;
      s = stdev;
      scale  = 2.0 / max_val;
    }
    public override double val()
    {
      // We don't have an extra deviate
      if  (cached == 0) 
      {

        // Pick two uniform numbers in the square extending from -1 tp +1
        // in each direction and check if they are in the unit circle
        double v1, v2, r;
        do 
        {
          v1 = scale * gen.Long() - 1; // scale maps the random long to [0,2]
          v2 = scale * gen.Long() - 1;
          r = v1 * v1 + v2 * v2;
        } while (r >= 1.0);

        double f = Math.Sqrt( (-2 * Math.Log(r)) / r);

        // Make Box-Muller transformation to get two normal deviates.
        // Return one and save the other for the next time.
        cacheval = v1 * f;
        cached = 1;
        return (v2 * f * s + m);

        // We have an extra deviate, so unset the flag and return it
      } 
      else 
      {
        cached = 0;
        return (cacheval * s + m);
      }
    }
    public override uint Long()
    {
      return gen.Long();
    }
    public virtual double Mean { get { return m; }}
    public virtual double Stdev {get { return s; }}


    static readonly double _OneBySqrt2Pi = 1/Math.Sqrt(2*Math.PI);
    static double Sqr(double x) { return x*x; }
    public override double PDF(double z)
    {
      return _OneBySqrt2Pi*Math.Exp(-0.5*Sqr((z-m)/s))/s;
    }

    static readonly double _OneBySqrt2 = 1/Math.Sqrt(2);
    public override double CDF(double z)
    {
      return 0.5*(1+Altaxo.Calc.ErrorFunction.Erf(_OneBySqrt2*(z-m)/s));
    }

    public override double Quantile(double p)
    {
      return m+s*ErrorFunction.QuantileOfNormalDistribution01(p);
    }
  }


  #endregion

  #region LogNormalDistribution

  /// <summary>
  /// Generates log-normal distributed random numbers.
  /// </summary>
  /// <remarks><code>                       
  /// Return log-normal distributed random deviates           
  /// with given mean and standard deviation stdev              
  /// according to the density function:                
  ///                                                2            
  ///                     1                (ln x - m)           
  /// p   (x) dx =  -------------- exp( - ------------ ) dx  for x > 0       
  ///  m,s          sqrt(2 pi x) s               2              
  ///                                         2 s             
  ///                       
  ///            = 0   otherwise                  
  ///                       
  /// where  m  and  s  are related to the arguments mean and stdev by:       
  ///                               
  ///                        2                  
  ///                    mean                 
  /// m = ln ( --------------------- )               
  ///                     2      2                  
  ///          sqrt( stdev + mean  )                
  ///                         
  ///                     2      2                  
  ///                stdev + mean                 
  /// s = sqrt( ln( -------------- ) )                
  ///                        2                  
  ///                    mean                  
  ///                            
  /// 
  /// </code></remarks>
  public class LogNormalDistribution 
    : NormalDistribution 
  {
    protected double m_log, s_log;
    protected void Initialize (double mean, double stdev)
    {
      // set mean and standard deviation of the log-normal distribution
      m_log = mean; 
      s_log = stdev; 
    
      // mean "m" and standard deviation "s" of the corresponding 
      // normal distribution wich is a base class of this class
      double m2 = m_log * m_log, 
        s2 = s_log * s_log,
        sm2 = s2+m2;
      m = Math.Log( m2 / Math.Sqrt(sm2) );
      s = Math.Sqrt( Math.Log(sm2/m2) );
    }


    protected LogNormalDistribution() {}
    public LogNormalDistribution(double mean, double stdev)
      : this(mean, stdev, DefaultGenerator)
    {
    }

    public LogNormalDistribution (double mean, double stdev, RandomGenerator ran) 
      : base(mean,stdev,ran) 
    {
      Initialize(mean,stdev);
    }
    public override double val() 
    {
      return Math.Exp(base.val());
    }
    public override double Mean { get { return m_log; }}
    public override double Stdev { get { return s_log; }}


    static double Sqr(double x) { return x*x; }
    static readonly double _OneBySqrt2Pi = 1/Math.Sqrt(2*Math.PI);
    public override double PDF(double z)
    {
      if(z<=0)
        return 0;
      else
        return _OneBySqrt2Pi*Math.Exp(-0.5*Sqr((Math.Log(z)-m)/s))/(s*z);
    }

    static readonly double _OneBySqrt2 = 1/Math.Sqrt(2);
    public override double CDF(double z)
    {
      return 0.5*(1+Altaxo.Calc.ErrorFunction.Erf(_OneBySqrt2*(Math.Log(z)-m)/s));
    }

    public override double Quantile(double p)
    {
      return Math.Exp(m+s*ErrorFunction.QuantileOfNormalDistribution01(p));
    }
  }

  #endregion

  #region ExponentialDistribution
  /// <summary>
  /// Generates exponentially distributed random numbers.
  /// </summary>
  /// <remarks><code>
  ///                            
  /// Return exponentially distributed random deviates according to:      
  ///                            
  /// p (x) = 1/m * exp(-x/m) dx   for x >= 0             
  ///  m                              
  ///       = 0                    otherwise             
  ///                            
  /// The probability density has mean = stdev = m.          
  ///                            
  /// </code></remarks>
  public class ExponentialDistribution :  ProbabilityDistribution 
  {
    protected double m, scale;
    protected ExponentialDistribution() {}
    
    public ExponentialDistribution (double mean, RandomGenerator ran)
      : base(ran)
    {
      m = mean;
      scale = 1.0/max_val;
    }

    public override double val() 
    {
      return (-m * Math.Log( gen.Long() * scale)); 
    }
    
    public override uint Long()
    {
      return gen.Long();
    }
    public double Mean { get {return m; }}
  
  
    public override double PDF(double z)
    {
      if(z<0)
        return 0;
      else
        return Math.Exp(-z/m)/m;
    }
    public override double CDF(double z)
    {
      if(z<0)
        return 0;
      else
        return 1-Math.Exp(-z/m);
    }
    public override double Quantile(double p)
    {
      return -m*Math.Log(1-p);
    }
  }

  
  #endregion

  #region ErlangDistribution

  /// <summary>
  /// Generates Erlang distributed random numbers.
  /// </summary>
  /// <remarks><code>
  ///                           
  /// Return Erlang distributed random deviates according to:        
  ///                             
  ///                      a-1  -bx              
  ///                b (bx)    e                  
  ///  p   (x) dx = ---------------- dx   for x > 0           
  ///   a,b             Gamma(a)                 
  ///                         
  ///             =  0                    otherwise          
  ///                             
  /// The Erlang distribution is a special case of the Gamma distribution         
  /// with integer valued order a.                       
  ///                            
  /// References:                     
  /// see references in:                       
  /// W. H. Press, B. P. Flannery, S. A. Teukolsky, W. T. Vetterling,       
  /// Numerical Recipies in C, Cambridge Univ. Press, 1988.         
  /// </code></remarks>

  public class ErlangDistribution :  ProbabilityDistribution 
  {
    protected int A;
    protected double B, a1, sq, scale, scale2;
    protected void SetOrder (int order, double loc)
    {
      if (order < 1) 
        throw new ArgumentException("order must be greater or equal than 1");
      else if (loc == 0)
        throw new ArgumentException("location parameter must be non-zero");
      else 
      {
        scale  = 1.0 / max_val;   // scale long to [0,1]
        scale2 = 2.0 / max_val;   // auxilliary
        A = order;      // order of Erlang distribution
        a1 = A-1.0;     // auxilliary
        sq = Math.Sqrt( 2 * a1 + 1 ); // auxilliary
        B = loc;      // location parmeter
      }
    }
    
    public ErlangDistribution (int order, double loc) : this(order,loc,DefaultGenerator) {}
    public ErlangDistribution (int order, double loc, 
      RandomGenerator ran): base(ran) 
    { 
      SetOrder(order,loc); 
    }
    public override double val()
    {
      if (A < 6) 
      { // direct method
        double x;
        do 
        {
          x = gen.Long() * scale;
          for (int i = 1; i < A; i++) x *= gen.Long() * scale;
        } while (x <= 0.0);
        return ( -Math.Log(x)/B );

      } 
      else 
      {   // rejection method
        double x, y, b;
        do 
        {
          do 
          {
            double v1,v2;
            do 
            {
              v1 = scale2 * gen.Long() - 1;
              v2 = scale2 * gen.Long() - 1;
            } while ( (v1 == 0.0) || (v1*v1 + v2*v2 > 1.0) );
            y = v2/v1;
            x = sq*y + a1;
          } while (x <= 0.0);
          b = (1.0 + y*y) * Math.Exp( a1 * Math.Log(x/a1) - sq*y );
        } while ( (scale * gen.Long()) > b );
        return x/B;
      }
    }


    public override uint Long()
    {
      return gen.Long();
    }
    public int Order { get {return A; }}
    public double Location { get { return B; }}
  
  
    public override double PDF(double x)
    {
      return Math.Exp(-B*x)*Math.Pow(B*x,A-1)*B/Calc.GammaRelated.Gamma(A);
    }

    public override double CDF(double x)
    {
      return GammaRelated.GammaRegularized(A,0,B*x);
    }

    public override double Quantile(double p)
    {
      return GammaRelated.InverseGammaRegularized(A,1-p)/B;
    }

  
  }

  #endregion

  #region GammaDistribution

  /// <summary>
  /// Generates Gamma distributed random numbers.
  /// </summary>
  /// <remarks><code>
  /// Return Gamma distributed random deviates according to:        
  ///                            
  ///                      a-1  -bx               
  ///                b (bx)    e                  
  ///  p   (x) dx = ---------------- dx   for x > 0           
  ///   a,b             Gamma(a)                  
  ///                        
  ///             =  0                    otherwise           
  ///                             //
  /// The arguments must satisfy the conditions:                   
  /// a > 0   (positive)                       
  /// b != 0  (non-zero)                        
  ///                             
  /// References:                     
  ///                            
  /// For parameter a >= 1 corresponds to algorithm GD in:                
  /// J. H. Ahrens and U. Dieter, Generating Gamma Variates by a        
  /// Modified Rejection Technique, Comm. ACM, 25, 1, 47-54 (1982).       
  /// For parameter 0 &lt; a &lt; 1 corresponds to algorithm GS in:               
  /// J. H. Ahrens and U. Dieter, Computer Methods for Sampling        
  /// from Gamma, Beta, Poisson and Binomial Distributions,         
  /// Computing, 12, 223-246 (1974).                
  /// </code></remarks>

  public class GammaDistribution : ProbabilityDistribution // , public ExponentialDistribution 
  {
    protected NormalDistribution normalDistribution;
    protected ExponentialDistribution exponentialDistribution;
    protected double A, B, s, s2, d, r, q0, b, si, c, scale;
    protected bool algorithmGD;
    protected void Initialize (double order, double loc)
    {
      // check parameters
      if (order <= 0)
        throw new ArgumentException("order must be greater than zero");
      if (loc == 0)
        throw new ArgumentException("location parameter must be non-zero");

      // store parameters
      A = order;  // a is the mean of the standard gamma distribution (b = 0)
      B = loc;

      // scale random long to (0,1) - boundaries are not allowed !
      scale  = 1.0 / (normalDistribution.Maximum+1.0); // original: scale  = 1.0 / (NormalDistribution::max_val+1.0);

      // select algorithm
      algorithmGD = (A >= 1);

      // initialize algorithm GD
      if (algorithmGD) 
      {    

        // coefficients q(k) for q0 = sum(q(k)*a**(-k))
        const double 
                q1 = 4.166669e-2,
                q2 = 2.083148e-2,
                q3 = 8.01191e-3,
                q4 = 1.44121e-3,
                q5 = -7.388e-5,
                q6 = 2.4511e-4,
                q7 = 2.424e-4;

        // calculates s, s2, and d
        s2 = A - 0.5;
        s = Math.Sqrt(s2);
        d = Math.Sqrt(32.0) - 12.0 * s; 

        // calculate q0, b, si, and c
        r = 1.0 / A;
        q0 = ((((((q7*r+q6)*r+q5)*r+q4)*r+q3)*r+q2)*r+q1)*r;

        // Approximation depending on size of parameter A.
        // The constants in the expressions for b, si, and
        // c were established by numerical experiments.

        if (A <= 3.686) 
        {   // case 1.0 <= A <= 3.686
          b = 0.463 + s + 0.178 * s2;
          si = 1.235;
          c = 0.195 / s - 7.9e-2 + 1.6e-1 * s;
        } 
        else if (A <= 13.022) 
        { // case  3.686 < A <= 13.022
          b = 1.654 + 7.6e-3 * s2;
          si = 1.68 / s + 0.275;
          c = 6.2e-2 / s + 2.4e-2;
        } 
        else 
        {     // case A > 13.022
          b = 1.77;
          si = 0.75;
          c = 0.1515 / s;
        }

        // initialize algorithm GS
      } 
      else 
      {
        b = 1.0 + 0.3678794 * A;
      }
    }

    public GammaDistribution (double order, double loc) : this(order,loc,DefaultGenerator) {}
    public GammaDistribution (double order, double loc, 
      RandomGenerator ran) 
      // original: NormalDistribution(0.0,1.0,ran), // std. normal
    {
      normalDistribution = new NormalDistribution(0.0,1.0,ran);
      exponentialDistribution = new ExponentialDistribution(1.0,ran); // std. exponential
      
      Initialize(order,loc);
    }
  
    public override double val()
    {
      // algorithm GD for A >= 1
      if (algorithmGD) 
      {

        const double 
                // coefficients a(k) for q = q0+(t*t/2)*sum(a(k)*v**k)  
                a1 = 0.3333333,
                a2 = -0.250003,
                a3 = 0.2000062,
                a4 = -0.1662921,
                a5 = 0.1423657,
                a6 = -0.1367177,
                a7 = 0.1233795,
                // coefficients e(k) for exp(q)-1 = sum(e(k)*q**k)  
                e1 = 1.0,
                e2 = 0.4999897,
                e3 = 0.166829,
                e4 = 4.07753E-2,
                e5 = 1.0293E-2;
  
        double q, w, gamdis;

        // standard normal deviate
        double t = normalDistribution.val(); // original  this->NormalDistribution::operator()();

        // (s,1/2)-normal deviate
        double x = s + 0.5 * t;
  
        // immediate acceptance
        gamdis = x * x;
        if (t >= 0.0) return gamdis/B;

        // (0,1) uniform sample, squeeze acceptance
        double u = normalDistribution.Long() * scale; // original NormalDistribution::gen->Long() * scale;
        if ( d * u <= t * t * t ) return gamdis/B;

        // no quotient test if x not positive
        if (x > 0.0) 
        { 

          // calculation of v and quotient q
          double vv = t / (s + s);
          if (Math.Abs(vv) <= 0.25)
            q = q0 + 0.5*t*t * ((((((a7*vv+a6)*vv+a5)*vv+a4)*vv+a3)*vv+a2)*vv+a1)*vv;
          else 
            q = q0 - s * t + 0.25 * t * t + (s2+s2) * Math.Log(1.0+vv);
      
          // quotient acceptance 
          if (Math.Log(1.0-u) <= q) return gamdis/B;
        }

      loop:

        // stdandard exponential deviate
        double e = exponentialDistribution.val(); // original this->ExponentialDistribution::operator()();

        // (0,1) uniform deviate
        u =  normalDistribution.Long() * scale; // NormalDistribution::gen->Long() * scale;

        u += (u-1.0); 

        // (b,si) double exponential (Laplace)
        t = b + CopySign(si*e,u);

        // rejection if t < tau(1) = -0.71874483771719
        if (t < -0.71874483771719) goto loop;

        // calculation of v and quotient q
        double v = t / (s + s);
        if (Math.Abs(v) <= 0.25) 
          q = q0 + 0.5*t*t * ((((((a7*v+a6)*v+a5)*v+a4)*v+a3)*v+a2)*v+a1)*v;
        else
          q = q0 - s * t + 0.25*t*t + (s2+s2) * Math.Log(1.0+v);
  
        // hat acceptance
        if (q <= 0.0) goto loop;

        if (q <= 0.5) 
          w = ((((e5*q+e4)*q+e3)*q+e2)*q+e1)*q;
        else
          w = Math.Exp(q) - 1.0;

        // if t is rejected, sample again
        if ( c * Math.Abs(u) > w * Math.Exp(e-0.5*t*t) ) goto loop;

        x = s + 0.5*t;
        gamdis = x * x;
        return gamdis/B;


        // algorithm GS for 0 < A < 1
      } 
      else 
      {
  
        double gamdis;
        for (;;) 
        {
          double p = b * normalDistribution.Long() * scale;
          if (p < 1.0) 
          {
            gamdis = Math.Exp( Math.Log(p) / A );
            if (exponentialDistribution.val() >= gamdis)
              return gamdis/B;
          } 
          else 
          {
            gamdis = -Math.Log( (b-p) / A );
            if (exponentialDistribution.val() >= (1.0-A)*Math.Log(gamdis)) return gamdis/B;
          }
        } // for

      }
    }

    public override uint Long() 
    {
      return normalDistribution.Long(); 
    }
    public double Order { get { return A; }}
    public double Location { get { return B; }}



    public override double PDF(double x)
    {
      return Math.Exp(-B*x)*Math.Pow(B*x,A-1)*B/Calc.GammaRelated.Gamma(A);
    }

    public override double CDF(double x)
    {
      return GammaRelated.GammaRegularized(A,0,B*x);
    }

    public override double Quantile(double p)
    {
      return GammaRelated.InverseGammaRegularized(A,1-p)/B;
    }


  }

  #endregion

  #region BetaDistribution

  /// <summary>
  /// Generates Beta distributed random numbers.
  /// </summary>
  /// <remarks><code>
  ///                            
  /// Return Beta distributed random deviates according to the density    
  ///                             
  ///                 a-1       b-1             
  ///                x     (1-x)                 
  ///  p   (x) dx = --------------- dx   for 0 &lt; x &lt; 1          
  ///   a,b              B(a,b)                
  ///                        
  ///             =  0                   otherwise             
  ///                             
  /// References:                     
  ///                            
  /// R. C. H. Cheng, Generating Beta Variatew with Non-integral Shape      
  /// Parameters, Comm. ACM, 21, 317-322 (1978). (Algorithms BB and BC).   
  ///                             
  /// </code></remarks>

  public class BetaDistribution : ProbabilityDistribution 
  {
    protected double aa, bb;
    protected double scale, a, alpha, b, beta, delta, gamma, k1, k2, maxexp;
    protected bool algorithmBB;
    protected void Initialize (double pa, double pb)
    {
      // check parameters
      if (pa <= 0.0 || pb <= 0.0)
        throw new ArgumentException("arguments a and b must be positive");

      // store parameters
      aa = pa; 
      bb = pb;

      // scale random long to (0,1) - boundaries are not allowed !
      scale  = 1.0 / (max_val+1.0);

      // maximal exponent for exp() function in evaluation "a*exp(v)" below
      maxexp = DBL_MAX_EXP * M_LN2 - 1;

      if (a > 1.0) maxexp -= Math.Ceiling(Math.Log(a));

      algorithmBB = Math.Min(aa,bb) > 1.0;

      // initialize algorithm BB
      if (algorithmBB) 
      {
        a = Math.Min(aa,bb);
        b = Math.Max(aa,bb);
        alpha = a + b;
        beta  = Math.Sqrt( (alpha - 2.0) / (2.0 * a * b - alpha) );
        gamma = a + 1.0/beta;

        // initialize algorithm BC
      } 
      else 
      {
        a = Math.Max(aa,bb);
        b = Math.Min(aa,bb);
        alpha = a + b;
        beta  = 1.0/b;
        delta = 1.0 + a - b;
        k1 = delta * (1.38889e-2 + 4.16667e-2 * b) / (a * beta - 0.777778);
        k2 = 0.25 + (0.5 + 0.25 / delta) * b;
      }
    }
    public BetaDistribution(double pa, double pb) : this(pa,pb,DefaultGenerator) {}
    public BetaDistribution (double pa, double pb, 
      RandomGenerator ran)
      : base(ran) 
    { Initialize(pa,pb); }
    public override double val()
    {         
      // returned on overflow
      const double infinity = DBL_MAX;

      double w;

      // Algorithm BB
      if (algorithmBB) 
      {

        double r, t;
        do 
        {
          double u1 = gen.Long() * scale;
          double u2 = gen.Long() * scale;
          double v = beta * Math.Log(u1/(1.0-u1));
          w = (v > maxexp) ? infinity : a * Math.Exp(v);  
          double z = u1 * u1 * u2;
          r = gamma * v - 1.3862944;
          double s = a + r - w;
          if (s + 2.609438 >= 5.0 * z) break;
          t = Math.Log(z);
          if (s > t) break;     
        } while (r + alpha * Math.Log(alpha/(b+w)) < t);

        // Algorithm BC
      } 
      else 
      { 

      loop:

        double v, y, z;
        double u1 = gen.Long() * scale;
        double u2 = gen.Long() * scale;

        if (u1 < 0.5) 
        {
          y = u1 * u2;
          z = u1 * y;
          if (0.25 * u2 + z - y >= k1) goto loop;
        } 
        else 
        {
          z = u1 * u1 * u2;
          if (z <= 0.25) 
          {
            v = beta * Math.Log(u1/(1.0-u1));
            w = (v > maxexp) ? infinity: a * Math.Exp(v);
            goto fin;
          }
          if (z >= k2) goto loop;
        }
        v = beta * Math.Log(u1/(1.0-u1));
        w = (v > maxexp) ? infinity : a * Math.Exp(v);
        if (alpha * (Math.Log(alpha/(b+w))+v) - 1.3862944 < Math.Log(z)) goto loop;

      fin: ;
      }

      // return result
      return (a == aa) ? w/(b+w) : b/(b+w);   
    }


    public override uint Long()
    {
      return gen.Long(); 
    }
    public double A {  get{ return aa; }}
    public double B { get { return bb; }}


    public override double PDF(double x)
    {
      if (x < 0 || x > 1)
      {
        return 0 ;
      }
      else 
      {
        double p;

        double gab = Calc.GammaRelated.LnGamma(a + b);
        double ga = Calc.GammaRelated.LnGamma(a);
        double gb = Calc.GammaRelated.LnGamma(b);

        p = Math.Exp (gab - ga - gb) * Math.Pow (x, A - 1) * Math.Pow(1 - x, B - 1);

        return p;
      }
    } 
  

    public override double CDF(double x)
    {
      return Calc.GammaRelated.BetaRegularized(x,A,B);
    }
    public override double Quantile(double p)
    {
      return Calc.GammaRelated.InverseBetaRegularized(p,A,B);
    }
  }


  #endregion

  #region ChiSquareDistribution

  /// <summary>
  /// Generates central chi-square distributed random numbers.
  /// </summary>
  /// <remarks><code>
  /// Generates random deviates from a central chi-square distribution with 
  /// f degrees of freedom. f must be positive. 
  /// The density of this distribution is:
  ///
  ///
  ///                -f/2   f/2-1  -x/2
  ///               2      x      e
  ///  p (x) dx =  --------------------- dx  for x > 0
  ///   f               Gamma(f/2)
  ///
  ///           =  0                         otherwise
  ///
  /// The calculation uses the relation between chi-square and gamma distribution:
  ///
  ///  ChiSquare(f) = GammaDistribution(f/2,1/2)
  ///
  /// References:
  ///    K. Behnen, G. Neuhaus, "Grundkurs Stochastik", Teubner Studienbuecher
  ///    Mathematik, Teubner Verlag, Stuttgart, 1984.
  ///
  /// </code></remarks>
  public class ChiSquareDistribution : GammaDistribution
  {
    protected double F;
    public ChiSquareDistribution (double f) : this(f,DefaultGenerator) {}
    public ChiSquareDistribution (double f, RandomGenerator ran) 
      : base(0.5*f,1.0,ran)
    {
      F = f;
    }
    public new double val() 
    {
      return 2.0 * base.val();
    }
    public double Freedom  { get { return F; }}

    public override double PDF(double x)
    {
      return Math.Pow(x,-1 + 0.5*F)/(Math.Pow(2,0.5*F)*Math.Exp(0.5*x)*Calc.GammaRelated.Gamma(0.5*F));
    }

    public override double CDF(double x)
    {
      return Calc.GammaRelated.GammaRegularized(0.5*F, 0, 0.5*x);
    }

    public override double Quantile(double p)
    {
      return 2*GammaRelated.InverseGammaRegularized(0.5*F,1-p);
    }
  }
  #endregion

  #region FDistribution

  /// <summary>
  /// Generates F-distributed random numbers.
  /// </summary>
  /// <remarks><code>
  /// Return F-distributed (variance ratio distributed) random deviates       
  /// with n numerator degrees of freedom and d denominator degrees of freedom  
  /// according to the density:                 
  ///                            
  ///   p   (x) dx =  ... dx                 
  ///    n,d                      
  ///                       
  /// Both paramters n and d must be positive.              
  ///                       
  /// Method: The random numbers are directly generated from ratios of           
  ///         ChiSquare variates according to:              
  ///                            
  ///  F = (ChiSquare(n)/n) / (ChiSquare(d)/d)              
  ///                            
  /// </code></remarks>
  public class FDistribution : ProbabilityDistribution
  {
    //const double DBL_MIN = double.Epsilon; 
    //const double DBL_MAX = double.MaxValue; 

    protected double NF, DF;
    protected ChiSquareDistribution NumChi2, DenomChi2; // uninitialized !
    public  FDistribution (double numf, double denomf) : this(numf, denomf, RandomGenerator.DefaultGenerator) {}
    FDistribution (double numf, double denomf, RandomGenerator ran)
      : base(ran)
    { 
      // check parameter range
      if (numf < 0.0 || denomf < 0.0)
        throw new ArgumentException(string.Format("Numerator ({0}) and denominator ({1}) degrees of freedom must be positive", numf, denomf));
  
      // store parameters
      NF = numf; 
      DF = denomf; 
  
      // initialize numerator and denominator chi-square distributions
      NumChi2 = new ChiSquareDistribution(NF,ran);
      DenomChi2 = new ChiSquareDistribution(DF,ran);
    }
 
    public override double val() 
    {
      double numerator   = NumChi2.val()/NF,
        denominator = DenomChi2.val()/DF;
  
      // return avoiding overflow
      return (denominator <= numerator*DBL_MIN) ? DBL_MAX : numerator/denominator;
    }
      
    public double NumF { get { return NF; }}
    public double DenomF { get { return DF; }}


    /// <summary>
    /// Returns the probability density function for value x with the distribution parameters p and q.
    /// </summary>
    /// <param name="x">The function argument.</param>
    /// <returns>The probability density of the distribution at value x.</returns>
    public override double PDF(double x)
    {
      return (Math.Pow(NF,NF/2)*Math.Pow(DF,DF/2)*Math.Pow(x,(-2 + NF)/2)*Math.Pow(DF + NF*x,(-NF - DF)/2))/GammaRelated.Beta(NF/2,DF/2);
    }

    /// <summary>
    /// Returns the cumulated distribution function for value x with the distribution parameters numf and denomf.
    /// </summary>
    /// <param name="x">The function argument.</param>
    /// <returns>The cumulated distribution (probability) of the distribution at value x.</returns>
    public override double CDF(double x)
    {
      double n1x = NF * x;
      return GammaRelated.BetaIR(n1x/(DF+n1x),0.5*NF,0.5*DF);
    }

    /// <summary>
    /// Quantile of the F-distribution.
    /// </summary>
    /// <param name="alpha">Probability (0..1).</param>
    /// <returns>The quantile of the F-Distribution.</returns>
    public override double Quantile(double alpha)
    {
      double inverse_beta = GammaRelated.InverseBetaRegularized(1-alpha, DF/2, NF/2);
      return (DF/NF) * (1.0 / inverse_beta - 1.0);
    }

    /// <summary>
    /// Returns the cumulated distribution function for value x with the distribution parameters numf and denomf.
    /// </summary>
    /// <param name="x">The function argument.</param>
    /// <param name="numf">First parameter of the distribution.</param>
    /// <param name="denomf">Second paramenter of the distribution.</param>
    /// <returns>The cumulated distribution (probability) of the distribution at value x.</returns>
    public static double CDF(double x, double numf, double denomf)
    {
      double n1x = numf * x;
      return GammaRelated.BetaIR(n1x/(denomf+n1x),0.5*numf,0.5*denomf);
    }

    /// <summary>
    /// Returns the probability density function for value x with the distribution parameters p and q.
    /// </summary>
    /// <param name="x">The function argument.</param>
    /// <param name="p">First parameter of the distribution.</param>
    /// <param name="q">Second paramenter of the distribution.</param>
    /// <returns>The probability density of the distribution at value x.</returns>
    public static double PDF(double x, double p, double q)
    {
      return (Math.Pow(p,p/2)*Math.Pow(q,q/2)*Math.Pow(x,(-2 + p)/2)*Math.Pow(q + p*x,(-p - q)/2))/GammaRelated.Beta(p/2,q/2);
    }

    /// <summary>
    /// Quantile of the F-distribution.
    /// </summary>
    /// <param name="alpha">Probability (0..1).</param>
    /// <param name="p">First parameter of the distribution.</param>
    /// <param name="q">Second parameter of the distribution.</param>
    /// <returns>The quantile of the F-Distribution.</returns>
    public static double Quantile(double alpha, double p, double q)
    {
      double inverse_beta = GammaRelated.InverseBetaRegularized(1-alpha, q/2, p/2);
      return (q/p) * (1.0 / inverse_beta - 1.0);
    }
  }

  #endregion

  #region PoissonDistribution (Discrete)

  /// <summary>
  /// Generates Poisson distributed random numbers.
  /// </summary>
  /// <remarks><code>
  /// Returns a Poisson distributed deviate (integer returned in a double)    
  /// from a distribution of mean m.               
  /// The Poisson distribution gives the probability of a certain integer      
  /// number m of unit rate Poisson random events occurring in a given       
  /// interval of time x.                   
  ///                                   j  -x                   
  ///              j+eps               x  e            
  ///      integral       p (m) dm  = -------                         
  ///              j-eps   x            j !                       
  ///                             
  /// References: The method follows the outlines of:                 
  /// W. H. Press, B. P. Flannery, S. A. Teukolsky, W. T. Vetterling,       
  /// Numerical Recipies in C, Cambridge Univ. Press, 1988.        
  /// </code></remarks>
  public class PoissonDistribution : ProbabilityDistribution 
  {
    protected double scale, scalepi, m, sq, alm, g;
    protected void Initialize (double mean)
    {
      m = mean; 
      scale  = 1.0 / max_val; 
    
      if (m < 12.0) 
      { // direct method
        g = Math.Exp(-m);
    
      } 
      else 
      {   // rejection method
        scalepi = Math.PI / max_val; 
        sq = Math.Sqrt(2.0*m); 
        alm = Math.Log(m);
        g = m*alm - GammaRelated.LnGamma(m+1.0);
      }

    }
    public PoissonDistribution (double mean) : this(mean,DefaultGenerator) {}
    public PoissonDistribution (double mean, 
      RandomGenerator ran)
      : base(ran) 
    {
      Initialize(mean);
    }
    public override double val()
    {
      double em, t, y; 
    
      if (m < 12.0) 
      {         // direct method

        em = -1.0;
        t = 1.0;
        do 
        {
          em += 1.0;
          t *= gen.Long() * scale;
        } while (t > g);

      } 
      else 
      {                // rejection method

        do 
        {
          do 
          {
            y = Math.Tan( gen.Long() * scalepi );
            em = sq * y + m;
          } while (em < 0.0);
          em = Math.Floor(em);
          t = 0.9 * (1.0 +  y*y) * Math.Exp( em * alm - GammaRelated.LnGamma(em+1.0) - g );
        } while ( gen.Long() * scale > t);
      }

      return em;
    }
    public override uint Long()
    {
      return gen.Long();
    }
    double Mean { get { return m; }}


    public override double PDF(double x)
    {
      return Math.Exp(-m +x*Math.Log(m) - Calc.GammaRelated.LnGamma(x+1));
    }

    public override double CDF(double x)
    {
      return Calc.GammaRelated.GammaRegularized(1 + Math.Floor(x),m);
    }

    public override double Quantile(double x)
    {
      throw new NotSupportedException("Sorry, Quantile is not supported here since it is a discrete distribution");
    }

  }
  #endregion

  #region BinomialDistribution

  /// <summary>
  /// Generates Binomial distributed random numbers.
  /// </summary>  
  /// <remarks><code>                      
  /// Returns a binomial distributed deviate (integer returned in a double)     
  /// according to the distribution:                      
  ///                                                     
  ///              j+eps                 / n \    j      n-j         
  ///      integral       p   (m) dm  = |     |  q  (1-q)                 
  ///              j-eps   n,q           \ j /                  
  ///                             
  /// References:                     
  /// D. E. Knuth: The Art of Computer Programming, Vol. 2, Seminumerical   
  /// Algorithms, pp. 120, 2nd edition, 1981.                   
  ///                             //
  /// W. H. Press, B. P. Flannery, S. A. Teukolsky, W. T. Vetterling,       
  /// Numerical Recipies in C, Cambridge Univ. Press, 1988.         
  /// </code></remarks>
  public class BinomialDistribution : ProbabilityDistribution 
  {
    protected double scale, scalepi, p, pc, plog, pclog, np, npexp, en, en1, gamen1, sq;
    protected int n;
    protected bool sym;
    protected void Initialize (double pp, int nn)
    {
      if (pp >= 0.0 && pp <= 1.0) 
      {

        scale = 1.0 / max_val;
        scalepi = Math.PI / max_val;

        if (pp <= 0.5) 
        { // use invariance under  p  <==> 1-p 
          p = pp; sym = false;
        } 
        else 
        {
          p = 1.0 - pp; sym = true;
        }

        n = nn;
        np = n * p;
        npexp = Math.Exp(-np);

        en = n;
        en1 = en + 1.0;
        gamen1 = GammaRelated.LnGamma(en1);
        pc = 1.0-p;
        plog = Math.Log(p);
        pclog = Math.Log(pc);
        sq = Math.Sqrt(2*np*pc);

      } 
      else 
        throw new ArgumentException("BinomialDistribution: probability must be within [0,1]");
    }
    public BinomialDistribution (double prob, int num) : this(prob,num,DefaultGenerator) {}
    public BinomialDistribution (double prob, int num, 
      RandomGenerator ran)
      : base(ran)
    { 
      Initialize(prob,num);
    }

    public override double val()
    {
      double bnl;

      if (n < 25) 
      {   // direct method for moderate n

        bnl = 0.0;
        for (int j = 0; j < n; j++)
          if (scale * gen.Long() < p) bnl++;
  
      } 
      else if (np < 1.0) 
      { // use direct Poisson method

        int j;
        double t = 1.0;
        for (j = 0; j <= n; j++) 
        {
          t *= scale * gen.Long();
          if (t < npexp) break;
        }
        bnl = (j <= n ? j : n);

      } 
      else 
      {     // use rejection method

        double em, y, t;
        do 
        {
          do 
          {
            y = Math.Tan( scalepi * gen.Long() );
            em = sq * y + np;
          } while (em < 0.0 || em >= en1);
          em = Math.Floor(em);
          t = 1.2 * sq * (1.0 + y*y) * Math.Exp( gamen1 
            - GammaRelated.LnGamma(em+1.0) 
            - GammaRelated.LnGamma(en1-em) 
            + em * plog
            + (en-em) * pclog );
        } while (scale * gen.Long() > t);
        bnl = em;
      }
    
      if (sym) bnl = n - bnl; // undo symmetry transformation

      return bnl;
    }
    public override uint Long()
    {
      return gen.Long();
    }
    public double Prob { get { return p; }}
    public int Num { get { return n; }}

    public override double PDF(double x)
    {
      return Math.Pow(1 - p,n - x)*Math.Pow(p,x)*Calc.GammaRelated.Binomial(n,x);
    }


    public override double CDF(double x)
    {
      return Calc.GammaRelated.BetaRegularized(1 - p,n - Math.Floor(x),1 + Math.Floor(x));
    }

    public override double Quantile(double x)
    {
      throw new NotSupportedException("Sorry, Quantile is not supported here since it is a discrete distribution");
    }
  }

  #endregion

  #region StudentTDistribution

  /// <summary>
  /// Implements the Student t distribution.
  /// </summary>
  public class StudentTDistribution
  {
    int n;
 
    public StudentTDistribution(int N)
    {
      this.n = N;
    }
    public int N { get { return n; }}

    public double PDF(double x)
    {
      return PDF(x,n);
    }

    public double CDF(double x)
    {
      return CDF(x,n);
    }

    public double Quantile(double p)
    {
      return Quantile(p,n);
    }

    public static double PDF(double x, int n)
    {
      return Math.Pow(n/(n + (x*x)),(1 + n)/2.0)/(Math.Sqrt(n)*GammaRelated.Beta(n/2.0,0.5));
    }

    public static double CDF(double x, int n)
    {
      return (1 + (1-GammaRelated.BetaIR(n/(n + (x*x)),n*0.5,0.5))*Math.Sign(x))/2.0;
    }

    public static double Quantile(double alpha, int n)
    {
      return Math.Sqrt(n)*Math.Sqrt(-1 + 1/GammaRelated.InverseBetaRegularized(1-Math.Abs(1 - 2*alpha),n*0.5,0.5))*Math.Sign(-1 + 2*alpha);
    }
  }
  #endregion

  #region Ranmar

  //----------------------------------------------------------------------------//
  // Here start the exceptions from our nice scheme ...
  //----------------------------------------------------------------------------//

  /// <summary>
  /// Universal random number generator proposed by Marsaglia, Zaman, and Tsang.
  /// It has a period of 2^144 = 2*10^43, and is completely portable.
  /// Only 24 bits are garantueed to be completely random. 
  /// </summary>
  /// <remarks><code>
  /// Upto now this generator passes all statistical tests on randomness.
  /// Ranmar generates a sequence of random numbers uniformly distributed in the
  /// interval (0,1), the end points excluded. 
  /// The seed value must be in the range 0 &lt;= ijkl &lt;= 900 000 000.
  ///
  /// References:
  ///      
  ///  1. G. Marsaglia and A. Zaman, Toward a Universal Random Number Generator,
  ///     Florida State University FSU-SCRI-87-50 (1987).
  ///  2. F. James, A Review of Pseudorandom Number Generators, Computer Phys.
  ///     Comm. 60, 329-344 (1990).    
  /// </code></remarks>
  public class Ranmar   // James - Marsaglia - RANMAR
  {
    private double[] u = new double[97];
    private double c,cd,cm;
    private int i97, j97;
    public void Seed()
    {
      Seed(0);
    }
    public void Seed (uint ijkl)
    {
      // Initializing routine for ranmar: Called by the constructor, can be
      // also called later to reseed the generator.
      // The input value must be in the range:  
      //
      //     0 <= ijkl <= 900 000 000
      //
      // Correspondence between the simplified input seed ijkl 
      // and the original Marsaglia-Zaman paper (i=12, j=34, k=56, l=78)  
      // is found with ijkl = 54217137.
      // 
      // (If a two-seed version is used the input values must be in the ranges
      // 0 <= ij <= 31328, 0 <= kl <= 30081 and Marsaglia-Zaman is found with
      // ij=1802, kl=9373)

      // if the seed is zero then a unique seed will be determined automatically
      if (ijkl == 0) ijkl = RandomGenerator.UniqueSeed();
      ijkl &= 0x7fffffff;
      if (ijkl > 900000000) ijkl %= 900000001;

      //if ( ijkl < 0 || ijkl > 900000000 )
      //  Matpack.Error("Ranmar::Seed: 0 <= seed <= 900000000");

      int   ij, kl, i, j, k, l, m;

      ij = (int)(ijkl / 30082);
      kl = (int)(ijkl - 30082 * ij);
      i = ((ij / 177) % 177) + 2;
      j = (ij % 177) + 2;
      k = ((kl/169) % 178) + 1;
      l = kl % 169;

      for (int ii = 1; ii <= 97; ii++) 
      {
        double s = 0.0;
        double t = 0.5;
        for (int jj = 1; jj <= 24; jj++) 
        {
          m = (((i*j) % 179)*k) % 179;
          i = j;
          j = k;
          k = m;
          l = (53*l+1) % 169;
          if ( ((l*m) % 64) >= 32) s += t;
          t = 0.5 * t;
        }
        u[ii-1] = s;
      }
      c  =   362436.0 / 16777216.0;
      cd =  7654321.0 / 16777216.0;
      cm = 16777213.0 / 16777216.0;
      i97 = 97;
      j97 = 33;
    }
    public Ranmar() : this(0) {}

    public Ranmar (uint seed)
    {
      Seed(seed);
    }
    public double val()
    {
      double uni = u[i97-1] - u[j97-1];
      if (uni < 0) uni += 1.0;
      u[--i97] = uni;
      if (i97 == 0) i97 = 97;
      j97--;
      if (j97 == 0) j97 = 97;
      c -= cd;
      if (c < 0) c += cm;
      uni -= c;
      if (uni < 0) uni++;

      // to avoid that the return value is exactly zero (see F. James)
      if (uni == 0.0) 
      {
        uni = u[j97-1] / 16777216.0; // * 2^-24
        if (uni == 0.0) 
          uni = 1.0/281474976710656.0;  // * 2^-48
      }

      return uni;
    }
  }


  #endregion

  #region RanLux

  /// <summary>
  /// Subtract-and-borrow random number generator proposed by 
  /// Marsaglia and Zaman, implemented by F. James with the name 
  /// RCARRY in 1991, and later improved by Martin Luescher 
  /// in 1993 to produce "Luxury Pseudorandom Numbers". 
  /// </summary>
  /// <remarks><code>
  /// Fortran 77 coded by F. James, 1993 
  ///
  /// LUXURY LEVELS 
  /// -------------
  /// The available luxury levels are: 
  ///
  ///  level 0  (p=24): equivalent to the original RCARRY of Marsaglia 
  ///           and Zaman, very long period, but fails many tests. 
  ///  level 1  (p=48): considerable improvement in quality over level 0, 
  ///           now passes the gap test, but still fails spectral test. 
  ///  level 2  (p=97): passes all known tests, but theoretically still 
  ///           defective. 
  ///  level 3  (p=223): DEFAULT VALUE.  Any theoretically possible 
  ///           correlations have very small chance of being observed. 
  ///  level 4  (p=389): highest possible luxury, all 24 bits chaotic. 
  ///
  ///
  /// Calling sequences for RANLUX
  /// ----------------------------
  ///      CALL RANLUX (RDUM) returns a 
  ///                   32-bit random floating point numbers between
  ///                   zero (not included) and one (also not incl.).
  ///
  ///      CALL RLUXGO(LUX,INT,K1,K2) initializes the generator from
  ///               one 32-bit integer INT and sets Luxury Level LUX
  ///               which is integer between zero and MAXLEV, or if
  ///               LUX .GT. 24, it sets p=LUX directly.  K1 and K2 
  ///               should be set to zero unless restarting at a break 
  ///               point given by output of RLUXAT (see RLUXAT). 
  ///
  ///      CALL RLUXAT(LUX,INT,K1,K2) gets the values of four integers 
  ///               which can be used to restart the RANLUX generator 
  ///               at the current point by calling RLUXGO.  K1 and K2 
  ///               specify how many numbers were generated since the 
  ///               initialization with LUX and INT.  The restarting 
  ///               skips over  K1+K2*E9   numbers, so it can be long. 
  ///
  ///   A more efficient but less convenient way of restarting is by: 
  ///      CALL RLUXIN(ISVEC)    restarts the generator from vector 
  ///                   ISVEC of 25 32-bit integers (see RLUXUT) 
  ///      CALL RLUXUT(ISVEC)    outputs the current values of the 25 
  ///                 32-bit integer seeds, to be used for restarting 
  ///      ISVEC must be dimensioned 25 in the calling program
  /// </code></remarks>
  
  class RanLux    // Luescher - James - Marsaglia - Zaman - RANLUX
  {
    private int k1,k2;
    public RanLux (int luxury, uint seed)
    {
      SetState(luxury,seed);
    }
    public void SetState (int luxury, uint seed)
    {
      

      // if the seed is zero then a unique seed will be determined automatically
      if (seed == 0) seed = RandomGenerator.UniqueSeed();

      // The seed is masked with 0x7fffffff to ensure a positive integer value
      seed &= 0x7fffffff;

      k1 = k2 = 0;
      rluxgo(luxury, (int)(seed), ref k1, ref k2);
    }
    public double val()
    {
      double rdum=0;
      ranlux(ref rdum);
      return rdum;
    }
    
    #region helper functions
    
    static bool notyet = true;
    static int luxlev = 3;
    static int in24 = 0;
    static int kount = 0;
    static int mkount = 0;
    static int i24 = 24;
    static int j24 = 10;
    static int[] ndskip = { 0,24,73,199,365 };
    static double carry = 0.0;

    static int[] next = new int[24];
    static int nskip;
    static int inseed;
    static double[] seeds = new double[24];
    static double twom12, twom24;

    
    static void ranlux_driver (int what, ref double rdum, 
      int[] isdext, ref int lout, ref int inout, 
      ref int k1, ref int k2, int lux, int ins)
    {
  
      // System generated locals 
      int i__1, i__2;

      // Local variables 
      int izip;
      int inner;
      int izip2, i, k, jseed;
      int lp;
      int[] iseeds = new int[24];
      int iouter, isd, isk;
      int ilx;

      double uni;


      switch (what) 
      {
        case 1: goto L_rluxin;
        case 2: goto L_rluxut;
        case 3: goto L_rluxat;
        case 4: goto L_rluxgo;
      }

      //                                     default 
      // Luxury Level      0      1       2   *3*     4 
      // corresponds to p=24     48      97   223   389 
      // time factor       1      2       3     6    10   on slow workstation 
      //                   1      1.5     2     3     5   on fast mainframe 

      // notyet is true if no initialization has been performed yet. 
      // Default Initialization by Multiplicative Congruential 
      if (notyet) 
      {
        notyet = false;
        jseed = 314159265;
        inseed = jseed;

#if _RANLUX_DEBUG_
    cout << " RANLUX DEFAULT INITIALIZATION: " << jseed << endl;
#endif

        luxlev = 3;
        nskip = ndskip[luxlev];
        lp = nskip + 24;
        in24 = 0;
        kount = 0;
        mkount = 0;

#if _RANLUX_DEBUG_
    cout << " RANLUX DEFAULT LUXURY LEVEL =  " 
   << luxlev << " " << "      p = " << lp << endl;
#endif

        twom24 = 1.0;
        for (i = 1; i <= 24; ++i) 
        {
          twom24 *= 0.5;
          k = jseed / 53668;
          jseed = (jseed - k * 53668) * 40014 - k * 12211;
          if (jseed < 0) jseed += 2147483563;
          iseeds[i - 1] = jseed % 16777216;
        }
        twom12 = twom24 * 4096.0;
        for (i = 1; i <= 24; ++i) 
        {
          seeds[i - 1] = (double) iseeds[i - 1] * twom24;
          next[i - 1] = i - 1;
        }
        next[0] = 24;
        i24 = 24;
        j24 = 10;
        carry = 0.0;
        if (seeds[23] == 0.0) carry = twom24;
      }

      // The Generator proper: "Subtract-with-borrow", 
      // as proposed by Marsaglia and Zaman, 
      // Florida State University, March, 1989 

      uni = seeds[j24-1] - seeds[i24-1] - carry;
      if (uni < 0.0) 
      {
        uni += 1.0;
        carry = twom24;
      } 
      else 
        carry = 0.0;
  
      seeds[i24 - 1] = uni;
      i24 = next[i24 - 1];
      j24 = next[j24 - 1];
      rdum = uni;

      // small numbers (with less than 12 "significant" bits) are "padded".
      if (uni < twom12) 
      {
        rdum += twom24 * seeds[j24 - 1];
        // and zero is forbidden in case someone takes a logarithm 
        if (rdum == 0.0) rdum = twom24 * twom24;
      }
  
      // Skipping to luxury.  As proposed by Martin Luscher. 
      ++in24;
      if (in24 == 24) 
      {
        in24 = 0;
        kount += nskip;
        i__2 = nskip;
        for (isk = 1; isk <= i__2; ++isk) 
        {
          uni = seeds[j24-1] - seeds[i24-1] - carry;
          if (uni < 0.0) 
          {
            uni += 1.0;
            carry = twom24;
          } 
          else 
            carry = 0.0;
      
          seeds[i24 - 1] = uni;
          i24 = next[i24 - 1];
          j24 = next[j24 - 1];
        }
      }
  
      kount++;
      if (kount >= 1000000000) 
      {
        ++mkount;
        kount -= 1000000000;
      }
      return;


      //---------------------------------------------------------------------------//
      // Entry to input and float integer seeds from previous run 
      //---------------------------------------------------------------------------//
      L_rluxin:

        twom24 = 1.0;
      for (i = 1; i <= 24; ++i) 
      {
        next[i - 1] = i - 1;
        twom24 *= 0.5;
      }
      next[0] = 24;
      twom12 = twom24 * 4096.0;

#if _RANLUX_DEBUG_
  cout << " FULL INITIALIZATION OF RANLUX WITH 25 INTEGERS: ";
  for (int ii = 0; ii < 25; ii++) { 
    cout << setw(12) << isdext[ii];
    if (i % 5 == 0) cout << endl;
  }
#endif

      for (i = 0; i < 24; ++i) 
        seeds[i] = (double) isdext[i] * twom24;

      carry = 0.0;
      if (isdext[24] < 0) carry = twom24;
      isd = Math.Abs(isdext[24]);
      i24 = isd % 100;
      isd /= 100;
      j24 = isd % 100;
      isd /= 100;
      in24 = isd % 100;
      isd /= 100;
      luxlev = isd;
      if (luxlev <= 4) 
      {
        nskip = ndskip[luxlev];

#if _RANLUX_DEBUG_
    cout << " RANLUX LUXURY LEVEL SET BY RLUXIN TO: " << luxlev << endl;
#endif

      } 
      else if (luxlev >= 24) 
      {
        nskip = luxlev - 24;

#if _RANLUX_DEBUG_
    cout <<  " RANLUX P-VALUE SET BY RLUXIN TO: " << luxlev << endl;
#endif

      } 
      else 
      {
        nskip = ndskip[4];

#if _RANLUX_DEBUG_
    cout <<  " RANLUX ILLEGAL LUXURY RLUXIN: " << luxlev << endl;
#endif
        luxlev = 4;
      }

      inseed = -1;
      return;

      //---------------------------------------------------------------------------//
      // Entry to ouput seeds as integers 
      //---------------------------------------------------------------------------//
      L_rluxut:

        for (i = 0; i < 24; ++i) 
          isdext[i] = (int) (seeds[i] * 4096.0 * 4096.0);
      isdext[24] = i24 + j24 * 100 + in24 * 10000 + luxlev * 1000000;
      if (carry > 0.0) isdext[24] = -isdext[24];
      return;

      //---------------------------------------------------------------------------//
      // Entry to output the "convenient" restart point 
      //---------------------------------------------------------------------------//
      L_rluxat:

        lout = luxlev;
      inout = inseed;
      k1 = kount;
      k2 = mkount;
      return;

      //---------------------------------------------------------------------------//
      // Entry to initialize from one or three integers 
      //---------------------------------------------------------------------------//
      L_rluxgo:

        if (lux < 0)
          luxlev = 3;
        else if (lux <= 4)
          luxlev = lux;
        else if (lux < 24 || lux > 2000) 
        {
          luxlev = 4;

#if _RANLUX_DEBUG_
    cout <<  " RANLUX ILLEGAL LUXURY RLUXGO: " << lux << endl;
#endif

        } 
        else 
        {
          luxlev = lux;
          for (ilx = 0; ilx <= 4; ++ilx) 
          {
            if (lux == ndskip[ilx] + 24) 
            {
              luxlev = ilx;
            }
          }
        }

      if (luxlev <= 4) 
      {
        nskip = ndskip[luxlev];
        i__1 = nskip + 24;

#if _RANLUX_DEBUG_
    cout << " RANLUX LUXURY LEVEL SET BY RLUXGO : " 
   << luxlev << "     P = " << i__1 << endl;
#endif

      } 
      else 
      {
        nskip = luxlev - 24;
#if _RANLUX_DEBUG_
    cout << " RANLUX P-VALUE SET BY RLUXGO TO : " << luxlev << endl;
#endif
      }

      in24 = 0;
      if (ins < 0) 
      {

#if _RANLUX_DEBUG_
    cout << " Illegal initialization by RLUXGO, negative input seed" << endl;
#endif

      }

      if (ins > 0) 
      {
        jseed = ins;

#if _RANLUX_DEBUG_
    cout << " RANLUX INITIALIZED BY RLUXGO FROM SEEDS "
   << jseed << " " << *k1 << " " << *k2 << endl;
#endif

      } 
      else 
      {
        jseed = 314159265;

#if _RANLUX_DEBUG_
    cout << " RANLUX INITIALIZED BY RLUXGO FROM DEFAULT SEED" << endl;
#endif

      }

      inseed = jseed;
      notyet = false;
      twom24 = 1.0;
      for (i = 1; i <= 24; ++i) 
      {
        twom24 *= 0.5;
        k = jseed / 53668;
        jseed = (jseed - k * 53668) * 40014 - k * 12211;
        if (jseed < 0) jseed += 2147483563;
        iseeds[i - 1] = jseed % 16777216;
      }
      twom12 = twom24 * 4096.0;
      for (i = 1; i <= 24; ++i) 
      {
        seeds[i - 1] = (double) iseeds[i - 1] * twom24;
        next[i - 1] = i - 1;
      }
      next[0] = 24;
      i24 = 24;
      j24 = 10;
      carry = 0.0;
      if (seeds[23] == 0.0) carry = twom24;

      // If restarting at a break point, skip K1 + IGIGA*K2 
      // Note that this is the number of numbers delivered to 
      // the user PLUS the number skipped (if luxury .GT. 0). 
      kount = k1;
      mkount = k2;
      if (k1 + k2 != 0) 
      {
        i__1 = k2 + 1;
        for (iouter = 1; iouter <= i__1; ++iouter) 
        {
          inner = 1000000000;
          if (iouter == k2 + 1) inner = k1;
          i__2 = inner;
          for (isk = 1; isk <= i__2; ++isk) 
          {
            uni = seeds[j24 - 1] - seeds[i24 - 1] - carry;
            if (uni < 0.0) 
            {
              uni += 1.0;
              carry = twom24;
            } 
            else 
              carry = 0.0;

            seeds[i24 - 1] = uni;
            i24 = next[i24 - 1];
            j24 = next[j24 - 1];
          }
        }

        // Get the right value of IN24 by direct calculation 
        in24 = kount % (nskip + 24);
        if (mkount > 0) 
        {
          izip = 1000000000 % (nskip + 24);
          izip2 = mkount * izip + in24;
          in24 = izip2 % (nskip + 24);
        }

        // Now IN24 had better be between zero and 23 inclusive 
        if (in24 > 23) 
        {

#if _RANLUX_DEBUG_
      cout << "  Error in RESTARTING with RLUXGO: " << endl
     << "  The values " << ins << " " << *k1 << " " << *k2
     << " cannot occur at luxury level " << luxlev << endl;
#endif

          in24 = 0;
        }
      }
    }

    //-----------------------------------------------------------------------------//

    static void ranlux (ref double rdum)
    { 
      int b=0, c=0, d=0, e=0;

      ranlux_driver(0, ref rdum, null, ref b, ref c, 
        ref d, ref e, 0, 0);
    }

    //-----------------------------------------------------------------------------//

    static void rluxin (int[] isdext)
    {
      double a=0;
      int b=0, c=0, d=0, e=0;
      ranlux_driver(1, ref a, isdext, ref b, 
        ref c, ref d, ref e, 0 ,0);
    }

    //-----------------------------------------------------------------------------//

    static void rluxut (int []isdext)
    {
      double a=0;
      int b=0, c=0, d=0, e=0;
      ranlux_driver(2, ref a, isdext, ref b, ref c, 
        ref d, ref e, 0,0);
    }

    //-----------------------------------------------------------------------------//

    static void rluxat(ref int lout, ref int inout, ref int k1, ref int k2)
    {
      double a=0;     
      ranlux_driver(3, ref a, null, ref lout, 
        ref inout, ref k1, ref k2, 0,0);
    }

    //-----------------------------------------------------------------------------//

    static void rluxgo(int lux, int ins, ref int k1, ref int k2)
    {
      double a=0;
      int c=0,d=0;

      ranlux_driver(4, ref a, null, ref c, ref d, 
        ref k1, ref k2, lux,ins);
    }


    #endregion

  }

  #endregion

  #region Ran32k3a

  /// <summary>
  /// This is Pierre L'Ecuyer's pseudorandum number generator MRG32k3a.
  /// The period length is about 2^191 (which is approximately 3*10^57).
  /// </summary>
  /// <remarks><code>
  /// The implementation uses floating-point arithmetic and works under the 
  /// sufficient condition that the double precision floating-point arithmetic 
  /// satisfies the IEEE 754 standard. That means all integers between -2^53
  /// and 2^53 are represented exactly in a number of type double. Note, that
  /// this generator gives no more than 32 random bits although a 53-bit floating
  /// point number is returned.
  ///
  /// Initialization:
  ///    The six global variables s10,s11,s12, s20,s21,s22 constitute the seed.
  ///    Before called the first time one must initialize s10,s11,s12 to (exact)
  ///    non-negative integer values less than m1 (i.e. in [0...4294967086]), but
  ///    not all zero, and s20,s21,s22 to (exact) non-negative integer values 
  ///    less than m2  (i.e. in [0...4294944442]), but not all zero.
  ///
  /// Reference:
  ///    Pierre L'Ecuyer, 
  ///    Good Parameter Sets for Combined Multiple Recursive Random
  ///    Number Generators, Preprint, July 28, 1997, Code of Figure I.
  ///
  /// Note:
  ///   1. The time being this is presumably one of the best pseudorandom number
  ///      generators and it is also very fast. (Sep 26, 1997, B.M.Gammel) 
  ///   2. CPU time to generate and add 10^7 numbers:
  ///      6 sec for DEC Alpha Station, 21161 CPU, 400 MHz, GNU g++ 2.7.1 compiler
  ///   3. The time overhead for virtual vs. non-virtual operator () is 
  ///      about 1-2 percent and is therefore considered as neglegible.
  /// </code></remarks>
  class Ran32k3a    // L'Ecuyer's 1997 MRG
  {
    private double s10,s11,s12,s20,s21,s22;

    //----------------------------------------------------------------------------//
    // Seeding
    //----------------------------------------------------------------------------//
    public void SetState (uint u10, uint u11, uint u12, 
      uint u20, uint u21, uint u22)
    {
      // make some consistency checks on the seeds
      const uint u1 = 4294967086, u2 = 4294944442;

      if ( u10 > u1 || u11 > u1 || u12 > u1 ||
        u20 > u2 || u21 > u2 || u22 > u2 ||
        (u10 == 0 && u11 == 0 && u12 == 0) ||
        (u20 == 0 && u21 == 0 && u22 == 0) )
        throw new ArgumentException("Illegal seed values given");
  
      // set the new state
      s10 = u10; s11 = u11; s12 = u12; 
      s20 = u20; s21 = u21; s22 = u22;
    }

    
    //----------------------------------------------------------------------------//
    // Constructor
    //----------------------------------------------------------------------------//
    Ran32k3a (uint u10, uint u11, uint u12, uint u20, uint u21, uint u22)
    {
      const double test1 = 9007199254740991;
      const long   test2 = 9007199254740991;
      // assure that we have at least 53 bit precission floating point arithmetic
      System.Diagnostics.Debug.Assert(test1==test2);
    
      // now set the new state
      SetState(u10,u11,u12,u20,u21,u22);
    }

    
    //----------------------------------------------------------------------------//
    // The generator
    //----------------------------------------------------------------------------//
    public double val()
    {
      // Implementation constants
      const double norm = 2.328306549295728e-10,
              m1   = 4294967087.0,
              m2   = 4294944443.0;

      int k;
      double p;

      // Component 1
      p = 1403580.0 * s11 - 810728.0 * s10;
      k = (int)(p / m1);
      p -= k * m1;
      if (p < 0.0) p += m1;
      s10 = s11; s11 = s12; s12 = p;

      // Component 2
      p = 527612.0 * s22 - 1370589.0 * s20;
      k = (int)(p / m2);
      p -= k * m2;
      if (p < 0.0) p += m2;
      s20 = s21; s21 = s22; s22 = p;

      // Combination
      return (s12 <= s22) ? ((s12 - s22 + m1) * norm) : ((s12 - s22) * norm);
    }

  }
  #endregion

  #region UnitSphereDistribution

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
  
  public class UnitSphereDistribution : ProbabilityDistribution 
  {
    protected double scale;
    public  UnitSphereDistribution() : this(DefaultGenerator) {}
    public UnitSphereDistribution(RandomGenerator ran) 
      : base(ran) 
    {
      scale = 2.0 / Maximum;
    }
    
    public override double val()
    {
      throw new NotSupportedException("Use val(out double x, out double y, out double z) instead of this method");
    }

    public void val( out double x, out double y, out double z)
    { 
      for (;;) 
      {
        double d1 = 1.0 - scale * Long(),
          d2 = 1.0 - scale * Long(),
          dd = d1 * d1 + d2 * d2;
        if (dd < 1.0) 
        { 
          z = 1 - 2 * dd;
          dd = 2 * Math.Sqrt(1.0-dd);
          x = d1 * dd;
          y = d2 * dd;
          return;
        }
      }
    }
    public override uint Long() 
    {
      return gen.Long();
    }

    public override double PDF(double x)
    {
      throw new NotSupportedException();
    }

    public override double CDF(double x)
    {
      throw new NotSupportedException();
    }

    public override double Quantile(double x)
    {
      throw new NotSupportedException();
    }



  }

  #endregion

}

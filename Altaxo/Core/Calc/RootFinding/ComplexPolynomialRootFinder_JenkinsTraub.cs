// Copyright Dr. Dirk Lellinger 2013. Adapted from Kenneth Haugland to the reqirements of Altaxo.
// Author: Translated to C# by Kenneth Haugland (Code project open license). See http://www.codeproject.com/Articles/552678/Polynomial-Equation-Solver
// from C++ code that was translated by Laurent Bartholdi for Real coefficients from the original Netlib site in FORTRAN
// and from C code written by Henrik Vestermark for complex coefficients translated from the original Netlib site in FORTRAN

using System;
using System.Collections.Generic;

namespace Altaxo.Calc.RootFinding
{
  /// <summary>
  /// Implements the Jenkins-Traub algorithm for polynoms with real coefficients.
  /// </summary>
  public class ComplexPolynomialRootFinder_JenkinsTraub
  {
    private double sr;
    private double si;
    private double tr;
    private double ti;
    private double pvr;
    private double pvi;
    private double are;
    private double mre;
    private double eta;
    private double infin;

    private int nn;

    //Global variables that assist the computation, taken from the Visual Studio C++ compiler class float
    // smallest such that 1.0+DBL_EPSILON != 1.0
    private const double DBL_EPSILON = 2.22044604925031E-16;

    // max value
    private const double DBL_MAX = 1.79769313486232E+307;

    // min positive value
    private const double DBL_MIN = 2.2250738585072E-308;

    // exponent radix
    private const double DBL_RADIX = 2;

    // Allocate arrays
    private double[] pr;

    private double[] pi;
    private double[] hr;
    private double[] hi;
    private double[] qpr;
    private double[] qpi;
    private double[] qhr;
    private double[] qhi;
    private double[] shr;
    private double[] shi;

    /// <summary>
    /// The Jenkins–Traub algorithm for finding the roots of a polynomial.
    /// </summary>
    /// <param name="Input">The coefficients for the polynomial starting with the constant (zero degree) and ends with the highest degree. Missing coefficients must be provided as zeros.</param>
    /// <returns>All the real and complex roots that are found are returned in a list of complex numbers. The list is not neccessarily sorted.</returns>
    public static List<Complex> FindRoots(params Complex[] Input)
    {
      var r = new ComplexPolynomialRootFinder_JenkinsTraub();
      return r.Execute(Input);
    }

    /// <summary>
    /// The Jenkins–Traub algorithm for finding the roots of a polynomial.
    /// </summary>
    /// <param name="Input">The coefficients for the polynomial starting with the constant (zero degree) and ends with the highest degree. Missing coefficients must be provided as zeros.</param>
    /// <returns>All the real and complex roots that are found are returned in a list of complex numbers. The list is not neccessarily sorted.</returns>
    public List<Complex> Execute(params Complex[] Input)
    {
      if (null == Input)
        throw new ArgumentNullException("Input");

      //Actual degree calculated from the items in the Input ParamArray
      int Degree;
      for (Degree = Input.Length - 1; Degree >= 0 && Input[Degree] == 0; --Degree)
        ;

      if (Degree <= 0)
        throw new ArgumentException("Provided polynomial has a degree of zero. Root finding is therefore not possible");

      List<Complex> result = new List<Complex>();

      const double cosr = -0.06975647374412530077596; // Math.Cos(-94 * Math.PI / 180);
      const double sinr = -0.9975640502598242476132; // Math.Sin(-94 * Math.PI / 180);

      int idnn2 = 0;
      int conv = 0;
      double xx = 0;
      double yy = 0;

      double smalno = 0;
      double @base = 0;
      double xxx = 0;
      double zr = 0;
      double zi = 0;
      double bnd = 0;

      pr = new double[Degree + 1];
      pi = new double[Degree + 1];
      hr = new double[Degree + 1];
      hi = new double[Degree + 1];
      qpr = new double[Degree + 1];
      qpi = new double[Degree + 1];
      qhr = new double[Degree + 1];
      qhi = new double[Degree + 1];
      shr = new double[Degree + 1];
      shi = new double[Degree + 1];

      double[] opr = new double[Degree + 1];
      double[] opi = new double[Degree + 1];
      double[] zeror = new double[Degree + 1];
      double[] zeroi = new double[Degree + 1];

      for (int i = 0, k = Degree; i <= Degree; ++i, --k)
      {
        var coeffR = Input[k].Re;
        var coeffI = Input[k].Im;

        if ((!(coeffR >= double.MinValue && coeffR <= double.MaxValue)) ||
            (!(coeffI >= double.MinValue && coeffI <= double.MaxValue)))
          throw new ArgumentOutOfRangeException(string.Format("Input[{0}] is {1}. This value is not acceptable. Exiting root finding algorithm.", k, Input[k]));

        opr[i] = coeffR;
        opi[i] = coeffI;
      }

      mcon(ref eta, ref infin, ref smalno, ref @base);
      are = eta;
      mre = 2.0 * Math.Sqrt(2.0) * eta;
      xx = Math.Sqrt(0.5);
      yy = -xx;
      nn = Degree;

      // Remove the zeros at the origin if any
      while ((opr[nn] == 0 & opi[nn] == 0))
      {
        idnn2 = Degree - nn;
        zeror[idnn2] = 0;
        zeroi[idnn2] = 0;
        nn -= 1;
      }

      // Make a copy of the coefficients
      for (int i = 0; i <= nn; i++)
      {
        pr[i] = opr[i];
        pi[i] = opi[i];
        shr[i] = cmod(pr[i], pi[i]);
      }

      // Scale the polynomial
      bnd = scale(nn, shr, eta, infin, smalno, @base);
      if ((bnd != 1))
      {
        for (int i = 0; i <= nn; i++)
        {
          pr[i] *= bnd;
          pi[i] *= bnd;
        }
      }
search:

      if ((nn <= 1))
      {
        cdivid(-pr[1], -pi[1], pr[0], pi[0], ref zeror[Degree - 1], ref zeroi[Degree - 1]);

        for (int i = 0; i <= Degree - 1; i++)
        {
          result.Add(new Complex(zeror[i], zeroi[i]));
        }
        return result;
      }

      // Calculate bnd, alower bound on the modulus of the zeros
      for (int i = 0; i <= nn; i++)
      {
        shr[i] = cmod(pr[i], pi[i]);
      }

      cauchy(nn, shr, shi, ref bnd);

      // Outer loop to control 2 Major passes with different sequences of shifts
      for (int cnt1 = 1; cnt1 <= 2; cnt1++)
      {
        // First stage  calculation , no shift
        noshft(5);

        // Inner loop to select a shift
        for (int cnt2 = 1; cnt2 <= 9; cnt2++)
        {
          // Shift is chosen with modulus bnd and amplitude rotated by 94 degree from the previous shif
          xxx = cosr * xx - sinr * yy;
          yy = sinr * xx + cosr * yy;
          xx = xxx;
          sr = bnd * xx;
          si = bnd * yy;

          // Second stage calculation, fixed shift
          fxshft(10 * cnt2, ref zr, ref zi, ref conv);
          if ((conv == 1))
          {
            // The second stage jumps directly to the third stage ieration
            // If successful the zero is stored and the polynomial deflated
            idnn2 = Degree - nn;
            zeror[idnn2] = zr;
            zeroi[idnn2] = zi;
            nn -= 1;
            for (int i = 0; i <= nn; i++)
            {
              pr[i] = qpr[i];
              pi[i] = qpi[i];
            }

            goto search;
          }
          // If the iteration is unsuccessful another shift is chosen
        }
        // if 9 shifts fail, the outer loop is repeated with another sequence of shifts
      }

      // The zerofinder has failed on two major passes
      // return empty handed with the number of roots found (less than the original degree)
      Degree -= nn;

      for (int i = 0; i <= Degree - 1; i++)
      {
        result.Add(new Complex(zeror[i], zeroi[i]));
      }

      return result;
      throw new Exception("The program could not converge to find all the zeroes, but a prelimenary result with the ones that are found is returned.");
    }

    // COMPUTES  THE DERIVATIVE  POLYNOMIAL AS THE INITIAL H
    // POLYNOMIAL AND COMPUTES L1 NO-SHIFT H POLYNOMIALS.
    //
    private void noshft(int l1)
    {
      int j = 0;
      int n = 0;
      int nm1 = 0;
      double xni = 0;
      double t1 = 0;
      double t2 = 0;

      n = nn;
      nm1 = n - 1;
      for (int i = 0; i <= n; i++)
      {
        xni = nn - i;
        hr[i] = xni * pr[i] / n;
        hi[i] = xni * pi[i] / n;
      }
      for (int jj = 1; jj <= l1; jj++)
      {
        if ((cmod(hr[n - 1], hi[n - 1]) > eta * 10 * cmod(pr[n - 1], pi[n - 1])))
        {
          cdivid(-pr[nn], -pi[nn], hr[n - 1], hi[n - 1], ref tr, ref ti);
          for (int i = 0; i <= nm1 - 1; i++)
          {
            j = nn - i - 1;
            t1 = hr[j - 1];
            t2 = hi[j - 1];
            hr[j] = tr * t1 - ti * t2 + pr[j];
            hi[j] = tr * t2 + ti * t1 + pi[j];
          }
          hr[0] = pr[0];
          hi[0] = pi[0];
        }
        else
        {
          // If the constant term is essentially zero, shift H coefficients
          for (int i = 0; i <= nm1 - 1; i++)
          {
            j = nn - i - 1;
            hr[j] = hr[j - 1];
            hi[j] = hi[j - 1];
          }
          hr[0] = 0;
          hi[0] = 0;
        }
      }
    }

    // COMPUTES L2 FIXED-SHIFT H POLYNOMIALS AND TESTS FOR CONVERGENCE.
    // INITIATES A VARIABLE-SHIFT ITERATION AND RETURNS WITH THE
    // APPROXIMATE ZERO IF SUCCESSFUL.
    // L2 - LIMIT OF FIXED SHIFT STEPS
    // ZR,ZI - APPROXIMATE ZERO IF CONV IS .TRUE.
    // CONV  - LOGICAL INDICATING CONVERGENCE OF STAGE 3 ITERATION
    //
    private void fxshft(int l2, ref double zr, ref double zi, ref int conv)
    {
      int n = 0;
      int test = 0;
      int pasd = 0;
      int bol = 0;
      double otr = 0;
      double oti = 0;
      double svsr = 0;
      double svsi = 0;

      n = nn;
      polyev(nn, sr, si, pr, pi, qpr, qpi, ref pvr, ref pvi);
      test = 1;
      pasd = 0;

      // Calculate first T = -P(S)/H(S)
      calct(ref bol);

      // Main loop for second stage
      for (int j = 1; j <= l2; j++)
      {
        otr = tr;
        oti = ti;

        // Compute the next H Polynomial and new t
        nexth(bol);
        calct(ref bol);
        zr = sr + tr;
        zi = si + ti;

        // Test for convergence unless stage 3 has failed once or this
        // is the last H Polynomial
        if ((!(bol == 1 | !(test == 1) | j == 12)))
        {
          if ((cmod(tr - otr, ti - oti) < 0.5 * cmod(zr, zi)))
          {
            if ((pasd == 1))
            {
              // The weak convergence test has been passwed twice, start the third stage
              // Iteration, after saving the current H polynomial and shift
              for (int i = 0; i <= n - 1; i++)
              {
                shr[i] = hr[i];
                shi[i] = hi[i];
              }
              svsr = sr;
              svsi = si;
              vrshft(10, ref zr, ref zi, ref conv);
              if ((conv == 1))
                return;

              //The iteration failed to converge. Turn off testing and restore h,s,pv and T
              test = 0;
              for (int i = 0; i <= n - 1; i++)
              {
                hr[i] = shr[i];
                hi[i] = shi[i];
              }
              sr = svsr;
              si = svsi;
              polyev(nn, sr, si, pr, pi, qpr, qpi, ref pvr, ref pvi);
              calct(ref bol);
              continue;
            }
            pasd = 1;
          }
        }
        else
        {
          pasd = 0;
        }
      }

      // Attempt an iteration with final H polynomial from second stage
      vrshft(10, ref zr, ref zi, ref conv);
    }

    // CARRIES OUT THE THIRD STAGE ITERATION.
    // L3 - LIMIT OF STEPS IN STAGE 3.
    // ZR,ZI   - ON ENTRY CONTAINS THE INITIAL ITERATE, IF THE
    //           ITERATION CONVERGES IT CONTAINS THE FINAL ITERATE ON EXIT.
    // CONV    -  .TRUE. IF ITERATION CONVERGES
    //
    private void vrshft(int l3, ref double zr, ref double zi, ref int conv)
    {
      int b = 0;
      int bol = 0;
      // Int(i, j)

      double mp = 0;
      double ms = 0;
      double omp = 0;
      double relstp = 0;
      double r1 = 0;
      double r2 = 0;
      double tp = 0;

      conv = 0;
      b = 0;
      sr = zr;
      si = zi;

      // Main loop for stage three

      for (int i = 1; i <= l3; i++)
      {
        // Evaluate P at S and test for convergence
        polyev(nn, sr, si, pr, pi, qpr, qpi, ref pvr, ref pvi);
        mp = cmod(pvr, pvi);
        ms = cmod(sr, si);
        if ((mp <= 20 * errev(nn, qpr, qpi, ms, mp, are, mre)))
        {
          // Polynomial value is smaller in value than a bound onthe error
          // in evaluationg P, terminate the ietartion
          conv = 1;
          zr = sr;
          zi = si;
          return;
        }
        if ((i != 1))
        {
          if ((!(b == 1 | mp < omp | relstp >= 0.05)))
          {
            // Iteration has stalled. Probably a cluster of zeros. Do 5 fixed
            // shift steps into the cluster to force one zero to dominate
            tp = relstp;
            b = 1;
            if ((relstp < eta))
              tp = eta;
            r1 = Math.Sqrt(tp);
            r2 = sr * (1 + r1) - si * r1;
            si = sr * r1 + si * (1 + r1);
            sr = r2;
            polyev(nn, sr, si, pr, pi, qpr, qpi, ref pvr, ref pvi);
            for (int j = 1; j <= 5; j++)
            {
              calct(ref bol);
              nexth(bol);
            }

            omp = infin;
            goto _20;
          }

          // Exit if polynomial value increase significantly
          if ((mp * 0.1 > omp))
            return;
        }

        omp = mp;
_20:

// Calculate next iterate
        calct(ref bol);
        nexth(bol);
        calct(ref bol);
        if ((!(bol == 1)))
        {
          relstp = cmod(tr, ti) / cmod(sr, si);
          sr += tr;
          si += ti;
        }
      }
    }

    // COMPUTES  T = -P(S)/H(S).
    // BOOL   - LOGICAL, SET TRUE IF H(S) IS ESSENTIALLY ZERO.
    private void calct(ref int bol)
    {
      // Int(n)
      int n = 0;
      double hvr = 0;
      double hvi = 0;

      n = nn;

      // evaluate h(s)
      polyev(n - 1, sr, si, hr, hi, qhr, qhi, ref hvr, ref hvi);

      if (cmod(hvr, hvi) <= are * 10 * cmod(hr[n - 1], hi[n - 1]))
      {
        bol = 1;
      }
      else
      {
        bol = 0;
      }

      if ((!(bol == 1)))
      {
        cdivid(-pvr, -pvi, hvr, hvi, ref tr, ref ti);
        return;
      }

      tr = 0;
      ti = 0;
    }

    // CALCULATES THE NEXT SHIFTED H POLYNOMIAL.
    // BOOL   -  LOGICAL, IF .TRUE. H(S) IS ESSENTIALLY ZERO
    //

    private void nexth(int bol)
    {
      int n = 0;
      double t1 = 0;
      double t2 = 0;

      n = nn;
      if ((!(bol == 1)))
      {
        for (int j = 1; j <= n - 1; j++)
        {
          t1 = qhr[j - 1];
          t2 = qhi[j - 1];
          hr[j] = tr * t1 - ti * t2 + qpr[j];
          hi[j] = tr * t2 + ti * t1 + qpi[j];
        }
        hr[0] = qpr[0];
        hi[0] = qpi[0];
        return;
      }

      // If h(s) is zero replace H with qh

      for (int j = 1; j <= n - 1; j++)
      {
        hr[j] = qhr[j - 1];
        hi[j] = qhi[j - 1];
      }
      hr[0] = 0;
      hi[0] = 0;
    }

    // EVALUATES A POLYNOMIAL  P  AT  S  BY THE HORNER RECURRENCE
    // PLACING THE PARTIAL SUMS IN Q AND THE COMPUTED VALUE IN PV.
    //
    private static void polyev(int nn, double sr, double si, double[] pr, double[] pi, double[] qr, double[] qi, ref double pvr, ref double pvi)
    {
      //{
      //     Int(i)
      double t = 0;

      qr[0] = pr[0];
      qi[0] = pi[0];
      pvr = qr[0];
      pvi = qi[0];

      for (int i = 1; i <= nn; i++)
      {
        t = (pvr) * sr - (pvi) * si + pr[i];
        pvi = (pvr) * si + (pvi) * sr + pi[i];
        pvr = t;
        qr[i] = pvr;
        qi[i] = pvi;
      }
    }

    // BOUNDS THE ERROR IN EVALUATING THE POLYNOMIAL BY THE HORNER RECURRENCE.
    // QR,QI - THE PARTIAL SUMS
    // MS    -MODULUS OF THE POINT
    // MP    -MODULUS OF POLYNOMIAL VALUE
    // ARE, MRE -ERROR BOUNDS ON COMPLEX ADDITION AND MULTIPLICATION
    //
    private static double errev(int nn, double[] qr, double[] qi, double ms, double mp, double are, double mre)
    {
      //{
      //     Int(i)
      double e = 0;

      e = cmod(qr[0], qi[0]) * mre / (are + mre);
      for (int i = 0; i <= nn; i++)
      {
        e = e * ms + cmod(qr[i], qi[i]);
      }

      return e * (are + mre) - mp * mre;
    }

    // CAUCHY COMPUTES A LOWER BOUND ON THE MODULI OF THE ZEROS OF A
    // POLYNOMIAL - PT IS THE MODULUS OF THE COEFFICIENTS.
    //
    private static void cauchy(int nn, double[] pt, double[] q, ref double fn_val)
    {
      int n = 0;
      double x = 0;
      double xm = 0;
      double f = 0;
      double dx = 0;
      double df = 0;

      pt[nn] = -pt[nn];

      // Compute upper estimate bound
      n = nn;
      x = Math.Exp(Math.Log(-pt[nn]) - Math.Log(pt[0])) / n;
      if ((pt[n - 1] != 0))
      {
        //// Newton step at the origin is better, use it
        xm = -pt[nn] / pt[n - 1];
        if ((xm < x))
          x = xm;
      }

      // Chop the interval (0,x) until f < 0

      while ((true))
      {
        xm = x * 0.1;
        f = pt[0];
        for (int i = 1; i <= nn; i++)
        {
          f = f * xm + pt[i];
        }
        if ((f <= 0))
          break; // TODO: might not be correct. Was : Exit While
        x = xm;
      }
      dx = x;

      // Do Newton iteration until x converges to two decimal places
      while ((Math.Abs(dx / x) > 0.005))
      {
        q[0] = pt[0];
        for (int i = 1; i <= nn; i++)
        {
          q[i] = q[i - 1] * x + pt[i];
        }
        f = q[nn];
        df = q[0];
        for (int i = 1; i <= n - 1; i++)
        {
          df = df * x + q[i];
        }
        dx = f / df;
        x -= dx;
      }

      fn_val = x;
    }

    // RETURNS A SCALE FACTOR TO MULTIPLY THE COEFFICIENTS OF THE POLYNOMIAL.
    // THE SCALING IS DONE TO AVOID OVERFLOW AND TO AVOID UNDETECTED UNDERFLOW
    // INTERFERING WITH THE CONVERGENCE CRITERION.  THE FACTOR IS A POWER OF THE
    // BASE.
    // PT - MODULUS OF COEFFICIENTS OF P
    // ETA, INFIN, SMALNO, BASE - CONSTANTS DESCRIBING THE FLOATING POINT ARITHMETIC.
    //
    private static double scale(int nn, double[] pt, double eta, double infin, double smalno, double @base)
    {
      //{
      //     Int(i, l)
      int l = 0;
      double hi = 0;
      double lo = 0;
      double max = 0;
      double min = 0;
      double x = 0;
      double sc = 0;
      double fn_val = 0;

      // Find largest and smallest moduli of coefficients
      hi = Math.Sqrt(infin);
      lo = smalno / eta;
      max = 0;
      min = infin;

      for (int i = 0; i <= nn; i++)
      {
        x = pt[i];
        if ((x > max))
          max = x;
        if ((x != 0 & x < min))
          min = x;
      }

      // Scale only if there are very large or very small components
      fn_val = 1;
      if ((min >= lo & max <= hi))
        return fn_val;
      x = lo / min;
      if ((x <= 1))
      {
        sc = 1 / (Math.Sqrt(max) * Math.Sqrt(min));
      }
      else
      {
        sc = x;
        if ((infin / sc > max))
          sc = 1;
      }
      l = Convert.ToInt32(Math.Log(sc) / Math.Log(@base) + 0.5);
      fn_val = Math.Pow(@base, l);
      return fn_val;
    }

    // COMPLEX DIVISION C = A/B, AVOIDING OVERFLOW.
    //
    private static void cdivid(double ar, double ai, double br, double bi, ref double cr, ref double ci)
    {
      double r = 0;
      double d = 0;
      double t = 0;
      double infin = 0;

      if ((br == 0 & bi == 0))
      {
        // Division by zero, c = infinity
        mcon(ref t, ref infin, ref t, ref t);
        cr = infin;
        ci = infin;
        return;
      }

      if ((Math.Abs(br) < Math.Abs(bi)))
      {
        r = br / bi;
        d = bi + r * br;
        cr = (ar * r + ai) / d;
        ci = (ai * r - ar) / d;
        return;
      }

      r = bi / br;
      d = br + r * bi;
      cr = (ar + ai * r) / d;
      ci = (ai - ar * r) / d;
    }

    // MODULUS OF A COMPLEX NUMBER AVOIDING OVERFLOW.
    //
    private static double cmod(double r, double i)
    {
      double ar = 0;
      double ai = 0;

      ar = Math.Abs(r);
      ai = Math.Abs(i);
      if ((ar < ai))
      {
        return ai * Math.Sqrt(1.0 + Math.Pow((ar / ai), 2.0));
      }
      else if ((ar > ai))
      {
        return ar * Math.Sqrt(1.0 + Math.Pow((ai / ar), 2.0));
      }
      else
      {
        return ar * Math.Sqrt(2.0);
      }
    }

    // MCON PROVIDES MACHINE CONSTANTS USED IN VARIOUS PARTS OF THE PROGRAM.
    // THE USER MAY EITHER SET THEM DIRECTLY OR USE THE STATEMENTS BELOW TO
    // COMPUTE THEM. THE MEANING OF THE FOUR CONSTANTS ARE -
    // ETA       THE MAXIMUM RELATIVE REPRESENTATION ERROR WHICH CAN BE DESCRIBED
    //           AS THE SMALLEST POSITIVE FLOATING-POINT NUMBER SUCH THAT
    //           1.0_dp + ETA > 1.0.
    // INFINY    THE LARGEST FLOATING-POINT NUMBER
    // SMALNO    THE SMALLEST POSITIVE FLOATING-POINT NUMBER
    // BASE      THE BASE OF THE FLOATING-POINT NUMBER SYSTEM USED
    //

    private static void mcon(ref double eta, ref double infiny, ref double smalno, ref double @base)
    {
      @base = DBL_RADIX;
      eta = DBL_EPSILON;
      infiny = DBL_MAX;
      smalno = DBL_MIN;
    }
  }
}

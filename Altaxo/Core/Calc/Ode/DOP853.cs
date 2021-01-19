#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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

#endregion Copyright

using System;
using System.Diagnostics.CodeAnalysis;

namespace Altaxo.Calc.Ode
{

  /// <summary>
  /// Runge-Kutta method of 8th order of Dormand and Prince. Supports step size control, stiffness detection and dense output.
  /// If stiffness detection and dense output is not needed, it is recommended to use <see cref="RK8713M"/>,
  /// which gives slighly better accuracy.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>[1] Hairer, Ordinary differential equations I, 2nd edition, 1993.</para>
  /// <para>[2] Söderlind et al., Adaptive Time-Stepping and Computational Stability, 2003</para>
  /// <remarks>
  /// For the coefficients see https://github.com/jacobwilliams/dop853/blob/master/src/dop853_constants.f90
  /// Implementation in C: https://github.com/robclewley/pydstool/blob/master/PyDSTool/integrator/dop853.c
  /// </remarks>
  public class DOP853 : RungeKuttaExplicitBase
  {
    #region Coefficients

    // Step partitions
    private const double c1  = 0.526001519587677318785587544488E-01;
    private const double c2  = 0.789002279381515978178381316732E-01;
    private const double c3  = 0.118350341907227396726757197510E+00;
    private const double c4  = 0.281649658092772603273242802490E+00;
    private const double c5  = 0.333333333333333333333333333333E+00;
    private const double c6  = 0.25E+00;
    private const double c7  = 0.307692307692307692307692307692E+00;
    private const double c8  = 0.651282051282051282051282051282E+00;
    private const double c9 = 0.6E+00;
    private const double c10 = 0.857142857142857142857142857142E+00;
    private const double c11 = 1;
    private const double c12 = 0.1E+00;
    private const double c13 = 0.2E+00;
    private const double c14 = 0.777777777777777777777777777778E+00;

    // high order coefficients
    private const double b0 =   5.42937341165687622380535766363E-2;
    private const double b5 =   4.45031289275240888144113950566E0;
    private const double b6 =   1.89151789931450038304281599044E0;
    private const double b7 =  -5.8012039600105847814672114227E0;
    private const double b8 =   3.1116436695781989440891606237E-1;
    private const double b9 = -1.52160949662516078556178806805E-1;
    private const double b10 =  2.01365400804030348374776537501E-1;
    private const double b11 =  4.47106157277725905176885569043E-2;

    // not needed here (advanced error calculation)
    private const double bhh1 = 0.244094488188976377952755905512E+00;
    private const double bhh2 = 0.733846688281611857341361741547E+00;
    private const double bhh3 = 0.220588235294117647058823529412E-01;

    // differences between high order and low order coefficients
    private const double er0  =  0.1312004499419488073250102996E-01;
    private const double er5  = -0.1225156446376204440720569753E+01;
    private const double er6  = -0.4957589496572501915214079952E+00;
    private const double er7  =  0.1664377182454986536961530415E+01;
    private const double er8  = -0.3503288487499736816886487290E+00;
    private const double er9 =  0.3341791187130174790297318841E+00;
    private const double er10 =  0.8192320648511571246570742613E-01;
    private const double er11 = -0.2235530786388629525884427845E-01;

    // scheme coefficients
    private const double a10 =    5.26001519587677318785587544488E-2;
    private const double a20 =    1.97250569845378994544595329183E-2;
    private const double a21 =    5.91751709536136983633785987549E-2;
    private const double a30 =    2.95875854768068491816892993775E-2;
    private const double a32 =    8.87627564304205475450678981324E-2;
    private const double a40 =    2.41365134159266685502369798665E-1;
    private const double a42 =   -8.84549479328286085344864962717E-1;
    private const double a43 =    9.24834003261792003115737966543E-1;
    private const double a50 =    3.7037037037037037037037037037E-2;
    private const double a53 =    1.70828608729473871279604482173E-1;
    private const double a54 =    1.25467687566822425016691814123E-1;
    private const double a60 =    3.7109375E-2;
    private const double a63 =    1.70252211019544039314978060272E-1;
    private const double a64 =    6.02165389804559606850219397283E-2;
    private const double a65 =   -1.7578125E-2;
    private const double a70 =    3.70920001185047927108779319836E-2;
    private const double a73 =    1.70383925712239993810214054705E-1;
    private const double a74 =    1.07262030446373284651809199168E-1;
    private const double a75 =   -1.53194377486244017527936158236E-2;
    private const double a76 =    8.27378916381402288758473766002E-3;
    private const double a80 =    6.24110958716075717114429577812E-1;
    private const double a83 =   -3.36089262944694129406857109825E0;
    private const double a84 =   -8.68219346841726006818189891453E-1;
    private const double a85 =    2.75920996994467083049415600797E1;
    private const double a86 =    2.01540675504778934086186788979E1;
    private const double a87 =   -4.34898841810699588477366255144E1;
    private const double a90 =   4.77662536438264365890433908527E-1;
    private const double a93 =  -2.48811461997166764192642586468E0;
    private const double a94 =  -5.90290826836842996371446475743E-1;
    private const double a95 =   2.12300514481811942347288949897E1;
    private const double a96 =   1.52792336328824235832596922938E1;
    private const double a97 =  -3.32882109689848629194453265587E1;
    private const double a98 =  -2.03312017085086261358222928593E-2;
    private const double a101 =  -9.3714243008598732571704021658E-1;
    private const double a103 =   5.18637242884406370830023853209E0;
    private const double a104 =   1.09143734899672957818500254654E0;
    private const double a105 =  -8.14978701074692612513997267357E0;
    private const double a106 =  -1.85200656599969598641566180701E1;
    private const double a107 =   2.27394870993505042818970056734E1;
    private const double a108 =   2.49360555267965238987089396762E0;
    private const double a109 = -3.0467644718982195003823669022E0;
    private const double a110 =   2.27331014751653820792359768449E0;
    private const double a113 =  -1.05344954667372501984066689879E1;
    private const double a114 =  -2.00087205822486249909675718444E0;
    private const double a115 =  -1.79589318631187989172765950534E1;
    private const double a116 =   2.79488845294199600508499808837E1;
    private const double a117 =  -2.85899827713502369474065508674E0;
    private const double a118 =  -8.87285693353062954433549289258E0;
    private const double a119 =  1.23605671757943030647266201528E1;
    private const double a1110 =  6.43392746015763530355970484046E-1;
    // scheme coefficients for additional stages for dense output
    private const double a130 =  5.61675022830479523392909219681E-2;
    private const double a136 =  2.53500210216624811088794765333E-1;
    private const double a137 = -2.46239037470802489917441475441E-1;
    private const double a138 = -1.24191423263816360469010140626E-1;
    private const double a139 =  1.5329179827876569731206322685E-1;
    private const double a1310 =  8.20105229563468988491666602057E-3;
    private const double a1311 =  7.56789766054569976138603589584E-3;
    private const double a1312 = -8.298E-3;
    private const double a140 =  3.18346481635021405060768473261E-2;
    private const double a145 =  2.83009096723667755288322961402E-2;
    private const double a146 =  5.35419883074385676223797384372E-2;
    private const double a147 = -5.49237485713909884646569340306E-2;
    private const double a1410 = -1.08347328697249322858509316994E-4;
    private const double a1411 =  3.82571090835658412954920192323E-4;
    private const double a1412 = -3.40465008687404560802977114492E-4;
    private const double a1413 =  1.41312443674632500278074618366E-1;
    private const double a150 = -4.28896301583791923408573538692E-1;
    private const double a155 = -4.69762141536116384314449447206E0;
    private const double a156 =  7.68342119606259904184240953878E0;
    private const double a157 =  4.06898981839711007970213554331E0;
    private const double a158 =  3.56727187455281109270669543021E-1;
    private const double a1512 = -1.39902416515901462129418009734E-3;
    private const double a1513 =  2.9475147891527723389556272149E0;
    private const double a1514 = -9.15095847217987001081870187138E0;

    // Coefficients for dense output
    private const double d41  = -0.84289382761090128651353491142E+01;
    private const double d46  =  0.56671495351937776962531783590E+00;
    private const double d47  = -0.30689499459498916912797304727E+01;
    private const double d48  =  0.23846676565120698287728149680E+01;
    private const double d49  =  0.21170345824450282767155149946E+01;
    private const double d410 = -0.87139158377797299206789907490E+00;
    private const double d411 =  0.22404374302607882758541771650E+01;
    private const double d412 =  0.63157877876946881815570249290E+00;
    private const double d413 = -0.88990336451333310820698117400E-01;
    private const double d414 =  0.18148505520854727256656404962E+02;
    private const double d415 = -0.91946323924783554000451984436E+01;
    private const double d416 = -0.44360363875948939664310572000E+01;
    private const double d51  =  0.10427508642579134603413151009E+02;
    private const double d56  =  0.24228349177525818288430175319E+03;
    private const double d57  =  0.16520045171727028198505394887E+03;
    private const double d58  = -0.37454675472269020279518312152E+03;
    private const double d59  = -0.22113666853125306036270938578E+02;
    private const double d510 =  0.77334326684722638389603898808E+01;
    private const double d511 = -0.30674084731089398182061213626E+02;
    private const double d512 = -0.93321305264302278729567221706E+01;
    private const double d513 =  0.15697238121770843886131091075E+02;
    private const double d514 = -0.31139403219565177677282850411E+02;
    private const double d515 = -0.93529243588444783865713862664E+01;
    private const double d516 =  0.35816841486394083752465898540E+02;
    private const double d61 =  0.19985053242002433820987653617E+02;
    private const double d66 = -0.38703730874935176555105901742E+03;
    private const double d67 = -0.18917813819516756882830838328E+03;
    private const double d68 =  0.52780815920542364900561016686E+03;
    private const double d69 = -0.11573902539959630126141871134E+02;
    private const double d610 =  0.68812326946963000169666922661E+01;
    private const double d611 = -0.10006050966910838403183860980E+01;
    private const double d612 =  0.77771377980534432092869265740E+00;
    private const double d613 = -0.27782057523535084065932004339E+01;
    private const double d614 = -0.60196695231264120758267380846E+02;
    private const double d615 =  0.84320405506677161018159903784E+02;
    private const double d616 =  0.11992291136182789328035130030E+02;
    private const double d71  = -0.25693933462703749003312586129E+02;
    private const double d76  = -0.15418974869023643374053993627E+03;
    private const double d77  = -0.23152937917604549567536039109E+03;
    private const double d78  =  0.35763911791061412378285349910E+03;
    private const double d79  =  0.93405324183624310003907691704E+02;
    private const double d710 = -0.37458323136451633156875139351E+02;
    private const double d711 =  0.10409964950896230045147246184E+03;
    private const double d712 =  0.29840293426660503123344363579E+02;
    private const double d713 = -0.43533456590011143754432175058E+02;
    private const double d714 =  0.96324553959188282948394950600E+02;
    private const double d715 = -0.39177261675615439165231486172E+02;
    private const double d716 = -0.14972683625798562581422125276E+03;

    #endregion

    private static readonly double[][] _sa = new double[][]
        {
          new double[] { },
          new double[] { a10 },
          new double[] { a20,  a21 },
          new double[] { a30,  0,  a32 },
          new double[] { a40,  0,  a42, a43 },
          new double[] { a50,  0,  0,   a53,  a54 },
          new double[] { a60,  0,  0,   a63,  a64,  a65 },
          new double[] { a70,  0,  0,   a73,  a74,  a75,  a76  },
          new double[] { a80,  0,  0,   a83,  a84,  a85,  a86,  a87  },
          new double[] { a90,  0,  0,   a93,  a94,  a95,  a96,  a97,  a98 },
          new double[] { a101, 0,  0,   a103, a104, a105, a106, a107, a108, a109 },
          new double[] { a110, 0,  0,   a113, a114, a115, a116, a117, a118, a119, a1110 },
        };

    private static readonly double[] _sc = new double[] { 0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11 };

    /// <summary>The high order coefficients.</summary>
    private static readonly double[] _sbh = new double[] { b0, 0, 0, 0, 0, b5, b6, b7, b8, b9, b10, b11};

    /// <summary>
    /// Attention: this are <b>not</b> the low order coefficients itself, but the differenced between
    /// high order coefficients and low order coefficients.
    /// </summary>
    private static readonly double[] _sbl = new double[] { er0, 0, 0, 0, 0, er5, er6, er7, er8, er9, er10, er11 };  

#if true
    protected class Core1 : RungeKuttaExplicitBase.Core
    {
      /// <summary>True if the _k[12] (13th stage) was evaluated</summary>
      private bool _isK12Evaluated;

      /// <summary>True if dense output was prepared, i.e. the array _rcont contains values.</summary>
      private bool _isDenseOutputPrepared;

      /// <summary>Contains the precalcuated polynomial coefficients for dense output.</summary>
      private double[][] _rcont;



      public Core1(int order, int numberOfStages, double[][] a, double[] b, double[]? bl, double[] c, double x0, double[] y, Action<double, double[], double[]> f)
        : base(order, numberOfStages, a, b, bl, c, x0, y, f)
      {
      }

        /// <summary>
        /// Evaluates the next solution point in one step. To get the results, see <see cref="X"/> and <see cref="Y_volatile"/>.
        /// </summary>
        /// <param name="stepSize">Size of the step.</param>
        public override void EvaluateNextSolutionPoint(double stepSize)
      {
        var a = _a;
        var b = _b;
        var c = _c;
        var k = _k;

        int n = _y_current.Length; // number of variables
        int s = a.Length; // number of stages

        var h = stepSize;
        _stepSize_previous = _stepSize_current;
        _stepSize_current = stepSize;
        _x_previous = _x_current;
        _isDenseOutputPrepared = false;
        Exchange(ref _y_previous, ref _y_current); // swap the two arrays => what was current is now previous

        var x_previous = _x_previous;
        var y_previous = _y_previous;

        // calculate the derivatives k0 .. ks-1 (see [1] page 134)

        if (_wasSolutionPointEvaluated && _isK12Evaluated)
        {
          Exchange(ref k[12], ref k[0]);
        }
        else
        {
          _f(x_previous, y_previous, k[0]); // else we have to calculate the 1st stage
        }
        _isK12Evaluated = false;

        // Note: due to many zeros in the _a array it is preferrable to multiply directly
        var ysi = _ytemp;
        for (int ni = 0; ni < n; ++ni) // for all n
        {
          ysi[ni] = y_previous[ni] + h * a10 * k[0][ni];
        }
        _f(x_previous + h * c[1], ysi, k[1]); // calculate derivative k

        for (int ni = 0; ni < n; ++ni) // for all n
        {
          ysi[ni] = y_previous[ni] + h * (a20 * k[0][ni] + a21 * k[1][ni]);
        }
        _f(x_previous + h * c[1], ysi, k[2]); // calculate derivative k

        for (int ni = 0; ni < n; ++ni) // for all n
        {
          ysi[ni] = y_previous[ni] + h * (a30 * k[0][ni] + a32 * k[2][ni]);
        }
        _f(x_previous + h * c[2], ysi, k[3]); // calculate derivative k

        for (int ni = 0; ni < n; ++ni) // for all n
        {
          ysi[ni] = y_previous[ni] + h * (a40 * k[0][ni] + a42 * k[2][ni] + a43 * k[3][ni]);
        }
        _f(x_previous + h * c[3], ysi, k[4]); // calculate derivative k

        for (int ni = 0; ni < n; ++ni) // for all n
        {
          ysi[ni] = y_previous[ni] + h * (a50 * k[0][ni] + a53 * k[3][ni] + a54 * k[4][ni]);
        }
        _f(x_previous + h * c[4], ysi, k[5]); // calculate derivative k

        for (int ni = 0; ni < n; ++ni) // for all n
        {
          ysi[ni] = y_previous[ni] + h * (a60 * k[0][ni] + a63 * k[3][ni] + a64 * k[4][ni] + a65 * k[5][ni]);
        }
        _f(x_previous + h * c[5], ysi, k[6]); // calculate derivative k

        for (int ni = 0; ni < n; ++ni) // for all n
        {
          ysi[ni] = y_previous[ni] + h * (a70 * k[0][ni] + a73 * k[3][ni] + a74 * k[4][ni] + a75 * k[5][ni] + a76 * k[6][ni]);
        }
        _f(x_previous + h * c[6], ysi, k[7]); // calculate derivative k

        for (int ni = 0; ni < n; ++ni) // for all n
        {
          ysi[ni] = y_previous[ni] + h * (a80 * k[0][ni] + a83 * k[3][ni] + a84 * k[4][ni] + a85 * k[5][ni] + a86 * k[6][ni] + a87 * k[7][ni]);
        }
        _f(x_previous + h * c[7], ysi, k[8]); // calculate derivative k

        for (int ni = 0; ni < n; ++ni) // for all n
        {
          ysi[ni] = y_previous[ni] + h * (a90 * k[0][ni] + a93 * k[3][ni] + a94 * k[4][ni] + a95 * k[5][ni] + a96 * k[6][ni] + a97 * k[7][ni] + a98 * k[8][ni]);
        }
        _f(x_previous + h * c[8], ysi, k[9]); // calculate derivative k

        for (int ni = 0; ni < n; ++ni) // for all n
        {
          ysi[ni] = y_previous[ni] + h * (a101 * k[0][ni] + a103 * k[3][ni] + a104 * k[4][ni] + a105 * k[5][ni] + a106 * k[6][ni] + a107 * k[7][ni] + a108 * k[8][ni] + a109 * k[9][ni]);
        }
        _f(x_previous + h * c[9], ysi, k[10]); // calculate derivative k

        for (int ni = 0; ni < n; ++ni) // for all n
        {
          ysi[ni] = y_previous[ni] + h * (a110 * k[0][ni] + a113 * k[3][ni] + a114 * k[4][ni] + a115 * k[5][ni] + a116 * k[6][ni] + a117 * k[7][ni] + a118 * k[8][ni] + a119 * k[9][ni] + a1110 * k[10][ni]);
        }
        _f(x_previous + h * c[10], ysi, k[11]); // calculate derivative k

        // end calculation of k0 .. k[s-1]

        // Calculate y (high order)
        var y_current = _y_current;
        for (int ni = 0; ni < n; ++ni) // TODO Test if exchanging the order of sums is faster in calculation
        {
          double sum = 0;
          for (int si = 0; si < s; ++si)
          {
            sum += b[si] * k[si][ni];
          }
          y_current[ni] = y_previous[ni] + h * sum;
        }

       
        {
          // Attention: here we calculate not yl, but because the _bl contains only the
          // differences between high and low order coefficient, we calculate the (local) error
          var bl = _bl!;
          var yl = _y_current_lowPrecision;
          for (int ni = 0; ni < n; ++ni) // TODO Test if exchanging the order of sums is faster in calculation
          {
            double sum = 0;
            for (int si = 0; si < s; ++si)
            {
              sum += bl[si] * k[si][ni];
            }
            yl[ni] = h * sum; // yl contains now local error
          }
        }

        _x_current += stepSize;
        _wasSolutionPointEvaluated = true;
      }

      /// <summary>
      /// Gets the relative error, which should be in the order of 1, if the step size is optimally chosen.
      /// </summary>
      /// <returns>The relative error (relative to the absolute and relative tolerance).</returns>
      public override double GetRelativeError()
      {
        // Compute error (see [1], page 168
        // error computation in L2 or L-infinity norm is possible
        // here, L-infinity is used

        if (_bl is null)
        {
          throw new InvalidOperationException("In order to evaluate errors, the evaluation of the low order y has to be done, but the low order coefficients were not set!");
        }

        var y = _y_current;
        var yl = _y_current_lowPrecision;
        var yp = _y_previous;

        double e = double.MinValue;
        for (int i = 0; i < y.Length; ++i)
        {
          e = Math.Max(e, Math.Abs(yl[i]) / Math.Max(_absoluteTolerance, _relativeTolerance * Math.Max(Math.Abs(y[i]), Math.Abs(yp[i]))));
        }

        return e;
      }

      /// <summary>
      /// Gets the recommended step size.  
      /// </summary>
      /// <param name="error_current">The relative error of the current step.</param>
      /// <param name="error_previous">The relative error of the previous step.</param>
      /// <returns>The recommended step size in the context of the absolute and relative tolerances.</returns>
      public override double GetRecommendedStepSize(double error_current, double error_previous)
      {
        // H211b digital filter, see Table 1 in [Söderlind, 2003, Adaptive Time-Stepping and Computational Stability]
        if (_stepSize_previous == 0)
        {
          _stepSize_previous = _stepSize_current;
        }
          var fac = Math.Pow(error_current * error_previous, -0.25 / _order) * Math.Pow(_stepSize_current / _stepSize_previous, -0.25);
          fac = Math.Min(StepSize_MaxFactor, Math.Max(StepSize_MinFactor, StepSize_SafetyFactor*fac));
          return fac * _stepSize_current;
      }


      /// <summary>
      /// Determines whether the ODE has become stiff. If a stiffness condition is detected, then an exception will be thrown.
      /// </summary>
      public override void ThrowIfStiffnessDetected()
      {
        if(_stiffnessDetectionEveryNumberOfSteps > 0 &&
             (  (++_numberOfRejectedStiffnessDetectionCalls >= _stiffnessDetectionEveryNumberOfSteps) ||
                (_numberOfNonstiffEvaluationResults > 0)
             )
          )
        {
          _numberOfRejectedStiffnessDetectionCalls = 0;

          if (!_isK12Evaluated)
          {
            _isK12Evaluated = true;
            _f(_x_current, _y_current, _k[12]); // evaluate slope with new y at new x
          }

          int n = _y_current.Length;
          double sumSquaredSlopeDifferences = 0;
          double sumSquaredValueDifferences = 0;
          double q;
          double h = _stepSize_current;
          for (int ni = 0; ni < n; ni++)
          {
            q = _k[12][ni] - _k[11][ni]; // difference between slope k[12] and the slope k[11]
            sumSquaredSlopeDifferences += q * q;
            q = _y_current[ni] - _ytemp[ni]; // difference of the y at the end of the step used to calc k[12] and the y used to calculate k[11]
            sumSquaredValueDifferences += q * q;
          }

          if (sumSquaredValueDifferences > 0 && (h * h * sumSquaredSlopeDifferences) / sumSquaredValueDifferences > 37.21)
          {
            _numberOfNonstiffEvaluationResults = 0;
            _numberOfStiffEvaluationResults++;
            if (_numberOfStiffEvaluationResults == 15)
            {
              throw new InvalidOperationException($"Stiffness detected in ODE at x={_x_current}");
            }
          }
          else
          {
            _numberOfNonstiffEvaluationResults++;
            if (_numberOfNonstiffEvaluationResults == 6)
              _numberOfStiffEvaluationResults = 0;
          }
        }
      }

      /// <summary>Get an interpolated point in the last evaluated interval.
      /// Please use the result immediately, or else make a copy of the result, since a internal array
      /// is returned, which is overwritten at the next operation.</summary>
      /// <param name="theta">Relative location (0..1) in the last evaluated interval.</param>
      /// <returns>Interpolated y values at the relative point of the last evaluated interval <paramref name="theta"/>.</returns>
      /// <remarks>See ref. [2] section 3.3.</remarks>
      public override double[] GetInterpolatedY_volatile(double theta)
      {
        var k = _k;
        var y = _y_previous;
        var ys = _ytemp;
        int n = y.Length;

        /* the next three function evaluations */
        var h = _stepSize_current;
        var k0 = _k[0];
        var k1 = _k[1];
        var k2 = _k[2];
        var k3 = _k[3];
        var k4 = _k[4];
        var k5 = _k[5];
        var k6 = _k[6];
        var k7 = _k[7];
        var k8 = _k[8];
        var k9 = _k[9];
        var k10 = _k[10];
        var k11 = _k[11];
        var k12 = _k[12];
        var k13 = _k[13];
        var k14 = _k[14];
        var k15 = _k[15];

        if (!_isK12Evaluated)
        {
          _isK12Evaluated = true;
          _f(_x_current, _y_current, k[12]); // evaluate slope with new y at new x
        }

        if (_rcont is null)
        {
          _rcont = new double[8][];
          for (int i = 0; i < _rcont.Length; ++i)
            _rcont[i] = new double[n];
        }
        var rcont0 = _rcont[0];
        var rcont1 = _rcont[1];
        var rcont2 = _rcont[2];
        var rcont3 = _rcont[3];
        var rcont4 = _rcont[4];
        var rcont5 = _rcont[5];
        var rcont6 = _rcont[6];
        var rcont7 = _rcont[7];


        if (!_isDenseOutputPrepared)
        {
          _isDenseOutputPrepared = true;

          for (int i = 0; i < n; i++)
          {
            ys[i] = y[i] + h * (a130 * k0[i] + a136 * k6[i] + a137 * k7[i] + a138 * k8[i] + a139 * k9[i] + a1310 * k10[i] + a1311 * k11[i] + a1312 * k12[i]);
          }
          _f(_x_previous + c12 * h, ys, k13);

          for (int i = 0; i < n; i++)
          {
            ys[i] = y[i] + h * (a140 * k0[i] + a145 * k5[i] + a146 * k6[i] + a147 * k7[i] + a1410 * k10[i] + a1411 * k11[i] + a1412 * k12[i] + a1413 * k13[i]);
          }
          _f(_x_previous + c13 * h, ys, k14);

          for (int i = 0; i < n; i++)
          {
            ys[i] = y[i] + h * (a150 * k0[i] + a155 * k5[i] + a156 * k6[i] + a157 * k7[i] + a158 * k8[i] + a1512 * k12[i] + a1513 * k13[i] + a1514 * k14[i]);
          }
          _f(_x_previous + c14 * h, ys, k15);

          

         
          for (int i = 0; i < n; i++)
          {
            rcont0[i] = y[i]; // values at begin of step
            var ydiff = _y_current[i] - y[i]; // values at end of step minus values at begin of step
            rcont1[i] = ydiff;
            var bspl = h * k0[i] - ydiff;
            rcont2[i] = bspl;
            rcont3[i] = ydiff - h * k12[i] - bspl; 
            rcont4[i] = d41 * k0[i] + d46 * k5[i] + d47 * k6[i] + d48 * k7[i] +
                d49 * k8[i] + d410 * k9[i] + d411 * k10[i] + d412 * k11[i];
            rcont5[i] = d51 * k0[i] + d56 * k5[i] + d57 * k6[i] + d58 * k7[i] +
                d59 * k8[i] + d510 * k9[i] + d511 * k10[i] + d512 * k11[i];
            rcont6[i] = d61 * k0[i] + d66 * k5[i] + d67 * k6[i] + d68 * k7[i] +
                d69 * k8[i] + d610 * k9[i] + d611 * k10[i] + d612 * k11[i];
            rcont7[i] = d71 * k0[i] + d76 * k5[i] + d77 * k6[i] + d78 * k7[i] +
                d79 * k8[i] + d710 * k9[i] + d711 * k10[i] + d712 * k11[i];
          }
          // TODO append upstairs
          for (int i = 0; i < n; i++)
          {
            rcont4[i] = h * (rcont4[i] + d413 * k12[i] + d414 * k13[i] +
                d415 * k14[i] + d416 * k15[i]);
            rcont5[i] = h * (rcont5[i] + d513 * k12[i] + d514 * k13[i] +
                d515 * k14[i] + d516 * k15[i]);
            rcont6[i] = h * (rcont6[i] + d613 * k12[i] + d614 * k13[i] +
                d615 * k14[i] + d616 * k15[i]);
            rcont7[i] = h * (rcont7[i] + d713 * k12[i] + d714 * k13[i] +
                d715 * k14[i] + d716 * k15[i]);
          }
        }

        var s = theta;
        var s1 = 1 - theta;


        for (int i = 0; i < n; i++)
        {
          ys[i] = rcont0[i] + s * (rcont1[i] + s1 * (rcont2[i] + s * (rcont3[i] + s1 * (rcont4[i] + s * (rcont5[i] + s1 * (rcont6[i] + s * rcont7[i]))))));
        }

        return ys;
      }
    }
#endif

    /// <summary>
    /// Initializes the Runge-Kutta method.
    /// </summary>
    /// <param name="x">The initial x value.</param>
    /// <param name="y">The initial y values.</param>
    /// <param name="f">Calculation of the derivatives. First argument is x value, 2nd argument are the current y values. The 3rd argument is an array that store the derivatives.</param>
    /// <returns>This instance (for a convenient way to chain this method with sequence creation).</returns>
    public override RungeKuttaExplicitBase Initialize(double x, double[] y, Action<double, double[], double[]> f)
    {
      _core = new Core1(Order, NumberOfStages, A, BH, BL, C, x, y, f);
      if (InterpolationCoefficients is not null)
        _core.InterpolationCoefficients = InterpolationCoefficients;

      return this;
    }

    /// <inheritdoc/>
    public override int Order => 8;

    public override int NumberOfStages => 16;

    /// <inheritdoc/>
    protected override double[][] A => _sa;

    /// <inheritdoc/>
    protected override double[] BH => _sbh;

    /// <inheritdoc/>
    protected override double[] BL => _sbl;

    /// <inheritdoc/>
    protected override double[] C => _sc;

    /// <inheritdoc/>
    protected override double[][]? InterpolationCoefficients => null;
  }
}

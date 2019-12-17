#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Science.Thermodynamics.Fluids
{

  /// <summary>
  /// State equations and constants of water.
  /// Short name: water.
  /// Synomym: R-718.
  /// Chemical formula: H2O.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'water.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Wagner, W. and Pruss, A., "The IAPWS Formulation 1995 for the Thermodynamic Properties of Ordinary Water Substance for General and Scientific Use," J. Phys. Chem. Ref. Data, 31(2):387-535, 2002.</para>
  /// <para>HeatCapacity (CPP): Wagner, W. and Pruss, A., "The IAPWS Formulation 1995 for the Thermodynamic Properties of Ordinary Water Substance for General and Scientific Use," J. Phys. Chem. Ref. Data, 31(2):387-535, 2002.</para>
  /// <para>Melting pressure: Wagner, W., Saul, A., and Pruss, A., "International Equations for the Pressure Along the Melting and Along the Sublimation Curve of Ordinary Water Substance," J. Phys. Chem. Ref. Data, 23(3):515-527, 1994.</para>
  /// <para>Sublimation pressure:  Wagner, W., Riethmann, T., Feistel, R., and Harvey, A.H., "New Equations for the Sublimation Pressure and Melting Pressure of H2O Ice Ih," J. Phys. Chem. Ref. Data, 40(4), 2011.</para>
  /// <para>Saturated vapor pressure: See EOS</para>
  /// <para>Saturated liquid density: See EOS</para>
  /// <para>Saturated vapor density: See EOS</para>
  /// </remarks>
  [CASRegistryNumber("7732-18-5")]
  public class Water : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Water Instance { get; } = new Water();

    #region Constants for water

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "water";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "water";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "R-718";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "H2O";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "7732-18-5";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314371357587;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.018015268; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 647.096;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 22064000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 17873.7279956;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 273.16;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 612.48;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 55496.9551400019;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.269470080865637;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 373.124295864882;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.3443;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.855;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 273.16;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 2000;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 73960;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 1000000000;

    #endregion Constants for water

    private Water()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -8.32044648376779;
      _alpha0_n_tau = 6.68321052759772;
      _alpha0_n_lntau = 3.00632;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (            0.012436,     1.28728967572045),
          (             0.97315,     3.53734221815619),
          (              1.2795,     7.74073707765154),
          (             0.96956,     9.24437795937543),
          (             0.24873,     27.5075104775798),
      };

      _alpha0_Cosh = new (double ni, double thetai)[]
      {
      };

      _alpha0_Sinh = new (double ni, double thetai)[]
      {
      };
      #endregion Ideal part of dimensionless Helmholtz energy and derivatives

      #region Residual part(s) of dimensionless Helmholtz energy and derivatives

      _alphaR_Poly = new (double ni, double ti, int di)[]
      {
          (   0.012533547935523,                 -0.5,                    1),
          (     7.8957634722828,                0.875,                    1),
          (    -8.7803203303561,                    1,                    1),
          (    0.31802509345418,                  0.5,                    2),
          (   -0.26145533859358,                 0.75,                    2),
          ( -0.0078199751687981,                0.375,                    3),
          (  0.0088089493102134,                    1,                    4),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (   -0.66856572307965,                    4,                    1,                   -1,                    1),
          (    0.20433810950965,                    6,                    1,                   -1,                    1),
          (-6.6212605039687E-05,                   12,                    1,                   -1,                    1),
          (   -0.19232721156002,                    1,                    2,                   -1,                    1),
          (   -0.25709043003438,                    5,                    2,                   -1,                    1),
          (    0.16074868486251,                    4,                    3,                   -1,                    1),
          (  -0.040092828925807,                    2,                    4,                   -1,                    1),
          ( 3.9343422603254E-07,                   13,                    4,                   -1,                    1),
          (-7.5941377088144E-06,                    9,                    5,                   -1,                    1),
          ( 0.00056250979351888,                    3,                    7,                   -1,                    1),
          (-1.5608652257135E-05,                    4,                    9,                   -1,                    1),
          ( 1.1537996422951E-09,                   11,                   10,                   -1,                    1),
          ( 3.6582165144204E-07,                    4,                   11,                   -1,                    1),
          (-1.3251180074668E-12,                   13,                   13,                   -1,                    1),
          (-6.2639586912454E-10,                    1,                   15,                   -1,                    1),
          (   -0.10793600908932,                    7,                    1,                   -1,                    2),
          (   0.017611491008752,                    1,                    2,                   -1,                    2),
          (    0.22132295167546,                    9,                    2,                   -1,                    2),
          (   -0.40247669763528,                   10,                    2,                   -1,                    2),
          (    0.58083399985759,                   10,                    3,                   -1,                    2),
          (  0.0049969146990806,                    3,                    4,                   -1,                    2),
          (  -0.031358700712549,                    7,                    4,                   -1,                    2),
          (   -0.74315929710341,                   10,                    4,                   -1,                    2),
          (     0.4780732991548,                   10,                    5,                   -1,                    2),
          (   0.020527940895948,                    6,                    6,                   -1,                    2),
          (   -0.13636435110343,                   10,                    6,                   -1,                    2),
          (   0.014180634400617,                   10,                    7,                   -1,                    2),
          (  0.0083326504880713,                    1,                    9,                   -1,                    2),
          (  -0.029052336009585,                    2,                    9,                   -1,                    2),
          (   0.038615085574206,                    3,                    9,                   -1,                    2),
          (  -0.020393486513704,                    4,                    9,                   -1,                    2),
          ( -0.0016554050063734,                    8,                    9,                   -1,                    2),
          (  0.0019955571979541,                    6,                   10,                   -1,                    2),
          ( 0.00015870308324157,                    9,                   10,                   -1,                    2),
          ( -1.638856834253E-05,                    8,                   12,                   -1,                    2),
          (   0.043613615723811,                   16,                    3,                   -1,                    3),
          (   0.034994005463765,                   22,                    4,                   -1,                    3),
          (  -0.076788197844621,                   23,                    4,                   -1,                    3),
          (   0.022446277332006,                   23,                    5,                   -1,                    3),
          (-6.2689710414685E-05,                   10,                   14,                   -1,                    4),
          (-5.5711118565645E-10,                   50,                    3,                   -1,                    6),
          (   -0.19905718354408,                   44,                    6,                   -1,                    6),
          (    0.31777497330738,                   46,                    6,                   -1,                    6),
          (   -0.11841182425981,                   50,                    6,                   -1,                    6),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (    -31.306260323435,                    0,                    3,                  -20,                 -150,                 1.21,                    1),
          (     31.546140237781,                    1,                    3,                  -20,                 -150,                 1.21,                    1),
          (    -2521.3154341695,                    4,                    3,                  -20,                 -250,                 1.25,                    1),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
          (   -0.14874640856724,                 0.85,                  0.3,                 0.32,                   28,                  700,                  0.2,                  3.5),
          (    0.31806110878444,                 0.95,                  0.3,                 0.32,                   32,                  800,                  0.2,                  3.5),
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 2;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (          1.99274064,                    1),
          (          1.09965342,                    2),
          (        -0.510839303,                    5),
          (         -1.75493479,                   16),
          (         -45.5170352,                   43),
          (          -674694.45,                  110),
      };

      _saturatedVaporDensity_Type = 4;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (          -2.0315024,                    1),
          (          -2.6830294,                    2),
          (         -5.38626492,                    4),
          (         -17.2991605,                    9),
          (         -44.7586581,                 18.5),
          (         -63.9201063,                 35.5),
      };

      _saturatedVaporPressure_Type = 6;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (         -7.85951783,                    2),
          (          1.84408259,                    3),
          (         -11.7866497,                    6),
          (          22.6807411,                    7),
          (         -15.9618719,                    8),
          (          1.80122502,                   15),
      };

      #endregion Saturated densities and pressure

      #region Sublimation pressure

      _sublimationPressure_Type = 2;
      _sublimationPressure_ReducingTemperature = 273.16;
      _sublimationPressure_ReducingPressure = 611.657;
      _sublimationPressure_PolynomialCoefficients1 = new (double factor, double exponent)[]
      {
          (         -21.2144006,       -0.99666666667),
          (          27.3203819,           0.20666667),
          (          -6.1059813,           0.70333333),
      };

      #endregion Sublimation pressure

      #region Melting pressure

      _meltingPressure_Type = 'H';
      _meltingPressure_ReducingTemperature = 273.16;
      _meltingPressure_ReducingPressure = 611657;
      _meltingPressure_Coefficients = new (double factor, double exponent)[][]
      {
      new (double factor, double exponent)[]
        {
            (          1195393.37,                    3),
            (          80818.3159,                25.75),
            (           3338.2686,               103.75),
        },

      new (double factor, double exponent)[]
        {
            (           -0.299948,                   60),
        },

      new (double factor, double exponent)[]
        {
            (            -1.18721,                    8),
        },

      new (double factor, double exponent)[]
        {
            (            -1.07476,                  4.6),
        },

      new (double factor, double exponent)[]
        {
            (             1.73683,                   -1),
            (          -0.0544606,                    5),
            (         8.06106E-08,                   22),
        },

      };

      #endregion Melting pressure

    }
  }
}

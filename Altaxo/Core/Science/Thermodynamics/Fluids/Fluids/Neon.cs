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
  /// State equations and constants of neon.
  /// Short name: neon.
  /// Synomym: R-720.
  /// Chemical formula: Ne.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'neon.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Katti, R.S., Jacobsen, R.T, Stewart, R.B., and Jahangiri, M., "Thermodynamic Properties for Neon for Temperatures from the Triple Point to 700 K at Pressures to 700 MPa," Adv. Cryo. Eng., 31:1189-1197, 1986.</para>
  /// <para>HeatCapacity (CPP): Katti, R.S., Jacobsen, R.T, Stewart, R.B., and Jahangiri, M., "Thermodynamic Properties for Neon for Temperatures from the Triple Point to 700 K at Pressures to 700 MPa," Adv. Cryo. Eng., 31:1189-1197, 1986.</para>
  /// <para>Melting pressure: Lemmon, E.W., preliminary equation, 2010.</para>
  /// <para>Sublimation pressure: Lemmon, E.W., 2004.</para>
  /// <para>Saturated vapor pressure: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// <para>Saturated liquid density: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// <para>Saturated vapor density: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("7440-01-9")]
  public class Neon : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Neon Instance { get; } = new Neon();

    #region Constants for neon

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "neon";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "neon";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "R-720";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "Ne";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "7440-01-9";

    private int[] _unNumbers = new int[] { 1065, 1913, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.31434;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.020179; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 44.4918;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 2678600;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 23882;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 24.556;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 43368;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 62065.0134179137;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 219.817931603607;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 27.1044323280105;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = -0.0387;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 24.556;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 700;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 90560;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 700000000;

    #endregion Constants for neon

    private Neon()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -3.05975215752343;
      _alpha0_n_tau = 3.25252304474641;
      _alpha0_n_lntau = 1.5;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
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
          (         3.532653449,                  0.5,                    1),
          (        -4.513954384,                 0.75,                    1),
          (       -0.1524027959,                  3.5,                    1),
          (         2.188568609,                  0.5,                    2),
          (         -7.44299997,                 0.75,                    2),
          (         7.755627402,                    1,                    2),
          (        -3.122553128,                  1.5,                    2),
          (         1.014206899,                  2.5,                    2),
          (      -0.05289214086,                 0.25,                    3),
          (        0.1566849239,                  0.5,                    3),
          (        -0.222852705,                  2.5,                    3),
          (      -0.01410150942,                    1,                    4),
          (       0.07036229719,                    3,                    4),
          (      -0.05882048367,                    4,                    4),
          (       0.01571172741,                    5,                    4),
          (      0.001292202769,                    1,                    6),
          (     0.0007902035603,                    5,                    6),
          (    -0.0003794403616,                    6,                    6),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (       0.04652799333,                    4,                    1,                   -1,                    3),
          (       0.04524001818,                    1,                    2,                   -1,                    2),
          (       -0.2383421991,                    5,                    2,                   -1,                    2),
          (       0.00629359013,                    8,                    2,                   -1,                    4),
          (     -0.001272313644,                   12,                    2,                   -1,                    6),
          (     -1.75235256E-07,                   32,                    2,                   -1,                    6),
          (      0.007188419232,                   10,                    4,                   -1,                    2),
          (      -0.05403006914,                    6,                    8,                   -1,                    2),
          (       0.07578222187,                    7,                    8,                   -1,                    2),
          (      -0.03808588254,                    8,                    8,                   -1,                    2),
          (      0.006034022431,                    9,                    8,                   -1,                    2),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              1.0601,                 0.33),
          (              120.76,                  1.4),
          (             -385.53,                  1.7),
          (              816.55,                  2.2),
          (             -899.07,                  2.6),
          (              354.66,                    3),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -2.3338,                0.444),
          (             -3.6834,                 0.95),
          (             -85.368,                  3.5),
          (              227.69,                  4.1),
          (              -172.9,                  4.5),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -5.5805,                    1),
          (            0.068795,                  1.5),
          (               5.484,                  2.3),
          (              -8.376,                  2.8),
          (              3.4276,                  3.4),
      };

      #endregion Saturated densities and pressure

      #region Sublimation pressure

      _sublimationPressure_Type = 3;
      _sublimationPressure_ReducingTemperature = 24.556;
      _sublimationPressure_ReducingPressure = 43464;
      _sublimationPressure_PolynomialCoefficients1 = new (double factor, double exponent)[]
      {
      };

      _sublimationPressure_PolynomialCoefficients2 = new (double factor, double exponent)[]
      {
          (              -10.65,                    1),
      };

      #endregion Sublimation pressure

      #region Melting pressure

      _meltingPressure_Type = '1';
      _meltingPressure_ReducingTemperature = 24.556;
      _meltingPressure_ReducingPressure = 43368.14;
      _meltingPressure_Coefficients = new (double factor, double exponent)[][]
      {
      new (double factor, double exponent)[]
        {
            (                   1,                    0),
        },

      new (double factor, double exponent)[]
        {
            (                4437,                 1.33),
        },

      new (double factor, double exponent)[]{},
      };

      #endregion Melting pressure

    }
  }
}

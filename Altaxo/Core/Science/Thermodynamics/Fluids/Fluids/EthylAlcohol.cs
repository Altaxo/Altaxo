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
  /// State equations and constants of ethyl alcohol.
  /// Short name: ethanol.
  /// Synomym: methyl carbinol.
  /// Chemical formula: C2H6O.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'ethanol.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>Saturated vapor pressure: Lemmon, E.W., 2011.</para>
  /// <para>Saturated liquid density: Lemmon, E.W., 2011.</para>
  /// <para>Saturated vapor density: Lemmon, E.W., 2011.</para>
  /// </remarks>
  [CASRegistryNumber("64-17-5")]
  public class EthylAlcohol : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static EthylAlcohol Instance { get; } = new EthylAlcohol();

    #region Constants for ethyl alcohol

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "ethyl alcohol";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "ethanol";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "methyl carbinol";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C2H6O";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "alcohol";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "64-17-5";

    private int[] _unNumbers = new int[] { 1170, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.04606844; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 514.71;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 6268000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 5930;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 159;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.0007184;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 19731.4862945971;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 5.43463379816621E-07;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 351.570404513367;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.646;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.6909;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 159;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 650;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 19740;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 280000000;

    #endregion Constants for ethyl alcohol

    private EthylAlcohol()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -3.64440074896309;
      _alpha0_n_tau = 5.0708082313992;
      _alpha0_n_lntau = 3.43069;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (             2.14326,    0.816770608692273),
          (             5.09206,     2.59175069456587),
          (             6.60138,     3.80408385304346),
          (             5.70777,     8.58735987254959),
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
          (         0.058200796,                    1,                    4),
          (          0.94391227,                 1.04,                    1),
          (         -0.80941908,                 2.72,                    1),
          (          0.55359038,                1.174,                    2),
          (          -1.4269032,                1.329,                    2),
          (          0.13448717,                0.195,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (          0.42671978,                 2.43,                    1,                   -1,                    1),
          (          -1.1700261,                1.274,                    1,                   -1,                    1),
          (         -0.92405872,                 4.16,                    1,                   -1,                    2),
          (          0.34891808,                  3.3,                    3,                   -1,                    1),
          (          -0.9132772,                4.177,                    3,                   -1,                    2),
          (         0.022629481,                  2.5,                    2,                   -1,                    1),
          (         -0.15513423,                 0.81,                    2,                   -1,                    2),
          (          0.21055146,                 2.02,                    6,                   -1,                    1),
          (          -0.2199769,                1.606,                    6,                   -1,                    1),
          (       -0.0065857238,                 0.86,                    8,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (          0.75564749,                  2.5,                    1,               -1.075,               -1.207,                1.194,                0.779),
          (           0.1069411,                 3.72,                    1,               -0.463,              -0.0895,                1.986,                0.805),
          (        -0.069533844,                 1.19,                    2,               -0.876,               -0.581,                1.583,                1.869),
          (         -0.24947395,                 3.25,                    3,               -1.108,               -0.947,                0.756,                0.694),
          (         0.027177891,                    3,                    3,               -0.741,               -2.356,                0.495,                1.312),
          (       -0.0009053953,                    2,                    2,               -4.032,               -27.01,                1.002,                2.054),
          (         -0.12310953,                    2,                    2,               -2.453,               -4.542,                1.077,                0.441),
          (         -0.08977971,                    1,                    2,                 -2.3,               -1.287,                1.493,                0.793),
          (         -0.39512601,                    1,                    1,               -3.143,                -3.09,                1.542,                0.313),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              11.632,                 0.66),
          (             -218.66,                  1.5),
          (              826.94,                  1.9),
          (             -1351.2,                  2.3),
          (              1051.7,                  2.7),
          (             -318.09,                  3.1),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              2.2543,                 0.18),
          (             -24.734,                 0.44),
          (              48.993,                 0.68),
          (             -41.689,                 0.95),
          (             -45.104,                    4),
          (             -107.32,                   10),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -9.1043,                    1),
          (              4.7263,                  1.5),
          (             -9.7145,                    2),
          (              4.1536,                 2.55),
          (             -2.0739,                    4),
      };

      #endregion Saturated densities and pressure

    }
  }
}

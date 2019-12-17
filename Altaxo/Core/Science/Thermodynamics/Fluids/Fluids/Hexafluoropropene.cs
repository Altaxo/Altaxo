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
  /// State equations and constants of hexafluoropropene.
  /// Short name: R1216.
  /// Synomym: hexafluoropropylene.
  /// Chemical formula: C3F6.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'r1216.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Zhou, Y. and Lemmon, E.W.preliminary equation, 2010.</para>
  /// <para>Saturated vapor pressure: Herrig, S., 2013.</para>
  /// <para>Saturated liquid density: Herrig, S., 2013.</para>
  /// <para>Saturated vapor density: Herrig, S., 2013.</para>
  /// </remarks>
  [CASRegistryNumber("116-15-4")]
  public class Hexafluoropropene : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Hexafluoropropene Instance { get; } = new Hexafluoropropene();

    #region Constants for hexafluoropropene

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "hexafluoropropene";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "R1216";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "hexafluoropropylene";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C3F6";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "116-15-4";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.1500225192; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 358.9;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3149528;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 3888.8;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 117.654;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.0936;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 12881.5906500646;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 9.56501791286983E-05;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 242.812222297852;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.333;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.088;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 117.654;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 400;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 12890;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 12000000;

    #endregion Constants for hexafluoropropene

    private Hexafluoropropene()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -15.4368793052103;
      _alpha0_n_tau = 9.86463892325139;
      _alpha0_n_lntau = 4.878676;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (            9.351559,     1.56310950125383),
          (            9.192089,      4.1404290888827),
          (            7.983222,     21.1618835330176),
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
          (         0.037582356,                    1,                    4),
          (           1.4558246,                  0.3,                    1),
          (           -2.701615,                    1,                    1),
          (          -0.3357347,                 1.35,                    2),
          (           0.1885495,                  0.4,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (          -0.1689206,                    1,                    3,                   -1,                    2),
          (            1.122147,                 1.68,                    2,                   -1,                    1),
          (          -0.6405048,                 2.36,                    2,                   -1,                    2),
          (        -0.025931535,                0.615,                    7,                   -1,                    1),
          (          0.42940852,                 1.32,                    1,                   -1,                    1),
          (          -1.0163408,                 2.12,                    1,                   -1,                    2),
          (        -0.043691328,                    3,                    1,                   -1,                    3),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (           1.2530663,                 0.82,                    1,              -0.9665,                -1.24,                1.284,                 0.67),
          (         -0.54254994,                 2.85,                    1,               -1.503,               -0.776,                 0.42,                0.925),
          (         -0.15327764,                 2.83,                    3,                -0.97,                -0.86,                0.434,                 0.75),
          (       -0.0092102535,                 1.67,                    3,                -5.87,                 -478,                1.074,                 0.73),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              1.7159,                 0.31),
          (              2.3953,                 0.97),
          (             -5.8035,                  1.7),
          (              10.749,                  2.4),
          (             -10.537,                  3.2),
          (              4.7535,                  4.1),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -2.4969,                0.353),
          (             -5.8935,                 1.05),
          (             -16.846,                 2.74),
          (             -55.082,                    6),
          (             -140.43,                 13.3),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.9011,                    1),
          (              3.1506,                  1.5),
          (             -3.0852,                    2),
          (             -4.2112,                  4.5),
          (             -15.438,                   19),
      };

      #endregion Saturated densities and pressure

    }
  }
}

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
  /// State equations and constants of diethanolamine.
  /// Short name: diethanolamine.
  /// Synomym: bis(2-hydroxyethyl)amine.
  /// Chemical formula: C4H11NO2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'dea.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>HeatCapacity (CPP): See EOS of Herrig et al. (2017)</para>
  /// <para>Saturated vapor pressure: See EOS</para>
  /// <para>Saturated liquid density: See EOS</para>
  /// <para>Saturated vapor density: See EOS</para>
  /// </remarks>
  [CASRegistryNumber("111-42-2")]
  public class Diethanolamine : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Diethanolamine Instance { get; } = new Diethanolamine();

    #region Constants for diethanolamine

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "diethanolamine";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "diethanolamine";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "bis(2-hydroxyethyl)amine";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C4H11NO2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "111-42-2";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.3144621;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.1051356; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 736.5;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 4950746;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 3300;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 301.1;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.134;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 10391.2754991951;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 5.35087397543665E-05;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 541.234104107913;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 1.013;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = -1;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 301.1;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 1000;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 10400;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 10000000;

    #endregion Constants for diethanolamine

    private Diethanolamine()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 19.509551965002;
      _alpha0_n_tau = -2.97030848657288;
      _alpha0_n_lntau = 3;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (                4.25,    0.130346232179226),
          (                37.7,     1.58180583842498),
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
          (         0.066088158,                    1,                    4),
          (           6.1059245,                0.507,                    1),
          (          -7.0526968,                0.907,                    1),
          (         -0.29739545,                 1.22,                    2),
          (          0.11592105,                0.649,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (          -1.8616953,                 2.14,                    1,                   -1,                    2),
          (         -0.97392153,                 2.89,                    3,                   -1,                    2),
          (          0.14690655,                 1.54,                    2,                   -1,                    1),
          (         -0.63284478,                 3.34,                    2,                   -1,                    2),
          (        -0.037820123,                0.998,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (            2.774726,                 0.92,                    1,              -0.8971,              -0.9691,                1.216,               0.6694),
          (          -1.0230468,                 1.16,                    1,               -1.499,               -1.518,               0.6775,               0.6466),
          (         -0.19552536,                 1.43,                    3,               -1.681,               -1.328,               0.7815,               0.6669),
          (         -0.14997584,                 1.24,                    2,               -1.661,                -1.23,               0.8796,                1.189),
          (         -0.23421918,                0.801,                    2,               -1.245,               -1.112,                1.357,                1.248),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 2;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              16.042,                  1.6),
          (             -42.419,                  2.3),
          (              57.684,                  3.1),
          (             -39.054,                    4),
          (              10.561,                  5.2),
      };

      _saturatedVaporDensity_Type = 4;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -16.987,                 5.25),
          (             -5.6021,                 1.33),
          (              -58.91,                   13),
          (             -131.44,                   30),
          (             -286.92,                   62),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (              -13.82,                    1),
          (              12.129,                  1.5),
          (             -11.899,                    2),
          (             -2.3243,                  3.9),
          (             -3.0683,                   11),
      };

      #endregion Saturated densities and pressure

    }
  }
}

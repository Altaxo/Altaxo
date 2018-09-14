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
  /// State equations and constants of Dodecafluoro-2-methylpentan-3-one.
  /// Short name: 1,1,1,2,2,4,5,5,5-nonafluoro-4-(trifluoromethyl)-3-pentanone.
  /// Synomym: Novec 649, 1230, FK-5-1-12.
  /// Chemical formula: CF3CF2C(=O)CF(CF3)2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'novec649.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): McLinden, M.O., Perkins, R.A., Lemmon, E.W., and Fortin, T.J. "Thermodynamic Properties of 1,1,1,2,2,4,5,5,5-Nonafluoro-4- (trifluoromethyl)-3-pentanone: Vapor Pressure, (p, rho, T) Behavior, and Speed of Sound Measurements, and an Equation of State," J. Chem. Eng. Data, 60:3646-3659, 2015. DOI: 10.1021/acs.jced.5b00623</para>
  /// <para>HeatCapacity (CPP): see EOS</para>
  /// <para>Saturated vapor pressure: See EOS</para>
  /// <para>Saturated liquid density: See EOS</para>
  /// <para>Saturated vapor density: See EOS</para>
  /// </remarks>
  [CASRegistryNumber("756-13-8")]
  public class Dodecafluoro_2_methylpentan_3_one : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Dodecafluoro_2_methylpentan_3_one Instance { get; } = new Dodecafluoro_2_methylpentan_3_one();

    #region Constants for Dodecafluoro-2-methylpentan-3-one

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "Dodecafluoro-2-methylpentan-3-one";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "1,1,1,2,2,4,5,5,5-nonafluoro-4-(trifluoromethyl)-3-pentanone";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "Novec 649, 1230, FK-5-1-12";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CF3CF2C(=O)CF(CF3)2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "756-13-8";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.3144621;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.3160444; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 441.81;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 1869000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 1920;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 165;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.2315;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 6234.0305707654;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.000168712105639693;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 322.201649728293;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.471;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0.36;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 165;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 500;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 6240;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 50000000;

    #endregion Constants for Dodecafluoro-2-methylpentan-3-one

    private Dodecafluoro_2_methylpentan_3_one()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -30.6610457048243;
      _alpha0_n_tau = 6.83052754651674;
      _alpha0_n_lntau = 29.8;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (                29.8,     4.39102781738757),
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
          (          0.05623648,                    1,                    4),
          (            2.973616,                 0.25,                    1),
          (            -6.12697,                0.793,                    1),
          (             3.44024,                 1.16,                    1),
          (            1.451737,                 0.75,                    2),
          (           -2.837857,                 1.09,                    2),
          (           0.2077767,                 0.75,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (            2.168307,                  1.3,                    2,                   -1,                    1),
          (           -2.124648,                 2.25,                    1,                   -1,                    2),
          (           -1.296704,                  1.9,                    2,                   -1,                    2),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (           -1.010569,                 0.88,                    1,                -0.32,                -0.12,                  1.1,                 1.16),
          (            2.701505,                 1.63,                    1,                -1.32,                -0.83,                 1.04,                0.793),
          (           0.8167202,                  1.3,                    2,                -1.35,                -0.19,                 1.15,                 1.13),
          (           -1.814579,                    2,                    2,                -1.48,                -0.95,                  0.9,                0.527),
          (           0.2075389,                 1.15,                    3,                -0.51,                 -0.1,                  0.8,                 1.19),
          (           -1.009347,                 1.66,                    3,                 -1.3,                -0.11,                  1.2,                 0.83),
          (         -0.04848043,                  1.5,                    1,                -5.15,                  -65,                 1.19,                 0.82),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              1.5545,                0.297),
          (               1.149,                  0.7),
          (             0.51565,                  4.4),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -1.6073,                0.291),
          (             -5.8095,                 0.82),
          (             -17.824,                 2.45),
          (             -61.012,                  5.5),
          (              -151.3,                   12),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -8.4411,                    1),
          (               2.711,                  1.5),
          (             -3.6354,                  2.2),
          (             -5.3872,                  4.4),
          (             -8.1641,                   15),
      };

      #endregion Saturated densities and pressure

    }
  }
}

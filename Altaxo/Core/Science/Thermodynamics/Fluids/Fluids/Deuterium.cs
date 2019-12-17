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
  /// State equations and constants of deuterium.
  /// Short name: deuterium.
  /// Synomym: deuterium.
  /// Chemical formula: D2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'd2.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Richardson, I.A., Leachman, J.W., and Lemmon, E.W.  J. Phys. Chem. Ref. Data, in preparation (2013).</para>
  /// <para>HeatCapacity (CPP): see EOS of Richardson et al. (2013)</para>
  /// <para>Saturated vapor pressure: see EOS for referenceequation fit by Stefan Herrig, 2013.</para>
  /// <para>Saturated liquid density: see EOS for referenceequation fit by Stefan Herrig, 2013.</para>
  /// <para>Saturated vapor density: see EOS for referenceequation fit by Stefan Herrig, 2013.</para>
  /// </remarks>
  [CASRegistryNumber("7782-39-0")]
  public class Deuterium : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Deuterium Instance { get; } = new Deuterium();

    #region Constants for deuterium

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "deuterium";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "deuterium";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "deuterium";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "D2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "7782-39-0";

    private int[] _unNumbers = new int[] { 1957, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.3144621;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.0040282; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 38.34;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 1679600;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 17230;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 18.724;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 17189;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 43350.9208903565;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 112.956403421749;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 23.6613147623572;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = -0.136;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 18.724;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 600;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 43351;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 2000000000;

    #endregion Constants for deuterium

    private Deuterium()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -2.06773517486885;
      _alpha0_n_tau = 2.42371514995516;
      _alpha0_n_lntau = 1.5;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (            -3.54145,     187.117892540428),
          (              3.0326,     225.221700573813),
          (            -3.52422,     23.5446009389671),
          (            -1.73421,     4.72352634324465),
          (            -3.57135,     11.4371413667188),
          (             2.14858,     131.304121022431),
          (             6.23107,      7.0396452790819),
          (            -3.30425,     5.99634846113719),
          (             6.23098,     17.3813249869588),
          (            -3.57137,     11.8101199791341),
          (             3.32901,     5.00782472613458),
          (             0.97782,     30.9754825247783),
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
          (         0.006267958,                    1,                    4),
          (            10.53609,                0.462,                    1),
          (           -10.14149,               0.5584,                    1),
          (            0.356061,                0.627,                    2),
          (           0.1824472,                1.201,                    3),
          (           -1.129638,                0.309,                    1),
          (          -0.0549812,                1.314,                    3),
          (          -0.6791329,               1.1166,                    2),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (            1.347918,                 1.25,                    2,                   -1,                    1),
          (          -0.8657582,                 1.25,                    2,                   -1,                    1),
          (            1.719146,                1.395,                    1,                   -1,                    2),
          (           -1.917977,                1.627,                    1,                   -1,                    2),
          (           0.1233365,                    1,                    3,                   -1,                    2),
          (         -0.07936891,                  2.5,                    2,                   -1,                    2),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (            1.686617,                0.635,                    1,               -0.868,               -0.613,               0.6306,                 1.46),
          (           -4.240326,                0.664,                    1,               -0.636,               -0.584,                0.711,               1.7864),
          (            1.857114,               0.7082,                    2,               -0.668,                -0.57,               0.6446,                1.647),
          (          -0.5903705,                 2.25,                    3,                -0.65,               -1.056,               0.8226,                0.541),
          (            1.520171,                1.524,                    3,               -0.745,                -1.01,                0.992,                0.969),
          (            2.361373,                 0.67,                    1,               -0.782,               -1.025,               1.2184,                1.892),
          (           -2.297315,                0.709,                    3,               -0.693,               -1.029,                1.203,                1.076),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              3.3769,                0.512),
          (             -5.3693,                 1.12),
          (              11.943,                  1.8),
          (             -17.361,                 2.55),
          (               15.17,                  3.4),
          (             -6.3079,                  4.4),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -3.8111,                0.528),
          (             -7.3624,                 2.03),
          (              2.2294,                  3.6),
          (             -21.443,                    5),
          (              12.796,                  6.5),
          (             -31.334,                    9),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -5.5706,                    1),
          (              1.7631,                  1.5),
          (             -0.5458,                 2.83),
          (              1.2154,                 4.06),
          (             -1.1556,                  5.4),
      };

      #endregion Saturated densities and pressure

    }
  }
}

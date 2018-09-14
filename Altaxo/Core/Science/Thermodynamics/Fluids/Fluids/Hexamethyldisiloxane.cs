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
  /// State equations and constants of hexamethyldisiloxane.
  /// Short name: MM.
  /// Synomym: MM.
  /// Chemical formula: C6H18OSi2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'mm.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Thol, M., Dubberke, F.H., Rutkai, G., Windmann, T., K�ster, A., Span, R., Vrabec, J. "Fundamental equation of state correlation for hexamethyldisiloxane based on experimental and molecular simulation data," Fluid Phase Equilibria, 418:133-151, 2016.</para>
  /// <para>HeatCapacity (CPP):  see EOS Thol et al. (2015).</para>
  /// <para>Saturated vapor pressure: See EOS</para>
  /// <para>Saturated liquid density: See EOS</para>
  /// <para>Saturated vapor density: See EOS</para>
  /// </remarks>
  [CASRegistryNumber("107-46-0")]
  public class Hexamethyldisiloxane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Hexamethyldisiloxane Instance { get; } = new Hexamethyldisiloxane();

    #region Constants for hexamethyldisiloxane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "hexamethyldisiloxane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "MM";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "MM";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C6H18OSi2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "107-46-0";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.3144621;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.1623768; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 518.7;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 1931130;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 1653;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 204.93;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 2.95;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 5266.15208859688;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.00173359126223292;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 373.657861419576;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.418;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0.801;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 204.93;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 580;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 5266;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 130000000;

    #endregion Constants for hexamethyldisiloxane

    private Hexamethyldisiloxane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 72.1107540155206;
      _alpha0_n_tau = -10.431499346371;
      _alpha0_n_lntau = 3;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (               19.74,     6.94042799305957),
          (               29.58,     2.69905533063428),
          (               18.59,   0.0385579332947754),
          (                4.87,     12.1457489878542),
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
          (          0.05063651,                    1,                    4),
          (            8.604724,                0.346,                    1),
          (           -9.179684,                 0.46,                    1),
          (           -1.146325,                 1.01,                    2),
          (           0.4878559,                 0.59,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (           -2.434088,                  2.6,                    1,                   -1,                    2),
          (           -1.621326,                 3.33,                    3,                   -1,                    2),
          (           0.6239872,                 0.75,                    2,                   -1,                    1),
          (           -2.306057,                 2.95,                    2,                   -1,                    2),
          (         -0.05555096,                 0.93,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (            9.385015,                 1.33,                    1,              -1.0334,              -0.4707,               1.7754,               0.8927),
          (           -2.493508,                 1.68,                    1,               -1.544,                -0.32,                0.692,               0.5957),
          (           -3.308032,                  1.7,                    3,               -1.113,               -0.404,                1.242,                0.559),
          (          -0.1885803,                 3.08,                    3,               -1.113,               -0.517,                0.421,                1.056),
          (         -0.09883865,                 5.41,                    1,                -1.11,               -0.432,                0.406,                  1.3),
          (            0.111109,                  1.4,                    2,                 -7.2,                 -7.2,                0.163,                0.106),
          (           0.1061928,                  1.1,                    3,                -1.45,                 -1.2,                0.795,                0.181),
          (         -0.01452454,                  5.3,                    1,                -4.73,                -35.8,                 0.88,                0.525),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (               4.003,                0.436),
          (              -6.406,                0.827),
          (                11.5,                 1.24),
          (              -10.04,                  1.7),
          (                   4,                 2.23),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -3.7421,                0.428),
          (             -37.087,                 1.79),
          (               75.46,                 2.28),
          (              -71.67,                  2.8),
          (              -68.69,                    7),
          (              -178.4,                 15.4),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -8.5023,                    1),
          (               3.803,                  1.5),
          (              -3.415,                 1.98),
          (              -4.679,                 3.86),
          (              -3.106,                 14.6),
      };

      #endregion Saturated densities and pressure

    }
  }
}

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
  /// State equations and constants of 1,1,1,2,3,3-hexafluoropropane.
  /// Short name: R236ea.
  /// Synomym: HFC-236ea.
  /// Chemical formula: CF3CHFCHF2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'r236ea.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Rui, X., Pan, J., Wang, Y. "An Equation of State for Thermodynamic Properties of 1,1,1,2,3,3-Hexafluoropropane (R236ea)," Fluid Phase Equilib., 341:78-85, 2013.</para>
  /// <para>HeatCapacity (CPP): see EOS of Rui et al. (2013)</para>
  /// <para>Saturated vapor pressure: Lemmon, E.W., 2012.</para>
  /// <para>Saturated liquid density: Lemmon, E.W., 2012.</para>
  /// <para>Saturated vapor density: Lemmon, E.W., 2012.</para>
  /// </remarks>
  [CASRegistryNumber("431-63-0")]
  public class Fluid_1_1_1_2_3_3_hexafluoropropane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Fluid_1_1_1_2_3_3_hexafluoropropane Instance { get; } = new Fluid_1_1_1_2_3_3_hexafluoropropane();

    #region Constants for 1,1,1,2,3,3-hexafluoropropane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "1,1,1,2,3,3-hexafluoropropane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "R236ea";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "HFC-236ea";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CF3CHFCHF2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "halocb";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "431-63-0";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.1520384; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 412.44;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3420000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 3716.16644;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 170;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 20;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 11706.9789404426;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.016730464154324;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 279.322154386101;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.369;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.129;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 240;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 412;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 10500;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 6000000;

    #endregion Constants for 1,1,1,2,3,3-hexafluoropropane

    private Fluid_1_1_1_2_3_3_hexafluoropropane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -14.1214241350077;
      _alpha0_n_tau = 10.2355589224742;
      _alpha0_n_lntau = 2.762;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (              0.7762,    0.349141693337213),
          (               10.41,     0.93346911065852),
          (               12.18,      3.7241780622636),
          (               3.332,     17.2655416545437),
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
          (            0.051074,                    1,                    4),
          (              2.5584,                0.264,                    1),
          (              -2.918,               0.5638,                    1),
          (            -0.71485,                1.306,                    2),
          (             0.15534,               0.2062,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (             -1.5894,                2.207,                    1,                   -1,                    2),
          (              -0.784,                2.283,                    3,                   -1,                    2),
          (             0.85767,                1.373,                    2,                   -1,                    1),
          (            -0.67235,                 2.33,                    2,                   -1,                    2),
          (           -0.017953,               0.6376,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (              1.3165,                 1.08,                    1,               -1.019,                 -1.3,                 1.13,               0.7119),
          (            -0.42023,                 1.67,                    1,               -1.341,               -2.479,               0.6691,               0.9102),
          (            -0.28053,                3.502,                    3,               -1.034,               -1.068,                0.465,                0.678),
          (             -1.4134,                4.357,                    3,               -5.264,               -79.85,                 1.28,               0.7091),
          (         -6.2617E-06,               0.6945,                    2,               -24.44,               -49.06,               0.8781,                1.727),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              1.6074,                 0.31),
          (              1.5021,                 0.75),
          (              -1.106,                  1.3),
          (             0.91146,                  1.9),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -2.7426,                0.376),
          (             -6.2268,                  1.1),
          (             -15.109,                  2.7),
          (             -49.524,                  5.5),
          (             -114.11,                   11),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.9095,                    1),
          (              2.3374,                  1.5),
          (             -2.6453,                 2.15),
          (             -5.7058,                 4.75),
      };

      #endregion Saturated densities and pressure

    }
  }
}

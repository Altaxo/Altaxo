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
  /// State equations and constants of 1-butene.
  /// Short name: butene.
  /// Synomym: 1-butylene.
  /// Chemical formula: CH3-CH2-CH=CH2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file '1butene.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Lemmon, E.W. and Ihmels, E.C., "Thermodynamic Properties of the Butenes.  Part II. Short Fundamental Equations of State," Fluid Phase Equilibria, 228-229C:173-187, 2005.</para>
  /// <para>HeatCapacity (CPP): Lemmon, E.W. and Ihmels, E.C.,</para>
  /// <para>Saturated vapor pressure: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// <para>Saturated liquid density: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// <para>Saturated vapor density: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("106-98-9")]
  public class Fluid_1_butene : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Fluid_1_butene Instance { get; } = new Fluid_1_butene();

    #region Constants for 1-butene

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "1-butene";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "butene";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "1-butylene";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CH3-CH2-CH=CH2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "n-alkene";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "106-98-9";

    private int[] _unNumbers = new int[] { 1012, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.05610632; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 419.29;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 4005100;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 4240;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 87.8;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 5.945E-07;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = double.NaN;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = double.NaN;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 266.844025286423;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.192;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0.339;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 87.8;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 525;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 14590;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 70000000;

    #endregion Constants for 1-butene

    private Fluid_1_butene()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -0.00101125749664277;
      _alpha0_n_tau = 2.38691738589906;
      _alpha0_n_lntau = 2.9197;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (              2.9406,    0.653485654320399),
          (              6.5395,     2.26811991700255),
          (              14.535,     5.07286126547258),
          (              5.8971,     13.7184287724487),
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
          (             0.78084,                 0.12,                    1),
          (             -2.8258,                  1.3,                    1),
          (             0.99403,                 1.74,                    1),
          (            0.017951,                  2.1,                    2),
          (            0.088889,                 0.28,                    3),
          (          0.00024673,                 0.69,                    7),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (             0.22846,                 0.75,                    2,                   -1,                    1),
          (           -0.074009,                    2,                    5,                   -1,                    1),
          (            -0.22913,                  4.4,                    1,                   -1,                    2),
          (           -0.062334,                  4.7,                    4,                   -1,                    2),
          (           -0.025385,                   15,                    3,                   -1,                    3),
          (             0.01104,                   14,                    4,                   -1,                    3),
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
          (              16.857,                0.547),
          (              -46.28,                 0.73),
          (              53.727,                 0.92),
          (             -23.314,                 1.14),
          (              1.8889,                  2.1),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -3.1106,                0.415),
          (             -6.3103,                 1.27),
          (             -19.272,                 3.34),
          (             -48.739,                    7),
          (             -99.898,                 14.5),
          (             -190.01,                   28),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.1727,                    1),
          (               2.636,                  1.5),
          (             -2.0781,                    2),
          (              -2.886,                 4.35),
          (             -1.3041,                   16),
      };

      #endregion Saturated densities and pressure

    }
  }
}

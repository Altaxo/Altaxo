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
  /// State equations and constants of dodecane.
  /// Short name: dodecane.
  /// Synomym: n-dodecane.
  /// Chemical formula: CH3-10(CH2)-CH3.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'c12.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Lemmon, E.W. and Huber, M.L. "Thermodynamic Properties of n-Dodecane," Energy &amp; Fuels, 18:960-967, 2004.</para>
  /// <para>HeatCapacity (CPP): Lemmon, E.W., Based on TRC Thermodynamic Properties of Substances in the Ideal Gas State Version 1.0M, 1994.</para>
  /// <para>Saturated vapor pressure: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// <para>Saturated liquid density: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// <para>Saturated vapor density: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("112-40-3")]
  public class Dodecane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Dodecane Instance { get; } = new Dodecane();

    #region Constants for dodecane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "dodecane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "dodecane";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "n-dodecane";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CH3-10(CH2)-CH3";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "n-alkane";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "112-40-3";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.17033484; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 658.1;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 1817000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 1330;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 263.6;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.6262;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 4529.14637363983;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.00028572146420915;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 489.441527973425;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.574;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 263.6;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 700;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 4530;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 700000000;

    #endregion Constants for dodecane

    private Dodecane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 20.5642482037707;
      _alpha0_n_tau = -15.5930644722644;
      _alpha0_n_lntau = 22.085;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (              37.776,     1.94499316213341),
          (              29.369,     3.64534265309224),
          (              12.461,     8.66129767512536),
          (              7.7733,     21.0743048168971),
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
          (             1.38031,                 0.32,                    1),
          (            -2.85352,                 1.23,                    1),
          (            0.288897,                  1.5,                    1),
          (           -0.165993,                  1.4,                    2),
          (           0.0923993,                 0.07,                    3),
          (         0.000282772,                  0.8,                    7),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (            0.956627,                 2.16,                    2,                   -1,                    1),
          (           0.0353076,                  1.1,                    5,                   -1,                    1),
          (           -0.445008,                  4.1,                    1,                   -1,                    2),
          (           -0.118911,                  5.6,                    4,                   -1,                    2),
          (          -0.0366475,                 14.5,                    3,                   -1,                    3),
          (           0.0184223,                   12,                    4,                   -1,                    3),
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
          (             0.92236,                 0.21),
          (             0.92047,                 0.49),
          (              5.5713,                 1.08),
          (             -9.2253,                 1.49),
          (              5.1763,                  1.9),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -1.7859,                0.298),
          (             -7.5436,                 0.91),
          (             -22.848,                  2.8),
          (             -81.355,                    6),
          (              92.283,                    9),
          (             -217.25,                   11),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -9.4217,                    1),
          (              -4.189,                  1.5),
          (              5.4999,                1.359),
          (             -6.7789,                 3.56),
          (             -1.7161,                  9.2),
      };

      #endregion Saturated densities and pressure

    }
  }
}

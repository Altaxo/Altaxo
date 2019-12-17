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
  /// State equations and constants of chloroethylene.
  /// Short name: vinyl chloride.
  /// Synomym: R1140.
  /// Chemical formula: C2H3Cl.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'vnychlrd.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Thol, M. and Span, R. (2014)</para>
  /// <para>HeatCapacity (CPP): see EOS</para>
  /// <para>Saturated vapor pressure: See EOS</para>
  /// <para>Saturated liquid density: See EOS</para>
  /// <para>Saturated vapor density: See EOS</para>
  /// </remarks>
  [CASRegistryNumber("75-01-4")]
  public class Chloroethylene : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Chloroethylene Instance { get; } = new Chloroethylene();

    #region Constants for chloroethylene

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "chloroethylene";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "vinyl chloride";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "R1140";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C2H3Cl";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "75-01-4";

    private int[] _unNumbers = new int[] { 1086, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.3144621;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.06249822; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 424.964;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 5590300;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 5620;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 119.31;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.0649;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 19231.0521298089;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 6.54679440943071E-05;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 259.443328187673;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.161;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = -1;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 190;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 450;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 19240;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 10000000;

    #endregion Constants for chloroethylene

    private Chloroethylene()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -3.3865189389002;
      _alpha0_n_tau = 3.58796064682482;
      _alpha0_n_lntau = 3;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (               3.354,     1.89192496305569),
          (               3.182,     10.3844090322945),
          (                5.49,     4.07563934827421),
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
          (         0.027915646,                    1,                    4),
          (             1.56343,                  0.2,                    1),
          (            -1.98447,                 0.76,                    1),
          (           -0.618706,                1.076,                    2),
          (            0.160016,                 0.49,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (           -0.987704,                 1.52,                    1,                   -1,                    2),
          (           -0.363759,                 2.93,                    3,                   -1,                    2),
          (            0.820064,                 1.16,                    2,                   -1,                    1),
          (           -0.380335,                 2.56,                    2,                   -1,                    2),
          (         -0.00952795,                    1,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (            0.583237,                 0.82,                    1,                -1.02,                -1.34,                 1.12,                0.717),
          (           -0.201067,                 0.86,                    1,                -1.42,                -1.62,                 0.65,                0.921),
          (           -0.153546,                  2.3,                    3,                   -1,                   -1,                  0.5,                 0.69),
          (           -0.519717,                  4.8,                    3,                -7.92,                -91.6,                 1.26,                0.763),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              1.5668,                  0.3),
          (               1.655,                 0.83),
          (               -1.03,                  1.5),
          (                0.87,                  2.6),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -1.7237,                  0.3),
          (              -4.431,                0.853),
          (              -12.02,                 2.39),
          (             -35.585,                 5.43),
          (               -57.6,                 10.6),
          (                -144,                   20),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -6.9978,                    1),
          (              2.3668,                  1.5),
          (               -1.92,                 2.12),
          (              -2.525,                 4.45),
          (               -78.9,                   31),
      };

      #endregion Saturated densities and pressure

    }
  }
}

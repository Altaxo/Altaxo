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
  /// State equations and constants of methylcyclohexane.
  /// Short name: methylcyclohexane.
  /// Synomym: cyclohexylmethane.
  /// Chemical formula: C6H11(CH3).
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'c1cc6.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Lemmon, E.W., unpublished equation, 2007.</para>
  /// <para>HeatCapacity (CPP): ThermoData Engine (TRC, NIST)</para>
  /// <para>Saturated vapor pressure: Lemmon, E.W., 2010.</para>
  /// <para>Saturated liquid density: Lemmon, E.W., 2010.</para>
  /// <para>Saturated vapor density: Lemmon, E.W., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("108-87-2")]
  public class Methylcyclohexane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Methylcyclohexane Instance { get; } = new Methylcyclohexane();

    #region Constants for methylcyclohexane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "methylcyclohexane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "methylcyclohexane";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "cyclohexylmethane";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C6H11(CH3)";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "naphthene";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "108-87-2";

    private int[] _unNumbers = new int[] { 2296, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.09818606; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 572.2;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3470000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 2720;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 146.7;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.0002726;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 9120.17920423216;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 2.23543368629355E-07;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 374.009893985061;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.234;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 146.7;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 600;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 9130;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 500000000;

    #endregion Constants for methylcyclohexane

    private Methylcyclohexane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 9.28867573062526;
      _alpha0_n_tau = -0.0546854651448681;
      _alpha0_n_lntau = 1.04122;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          (          -4.6969037,                   -1),
          (   -10.1124184074333,                   -2),
          (    4.91510619841781,                   -3),
          (  -0.887432230731183,                   -4),
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
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
          (              1.3026,                 0.38,                    1),
          (              -2.627,                  1.2,                    1),
          (             0.68834,                 2.14,                    1),
          (            -0.16415,                  1.6,                    2),
          (            0.092174,                  0.3,                    3),
          (           0.0003842,                  0.7,                    7),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (            -0.29737,                  2.7,                    1,                   -1,                    1),
          (           -0.078187,                 3.25,                    2,                   -1,                    1),
          (           -0.049139,                 2.35,                    5,                   -1,                    1),
          (            -0.30402,                  3.7,                    1,                   -1,                    2),
          (           -0.074888,                  4.1,                    4,                   -1,                    2),
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
          (            0.018273,                  0.1),
          (              15.215,                 0.64),
          (             -21.951,                  0.8),
          (              9.4466,                    1),
          (             0.16781,                  4.5),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -5.2572,                0.544),
          (             -13.417,                  2.3),
          (             -2.4271,                  2.5),
          (             -54.482,                  6.1),
          (             -157.91,                   15),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -6.5871,                    1),
          (             -5.6553,                  1.5),
          (              6.8947,                  1.6),
          (             -4.1281,                  3.2),
          (             -2.5444,                   10),
      };

      #endregion Saturated densities and pressure

    }
  }
}

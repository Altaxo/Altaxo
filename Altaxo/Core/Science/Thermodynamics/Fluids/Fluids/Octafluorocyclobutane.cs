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
  /// State equations and constants of octafluorocyclobutane.
  /// Short name: RC318.
  /// Synomym: FC-C318.
  /// Chemical formula: cyclo-C4F8.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'rc318.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Platzer, B., Polt, A., and Maurer, G., "Thermophysical properties of refrigerants," Berlin,  Springer-Verlag, 1990.\</para>
  /// <para>HeatCapacity (CPP): Platzer, B., Polt, A., and Maurer, G., "Thermophysical properties of refrigerants," Berlin,  Springer-Verlag, 1990.\</para>
  /// <para>Saturated vapor pressure: Cullimore, I.D., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("115-25-3")]
  public class Octafluorocyclobutane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Octafluorocyclobutane Instance { get; } = new Octafluorocyclobutane();

    #region Constants for octafluorocyclobutane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "octafluorocyclobutane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "RC318";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "FC-C318";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "cyclo-C4F8";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "halocb";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "115-25-3";

    private int[] _unNumbers = new int[] { 1976, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.31451;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.20004; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 388.38;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 2777500;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 3099.38;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 233.35;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 19461;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 8645.18363397638;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 10.1569040024854;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 267.17532407243;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.3553;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 233.35;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 623;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 8645.2;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 60000000;

    #endregion Constants for octafluorocyclobutane

    private Octafluorocyclobutane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -29.5113255756708;
      _alpha0_n_tau = 18.6074967761369;
      _alpha0_n_lntau = 0.0794357628474305;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          (         -0.56373357,                   -1),
          (    0.06367166618298,                   -2),
          (-0.00376839130762081,                   -3),
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
          (      -1.04729119796,                    3,                    0),
          (       1.38034128822,                    4,                    0),
          (     -0.333625769594,                    5,                    0),
          (       1.09415569278,                    0,                    1),
          (      -2.68265237178,                    1,                    1),
          (       1.73403063905,                    2,                    1),
          (      -1.63611246876,                    3,                    1),
          (      0.304834499143,                    4,                    1),
          (      0.102771551787,                    0,                    2),
          (    -0.0232367895587,                    1,                    2),
          (      0.166151957803,                    2,                    2),
          (    -0.0250103914479,                    0,                    3),
          (     0.0935680977639,                    1,                    3),
          (     0.0431929127445,                    0,                    4),
          (     -0.133439845861,                    1,                    4),
          (     0.0255416632165,                    1,                    5),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (       1.04729119796,                    3,                    0,          -0.99943992,                    2),
          (      -1.38034128822,                    4,                    0,          -0.99943992,                    2),
          (      0.333625769594,                    5,                    0,          -0.99943992,                    2),
          (     -0.510485781618,                    3,                    2,          -0.99943992,                    2),
          (       1.81840728111,                    4,                    2,          -0.99943992,                    2),
          (       -1.3853089397,                    5,                    2,          -0.99943992,                    2),
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
          (            -0.30181,                 0.11),
          (              2.9345,                 0.32),
          (             -1.3741,                 0.57),
          (               1.465,                 0.84),
          (             0.16963,                  2.9),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -24.491,                 0.61),
          (              53.255,                 0.77),
          (             -38.863,                 0.92),
          (             -24.938,                  3.3),
          (             -90.092,                  7.5),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.8467,                    1),
          (              2.4555,                  1.5),
          (             -3.0824,                  2.2),
          (             -5.8263,                  4.8),
          (              3.5483,                  6.2),
      };

      #endregion Saturated densities and pressure

    }
  }
}

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
  /// State equations and constants of cyclopropane.
  /// Short name: cyclopropane.
  /// Synomym: trimethylene.
  /// Chemical formula: cyclo-C3H6.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'cyclopro.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Polt, A., Platzer, B., and Maurer, G., "Parameter der thermischen Zustandsgleichung von Bender fuer 14 mehratomige reine Stoffe," Chem. Tech. (Leipzig), 44(6):216-224, 1992.</para>
  /// <para>HeatCapacity (CPP): Polt, A., Platzer, B., and Maurer, G., "Parameter der thermischen Zustandsgleichung von Bender fuer 14 mehratomige reine Stoffe," Chem. Tech. (Leipzig), 44(6):216-224, 1992.</para>
  /// <para>Saturated vapor pressure: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// <para>Saturated liquid density: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// <para>Saturated vapor density: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("75-19-4")]
  public class Cyclopropane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Cyclopropane Instance { get; } = new Cyclopropane();

    #region Constants for cyclopropane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "cyclopropane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "cyclopropane";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "trimethylene";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "cyclo-C3H6";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "naphthene";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "75-19-4";

    private int[] _unNumbers = new int[] { 1027, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.3143;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.042081; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 398.3;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 5579700;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 6142.9149;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 145.7;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 342710;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = double.NaN;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = double.NaN;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 241.67;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.1305;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 273;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 473;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 15595;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 28000000;

    #endregion Constants for cyclopropane

    private Cyclopropane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -10.1443032340771;
      _alpha0_n_tau = 7.15851443061926;
      _alpha0_n_lntau = 1.06258152040113;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          (        1.8029188905,                   -1),
          (   -1.33657692444267,                   -2),
          (   0.406630807765964,                   -3),
          ( -0.0510121406352334,                   -4),
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
          (      -1.37016097588,                    3,                    0),
          (       2.12444673002,                    4,                    0),
          (     -0.578908942724,                    5,                    0),
          (      -1.15633726379,                    0,                    1),
          (       2.52574014413,                    1,                    1),
          (      -2.82265442929,                    2,                    1),
          (      0.283576113255,                    3,                    1),
          (    -0.0842718450726,                    4,                    1),
          (      0.931086305879,                    0,                    2),
          (      -1.05296584292,                    1,                    2),
          (       0.43202053292,                    2,                    2),
          (     -0.251108254803,                    0,                    3),
          (      0.127725582443,                    1,                    3),
          (     0.0483621161849,                    0,                    4),
          (    -0.0116473795607,                    1,                    4),
          (   0.000334005754773,                    1,                    5),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (       1.37016097588,                    3,                    0,                   -1,                    2),
          (      -2.12444673002,                    4,                    0,                   -1,                    2),
          (      0.578908942724,                    5,                    0,                   -1,                    2),
          (      0.304945770499,                    3,                    2,                   -1,                    2),
          (     -0.184276165165,                    4,                    2,                   -1,                    2),
          (     -0.292111460397,                    5,                    2,                   -1,                    2),
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
          (             0.16998,                 0.11),
          (              3.5101,                  0.5),
          (             -2.7092,                  0.8),
          (              1.7644,                  1.1),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (            -0.33232,                  0.1),
          (             -29.566,                 0.87),
          (              57.762,                 1.14),
          (             -142.21,                 1.78),
          (              325.73,                 2.32),
          (             -244.39,                  2.6),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.3438,                    1),
          (              17.584,                  1.5),
          (             -34.265,                 1.71),
          (              20.155,                 1.95),
          (             -7.7259,                    4),
      };

      #endregion Saturated densities and pressure

    }
  }
}

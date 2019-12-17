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
  /// State equations and constants of tetrafluoromethane.
  /// Short name: R14.
  /// Synomym: FC-14.
  /// Chemical formula: CF4.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'r14.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Platzer, B., Polt, A., and Maurer, G., "Thermophysical properties of refrigerants," Berlin,  Springer-Verlag, 1990.\</para>
  /// <para>HeatCapacity (CPP): Platzer, B., Polt, A., and Maurer, G., "Thermophysical properties of refrigerants," Berlin,  Springer-Verlag, 1990.\</para>
  /// <para>Saturated vapor pressure: Cullimore, I.D., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("75-73-0")]
  public class Tetrafluoromethane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Tetrafluoromethane Instance { get; } = new Tetrafluoromethane();

    #region Constants for tetrafluoromethane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "tetrafluoromethane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "R14";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "FC-14";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CF4";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "halocb";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "75-73-0";

    private int[] _unNumbers = new int[] { 1982, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.31451;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.08801; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 227.51;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3750000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 7109.4194;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 89.54;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 101.2;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 21165.5015778192;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.135950515056038;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 145.104844365671;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.1785;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 120;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 623;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 20764;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 51000000;

    #endregion Constants for tetrafluoromethane

    private Tetrafluoromethane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -8.53898542941672;
      _alpha0_n_tau = 13.4877018150024;
      _alpha0_n_lntau = 2.9465247;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          (     1.0077182902375,                   -1),
          (   -1.20254365809127,                   -2),
          (   0.294954044021814,                   -3),
          ( -0.0274669590771138,                   -4),
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
          (     -0.334698748966,                    3,                    0),
          (      0.586690904687,                    4,                    0),
          (     -0.147068929692,                    5,                    0),
          (       1.03999039623,                    0,                    1),
          (      -2.45792025288,                    1,                    1),
          (      0.799614557889,                    2,                    1),
          (     -0.749498954929,                    3,                    1),
          (      0.152177772502,                    4,                    1),
          (     -0.293408331764,                    0,                    2),
          (      0.717794502866,                    1,                    2),
          (    -0.0426467444199,                    2,                    2),
          (      0.226562749365,                    0,                    3),
          (     -0.391091694003,                    1,                    3),
          (    -0.0257394804936,                    0,                    4),
          (     0.0554844884782,                    1,                    4),
          (    0.00610988261204,                    1,                    5),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (      0.334698748966,                    3,                    0,          -0.99832625,                    2),
          (     -0.586690904687,                    4,                    0,          -0.99832625,                    2),
          (      0.147068929692,                    5,                    0,          -0.99832625,                    2),
          (     -0.190315426142,                    3,                    2,          -0.99832625,                    2),
          (      0.716157133959,                    4,                    2,          -0.99832625,                    2),
          (     -0.703161904626,                    5,                    2,          -0.99832625,                    2),
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
          (             -1.0612,                  0.1),
          (              4.4343,                 0.24),
          (             -3.8753,                  0.4),
          (              2.9825,                  0.6),
          (             0.30746,                  3.9),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -55.804,                0.713),
          (              108.68,                 0.84),
          (             -64.257,                    1),
          (             -1195.4,                  5.8),
          (              3668.8,                  6.3),
          (             -2595.6,                  6.6),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -6.1905,                    1),
          (             -9.1398,                  1.5),
          (              12.192,                 1.64),
          (             -4.7215,                  2.5),
          (             -2.0439,                  7.3),
      };

      #endregion Saturated densities and pressure

    }
  }
}

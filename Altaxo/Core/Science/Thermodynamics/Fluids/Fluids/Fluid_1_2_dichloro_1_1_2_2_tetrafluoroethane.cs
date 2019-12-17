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
  /// State equations and constants of 1,2-dichloro-1,1,2,2-tetrafluoroethane.
  /// Short name: R114.
  /// Synomym: CFC-114.
  /// Chemical formula: CClF2CClF2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'r114.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Platzer, B., Polt, A., and Maurer, G., "Thermophysical properties of refrigerants," Berlin,  Springer-Verlag, 1990.\</para>
  /// <para>HeatCapacity (CPP): Platzer, B., Polt, A., and Maurer, G., "Thermophysical properties of refrigerants," Berlin,  Springer-Verlag, 1990.\</para>
  /// <para>Saturated vapor pressure: Cullimore, I.D., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("76-14-2")]
  public class Fluid_1_2_dichloro_1_1_2_2_tetrafluoroethane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Fluid_1_2_dichloro_1_1_2_2_tetrafluoroethane Instance { get; } = new Fluid_1_2_dichloro_1_1_2_2_tetrafluoroethane();

    #region Constants for 1,2-dichloro-1,1,2,2-tetrafluoroethane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "1,2-dichloro-1,1,2,2-tetrafluoroethane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "R114";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "CFC-114";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CClF2CClF2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "halocb";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "76-14-2";

    private int[] _unNumbers = new int[] { 1958, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.31451;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.170921; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 418.83;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3257000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 3393.2;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 180.63;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 202.1;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = double.NaN;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = double.NaN;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 276.74147589048;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.2523;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0.658;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 273.15;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 507;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 8942;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 21000000;

    #endregion Constants for 1,2-dichloro-1,1,2,2-tetrafluoroethane

    private Fluid_1_2_dichloro_1_1_2_2_tetrafluoroethane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -25.7463599849729;
      _alpha0_n_tau = 15.7202866192649;
      _alpha0_n_lntau = 0.0490086022547242;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          (     -0.678684906315,                   -1),
          (   0.172359386004097,                   -2),
          ( -0.0412532842969466,                   -3),
          ( 0.00545637790050856,                   -4),
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
          (     -0.340776521414,                    3,                    0),
          (       0.32300139842,                    4,                    0),
          (    -0.0424950537596,                    5,                    0),
          (        1.0793887971,                    0,                    1),
          (      -1.99243619673,                    1,                    1),
          (     -0.155135133506,                    2,                    1),
          (     -0.121465790553,                    3,                    1),
          (    -0.0165038582393,                    4,                    1),
          (     -0.186915808643,                    0,                    2),
          (      0.308074612567,                    1,                    2),
          (      0.115861416115,                    2,                    2),
          (     0.0276358316589,                    0,                    3),
          (      0.108043243088,                    1,                    3),
          (     0.0460683793064,                    0,                    4),
          (     -0.174821616881,                    1,                    4),
          (     0.0317530854287,                    1,                    5),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (      0.340776521414,                    3,                    0,          -1.21103865,                    2),
          (      -0.32300139842,                    4,                    0,          -1.21103865,                    2),
          (     0.0424950537596,                    5,                    0,          -1.21103865,                    2),
          (      -1.66940100976,                    3,                    2,          -1.21103865,                    2),
          (       4.08693082002,                    4,                    2,          -1.21103865,                    2),
          (      -2.41738963889,                    5,                    2,          -1.21103865,                    2),
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
          (             0.43023,                0.095),
          (              22.722,                 0.93),
          (             -27.118,                  1.1),
          (              13.247,                    2),
          (             -9.0529,                    3),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (            -0.46609,                 0.09),
          (             -6.8355,                 0.76),
          (             -167.15,                    4),
          (               15805,                  6.5),
          (              -31859,                    7),
          (               21548,                    8),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.2195,                    1),
          (              1.6357,                  1.5),
          (             -1.4576,                  2.2),
          (              -6.958,                  4.8),
          (              5.7181,                  6.2),
      };

      #endregion Saturated densities and pressure

    }
  }
}

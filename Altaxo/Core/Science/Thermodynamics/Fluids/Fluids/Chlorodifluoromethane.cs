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
  /// State equations and constants of chlorodifluoromethane.
  /// Short name: R22.
  /// Synomym: HCFC-22.
  /// Chemical formula: CHClF2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'r22.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Kamei, A., Beyerlein, S.W., and Jacobsen, R.T, "Application of nonlinear regression in the development of a wide range formulation for HCFC-22," Int. J. Thermophysics, 16:1155-1164, 1995.\</para>
  /// <para>HeatCapacity (CPP): Kamei, A., Beyerlein, S.W., and Jacobsen, R.T, "Application of nonlinear regression in the development of a wide range formulation for HCFC-22," Int. J. Thermophysics, 16:1155-1164, 1995.\</para>
  /// <para>Saturated vapor pressure: Cullimore, I.D., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("75-45-6")]
  public class Chlorodifluoromethane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Chlorodifluoromethane Instance { get; } = new Chlorodifluoromethane();

    #region Constants for chlorodifluoromethane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "chlorodifluoromethane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "R22";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "HCFC-22";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CHClF2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "halocb";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "75-45-6";

    private int[] _unNumbers = new int[] { 1018, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.31451;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.086468; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 369.295;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 4990000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 6058.22;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 115.73;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.3793;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 19906.5340608493;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.000394362112491353;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 232.33952525944;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.22082;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.458;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 115.73;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 550;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 19910;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 60000000;

    #endregion Constants for chlorodifluoromethane

    private Chlorodifluoromethane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -11.8534209226296;
      _alpha0_n_tau = 8.0868949821073;
      _alpha0_n_lntau = 3.00526140446;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          ( -0.0222800387550675,                   -1),
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (                   1,     11.7854547177731),
          (                   1,     5.24014432905942),
          (                   1,     5.11157573213826),
          (                   1,     4.58950930827658),
          (                   1,     4.34795618678834),
          (                   1,     3.14798261552417),
          (                   1,     2.32202678075793),
          (                   1,     1.64022361526693),
          (                   1,     1.43763067466389),
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
          (     0.0695645445236,                   -1,                    1),
          (       25.2275419999,                 1.75,                    1),
          (      -202.351148311,                 2.25,                    1),
          (       350.063090302,                  2.5,                    1),
          (      -223.134648863,                 2.75,                    1),
          (       48.8345904592,                    3,                    1),
          (     0.0108874958556,                  5.5,                    1),
          (      0.590315073614,                  1.5,                    2),
          (     -0.689043767432,                 1.75,                    2),
          (      0.284224445844,                  3.5,                    2),
          (      0.125436457897,                    1,                    3),
          (    -0.0113338666416,                  4.5,                    3),
          (     -0.063138895917,                  1.5,                    4),
          (    0.00974021015232,                  0.5,                    5),
          (  -0.000408406844722,                  4.5,                    6),
          (    0.00074194877357,                    1,                    7),
          (   0.000315912525922,                    4,                    7),
          (   8.76009723338E-06,                    5,                    7),
          (  -0.000110343340301,                 -0.5,                    8),
          (  -7.05323356879E-05,                  3.5,                    8),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (       0.23585073151,                    5,                    2,                   -1,                    2),
          (     -0.192640494729,                    7,                    2,                   -1,                    2),
          (    0.00375218008557,                   12,                    2,                   -1,                    2),
          (  -4.48926036678E-05,                   15,                    2,                   -1,                    2),
          (     0.0198120520635,                  3.5,                    3,                   -1,                    3),
          (    -0.0356958425255,                  3.5,                    4,                   -1,                    2),
          (     0.0319594161562,                    8,                    4,                   -1,                    2),
          (   2.60284291078E-06,                   15,                    4,                   -1,                    2),
          (   -0.00897629021967,                   25,                    4,                   -1,                    4),
          (     0.0345482791645,                    3,                    6,                   -1,                    2),
          (   -0.00411831711251,                    9,                    6,                   -1,                    2),
          (    0.00567428536529,                   19,                    6,                   -1,                    4),
          (   -0.00563368989908,                    2,                    8,                   -1,                    2),
          (    0.00191384919423,                    7,                    8,                   -1,                    2),
          (   -0.00178930036389,                   13,                    8,                   -1,                    4),
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
          (              1.8762,                0.345),
          (             0.68216,                 0.74),
          (            0.041342,                  1.2),
          (             0.22589,                  2.6),
          (             0.15407,                  7.2),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -2.3231,                0.353),
          (             -5.9231,                 1.06),
          (             -16.331,                  2.9),
          (             -49.343,                  6.4),
          (             -25.662,                   12),
          (             -89.335,                   15),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (              -7.078,                    1),
          (              1.7211,                  1.5),
          (             -1.6379,                  2.2),
          (             -3.7952,                  4.8),
          (             0.86937,                  6.2),
      };

      #endregion Saturated densities and pressure

    }
  }
}

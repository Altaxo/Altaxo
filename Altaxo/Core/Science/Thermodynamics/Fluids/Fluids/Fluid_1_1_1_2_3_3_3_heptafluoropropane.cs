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
  /// State equations and constants of 1,1,1,2,3,3,3-heptafluoropropane.
  /// Short name: R227ea.
  /// Synomym: HFC-227ea.
  /// Chemical formula: CF3CHFCF3.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'r227ea.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Lemmon, E.W. and Span, R." Thermodynamic Properties of R-227ea, R-365mfc, R-115, and R-13I1," J. Chem. Eng. Data, 60(12):3745-3758, 2015.</para>
  /// <para>HeatCapacity (CPP): see EOS</para>
  /// <para>Saturated vapor pressure: Cullimore, I.D., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("431-89-0")]
  public class Fluid_1_1_1_2_3_3_3_heptafluoropropane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Fluid_1_1_1_2_3_3_3_heptafluoropropane Instance { get; } = new Fluid_1_1_1_2_3_3_3_heptafluoropropane();

    #region Constants for 1,1,1,2,3,3,3-heptafluoropropane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "1,1,1,2,3,3,3-heptafluoropropane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "R227ea";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "HFC-227ea";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CF3CHFCF3";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "halocb";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "431-89-0";

    private int[] _unNumbers = new int[] { 3296, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.17002886; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 374.9;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 2925000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 3495;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 146.35;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 7.331;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 11046.7058998001;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.00602534818349834;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 256.809045234864;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.357;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.456;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 200;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 475;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 11050;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 60000000;

    #endregion Constants for 1,1,1,2,3,3,3-heptafluoropropane

    private Fluid_1_1_1_2_3_3_3_heptafluoropropane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -15.8290880642424;
      _alpha0_n_tau = 11.087938006383;
      _alpha0_n_lntau = 3;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (               11.43,     1.07495332088557),
          (               12.83,     3.80901573753001),
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
          (            2.024341,                 0.34,                    1),
          (            -2.60593,                 0.77,                    1),
          (           0.4957216,                 0.36,                    2),
          (           -0.824082,                  0.9,                    2),
          (          0.06543703,                    1,                    4),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (            -1.02461,                 2.82,                    1,                   -1,                    1),
          (           0.6247065,                  2.1,                    3,                   -1,                    1),
          (           0.2997521,                  0.9,                    6,                   -1,                    1),
          (           -0.353917,                 1.13,                    6,                   -1,                    1),
          (           -1.232043,                  3.8,                    2,                   -1,                    2),
          (          -0.8824483,                 2.75,                    3,                   -1,                    2),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (           0.1349661,                  1.5,                    1,                -0.83,                -1.72,                0.414,                 1.13),
          (          -0.2662928,                  2.5,                    2,                -2.19,                 -5.2,                1.051,                 0.71),
          (           0.1764733,                  2.5,                    1,                -2.44,                -2.31,                1.226,                  1.2),
          (          0.01536163,                  5.4,                    1,                -3.65,                -1.02,                  1.7,                  1.7),
          (        -0.004667185,                    4,                    4,                -8.88,                -5.63,                0.904,                0.546),
          (           -11.70854,                    1,                    2,                -8.23,                -50.9,                 1.42,                0.896),
          (           0.9114512,                  3.5,                    1,                -2.01,                -1.56,                0.926,                0.747),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              2.0032,                0.345),
          (             0.49235,                 0.74),
          (             0.13738,                  1.2),
          (             0.21057,                  2.6),
          (            -0.12834,                  7.2),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              -2.135,                0.324),
          (             -6.8425,                 1.03),
          (             -21.447,                    3),
          (             -204.57,                  7.4),
          (              517.95,                    9),
          (             -459.08,                   10),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.7961,                    1),
          (              2.1366,                  1.5),
          (             -2.6023,                  2.2),
          (             -5.7444,                  4.8),
          (              2.3982,                  6.2),
      };

      #endregion Saturated densities and pressure

    }
  }
}

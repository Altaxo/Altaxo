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
  /// State equations and constants of 1-chloro-1,2,2,2-tetrafluoroethane.
  /// Short name: R124.
  /// Synomym: HCFC-124.
  /// Chemical formula: CHClFCF3.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'r124.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): de Vries, B., Tillner-Roth, R., and Baehr, H.D., "Thermodynamic Properties of HCFC 124," 19th International Congress of Refrigeration, The Hague, The Netherlands, International Institute of Refrigeration, IVa:582-589, 1995.</para>
  /// <para>HeatCapacity (CPP): de Vries, B., Tillner-Roth, R., and Baehr, H.D., "Thermodynamic Properties of HCFC 124," 19th International Congress of Refrigeration, The Hague, The Netherlands, International Institute of Refrigeration, IVa:582-589, 1995.</para>
  /// <para>Saturated vapor pressure: Cullimore, I.D., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("2837-89-0")]
  public class Fluid_1_chloro_1_2_2_2_tetrafluoroethane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Fluid_1_chloro_1_2_2_2_tetrafluoroethane Instance { get; } = new Fluid_1_chloro_1_2_2_2_tetrafluoroethane();

    #region Constants for 1-chloro-1,2,2,2-tetrafluoroethane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "1-chloro-1,2,2,2-tetrafluoroethane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "R124";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "HCFC-124";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CHClFCF3";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "halocb";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "2837-89-0";

    private int[] _unNumbers = new int[] { 1021, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314471;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.136475; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 395.425;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3624295;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 4103.3156;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 74;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 3.228E-11;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = double.NaN;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = double.NaN;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 261.186919405357;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.2881;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.469;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 120;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 470;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 13575.8;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 40000000;

    #endregion Constants for 1-chloro-1,2,2,2-tetrafluoroethane

    private Fluid_1_chloro_1_2_2_2_tetrafluoroethane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -11.6694057098269;
      _alpha0_n_tau = 9.87604431058531;
      _alpha0_n_lntau = 2.175638;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          (           -7.389735,                   -1),
          (           0.8736831,                   -2),
          (          -0.1115133,                   -3),
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
          (         -0.01262962,                    2,                    1),
          (            2.168373,                  0.5,                    1),
          (           -3.330033,                    1,                    1),
          (           0.1610361,                  0.5,                    2),
          (       -9.666145E-05,                  2.5,                    2),
          (           0.0119131,                   -1,                    3),
          (        -0.002880217,                    1,                    5),
          (         0.001681346,                    0,                    6),
          (        1.594968E-05,                 -0.5,                    8),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (           0.1289674,                  1.5,                    2,                   -1,                    1),
          (        1.182213E-05,                    1,                   12,                   -1,                    1),
          (          -0.4713997,                  2.5,                    1,                   -1,                    2),
          (          -0.2412873,                -0.25,                    1,                   -1,                    2),
          (           0.6868066,                    1,                    1,                   -1,                    2),
          (         -0.08621095,                    5,                    1,                   -1,                    2),
          (        4.728645E-06,                    2,                   15,                   -1,                    2),
          (          0.01487933,                   15,                    3,                   -1,                    3),
          (         -0.03001338,                   20,                    3,                   -1,                    3),
          (         0.001849606,                   15,                    4,                   -1,                    4),
          (        0.0004126073,                   45,                    9,                   -1,                    4),
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
          (              1.9127,                0.345),
          (             0.67778,                 0.74),
          (           -0.035129,                  1.2),
          (             0.30407,                  2.6),
          (            0.069503,                  7.2),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -2.8551,                0.388),
          (              -6.385,                 1.17),
          (             -17.616,                    3),
          (             -37.828,                    6),
          (             -23.785,                    8),
          (             -134.59,                   15),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.5146,                    1),
          (              3.7481,                  1.5),
          (             -3.0124,                 1.68),
          (             -3.7808,                  3.8),
          (            -0.53114,                    8),
      };

      #endregion Saturated densities and pressure

    }
  }
}

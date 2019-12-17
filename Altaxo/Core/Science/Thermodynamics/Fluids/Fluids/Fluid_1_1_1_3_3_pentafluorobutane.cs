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
  /// State equations and constants of 1,1,1,3,3-pentafluorobutane.
  /// Short name: R365mfc.
  /// Synomym: HFC-365mfc.
  /// Chemical formula: CF3CH2CF2CH3.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'r365mfc.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Lemmon, E.W. and Span, R.Thermodynamic Properties of R-227ea, R-365mfc, R-115, and R-13I1J. Chem. Eng. Data, 60(12):3745-3758, 2015.</para>
  /// <para>HeatCapacity (CPP): see EOS</para>
  /// <para>Saturated vapor pressure: Cullimore, I.D., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("406-58-6")]
  public class Fluid_1_1_1_3_3_pentafluorobutane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Fluid_1_1_1_3_3_pentafluorobutane Instance { get; } = new Fluid_1_1_1_3_3_pentafluorobutane();

    #region Constants for 1,1,1,3,3-pentafluorobutane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "1,1,1,3,3-pentafluorobutane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "R365mfc";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "HFC-365mfc";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CF3CH2CF2CH3";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "halocb";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "406-58-6";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.14807452; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 460;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3266000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 3200;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 239;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 2478;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 9298.15710146645;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 1.25117271637299;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 313.343035787337;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.377;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 3.807;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 239;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 500;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 9300;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 35000000;

    #endregion Constants for 1,1,1,3,3-pentafluorobutane

    private Fluid_1_1_1_3_3_pentafluorobutane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -16.3423492459187;
      _alpha0_n_tau = 10.2889618649051;
      _alpha0_n_lntau = 3;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (               17.47,     1.23695652173913),
          (               16.29,     4.85217391304348),
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
          (             2.20027,                 0.24,                    1),
          (             -2.8624,                 0.67,                    1),
          (            0.384559,                  0.5,                    2),
          (           -0.621227,                 1.25,                    2),
          (           0.0665967,                    1,                    4),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (            -1.19383,                 3.35,                    1,                   -1,                    1),
          (            0.635935,                  2.5,                    3,                   -1,                    1),
          (            0.461728,                 0.96,                    6,                   -1,                    1),
          (           -0.533472,                 1.07,                    6,                   -1,                    1),
          (            -1.07101,                  5.6,                    2,                   -1,                    2),
          (             0.13929,                  6.9,                    3,                   -1,                    2),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (           -0.385506,                    3,                    1,                -0.97,                -1.07,                 1.48,                 1.02),
          (            0.885653,                  3.6,                    1,                -0.94,                -1.08,                 1.49,                 0.62),
          (            0.226303,                    5,                    1,                -2.15,                -10.9,                 1.01,                 0.53),
          (           -0.166116,                 1.25,                    2,                -2.66,                -22.6,                 1.16,                 0.48),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              1.7933,                 0.31),
          (             -1.8792,                  0.6),
          (              9.0006,                  0.9),
          (             -11.669,                  1.2),
          (              5.6329,                  1.5),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              -1.612,                0.281),
          (             -6.7679,                 0.91),
          (             -24.499,                    3),
          (              3.3398,                    5),
          (              -211.1,                    8),
          (              258.07,                   10),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -8.0955,                    1),
          (              2.0414,                  1.5),
          (             -13.333,                  3.4),
          (              25.514,                  4.3),
          (             -19.967,                    5),
      };

      #endregion Saturated densities and pressure

    }
  }
}

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
  /// State equations and constants of 2,3,3,3-tetrafluoroprop-1-ene.
  /// Short name: R1234yf.
  /// Synomym: R-1234yf.
  /// Chemical formula: CF3CF=CH2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'r1234yf.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Richter, M., McLinden, M.O., and Lemmon, E.W. "Thermodynamic Properties of 2,3,3,3-Tetrafluoroprop-1-ene (R1234yf): Vapor Pressure and p-rho-T Measurements and an Equation of State," J. Chem. Eng. Data, 56(7):3254-3264, 2011.</para>
  /// <para>Saturated vapor pressure: Lemmon, E.W., 2010.</para>
  /// <para>Saturated liquid density: Lemmon, E.W., 2010.</para>
  /// <para>Saturated vapor density: Lemmon, E.W., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("754-12-1")]
  public class Fluid_2_3_3_3_tetrafluoroprop_1_ene : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Fluid_2_3_3_3_tetrafluoroprop_1_ene Instance { get; } = new Fluid_2_3_3_3_tetrafluoroprop_1_ene();

    #region Constants for 2,3,3,3-tetrafluoroprop-1-ene

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "2,3,3,3-tetrafluoroprop-1-ene";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "R1234yf";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "R-1234yf";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CF3CF=CH2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "halocb";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "754-12-1";

    private int[] _unNumbers = new int[] { 3161, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.1140415928; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 367.85;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3382200;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 4170;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 220;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 31500;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 11633.1073088325;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 17.5835255519606;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 243.664873779363;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.276;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 2.48;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 220;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 410;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 11640;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 30000000;

    #endregion Constants for 2,3,3,3-tetrafluoroprop-1-ene

    private Fluid_2_3_3_3_tetrafluoroprop_1_ene()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -12.8379284042223;
      _alpha0_n_tau = 8.04260467494662;
      _alpha0_n_lntau = 4.944;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (               7.549,     1.95188256082642),
          (               1.537,     2.38412396357211),
          (                2.03,     12.1380997689276),
          (               7.455,     4.77096642653255),
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
          (          0.04592563,                    1,                    4),
          (            1.546958,                 0.32,                    1),
          (           -2.355237,                0.929,                    1),
          (          -0.4827835,                 0.94,                    2),
          (           0.1758022,                 0.38,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (           -1.210006,                 2.28,                    1,                   -1,                    2),
          (          -0.6177084,                 1.76,                    3,                   -1,                    2),
          (           0.6805262,                 0.97,                    2,                   -1,                    1),
          (          -0.6968555,                 2.44,                    2,                   -1,                    2),
          (         -0.02695779,                 1.05,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (            1.389966,                  1.4,                    1,                -1.02,                -1.42,                 1.13,                0.712),
          (          -0.4777136,                    3,                    1,               -1.336,                -2.31,                 0.67,                 0.91),
          (          -0.1975184,                  3.5,                    3,               -1.055,                -0.89,                 0.46,                0.677),
          (           -1.147646,                    1,                    3,                -5.84,                  -80,                 1.28,                0.718),
          (        0.0003428541,                  3.5,                    2,                -16.2,                 -108,                  1.2,                 1.64),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              1.9083,                 0.32),
          (             -2.1383,                 0.56),
          (              9.3653,                  0.8),
          (             -9.8659,                    1),
          (              3.5859,                  1.3),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -2.3511,                0.355),
          (             -11.515,                 2.45),
          (             -5.3984,                    1),
          (             -37.937,                  5.1),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.4697,                    1),
          (              2.7915,                  1.5),
          (             -2.1312,                  1.8),
          (             -2.9531,                  3.8),
      };

      #endregion Saturated densities and pressure

    }
  }
}

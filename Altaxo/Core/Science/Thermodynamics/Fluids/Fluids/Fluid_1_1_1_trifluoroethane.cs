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
  /// State equations and constants of 1,1,1-trifluoroethane.
  /// Short name: R143a.
  /// Synomym: HFC-143a.
  /// Chemical formula: CF3CH3.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'r143a.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Lemmon, E.W. and Jacobsen, R.T, "An International Standard Formulation for the Thermodynamic Properties of 1,1,1-Trifluoroethane (HFC-143a) for Temperatures from 161 to 450 K and Pressures to 50 MPa," J. Phys. Chem. Ref. Data, 29(4):521-552, 2000.</para>
  /// <para>HeatCapacity (CPP): Lemmon, E.W. and Jacobsen, R.T, "An International Standard Formulation for the Thermodynamic Properties of 1,1,1-Trifluoroethane (HFC-143a) for Temperatures from 161 to 450 K and Pressures to 50 MPa," J. Phys. Chem. Ref. Data, 29(4):521-552, 2000.</para>
  /// <para>Saturated vapor pressure: Cullimore, I.D., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("420-46-2")]
  public class Fluid_1_1_1_trifluoroethane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Fluid_1_1_1_trifluoroethane Instance { get; } = new Fluid_1_1_1_trifluoroethane();

    #region Constants for 1,1,1-trifluoroethane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "1,1,1-trifluoroethane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "R143a";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "HFC-143a";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CF3CH3";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "halocb";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "420-46-2";

    private int[] _unNumbers = new int[] { 2035, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.084041; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 345.857;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3761000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 5128.45;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 161.34;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 1074.9;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 15832.1046985111;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.803616507488145;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 225.909433850125;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.2615;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 2.34;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 161.34;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 650;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 15850;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 100000000;

    #endregion Constants for 1,1,1-trifluoroethane

    private Fluid_1_1_1_trifluoroethane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 5.90304811943166;
      _alpha0_n_tau = 7.30725062149164;
      _alpha0_n_lntau = -1;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          (    -16.591049152079,                -0.33),
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (              4.4402,     5.17844080067774),
          (              3.7515,     2.37959619148955),
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
          (           7.7736443,                 0.67,                    1),
          (            -8.70185,                0.833,                    1),
          (         -0.27779799,                  1.7,                    1),
          (           0.1460922,                 1.82,                    2),
          (        0.0089581616,                 0.35,                    5),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (         -0.20552116,                  3.9,                    1,                   -1,                    1),
          (          0.10653258,                 0.95,                    3,                   -1,                    1),
          (         0.023270816,                    0,                    5,                   -1,                    1),
          (        -0.013247542,                 1.19,                    7,                   -1,                    1),
          (         -0.04279387,                  7.2,                    1,                   -1,                    2),
          (          0.36221685,                  5.9,                    2,                   -1,                    2),
          (         -0.25671899,                 7.65,                    2,                   -1,                    2),
          (        -0.092326113,                  7.5,                    3,                   -1,                    2),
          (         0.083774837,                 7.45,                    4,                   -1,                    2),
          (         0.017128445,                 15.5,                    2,                   -1,                    3),
          (         -0.01725611,                   22,                    3,                   -1,                    3),
          (        0.0049080492,                   19,                    5,                   -1,                    3),
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
          (              2.1135,                0.348),
          (                10.2,                  1.6),
          (             -30.836,                    2),
          (              39.909,                  2.4),
          (             -18.557,                  2.7),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -2.8673,                0.384),
          (             -6.3818,                 1.17),
          (             -16.314,                    3),
          (             -45.947,                  6.2),
          (             -1.3956,                    7),
          (             -246.71,                   15),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.3938,                    1),
          (              1.9948,                  1.5),
          (             -1.8487,                  2.2),
          (             -4.1927,                  4.8),
          (              1.4862,                  6.2),
      };

      #endregion Saturated densities and pressure

    }
  }
}

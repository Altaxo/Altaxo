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
  /// State equations and constants of octamethylcyclotetrasiloxane.
  /// Short name: D4.
  /// Synomym: D4.
  /// Chemical formula: C8H24O4Si4.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'd4.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS):  Thol, M.; Rutkai, G.; K�ster, A.; Dubberke, F.H.; Windmann, T.; Span, R.; Vrabec, J. "Thermodynamic Properties for Octamethylcyclotetrasiloxane" J. Chem. Eng. Data, 61(7):2580-2595, 2016The expected uncertainty of vapor pressure data from the present equation of state is 1.5 % for T = 460 K and 2 % for higher temperatures. Saturated liquid density data are accurate within 0.1 % for T &lt; 360 K and 0.5 % for higher temperatures. The uncertainty of the homogeneous density at atmospheric pressure is assessed to be 0.1 %. The available experimental data in the high pressure region are not consistent with the present speed of sound measurements so that the equation is assumed to be accurate within only 0.7 %. The expected uncertainty of speed of sound data calculated with the present equation of state is 0.5 %.</para>
  /// </remarks>
  [CASRegistryNumber("556-67-2")]
  public class Octamethylcyclotetrasiloxane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Octamethylcyclotetrasiloxane Instance { get; } = new Octamethylcyclotetrasiloxane();

    #region Constants for octamethylcyclotetrasiloxane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "octamethylcyclotetrasiloxane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "D4";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "D4";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C8H24O4Si4";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "556-67-2";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.3144621;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.29661576; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 586.5;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 1347200;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 1043;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 290.25;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 73;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 3235.57918132721;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.0302842428684499;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 448.890570431229;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.596;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.09;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 290.25;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 590;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 3240;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 180000000;

    #endregion Constants for octamethylcyclotetrasiloxane

    private Octamethylcyclotetrasiloxane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 71.163604979299;
      _alpha0_n_tau = -21.6743650975648;
      _alpha0_n_lntau = 3;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (             38.2456,    0.341005967604433),
          (              58.975,      3.0690537084399),
          (            0.292757,   0.0682011935208866),
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
          (          0.05273743,                    1,                    4),
          (            4.176401,                 0.27,                    1),
          (            -4.73707,                 0.51,                    1),
          (           -1.289588,                0.998,                    2),
          (           0.5272749,                 0.56,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (           -2.558391,                 1.75,                    1,                   -1,                    2),
          (          -0.9726737,                 3.09,                    3,                   -1,                    2),
          (           0.7208209,                 0.79,                    2,                   -1,                    1),
          (          -0.4789456,                 2.71,                    2,                   -1,                    2),
          (         -0.05563239,                0.998,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (            3.766589,                 0.93,                    1,               -0.861,                -0.75,                1.124,                0.926),
          (          0.08786997,                 3.17,                    1,               -1.114,                -0.55,                1.388,                  1.3),
          (          -0.1267646,                 1.08,                    3,                -1.01,                   -1,                1.148,                1.114),
          (           -1.004246,                 1.41,                    2,                -1.11,                -0.47,                1.197,                0.996),
          (           -1.641887,                 0.89,                    2,               -1.032,                -1.36,                0.817,                0.483),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              2.7216,                 0.38),
          (             -1.5754,                 0.89),
          (              3.9887,                 1.44),
          (             -3.7683,                 2.06),
          (              1.9445,                 2.78),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              -3.745,                0.416),
          (             -9.2075,                 1.35),
          (             -71.786,                  3.8),
          (              108.85,                  4.8),
          (             -141.61,                  5.8),
          (             -227.19,                   14),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -9.2842,                    1),
          (              3.8173,                  1.5),
          (             -4.4415,                  2.1),
          (             -7.7628,                   15),
          (             -6.9289,                  3.9),
      };

      #endregion Saturated densities and pressure

    }
  }
}

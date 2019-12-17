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
  /// State equations and constants of dimethyl ester carbonic acid.
  /// Short name: dimethyl carbonate.
  /// Synomym: DMC.
  /// Chemical formula: C3H6O3.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'dmc.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Zhou, Y., Wu, J., and Lemmon, E.W. "Thermodynamic Properties of Dimethyl Carbonate," J. Phys. Chem. Ref. Data, 40(043106):1-11, 2011.</para>
  /// <para>Saturated vapor pressure: Herrig, S., 2013.</para>
  /// <para>Saturated liquid density: Herrig, S., 2013.</para>
  /// <para>Saturated vapor density: Herrig, S., 2013.</para>
  /// </remarks>
  [CASRegistryNumber("616-38-6")]
  public class DimethylEsterCarbonicAcid : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static DimethylEsterCarbonicAcid Instance { get; } = new DimethylEsterCarbonicAcid();

    #region Constants for dimethyl ester carbonic acid

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "dimethyl ester carbonic acid";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "dimethyl carbonate";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "DMC";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C3H6O3";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "616-38-6";

    private int[] _unNumbers = new int[] { 1161, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.0900779; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 557;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 4908800;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 4000;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 277.06;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 2226.5;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 12111.2016611721;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.968013025915485;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 363.256084116058;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.346;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0.899;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 277.06;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 600;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 12112;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 60000000;

    #endregion Constants for dimethyl ester carbonic acid

    private DimethylEsterCarbonicAcid()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 4.99164616143909;
      _alpha0_n_tau = -0.170944936430967;
      _alpha0_n_lntau = 8.28421;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (             1.48525,   0.0377019748653501),
          (            0.822585,     2.40574506283662),
          (             16.2453,     3.00179533213645),
          (             1.15925,     13.2764811490126),
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
          (       0.00052683187,                    1,                    5),
          (            1.353396,                0.227,                    1),
          (           -2.649283,                 1.05,                    1),
          (          -0.2785412,                 1.06,                    2),
          (           0.1742554,                  0.5,                    3),
          (         0.031606252,                 0.78,                    4),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (            0.399866,                  1.3,                    1,                   -1,                    1),
          (            1.178144,                1.347,                    2,                   -1,                    1),
          (          -0.0235281,                0.706,                    7,                   -1,                    1),
          (              -1.015,                    2,                    1,                   -1,                    2),
          (          -0.7880436,                  2.5,                    2,                   -1,                    2),
          (            -0.12696,                4.262,                    3,                   -1,                    2),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (              1.2198,                    1,                    1,              -0.9667,                -1.24,               1.2827,               0.6734),
          (             -0.4883,                2.124,                    1,              -1.5154,               -0.821,               0.4317,               0.9239),
          (          -0.0033293,                  0.4,                    2,              -1.0591,               -15.45,               1.1217,               0.8636),
          (          -0.0035387,                  3.5,                    2,              -1.6642,                -2.21,               1.1871,               1.0507),
          (            -0.51172,                  0.5,                    3,             -12.4856,                 -437,               1.1243,               0.8482),
          (            -0.16882,                  2.7,                    3,              -0.9662,               -0.743,               0.4203,               0.7522),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              1.1572,                 0.27),
          (               4.969,                 0.77),
          (             -14.451,                 1.29),
          (              27.569,                 1.85),
          (             -26.223,                 2.46),
          (              10.526,                 3.16),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (            -0.54715,                0.197),
          (            -5.19277,                  0.6),
          (             -94.048,                 2.86),
          (              327.21,                 3.65),
          (            -676.871,                  4.5),
          (             716.072,                  5.4),
          (            -379.799,                  6.4),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -8.3197,                    1),
          (               3.426,                  1.5),
          (             -3.5905,                  2.3),
          (             -3.3194,                  4.7),
      };

      #endregion Saturated densities and pressure

    }
  }
}

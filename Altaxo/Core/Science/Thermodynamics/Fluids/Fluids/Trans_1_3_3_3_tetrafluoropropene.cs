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
  /// State equations and constants of trans-1,3,3,3-tetrafluoropropene.
  /// Short name: R1234ze(E).
  /// Synomym: HFO-1234ze(E).
  /// Chemical formula: CHF=CHCF3 (trans).
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'r1234zee.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Thol, M., Lemmon, E.W. "Equation of State for the Thermodynamic Properties of trans-1,3,3,3-Tetrafluoropropene [R1234ze(E)]," Int. J. Thermophys., 37:28, 2016.</para>
  /// <para>HeatCapacity (CPP): see EOS for reference</para>
  /// <para>Saturated vapor pressure: Thol, M. and Lemmon, E.W., 2013.</para>
  /// <para>Saturated liquid density: Thol, M. and Lemmon, E.W., 2013.</para>
  /// <para>Saturated vapor density: Thol, M. and Lemmon, E.W., 2013.</para>
  /// </remarks>
  [CASRegistryNumber("29118-24-9")]
  public class Trans_1_3_3_3_tetrafluoropropene : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Trans_1_3_3_3_tetrafluoropropene Instance { get; } = new Trans_1_3_3_3_tetrafluoropropene();

    #region Constants for trans-1,3,3,3-tetrafluoropropene

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "trans-1,3,3,3-tetrafluoropropene";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "R1234ze(E)";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "HFO-1234ze(E)";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CHF=CHCF3 (trans)";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "halocb";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "29118-24-9";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.1140416; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 382.513;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3634900;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 4290;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 168.62;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 218.7;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 13255.8957892469;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.156012264021838;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 254.177474975783;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.313;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.27;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 168.62;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 420;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 13260;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 20000000;

    #endregion Constants for trans-1,3,3,3-tetrafluoropropene

    private Trans_1_3_3_3_tetrafluoropropene()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -12.558331205379;
      _alpha0_n_tau = 8.79122122329895;
      _alpha0_n_lntau = 3;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (              9.3575,     1.34113088966911),
          (              10.717,     5.15538034001459),
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
          (          0.03982797,                    1,                    4),
          (            1.812227,                0.223,                    1),
          (           -2.537512,                0.755,                    1),
          (          -0.5333254,                 1.24,                    2),
          (           0.1677031,                 0.44,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (           -1.323801,                    2,                    1,                   -1,                    2),
          (          -0.6694654,                  2.2,                    3,                   -1,                    2),
          (           0.8072718,                  1.2,                    2,                   -1,                    1),
          (          -0.7740229,                  1.5,                    2,                   -1,                    2),
          (         -0.01843846,                  0.9,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (            1.407916,                 1.33,                    1,                   -1,                -1.21,                0.943,                0.728),
          (          -0.4237082,                 1.75,                    1,                -1.61,                -1.37,                0.642,                 0.87),
          (          -0.2270068,                 2.11,                    3,                -1.24,                -0.98,                 0.59,                0.855),
          (           -0.805213,                    1,                    3,                -9.34,                 -171,                  1.2,                 0.79),
          (          0.00994318,                  1.5,                    2,                -5.78,                -47.4,                 1.33,                  1.3),
          (        -0.008798793,                    1,                    1,                -3.08,                -15.4,                 0.64,                 0.71),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              1.1913,                 0.27),
          (              2.2456,                  0.7),
          (             -1.7747,                 1.25),
          (              1.3096,                  1.9),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -1.0308,                 0.24),
          (             -5.0422,                 0.72),
          (               -11.5,                  2.1),
          (             -37.499,                  4.8),
          (             -77.945,                  9.5),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.5888,                    1),
          (              1.9696,                  1.5),
          (             -2.0827,                  2.2),
          (             -4.1238,                  4.6),
      };

      #endregion Saturated densities and pressure

    }
  }
}

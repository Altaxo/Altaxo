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
  /// State equations and constants of undecane.
  /// Short name: undecane.
  /// Synomym: n-undecane.
  /// Chemical formula: CH3-9(CH2)-CH3.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'c11.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Aleksandrov, I.S., Gerasimov, A.A., and Grigor�ev, B.A. "Using Fundamental Equations of State for Calculating the Thermodynamic Properties of Normal Undecane," Thermal Engineering, 58(8):691-698, 2011.</para>
  /// <para>HeatCapacity (CPP): see EOS</para>
  /// <para>Saturated vapor pressure: Herrig, S., 2013.</para>
  /// <para>Saturated liquid density: Herrig, S., 2013.</para>
  /// <para>Saturated vapor density: Herrig, S., 2013.</para>
  /// </remarks>
  [CASRegistryNumber("1120-21-4")]
  public class Undecane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Undecane Instance { get; } = new Undecane();

    #region Constants for undecane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "undecane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "undecane";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "n-undecane";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CH3-9(CH2)-CH3";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "n-alkane";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "1120-21-4";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.15630826; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 638.8;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 1990400;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 1514.9;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 247.541;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.4461;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 4962.01119130481;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.000216723702312062;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 468.934174523336;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.539;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 247.541;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 700;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 4970;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 500000000;

    #endregion Constants for undecane

    private Undecane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 30.0676204994156;
      _alpha0_n_tau = 79.0293507708593;
      _alpha0_n_lntau = -120.4274;
      _alpha0_n_taulntau = -31.8124608641202;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          (    1.41992911725139,                    2),
          (        -136.8378271,                   -1),
          (    28.2770850953387,                   -2),
          (   -3.51533843989463,                   -3),
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
          (         -0.66172706,                  1.5,                    1),
          (           1.3375396,                 0.25,                    1),
          (          -2.5608399,                 1.25,                    1),
          (           0.1067891,                 0.25,                    3),
          (       0.00028873614,                0.875,                    7),
          (         0.049587209,                1.375,                    2),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (       5.5407101E-08,                    0,                    1,                   -1,                    1),
          (          0.99754712,                2.375,                    1,                   -1,                    1),
          (           1.5774025,                    2,                    2,                   -1,                    1),
          (        0.0013108354,                2.125,                    5,                   -1,                    1),
          (         -0.59326961,                  3.5,                    1,                   -1,                    2),
          (        -0.093001876,                  6.5,                    1,                   -1,                    2),
          (         -0.17960228,                 4.75,                    4,                   -1,                    2),
          (        -0.022560853,                 12.5,                    2,                   -1,                    3),
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
          (              4.5273,                 0.46),
          (             -7.5714,                 0.84),
          (               13.92,                 1.25),
          (             -13.464,                  1.7),
          (              5.8411,                  2.2),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -4.3093,                0.466),
          (             -3.4358,                 1.02),
          (             -17.473,                  2.4),
          (             -58.573,                  5.3),
          (             -133.83,                 11.4),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -9.3961,                    1),
          (              4.4531,                  1.5),
          (             -5.2658,                  2.2),
          (             -4.7352,                  4.5),
      };

      #endregion Saturated densities and pressure

    }
  }
}

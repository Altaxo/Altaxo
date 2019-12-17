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
  /// State equations and constants of ethylene oxide.
  /// Short name: ethylene oxide.
  /// Synomym: Oxirane.
  /// Chemical formula: C2H4O.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'etyoxide.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): M. Thol, G. Rutkai, A. K�ster, M. Kortmann, R. Span, J. Vrabec, Corrigendum:"Fundamental Equation of State for Ethylene Oxide Based on a Hybrid Dataset," Chem. Eng. Sci.,134:887-890, 2015.</para>
  /// <para>HeatCapacity (CPP):  see EOS</para>
  /// <para>Saturated vapor pressure: See EOS</para>
  /// <para>Saturated liquid density: See EOS</para>
  /// <para>Saturated vapor density: See EOS</para>
  /// </remarks>
  [CASRegistryNumber("75-21-8")]
  public class EthyleneOxide : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static EthyleneOxide Instance { get; } = new EthyleneOxide();

    #region Constants for ethylene oxide

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "ethylene oxide";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "ethylene oxide";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "Oxirane";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C2H4O";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "halocb";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "75-21-8";

    private int[] _unNumbers = new int[] { 1040, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.3144621;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.04405256; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 468.92;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 7304700;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 7170;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 160.654;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 8.26;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 23670.260563239;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.00618122577189759;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 283.659549107837;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.21;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = -1;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 160.654;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 500;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 23700;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 10000000;

    #endregion Constants for ethylene oxide

    private EthyleneOxide()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -3.90644775159331;
      _alpha0_n_tau = 4.00009559778596;
      _alpha0_n_lntau = 3;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (                6.79,     2.83630470016207),
          (                4.53,     4.62765503710654),
          (                3.68,     9.53254286445449),
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
          (           0.0300676,                    1,                    4),
          (              2.1629,                 0.41,                    1),
          (            -2.72041,                 0.79,                    1),
          (            -0.53931,                 1.06,                    2),
          (            0.181051,                 0.58,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (            -2.61292,                    2,                    1,                   -1,                    2),
          (            -2.08004,                  2.2,                    3,                   -1,                    2),
          (           0.3169968,                 0.73,                    2,                   -1,                    1),
          (             -1.6532,                  2.4,                    2,                   -1,                    2),
          (         -0.01981719,                 0.97,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (             3.34387,                 1.87,                    1,                -1.02,                -0.62,                0.847,                0.705),
          (           -0.950671,                 2.08,                    1,                -1.55,                -1.11,                 0.34,                0.821),
          (           -0.445528,                  2.8,                    3,                -1.44,                -0.62,                0.265,                0.791),
          (        -0.005409938,                 0.97,                    3,                  -14,                 -368,                 1.13,                 1.08),
          (          -0.0638824,                 3.15,                    2,                -1.63,                -0.66,                 0.36,                 1.64),
          (           -0.093912,                  0.7,                    1,                 -1.9,                -1.87,                 1.05,                 1.51),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              2.3014,                0.382),
          (            -0.08549,                 0.93),
          (               2.055,                 1.48),
          (              -2.883,                  2.1),
          (               1.686,                 2.95),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -3.0498,                0.414),
          (             -7.1199,                1.276),
          (             -23.067,                 3.63),
          (              -56.11,                 7.84),
          (              -127.8,                 16.9),
          (              -382.3,                 36.8),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (              -7.002,                    1),
          (              1.1835,                  1.5),
          (              -2.196,                  3.3),
          (              -1.394,                 5.05),
          (              -1.582,                   17),
      };

      #endregion Saturated densities and pressure

    }
  }
}

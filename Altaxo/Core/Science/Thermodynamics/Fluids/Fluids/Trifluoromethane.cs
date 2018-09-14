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
  /// State equations and constants of trifluoromethane.
  /// Short name: R23.
  /// Synomym: HFC-23.
  /// Chemical formula: CHF3.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'r23.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Penoncello, S.G., Lemmon, E.W., Jacobsen, R.T, Shan, Z., "A Fundamental Equation for Triflurormethane (R-23)," J. Phys. Chem. Ref. Data, 32(4):1473-1499, 2003.</para>
  /// <para>HeatCapacity (CPP): Penoncello, S.G., Lemmon, E.W., Jacobsen, R.T, Shan, Z., "A Fundamental Equation for Triflurormethane (R-23)," J. Phys. Chem. Ref. Data, 32(4):1473-1499, 2003.</para>
  /// <para>Saturated vapor pressure: See EOS</para>
  /// <para>Saturated liquid density: See EOS</para>
  /// <para>Saturated vapor density: See EOS</para>
  /// </remarks>
  [CASRegistryNumber("75-46-7")]
  public class Trifluoromethane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Trifluoromethane Instance { get; } = new Trifluoromethane();

    #region Constants for trifluoromethane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "trifluoromethane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "R23";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "HFC-23";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CHF3";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "halocb";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "75-46-7";

    private int[] _unNumbers = new int[] { 1984, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.07001385; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 299.293;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 4832000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 7520;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 118.02;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 58.04;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 24307.7115935144;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.0591764752688104;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 191.132136193344;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.263;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.649;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 118.02;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 475;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 24310;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 120000000;

    #endregion Constants for trifluoromethane

    private Trifluoromethane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -8.3138606425999;
      _alpha0_n_tau = 6.55087253002604;
      _alpha0_n_lntau = 2.999;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (               2.371,     2.48585833948672),
          (               3.237,     4.87482166305259),
          (                2.61,     7.13347789624214),
          (              0.8274,     16.4086697650797),
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
          (            7.041529,                0.744,                    1),
          (           -8.259512,                 0.94,                    1),
          (          0.00805304,                  4.3,                    1),
          (         -0.08617615,                 1.46,                    2),
          (          0.00633341,                 0.68,                    5),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (          -0.1863285,                  4.8,                    1,                   -1,                    1),
          (            0.328051,                  1.5,                    2,                   -1,                    1),
          (           0.5191023,                 2.07,                    3,                   -1,                    1),
          (          0.06916144,                 0.09,                    5,                   -1,                    1),
          (        -0.005045875,                  9.6,                    1,                   -1,                    2),
          (         -0.01744221,                 0.19,                    2,                   -1,                    2),
          (         -0.05003972,                 11.2,                    2,                   -1,                    2),
          (          0.04729813,                 0.27,                    4,                   -1,                    2),
          (         -0.06164031,                  1.6,                    4,                   -1,                    2),
          (          0.01583585,                 10.3,                    4,                   -1,                    2),
          (         -0.00179579,                   14,                    2,                   -1,                    3),
          (        -0.001099007,                   15,                    2,                   -1,                    4),
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
          (              2.2636,                 0.37),
          (             0.47007,                 0.94),
          (              0.2866,                  3.1),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -3.5136,                 0.43),
          (             -7.7491,                  1.4),
          (             -24.871,                  3.7),
          (             -65.637,                    8),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.2631,                    1),
          (               1.314,                  1.5),
          (            -0.78507,                  2.4),
          (             -3.1991,                  3.9),
      };

      #endregion Saturated densities and pressure

    }
  }
}

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
  /// State equations and constants of 2,2,2-trifluoroethyl-difluoromethyl-ether.
  /// Short name: RE245fa2.
  /// Synomym: HFE-245fa2.
  /// Chemical formula: CHF2OCH2CF3.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 're245fa2.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Zhou, Y. and Lemmon, E.W.preliminary equation, 2010.</para>
  /// <para>Saturated vapor pressure: Herrig, S., 2013.</para>
  /// <para>Saturated liquid density: Herrig, S., 2013.</para>
  /// <para>Saturated vapor density: Herrig, S., 2013.</para>
  /// </remarks>
  [CASRegistryNumber("1885-48-9")]
  public class Fluid_2_2_2_trifluoroethyl_difluoromethyl_ether : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Fluid_2_2_2_trifluoroethyl_difluoromethyl_ether Instance { get; } = new Fluid_2_2_2_trifluoroethyl_difluoromethyl_ether();

    #region Constants for 2,2,2-trifluoroethyl-difluoromethyl-ether

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "2,2,2-trifluoroethyl-difluoromethyl-ether";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "RE245fa2";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "HFE-245fa2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CHF2OCH2CF3";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "halocb";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "1885-48-9";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.150047336; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 444.88;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3433000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 3432.258;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 250;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 8272;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 10018.8726358276;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 4.00485259055916;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 302.396436136354;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.387;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.631;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 250;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 500;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 10020;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 400000000;

    #endregion Constants for 2,2,2-trifluoroethyl-difluoromethyl-ether

    private Fluid_2_2_2_trifluoroethyl_difluoromethyl_ether()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -16.5388709018863;
      _alpha0_n_tau = 10.1324267670689;
      _alpha0_n_lntau = 4.259865;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (            12.12843,     1.09242941916921),
          (            13.25677,      3.9606185937781),
          (            0.521867,     17.1529401186837),
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
          (         0.047771378,                    1,                    4),
          (           1.5745383,                 0.32,                    1),
          (          -2.4763491,                 0.91,                    1),
          (         -0.49414564,                1.265,                    2),
          (          0.19380498,               0.4266,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (         -0.97863158,                 2.24,                    1,                   -1,                    2),
          (         -0.42660297,                 1.64,                    3,                   -1,                    2),
          (          0.85352583,                 1.65,                    2,                   -1,                    1),
          (         -0.53380114,                 3.28,                    2,                   -1,                    2),
          (        -0.029780036,                0.855,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (          0.97659111,                1.227,                    1,               -1.005,                   -2,                1.084,                0.723),
          (         -0.33121365,                    3,                    1,               -1.515,                -3.42,                 0.72,               0.9488),
          (         -0.14122591,                  4.3,                    3,               -1.156,                -1.37,                 0.49,                0.818),
          (          -15.312295,                  2.5,                    3,                -17.7,                 -471,                1.152,                0.891),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              1.2479,                 0.34),
          (              5.5732,                 0.75),
          (              -12.26,                  1.2),
          (              13.964,                  1.7),
          (             -6.0384,                  2.3),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              -0.667,                 0.28),
          (             -5.8238,                 0.66),
          (             -26.927,                  2.6),
          (              21.574,                  3.5),
          (             -65.645,                  5.2),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -8.9235,                    1),
          (              10.527,                  1.5),
          (             -23.058,                  1.9),
          (              30.291,                  2.4),
          (             -20.913,                  2.9),
      };

      #endregion Saturated densities and pressure

    }
  }
}

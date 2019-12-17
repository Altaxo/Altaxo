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
  /// State equations and constants of methyl-pentafluoroethyl-ether.
  /// Short name: RE245cb2.
  /// Synomym: HFE-245cb2.
  /// Chemical formula: CF3CF2OCH3.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 're245cb2.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Zhou, Y. and Lemmon, E.W.preliminary equation, 2010.</para>
  /// <para>Saturated vapor pressure: Herrig, S., 2013.</para>
  /// <para>Saturated liquid density: Herrig, S., 2013.</para>
  /// <para>Saturated vapor density: Herrig, S., 2013.</para>
  /// </remarks>
  [CASRegistryNumber("22410-44-2")]
  public class Methyl_pentafluoroethyl_ether : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Methyl_pentafluoroethyl_ether Instance { get; } = new Methyl_pentafluoroethyl_ether();

    #region Constants for methyl-pentafluoroethyl-ether

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "methyl-pentafluoroethyl-ether";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "RE245cb2";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "HFE-245cb2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CF3CF2OCH3";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "halocb";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "22410-44-2";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.150047336; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 406.813;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 2886400;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 3329;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 250;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 27670;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 9330.16947821731;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 13.5885804863711;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 278.755694188439;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.354;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 2.785;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 250;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 500;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 9331;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 20000000;

    #endregion Constants for methyl-pentafluoroethyl-ether

    private Methyl_pentafluoroethyl_ether()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -16.689886198436;
      _alpha0_n_tau = 8.26867584312834;
      _alpha0_n_lntau = 9.196438;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (           10.214789,     2.00091934131898),
          (           10.503071,     4.99246582582169),
          (          0.98682562,     7.47272088158441),
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
          (         0.041453162,                    1,                    4),
          (           1.5010352,                 0.25,                    1),
          (          -2.3142144,                0.786,                    1),
          (           -0.471412,                 1.32,                    2),
          (             0.17182,                0.338,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (            -0.98793,                 2.82,                    1,                   -1,                    2),
          (           -0.392049,                    2,                    3,                   -1,                    2),
          (           0.6848583,                    1,                    2,                   -1,                    1),
          (         -0.32413816,                    3,                    2,                   -1,                    2),
          (         -0.02414796,                0.766,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (          0.82792487,                 1.75,                    1,               -1.023,               -1.727,                  1.1,                0.713),
          (         -0.31833343,                  3.5,                    1,               -1.384,               -1.543,                 0.64,                0.917),
          (         -0.11929747,                 3.86,                    3,               -0.998,               -1.075,                  0.5,                 0.69),
          (         -0.65010212,                 2.75,                    3,                 -6.9,                  -88,                 1.26,                0.743),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              1.8378,                 0.32),
          (              2.5311,                 1.08),
          (              -7.084,                  1.9),
          (              18.678,                  2.8),
          (             -30.228,                  3.8),
          (              22.985,                  4.9),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -1.5224,                0.286),
          (             -5.7245,                 0.82),
          (             -15.972,                  2.5),
          (             -50.473,                  5.6),
          (             -6.8916,                  7.3),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.8026,                    1),
          (              1.8804,                  1.5),
          (             -2.8375,                  2.5),
          (             -4.3077,                    5),
      };

      #endregion Saturated densities and pressure

    }
  }
}

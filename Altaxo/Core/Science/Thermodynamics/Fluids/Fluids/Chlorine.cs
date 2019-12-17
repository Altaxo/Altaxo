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
  /// State equations and constants of chlorine.
  /// Short name: chlorine.
  /// Synomym: chlorine.
  /// Chemical formula: Cl2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'chlorine.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Herrig S., Thol M., Span. R. (2015)</para>
  /// <para>HeatCapacity (CPP): see EOS of Herrig et al. (2015)</para>
  /// <para>Saturated vapor pressure: See EOS</para>
  /// <para>Saturated liquid density: See EOS</para>
  /// <para>Saturated vapor density: See EOS</para>
  /// </remarks>
  [CASRegistryNumber("7782-50-5")]
  public class Chlorine : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Chlorine Instance { get; } = new Chlorine();

    #region Constants for chlorine

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "chlorine";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "chlorine";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "chlorine";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "Cl2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "7782-50-5";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.3144621;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.070906; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 416.8654;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 7642400;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 8060;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 172.17;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 1380.8;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 24601.7136293273;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.965541249477946;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 239.197631606052;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.07;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 172.17;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 440;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 24600;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 20000000;

    #endregion Constants for chlorine

    private Chlorine()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 7.22076220565553;
      _alpha0_n_tau = -2.6476683419817;
      _alpha0_n_lntau = 2.5;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (             0.14068,     19.6706179020854),
          (              1.0256,     1.91908467337419),
          (            0.067756,      7.1965675251532),
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
          (           0.0245017,                    1,                    4),
          (           0.9132904,                0.196,                    1),
          (            -1.72309,                    1,                    1),
          (          -0.3359344,                 1.08,                    2),
          (           0.1200495,                 0.39,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (           -1.214889,                 1.64,                    1,                   -1,                    2),
          (            -0.10167,                  3.2,                    3,                   -1,                    2),
          (           0.6196819,                 1.32,                    2,                   -1,                    1),
          (          -0.6578512,                2.163,                    2,                   -1,                    2),
          (        -0.009159452,                 0.93,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (            1.909418,                0.872,                    1,               -0.969,                -1.22,                1.142,                 0.88),
          (         -0.07163412,                 2.08,                    1,                -1.89,                 -6.8,                 1.22,                 0.73),
          (          -0.1893345,                  1.6,                    3,                -1.32,                 -3.5,                1.552,                 0.28),
          (          -0.5698469,                 1.37,                    2,               -1.012,               -1.276,                1.135,                0.863),
          (          -0.8964496,                 1.05,                    2,                -0.98,                 -1.6,                0.754,                0.554),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              0.9662,                0.234),
          (              1.7744,                 0.68),
          (            -0.23081,                  1.3),
          (             0.47213,                 3.35),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -1.7673,                  0.3),
          (              -5.173,                0.994),
          (             -12.539,                  2.7),
          (             -37.552,                6.155),
          (             -151.49,                   24),
          (             -64.404,                 12.4),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -6.1289,                    1),
          (              1.5112,                  1.5),
          (             -1.4523,                    2),
          (             -5.6038,                 5.94),
          (             -1.2651,                 14.8),
          (              3.9923,                    7),
      };

      #endregion Saturated densities and pressure

    }
  }
}

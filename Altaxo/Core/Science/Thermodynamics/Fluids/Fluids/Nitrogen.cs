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
  /// State equations and constants of nitrogen.
  /// Short name: nitrogen.
  /// Synomym: R-728.
  /// Chemical formula: N2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'nitrogen.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Span, R., Lemmon, E.W., Jacobsen, R.T, Wagner, W., and Yokozeki, A."A Reference Equation of State for the Thermodynamic Properties of Nitrogen for Temperatures from 63.151 to 1000 K and Pressures to 2200 MPa," J. Phys. Chem. Ref. Data, 29(6):1361-1433, 2000.</para>
  /// <para>HeatCapacity (CPP): Span, R., Lemmon, E.W., Jacobsen, R.T, Wagner, W., and Yokozeki, A."A Reference Equation of State for the Thermodynamic Properties of Nitrogen for Temperatures from 63.151 to 1000 K and Pressures to 2200 MPa," J. Phys. Chem. Ref. Data, 29(6):1361-1433, 2000.</para>
  /// <para>Melting pressure: Span, R., Lemmon, E.W., Jacobsen, R.T, Wagner, W., and Yokozeki, A."A Reference Equation of State for the Thermodynamic Properties of Nitrogen for Temperatures from 63.151 to 1000 K and Pressures to 2200 MPa," J. Phys. Chem. Ref. Data, 29(6):1361-1433, 2000.</para>
  /// <para>Sublimation pressure: Lemmon, E.W., 1999.</para>
  /// <para>Saturated vapor pressure: See EOS</para>
  /// <para>Saturated liquid density: See EOS</para>
  /// <para>Saturated vapor density: See EOS</para>
  /// </remarks>
  [CASRegistryNumber("7727-37-9")]
  public class Nitrogen : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Nitrogen Instance { get; } = new Nitrogen();

    #region Constants for nitrogen

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "nitrogen";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "nitrogen";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "R-728";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "N2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "7727-37-9";

    private int[] _unNumbers = new int[] { 1066, 1977, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.31451;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.02801348; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 126.192;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3395800;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 11183.9;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 63.151;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 12519.8;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 30957.3062207036;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 24.06956131607;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 77.3549950205423;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.0372;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 63.151;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 2000;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 53150;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 2200000000;

    #endregion Constants for nitrogen

    private Nitrogen()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -12.7695270804628;
      _alpha0_n_tau = -0.00784162968060448;
      _alpha0_n_lntau = 2.5;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          (  -0.000193481928024,                   -1),
          (-1.24774207237786E-05,                   -2),
          (6.67832625326269E-08,                   -3),
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (            1.012941,     26.6578784709015),
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
          (      0.924803575275,                 0.25,                    1),
          (     -0.492448489428,                0.875,                    1),
          (      0.661883336938,                  0.5,                    2),
          (      -1.92902649201,                0.875,                    2),
          (    -0.0622469309629,                0.375,                    3),
          (      0.349943957581,                 0.75,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (      0.564857472498,                  0.5,                    1,                   -1,                    1),
          (      -1.61720005987,                 0.75,                    1,                   -1,                    1),
          (     -0.481395031883,                    2,                    1,                   -1,                    1),
          (      0.421150636384,                 1.25,                    3,                   -1,                    1),
          (    -0.0161962230825,                  3.5,                    3,                   -1,                    1),
          (      0.172100994165,                    1,                    4,                   -1,                    1),
          (    0.00735448924933,                  0.5,                    6,                   -1,                    1),
          (     0.0168077305479,                    3,                    6,                   -1,                    1),
          (   -0.00107626664179,                    0,                    7,                   -1,                    1),
          (    -0.0137318088513,                 2.75,                    7,                   -1,                    1),
          (   0.000635466899859,                 0.75,                    8,                   -1,                    1),
          (    0.00304432279419,                  2.5,                    8,                   -1,                    1),
          (    -0.0435762336045,                    4,                    1,                   -1,                    2),
          (    -0.0723174889316,                    6,                    2,                   -1,                    2),
          (     0.0389644315272,                    6,                    3,                   -1,                    2),
          (     -0.021220136391,                    3,                    4,                   -1,                    2),
          (    0.00408822981509,                    3,                    5,                   -1,                    2),
          (  -5.51990017984E-05,                    6,                    8,                   -1,                    2),
          (    -0.0462016716479,                   16,                    4,                   -1,                    3),
          (   -0.00300311716011,                   11,                    5,                   -1,                    3),
          (     0.0368825891208,                   15,                    5,                   -1,                    3),
          (    -0.0025585684622,                   12,                    8,                   -1,                    3),
          (    0.00896915264558,                   12,                    3,                   -1,                    4),
          (    -0.0044151337035,                    7,                    5,                   -1,                    4),
          (    0.00133722924858,                    4,                    6,                   -1,                    4),
          (   0.000264832491957,                   16,                    9,                   -1,                    4),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (       19.6688194015,                    0,                    1,                  -20,                 -325,                 1.16,                    1),
          (       -20.911560073,                    1,                    1,                  -20,                 -325,                 1.16,                    1),
          (     0.0167788306989,                    2,                    3,                  -15,                 -300,                 1.13,                    1),
          (       2627.67566274,                    3,                    2,                  -25,                 -275,                 1.25,                    1),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 4;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (          1.48654237,               0.9882),
          (        -0.280476066,                    2),
          (        0.0894143085,                    8),
          (        -0.119879866,                 17.5),
      };

      _saturatedVaporDensity_Type = 6;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (         -1.70127164,                 1.02),
          (         -3.70402649,                  2.5),
          (          1.29859383,                  3.5),
          (        -0.561424977,                  6.5),
          (         -2.68505381,                   14),
      };

      _saturatedVaporPressure_Type = 6;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (         -6.12445284,                    2),
          (           1.2632722,                    3),
          (        -0.765910082,                    5),
          (         -1.77570564,                   10),
      };

      #endregion Saturated densities and pressure

      #region Sublimation pressure

      _sublimationPressure_Type = 3;
      _sublimationPressure_ReducingTemperature = 63.151;
      _sublimationPressure_ReducingPressure = 12523;
      _sublimationPressure_PolynomialCoefficients1 = new (double factor, double exponent)[]
      {
      };

      _sublimationPressure_PolynomialCoefficients2 = new (double factor, double exponent)[]
      {
          (          -13.088692,                    1),
      };

      #endregion Sublimation pressure

      #region Melting pressure

      _meltingPressure_Type = '1';
      _meltingPressure_ReducingTemperature = 63.151;
      _meltingPressure_ReducingPressure = 12523;
      _meltingPressure_Coefficients = new (double factor, double exponent)[][]
      {
      new (double factor, double exponent)[]
        {
            (                   1,                    0),
            (            12798.61,              1.78963),
            (           -12798.61,                    0),
        },

      new (double factor, double exponent)[]{},
      new (double factor, double exponent)[]{},
      };

      #endregion Melting pressure

    }
  }
}

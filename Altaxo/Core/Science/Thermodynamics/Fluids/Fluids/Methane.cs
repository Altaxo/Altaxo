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
  /// State equations and constants of methane.
  /// Short name: methane.
  /// Synomym: R-50.
  /// Chemical formula: CH4.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'methane.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Setzmann, U. and Wagner, W., "A New Equation of State and Tables of Thermodynamic Properties for Methane Covering the Range from the Melting Line to 625 K at Pressures up to 1000 MPa," J. Phys. Chem. Ref. Data, 20(6):1061-1151, 1991.</para>
  /// <para>HeatCapacity (CPP): Setzmann, U. and Wagner, W., "A New Equation of State and Tables of Thermodynamic Properties for Methane Covering the Range from the Melting Line to 625 K at Pressures up to 1000 MPa," J. Phys. Chem. Ref. Data, 20(6):1061-1151, 1991.</para>
  /// <para>Melting pressure: Setzmann, U. and Wagner, W., "A New Equation of State and Tables of Thermodynamic Properties for Methane Covering the Range from the Melting Line to 625 K at Pressures up to 1000 MPa," J. Phys. Chem. Ref. Data, 20(6):1061-1151, 1991.</para>
  /// <para>Sublimation pressure: Lemmon, E.W., 2002.</para>
  /// <para>Saturated vapor pressure: See EOS</para>
  /// <para>Saturated liquid density: See EOS</para>
  /// <para>Saturated vapor density: See EOS</para>
  /// </remarks>
  [CASRegistryNumber("74-82-8")]
  public class Methane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Methane Instance { get; } = new Methane();

    #region Constants for methane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "methane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "methane";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "R-50";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CH4";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "n-alkane";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "74-82-8";

    private int[] _unNumbers = new int[] { 1971, 1972, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.31451;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.0160428; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 190.564;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 4599200;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 10139.128;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 90.6941;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 11696;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 28141.9147926592;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 15.6296918097174;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 111.667205474962;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.01142;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 90.6941;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 625;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 40072;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 1000000000;

    #endregion Constants for methane

    private Methane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -2.97054966671961;
      _alpha0_n_tau = 2.89074538305258;
      _alpha0_n_lntau = 3.0016;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (            0.008449,     3.40043240066329),
          (              4.6942,     10.2695157532378),
          (              3.4865,     20.4393274700363),
          (              1.6572,     29.9374488360866),
          (              1.4115,     79.1335194475347),
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
          (       0.04367901028,                 -0.5,                    1),
          (        0.6709236199,                  0.5,                    1),
          (        -1.765577859,                    1,                    1),
          (        0.8582330241,                  0.5,                    2),
          (        -1.206513052,                    1,                    2),
          (         0.512046722,                  1.5,                    2),
          (    -0.0004000010791,                  4.5,                    2),
          (      -0.01247842423,                    0,                    3),
          (       0.03100269701,                    1,                    4),
          (      0.001754748522,                    3,                    4),
          (    -3.171921605E-06,                    1,                    8),
          (     -2.24034684E-06,                    3,                    9),
          (     2.947056156E-07,                    3,                   10),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (        0.1830487909,                    0,                    1,                   -1,                    1),
          (        0.1511883679,                    1,                    1,                   -1,                    1),
          (       -0.4289363877,                    2,                    1,                   -1,                    1),
          (       0.06894002446,                    0,                    2,                   -1,                    1),
          (      -0.01408313996,                    0,                    4,                   -1,                    1),
          (       -0.0306305483,                    2,                    5,                   -1,                    1),
          (      -0.02969906708,                    2,                    6,                   -1,                    1),
          (      -0.01932040831,                    5,                    1,                   -1,                    2),
          (       -0.1105739959,                    5,                    2,                   -1,                    2),
          (       0.09952548995,                    5,                    3,                   -1,                    2),
          (      0.008548437825,                    2,                    4,                   -1,                    2),
          (      -0.06150555662,                    4,                    4,                   -1,                    2),
          (      -0.04291792423,                   12,                    3,                   -1,                    3),
          (       -0.0181320729,                    8,                    5,                   -1,                    3),
          (        0.0344590476,                   10,                    5,                   -1,                    3),
          (      -0.00238591945,                   10,                    8,                   -1,                    3),
          (      -0.01159094939,                   10,                    2,                   -1,                    4),
          (       0.06641693602,                   14,                    3,                   -1,                    4),
          (       -0.0237154959,                   12,                    4,                   -1,                    4),
          (      -0.03961624905,                   18,                    4,                   -1,                    4),
          (      -0.01387292044,                   22,                    4,                   -1,                    4),
          (       0.03389489599,                   18,                    5,                   -1,                    4),
          (     -0.002927378753,                   14,                    6,                   -1,                    4),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (     9.324799946E-05,                    2,                    2,                  -20,                 -200,                 1.07,                    1),
          (        -6.287171518,                    0,                    0,                  -40,                 -250,                 1.11,                    1),
          (         12.71069467,                    1,                    0,                  -40,                 -250,                 1.11,                    1),
          (        -6.423953466,                    2,                    0,                  -40,                 -250,                 1.11,                    1),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 3;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (           1.9906389,                0.354),
          (         -0.78756197,                  0.5),
          (         0.036976723,                  2.5),
      };

      _saturatedVaporDensity_Type = 4;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (           -1.880284,                1.062),
          (          -2.8526531,                  2.5),
          (           -3.000648,                  4.5),
          (           -5.251169,                  7.5),
          (          -13.191859,                 12.5),
          (          -37.553961,                 23.5),
      };

      _saturatedVaporPressure_Type = 6;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (           -6.036219,                    2),
          (            1.409353,                    3),
          (          -0.4945199,                    4),
          (           -1.443048,                    9),
      };

      #endregion Saturated densities and pressure

      #region Sublimation pressure

      _sublimationPressure_Type = 3;
      _sublimationPressure_ReducingTemperature = 90.6941;
      _sublimationPressure_ReducingPressure = 11696;
      _sublimationPressure_PolynomialCoefficients1 = new (double factor, double exponent)[]
      {
      };

      _sublimationPressure_PolynomialCoefficients2 = new (double factor, double exponent)[]
      {
          (              -12.84,                    1),
      };

      #endregion Sublimation pressure

      #region Melting pressure

      _meltingPressure_Type = '1';
      _meltingPressure_ReducingTemperature = 90.6941;
      _meltingPressure_ReducingPressure = 11696;
      _meltingPressure_Coefficients = new (double factor, double exponent)[][]
      {
      new (double factor, double exponent)[]
        {
            (                   1,                    0),
            (             24756.8,                 1.85),
            (            -7366.02,                  2.1),
            (            -24756.8,                    0),
            (             7366.02,                    0),
        },

      new (double factor, double exponent)[]{},
      new (double factor, double exponent)[]{},
      };

      #endregion Melting pressure

    }
  }
}

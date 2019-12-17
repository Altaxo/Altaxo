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
  /// State equations and constants of argon.
  /// Short name: argon.
  /// Synomym: R-740.
  /// Chemical formula: Ar.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'argon.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Tegeler, Ch., Span, R., and Wagner, W., "A New Equation of State for Argon Covering the Fluid Region for Temperatures from the Melting Line to 700 K at Pressures up to 1000 MPa," J. Phys. Chem. Ref. Data, 28(3):779-850, 1999.</para>
  /// <para>HeatCapacity (CPP): Tegeler, Ch., Span, R., and Wagner, W., "A New Equation of State for Argon Covering the Fluid Region for Temperatures from the Melting Line to 700 K at Pressures up to 1000 MPa," J. Phys. Chem. Ref. Data, 28(3):779-850, 1999.</para>
  /// <para>Melting pressure: Tegeler, Ch., Span, R., and Wagner, W., "A New Equation of State for Argon Covering the Fluid Region for Temperatures from the Melting Line to 700 K at Pressures up to 1000 MPa," J. Phys. Chem. Ref. Data, 28(3):779-850, 1999.</para>
  /// <para>Sublimation pressure: Lemmon, E.W., 2002.</para>
  /// <para>Saturated vapor pressure: See EOS</para>
  /// <para>Saturated liquid density: See EOS</para>
  /// <para>Saturated vapor density: Equation of Tegeler appears to be wrong, and new equation was fitted here.</para>
  /// </remarks>
  [CASRegistryNumber("7440-37-1")]
  public class Argon : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Argon Instance { get; } = new Argon();

    #region Constants for argon

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "argon";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "argon";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "R-740";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "Ar";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "7440-37-1";

    private int[] _unNumbers = new int[] { 1951, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.31451;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.039948; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 150.687;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 4863000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 13407.42965;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 83.8058;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 68891;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 35465.2741564102;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 101.496365183612;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 87.3021362362265;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = -0.00219;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 83.8058;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 700;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 50650;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 1000000000;

    #endregion Constants for argon

    private Argon()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -10.293814800476;
      _alpha0_n_tau = -0.000341523788149916;
      _alpha0_n_lntau = 1.5;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
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
          (       0.08872230499,                    0,                    1),
          (      0.705148051673,                 0.25,                    1),
          (      -1.68201156541,                    1,                    1),
          (     -0.149090144315,                 2.75,                    1),
          (     -0.120248046009,                    4,                    1),
          (     -0.121649787986,                    0,                    2),
          (      0.400359336268,                 0.25,                    2),
          (     -0.271360626991,                 0.75,                    2),
          (      0.242119245796,                 2.75,                    2),
          (    0.00578895831856,                    0,                    3),
          (    -0.0410973356153,                    2,                    3),
          (     0.0247107615416,                 0.75,                    4),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (     -0.321813917507,                    3,                    1,                   -1,                    1),
          (      0.332300176958,                  3.5,                    1,                   -1,                    1),
          (     0.0310199862873,                    1,                    3,                   -1,                    1),
          (    -0.0307770860024,                    2,                    4,                   -1,                    1),
          (     0.0938911374196,                    4,                    4,                   -1,                    1),
          (     -0.090643210682,                    3,                    5,                   -1,                    1),
          (  -0.000457783492767,                    0,                    7,                   -1,                    1),
          (  -8.26597290252E-05,                  0.5,                   10,                   -1,                    1),
          (   0.000130134156031,                    1,                   10,                   -1,                    1),
          (     -0.011397840002,                    1,                    2,                   -1,                    2),
          (    -0.0244551699605,                    7,                    2,                   -1,                    2),
          (     -0.064324067176,                    5,                    4,                   -1,                    2),
          (     0.0588894710937,                    6,                    4,                   -1,                    2),
          (   -0.00064933552113,                    6,                    8,                   -1,                    2),
          (    -0.0138898621584,                   10,                    3,                   -1,                    3),
          (      0.404898392969,                   13,                    5,                   -1,                    3),
          (     -0.386125195947,                   14,                    5,                   -1,                    3),
          (     -0.188171423322,                   11,                    6,                   -1,                    3),
          (      0.159776475965,                   14,                    6,                   -1,                    3),
          (     0.0539855185139,                    8,                    7,                   -1,                    3),
          (     -0.028953417958,                   14,                    7,                   -1,                    3),
          (    -0.0130254133814,                    6,                    8,                   -1,                    3),
          (    0.00289486967758,                    7,                    9,                   -1,                    3),
          (   -0.00226471343048,                   24,                    5,                   -1,                    4),
          (    0.00176164561964,                   22,                    6,                   -1,                    4),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (    0.00585524544828,                    3,                    2,                  -20,                 -250,                 1.11,                    1),
          (       -0.6925190827,                    1,                    1,                  -20,                 -375,                 1.14,                    1),
          (       1.53154900305,                    0,                    2,                  -20,                 -300,                 1.17,                    1),
          (   -0.00273804474498,                    0,                    3,                  -20,                 -225,                 1.11,                    1),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 3;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (           1.5004264,                0.334),
          (          -0.3138129,      0.6666666666666),
          (         0.086461622,      2.3333333333333),
          (        -0.041477525,                    4),
      };

      _saturatedVaporDensity_Type = 5;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -2.9182,                 0.72),
          (             0.09793,                 1.25),
          (             -1.3721,                 0.32),
          (             -2.2898,                 4.34),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (          -5.9409785,                    1),
          (           1.3553888,                  1.5),
          (          -0.4649761,                    2),
          (          -1.5399043,                  4.5),
      };

      #endregion Saturated densities and pressure

      #region Sublimation pressure

      _sublimationPressure_Type = 3;
      _sublimationPressure_ReducingTemperature = 83.8058;
      _sublimationPressure_ReducingPressure = 68891;
      _sublimationPressure_PolynomialCoefficients1 = new (double factor, double exponent)[]
      {
      };

      _sublimationPressure_PolynomialCoefficients2 = new (double factor, double exponent)[]
      {
          (            -11.1307,                    1),
      };

      #endregion Sublimation pressure

      #region Melting pressure

      _meltingPressure_Type = '1';
      _meltingPressure_ReducingTemperature = 83.8058;
      _meltingPressure_ReducingPressure = 68891;
      _meltingPressure_Coefficients = new (double factor, double exponent)[][]
      {
      new (double factor, double exponent)[]
        {
            (                   1,                    0),
            (         -7476.26651,                 1.05),
            (          9959.06125,                1.275),
            (          7476.26651,                    0),
            (         -9959.06125,                    0),
        },

      new (double factor, double exponent)[]{},
      new (double factor, double exponent)[]{},
      };

      #endregion Melting pressure

    }
  }
}

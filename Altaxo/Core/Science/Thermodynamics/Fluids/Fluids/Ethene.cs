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
  /// State equations and constants of ethene.
  /// Short name: ethylene.
  /// Synomym: R-1150.
  /// Chemical formula: CH2=CH2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'ethylene.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Smukala, J., Span, R. and Wagner, W. "A New Equation of State for Ethylene Covering the Fluid Region for Temperatures from the Melting Line to 450 K at Pressures up to 300 MPa," J. Phys. Chem. Ref. Data, 29(5):1053-1122, 2000.</para>
  /// <para>HeatCapacity (CPP): Smukala, J., Span, R. and Wagner, W. "A New Equation of State for Ethylene Covering the Fluid Region for Temperatures from the Melting Line to 450 K at Pressures up to 300 MPa," J. Phys. Chem. Ref. Data, 29(5):1053-1122, 2000.</para>
  /// <para>Melting pressure: Jahangiri, M., Jacobsen, R.T, Stewart, R.B., and McCarty, R.D., "Thermodynamic properties of ethylene from the freezing line to 450 K at pressures to 260 MPa," J. Phys. Chem. Ref. Data, 15(2):593-734, 1986.</para>
  /// <para>Saturated vapor pressure: See EOS</para>
  /// <para>Saturated liquid density: See EOS</para>
  /// <para>Saturated vapor density: See EOS</para>
  /// </remarks>
  [CASRegistryNumber("74-85-1")]
  public class Ethene : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Ethene Instance { get; } = new Ethene();

    #region Constants for ethene

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "ethene";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "ethylene";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "R-1150";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CH2=CH2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "n-alkene";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "74-85-1";

    private int[] _unNumbers = new int[] { 1962, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.31451;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.02805376; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 282.35;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 5041800;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 7636.76598;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 103.986;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 121.96;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 23333.9418274081;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.141090901368336;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 169.378648433567;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.0866;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 103.986;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 450;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 27030;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 300000000;

    #endregion Constants for ethene

    private Ethene()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -3.5344235665635;
      _alpha0_n_tau = 3.41964352925595;
      _alpha0_n_lntau = 3;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (          2.49395851,        4.43266896051),
          (           3.0027152,      5.7484014910572),
          (           2.5126584,     7.80278250044271),
          (          3.99064217,     15.5851153993271),
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
          (      1.861742910067,                  0.5,                    1),
          (    -3.0913708460844,                    1,                    1),
          (   -0.17384817095516,                  2.5,                    1),
          (    0.08037098569284,                    0,                    2),
          (    0.23682707317354,                    2,                    2),
          (   0.021922786610247,                  0.5,                    4),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (    0.11827885813193,                    1,                    1,                   -1,                    1),
          (  -0.021736384396776,                    4,                    1,                   -1,                    1),
          (   0.044007990661139,                 1.25,                    3,                   -1,                    1),
          (    0.12554058863881,                 2.75,                    4,                   -1,                    1),
          (   -0.13167945577241,                 2.25,                    5,                   -1,                    1),
          ( -0.0052116984575897,                    1,                    7,                   -1,                    1),
          ( 0.00015236081265419,                 0.75,                   10,                   -1,                    1),
          (-2.4505335342756E-05,                  0.5,                   11,                   -1,                    1),
          (    0.28970524924022,                  2.5,                    1,                   -1,                    2),
          (   -0.18075836674288,                  3.5,                    1,                   -1,                    2),
          (    0.15057272878461,                    4,                    2,                   -1,                    2),
          (   -0.14093151754458,                    6,                    2,                   -1,                    2),
          (   0.022755109070253,                  1.5,                    4,                   -1,                    2),
          (   0.014026070529061,                    5,                    4,                   -1,                    2),
          (  0.0061697454296214,                  4.5,                    6,                   -1,                    2),
          (-0.00041286083451333,                   15,                    7,                   -1,                    3),
          (   0.012885388714785,                   20,                    4,                   -1,                    4),
          (  -0.069128692157093,                   23,                    5,                   -1,                    4),
          (    0.10936225568483,                   22,                    6,                   -1,                    4),
          ( -0.0081818875271794,                   29,                    6,                   -1,                    4),
          (   -0.05641847211717,                   19,                    7,                   -1,                    4),
          (  0.0016517867750633,                   15,                    8,                   -1,                    4),
          (  0.0095904006517001,                   13,                    9,                   -1,                    4),
          ( -0.0026236572984886,                   10,                   10,                   -1,                    4),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (    -50.242414011355,                    1,                    2,                  -25,                 -325,                 1.16,                    1),
          (     7484.6420119299,                    0,                    2,                  -25,                 -300,                 1.19,                    1),
          (    -6873.4299232625,                    1,                    2,                  -25,                 -300,                 1.19,                    1),
          (    -935.77982814338,                    2,                    3,                  -25,                 -300,                 1.19,                    1),
          (     941.33024786113,                    3,                    3,                  -25,                 -300,                 1.19,                    1),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 4;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (           1.8673079,                1.029),
          (         -0.61533892,                  1.5),
          (        -0.058973772,                    4),
          (           0.1074472,                    6),
      };

      _saturatedVaporDensity_Type = 4;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (          -1.9034556,                1.047),
          (         -0.75837929,                    2),
          (          -3.7717969,                    3),
          (          -8.7478586,                    7),
          (          -23.885296,                 14.5),
          (          -54.197979,                   28),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (          -6.3905741,                    1),
          (           1.4060338,                  1.5),
          (          -1.6589923,                  2.5),
          (           1.0278028,                    3),
          (          -2.5071716,                  4.5),
      };

      #endregion Saturated densities and pressure

      #region Melting pressure

      _meltingPressure_Type = '1';
      _meltingPressure_ReducingTemperature = 103.986;
      _meltingPressure_ReducingPressure = 1000000;
      _meltingPressure_Coefficients = new (double factor, double exponent)[][]
      {
      new (double factor, double exponent)[]
        {
            (           0.0001225,                    0),
            (             357.924,               2.0645),
            (            -357.924,                    0),
        },

      new (double factor, double exponent)[]{},
      new (double factor, double exponent)[]{},
      };

      #endregion Melting pressure

    }
  }
}

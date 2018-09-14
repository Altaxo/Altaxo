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
  /// State equations and constants of n-butane.
  /// Short name: butane.
  /// Synomym: R-600.
  /// Chemical formula: CH3-2(CH2)-CH3.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'butane.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Buecker, D. and Wagner, W., "Reference Equations of State for the Thermodynamic Properties of Fluid Phase n-Butane and Isobutane," J. Phys. Chem. Ref. Data, 35(2):929-1019, 2006.</para>
  /// <para>HeatCapacity (CPP): see Buecker and Wagner EOS for reference</para>
  /// <para>Melting pressure:  see EOS for reference</para>
  /// <para>Saturated vapor pressure: Lemmon, E.W., 2010.</para>
  /// <para>Saturated liquid density: Lemmon, E.W., 2010.</para>
  /// <para>Saturated vapor density: Lemmon, E.W., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("106-97-8")]
  public class N_butane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static N_butane Instance { get; } = new N_butane();

    #region Constants for n-butane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "n-butane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "butane";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "R-600";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CH3-2(CH2)-CH3";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "n-alkane";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "106-97-8";

    private int[] _unNumbers = new int[] { 1011, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.0581222; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 425.125;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3796000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 3922.769613;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 134.895;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.6656;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 12645.0621333144;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.000593502699823465;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 272.659861908891;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.201;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0.05;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 134.895;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 575;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 13860;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 200000000;

    #endregion Constants for n-butane

    private N_butane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -5.42495953152439;
      _alpha0_n_tau = 4.91949511155669;
      _alpha0_n_lntau = 3.24680487;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (          5.54913289,    0.774840435166128),
          (          11.4648996,     3.34060255219053),
          (          7.59987584,     4.97051309614819),
          (          9.66033239,      9.9755537783005),
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
          (     2.5536998241635,                  0.5,                    1),
          (    -4.4585951806696,                    1,                    1),
          (    0.82425886369063,                  1.5,                    1),
          (    0.11215007011442,                    0,                    2),
          (  -0.035910933680333,                  0.5,                    3),
          (   0.016790508518103,                  0.5,                    4),
          (   0.032734072508724,                 0.75,                    4),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (    0.95571232982005,                    2,                    1,                   -1,                    1),
          (    -1.0003385753419,                  2.5,                    1,                   -1,                    1),
          (   0.085581548803855,                  2.5,                    2,                   -1,                    1),
          (  -0.025147918369616,                  1.5,                    7,                   -1,                    1),
          ( -0.0015202958578918,                    1,                    8,                   -1,                    1),
          (   0.004706068232642,                  1.5,                    8,                   -1,                    1),
          (  -0.097845414174006,                    4,                    1,                   -1,                    2),
          (   -0.04831790415876,                    7,                    2,                   -1,                    2),
          (    0.17841271865468,                    3,                    3,                   -1,                    2),
          (   0.018173836739334,                    7,                    3,                   -1,                    2),
          (   -0.11399068074953,                    3,                    4,                   -1,                    2),
          (   0.019329896666669,                    1,                    5,                   -1,                    2),
          (   0.001157587740101,                    6,                    5,                   -1,                    2),
          ( 0.00015253808698116,                    0,                   10,                   -1,                    2),
          (  -0.043688558458471,                    6,                    2,                   -1,                    3),
          ( -0.0082403190629989,                   13,                    6,                   -1,                    3),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (  -0.028390056949441,                    2,                    1,                  -10,                 -150,                 1.16,                 0.85),
          (  0.0014904666224681,                    0,                    2,                  -10,                 -200,                 1.13,                    1),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              5.2341,                 0.44),
          (             -6.2011,                  0.6),
          (              3.6063,                 0.76),
          (             0.22137,                    5),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              -2.739,                0.391),
          (             -5.7347,                 1.14),
          (             -16.408,                    3),
          (             -46.986,                  6.5),
          (              -100.9,                   14),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.1897,                    1),
          (              2.6122,                  1.5),
          (             -2.1729,                    2),
          (              -2.723,                  4.5),
      };

      #endregion Saturated densities and pressure

      #region Melting pressure

      _meltingPressure_Type = '1';
      _meltingPressure_ReducingTemperature = 134.895;
      _meltingPressure_ReducingPressure = 0.66566;
      _meltingPressure_Coefficients = new (double factor, double exponent)[][]
      {
      new (double factor, double exponent)[]
        {
            (        -558558235.4,                    0),
            (         558558236.4,                2.206),
        },

      new (double factor, double exponent)[]{},
      new (double factor, double exponent)[]{},
      };

      #endregion Melting pressure

    }
  }
}

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
  /// State equations and constants of methoxymethane.
  /// Short name: dimethylether.
  /// Synomym: RE-170.
  /// Chemical formula: (CH3)2O.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'dme.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Wu, J., Zhou, Y., and Lemmon, E.W., "An equation of state for the thermodynamic properties of dimethyl ether," J. Phys. Chem. Ref. Data, 40(023104):1-16, 2011.</para>
  /// <para>HeatCapacity (CPP): Ihmels, E.C. and Lemmon, E.W.see EOS</para>
  /// </remarks>
  [CASRegistryNumber("115-10-6")]
  public class Methoxymethane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Methoxymethane Instance { get; } = new Methoxymethane();

    #region Constants for methoxymethane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "methoxymethane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "dimethylether";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "RE-170";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "(CH3)2O";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "115-10-6";

    private int[] _unNumbers = new int[] { 1033, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.04606844; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 400.378;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 5336800;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 5940;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 131.66;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 2.21;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 19149.5234686767;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.00201953171982851;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 248.367804166716;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.196;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.301;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 131.66;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 525;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 19150;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 40000000;

    #endregion Constants for methoxymethane

    private Methoxymethane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -1.9809704305085;
      _alpha0_n_tau = 3.17121661024589;
      _alpha0_n_lntau = 3.039;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (               2.641,    0.901647942694154),
          (               2.123,      2.4327010974629),
          (               8.992,     4.78547772355124),
          (               6.191,     10.3652048813871),
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
          (         0.029814139,                    1,                    4),
          (             1.43517,               0.4366,                    1),
          (            -2.64964,                1.011,                    1),
          (         -0.29515532,                1.137,                    2),
          (          0.17035607,                 0.45,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (         -0.94642918,                 2.83,                    1,                   -1,                    2),
          (        -0.099250514,                  1.5,                    3,                   -1,                    2),
          (           1.1264071,                1.235,                    2,                   -1,                    1),
          (         -0.76936548,                2.675,                    2,                   -1,                    2),
          (        -0.020717696,               0.7272,                    7,                   -1,                    1),
          (          0.24527037,                1.816,                    1,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (           1.1863438,                1.783,                    1,            -0.965336,             -1.28719,              1.27772,             0.672698),
          (         -0.49398368,                3.779,                    1,             -1.50858,            -0.806235,              0.43075,             0.924246),
          (         -0.16388716,                3.282,                    3,            -0.963855,            -0.777942,             0.429607,             0.750815),
          (        -0.027583584,                1.059,                    3,             -9.72643,             -197.681,              1.13849,             0.800022),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (            7.884834,                 0.54),
          (          -10.516328,                 0.74),
          (             5.39142,                 0.95),
          (             0.40489,                11.43),
      };

      _saturatedVaporDensity_Type = 4;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (           -4.136444,                1.467),
          (           -4.302025,                  4.2),
          (           -12.03214,                    8),
          (          -39.527936,                   17),
          (           -89.47686,                   36),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (           -7.112782,                    1),
          (            1.971239,                  1.5),
          (           -2.276083,                  2.5),
          (           -2.215774,                    5),
      };

      #endregion Saturated densities and pressure

    }
  }
}

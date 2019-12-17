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
  /// State equations and constants of 1,2-dimethylbenzene.
  /// Short name: o-xylene.
  /// Synomym: o-xylene.
  /// Chemical formula: C8H10.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'oxylene.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Zhou, Y., Lemmon, E.W., and Wu, J."Thermodynamic Properties of o-Xylene, m-Xylene, p-Xylene, and Ethylbenzene"J. Phys. Chem. Ref. Data, 41(023103):1-26, 2012.</para>
  /// <para>HeatCapacity (CPP): see EOS for reference</para>
  /// <para>Saturated vapor pressure: Herrig, S., 2013.</para>
  /// <para>Saturated liquid density: Herrig, S., 2013.</para>
  /// <para>Saturated vapor density: Herrig, S., 2013.</para>
  /// </remarks>
  [CASRegistryNumber("95-47-6")]
  public class Fluid_1_2_dimethylbenzene : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Fluid_1_2_dimethylbenzene Instance { get; } = new Fluid_1_2_dimethylbenzene();

    #region Constants for 1,2-dimethylbenzene

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "1,2-dimethylbenzene";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "o-xylene";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "o-xylene";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C8H10";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "95-47-6";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.106165; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 630.259;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3737500;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 2684.5;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 247.985;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 22.8;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 8647.22032571275;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.0110615769296471;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 417.52096935897;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.312;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0.63;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 247.985;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 700;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 8648;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 70000000;

    #endregion Constants for 1,2-dimethylbenzene

    private Fluid_1_2_dimethylbenzene()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 10.1373758815127;
      _alpha0_n_tau = -0.912829928662992;
      _alpha0_n_lntau = 2.748798;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (            4.754892,    0.356996092082779),
          (            6.915052,    0.994829109937343),
          (            25.84813,     2.73855668859945),
          (            10.93886,     7.83963418213782),
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
          (        0.0036765156,                    1,                    5),
          (         -0.13918171,                  0.6,                    1),
          (         0.014104203,                 0.91,                    4),
          (           1.5398899,                  0.3,                    1),
          (          -2.3600925,                0.895,                    1),
          (         -0.44359159,                1.167,                    2),
          (          0.19596977,                0.435,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (          -1.0909408,                2.766,                    1,                   -1,                    2),
          (         -0.21890801,                  3.8,                    3,                   -1,                    2),
          (           1.1179223,                 1.31,                    2,                   -1,                    1),
          (         -0.93563815,                    3,                    2,                   -1,                    2),
          (        -0.018102996,                 0.77,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (           1.4172368,                 1.41,                    1,              -1.1723,               -2.442,               1.2655,                0.552),
          (         -0.57134695,                  4.8,                    1,               -1.095,               -1.342,               0.3959,                0.728),
          (        -0.081944041,                1.856,                    3,              -1.6166,                   -3,               0.7789,                0.498),
          (          -40.682878,                    2,                    3,                -20.4,                 -450,                1.162,                0.894),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              0.9743,                  0.3),
          (              16.511,                 0.96),
          (             -52.934,                  1.4),
          (              87.962,                  1.9),
          (             -71.719,                  2.4),
          (              22.569,                    3),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (            -1.29038,                 0.32),
          (            -33.3428,                 1.14),
          (             142.046,                  1.7),
          (            -292.211,                  2.2),
          (              293.95,                  2.8),
          (            -159.504,                  3.5),
          (             -88.217,                  9.8),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.2834,                    1),
          (             -1.5813,                  1.5),
          (              7.6516,                  1.9),
          (             -7.9953,                  2.4),
          (             -2.2277,                    6),
      };

      #endregion Saturated densities and pressure

    }
  }
}

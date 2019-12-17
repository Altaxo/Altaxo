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
  /// State equations and constants of 1,4-dimethylbenzene.
  /// Short name: p-xylene.
  /// Synomym: p-xylene.
  /// Chemical formula: C8H10.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'pxylene.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Zhou, Y., Lemmon, E.W., and Wu, J."Thermodynamic Properties of o-Xylene, m-Xylene, p-Xylene, and Ethylbenzene"J. Phys. Chem. Ref. Data, 41(023103):1-26, 2012.</para>
  /// <para>HeatCapacity (CPP): see EOS for reference</para>
  /// <para>Saturated vapor pressure: Herrig, S., 2013.</para>
  /// <para>Saturated liquid density: Herrig, S., 2013.</para>
  /// <para>Saturated vapor density: Herrig, S., 2013.</para>
  /// </remarks>
  [CASRegistryNumber("106-42-3")]
  public class Fluid_1_4_dimethylbenzene : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Fluid_1_4_dimethylbenzene Instance { get; } = new Fluid_1_4_dimethylbenzene();

    #region Constants for 1,4-dimethylbenzene

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "1,4-dimethylbenzene";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "p-xylene";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "p-xylene";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C8H10";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "106-42-3";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.106165; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 616.168;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3531500;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 2693.92;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 286.4;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 580;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 8165.00473962458;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.243846640741241;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 411.470416952572;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.324;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 286.4;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 700;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 8166;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 200000000;

    #endregion Constants for 1,4-dimethylbenzene

    private Fluid_1_4_dimethylbenzene()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 5.98152412733287;
      _alpha0_n_tau = -0.524778348984639;
      _alpha0_n_lntau = 4.2430504;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (           5.2291378,    0.671894678074811),
          (           19.549862,     2.03840511029459),
          (           16.656178,     4.29915217927578),
          (           5.9390291,     10.8428220874826),
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
          (        0.0010786811,                    1,                    5),
          (        -0.103161822,                 0.83,                    1),
          (        0.0421544125,                 0.83,                    4),
          (          1.47865376,                0.281,                    1),
          (             -2.4266,                0.932,                    1),
          (         -0.46575193,                  1.1,                    2),
          (         0.190290995,                0.443,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (         -1.06376565,                 2.62,                    1,                   -1,                    2),
          (        -0.209934069,                  2.5,                    3,                   -1,                    2),
          (          1.25159879,                  1.2,                    2,                   -1,                    1),
          (        -0.951328356,                    3,                    2,                   -1,                    2),
          (       -0.0269980032,                0.778,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (           1.3710318,                 1.13,                    1,               -1.179,               -2.445,                1.267,              0.54944),
          (        -0.494160616,                  4.5,                    1,               -1.065,               -1.483,               0.4242,               0.7234),
          (       -0.0724317468,                  2.2,                    3,               -1.764,               -4.971,                0.864,               0.4926),
          (         -3.69464746,                    2,                    3,              -13.675,                 -413,               1.1465,               0.8459),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              0.1783,                 0.15),
          (              3.4488,                  0.5),
          (             -2.3906,                  0.9),
          (              1.5933,                  1.3),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (            -6.17784,                0.653),
          (            -0.38825,                 0.17),
          (            -19.0575,                  2.6),
          (            -541.124,                  7.8),
          (             1251.55,                  8.9),
          (            -920.226,                   10),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.7221,                    1),
          (              1.5789,                  1.5),
          (             -13.035,                  3.8),
          (              18.453,                  4.6),
          (             -11.345,                  5.5),
      };

      #endregion Saturated densities and pressure

    }
  }
}

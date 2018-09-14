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
  /// State equations and constants of 1,3-dimethylbenzene.
  /// Short name: m-xylene.
  /// Synomym: m-xylene.
  /// Chemical formula: C8H10.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'mxylene.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Zhou, Y., Lemmon, E.W., and Wu, J."Thermodynamic Properties of o-Xylene, m-Xylene, p-Xylene, and Ethylbenzene"J. Phys. Chem. Ref. Data, 41(023103):1-26, 2012.</para>
  /// <para>HeatCapacity (CPP): see EOS for reference</para>
  /// <para>Saturated vapor pressure: Herrig, S., 2013.</para>
  /// <para>Saturated liquid density: Herrig, S., 2013.</para>
  /// <para>Saturated vapor density: Herrig, S., 2013.</para>
  /// </remarks>
  [CASRegistryNumber("108-38-3")]
  public class Fluid_1_3_dimethylbenzene : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Fluid_1_3_dimethylbenzene Instance { get; } = new Fluid_1_3_dimethylbenzene();

    #region Constants for 1,3-dimethylbenzene

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "1,3-dimethylbenzene";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "m-xylene";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "m-xylene";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C8H10";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "108-38-3";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.106165; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 616.89;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3534600;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 2665;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 225.3;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 3.123;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 8676.64949512722;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.00166731739901124;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 412.213947892224;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.326;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0.3;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 225.3;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 700;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 8677;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 200000000;

    #endregion Constants for 1,3-dimethylbenzene

    private Fluid_1_3_dimethylbenzene()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 12.652886922064;
      _alpha0_n_tau = -0.459756235110525;
      _alpha0_n_lntau = 1.169909;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (             4.44312,    0.259365527079382),
          (            2.862794,    0.307996563406766),
          (            24.83298,      2.1608390474801),
          (            16.26077,      5.6671367666845),
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
          (       1.2791017E-05,                    1,                    8),
          (         0.041063111,                 0.91,                    4),
          (            1.505996,                0.231,                    1),
          (          -2.3095875,                0.772,                    1),
          (            -0.46969,                1.205,                    2),
          (            0.171031,                0.323,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (           -1.001728,                  2.7,                    1,                   -1,                    2),
          (          -0.3945766,                 3.11,                    3,                   -1,                    2),
          (           0.6970578,                0.768,                    2,                   -1,                    1),
          (          -0.3002876,                  4.1,                    2,                   -1,                    2),
          (           -0.024311,                0.818,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (            0.815488,                    2,                    1,              -1.0244,                -1.66,               1.1013,                0.713),
          (           -0.330647,                  2.9,                    1,              -1.3788,              -1.9354,               0.6515,               0.9169),
          (           -0.123393,                 3.83,                    3,              -0.9806,              -1.0323,               0.4975,               0.6897),
          (            -0.54661,                  0.5,                    3,              -6.3563,                  -78,                 1.26,               0.7245),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             0.43346,                 0.16),
          (              3.8716,                  0.6),
          (             -3.0144,                    1),
          (               1.619,                  1.5),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -1.1597,                 0.26),
          (             -6.0358,                 0.78),
          (             -16.712,                  2.6),
          (             -45.482,                  5.7),
          (             -98.418,                 11.7),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.5635,                    1),
          (              1.2857,                  1.5),
          (             -3.2346,                  3.1),
          (             -1.9018,                  5.6),
      };

      #endregion Saturated densities and pressure

    }
  }
}

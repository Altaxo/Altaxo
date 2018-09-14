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
  /// State equations and constants of hydrogen chloride.
  /// Short name: hydrogen chloride.
  /// Synomym: hydrogen chloride.
  /// Chemical formula: HCl.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'hcl.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>Saturated vapor pressure: see EOS Thol et al. (2016)</para>
  /// <para>Saturated liquid density: See EOS</para>
  /// <para>Saturated vapor density: See EOS</para>
  /// </remarks>
  [CASRegistryNumber("7647-01-0")]
  public class HydrogenChloride : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static HydrogenChloride Instance { get; } = new HydrogenChloride();

    #region Constants for hydrogen chloride

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "hydrogen chloride";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "hydrogen chloride";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "hydrogen chloride";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "HCl";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "7647-01-0";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.3144621;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.036460939169; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 324.55;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 8288140;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 11800;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 159.066;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 13770;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 34377.7319460934;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 10.5440829035987;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 188.178727145864;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.128;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.079;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 159.066;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 480;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 34381;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 40000000;

    #endregion Constants for hydrogen chloride

    private HydrogenChloride()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -4.07986174025576;
      _alpha0_n_tau = 4.03177000030705;
      _alpha0_n_lntau = 2.5;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (           0.0033327,    0.924356801725466),
          (            0.935243,     12.3247573563395),
          (            0.209996,     19.4114928362348),
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
          (           0.0236157,                    1,                    4),
          (             3.31769,                 0.64,                    1),
          (            -4.47005,                 0.91,                    1),
          (          -0.0770992,                 1.25,                    2),
          (           0.0899837,                 0.37,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (            -1.47613,                 2.45,                    1,                   -1,                    2),
          (            0.259125,                 2.67,                    3,                   -1,                    2),
          (            0.325788,                 1.14,                    2,                   -1,                    1),
          (           -0.913889,                  2.6,                    2,                   -1,                    2),
          (          -0.0150363,                 0.79,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (             1.92441,                 2.32,                    1,               -1.102,               -0.511,                0.757,                0.521),
          (           -0.725486,                 2.74,                    1,                -2.39,               -0.559,                0.269,                0.679),
          (           -0.222758,                 1.79,                    3,               -1.589,               -0.678,                1.352,                0.838),
          (           -0.159802,                  3.1,                    1,               -2.832,               -1.022,                0.368,                1.134),
          (           -0.912135,                 0.35,                    3,                -6.54,               -66.75,                1.325,                0.858),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              2.0771,                0.365),
          (               1.945,                 1.37),
          (             -2.7097,                    2),
          (              1.4149,                 2.76),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (            -2.27827,                0.366),
          (             -7.1982,                 1.16),
          (              -25.57,                  3.9),
          (            -1336.24,                   12),
          (              2889.2,                   13),
          (             -1721.1,                   14),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -6.4137,                    1),
          (             0.33046,                  1.5),
          (              2.0854,                  2.2),
          (             -3.1393,                  2.9),
          (            -0.70406,                  5.9),
      };

      #endregion Saturated densities and pressure

    }
  }
}

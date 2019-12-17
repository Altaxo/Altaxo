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
  /// State equations and constants of decamethyltetrasiloxane.
  /// Short name: MD2M.
  /// Synomym: MD2M.
  /// Chemical formula: C10H30Si4O3.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'md2m.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Thol, M., Dubberke, F.H, Vrabec, J., Span, R."Thermodynamic Properties of Octamethyltrisiloxane and Decamethyltetrasiloxane" to be submitted to J. Chem. Eng. Data, 2017</para>
  /// <para>HeatCapacity (CPP): see EOS of Toris and Thol (2016).</para>
  /// <para>Saturated vapor pressure: see EOS of Thol et al. (2017).</para>
  /// <para>Saturated liquid density: see EOS of Thol et al. (2017).</para>
  /// <para>Saturated vapor density: see EOS of Thol et al. (2017).</para>
  /// </remarks>
  [CASRegistryNumber("141-62-8")]
  public class Decamethyltetrasiloxane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Decamethyltetrasiloxane Instance { get; } = new Decamethyltetrasiloxane();

    #region Constants for decamethyltetrasiloxane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "decamethyltetrasiloxane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "MD2M";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "MD2M";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C10H30Si4O3";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "141-62-8";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.3144621;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.3106854; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 599.4;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 1144000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 864;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 205.2;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.0003127;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 3038.15568104603;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 1.83296631905013E-07;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 467.590516735385;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.635;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.12;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 205.2;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 600;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 3039;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 130000000;

    #endregion Constants for decamethyltetrasiloxane

    private Decamethyltetrasiloxane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 131.089725009606;
      _alpha0_n_tau = -26.3839137983711;
      _alpha0_n_lntau = 3;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (               28.59,   0.0333667000333667),
          (               56.42,     1.96863530196864),
          (               50.12,     7.07374040707374),
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
          (          0.01458333,                    1,                    4),
          (            3.227554,                0.319,                    1),
          (           -3.503565,                0.829,                    1),
          (           -2.017391,                 0.78,                    2),
          (           0.8606129,                0.687,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (           -2.196015,                 1.29,                    1,                   -1,                    2),
          (          -0.9289014,                 3.91,                    3,                   -1,                    2),
          (             2.02774,                 0.77,                    2,                   -1,                    1),
          (          -0.9168439,                3.055,                    2,                   -1,                    2),
          (         -0.06383507,                1.013,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (            2.674255,                 1.07,                    1,               -0.982,              -0.7323,                1.042,                0.874),
          (          0.04662529,                 1.89,                    1,                 -2.7,               -0.543,                  1.1,                 1.43),
          (          -0.3835361,                1.133,                    3,               -1.347,                -1.26,                1.146,                0.855),
          (          -0.4273462,                0.826,                    2,               -0.864,               -0.878,                1.085,                0.815),
          (           -1.148009,                 0.83,                    2,               -1.149,                -2.22,               0.6844,                0.491),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (               8.215,                0.498),
          (              -24.65,                0.855),
          (               47.23,                 1.22),
          (              -42.44,                  1.6),
          (               15.18,                 2.04),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -4.5483,                0.428),
          (            -101.989,                 2.32),
          (              224.06,                  2.8),
          (             -182.79,                  3.3),
          (             -110.45,                  8.5),
          (             -330.87,                 17.5),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -10.174,                    1),
          (               9.607,                  1.5),
          (              -10.08,                 1.83),
          (              -7.242,                 4.15),
          (              -30.56,                 17.8),
      };

      #endregion Saturated densities and pressure

    }
  }
}

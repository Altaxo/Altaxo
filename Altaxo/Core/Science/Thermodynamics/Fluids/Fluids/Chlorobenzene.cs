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
  /// State equations and constants of chlorobenzene.
  /// Short name: chlorobenzene.
  /// Synomym: phenyl chloride.
  /// Chemical formula: C6H5CHL.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'cbenzene.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS):  Thol, M., Alexandrov, I.S., Span, R., Lemmon, E.W. to be published in J. Chem. Eng. Data, 2016.</para>
  /// <para>HeatCapacity (CPP): see EOS of Thol et al. (2016)</para>
  /// <para>Saturated vapor pressure: See EOS</para>
  /// <para>Saturated liquid density: See EOS</para>
  /// <para>Saturated vapor density: See EOS</para>
  /// </remarks>
  [CASRegistryNumber("108-90-7")]
  public class Chlorobenzene : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Chlorobenzene Instance { get; } = new Chlorobenzene();

    #region Constants for chlorobenzene

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "chlorobenzene";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "chlorobenzene";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "phenyl chloride";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C6H5CHL";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "108-90-7";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.3144621;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.112557; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 632.35;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 4520600;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 3240;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 227.9;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 7.14;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 10468.0381830604;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.00376859487836371;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 405.214937854792;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.2532;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = -1;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 227.9;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 700;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 10468;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 100000000;

    #endregion Constants for chlorobenzene

    private Chlorobenzene()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 2.6514717157401;
      _alpha0_n_tau = 1.29476348223411;
      _alpha0_n_lntau = 3;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (              6.2566,     6.57863524946628),
          (              16.273,     2.49861627263383),
          (              7.6017,    0.948841622519174),
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
          (          0.03675169,                    1,                    4),
          (              1.2629,                 0.25,                    1),
          (           -2.092176,                0.967,                    1),
          (          -0.5062699,                 1.06,                    2),
          (           0.1826893,                0.527,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (          -0.9710427,                 1.93,                    1,                   -1,                    2),
          (          -0.3295967,                 2.44,                    3,                   -1,                    2),
          (           0.8757209,                 1.28,                    2,                   -1,                    1),
          (          -0.3980378,                 3.06,                    2,                   -1,                    2),
          (         -0.02049013,                1.013,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (            1.307316,                0.768,                    1,               -0.815,                -1.45,                 1.31,                1.042),
          (         -0.07704369,                  1.4,                    1,                -1.25,                -1.65,                    1,                1.638),
          (         -0.00306347,                  1.2,                    3,                -3.76,                -62.7,                 0.87,                0.823),
          (          -0.2117575,                  1.3,                    2,               -0.876,                -1.51,                 1.29,                1.139),
          (          -0.5223262,                 1.16,                    2,               -1.034,                -1.24,                1.114,                0.799),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              6.4638,                  0.5),
          (               -17.8,                 0.88),
          (              39.155,                 1.28),
          (              -47.82,                 1.71),
          (               30.03,                 2.18),
          (              -7.079,                 2.73),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              -3.991,                0.444),
          (              -39.27,                 2.04),
          (                  78,                 2.55),
          (              -69.23,                 3.08),
          (              -60.59,                 7.63),
          (             -158.25,                 16.8),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.6061,                    1),
          (              3.3469,                  1.5),
          (             -2.8389,                 1.95),
          (               -3.43,                 4.43),
          (              -116.4,                   29),
      };

      #endregion Saturated densities and pressure

    }
  }
}

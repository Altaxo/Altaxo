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
  /// State equations and constants of trans-2-butene.
  /// Short name: trans-butene.
  /// Synomym: (E)-2-butene.
  /// Chemical formula: CH3-CH=CH-CH3.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 't2butene.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Lemmon, E.W. and Ihmels, E.C., "Thermodynamic Properties of the Butenes.  Part II. Short Fundamental Equations of State," Fluid Phase Equilibria, 228-229C:173-187, 2005.</para>
  /// <para>HeatCapacity (CPP): Lemmon, E.W. and Ihmels, E.C.,</para>
  /// <para>Saturated vapor pressure: Cullimore, I.D., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("624-64-6")]
  public class Trans_2_butene : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Trans_2_butene Instance { get; } = new Trans_2_butene();

    #region Constants for trans-2-butene

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "trans-2-butene";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "trans-butene";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "(E)-2-butene";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CH3-CH=CH-CH3";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "n-alkene";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "624-64-6";

    private int[] _unNumbers = new int[] { 1012, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.05610632; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 428.61;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 4027300;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 4213;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 167.6;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 74.81;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 13140.7558213585;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.0537026184735124;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 274.029858142487;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.21;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 167.6;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 525;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 13141;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 50000000;

    #endregion Constants for trans-2-butene

    private Trans_2_butene()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 0.591781562211686;
      _alpha0_n_tau = 2.14277584407775;
      _alpha0_n_lntau = 2.9988;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (              5.3276,    0.844590653507851),
          (               13.29,     3.73999673362731),
          (              9.6745,     8.70021698047176),
          (             0.40087,     10.5620494155526),
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
          (             0.81107,                 0.12,                    1),
          (             -2.8846,                  1.3,                    1),
          (              1.0265,                 1.74,                    1),
          (            0.016591,                  2.1,                    2),
          (            0.086511,                 0.28,                    3),
          (          0.00023256,                 0.69,                    7),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (             0.22654,                 0.75,                    2,                   -1,                    1),
          (           -0.072182,                    2,                    5,                   -1,                    1),
          (            -0.24849,                  4.4,                    1,                   -1,                    2),
          (           -0.071374,                  4.7,                    4,                   -1,                    2),
          (           -0.024737,                   15,                    3,                   -1,                    3),
          (            0.011843,                   14,                    4,                   -1,                    3),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              12.452,                 0.52),
          (             -34.419,                 0.73),
          (              52.257,                 0.97),
          (             -42.889,                 1.24),
          (              15.463,                  1.5),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -3.1276,                0.412),
          (             -6.0548,                 1.24),
          (             -18.243,                  3.2),
          (             -60.842,                    7),
          (              135.95,                   10),
          (              -182.7,                   11),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.6226,                    1),
          (              7.9421,                  1.5),
          (             -6.9631,                 1.65),
          (             -6.5517,                  4.8),
          (              3.9584,                  5.3),
      };

      #endregion Saturated densities and pressure

    }
  }
}

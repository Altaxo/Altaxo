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
  /// State equations and constants of tetradecamethylhexasiloxane.
  /// Short name: MD4M.
  /// Synomym: MD4M.
  /// Chemical formula: C14H42O5Si6.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'md4m.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS):  Colonna, P., Nannan, N.R., Guardone, A., Lemmon, E.W., Multiparameter Equations of State for Selected Siloxanes, Fluid Phase Equilibria, 244:193-211, 2006.</para>
  /// <para>HeatCapacity (CPP):  Colonna, P., Nannan, N.R., Guardone, A., Lemmon, E.W., Multiparameter Equations of State for Selected Siloxanes, Fluid Phase Equilibria, 244:193-211, 2006.</para>
  /// <para>Saturated vapor pressure: Lemmon, E.W., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("107-52-8")]
  public class Tetradecamethylhexasiloxane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Tetradecamethylhexasiloxane Instance { get; } = new Tetradecamethylhexasiloxane();

    #region Constants for tetradecamethylhexasiloxane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "tetradecamethylhexasiloxane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "MD4M";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "MD4M";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C14H42O5Si6";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "107-52-8";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.45899328; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 653.2;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 877470;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 622.35694;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 214.15;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 1.033E-06;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = double.NaN;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = double.NaN;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 532.723355998617;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.825;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.308;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 300;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 673;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 2090;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 30000000;

    #endregion Constants for tetradecamethylhexasiloxane

    private Tetradecamethylhexasiloxane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 112.076644492885;
      _alpha0_n_tau = -36.3787595851835;
      _alpha0_n_lntau = -3.41398371417933;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          (   -87.5375008779872,                   -1),
          (    11.2160931112242,                   -2),
          (   -0.79945171122313,                   -3),
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
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
          (          1.18492421,                 0.25,                    1),
          (         -1.87465636,                1.125,                    1),
          (         -0.06571351,                  1.5,                    1),
          (         -0.61812689,                1.375,                    2),
          (          0.19535804,                 0.25,                    3),
          (        0.0005067874,                0.875,                    7),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (          1.23544082,                0.625,                    2,                   -1,                    1),
          (         0.049462708,                 1.75,                    5,                   -1,                    1),
          (         -0.73685283,                3.625,                    1,                   -1,                    2),
          (         -0.19991438,                3.625,                    4,                   -1,                    2),
          (        -0.055118673,                 14.5,                    3,                   -1,                    3),
          (         0.028325885,                   12,                    4,                   -1,                    3),
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
          (              1.0453,                0.235),
          (             0.55476,                  0.6),
          (              4.4536,                 0.95),
          (             -7.6179,                 1.35),
          (              4.6237,                  1.7),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              -1.089,                0.231),
          (             -8.4374,                  0.8),
          (             -35.615,                  2.9),
          (             -734.78,                  7.7),
          (              1991.5,                    9),
          (             -1631.7,                   10),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -10.532,                    1),
          (              3.3939,                  1.5),
          (             -8.9744,                 2.75),
          (              -5.615,                  5.1),
      };

      #endregion Saturated densities and pressure

    }
  }
}

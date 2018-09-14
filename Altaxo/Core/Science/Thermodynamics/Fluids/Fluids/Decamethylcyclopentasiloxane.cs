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
  /// State equations and constants of decamethylcyclopentasiloxane.
  /// Short name: D5.
  /// Synomym: D5.
  /// Chemical formula: C10H30O5Si5.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'd5.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS):  Colonna, P., Nannan, N.R., Guardone, A., Lemmon, E.W., Multiparameter Equations of State for Selected Siloxanes, Fluid Phase Equilibria, 244:193-211, 2006.</para>
  /// <para>HeatCapacity (CPP):  Colonna, P., Nannan, N.R., Guardone, A., Lemmon, E.W., Multiparameter Equations of State for Selected Siloxanes, Fluid Phase Equilibria, 244:193-211, 2006.</para>
  /// <para>Saturated vapor pressure: Lemmon, E.W., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("541-02-6")]
  public class Decamethylcyclopentasiloxane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Decamethylcyclopentasiloxane Instance { get; } = new Decamethylcyclopentasiloxane();

    #region Constants for decamethylcyclopentasiloxane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "decamethylcyclopentasiloxane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "D5";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "D5";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C10H30O5Si5";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "541-02-6";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.3707697; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 619.23462341;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 1161460;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 789.09027;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 226;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.005304;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 2829.84826067343;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 2.82276629227225E-06;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 484.050286854337;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.658;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.349;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 300;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 673;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 2830;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 30000000;

    #endregion Constants for decamethylcyclopentasiloxane

    private Decamethylcyclopentasiloxane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 79.8331483238124;
      _alpha0_n_tau = -21.7769511940726;
      _alpha0_n_lntau = -5.19725991019033;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          (   -69.3192094144833,                   -1),
          (    10.7871323944891,                   -2),
          (   -1.18992551410419,                   -3),
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
          (          1.40844725,                 0.25,                    1),
          (         -2.29248044,                1.125,                    1),
          (          0.42851607,                  1.5,                    1),
          (         -0.73506382,                1.375,                    2),
          (          0.16103808,                 0.25,                    3),
          (       0.00029643278,                0.875,                    7),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (          0.82412481,                0.625,                    2,                   -1,                    1),
          (          0.15214274,                 1.75,                    5,                   -1,                    1),
          (          -0.6849589,                3.625,                    1,                   -1,                    2),
          (        -0.055703624,                3.625,                    4,                   -1,                    2),
          (         0.013055391,                 14.5,                    3,                   -1,                    3),
          (        -0.031853761,                   12,                    4,                   -1,                    3),
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
          (             303.988,                 0.57),
          (            -1103.42,                 0.65),
          (             1343.59,                 0.73),
          (            -705.243,                 0.84),
          (              164.54,                 0.96),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -3.7577,                0.459),
          (             -4.7669,                 1.02),
          (             -24.233,                  2.6),
          (             -298.72,                  6.7),
          (              344.41,                  7.7),
          (             -324.98,                   11),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -9.9967,                    1),
          (              7.0091,                  1.5),
          (             -7.2265,                 1.87),
          (             -6.2938,                  3.8),
      };

      #endregion Saturated densities and pressure

    }
  }
}

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
  /// State equations and constants of dodecamethylcyclohexasiloxane.
  /// Short name: D6.
  /// Synomym: D6.
  /// Chemical formula: C12H36Si6O6.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'd6.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Colonna, P., Nannan, N.R., and Guardone, A., "Multiparameter Equations of State for Siloxanes," Fluid Phase Equilibria, 263:115-130, 2008.</para>
  /// <para>HeatCapacity (CPP): Colonna, P., Nannan, N.R., and Guardone, A., "Multiparameter Equations of State for Siloxanes," Fluid Phase Equilibria, 263:115-130, 2008.</para>
  /// <para>Saturated vapor pressure: Cullimore, I.D., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("540-97-6")]
  public class Dodecamethylcyclohexasiloxane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Dodecamethylcyclohexasiloxane Instance { get; } = new Dodecamethylcyclohexasiloxane();

    #region Constants for dodecamethylcyclohexasiloxane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "dodecamethylcyclohexasiloxane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "D6";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "D6";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C12H36Si6O6";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "540-97-6";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.444924; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 645.78;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 961000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 627.2885478;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 270.2;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.1597;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 2245.07628990764;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 7.11098197032433E-05;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 518.109977174847;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.736;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.559;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 270.2;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 673;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 2246;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 30000000;

    #endregion Constants for dodecamethylcyclohexasiloxane

    private Dodecamethylcyclohexasiloxane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 375.488207820075;
      _alpha0_n_tau = -2005.03089149637;
      _alpha0_n_lntau = 55.371589200132;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
      };

      _alpha0_Cosh = new (double ni, double thetai)[]
      {
          (   -686.699999987077,     1.21837158165319),
      };

      _alpha0_Sinh = new (double ni, double thetai)[]
      {
          (    981.200000095902,     2.77509368515594),
      };
      #endregion Ideal part of dimensionless Helmholtz energy and derivatives

      #region Residual part(s) of dimensionless Helmholtz energy and derivatives

      _alphaR_Poly = new (double ni, double ti, int di)[]
      {
          (          1.69156186,                 0.25,                    1),
          (         -3.37962568,                1.125,                    1),
          (          0.38609039,                  1.5,                    1),
          (         0.064598995,                1.375,                    2),
          (          0.10589012,                 0.25,                    3),
          (       4.5456825E-05,                0.875,                    7),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (          0.74169279,                0.625,                    2,                   -1,                    1),
          (        -0.088102648,                 1.75,                    5,                   -1,                    1),
          (         -0.17373336,                3.625,                    1,                   -1,                    2),
          (         -0.10951368,                3.625,                    4,                   -1,                    2),
          (        -0.062695695,                 14.5,                    3,                   -1,                    3),
          (         0.037459986,                   12,                    4,                   -1,                    3),
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
          (              42.563,                0.537),
          (             -157.07,                 0.68),
          (              295.02,                 0.85),
          (             -241.91,                    1),
          (              65.145,                  1.2),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              -2.093,                0.338),
          (             -9.4442,                 1.02),
          (             -44.731,                 3.46),
          (             -57.898,                  7.1),
          (             -35.144,                  7.4),
          (             -296.61,                   15),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -9.6557,                    1),
          (             0.62155,                  1.5),
          (              1.7863,                 1.72),
          (             -10.496,                 3.18),
          (             -8.4102,                   11),
      };

      #endregion Saturated densities and pressure

    }
  }
}

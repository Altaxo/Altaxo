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
  /// State equations and constants of trichlorofluoromethane.
  /// Short name: R11.
  /// Synomym: CFC-11.
  /// Chemical formula: CCl3F.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'r11.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Jacobsen, R.T, Penoncello, S.G., and Lemmon, E.W., "A fundamental equation for trichlorofluoromethane (R-11)," Fluid Phase Equilibria, 80:45-56, 1992.\</para>
  /// <para>HeatCapacity (CPP): Jacobsen, R.T, Penoncello, S.G., and Lemmon, E.W., "A fundamental equation for trichlorofluoromethane (R-11)," Fluid Phase Equilibria, 80:45-56, 1992.\</para>
  /// <para>Saturated vapor pressure: Cullimore, I.D., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("75-69-4")]
  public class Trichlorofluoromethane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Trichlorofluoromethane Instance { get; } = new Trichlorofluoromethane();

    #region Constants for trichlorofluoromethane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "trichlorofluoromethane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "R11";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "CFC-11";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CCl3F";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "halocb";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "75-69-4";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.31451;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.137368; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 471.11;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 4407638;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 4032.962;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 162.68;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 6.51;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 12874.405675062;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.00481334371794063;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 296.858072364689;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.18875;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0.45;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 200;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 625;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 12880;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 30000000;

    #endregion Constants for trichlorofluoromethane

    private Trichlorofluoromethane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -17.7937112647469;
      _alpha0_n_tau = 10.0839287453981;
      _alpha0_n_lntau = 3.00564923;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          (    -0.0525022650625,                   -1),
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (                   1,     3.31361253210503),
          (                   2,      2.5867568083887),
          (                   1,     1.63451210969837),
          (                   2,     1.21549956485746),
          (                   1,     1.06738129099361),
          (                   2,    0.736019188724502),
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
          (       1.25993633881,                  0.5,                    1),
          (      -2.60818574641,                  1.5,                    1),
          (    0.00982122542463,                    5,                    1),
          (      -1.06085385839,                    1,                    2),
          (        1.2282036351,                  1.5,                    2),
          (      0.118000776439,                    0,                    3),
          (  -0.000698956926463,                    5,                    3),
          (    -0.0355428373358,                    2,                    4),
          (    0.00197169579643,                    3,                    4),
          (   -0.00848363012252,                    1,                    5),
          (    0.00417997567653,                    2,                    5),
          (  -0.000242772533848,                    4,                    5),
          (    0.00313371368974,                    1,                    6),
          (   3.96182646586E-06,                    4,                    8),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (      0.339736319502,                    5,                    1,                   -1,                    2),
          (     -0.203010634531,                    6,                    1,                   -1,                    2),
          (       -0.1060178599,                  3.5,                    2,                   -1,                    2),
          (       0.45156488259,                  5.5,                    2,                   -1,                    2),
          (     -0.339265767612,                  7.5,                    2,                   -1,                    2),
          (      0.114338523359,                    3,                    3,                   -1,                    2),
          (     0.0319537833995,                  2.5,                    4,                   -1,                    2),
          (      0.036790825978,                    5,                    6,                   -1,                    2),
          (  -9.61768948364E-06,                  1.5,                   10,                   -1,                    2),
          (    0.00246717966418,                   11,                    3,                   -1,                    4),
          (   -0.00167030256045,                    9,                    5,                   -1,                    6),
          (    0.00240710110806,                   13,                    8,                   -1,                    6),
          (    0.00156214678738,                    5,                    9,                   -1,                    6),
          (   -0.00323352596704,                    9,                    9,                   -1,                    6),
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
          (              2.0368,                0.357),
          (               12.85,                  1.5),
          (             -22.521,                  1.7),
          (               11.34,                    2),
          (            -0.94375,                    3),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -3.0296,                0.417),
          (             -6.0723,                 1.25),
          (              -15.89,                  3.1),
          (             -63.024,                  6.8),
          (              87.167,                   10),
          (             -157.15,                   12),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.0742,                    1),
          (              3.8118,                  1.5),
          (              -3.285,                 1.73),
          (              -7.634,                  5.2),
          (              5.0598,                    6),
      };

      #endregion Saturated densities and pressure

    }
  }
}

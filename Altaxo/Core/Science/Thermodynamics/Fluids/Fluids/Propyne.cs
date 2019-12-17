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
  /// State equations and constants of propyne.
  /// Short name: propyne.
  /// Synomym: methyl acetylene.
  /// Chemical formula: CH3CCH.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'propyne.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Polt, A., Platzer, B., and Maurer, G., "Parameter der thermischen Zustandsgleichung von Bender fuer 14 mehratomige reine Stoffe," Chem. Tech. (Leipzig), 44(6):216-224, 1992.</para>
  /// <para>HeatCapacity (CPP): Polt, A., Platzer, B., and Maurer, G., "Parameter der thermischen Zustandsgleichung von Bender fuer 14 mehratomige reine Stoffe," Chem. Tech. (Leipzig), 44(6):216-224, 1992.</para>
  /// <para>Saturated vapor pressure: Cullimore, I.D., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("74-99-7")]
  public class Propyne : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Propyne Instance { get; } = new Propyne();

    #region Constants for propyne

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "propyne";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "propyne";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "methyl acetylene";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CH3CCH";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "alkyne";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "74-99-7";

    private int[] _unNumbers = new int[] { 1060, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.3143;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.04006; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 402.38;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 5626000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 6113.33;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 170.5;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 186.3;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = double.NaN;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = double.NaN;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 248;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.204;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0.781;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 273;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 474;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 16280;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 32000000;

    #endregion Constants for propyne

    private Propyne()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -8.48396148500359;
      _alpha0_n_tau = 7.31970065744557;
      _alpha0_n_lntau = 0.134871819271093;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          (       -0.9745703957,                   -1),
          (  0.0937494735797693,                   -2),
          (-0.00786606141630701,                   -3),
          (0.000351474118855712,                   -4),
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
          (       1.02590136933,                    3,                    0),
          (      -2.20786016506,                    4,                    0),
          (       1.07889905204,                    5,                    0),
          (     -0.986950667682,                    0,                    1),
          (       4.59528109357,                    1,                    1),
          (      -8.86063623532,                    2,                    1),
          (       5.56346955561,                    3,                    1),
          (      -1.57450028544,                    4,                    1),
          (     -0.159068753573,                    0,                    2),
          (      0.235738270184,                    1,                    2),
          (      0.440755494599,                    2,                    2),
          (      0.196126150614,                    0,                    3),
          (      -0.36775965033,                    1,                    3),
          (    0.00792931851008,                    0,                    4),
          (    0.00247509085735,                    1,                    4),
          (    0.00832903610194,                    1,                    5),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (      -1.02590136933,                    3,                    0,          -1.65533788,                    2),
          (       2.20786016506,                    4,                    0,          -1.65533788,                    2),
          (      -1.07889905204,                    5,                    0,          -1.65533788,                    2),
          (      -3.82188466986,                    3,                    2,          -1.65533788,                    2),
          (       8.30345065619,                    4,                    2,          -1.65533788,                    2),
          (      -4.48323072603,                    5,                    2,          -1.65533788,                    2),
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
          (             0.22754,                  0.1),
          (              3.3173,                 0.53),
          (             -1.8041,                    1),
          (               2.244,                    2),
          (            -0.35823,                    3),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (            -0.17504,                  0.1),
          (             -4.6021,                 0.56),
          (             -89.211,                  2.5),
          (              180.02,                    3),
          (             -243.99,                    4),
          (              160.35,                    5),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -6.9162,                    1),
          (              1.0904,                  1.5),
          (            -0.74791,                  2.2),
          (              7.5926,                  4.8),
          (             -25.926,                  6.2),
      };

      #endregion Saturated densities and pressure

    }
  }
}

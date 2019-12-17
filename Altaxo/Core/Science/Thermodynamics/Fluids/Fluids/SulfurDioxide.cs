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
  /// State equations and constants of sulfur dioxide.
  /// Short name: sulfur dioxide.
  /// Synomym: R-764.
  /// Chemical formula: SO2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'so2.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Gao, K., Wu, J., Zhang, P., Lemmon, E.W. "A Helmholtz Energy Equation of State for Sulfur Dioxide," J. Chem. Eng. Data, 61:2859-2872, 2016.</para>
  /// <para>HeatCapacity (CPP): see EOS for reference</para>
  /// <para>Saturated vapor pressure: Cullimore, I.D., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("7446-09-5")]
  public class SulfurDioxide : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static SulfurDioxide Instance { get; } = new SulfurDioxide();

    #region Constants for sulfur dioxide

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "sulfur dioxide";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "sulfur dioxide";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "R-764";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "SO2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "7446-09-5";

    private int[] _unNumbers = new int[] { 1079, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.3144621;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.0640638; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 430.64;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 7886600;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 8078;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 197.7;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 1666.1;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 25411.3182545548;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 1.0150084921731;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 263.137009162303;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.256;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.6;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 197.7;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 525;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 25420;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 35000000;

    #endregion Constants for sulfur dioxide

    private SulfurDioxide()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -4.54142357213728;
      _alpha0_n_tau = 4.47322895714972;
      _alpha0_n_lntau = 3;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          (       -0.0159272204,                   -1),
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (              1.0875,     1.81822403864016),
          (               1.916,     4.32844138955973),
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
          (          0.01744413,                    1,                    4),
          (            1.814878,                 0.45,                    1),
          (           -2.246338,               0.9994,                    1),
          (          -0.4602906,                    1,                    2),
          (           0.1097049,                 0.45,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (          -0.9485769,                2.907,                    1,                   -1,                    2),
          (          -0.8751541,                2.992,                    3,                   -1,                    2),
          (           0.4228777,                 0.87,                    2,                   -1,                    1),
          (          -0.4174962,                3.302,                    2,                   -1,                    2),
          (        -0.002903451,                1.002,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (             1.64041,                 1.15,                    1,               -1.061,               -0.967,                1.276,                0.832),
          (          -0.4103535,                0.997,                    1,               -0.945,               -2.538,                0.738,                 0.69),
          (         -0.08316597,                 1.36,                    3,               -1.741,               -2.758,                 0.71,                 0.35),
          (          -0.2728126,                2.086,                    2,               -1.139,               -1.062,                0.997,                0.961),
          (          -0.1075782,                0.855,                    2,               -1.644,               -1.039,                 1.35,                0.981),
          (          -0.4348434,                0.785,                    1,               -0.647,                -0.41,                0.919,                0.333),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              17.156,                 0.57),
          (             -60.441,                  0.8),
          (              81.407,                    1),
          (             -51.871,                  1.3),
          (              16.754,                  1.6),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -3.3832,                0.424),
          (             -7.6873,                  1.4),
          (             -23.614,                  3.6),
          (              -137.2,                  8.5),
          (              1866.4,                   13),
          (             -2446.9,                   14),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.3845,                    1),
          (              2.2867,                  1.5),
          (             -2.4669,                  2.2),
          (             -3.2217,                  4.8),
          (             0.23109,                  6.2),
      };

      #endregion Saturated densities and pressure

    }
  }
}

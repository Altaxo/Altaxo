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
  /// State equations and constants of methylbenzene.
  /// Short name: toluene.
  /// Synomym: toluene.
  /// Chemical formula: CH3-C6H5.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'toluene.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Lemmon, E.W. and Span, R., "Short Fundamental Equations of State for 20 Industrial Fluids," J. Chem. Eng. Data, 51:785-850, 2006.</para>
  /// <para>HeatCapacity (CPP): Lemmon, E.W. and Span, R. (see eos for reference)</para>
  /// <para>Saturated vapor pressure: Lemmon, E.W., 2010.</para>
  /// <para>Saturated liquid density: Lemmon, E.W., 2010.</para>
  /// <para>Saturated vapor density: Lemmon, E.W., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("108-88-3")]
  public class Methylbenzene : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Methylbenzene Instance { get; } = new Methylbenzene();

    #region Constants for methylbenzene

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "methylbenzene";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "toluene";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "toluene";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CH3-C6H5";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "aromatic";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "108-88-3";

    private int[] _unNumbers = new int[] { 1294, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.09213842; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 591.75;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 4126300;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 3169;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 178;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.03939;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 10580.0604816307;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 2.66176552511024E-05;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 383.745701480483;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.2657;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0.36;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 178;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 700;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 10581;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 500000000;

    #endregion Constants for methylbenzene

    private Methylbenzene()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 3.52411748317427;
      _alpha0_n_tau = 1.13608234642632;
      _alpha0_n_lntau = 3;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (              1.6994,    0.321081537811576),
          (              8.0577,     1.34685255597803),
          (              17.059,     2.73595268272074),
          (              8.4567,       5.191381495564),
          (              8.6423,      13.375580904098),
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
          (             0.96464,                 0.25,                    1),
          (             -2.7855,                1.125,                    1),
          (             0.86712,                  1.5,                    1),
          (             -0.1886,                1.375,                    2),
          (             0.11804,                 0.25,                    3),
          (          0.00025181,                0.875,                    7),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (             0.57196,                0.625,                    2,                   -1,                    1),
          (           -0.029287,                 1.75,                    5,                   -1,                    1),
          (            -0.43351,                3.625,                    1,                   -1,                    2),
          (             -0.1254,                3.625,                    4,                   -1,                    2),
          (           -0.028207,                 14.5,                    3,                   -1,                    3),
          (            0.014076,                   12,                    4,                   -1,                    3),
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
          (             14.0531,                 0.54),
          (            -32.5072,                 0.72),
          (             35.1091,                 0.93),
          (            -16.0694,                  1.2),
          (             2.38699,                    2),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (            -2.97587,                0.425),
          (            -5.34939,                 1.06),
          (            -19.1781,                    3),
          (            -24.0058,                  6.3),
          (            -32.4034,                    7),
          (            -140.645,                   15),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (            -7.45201,                    1),
          (             2.03681,                  1.5),
          (            -1.43777,                 2.13),
          (            -3.51652,                    4),
          (            -1.75818,                   12),
      };

      #endregion Saturated densities and pressure

    }
  }
}

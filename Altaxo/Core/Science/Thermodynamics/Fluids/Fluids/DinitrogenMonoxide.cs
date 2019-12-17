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
  /// State equations and constants of dinitrogen monoxide.
  /// Short name: nitrous oxide.
  /// Synomym: R-744A.
  /// Chemical formula: N2O.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'n2o.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Lemmon, E.W. and Span, R., "Short Fundamental Equations of State for 20 Industrial Fluids," J. Chem. Eng. Data, 51:785-850, 2006.</para>
  /// <para>HeatCapacity (CPP): Lemmon, E.W. and Span, R. (see eos for reference)</para>
  /// <para>Saturated vapor pressure: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// <para>Saturated liquid density: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// <para>Saturated vapor density: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("10024-97-2")]
  public class DinitrogenMonoxide : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static DinitrogenMonoxide Instance { get; } = new DinitrogenMonoxide();

    #region Constants for dinitrogen monoxide

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "dinitrogen monoxide";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "nitrous oxide";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "R-744A";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "N2O";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "10024-97-2";

    private int[] _unNumbers = new int[] { 1070, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.0440128; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 309.52;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 7245000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 10270;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 182.33;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 87840;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 28113.4320793575;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 59.3360178315936;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 184.683919104175;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.162;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0.1608;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 182.33;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 525;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 28120;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 50000000;

    #endregion Constants for dinitrogen monoxide

    private DinitrogenMonoxide()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -4.42627362713101;
      _alpha0_n_tau = 4.31204752433017;
      _alpha0_n_lntau = 2.5;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (              2.1769,       2.839881106229),
          (              1.6145,     7.66347893512536),
          (             0.48393,      17.598216593435),
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
          (             0.88045,                 0.25,                    1),
          (             -2.4235,                 1.25,                    1),
          (             0.38237,                  1.5,                    1),
          (            0.068917,                 0.25,                    3),
          (          0.00020367,                0.875,                    7),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (             0.13122,                2.375,                    1,                   -1,                    1),
          (             0.46032,                    2,                    2,                   -1,                    1),
          (          -0.0036985,                2.125,                    5,                   -1,                    1),
          (            -0.23263,                  3.5,                    1,                   -1,                    2),
          (         -0.00042859,                  6.5,                    1,                   -1,                    2),
          (            -0.04281,                 4.75,                    4,                   -1,                    2),
          (           -0.023038,                 12.5,                    2,                   -1,                    3),
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
          (              6.7919,                 0.47),
          (             -16.069,                 0.72),
          (              25.632,                    1),
          (             -20.755,                  1.3),
          (              7.1963,                  1.6),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -3.1287,                0.409),
          (             -77.651,                 1.91),
          (              214.42,                 2.33),
          (             -478.09,                    3),
          (              751.85,                  3.6),
          (             -462.79,                    4),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -6.9078,                    1),
          (               2.662,                  1.5),
          (             -2.2386,                  1.9),
          (             -3.8002,                  4.8),
          (             0.76922,                  5.8),
      };

      #endregion Saturated densities and pressure

    }
  }
}

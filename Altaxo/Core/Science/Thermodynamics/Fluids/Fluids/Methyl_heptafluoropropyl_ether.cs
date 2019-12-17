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
  /// State equations and constants of methyl-heptafluoropropyl-ether.
  /// Short name: RE347mcc (HFE-7000).
  /// Synomym: HFE-7000.
  /// Chemical formula: CF3CF2CF2OCH3.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 're347mcc.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Zhou, Y. and Lemmon, E.W.preliminary equation, 2012.</para>
  /// <para>Saturated vapor pressure: See EOS</para>
  /// <para>Saturated liquid density: See EOS</para>
  /// <para>Saturated vapor density: See EOS</para>
  /// </remarks>
  [CASRegistryNumber("375-03-1")]
  public class Methyl_heptafluoropropyl_ether : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Methyl_heptafluoropropyl_ether Instance { get; } = new Methyl_heptafluoropropyl_ether();

    #region Constants for methyl-heptafluoropropyl-ether

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "methyl-heptafluoropropyl-ether";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "RE347mcc (HFE-7000)";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "HFE-7000";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CF3CF2CF2OCH3";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "halocb";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "375-03-1";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.2000548424; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 437.7;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 2476200;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 2620;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 250;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 6825;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 7661.39689931627;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 3.31136637187885;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 307.34954367739;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.403;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 3.13;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 250;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 500;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 7662;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 20000000;

    #endregion Constants for methyl-heptafluoropropyl-ether

    private Methyl_heptafluoropropyl_ether()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -22.4429070001326;
      _alpha0_n_tau = 9.66408874114219;
      _alpha0_n_lntau = 12.09;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (               13.78,     4.67214987434316),
          (               14.21,     1.94196938542381),
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
          (           0.0330627,                    1,                    4),
          (            2.606165,                 0.34,                    1),
          (           -4.902937,                 0.77,                    1),
          (            2.228012,                 1.02,                    1),
          (            1.494115,                 0.79,                    2),
          (           -2.420459,                1.017,                    2),
          (            0.160067,                0.634,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (            1.383893,                 1.35,                    2,                   -1,                    1),
          (           -2.092005,                 2.25,                    1,                   -1,                    2),
          (          -0.5904708,                  2.5,                    2,                   -1,                    2),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (           -0.701794,                    2,                    1,               -0.593,              -0.0872,                 1.06,                 1.12),
          (            2.765425,                 1.66,                    1,                -1.36,               -1.176,                 1.22,                 0.79),
          (           0.6860982,                 1.33,                    2,                -1.73,                -1.53,                 0.92,                1.055),
          (            -2.20817,                    2,                    2,               -1.483,                -0.78,                 1.08,                  0.5),
          (           0.1739594,                 1.87,                    3,               -0.617,               -0.088,                 1.21,                 0.84),
          (          -0.9028007,                 1.75,                    3,               -1.596,                -1.04,                 0.85,                 0.85),
          (          -0.0213123,                 1.05,                    1,                -9.64,                 -263,                 1.12,                 0.91),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              1.5144,                 0.29),
          (              2.3745,                 0.85),
          (             -2.6363,                  1.5),
          (               2.083,                  2.2),
          (             0.50537,                    9),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              -2.064,                0.321),
          (             -6.4226,                 0.96),
          (             -18.982,                 2.75),
          (             -58.689,                  5.9),
          (             -117.64,                   12),
          (             -253.93,                   22),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -8.0456,                    1),
          (              2.6285,                  1.5),
          (             -2.7498,                    2),
          (             -5.4277,                 4.25),
          (             -4.3693,                 12.8),
      };

      #endregion Saturated densities and pressure

    }
  }
}

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
  /// State equations and constants of hydrogen (normal).
  /// Short name: hydrogen (normal).
  /// Synomym: R-702.
  /// Chemical formula: H2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'hydrogen.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Leachman, J.W., Jacobsen, R.T, Penoncello, S.G., Lemmon, E.W."Fundamental Equations of State for Parahydrogen, Normal Hydrogen, andOrthohydrogen,"J. Phys. Chem. Ref. Data, 38(3):721-748, 2009.</para>
  /// <para>HeatCapacity (CPP): Leachman, J.W., Jacobsen, R.T, Penoncello, S.G., Lemmon, E.W."Fundamental Equations of State for Parahydrogen, Normal Hydrogen, andOrthohydrogen,"J. Phys. Chem. Ref. Data, 38(3):721-748, 2009.</para>
  /// <para>Melting pressure: preliminary equation, 2007.</para>
  /// <para>Sublimation pressure: Lemmon, E.W., 2003.</para>
  /// <para>Saturated vapor pressure: Leachman, J.W., Jacobsen, R.T, Penoncello, S.G., Lemmon, E.W."Fundamental Equations of State for Parahydrogen, Normal Hydrogen, andOrthohydrogen,"J. Phys. Chem. Ref. Data, 38(3):721-748, 2009.</para>
  /// <para>Saturated liquid density: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// <para>Saturated vapor density: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("1333-74-0")]
  public class Hydrogen_Normal : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Hydrogen_Normal Instance { get; } = new Hydrogen_Normal();

    #region Constants for hydrogen (normal)

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "hydrogen (normal)";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "hydrogen (normal)";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "R-702";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "H2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "1333-74-0";

    private int[] _unNumbers = new int[] { 1049, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.00201588; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 33.145;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 1296400;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 15508;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 13.957;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 7357;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 38198.5416103163;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 64.4152691735028;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 20.3689036324222;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = -0.219;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 13.957;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 1000;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 102000;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 2000000000;

    #endregion Constants for hydrogen (normal)

    private Hydrogen_Normal()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -1.45798564005451;
      _alpha0_n_tau = 1.8880767711858;
      _alpha0_n_lntau = 1.5;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (               1.616,     16.0205159149193),
          (             -0.4117,     22.6580178005732),
          (              -0.792,      60.009051138935),
          (               0.758,     74.9434303816564),
          (               1.217,      206.93920651682),
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
          (            -6.93643,               0.6844,                    1),
          (                0.01,                    1,                    4),
          (              2.1101,                0.989,                    1),
          (             4.52059,                0.489,                    1),
          (            0.732564,                0.803,                    2),
          (            -1.34086,               1.1444,                    2),
          (            0.130985,                1.409,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (           -0.777414,                1.754,                    1,                   -1,                    1),
          (            0.351944,                1.311,                    3,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (          -0.0211716,                4.187,                    2,               -1.685,               -0.171,               0.7164,                1.506),
          (           0.0226312,                5.646,                    1,               -0.489,              -0.2245,               1.3444,                0.156),
          (            0.032187,                0.791,                    3,               -0.103,              -0.1304,               1.4517,                1.736),
          (          -0.0231752,                7.249,                    1,               -2.506,              -0.2785,               0.7204,                 0.67),
          (           0.0557346,                2.986,                    1,               -1.607,              -0.3967,               1.5445,                1.662),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              15.456,                 0.62),
          (              -41.72,                 0.83),
          (              50.276,                 1.05),
          (             -27.947,                  1.3),
          (              5.6718,                  1.6),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -2.9962,                0.466),
          (             -16.724,                    2),
          (              15.819,                  2.4),
          (             -16.852,                    4),
          (              34.586,                    7),
          (             -53.754,                    8),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (            -4.89789,                    1),
          (            0.988558,                  1.5),
          (            0.349689,                    2),
          (            0.499356,                 2.85),
      };

      #endregion Saturated densities and pressure

      #region Sublimation pressure

      _sublimationPressure_Type = 3;
      _sublimationPressure_ReducingTemperature = 13.957;
      _sublimationPressure_ReducingPressure = 7700;
      _sublimationPressure_PolynomialCoefficients1 = new (double factor, double exponent)[]
      {
      };

      _sublimationPressure_PolynomialCoefficients2 = new (double factor, double exponent)[]
      {
          (              -8.065,                 0.93),
      };

      #endregion Sublimation pressure

      #region Melting pressure

      _meltingPressure_Type = '1';
      _meltingPressure_ReducingTemperature = 13.957;
      _meltingPressure_ReducingPressure = 7357.8;
      _meltingPressure_Coefficients = new (double factor, double exponent)[][]
      {
      new (double factor, double exponent)[]
        {
            (                   1,                    0),
        },

      new (double factor, double exponent)[]
        {
            (              5626.3,                    1),
            (              2717.2,                 1.83),
        },

      new (double factor, double exponent)[]{},
      };

      #endregion Melting pressure

    }
  }
}

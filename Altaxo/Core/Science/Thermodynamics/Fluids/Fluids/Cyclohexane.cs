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
  /// State equations and constants of cyclohexane.
  /// Short name: cyclohexane.
  /// Synomym: hexahydrobenzene.
  /// Chemical formula: cyclo-C6H12.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'cyclohex.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Zhou, Y., Penoncello, S.G., and Lemmon, E.W., to be submitted to J. Phys. Chem. Ref. Data, 2013.</para>
  /// <para>HeatCapacity (CPP): see EOS</para>
  /// <para>Melting pressure: Refit by E.W. Lemmon of data reported in:Penoncello, S.G., Goodwin, A.R.H., and Jacobsen, R.T, "A Thermodynamic Property Formulation for Cyclohexane," Int. J. Thermophys., 16(2):519-531, 1995.</para>
  /// <para>Saturated vapor pressure: Herrig, S., 2013.</para>
  /// <para>Saturated liquid density: Herrig, S., 2013.</para>
  /// <para>Saturated vapor density: Herrig, S., 2013.</para>
  /// </remarks>
  [CASRegistryNumber("110-82-7")]
  public class Cyclohexane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Cyclohexane Instance { get; } = new Cyclohexane();

    #region Constants for cyclohexane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "cyclohexane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "cyclohexane";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "hexahydrobenzene";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "cyclo-C6H12";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "naphthene";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "110-82-7";

    private int[] _unNumbers = new int[] { 1145, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.3144621;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.08415948; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 553.6;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 4080500;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 3224;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 279.47;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 5240.2;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 9403.39213516818;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 2.26451515554311;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 353.864939258017;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.2096;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0.3;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 279.47;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 700;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 10300;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 250000000;

    #endregion Constants for cyclohexane

    private Cyclohexane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 0.989114723769908;
      _alpha0_n_tau = 1.63596529237175;
      _alpha0_n_lntau = 3.00000119069497;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (             0.83775,     1.39631502890173),
          (              16.036,     1.69978323699422),
          (              24.636,     3.94689306358381),
          (              7.1715,     8.11958092485549),
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
          (          0.05483581,                    1,                    4),
          (            1.607734,                 0.37,                    1),
          (           -2.375928,                 0.79,                    1),
          (          -0.5137709,                1.075,                    2),
          (           0.1858417,                 0.37,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (          -0.9007515,                  2.4,                    1,                   -1,                    2),
          (          -0.5628776,                  2.5,                    3,                   -1,                    2),
          (           0.2903717,                  0.5,                    2,                   -1,                    1),
          (          -0.3279141,                    3,                    2,                   -1,                    2),
          (         -0.03177644,                 1.06,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (           0.8668676,                  1.6,                    1,                -0.99,                -0.38,                 0.65,                 0.73),
          (          -0.1962725,                 0.37,                    1,                -1.43,                 -4.2,                 0.63,                 0.75),
          (          -0.1425992,                 1.33,                    3,                -0.97,                 -1.2,                 1.14,                 0.48),
          (         0.004197016,                  2.5,                    3,                -1.93,                 -0.9,                 0.09,                 2.32),
          (           0.1776584,                  0.9,                    2,                -0.92,                 -1.2,                 0.56,                  0.2),
          (         -0.04433903,                  0.5,                    2,                -1.27,                 -2.6,                  0.4,                 1.33),
          (         -0.03861246,                 0.73,                    3,                -0.87,                 -5.3,                 1.01,                 0.68),
          (          0.07399692,                  0.2,                    2,                -0.82,                 -4.4,                 0.45,                 1.11),
          (          0.02036006,                  1.5,                    3,                 -1.4,                 -4.2,                 0.85,                 1.47),
          (          0.00272825,                  1.5,                    2,                   -3,                  -25,                 0.86,                 0.99),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              5.5081,                 0.51),
          (             -14.486,                 0.94),
          (              38.241,                  1.4),
          (             -64.589,                  1.9),
          (              57.919,                  2.4),
          (              -20.55,                    3),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (            -3.69006,                0.446),
          (            -41.4239,                 1.98),
          (             220.914,                 2.75),
          (             -443.72,                  3.3),
          (              491.49,                  4.1),
          (            -296.373,                  4.8),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.0342,                    1),
          (              1.7311,                  1.5),
          (             -1.7572,                  2.3),
          (             -3.3406,                  4.6),
      };

      #endregion Saturated densities and pressure

      #region Melting pressure

      _meltingPressure_Type = '1';
      _meltingPressure_ReducingTemperature = 1;
      _meltingPressure_ReducingPressure = 1000000;
      _meltingPressure_Coefficients = new (double factor, double exponent)[][]
      {
      new (double factor, double exponent)[]
        {
            (        0.1329969885,                 1.41),
            (         -374.255624,                    0),
        },

      new (double factor, double exponent)[]{},
      new (double factor, double exponent)[]{},
      };

      #endregion Melting pressure

    }
  }
}

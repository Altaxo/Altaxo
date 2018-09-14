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
  /// State equations and constants of ammonia.
  /// Short name: ammonia.
  /// Synomym: R-717.
  /// Chemical formula: NH3.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'ammonia.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Tillner-Roth, R., Harms-Watzenberg, F., and Baehr, H.D., "Eine neue Fundamentalgleichung fuer Ammoniak," DKV-Tagungsbericht, 20:167-181, 1993.</para>
  /// <para>HeatCapacity (CPP): Tillner-Roth, R., Harms-Watzenberg, F., and Baehr, H.D., "Eine neue Fundamentalgleichung fuer Ammoniak," DKV-Tagungsbericht, 20:167-181, 1993.</para>
  /// <para>Melting pressure: Haar, L. and Gallagher, J.S., "Thermodynamic Properties of Ammonia," J. Phys. Chem. Ref. Data, 7(3):635-792, 1978.</para>
  /// <para>Saturated vapor pressure: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// <para>Saturated liquid density: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// <para>Saturated vapor density: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("7664-41-7")]
  public class Ammonia : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Ammonia Instance { get; } = new Ammonia();

    #region Constants for ammonia

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "ammonia";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "ammonia";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "R-717";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "NH3";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "7664-41-7";

    private int[] _unNumbers = new int[] { 1005, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314471;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.01703026; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 405.4;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 11333000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 13211.7771543124;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 195.495;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 6091;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 43035.3392920733;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 3.76350602709116;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 239.823552604811;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.25601;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.47;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 195.495;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 700;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 52915;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 1000000000;

    #endregion Constants for ammonia

    private Ammonia()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -16.7809579078942;
      _alpha0_n_tau = 4.97900966365871;
      _alpha0_n_lntau = -1;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          (    11.4743369557407,       0.333333333333),
          (   -1.29621078825333,                 -1.5),
          (   0.570675699335065,                -1.75),
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
          (           -1.858814,                  1.5,                    1),
          (          0.04554431,                 -0.5,                    2),
          (           0.7238548,                  0.5,                    1),
          (           0.0122947,                    1,                    4),
          (        2.141882E-11,                    3,                   15),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (          -0.0143002,                    0,                    3,                   -1,                    1),
          (           0.3441324,                    3,                    3,                   -1,                    1),
          (          -0.2873571,                    4,                    1,                   -1,                    1),
          (        2.352589E-05,                    4,                    8,                   -1,                    1),
          (         -0.03497111,                    5,                    2,                   -1,                    1),
          (         0.001831117,                    5,                    8,                   -1,                    2),
          (          0.02397852,                    3,                    1,                   -1,                    2),
          (         -0.04085375,                    6,                    1,                   -1,                    2),
          (           0.2379275,                    8,                    2,                   -1,                    2),
          (         -0.03548972,                    8,                    3,                   -1,                    2),
          (          -0.1823729,                   10,                    2,                   -1,                    2),
          (          0.02281556,                   10,                    4,                   -1,                    2),
          (        -0.006663444,                    5,                    3,                   -1,                    3),
          (        -0.008847486,                  7.5,                    1,                   -1,                    3),
          (         0.002272635,                   15,                    2,                   -1,                    3),
          (       -0.0005588655,                   30,                    4,                   -1,                    3),
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
          (              34.488,                 0.58),
          (             -128.49,                 0.75),
          (              173.82,                  0.9),
          (             -106.99,                  1.1),
          (              30.339,                  1.3),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (            -0.38435,                0.218),
          (             -4.0846,                 0.55),
          (             -6.6634,                  1.5),
          (             -31.881,                  3.7),
          (              213.06,                  5.5),
          (             -246.48,                  5.8),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.0993,                    1),
          (              -2.433,                  1.5),
          (              8.7591,                  1.7),
          (             -6.4091,                 1.95),
          (             -2.1185,                  4.2),
      };

      #endregion Saturated densities and pressure

      #region Melting pressure

      _meltingPressure_Type = '1';
      _meltingPressure_ReducingTemperature = 195.49453;
      _meltingPressure_ReducingPressure = 1000000;
      _meltingPressure_Coefficients = new (double factor, double exponent)[][]
      {
      new (double factor, double exponent)[]
        {
        },

      new (double factor, double exponent)[]
        {
        },

      new (double factor, double exponent)[]
        {
            (            2533.125,                    1),
        },

      };

      #endregion Melting pressure

    }
  }
}

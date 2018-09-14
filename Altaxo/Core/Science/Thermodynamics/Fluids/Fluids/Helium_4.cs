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
  /// State equations and constants of helium-4.
  /// Short name: helium.
  /// Synomym: R-704.
  /// Chemical formula: He.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'helium.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Ortiz-Vega, D.O., Hall, K.R., Holste, J.C., Arp, V.D., and Lemmon, E.W.,Interim equation,final equation of state to be published in J. Phys. Chem. Ref. Data, 2013.</para>
  /// <para>Melting pressure: McCarty, R.D. and Arp, V.D., "A New Wide Range Equation of State for Helium," Adv. Cryo. Eng., 35:1465-1475, 1990.</para>
  /// <para>Saturated vapor pressure:  see EOS for reference.</para>
  /// <para>Saturated liquid density:  see EOS for reference.</para>
  /// <para>Saturated vapor density:  see EOS for reference.</para>
  /// </remarks>
  [CASRegistryNumber("7440-59-7")]
  public class Helium_4 : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Helium_4 Instance { get; } = new Helium_4();

    #region Constants for helium-4

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "helium-4";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "helium";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "R-704";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "He";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "7440-59-7";

    private int[] _unNumbers = new int[] { 1046, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.004002602; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 5.1953;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 227610;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 17383.7;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 2.1768;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 5033.5;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 36477.0645831066;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 293.442213188678;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 4.22257302137216;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = -0.385;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 2.1768;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 2000;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 141220;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 1000000000;

    #endregion Constants for helium-4

    private Helium_4()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 0.159269578209168;
      _alpha0_n_tau = 0.476532113715323;
      _alpha0_n_lntau = 1.5;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
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
          (         0.014799269,                    1,                    4),
          (          3.06281562,                0.426,                    1),
          (         -4.25338698,                0.631,                    1),
          (          0.05192797,                0.596,                    2),
          (        -0.165087335,                1.705,                    2),
          (         0.087236897,                0.568,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (          2.10653786,               0.9524,                    1,                   -1,                    1),
          (          -0.6283503,                1.471,                    1,                   -1,                    2),
          (         -0.28200301,                 1.48,                    3,                   -1,                    2),
          (          1.04234019,                1.393,                    2,                   -1,                    1),
          (         -0.07620555,                3.863,                    2,                   -1,                    2),
          (         -1.35006365,                0.803,                    1,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (          0.11997252,                3.273,                    1,               -8.674,               -8.005,               1.1475,                0.912),
          (            0.107245,                 0.66,                    1,               -4.006,                -1.15,               1.7036,                 0.79),
          (         -0.35374839,                2.629,                    1,              -8.1099,               -2.143,               1.6795,              0.90567),
          (          0.75348862,               1.4379,                    2,              -0.1449,               -0.147,               0.9512,               5.1136),
          (          0.00701871,                3.317,                    2,              -0.1784,               -0.154,                4.475,               3.6022),
          (         0.226283167,               2.3676,                    2,               -2.432,               -0.701,               2.7284,               0.6488),
          (         -0.22464733,               0.7545,                    3,              -0.0414,                -0.21,               1.7167,               4.2753),
          (          0.12413584,                1.353,                    2,               -0.421,               -0.134,               1.5237,                2.744),
          (          0.00901399,                1.982,                    2,              -5.8575,              -19.256,               0.7649,               0.8736),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              1.0929,                0.286),
          (              1.6584,                  1.2),
          (             -3.6477,                    2),
          (               2.744,                  2.8),
          (             -2.3859,                  6.5),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -1.5789,                0.333),
          (             -10.749,                  1.5),
          (              17.711,                  2.1),
          (             -15.413,                  2.7),
          (             -14.352,                    9),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -3.8357,                    1),
          (              1.7062,                  1.5),
          (            -0.71231,                 1.25),
          (              1.0862,                  2.8),
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
            (          -1.7455837,                    0),
            (           1.6979793,             1.555414),
        },

      new (double factor, double exponent)[]{},
      new (double factor, double exponent)[]{},
      };

      #endregion Melting pressure

    }
  }
}

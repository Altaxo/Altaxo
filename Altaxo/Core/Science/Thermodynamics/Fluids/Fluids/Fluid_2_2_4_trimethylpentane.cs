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
  /// State equations and constants of 2,2,4-trimethylpentane.
  /// Short name: isooctane.
  /// Synomym: isobutyltrimethylmethane.
  /// Chemical formula: (CH3)2CHCH2C(CH3)3.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'ioctane.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Blackham, T.M. and Lemmon, E.W.to be published in Int. J. Thermophys., 2011.</para>
  /// <para>HeatCapacity (CPP): Blackham, T.M. and Lemmon, E.W.</para>
  /// <para>Saturated vapor pressure: See EOS</para>
  /// <para>Saturated liquid density: See EOS</para>
  /// <para>Saturated vapor density: See EOS</para>
  /// </remarks>
  [CASRegistryNumber("540-84-1")]
  public class Fluid_2_2_4_trimethylpentane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Fluid_2_2_4_trimethylpentane Instance { get; } = new Fluid_2_2_4_trimethylpentane();

    #region Constants for 2,2,4-trimethylpentane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "2,2,4-trimethylpentane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "isooctane";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "isobutyltrimethylmethane";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "(CH3)2CHCH2C(CH3)3";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "br-alkane";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "540-84-1";

    private int[] _unNumbers = new int[] { 1262, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.11422852; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 544;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 2572000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 2120;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 165.77;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.01796;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 6961.07620120858;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 1.30322349639554E-05;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 372.358256398584;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.303;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 165.77;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 600;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 6970;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 1000000000;

    #endregion Constants for 2,2,4-trimethylpentane

    private Fluid_2_2_4_trimethylpentane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 8.14014579685024;
      _alpha0_n_tau = -4.34157173971683;
      _alpha0_n_lntau = 9.76;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (               15.48,     1.42463235294118),
          (               34.42,     3.49264705882353),
          (               21.42,                9.375),
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
          (           0.0568901,                    1,                    4),
          (             1.96155,                  0.3,                    1),
          (            -2.81164,                 0.75,                    1),
          (           -0.815112,                 1.11,                    2),
          (            0.326583,                 0.55,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (            -1.60893,                  2.2,                    1,                   -1,                    2),
          (           -0.454734,                  3.7,                    3,                   -1,                    2),
          (             1.08306,                 1.53,                    2,                   -1,                    1),
          (           -0.722876,                  2.1,                    2,                   -1,                    2),
          (          -0.0434052,                  0.9,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (             1.96648,                 0.88,                    1,                -0.75,                -0.59,                 1.44,                 0.66),
          (           -0.465082,                  1.1,                    1,                -1.13,                -1.45,                 0.68,                  0.9),
          (           -0.409398,                 2.75,                    3,                -0.87,                 -0.5,                 0.51,                 0.54),
          (           0.0232131,                    1,                    3,                -4.73,               -10.52,                  0.8,                 0.18),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              1.1535,                0.286),
          (              1.3709,                 0.54),
          (             0.38804,                  3.3),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -2.5793,                0.366),
          (             -6.4934,                 1.11),
          (             -18.631,                    3),
          (             -54.123,                  6.4),
          (             -123.58,                   14),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.7985,                    1),
          (               8.128,                  1.5),
          (             -7.3106,                  1.6),
          (             -3.9392,                    4),
          (             -1.6732,                   16),
      };

      #endregion Saturated densities and pressure

    }
  }
}

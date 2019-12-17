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
  /// State equations and constants of methyl (Z,Z,Z)-9,12,15-octadecatrienoate.
  /// Short name: methyl linolenate.
  /// Synomym: methyl ester linolenic acid.
  /// Chemical formula: C19H32O2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'mlinolen.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS):  Huber, M.L., Lemmon, E.W., Kazakov, A., Ott, L.S., and Bruno, T.J. "Model for the Thermodynamic Properties of a Biodiesel Fuel," Energy &amp; Fuels, 23:3790-3797, 2009.</para>
  /// <para>HeatCapacity (CPP):  TDE 3.0 internal version, March 2008, Planck-Einstein form based on estimation from Joback method, uncertainty 10%</para>
  /// <para>Saturated vapor pressure: Cullimore, I.D., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("301-00-8")]
  public class Methyl_Z_Z_Z__9_12_15_octadecatrienoate : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Methyl_Z_Z_Z__9_12_15_octadecatrienoate Instance { get; } = new Methyl_Z_Z_Z__9_12_15_octadecatrienoate();

    #region Constants for methyl (Z,Z,Z)-9,12,15-octadecatrienoate

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "methyl (Z,Z,Z)-9,12,15-octadecatrienoate";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "methyl linolenate";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "methyl ester linolenic acid";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C19H32O2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "301-00-8";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.29245618; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 772;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 1369000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 847.3;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 218.65;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 8.28E-12;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = double.NaN;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = double.NaN;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 629.129108153122;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 1.14;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.54;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 218.65;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 1000;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 3290;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 50000000;

    #endregion Constants for methyl (Z,Z,Z)-9,12,15-octadecatrienoate

    private Methyl_Z_Z_Z__9_12_15_octadecatrienoate()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 199.73645387554;
      _alpha0_n_tau = -31.8765773389633;
      _alpha0_n_lntau = -1;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          (   -152.994181938524,            -0.214648),
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (    34.9245267769258,     1.57155440414508),
          (    9.79404344617433,    0.749678756476684),
          (    57.1149917878129,     3.62667098445596),
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
          (          0.04070829,                    1,                    4),
          (            2.412375,                 0.15,                    1),
          (           -3.756194,                 1.24,                    1),
          (          -0.1526466,                  1.6,                    2),
          (          0.04682918,                 1.28,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (           -1.470958,                  2.9,                    1,                   -1,                    2),
          (            -0.76455,                 3.15,                    3,                   -1,                    2),
          (            1.908964,                 2.16,                    2,                   -1,                    1),
          (           -1.629366,                  2.8,                    2,                   -1,                    2),
          (         -0.01242073,                  1.4,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (            2.180707,                  2.5,                    1,                 -1.1,                 -0.9,                 1.14,                 0.79),
          (          -0.7537264,                    3,                    1,                 -1.6,                -0.65,                 0.65,                  0.9),
          (          -0.4347781,                  3.1,                    3,                 -1.1,                -0.75,                 0.77,                 0.76),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              16.258,                0.681),
          (             -194.65,                 1.26),
          (              1423.1,                 1.58),
          (               -2242,                  1.7),
          (              1001.2,                  1.8),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -11.463,                 0.65),
          (              45.192,                 1.55),
          (             -65.779,                  1.8),
          (             -1838.6,                  6.6),
          (              4068.9,                  7.2),
          (             -2512.4,                  7.8),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -14.239,                    1),
          (              8.5361,                  1.5),
          (             -29.678,                 2.86),
          (              29.106,                  3.5),
          (             -16.922,                  4.6),
      };

      #endregion Saturated densities and pressure

    }
  }
}

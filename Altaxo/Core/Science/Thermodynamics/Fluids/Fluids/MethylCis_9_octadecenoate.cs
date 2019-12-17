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
  /// State equations and constants of methyl cis-9-octadecenoate.
  /// Short name: methyl oleate.
  /// Synomym: methyl ester oleic acid.
  /// Chemical formula: C19H36O2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'moleate.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS):  Huber, M.L., Lemmon, E.W., Kazakov, A., Ott, L.S., and Bruno, T.J. "Model for the Thermodynamic Properties of a Biodiesel Fuel," Energy &amp; Fuels, 23:3790-3797, 2009.</para>
  /// <para>HeatCapacity (CPP):  TDE 3.0 internal version, March 2008, Planck-Einstein form based on estimation from Joback method, uncertainty 10%</para>
  /// <para>Saturated vapor pressure: Cullimore, I.D., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("112-62-9")]
  public class MethylCis_9_octadecenoate : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static MethylCis_9_octadecenoate Instance { get; } = new MethylCis_9_octadecenoate();

    #region Constants for methyl cis-9-octadecenoate

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "methyl cis-9-octadecenoate";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "methyl oleate";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "methyl ester oleic acid";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C19H36O2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "112-62-9";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.29648794; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 782;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 1246000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 812.85;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 253.47;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 3.781E-07;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = double.NaN;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = double.NaN;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 627.176002226324;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.906;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.63;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 253.47;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 1000;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 3050;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 50000000;

    #endregion Constants for methyl cis-9-octadecenoate

    private MethylCis_9_octadecenoate()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 230.178780467906;
      _alpha0_n_tau = -34.1982753008324;
      _alpha0_n_lntau = -1;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          (   -171.543285786591,            -0.146118),
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (    28.2395562821067,    0.784563938618926),
          (    40.3835625401108,      1.7970716112532),
          (    51.9167061961361,     3.66721227621483),
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
          (          0.04596121,                    1,                    4),
          (              2.2954,                 0.34,                    1),
          (           -3.554366,                 1.14,                    1),
          (          -0.2291674,                  1.4,                    2),
          (          0.06854534,                  0.6,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (           -1.535778,                  3.3,                    1,                   -1,                    2),
          (          -0.7334697,                  4.1,                    3,                   -1,                    2),
          (              1.7127,                  1.9,                    2,                   -1,                    1),
          (           -1.471394,                  3.8,                    2,                   -1,                    2),
          (         -0.01724678,                  1.3,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (             2.11547,                  3.4,                    1,                 -1.1,                 -0.9,                 1.14,                 0.79),
          (          -0.7555374,                  3.8,                    1,                 -1.6,                -0.65,                 0.65,                  0.9),
          (          -0.4134269,                    4,                    3,                 -1.1,                -0.75,                 0.77,                 0.76),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              -19.92,                0.461),
          (               122.3,                  0.6),
          (             -235.82,                 0.75),
          (              210.09,                 0.91),
          (             -73.435,                 1.05),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -13.426,                0.667),
          (              180.69,                 1.71),
          (             -1110.8,                  2.2),
          (              1326.5,                 2.46),
          (             -464.21,                    3),
          (              -210.7,                  9.7),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (               -13.9,                    1),
          (              16.246,                  1.5),
          (             -15.568,                 1.93),
          (             -7.3568,                  4.2),
          (             -4.8739,                    8),
      };

      #endregion Saturated densities and pressure

    }
  }
}

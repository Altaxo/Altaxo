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
  /// State equations and constants of methyl trifluoromethyl ether.
  /// Short name: RE143a.
  /// Synomym: HFE-143a.
  /// Chemical formula: CH3-O-CF3.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 're143a.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Akasaka, R. and Kayukawa, Y. "A Fundamental Equation of State for Trifluoromethyl Methyl Ether  (HFE-143m) and Its Application to Refrigeration Cycle Analysis," Int. J. Refrig., 35(4):1003-1013, 2012.</para>
  /// <para>HeatCapacity (CPP): see EOS of Akasaka and Kayukawa (2012).</para>
  /// <para>Saturated vapor pressure: See EOS</para>
  /// <para>Saturated liquid density: See EOS</para>
  /// <para>Saturated vapor density: See EOS</para>
  /// </remarks>
  [CASRegistryNumber("421-14-7")]
  public class MethylTrifluoromethylEther : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static MethylTrifluoromethylEther Instance { get; } = new MethylTrifluoromethylEther();

    #region Constants for methyl trifluoromethyl ether

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "methyl trifluoromethyl ether";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "RE143a";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "HFE-143a";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CH3-O-CF3";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "421-14-7";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.1000398; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 377.921;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3635000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 4648.140744;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 240;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 65350;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 12615.4015268133;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 34.0154110473009;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 249.572197590674;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.289;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 2.32;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 240;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 420;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 12620;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 7200000;

    #endregion Constants for methyl trifluoromethyl ether

    private MethylTrifluoromethylEther()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -7.57307006765368;
      _alpha0_n_tau = 8.09228965002623;
      _alpha0_n_lntau = 1.4499451077591;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          (   -6.63165068088509,                   -1),
          (   0.558278285480124,                   -2),
          ( -0.0251559143297498,                   -3),
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
          (           7.7715884,                0.682,                    1),
          (           -8.704275,                0.851,                    1),
          (         -0.28095049,                 1.84,                    1),
          (          0.14540153,                 1.87,                    2),
          (        0.0092291277,                0.353,                    5),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (          -0.2141651,                 3.92,                    1,                   -1,                    1),
          (         0.099475155,                 1.14,                    3,                   -1,                    1),
          (         0.023247135,                0.104,                    5,                   -1,                    1),
          (        -0.012873573,                 1.19,                    7,                   -1,                    1),
          (        -0.057366549,                 6.58,                    1,                   -1,                    2),
          (           0.3650465,                 6.73,                    2,                   -1,                    2),
          (         -0.25433763,                 7.99,                    2,                   -1,                    2),
          (        -0.090896436,                 7.31,                    3,                   -1,                    2),
          (         0.083503619,                 7.45,                    4,                   -1,                    2),
          (         0.015477603,                 16.5,                    2,                   -1,                    3),
          (        -0.016641941,                 24.8,                    3,                   -1,                    3),
          (        0.0052410163,                 10.5,                    5,                   -1,                    3),
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
          (             1.20552,                 0.33),
          (             1.33568,                  0.5),
          (           0.0981486,                  1.5),
          (            0.248917,                  2.5),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (            -3.02576,                 0.38),
          (            -6.97239,                 1.24),
          (            -20.2601,                  3.2),
          (            -53.4441,                  6.9),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (            -7.44314,                    1),
          (             1.69164,                  1.5),
          (            -2.27778,                  2.5),
          (              -4.094,                    5),
      };

      #endregion Saturated densities and pressure

    }
  }
}

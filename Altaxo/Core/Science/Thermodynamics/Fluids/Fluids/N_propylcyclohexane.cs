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
  /// State equations and constants of n-propylcyclohexane.
  /// Short name: propylcyclohexane.
  /// Synomym: propylcyclohexane.
  /// Chemical formula: (C6H11)CH2CH2CH3.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'c3cc6.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>HeatCapacity (CPP): ThermoData Engine (TRC, NIST)</para>
  /// <para>Saturated vapor pressure: Lemmon, E.W., 2010.</para>
  /// <para>Saturated liquid density: Lemmon, E.W., 2010.</para>
  /// <para>Saturated vapor density: Lemmon, E.W., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("1678-92-8")]
  public class N_propylcyclohexane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static N_propylcyclohexane Instance { get; } = new N_propylcyclohexane();

    #region Constants for n-propylcyclohexane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "n-propylcyclohexane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "propylcyclohexane";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "propylcyclohexane";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "(C6H11)CH2CH2CH3";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "naphthene";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "1678-92-8";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.12623922; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 630.8;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 2860000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 2060;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 178.2;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.0007;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 7027.19768068141;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 4.84590196996819E-07;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 429.855979398564;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.33;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = -1;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 178.2;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 650;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 7030;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 50000000;

    #endregion Constants for n-propylcyclohexane

    private N_propylcyclohexane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 36.0099215193789;
      _alpha0_n_tau = -4.16497970167273;
      _alpha0_n_lntau = -1;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          (   -25.1543249065274,            -0.385871),
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (   0.164834279314429,     274.722574508561),
          (    12.8000912144511,    0.889568801521877),
          (    37.7309587427801,      3.0429930247305),
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
          (             1.01911,                  0.2,                    1),
          (            -2.59762,                  1.2,                    1),
          (            0.675152,                  1.8,                    1),
          (           -0.230891,                  1.5,                    2),
          (            0.120966,                  0.3,                    3),
          (         0.000309038,                  0.9,                    7),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (            0.526461,                  1.4,                    2,                   -1,                    1),
          (          -0.0188462,                  2.2,                    5,                   -1,                    1),
          (           -0.549272,                  3.7,                    1,                   -1,                    2),
          (           -0.139233,                  4.2,                    4,                   -1,                    2),
          (            0.121242,                  2.4,                    1,                   -1,                    1),
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
          (            0.039271,                  0.1),
          (              38.257,                 0.75),
          (             -65.743,                 0.87),
          (              30.332,                    1),
          (             0.17224,                    5),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -6.4572,                  0.6),
          (              9.1228,                  1.8),
          (             -25.806,                  2.2),
          (             -59.044,                    6),
          (             -147.09,                   14),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.6296,                    1),
          (              1.6538,                  1.5),
          (             -2.8518,                  2.7),
          (             -2.8205,                  4.7),
          (             -2.8144,                   15),
      };

      #endregion Saturated densities and pressure

    }
  }
}

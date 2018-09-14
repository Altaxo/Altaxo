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
  /// State equations and constants of 1,1,2-trichloro-1,2,2-trifluoroethane.
  /// Short name: R113.
  /// Synomym: CFC-113.
  /// Chemical formula: CCl2FCClF2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'r113.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Marx, V., Pruss, A., and Wagner, W., "Neue Zustandsgleichungen fuer R 12, R 22, R 11 und R 113.  Beschreibung des thermodynamishchen Zustandsverhaltens bei Temperaturen bis 525 K und Druecken bis 200 MPa," Duesseldorf, VDI Verlag, Series 19 (Waermetechnik/Kaeltetechnik), No. 57, 1992.</para>
  /// <para>HeatCapacity (CPP): Marx, V., Pruss, A., and Wagner, W., "Neue Zustandsgleichungen fuer R 12, R 22, R 11 und R 113.  Beschreibung des thermodynamishchen Zustandsverhaltens bei Temperaturen bis 525 K und Druecken bis 200 MPa," Duesseldorf, VDI Verlag, Series 19 (Waermetechnik/Kaeltetechnik), No. 57, 1992.</para>
  /// <para>Saturated vapor pressure: Cullimore, I.D., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("76-13-1")]
  public class Fluid_1_1_2_trichloro_1_2_2_trifluoroethane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Fluid_1_1_2_trichloro_1_2_2_trifluoroethane Instance { get; } = new Fluid_1_1_2_trichloro_1_2_2_trifluoroethane();

    #region Constants for 1,1,2-trichloro-1,2,2-trifluoroethane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "1,1,2-trichloro-1,2,2-trifluoroethane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "R113";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "CFC-113";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CCl2FCClF2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "halocb";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "76-13-1";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314471;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.187375; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 487.21;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3392200;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 2988.659;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 236.93;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 1871;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 9098.73877849907;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.951990576014559;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 320.735175531632;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.25253;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0.803;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 236.93;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 525;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 9100;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 200000000;

    #endregion Constants for 1,1,2-trichloro-1,2,2-trifluoroethane

    private Fluid_1_1_2_trichloro_1_2_2_trifluoroethane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -21.8557695997667;
      _alpha0_n_tau = 11.9424414969471;
      _alpha0_n_lntau = 3.00000129059512;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (          12.4464495,     1.04971737033312),
          (          2.72181845,     3.29788641448246),
          (         0.692712415,     8.62650811764947),
          (          3.32248298,     3.29670446008908),
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
          (        0.8432092286,                  0.5,                    1),
          (        -2.019185967,                  1.5,                    1),
          (        0.2920612996,                  1.5,                    2),
          (       0.05323107661,                 -0.5,                    3),
          (      0.003214971931,                    2,                    4),
          (     4.667858574E-05,                    0,                    8),
          (    -1.227522799E-06,                    3,                    8),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (        0.8167288718,                 -0.5,                    3,                   -1,                    1),
          (        -1.340790803,                    0,                    3,                   -1,                    1),
          (        0.4065752705,                    2,                    3,                   -1,                    1),
          (       -0.1534754634,                  1.5,                    5,                   -1,                    1),
          (      -0.02414435149,                    6,                    1,                   -1,                    2),
          (      -0.02113056197,                    2,                    2,                   -1,                    2),
          (      -0.03565436205,                   10,                    2,                   -1,                    2),
          (      0.001364654968,                    6,                    9,                   -1,                    2),
          (      -0.01251838755,                   18,                    3,                   -1,                    3),
          (     -0.001385761351,                   15,                    7,                   -1,                    3),
          (     0.0007206335486,                   33,                    8,                   -1,                    4),
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
          (              1.5785,                  0.3),
          (              1.2404,                  0.7),
          (            -0.66933,                    2),
          (              4.9775,                    4),
          (             -5.5253,                    5),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -2.6225,                0.379),
          (             -6.0753,                 1.13),
          (             -15.768,                  2.9),
          (             -42.361,                    6),
          (             -7.9071,                    7),
          (             -319.66,                   15),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.3838,                    1),
          (              3.2594,                  1.5),
          (             -2.7761,                  1.8),
          (             -3.7758,                  4.3),
          (            -0.19921,                  6.2),
      };

      #endregion Saturated densities and pressure

    }
  }
}

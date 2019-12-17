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
  /// State equations and constants of dichlorodifluoromethane.
  /// Short name: R12.
  /// Synomym: CFC-12.
  /// Chemical formula: CCl2F2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'r12.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Marx, V., Pruss, A., and Wagner, W., "Neue Zustandsgleichungen fuer R 12, R 22, R 11 und R 113.  Beschreibung des thermodynamishchen Zustandsverhaltens bei Temperaturen bis 525 K und Druecken bis 200 MPa," Duesseldorf, VDI Verlag, Series 19 (Waermetechnik/Kaeltetechnik), No. 57, 1992.</para>
  /// <para>HeatCapacity (CPP): Marx, V., Pruss, A., and Wagner, W., "Neue Zustandsgleichungen fuer R 12, R 22, R 11 und R 113.  Beschreibung des thermodynamishchen Zustandsverhaltens bei Temperaturen bis 525 K und Druecken bis 200 MPa," Duesseldorf, VDI Verlag, Series 19 (Waermetechnik/Kaeltetechnik), No. 57, 1992.</para>
  /// <para>Saturated vapor pressure: Cullimore, I.D., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("75-71-8")]
  public class Dichlorodifluoromethane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Dichlorodifluoromethane Instance { get; } = new Dichlorodifluoromethane();

    #region Constants for dichlorodifluoromethane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "dichlorodifluoromethane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "R12";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "CFC-12";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CCl2F2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "halocb";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "75-71-8";

    private int[] _unNumbers = new int[] { 1028, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314471;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.120913; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 385.12;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 4136100;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 4672.781;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 116.099;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.2425;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 15125.2904818736;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.000251269102693918;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 243.397713977771;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.17948;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0.51;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 116.099;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 525;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 15130;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 200000000;

    #endregion Constants for dichlorodifluoromethane

    private Dichlorodifluoromethane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -14.7178750731462;
      _alpha0_n_tau = 9.40300232133876;
      _alpha0_n_lntau = 3.00363864927222;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (         3.160638395,     3.72204559617781),
          (        0.3712598774,     6.30985095554632),
          (         3.562277099,      1.7803788948899),
          (         2.121533311,     1.07087606460324),
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
          (         2.075343402,                  0.5,                    1),
          (        -2.962525996,                    1,                    1),
          (       0.01001589616,                    2,                    1),
          (       0.01781347612,                  2.5,                    2),
          (       0.02556929157,                 -0.5,                    4),
          (      0.002352142637,                    0,                    6),
          (    -8.495553314E-05,                    0,                    8),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (      -0.01535945599,                 -0.5,                    1,                   -1,                    1),
          (       -0.2108816776,                  1.5,                    1,                   -1,                    1),
          (      -0.01654228806,                  2.5,                    5,                   -1,                    1),
          (       -0.0118131613,                 -0.5,                    7,                   -1,                    1),
          (     -4.16029583E-05,                    0,                   12,                   -1,                    1),
          (     2.784861664E-05,                  0.5,                   12,                   -1,                    1),
          (     1.618686433E-06,                 -0.5,                   14,                   -1,                    1),
          (       -0.1064614686,                    4,                    1,                   -1,                    2),
          (     0.0009369665207,                    4,                    9,                   -1,                    2),
          (       0.02590095447,                    2,                    1,                   -1,                    3),
          (      -0.04347025025,                    4,                    1,                   -1,                    3),
          (        0.1012308449,                   12,                    3,                   -1,                    3),
          (       -0.1100003438,                   14,                    3,                   -1,                    3),
          (     -0.003361012009,                    0,                    5,                   -1,                    3),
          (     0.0003789190008,                   14,                    9,                   -1,                    4),
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
          (              32.983,                 0.57),
          (             -109.97,                 0.72),
          (              170.67,                 0.89),
          (             -133.42,                 1.07),
          (              42.525,                 1.25),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              -3.153,                0.418),
          (             -6.4734,                 1.32),
          (             -17.346,                  3.3),
          (             -15.918,                  6.6),
          (             -32.492,                    7),
          (             -120.72,                   15),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.0834,                    1),
          (              4.3562,                  1.5),
          (             -3.5249,                 1.67),
          (             -2.8872,                 4.14),
          (            -0.89926,                   10),
      };

      #endregion Saturated densities and pressure

    }
  }
}

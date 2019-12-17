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
  /// State equations and constants of dichlorofluoromethane.
  /// Short name: R21.
  /// Synomym: HCFC-21.
  /// Chemical formula: CHCl2F.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'r21.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Platzer, B., Polt, A., and Maurer, G.,"Thermophysical properties of refrigerants,"Berlin,  Springer-Verlag, 1990.</para>
  /// <para>HeatCapacity (CPP): Platzer, B., Polt, A., and Maurer, G.,"Thermophysical properties of refrigerants,"Berlin,  Springer-Verlag, 1990.</para>
  /// <para>Saturated vapor pressure: Cullimore, I.D., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("75-43-4")]
  public class Dichlorofluoromethane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Dichlorofluoromethane Instance { get; } = new Dichlorofluoromethane();

    #region Constants for dichlorofluoromethane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "dichlorofluoromethane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "R21";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "HCFC-21";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CHCl2F";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "halocb";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "75-43-4";

    private int[] _unNumbers = new int[] { 1029, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.31451;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.10292; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 451.48;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 5181200;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 5110.7656;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 142.8;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.06828;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = double.NaN;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = double.NaN;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 282.011927968133;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.2061;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.37;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 200;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 473;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 15360;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 138000000;

    #endregion Constants for dichlorofluoromethane

    private Dichlorofluoromethane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -17.9993885955631;
      _alpha0_n_tau = 11.4378856842308;
      _alpha0_n_lntau = 0.156871455421687;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          (      -0.28701328542,                   -1),
          (  -0.011011639345357,                   -2),
          (  0.0191142351503355,                   -3),
          (-0.00356736028368424,                   -4),
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
          (       -44.386484873,                    3,                    0),
          (       9.26505600935,                    4,                    0),
          (     -0.551709104376,                    5,                    0),
          (      0.504676623431,                    0,                    1),
          (     -0.732431415692,                    1,                    1),
          (     -0.868403860387,                    2,                    1),
          (      0.146234705555,                    3,                    1),
          (     -0.280576335053,                    4,                    1),
          (      0.864743656093,                    0,                    2),
          (      -2.70767233732,                    1,                    2),
          (       3.30476390706,                    2,                    2),
          (     -0.210878239171,                    0,                    3),
          (      0.449531449589,                    1,                    3),
          (      0.120779813143,                    0,                    4),
          (     -0.277297953777,                    1,                    4),
          (     0.0305441291172,                    1,                    5),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (        44.386484873,                    3,                    0,          -0.07470252,                    2),
          (      -9.26505600935,                    4,                    0,          -0.07470252,                    2),
          (      0.551709104376,                    5,                    0,          -0.07470252,                    2),
          (       1.21128809552,                    3,                    2,          -0.07470252,                    2),
          (      0.167119476587,                    4,                    2,          -0.07470252,                    2),
          (    -0.0504876793028,                    5,                    2,          -0.07470252,                    2),
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
          (             0.33546,                 0.09),
          (              18.208,                 0.78),
          (               -26.4,                 0.92),
          (              10.586,                  1.1),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (            -0.38213,                 0.09),
          (             -5.5559,                0.667),
          (             -15.886,                  2.5),
          (             -44.766,                    6),
          (             -276.06,                   15),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.0336,                    1),
          (              1.5672,                  1.5),
          (             -3.3932,                    3),
          (              1.7582,                    7),
          (             -8.6765,                   10),
      };

      #endregion Saturated densities and pressure

    }
  }
}

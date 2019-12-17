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
  /// State equations and constants of difluoromethane.
  /// Short name: R32.
  /// Synomym: HFC-32.
  /// Chemical formula: CH2F2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'r32.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Tillner-Roth, R. and Yokozeki, A., "An international standard equation of state for difluoromethane (R-32) for temperatures from the triple point at 136.34 K to 435 K and pressures up to 70 MPa," J. Phys. Chem. Ref. Data, 26(6):1273-1328, 1997.</para>
  /// <para>HeatCapacity (CPP): Tillner-Roth, R. and Yokozeki, A., "An international standard equation of state for difluoromethane (R-32) for temperatures from the triple point at 136.34 K to 435 K and pressures up to 70 MPa," J. Phys. Chem. Ref. Data, 26(6):1273-1328, 1997.</para>
  /// <para>Saturated vapor pressure: Cullimore, I.D., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("75-10-5")]
  public class Difluoromethane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Difluoromethane Instance { get; } = new Difluoromethane();

    #region Constants for difluoromethane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "difluoromethane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "R32";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "HFC-32";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CH2F2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "halocb";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "75-10-5";

    private int[] _unNumbers = new int[] { 3252, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314471;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.052024; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 351.255;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 5782000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 8150.0846;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 136.34;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 48;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 27473.3449900023;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.0423525443559032;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 221.498656013835;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.2769;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.978;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 136.34;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 435;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 27473.4;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 70000000;

    #endregion Constants for difluoromethane

    private Difluoromethane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -8.25809595998889;
      _alpha0_n_tau = 6.35309775823427;
      _alpha0_n_lntau = 3.004486;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (            1.160761,     2.27185378144083),
          (            2.645151,     11.9144211470299),
          (            5.794987,     5.14156382115557),
          (            1.129475,     32.7682168225363),
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
          (            1.046634,                 0.25,                    1),
          (          -0.5451165,                    1,                    2),
          (        -0.002448595,                -0.25,                    5),
          (         -0.04877002,                   -1,                    1),
          (          0.03520158,                    2,                    1),
          (          0.00162275,                    2,                    3),
          (        2.377225E-05,                 0.75,                    8),
          (            0.029149,                 0.25,                    4),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (         0.003386203,                   18,                    4,                   -1,                    4),
          (        -0.004202444,                   26,                    4,                   -1,                    3),
          (        0.0004782025,                   -1,                    8,                   -1,                    1),
          (        -0.005504323,                   25,                    3,                   -1,                    4),
          (         -0.02418396,                 1.75,                    5,                   -1,                    1),
          (           0.4209034,                    4,                    1,                   -1,                    2),
          (          -0.4616537,                    5,                    1,                   -1,                    2),
          (           -1.200513,                    1,                    3,                   -1,                    1),
          (            -2.59155,                  1.5,                    1,                   -1,                    1),
          (           -1.400145,                    1,                    2,                   -1,                    1),
          (           0.8263017,                  0.5,                    3,                   -1,                    1),
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
          (              1.2584,                 0.27),
          (               4.641,                  0.8),
          (              -5.487,                  1.1),
          (              3.3115,                  1.5),
          (             -0.6137,                  1.8),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -2.2002,                0.336),
          (              -5.972,                 0.98),
          (             -14.571,                  2.7),
          (             -42.598,                  5.7),
          (              4.2686,                  6.5),
          (             -73.373,                   11),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.4883,                    1),
          (              1.9697,                  1.5),
          (             -1.7496,                  2.2),
          (             -4.0224,                  4.8),
          (              1.5209,                  6.2),
      };

      #endregion Saturated densities and pressure

    }
  }
}

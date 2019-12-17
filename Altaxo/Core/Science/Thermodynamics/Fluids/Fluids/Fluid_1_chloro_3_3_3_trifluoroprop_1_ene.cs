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
  /// State equations and constants of 1-chloro-3,3,3-trifluoroprop-1-ene.
  /// Short name: R1233zd(E).
  /// Synomym: HFO-1233zd(E).
  /// Chemical formula: CHCl=CH-CF3.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'r1233zd.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Mondejar, M.E., McLinden, M.O., Lemmon, E.W."Thermodynamic Properties of Trans-1-chloro-3,3,3-trifluoropropene (R1233zd(E)): Vapor Pressure, p-rho-T Data, Speed of Sound Measurements and Equation of State," J. Chem. Eng. Data, 60:2477-2489, 2015. DOI: 10.1021/acs.jced.5b00348</para>
  /// <para>Saturated vapor pressure: See EOS</para>
  /// <para>Saturated liquid density: See EOS</para>
  /// <para>Saturated vapor density: See EOS</para>
  /// </remarks>
  [CASRegistryNumber("102687-65-0")]
  public class Fluid_1_chloro_3_3_3_trifluoroprop_1_ene : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Fluid_1_chloro_3_3_3_trifluoroprop_1_ene Instance { get; } = new Fluid_1_chloro_3_3_3_trifluoroprop_1_ene();

    #region Constants for 1-chloro-3,3,3-trifluoroprop-1-ene

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "1-chloro-3,3,3-trifluoroprop-1-ene";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "R1233zd(E)";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "HFO-1233zd(E)";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CHCl=CH-CF3";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "halocb";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "102687-65-0";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.1304961896; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 438.75;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3572600;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 3670;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 195.15;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 250;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 11408.679640913;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.154303919430212;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 291.463530231043;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.305;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.44;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 195.15;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 550;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 11410;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 100000000;

    #endregion Constants for 1-chloro-3,3,3-trifluoroprop-1-ene

    private Fluid_1_chloro_3_3_3_trifluoroprop_1_ene()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -14.3975405923364;
      _alpha0_n_tau = 9.54092596444082;
      _alpha0_n_lntau = 3;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (               8.962,    0.911680911680912),
          (               11.94,     4.33048433048433),
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
          (          0.03920261,                    1,                    4),
          (            1.639052,                 0.24,                    1),
          (           -1.997147,                 0.83,                    1),
          (          -0.6603372,                 1.17,                    2),
          (           0.1498682,                  0.6,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (           -1.408791,                  2.2,                    1,                   -1,                    2),
          (          -0.7920426,                 2.88,                    3,                   -1,                    2),
          (           0.8549678,                  1.1,                    2,                   -1,                    1),
          (           -0.530192,                    2,                    2,                   -1,                    2),
          (         -0.01408562,                 1.07,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (            1.335117,                 1.27,                    1,               -1.215,                -1.27,                 1.32,                 0.77),
          (          -0.5441797,                 1.94,                    1,                 -1.5,                -0.82,                 0.82,                0.976),
          (         -0.05862723,                    2,                    3,                 -1.1,                -0.94,                 0.66,                 1.08),
          (         -0.04123614,                  1.5,                    2,                -2.52,                  -20,                 0.66,                 0.62),
          (          -0.6619106,                    1,                    3,                -4.55,                  -32,                 1.39,                 0.61),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             2.13083,                0.355),
          (            0.583568,                  0.9),
          (            0.247871,                  3.5),
          (            0.472173,                    8),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -3.0152,                0.397),
          (             -6.5621,                  1.2),
          (             -19.427,                  3.1),
          (              -62.65,                  6.6),
          (             -181.64,                   15),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.6021,                    1),
          (              2.3265,                  1.5),
          (             -1.9771,                    2),
          (             -4.8451,                  4.3),
          (             -4.8762,                   14),
      };

      #endregion Saturated densities and pressure

    }
  }
}

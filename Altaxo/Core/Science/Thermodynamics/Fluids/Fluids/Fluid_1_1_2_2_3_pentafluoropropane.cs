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
  /// State equations and constants of 1,1,2,2,3-pentafluoropropane.
  /// Short name: R245ca.
  /// Synomym: HFC-245ca.
  /// Chemical formula: CHF2CF2CH2F.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'r245ca.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Zhou, Y., Lemmon, E.W. "Equation of State for the Thermodynamic Properties of 1,1,2,2,3-Pentafluoropropane (R-245ca)," Int. J. Thermophys., 37:27, 2016.</para>
  /// <para>HeatCapacity (CPP): see EOS</para>
  /// <para>Saturated vapor pressure: Herrig, S., 2013.</para>
  /// <para>Saturated liquid density: Herrig, S., 2013.</para>
  /// <para>Saturated vapor density: Herrig, S., 2013.</para>
  /// </remarks>
  [CASRegistryNumber("679-86-7")]
  public class Fluid_1_1_2_2_3_pentafluoropropane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Fluid_1_1_2_2_3_pentafluoropropane Instance { get; } = new Fluid_1_1_2_2_3_pentafluoropropane();

    #region Constants for 1,1,2,2,3-pentafluoropropane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "1,1,2,2,3-pentafluoropropane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "R245ca";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "HFC-245ca";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CHF2CF2CH2F";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "halocb";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "679-86-7";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.13404794; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 447.57;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3940700;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 3920;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 191.5;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 70.8;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 12207.0077842969;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.0444733878192057;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 298.412168948695;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.355;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.74;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 191.5;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 450;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 12210;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 10000000;

    #endregion Constants for 1,1,2,2,3-pentafluoropropane

    private Fluid_1_1_2_2_3_pentafluoropropane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -18.0940811126725;
      _alpha0_n_tau = 8.99607608679155;
      _alpha0_n_lntau = 7.888;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (              0.8843,     1.93265857854637),
          (               5.331,       6.323033268539),
          (               14.46,     2.50687043367518),
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
          (          0.04489247,                    1,                    4),
          (            1.526476,                 0.26,                    1),
          (            -2.40832,                    1,                    1),
          (          -0.5288088,                  1.2,                    2),
          (          0.18222346,                 0.67,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (           -1.063228,                 1.92,                    1,                   -1,                    2),
          (           -0.223149,                    2,                    3,                   -1,                    2),
          (             1.18738,                  1.5,                    2,                   -1,                    1),
          (          -0.9772383,                 1.93,                    2,                   -1,                    2),
          (         -0.02296938,                 1.06,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (            1.364444,                 0.17,                    1,                -1.16,                 -2.4,                1.265,                 0.55),
          (          -0.5080666,                  3.9,                    1,                 -1.1,                 -1.5,                 0.42,                0.724),
          (         -0.06649496,                    1,                    3,                -1.64,                 -4.2,                0.864,                0.524),
          (           -1.128359,                    1,                    3,                -13.8,                 -379,                 1.15,                0.857),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              4.0176,                 0.48),
          (             -4.7916,                    1),
          (              7.8662,                 1.62),
          (             -7.1049,                  2.3),
          (              3.1949,                  3.1),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (            -4.65885,                  0.5),
          (            -1.03328,                 1.09),
          (            -13.5047,                  2.1),
          (            -48.4225,                  5.1),
          (            -104.097,                 10.4),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.8807,                    1),
          (              2.1026,                  1.5),
          (             -3.0768,                  2.5),
          (             -4.9894,                 4.95),
      };

      #endregion Saturated densities and pressure

    }
  }
}

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
  /// State equations and constants of octamethyltrisiloxane.
  /// Short name: MDM.
  /// Synomym: MDM.
  /// Chemical formula: C8H24O2Si3.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'mdm.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Thol, M., Dubberke, F.H, Vrabec, J., Span, R."Thermodynamic Properties of Octamethyltrisiloxane and Decamethyltetrasiloxane" to be submitted to J. Chem. Eng. Data, 2017</para>
  /// <para>HeatCapacity (CPP): see EOS of Thol et al. (2017).</para>
  /// <para>Saturated vapor pressure: see EOS of Thol et al. (2017).</para>
  /// <para>Saturated liquid density: see EOS of Thol et al. (2017).</para>
  /// <para>Saturated vapor density: see EOS of Thol et al. (2017).</para>
  /// </remarks>
  [CASRegistryNumber("107-51-7")]
  public class Octamethyltrisiloxane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Octamethyltrisiloxane Instance { get; } = new Octamethyltrisiloxane();

    #region Constants for octamethyltrisiloxane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "octamethyltrisiloxane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "MDM";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "MDM";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C8H24O2Si3";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "107-51-7";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.3144621;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.23653146; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 565.3609;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 1437500;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 1134;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 187.2;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.0010815;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 3907.20135047451;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 6.94871319793325E-07;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 425.630478470065;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.524;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.079;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 187.2;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 575;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 3910;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 130000000;

    #endregion Constants for octamethyltrisiloxane

    private Octamethyltrisiloxane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 117.994606421883;
      _alpha0_n_tau = -19.6600754238182;
      _alpha0_n_lntau = 3;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (              28.817,   0.0353756335112669),
          (              46.951,     2.77698723063445),
          (              31.054,     8.31327387514771),
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
          (          0.05039724,                    1,                    4),
          (            1.189992,                0.188,                    1),
          (           -2.468723,                 1.03,                    1),
          (           -0.743856,                  0.7,                    2),
          (           0.4434056,                0.464,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (           -1.371359,                2.105,                    1,                   -1,                    2),
          (           -1.529621,                1.376,                    3,                   -1,                    2),
          (           0.4445898,                  0.8,                    2,                   -1,                    1),
          (           -1.009921,                  1.8,                    2,                   -1,                    2),
          (         -0.05903694,                1.005,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (            3.515188,                  0.7,                    1,               -0.986,               -0.966,                 1.25,                0.928),
          (          0.08367608,                 0.66,                    1,               -1.715,               -0.237,                1.438,                2.081),
          (            1.646856,                1.138,                    3,               -0.837,               -0.954,                0.894,                0.282),
          (          -0.2851917,                 1.56,                    2,               -1.312,               -0.861,                  0.9,                1.496),
          (           -2.457571,                 1.31,                    2,               -1.191,               -0.909,                0.899,                0.805),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (               7.016,                 0.54),
          (             -13.924,                  0.9),
          (               20.84,                  1.3),
          (              -16.64,                 1.73),
          (               5.906,                  2.2),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -5.3686,                0.515),
          (              -11.85,                 4.58),
          (              -16.64,                 2.06),
          (              -52.26,                 5.25),
          (              -125.6,                 11.3),
          (              -235.7,                 21.6),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -8.8192,                    1),
          (              4.0952,                  1.5),
          (              -4.062,                  1.9),
          (              -6.208,                 3.71),
          (              -3.212,                 14.6),
      };

      #endregion Saturated densities and pressure

    }
  }
}

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
  /// State equations and constants of 1,1,1,3,3,3-hexafluoropropane.
  /// Short name: R236fa.
  /// Synomym: HFC-236fa.
  /// Chemical formula: CF3CH2CF3.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'r236fa.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Pan, J., Rui, X., Zhao, X., Qiu, L."An equation of state for the thermodynamic properties of1,1,1,3,3,3-hexafluoropropane (HFC-236fa),"Fluid Phase Equilib., 321:10-16, 2012.</para>
  /// <para>HeatCapacity (CPP): see EOS of Pan et al. (2012)</para>
  /// <para>Saturated vapor pressure: Cullimore, I.D., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("690-39-1")]
  public class Fluid_1_1_1_3_3_3_hexafluoropropane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Fluid_1_1_1_3_3_3_hexafluoropropane Instance { get; } = new Fluid_1_1_1_3_3_3_hexafluoropropane();

    #region Constants for 1,1,1,3,3,3-hexafluoropropane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "1,1,1,3,3,3-hexafluoropropane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "R236fa";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "HFC-236fa";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CF3CH2CF3";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "halocb";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "690-39-1";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.1520384; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 398.07;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3200000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 3626;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 179.6;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 160.3;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 11234.5055619022;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.107398807244531;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 271.661627285125;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.377;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.982;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 179.6;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 400;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 11235;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 70000000;

    #endregion Constants for 1,1,1,3,3,3-hexafluoropropane

    private Fluid_1_1_1_3_3_3_hexafluoropropane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -17.5983848630696;
      _alpha0_n_tau = 8.87150448829181;
      _alpha0_n_lntau = 9.175;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (              9.8782,     2.41666038636421),
          (              18.236,     6.01401763508931),
          (              49.934,     13.0328836636772),
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
          (          0.04453255,                 1.07,                    4),
          (            1.777017,                0.222,                    1),
          (           -2.230519,                 0.66,                    1),
          (          -0.6708606,                 1.33,                    2),
          (           0.1587907,                0.227,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (           -1.425119,                 2.33,                    1,                   -1,                    2),
          (          -0.6461628,                 1.94,                    3,                   -1,                    2),
          (           0.8469985,                 1.53,                    2,                   -1,                    1),
          (          -0.5635356,                 2.65,                    2,                   -1,                    2),
          (         -0.01535611,                0.722,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (            1.156362,                 1.11,                    1,                -1.02,                -1.42,                 1.13,                0.712),
          (           -0.407031,                 2.31,                    1,               -1.336,                -2.31,                 0.67,                 0.91),
          (          -0.2172753,                 3.68,                    3,               -1.055,                -0.89,                 0.46,                0.677),
          (           -1.007176,                 4.23,                    3,                -5.84,                  -80,                 1.28,                0.718),
          (       -6.902909E-05,                0.614,                    2,                -16.2,                 -108,                  1.2,                 1.64),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (               12.32,                0.579),
          (             -27.579,                 0.77),
          (              40.114,                 0.97),
          (             -35.461,                 1.17),
          (              13.769,                  1.4),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -4.4507,                0.506),
          (             -3.7583,                 1.16),
          (             -20.279,                  2.8),
          (             -268.01,                    7),
          (              501.71,                    8),
          (             -349.17,                    9),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.8785,                    1),
          (              1.5884,                  1.5),
          (             -4.8864,                  3.1),
          (             -5.0273,                    8),
          (                8.99,                   10),
      };

      #endregion Saturated densities and pressure

    }
  }
}

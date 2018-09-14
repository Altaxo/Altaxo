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
  /// State equations and constants of diethyl ether.
  /// Short name: diethyl ether.
  /// Synomym: ethyl ether.
  /// Chemical formula: C4H10O.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'dee.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Thol, M., Piazza, L., and Span, R. "A New Functional Form for Equations of State for Some Polar and Weakly Associating Fluids," Int. J. Thermophys., 35(5):783-811, 2014.</para>
  /// <para>HeatCapacity (CPP): see EOS of Thol et al. (2013)</para>
  /// <para>Saturated vapor pressure: Herrig, S., 2013.</para>
  /// <para>Saturated liquid density: Herrig, S., 2013.</para>
  /// <para>Saturated vapor density: Herrig, S., 2013.</para>
  /// </remarks>
  [CASRegistryNumber("60-29-7")]
  public class DiethylEther : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static DiethylEther Instance { get; } = new DiethylEther();

    #region Constants for diethyl ether

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "diethyl ether";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "diethyl ether";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "ethyl ether";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C4H10O";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "60-29-7";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.31447188544;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.0741216; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 466.7;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3644000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 3561.71480478189;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 156.92;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.001;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 11479.5670146306;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.000425763665683735;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 307.604361337114;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.281;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 1.151;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 270;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 500;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 10685.1;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 40000000;

    #endregion Constants for diethyl ether

    private DiethylEther()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 7.03041163432812;
      _alpha0_n_tau = 0.509524787276232;
      _alpha0_n_lntau = 3.362810483;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          (   -8.94382165718001,                   -1),
          (      0.546209543877,                   -2),
          (    -0.0166036932614,                   -3),
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
          (    0.37670049856531,                -0.75,                    1),
          (   -0.11663033427561,                -0.25,                    1),
          (   -0.73801498033072,                 1.25,                    1),
          (   -0.27257010038884,                 0.75,                    2),
          (  -0.049792309995641,                   -1,                    3),
          (    0.17226702864911,               -0.375,                    3),
          (  0.0044161890982677,                 1.25,                    5),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (    -1.5395161186826,                2.375,                    1,                   -1,                    1),
          (     1.1560605212975,                    3,                    1,                   -1,                    1),
          (  -0.018450401885875,                2.625,                    2,                   -1,                    1),
          (   -0.10180059922897,                1.875,                    5,                   -1,                    1),
          (   -0.40359870380574,                  4.5,                    1,                   -1,                    2),
          (  0.0021305557123223,                 5.75,                    3,                   -1,                    2),
          (   -0.15474197558782,                5.375,                    4,                   -1,                    2),
          (   0.012095055234488,                 2.75,                    5,                   -1,                    2),
          (   -0.01431063712021,                 14.5,                    2,                   -1,                    3),
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
          (              0.3275,                 0.12),
          (              3.1842,                 0.55),
          (             -2.1407,                    1),
          (              1.4376,                  1.4),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (            -0.35858,                 0.06),
          (             -16.843,                 0.87),
          (              32.476,                  1.3),
          (             -33.444,                  1.7),
          (             -48.036,                  5.3),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.3059,                    1),
          (              1.1734,                  1.5),
          (              0.7142,                  2.2),
          (             -4.3219,                    3),
      };

      #endregion Saturated densities and pressure

    }
  }
}

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
  /// State equations and constants of cis-1,3,3,3-tetrafluoropropene.
  /// Short name: R1234ze(Z).
  /// Synomym: HFO-1234ze(Z).
  /// Chemical formula: CHF=CHCF3 (cis).
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'r1234zez.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Akasaka, R., Higashi, Y., Miyara, A., Koyama, S. "A Fundamental Equation of State for Cis-1,3,3,3-tetrafluoropropene (R-1234ze(Z))," Int. J. Refrig., 2014.</para>
  /// <para>HeatCapacity (CPP): see EOS for reference</para>
  /// <para>Saturated vapor pressure: Akasaka, R., Higashi, Y., Miyara, A., Koyama, S.</para>
  /// <para>Saturated liquid density: Akasaka, R., Higashi, Y., Miyara, A., Koyama, S.</para>
  /// <para>Saturated vapor density: Akasaka, R., Higashi, Y., Miyara, A., Koyama, S.</para>
  /// </remarks>
  [CASRegistryNumber("29118-25-0")]
  public class Cis_1_3_3_3_tetrafluoropropene : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Cis_1_3_3_3_tetrafluoropropene Instance { get; } = new Cis_1_3_3_3_tetrafluoropropene();

    #region Constants for cis-1,3,3,3-tetrafluoropropene

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "cis-1,3,3,3-tetrafluoropropene";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "R1234ze(Z)";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "HFO-1234ze(Z)";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CHF=CHCF3 (cis)";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "halocb";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "29118-25-0";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.1140416; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 423.27;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3533000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 4126.7;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 273;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 67800;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 11250.7668439848;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 31.0763256550951;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 282.895073070334;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.3274;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = -1;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 273;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 430;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 11260;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 6000000;

    #endregion Constants for cis-1,3,3,3-tetrafluoropropene

    private Cis_1_3_3_3_tetrafluoropropene()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -7.5935678406238;
      _alpha0_n_tau = 10.5332512952197;
      _alpha0_n_lntau = -2.6994;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          (            -12.2635,                   -1),
          (             1.65415,                   -2),
          (  -0.126316666666667,                   -3),
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
          (           7.7652368,                0.685,                    1),
          (          -8.7025756,               0.8494,                    1),
          (         -0.28352251,                 1.87,                    1),
          (          0.14534501,                    2,                    2),
          (        0.0092092105,                0.142,                    5),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (         -0.24997382,                  4.2,                    1,                   -1,                    1),
          (          0.09667436,                 0.08,                    3,                   -1,                    1),
          (         0.024685924,                    0,                    5,                   -1,                    1),
          (        -0.013255083,                  1.1,                    7,                   -1,                    1),
          (         -0.06423133,                  5.5,                    1,                   -1,                    2),
          (          0.36638206,                  6.6,                    2,                   -1,                    2),
          (         -0.25548847,                  8.4,                    2,                   -1,                    2),
          (        -0.095592361,                  7.2,                    3,                   -1,                    2),
          (         0.086271444,                  7.6,                    4,                   -1,                    2),
          (         0.015997412,                  8.5,                    2,                   -1,                    3),
          (        -0.013127234,                   23,                    3,                   -1,                    3),
          (         0.004229399,                   18,                    5,                   -1,                    3),
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
          (              1.1983,                 0.33),
          (               1.444,                  0.5),
          (            -0.11628,                  1.5),
          (             0.55483,                  2.5),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -3.1996,                 0.39),
          (             -7.0363,                 1.24),
          (             -21.124,                  3.2),
          (              -38.49,                  6.9),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.6208,                    1),
          (              1.5925,                  1.5),
          (             -2.3198,                  2.5),
          (              2.0196,                    5),
      };

      #endregion Saturated densities and pressure

    }
  }
}

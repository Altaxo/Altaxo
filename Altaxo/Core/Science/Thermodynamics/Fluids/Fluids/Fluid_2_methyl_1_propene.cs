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
  /// State equations and constants of 2-methyl-1-propene.
  /// Short name: isobutene.
  /// Synomym: methylpropene.
  /// Chemical formula: CH2=C(CH3)2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'ibutene.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Lemmon, E.W. and Ihmels, E.C., "Thermodynamic Properties of the Butenes.  Part II. Short Fundamental Equations of State," Fluid Phase Equilibria, 228-229C:173-187, 2005.</para>
  /// <para>HeatCapacity (CPP): Lemmon, E.W. and Ihmels, E.C.,</para>
  /// <para>Saturated vapor pressure: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// <para>Saturated liquid density: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// <para>Saturated vapor density: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("115-11-7")]
  public class Fluid_2_methyl_1_propene : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Fluid_2_methyl_1_propene Instance { get; } = new Fluid_2_methyl_1_propene();

    #region Constants for 2-methyl-1-propene

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "2-methyl-1-propene";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "isobutene";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "methylpropene";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CH2=C(CH3)2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "br-alkene";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "115-11-7";

    private int[] _unNumbers = new int[] { 1055, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.05610632; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 418.09;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 4009800;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 4170;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 132.4;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.6761;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 13666.7519208756;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.000614255027216308;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 266.145754759657;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.193;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0.5;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 132.4;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 550;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 13670;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 50000000;

    #endregion Constants for 2-methyl-1-propene

    private Fluid_2_methyl_1_propene()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -0.127378872689037;
      _alpha0_n_tau = 2.31251275366912;
      _alpha0_n_lntau = 3;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (              4.8924,    0.954339974646607),
          (               7.832,     3.03762347819847),
          (              7.2867,     4.79561816833696),
          (              8.7293,     9.60797914324667),
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
          (             0.77111,                 0.12,                    1),
          (             -2.7971,                  1.3,                    1),
          (              1.0118,                 1.74,                    1),
          (             0.02073,                  2.1,                    2),
          (            0.085086,                 0.28,                    3),
          (          0.00021968,                 0.69,                    7),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (             0.20633,                 0.75,                    2,                   -1,                    1),
          (           -0.078843,                    2,                    5,                   -1,                    1),
          (            -0.23726,                  4.4,                    1,                   -1,                    2),
          (           -0.080211,                  4.7,                    4,                   -1,                    2),
          (           -0.027001,                   15,                    3,                   -1,                    3),
          (            0.013072,                   14,                    4,                   -1,                    3),
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
          (              62.591,                 0.65),
          (             -208.05,                  0.8),
          (              332.43,                 0.98),
          (             -295.55,                 1.16),
          (              111.48,                  1.3),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -3.1841,                0.431),
          (             -6.4014,                 1.29),
          (             -9.3817,                  3.3),
          (              -11.16,                 3.54),
          (             -52.298,                  7.3),
          (             -121.95,                 15.8),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -6.8973,                    1),
          (              1.2475,                  1.5),
          (             -2.5441,                 3.16),
          (             -2.9282,                  6.2),
          (              1.5778,                    7),
      };

      #endregion Saturated densities and pressure

    }
  }
}

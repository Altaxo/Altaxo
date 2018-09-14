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
  /// State equations and constants of propene.
  /// Short name: propylene.
  /// Synomym: R-1270.
  /// Chemical formula: CH2=CH-CH3.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'propylen.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Lemmon, E.W., Overhoff, U., McLinden, M.O., Wagner, W. to be submitted to J. Phys. Chem. Ref. Data, 2017.</para>
  /// <para>HeatCapacity (CPP): see EOS for reference</para>
  /// <para>Melting pressure: Reeves, L.E., Scott, G.J., Babb, S.E., Jr. "Melting curves of pressure-transmitting fluids," J. Chem. Phys., 40(12):3662-6, 1964.</para>
  /// <para>Saturated vapor pressure: See EOS</para>
  /// <para>Saturated liquid density: See EOS</para>
  /// <para>Saturated vapor density: See EOS</para>
  /// </remarks>
  [CASRegistryNumber("115-07-1")]
  public class Propene : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Propene Instance { get; } = new Propene();

    #region Constants for propene

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "propene";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "propylene";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "R-1270";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CH2=CH-CH3";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "n-alkene";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "115-07-1";

    private int[] _unNumbers = new int[] { 1075, 1077, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.04207974; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 364.211;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 4555000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 5457;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 87.953;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.0007471;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 18254.5627805518;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 1.02172910029971E-06;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 225.530838982067;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.146;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0.366;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 87.953;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 575;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 23100;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 1000000000;

    #endregion Constants for propene

    private Propene()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -5.18232796514516;
      _alpha0_n_tau = 4.36399027654278;
      _alpha0_n_lntau = 3;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (               1.544,    0.889594218735843),
          (               4.013,     2.67152831737647),
          (               8.923,      5.3046173783878),
          (                6.02,     11.8530192663044),
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
          (          0.04341002,                    1,                    4),
          (            1.136592,                0.205,                    1),
          (          -0.8528611,                 0.56,                    1),
          (           0.5216669,                0.676,                    2),
          (           -1.382953,                    1,                    2),
          (           0.1214347,                  0.5,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (          -0.5984662,                    1,                    1,                   -1,                    1),
          (           -1.391883,                 1.94,                    1,                   -1,                    2),
          (           -1.008434,                    2,                    3,                   -1,                    2),
          (           0.1961249,                    1,                    2,                   -1,                    1),
          (           -0.360693,                 2.66,                    2,                   -1,                    2),
          (        -0.002407175,                 0.83,                    8,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (           0.7432121,                  1.6,                    1,                -1.07,                -0.77,                 1.21,                 0.78),
          (           0.1475162,                  2.5,                    1,                -0.66,                -0.83,                 1.08,                 0.82),
          (         -0.02503391,                    3,                    2,                 -1.2,               -0.607,                 0.83,                 1.94),
          (          -0.2734409,                  2.5,                    3,                -1.12,                 -0.4,                 0.56,                 0.69),
          (         0.006378889,                 2.72,                    3,                -1.47,                -0.66,                 1.22,                 1.96),
          (           0.0150294,                    4,                    2,                -1.93,                -0.07,                 1.81,                  1.3),
          (         -0.03162971,                    4,                    1,                 -3.3,                 -3.1,                 1.54,                 0.38),
          (         -0.04107194,                    1,                    2,                -15.4,                 -387,                 1.12,                 0.91),
          (           -1.190241,                    4,                    3,                   -6,                  -41,                  1.4,                  0.7),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             0.40543,                0.195),
          (             2.02481,                 0.47),
          (            0.304022,                 2.25),
          (            0.179159,                    8),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (            -1.59841,                0.309),
          (             -4.7384,                0.853),
          (            -10.8886,                 2.37),
          (            -31.0312,                  5.2),
          (            -56.9431,                   10),
          (            -143.544,                   20),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (            -6.75625,                    1),
          (               2.027,                  1.5),
          (            -1.35883,                  1.9),
          (            -2.74671,                  4.3),
          (           -0.936445,                   15),
      };

      #endregion Saturated densities and pressure

      #region Melting pressure

      _meltingPressure_Type = '1';
      _meltingPressure_ReducingTemperature = 87.953;
      _meltingPressure_ReducingPressure = 0.048475;
      _meltingPressure_Coefficients = new (double factor, double exponent)[][]
      {
      new (double factor, double exponent)[]
        {
            (         -6593000000,                    0),
            (          6593000001,                2.821),
        },

      new (double factor, double exponent)[]{},
      new (double factor, double exponent)[]{},
      };

      #endregion Melting pressure

    }
  }
}

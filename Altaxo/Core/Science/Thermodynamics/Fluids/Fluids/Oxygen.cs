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
  /// State equations and constants of oxygen.
  /// Short name: oxygen.
  /// Synomym: R-732.
  /// Chemical formula: O2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'oxygen.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Schmidt, R. and Wagner, W., "A New Form of the Equation of State for Pure Substances and its Application to Oxygen," Fluid Phase Equilibria, 19:175-200, 1985.</para>
  /// <para>HeatCapacity (CPP): Refit by Roland Span of the Schmidt and Wagner equation listed belowto account for the electronic contribution up to 2000 K by usingPlanck-Einstein terms only.</para>
  /// <para>Melting pressure: Schmidt, R. and Wagner, W., "A New Form of the Equation of State for Pure Substances and its Application to Oxygen," Fluid Phase Equilibria, 19:175-200, 1985.</para>
  /// <para>Sublimation pressure: Lemmon, E.W., 2003.</para>
  /// <para>Saturated vapor pressure: Cullimore, I.D., 2010.</para>
  /// <para>Saturated liquid density: Cullimore, I.D., 2010.</para>
  /// <para>Saturated vapor density: Cullimore, I.D., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("7782-44-7")]
  public class Oxygen : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Oxygen Instance { get; } = new Oxygen();

    #region Constants for oxygen

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "oxygen";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "oxygen";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "R-732";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "O2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "7782-44-7";

    private int[] _unNumbers = new int[] { 1072, 1073, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.31434;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.0319988; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 154.581;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 5043000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 13630;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 54.361;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 146.28;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 40816.4308177384;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.323703170090918;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 90.1878078804554;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.0222;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 54.361;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 2000;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 43348;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 82000000;

    #endregion Constants for oxygen

    private Oxygen()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -14.7168366664971;
      _alpha0_n_tau = -0.0110839854084601;
      _alpha0_n_lntau = 2.51808732;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (          1.02323928,     14.5316979447668),
          (         0.784357918,     72.8419165356674),
          (       0.00337183363,      7.7710849975094),
          (       -0.0170864084,    0.446425786480874),
          (        0.0463751562,     34.4677188658373),
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
          (        0.3983768749,                    0,                    1),
          (        -1.846157454,                  1.5,                    1),
          (        0.4183473197,                  2.5,                    1),
          (       0.02370620711,                 -0.5,                    2),
          (       0.09771730573,                  1.5,                    2),
          (       0.03017891294,                    2,                    2),
          (       0.02273353212,                    0,                    3),
          (       0.01357254086,                    1,                    3),
          (      -0.04052698943,                  2.5,                    3),
          (     0.0005454628515,                    0,                    6),
          (     0.0005113182277,                    2,                    7),
          (     2.953466883E-07,                    5,                    7),
          (    -8.687645072E-05,                    2,                    8),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (       -0.2127082589,                    5,                    1,                   -1,                    2),
          (       0.08735941958,                    6,                    1,                   -1,                    2),
          (         0.127550919,                  3.5,                    2,                   -1,                    2),
          (      -0.09067701064,                  5.5,                    2,                   -1,                    2),
          (      -0.03540084206,                    3,                    3,                   -1,                    2),
          (      -0.03623278059,                    7,                    3,                   -1,                    2),
          (        0.0132769929,                    6,                    5,                   -1,                    2),
          (    -0.0003254111865,                  8.5,                    6,                   -1,                    2),
          (     -0.008313582932,                    4,                    7,                   -1,                    2),
          (      0.002124570559,                  6.5,                    8,                   -1,                    2),
          (    -0.0008325206232,                  5.5,                   10,                   -1,                    2),
          (    -2.626173276E-05,                   22,                    2,                   -1,                    4),
          (      0.002599581482,                   11,                    3,                   -1,                    4),
          (      0.009984649663,                   18,                    3,                   -1,                    4),
          (      0.002199923153,                   11,                    4,                   -1,                    4),
          (      -0.02591350486,                   23,                    4,                   -1,                    4),
          (       -0.1259630848,                   17,                    5,                   -1,                    4),
          (        0.1478355637,                   18,                    5,                   -1,                    4),
          (      -0.01011251078,                   23,                    5,                   -1,                    4),
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
          (              1.6622,                0.345),
          (             0.76846,                 0.74),
          (            -0.10041,                  1.2),
          (              0.2048,                  2.6),
          (            0.011551,                  7.2),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -2.2695,               0.3785),
          (             -4.6578,                 1.07),
          (              -9.948,                  2.7),
          (             -22.845,                  5.5),
          (              -45.19,                   10),
          (             -25.101,                   20),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -6.0595,                    1),
          (               1.305,                  1.5),
          (            -0.54178,                  2.2),
          (              -1.941,                  4.8),
          (             0.35514,                  6.2),
      };

      #endregion Saturated densities and pressure

      #region Sublimation pressure

      _sublimationPressure_Type = 3;
      _sublimationPressure_ReducingTemperature = 54.361;
      _sublimationPressure_ReducingPressure = 146.28;
      _sublimationPressure_PolynomialCoefficients1 = new (double factor, double exponent)[]
      {
      };

      _sublimationPressure_PolynomialCoefficients2 = new (double factor, double exponent)[]
      {
          (             -20.714,                 1.06),
      };

      #endregion Sublimation pressure

      #region Melting pressure

      _meltingPressure_Type = '2';
      _meltingPressure_ReducingTemperature = 54.361;
      _meltingPressure_ReducingPressure = 146.33;
      _meltingPressure_Coefficients = new (double factor, double exponent)[][]
      {
      new (double factor, double exponent)[]
        {
        },

      new (double factor, double exponent)[]
        {
            (          -32.463539,               0.0625),
            (           142.78011,                0.125),
            (          -147.02341,               0.1875),
            (             52.0012,                 0.25),
        },

      new (double factor, double exponent)[]{},
      };

      #endregion Melting pressure

    }
  }
}

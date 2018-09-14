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
  /// State equations and constants of propane.
  /// Short name: propane.
  /// Synomym: R-290.
  /// Chemical formula: CH3CH2CH3.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'propane.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Lemmon, E.W., McLinden, M.O., Wagner, W."Thermodynamic Properties of Propane.  III.  A Reference Equation of Statefor Temperatures from the Melting Line to 650 K and Pressures up to 1000 MPa,"J. Chem. Eng. Data, 54:3141-3180, 2009.</para>
  /// <para>HeatCapacity (CPP): see EOS of Lemmon et al. (2009)</para>
  /// <para>Melting pressure: Reeves, L.E., Scott, G.J., Babb, S.E., Jr. "Melting curves of pressure-transmitting fluids," J. Chem. Phys., 40(12):3662-6, 1964.</para>
  /// <para>Saturated vapor pressure: See EOS</para>
  /// <para>Saturated liquid density: See EOS</para>
  /// <para>Saturated vapor density: See EOS</para>
  /// </remarks>
  [CASRegistryNumber("74-98-6")]
  public class Propane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Propane Instance { get; } = new Propane();

    #region Constants for propane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "propane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "propane";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "R-290";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CH3CH2CH3";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "n-alkane";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "74-98-6";

    private int[] _unNumbers = new int[] { 1075, 1978, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.04409562; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 369.89;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 4251200;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 5000;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 85.525;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.000172;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 16625.8055596502;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 2.4194535056959E-07;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 231.036214644318;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.1521;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0.084;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 85.525;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 650;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 20600;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 1000000000;

    #endregion Constants for propane

    private Propane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -4.97058342143591;
      _alpha0_n_tau = 4.29352021662331;
      _alpha0_n_lntau = 3;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (               3.043,     1.06247803401011),
          (               5.874,     3.34423747600638),
          (               9.337,     5.36375679255995),
          (               7.922,     11.7629565546514),
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
          (         0.042910051,                    1,                    4),
          (           1.7313671,                 0.33,                    1),
          (          -2.4516524,                  0.8,                    1),
          (          0.34157466,                 0.43,                    2),
          (         -0.46047898,                  0.9,                    2),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (         -0.66847295,                 2.46,                    1,                   -1,                    1),
          (          0.20889705,                 2.09,                    3,                   -1,                    1),
          (          0.19421381,                 0.88,                    6,                   -1,                    1),
          (         -0.22917851,                 1.09,                    6,                   -1,                    1),
          (         -0.60405866,                 3.25,                    2,                   -1,                    2),
          (         0.066680654,                 4.62,                    3,                   -1,                    2),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (         0.017534618,                 0.76,                    1,               -0.963,                -2.33,                0.684,                1.283),
          (          0.33874242,                  2.5,                    1,               -1.977,                -3.47,                0.829,               0.6936),
          (          0.22228777,                 2.75,                    1,               -1.917,                -3.15,                1.419,                0.788),
          (         -0.23219062,                 3.05,                    2,               -2.307,                -3.19,                0.817,                0.473),
          (         -0.09220694,                 2.55,                    2,               -2.546,                -0.92,                  1.5,               0.8577),
          (         -0.47575718,                  8.4,                    4,                -3.28,                -18.8,                1.426,                0.271),
          (        -0.017486824,                 6.75,                    1,                -14.6,               -547.8,                1.093,                0.948),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             1.82205,                0.345),
          (             0.65802,                 0.74),
          (             0.21109,                  2.6),
          (            0.083973,                  7.2),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -2.4887,               0.3785),
          (             -5.1069,                 1.07),
          (             -12.174,                  2.7),
          (             -30.495,                  5.5),
          (             -52.192,                   10),
          (             -134.89,                   20),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -6.7722,                    1),
          (              1.6938,                  1.5),
          (             -1.3341,                  2.2),
          (             -3.1876,                  4.8),
          (             0.94937,                  6.2),
      };

      #endregion Saturated densities and pressure

      #region Melting pressure

      _meltingPressure_Type = '1';
      _meltingPressure_ReducingTemperature = 85.525;
      _meltingPressure_ReducingPressure = 0.00017;
      _meltingPressure_Coefficients = new (double factor, double exponent)[][]
      {
      new (double factor, double exponent)[]
        {
            (      -4230000000000,                    0),
            (       4230000000001,                1.283),
        },

      new (double factor, double exponent)[]{},
      new (double factor, double exponent)[]{},
      };

      #endregion Melting pressure

    }
  }
}

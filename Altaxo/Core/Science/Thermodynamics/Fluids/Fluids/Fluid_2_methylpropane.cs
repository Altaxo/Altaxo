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
  /// State equations and constants of 2-methylpropane.
  /// Short name: isobutane.
  /// Synomym: R-600a.
  /// Chemical formula: CH(CH3)3.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'isobutan.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Buecker, D. and Wagner, W., "Reference Equations of State for the Thermodynamic Properties of Fluid Phase n-Butane and Isobutane," J. Phys. Chem. Ref. Data, 35(2):929-1019, 2006.</para>
  /// <para>HeatCapacity (CPP): see EOS of Buecker and Wagner for reference</para>
  /// <para>Melting pressure:  see EOS for reference</para>
  /// <para>Saturated vapor pressure: See EOS</para>
  /// <para>Saturated liquid density: See EOS</para>
  /// <para>Saturated vapor density: See EOS</para>
  /// </remarks>
  [CASRegistryNumber("75-28-5")]
  public class Fluid_2_methylpropane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Fluid_2_methylpropane Instance { get; } = new Fluid_2_methylpropane();

    #region Constants for 2-methylpropane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "2-methylpropane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "isobutane";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "R-600a";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CH(CH3)3";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "br-alkane";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "75-28-5";

    private int[] _unNumbers = new int[] { 1969, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.0581222; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 407.81;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3629000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 3879.756788;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 113.73;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.02289;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 12737.6244574213;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 2.420743019024E-05;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 261.400977443331;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.184;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0.132;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 113.73;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 575;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 12900;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 35000000;

    #endregion Constants for 2-methylpropane

    private Fluid_2_methylpropane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -5.99950937422589;
      _alpha0_n_tau = 5.01894580569273;
      _alpha0_n_lntau = 3.05956619;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (          4.94641014,    0.951277899021603),
          (          4.09475197,     2.38789588288664),
          (          15.6632824,     4.34690426914494),
          (          9.73918122,     10.3688586351487),
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
          (     2.0686820727966,                  0.5,                    1),
          (    -3.6400098615204,                    1,                    1),
          (    0.51968754427244,                  1.5,                    1),
          (    0.17745845870123,                    0,                    2),
          (   -0.12361807851599,                  0.5,                    3),
          (   0.045145314010528,                  0.5,                    4),
          (    0.03047647996598,                 0.75,                    4),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (    0.75508387706302,                    2,                    1,                   -1,                    1),
          (   -0.85885381015629,                  2.5,                    1,                   -1,                    1),
          (   0.036324009830684,                  2.5,                    2,                   -1,                    1),
          (   -0.01954879945055,                  1.5,                    7,                   -1,                    1),
          (  -0.004445239290496,                    1,                    8,                   -1,                    1),
          (   0.004641076366646,                  1.5,                    8,                   -1,                    1),
          (  -0.071444097992825,                    4,                    1,                   -1,                    2),
          (  -0.080765060030713,                    7,                    2,                   -1,                    2),
          (    0.15560460945053,                    3,                    3,                   -1,                    2),
          (  0.0020318752160332,                    7,                    3,                   -1,                    2),
          (   -0.10624883571689,                    3,                    4,                   -1,                    2),
          (   0.039807690546305,                    1,                    5,                   -1,                    2),
          (   0.016371431292386,                    6,                    5,                   -1,                    2),
          ( 0.00053212200682628,                    0,                   10,                   -1,                    2),
          ( -0.0078681561156387,                    6,                    2,                   -1,                    3),
          ( -0.0030981191888963,                   13,                    6,                   -1,                    3),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (  -0.042276036810382,                    2,                    1,                  -10,                 -150,                 1.16,                 0.85),
          ( -0.0053001044558079,                    0,                    2,                  -10,                 -200,                 1.13,                    1),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 2;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (          2.04025104,                1.065),
          (         0.850874089,                    3),
          (        -0.479052281,                    4),
          (         0.348201252,                    7),
      };

      _saturatedVaporDensity_Type = 6;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (         -2.12933323,                1.065),
          (         -2.93790085,                  2.5),
          (         -0.89441086,                  9.5),
          (         -3.46343707,                   13),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (         -6.85093103,                    1),
          (          1.36543198,                  1.5),
          (         -1.32542691,                  2.5),
          (         -2.56190994,                  4.5),
      };

      #endregion Saturated densities and pressure

      #region Melting pressure

      _meltingPressure_Type = '1';
      _meltingPressure_ReducingTemperature = 113.73;
      _meltingPressure_ReducingPressure = 0.022891;
      _meltingPressure_Coefficients = new (double factor, double exponent)[][]
      {
      new (double factor, double exponent)[]
        {
            (         -1953637129,                    0),
            (          1953637130,                 6.12),
        },

      new (double factor, double exponent)[]{},
      new (double factor, double exponent)[]{},
      };

      #endregion Melting pressure

    }
  }
}

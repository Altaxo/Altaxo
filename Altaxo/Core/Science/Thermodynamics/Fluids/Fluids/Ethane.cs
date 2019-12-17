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
  /// State equations and constants of ethane.
  /// Short name: ethane.
  /// Synomym: R-170.
  /// Chemical formula: CH3CH3.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'ethane.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Buecker, D. and Wagner, W. "A Reference Equation of State for the Thermodynamic Properties of Ethane for Temperatures from the Melting Line to 675 K and Pressures up to 900 MPa," J. Phys. Chem. Ref. Data, 35(1):205-266, 2006.</para>
  /// <para>HeatCapacity (CPP): see EOS for Reference</para>
  /// <para>Melting pressure:  see EOS for reference.</para>
  /// <para>Saturated vapor pressure:  see EOS for reference.</para>
  /// <para>Saturated liquid density:  see EOS for reference.</para>
  /// <para>Saturated vapor density:  see EOS for reference.</para>
  /// </remarks>
  [CASRegistryNumber("74-84-0")]
  public class Ethane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Ethane Instance { get; } = new Ethane();

    #region Constants for ethane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "ethane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "ethane";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "R-170";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CH3CH3";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "n-alkane";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "74-84-0";

    private int[] _unNumbers = new int[] { 1035, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.03006904; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 305.322;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 4872200;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 6856.886685;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 90.368;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 1.142;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 21667.7845593939;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.00152005545526249;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 184.568587832456;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.0995;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 90.368;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 675;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 22419;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 900000000;

    #endregion Constants for ethane

    private Ethane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -3.09382097259393;
      _alpha0_n_tau = 3.25030211006367;
      _alpha0_n_lntau = 3.003039265;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (         1.117433359,     1.40910523971414),
          (         3.467773215,     4.00991707115766),
          (          6.94194464,     6.59670983420782),
          (         5.970850948,     13.9798102658832),
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
          (    0.83440745735241,                 0.25,                    1),
          (    -1.4287360607171,                    1,                    1),
          (    0.34430242210927,                 0.25,                    2),
          (   -0.42096677920265,                 0.75,                    2),
          (   0.012094500886549,                 0.75,                    4),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (   -0.57976201597341,                    2,                    1,                   -1,                    1),
          (  -0.033127037870838,                 4.25,                    1,                   -1,                    1),
          (    -0.1175165489413,                 0.75,                    2,                   -1,                    1),
          (   -0.11160957833067,                 2.25,                    2,                   -1,                    1),
          (   0.062181592654406,                    3,                    3,                   -1,                    1),
          (   0.098481795434443,                    1,                    6,                   -1,                    1),
          (  -0.098268582682358,                 1.25,                    6,                   -1,                    1),
          (-0.00023977831007049,                 2.75,                    7,                   -1,                    1),
          ( 0.00069885663328821,                    1,                    9,                   -1,                    1),
          ( 1.9665987803305E-05,                    2,                   10,                   -1,                    1),
          (  -0.014586152207928,                  2.5,                    2,                   -1,                    2),
          (   0.046354100536781,                  5.5,                    4,                   -1,                    2),
          (  0.0060764622180645,                    7,                    4,                   -1,                    2),
          ( -0.0026447330147828,                  0.5,                    5,                   -1,                    2),
          (  -0.042931872689904,                  5.5,                    5,                   -1,                    2),
          (  0.0029987786517263,                  2.5,                    6,                   -1,                    2),
          (   0.005291933517501,                    4,                    8,                   -1,                    2),
          ( -0.0010383897798198,                    2,                    9,                   -1,                    2),
          (  -0.054260348214694,                   10,                    2,                   -1,                    3),
          (   -0.21959362918493,                   16,                    3,                   -1,                    3),
          (    0.35362456650354,                   18,                    3,                   -1,                    3),
          (   -0.12477390173714,                   20,                    3,                   -1,                    3),
          (    0.18425693591517,                   14,                    4,                   -1,                    3),
          (   -0.16192256436754,                   18,                    4,                   -1,                    3),
          (  -0.082770876149064,                   12,                    5,                   -1,                    3),
          (   0.050160758096437,                   19,                    5,                   -1,                    3),
          (  0.0093614326336655,                    7,                    6,                   -1,                    3),
          (-0.00027839186242864,                   15,                   11,                   -1,                    3),
          ( 2.3560274071481E-05,                    9,                   14,                   -1,                    3),
          (  0.0039238329738527,                   26,                    3,                   -1,                    4),
          (-0.00076488325813618,                   28,                    3,                   -1,                    4),
          (  -0.004994430444073,                   28,                    4,                   -1,                    4),
          (  0.0018593386407186,                   22,                    8,                   -1,                    4),
          (-0.00061404353331199,                   13,                   10,                   -1,                    4),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          ( -0.0023312179367924,                    0,                    1,                  -15,                 -150,                 1.05,                    1),
          (   0.002930104790876,                    3,                    1,                  -15,                 -150,                 1.05,                    1),
          (-0.00026912472842883,                    3,                    3,                  -15,                 -150,                 1.05,                    1),
          (     184.13834111814,                    0,                    3,                  -20,                 -275,                 1.22,                    1),
          (    -10.397127984854,                    3,                    2,                  -20,                 -400,                 1.16,                    1),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 4;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (          1.56138026,                0.987),
          (        -0.381552776,                    2),
          (         0.078537204,                    4),
          (        0.0370315089,                  9.5),
      };

      _saturatedVaporDensity_Type = 6;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (         -1.89879145,                1.038),
          (         -3.65459262,                  2.5),
          (         0.850562745,                    3),
          (         0.363965487,                    6),
          (         -1.50005943,                    9),
          (         -2.26690389,                   15),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (         -6.48647577,                    1),
          (          1.47010078,                  1.5),
          (         -1.66261122,                  2.5),
          (          3.57898378,                  3.5),
          (         -4.79105705,                    4),
      };

      #endregion Saturated densities and pressure

      #region Melting pressure

      _meltingPressure_Type = '1';
      _meltingPressure_ReducingTemperature = 90.368;
      _meltingPressure_ReducingPressure = 1.1421;
      _meltingPressure_Coefficients = new (double factor, double exponent)[][]
      {
      new (double factor, double exponent)[]
        {
            (                   1,                    0),
            (           105262374,                 2.55),
            (          -105262374,                    0),
        },

      new (double factor, double exponent)[]
        {
            (           223626315,                    1),
        },

      new (double factor, double exponent)[]{},
      };

      #endregion Melting pressure

    }
  }
}

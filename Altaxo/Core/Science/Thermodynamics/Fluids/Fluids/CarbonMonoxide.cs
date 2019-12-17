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
  /// State equations and constants of carbon monoxide.
  /// Short name: carbon monoxide.
  /// Synomym: carbon oxide.
  /// Chemical formula: CO.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'co.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Lemmon, E.W. and Span, R., "Short Fundamental Equations of State for 20 Industrial Fluids," J. Chem. Eng. Data, 51:785-850, 2006.</para>
  /// <para>HeatCapacity (CPP): Lemmon, E.W. and Span, R. (see eos for reference)</para>
  /// <para>Melting pressure: Barreiros, S.F., Calado, J.C.G., Nunes da Ponte, M. "The melting curve of carbon monoxide," J. Chem. Thermodyn., 14:1197-8, 1982.</para>
  /// <para>Saturated vapor pressure: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// <para>Saturated liquid density: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// <para>Saturated vapor density: Lemmon, C.K. and Lemmon, E.W., 2010.</para>
  /// </remarks>
  [CASRegistryNumber("630-08-0")]
  public class CarbonMonoxide : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static CarbonMonoxide Instance { get; } = new CarbonMonoxide();

    #region Constants for carbon monoxide

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "carbon monoxide";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "carbon monoxide";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "carbon oxide";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CO";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "630-08-0";

    private int[] _unNumbers = new int[] { 1016, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.0280101; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 132.86;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 3494000;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 10850;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 68.16;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 15530;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 30329.7059647197;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 27.7074338454231;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 81.6381700301537;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.0497;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0.1;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 68.16;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 500;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 33840;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 100000000;

    #endregion Constants for carbon monoxide

    private CarbonMonoxide()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -3.3728318565037;
      _alpha0_n_tau = 3.36834600328257;
      _alpha0_n_lntau = 2.5;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
          (-9.11127470123516E-05,                 -1.5),
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (              1.0128,     23.2500376335993),
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
          (             0.90554,                 0.25,                    1),
          (             -2.4515,                1.125,                    1),
          (             0.53149,                  1.5,                    1),
          (            0.024173,                1.375,                    2),
          (            0.072156,                 0.25,                    3),
          (          0.00018818,                0.875,                    7),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (             0.19405,                0.625,                    2,                   -1,                    1),
          (           -0.043268,                 1.75,                    5,                   -1,                    1),
          (            -0.12778,                3.625,                    1,                   -1,                    2),
          (           -0.027896,                3.625,                    4,                   -1,                    2),
          (           -0.034154,                 14.5,                    3,                   -1,                    3),
          (            0.016329,                   12,                    4,                   -1,                    3),
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
          (               2.957,                0.398),
          (              -4.288,                0.735),
          (              8.7643,                 1.08),
          (             -8.4001,                  1.5),
          (              3.6372,                  1.9),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -2.5439,                0.395),
          (             -5.5601,                 1.21),
          (             -8.5276,                    3),
          (             -5.1163,                  3.5),
          (             -17.701,                    6),
          (             -29.858,                    8),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -6.1192,                    1),
          (              1.0411,                  1.5),
          (             -6.2162,                  3.9),
          (              10.437,                  4.6),
          (             -7.6813,                  5.4),
      };

      #endregion Saturated densities and pressure

      #region Melting pressure

      _meltingPressure_Type = '1';
      _meltingPressure_ReducingTemperature = 1;
      _meltingPressure_ReducingPressure = 1000000;
      _meltingPressure_Coefficients = new (double factor, double exponent)[][]
      {
      new (double factor, double exponent)[]
        {
            (            -142.941,                    0),
            (           0.0195608,              2.10747),
        },

      new (double factor, double exponent)[]{},
      new (double factor, double exponent)[]{},
      };

      #endregion Melting pressure

    }
  }
}

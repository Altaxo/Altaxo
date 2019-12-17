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
  /// State equations and constants of carbon dioxide.
  /// Short name: carbon dioxide.
  /// Synomym: R-744.
  /// Chemical formula: CO2.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'co2.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Span, R. and Wagner, W., "A New Equation of State for Carbon Dioxide Covering the Fluid Region from the Triple-Point Temperature to 1100 K at Pressures up to 800 MPa," J. Phys. Chem. Ref. Data, 25(6):1509-1596, 1996.</para>
  /// <para>HeatCapacity (CPP): Span, R. and Wagner, W., "A New Equation of State for Carbon Dioxide Covering the Fluid Region from the Triple-Point Temperature to 1100 K at Pressures up to 800 MPa," J. Phys. Chem. Ref. Data, 25(6):1509-1596, 1996.</para>
  /// <para>Melting pressure: Span, R. and Wagner, W., "A New Equation of State for Carbon Dioxide Covering the Fluid Region from the Triple-Point Temperature to 1100 K at Pressures up to 800 MPa," J. Phys. Chem. Ref. Data, 25(6):1509-1596, 1996.</para>
  /// <para>Sublimation pressure: Span, R. and Wagner, W., "A New Equation of State for Carbon Dioxide Covering the Fluid Region from the Triple-Point Temperature to 1100 K at Pressures up to 800 MPa," J. Phys. Chem. Ref. Data, 25(6):1509-1596, 1996.</para>
  /// <para>Saturated vapor pressure: See EOS</para>
  /// <para>Saturated liquid density: See EOS</para>
  /// <para>Saturated vapor density: See EOS</para>
  /// </remarks>
  [CASRegistryNumber("124-38-9")]
  public class CarbonDioxide : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static CarbonDioxide Instance { get; } = new CarbonDioxide();

    #region Constants for carbon dioxide

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "carbon dioxide";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "carbon dioxide";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "R-744";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "CO2";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "other";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "124-38-9";

    private int[] _unNumbers = new int[] { 1013, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.31451;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.0440098; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 304.1282;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 7377300;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 10624.9063;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 216.592;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 517950;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 26777.2778601315;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 312.677744702294;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = null;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = 194.686;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.22394;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 216.592;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 2000;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 37240;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 800000000;

    #endregion Constants for carbon dioxide

    private CarbonDioxide()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = -6.12487106335332;
      _alpha0_n_tau = 5.11559631859622;
      _alpha0_n_lntau = 2.5;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (          1.99427042,     3.15163000339988),
          (         0.621052475,     6.11190001453334),
          (         0.411952928,     6.77707999455493),
          (          1.04028922,     11.3238400122054),
          (        0.0832767753,     27.0879199955808),
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
          (      0.388568232032,                    0,                    1),
          (       2.93854759427,                 0.75,                    1),
          (      -5.58671885349,                    1,                    1),
          (     -0.767531995925,                    2,                    1),
          (      0.317290055804,                 0.75,                    2),
          (      0.548033158978,                    2,                    2),
          (      0.122794112203,                 0.75,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (       2.16589615432,                  1.5,                    1,                   -1,                    1),
          (       1.58417351097,                  1.5,                    2,                   -1,                    1),
          (     -0.231327054055,                  2.5,                    4,                   -1,                    1),
          (     0.0581169164314,                    0,                    5,                   -1,                    1),
          (     -0.553691372054,                  1.5,                    5,                   -1,                    1),
          (      0.489466159094,                    2,                    5,                   -1,                    1),
          (    -0.0242757398435,                    0,                    6,                   -1,                    1),
          (     0.0624947905017,                    1,                    6,                   -1,                    1),
          (     -0.121758602252,                    2,                    6,                   -1,                    1),
          (     -0.370556852701,                    3,                    1,                   -1,                    2),
          (    -0.0167758797004,                    6,                    1,                   -1,                    2),
          (      -0.11960736638,                    3,                    4,                   -1,                    2),
          (    -0.0456193625088,                    6,                    4,                   -1,                    2),
          (     0.0356127892703,                    8,                    4,                   -1,                    2),
          (   -0.00744277271321,                    6,                    7,                   -1,                    2),
          (   -0.00173957049024,                    0,                    8,                   -1,                    2),
          (    -0.0218101212895,                    7,                    2,                   -1,                    3),
          (     0.0243321665592,                   12,                    3,                   -1,                    3),
          (    -0.0374401334235,                   16,                    3,                   -1,                    3),
          (      0.143387157569,                   22,                    5,                   -1,                    4),
          (     -0.134919690833,                   24,                    5,                   -1,                    4),
          (    -0.0231512250535,                   16,                    6,                   -1,                    4),
          (     0.0123631254929,                   24,                    7,                   -1,                    4),
          (    0.00210583219729,                    8,                    8,                   -1,                    4),
          (  -0.000339585190264,                    2,                   10,                   -1,                    4),
          (    0.00559936517716,                   28,                    4,                   -1,                    5),
          (  -0.000303351180556,                   14,                    8,                   -1,                    6),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (      -213.654886883,                    1,                    2,                  -25,                 -325,                 1.16,                    1),
          (       26641.5691493,                    0,                    2,                  -25,                 -300,                 1.19,                    1),
          (      -24027.2122046,                    1,                    2,                  -25,                 -300,                 1.19,                    1),
          (       -283.41603424,                    3,                    3,                  -15,                 -275,                 1.25,                    1),
          (       212.472844002,                    3,                    3,                  -20,                 -275,                 1.22,                    1),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
          (     -0.666422765408,                0.875,                  0.3,                  0.7,                   10,                  275,                  0.3,                  3.5),
          (      0.726086323499,                0.925,                  0.3,                  0.7,                   10,                  275,                  0.3,                  3.5),
          (     0.0550686686128,                0.875,                  0.3,                  0.7,                 12.5,                  275,                    1,                    3),
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 4;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (           1.9245108,                 1.02),
          (         -0.62385555,                  1.5),
          (         -0.32731127,                    5),
          (          0.39245142,                  5.5),
      };

      _saturatedVaporDensity_Type = 4;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (          -1.7074879,                 1.02),
          (          -0.8227467,                  1.5),
          (          -4.6008549,                    3),
          (          -10.111178,                    7),
          (          -29.742252,                   14),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (          -7.0602087,                    1),
          (           1.9391218,                  1.5),
          (          -1.6463597,                    2),
          (          -3.2995634,                    4),
      };

      #endregion Saturated densities and pressure

      #region Sublimation pressure

      _sublimationPressure_Type = 3;
      _sublimationPressure_ReducingTemperature = 216.592;
      _sublimationPressure_ReducingPressure = 517950;
      _sublimationPressure_PolynomialCoefficients1 = new (double factor, double exponent)[]
      {
      };

      _sublimationPressure_PolynomialCoefficients2 = new (double factor, double exponent)[]
      {
          (          -14.740846,                    1),
          (           2.4327015,                  1.9),
          (          -5.3061778,                  2.9),
      };

      #endregion Sublimation pressure

      #region Melting pressure

      _meltingPressure_Type = '1';
      _meltingPressure_ReducingTemperature = 216.592;
      _meltingPressure_ReducingPressure = 517950;
      _meltingPressure_Coefficients = new (double factor, double exponent)[][]
      {
      new (double factor, double exponent)[]
        {
            (                   1,                    0),
        },

      new (double factor, double exponent)[]
        {
            (            1955.539,                    1),
            (           2055.4593,                    2),
        },

      new (double factor, double exponent)[]{},
      };

      #endregion Melting pressure

    }
  }
}

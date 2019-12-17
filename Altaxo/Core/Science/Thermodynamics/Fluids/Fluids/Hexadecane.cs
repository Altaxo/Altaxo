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
  /// State equations and constants of hexadecane.
  /// Short name: hexadecane.
  /// Synomym: n-hexadecane.
  /// Chemical formula: C16H34.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'c16.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Romeo, R. and Lemmon, E.W.to be submitted, 2016.</para>
  /// <para>HeatCapacity (CPP): Romeo, R. and Lemmon, E.W.to be submitted, 2016.</para>
  /// </remarks>
  [CASRegistryNumber("544-76-3")]
  public class Hexadecane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Hexadecane Instance { get; } = new Hexadecane();

    #region Constants for hexadecane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "hexadecane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "hexadecane";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "n-hexadecane";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C16H34";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "n-alkane";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "544-76-3";

    private int[] _unNumbers = new int[] { };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.3144621;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.226441; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 722.1;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 1479850;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 1000;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 291.329;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 0.09387;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 3422.45200838472;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 3.87523583376862E-05;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 559.903352603204;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.749;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = -1;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 291.329;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 800;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 3423;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 50000000;

    #endregion Constants for hexadecane

    private Hexadecane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 45.9620654666393;
      _alpha0_n_tau = -26.1883378919061;
      _alpha0_n_lntau = 22.03;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (               18.91,    0.581636892397175),
          (               76.23,      2.5758205234732),
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
          (          0.03965879,                    1,                    4),
          (            1.945813,                0.224,                    1),
          (           -3.738575,                 0.91,                    1),
          (          -0.3428167,                 0.95,                    2),
          (           0.3427022,                0.555,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (           -2.519592,                 2.36,                    1,                   -1,                    2),
          (          -0.8948857,                 3.58,                    3,                   -1,                    2),
          (          0.10760773,                  0.5,                    2,                   -1,                    1),
          (           -1.297826,                 1.72,                    2,                   -1,                    2),
          (         -0.04832312,                1.078,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (            4.245522,                 1.14,                    1,               -0.641,               -0.516,                1.335,                 0.75),
          (         -0.31527585,                 2.43,                    1,               -1.008,               -0.669,                1.187,                1.616),
          (          -0.7212941,                 1.75,                    3,               -1.026,                -0.25,                 1.39,                 0.47),
          (          -0.2680657,                  1.1,                    2,                -1.21,                -1.33,                 1.23,                1.306),
          (          -0.7859567,                 1.08,                    2,                -0.93,                 -2.1,                0.763,                 0.46),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (       3.42998322366,            0.3914756),
          (      -4.00803134091,            0.8420581),
          (       8.47793585243,            1.2697844),
          (      -7.89397226458,            1.7191536),
          (       3.48240204649,            2.2626635),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (      -5.00955345137,             0.444489),
          (      0.906116155463,             2.320751),
          (      -15.2864999913,            1.7516002),
          (      -61.4138452304,            4.4017864),
          (      -143.522204557,            9.9673341),
          (      -369.022909728,           20.9017963),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (      -10.4856230531,                    1),
          (       3.82263749029,                  1.5),
          (      -8.67273606529,            2.7714755),
          (       -4.1440263845,            6.7037338),
          (      0.880051817286,            8.8876928),
          (       -5.7224276057,           15.5012049),
      };

      #endregion Saturated densities and pressure

    }
  }
}

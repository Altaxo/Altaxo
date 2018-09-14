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
  /// State equations and constants of cyclopentane.
  /// Short name: cyclopentane.
  /// Synomym: C5H10.
  /// Chemical formula: C5H10.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>The source code was created automatically using the fluid file 'cyclopen.fld' from the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// <para>Further references (extracted from the fluid file):</para>
  /// <para>EquationOfState (EOS): Gedanitz, H., D�vila, M.J., Lemmon, E.W. "Speed of Sound Measurements and a Fundamental Equation of State for Cyclopentane", J. Chem. Eng. Data, 60(5):1311-1337, 2015.</para>
  /// <para>HeatCapacity (CPP): see EOS</para>
  /// <para>Saturated vapor pressure: see eos</para>
  /// <para>Saturated liquid density: see eos</para>
  /// <para>Saturated vapor density: see eos</para>
  /// </remarks>
  [CASRegistryNumber("287-92-3")]
  public class Cyclopentane : HelmholtzEquationOfStateOfPureFluidsBySpanEtAl
  {

    /// <summary>Gets the (only) instance of this class.</summary>
    public static Cyclopentane Instance { get; } = new Cyclopentane();

    #region Constants for cyclopentane

    /// <summary>The full name of the fluid.</summary>
    public override string FullName => "cyclopentane";

    /// <summary>The short name of the fluid.</summary>
    public override string ShortName => "cyclopentane";

    /// <summary>The synonym of the name of the fluid.</summary>
    public override string Synonym => "C5H10";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string ChemicalFormula => "C5H10";

    /// <summary>The chemical formula of the fluid.</summary>
    public override string FluidFamily => "naphthene";

    /// <summary>Gets the CAS registry number.</summary>
    public override string CASRegistryNumber { get; } = "287-92-3";

    private int[] _unNumbers = new int[] { 1146, };
    /// <summary>The UN number of the fluid.</summary>
    public override IReadOnlyList<int> UN_Numbers => _unNumbers;

    /// <summary>The Universal Gas Constant R at the time the model was developed.</summary>
    public override double WorkingUniversalGasConstant => 8.314472;

    /// <summary>Gets the molecular weight in kg/mol.</summary>
    public override double MolecularWeight { get; } = 0.0701329; // kg/mol

    /// <summary>Gets the temperature at the critical point in K.</summary>
    public override double CriticalPointTemperature { get; } = 511.72;

    /// <summary>Gets the pressure at the critical point in Pa.</summary>);
    public override double CriticalPointPressure { get; } = 4571200;

    /// <summary>Gets the mole density at the critical point in mol/m³.</summary>
    public override double CriticalPointMoleDensity { get; } = 3820;

    /// <summary>Gets the triple point temperature in K.</summary>
    public override double TriplePointTemperature { get; } = 179.7;

    /// <summary>Gets the triple point pressure in Pa.</summary>
    public override double TriplePointPressure { get; } = 8.854;

    /// <summary>Gets the triple point liquid mole density in mol/m³.</summary>
    public override double TriplePointSaturatedLiquidMoleDensity { get; } = 12107.6087920319;

    /// <summary>Gets the triple point vapor mole density in mol/m³.</summary>
    public override double TriplePointSaturatedVaporMoleDensity { get; } = 0.00592621786616508;

    /// <summary>Gets the boiling temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalBoilingPointTemperature { get; } = 322.405003427223;

    /// <summary>Gets the sublimation temperature at normal pressure (101325 Pa) in K (if existent). If not existent, the return value is null.</summary>
    public override double? NormalSublimationPointTemperature { get; } = null;

    /// <summary>Gets the acentric factor.</summary>
    public override double AcentricFactor { get; } = 0.201;

    /// <summary>Gets the dipole moment in Debye.</summary>
    public override double DipoleMoment { get; } = 0;

    /// <summary>Gets the lower temperature limit of this model in K.</summary>
    public override double LowerTemperatureLimit { get; } = 179.7;

    /// <summary>Gets the upper temperature limit of this model in K.</summary>
    public override double UpperTemperatureLimit { get; } = 550;

    /// <summary>Gets the upper density limit of this model in mol/m³.</summary>
    public override double UpperMoleDensityLimit { get; } = 12110;

    /// <summary>Gets the upper pressure limit of this model in Pa.</summary>
    public override double UpperPressureLimit { get; } = 250000000;

    #endregion Constants for cyclopentane

    private Cyclopentane()
    {
      #region Ideal part of dimensionless Helmholtz energy and derivatives

      _alpha0_n_const = 3.24891313189374;
      _alpha0_n_tau = 2.64441662949569;
      _alpha0_n_lntau = 0.96;
      _alpha0_n_taulntau = 0;

      _alpha0_Poly = new (double ni, double thetai)[]
      {
      };

      _alpha0_Exp = new (double ni, double thetai)[]
      {
          (                3.34,    0.234503243961541),
          (                18.6,     2.54045180958337),
          (                13.9,     5.27632298913468),
          (                4.86,     10.3572266083014),
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
          (           0.0536938,                    1,                    4),
          (             1.60394,                 0.29,                    1),
          (            -2.41244,                  0.8,                    1),
          (           -0.474009,                 1.14,                    2),
          (            0.203482,                  0.5,                    3),
      };

      _alphaR_Exp = new (double ni, double ti, int di, double gi, int li)[]
      {
          (           -0.965616,                    2,                    1,                   -1,                    2),
          (           -0.344543,                  1.5,                    3,                   -1,                    2),
          (            0.353975,                    1,                    2,                   -1,                    1),
          (           -0.231373,                 3.36,                    2,                   -1,                    2),
          (          -0.0379099,                 0.95,                    7,                   -1,                    1),
      };

      _alphaR_Gauss = new (double ni, double ti, int di, double alpha, double beta, double gamma, double epsilon)[]
      {
          (            0.867586,                    1,                    1,                -0.82,                -1.15,                 1.08,                 0.68),
          (           -0.381827,                  2.5,                    1,                -1.19,                -1.61,                 0.36,                 0.97),
          (           -0.108741,                  2.5,                    3,                -0.79,                -0.66,                 0.09,                 0.84),
          (          -0.0976984,                  1.5,                    3,                -1.52,                -2.72,                 1.48,                 0.66),
      };

      _alphaR_Nonanalytical = new (double ni, double b, double beta, double A, double C, double D, double B, double a)[]
      {
      };

      #endregion

      #region Saturated densities and pressure

      _saturatedLiquidDensity_Type = 1;
      _saturatedLiquidDensity_Coefficients = new (double factor, double exponent)[]
      {
          (              0.0741,                  0.1),
          (              81.968,                  0.9),
          (              173.88,                 1.25),
          (             -68.519,                  1.4),
          (             -184.74,                 1.05),
      };

      _saturatedVaporDensity_Type = 3;
      _saturatedVaporDensity_Coefficients = new (double factor, double exponent)[]
      {
          (             -0.0559,                  0.1),
          (             -6.4211,                 0.65),
          (             -46.926,                  3.2),
          (              28.082,                 3.55),
          (             -70.838,                  7.5),
      };

      _saturatedVaporPressure_Type = 5;
      _saturatedVaporPressure_Coefficients = new (double factor, double exponent)[]
      {
          (             -7.1905,                    1),
          (              1.8637,                  1.5),
          (             -1.6442,                  5.5),
          (               -2.72,                  2.9),
      };

      #endregion Saturated densities and pressure

    }
  }
}
